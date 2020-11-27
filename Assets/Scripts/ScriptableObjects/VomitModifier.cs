using Assets.Scripts;
using Assets.Scripts.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VomitModifier : GnamModifier
{
    public GameObject modifierPrefab;
    public AudioClip modifierSound;


    public override void Activate(EaterDto eater)
    {
        var transform = eater.Mouth.GameObject.transform;
        var vomitGameobject = Instantiate(modifierPrefab, transform.position, transform.rotation);
        vomitGameobject.transform.parent = transform;
        eater.Mouth.PlaySound(modifierSound);
        eater.Mouth.DisableMouthForSeconds(3);
    }
}
