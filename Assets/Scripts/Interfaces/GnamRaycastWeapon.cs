using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamRaycastWeapon : RaycastWeapon
    {
        private void Start()
        {
            this.onAttachedAmmoEvent.AddListener(() => SwitchProjectile());
        }
        void SwitchProjectile()
        {
            var clip = GetComponentInChildren<GnamPistolClip>();
            this.ProjectilePrefab = clip.GnamProjectile;

            var autodestroyer = clip.GetComponent<Autodestroy>();
            if (autodestroyer != null)
            {
                Destroy(autodestroyer);
            }
        }

        public override void Shoot()
        {
            Debug.Log($"bullets count {base.GetBulletCount()}");
            var bulletCount = base.GetBulletCount();
            if(bulletCount == 0 && !BulletInChamber)
            {
                base.EjectMagazine();
            }

            base.Shoot();
        }
    }
}
