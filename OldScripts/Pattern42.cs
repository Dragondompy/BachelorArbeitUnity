using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern42 : EmptyPattern
	{
		public Pattern42 (int alpha, int beta)
		{
			size = 4;
			patternNumber = 2;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f, List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int ol = edges [(alphaPos - 1 + edges.Count) % edges.Count].getV1 ();
			int ul = alphaEdge.getV1 ();
			int ur = alphaEdge.getV2 ();
			int or = edges [(alphaPos + 1 + edges.Count) % edges.Count].getV2 ();

			int l = alphaEdge.getVerticesOnEdge() [0];
			int r = alphaEdge.getVerticesOnEdge() [1];

			createFace (ul, l, or, ol, newMesh);
			createFace (l, r, ur, or, newMesh);
		}
	}
}

