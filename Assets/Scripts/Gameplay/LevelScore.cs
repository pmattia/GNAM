using Assets.Scripts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay
{
    public class LevelScore : MonoBehaviour
    {
        [SerializeField] List<GameObject> stars;
        [SerializeField] Text scoreText;
        [SerializeField] Transform bonusHolder; //deprecated
        [SerializeField] SpriteRenderer BonusIcon;
        [SerializeField] Text foodsCount;
        [SerializeField] Text foodsResult;
        [SerializeField] Text secondsCount;
        [SerializeField] Text secondsResult;
        [SerializeField] Text objectivesCount;
        [SerializeField] Text objectivesResult;
        [SerializeField] Text killsCount;
        [SerializeField] Text killsResult;
        public void SetResult(LevelResults results, int rate, GameObject bonus = null)
        {
            scoreText.text = $"Score {results.TotalPoints}";
            SetFoodsStats(results);
            SetObjectivesStats(results);
            SetSecondsStats(results);
            SetKillsStats(results);

            for(int i = 0; i< stars.Count; i++)
            {
                //stars[i].SetActive(rate > i);
                stars[i].SetActive(false);
            }

            foreach (Transform child in bonusHolder.transform)
            {
                Destroy(child.gameObject);
            }
            BonusIcon.sprite = null;

            if (bonus != null)
            {
                var currentBonusIcon = bonus.GetComponent<GnamGrabbable>().Icon;
                if (currentBonusIcon != null)
                {
                    BonusIcon.sprite = currentBonusIcon;
                }
                else
                {
                    bonusHolder.gameObject.SetActive(true);
                    var holdedBonus = Instantiate(bonus, bonusHolder);
                    holdedBonus.transform.localPosition = Vector3.zero;
                    holdedBonus.transform.localRotation = Quaternion.identity;
                    holdedBonus.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
            else
            {
                bonusHolder.gameObject.SetActive(false);
            }
        }

        void SetFoodsStats(LevelResults results)
        {
            foodsCount.text = results.FoodsCount.ToString();
            foodsResult.text = $"{results.FoodsPoints} points";
        }
        void SetSecondsStats(LevelResults results)
        {
            secondsCount.text = results.SecondsCount.ToString();
            secondsResult.text = $"{results.SecondsPoints} points";
        }

        void SetObjectivesStats(LevelResults results)
        {
            objectivesCount.text = results.ObjectivesCount.ToString();
            objectivesResult.text = $"{results.ObjectivesPoints} points";
        }

        void SetKillsStats(LevelResults results)
        {
            killsCount.text = results.KillsCount.ToString();
            killsResult.text = $"{results.KillsPoints} points";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public class LevelResults
    {
        int foodPoints;
        int secondPoints;
        int objectivePoints;
        int killPoints;

        public int FoodsCount { get; set; }
        public int SecondsCount { get; set; }
        public int ObjectivesCount { get; set; }
        public int KillsCount { get; set; }

        public LevelResults(int foodPoints, int secondPoints, int objectivePoints, int killPoints)
        {
            this.foodPoints = foodPoints;
            this.secondPoints = secondPoints;
            this.objectivePoints = objectivePoints;
            this.killPoints = killPoints;
        }

        public static LevelResults GetNewInstance()
        {
            return new LevelResults(GnamConstants.foodScore, GnamConstants.secondsScore, GnamConstants.objectiveScore, GnamConstants.mobKillScore);
        }

        public int FoodsPoints { get { return FoodsCount * foodPoints; } }
        public int SecondsPoints { get { return SecondsCount * secondPoints; } }
        public int ObjectivesPoints { get { return ObjectivesCount * objectivePoints; } }
        public int KillsPoints { get { return KillsCount * killPoints; } }
        public int TotalPoints 
        { 
            get {
                return FoodsPoints
                        + SecondsPoints
                        + ObjectivesPoints
                        + KillsPoints;
            } 
        }
    }

}
