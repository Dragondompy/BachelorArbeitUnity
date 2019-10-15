using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class ObjLoader
    {
        private List<int> splitToNotSplitVertices;
        private List<int> splitToNotSplitFaces;
        public Mesh newLoad(ObjMesh o)
        {
            Debug.Log("DONT USE");
            List<List<int>> faces = o.getFaces();
            List<Vector3> oldVertices = o.getVertices();
            List<Vector3> vertices = new List<Vector3>();
            splitToNotSplitVertices = new List<int>();
            splitToNotSplitFaces = new List<int>();
            int[] triangles = new int[calcTri(faces) * 3];
            List<Color> colors = new List<Color>();

            int highest = 0;
            for (int i = 0; i < faces.Count; i++)
            {
                if (faces[i].Count > 2)
                {
                    List<int> faceVerticesNewIndex = new List<int>();
                    for (int k = 0; k < faces[i].Count; k++)
                    {
                        if (k == 0)
                        {
                            colors.Add(new Color(1, 0, 0, 1));
                        }
                        else if (k == 1 || k == faces[i].Count - 1)
                        {
                            colors.Add(new Color(1, 1, 0, 1));
                        }
                        else
                        {
                            colors.Add(new Color(0, 1, 0, 1));

                        }
                        vertices.Add(oldVertices[faces[i][k]]);
                        splitToNotSplitVertices.Add(faces[i][k]);
                        faceVerticesNewIndex.Add(vertices.Count - 1);
                    }

                    for (int j = 0; j < faces[i].Count - 2; j++)
                    {
                        splitToNotSplitFaces.Add(i);
                        triangles[highest + 0] = faceVerticesNewIndex[0];

                        triangles[highest + 1] = faceVerticesNewIndex[j + 1];

                        triangles[highest + 2] = faceVerticesNewIndex[j + 2];

                        highest += 3;
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = getArrayfromList(vertices);
            mesh.colors = getArrayfromList(colors);
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }
        public Mesh newLoad(MeshStruct m)
        {
            List<Face> faces = m.getFaces();
            List<Vertex> oldVertices = m.getVertices();
            List<Vector3> vertices = new List<Vector3>();
            splitToNotSplitVertices = new List<int>();
            splitToNotSplitFaces = new List<int>();
            int[] triangles = new int[calcTri(faces) * 3];
            List<Color> colors = new List<Color>();

            int highest = 0;
            for (int i = 0; i < faces.Count; i++)
            {
                Face f = faces[i];
                List<Vertex> verts = f.getVertices();
                if (verts.Count > 2)
                {
                    List<int> faceVerticesNewIndex = new List<int>();
                    for (int k = 0; k < verts.Count; k++)
                    {
                        if (verts.Count > 3)
                        {
                            if (k == 0)
                            {
                                colors.Add(new Color(1, 0, 0, 1));
                            }
                            else if (k == 1 || k == verts.Count - 1)
                            {
                                colors.Add(new Color(1, 1, 0, 1));
                            }
                            else
                            {
                                colors.Add(new Color(0, 1, 0, 1));
                            }
                        }
                        else
                        {
                            if (k == 0)
                            {
                                colors.Add(new Color(1, 1, 0, 1));
                            }
                            else if (k == 1)
                            {
                                colors.Add(new Color(1, 0, 1, 1));
                            }
                            else
                            {
                                colors.Add(new Color(0, 1, 1, 1));
                            }
                        }

                        vertices.Add(verts[k].getPosition());
                        splitToNotSplitVertices.Add(verts[k].getHandleNumber());
                        faceVerticesNewIndex.Add(vertices.Count - 1);
                    }

                    for (int j = 0; j < verts.Count - 2; j++)
                    {
                        splitToNotSplitFaces.Add(f.getHandleNumber());
                        triangles[highest + 0] = faceVerticesNewIndex[0];

                        triangles[highest + 1] = faceVerticesNewIndex[j + 1];

                        triangles[highest + 2] = faceVerticesNewIndex[j + 2];

                        highest += 3;
                    }
                }
            }
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = getArrayfromList(vertices);
            mesh.colors = getArrayfromList(colors);
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

        private Color[] getArrayfromList(List<Color> colors)
        {
            Color[] col = new Color[colors.Count];

            for (int i = 0; i < colors.Count; i++)
            {
                col[i] = colors[i];
            }

            return col;
        }

        //Depricated
        public Mesh load(ObjMesh o)
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

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private int calcTri(List<Face> faces)
        {
            int count = 0;
            foreach (Face f in faces)
            {
                if (f.getVertices().Count > 2)
                {
                    count += f.getVertices().Count - 2;
                }
            }
            return count;
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
            return count;
        }

        public void setSplitToNotSplitVertices(List<int> s)
        {
            splitToNotSplitVertices = s;
        }

        public List<int> getSplitToNotSplitVertices()
        {
            return splitToNotSplitVertices;
        }

        public void setSplitToNotSplitFaces(List<int> s)
        {
            splitToNotSplitFaces = s;
        }

        public List<int> getSplitToNotSplitFaces()
        {
            return splitToNotSplitFaces;
        }
    }
}