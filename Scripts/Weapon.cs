using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace Com.EfeBerke.TheConnector
{
    public class Weapon : MonoBehaviourPunCallbacks
    {

        #region -Variables-

        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject BulletholePrefab;
        public LayerMask canbeShot;

        private float currentcooldown;
        private GameObject currentweapon;
        private int currentIndex;


        #endregion

        #region MonoBehaviour CallBacks


        void Update()
        {
            //if (!photonView.IsMine) return;

            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 1); }
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha2)) { photonView.RPC("Equip", RpcTarget.All, 0); }
            if (currentweapon != null)
            {
                if (photonView.IsMine)
                {

                    Aim(Input.GetMouseButton(1));

                    if (loadout[currentIndex].burst != 1)
                    {
                        if (Input.GetMouseButtonDown(0) && currentcooldown <= 0)
                        {
                            photonView.RPC("Shoot", RpcTarget.All);
                        }
                    }
                    else
                    {
                        if (Input.GetMouseButton(0) && currentcooldown <= 0)
                        {
                            photonView.RPC("Shoot", RpcTarget.All);
                        }
                    }
                    //Cooldown
                    if (currentcooldown > 0) currentcooldown -= Time.deltaTime;
                }
                //Weapon position elasticity
                currentweapon.transform.localPosition = Vector3.Lerp(currentweapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

                

            }

        }

        #endregion

        #region Private Methods
        [PunRPC]
        void Equip(int Pind)
        {
            currentIndex = Pind;

            if (currentweapon != null) Destroy(currentweapon);
            GameObject TnewWeapon = Instantiate(loadout[Pind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            TnewWeapon.transform.localPosition = Vector3.zero;
            TnewWeapon.transform.localEulerAngles = Vector3.zero;
            TnewWeapon.GetComponent<Sway>().ismine = photonView.IsMine;

            currentweapon = TnewWeapon;

        }

        void Aim(bool PisAiming)
        {
            Transform Tanchor = currentweapon.transform.Find("Anchor");
            Transform TStateAds = currentweapon.transform.Find("States/ADS");
            Transform TStatesHip = currentweapon.transform.Find("States/Hip");

            if (PisAiming)
            {
                //Aim
                Tanchor.position = Vector3.Lerp(Tanchor.position, TStateAds.position, Time.deltaTime * loadout[currentIndex].aimspeed);

            }
            else
            {
                //Hip
                Tanchor.position = Vector3.Lerp(Tanchor.position, TStatesHip.position, Time.deltaTime * loadout[currentIndex].aimspeed);

            }
        }
        [PunRPC]
        void Shoot()
        {
            Transform TSpawn = transform.Find("Cameras/Normal Camera");

            //Bloom
            Vector3 TBloom = TSpawn.position + TSpawn.forward * 1000f;
            TBloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * TSpawn.up;
            TBloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * TSpawn.right;
            TBloom -= TSpawn.position;
            TBloom.Normalize();


            //Cooldown
            currentcooldown = loadout[currentIndex].firerate;


            //Raycast
            RaycastHit THit = new RaycastHit();
            if (Physics.Raycast(TSpawn.position, TBloom, out THit, 1000f, canbeShot))
            {
                GameObject TNewhole = Instantiate(BulletholePrefab, THit.point + THit.normal * 0.001f, Quaternion.identity) as GameObject;
                TNewhole.transform.LookAt(THit.point + THit.normal);
                Destroy(TNewhole, 2f);

                if (photonView.IsMine)
                {
                    //Shooting other player on network
                    if (THit.collider.gameObject.layer == 11)
                    {
                        //RPC Call the damage player goes here
                        THit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                    }
                }


            }

            //Gun FX

                currentweapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
                currentweapon.transform.position -= currentweapon.transform.forward * loadout[currentIndex].kickback;
            
        }
        [PunRPC]
        private void TakeDamage(int PDamage)
        {
            GetComponent<Motion>().TakeDamage(PDamage);
        }

        #endregion
    }
}