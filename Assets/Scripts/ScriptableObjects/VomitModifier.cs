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
    public float duration = 3;

    GameObject vomitGameobject;

    public override void Activate(EaterDto eater)
    {
        var transform = eater.Mouth.GameObject.transform;
        vomitGameobject = Instantiate(modifierPrefab, transform.position, transform.rotation);
        vomitGameobject.transform.parent = transform;
        eater.Mouth.PlaySound(modifierSound);
        eater.Mouth.DisableMouth();
        eater.Mouth.StartCoroutine(WaitToDeactivate(eater, duration));
    }

    public override void Deactivate(EaterDto eater)
    {
        Destroy(vomitGameobject);
        eater.Mouth.StopSound();
        eater.Mouth.EnableMouth();
    }
}
