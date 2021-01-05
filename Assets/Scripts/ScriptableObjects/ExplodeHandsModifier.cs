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

        public override void Activate(EaterDto eater)
        {
            eater.Mouth.PlaySound(explodeClip);
            leftHandIndex = eater.Hands.DisableLeftHand();
            eater.Hands.AttachToLeftHand(explodeParticle);

            rightHandIndex = eater.Hands.DisableRightHand();
            eater.Hands.AttachToRightHand(explodeParticle);

            eater.Hands.StartCoroutine(WaitToDeactivate(eater,duration));
        }

        public override void Deactivate(EaterDto eater)
        {
            eater.Hands.EnableLeftHand(leftHandIndex);
            eater.Hands.EnableRightHand(rightHandIndex);
        }
    }
}
