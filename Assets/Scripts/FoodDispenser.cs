using Assets.Scripts.Machines;
using Assets.Scripts.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class FoodDispenser : MonoBehaviour
    {
        [SerializeField] private GameObject[] foods;
        [SerializeField] private GnamModifier[] modifiers;
        public event Action<GameObject> onSpawned;

        private void OnTriggerEnter(Collider other)
        {
            var carController = other.gameObject.GetComponent<CarController>();
            if(carController != null)
            {
                if (carController.CanHoldFood)
                {
                    var food = foods[UnityEngine.Random.Range(0, foods.Length)];
                    
                    var clone = Instantiate(food, carController.HolderTransform.position, Quaternion.identity);

                    var eatables = new List<Eatable>();

                    var baseEatable = clone.GetComponent<Eatable>();
                    if (baseEatable != null)
                    {
                        eatables.Add(baseEatable);
                    }
                    var childEtables = clone.GetComponentsInChildren<Eatable>();
                    if(childEtables != null)
                    {
                        eatables.AddRange(childEtables);
                    }

                    var modifier = modifiers[UnityEngine.Random.Range(0, modifiers.Length)];
                    eatables.ForEach(e => { e.modifiers = new List<GnamModifier>() { modifier }; });

                    if(onSpawned != null)
                    {
                        onSpawned.Invoke(clone);
                    }
                }


            }
        }
    }
}
