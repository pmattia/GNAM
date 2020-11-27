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
            eater.Mouth.PlaySound(bonusMusic);
            eater.Mouth.StartCoroutine(WaitToDisable(eater));
        }

        IEnumerator WaitToDisable(EaterDto eater)
        {
            yield return new WaitForSeconds(duration);

            eater.Mouth.DisableTurbo();
        }
    }
}
