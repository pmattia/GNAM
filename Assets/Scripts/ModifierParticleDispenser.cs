using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ModifierParticleDispenser : MonoBehaviour
    {
        public List<GnamModifier> modifiers = new List<GnamModifier>();

        private void OnParticleCollision(GameObject other)
        {
            var food = other.gameObject.GetComponent<Food>();
            if(food == null)
            {
                food = other.gameObject.GetComponentInChildren<Food>();
            }

            if (food != null)
            {
                foreach (var item in food.eatableParts)
                {
                    var renderer = item.GetComponent<Renderer>();
                    if (renderer == null)
                    {
                        renderer = item.GetComponentInChildren<Renderer>();
                    }

                    if (renderer != null)
                    {
                        renderer.material.SetColor("Color_B5C1F6F5", new Color32(0x3D, 0xEE, 0x15, 0));
                        item.modifiers.AddRange(modifiers);
                    }
                }
            }
        }
    }
}
