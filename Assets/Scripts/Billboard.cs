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
        public TextMeshPro levelText;
        public TextMeshPro foodPointStatsText;
        public List<TextMeshPro> objectiveTexts;

        public event Action<Food.FoodFamily> onObjectiveCompleted;
        public event Action<int> onMatchCompleted;

        List<ObjectiveDto> objectives = new List<ObjectiveDto>();
        int foodsEated;

        public class LevelDto
        {
            public int level;
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
            levelText.text = string.Format("LIVELLO {0}",level.level.ToString());
            foodPointStatsText.text = GetFoodPointsString(level.foodsEated);
            foodsEated = level.foodsEated;
            for (int i =0; i< objectiveTexts.Count; i++)
            {
                var objective = level.objectives[i];
                objectiveTexts[i].text = GetStatsString(objective.family, objective.toEat, objective.eated);
                objectiveTexts[i].color = Color.red;
                objectives.Add(objective);
            }
        }

        public void AddFood(Food.FoodFamily family)
        {
            foodsEated++;
            foodPointStatsText.text = GetFoodPointsString(foodsEated);

            var objective = objectives.FirstOrDefault(s => s.family == family);
            var index = objectives.IndexOf(objective);
            if(objective != null)
            {
                objective.eated++;
                if (objective.toEat == objective.eated)
                {
                    if(objective.IsCompleted && onObjectiveCompleted != null)
                    {
                        onObjectiveCompleted(objective.family);
                        if(objectives.All(o => o.IsCompleted) && onMatchCompleted != null)
                        {
                            onMatchCompleted(foodsEated);
                        }
                    }
                }

                var objectiveText = objectiveTexts[index];
                objectiveText.text = GetStatsString(family, objective.toEat, objective.eated);
                objectiveText.color = objective.IsCompleted ? Color.green : Color.red;
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

        string GetFoodPointsString(int eated) {
            return string.Format("CIBI MANGIATI {0}", eated.ToString());
        }
    }
}
