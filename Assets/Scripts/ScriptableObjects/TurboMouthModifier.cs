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
    public class TurboMouthModifier : GnamModifier
    {
        public float duration = 5;
        public AudioClip bonusMusic;
        public override void Activate(EaterDto eater)
        {
            eater.Mouth.EnableTurbo();
            eater.Mouth.PlayLoop(bonusMusic);
            eater.Mouth.StartCoroutine(WaitToDeactivate(eater, duration));
        }

        public override void Deactivate(EaterDto eater)
        {
            eater.Mouth.DisableTurbo();
            eater.Mouth.StopSound();
        }
    }
}
