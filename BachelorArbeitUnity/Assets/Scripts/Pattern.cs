using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern : MonoBehaviour
	{
		private int size;
		private int patternNumber;
		private EmptyPattern pattern;

		public Pattern ()
		{
			Console.WriteLine ("THIS PATTERN IS NOT SPECIFIC !!!");
			Console.WriteLine ("MOST FUNCTIONS WILL NOT WORK !!!");
		}

		public Pattern (EmptyPattern p)
		{
			this.pattern = p;
			this.size = p.getSize ();
			this.patternNumber = p.getPatternNumber ();
			Console.WriteLine ("Size: " + size + "  Pattern: " + patternNumber);
		}

		public void fillPatch (Mesh oldMesh, Mesh newMesh, Face face)
		{
			addOuterFlowVertices (oldMesh, newMesh, face);
			addOuterFlowFaces (oldMesh, newMesh, face);

			List<List<int>> innerListVertices = getInnerVertices (face);
			pattern.createFaces (newMesh, face, innerListVertices);

			simpleRepositionVertices (face, 10);

			/*foreach (List<int>l in innerListVertices) {
				foreach (int i in l) {
					Console.Write (i + " ");
				}
				Console.WriteLine ();
			}
			List<int> test = new List<int> ();
			foreach (List<int> l in innerVertices) {
				foreach (int i in l) {
					test.Add (i);
				}
			}
			newMesh.addSimpleFace (test);*/
		}

		//adds the vertices of outer flows to the Mesh and the halfedges
		public void addOuterFlowVertices (Mesh oldMesh, Mesh newMesh, Face face)
		{
			List<HalfEdge> halfEdges = face.getHalfEdges ();
			int flowCount;

			HalfEdge prevHe = halfEdges [halfEdges.Count - 2];
			HalfEdge currentHe = halfEdges [halfEdges.Count - 1];

			foreach (HalfEdge nextHe in halfEdges) {
				flowCount = 0;

				for (int f = 0; f < currentHe.getOuterFlow (); f++) {
					Vertex vh1 = prevHe.getVerticesOnEdge () [prevHe.getSepNumber () - 2 - f];
					Vertex vh2 = nextHe.getVerticesOnEdge () [f];
					//double[] direction = { vh2.getX () - vh1.getX (), vh2.getY () - vh1.getY (), vh2.getZ () - vh1.getZ () };
                    Vector3 direction = vh2.getPosition() - vh1.getPosition();

					for (int i = 0; i < currentHe.getSepNumber () - 1; i++) {
						int prevHeV = prevHe.exists (i, prevHe.getSepNumber () - 2 - f);
						int nextHeV = nextHe.exists (currentHe.getSepNumber () - 2 - i, f);
						if (prevHeV >= 0) {
							currentHe.addVertex (newMesh.getVertexAt (prevHeV), f, i);
						} else if (nextHeV >= 0) {
							currentHe.addVertex (newMesh.getVertexAt (nextHeV), f, i);
						} else {
                            Vector3 pos = vh1.getPosition() + direction * (i + 1) / currentHe.getSepNumber();
							Vertex v = newMesh.addVertex (pos);
							prevHe.addVertex (v, i, prevHe.getSepNumber () - 1 - f);
							currentHe.addVertex (v, f, i);
							nextHe.addVertex (v, currentHe.getSepNumber () - 1 - i, f);

							face.addInnerVertex (v);
						}
					}
					flowCount++;
				}
				prevHe = currentHe;
				currentHe = nextHe;
			}
		}

		public void addOuterFlowFaces (Mesh oldMesh, Mesh newMesh, Face oldFace)
		{
			List<HalfEdge> halfEdges = oldFace.getHalfEdges ();

			int size = halfEdges.Count;

			for (int h = size; h < size * 2; h++) {
				HalfEdge prevHe = halfEdges [(h - 1) % size];
				HalfEdge curHe = halfEdges [(h) % size]; 
				HalfEdge nextHe = halfEdges [(h + 1) % size];

				int prevFlow = prevHe.getOuterFlow ();
				for (int flow = 0; flow < curHe.getOuterFlow (); flow++) {
					for (int index = prevFlow; index < curHe.getSepNumber (); index++) {
						List<int> faceList = new List<int> ();
						faceList.Add (getVertexAt (prevHe, curHe, nextHe, flow, index));
						faceList.Add (getVertexAt (prevHe, curHe, nextHe, flow, index + 1));
						faceList.Add (getVertexAt (prevHe, curHe, nextHe, flow + 1, index + 1));
						faceList.Add (getVertexAt (prevHe, curHe, nextHe, flow + 1, index));
						newMesh.addSimpleFace (faceList);
					}
				}
			}
		}

		public void simpleRepositionVertices (Face face, int runs)
		{
			for (int i = 0; i < runs; i++) {
				foreach (Vertex v in face.getInnerVertices()) {
                    Vector3 d = v.getPosition();

					float length = (float)v.getEdges ().Count;
					length *= 2;
					foreach (Edge e in v.getEdges()) {
						Vector3 h = e.getDirectionFrom (v);

                        d += h / length;
					}
                    v.setPosition(d);
				}
			}
			
		}

		//TODO needs rebuild 
		//calculate area of the faces and reposition vertices to achive similar areas for all faces
		public void repositionVertices (Face face, int runs, double strength)
		{
			for (int i = 0; i < runs; i++) {
				foreach (Vertex v in face.getInnerVertices()) {
                    Vector3 d = v.getPosition();

					float length = v.getEdges ().Count;
					//length *= 3;
					foreach (Edge e in v.getEdges()) {
						Vector3 h = e.getDirectionFrom (v);
						Console.WriteLine (h [0]);
						Console.WriteLine (h [1]);
						Console.WriteLine (h [2]);
						Console.WriteLine ();
						double a = (Math.Abs(h [0]) + Math.Abs(h [1]) + Math.Abs(h [2])) * strength;
                        d += h * (float)a / length;
					}
                    v.setPosition(d);
				}
			}
		}

		public int getVertexAt (HalfEdge prevHe, HalfEdge curHe, HalfEdge nextHe, int flow, int index)
		{
			if (flow == 0) {
				if (index == 0) {
					return curHe.getV1 ().getHandleNumber ();
				} else if (index == curHe.getSepNumber ()) {
					return curHe.getV2 ().getHandleNumber ();
				} else {
					return curHe.getVerticesOnEdge () [index - 1].getHandleNumber ();
				}
			} else if (index == 0) {
				return prevHe.getVerticesOnEdge () [prevHe.getSepNumber () - flow - 1].getHandleNumber ();
			} else if (index == curHe.getSepNumber ()) {
				return nextHe.getVerticesOnEdge () [flow - 1].getHandleNumber ();
			} else {
				return curHe.getAdditionalVertices () [flow - 1, index - 1];
			}
		}

		public List<List<int>> getInnerVertices (Face face)
		{

			List<List<int>> innerVertices = new List<List<int>> ();

			List<HalfEdge> HalfEdges = face.getHalfEdges ();
			for (int i = size; i < size * 2; i++) {
				HalfEdge prevHe = HalfEdges [(i - 1) % size];
				HalfEdge curHe = HalfEdges [i % size];
				HalfEdge nextHe = HalfEdges [(i + 1) % size];

				List<int> h = new List<int> ();
				for (int j = prevHe.getOuterFlow (); j <= curHe.getSepNumber () - nextHe.getOuterFlow (); j++) {
					int v = getVertexAt (prevHe, curHe, nextHe, curHe.getOuterFlow (), j);
					h.Add (v);
				}
				innerVertices.Add (h);

			}
			return innerVertices;
		}

		public int getSize ()
		{
			return size;
		}

		public int getPatternNumber ()
		{
			return patternNumber;
		}

		public EmptyPattern getPattern ()
		{
			return pattern;
		}

		public void setPattern (EmptyPattern pattern)
		{
			this.pattern = pattern;
		}
	}
}

