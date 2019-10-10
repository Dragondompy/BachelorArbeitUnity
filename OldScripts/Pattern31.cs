using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern31 : EmptyPattern
	{
		public Pattern31 (int alpha, int beta)
		{
			size = 3;
			patternNumber = 1;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f, List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos(edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int r = alphaEdge.getV1 ();
			int l = alphaEdge.getV2 ();
			int m = edges [(alphaPos + 1) % edges.Count].getV2 ();

			double[] pos = newMesh.getVertexAt (m).getPosition ();
			double[] dir = direction (m, middlePoint (r, l, newMesh), newMesh);

			int x = (alphaEdge.getLength () - 5) / 2;

			for (int i = 1; i <= x+1 ; i++) {
				int v = createVertex(pos,dir,x+2,i,f,newMesh);

				int nl = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];
				int nr = alphaEdge.getVerticesOnEdge () [i - 1];

				createFace (l, nl, v, m, newMesh);
				createFace (r, m, v, nr, newMesh);

				l = nl;
				m = v;
				r = nr;
			}

			int nv = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () / 2 - 1];

			createFace (l, nv, r, m, newMesh);
		}
	}
}

