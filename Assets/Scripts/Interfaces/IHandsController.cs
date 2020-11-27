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
    }
}
