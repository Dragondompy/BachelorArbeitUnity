using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern40 : EmptyPattern
	{
		public Pattern40 (int alpha,int beta)
		{
			size = 4;
			patternNumber = 0;
			this.alpha = alpha;
			this.beta = beta;
		}

		public override void createFaces (MeshStruct newMesh, Face f,List<List<int>> innerListVertices)
        {
            patch = f;
            List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);
			List<int> faceList = new List<int> ();
            int v1, v2, v3, v4;
            v1 = edges[0].getV1();
            v2 = edges[1].getV1();
            v3 = edges[2].getV1();
            v4 = edges[3].getV1();

            createFace(v1, v2, v3, v4, newMesh);
		}
	}
}

