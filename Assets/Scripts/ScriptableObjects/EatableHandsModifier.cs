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
        private int leftHandIndex;
        private int rightHandIndex;

        public GameObject eatablePrefab;
        public override void Activate(EaterDto eater)
        {
            leftHandIndex = eater.Hands.DisableLeftHand();
            var eatableLeft = eater.Hands.AttachToLeftHand(eatablePrefab).GetComponent<Eatable>();
            eatableLeft.onEated += (leftEater) =>
            {
                eater.Hands.EnableLeftHand(leftHandIndex);
            };

            rightHandIndex = eater.Hands.DisableRightHand();
            var eatableRight = eater.Hands.AttachToRightHand(eatablePrefab).GetComponent<Eatable>();
            eatableRight.onEated += (righEater) =>
            {
                eater.Hands.EnableRightHand(rightHandIndex);
            };
        }

        public override void Deactivate(EaterDto eater)
        {
            eater.Hands.EnableLeftHand(leftHandIndex);
            eater.Hands.EnableRightHand(rightHandIndex);
        }
    }
}
