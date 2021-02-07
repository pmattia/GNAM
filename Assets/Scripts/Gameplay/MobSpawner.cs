using Assets.Scripts.AI;
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
        [SerializeField] GameObject[] modifierMobs;
        [SerializeField] GameObject[] killerMobs;
        [SerializeField] Transform[] placeholders;
        public event Action OnMobDeath;

        public GameObject[] SpawnMob(int level) {
            var ret = new List<GameObject>();

            var tPlaceholders = new List<Transform>();
            var validPlaceholders = placeholders.Where(p => p.transform.childCount == 0);
            tPlaceholders.AddRange(validPlaceholders);

            if (tPlaceholders.Count() > 0)
            {
                var currentMobCount = FindObjectsOfType<ShootAtTargets>().Length;

                var newModifierMobsCount = 0;
                var newKillerMobsCount = 0;
                //if(level > 2)
                //{
                //    newMobCount = Mathf.FloorToInt(level-2 / (2 + UnityEngine.Random.Range(0, 1)));
                //}

                //if(newMobCount + currentMobCount > level)
                //{
                //    newMobCount = level - currentMobCount;
                //    Debug.Log($"{newMobCount} {level} {currentMobCount}");
                //}

                switch (level)
                {
                    case 0:
                    case 1:
                    case 2:
                        break;
                    case 3:
                        newModifierMobsCount = UnityEngine.Random.Range(1, 3);
                        newKillerMobsCount = 0;
                        break;
                    case 4:
                        newModifierMobsCount = UnityEngine.Random.Range(2, 4);
                        newKillerMobsCount = 1;
                        break;
                    case 5:
                        newModifierMobsCount = UnityEngine.Random.Range(4, 6);
                        newKillerMobsCount = UnityEngine.Random.Range(2, 3);
                        break;
                    case 6:
                        newModifierMobsCount = UnityEngine.Random.Range(5, 7);
                        newKillerMobsCount = UnityEngine.Random.Range(5, 7);
                        break;
                    case 7:
                        newModifierMobsCount = 13;
                        newKillerMobsCount = 0;
                        break;
                    case 8:
                        newModifierMobsCount = 0;
                        newKillerMobsCount = 13;
                        break;
                    default:
                        newModifierMobsCount = UnityEngine.Random.Range(10, 13);
                        newKillerMobsCount = UnityEngine.Random.Range(10, 13);
                        //newMobCount = Mathf.FloorToInt(level - 2 / (2 + UnityEngine.Random.Range(0, 1)));
                        //if (newMobCount + currentMobCount > level)
                        //{
                        //    newMobCount = level - currentMobCount;
                        //    Debug.Log($"{newMobCount} {level} {currentMobCount}");
                        //}
                        break;
                }

                var randomKillers = InstantiateRandomMobsFromList(modifierMobs.ToList(), tPlaceholders, newKillerMobsCount - currentMobCount);
                ret.AddRange(randomKillers);
                currentMobCount = currentMobCount + randomKillers.Count();
                var randomModifiers = InstantiateRandomMobsFromList(killerMobs.ToList(), tPlaceholders, newModifierMobsCount - currentMobCount);
                ret.AddRange(randomModifiers);
                currentMobCount = currentMobCount + randomModifiers.Count();
            }
            return ret.ToArray();
        }

        List<GameObject> InstantiateRandomMobsFromList(List<GameObject> mobs, List<Transform> placeholders, int count)
        {
            var ret = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var mob = mobs[UnityEngine.Random.Range(0, mobs.Count())];
                var placeholder = DrawPlaceholder(placeholders);

                ret.Add(InstantiateNewMob(mob, placeholder));
                placeholders.Remove(placeholder);
            }
            return ret;
        }

        Transform DrawPlaceholder(List<Transform> placeholders)
        {
            var maxPositionToSpawn = placeholders.Count() < 4 ? placeholders.Count() : 4; //defines priority to lower level placeholders
            return placeholders[UnityEngine.Random.Range(0, maxPositionToSpawn)];
        }
        GameObject InstantiateNewMob(GameObject mob, Transform placeholder)
        {
            var instancedMob = Instantiate(mob.gameObject, placeholder.position, Quaternion.identity);
            instancedMob.transform.SetParent(placeholder);
            var shooter = instancedMob.GetComponent<ShootAtTargets>();
            shooter.onDeath += () => { 
                if(OnMobDeath!= null)
                {
                    OnMobDeath();
                }
            };
            return instancedMob;
        }

        public void RemoveMobs()
        {
            foreach (var placeholder in placeholders)
            {
                foreach(Transform child in placeholder.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
