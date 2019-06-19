using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class gridcreate : MonoBehaviour
    {

        public GameObject testSphere;
        public int x;
        public int z;
        // Start is called before the first frame update
        void Start()
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    Instantiate(testSphere, new Vector3(i / 10, 0, j / 10), Quaternion.identity);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}