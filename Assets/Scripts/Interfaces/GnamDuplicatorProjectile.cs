using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamDuplicatorProjectile : GnamProjectile
    {
        public override void OnCollisionEvent(Collision collision)
        {
            var other = collision.collider.gameObject;
            var food = other.gameObject.GetComponent<Food>();
            if (food == null) {
                food = other.gameObject.GetComponentInParent<Food>();
            }


            if (food != null)
            {
                //    Debug.Log($"PROJECTILE {other.name}");
                //Debug.Log($"PROJECTILE {other.name} - {food.name}");

                var clone = Instantiate(food.gameObject, food.transform.position, food.transform.rotation) as GameObject;
                Destroy(clone.GetComponent<SnapZoneOffset>());
                clone.transform.parent = null;
                clone.GetComponent<Rigidbody>().isKinematic = false;
                var grabbabble = clone.GetComponent<GnamGrabbable>();
                grabbabble.ResetScale();
                grabbabble.ResetGrabbing(true);
               
                grabbabble.enabled = true;
                base.OnCollisionEvent(collision);
            }
        }
    }
}
