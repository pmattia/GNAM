using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    class ChangeTimeModifier : GnamModifier
    {
        public float duration = 3;
        public override void Activate(EaterDto eater)
        {
            eater.Time.SlowTime();
            eater.Hands.StartCoroutine(WaitToDeactivate(eater, duration));
        }

        public override void Deactivate(EaterDto eater)
        {
            eater.Time.ResumeTime();
        }
    }
}
