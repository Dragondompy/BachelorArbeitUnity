using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern43 : EmptyPattern
	{
		public Pattern43 (int alpha, int beta)
		{
			size = 4;
			patternNumber = 3;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f, List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];

			int x = (alphaEdge.getLength () - 3) / 2;

			int ol = edges [(alphaPos - 1 + edges.Count) % edges.Count].getV1 ();
			int ul = alphaEdge.getV1 ();
			int ur = alphaEdge.getV2 ();
			int or = edges [(alphaPos + 1 + edges.Count) % edges.Count].getV2 ();

			double[] pos1 = newMesh.getVertexAt (ol).getPosition ();
			double[] pos2 = newMesh.getVertexAt (or).getPosition ();
			double[] dir1 = direction (ol, middlePoint (ul, ur, newMesh), newMesh);
			double[] dir2 = direction (or, middlePoint (ul, ur, newMesh), newMesh);

			for (int i = 1; i <= x+1; i++) {
				int v1 = createVertex(pos1,dir1,x+2,i,f,newMesh);
				int v2 = createVertex(pos2,dir2,x+2,i,f,newMesh);

				int nul = alphaEdge.getVerticesOnEdge () [i - 1];
				int nur = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];

				createFace (ul, nul, v1, ol, newMesh);
				createFace (ol, v1, v2, or, newMesh);
				createFace (ur, or, v2, nur, newMesh);

				ul = nul;
				ur = nur;
				ol = v1;
				or = v2;
			}
			createFace (ul, ur, or, ol, newMesh);

		}
	}
}

