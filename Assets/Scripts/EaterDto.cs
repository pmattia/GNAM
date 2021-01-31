using Assets.Scripts;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ScriptableObjects;
using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaterDto
{
    public IMouthController Mouth { get; private set; }
    public IHandsController Hands { get; private set; }
    public ITimeController Time { get; private set; }
    public EaterDto(IMouthController mouth, IHandsController handSelector, ITimeController timeController)
    {
        Mouth = mouth;
        Hands = handSelector;
        Time = timeController;
    }
}
