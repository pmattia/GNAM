using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    public abstract class GnamModifier : ScriptableObject
    {
        public abstract void Activate(EaterDto eater);

        public GameObject DisableLeftHand(IHandsController handsController)
        {
            handsController.LeftGrabber.Enabled = false;
            return DisableHand(handsController.LeftHandHolder);
        }
        public GameObject DisableRightHand(IHandsController handsController)
        {
            handsController.RightGrabber.Enabled = false;
            return DisableHand(handsController.RightHandHolder);
        }
        public GameObject EnableLeftHand(IHandsController handsController, int index)
        {
            DisableChildren(handsController.LeftHandHolder);
            var selectedModel = handsController.LeftHandHolder.GetChild(index).gameObject;
            selectedModel.SetActive(true);
            return selectedModel;
        }
        public GameObject EnableRightHand(IHandsController handsController, int index)
        {
            DisableChildren(handsController.RightHandHolder);
            var selectedModel = handsController.RightHandHolder.GetChild(index).gameObject;
            selectedModel.SetActive(true);
            return selectedModel;
        }
        public GameObject AttachToLeftHand(IHandsController handsController, GameObject prefab)
        {
            var prefabGameobject = Instantiate(prefab, handsController.LeftHandHolder.transform.position, handsController.LeftHandHolder.transform.rotation);
            prefabGameobject.transform.parent = handsController.LeftHandHolder.transform;
            return prefabGameobject;
        }
        public GameObject AttachToRightHand(IHandsController handsController, GameObject prefab)
        {
            var prefabGameobject = Instantiate(prefab, handsController.RightHandHolder.transform.position, handsController.RightHandHolder.transform.rotation);
            prefabGameobject.transform.parent = handsController.RightHandHolder.transform;
            return prefabGameobject;
        }
        private GameObject DisableHand(Transform handHolder)
        {
            GameObject disabledHand = null;
            foreach (Transform model in handHolder.transform)
            {
                if (model.gameObject.activeSelf)
                {
                    model.gameObject.SetActive(false);
                    disabledHand = model.gameObject;
                }
            }
            return disabledHand;
        }
        private void DisableChildren(Transform handHolder)
        {
            foreach (Transform model in handHolder.transform)
            {
                model.gameObject.SetActive(false);             
            }
        }
    }
}
