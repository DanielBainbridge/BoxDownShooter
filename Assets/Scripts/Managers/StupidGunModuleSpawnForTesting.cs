using UnityEngine;
using Gun;

public class StupidGunModuleSpawnForTesting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GunModuleSpawner.SpawnGunModule("ShotGunBarrel", new Vector3(2,0,3));
    }
}
