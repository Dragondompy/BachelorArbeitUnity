using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class facePreview : MonoBehaviour
    {

        public GameObject arrow;
        public bool flipped;
        // Start is called before the first frame update
        void Start()
        {
            flipped = false;
            arrow.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void drawMesh(List<Vertex> verts)
        {
            Vector3[] vertices = getPos(verts);
            if (vertices.Length < 3)
            {
                GetComponent<MeshFilter>().mesh = new Mesh();
                arrow.SetActive(false);
                flipped = false;
                return;
            }

            int highest = 0;
            int[] triangles = new int[(vertices.Length - 2) * 3];
            for (int j = 0; j < vertices.Length - 2; j++)
            {
                triangles[highest + 0] = 0;

                triangles[highest + 1] = j + 1;

                triangles[highest + 2] = j + 2;

                highest += 3;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            GetComponent<MeshFilter>().mesh = mesh;

            arrow.transform.position = getMiddlePoint();
            arrow.transform.rotation = Quaternion.LookRotation(getNormal(), Vector3.up);
            arrow.SetActive(true);
        }

        public Vector3 getNormal()
        {
            Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
            Vector3 normal = new Vector3(0, 0, 0);
            foreach (Vector3 v in normals)
            {
                normal += v;
            }
            normal = normal / normals.Length;

            return normal;
        }

        private Vector3[] getPos(List<Vertex> vertices)
        {
            Vector3[] ver = new Vector3[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                if (flipped)
                {
                    ver[i] = vertices[vertices.Count - 1 - i].getPosition();
                }
                else
                {
                    ver[i] = vertices[i].getPosition();
                }
            }

            return ver;
        }

        public Vector3 getMiddlePoint()
        {
            Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;
            Vector3 middle = new Vector3(0, 0, 0);
            foreach (Vector3 v in vertices)
            {
                middle += v;
            }
            return middle / vertices.Length;
        }
    }
}