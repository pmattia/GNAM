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
            if (food != null)
            {
                foreach (var item in food.eatableParts)
                {
                    Debug.Log($"COLORA {item.name}");
                    var renderer = item.GetComponent<Renderer>();
                    renderer.material = null;
                    item.modifiers.AddRange(modifiers);
                }
            }
        }
    }
}
