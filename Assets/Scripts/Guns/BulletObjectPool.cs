using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gun
{

    public class BulletObjectPool : MonoBehaviour
    {
        List<Bullet> lC_freeBullets = new List<Bullet>();
        List<Bullet> lC_inUseBullets = new List<Bullet>();

        public void CreatePool(Gun gun)
        {
            int shotCount = gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount == 0 ? 1 : gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount;
            int bulletAmount = gun.aC_moduleArray[1].i_clipSize * shotCount;


            for(int i = 0; i < (int)(bulletAmount * 1.3f); i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.parent = transform;
                obj.name = $"Bullet: {i + 1}";
                obj.layer = 6;
                Bullet bulletRef = obj.AddComponent<Bullet>();
                bulletRef.C_poolOwner = this;
                lC_freeBullets.Add(bulletRef);
                obj.SetActive(false);
            }
        }

        public void ResizePool(Gun gun)
        {
            int shotCount = gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount == 0 ? 1 : gun.aC_moduleArray[2].S_shotPatternInformation.i_shotCount;
            int bulletAmount = gun.aC_moduleArray[1].i_clipSize * shotCount;


            for (int i = 0; i < (int)(bulletAmount * 1.3f); i++)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.parent = transform;
                obj.name = $"Bullet: {i + 1}";
                obj.layer = 6;
                Bullet bulletRef = obj.AddComponent<Bullet>();
                bulletRef.C_poolOwner = this;
                lC_freeBullets.Add(bulletRef);
                obj.SetActive(false);
            }
            //check difference between current pool size and new Gun clip size count
            //add/subtract the difference from object pool
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
