using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Vertex
    {
        private int handleNumber;
        private Vector3 position;
        private GameObject VertexObject;
        private bool isSelected;
        private bool isCreated;

        private List<Edge> edges;

        public Vertex(Vector3 pos)
        {
            edges = new List<Edge>();
            setPosition(pos);
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
            return position;
        }

        public GameObject getVertexObject()
        {
            return VertexObject;
        }

        public bool getIsSelected()
        {
            return isSelected;
        }

        public bool getIsCreated()
        {
            return isCreated;
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
            position = pos;
            //VertexObject.transform.position = pos;

            //m.updateEdges(edges);
        }

        public void setVertexObject(GameObject vo)
        {
            VertexObject = vo;
        }

        public void setIsSelected(bool s)
        {
            if (VertexObject != null)
            {
                GameObject cube = VertexObject.GetComponent<VertexObj>().cube;
                isSelected = s;
                if (isSelected)
                {
                    cube.GetComponent<MeshRenderer>().material = VertexObject.GetComponent<VertexObj>().select;
                }
                else
                {
                    if (isCreated)
                    {
                        cube.GetComponent<MeshRenderer>().material = VertexObject.GetComponent<VertexObj>().created;
                    }
                }
            }
        }

        public void setIsCreated(bool c)
        {
            isCreated = c;
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

