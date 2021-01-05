using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.ScriptableObjects
{
    [CreateAssetMenu]
    class ChangeSceneModifier : GnamModifier
    {
        public string sceneName;
        public override void Activate(EaterDto eater)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
            }
            else
            {
                SceneManager.LoadSceneAsync(sceneName);
            }
        }

        public override void Deactivate(EaterDto eater)
        {
            throw new NotImplementedException();
        }
    }
}
