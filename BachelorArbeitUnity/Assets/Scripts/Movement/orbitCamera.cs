using UnityEngine;

namespace BachelorArbeitUnity
{
    public class orbitCamera : MonoBehaviour
    {
        public float distanceMin;
        public float distanceMax;
        public float pitch;
        public float yaw;
        public float zoom;
        public GameObject cam;
        // Use this for initialization

        private void Start()
        {
            zoom = 10;
            distanceMin = 1f;
            distanceMax = 100f;
        }

        void LateUpdate()
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;

                pitch += -1 * Input.GetAxis("Mouse Y");
                yaw += Input.GetAxis("Mouse X");
                yaw *= InformationHolder.camRotSpeed;
                pitch = Mathf.Clamp(pitch * InformationHolder.camRotSpeed, -90f, 90f);

                transform.rotation = Quaternion.Euler(pitch, yaw, 0);

                /*Vector3 direction = new Vector3(0, 0, 0);
                if (Input.GetAxis("Vertical") != 0)
                {
                    direction.x = Input.GetAxis("Vertical");
                }
                if (Input.GetAxis("Horizontal") != 0)
                {
                    direction.z = Input.GetAxis("Horizontal");
                }
                direction.Normalize();
                transform.Translate(InformationHolder.camSpeed * Time.deltaTime * direction);
                */
            }
            else if (Input.GetMouseButton(2))
            {
                Cursor.lockState = CursorLockMode.Locked;

                zoom -= Input.GetAxis("Mouse Y") * InformationHolder.camSpeed * 0.2f;
                zoom = Mathf.Clamp(zoom, distanceMin, distanceMax);

                cam.transform.localPosition = new Vector3(0, 0, -zoom);
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