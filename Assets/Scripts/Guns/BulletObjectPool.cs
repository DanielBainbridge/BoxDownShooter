using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gun
{

    public class BulletObjectPool : MonoBehaviour
    {
        List<Bullet> lC_allBullets = new List<Bullet>();
        List<Bullet> lC_freeBullets = new List<Bullet>();
        List<Bullet> lC_inUseBullets = new List<Bullet>();
        int i_totalBullets;


        public void CreatePool(Gun gun)
        {
            int shotCount = gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount == 0 ? 1 : gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount;
            int bulletAmount = gun.aC_moduleArray[1].i_clipSize * shotCount * (int)gun.aC_moduleArray[0].f_fireRate;
            i_totalBullets = (int)(bulletAmount);


            for(int i = 0; i < i_totalBullets; i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.parent = transform;
                obj.name = $"Bullet: {i + 1}";
                obj.layer = 6;
                Bullet bulletRef = obj.AddComponent<Bullet>();
                bulletRef.C_poolOwner = this;
                lC_freeBullets.Add(bulletRef);
                lC_allBullets.Add(bulletRef);
                obj.SetActive(false);
            }
        }

        public void ResizePool(Gun gun)
        {
            int shotCount = gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount == 0 ? 1 : gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount;
            int bulletAmount = gun.aC_moduleArray[1].i_clipSize * shotCount * (int)gun.aC_moduleArray[0].f_fireRate;

            int countDifference = bulletAmount - i_totalBullets;

            if(countDifference == 0)
            {
                return;
            }
            if(countDifference < 0)
            {
                for(int i = 0; i < -countDifference; i++)
                {
                    Bullet bulletToRemove = lC_allBullets[lC_allBullets.Count - 1];
                    lC_allBullets.Remove(bulletToRemove);
                    if (bulletToRemove.gameObject.activeInHierarchy)
                    {
                        lC_inUseBullets.Remove(bulletToRemove);
                    }
                    else
                    {
                        lC_freeBullets.Remove(bulletToRemove);
                    }
                }
            }
            else
            {
                for (int i = 0; i < countDifference; i++)
                {
                    GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    obj.transform.parent = transform;
                    obj.name = $"Bullet: {i_totalBullets + i + 1}";
                    obj.layer = 6;
                    Bullet bulletRef = obj.AddComponent<Bullet>();
                    bulletRef.C_poolOwner = this;
                    lC_allBullets.Add(bulletRef);
                    lC_freeBullets.Add(bulletRef);
                    obj.SetActive(false);
                }
            }

            i_totalBullets = lC_allBullets.Count;
        }
        public Bullet GetFirstOpen()
        {
            return lC_freeBullets[0];
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
