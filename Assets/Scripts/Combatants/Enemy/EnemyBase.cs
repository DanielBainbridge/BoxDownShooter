using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Gun;
using static Gun.GunModule;
using System.Runtime.CompilerServices;

namespace Enemy
{

    public class EnemyBase : Combatant
    {
        
        [Rename("Lock Enemy Position")] public bool b_lockEnemyPosition;
        [Rename("Enemy Fire Testing")] public bool b_testFiring;


        //Kyles Enemy Variables
        private GameObject player;
        public GameObject bullet;
        public Transform barrel;
        public float reloadTime;
        public float force;
        [SerializeField] private bool canShoot;



        private void Start()
        {
            base.Start();
            //Kyles
            player = GameObject.Find("TestCharacter");
            canShoot = false;
        }

        private void Update()
        {
            base.Update();

            //Kyles Enemy Code
            gameObject.transform.LookAt(player.transform.position);
            if (canShoot) Shoot();
        }


        protected override void Move()
        {
            base.Move();
        }        

        //Kyle's Basic Enemy Look at player and Shoot
        private void Shoot()
        {
            //if (canShoot)
            // {
            canShoot = false;
            var clone = Instantiate(bullet, barrel.position, Quaternion.identity);
            clone.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
            //Invoke("Reload", reloadTime);
            //}   
        }

        private void Reload()
        {
            canShoot = true;
        }

    }
}