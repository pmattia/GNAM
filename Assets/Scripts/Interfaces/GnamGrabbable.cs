using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamGrabbable : Grabbable
    {
        [SerializeField] bool dontDestroyOnDrop = false;
        [SerializeField] public Sprite Icon;
        public GnamGrabbable() : base()
        {
            base.GrabButton = GrabButton.Grip;
            base.Grabtype = HoldType.HoldDown;
            base.GrabMechanic = GrabType.Snap;
            base.GrabPhysics = GrabPhysics.PhysicsJoint;
            base.CollisionSpring = 1000;
            base.RemoteGrabbable = false;
            base.RemoteGrabDistance = 0;
            base.ParentToHands = true;
            base.SnapHandModel = true;
            this.BreakDistance = 0.2f;
        }

        public override void GrabItem(Grabber grabbedBy)
        {
            base.GrabItem(grabbedBy);
            var autodestroyer = gameObject.GetComponent<Autodestroy>();
            if (autodestroyer != null) 
            {
                Destroy(autodestroyer);
            }
        }

        public override void DropItem(Grabber droppedBy)
        {
            base.DropItem(droppedBy);

            transform.SetParent(null);

            var autodestroyer = gameObject.GetComponent<Autodestroy>();
            if (autodestroyer == null && !dontDestroyOnDrop)
            {
                autodestroyer = gameObject.AddComponent<Autodestroy>();
                autodestroyer.Countdown = 4;
            }
        }
    }
}
