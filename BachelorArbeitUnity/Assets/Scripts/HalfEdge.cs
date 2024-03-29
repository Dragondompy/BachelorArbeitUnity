﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class HalfEdge
    {
        private Vertex v1;
        private Vertex v2;
        private Face f;
        private Edge e;
        private int handleNumber;

        private int outerFlowPreset;
        private int outerFlow;
        private int reduced;
        private int[,] additionalVertices;
        private Vertex[] verticesOnEdge;
        private MeshStruct m;

        private HalfEdge symHalfEdge;

        public HalfEdge(Vertex v1, Vertex v2, Edge e, Face f, MeshStruct m)
        {
            setV1(v1);
            setV2(v2);
            outerFlow = 0;
            reduced = -1;
            setE(e);
            setFace(f);

            e.addHalfEdge(this);
            m.addHalfEdge(this);
            this.m = m;
        }

        public Boolean contains(Vertex v)
        {
            return e.contains(v);
        }

        //adds the vertex in an outer flow to the halfedge
        public void addVertex(Vertex v, int flow, int index)
        {
            if (inrange(flow, index))
            {
                additionalVertices[flow, index] = v.getHandleNumber();
            }
        }

        //tests if the vertex in the flow at the position exists
        public int exists(int flow, int index)
        {
            if (inrange(flow, index))
            {
                return additionalVertices[flow, index];
            }
            else
            {
                return -2;
            }
        }

        //tests if the vertex at the position can and should be added to the halfedge
        public Boolean inrange(int flow, int index)
        {
            if (flow < outerFlow && index < getSepNumber() - 1)
                return true;
            return false;

        }

        public void createVerticesArray()
        {
            additionalVertices = new int[outerFlow, getSepNumber() - 1];
            for (int i = 0; i < additionalVertices.GetLength(0); i++)
            {
                for (int j = 0; j < additionalVertices.GetLength(1); j++)
                {
                    additionalVertices[i, j] = -1;
                }
            }
        }

        public void addOuterFlow()
        {
            outerFlow++;
        }

        public void reduceSep()
        {
            if (reduced - 1 < 1)
                throw new Exception("To Much SepNumberReduction");
            reduced--;
        }

        public void reduceSep(int i)
        {
            if (reduced - i < 1)
                throw new Exception("To Much SepNumberReduction");
            reduced -= i;
        }

        public void increaseOuterFlowPreset()
        {
            outerFlowPreset += 1;
        }

        public void decreaseOuterFlowPreset()
        {
            if (outerFlowPreset > 0)
            {
                outerFlowPreset -= 1;
            }
        }

        public String printVertices()
        {
            String output = "";
            output += v1;
            foreach (Vertex v in getVerticesOnEdge())
            {
                output += v;
            }
            output += v2;
            return output;
        }

        public void resetValues()
        {
            reduced = -1;
            outerFlow = 0;
            additionalVertices = null;
        }

        public void switchVertex(Vertex newV, Vertex oldV)
        {
            if (oldV.Equals(v1))
            {
                v1 = newV;
            }
            else if (oldV.Equals(v2))
            {
                v2 = newV;
            }
            f.switchVertex(oldV, newV);
        }

        public Vector3 getDirection()
        {
            return v2.getPosition() - v1.getPosition();
        }

        //returns if the HalfEdge is valid or deleted
        public Boolean isValid()
        {
            return handleNumber >= 0;
        }

        public void delete()
        {
            handleNumber = -1;
            if (e != null)
            {
                e.remHalfEdge(this);
            }
            m.removeHalfEdge(this);
        }

        public Face getF()
        {
            return f;
        }

        public Edge getE()
        {
            return e;
        }

        public Vertex getV1()
        {
            return v1;
        }

        public Vertex getV2()
        {
            return v2;
        }

        public Vertex getNewV1()
        {
            return v1.getRefinedVertex();
        }

        public Vertex getNewV2()
        {
            return v2.getRefinedVertex();
        }

        public int getReduced()
        {
            return reduced;
        }

        public int getOuterFlow()
        {
            return outerFlow;
        }

        public int getOuterFlowPreset()
        {
            return outerFlowPreset;
        }

        public int getHandleNumber()
        {
            return handleNumber;
        }

        public int getSepNumber()
        {
            return getE().getSepNumber();
        }

        public Vertex[] getVerticesOnEdge()
        {
            Vertex[] voe = e.getVerticesOnEdge();
            if (voe.Length > 0 && voe[0].distanceTo(v1) > voe[0].distanceTo(v2))
            {
                Vertex[] turnedAround = new Vertex[voe.Length];
                for (int i = 0; i < turnedAround.Length; i++)
                {
                    turnedAround[turnedAround.Length - i - 1] = voe[i];
                }
                voe = turnedAround;
            }
            return voe;
        }

        public int[,] getAdditionalVertices()
        {
            return additionalVertices;
        }

        public HalfEdge getSymHalfEdge()
        {
            return symHalfEdge;
        }

        public void setFace(Face f)
        {
            this.f = f;
        }

        public void setE(Edge e)
        {
            this.e = e;
        }

        public void setV1(Vertex v)
        {
            this.v1 = v;
        }

        public void setV2(Vertex v)
        {
            this.v2 = v;
        }
        
        public void setReduced(int reduced)
        {
            this.reduced = reduced;
        }

        public void setOuterFlow(int outerFlow)
        {
            this.outerFlow = outerFlow;
        }

        public void setOuterFlowPreset(int outerFlowPreset)
        {
            this.outerFlowPreset = outerFlowPreset;
        }

        public void setHandleNumber(int handleNumber)
        {
            this.handleNumber = handleNumber;
        }

        public void setSymHalfEdge(HalfEdge h)
        {
            symHalfEdge = h;
            if (h.getSymHalfEdge() == null)
            {
                h.setSymHalfEdge(this);
            }
        }

        public override string ToString()
        {
            return "HalfEdge:\n from  " + v1 + "\n to    " + v2 +
            "\n Reduced SepNumber: " + reduced + "\n Amount of Flow: " + outerFlow + "\n";
        }

        public bool Equals(HalfEdge he)
        {
            return this.getHandleNumber() == he.getHandleNumber();
        }
    }
}

