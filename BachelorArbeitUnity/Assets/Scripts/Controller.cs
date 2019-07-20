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

        public LayerMask maskOrg;
        public LayerMask maskPH;
        public LayerMask maskNM;

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
            GameObject MeshOB = Instantiate()
            InformationHolder.con = this;
            this.objName = InformationHolder.pathToMesh;

            initializeOldMesh();
            initializePatchHolder();
            initializeNewMesh();
        }

        public void initializeOldMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);
            MeshOB.layer = 8;

            ObjMesh o = new ObjMesh("./Assets/Meshes/" + objName + ".obj");

            MeshOB.name = "Original";
            MeshOB.layer = 10;

            MeshOB.GetComponent<MeshRenderer>().material = myMeshMaterial;

            myMesh = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            myMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            myMesh.loadMeshFromObj(o);
            myMesh.updateMesh();

            InformationHolder.myMeshToPatchHolder = new int[myMesh.getVertices().Count];
            for (int i = 0; i < InformationHolder.myMeshToPatchHolder.Length; i++) {
                InformationHolder.myMeshToPatchHolder[i] = -1;
            }
        }

        private void initializePatchHolder()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.name = "PatchHolder";
            MeshOB.layer = 11;

            MeshOB.GetComponent<MeshRenderer>().material = patchesMaterial;

            patchHolder = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            patchHolder.addGameObjects(VertexObj, EdgeObj, FaceObj);
            patchHolder.loadEmptyFromMesh(myMesh);
        }

        private void initializeNewMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.name = "NewMesh";
            MeshOB.layer = 12;

            MeshOB.GetComponent<MeshRenderer>().material = newMeshMaterial;

            newMesh = MeshOB.GetComponent<BachelorArbeitUnity.Mesh>();
            newMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            newMesh.loadEmptyFromMesh(myMesh);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask mask = new LayerMask();
                if (InformationHolder.selectVertices)
                {
                    mask = maskOrg;
                }
                else if (InformationHolder.selectEdge)
                {
                    mask = maskPH;
                }
                else if (InformationHolder.selectFace)
                {
                    mask = maskPH;
                }
                if (!Physics.Raycast(ray, out hit, 100.0f, mask))
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
                        UnityEngine.Mesh mesh = meshCollider.sharedMesh;
                        Transform hitTransform = hit.collider.transform;
                        Vector3 impactPoint = hit.point;

                        Vector3[] vertices = mesh.vertices;
                        int[] triangles = mesh.triangles;
                        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
                        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
                        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];

                        p0 = hitTransform.TransformPoint(p0);
                        p1 = hitTransform.TransformPoint(p1);
                        p2 = hitTransform.TransformPoint(p2);

                        float d0 = (impactPoint - p0).magnitude;
                        float d1 = (impactPoint - p1).magnitude;
                        float d2 = (impactPoint - p2).magnitude;

                        int v0 = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 1]];
                        int v1 = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 1]];
                        int v2 = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 2]];

                        if (InformationHolder.selectVertices)
                        {
                            int vertexIndex = -1;
                            if (d0 < d1 && d0 < d2)
                            {
                                vertexIndex = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 0]];
                            }
                            else if (d1 < d0 && d1 < d2)
                            {
                                vertexIndex = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 1]];
                            }
                            else if (d2 < d0 && d2 < d1)
                            {
                                vertexIndex = myMesh.getSplitToNotSplitVertices()[triangles[hit.triangleIndex * 3 + 2]];
                            }

                            myMesh.selectVertexAt(vertexIndex);
                        }
                        else if (InformationHolder.selectEdge)
                        {
                            Edge e;
                            if (d1 < d0 && d2 < d0)
                            {
                                e = myMesh.getVertexAt(v1).isConnected(myMesh.getVertexAt(v2));
                            }
                            else if (d0 < d1 && d2 < d1)
                            {
                                e = myMesh.getVertexAt(v0).isConnected(myMesh.getVertexAt(v2));
                            }
                            else if (d0 < d2 && d1 < d2)
                            {
                                e = myMesh.getVertexAt(v0).isConnected(myMesh.getVertexAt(v1));
                            }
                            else
                            {
                                e = null;
                            }

                            if (e != null)
                            {
                                patchHolder.selectEdgeAt(e.getHandleNumber());
                            }

                        }
                        else if (InformationHolder.selectFace)
                        {
                            patchHolder.selectFaceAt(patchHolder.getSplitToNotSplitFaces()[hit.triangleIndex]);
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
            saver.writeToFile(name, true);
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
                    int vHnMyMesh = selectedVertices[i].getHandleNumber();
                    int vHnPatchHolder = InformationHolder.myMeshToPatchHolder[vHnMyMesh];
                    Vertex v;

                    //Test if Vertex already Exists
                    if (patchHolder.vertexExists(vHnPatchHolder)) {
                        v = patchHolder.getVertexAt(vHnPatchHolder);
                    }
                    else{ 
                        v = patchHolder.addVertex(selectedVertices[i].getPosition());
                        InformationHolder.myMeshToPatchHolder[vHnMyMesh] = v.getHandleNumber();
                    }
                    verticesIndices.Add(v.getHandleNumber());
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
                myMesh.clearSelectedVertices();
            }
            else
            {
                print("Not enough Verices selected");
            }
        }

        public void deleteSelectedFace() {
            patchHolder.deleteSelectedFace();
            patchHolder.getFaces().Remove(patchHolder.getSelectedFace());
            patchHolder.updateMesh();
            newMesh.updateMesh();
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