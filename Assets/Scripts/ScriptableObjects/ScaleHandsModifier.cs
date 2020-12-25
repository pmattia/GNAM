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

        public override void Activate(EaterDto eater)
        {
            eater.Hands.StartCoroutine(LerpScale(eater.Hands.LeftHandHolder.transform,
                                       eater.Hands.LeftHandHolder.transform.localScale,
                                       eater.Hands.LeftHandHolder.transform.localScale * scaleFactor));

            eater.Hands.StartCoroutine(LerpScale(eater.Hands.RightHandHolder.transform,
                                       eater.Hands.RightHandHolder.transform.localScale,
                                       eater.Hands.RightHandHolder.transform.localScale * scaleFactor));

            eater.Hands.StartCoroutine(WaitToResize(eater));
        }

        IEnumerator WaitToResize(EaterDto eater)
        {
            yield return new WaitForSeconds(duration);

            eater.Hands.StartCoroutine(LerpScale(eater.Hands.LeftHandHolder.transform,
                                       eater.Hands.LeftHandHolder.transform.localScale,
                                       eater.Hands.LeftHandHolder.transform.localScale / scaleFactor));

            eater.Hands.StartCoroutine(LerpScale(eater.Hands.RightHandHolder.transform,
                                       eater.Hands.RightHandHolder.transform.localScale,
                                       eater.Hands.RightHandHolder.transform.localScale / scaleFactor));
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
    }
}
