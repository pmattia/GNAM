using Assets.Scripts.AI;
using System;
using System.Collections;
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
        public event Action<ShootAtTargets> OnMobDeath;

        public GameObject[] SpawnMob(int level) {
            var ret = new List<GameObject>();

            var tPlaceholders = new List<Transform>();
            var validPlaceholders = placeholders.Where(p => p.transform.childCount == 0);
            tPlaceholders.AddRange(validPlaceholders);

            if (tPlaceholders.Count() > 0)
            {
                var currentMofierMobCount = FindObjectsOfType<ShootAtTargets>().Where(s => s.Type == 0).Count();
                var currentKillerMobCount = FindObjectsOfType<ShootAtTargets>().Where(s => s.Type == 1).Count();

                var newModifierMobsCount = 0;
                var newKillerMobsCount = 0;
             
                switch (level)
                {
                    case 0:
                    case 1:
                    case 2:
                        break;
                    case 3:
                        newModifierMobsCount = 1;
                        newKillerMobsCount = 0;
                        break;
                    case 4:
                        newModifierMobsCount = 2;
                        newKillerMobsCount = 1;
                        break;
                    case 5:
                        newModifierMobsCount = 2;
                        newKillerMobsCount = 2;
                        break;
                    case 6:
                        newModifierMobsCount = 3;
                        newKillerMobsCount = 2;
                        break;
                    case 7:
                        newModifierMobsCount = 4;
                        newKillerMobsCount = 3;
                        break;
                    case 8:
                        newModifierMobsCount = 4;
                        newKillerMobsCount = 4;
                        break;
                    case 9:
                        newModifierMobsCount = 5;
                        newKillerMobsCount = 5;
                        break;
                    case 10:
                        newModifierMobsCount = 7;
                        newKillerMobsCount = 6;
                        break;
                    default:
                        newModifierMobsCount = UnityEngine.Random.Range(10, 13);
                        newKillerMobsCount = UnityEngine.Random.Range(10, 13);
                        break;
                }

                var randomModifiers = InstantiateRandomMobsFromList(modifierMobs.ToList(), tPlaceholders, newModifierMobsCount - currentMofierMobCount, level);
                ret.AddRange(randomModifiers);
                var randomKillers = InstantiateRandomMobsFromList(killerMobs.ToList(), tPlaceholders, newKillerMobsCount - currentKillerMobCount, level);
                ret.AddRange(randomKillers);
            }
            return ret.ToArray();
        }

        public void Party()
        {
            var validPlaceholders = placeholders.Where(p => p.transform.childCount == 0).ToArray();
            for(int i=0; i<validPlaceholders.Count(); i++)
            {
                GameObject mob;
                if(i%2== 0)
                {
                    mob = modifierMobs[UnityEngine.Random.Range(0, modifierMobs.Count())];
                }
                else
                {
                    mob = killerMobs[UnityEngine.Random.Range(0, killerMobs.Count())];
                }

                var instancedMob = Instantiate(mob.gameObject, validPlaceholders[i].position, Quaternion.identity);
                instancedMob.transform.SetParent(validPlaceholders[i]);
                instancedMob.GetComponents<Collider>().ToList().ForEach(c => c.enabled = false);
                instancedMob.GetComponentInChildren<Animator>().SetBool("exult", true);
            }

            StartCoroutine(WaitToStopParty());
        }

        IEnumerator WaitToStopParty()
        {
            yield return new WaitForSeconds(41);
            placeholders.ToList().ForEach(p => { 
                p.GetComponentInChildren<Animator>().SetBool("exult", false);
                p.GetComponentInChildren<Animator>().SetBool("die", true);
                p.GetComponentInChildren<ShootAtTargets>().StopTalking();
            });
        }

        List<GameObject> InstantiateRandomMobsFromList(List<GameObject> mobs, List<Transform> placeholders, int count, int level)
        {
            var ret = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var mob = mobs[UnityEngine.Random.Range(0, mobs.Count())];
                var placeholder = DrawPlaceholder(placeholders);

                ret.Add(InstantiateNewMob(mob, placeholder, level));
                placeholders.Remove(placeholder);
            }
            return ret;
        }

        Transform DrawPlaceholder(List<Transform> placeholders)
        {
            var maxPositionToSpawn = placeholders.Count() < 4 ? placeholders.Count() : 4; //defines priority to lower level placeholders
            return placeholders[UnityEngine.Random.Range(0, maxPositionToSpawn)];
        }
        GameObject InstantiateNewMob(GameObject mob, Transform placeholder, int level)
        {
            var instancedMob = Instantiate(mob.gameObject, placeholder.position, Quaternion.identity);
            instancedMob.transform.SetParent(placeholder);
            instancedMob.GetComponents<Collider>().ToList().ForEach(c => c.enabled = true);
            var shooter = instancedMob.GetComponent<ShootAtTargets>();
            shooter.onDeath += (killedmob) => { 
                if(OnMobDeath!= null)
                {
                    OnMobDeath(killedmob);
                }
            };
            if (level > 7)
            {
                shooter.SetShootingSpeed(ShootingSpeed.Fast);
            }
            else
            {
                shooter.SetShootingSpeed(ShootingSpeed.Standard);
            }
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
