using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern52 : EmptyPattern
	{
		public Pattern52 (int alpha,int beta)
		{
			size = 5;
			patternNumber = 2;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (MeshStruct newMesh, Face f,List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos(edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int x = (alphaEdge.getLength () - 5) / 2;

			int l = alphaEdge.getV1 ();
			int r = alphaEdge.getV2 ();

			int p1 = edges [(alphaPos - 1 + edges.Count) % edges.Count].getV1 ();
			int p2 = edges [(alphaPos - 2 + edges.Count) % edges.Count].getV1 ();
			int p3 = edges [(alphaPos - 3 + edges.Count) % edges.Count].getV1 ();

			Vector3 posV2 = newMesh.getVertexAt (p2).getPosition ();
			Vector3 dMiddle = direction (p2, middlePoint (r, l, newMesh), newMesh);

			int m = createVertex (posV2, dMiddle, x + 1, 1, f, newMesh);

			createFace (p1, l, m, p2, newMesh);
			createFace (p3, p2, m, r, newMesh);

			Vector3 pos = newMesh.getVertexAt (m).getPosition ();
			Vector3 dir = direction (m, middlePoint (r, l, newMesh), newMesh);

			for (int i = 1; i <= x +1; i++) {
				int v = createVertex(pos,dir,x+3,i+1,f,newMesh);

				int nl = alphaEdge.getVerticesOnEdge () [i - 1];
				int nr = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];

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

