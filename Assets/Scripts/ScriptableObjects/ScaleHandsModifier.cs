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
    public class ScaleHandsModifier : GnamModifier
    {
        public float duration = 3;
        public float scaleFactor = 2;

        float animationDuration = .5f;
        public GameObject magicParticle;

        public override void Activate(EaterDto eater)
        {
            ScaleHands(eater.Hands);
            eater.Hands.AttachToLeftHand(magicParticle);
            eater.Hands.AttachToRightHand(magicParticle);

            eater.Mouth.StartCoroutine(WaitToDeactivate(eater, duration));
        }

        IEnumerator LerpScale(Transform transform,Vector3 initialScale, Vector3 finalScale)
        {
            float progress = 0;

            while (progress <= animationDuration)
            {
                transform.localScale = Vector3.Lerp(initialScale, finalScale, progress);
                progress += Time.deltaTime;
                yield return null;
            }
            transform.localScale = finalScale;

        }

        void ScaleHands(IHandsController handsController, bool decrease = false)
        {
            if (decrease)
            {
                handsController.StartCoroutine(LerpScale(handsController.LeftHandHolder.transform,
                                           handsController.LeftHandHolder.transform.localScale,
                                           handsController.LeftHandHolder.transform.localScale / scaleFactor));

                handsController.StartCoroutine(LerpScale(handsController.RightHandHolder.transform,
                                           handsController.RightHandHolder.transform.localScale,
                                           handsController.RightHandHolder.transform.localScale / scaleFactor));
            }
            else
            {
                handsController.StartCoroutine(LerpScale(handsController.LeftHandHolder.transform,
                                       handsController.LeftHandHolder.transform.localScale,
                                       handsController.LeftHandHolder.transform.localScale * scaleFactor));

                handsController.StartCoroutine(LerpScale(handsController.RightHandHolder.transform,
                                           handsController.RightHandHolder.transform.localScale,
                                           handsController.RightHandHolder.transform.localScale * scaleFactor));
            }
        }

        public override void Deactivate(EaterDto eater)
        {
            eater.Hands.LeftHandHolder.transform.localScale = Vector3.one;
            eater.Hands.RightHandHolder.transform.localScale = Vector3.one;
            //ScaleHands(eater.Hands, true);
        }
    }
}
