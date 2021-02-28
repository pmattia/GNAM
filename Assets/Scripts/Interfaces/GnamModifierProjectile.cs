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
    public class GnamModifierProjectile : GnamProjectile
    {
        [SerializeField]
        List<GnamModifier> modifiers = new List<GnamModifier>();
        int modifiersLength = 1;
        [SerializeField] AudioClip hitSound;
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
                    Debug.Log($"COLORA {item.name}");
                    var renderer = item.GetComponent<Renderer>();
                    //renderer.material.SetColor("_Color", Color.white);
                    //renderer.material.mainTexture = null;
                    if (renderer != null)
                    {
                        //3DEE15
                        VRUtils.Instance.PlaySpatialClipAt(hitSound, transform.position, 1f, 0.5f);
                        renderer.material.SetColor("Color_B5C1F6F5", new Color32(0x3D, 0xEE, 0x15, 0));
                     //   renderer.material = null;
                        item.modifiers.AddRange(GetRandomModifiers());
                    }
                }
            }

            base.OnCollisionEvent(collision);
        }
    }
}
