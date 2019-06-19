using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern50 : EmptyPattern
	{
		public Pattern50 (int alpha,int beta)
		{
			size = 5;
			patternNumber = 0;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f,List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int m = edges [(alphaPos + 2) % edges.Count].getV2 ();

			int v = alphaEdge.getVerticesOnEdge () [0];
			int ur = alphaEdge.getV2 ();
			int or = edges [(alphaPos + 1) % edges.Count].getV2 ();
			int ul = alphaEdge.getV1 ();
			int ol = edges [(alphaPos - 1) % edges.Count].getV1 ();

			createFace (m, ol, ul, v, newMesh);
			createFace (m, v, ur, or, newMesh);
		}
	}
}

