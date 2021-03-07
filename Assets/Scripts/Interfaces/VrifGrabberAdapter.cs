using Assets.Scripts.ScriptableObjects;
using BNG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Interfaces
{
    public class VrifGrabberAdapter : IGrabber
    {
        Grabber _grabber;
        public VrifGrabberAdapter(Grabber grabber)
        {
            _grabber = grabber;
        }
        public bool ForceGrab { 
            get {
                return _grabber.ForceGrab;
            }
            set {
                _grabber.ForceGrab = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _grabber.enabled;
            }
            set
            {
                _grabber.enabled = value;
            }
        }

        public GnamGrabbable HeldGrabbable
        {
            get {
                return _grabber.HeldGrabbable as GnamGrabbable;
            }
        }

        public void TryRelease()
        {
            _grabber.TryRelease();
        }
    }
}
