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
            Debug.Log(clip.GnamProjectile.name);

            var autodestroyer = clip.GetComponent<Autodestroy>();
            if (autodestroyer != null)
            {
                Debug.Log("remove autodestroyer from clip");
                Destroy(autodestroyer);
            }
        }
    }
}
