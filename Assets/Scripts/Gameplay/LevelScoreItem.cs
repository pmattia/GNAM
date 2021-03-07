using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Gameplay
{
    public class LevelScoreItem : MonoBehaviour
    {
        [SerializeField] Image marker;
        [SerializeField] Text scoreText;
        [SerializeField] Text numberText;

        public void SetLevelScore(int level, int points)
        {
            numberText.text = level.ToString();

            if(points > 0)
            {
                marker.color = Color.green;
                scoreText.text = points.ToString();
            }
            else
            {
                marker.color = Color.red;
                scoreText.text = string.Empty;
            }
        }
    }
}
