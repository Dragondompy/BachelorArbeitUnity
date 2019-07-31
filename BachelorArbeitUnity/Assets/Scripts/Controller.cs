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
        public GameObject LineObj;
        public List<GameObject> lines;

        public LayerMask maskOrg;
        public LayerMask maskPH;
        public LayerMask maskNM;

        public Material myMeshMaterial;
        public Material patchesMaterial;
        public Material newMeshMaterial;

        Camera cam;
        MeshStruct myMesh;
        MeshStruct patchHolder;
        MeshStruct refinedMesh;

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
            MeshOB.layer = 8;

            ObjMesh o = new ObjMesh("./Assets/Meshes/" + objName + ".obj");

            MeshOB.name = "Original";
            MeshOB.layer = 10;

            MeshOB.GetComponent<MeshRenderer>().material = myMeshMaterial;

            myMesh = MeshOB.GetComponent<MeshStruct>();
            myMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            myMesh.loadMeshFromObj(o);
            myMesh.updateMesh();

            InformationHolder.myMeshToPatchHolder = new int[myMesh.getVertices().Count];
            for (int i = 0; i < InformationHolder.myMeshToPatchHolder.Length; i++)
            {
                InformationHolder.myMeshToPatchHolder[i] = -1;
            }
        }

        private void initializePatchHolder()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.name = "PatchHolder";
            MeshOB.layer = 11;

            MeshOB.GetComponent<MeshRenderer>().material = patchesMaterial;

            patchHolder = MeshOB.GetComponent<MeshStruct>();
            patchHolder.addGameObjects(VertexObj, EdgeObj, FaceObj);
            patchHolder.loadEmptyFromMesh(myMesh);
        }

        private void initializeNewMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.name = "NewMesh";
            MeshOB.layer = 12;

            MeshOB.GetComponent<MeshRenderer>().material = newMeshMaterial;

            refinedMesh = MeshOB.GetComponent<MeshStruct>();
            refinedMesh.addGameObjects(VertexObj, EdgeObj, FaceObj);
            refinedMesh.loadEmptyFromMesh(myMesh);
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
            refinedMesh.gameObject.SetActive(v);
        }

        public void saveMesh(string name)
        {
            ObjMesh saver = new ObjMesh(refinedMesh);
            saver.writeToFile(name, true);
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
                        Mesh mesh = meshCollider.sharedMesh;
                        Transform hitTransform = hit.collider.transform;
                        Vector3 impactPoint = hit.point;

                        if (InformationHolder.selectVertices)
                        {
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
                            Face f = patchHolder.getFaceAt(patchHolder.getSplitToNotSplitFaces()[hit.triangleIndex]);
                            Edge edge = null;
                            if (f != null)
                            {
                                float min = float.MaxValue;
                                foreach (Edge e in f.getEdges())
                                {
                                    Vector3 linePoint = e.getV1().getPosition();
                                    Vector3 lineVec = e.getV2().getPosition() - e.getV1().getPosition();

                                    Vector3 linePointToPoint = impactPoint - linePoint;
                                    float t = Vector3.Dot(linePointToPoint, lineVec);

                                    float distance = ((linePoint + lineVec * t) - impactPoint).magnitude;
                                    if (distance <= min)
                                    {
                                        min = distance;
                                        edge = e;
                                    }
                                }
                                edge = patchHolder.selectEdgeAt(edge.getHandleNumber());
                                removeLines();
                                if (edge != null)
                                {
                                    GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                                    LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                                    lineRend.positionCount = 2;
                                    lineRend.SetPosition(0, edge.getV1().getPosition());
                                    lineRend.SetPosition(1, edge.getV2().getPosition());
                                    lines.Add(Liner);
                                }
                            }
                        }
                        else if (InformationHolder.selectFace)
                        {
                            Face f = patchHolder.selectFaceAt(patchHolder.getSplitToNotSplitFaces()[hit.triangleIndex]);
                            removeLines();
                            if (f != null)
                            {
                                foreach (Edge e in f.getInnerEdges())
                                {
                                    GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                                    LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                                    lineRend.positionCount = 2;
                                    lineRend.SetPosition(0, e.getV1().getPosition());
                                    lineRend.SetPosition(1, e.getV2().getPosition());
                                    lines.Add(Liner);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void clearSelection()
        {
            removeLines();
            patchHolder.clearSelection();
        }

        //Starts creating the Faces in the patch specified by the selected vertices
        public void createFace()
        {
            removeLines();
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
                    if (patchHolder.vertexExists(vHnPatchHolder))
                    {
                        v = patchHolder.getVertexAt(vHnPatchHolder);
                    }
                    else
                    {
                        v = patchHolder.addVertex(selectedVertices[i].getPosition());
                        InformationHolder.myMeshToPatchHolder[vHnMyMesh] = v.getHandleNumber();
                    }
                    verticesIndices.Add(v.getHandleNumber());
                }
                Face f = patchHolder.addFace(verticesIndices);

                List<Edge> edges = f.getEdges();
                int count = 0;
                for (int i = 0; i < edges.Count - 1; i++)
                {
                    if (edges[i].getSepNumber() <= 0)
                    {
                        edges[i].setSepNumber(2);
                    }
                    count += edges[i].getSepNumber();
                }
                edges[edges.Count - 1].setSepNumber(2 + count % 2);
                myMesh.clearSelectedVertices();
                patchHolder.updateMesh();

                refreshRefinedMesh();
            }
            else
            {
                print("Not enough Verices selected");
            }
        }

        public void deleteSelectedFace()
        {
            patchHolder.deleteSelectedFace();
            patchHolder.getFaces().Remove(patchHolder.getSelectedFace());
            patchHolder.updateMesh();
            refinedMesh.updateMesh();
            removeLines();
        }

        public void increaseSepNumber()
        {
            patchHolder.getSelectedEdge().increaseSepNumber();
            refreshRefinedMesh();
        }

        public void decreaseSepNumber()
        {
            patchHolder.getSelectedEdge().decreaseSepNumber();
            refreshRefinedMesh();
        }

        public void removeLines()
        {
            foreach (GameObject o in lines)
            {
                Destroy(o);
            }
            lines.Clear();
        }

        public void refreshRefinedMesh()
        {
            foreach (HalfEdge he in patchHolder.getHalfEdges())
            {
                he.resetValues();
            }
            foreach (Edge e in patchHolder.getEdges())
            {
                e.resetValues();
            }
            foreach (Face f in patchHolder.getFaces())
            {
                f.resetValues();
            }

            refinedMesh.loadEmptyFromMesh(patchHolder);

            addCornerVertices(refinedMesh, patchHolder);

            addVerticesOnEdge(refinedMesh, patchHolder);

            foreach (Face f in patchHolder.getFaces())
            {
                if (f.isValid())
                {
                    executePatch(f, refinedMesh, patchHolder);
                }
            }

            Vector3 pos = myMesh.transform.position;
            Quaternion rot = myMesh.transform.rotation;

            //Streches the refinedMesh to fit the oldMesh
            foreach (Edge e in patchHolder.getEdges())
            {
                foreach (Vertex v in e.getVerticesOnEdge())
                {
                    //Vector3 newPos = Physics.ClosestPoint(v.getPosition(), myMesh.GetComponent<MeshCollider>(), pos, rot);
                    //v.setPosition(newPos);
                }
            }
            foreach (Face f in patchHolder.getFaces())
            {
                foreach (Vertex v in f.getInnerVertices())
                {
                    Vector3 prevDir = v.getEdges()[v.getEdges().Count - 1].getDirection();
                    Vector3 dir = new Vector3(0, 0, 0);
                    foreach (Edge e in v.getEdges())
                    {
                        Vector3 newDir = Vector3.Cross(prevDir, e.getDirection());
                        if (newDir.magnitude > 0.01)
                        {
                            dir += newDir;
                        }

                        prevDir = e.getDirection();
                    }
                    dir = dir.normalized;

                    GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                    LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                    lineRend.positionCount = 2;
                    lineRend.SetPosition(0, v.getPosition());
                    lineRend.SetPosition(1, v.getPosition() + dir);

                    RaycastHit hit;
                    Ray ray = new Ray(v.getPosition()+dir, -dir);
                    Physics.Raycast(ray, out hit, float.MaxValue, maskOrg);
                    //TODO better nearest point on Mesh
                    v.setPosition(hit.point);
                }
            }
            refinedMesh.updateMesh();
        }

        //creates the corner vertices of the face in patchHolderMesh in the refinedMesh
        public void addCornerVertices(MeshStruct newMesh, MeshStruct oldMesh)
        {
            foreach (Vertex v in oldMesh.getVertices())
            {
                if (v.isValid())
                {
                    Vertex newV = newMesh.addVertex(v.getPosition());
                    foreach (Edge e in v.getEdges())
                    {
                        if (e.isValid())
                        {
                            if (e.getV1().Equals(v))
                            {
                                e.setNewV1(newV);
                            }
                            else if (e.getV2().Equals(v))
                            {
                                e.setNewV2(newV);
                            }
                        }
                    }
                }
            }
            foreach (Edge e in oldMesh.getEdges())
            {
                if (e.isValid())
                {
                    HalfEdge h = e.getH1();
                    if (h != null && h.isValid())
                    {
                        if (h.getV1().Equals(e.getV1()))
                        {
                            h.setNewV1(e.getNewV1());
                            h.setNewV2(e.getNewV2());
                        }
                        else if (h.getV2().Equals(e.getV1()))
                        {
                            h.setNewV2(e.getNewV1());
                            h.setNewV1(e.getNewV2());
                        }
                    }

                    h = e.getH2();
                    if (h != null && h.isValid())
                    {
                        if (h.getV1().Equals(e.getV1()))
                        {
                            h.setNewV1(e.getNewV1());
                            h.setNewV2(e.getNewV2());
                        }
                        else if (h.getV2().Equals(e.getV1()))
                        {
                            h.setNewV2(e.getNewV1());
                            h.setNewV1(e.getNewV2());
                        }
                    }
                }
            }
        }

        //Adds all vertices on All Edges
        public void addVerticesOnEdge(MeshStruct newMesh, MeshStruct oldMesh)
        {
            foreach (Edge e in oldMesh.getEdges())
            {
                e.setVerticesOnEdge(addVerticesBetween(e.getV1(), e.getDirection(), e, newMesh));
            }
        }

        //Adds the Vertices on the Edge
        public Vertex[] addVerticesBetween(Vertex v1, Vector3 direction, Edge edge, MeshStruct newMesh)
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
        public void executePatch(Face face, MeshStruct newMesh, MeshStruct oldMesh)
        {
            int sumOfSepNumbers = 0;
            foreach (Edge e in face.getEdges())
            {
                e.getHalfEdge(face).setReduced(e.getSepNumber());

                sumOfSepNumbers += e.getSepNumber();
            }

            if (sumOfSepNumbers % 2 != 0)
            {
                print("the Sum of Seperation in a face Must be even " + sumOfSepNumbers + " is not even");
                return;
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