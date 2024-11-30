using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Threading.Tasks;
using System.IO;

namespace D4
{
    public class Default
    {
        private static AddRequest addRequest;
        private static RemoveRequest removeRequest;

        [MenuItem("D4/Init Defaults")]
        private static void Init()
        {
            InitFolders();
            InitPackages();
            AssetDatabase.Refresh();
        }

        private static void InitFolders()
        {
            string path = Application.dataPath;
            int defaultFolderCount = Folder.defaultFolders.Length;

            for (int i = 0; i < defaultFolderCount; i++)
            {
                string newPath = Path.Combine(path, Folder.defaultFolders[i]);
                if (Directory.Exists(newPath)) continue;
                Directory.CreateDirectory(newPath);
            }
        }

        private static async void InitPackages()
        {
            await RemoveDefaultPackages();
            await AddDefaultPackages();
        }

        private static async Task AddDefaultPackages()
        {
            int packageCount = Packages.defaultPackages.Length;

            for (int i = 0; i < packageCount; i++)
            {
                addRequest = Client.Add(Packages.defaultPackages[i]);

                while(addRequest.IsCompleted == false)
                {
                    await Task.Delay(100);
                }

                switch (addRequest.Status)
                {
                    case StatusCode.Success:
                        Debug.Log($"added {Packages.defaultPackages[i]}");
                        break;
                    case StatusCode.Failure:
                        Debug.LogError(addRequest.Error.message);
                        break;
                }
            }
        }

        private static async Task RemoveDefaultPackages()
        {
            string[] packages = Packages.uneededPackages;
            
            for (int i = 0; i < packages.Length; i++)
            {
                removeRequest = Client.Remove(packages[i]);
                while(removeRequest.IsCompleted == false)
                {
                    await Task.Delay(100);
                }

                switch (removeRequest.Status)
                {
                    case StatusCode.Success:
                        Debug.Log($"removed {packages[i]}");
                        break;
                    case StatusCode.Failure:
                        Debug.LogError(removeRequest.Error.message);
                        break;
                }
            }
        }
    }
}
