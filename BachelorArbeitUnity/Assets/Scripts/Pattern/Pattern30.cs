using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern30 : EmptyPattern
	{
		public Pattern30 (int alpha,int beta)
		{
			size = 3;
			patternNumber = 0;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (MeshStruct newMesh, Face f,List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);
			List<int> faceList = new List<int> ();
			foreach (SyntheticEdge e in edges) {
				faceList.Add (e.getV1());
				foreach (int i in e.getVerticesOnEdge()) {
					faceList.Add (i);
				}
            }
            int v1, v2, v3, v4;
            v1 = faceList[0];
            v2 = faceList[1];
            v3 = faceList[2];
            v4 = faceList[3];

            createFace(v1, v2, v3, v4, newMesh);
		}
	}
}

