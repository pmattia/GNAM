using Assets.Scripts.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] List<BillboardObjective> billboardObjectives;
        [SerializeField] List<Sprite> foodFamilyIcons;
        [SerializeField] GameObject gameover;
        [SerializeField] GameObject youwin;
        [SerializeField] Timer timer;
        [SerializeField] TextMeshPro foodCount;
        [SerializeField] TextMeshPro levelLabel;
        [SerializeField] LevelScore levelScore;
        [SerializeField] GameScores gameScores;
 
        public event Action<Food.FoodFamily, List<Food.FoodFamily>, GameObject> onObjectiveCompleted;
        public event Action<Food.FoodFamily, List<Food.FoodFamily>> onObjectiveExpired;
        public event Action<int> onGameCompleted;
        public event Action onTimeExpired;

        List<ObjectiveDto> objectives = new List<ObjectiveDto>();
        int foodsEated;
        int foodsToEat;
        [SerializeField] RadialProgress coronaProgress; //disabled until reviewed

        private void Start()
        {

            billboardObjectives.ForEach(o =>
            {
                o.Init(foodFamilyIcons);
                o.onTimeExpired += (family) =>
                {
                    var objective = objectives.FirstOrDefault(obj => obj.family == family);
                    objectives.Remove(objective);
                    RefreshObjectives();

                    if (onObjectiveExpired != null)
                    {
                        var objectivesFamilies = objectives.Select(obj => obj.family).ToList();
                        onObjectiveExpired(family, objectivesFamilies);
                    }
                };
            });
            timer.onExpired += () =>
            {
                
                if (onTimeExpired != null)
                {
                    onTimeExpired();
                }
            };

            foodCount.gameObject.SetActive(false);
            levelLabel.gameObject.SetActive(false);
            foreach(var item in billboardObjectives)
            {
                item.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (timer.isRunning)
            {
                timer.Highligh(timer.isExpiring);
            }
        }

        public void SetLevel(LevelDto level)
        {
            foodCount.text = $"Eat {level.foodToEat - level.foodsEated} more";
            foodCount.gameObject.SetActive(true);
            
            levelLabel.text = $"Level {level.levelIndex}";
            levelLabel.gameObject.SetActive(true);

            objectives.Clear();
            foodsEated = level.foodsEated;
            foodsToEat = level.foodToEat;
            coronaProgress.currentValue = foodsEated;
            coronaProgress.totalValue = foodsToEat;
            timer.SetTimer(level.time);
            timer.Highligh(false);

            StartCoroutine(DelayedObjectives(level.objectives));

            this.gameover.SetActive(false);
            this.youwin.SetActive(false);
        }

        IEnumerator DelayedObjectives(List<ObjectiveDto> dObjectives)
        {
            yield return new WaitForSeconds(10);
            for (int i = 0; i < dObjectives.Count; i++)
            {
                var objective = dObjectives[i];
                objectives.Add(objective);
            }

            RefreshObjectives();
        }

        public void StartTimer()
        {
            foodCount.gameObject.SetActive(true);
            levelLabel.gameObject.SetActive(true);
            //coronaProgress.gameObject.SetActive(true); disabled unitil reviewed

            levelScore.Hide();
            gameScores.Hide();
            timer.StartTimer();
            timer.Highligh(false);
        }

        public void StopTimer()
        {
            timer.StopTimer();
            billboardObjectives.ForEach(o => o.Hide());
        }

        public void AddTime(float time)
        {
            timer.AddTime(time);
        }

        public void GameOver()
        {
            this.gameover.SetActive(true);
            billboardObjectives.ForEach(o => o.Hide());
        }

        public void ShowResults(Dictionary<int, LevelResults> results, bool isNewRecord)
        {
            levelScore.Hide();
            gameScores.ShowResults(results,isNewRecord);
        }

        public void YouWin(LevelResults results,int rate, GameObject bonus = null)
        {
            this.youwin.SetActive(true);
            levelScore.Show();
            levelScore.SetResult(results, rate, bonus);
            billboardObjectives.ForEach(o => o.Hide());
        }

        public void AddFood(Food.FoodFamily family)
        {
            
            foodsEated++;
            coronaProgress.currentValue = foodsEated;

            var objective = objectives.FirstOrDefault(s => s.family == family);
            var index = objectives.IndexOf(objective);

            if (objective != null)
            {
                objective.eated++;
                if (objective.IsCompleted)
                {
                    objectives.Remove(objective);
                    RefreshObjectives();

                    if (onObjectiveCompleted != null)
                    {
                        var objectivesFamilies = objectives.Select(o => o.family).ToList();
                        onObjectiveCompleted(objective.family, objectivesFamilies, objective.bonus);
                    }
                }
                else {
                    RefreshObjectives();
                }
            }

            if(foodsToEat == foodsEated)
            {
                if (onGameCompleted != null)
                {
                    onGameCompleted(timer.GetResidueSeconds());
                }
            }

            foodCount.text = $"Eat {foodsToEat - foodsEated} more";
        }

        public void AddObjective(ObjectiveDto objective) {
            objectives.Insert(0,objective);

            RefreshObjectives();
        }

        void RefreshObjectives() {

            billboardObjectives.ForEach(o => o.Hide());
            for (int i = 0; i < objectives.Count; i++)
            {
                billboardObjectives[i].setValue(objectives[i]);
                billboardObjectives[i].Show();
            }
        }

        public List<ObjectiveDto> GetObjectives()
        {
            return objectives;
        }
    }
}
