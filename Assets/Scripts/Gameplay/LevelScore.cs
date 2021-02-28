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
        [SerializeField] Transform bonusHolder;

        public void SetResult(int score, int rate, GameObject bonus = null)
        {
            scoreText.text = $"Score {score}";
            for(int i = 0; i< stars.Count; i++)
            {
                //stars[i].SetActive(rate > i);
                stars[i].SetActive(false);
            }

            foreach (Transform child in bonusHolder.transform)
            {
                Destroy(child.gameObject);
            }

            if (bonus != null)
            {
                bonusHolder.gameObject.SetActive(true);
                var holdedBonus = Instantiate(bonus, bonusHolder);
                holdedBonus.transform.localPosition = Vector3.zero;
                holdedBonus.transform.localRotation = Quaternion.identity;
                holdedBonus.GetComponent<Rigidbody>().isKinematic = true;
            }
            else
            {
                bonusHolder.gameObject.SetActive(false);
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
    }

}
