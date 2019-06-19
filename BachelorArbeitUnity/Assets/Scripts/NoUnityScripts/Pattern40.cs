using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
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

		public override void createFaces (Mesh newMesh, Face f,List<List<int>> innerListVertices)
		{
			List<SyntheticEdge> edges = createSyntheticEdges (innerListVertices);
			List<int> faceList = new List<int> ();
			foreach (SyntheticEdge e in edges) {
				faceList.Add (e.getV1());
				foreach (int i in e.getVerticesOnEdge()) {
					faceList.Add (i);
				}
			}
			newMesh.addSimpleFace (faceList);
		}
	}
}

