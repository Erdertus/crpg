using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LauncherV2;
namespace LauncherV2.Gui.LauncherHelper;
internal static class CrpgHashMethods
{
    public static void WriteToConsole(string text)
    {
#if Launcher_Gui
        Form1.Instance?.WriteToConsole(text);
#endif
#if Launcher_Console
        Console.WriteLine(text);
#endif
    }
    public static async Task VerifyGameFiles(string path)
    {
        WriteToConsole($"Verifying Game Files now");
        Stopwatch stopwatch = new Stopwatch(); // Create a Stopwatch instance

        if (Directory.Exists(path))
        {
            stopwatch.Start(); // Start the timing
            var xmlDoc = await GenerateCrpgFolderHashMap(Path.Combine(path, "Modules/cRPG"));
            stopwatch.Stop(); // Stop the timing
            xmlDoc.Save("crpgXmlHash.xml");
            WriteToConsole($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
        }
        else
        {
            WriteToConsole("Please specify the bannerlord folder location before");
        }
    }

    public static async Task<XmlDocument> GenerateCrpgFolderHashMap(string path)
    {
        XmlDocument document = new XmlDocument();
        var root = document.CreateElement("CrpgHashMap");
        document.AppendChild(root);
        if (!Directory.Exists(path))
        {
            WriteToConsole($"cRPG is not installed at {path}");
            return document;
        }

        
        string[] folders = Directory.GetDirectories(path);
        string[] topFiles = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
        ulong restHash = 0;
        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            if (folderName == "AssetPackages")
            {
                await GenerateCrpgAssetsHashMap(folder, document);
            }
            else if (folderName == "SceneObj")
            {
                await GenerateCrpgSceneObjHashMap(folder, document);
            }
            else
            {
                restHash = await HashFolder(folder) ^ restHash;
            }
        }

        foreach (string file in topFiles)
        {
            {
                restHash = await HashFile(file) ^ restHash;
            }
        }

        var restNode = document.CreateElement("Rest");
        restNode.SetAttribute("Name", "Rest");
        restNode.SetAttribute("Hash", restHash.ToString());

        document.DocumentElement!.AppendChild(restNode);
        return document;
    }

    public static async Task GenerateCrpgSceneObjHashMap(string sceneObjPath, XmlDocument doc)
    {
        var mapsNode = doc.CreateElement("Maps");
        string[] mapFolders = Directory.GetDirectories(sceneObjPath);

        var mapHashTasks = mapFolders.Select(map =>
            HashFolder(map).ContinueWith(t =>
            {
                var mapElement = doc.CreateElement("Map");
                mapElement.SetAttribute("Name", Path.GetFileName(map));
                mapElement.SetAttribute("Hash", t.Result.ToString());
                return mapElement;
            })
        );

        var mapElements = await Task.WhenAll(mapHashTasks);
        foreach (var mapElement in mapElements)
        {
            mapsNode.AppendChild(mapElement);
        }

        doc.DocumentElement!.AppendChild(mapsNode);
    }

    public static async Task GenerateCrpgAssetsHashMap(string assetsPath, XmlDocument doc)
    {
        var assetsNode = doc.CreateElement("Assets");
        string[] assetFiles = Directory.GetFiles(assetsPath);

        var assetHashTasks = assetFiles.Select(file =>
            HashFile(file).ContinueWith(t =>
            {
                var assetElement = doc.CreateElement("Asset");
                assetElement.SetAttribute("Name", Path.GetFileName(file));
                assetElement.SetAttribute("Hash", t.Result.ToString());
                return assetElement;
            })
        );

        var assetElements = await Task.WhenAll(assetHashTasks);
        foreach (var assetElement in assetElements)
        {
            assetsNode.AppendChild(assetElement);
        }

        doc.DocumentElement!.AppendChild(assetsNode);
    }

    public static async Task<ulong> HashFolder(string folderPath)
    {
        string[] allFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        List<Task<ulong>> hashTasks = new List<Task<ulong>>();
        foreach (string file in allFiles)
        {
            hashTasks.Add(HashFile(file));
        }

        ulong[] hashResults = await Task.WhenAll(hashTasks);
        ulong hash = 0;
        foreach (ulong fileHash in hashResults)
        {
            hash ^= fileHash;
        }

        return hash;
    }

    public static async Task<ulong> HashFile(string filePath)
    {
        ulong fileHash;
        WriteToConsole($"Hashing {Path.GetFileName(filePath)}");
        using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, useAsync: true))
        {
            fileHash = await XXHash.xxHash64.ComputeHashAsync(stream);
        }

        return fileHash;
    }
}
