using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Controller : MonoBehaviour
    {
        public string objName;
        public GameObject MeshObject;
        public GameObject VertexObj;
        public GameObject EdgeObj;
        public GameObject FaceObj;

        public Material myMeshMaterial;
        public Material patchesMaterial;
        public Material newMeshMaterial;

        Camera cam;
        BachelorArbeitUnity.Mesh myMesh;
        BachelorArbeitUnity.Mesh patchHolder;
        BachelorArbeitUnity.Mesh newMesh;

        // Start is called before the first frame update
        void Start()
        {
            InformationHolder.con = this;
            this.objName = InformationHolder.pathToMesh;

            initializeOldMesh();
            initializePatchHolder();
            initializeNewMesh();
        }

        public void initializeOldMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            ObjMesh o = new ObjMesh("./Assets/Meshes/" + objName + ".obj");

            MeshOB.GetComponent<MeshFilter>().mesh = new ObjLoader().newLoad(o);

            MeshOB.AddComponent<MeshCollider>();

            MeshOB.GetComponent<MeshRenderer>().material = myMeshMaterial;

            myMesh = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            myMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            myMesh.loadMeshFromObj(o);
        }

        private void initializePatchHolder()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);
            
            MeshOB.GetComponent<MeshRenderer>().material = patchesMaterial;

            patchHolder = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            patchHolder.addGameObjects(VertexObj, EdgeObj, FaceObj);
            patchHolder.loadMeshFromMesh(myMesh);

            MeshOB.AddComponent<MeshCollider>();
        }

        private void initializeNewMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.GetComponent<MeshRenderer>().material = newMeshMaterial;

            newMesh = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            newMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            newMesh.loadEmptyFromMesh(myMesh);

            MeshOB.AddComponent<MeshCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InformationHolder.selectVertices)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!Physics.Raycast(ray, out hit, 100.0f))
                    {
                        return;
                    }

                    MeshCollider meshCollider = hit.collider as MeshCollider;
                    BoxCollider boxCollider = hit.collider as BoxCollider;
                    if (meshCollider != null && meshCollider.sharedMesh != null)
                    {
                        GameObject objhit = meshCollider.gameObject;
                        if (objhit.CompareTag("MeshObject"))
                        {
                            print("mesh hit");
                            UnityEngine.Mesh mesh = meshCollider.sharedMesh;
                            Vector3[] vertices = mesh.vertices;
                            int[] triangles = mesh.triangles;
                            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
                            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
                            Transform hitTransform = hit.collider.transform;

                            Vector3 impactPoint = hit.point;

                            p0 = hitTransform.TransformPoint(p0);
                            p1 = hitTransform.TransformPoint(p1);
                            p2 = hitTransform.TransformPoint(p2);

                            float d0 = (impactPoint - p0).magnitude;
                            float d1 = (impactPoint - p1).magnitude;
                            float d2 = (impactPoint - p2).magnitude;

                            int vertexIndex = 0;
                            if (d0 < d1 && d0 < d2)
                            {
                                vertexIndex = InformationHolder.splitToNotSplitVertices[triangles[hit.triangleIndex * 3 + 0]];
                            }
                            else if (d1 < d0 && d1 < d2)
                            {
                                vertexIndex = InformationHolder.splitToNotSplitVertices[triangles[hit.triangleIndex * 3 + 1]];
                            }
                            else if (d2 < d0 && d2 < d1)
                            {
                                vertexIndex = InformationHolder.splitToNotSplitVertices[triangles[hit.triangleIndex * 3 + 2]];
                            }
                            myMesh.selectVertexAt(vertexIndex);

                            Debug.DrawLine(p0, p1);
                            Debug.DrawLine(p1, p2);
                            Debug.DrawLine(p2, p0);
                        }
                    }
                    else if (boxCollider != null)
                    {
                        if (boxCollider.transform.parent != null)
                        {
                            GameObject objhit = boxCollider.transform.parent.gameObject;
                            if (objhit.CompareTag("Vertex"))
                            {
                                print("vertex hit");
                                myMesh.selectVertexAt(objhit.GetComponent<VertexObj>().vertexIndex);
                            }
                        }
                    }
                }
            }
        }

        public void showOriginal(bool v)
        {
            myMesh.gameObject.SetActive(v);
        }

        public void showPatches(bool v)
        {
            patchHolder.gameObject.SetActive(v);
        }

        public void showNewMesh(bool v)
        {
            newMesh.gameObject.SetActive(v);
        }

        public void saveMesh(string name)
        {
            ObjMesh saver = new ObjMesh(newMesh);
            saver.writeToFile(name,true);
        }

        //Starts creating the Faces in the patch specified by the selected vertices
        public void createFace()
        {
            List<Vertex> selectedVertices = myMesh.getSelectedVertices();
            List<int> verticesIndices = new List<int>();
            if (selectedVertices.Count > 2)
            {
                for (int i = 0; i < selectedVertices.Count; i++)
                {
                    newMesh.addVertexAtIndex(selectedVertices[i].getHandleNumber(), selectedVertices[i].getPosition());
                    verticesIndices.Add(selectedVertices[i].getHandleNumber());
                }
                Face f = patchHolder.addFace(verticesIndices);

                foreach (Edge e in f.getEdges())
                {
                    e.setSepNumber(2);
                    e.setVerticesOnEdge(addVerticesBetween(e.getV1(), e.getDirection(), e, newMesh));
                }

                executePatch(f, newMesh, patchHolder);
                patchHolder.updateMesh();
                newMesh.updateMesh();
                myMesh.selectedVerticesCreated();
                myMesh.clearSelectedVertices();
            }
            else
            {
                print("Not enough Verices selected");
            }
        }

        //Adds the Vertices on the Edge
        public Vertex[] addVerticesBetween(Vertex v1, Vector3 direction, Edge edge, Mesh newMesh)
        {
            if (edge.getSepNumber() > 0)
            {
                int sepNumber = edge.getSepNumber();

                Vertex[] newVertices = new Vertex[edge.getSepNumber() - 1];

                for (int i = 0; i < edge.getSepNumber() - 1; i++)
                {
                    Vector3 pos = v1.getPosition() + (direction / sepNumber * (i + 1));
                    newVertices[i] = newMesh.addVertex(pos);
                }
                return newVertices;
            }
            else
            {
                print("choose sepNumber first");
                return null;
            }
        }

        //Adds new Faces to the New Mesh replacing the patchFace in the patchHolder
        public void executePatch(Face face, Mesh newMesh, Mesh oldMesh)
        {
            int sumOfSepNumbers = 0;
            foreach (Edge e in face.getEdges())
            {
                e.getHalfEdge(face).setReduced(e.getSepNumber());

                sumOfSepNumbers += e.getSepNumber();
            }

            if (sumOfSepNumbers % 2 != 0)
            {
                throw new Exception("the Sum of Seperation in a face Must be even " + sumOfSepNumbers + " is not even");
            }

            reducePattern(face);

            List<int> reducedValues = new List<int>();
            foreach (HalfEdge he in face.getHalfEdges())
            {
                reducedValues.Add(he.getReduced());
                Console.WriteLine(he.getReduced());
            }
            Patterndecider patDec = createPatterndecider(reducedValues);
            Pattern p = new Pattern(patDec.choosePattern());
            p.fillPatch(oldMesh, newMesh, face);
        }

        //Reduces the seperator count on each side by adding trivial quads in the  current patch
        //Does not actually add the quads, just calculates the amount of seperations that remain
        //Also adds the flow to each HalfEdge
        //Provides better Pattern than the old Function
        public void reducePattern(Face face)
        {
            List<HalfEdge> halfEdges = face.getHalfEdges();

            int size = halfEdges.Count;
            int noFlowCount = 0;
            int counter = size;

            while (noFlowCount < size)
            {
                int prevHe = counter % size;
                int curHe = (counter + 1) % size;
                int nextHe = (counter + 2) % size;

                int rem = Math.Min(halfEdges[prevHe].getReduced(), halfEdges[nextHe].getReduced()) - 1;
                if (rem > 0)
                {
                    halfEdges[curHe].addOuterFlow();
                    halfEdges[prevHe].reduceSep();
                    halfEdges[nextHe].reduceSep();
                    noFlowCount = 0;
                }
                else
                {
                    noFlowCount++;
                }
                counter++;
            }

            foreach (HalfEdge he in halfEdges)
            {
                he.createVerticesArray();
            }
        }

        //creates new class "Patterndecider" which has all the information
        //to choose the right pattern for the current patch
        //t contains the information on alhpa and beta
        public Patterndecider createPatterndecider(List<int> reducedPattern)
        {
            int c = 0;
            List<int> t = new List<int>();
            foreach (int i in reducedPattern)
            {
                if (i > 1)
                {
                    t.Add(i);
                    t.Add(c);
                }
                c++;
            }
            if (t.Count > 4)
            {
                throw new Exception("Error while reducing the pattern");
            }
            Patterndecider pat = new Patterndecider(reducedPattern.Count, t);
            return pat;

        }
    }
}