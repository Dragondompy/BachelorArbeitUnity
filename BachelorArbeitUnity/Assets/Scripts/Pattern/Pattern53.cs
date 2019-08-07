using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Pattern53 : EmptyPattern
    {
        public Pattern53(int alpha, int beta)
        {
            size = 5;
            patternNumber = 3;
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

            int y = betaEdge.getLength() - 3;
            int x = (alphaEdge.getLength() - 5 - y) / 2;

            int v1, v2, v3, v4, v5, v6;
            if (!mirrored)
            {
                v1 = alphaEdge.getV1();
                v2 = edges[(alphaPos - 1 + edges.Count) % edges.Count].getV1();
                v3 = edges[(alphaPos - 2 + edges.Count) % edges.Count].getV1();
                v4 = betaEdge.getV2();
                v5 = betaEdge.getVerticesOnEdge()[0];
                v6 = betaEdge.getV1();
            }
            else
            {
                v1 = alphaEdge.getV2();
                v2 = edges[(alphaPos + 1) % edges.Count].getV2();
                v3 = edges[(alphaPos + 2) % edges.Count].getV2();
                v4 = betaEdge.getV1();
                v5 = betaEdge.getVerticesOnEdge()[y];
                v6 = betaEdge.getV2();

            }

            int[,] VertexMatrix = new int[x + 3, y + 2];
            VertexMatrix[x + 2, 0] = v4;
            VertexMatrix[x + 2, y + 1] = v5;

            Vector3 pos1 = newMesh.getVertexAt(v2).getPosition();
            Vector3 pos2 = newMesh.getVertexAt(v3).getPosition();
            Vector3 pos3 = newMesh.getVertexAt(v4).getPosition();
            Vector3 pos4 = newMesh.getVertexAt(v5).getPosition();
            Vector3 dir1 = direction(v2, middlePoint(v1, v6, newMesh), newMesh);
            Vector3 dir2 = direction(v3, middlePoint(v1, v6, newMesh), newMesh);
            Vector3 dir3 = direction(v4, middlePoint(v1, v6, newMesh), newMesh);
            Vector3 dir4 = direction(v5, middlePoint(v1, v6, newMesh), newMesh);
            for (int i = 1; i <= x + 1; i++)
            {
                int nv2 = createVertex(pos1, dir1, x + 2, i, f, newMesh);
                int nv3 = createVertex(pos2, dir2, x + 2, i, f, newMesh);
                int nv4 = createVertex(pos3, dir3, x + 2, i, f, newMesh);
                int nv5 = createVertex(pos4, dir4, x + 2, i, f, newMesh);

                int nv1, nv6;
                if (!mirrored)
                {
                    nv1 = alphaEdge.getVerticesOnEdge()[i - 1];
                    nv6 = alphaEdge.getVerticesOnEdge()[alphaEdge.getLength() - 2 - i];
                }
                else
                {
                    nv1 = alphaEdge.getVerticesOnEdge()[alphaEdge.getLength() - 2 - i];
                    nv6 = alphaEdge.getVerticesOnEdge()[i - 1];
                }
                createFace(v1, nv1, nv2, v2, newMesh);
                createFace(v2, nv2, nv3, v3, newMesh);
                createFace(v3, nv3, nv4, v4, newMesh);
                createFace(v5, nv5, nv6, v6, newMesh);

                v1 = nv1;
                v2 = nv2;
                v3 = nv3;
                v4 = nv4;
                v5 = nv5;
                v6 = nv6;

                VertexMatrix[x + 2 - i, 0] = v4;
                VertexMatrix[x + 2 - i, y + 1] = v5;
            }

            int vx4, vx5;

            if (!mirrored)
            {
                vx4 = alphaEdge.getVerticesOnEdge()[x + 2];
                vx5 = alphaEdge.getVerticesOnEdge()[x + y + 3];
                createFace(v1, alphaEdge.getVerticesOnEdge()[x + 1], v3, v2, newMesh);
                createFace(alphaEdge.getVerticesOnEdge()[x + 1], vx4, v4, v3, newMesh);
            }
            else
            {
                vx4 = alphaEdge.getVerticesOnEdge()[x + y + 1];
                vx5 = alphaEdge.getVerticesOnEdge()[x];
                createFace(v1, alphaEdge.getVerticesOnEdge()[x + y + 2], v3, v2, newMesh);
                createFace(alphaEdge.getVerticesOnEdge()[x + y + 2], vx4, v4, v3, newMesh);
            }

            VertexMatrix[0, 0] = vx4;
            VertexMatrix[0, y + 1] = vx5;

            for (int i = 1; i <= y; i++)
            {
                if (!mirrored)
                {
                    VertexMatrix[0, i] = alphaEdge.getVerticesOnEdge()[x + 2 + i];
                    VertexMatrix[x + 2, i] = betaEdge.getVerticesOnEdge()[y + 1 - i];
                }
                else
                {
                    VertexMatrix[0, i] = alphaEdge.getVerticesOnEdge()[x + y + 1 - i];
                    VertexMatrix[x + 2, i] = betaEdge.getVerticesOnEdge()[i - 1];
                }
            }

            fillMatrixwithFaces(VertexMatrix, f, newMesh);
        }
    }
}

