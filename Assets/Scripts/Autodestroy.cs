using System;
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
        [SerializeField] int cooldown = 4;
        public int Countdown { get { return cooldown; } set { cooldown = value; } }
        float lifetime = 0;

        private void FixedUpdate()
        {
            lifetime += Time.deltaTime;
            if (lifetime > cooldown) {
                var particle = Resources.Load<GameObject>("GnamAutodestroy");
                Instantiate(particle, transform.position, transform.rotation);

                if (onDestroy != null)
                {
                    onDestroy();
                }
                Destroy(gameObject);
            }
        }
    }
}
