using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class Billboard : MonoBehaviour
    {
        public TextMeshPro foodPointStatsText;
        public List<TextMeshPro> objectiveTexts;
        public List<SpriteRenderer> objectiveIcons;
        public List<Sprite> foodFamilyIcons;
        public GameObject gameover;
        public GameObject youwin;

        public event Action<Food.FoodFamily, List<Food.FoodFamily>> onObjectiveCompleted;
        public event Action onGameCompleted;

        List<ObjectiveDto> objectives = new List<ObjectiveDto>();
        int foodsEated;
        int foodsToEat;

        public class LevelDto
        {
            public int foodToEat;
            public int foodsEated;
            public List<ObjectiveDto> objectives = new List<ObjectiveDto>();
        }
        public class ObjectiveDto
        {
            public Food.FoodFamily family;
            public int toEat;
            public int eated;
            public bool IsCompleted
            {
                get
                {
                    return eated >= toEat && toEat > 0;
                }
            }
        }

        public void SetLevel(LevelDto level)
        {
            objectives.Clear();
            foodPointStatsText.text = GetFoodPointsString(level.foodToEat, level.foodsEated);
            foodsEated = level.foodsEated;
            foodsToEat = level.foodToEat;
            for (int i =0; i< level.objectives.Count; i++)
            {
                var objective = level.objectives[i];
                objectives.Add(objective);
            }

            RefreshObjectives();

            this.gameover.SetActive(false);
            this.youwin.SetActive(false);
            objectiveTexts.ForEach(o => o.gameObject.SetActive(true));
            objectiveIcons.ForEach(o => o.gameObject.SetActive(true));
            foodPointStatsText.gameObject.SetActive(true);

        }

        public void GameOver()
        {
            this.gameover.SetActive(true);
            objectiveTexts.ForEach(o => o.gameObject.SetActive(false));
            objectiveIcons.ForEach(o => o.gameObject.SetActive(false));
            foodPointStatsText.gameObject.SetActive(false);
        }

        public void YouWin()
        {
            this.youwin.SetActive(true);
            objectiveTexts.ForEach(o => o.gameObject.SetActive(false));
            objectiveIcons.ForEach(o => o.gameObject.SetActive(false));
            foodPointStatsText.gameObject.SetActive(false);
        }

        public void AddFood(Food.FoodFamily family)
        {
            
            foodsEated++;
            foodPointStatsText.text = GetFoodPointsString(foodsToEat,foodsEated);

            var objective = objectives.FirstOrDefault(s => s.family == family);
            var index = objectives.IndexOf(objective);

            Debug.Log($"{family} obj-> {index}");

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
                        onObjectiveCompleted(objective.family, objectivesFamilies);
                    }
                }
                else {
                    RefreshObjectives();
                }
            }

            Debug.Log($"FINE {foodsToEat} - {foodsEated}");
            if(foodsToEat == foodsEated)
            {
                if (onGameCompleted != null)
                {
                    onGameCompleted();
                }
            }
        }

        public void AddObjective(ObjectiveDto objective) {
            objectives.Insert(0,objective);

            RefreshObjectives();
        }

        void RefreshObjectives() {
            for (int i = 0; i < objectives.Count; i++)
            {
                var objective = objectives[i];
                objectiveTexts[i].text = GetStatsString(objective.family, objective.toEat, objective.eated);
                objectiveTexts[i].color = Color.red;

                objectiveIcons[i].sprite = foodFamilyIcons[(int)objective.family];
            }
        }

        string GetStatsString(Food.FoodFamily family, int toEat, int eated = 0, bool isCompleted = false)
        {
            var ret = string.Empty;
            switch (family)
            {
                case Food.FoodFamily.Candy:
                    ret = string.Format("DOLCI {0} SU {1}", eated.ToString(), toEat.ToString());
                    break;
                case Food.FoodFamily.Carbo:
                    ret = string.Format("CARBOIDRATI {0} SU {1}", eated.ToString(), toEat.ToString());
                    break;
                case Food.FoodFamily.Fruit:
                    ret = string.Format("FRUTTA {0} SU {1}", eated.ToString(), toEat.ToString());
                    break;
                case Food.FoodFamily.Meat:
                    ret = string.Format("CARNE {0} SU {1}", eated.ToString(), toEat.ToString());
                    break;
                case Food.FoodFamily.Vegetable:
                    ret = string.Format("VERDURA {0} SU {1}", eated.ToString(), toEat.ToString());
                    break;
            }

            return ret;
        }

        string GetFoodPointsString(int toEat, int eated) {
            return string.Format("CIBI DA MANGIARE {0} SU {1}", eated.ToString(), toEat.ToString());
        }
    }
}
