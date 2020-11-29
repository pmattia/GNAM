using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public class GnamGrabbable: Grabbable
    {
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
    }
}
