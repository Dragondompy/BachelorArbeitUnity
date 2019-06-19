using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitCSharp
{
	public class Vertex
    {        
        private double x;
		private double y;
		private double z;

		private int handleNumber;

		private List<Edge> edges = new List<Edge> ();

		public Vertex (double x, double y, double z, Mesh m)
		{
			setX (x);
			setY (y);
			setZ (z);

			setHandleNumber (m.getVertexHandleNumber ());
			m.getVertices ().Add (this);
        }

		//returns edge that connects this vertex to vertex v, returns null if there is no connection
		public Edge isConnected (Vertex v)
		{
			foreach (Edge e in edges) {
				if (e.contains (v)) {
					return e;
				}
			}
			return null;
		}

		//adds edge to the edge list of this vertex
		public void addEdge (Edge e)
		{
			edges.Add (e);
		}

		//deletes this vertex and all connected edges
		public void delete()
		{
			handleNumber = -1;
			foreach (Edge e in edges) {
				e.delete ();
			}
		}

		public double distanceTo(Vertex v){
			double x = Math.Abs( getX () - v.getX ());
			double y = Math.Abs( getY () - v.getY ());
			double z = Math.Abs( getZ () - v.getZ ());

			return Math.Sqrt (x + y + z);
		}

		//returns if the vertex is valid or deleted
		public Boolean isValid(){
			return handleNumber >= 0;
		}

		public double[] getPosition()
		{
			double[] p = new double[3];
			p[0] = x;
			p[1] = y;
			p[2] = z;

			return p;
		}
		public double getX ()
		{
			return x;	
		}

		public double getY ()
		{
			return y;	
		}

		public double getZ ()
		{
			return z;	
		}

		public List<Edge> getEdges(){
			return edges;
		}

		public int getHandleNumber ()
		{
			return handleNumber;
		}

		public void setX (double x)
		{
			this.x = x;
		}

		public void setY (double y)
		{
			this.y = y;
		}

		public void setZ (double z)
		{
			this.z = z;
		}

		public void setHandleNumber(int handleNumber){
			this.handleNumber = handleNumber;
		}

		public override string ToString ()
		{
			return "Vertex at: " + getX () + "| " + getY () + "| " + getZ ()+"    hN: "+handleNumber;
		}

		public bool Equals (Vertex v)
		{
			return this.getHandleNumber()==v.getHandleNumber();
		}
	}
}

