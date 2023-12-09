using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crpg.Launcher;
using LaucherV2;
using LauncherV2;

namespace LauncherV2;
internal class Config
{
    public static string gameLocation = string.Empty;
    public static GameInstallationFolderResolver.Platform platform;

    public static bool ReadConfig()
    {
        if (!File.Exists("config.ini"))
        {
            Form1.Instance!.WriteToConsole("no config file found");
            return false;
        }

        var lines = File.ReadAllLines("config.ini");

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith(";"))
            {
                var keyValue = line.Split(new char[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();
                    if(key == "Platform")
                    {
                        if (Enum.TryParse(value, out GameInstallationFolderResolver.Platform platformInConfig))
                        {
                            platform = (GameInstallationFolderResolver.Platform)platformInConfig;
                            Form1.Instance!.WriteToConsole("parsing platform");
                        }
                    }

                    if (key == "GameLocation")
                    {
                        gameLocation = value;
                    }
                }
            }
        }

        return true;
    }

    public static void WriteConfig()
    {
        List<string> lines = new()
        {
            $"GameLocation = {gameLocation}",
            $"Platform = {platform.ToString()}",
        };
        File.WriteAllLines("config.ini", lines);
    }

}
