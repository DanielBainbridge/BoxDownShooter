using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Gun
{
    public class Bullet : MonoBehaviour
    {
        
        [HideInInspector] public GameObject C_prefab;
        [HideInInspector] public BulletObjectPool C_poolOwner;


        [HideInInspector] public bool b_playerOwned;
        [HideInInspector] public Vector3 S_firingOrigin;
        [HideInInspector] public Vector3 S_firingDirection;
        [HideInInspector] public float f_range;
        [HideInInspector] public float f_damage;
        [HideInInspector] public float f_speed;



        void Update()
        {
            //check that it hasn't gone past its range, set inactive
            if (Vector3.Distance(S_firingOrigin, transform.position) > f_range)
            {
                C_poolOwner.MoveToClosed(this);
            }
            // move in direction by speed, 

            transform.position += S_firingDirection * f_speed * Time.deltaTime;

        }
        void FireBullet(GameObject prefab, Vector3 startPosition, Vector3 firingDirection)
        {

        }
    }
}
