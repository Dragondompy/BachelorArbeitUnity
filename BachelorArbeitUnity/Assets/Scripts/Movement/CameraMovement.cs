using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class CameraMovement : MonoBehaviour
    {
        public float pitch;
        public float yaw;
        // Start is called before the first frame update
        void Start()
        {
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                pitch += -1 * Input.GetAxis("Mouse Y");
                yaw += Input.GetAxis("Mouse X");

                pitch = Mathf.Clamp(pitch, -90f, 90f);
                transform.rotation = Quaternion.Euler(pitch, yaw, 0);

                Vector3 direction = new Vector3(0, 0, 0);
                if (Input.GetAxis("Vertical") != 0)
                {
                    direction += Vector3.forward * Input.GetAxis("Vertical");
                }
                if (Input.GetAxis("Horizontal") != 0)
                {
                    direction += Vector3.right * Input.GetAxis("Horizontal");
                }
                if (Input.GetAxis("FlyUp") != 0)
                {
                    direction += Vector3.up * Input.GetAxis("FlyUp");
                }
                direction.Normalize();
                transform.Translate(InformationHolder.camSpeed * Time.deltaTime * direction);
                //transform.eulerAngles(InformationHolder.camRotSpeed*())
            }
            else
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }
    }
}
