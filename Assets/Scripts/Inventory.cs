using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory: MonoBehaviour
    {
        List<GameObject> ownedObject = new List<GameObject>();
        public List<GameObject> OwnedObject { get { return ownedObject; } }
        public virtual void OnSnap(Grabbable gnabbable)
        {
            var autodestroyer = gnabbable.GetComponent<Autodestroy>();
            if (autodestroyer != null)
            {
                Destroy(autodestroyer);
            }

            ownedObject.Add(gnabbable.gameObject);
           // ownedObject.ForEach(o => Debug.Log($"inventory {gnabbable.gameObject.name}"));

            ownedObject = ownedObject.Where(o => o != null).ToList();
        }

        public virtual void OnDetach(Grabbable gnabbable)
        {
            ownedObject.Remove(gnabbable.gameObject);
          //  Debug.Log($"inventory {gnabbable.gameObject.name}");

            ownedObject = ownedObject.Where(o => o != null).ToList();
        }

        public bool CheckObjectExistance<T>() { 
            foreach(var item in ownedObject)
            {
                var itemExistance = item.GetComponent<T>();
                var childExistance = item.GetComponentInChildren<T>();

                if(itemExistance != null || childExistance != null)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
