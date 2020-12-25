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
        private GameObject leftModel;
        private GameObject rightModel;

        public GameObject eatablePrefab;
        public override void Activate(EaterDto eater)
        {
            leftModel = base.DisableLeftHand(eater.Hands);
            var eatableLeft = base.AttachToLeftHand(eater.Hands, eatablePrefab).GetComponent<Eatable>();
            eatableLeft.onEated += (leftEater) =>
            {
                leftModel.SetActive(true);
                leftEater.Hands.LeftGrabber.Enabled = true;
            };

            rightModel = base.DisableRightHand(eater.Hands);
            var eatableRight = base.AttachToRightHand(eater.Hands, eatablePrefab).GetComponent<Eatable>();
            eatableRight.onEated += (righEater) =>
            {
                rightModel.SetActive(true);
                righEater.Hands.RightGrabber.Enabled = true;
            };
        }
    }
}
