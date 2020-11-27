using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    public class ExplodeHandsModifier : GnamModifier
    {
        public AudioClip explodeClip;
        public GameObject explodeParticle;
        public override void Activate(EaterDto eater)
        {
            eater.Mouth.PlaySound(explodeClip);
            foreach (Transform model in eater.Hands.LeftHandHolder.transform)
            {
                Destroy(model.gameObject);
            }
            var particleGameobjectL = Instantiate(explodeParticle, eater.Hands.LeftHandHolder.transform.position, eater.Hands.LeftHandHolder.transform.rotation);
            particleGameobjectL.transform.parent = eater.Hands.LeftHandHolder.transform;

            foreach (Transform model in eater.Hands.RightHandHolder.transform)
            {
                Destroy(model.gameObject);
            }
            var particleGameobjectR = Instantiate(explodeParticle, eater.Hands.RightHandHolder.transform.position, eater.Hands.RightHandHolder.transform.rotation);
            particleGameobjectR.transform.parent = eater.Hands.RightHandHolder.transform;   
        }
    }
}
