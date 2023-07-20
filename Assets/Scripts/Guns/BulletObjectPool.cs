using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gun
{

    public class BulletObjectPool : MonoBehaviour
    {
        List<Bullet> lC_freeBullets;
        List<Bullet> lC_inUseBullets;

        void CreatePool()
        {
            foreach (Gun gun in FindObjectsOfType<Gun>())
            {
                // for each gun get clip size, add bullets per 
            }
        }

        void ResizePool()
        {
            //check difference between current pool size and new Gun clip size count
            //add/subtract the difference from object pool
        }
        public Bullet GetFirstOpen()
        {
            return null;
        }
        public void MoveToOpen(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            lC_freeBullets.Add(bullet);
            lC_inUseBullets.Remove(bullet);
        }
        public void MoveToClosed(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
            lC_inUseBullets.Add(bullet);
            lC_freeBullets.Remove(bullet);
        }
    }
}
