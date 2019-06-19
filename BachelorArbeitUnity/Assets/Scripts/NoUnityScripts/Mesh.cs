using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Mesh
	{
		private List<Vertex> vertices = new List<Vertex> ();
		private List<Face> faces = new List<Face> ();
		private List<Edge> edges = new List<Edge> ();
		private List<HalfEdge> halfEdges = new List<HalfEdge> ();
		private string comments = "";

		//initializes the Mesh struktur with only the Vertices of another Mesh
		public Mesh (Mesh old)
		{
			foreach (Vertex v in old.getVertices()) {
				addVertex(v.getX(), v.getY(), v.getZ());
			}

        }

        //initializes the Mesh struktur from objMesh by adding all vertices,edges and faces to lists
        public Mesh (ObjMesh obj)
		{
			foreach (double[] v in obj.getVertices()) {
				addVertex (v [0], v [1], v [2]);
			}
			foreach (List<int> faceList in obj.getFaces()) {
				addFace (faceList);
			}

            comments = obj.getComments();
        }

		//adds a Vertex to the Mesh and sets the handlenumber for that vertex
		public Vertex addVertex (double x, double y, double z)
		{
			Vertex v = new Vertex (x, y, z, this);

			return v;
		}

		//adds the edge between to vertices to the mesh if it doesnt exist already
		//also adds the face to the edge
		public Edge addEdge (Face f, int v1, int v2)
		{
			Vertex vertex1 = (vertices [v1]);
			Vertex vertex2 = (vertices [v2]);
			Edge e = vertex1.isConnected (vertex2);
			if (e == null) {
				e = new Edge (vertex1, vertex2, f, this);

			} else {
				e.addFace (f, this);
			}
			return e;
		}

		public Face addFace(List<int> vertices){
			Face face = addSimpleFace (vertices);
			face.createHalfEdges ();
			return face;
		}

		public Face addSimpleFace(List<int> vertices){
			Face face = new Face (this);
			int prevVertex = vertices [vertices.Count - 1];
			foreach (int i in vertices) {
				face.addVertex (i);
				face.addEdge (addEdge (face, prevVertex, i));
				prevVertex = i;
			}
			return face;
		}

		//deletes Vertex from Mesh TODO concatinate faces
		public void deleteVertex (int handleNumber)
		{
			vertices [handleNumber].delete ();
		}

		public Vertex getVertexAt(int v){
			return vertices [v];
		}
		public Edge getEdgeAt(int e){
			return edges [e];
		}
		public Face getFaceAt(int f){
			return faces [f];
		}

		public List<Vertex> getVertices ()
		{
			return vertices;
		}

		public List<Face> getFaces ()
		{
			return faces;
		}

		public List<Edge> getEdges ()
		{
			return edges;
		}

		public List<HalfEdge> getHalfEdges ()
		{
			return halfEdges;
		}

		public string getComments ()
		{
			return comments;
		}

		public void setComments (string c)
		{
			comments = c;
		}

		public int getVertexHandleNumber(){
			return vertices.Count;
		}

		public int getFaceHandleNumber(){
			return faces.Count;
		}

		public int getEdgeHandleNumber(){
			return edges.Count;
		}

		public int getHalfEdgeHandleNumber(){
			return halfEdges.Count;
		}
	}
}

