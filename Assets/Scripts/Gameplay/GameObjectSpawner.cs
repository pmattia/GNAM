using Assets.Scripts;
using BNG;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    
    protected GameObject lastGameobject;
    

    public GameObject SpawnObject(GameObject prefab, Action<EaterDto> onEated)
    {
        lastGameobject = Instantiate(prefab, transform.position, Quaternion.identity);
        lastGameobject.GetComponent<Eatable>().onEated += onEated;
        return lastGameobject;
    }
}
