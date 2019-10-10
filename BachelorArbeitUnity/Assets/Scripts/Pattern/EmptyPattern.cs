using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class EmptyPattern
    {
        protected int size;
        protected int patternNumber;
        protected int alpha;
        protected int beta;
        protected Face patch;

        protected bool mirrored;

        public EmptyPattern()
        {
        }

        public virtual void createFaces(MeshStruct newMesh, Face f, List<List<int>> innerListVertices)
        {

        }

        //Creates Synthetic edges as new datastructure for the pattern
        public List<SyntheticEdge> createSyntheticEdges(List<List<int>> innerVertices)
        {
            List<SyntheticEdge> edges = new List<SyntheticEdge>();

            foreach (List<int> l in innerVertices)
            {
                SyntheticEdge e = new SyntheticEdge();
                e.setV1(l[0]);
                e.setV2(l[l.Count - 1]);
                List<int> vOE = new List<int>();

                for (int i = 1; i < l.Count - 1; i++)
                {
                    vOE.Add(l[i]);
                }
                e.setVerticesOnEdge(vOE);
                edges.Add(e);
            }
            return edges;
        }

        //creates new Quad with vertices 1,2,3,4
        public Face createFace(int v1, int v2, int v3, int v4, MeshStruct newMesh)
        {
            List<int> f = new List<int>();
            if (mirrored)
            {
                f.Add(v1);
                f.Add(v4);
                f.Add(v3);
                f.Add(v2);
            }
            else
            {
                f.Add(v1);
                f.Add(v2);
                f.Add(v3);
                f.Add(v4);
            }

            Face newF = newMesh.addSimpleFace(f);
            patch.addInnerFace(newF);
            return newF;
        }

        //Creates new Vertex at with position at pos + (number/max)*dir
        public int createVertex(Vector3 pos, Vector3 dir, int max, int number, Face f, MeshStruct newMesh)
        {
            Vector3 d = pos + (number * dir / max);

            Vertex ver = newMesh.addVertex(pos);
            f.addInnerVertex(ver);
            return ver.getHandleNumber();
        }

        //calculatess the middlePoint of to Vertices
        public Vector3 middlePoint(int v1, int v2, MeshStruct newMesh)
        {
            Vertex vertex1 = newMesh.getVertexAt(v1);
            Vertex vertex2 = newMesh.getVertexAt(v2);

            return (vertex1.getPosition() + vertex2.getPosition()) / 2;
        }

        //calculatess the middlePoint of two Vertices
        public Vector3 middlePoint(Vertex v1, Vertex v2)
        {
            return (v1.getPosition() + v2.getPosition()) / 2;
        }

        //Calculates the direction from f to t
        public Vector3 direction(int f, int t, MeshStruct newMesh)
        {
            Vertex from = newMesh.getVertexAt(f);
            Vertex to = newMesh.getVertexAt(t);

            return to.getPosition() - from.getPosition();
        }

        //Calculates the direction from f to t
        public Vector3 direction(Vertex f, Vertex t)
        {
            return t.getPosition() - f.getPosition();
        }

        //Calculates the direction from v to p
        public Vector3 direction(int v, Vector3 p, MeshStruct newMesh)
        {
            Vertex vertex = newMesh.getVertexAt(v);
            return p - vertex.getPosition();
        }

        //returns the position of the first non trivial edge (has vertices on edge)
        public int getAlphaPos(List<SyntheticEdge> edges)
        {
            int alphaPos = 0;
            int count = 0;
            int max = 0;
            foreach (SyntheticEdge e in edges)
            {
                if (e.getLength() > max)
                {
                    alphaPos = count;
                    max = e.getLength();
                }
                count++;
            }
            return alphaPos;
        }

        public void fillMatrixwithFaces(int[,] VertexMatrix, Face f, MeshStruct newMesh)
        {
            int length0 = VertexMatrix.GetLength(0);
            int length1 = VertexMatrix.GetLength(1);

            for (int i = 1; i < length0 - 1; i++)
            {
                Vector3 pos = newMesh.getVertexAt(VertexMatrix[i, 0]).getPosition();
                Vector3 dir = direction(VertexMatrix[i, 0], VertexMatrix[i, length1 - 1], newMesh);
                for (int j = 1; j < length1 - 1; j++)
                {
                    int v = createVertex(pos, dir, length1, j, f, newMesh);
                    VertexMatrix[i, j] = v;
                    //createFace (VertexMatrix [i - 1, j - 1], VertexMatrix [i, j - 1], VertexMatrix [i, j], VertexMatrix [i - 1, j],newMesh);
                    createFace(VertexMatrix[i - 1, j - 1], VertexMatrix[i - 1, j], VertexMatrix[i, j], VertexMatrix[i, j - 1], newMesh);
                }
                //createFace(VertexMatrix[i - 1, length1 - 2], VertexMatrix[i, length1 - 2], VertexMatrix[i, length1 - 1], VertexMatrix[i - 1, length1 - 1], newMesh);
                createFace(VertexMatrix[i - 1, length1 - 2], VertexMatrix[i - 1, length1 - 1], VertexMatrix[i, length1 - 1], VertexMatrix[i, length1 - 2], newMesh);
            }
            for (int j = 1; j < length1; j++)
            {
                //createFace(VertexMatrix[length0 - 2, j - 1], VertexMatrix[length0 - 1, j - 1], VertexMatrix[length0 - 1, j], VertexMatrix[length0 - 2, j], newMesh);
                createFace(VertexMatrix[length0 - 2, j - 1], VertexMatrix[length0 - 2, j], VertexMatrix[length0 - 1, j], VertexMatrix[length0 - 1, j - 1], newMesh);
            }
        }

        public int getSize()
        {
            return size;
        }

        public int getPatternNumber()
        {
            return patternNumber;
        }

        public int getAlpha()
        {
            return alpha;
        }

        public int getBeta()
        {
            return beta;
        }

    }
}

