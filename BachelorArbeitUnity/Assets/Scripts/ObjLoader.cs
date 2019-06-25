using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class ObjLoader
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public UnityEngine.Mesh newLoad(ObjMesh o)
        {
            List<List<int>> faces = o.getFaces();
            List<Vector3> oldVertices = o.getVertices();
            List<Vector3> vertices = new List<Vector3>();
            List<int> splitToNotSplitVertices = new List<int>();
            int[] triangles = new int[calcTri(faces) * 3];

            int highest = 0;
            for (int i = 0; i < o.getFaces().Count; i++)
            {
                if (faces[i].Count > 2)
                {
                    List<int> faceVerticesNewIndex = new List<int>();
                    for (int k = 0; k < faces[i].Count; k++)
                    {
                        vertices.Add(oldVertices[faces[i][k]]);
                        splitToNotSplitVertices.Add(faces[i][k]);
                        faceVerticesNewIndex.Add(vertices.Count - 1);
                    }

                    for (int j = 0; j < faces[i].Count - 2; j++)
                    {
                        triangles[highest + 0] = faceVerticesNewIndex[0];

                        triangles[highest + 1] = faceVerticesNewIndex[j + 1];

                        triangles[highest + 2] = faceVerticesNewIndex[j + 2];

                        highest += 3;
                    }
                }
            }

            InformationHolder.splitToNotSplitVertices = splitToNotSplitVertices;

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            mesh.vertices = getArrayfromList(vertices);

            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private Vector3[] getArrayfromList(List<Vector3> vertices)
        {
            Vector3[] ver = new Vector3[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                ver[i] = vertices[i];
            }

            return ver;
        }

        //Depricated
        public UnityEngine.Mesh load(ObjMesh o)
        {
            List<List<int>> faces = o.getFaces();

            Vector3[] vertices = new Vector3[o.getVertices().Count];
            //Vector2[] uv = new Vector2[o.getVertices().Count];
            int[] triangles = new int[calcTri(faces) * 3];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = o.getVertices()[i];
                //uv[i] = new Vector2(0, 0);
            }

            int highest = 0;
            for (int i = 0; i < o.getFaces().Count; i++)
            {
                if (faces[i].Count > 2)
                {
                    for (int j = 0; j < faces[i].Count - 2; j++)
                    {
                        triangles[highest + 0] = faces[i][0];
                        triangles[highest + 1] = faces[i][j + 1];
                        triangles[highest + 2] = faces[i][j + 2];
                        highest += 3;
                    }
                }
            }

            UnityEngine.Mesh mesh = new UnityEngine.Mesh();
            mesh.vertices = vertices;
            //mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private int calcTri(List<List<int>> faces)
        {
            int count = 0;
            foreach (List<int> f in faces)
            {
                if (f.Count > 2)
                {
                    count += f.Count - 2;
                }
            }
            Debug.Log(count);
            return count;
        }
    }
}