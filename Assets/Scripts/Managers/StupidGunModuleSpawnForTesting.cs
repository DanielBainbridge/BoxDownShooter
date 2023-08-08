using UnityEngine;
using Gun;

public class StupidGunModuleSpawnForTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int incCol = 0;
        int incRow = 0;

        GunModuleSpawner.SpawnGunModule("Buckshot(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("BuckSpray(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("MultiShot(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Spray(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Straight(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Wave(Barrel)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));

        incRow++;
        incRow++;
        incCol = 0;
        GunModuleSpawner.SpawnGunModule("Largest(Clip)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Large(Clip)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Med(Clip)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Small(Clip)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Smaller(Clip)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));

        incRow++;
        incRow++;
        incCol = 0;
        GunModuleSpawner.SpawnGunModule("Largest Trigger", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Medium(Trigger)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Large(Trigger)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Small(Trigger)", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));
        incCol++;
        incCol++;
        GunModuleSpawner.SpawnGunModule("Smallest Trigger", new Vector3(incCol - 7.5f, 0.15f, incRow - 1.5f));

    }
}
