using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LauncherV2;

namespace LauncherV2.Gui.LauncherHelper;
internal class Config
{
    public static string gameLocation = string.Empty;
    public static GameInstallationFolderResolver.Platform platform;

    public static bool ReadConfig()
    {
        if (!File.Exists("config.ini"))
        {
            CrpgHashMethods.WriteToConsole("no config file found");
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
                    if (key == "Platform")
                    {
                        if (Enum.TryParse(value, out GameInstallationFolderResolver.Platform platformInConfig))
                        {
                            platform = platformInConfig;
                            CrpgHashMethods.WriteToConsole("parsing platform");
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
