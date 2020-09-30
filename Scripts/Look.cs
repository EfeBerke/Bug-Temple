using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.EfeBerke.TheConnector
{

    public class Look : MonoBehaviourPunCallbacks
    {
        #region -Variables-

        public static bool cursorlocked = true;

        public Transform player;
        public Transform cams;
        public Transform weapon;

        public float xSensitivity;
        public float ySensitivity;
        public float maxAngle;

        private Quaternion camCenter;

        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            camCenter = cams.localRotation; //Set rotation origin for cameras to camcenter
        }


        void Update()
        {
            if (!photonView.IsMine) return;

            SetY();
            SetX();
            UpdateCursorLock();
        }

        #endregion

        #region -Private Methods-

        void SetY()
        {
            float Tinput = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion Tadj = Quaternion.AngleAxis(Tinput, -Vector3.right);
            Quaternion Tdelta = cams.localRotation * Tadj;

            if (Quaternion.Angle(camCenter, Tdelta) < maxAngle)
            {
                cams.localRotation = Tdelta;
            }
            weapon.rotation = cams.rotation;

        }
        void SetX()
        {
            float Tinput = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion Tadj = Quaternion.AngleAxis(Tinput, Vector3.up);
            Quaternion Tdelta = player.localRotation * Tadj;


            player.localRotation = Tdelta;



        }
        void UpdateCursorLock()
        {
            if (cursorlocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorlocked = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorlocked = true;
                }
            }
        }

        #endregion
        
    }
}