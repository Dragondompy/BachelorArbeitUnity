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
                    Debug.Log("replaced " + oldV);
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

