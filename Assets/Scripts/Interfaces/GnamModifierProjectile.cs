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
    public class GnamModifierProjectile : Projectile
    {
        [SerializeField]
        List<GnamModifier> modifiers = new List<GnamModifier>();
        int modifiersLength = 1;

        public List<GnamModifier> GetRandomModifiers()
        {
            var ret = new List<GnamModifier>();
            var tModifiers = modifiers.ToList();
            for(int i =0; i< modifiersLength; i++)
            {
                var randModifier = tModifiers[UnityEngine.Random.Range(0, tModifiers.Count)];
                ret.Add(randModifier);
                tModifiers.Remove(randModifier);
            }

            return ret;
        }

        public override void OnCollisionEvent(Collision collision)
        {
            var eatablesToAttach = new List<Eatable>();
            var other = collision.collider.gameObject;
            var eatable = other.gameObject.GetComponent<Eatable>();
            if (eatable != null)
            {
                eatablesToAttach.Add(eatable);
            }
            else
            {
                var eatables = other.gameObject.GetComponentsInChildren<Eatable>();
                if (eatables != null)
                {
                    eatablesToAttach.AddRange(eatables);
                }
            }

            if (eatablesToAttach.Count > 0)
            {
                foreach (var item in eatablesToAttach)
                {
                    item.modifiers.AddRange(GetRandomModifiers());
                }
            }

            base.OnCollisionEvent(collision);
        }
    }
}
