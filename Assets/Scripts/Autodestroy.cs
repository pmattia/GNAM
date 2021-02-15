﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Autodestroy : MonoBehaviour
    {
        public event Action onDestroy;
        public int Countdown { get; set; }
        float lifetime = 0;

        private void FixedUpdate()
        {
            lifetime += Time.deltaTime;
            if (lifetime > Countdown) {
                if (onDestroy != null)
                {
                    onDestroy();
                }
                Destroy(gameObject);
            }
        }
    }
}
