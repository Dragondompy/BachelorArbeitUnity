using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Face : MonoBehaviour
    {
        private List<Vertex> vertices;
        private List<Edge> edges;
        private List<HalfEdge> halfEdges;
        private List<Vertex> innerVertices;
        private Boolean needToUpdateTransform;

        private int handleNumber;
        private Mesh mesh;

        public void loadFace(Mesh m)
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            innerVertices = new List<Vertex>();
            needToUpdateTransform = false;

            setMesh(m);
            setHandleNumber(m.getFaceHandleNumber());
            m.getFaces().Add(this);
        }

        public void Update()
        {
            if (needToUpdateTransform)
            {
                updateTransform();
            }
        }

        //adds vertex to this face
        public void addVertex(Vertex v)
        {
            vertices.Add(v);
            needToUpdateTransform = true;
        }

        public void addVertex(int v)
        {
            addVertex(mesh.getVertexAt(v));
        }

        public void addInnerVertex(Vertex v)
        {
            innerVertices.Add(v);
        }

        //adds edge to this face
        public void addEdge(Edge e)
        {
            edges.Add(e);
        }

        public void setNeedToUpdateTransform()
        {
            needToUpdateTransform = true;
        }

        public void updateTransform()
        {
            UnityEngine.Mesh pol = GetComponent<MeshFilter>().mesh;
            pol.vertices = getVertexPositions();
            pol.triangles = getTriangles();
            gameObject.GetComponent<MeshFilter>().mesh = pol;
            needToUpdateTransform = false;
        }

        public int[] getTriangles() {
            int[] tri = new int[vertices.Count*3];
            for (int i = 0; i < vertices.Count-2; i++)
            {
                tri[i*3] = 0;
                tri[i*3 + 1] = i + 1;
                tri[i*3 + 2] = i + 2;
            }
            return tri;
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

        //connects face f with this face TODO concatinate faces
        // TODO remove the halfedge from the list? done with handlenumber = -1 ?
        public void connectFace(Face f, Edge e)
        {
            edges.Remove(e);
        }

        public void delete()
        {
            handleNumber = -1;
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
            string output = "Face\n";
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

