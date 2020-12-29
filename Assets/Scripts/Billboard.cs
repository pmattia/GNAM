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
        public TextMeshPro vegetablesStatsText;
        public TextMeshPro fruitsStatsText;
        public TextMeshPro meatStatsText;
        public TextMeshPro candyStatsText;
        public TextMeshPro carboStatsText;

        public event Action<Food.FoodFamily> onObjectiveCompleted;
        public event Action onMatchCompleted;

        List<ObjectiveDto> objectives = new List<ObjectiveDto>();

        public class LevelDto
        {
            public int level;
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
            foreach(var objective in level.objectives)
            {
                SetStatsString(objective.family, objective.toEat, objective.eated);
                objectives.Add(objective);
            }
        }

        public void AddFood(Food.FoodFamily family)
        {
            var objective = objectives.FirstOrDefault(s => s.family == family);
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
                            onMatchCompleted();
                        }
                    }
                }
                SetStatsString(family, objective.toEat, objective.eated, objective.IsCompleted);
            }
        }

        void SetStatsString(Food.FoodFamily family, int toEat, int eated = 0, bool isCompleted = false)
        {
            TextMeshPro currentText = null;
            switch (family)
            {
                case Food.FoodFamily.Candy:
                    candyStatsText.text = string.Format("DOLCI {0} SU {1}", eated.ToString(), toEat.ToString());
                    currentText = candyStatsText;
                    break;
                case Food.FoodFamily.Carbo:
                    carboStatsText.text = string.Format("CARBOIDRATI {0} SU {1}", eated.ToString(), toEat.ToString());
                    currentText = carboStatsText;
                    break;
                case Food.FoodFamily.Fruit:
                    fruitsStatsText.text = string.Format("FRUTTA {0} SU {1}", eated.ToString(), toEat.ToString());
                    currentText = fruitsStatsText;
                    break;
                case Food.FoodFamily.Meat:
                    meatStatsText.text = string.Format("CARNE {0} SU {1}", eated.ToString(), toEat.ToString());
                    currentText = meatStatsText;
                    break;
                case Food.FoodFamily.Vegetable:
                    vegetablesStatsText.text = string.Format("VERDURA {0} SU {1}", eated.ToString(), toEat.ToString());
                    currentText = vegetablesStatsText;
                    break;
            }
            if (currentText != null && isCompleted)
            {
                currentText.color = Color.green;
            }
        }
    }
}
