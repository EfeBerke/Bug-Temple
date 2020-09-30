using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Com.EfeBerke.TheConnector
{
    public class Motion : MonoBehaviourPunCallbacks
    {
        #region -Variables-

        public float speed;
        public float sprintmodifier;
        public float Jumpforce;
        //public float jetwait;
        //public float jetrecovery;
        //public float maxfuel;
        //public float jetpackforce; //efeberke
        public float MaxHealth;

        public Camera normalCam;
        public GameObject cameraparent;
        public Transform groundDetector;
        public LayerMask ground;

        private Rigidbody r;
        private Transform UIHealthBar;
        //private Transform UIFuelBar;
        private float baseFOV;
        private float sprintFOVModifier = 1.5f;
        private float CurrentHealth;
        //private float CurrentFuel;
        //private float CurrentRecovery;
        private Manager manager;
        //private bool canJet;

        #endregion

        #region MonoBehaviour Callbacks

        private void Start()
        {
            manager = GameObject.Find("Manager").GetComponent<Manager>();
            CurrentHealth = MaxHealth;
            //CurrentFuel = maxfuel;
            cameraparent.SetActive(photonView.IsMine);
            if (!photonView.IsMine)gameObject.layer=11;
            baseFOV = normalCam.fieldOfView;
            if(Camera.main)Camera.main.enabled = false;

            r = GetComponent<Rigidbody>();

            if (photonView.IsMine)
            {
                UIHealthBar = GameObject.Find("HUD/Health/Bar").transform;
                //UIFuelBar = GameObject.Find("HUD/Fuel/Bar2").transform;
                RefreshHealtbar();
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            //Axles
            float hmove = Input.GetAxisRaw("Horizontal");
            float vmove = Input.GetAxisRaw("Vertical");

            //Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position, Vector3.down, 0.1f, ground);
            bool isJumping = jump && isGrounded;
            bool isSprinting = sprint && vmove > 0 && !isJumping && isGrounded; //If you want add not sprint while jumping add (&& !isJumping)
            

            //Jetpack //efeberke
            
            //Jumping
            if (isJumping)
            {
                r.AddForce(Vector3.up * Jumpforce);
                //CurrentRecovery = 0f;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                TakeDamage(30);
            }

            RefreshHealtbar();
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            //Axles
            float hmove = Input.GetAxisRaw("Horizontal");
            float vmove = Input.GetAxisRaw("Vertical");

            //Controls
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            //bool jet = Input.GetKey(KeyCode.Space);

            //States
            bool isGrounded = Physics.Raycast(groundDetector.position,Vector3.down,0.1f,ground);
            bool isJumping = jump && isGrounded; 
            bool isSprinting = sprint && vmove > 0 && !isJumping && isGrounded; //If you want add not sprint while jumping add (&& !isJumping)

            //Movement
            Vector3 direction = new Vector3(hmove,0, vmove);
            direction.Normalize();

            float TadjustedSpeed = speed;
            if (isSprinting) TadjustedSpeed *= sprintmodifier;
            Vector3 TargetVelocity = transform.TransformDirection(direction) * TadjustedSpeed * Time.deltaTime;
            TargetVelocity.y = r.velocity.y;
            r.velocity = TargetVelocity;

            //Field of View
            if (isSprinting) { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView,baseFOV * sprintFOVModifier,Time.deltaTime * 8f); }
            else { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV , Time.deltaTime * 8f); }

            /*
            //Jetpack
            if (jump && !isGrounded)
            {
                canJet = true;
            }
            if (isGrounded)
            {
                canJet = false; //false
            }
            if (canJet && jet && CurrentFuel > 0)
            {
                r.AddForce(Vector3.up * jetpackforce * Time.fixedDeltaTime /*ForceMode.Acceleration);
                CurrentFuel = Mathf.Max(0, CurrentFuel - Time.fixedDeltaTime);
            }
            if (isGrounded)
            {
                if (CurrentRecovery < jetwait)
                {
                    CurrentRecovery = Mathf.Min(jetwait, CurrentRecovery + Time.fixedDeltaTime);
                }
                else
                {
                    CurrentFuel = Mathf.Min(maxfuel, CurrentFuel + Time.fixedDeltaTime * jetrecovery);
                }
            }
            UIFuelBar.localScale = new Vector3(CurrentFuel / maxfuel, 1, 1);

*/


        }
        #endregion

        #region -Public methods-
        
        public void TakeDamage(int PDamage)
        {
            if (photonView.IsMine)
            {
                CurrentHealth -= PDamage;
                RefreshHealtbar();

                if (CurrentHealth <= 0)
                {
                    manager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                }

            }

            
        }

        #endregion

        #region -Private methods-

        private void RefreshHealtbar()
        {
            float THealthRatio = (float)CurrentHealth / (float)MaxHealth;
            UIHealthBar.localScale = Vector3.Lerp(UIHealthBar.localScale, new Vector3(THealthRatio,1,1),Time.deltaTime * 8f);
        }



        #endregion
    }
}