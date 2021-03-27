using System;
using System.Collections;
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

        public GameObject magicParticle;
        public GameObject eatablePrefab;

        bool isActive = false;

        public override void Activate(EaterDto eater)
        {
            isActive = true;
            eater.Mouth.StartCoroutine(DelayedFun(eater));
        }

        public override void Deactivate(EaterDto eater)
        {
            isActive = false;
            eater.Hands.EnableLeftHand(leftHandIndex);
            eater.Hands.EnableRightHand(rightHandIndex);
        }

        IEnumerator DelayedFun(EaterDto eater)
        {
            if (isActive)
            {
                yield return new WaitForSeconds(.2f);
                leftHandIndex = eater.Hands.DisableLeftHand();
                eater.Hands.AttachToLeftHand(magicParticle);
                var eatableLeft = eater.Hands.AttachToLeftHand(eatablePrefab).GetComponent<Eatable>();

                eatableLeft.onEated += (leftEater) =>
                {
                    eater.Hands.EnableLeftHand(leftHandIndex);
                };

                rightHandIndex = eater.Hands.DisableRightHand();
                eater.Hands.AttachToRightHand(magicParticle);
                var eatableRight = eater.Hands.AttachToRightHand(eatablePrefab).GetComponent<Eatable>();
                eatableRight.onEated += (righEater) =>
                {
                    eater.Hands.EnableRightHand(rightHandIndex);
                };
            }
        }
    }
}
