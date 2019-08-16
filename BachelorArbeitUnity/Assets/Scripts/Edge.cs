using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Edge
    {
        private HalfEdge h1;
        private HalfEdge h2;
        private Face f1;
        private Face f2;
        private Vertex v1;
        private Vertex v2;
        private int sepNumber;
        private MeshStruct m;
        private GameObject EdgeObject;

        private Vertex[] verticesOnEdge;
        private Vertex newV1;
        private Vertex newV2;

        private Edge symEdge;

        private int handleNumber;

        public Edge(Vertex v1, Vertex v2, Face f, MeshStruct m)
        {
            setV1(v1);
            setV2(v2);
            v1.addEdge(this);
            v2.addEdge(this);
            f1 = f;
            sepNumber = -1;

            this.m = m;
        }

        public Edge(String isEmpty)
        {
            if (isEmpty.Equals("empty"))
            {
                handleNumber = -1;
            }
        }

        //tests if this vertex connects v to any other vertex
        public Boolean contains(Vertex v)
        {
            if (v == v1 || v == v2)
                return true;
            return false;
        }

        //adds face to the list of faces of this edge
        //also adds the vertices in different order to the edges
        public void addFace(Face f, MeshStruct m)
        {
            f2 = f;
        }

        public void addHalfEdge(HalfEdge he)
        {
            if (h1 == null || !h1.isValid())
                h1 = he;
            else if (h2 == null || !h2.isValid())
                h2 = he;
            else
                throw new Exception("This Edge already has 2 Halfedges");
        }

        //deletes this edge and concatinates the to faces previously serperated by this edge
        public void delete()
        {
            handleNumber = -1;
            if (f1 != null && f1.isValid())
            {
                f1.delete();
            }
            if (f2 != null && f2.isValid())
            {
                f2.delete();
            }
            if (h1 != null && h1.isValid())
            {
                h1.delete();
            }
            if (h2 != null && h2.isValid())
            {
                h2.delete();
            }
            if (verticesOnEdge != null)
            {
                foreach (Vertex v in verticesOnEdge)
                {
                    if (v != null && v.isValid())
                    {
                        v.delete();
                    }
                }
            }
            if (v1 != null && v1.isValid())
            {
                v1.remEdge(this);
            }
            if (v2 != null && v2.isValid())
            {
                v2.remEdge(this);
            }
        }

        public void remHalfEdge(HalfEdge he)
        {
            if ((h1 == null || !h1.isValid()) && (h2 == null || !h2.isValid()))
            {
                delete();
            }
        }

        //returns the Vector from Vertex v1 to Vertex v2
        public Vector3 getDirection()
        {
            return v2.getPosition() - v1.getPosition();
        }

        public Vector3 getDirectionFrom(Vertex v)
        {
            if (contains(v))
            {
                double[] d = new double[3];
                if (v.Equals(v1))
                {
                    return getDirection();
                }
                else
                {
                    return (-1) * getDirection();
                }
            }
            throw new Exception("The Edge " + this + " does not contain " + v);
        }

        //returns the halfedge corresponding to the given face
        public HalfEdge getHalfEdge(Face f)
        {
            if (f.getHandleNumber() == h1.getF().getHandleNumber())
                return h1;
            else if (f.getHandleNumber() == h2.getF().getHandleNumber())
                return h2;
            return null;
        }

        //adds points on the edge to the edge and corresponding halfedges in their respective order
        public void setVerticesOnEdge(Vertex[] voe)
        {
            if (voe.Length > 0 && voe[0].distanceTo(v1) > voe[0].distanceTo(v2))
            {
                Vertex[] turnedAround = new Vertex[voe.Length];
                for (int i = 0; i < turnedAround.Length; i++)
                {
                    turnedAround[turnedAround.Length - i - 1] = voe[i];
                }
                voe = turnedAround;
            }
            verticesOnEdge = voe;
            h1.setVerticesOnEdge(voe);

            if (h2 != null)
            {
                h2.setVerticesOnEdge(voe);
            }
        }

        public void increaseSepNumber()
        {
            sepNumber += 1;
        }

        public void decreaseSepNumber()
        {
            if (sepNumber > 1) {
                sepNumber -= 1;
            }
        }

        //Calculates the direction from v1 to v2
        public Vector3 direction()
        {
            return v2.getPosition() - v1.getPosition();
        }

        //calculatess the middlePoint of the two Vertices
        public Vector3 middlePoint()
        {
            return (v1.getPosition() + v2.getPosition()) / 2;
        }

        public String printVertices()
        {
            String output = "";
            output += v1;
            foreach (Vertex v in verticesOnEdge)
            {
                output += v;
            }
            output += v2;
            return output;
        }

        public void resetValues()
        {
            verticesOnEdge = null;
            newV1 = null;
            newV2 = null;
        }
        
        //returns if the edge is valid or deleted
        public Boolean isValid()
        {
            return handleNumber >= 0;
        }

        public Vertex getV1()
        {
            return v1;
        }

        public Vertex getV2()
        {
            return v2;
        }

        public Face getF1()
        {
            return f1;
        }

        public Face getF2()
        {
            return f2;
        }

        public HalfEdge getH1()
        {
            return h1;
        }

        public HalfEdge getH2()
        {
            return h2;
        }

        public Vertex getNewV1()
        {
            return newV1;
        }

        public Vertex getNewV2()
        {
            return newV2;
        }

        public int getHandleNumber()
        {
            return handleNumber;
        }

        public int getSepNumber()
        {
            return sepNumber;
        }

        public Vertex[] getVerticesOnEdge()
        {
            return verticesOnEdge;
        }

        public Edge getSymEdge()
        {
            return symEdge;
        }

        public void setSepNumber(int sepNumber)
        {
            this.sepNumber = sepNumber;
        }

        public void setV1(Vertex v)
        {
            this.v1 = v;
        }

        public void setV2(Vertex v)
        {
            this.v2 = v;
        }

        public void setF1(Face f)
        {
            this.f1 = f;
        }

        public void setF2(Face f)
        {
            this.f2 = f;
        }

        public void setNewV1(Vertex v)
        {
            this.newV1 = v;
        }

        public void setNewV2(Vertex v)
        {
            this.newV2 = v;
        }

        public void setHandleNumber(int handleNumber)
        {
            this.handleNumber = handleNumber;
        }

        public void setSymEdge(Edge e)
        {
            symEdge = e;
            if (e.getSymEdge() == null)
            {
                e.setSymEdge(this);
            }
        }

        public override string ToString()
        {
            return ("Edge "+handleNumber+"\n from  " + v1 + "\n to    " + v2 + "\n SeperationNumber: " + sepNumber + "\n");
        }

        public bool Equals(Edge e)
        {
            return this.getHandleNumber() == e.getHandleNumber();
        }
    }
}

