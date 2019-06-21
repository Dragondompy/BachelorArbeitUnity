using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class HalfEdge
	{
		private Vertex v1;
		private Vertex v2;
		private Face f;
		private Edge e;
		private int handleNumber;
		private int[,] additionalVertices;
		private Vertex[] verticesOnEdge;
		private int outerFlow;
		private int reduced;

		public HalfEdge (Vertex v1,Vertex v2,Edge e, Face f, Mesh m)
		{
			setV1 (v1);
			setV2 (v2);
			outerFlow = 0;
			reduced = -1;
			setE (e);
			setFace (f);

			e.addHalfEdge (this);
			this.handleNumber = m.getHalfEdgeHandleNumber ();
			m.getHalfEdges ().Add (this);
		}

		public Boolean contains (Vertex v)
		{
			return e.contains (v);
		}

		//adds the vertex in an outer flow to the halfedge
		public void addVertex (Vertex v, int flow, int index)
		{
			if (inrange (flow, index)) {
				additionalVertices [flow, index] = v.getHandleNumber ();
			}
		}

		//tests if the vertex in the flow at the position exists
		public int exists (int flow, int index)
		{
			if (inrange (flow, index)) {
				return additionalVertices [flow, index];
			} else {
				return -2;
			}
		}

		//tests if the vertex at the position can and should be added to the halfedge
		public Boolean inrange (int flow, int index)
		{
			if (flow < outerFlow && index < getSepNumber () - 1)
				return true;
			return false;

		}

		public void createVerticesArray ()
		{
			additionalVertices = new int[outerFlow, getSepNumber () - 1];
			for (int i = 0; i < additionalVertices.GetLength (0); i++) {
				for (int j = 0; j < additionalVertices.GetLength (1); j++) {
					additionalVertices [i, j] = -1;
				}
			}
		}

		public void addOuterFlow ()
		{
			outerFlow++;
		}

		public void reduceSep ()
		{
			if (reduced < 0)
				throw new Exception ("SepNumber has not been set correctly");
			reduced--;
		}

		public void setVerticesOnEdge (Vertex[] voe)
		{
			if (voe.Length > 0 && voe [0].distanceTo (v1) > voe [0].distanceTo (v2)) {
				Vertex[] turnedAround = new Vertex[voe.Length];
				for (int i = 0; i < turnedAround.Length; i++) {
					turnedAround [turnedAround.Length - i - 1] = voe [i];
				}
				voe = turnedAround;
			}
			verticesOnEdge = voe;
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

		public void delete ()
		{
			handleNumber = -1;
		}

		public Face getF ()
		{
			return f;
		}

		public Edge getE ()
		{
			return e;
		}

		public Vertex getV1 ()
		{
			return v1;
		}

		public Vertex getV2 ()
		{
			return v2;
		}

		public int getReduced ()
		{
			return reduced;
		}

		public int getOuterFlow ()
		{
			return outerFlow;
		}

		public int getHandleNumber ()
		{
			return handleNumber;
		}

		public int getSepNumber ()
		{
			return getE ().getSepNumber ();
		}

		public Vertex[] getVerticesOnEdge ()
		{
			return verticesOnEdge;
		}

		public int[,] getAdditionalVertices ()
		{
			return additionalVertices;
		}

		public void setFace (Face f)
		{
			this.f = f;
		}

		public void setE (Edge e)
		{
			this.e = e;
		}

		public void setV1 (Vertex v)
		{
			this.v1 = v;
		}

		public void setV2 (Vertex v)
		{
			this.v2 = v;
		}

		public void setReduced (int reduced)
		{
			this.reduced = reduced;
		}

		public void setOuterFlow (int outerFlow)
		{
			this.outerFlow = outerFlow;
		}

		public override string ToString ()
		{
			return "HalfEdge:\n from  " + v1 + "\n to    " + v2 +
			"\n Reduced SepNumber: " + reduced + "\n Amount of Flow: " + outerFlow + "\n";
		}

		public bool Equals (HalfEdge he)
		{
			return this.getHandleNumber () == he.getHandleNumber ();
		}
	}
}

