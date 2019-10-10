using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern60 : EmptyPattern
	{
		public Pattern60 (int alpha,int beta)
		{
			size = 6;
			patternNumber = 0;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f,List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];
			SyntheticEdge betaEdge = edges [(alphaPos + 3)%edges.Count];

			int oben = alphaEdge.getV2 ();
			int unten = betaEdge.getV1 ();
			int m = edges [(alphaPos + 1) % edges.Count].getV2 ();
			int opp = edges [(alphaPos - 1+edges.Count) % edges.Count].getV1 ();

			double[] pos = newMesh.getVertexAt (m).getPosition ();
			double[] dir = direction(m,opp,newMesh);

			int x = alphaEdge.getLength ()-2;

			for (int i = 1; i <= x; i++) {
				int v = createVertex(pos,dir,x+1,i,f,newMesh);

				int nUnten = betaEdge.getVerticesOnEdge ()[i-1];
				int nOben = alphaEdge.getVerticesOnEdge ()[x-i];

				createFace (m, v, nOben, oben, newMesh);
				createFace (m, unten, nUnten, v, newMesh);

				unten = nUnten;
				oben = nOben;
				m = v;
			}
			createFace (m, opp, alphaEdge.getV1 (), oben, newMesh);
			createFace (m, unten, betaEdge.getV2 (), opp, newMesh);
		}
	}
}

