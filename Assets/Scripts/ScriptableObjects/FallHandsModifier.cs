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
    public class FallHandsModifier : GnamModifier
    {
        public float duration = 3;
        Transform prevParentL;
        Transform prevParentR;

        public override void Activate(EaterDto eater)
        {
            var leftHandHolder = eater.Hands.LeftHandHolder;
            var rightHandHolder = eater.Hands.RightHandHolder;

            prevParentL = leftHandHolder.parent;
            leftHandHolder.parent = null;
            var rigidBodyL = leftHandHolder.gameObject.AddComponent<Rigidbody>();
            rigidBodyL.useGravity = true;
            rigidBodyL.mass = 1;

            prevParentR = rightHandHolder.parent;
            rightHandHolder.parent = null;
            var rigidBodyR = rightHandHolder.gameObject.AddComponent<Rigidbody>();
            rigidBodyR.useGravity = true;
            rigidBodyR.mass = 1;
            eater.Hands.StartCoroutine(WaitToDeactivate(eater, duration));
        }

        public override void Deactivate(EaterDto eater)
        {
            Destroy(eater.Hands.LeftHandHolder.GetComponent<Rigidbody>());
            eater.Hands.LeftHandHolder.SetParent(prevParentL);
            eater.Hands.LeftHandHolder.localPosition = Vector3.zero;
            eater.Hands.LeftHandHolder.localRotation = Quaternion.identity;

            Destroy(eater.Hands.RightHandHolder.GetComponent<Rigidbody>());
            eater.Hands.RightHandHolder.SetParent(prevParentR);
            eater.Hands.RightHandHolder.localPosition = Vector3.zero;
            eater.Hands.RightHandHolder.localRotation = Quaternion.identity;
        }
    }
}
