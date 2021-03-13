using Assets.Scripts.Interfaces;
using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory : MonoBehaviour
    {
        List<SnapZone> snapZones = new List<SnapZone>();
        List<SnapZone> EmptySnapZones {
            get
            {
                return snapZones.Where(s => s.HeldItem == null).ToList();
            }
        }
        List<GameObject> ownedObject = new List<GameObject>();
        public List<GameObject> OwnedObject { get { return ownedObject; } }
        public event Action<GnamGrabbable> onSnap;
        public event Action<GnamGrabbable> onDetach;

        private void Awake()
        {
            snapZones = GetComponentsInChildren<SnapZone>().ToList();
        }

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

            if (onSnap != null)
            {
                onSnap(gnabbable as GnamGrabbable);
            }
        }

        public virtual void OnDetach(Grabbable gnabbable)
        {
            ownedObject.Remove(gnabbable.gameObject);
            //  Debug.Log($"inventory {gnabbable.gameObject.name}");

            ownedObject = ownedObject.Where(o => o != null).ToList();

            if (onDetach != null)
            {
                onDetach(gnabbable as GnamGrabbable);
            }
        }

        public bool CheckObjectExistance<T>() {
            foreach (var item in ownedObject)
            {
                var itemExistance = item.GetComponent<T>();
                var childExistance = item.GetComponentInChildren<T>();

                if (itemExistance != null || childExistance != null)
                {
                    return true;
                }
            }

            return false;
        }

        public void HideItems()
        {
            snapZones.ForEach(s => s.gameObject.SetActive(false));
        }

        public void ShowItems()
        {
            snapZones.ForEach(s => s.gameObject.SetActive(true));
        }

        public void SnapEmptyHolders(GnamGrabbable gnabbable)
        {
            EmptySnapZones.ForEach(s => {
                var clone = Instantiate(gnabbable.gameObject);
                clone.transform.localPosition = s.transform.localPosition;
                clone.transform.localRotation = s.transform.localRotation;
                if (s.HeldItem == null)
                {
                    s.GrabGrabbable(clone.GetComponent<GnamGrabbable>());
                }
            });
        }
    }
}
