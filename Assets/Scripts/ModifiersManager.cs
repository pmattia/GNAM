using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Mouth))]
public class ModifiersManager : MonoBehaviour
{
    Mouth mouth;
    void Awake()
    {
        mouth = GetComponent<Mouth>();
    }
}
