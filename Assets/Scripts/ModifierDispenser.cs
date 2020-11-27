using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ModifierDispenser : MonoBehaviour
    {
        public List<GnamModifier> modifiers = new List<GnamModifier>();

        private void OnTriggerEnter(Collider other)
        {
            var eatablesToAttach = new List<Eatable>();
            var eatable = other.gameObject.GetComponent<Eatable>();
            if(eatable != null)
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

            if(eatablesToAttach.Count > 0)
            {
                foreach(var item in eatablesToAttach)
                {
                    item.modifiers.AddRange(modifiers);
                }
            }
        }
    }
}
