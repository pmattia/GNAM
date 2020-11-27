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
        public VrifHandsControllerAdapter(HandModelSelector handModelSelector)
        {
            _handModelSelector = handModelSelector;
        }

        public int ModelCount { get { return _handModelSelector.LeftHandGFXHolder.childCount; } }

        public Transform LeftHandHolder { get { return _handModelSelector.LeftHandGFXHolder; } }

        public Transform RightHandHolder { get { return _handModelSelector.RightHandGFXHolder; } }

        public IGrabber LeftGrabber { get {
                var grabber = LeftHandHolder.parent.GetComponentInChildren<Grabber>();
                return new VrifGrabberAdapter(grabber);
            } 
        }

        public IGrabber RightGrabber
        {
            get
            {
                var grabber = RightHandHolder.parent.GetComponentInChildren<Grabber>();
                return new VrifGrabberAdapter(grabber);
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
    }
}
