using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
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

		public override void createFaces (MeshStruct newMesh, Face f, List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos(edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int r = alphaEdge.getV1 ();
			int l = alphaEdge.getV2 ();
			int m = edges [(alphaPos + 1) % edges.Count].getV2 ();

			Vector3 pos = newMesh.getVertexAt (m).getPosition ();
            Vector3 dir = direction (m, middlePoint (r, l, newMesh), newMesh);

			int x = (alphaEdge.getLength () - 5) / 2;

			for (int i = 1; i <= x+1 ; i++) {
				int v = createVertex(pos,dir,x+2,i,f,newMesh);

				int nl = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];
				int nr = alphaEdge.getVerticesOnEdge () [i - 1];

				createFace (l, m, v, nl, newMesh);
				createFace (r, nr, v, m, newMesh);

				l = nl;
				m = v;
				r = nr;
			}

			int nv = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () / 2 - 1];

			createFace (l, m, r, nv, newMesh);
		}
	}
}

