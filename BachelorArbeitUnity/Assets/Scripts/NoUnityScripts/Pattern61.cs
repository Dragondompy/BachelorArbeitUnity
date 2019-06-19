using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern61 : EmptyPattern
	{
		public Pattern61 (int alpha,int beta)
		{
			size = 6;
			patternNumber = 1;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f,List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];
			SyntheticEdge betaEdge;

			if (edges [(alphaPos + 1) % edges.Count].getLength () > 2) {
				betaEdge = edges [(alphaPos + 1) % edges.Count];
			} else {
				SyntheticEdge h = alphaEdge;
				alphaEdge = edges [(alphaPos - 1 + edges.Count) % edges.Count];
				betaEdge = h;
				alphaPos=(alphaPos - 1 + edges.Count)%edges.Count;
			}

			int x = alphaEdge.getLength () -3;

			int l = alphaEdge.getV1();
			int r = betaEdge.getV2 ();

			int p1 = edges [(alphaPos - 1 + edges.Count) % edges.Count].getV1 ();
			int p2 = edges [(alphaPos - 2 + edges.Count) % edges.Count].getV1 ();
			int p3 = edges [(alphaPos - 3 + edges.Count) % edges.Count].getV1 ();

			double[] posV2 = newMesh.getVertexAt (p2).getPosition ();
			double[] dMiddle = direction (p2, middlePoint (r, l, newMesh), newMesh);
			int m = createVertex(posV2,dMiddle,x+2,1,f,newMesh);

			createFace (p1, l, m, p2, newMesh);
			createFace (p3, p2, m, r, newMesh);

			double[] pos = newMesh.getVertexAt (m).getPosition ();
			double[] dir = direction(m,alphaEdge.getV2(),newMesh);

			for (int i = 1; i <= x+1; i++) {
				int v = createVertex(pos,dir,x+2,i,f,newMesh);

				int nl = alphaEdge.getVerticesOnEdge () [i - 1];
				int nr = betaEdge.getVerticesOnEdge () [x+1 - i];

				createFace (l, nl, v, m, newMesh);

				createFace (r, m, v, nr, newMesh);

				l = nl;
				m = v;
				r = nr;
			}
			int nv = alphaEdge.getV2();

			createFace (l, nv, r, m, newMesh);
		}
	}
}

