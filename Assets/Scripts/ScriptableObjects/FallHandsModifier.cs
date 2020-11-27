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
            eater.Hands.StartCoroutine(WaitToReattach(leftHandHolder, rightHandHolder));
        }

        IEnumerator WaitToReattach(Transform leftHandHolder, Transform rightHandHolder)
        {
            yield return new WaitForSeconds(duration);

            Destroy(leftHandHolder.GetComponent<Rigidbody>());
            leftHandHolder.SetParent(prevParentL);
            leftHandHolder.localPosition = Vector3.zero;
            leftHandHolder.localRotation = Quaternion.identity;

            Destroy(rightHandHolder.GetComponent<Rigidbody>());
            rightHandHolder.SetParent(prevParentR);
            rightHandHolder.localPosition = Vector3.zero;
            rightHandHolder.localRotation = Quaternion.identity;

        }
    }
}
