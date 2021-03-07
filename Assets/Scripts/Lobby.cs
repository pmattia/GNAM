using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Lobby: MonoBehaviour
    {
        [SerializeField] Food foodStarter;
        [SerializeField] string sceneToStart;

        private void Start()
        {
            foodStarter.onEated += (eater) =>
            {
                SceneManager.LoadSceneAsync(sceneToStart);
            };
        }
    }
}
