using System;
using UnityEngine;
using System.IO;


namespace Gun
{
    public static class GunModuleSpawner
    {
        public static void SpawnGunModule(string gunModuleName, Vector3 worldPos)
        {
            GunModule moduleToLoad = Resources.Load<GunModule>(GetGunModuleResourcesPath(gunModuleName));
            if (moduleToLoad == null)
            {
                throw new NullReferenceException("Module attempted to load is null");
            }
            moduleToLoad.Spawn(worldPos);
        }

        public static string GetGunModuleResourcesPath(string gunModuleName)
        {
            // asset is "..\\..\\{gunModuleName}.asset"
            // resources.load requires a direct file path from resources folder
            // resources.load is templated and wants to be overloaded with the type of the asset, it doesn't read the file extension in the string

            string currentDirectory = Directory.GetCurrentDirectory();
            currentDirectory += "\\Assets\\Resources\\GunModules";

            if (gunModuleName.Contains("Base"))
            {
                currentDirectory += "\\Base";
            }
            else if (gunModuleName.Contains("Barrel"))
            {
                currentDirectory += "\\Barrel";
            }
            else if (gunModuleName.Contains("Clip"))
            {
                currentDirectory += "\\Clip";
            }
            else if (gunModuleName.Contains("Trigger"))
            {
                currentDirectory += "\\Trigger";
            }
            else
            {
                throw new NullReferenceException("Name of gun module does not match any folders");
            }


            string[] paths = Directory.GetFiles(currentDirectory);

            string filePath = "IF THIS PRINTS THE FILE PATH IS DODGY";

            for (int q = 0; q < paths.Length; q++)
            {
                if (paths[q].Contains(gunModuleName))
                {
                    filePath = paths[q];
                }
            }

            string[] pathSections = filePath.Split("Resources\\");

            filePath = pathSections[pathSections.Length - 1];
            if (filePath.Contains(".asset.meta"))
            {
                filePath = filePath.Remove(filePath.Length - 11);
            }
            else if (filePath.Contains(".asset"))
            {
                filePath = filePath.Remove(filePath.Length - 6);
            }
            return filePath;
        }
    }
}
