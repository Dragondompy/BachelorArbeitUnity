using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Vertex : MonoBehaviour
    {
        private int handleNumber;
        private Mesh m;

        private List<Edge> edges;

        public void loadVertex(Mesh m)
        {
            edges = new List<Edge>();
            gameObject.transform.localScale = new Vector3(m.getSize(), m.getSize(), m.getSize());
            this.m = m;
        }

        //returns edge that connects this vertex to vertex v, returns null if there is no connection
        public Edge isConnected(Vertex v)
        {
            foreach (Edge e in edges)
            {
                if (e.contains(v))
                {
                    return e;
                }
            }
            return null;
        }

        //adds edge to the edge list of this vertex
        public void addEdge(Edge e)
        {
            edges.Add(e);
        }

        //deletes this vertex and all connected edges
        public void delete()
        {
            handleNumber = -1;
            foreach (Edge e in edges)
            {
                e.delete();
            }
        }

        public double distanceTo(Vertex v)
        {

            return (getPosition() - v.getPosition()).magnitude;
        }

        //returns if the vertex is valid or deleted
        public Boolean isValid()
        {
            return handleNumber >= 0;
        }

        public Vector3 getPosition()
        {
            return gameObject.transform.position;
        }

        public List<Edge> getEdges()
        {
            return edges;
        }

        public int getHandleNumber()
        {
            return handleNumber;
        }

        public void setPosition(Vector3 pos)
        {

            gameObject.transform.position = pos;

            m.updateEdges(edges);
        }

        public void setHandleNumber(int handleNumber)
        {
            this.handleNumber = handleNumber;
        }

        public override string ToString()
        {
            return "Vertex at: " + getPosition().x + "| " + getPosition().y + "| " + getPosition().z + "    hN: " + handleNumber;
        }

        public bool Equals(Vertex v)
        {
            return this.getHandleNumber() == v.getHandleNumber();
        }
    }
}

