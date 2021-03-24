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
    public class ExplodeHandsModifier : GnamModifier
    {
        public float duration = 3;
        public AudioClip explodeClip;
        public GameObject explodeParticle;

        private int leftHandIndex;
        private int rightHandIndex;

        public float animationDuration = 2f;
        public float scaleFactor = 4;

        bool isActive = false;

        public override void Activate(EaterDto eater)
        {
            isActive = true;
            ScaleHands(eater.Hands);
            eater.Mouth.StartCoroutine(WaitTillExplode(eater));
        }

        IEnumerator LerpScale(Transform transform, Vector3 initialScale, Vector3 finalScale)
        {
            float progress = 0;

            while (progress <= animationDuration && isActive)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, progress);
                progress += Time.deltaTime;
                yield return null;
            }

            if (isActive)
            {
                transform.localScale = finalScale;
            }
        }

        void ScaleHands(IHandsController handsController)
        {
            
            handsController.StartCoroutine(LerpScale(handsController.LeftHandHolder.transform,
                                    handsController.LeftHandHolder.transform.localScale,
                                    handsController.LeftHandHolder.transform.localScale * scaleFactor));

            handsController.StartCoroutine(LerpScale(handsController.RightHandHolder.transform,
                                        handsController.RightHandHolder.transform.localScale,
                                        handsController.RightHandHolder.transform.localScale * scaleFactor));
            
        }

        IEnumerator WaitTillExplode(EaterDto eater)
        {
            yield return new WaitForSeconds(animationDuration);

            if (isActive)
            {
                eater.Mouth.PlaySound(explodeClip);
                leftHandIndex = eater.Hands.DisableLeftHand();
                eater.Hands.AttachToLeftHand(explodeParticle);

                rightHandIndex = eater.Hands.DisableRightHand();
                eater.Hands.AttachToRightHand(explodeParticle);

                eater.Hands.StartCoroutine(WaitToDeactivate(eater, duration));
            }

            
        }

        public override void Deactivate(EaterDto eater)
        {
            isActive = false;
            eater.Hands.LeftHandHolder.transform.localScale = Vector3.one;
            eater.Hands.RightHandHolder.transform.localScale = Vector3.one;
            eater.Hands.EnableLeftHand(leftHandIndex);
            eater.Hands.EnableRightHand(rightHandIndex);
        }
    }
}
