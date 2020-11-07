using Assets.Scripts;
using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaterDto
{
    public Mouth Mouth { get; private set; }
    public HandModelSelector HandsSelector { get; private set; }
    public EaterDto(Mouth mouth, HandModelSelector handSelector)
    {
        Mouth = mouth;
        HandsSelector = handSelector;
    }
}
