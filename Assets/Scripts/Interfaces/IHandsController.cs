using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    public interface IHandsController
    {
        void ChangeHandsModel(int childIndex, bool save = false);
        void SwitchHands();
        int ModelCount { get; }
        Transform LeftHandHolder { get; }
        Transform RightHandHolder { get; }
        IGrabber LeftGrabber { get; }
        IGrabber RightGrabber { get; }
        Coroutine StartCoroutine(IEnumerator routine);
        int DisableLeftHand();
        int DisableRightHand();
        int GetLeftHandIndex();
        int GetRightHandIndex();
        GameObject GetLeftHand(int index = -1);
        GameObject GetRightHand(int index = -1);
        GameObject EnableLeftHand(int index);
        GameObject EnableRightHand(int index);
        GameObject AttachToLeftHand(GameObject prefab);
        GameObject AttachToRightHand(GameObject prefab);
    }
}
