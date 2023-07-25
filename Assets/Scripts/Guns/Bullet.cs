using UnityEngine;
using static Gun.GunModule;

namespace Gun
{
    public class Bullet : MonoBehaviour
    {
        public struct BulletBaseInfo
        {
            [HideInInspector] public bool b_playerOwned;
            [HideInInspector] public Vector3 S_firingOrigin;
            [HideInInspector] public Vector3 S_firingDirection;
            [HideInInspector] public float f_range;
            [HideInInspector] public float f_damage;
            [HideInInspector] public float f_speed;

            public BulletBaseInfo(bool playerOwned, Vector3 origin, Vector3 direction, float range, float damage, float speed)
            {
                b_playerOwned = playerOwned;
                S_firingOrigin = origin;
                S_firingDirection = direction;
                f_range = range;
                f_damage = damage;
                f_speed = speed;
            }
        }

        [HideInInspector] public GameObject C_prefab;
        [HideInInspector] public BulletObjectPool C_poolOwner;
        [HideInInspector] private BulletBaseInfo S_baseInformation;

        GunModule.BulletEffect e_bulletEffect;
        GunModule.BulletTrait e_bulletTrait;



        void Update()
        {
            //check that it hasn't gone past its range, set inactive
            if (Vector3.Distance(S_baseInformation.S_firingOrigin, transform.position) > S_baseInformation.f_range)
            {
                C_poolOwner.MoveToOpen(this);
            }
            // move in direction by speed, 



            //TO DO, update differently based on bullet type
            transform.position += transform.forward * S_baseInformation.f_speed * Time.deltaTime;

        }

        public void FireBullet(Vector3 originOffset, Vector3 directionOffset, BulletBaseInfo bulletInfo, GunModule.BulletTraitInfo bulletTrait, GunModule.BulletEffectInfo bulletEffect)
        {
            //bullet effect colour/ particles bullet trait mesh **stubbed**
            //UpdateBulletGraphics()

            //move bullet to closed list
            C_poolOwner.MoveToClosed(this);

            S_baseInformation = bulletInfo;


            Debug.Log($"Firing Offset: {originOffset}");
            Debug.Log($"Bullet Info Origin: {bulletInfo.S_firingOrigin}");

            transform.position = bulletInfo.S_firingOrigin + originOffset;
            transform.rotation = Quaternion.Euler(new Vector3(0,Mathf.Atan2(-bulletInfo.S_firingDirection.z, bulletInfo.S_firingDirection.x) * Mathf.Rad2Deg + 90 ,0) + directionOffset);

            e_bulletEffect = bulletEffect.e_bulletEffects;
            e_bulletTrait = bulletTrait.e_bulletTrait;
        }

        public void BulletChangeDirection(Vector3 direction)
        {
            S_baseInformation.S_firingDirection = direction;
        }
        public void BulletChangeOrigin(Vector3 origin)
        {
            S_baseInformation.S_firingOrigin = origin;
        }

        public void UpdateBulletGraphics(Vector4 Colour, GameObject meshPrefab, GameObject particlePrefab)
        {

        }

        public void OnHit()
        {

        }
    }
}
