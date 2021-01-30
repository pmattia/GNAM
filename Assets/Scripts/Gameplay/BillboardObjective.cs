using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Gameplay
{
    public class BillboardObjective: MonoBehaviour
    {
        public TextMeshPro ObjectiveText;
        public SpriteRenderer ObjectiveIcon;
        List<Sprite> foodFamilyIcons;

        public void Init(List<Sprite> foodFamilyIcons)
        {
            this.foodFamilyIcons = foodFamilyIcons;
        }

        public void setValue(ObjectiveDto objective)
        {
            ObjectiveText.text = GetStatsString(objective.family, objective.toEat, objective.eated);
            //  objectiveTexts[i].color = Color.red;
            ObjectiveIcon.sprite = foodFamilyIcons[(int)objective.family];
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        string GetStatsString(Food.FoodFamily family, int toEat, int eated = 0, bool isCompleted = false)
        {
            var ret = string.Empty;
            //switch (family)
            //{
            //    case Food.FoodFamily.Candy:
            //        ret = string.Format("DOLCI {0} SU {1}", eated.ToString(), toEat.ToString());
            //        break;
            //    case Food.FoodFamily.Carbo:
            //        ret = string.Format("CARBOIDRATI {0} SU {1}", eated.ToString(), toEat.ToString());
            //        break;
            //    case Food.FoodFamily.Fruit:
            //        ret = string.Format("FRUTTA {0} SU {1}", eated.ToString(), toEat.ToString());
            //        break;
            //    case Food.FoodFamily.Meat:
            //        ret = string.Format("CARNE {0} SU {1}", eated.ToString(), toEat.ToString());
            //        break;
            //    case Food.FoodFamily.Vegetable:
            //        ret = string.Format("VERDURA {0} SU {1}", eated.ToString(), toEat.ToString());
            //        break;
            //}
            ret = string.Format("{0} su {1}", eated.ToString(), toEat.ToString());

            return ret;
        }
    }
}
