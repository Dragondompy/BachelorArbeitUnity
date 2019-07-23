using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern41 : EmptyPattern
	{
		public Pattern41 (int alpha,int beta)
		{
			size = 4;
			patternNumber = 1;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (MeshStruct newMesh, Face f,List<List<int>> innerListVertices)
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
			int m = edges [(alphaPos + 2) % edges.Count].getV2 ();

            Vector3 pos = newMesh.getVertexAt (m).getPosition ();
            Vector3 dir = direction(m,alphaEdge.getV2(),newMesh);

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

