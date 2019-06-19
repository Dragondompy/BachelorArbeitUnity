using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class EmptyPattern
	{
		protected int size;
		protected int patternNumber;
		protected int alpha;
		protected int beta;

		public EmptyPattern ()
		{
		}

		public virtual void createFaces (Mesh newMesh, Face f, List<List<int>> innerListVertices)
		{
			
		}

		//Creates Synthetic edges as new datastructure for the pattern
		public List<SyntheticEdge> createSyntheticEdges (List<List<int>> innerVertices)
		{
			List<SyntheticEdge> edges = new List<SyntheticEdge> ();

			foreach (List<int> l in innerVertices) {
				SyntheticEdge e = new SyntheticEdge ();
				e.setV1 (l [0]);
				e.setV2 (l [l.Count - 1]);
				List<int> vOE = new List<int> ();

				for (int i = 1; i < l.Count - 1; i++) {
					vOE.Add (l [i]);
				}
				e.setVerticesOnEdge (vOE);
				edges.Add (e);
			}
			return edges;
		}

		//creates new Quad with vertices 1,2,3,4
		public Face createFace(int v1,int v2,int v3,int v4, Mesh newMesh){
			List<int> f = new List<int> ();
			f.Add (v1);
			f.Add (v2);
			f.Add (v3);
			f.Add (v4);

			return newMesh.addSimpleFace (f);
		}

		//Creates new Vertex at with position at pos + (number/max)*dir
		public int createVertex(double[] pos,double[] dir,int max, int number,Face f,Mesh newMesh){
			double[] d = new double[3];
			d [0] = pos [0] + (number * dir [0] / (max));
			d [1] = pos [1] + (number * dir [1] / (max));
			d [2] = pos [2] + (number * dir [2] / (max));

			Vertex ver = newMesh.addVertex (d [0], d [1], d [2]);
			f.addInnerVertex (ver);
			return ver.getHandleNumber ();
		}

		//calculatess the middlePoint of to Vertices
		public double[] middlePoint (int v1, int v2, Mesh newMesh)
		{
			double[] d = new double[3];

			Vertex vertex1 = newMesh.getVertexAt (v1);
			Vertex vertex2 = newMesh.getVertexAt (v2);

			d [0] = (vertex1.getX () + vertex2.getX ()) / 2;
			d [1] = (vertex1.getY () + vertex2.getY ()) / 2;
			d [2] = (vertex1.getZ () + vertex2.getZ ()) / 2;

			return d;
		}

		//Calculates the direction from f to t
		public double[] direction (int f, int t, Mesh newMesh)
		{
			Vertex from = newMesh.getVertexAt (f);
			Vertex to = newMesh.getVertexAt (t);
			double[] p = new double [3];
			p [0] = to.getX () - from.getX ();
			p [1] = to.getY () - from.getY ();
			p [2] = to.getZ () - from.getZ ();
			return p;
		}

		//Calculates the direction from v to p
		public double[] direction (int v, double[] p,Mesh newMesh)
		{
			Vertex vertex = newMesh.getVertexAt (v);

			double[] d = new double[3];
			d [0] = p [0] - vertex.getX ();
			d [1] = p [1] - vertex.getY ();
			d [2] = p [2] - vertex.getZ ();

			return d;
		}

		//returns the position of the first non trivial edge (has vertices on edge)
		public int getAlphaPos(List<SyntheticEdge> edges){
			int alphaPos = 0;
			int count =0;
			int max = 0;
			foreach (SyntheticEdge e in edges) {
				if (e.getLength () > max) {
					alphaPos = count;
					max = e.getLength ();
				}
				count++;
			}
			return alphaPos;
		}

		public void fillMatrixwithFaces (int[,] VertexMatrix,Face f,Mesh newMesh){
			int length0 = VertexMatrix.GetLength (0);
			int length1 = VertexMatrix.GetLength (1);

			for (int i = 1; i < length0 - 1; i++) {
				double[] pos = newMesh.getVertexAt (VertexMatrix [i, 0]).getPosition ();
				double[] dir = direction(VertexMatrix[i,0],VertexMatrix[i,length1-1],newMesh);
				for (int j = 1; j < length1 - 1; j++) {
					int v = createVertex (pos, dir, length1, j, f, newMesh);
					VertexMatrix [i, j] = v;
					createFace (VertexMatrix [i - 1, j - 1], VertexMatrix [i, j - 1], VertexMatrix [i, j], VertexMatrix [i - 1, j],newMesh);
				}
				createFace (VertexMatrix [i - 1, length1 - 2], VertexMatrix [i, length1 - 2], VertexMatrix [i, length1-1], VertexMatrix [i - 1, length1-1],newMesh);
			}
			for (int j = 1; j < length1; j++) {
				createFace (VertexMatrix [length0 - 2, j-1], VertexMatrix [length0-1, j-1], VertexMatrix [length0-1, j], VertexMatrix [length0 - 2, j],newMesh);
			}
		}

		public int getSize ()
		{
			return size;
		}

		public int getPatternNumber ()
		{
			return patternNumber;
		}

		public int getAlpha ()
		{
			return alpha;
		}

		public int getBeta ()
		{
			return beta;
		}

	}
}

