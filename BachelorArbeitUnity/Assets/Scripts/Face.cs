using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Face
    {
        private List<Vertex> vertices;
        private List<Edge> edges;
        private List<HalfEdge> halfEdges;
        private List<Vertex> innerVertices;
        private Face symFace;

        private int handleNumber;
        private MeshStruct mesh;
        private GameObject errorMess;

        public Face(MeshStruct m)
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            innerVertices = new List<Vertex>();

            setMesh(m);
        }

        public Face(String isEmpty)
        {
            if (isEmpty.Equals("empty"))
            {
                handleNumber = -1;
            }
        }

        //adds vertex to this face
        public void addVertex(Vertex v)
        {
            vertices.Add(v);
        }

        public void addVertex(int v)
        {
            addVertex(mesh.getVertexAt(v));
        }

        public void addInnerVertex(Vertex v)
        {
            innerVertices.Add(v);
        }

        public int containsVertex(int p)
        {
            foreach (Vertex v in vertices)
            {
                if (v.getHandleNumber() == p)
                {
                    return p;
                }
            }
            return -1;
        }

        //adds edge to this face
        public void addEdge(Edge e)
        {
            edges.Add(e);
        }

        public Vector3[] getVertexPositions()
        {
            Vector3[] pos = new Vector3[vertices.Count];
            int i = 0;
            foreach (Vertex v in vertices)
            {
                pos[i] = v.getPosition();
                i++;
            }
            return pos;
        }

        public Vector3 getMiddle()
        {
            Vector3 middle = new Vector3(0, 0, 0);
            Vector3[] vertices = getVertexPositions();
            foreach (Vector3 v in vertices)
            {
                middle += v;
            }
            return middle / vertices.Length;
        }

        public float getSize()
        {
            Vector3 maxholder = new Vector3(0, 0, 0);
            Vector3 minholder = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            foreach (Vector3 v in getVertexPositions())
            {
                maxholder.x = Mathf.Max(maxholder.x, v.x);
                maxholder.y = Mathf.Max(maxholder.y, v.y);
                maxholder.z = Mathf.Max(maxholder.z, v.z);

                minholder.x = Mathf.Min(minholder.x, v.x);
                minholder.y = Mathf.Min(minholder.y, v.y);
                minholder.z = Mathf.Min(minholder.z, v.z);
            }
            float help = Math.Max(maxholder.x - minholder.x, maxholder.y - minholder.y);
            return Math.Max(help, maxholder.z - minholder.z);
        }

        public (float, Vector3) squaredDistanceTo(Vector3 p)
        {
            Vector3[] pos = getVertexPositions();
            float minVal = float.MaxValue;
            Vector3 minDistPoint = new Vector3(0, 0, 0);
            for (int i = 0; i < pos.Length - 2; i++)
            {
                (float, Vector3) tup = sqrtDistanceTriangleToPoint(pos[0], pos[i + 1], pos[i + 2], p);
                if (tup.Item1 <= minVal)
                {
                    minVal = tup.Item1;
                    minDistPoint = tup.Item2;
                }
            }
            return (minVal, minDistPoint);
        }

        public (float, Vector3) sqrtDistanceTriangleToPoint(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 p)
        {
            //Precalculation
            Vector3 edge0 = v1 - v0;
            Vector3 edge1 = v2 - v0;

            float a = edge0.sqrMagnitude;
            float b = Vector3.Dot(edge0, edge1);
            float c = edge1.sqrMagnitude;
            float d = Vector3.Dot(edge0, (v0 - p));
            float e = Vector3.Dot(edge1, (v0 - p));

            float det = a * c - b * b;
            float s = b * e - c * d;
            float t = b * d - a * e;

            if (s + t < det)
            {
                if (s < 0f)
                {
                    if (t < 0f)
                    {
                        //Region 4
                        if (d < 0)
                        {
                            t = 0;
                            if (-d >= a)
                            {
                                s = 1;
                            }
                            else
                            {
                                s = -d / a;
                            }
                        }
                        else
                        {
                            s = 0;
                            if (e >= 0)
                            {
                                t = 0;
                            }
                            else if (-e >= c)
                            {
                                t = 1;
                            }
                            else
                            {
                                t = -e / c;
                            }
                        }
                    }
                    else
                    {
                        //Region 3
                        s = 0;
                        if (e >= 0)
                        {
                            t = 0;
                        }
                        else if (-e >= c)
                        {
                            t = 1;
                        }
                        else
                        {
                            t = -e / c;
                        }
                    }
                }
                else if (t < 0f)
                {
                    //Region 5
                    t = 0;
                    if (d >= 0)
                    {
                        s = 0;
                    }
                    else if (-d >= a)
                    {
                        s = 1;
                    }
                    else
                    {
                        s = -d / a;
                    }
                }
                else
                {
                    //Region 0
                    float invDet = 1f / det;
                    s *= invDet;
                    t *= invDet;
                }
            }
            else
            {
                if (s < 0f)
                {
                    //Region 2
                    float tmp0 = b + d;
                    float tmp1 = c + e;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2 * b + c;
                        if (numer >= denom)
                        {
                            s = 1;
                        }
                        else
                        {
                            s = numer / denom;
                        }
                        t = 1 - s;
                    }
                    else
                    {
                        s = 0;
                        if (tmp1 <= 0)
                        {
                            t = 1;
                        }
                        else if (e >= 0)
                        {
                            t = 0;
                        }
                        else
                        {
                            t = -e / c;
                        }
                    }
                }
                else if (t < 0f)
                {
                    //Region 6
                    float tmp0 = b + e;
                    float tmp1 = a + d;
                    if (tmp1 > tmp0)
                    {
                        float numer = tmp1 - tmp0;
                        float denom = a - 2 * b + c;
                        if (numer >= denom)
                        {
                            t = 1;
                        }
                        else
                        {
                            t = numer / denom;
                        }
                        s = 1 - t;
                    }
                    else
                    {
                        t = 0;
                        if (tmp1 <= 0)
                        {
                            s = 1;
                        }
                        else if (d >= 0)
                        {
                            s = 0;
                        }
                        else
                        {
                            s = -d / a;
                        }
                    }
                }
                else
                {
                    //Region 1
                    float numer = c + e - (b + d);
                    if (numer <= 0)
                    {
                        s = 0;
                    }
                    else
                    {
                        float denom = a - 2 * b + c;
                        if (numer >= denom)
                        {
                            s = 1;
                        }
                        else
                        {
                            s = numer / denom;
                        }
                    }
                    t = 1f - s;
                }
            }

            Vector3 nearestPoint = v0 + s * edge0 + t * edge1;

            return ((p - nearestPoint).sqrMagnitude, nearestPoint);
        }

        public void createHalfEdges()
        {
            Vertex prevVertex = vertices[vertices.Count - 1];
            foreach (Vertex v in vertices)
            {
                Edge e = prevVertex.isConnected(v);
                HalfEdge he = new HalfEdge(prevVertex, v, e, this, mesh);
                halfEdges.Add(he);
                prevVertex = v;
            }
        }

        public HalfEdge prevHalfEdge(HalfEdge he)
        {
            HalfEdge prevHalfEdge = halfEdges[halfEdges.Count - 1];
            foreach (HalfEdge currHe in halfEdges)
            {
                if (currHe.Equals(he))
                    return prevHalfEdge;
                prevHalfEdge = currHe;
            }
            throw new Exception("the given Halfedge \n" + he + "\n is not in this Face \n" + this);
        }

        public List<Edge> getInnerEdges()
        {
            List<Edge> innerEdges = new List<Edge>();

            foreach (Edge e in edges)
            {
                Vertex prevVertex = e.getNewV1();
                foreach (Vertex v in e.getVerticesOnEdge())
                {
                    innerEdges.Add(prevVertex.isConnected(v));
                    prevVertex = v;
                }

                innerEdges.Add(prevVertex.isConnected(e.getNewV2()));
            }

            foreach (Vertex v in innerVertices)
            {
                foreach (Edge e in v.getEdges())
                {
                    if (!innerEdges.Contains(e))
                    {
                        innerEdges.Add(e);
                    }
                }
            }

            return innerEdges;
        }

        public void resetValues()
        {
            foreach (Vertex v in innerVertices)
            {
                v.delete();
            }
            innerVertices.Clear();
        }

        public void switchVertex(Vertex oldV, Vertex newV)
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                if (oldV.Equals(vertices[i]))
                {
                    vertices[i] = newV;
                }
            }
        }

        public void delete()
        {
            handleNumber = -1;
            foreach (Vertex v in innerVertices)
            {
                if (v != null && v.isValid())
                {
                    v.delete();
                }
            }
            foreach (HalfEdge he in halfEdges)
            {
                if (he != null && he.isValid())
                {
                    he.delete();
                }
            }

            if (symFace != null)
            {
                symFace.setSymFace(null);
            }
            mesh.removeFace(this);
        }

        public Vector3 getNormal()
        {
            HalfEdge prevEdge = halfEdges[edges.Count - 1];
            Vector3 normal = new Vector3(0, 0, 0);
            foreach (HalfEdge e in halfEdges)
            {
                normal += Vector3.Cross(prevEdge.getDirection(), e.getDirection());

                prevEdge = e;
            }
            return normal.normalized;
        }

        public int getSepNumSum()
        {
            int sum = 0;
            foreach (Edge e in edges)
            {
                sum += e.getSepNumber();
            }
            return sum;
        }

        //returns if the face is valid or deleted
        public Boolean isValid()
        {
            return handleNumber >= 0;
        }

        public List<Vertex> getVertices()
        {
            return vertices;
        }

        public List<Edge> getEdges()
        {
            return edges;
        }

        public List<HalfEdge> getHalfEdges()
        {
            return halfEdges;
        }

        public int getHandleNumber()
        {
            return handleNumber;
        }

        public MeshStruct getMesh()
        {
            return mesh;
        }

        public List<Vertex> getInnerVertices()
        {
            return innerVertices;
        }

        public Face getSymFace()
        {
            return symFace;
        }

        public GameObject getErrorMess()
        {
            return errorMess;
        }

        public void setVertices(List<Vertex> v)
        {
            this.vertices = v;
        }

        public void setHandleNumber(int handleNumber)
        {
            this.handleNumber = handleNumber;
        }

        public void setMesh(MeshStruct m)
        {
            this.mesh = m;
        }

        public void setErrorMess(GameObject eM)
        {
            errorMess = eM;
        }

        public void setSymFace(Face f)
        {
            symFace = f;
            if (f.getSymFace() == null)
            {
                f.setSymFace(this);
            }
        }

        public override string ToString()
        {
            string output = "Face " + handleNumber + "\n";
            foreach (Vertex v in vertices)
            {
                output += v + "\n";
            }
            return output;
        }

        public bool Equals(Face f)
        {
            return this.getHandleNumber() == f.getHandleNumber();
        }
    }
}

