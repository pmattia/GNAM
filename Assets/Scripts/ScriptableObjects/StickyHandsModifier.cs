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
    public class StickyHandsModifier : GnamModifier
    {
        public float duration;
        public override void Activate(EaterDto eater)
        {
            var leftGrabber = eater.Hands.LeftGrabber;
            var rightGrabber = eater.Hands.RightGrabber;

            leftGrabber.ForceGrab = true;
            rightGrabber.ForceGrab = true;

            eater.Hands.StartCoroutine(WaitToDisable(eater));
        }

        IEnumerator WaitToDisable(EaterDto eater)
        {
            yield return new WaitForSeconds(duration);

            var leftGrabber = eater.Hands.LeftGrabber;
            var rightGrabber = eater.Hands.RightGrabber;

            leftGrabber.ForceGrab = false;
            rightGrabber.ForceGrab = false;
        }
    }
}
