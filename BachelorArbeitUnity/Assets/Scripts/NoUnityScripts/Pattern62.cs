using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Pattern62 : EmptyPattern
	{
		public Pattern62 (int alpha,int beta)
		{
			size = 6;
			patternNumber = 2;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (Mesh newMesh, Face f,List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];
			SyntheticEdge betaEdge;

			betaEdge = edges [(alphaPos + 3) % edges.Count];

			int y = betaEdge.getLength () - 2; 
			int x = (alphaEdge.getLength () - 4 - y) / 2;

			int v1,v2,v3,v4,v5,v6;
			v1 = alphaEdge.getV1();
			v2 = edges[(alphaPos-1+edges.Count)%edges.Count].getV1();
			v3 = betaEdge.getV2 ();
			v4 = betaEdge.getV1 ();
			v5 = edges[(alphaPos+1)%edges.Count].getV2();
			v6 = alphaEdge.getV2 ();

			int[,] VertexMatrix = new int[x+4,y+2];
			VertexMatrix [x+3, 0] = v3;
			VertexMatrix [x+3, y+1] = v4;

			double[] pos1 = newMesh.getVertexAt (v2).getPosition ();
			double[] pos2 = newMesh.getVertexAt (v3).getPosition ();
			double[] pos3 = newMesh.getVertexAt (v4).getPosition ();
			double[] pos4 = newMesh.getVertexAt (v5).getPosition ();
			double[] dir1 = direction (v2, middlePoint (v1, v6, newMesh), newMesh);
			double[] dir2 = direction (v3, middlePoint (v1, v6, newMesh), newMesh);
			double[] dir3 = direction (v4, middlePoint (v1, v6, newMesh), newMesh);
			double[] dir4 = direction (v5, middlePoint (v1, v6, newMesh), newMesh);
			for (int i = 1; i <= x+1; i++) {
				int nv2 = createVertex(pos1,dir1,x+2,i,f,newMesh);
				int nv3 = createVertex(pos2,dir2,x+2,i,f,newMesh);
				int nv4 = createVertex(pos3,dir3,x+2,i,f,newMesh);
				int nv5 = createVertex(pos4,dir4,x+2,i,f,newMesh);

				int nv1 = alphaEdge.getVerticesOnEdge () [i - 1];
				int nv6 = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];

				createFace (v1, nv1, nv2, v2, newMesh);
				createFace (v2, nv2, nv3, v3, newMesh);
				createFace (v4, nv4, nv5, v5, newMesh);
				createFace (v5, nv5, nv6, v6, newMesh);

				v1 = nv1;
				v2 = nv2;
				v3 = nv3;
				v4 = nv4;
				v5 = nv5;
				v6 = nv6;

				VertexMatrix [x+3 - i, 0] = v3;
				VertexMatrix [x+3 - i, y+1] = v4;
			}

			VertexMatrix [1, 0] = v2;
			VertexMatrix [1, y+1] = v5;
			VertexMatrix [0, 0] = v1;
			VertexMatrix [0, y+1] = v6;

			for (int i = 1; i <= y; i++) {
				VertexMatrix [0, i] = alphaEdge.getVerticesOnEdge () [x+ i];
				VertexMatrix [x+3, i] = betaEdge.getVerticesOnEdge () [y-i];
			}

			fillMatrixwithFaces (VertexMatrix, f, newMesh);
		}
	}
}

