using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay
{
    public class GameScores: MonoBehaviour
    {
        [SerializeField] GameObject itemPrefab;
        [SerializeField] Transform resultGrid;
        [SerializeField] Text totalScore;
        [SerializeField] GameObject newRecordRibbon;
        [SerializeField] Text foodLeft;
        [SerializeField] GameObject leftBlock;
        [SerializeField] GameObject rightBlock;

        float deltaPositionX = 0.316f;

        public void ShowResults(Dictionary<int, LevelResults> results, bool isNewRecord, int lastFoodLeft = 0)
        {
            gameObject.SetActive(true);

            foreach(Transform item in resultGrid)
            {
                Destroy(item.gameObject);
            }

            int scoresSum = 0;
            foreach (var item in results)
            {
                var listItem = Instantiate(itemPrefab, resultGrid);
                var levelScoreItem = listItem.GetComponent<LevelScoreItem>();
                levelScoreItem.SetLevelScore(item.Key, item.Value.TotalPoints);

                scoresSum += item.Value.TotalPoints;
            }

            totalScore.text = $"Score {scoresSum}";
            newRecordRibbon.SetActive(isNewRecord);

            if(lastFoodLeft > 0)
            {
                leftBlock.SetActive(true);
                rightBlock.transform.localPosition = new Vector3(deltaPositionX, 0, 0);
                foodLeft.text = lastFoodLeft.ToString();
            }
            else
            {
                leftBlock.SetActive(false);
                rightBlock.transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        IEnumerator DelayedCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback.Invoke();
        }
    }
}
