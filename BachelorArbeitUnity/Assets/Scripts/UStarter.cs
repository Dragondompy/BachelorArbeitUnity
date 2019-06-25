using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class UStarter : MonoBehaviour
    {
        public string objName;
        public GameObject MeshObject;
        public Material material;
        Camera cam;
        BachelorArbeitUnity.Mesh myMesh;

        // Start is called before the first frame update
        void Start()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);
            cam = GetComponent<Camera>();
            ObjMesh o = new ObjMesh("./Assets/Meshes/" + objName + ".obj");

            MeshOB.GetComponent<MeshFilter>().mesh = new ObjLoader().load(o);

            MeshOB.AddComponent<MeshCollider>();

            MeshOB.GetComponent<MeshRenderer>().material = material;

            myMesh = MeshOB.AddComponent<BachelorArbeitUnity.Mesh>();
            myMesh.loadMeshFromObj(o);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out hit, 100.0f))
                {
                    print("nothing");
                    return;
                }
                MeshCollider meshCollider = hit.collider as MeshCollider;
                if (meshCollider == null || meshCollider.sharedMesh == null)
                {
                    print(meshCollider);
                    return;
                }

                UnityEngine.Mesh mesh = meshCollider.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                int[] triangles = mesh.triangles;
                Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
                Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
                Transform hitTransform = hit.collider.transform;

                Vector3 impactPoint = hit.point;
                
                p0 = hitTransform.TransformPoint(p0);
                p1 = hitTransform.TransformPoint(p1);
                p2 = hitTransform.TransformPoint(p2);

                float d0 = (impactPoint - p0).magnitude;
                float d1 = (impactPoint - p1).magnitude;
                float d2 = (impactPoint - p2).magnitude;

                if (d0 < d1 && d0 < d2)
                {
                    myMesh.selectVertexAt(triangles[hit.triangleIndex * 3 + 0]);
                }
                else if (d1 < d0 && d1 < d2)
                {
                    myMesh.selectVertexAt(triangles[hit.triangleIndex * 3 + 1]);
                }
                else if (d2 < d0 && d2 < d1)
                {
                    myMesh.selectVertexAt(triangles[hit.triangleIndex * 3 + 2]);
                }

                Debug.DrawLine(p0, p1);
                Debug.DrawLine(p1, p2);
                Debug.DrawLine(p2, p0);
            }

        }

        void testProgramm()
        {
        }
    }
}