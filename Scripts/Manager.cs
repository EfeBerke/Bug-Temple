using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
namespace Com.EfeBerke.TheConnector
{
    public class Manager : MonoBehaviour
    {
        public string PlayerPrefab;
        public Transform[] SpawnPoint;

        private void Start()
        {
            Spawn();
        }
        public void Spawn()
        {
            Transform TSpawn = SpawnPoint[Random.Range(0, SpawnPoint.Length)];
            PhotonNetwork.Instantiate(PlayerPrefab, TSpawn.position, TSpawn.rotation);

        }
        
    }
}