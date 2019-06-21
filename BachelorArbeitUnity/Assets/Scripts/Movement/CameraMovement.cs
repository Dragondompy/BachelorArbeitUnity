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
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    GameObject gHit = hit.transform.gameObject.transform.parent.gameObject;
                    if(gHit.GetComponent<Edge>())
                        Debug.Log("You selected an Edge");
                    if (gHit.GetComponent<Vertex>())
                        Debug.Log("You selected an Vertex");
                    if (gHit.GetComponent<Face>())
                        Debug.Log("You selected an Face");
                }
                else {
                    Debug.Log("nothing");
                }
            }
        }
    }
}
