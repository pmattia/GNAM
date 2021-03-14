using Assets.Scripts.Interfaces;
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
        //[SerializeField] Transform bonusHolder; //deprecated
        [SerializeField] SpriteRenderer BonusIcon;
        public event Action<Food.FoodFamily> onTimeExpired;
        float lifeTime = 0;
        float cooldown = 0;
        public Food.FoodFamily Family;

        public void Init(List<Sprite> foodFamilyIcons)
        {
            this.foodFamilyIcons = foodFamilyIcons;
        }

        public void setValue(ObjectiveDto objective)
        {
            cooldown = objective.cooldown;
            Family = objective.family;
            ObjectiveText.text = GetStatsString(objective.family, objective.toEat, objective.eated);
            ObjectiveIcon.sprite = foodFamilyIcons[(int)objective.family];

            var currentBonusIcon = objective.bonus.GetComponent<GnamGrabbable>().Icon;
            if (currentBonusIcon != null)
            {
                BonusIcon.sprite = currentBonusIcon;
            }
            else
            {
                BonusIcon.sprite = null;
            }

            //foreach(Transform child in bonusHolder.transform)
            //{
            //    Destroy(child.gameObject);
            //}

            //var bonus = Instantiate(objective.bonus, bonusHolder);
            //bonus.transform.localPosition= Vector3.zero;
            //bonus.transform.localRotation = Quaternion.identity;
            //bonus.GetComponent<Rigidbody>().isKinematic = true;
        }

        private void Update()
        {
            lifeTime += Time.deltaTime;
            if(lifeTime > cooldown)
            {
                if (onTimeExpired != null)
                {
                    onTimeExpired(Family);
                    lifeTime = 0;
                }
            }
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

            //ret = string.Format("{0} su {1}", eated.ToString(), toEat.ToString());
            ret = string.Format("{0}", (toEat - eated).ToString());

            return ret;
        }
    }
}
