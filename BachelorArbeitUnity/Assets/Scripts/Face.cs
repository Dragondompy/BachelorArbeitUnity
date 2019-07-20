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

        private int handleNumber;
        private Mesh mesh;

        public Face(Mesh m)
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            innerVertices = new List<Vertex>();

            setMesh(m);
            setHandleNumber(m.getFaceHandleNumber());
            m.getFaces().Add(this);
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

        public Mesh getMesh()
        {
            return mesh;
        }

        public List<Vertex> getInnerVertices()
        {
            return innerVertices;
        }

        public void setVertices(List<Vertex> v)
        {
            this.vertices = v;
        }

        public void setHandleNumber(int handleNumber)
        {
            this.handleNumber = handleNumber;
        }

        public void setMesh(Mesh m)
        {
            this.mesh = m;
        }

        public override string ToString()
        {
            string output = "Face " +handleNumber+"\n";
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

