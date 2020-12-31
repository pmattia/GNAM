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

        private GameObject leftModel;
        private GameObject rightModel;

        public override void Activate(EaterDto eater)
        {
            var leftHandHolder = eater.Hands.LeftHandHolder;
            var rightHandHolder = eater.Hands.RightHandHolder;

            eater.Mouth.PlaySound(explodeClip);
            leftModel = base.DisableLeftHand(eater.Hands);
            base.AttachToLeftHand(eater.Hands, explodeParticle);

            rightModel = base.DisableRightHand(eater.Hands);
            base.AttachToRightHand(eater.Hands, explodeParticle);

            eater.Hands.StartCoroutine(WaitToReattach(eater));
        }

        IEnumerator WaitToReattach(EaterDto eater)
        {
            yield return new WaitForSeconds(duration);

            leftModel.SetActive(true);
            eater.Hands.LeftGrabber.Enabled = true;
            rightModel.SetActive(true);
            eater.Hands.RightGrabber.Enabled = true;
        }
    }
}
