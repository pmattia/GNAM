using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetSceneModifier : BaseModifier
{
    public string sceneName;
    public override void Activate(EaterDto eater)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
