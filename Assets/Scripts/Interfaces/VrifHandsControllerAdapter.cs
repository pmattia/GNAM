using Assets.Scripts.Interfaces;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    public class VrifHandsControllerAdapter : IHandsController
    {
        HandModelSelector _handModelSelector;
        IGrabber _leftHandGrabber;
        IGrabber _rightHandGrabber;

        public event Action<Grabbable> onLeftHandGrab;
        public event Action<Grabbable> onRightHandGrab;
        public event Action<Grabbable> onLeftHandRelease;
        public event Action<Grabbable> onRightHandRelease;

        public VrifHandsControllerAdapter(HandModelSelector handModelSelector)
        {
            _handModelSelector = handModelSelector;

            var leftGrabber = LeftHandHolder.parent.GetComponentInChildren<Grabber>();
            _leftHandGrabber = new VrifGrabberAdapter(leftGrabber);
            _leftHandGrabber.onGrabEvent.AddListener(OnLeftHandGrab);
            _leftHandGrabber.onReleaseEvent.AddListener(OnLeftHandRelease);

            var rightGrabber = RightHandHolder.parent.GetComponentInChildren<Grabber>();
            _rightHandGrabber = new VrifGrabberAdapter(rightGrabber);
            _rightHandGrabber.onGrabEvent.AddListener(OnRightHandGrab);
            _rightHandGrabber.onReleaseEvent.AddListener(OnRightHandRelease);
        }

        void OnLeftHandGrab(Grabbable grabbable)
        {
            if (onLeftHandGrab != null)
            {
                Debug.Log("grab left");
                onLeftHandGrab.Invoke(grabbable);
            }
        }

        void OnRightHandGrab(Grabbable grabbable)
        {
            if (onRightHandGrab != null)
            {
                onRightHandGrab.Invoke(grabbable);
            }
        }

        void OnLeftHandRelease(Grabbable grabbable)
        {
            if (onLeftHandRelease != null)
            {
                Debug.Log("drop left");
                onLeftHandRelease.Invoke(grabbable);
            }
        }

        void OnRightHandRelease(Grabbable grabbable)
        {
            if (onRightHandRelease != null)
            {
                onRightHandRelease.Invoke(grabbable);
            }
        }

        public int ModelCount { get { return _handModelSelector.LeftHandGFXHolder.childCount; } }

        public Transform LeftHandHolder { get { return _handModelSelector.LeftHandGFXHolder; } }

        public Transform RightHandHolder { get { return _handModelSelector.RightHandGFXHolder; } }

        public IGrabber LeftGrabber { 
            get 
            {
                return _leftHandGrabber;
            } 
        }

        public IGrabber RightGrabber
        {
            get
            {
                return _rightHandGrabber;
            }
        }

        public void ChangeHandsModel(int childIndex, bool save = false)
        {
            _handModelSelector.ChangeHandsModel(childIndex, save);
        }

        public void SwitchHands()
        {
            var leftController = LeftHandHolder.parent.GetComponent<HandController>();
            var rightController = RightHandHolder.parent.GetComponent<HandController>();

            var tGrabber = leftController.grabber;
            leftController.grabber = rightController.grabber;
            rightController.grabber = tGrabber;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _handModelSelector.StartCoroutine(routine);
        }

        public int DisableLeftHand()
        {
            LeftGrabber.TryRelease();
            LeftGrabber.Enabled = false;
            return DisableHand(LeftHandHolder);
        }
        public int DisableRightHand()
        {
            RightGrabber.TryRelease();
            RightGrabber.Enabled = false;
            return DisableHand(RightHandHolder);
        }
        public int GetLeftHandIndex()
        {
            return GetHandIndex(LeftHandHolder);
        }
        public int GetRightHandIndex()
        {
            return GetHandIndex(RightHandHolder);
        }
        public GameObject GetLeftHand(int index = -1)
        {
            if (index == -1)
            {
                return GetHand(LeftHandHolder);
            }
            else
            {
                return LeftHandHolder.GetChild(index).gameObject;
            }
        }
        public GameObject GetRightHand(int index = -1)
        {
            if (index == -1)
            {
                return GetHand(RightHandHolder);
            }
            else
            {
                return RightHandHolder.GetChild(index).gameObject;
            }
        }
        public GameObject EnableLeftHand(int index)
        {
            DisableChildren(LeftHandHolder);
            LeftGrabber.Enabled = true;
            var selectedModel = LeftHandHolder.GetChild(index).gameObject;
            selectedModel.SetActive(true);
            return selectedModel;
        }
        public GameObject EnableRightHand(int index)
        {
            DisableChildren(RightHandHolder);
            RightGrabber.Enabled = true;
            var selectedModel = RightHandHolder.GetChild(index).gameObject;
            selectedModel.SetActive(true);
            return selectedModel;
        }
        public GameObject AttachToLeftHand(GameObject prefab)
        {
            var prefabGameobject = UnityEngine.Object.Instantiate(prefab, LeftHandHolder.transform.position, LeftHandHolder.transform.rotation);
            prefabGameobject.transform.parent = LeftHandHolder.transform;
            return prefabGameobject;
        }
        public GameObject AttachToRightHand(GameObject prefab)
        {
            var prefabGameobject = UnityEngine.Object.Instantiate(prefab, RightHandHolder.transform.position, RightHandHolder.transform.rotation);
            prefabGameobject.transform.parent = RightHandHolder.transform;
            return prefabGameobject;
        }
        private int DisableHand(Transform handHolder)
        {
            int index = 0;
            foreach (Transform model in handHolder.transform)
            {
                if (model.gameObject.activeSelf)
                {
                    model.gameObject.SetActive(false);
                    return index;
                }
                index++;
            }
            return -1;
        }
        private GameObject GetHand(Transform handHolder)
        {
            GameObject activeHand = null;
            foreach (Transform model in handHolder.transform)
            {
                if (model.gameObject.activeSelf)
                {
                    activeHand = model.gameObject;
                }
            }
            return activeHand;
        }
        private int GetHandIndex(Transform handHolder)
        {
            int index = 0;
            foreach (Transform model in handHolder.transform)
            {
                if (model.gameObject.activeSelf)
                {
                    return index;
                }
                index++;
            }
            return -1;
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
