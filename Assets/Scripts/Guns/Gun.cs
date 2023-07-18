using UnityEngine;

namespace Gun
{
    public class Gun : MonoBehaviour
    {
        /// <summary>
        /// Enum setups for further use in variables
        /// </summary>

        public enum FireType
        {
            FullAuto,
            SemiAuto,
            Burst,
            Count
        }

        GunModule[] aC_moduleArray = new GunModule[3];

        public void Fire()
        {

        }

        public void UpdateGunStats()
        {
            for (int i = 0; i < aC_moduleArray.Length; i++)
            {
                switch (aC_moduleArray[i].e_moduleType)
                {
                    case GunModule.ModuleSection.Trigger:
                        return;
                    case GunModule.ModuleSection.Clip:
                        return;
                    case GunModule.ModuleSection.Barrel:
                        return;
                }
            }
        }

        public void ResetToBaseStats()
        {

        }
    }
}
