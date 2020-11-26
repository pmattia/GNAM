using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class VomitModifierBuilder : ScriptableObject
{
    public GameObject modifierPrefab;
    public AudioClip modifierSound;

    public void Activate(Mouth mouth)
    {
        var vomitGameobject = Instantiate(modifierPrefab, mouth.transform.position, mouth.transform.rotation);
        vomitGameobject.transform.parent = mouth.transform;
        mouth.PlaySound(modifierSound);
        mouth.DisableMouthForSeconds(3);
    }
}
