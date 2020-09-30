using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace Com.EfeBerke.TheConnector
{
    public class Sway : MonoBehaviourPunCallbacks
    {

        #region -Variables-
        public float intensity;
        public float smooth;
        public bool ismine;
        private Quaternion OriginRotation;

        #endregion

        #region MonoBehaviour CallBacks

        private void Start()
        {
           
            OriginRotation = transform.localRotation;
        }

        private void Update()
        {
            
            UpdateSway();
        }

        #endregion

        #region Private Methods

        private void UpdateSway()
        {
            //Controls
            float TXMouse = Input.GetAxis("Mouse X");
            float TYMouse = Input.GetAxis("Mouse Y");

            if (!ismine)
            {
                TXMouse = 0;
                TYMouse = 0;
            }

            //Calculate Target Rotation
            Quaternion TXadj = Quaternion.AngleAxis(-intensity * TXMouse,Vector3.up);
            Quaternion TYadj = Quaternion.AngleAxis(intensity * TYMouse, Vector3.right);
            Quaternion TargetRotation = OriginRotation * TXadj * TYadj;

            //Rotate towards target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, TargetRotation, Time.deltaTime * smooth);

        }

        #endregion
    }
}