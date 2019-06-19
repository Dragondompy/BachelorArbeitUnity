using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Mesh : MonoBehaviour
    {
        public GameObject VertexObj;
        public GameObject EdgeObj;
        public GameObject FaceObj;

        private EmptyPattern utils;
        private List<Vertex> vertices;
        private List<Face> faces;
        private List<Edge> edges;
        private List<HalfEdge> halfEdges;
        private string comments;

        //initializes the Mesh struktur from objMesh by adding all vertices,edges and faces to lists
        public void loadMeshFromObj(ObjMesh obj)
        {
            utils = new EmptyPattern();
            vertices = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            comments = "";

            foreach (Vector3 v in obj.getVertices())
            {
                addVertex(v);
            }
            foreach (List<int> faceList in obj.getFaces())
            {
                addFace(faceList);
            }

            comments = obj.getComments();

        }

        //initializes the Mesh struktur with only the Vertices of another Mesh
        public void loadMeshFromMesh(Mesh old)
        {
            foreach (Vertex v in old.getVertices())
            {
                addVertex(v.getPosition());
            }
        }

        //adds a Vertex to the Mesh and sets the handlenumber for that vertex
        public Vertex addVertex(Vector3 pos)
        {
            Vertex v = Instantiate(VertexObj, pos, Quaternion.identity).GetComponent<Vertex>();

            v.loadVertex(this);
            v.setHandleNumber(getVertexHandleNumber());
            vertices.Add(v);

            return v;
        }

        //adds the edge between to vertices to the mesh if it doesnt exist already
        //also adds the face to the edge
        public Edge addEdge(Face f, int v1, int v2)
        {
            Vertex vertex1 = (vertices[v1]);
            Vertex vertex2 = (vertices[v2]);
            Edge e = vertex1.isConnected(vertex2);

            if (e == null)
            {
                Quaternion rot = Quaternion.LookRotation(utils.direction(vertex1, vertex2), new Vector3(0, 1, 0)); ;
                Vector3 pos = utils.middlePoint(vertex1, vertex2);
                GameObject eGo = Instantiate(EdgeObj, pos, rot);
                eGo.transform.localScale = new Vector3(1, 1, utils.direction(vertex1, vertex2).magnitude / 2);
                e = eGo.GetComponent<Edge>();
                e.loadEdge(vertex1, vertex2, f, this);
                e.setHandleNumber(getEdgeHandleNumber());
                edges.Add(e);
            }
            else
            {
                e.addFace(f, this);
            }
            return e;
        }

        internal void updateEdges(List<Edge> edgesOfVertex)
        {
            foreach (Edge e in edgesOfVertex)
            {
                e.updateTransform();
            }
        }

        public Face addFace(List<int> vertices)
        {
            Face face = addSimpleFace(vertices);
            face.createHalfEdges();
            return face;
        }

        public Face addSimpleFace(List<int> vertices)
        {
            GameObject fGo = Instantiate(FaceObj, new Vector3(0, 0, 0), Quaternion.identity);
            Face face = fGo.GetComponent<Face>();
            face.loadFace(this);
            int prevVertex = vertices[vertices.Count - 1];
            foreach (int i in vertices)
            {
                face.addVertex(i);
                face.addEdge(addEdge(face, prevVertex, i));
                prevVertex = i;
            }
            face.updateTransform();
            return face;
        }

        internal void updateFaces(Face f1, Face f2)
        {
            f1.setNeedToUpdateTransform();
            f2.setNeedToUpdateTransform();
        }

        //deletes Vertex from Mesh TODO concatinate faces
        public void deleteVertex(int handleNumber)
        {
            vertices[handleNumber].delete();
        }
        
        public Vertex getVertexAt(int v)
        {
            return vertices[v];
        }
        public Edge getEdgeAt(int e)
        {
            return edges[e];
        }
        public Face getFaceAt(int f)
        {
            return faces[f];
        }

        public List<Vertex> getVertices()
        {
            return vertices;
        }

        public List<Face> getFaces()
        {
            return faces;
        }

        public List<Edge> getEdges()
        {
            return edges;
        }

        public List<HalfEdge> getHalfEdges()
        {
            return halfEdges;
        }

        public EmptyPattern getUtils()
        {
            return utils;
        }

        public string getComments()
        {
            return comments;
        }

        public void setComments(string c)
        {
            comments = c;
        }

        public int getVertexHandleNumber()
        {
            return vertices.Count;
        }

        public int getFaceHandleNumber()
        {
            return faces.Count;
        }

        public int getEdgeHandleNumber()
        {
            return edges.Count;
        }

        public int getHalfEdgeHandleNumber()
        {
            return halfEdges.Count;
        }
    }
}

