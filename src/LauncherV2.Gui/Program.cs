using Gameloop.Vdf;
using Microsoft.Win32;
using static LauncherV2.Gui.LauncherHelper.GameInstallationFolderResolver;
using System.Text.Json;
using System.Web;
using System.Runtime.InteropServices;
using LauncherV2.Gui.LauncherHelper;

namespace LauncherV2;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }

}
