using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern44 : EmptyPattern
	{
		public Pattern44 (int alpha, int beta)
		{
			size = 4;
			patternNumber = 4;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (MeshStruct newMesh, Face f, List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);
			Boolean mirrored;

			int alphaPos = getAlphaPos (edges);
			SyntheticEdge alphaEdge = edges [alphaPos];
			SyntheticEdge betaEdge;

			if (edges [(alphaPos + 1) % edges.Count].getLength () > 2) {
				betaEdge = edges [(alphaPos + 1) % edges.Count];
				mirrored = false;
			} else {
				betaEdge = edges [(alphaPos - 1 + edges.Count) % edges.Count];
				mirrored = true;
			}

			int y = betaEdge.getLength () - 3; 
			int x = (alphaEdge.getLength () - 4 - y) / 2;

			int ol, ul, ur, or, mr;
			if (!mirrored) {
				ol = edges [(alphaPos - 1 + edges.Count) % edges.Count].getV1 ();
				ul = alphaEdge.getV1 ();
				ur = alphaEdge.getV2 ();
				or = betaEdge.getV2 ();
				mr = betaEdge.getVerticesOnEdge () [0];
			} else {
				ol = edges [(alphaPos + 1) % edges.Count].getV2 ();
				ul = alphaEdge.getV2 ();
				ur = alphaEdge.getV1 ();
				or = betaEdge.getV1 ();
				mr = betaEdge.getVerticesOnEdge () [betaEdge.getLength () - 3];
			}

			int[,] VertexMatrix = new int[x+3,y+2];
			VertexMatrix [x+2, 0] = or;
			VertexMatrix [x+2, y+1] = mr;

			Vector3 pos1 = newMesh.getVertexAt (ol).getPosition ();
			Vector3 pos2 = newMesh.getVertexAt (or).getPosition ();
			Vector3 pos3 = newMesh.getVertexAt (mr).getPosition ();
			Vector3 dir1 = direction (ol, middlePoint (ul, ur, newMesh), newMesh);
			Vector3 dir2 = direction (or, middlePoint (ul, ur, newMesh), newMesh);
			Vector3 dir3 = direction (mr, middlePoint (ul, ur, newMesh), newMesh);
			for (int i = 1; i <= x+1; i++) {
				int v1 = createVertex(pos1,dir1,x+2,i,f,newMesh);
				int v2 = createVertex(pos2,dir2,x+2,i,f,newMesh);
				int v3 = createVertex(pos3,dir3,x+2,i,f,newMesh);

				int nul, nur;
				if (!mirrored) {
					nul = alphaEdge.getVerticesOnEdge () [i - 1];
					nur = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];
				} else {
					nul = alphaEdge.getVerticesOnEdge () [alphaEdge.getLength () - 2 - i];
					nur = alphaEdge.getVerticesOnEdge () [i - 1];
				}
				createFace (ul, nul, v1, ol, newMesh);
				createFace (ol, v1, v2, or, newMesh);
				createFace (nur, ur, mr, v3, newMesh);

				ul = nul;
				ur = nur;
				ol = v1;
				or = v2;
				mr = v3;

				VertexMatrix [x+2 - i, 0] = v2;
				VertexMatrix [x+2 - i, y+1] = v3;
			}

			int v4, v5;

			if (!mirrored) {
				v4 = alphaEdge.getVerticesOnEdge () [x+1];
				v5 = alphaEdge.getVerticesOnEdge () [x + y+2];
			} else {
				v4 = alphaEdge.getVerticesOnEdge () [x + y+1];
				v5 = alphaEdge.getVerticesOnEdge () [x];
			}
			createFace (ul, v4, or, ol, newMesh);

			VertexMatrix [0, 0] = v4;
			VertexMatrix [0, y+1] = v5;

			for (int i = 1; i <= y; i++) {
				if(!mirrored){
					VertexMatrix [0, i] = alphaEdge.getVerticesOnEdge () [x+1 + i];
					VertexMatrix [x+2, i] = betaEdge.getVerticesOnEdge () [y+1-i];
				}
				else{
					VertexMatrix [0, i] = alphaEdge.getVerticesOnEdge () [x+y+1-i];
					VertexMatrix [x+2, i] = betaEdge.getVerticesOnEdge () [i-1];
				}
			}

			fillMatrixwithFaces (VertexMatrix, f, newMesh);
		}
	}
}

