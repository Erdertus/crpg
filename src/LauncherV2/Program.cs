using Gameloop.Vdf;
using Microsoft.Win32;
using static Crpg.Launcher.GameInstallationFolderResolver;
using System.Text.Json;
using System.Web;

namespace LaucherV2;

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
