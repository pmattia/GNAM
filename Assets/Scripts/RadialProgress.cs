using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class RadialProgress : MonoBehaviour
	{
		public Image LoadingBar;
		public int currentValue;
		public int totalValue;

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			Debug.Log($"{currentValue} {totalValue}");
			if (totalValue > 0)
			{
				float normalValue = (float)currentValue / (float)totalValue;
				Debug.Log($"{currentValue} {totalValue} {normalValue}");
				LoadingBar.fillAmount = normalValue;
			}
		}
	}
}
