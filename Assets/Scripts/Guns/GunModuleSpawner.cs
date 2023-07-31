using System;
using System.Collections.Generic;
using UnityEngine;


namespace Gun
{
    public static class GunModuleSpawner
    {
        public static void SpawnGunModule(string gunModuleName, Vector3 worldPos)
        {
            //Debug.Log(Resources.Load($"GunModules/Base/{gunModuleName}.asset").name);
            GunModule moduleToLoad = (GunModule)Resources.Load($"\\..\\..\\{gunModuleName}.asset");
            if(moduleToLoad == null)
            {
                throw new NullReferenceException("Module attempted to load is null");
            }
            moduleToLoad.Spawn(worldPos);
        }
    }
}
