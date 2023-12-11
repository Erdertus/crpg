using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LauncherV2.Gui.LauncherHelper;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.Mime.MediaTypeNames;
using static LauncherV2.Gui.LauncherHelper.GameInstallationFolderResolver;
using Application = System.Windows.Forms.Application;
using ICSharpCode.SharpZipLib.Tar;
using System.Drawing.Design;
namespace LauncherV2;

public partial class Form1 : Form
{
    public static Form1? Instance { get; private set; }
    private GameInstallationInfo? gameLocation;
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private System.Windows.Forms.Timer? _flushTimer;
    private Platform platform = Platform.Epic;
    private bool isLoading;
    private bool dragging = false;
    private Point dragCursorPoint;
    private Point dragFormPoint;
    public Form1()
    {
        InitializeComponent();
        Instance = this;
        // Initialize the timer

        _flushTimer = new System.Windows.Forms.Timer();
        _flushTimer.Interval = 400; // Set the interval for flushing the text
        _flushTimer.Tick += FlushTimer_Tick!;
        _flushTimer.Start();
        FormBorderStyle = FormBorderStyle.None;
        MouseDown += new MouseEventHandler(Form_MouseDown);
        MouseMove += new MouseEventHandler(Form_MouseMove);
        MouseUp += new MouseEventHandler(Form_MouseUp);
        tableLayoutPanel4.MouseDown += new MouseEventHandler(Form_MouseDown);
        tableLayoutPanel4.MouseMove += new MouseEventHandler(Form_MouseMove);
        tableLayoutPanel4.MouseUp += new MouseEventHandler(Form_MouseUp);
        tableLayoutPanel3.MouseDown += new MouseEventHandler(Form_MouseDown);
        tableLayoutPanel3.MouseMove += new MouseEventHandler(Form_MouseMove);
        tableLayoutPanel3.MouseUp += new MouseEventHandler(Form_MouseUp);
        tableLayoutPanel2.MouseDown += new MouseEventHandler(Form_MouseDown);
        tableLayoutPanel2.MouseMove += new MouseEventHandler(Form_MouseMove);
        tableLayoutPanel2.MouseUp += new MouseEventHandler(Form_MouseUp);
        tableLayoutPanel1.MouseDown += new MouseEventHandler(Form_MouseDown);
        tableLayoutPanel1.MouseMove += new MouseEventHandler(Form_MouseMove);
        tableLayoutPanel1.MouseUp += new MouseEventHandler(Form_MouseUp);
    }

    private void button2_Click(object sender, EventArgs e)
    {
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
    }

