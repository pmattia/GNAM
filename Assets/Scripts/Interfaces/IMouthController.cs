using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    public interface IMouthController
    {
        void PlaySound(AudioClip clip);
        void StopSound();
        void DisableMouthForSeconds(float time);
        GameObject GameObject { get; }
        Coroutine StartCoroutine(IEnumerator routine);
        void EnableMouth();
        void DisableMouth();
        void EnableTurbo();
        void DisableTurbo();
    }
}
