using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Edge
	{
		private HalfEdge h1;
		private HalfEdge h2;
		private Face f1;
		private Face f2;
		private Vertex v1;
		private Vertex v2;
		private int sepNumber;

		private Vertex[] verticesOnEdge;

		private int handleNumber;

		public Edge (Vertex v1, Vertex v2, Face f, Mesh m)
		{
			setV1 (v1);
			setV2 (v2);
			v1.addEdge (this);
			v2.addEdge (this);
			f1 = f;

			setHandleNumber (m.getEdgeHandleNumber ());
			m.getEdges ().Add (this);
		}

		//tests if this vertex connects v to any other vertex
		public Boolean contains (Vertex v)
		{
			if (v == v1 || v == v2)
				return true;
			return false;
		}

		//adds face to the list of faces of this edge
		//also adds the vertices in different order to the edges
		public void addFace (Face f, Mesh m)
		{
			f2 = f;
		}

		public void addHalfEdge(HalfEdge he)
		{
			if (h1 == null)
				h1 = he;
			else if (h2 == null)
				h2 = he;
			else
				throw new Exception ("This Edge already has 2 Halfedges");
		}

		//deletes this edge and concatinates the to faces previously serperated by this edge
		public void delete ()
		{
			handleNumber = -1;
			if (f1.getHandleNumber () < 0 || f2.getHandleNumber () < 0) {
				f1.delete ();		
				f2.delete ();
			} else {
				f1.connectFace (f2, this);
				f2.delete ();
			}
			h1.delete ();
			h2.delete ();
		}

		//returns the Vector from Vertex v1 to Vertex v2
		public double[] getDirection ()
		{
			double[] d = new double[3];
			d [0] = v2.getX () - v1.getX ();
			d [1] = v2.getY () - v1.getY ();
			d [2] = v2.getZ () - v1.getZ ();
			return d;
		}

		public double[] getDirectionFrom(Vertex v){
			if (contains (v)) {
				double[] d = new double[3];
				if (v.Equals (v1)) {
					d [0] = v2.getX () - v1.getX ();
					d [1] = v2.getY () - v1.getY ();
					d [2] = v2.getZ () - v1.getZ ();
				} else {
					d [0] = v1.getX () - v2.getX ();
					d [1] = v1.getY () - v2.getY ();
					d [2] = v1.getZ () - v2.getZ ();					
				}
				return d;
			}
			throw new Exception ("The Edge " + this + " does not contain " + v);
		}

		//returns the halfedge corresponding to the given face
		public HalfEdge getHalfEdge (Face f)
		{
			if (f.getHandleNumber () == h1.getF ().getHandleNumber ())
				return h1;
			else if (f.getHandleNumber () == h2.getF ().getHandleNumber ())
				return h2;
			return null;
		}

		//adds points on the edge to the edge and corresponding halfedges in their respective order
		public void setVerticesOnEdge (Vertex[] voe)
		{
			if (voe.Length>0 && voe [0].distanceTo (v1) > voe [0].distanceTo (v2)) {
				Vertex[] turnedAround = new Vertex[voe.Length];
				for (int i = 0; i < turnedAround.Length; i++) {
					turnedAround [turnedAround.Length - i - 1] = voe [i];
				}
				voe = turnedAround;
			}
			verticesOnEdge = voe;
			h1.setVerticesOnEdge (voe);

			if (h2 != null) {
				h2.setVerticesOnEdge (voe);
			}
		}

		public String printVertices(){
			String output = "";
			output += v1;
			foreach (Vertex v in verticesOnEdge) {
				output += v;
			}
			output += v2;
			return output;
		}

		//returns if the edge is valid or deleted
		public Boolean isValid ()
		{
			return handleNumber >= 0;
		}

		public Vertex getV1 ()
		{
			return v1;	
		}

		public Vertex getV2 ()
		{
			return v2;	
		}

		public Face getF1 ()
		{
			return f1;
		}

		public Face getF2 ()
		{
			return f2;
		}

		public int getHandleNumber ()
		{
			return handleNumber;
		}

		public int getSepNumber ()
		{
			return sepNumber;
		}

		public Vertex[] getVerticesOnEdge ()
		{
			return verticesOnEdge;
		}

		public void setSepNumber (int sepNumber)
		{
			this.sepNumber = sepNumber;
		}

		public void setV1 (Vertex v)
		{
			this.v1 = v;
		}

		public void setV2 (Vertex v)
		{
			this.v2 = v;
		}

		public void setF1 (Face f)
		{
			this.f1 = f;
		}

		public void setF2 (Face f)
		{
			this.f2 = f;
		}

		public void setHandleNumber (int handleNumber)
		{
			this.handleNumber = handleNumber;
		}

		public override string ToString ()
		{
			return ("Edge:\n from  " + v1 + "\n to    " + v2 + "\n SeperationNumber: " + sepNumber + "\n");
		}

		public bool Equals (Edge e)
		{
			return this.getHandleNumber()==e.getHandleNumber();
		}
	}
}

