using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Crpg.Launcher;
using LauncherV2;
using Microsoft.WindowsAPICodePack.Dialogs;
using static System.Net.Mime.MediaTypeNames;
using static Crpg.Launcher.GameInstallationFolderResolver;
namespace LaucherV2;

public partial class Form1 : Form
{
    public static Form1? Instance { get; private set; }
    private GameInstallationInfo? gameLocation;
    private StringBuilder _updateText = new StringBuilder();
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
        isLoading = true;
        comboBox1.DataSource = Enum.GetValues(typeof(Platform))
                  .Cast<Platform>()
                  .ToList();
        comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        if (gameLocation != null)
        {
            platform = gameLocation.platform;
        }

        bool configFound = Config.ReadConfig();
        if (configFound && Config.platform != Platform.Epic)
        {
            platform = Config.platform;
                gameLocation = CreateGameInstallationInfo(Config.gameLocation, platform);
            HandleGameLocationChange();
            if (!HashExist())
            {
                WriteToConsole("Please Verifiy Your Game File Before Starting The Game");
            }
            comboBox1.SelectedItem = platform;
            isLoading = false;
        }
        else
        {
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                comboBox1.SelectedItem = platform;
                isLoading = false;
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
            if (_updateText.Length > 0)
            {
                ConsoleTextBox.AppendText(_updateText.ToString());
                ConsoleTextBox.ScrollToCaret();
                _updateText.Clear();
            }
        }
    }
    public void WriteToConsole(string text)
    {
        // Locking is not strictly necessary if all updates come from other threads,
        // but it's good practice if there's any chance of concurrent access.
        lock (_updateText)
        {
            _updateText.AppendLine(text);
        }
        // No need to call FlushTextToTextBox here since the timer will handle it.
    }

    private void textBox2_TextChanged(object sender, EventArgs e)
    {

    }

    private async void VerifyGameFilesButton_Click_1(object sender, EventArgs e)
    {
        verifyGameFilesButton.Enabled = false;
        if (gameLocation == null)
        {
            WriteToConsole("Game Location is not properly set");
            return;
        }

        await CrpgHashMethods.VerifyGameFiles(gameLocation.InstallationPath);

        verifyGameFilesButton.Enabled = true;

        // To do : enable after Update
        startCrpgButton.Enabled = true;
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {

    }

    private void platformComboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Assuming your ComboBox is bound to the enum values
        if (comboBox1.SelectedItem is Platform selectedPlatform)
        {
            // Update your enum variable with the selected value
            platform = selectedPlatform;

            // Optionally, do something with the newly selected enum value
            HandlePlatformChange(platform);
        }
    }

    private void HandlePlatformChange(Platform platform)
    {
        if(isLoading)
        {
            return;
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
        WriteToConsole("Not Implemented Yet");
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
                    startCrpgButton.Enabled = false;
                }
                else
                {
                    WriteToConsole("Discovering Potential cRPG Installation");
                    verifyGameFilesButton.Enabled = false;

                    await CrpgHashMethods.VerifyGameFiles(gameLocation.InstallationPath);

                    verifyGameFilesButton.Enabled = true;
                }
            }
        }

    }
}