    private void ChangeLocationButton_Click(object sender, EventArgs e)
    {
        var folderDialog = new CommonOpenFileDialog();
        folderDialog.IsFolderPicker = true;
        if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            gameLocation = CreateGameInstallationInfo(folderDialog.FileName, platform);
            HandleGameLocationChange();
            if (gameLocation == null)
            {
                locationTextBox.Text = folderDialog.FileName;
            }
        }
    }

    private void StartCrpgButton_Click(object sender, EventArgs e)
    {
        if (gameLocation == null)
        {
            WriteToConsole("Game Location is not set!");
            return;
        }

        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = gameLocation.ProgramWorkingDirectory ?? string.Empty,
            FileName = gameLocation.Program,
            Arguments = gameLocation.ProgramArguments ?? string.Empty,
            UseShellExecute = true,
        });
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        if (!HasWritePermissionOnDir(Application.StartupPath))
        {
            MessageBox.Show(
                "Please extract the launcher first and put it in a folder where you have write permission.",
                "Write Permission Required",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
        }

        isLoading = true;
        SetDarkMode(Config.darkMode);
        darkModecheckBox1.Checked = Config.darkMode;
        platformComboBox1.DataSource = Enum.GetValues(typeof(Platform))
                  .Cast<Platform>()
                  .ToList();
        platformComboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        if (gameLocation != null)
        {
            platform = gameLocation.platform;
        }

        bool configFound = Config.ReadConfig();
        if (configFound)
        {
            platform = Config.platform;
            gameLocation = CreateGameInstallationInfo(Config.gameLocation, platform);
            SetDarkMode(Config.darkMode);
            darkModecheckBox1.Checked = Config.darkMode;
            devModeCheckBox.Checked = Config.devMode;
            HandleGameLocationChange();
            if (!HashExist())
            {
                WriteToConsole("Please Verifiy Your Game File Before Starting The Game");
            }

            platformComboBox1.SelectedItem = platform;
            isLoading = false;
        }
        else
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                isLoading = false;
                HandlePlatformChange(null);
            }
        }
    }

    private async void FlushTimer_Tick(object sender, EventArgs e)
    {
        // Stop the timer to prevent re-entrancy issues
        _flushTimer?.Stop();

        // Thread-safe asynchronous flush
        await FlushTextToTextBoxAsync();

        // Restart the timer
        _flushTimer?.Start();
    }

    private async Task FlushTextToTextBoxAsync()
    {
        if (ConsoleTextBox.InvokeRequired)
        {
            // Invoke the method on the UI thread
            ConsoleTextBox.BeginInvoke(new MethodInvoker(() => FlushTextToTextBoxAsync()));
        }
        else
        {
            while (_messageQueue.TryDequeue(out string? text))
            {
                ConsoleTextBox.AppendText(text + Environment.NewLine);
            }

            ConsoleTextBox.ScrollToCaret();
        }
    }

    public void WriteToConsole(string text)
    {
        _messageQueue.Enqueue(text);
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {
    }

    private async void VerifyGameFilesButton_Click_1(object sender, EventArgs e)
    {
        await Verify();
        Update();
    }

    private async Task Verify()
    {
        EnableAllButton(false);
        if (gameLocation == null)
        {
            WriteToConsole("Game Location is not properly set");
            EnableAllButton(true);
            return;
        }

        WriteToConsole("Verifying Game Files, Launcher May become unresponsive for the next 60 secs");
        await CrpgHashMethods.VerifyGameFiles(gameLocation.InstallationPath);

        verifyGameFilesButton.Enabled = true;

        // To do : enable after Update
        EnableAllButton(true);
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
    }

    private void platformComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Assuming your ComboBox is bound to the enum values
        if (platformComboBox1.SelectedItem is Platform selectedPlatform)
        {
            // Update your enum variable with the selected value
            platform = selectedPlatform;

            // Optionally, do something with the newly selected enum value
            HandlePlatformChange(platform);
        }
    }

    private void HandlePlatformChange(Platform? platform)
    {
        if (isLoading)
        {
            return;
        }

        if (platform == null)
        {
            gameLocation = ResolveBannerlordInstallation();
            if (gameLocation != null)
            {
                platform = gameLocation.platform;
                platformComboBox1.SelectedItem = platform;
                return;
            }

            locationTextBox.Text = gameLocation?.InstallationPath ?? string.Empty;
        }

        WriteToConsole("Trying to Auto Resolve Bannerlord Location");
        if (platform == Platform.Epic)
        {
            changeLocationButton.Enabled = false;
            gameLocation = ResolveBannerlordEpicGamesInstallation();
            locationTextBox.Text = gameLocation?.InstallationPath ?? string.Empty;
            HandleGameLocationChange();
        }

        if (platform == Platform.Steam)
        {
            changeLocationButton.Enabled = true;
            gameLocation = ResolveBannerlordSteamInstallation();
            locationTextBox.Text = gameLocation?.InstallationPath ?? string.Empty;
            HandleGameLocationChange();
        }

        if (platform == Platform.Xbox)
        {
            changeLocationButton.Enabled = true;
            gameLocation = ResolveBannerlordEpicGamesInstallation();
            locationTextBox.Text = gameLocation?.InstallationPath ?? string.Empty;
            HandleGameLocationChange();
        }
    }

    private async void UpdateOrInstallButton_Click(object sender, EventArgs e)
    {
        Update();
    }
    private async void Update()
    {
        EnableAllButton(false);
        if (!HashExist())
        {
            await Verify();
        }

        XmlDocument doc = new XmlDocument();
        try
        {
            string url = "http://namidaka.fr/hash.xml"; // Replace with your XML URL
            using (WebClient client = new WebClient())
            {
                string xmlContent = client.DownloadString(url);
                doc.LoadXml(xmlContent);
            }
        }
        catch (Exception ex)
        {
            WriteToConsole($"Error: {ex.Message}");
            EnableAllButton(true);
            return;
        }


        if (doc?.DocumentElement == null)
        {
            EnableAllButton(true);
            return;
        }

        Dictionary<string, string> distantAssets = new Dictionary<string, string>();
        Dictionary<string, string> distantMaps = new Dictionary<string, string>();
        string distantRestHash = ReadHash(doc, distantAssets, distantMaps);
        XmlDocument doc2 = new XmlDocument();
        try
        {
            doc2.Load("crpgXmlHash.xml");
        }
        catch (Exception ex)
        {
            WriteToConsole(ex.Message);
            EnableAllButton(true);
            return;
        }

        Dictionary<string, string> localAssets = new Dictionary<string, string>();
        Dictionary<string, string> localMaps = new Dictionary<string, string>();

        string localRestHash = ReadHash(doc2, localAssets, localMaps);
        bool downloadRest = localRestHash != distantRestHash;

        var assetsToDownload = distantAssets.Where(a => !localAssets.Contains(a)).ToList();
        var assetsToDelete = localAssets.Where(a => !distantAssets.Contains(a)).ToList();
        if (Config.devMode)
        {
            assetsToDelete = localAssets.Where(a => distantAssets.ContainsKey(a.Key) && !distantAssets.ContainsValue(a.Value)).ToList();
        }

        var mapsToDelete = localMaps.Where(a => !distantMaps.Contains(a)).ToList();
        if (Config.devMode)
        {
            mapsToDelete = localMaps.Where(a => distantMaps.ContainsKey(a.Key) && !distantMaps.ContainsValue(a.Value)).ToList();
        }

        var mapsToDownload = distantMaps.Where(a => !localMaps.Contains(a)).ToList();

        if (assetsToDelete.Count == 0 && assetsToDownload.Count == 0 && mapsToDownload.Count == 0 && mapsToDelete.Count == 0 && !downloadRest)
        {
            WriteToConsole("Your game is Up To Date");
            EnableAllButton(true);
            return;
        }

        if (gameLocation == null)
        {
            WriteToConsole("Cannot Download update as Bannerlord Location is not known");
            EnableAllButton(true);
            return;
        }

        foreach (var assetToDelete in assetsToDelete)
        {
            string pathToDelete = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/AssetPackages/", assetToDelete.Key);
            WriteToConsole(pathToDelete);
            try
            {
                File.Delete(pathToDelete);
            }
            catch
            {
            }
        }

        foreach (var mapToDelete in mapsToDelete)
        {
            string pathToDelete = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/SceneObj/", mapToDelete.Key);
            WriteToConsole($"deleting {pathToDelete}");
            try
            {
                Directory.Delete(pathToDelete, recursive: true);
            }
            catch
            {
            }
        }
        string cRPGFolder = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/");
        if (Config.devMode)
        {
            WriteToConsole("You're in Dev Mode. Only Assets and Maps will update. Other files remain untouched");
            WriteToConsole("If you want to update the other files, Uncheck Dev Mode , then put back your files");
        }
        else
        {
            if (downloadRest && Directory.Exists(cRPGFolder))
            {
                foreach (var dir in Directory.GetDirectories(cRPGFolder))
                {
                    if (Path.GetFileName(dir) == "SceneObj" || Path.GetFileName(dir) == "AssetPackages")
                    { continue; }
                    else
                    {
                        WriteToConsole($"deleting {dir}");
                        Directory.Delete(dir, recursive: true);
                    }
                }
                foreach (var file in Directory.GetFiles(cRPGFolder))
                {
                    WriteToConsole($"deleting {file}");
                    File.Delete(file);
                }

                try
                {
                    string subModulePath = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/SubModule.xml");
                    if (File.Exists(subModulePath))
                    {
                        WriteToConsole($"deleting {subModulePath}");
                        File.Delete(subModulePath);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        List<Task> allTasks = new List<Task>();
        bool updateSuccessful = true;
        using (var client = new HttpClient())
        {
            foreach (var assetToDownload in assetsToDownload)
            {
                try
                {
                    // Download the file
                    WriteToConsole($"Downloading and extracting {assetToDownload.Key + ".tar.gz"} ");
                    var response = await client.GetAsync("https://namidaka.fr/AssetPackages/" + assetToDownload.Key + ".tar.gz");
                    response.EnsureSuccessStatusCode();
                    var contentType = response.Content!.Headers!.ContentType!.MediaType;
                    if (contentType == "text/html")
                    {
                        throw new Exception("Expected file, but received HTML content. The file may not exist at the specified URL.");
                    }

                    var extractionTask1 = Task.Run(() => Extract(response, Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/AssetPackages/")));
                    allTasks.Add(extractionTask1);
                }
                catch (Exception ex)
                {
                    updateSuccessful = false;
                    WriteToConsole(ex.Message);
                }
            }

            foreach (var mapToDownload in mapsToDownload)
            {
                try
                {
                    // Download the file
                    WriteToConsole($"Downloading and extracting {mapToDownload.Key + ".tar.gz"} ");
                    var response = await client.GetAsync("https://namidaka.fr/SceneObj/" + mapToDownload.Key + ".tar.gz");
                    response.EnsureSuccessStatusCode();
                    var contentType = response.Content!.Headers!.ContentType!.MediaType;
                    if (contentType == "text/html")
                    {
                        throw new Exception("Expected file, but received HTML content. The file may not exist at the specified URL.");
                    }
                    var extractionTask2 = Task.Run(() => Extract(response, Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/SceneObj/")));
                    allTasks.Add(extractionTask2);
                }
                catch (Exception ex)
                {
                    updateSuccessful = false;
                    WriteToConsole(ex.Message);
                }
            }

            if (downloadRest)
            {
                try
                {
                    // Download the file
                    WriteToConsole($"Downloading and extracting the xmls files : rest.tar.gz");
                    var response = await client.GetAsync("https://namidaka.fr/rest.tar.gz");
                    response.EnsureSuccessStatusCode();
                    var contentType = response.Content!.Headers!.ContentType!.MediaType;
                    if (contentType == "text/html")
                    {
                        throw new Exception("Expected file, but received HTML content. The file may not exist at the specified URL.");
                    }

                    var extractionTask3 = Task.Run(() => Extract(response, Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/")));
                    allTasks.Add(extractionTask3);
                }

                catch (Exception ex)
                {
                    updateSuccessful = false;
                    WriteToConsole(ex.Message);
                }
            }
        }

        await Task.WhenAll(allTasks);
        if (updateSuccessful)
        {
            doc.Save("crpgXmlHash.xml");
            WriteToConsole("Update Finished");
        }
        else
        {
            WriteToConsole("There were issues during the update");
            WriteToConsole("It is possible that we are currently updating cRPG");
            WriteToConsole("If problem persist in an hour, please contact and moderator on discord");
        }

        UpdateOrInstallButton.Text = "Check For Update";
        EnableAllButton(true);
    }

    private bool HashExist()
    {
        return File.Exists("crpgXmlHash.xml");
    }
    private async void HandleGameLocationChange()
    {
        if (gameLocation == null)
        {
            WriteToConsole("Bannerlord was not found in the location");
            startCrpgButton.Enabled = false;
            verifyGameFilesButton.Enabled = false;
            UpdateOrInstallButton.Enabled = false;
            Config.gameLocation = string.Empty;
            Config.platform = platform;
            Config.WriteConfig();
        }
        else
        {
            WriteToConsole("Bannerlord was detected in the location");
            startCrpgButton.Enabled = true;
            verifyGameFilesButton.Enabled = true;
            UpdateOrInstallButton.Enabled = true;
            Config.gameLocation = gameLocation.InstallationPath;
            locationTextBox.Text = gameLocation.InstallationPath;
            Config.platform = platform;
            Config.WriteConfig();
            if (!HashExist())
            {
                if (!Directory.Exists(Path.Combine(gameLocation.InstallationPath, "Modules/cRPG")))
                {
                    WriteToConsole("cRPG is not Installed, Click on Install Mod to Install");
                    UpdateOrInstallButton.Text = "Install Mod";
                    startCrpgButton.Enabled = false;
                }
                else
                {
                    WriteToConsole("Discovering Potential cRPG Installation");
                    EnableAllButton(false);
                    await CrpgHashMethods.VerifyGameFiles(gameLocation.InstallationPath);
                    EnableAllButton(true);
                }
            }
        }
    }

    void EnableAllButton(bool enabled)
    {
        verifyGameFilesButton.Enabled = enabled;
        startCrpgButton.Enabled = enabled;
        changeLocationButton.Enabled = enabled;
        UpdateOrInstallButton.Enabled = enabled;
        platformComboBox1.Enabled = enabled;
    }

    private bool HasWritePermissionOnDir(string path)
    {
        try
        {
            List<string> lines = new()
        {
            $"GameLocation = {gameLocation}",
            $"Platform = {platform.ToString()}",
        };
            File.WriteAllLines("test.ini", lines);
            File.Delete("test.ini");
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // If an unauthorized access exception occurred, we don't have write permissions
            return false;
        }
        catch (IOException)
        {
            // Handle other IO exceptions if necessary
            return false;
        }
    }

    private string ReadHash(XmlDocument doc, Dictionary<string, string> assets, Dictionary<string, string> maps)
    {
        foreach (var node in doc!.DocumentElement!.ChildNodes.Cast<XmlNode>().ToArray())
        {
            if (node.Name == "Assets")
            {
                foreach (var node1 in node.ChildNodes.Cast<XmlNode>().ToArray())
                {
                    assets[node1!.Attributes!["Name"]!.Value] = node1!.Attributes!["Hash"]!.Value;
                }
            }

            if (node.Name == "Maps")
            {
                foreach (var node1 in node.ChildNodes.Cast<XmlNode>().ToArray())
                {
                    maps[node1!.Attributes!["Name"]!.Value] = node1!.Attributes!["Hash"]!.Value;
                }
            }

            if (node.Name == "Rest")
            {
                return node!.Attributes!["Hash"]!.Value;
            }
        }

        return string.Empty;
    }

    private async void Extract(HttpResponseMessage response, string path)
    {
        using (var stream = await response.Content.ReadAsStreamAsync())
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var tarArchive = TarArchive.CreateInputTarArchive(gzipStream))
                {
                    tarArchive.ExtractContents(path);
                }
            }
        }
    }
    private void SetDarkMode(bool darkModeEnabled)
    {
        Color backgroundColor = darkModeEnabled ? Color.FromArgb(45, 45, 48) : SystemColors.Window;
        Color foregroundColor = darkModeEnabled ? Color.White : SystemColors.WindowText;
        //Color controlBackgroundColor = darkModeEnabled ? Color.FromArgb(30, 30, 30) : SystemColors.Control;

        BackColor = darkModeEnabled ? Color.FromArgb(30, 30, 32) : SystemColors.Window;
        ForeColor = foregroundColor;
        ConsoleTextBox.BackColor = backgroundColor;
        ConsoleTextBox.ForeColor = foregroundColor;
        locationTextBox.BackColor = backgroundColor;
        locationTextBox.ForeColor = foregroundColor;

        foreach (Control ctrl in this.Controls)
        {
            //ctrl.BackColor = controlBackgroundColor;
            //ctrl.ForeColor = foregroundColor;

            // Add cases for other control types as needed
        }

        // Update MenuStrip, ToolStrip, DataGridView, etc., as needed
    }

    private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
    {
    }
    void Form_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }
    }

    void Form_MouseMove(object? sender, MouseEventArgs e)
    {
        if (dragging)
        {
            Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
            this.Location = Point.Add(dragFormPoint, new Size(dif));
        }
    }

    void Form_MouseUp(object? sender, MouseEventArgs e)
    {
        dragging = false;
    }

    private void checkBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (isLoading)
        {
            return;
        }
        if (darkModecheckBox1.Checked)
        {
            SetDarkMode(true);
            Config.darkMode = true;
            Config.WriteConfig();
        }
        else
        {
            SetDarkMode(false);
            Config.darkMode = false;
            Config.WriteConfig();
        }
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e)
    {
        if (isLoading)
        {
            return;
        }

        if (devModeCheckBox.Checked)
        {
            Config.devMode = true;
            Config.WriteConfig();
        }
        else
        {
            Config.devMode = false;
            Config.WriteConfig();
        }
    }
    private void buttonEnableStatusChange(object sender, EventArgs e)
    {
        foreach (Control ctrl in Controls)
        {
            Color backgroundColor = Config.darkMode ? Color.FromArgb(45, 45, 48) : SystemColors.Window;
            Color foregroundColor = Config.darkMode ? Color.White : SystemColors.WindowText;
            if (ctrl is Button button)
            {
                if (button.Enabled == false)
                {
                    ForeColor = Color.Red;
                    BackColor = Color.Red;
                }
            }
            //ctrl.BackColor = controlBackgroundColor;
            //ctrl.ForeColor = foregroundColor;

            // Add cases for other control types as needed
        }
    }

    private void close_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void minimize_Click(object sender, EventArgs e)
    {
        this.WindowState = FormWindowState.Minimized;
    }
}
