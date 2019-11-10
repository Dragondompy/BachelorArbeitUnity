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
        private int vertexHn;
        private List<Face> faces;
        private int faceHn;
        private List<Edge> edges;
        private int edgeHn;
        private List<HalfEdge> halfEdges;
        private int halfedgeHn;
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

        public Boolean vertexExists(int hn)
        {
            if (hn < getVertexHandleNumber() && hn >= 0 && getVertexAt(hn).getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean edgeExists(int hn)
        {
            if (hn < getEdgeHandleNumber() && hn >= 0 && getEdgeAt(hn).getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
        }

        public Boolean faceExists(int hn)
        {
            if (hn < getFaceHandleNumber() && hn >= 0 && getFaceAt(hn).getHandleNumber() >= 0)
            {
                return true;
            }
            return false;
        }

        //adds a Vertex to the Mesh and sets the handlenumber for that vertex
        public Vertex addVertex(Vector3 pos)
        {
            Vertex v = new Vertex(pos, this);
            v.setPosition(pos);
            v.setHandleNumber(getVertexHandleNumber());
            vertices.Add(v);
            vertexHn++;

            return v;
        }

        //adds the edge between to vertices to the mesh if it doesnt exist already
        //also adds the face to the edge
        public Edge addEdge(Face f, int v1, int v2)
        {
            Vertex vertex1 = (getVertexAt(v1));
            Vertex vertex2 = (getVertexAt(v2));
            Edge e = vertex1.isConnected(vertex2);

            if (e == null)
            {
                e = new Edge(vertex1, vertex2, f, this);
                e.setHandleNumber(getEdgeHandleNumber());
                edges.Add(e);
                edgeHn++;
            }
            else
            {
                e.addFace(f, this);
            }
            if (vertex1.getSymVertex() != null && vertex2.getSymVertex() != null)
            {
                e.setSymEdge(vertex1.getSymVertex().isConnected(vertex2.getSymVertex()));
                e.setSepNumber(e.getSymEdge().getSepNumber());
            }
            return e;
        }

        public Face addFace(List<int> vertices)
        {
            Face face = addSimpleFace(vertices);
            face.createHalfEdges();
            return face;
        }

        public HalfEdge addHalfEdge(HalfEdge h)
        {
            h.setHandleNumber(getHalfEdgeHandleNumber());
            halfEdges.Add(h);
            halfedgeHn++;
            return h;
        }

        public Face addSimpleFace(List<int> faceVer)
        {
            Face face = new Face(this);
            int prevVertex = faceVer[faceVer.Count - 1];
            foreach (int i in faceVer)
            {
                face.addVertex(i);
                face.addEdge(addEdge(face, prevVertex, i));
                prevVertex = i;
            }
            face.setHandleNumber(getFaceHandleNumber());
            faces.Add(face);
            faceHn++;
            return face;
        }

        public void removeVertex(Vertex v)
        {
            vertices.Remove(v);
        }

        public void removeEdge(Edge e)
        {
            edges.Remove(e);
        }

        public void removeFace(Face f)
        {
            faces.Remove(f);
        }

        public void removeHalfEdge(HalfEdge h)
        {
            halfEdges.Remove(h);
        }

        public void selectVertex(Vertex ver)
        {
            if (!ver.getIsSelected())
            {
                if (selectedVertices.Count < 6)
                {
                    if (!ver.getIsCreated())
                    {
                        GameObject vOb = Instantiate(VertexObj, transform.position + ver.getPosition(), Quaternion.identity);
                        vOb.transform.localScale = new Vector3(Mathf.Sqrt(size) * 3, Mathf.Sqrt(size) * 3, Mathf.Sqrt(size) * 3);
                        vOb.GetComponent<VertexObj>().vertex = ver;
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
                ver.setIsSelected(false);
                selectedVertices.Remove(ver);
                if (!ver.getIsCreated())
                {
                    Destroy(ver.getVertexObject());
                    ver.delete();
                }
            }

        }

        public void clearSelectedVertices()
        {
            foreach (Vertex v in selectedVertices)
            {
                v.setIsSelected(false);
                if (!v.getIsCreated())
                {
                    Destroy(v.getVertexObject());
                }
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
            if (selectedEdge != null && selectedEdge.Equals(getEdgeAt(e)))
            {
                selectedEdge = null;
                return selectedEdge;
            }
            selectedEdge = getEdgeAt(e);

            return selectedEdge;
        }

        public Face selectFaceAt(int f)
        {
            if (selectedFace != null && selectedFace.Equals(getFaceAt(f)) && InformationHolder.selectFace)
            {
                selectedFace = null;
            }
            else
            {
                selectedFace = getFaceAt(f);
            }
            return selectedFace;
        }

        public void deleteSelectedFace()
        {
            if (selectedFace != null)
            {
                if (faceExists(selectedFace.getHandleNumber()))
                {
                    int hn = selectedFace.getHandleNumber();
                    selectedFace.delete();
                    selectedFace = null;
                }
                else
                {
                    print("Face is not Valid");
                }
            }
            else
            {
                print("No Face selected");
            }
        }

        public void updateMesh()
        {
            ObjLoader loader = new ObjLoader();
            Mesh ownMesh = loader.newLoad(this);
            gameObject.GetComponent<MeshFilter>().mesh = ownMesh;
            gameObject.GetComponent<MeshCollider>().sharedMesh = ownMesh;
            splitToNotSplitVertices = loader.getSplitToNotSplitVertices();
            splitToNotSplitFaces = loader.getSplitToNotSplitFaces();
        }

        internal void concatinateVertices(Vertex vertex1, Vertex vertex2)
        {
            Vector3 pos = (vertex1.getPosition() + vertex2.getPosition()) / 2f;
            Edge edge = vertex1.isConnected(vertex2);
            if (edge == null)
            {
                foreach (Edge e in vertex2.getEdges())
                {
                    e.switchVertex(vertex1, vertex2);
                }
                vertex2.removeAllEdges();
            }
        }

        public (float, Vector3) minDistanceToPoint(Vector3 p)
        {
            float minDist = float.MaxValue;
            Vector3 minDistancePoint = new Vector3(0, 0, 0);
            foreach (Face f in faces)
            {
                (float, Vector3) tup = f.squaredDistanceTo(p);
                if (tup.Item1 <= minDist)
                {
                    minDist = tup.Item1;
                    minDistancePoint = tup.Item2;
                }
            }
            return (minDist, minDistancePoint);
        }

        public Vertex getVertexAt(int v)
        {
            for (int i = Mathf.Min(v, vertices.Count - 1); i >= 0; i--)
            {
                if (vertices[i].getHandleNumber() == v)
                {
                    return vertices[i];
                }
            }
            Debug.Log("There is no Vertex at " + v);
            return new Vertex("Empty");
        }

        public Edge getEdgeAt(int e)
        {
            for (int i = Mathf.Min(e, edges.Count - 1); i >= 0; i--)
            {
                if (edges[i].getHandleNumber() == e)
                {
                    return edges[i];
                }
            }
            Debug.Log("There is no Edge at " + e);
            return new Edge("Empty");
        }

        public Face getFaceAt(int f)
        {
            for (int i = Mathf.Min(f, faces.Count - 1); i >= 0; i--)
            {
                if (faces[i].getHandleNumber() == f)
                {
                    return faces[i];
                }
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
            return vertexHn;
        }

        public int getFaceHandleNumber()
        {
            return faceHn;
        }

        public int getEdgeHandleNumber()
        {
            return edgeHn;
        }

        public int getHalfEdgeHandleNumber()
        {
            return halfedgeHn;
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

