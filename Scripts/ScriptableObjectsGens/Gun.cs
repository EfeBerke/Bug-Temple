using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Com.EfeBerke.TheConnector
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string namee;

        public int damage;
        public int burst; // | 0 semi | 1 auto | 2+ burst fire |

        public float firerate;
        public float bloom;
        public float recoil;
        public float kickback;
        public float aimspeed;

        public GameObject prefab;

    }
}