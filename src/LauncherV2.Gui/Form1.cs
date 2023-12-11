using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LauncherV2.Gui.LauncherHelper;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.Mime.MediaTypeNames;
using static LauncherV2.Gui.LauncherHelper.GameInstallationFolderResolver;
using Application = System.Windows.Forms.Application;
namespace LauncherV2;

public partial class Form1 : Form
{
    public static Form1? Instance { get; private set; }
    private GameInstallationInfo? gameLocation;
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private System.Windows.Forms.Timer? _flushTimer;
    private Platform platform = Platform.Epic;
    private bool isLoading;
    public Form1()
    {
        InitializeComponent();
        Instance = this;
        // Initialize the timer
        
        _flushTimer = new System.Windows.Forms.Timer();
        _flushTimer.Interval = 400; // Set the interval for flushing the text
        _flushTimer.Tick += FlushTimer_Tick!;
        _flushTimer.Start();
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

    private void FlushTimer_Tick(object sender, EventArgs e)
    {
        // Thread-safe flush
        FlushTextToTextBox();
    }

    private void FlushTextToTextBox()
    {
        if (ConsoleTextBox.InvokeRequired)
        {
            ConsoleTextBox.Invoke(new Action(FlushTextToTextBox));
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
        EnableAllButton(false);
        if (gameLocation == null)
        {
            WriteToConsole("Game Location is not properly set");
            return;
        }

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
        if(isLoading)
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

    private void UpdateOrInstallButton_Click(object sender, EventArgs e)
    {
        if (!HashExist())
        {
            WriteToConsole("please verify your game files first");
            return;
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
            return;
        }


        if (doc?.DocumentElement == null)
        {
            return;
        }

        Dictionary<string, string> distantAssets = new Dictionary<string,string>();
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
            return;
        }

        Dictionary<string, string> localAssets = new Dictionary<string, string>();
        Dictionary<string, string> localMaps = new Dictionary<string, string>();

        string localRestHash = ReadHash(doc2, localAssets, localMaps);
        bool downloadRest = localRestHash != distantRestHash;

        var assetsToDownload = distantAssets.Where(a => !localAssets.Contains(a)).ToList();
        var assetsToDelete = localAssets.Where(a => !distantAssets.Contains(a)).ToList();
        var mapsToDownload = distantMaps.Where(a => !localMaps.Contains(a)).ToList();
        var mapsToDelete = localMaps.Where(a => !distantMaps.Contains(a)).ToList();

        if (gameLocation == null)
        {
            WriteToConsole("Cannot Download update as Bannerlord Location is not known");
            return;
        }

        foreach (var assetToDelete in assetsToDelete)
        {
            string pathToDelete = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/AssetPackages/", assetToDelete.Key);
            WriteToConsole(pathToDelete);
            File.Delete(pathToDelete);
        }

        foreach (var mapToDelete in mapsToDelete)
        {
            string pathToDelete = Path.Combine(gameLocation.InstallationPath, "Modules/cRPG/SceneObj/", mapToDelete.Key);
            WriteToConsole($"deleting {pathToDelete}");
            Directory.Delete(pathToDelete, recursive: true);
        }

        foreach(var assetToDownload in assetsToDownload)
        {

        }
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
}
