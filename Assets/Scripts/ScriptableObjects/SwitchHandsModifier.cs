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
    public class SwitchHandsModifier : GnamModifier
    {
        public float duration = 5;
        public override void Activate(EaterDto eater)
        {
            eater.Hands.SwitchHands();
            eater.Hands.StartCoroutine(WaitToDisable(eater));
        }
        IEnumerator WaitToDisable(EaterDto eater)
        {
            yield return new WaitForSeconds(duration);

            eater.Hands.SwitchHands();
        }
    }
}
