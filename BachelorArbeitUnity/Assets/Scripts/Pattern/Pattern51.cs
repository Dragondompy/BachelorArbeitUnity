using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern51 : EmptyPattern
    {
        public Pattern51(int alpha, int beta)
        {
            size = 5;
            patternNumber = 1;
            this.alpha = alpha;
            this.beta = beta;
        }

        public override void createFaces(MeshStruct newMesh, Face f, List<List<int>> innerListVertices)
        {
            List<SyntheticEdge> edges = createSyntheticEdges(innerListVertices);

            int alphaPos = getAlphaPos(edges);
            SyntheticEdge alphaEdge = edges[alphaPos];
            SyntheticEdge betaEdge;

            if (edges[(alphaPos + 1) % edges.Count].getLength() > 2)
            {
                betaEdge = edges[(alphaPos + 1) % edges.Count];
                mirrored = false;
            }
            else
            {
                betaEdge = edges[(alphaPos - 1 + edges.Count) % edges.Count];
                mirrored = true;
            }

            int x = alphaEdge.getLength() - 3;

            int v1, v2, v3, v4;
            if (!mirrored)
            {
                v1 = alphaEdge.getV1();
                v2 = betaEdge.getV2();
                v3 = edges[(alphaPos - 1 + edges.Count) % edges.Count].getV1();
                v4 = edges[(alphaPos - 2 + edges.Count) % edges.Count].getV1();
            }
            else
            {
                v1 = alphaEdge.getV2();
                v2 = betaEdge.getV1();
                v3 = edges[(alphaPos + 1) % edges.Count].getV2();
                v4 = edges[(alphaPos + 2) % edges.Count].getV2();
            }

            createFace(v1, v2, v4, v3, newMesh);

            for (int i = 1; i <= x; i++)
            {

                int nv1, nv2;
                if (!mirrored)
                {
                    nv1 = alphaEdge.getVerticesOnEdge()[i - 1];
                    nv2 = betaEdge.getVerticesOnEdge()[betaEdge.getLength() - 2 - i];
                }
                else
                {
                    nv1 = alphaEdge.getVerticesOnEdge()[alphaEdge.getLength() - 2 - i];
                    nv2 = betaEdge.getVerticesOnEdge()[i - 1];
                }
                createFace(v1, nv1, nv2, v2, newMesh);

                v1 = nv1;
                v2 = nv2;
            }

            if (!mirrored)
            {
                createFace(v1, alphaEdge.getVerticesOnEdge()[x], alphaEdge.getV2(), v2, newMesh);
            }
            else
            {
                createFace(v1, alphaEdge.getVerticesOnEdge()[0], alphaEdge.getV1(), v2, newMesh);
            }
        }
    }
}

