using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Mesh : MonoBehaviour
    {
        private GameObject VertexObj;
        private GameObject EdgeObj;
        private GameObject FaceObj;

        private EmptyPattern utils;
        private List<Vertex> vertices;
        private List<Face> faces;
        private List<Edge> edges;
        private List<HalfEdge> halfEdges;
        private List<Vertex> selectedVertices;
        private string comments;
        private float size;

        //initializes the Mesh struktur from objMesh by adding all vertices,edges and faces to lists
        public void loadMeshFromObj(ObjMesh obj)
        {
            utils = new EmptyPattern();
            vertices = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            selectedVertices = new List<Vertex>();
            comments = "";

            size = calcSize(obj.getVertices());

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
        
        public void addGameObjects(GameObject vOb, GameObject eOb, GameObject fObj)
        {
            VertexObj = vOb;
            EdgeObj = eOb;
            FaceObj = fObj;
        }

        public float calcSize(List<Vector3> verts)
        {
            float minX = 0f;
            float minY = 0f;
            float minZ = 0f;

            float maxX = 0f;
            float maxY = 0f;
            float maxZ = 0f;

            foreach (Vector3 v in verts)
            {
                maxX = Mathf.Max(maxX, v.x);
                maxY = Mathf.Max(maxY, v.y);
                maxZ = Mathf.Max(maxZ, v.z);

                minX = Mathf.Min(minX, v.x);
                minY = Mathf.Min(minY, v.y);
                minZ = Mathf.Min(minZ, v.z);
            }
            float param = Mathf.Log(verts.Count, 2f);
            return Mathf.Max(maxX - minX, maxY - minY, maxZ - minZ) / (param);
        }

        //adds a Vertex to the Mesh and sets the handlenumber for that vertex
        public Vertex addVertex(Vector3 pos)
        {
            Vertex v = new Vertex(pos);
            v.setPosition(pos);
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
                e = new Edge(vertex1, vertex2, f, this);
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
            Face face = new Face(this);
            int prevVertex = vertices[vertices.Count - 1];
            foreach (int i in vertices)
            {
                face.addVertex(i);
                face.addEdge(addEdge(face, prevVertex, i));
                prevVertex = i;
            }
            return face;
        }

        internal void selectVertexAt(int v)
        {
            Vertex ver = getVertexAt(v);
            if (ver.getVertexObject() == null)
            {
                if (selectedVertices.Count < 6)
                {
                    GameObject vOb = Instantiate(VertexObj, ver.getPosition(), Quaternion.identity);
                    vOb.GetComponent<VertexObj>().vertexIndex = v;
                    ver.setVertexObject(vOb);
                    selectedVertices.Add(ver);
                }
                else
                {
                    print("To many Vertices selected");
                }
            }
            else
            {
                Destroy(ver.getVertexObject());
                selectedVertices.Remove(ver);
            }

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

        public float getSize()
        {
            return size;
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

