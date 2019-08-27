using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Vertex
    {
        private int handleNumber;
        private Vector3 position;
        private Vector3 newPosition;
        private GameObject VertexObject;
        private bool isSelected;
        private bool isCreated;

        private List<Edge> edges;
        private Vertex symVertex;

        public Vertex(Vector3 pos)
        {
            edges = new List<Edge>();
            setPosition(pos);
        }

        public Vertex(string isEmpty)
        {
            if (isEmpty.Equals("empty"))
            {
                handleNumber = -1;
            }
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
            UnityEngine.Object.Destroy(VertexObject);
            foreach (Edge e in edges)
            {
                if (e != null && e.isValid())
                {
                    e.delete();
                }
            }
            edges.Clear();

            if (symVertex != null)
            {
                symVertex.setSymVertex(null);
            }
        }

        public void removeAllEdges() {
            edges.Clear();
            delete();
        }

        public void remEdge(Edge edge)
        {
            edges.Remove(edge);
            if (edges.Count <= 0)
            {
                delete();
            }
        }

        public double distanceTo(Vertex v)
        {
            return (getPosition() - v.getPosition()).magnitude;
        }

        public void updatePosition()
        {
            setPosition(newPosition);
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

        public Vector3 getNewPosition()
        {
            return newPosition;
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

        public Vertex getSymVertex()
        {
            return symVertex;
        }

        public void setPosition(Vector3 pos)
        {
            position = pos;
            newPosition = pos;
            if (VertexObject != null) {
                VertexObject.transform.position = pos;
            }
        }

        public void setNewPosition(Vector3 pos)
        {
            newPosition = pos;
        }

        public void setVertexObject(GameObject vo)
        {
            VertexObject = vo;
        }

        public void setSymVertex(Vertex v)
        {
            symVertex = v;
            if (v.getSymVertex() == null)
            {
                v.setSymVertex(this);
            }
        }

        public void setIsSelected(bool s)
        {
            if (VertexObject != null)
            {
                GameObject sphere = VertexObject.GetComponent<VertexObj>().sphere;
                isSelected = s;
                if (isSelected)
                {
                    sphere.GetComponent<MeshRenderer>().material = VertexObject.GetComponent<VertexObj>().select;
                }
                else
                {
                    sphere.GetComponent<MeshRenderer>().material = VertexObject.GetComponent<VertexObj>().created;
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

