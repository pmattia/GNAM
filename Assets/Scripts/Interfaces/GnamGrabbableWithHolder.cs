using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamGrabbableWithHolder: GnamGrabbable
    {
        public Eatable holder;
        public Food holdedFood;
        //public List<Transform> holderGrabPoints;
        private List<Transform> grabpoints = new List<Transform>();

        private void Start()
        {
            grabpoints.AddRange(GrabPoints);
            holder.grabPoints.ForEach(p => grabpoints.Remove(p.transform));

            GrabPoints.Clear();
            GrabPoints.AddRange(holder.grabPoints.Select(g => g.transform));

            holder.IsEatable = false;
            holder.eatTime = 3;

            holdedFood.onEated += (eater) => {
                holder.IsEatable = true;
            };
        }

        private void OnCollisionEnter(Collision collision)
        {
            CheckProjectile(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckProjectile(other.gameObject);
        }

        private void CheckProjectile(GameObject projectile)
        {
            if (projectile.GetComponent<Projectile>() != null)
            {
                if (projectile.GetComponent<GnamModifierProjectile>() == null)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
