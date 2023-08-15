using Enemy;
using Gun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<EnemyBase> lC_enemylist = new List<EnemyBase>();
    public PolyBrushManager manager;
    public bool endTriggered = false;

    private void Start()
    {
        manager.ResetGoo();
    }

    private void Update()
    {
        if (!endTriggered)
        {

            foreach (EnemyBase e in lC_enemylist)
            {
                if (!e.b_isDead)
                {
                    return;
                }
            }
            manager.RemoveSpaceGoo();
            endTriggered = true;
            GunModuleSpawner.SpawnGunModule("BuckSpray(Barrel)", lC_enemylist[3].transform.position);
        }
    }

}
