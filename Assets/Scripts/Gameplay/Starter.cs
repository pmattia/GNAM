using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay
{
    public class Starter: MonoBehaviour
    {
        [SerializeField] GameObject nextLevelEatable;
        [SerializeField] GameObject startEatable;
        [SerializeField] GameObject restartEatable;
        [SerializeField] GameObject retryEatable;
        [SerializeField] GameObjectSpawner commandSpawner;
        [SerializeField] GameObject instructionsPanel;
        [SerializeField] GameObject endgamePanel;
        [SerializeField] Text bestScoreLabel;

        public event Action<EaterDto> onStart;
        public event Action<EaterDto> onNextlevel;
        public event Action<EaterDto> onRetry;

        List<GameObject> spawnedItems = new List<GameObject>();


        private void Awake()
        {
            onStart += (eater) =>
            {
                if (instructionsPanel != null)
                {
                    instructionsPanel.SetActive(false);
                }
            };
        }

        public void InitStarter(int score)
        {
            gameObject.SetActive(true);
            bestScoreLabel.text = score.ToString();
        }

        public void SpawnNextLevelEatable()
        {
            InitStarter(()=> {
                spawnedItems.Add(commandSpawner.SpawnObject(nextLevelEatable, onNextlevel));
            });
        }

        public void SpawnStartEatable()
        {
            InitStarter(() => {
                spawnedItems.Add(commandSpawner.SpawnObject(startEatable, onStart));
            });
        }
        public void SpawnRetryEatable()
        {
            InitStarter(() => {
                spawnedItems.Add(commandSpawner.SpawnObject(retryEatable, onRetry));
            });
        }

        public void ShowEndgameMessage()
        {
            endgamePanel.SetActive(true);
        }

        void InitStarter(Action delayedCallback = null)
        {
            ClearStarter();
            spawnedItems.Add(commandSpawner.SpawnObject(restartEatable, (eater) => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }));
            if (delayedCallback != null)
            {
                StartCoroutine(DelayedFun(delayedCallback));
            }
        }

        IEnumerator DelayedFun(Action fun)
        {
            yield return new WaitForSeconds(.1f);
            fun.Invoke();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            InitStarter();
        }

        public void Hide()
        {
            ClearStarter();
            gameObject.SetActive(false);
        }

        void ClearStarter()
        {
            foreach (var item in spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            spawnedItems.Clear();
        }
    }
}
