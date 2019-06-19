using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class CameraMovement : MonoBehaviour
    {
        Rigidbody r;
        // Start is called before the first frame update
        void Start()
        {
            r = GetComponent<Rigidbody>();
        }
        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Transform>().position.x < 2.5f)
            {
                r.AddForce(new Vector3(0.1f, 0, 0.1f));
            }
            else
            {
                r.AddForce(new Vector3(-0.1f, 0, -0.1f));
            }
        }

    }
}
