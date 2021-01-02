using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class MobSpawner: MonoBehaviour
    {
        [SerializeField] GameObject[] mobs;
        [SerializeField] Transform[] placeholders;

        public GameObject[] SpawnMob(int level) {
            var ret = new List<GameObject>();

            var tPlaceholders = new List<Transform>();
            var validPlaceholders = placeholders.Where(p => p.transform.childCount == 0);
            tPlaceholders.AddRange(validPlaceholders);

            if (tPlaceholders.Count() > 0)
            {
                for (int i = 0; i < level; i++)
                {
                    var mob = mobs[UnityEngine.Random.Range(0, mobs.Length)];

                    var placeholder = tPlaceholders[UnityEngine.Random.Range(0, tPlaceholders.Count())];
                    var instancedMob = Instantiate(mob.gameObject, placeholder.position, Quaternion.identity);
                    instancedMob.transform.SetParent(placeholder);
                    ret.Add(instancedMob);
                    tPlaceholders.Remove(placeholder);

                }
            }
            return ret.ToArray();
        }
    }
}
