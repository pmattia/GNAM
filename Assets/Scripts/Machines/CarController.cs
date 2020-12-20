using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Machines
{
    public class CarController : MonoBehaviour
    {
        private const string HORIZONTAL = "Horizontal";
        private const string VERTICAL = "Vertial";

        private float horizontalInput;
        private float verticalInput;
        private float currentSteerAngle;
        private int gear = 1;

        [SerializeField]
        private float motorForce;
        [SerializeField]
        private float maxSteerAngle;

        [SerializeField]
        private WheelCollider frontLeftWheelCollider;
        [SerializeField]
        private WheelCollider frontRightWheelCollider;
        [SerializeField]
        private WheelCollider rearLeftWheelCollider;
        [SerializeField]
        private WheelCollider rearRightWheelCollider;

        [SerializeField]
        private Transform frontLeftWheelTransform;
        [SerializeField]
        private Transform frontRightWheelTransform;
        [SerializeField]
        private Transform rearLeftWheelTransform;
        [SerializeField]
        private Transform rearRightWheelTransform;

        [SerializeField]
        private Transform holderTransform;

        public Transform HolderTransform { get { return holderTransform; } }

        public bool CanHoldFood { get { return holderTransform != null && holderTransform.childCount == 0; } }

        private void FixedUpdate()
        {
            GetInput();
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }

        private void GetInput()
        {
            //verticalInput = 0;
            //horizontalInput = 0;
        }

        private void HandleMotor()
        {
            frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
            frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
            rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        }

        private void HandleSteering()
        {
            currentSteerAngle = horizontalInput;
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;
        }

        private void UpdateWheels()
        {
            UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
            UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
            UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
            UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        }

        private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
        {
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }

        public void SetSpeed(float value)
        {

            //verticalInput = -1 * (value - 50f) / 5000f;
            verticalInput = gear * value /50f;
            Debug.Log(value + " | " + verticalInput);
        }

        public void SetSteering(float value)
        {
            horizontalInput = -1 * (value - 50f);
        }

        public void SetGear(float value)
        {
            if(value < 0)
            {
                gear = 1;
            }
            else
            {
                gear = -1;
            }
        }
    }
}
