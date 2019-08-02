using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class MeshStruct : MonoBehaviour
    {
        private GameObject VertexObj;

        private EmptyPattern utils;
        private List<Vertex> vertices;
        private List<Face> faces;
        private List<Edge> edges;
        private List<HalfEdge> halfEdges;
        private List<Vertex> selectedVertices;
        private Edge selectedEdge;
        private Face selectedFace;
        private string comments;
        private float size;
        private List<int> splitToNotSplitVertices;
        private List<int> splitToNotSplitFaces;

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

        //initializes with vertices from old Mesh
        public void loadVerticesFromMesh(MeshStruct old)
        {
            utils = new EmptyPattern();
            vertices = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            selectedVertices = new List<Vertex>();
            comments = "";

            size = old.getSize(); ;
            foreach (Vertex v in old.getVertices())
            {
                addVertex(v.getPosition());
            }
        }

        //initializes Empty Mesh
        public void loadEmptyFromMesh(MeshStruct old)
        {
            utils = new EmptyPattern();
            vertices = new List<Vertex>();
            faces = new List<Face>();
            edges = new List<Edge>();
            halfEdges = new List<HalfEdge>();
            selectedVertices = new List<Vertex>();
            comments = "";

            size = old.getSize(); ;
        }

        public void addGameObjects(GameObject vOb)
        {
            VertexObj = vOb;
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

        public Boolean vertexExists(int hn)
        {
            if (hn < getVertexHandleNumber() && hn >= 0 && vertices[hn].getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean edgeExists(int hn)
        {
            if (hn < getEdgeHandleNumber() && hn >= 0 && edges[hn].getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean faceExists(int hn)
        {
            if (hn < getFaceHandleNumber() && hn >= 0 && faces[hn].getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
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

        public void selectVertexAt(int v)
        {
            Vertex ver = getVertexAt(v);
            if (!ver.getIsSelected())
            {
                if (selectedVertices.Count < 6)
                {
                    if (ver.getVertexObject() == null)
                    {
                        GameObject vOb = Instantiate(VertexObj, transform.position + ver.getPosition(), Quaternion.identity);
                        vOb.GetComponent<VertexObj>().vertexIndex = v;
                        ver.setVertexObject(vOb);
                    }
                    ver.setIsSelected(true);
                    selectedVertices.Add(ver);
                }
                else
                {
                    print("To many Vertices selected");
                }
            }
            else
            {
                if (!ver.getIsCreated())
                {
                    Destroy(ver.getVertexObject());
                }
                ver.setIsSelected(false);
                selectedVertices.Remove(ver);
            }

        }

        public void clearSelectedVertices()
        {
            foreach (Vertex v in selectedVertices)
            {
                Destroy(v.getVertexObject());
                v.setIsSelected(false);
            }
            selectedVertices.Clear();
        }

        public void clearSelection()
        {
            selectedFace = null;
            selectedEdge = null;
            clearSelectedVertices();
        }

        public Edge selectEdgeAt(int e)
        {
            if (selectedEdge != null && selectedEdge.Equals(edges[e]))
            {
                selectedEdge = null;
            }
            else
            {
                selectedEdge = edges[e];
            }
            return selectedEdge;
        }

        public Face selectFaceAt(int f)
        {
            if (selectedFace != null && selectedFace.Equals(faces[f]))
            {
                selectedFace = null;
            }
            else
            {
                selectedFace = faces[f];
            }
            return selectedFace;
        }

        public void deleteSelectedFace()
        {
            if (faceExists(selectedFace.getHandleNumber()))
            {
                selectedFace.delete();
            }
            else
            {
                print("No Face Selected");
            }
        }

        public void updateMesh()
        {
            ObjMesh helper = new ObjMesh(this);
            ObjLoader loader = new ObjLoader();
            Mesh ownMesh = loader.newLoad(helper);
            gameObject.GetComponent<MeshFilter>().mesh = ownMesh;
            gameObject.GetComponent<MeshCollider>().sharedMesh = ownMesh;
            splitToNotSplitVertices = loader.getSplitToNotSplitVertices();
            splitToNotSplitFaces = loader.getSplitToNotSplitFaces();
        }

        //deletes Vertex from Mesh TODO concatinate faces
        public void deleteVertex(int handleNumber)
        {
            vertices[handleNumber].delete();
        }

        public Vertex getVertexAt(int v)
        {
            if (vertexExists(v))
            {
                return vertices[v];
            }
            Debug.Log("There is no Vertex at " + v);
            return new Vertex("Empty");
        }

        public Edge getEdgeAt(int e)
        {
            if (edgeExists(e))
            {
                return edges[e];
            }
            Debug.Log("There is no Edge at " + e);
            return new Edge("Empty");
        }

        public Face getFaceAt(int f)
        {
            if (faceExists(f))
            {
                return faces[f];
            }
            Debug.Log("There is no Face at " + f);
            return new Face("Empty");
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

        public List<Vertex> getSelectedVertices()
        {
            return selectedVertices;
        }

        public Edge getSelectedEdge()
        {
            return selectedEdge;
        }

        public Face getSelectedFace()
        {
            return selectedFace;
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

