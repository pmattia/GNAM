using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class EatableHandsModifier : GnamModifier
    {
        public GameObject eatablePrefab;
        public override void Activate(EaterDto eater)
        {
            Debug.Log("eatable");
            foreach (Transform model in eater.Hands.LeftHandHolder.transform)
            {
                Destroy(model.gameObject);
            }
            var particleGameobjectL = Instantiate(eatablePrefab, eater.Hands.LeftHandHolder.transform.position, eater.Hands.LeftHandHolder.transform.rotation);
            particleGameobjectL.transform.parent = eater.Hands.LeftHandHolder.transform;

            foreach (Transform model in eater.Hands.RightHandHolder.transform)
            {
                Destroy(model.gameObject);
            }
            var particleGameobjectR = Instantiate(eatablePrefab, eater.Hands.RightHandHolder.transform.position, eater.Hands.RightHandHolder.transform.rotation);
            particleGameobjectR.transform.parent = eater.Hands.RightHandHolder.transform;
        }
    }
}
