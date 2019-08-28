using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BachelorArbeitUnity
{
    public class Controller : MonoBehaviour
    {
        public GameObject MeshObject;
        public GameObject VertexObj;
        public GameObject LineObj;
        public GameObject MeshLineObj;
        public GameObject facePreviewObj;

        public LayerMask maskOrg;
        public LayerMask maskPH;
        public LayerMask maskNM;
        public LayerMask maskOrgOnly;

        public Material myMeshMaterial;
        public Material patchesMaterial;
        public Material newMeshMaterial;

        public GameObject symPlaneObject;

        public float size;

        Camera cam;
        MeshStruct myMesh;
        MeshStruct patchHolder;
        MeshStruct refinedMesh;
        SymmetryPlane symPlane;

        private string objName;
        private List<GameObject> lines;
        private facePreview facePre;

        private void Awake()
        {
            InformationHolder.con = this;
            this.objName = InformationHolder.pathToMesh;

            initializeOldMesh();
            initializePatchHolder();
            initializeNewMesh();
        }

        // Start is called before the first frame update
        void Start()
        {
            lines = new List<GameObject>();

            GameObject FacePreObj = Instantiate(facePreviewObj, new Vector3(0, 0, 0), Quaternion.identity);
            facePre = FacePreObj.GetComponent<facePreview>();
            facePre.scale(myMesh.getSize() * 3);
        }

        public void initializeOldMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);
            MeshOB.layer = 8;

            ObjMesh o = new ObjMesh("./Assets/Meshes/" + objName + ".obj");
            o.scaleMesh(size);

            MeshOB.tag = "Original";
            MeshOB.name = "Original";
            MeshOB.layer = 10;

            MeshOB.GetComponent<MeshRenderer>().material = myMeshMaterial;

            myMesh = MeshOB.GetComponent<MeshStruct>();
            myMesh.addGameObjects(VertexObj);
            myMesh.loadMeshFromObj(o);
            myMesh.updateMesh();
        }

        private void initializePatchHolder()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.tag = "PatchHolder";
            MeshOB.name = "PatchHolder";
            MeshOB.layer = 11;

            MeshOB.GetComponent<MeshRenderer>().material = patchesMaterial;

            patchHolder = MeshOB.GetComponent<MeshStruct>();
            patchHolder.addGameObjects(VertexObj);
            patchHolder.loadEmptyFromMesh(myMesh);
        }

        private void initializeNewMesh()
        {
            GameObject MeshOB = Instantiate(MeshObject, new Vector3(0, 0, 0), Quaternion.identity);

            MeshOB.name = "RefinedMesh";
            MeshOB.name = "RefinedMesh";
            MeshOB.layer = 12;

            MeshOB.GetComponent<MeshRenderer>().material = newMeshMaterial;

            refinedMesh = MeshOB.GetComponent<MeshStruct>();
            refinedMesh.addGameObjects(VertexObj);
            refinedMesh.loadEmptyFromMesh(myMesh);
        }

        public void showOriginal(bool v)
        {
            myMesh.GetComponent<MeshRenderer>().enabled = v;
        }

        public void showPatches(bool v)
        {
            patchHolder.GetComponent<MeshRenderer>().enabled = v;
        }

        public void showNewMesh(bool v)
        {
            refinedMesh.GetComponent<MeshRenderer>().enabled = v;
        }

        public void saveMesh(string name)
        {
            ObjMesh saver = new ObjMesh(refinedMesh);
            saver.writeToFile(name, true);
        }

        public void swapNormal()
        {
            facePre.flipped = !facePre.flipped;
            facePre.drawMesh(patchHolder.getSelectedVertices());
        }

        public void deleteSelectedFace()
        {
            patchHolder.deleteSelectedFace();
            patchHolder.updateMesh();
            refinedMesh.updateMesh();
            removeLines();
        }

        private void moveVertex(Vertex vertex, Vector3 point)
        {
            vertex.setPosition(point);
            foreach (Face f in vertex.getFaces())
            {
                reDrawFace(f);
            }
        }

        public void increaseSepNumber()
        {
            if (patchHolder.getSelectedEdge() != null)
            {
                Edge e = patchHolder.getSelectedEdge();
                changeSepNumber(e, e.getSepNumber() + 1);
                if (e.getSymEdge() != null)
                {
                    changeSepNumber(e.getSymEdge(), e.getSymEdge().getSepNumber() + 1);
                }
            }
            else if (patchHolder.getSelectedFace() != null)
            {
                changeSepNumber(patchHolder.getSelectedFace(), +1);
                if (patchHolder.getSelectedFace().getSymFace() != null)
                {
                    changeSepNumber(patchHolder.getSelectedFace().getSymFace(), +1);
                }
            }
        }

        public void decreaseSepNumber()
        {
            if (patchHolder.getSelectedEdge() != null)
            {
                Edge e = patchHolder.getSelectedEdge();
                changeSepNumber(e, e.getSepNumber() - 1);
                if (e.getSymEdge() != null)
                {
                    changeSepNumber(e.getSymEdge(), e.getSymEdge().getSepNumber() - 1);
                }
            }
            else if (patchHolder.getSelectedFace() != null)
            {
                changeSepNumber(patchHolder.getSelectedFace(), -1);
                if (patchHolder.getSelectedFace().getSymFace() != null)
                {
                    changeSepNumber(patchHolder.getSelectedFace().getSymFace(), -1);
                }
            }
        }

        public void changeSepNumber(Edge e, int newSepN)
        {
            e.setSepNumber(newSepN);
            e.setVerticesOnEdge(addVerticesBetween(e.getNewV1(), e.getDirection(), e, refinedMesh));
            if (e.getF1() != null)
            {
                reDrawFace(e.getF1());
            }
            if (e.getF2() != null)
            {
                reDrawFace(e.getF2());
            }
        }

        public void changeSepNumber(Face f, int deltaSep)
        {
            foreach (Edge e in f.getEdges())
            {
                changeSepNumber(e, e.getSepNumber() + deltaSep);
            }
            removeLines();
            foreach (Edge e in patchHolder.getSelectedFace().getInnerEdges())
            {
                GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                lineRend.widthMultiplier = myMesh.getSize();
                lineRend.positionCount = 2;
                lineRend.SetPosition(0, e.getV1().getPosition());
                lineRend.SetPosition(1, e.getV2().getPosition());
                lines.Add(Liner);
            }
        }

        public void increaseOuterFlowPreset()
        {
            if (patchHolder.getSelectedEdge() != null)
            {
                patchHolder.getSelectedEdge().getHalfEdge(patchHolder.getSelectedFace()).increaseOuterFlowPreset();
                reDrawFace(patchHolder.getSelectedFace());
            }
        }

        public void decreaseOuterFlowPreset()
        {
            if (patchHolder.getSelectedEdge() != null)
            {
                patchHolder.getSelectedEdge().getHalfEdge(patchHolder.getSelectedFace()).decreaseOuterFlowPreset();
                reDrawFace(patchHolder.getSelectedFace());
            }
        }

        public void removeLines()
        {
            foreach (GameObject o in lines)
            {
                Destroy(o);
            }
            lines.Clear();
        }

        public void createSymmetryPlane()
        {
            List<Vertex> selectedVertices = patchHolder.getSelectedVertices();
            if (selectedVertices.Count < 3)
            {
                return;
            }
            if (symPlane != null)
            {
                Destroy(symPlane.gameObject);
            }
            GameObject go = Instantiate(symPlaneObject, new Vector3(0, 0, 0), Quaternion.identity);
            symPlane = go.GetComponent<SymmetryPlane>();
            symPlane.setPlane(selectedVertices[0].getPosition(), selectedVertices[1].getPosition(), selectedVertices[2].getPosition());

            clearSelection();
        }

        public void concatinateVertices() //TODO
        {
            List<Vertex> selectedVertices = patchHolder.getSelectedVertices();
            if (selectedVertices.Count == 2)
            {
                patchHolder.concatinateVertices(selectedVertices[0], selectedVertices[1]);
            }
            foreach (Face f in selectedVertices[0].getFaces())
            {
                reDrawFace(f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                createFace();
                return;
            }
            if (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(-1))
            {
                return;
            }

            if (InformationHolder.moveVertex)
            {
                if (Input.GetMouseButton(0))
                {
                    if (patchHolder.getSelectedVertices().Count == 1)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (!Physics.Raycast(ray, out hit, float.MaxValue, maskOrgOnly))
                        {
                            return;
                        }
                        moveVertex(patchHolder.getSelectedVertices()[0], hit.point);
                    }
                    return;
                }
            }

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

                if (!Physics.Raycast(ray, out hit, float.MaxValue, mask))
                {
                    return;
                }
                GameObject objhit = hit.transform.gameObject;
                Vector3 impactPoint = hit.point;

                switch (objhit.tag)
                {
                    case "Original":
                        Vertex v = patchHolder.addVertex(impactPoint);
                        if (v != null && v.isValid())
                        {
                            patchHolder.selectVertex(v);
                            facePre.drawMesh(patchHolder.getSelectedVertices());
                        }
                        break;
                    case "Vertex":
                        Vertex ver = hit.transform.parent.gameObject.GetComponent<VertexObj>().vertex;
                        if (ver != null && ver.isValid())
                        {
                            patchHolder.selectVertex(ver);
                            facePre.drawMesh(patchHolder.getSelectedVertices());
                        }
                        break;
                    case "PatchHolder":
                        if (InformationHolder.selectEdge)
                        {
                            Face f = patchHolder.selectFaceAt(patchHolder.getSplitToNotSplitFaces()[hit.triangleIndex]);
                            Edge edge = null;
                            if (f != null && f.isValid())
                            {
                                float min = float.MaxValue;
                                foreach (Edge e in f.getEdges())
                                {
                                    Vector3 linePoint = e.getV1().getPosition();
                                    Vector3 lineVec = e.getDirection().normalized;

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
                                if (edge != null && edge.isValid())
                                {
                                    GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                                    LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                                    lineRend.positionCount = edge.getSepNumber() + 1;
                                    lineRend.widthMultiplier = myMesh.getSize();
                                    int count = 0;
                                    lineRend.SetPosition(count, edge.getV1().getPosition());
                                    count++;
                                    foreach (Vertex vertex in edge.getVerticesOnEdge())
                                    {
                                        lineRend.SetPosition(count, vertex.getPosition());
                                        count++;
                                    }
                                    lineRend.SetPosition(count, edge.getV2().getPosition());
                                    lines.Add(Liner);
                                }
                            }

                        }
                        else if (InformationHolder.selectFace)
                        {
                            Face f = patchHolder.selectFaceAt(patchHolder.getSplitToNotSplitFaces()[hit.triangleIndex]);
                            removeLines();
                            if (f != null && f.isValid())
                            {
                                foreach (Edge e in f.getInnerEdges())
                                {
                                    GameObject Liner = Instantiate(LineObj, new Vector3(0, 0, 0), Quaternion.identity);
                                    LineRenderer lineRend = Liner.GetComponent<LineRenderer>();
                                    lineRend.widthMultiplier = myMesh.getSize();
                                    lineRend.positionCount = 2;
                                    lineRend.SetPosition(0, e.getV1().getPosition());
                                    lineRend.SetPosition(1, e.getV2().getPosition());
                                    lines.Add(Liner);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public void clearSelection()
        {
            removeLines();
            patchHolder.clearSelection();
            facePre.drawMesh(patchHolder.getSelectedVertices());
        }

        //Starts creating the Faces in the patch specified by the selected vertices
        public void createFace()
        {
            List<Vertex> selectedVertices = patchHolder.getSelectedVertices();
            if (selectedVertices.Count > 2)
            {
                Face f = newFace(selectedVertices, facePre.flipped);

                clearSelection();
            }
            else
            {
                print("Not enough Verices selected");
            }
        }

        public void createSymmetryFace()
        {
            if (patchHolder.getSelectedFace() != null && symPlane != null && patchHolder.getSelectedFace().getSymFace() == null)
            {
                patchHolder.clearSelectedVertices();
                Face orgFace = patchHolder.getSelectedFace();
                foreach (Vertex v in orgFace.getVertices())
                {
                    Vertex vMir;
                    if (v.getSymVertex() != null && v.getSymVertex().isValid())
                    {
                        vMir = v.getSymVertex();
                    }
                    else
                    {
                        vMir = patchHolder.addVertex(symPlane.mirroredPos(v.getPosition()));
                        v.setSymVertex(vMir);
                    }
                    patchHolder.selectVertex(vMir);
                }
                Face f = newFace(patchHolder.getSelectedVertices(), true);
                orgFace.setSymFace(f);

                clearSelection();
            }
        }

        public Face newFace(List<Vertex> selectedVertices, bool flipped)
        {
            List<int> verticesIndices = new List<int>();

            Vector3 prevDir = selectedVertices[selectedVertices.Count - 1].getPosition();
            foreach (Vertex v in selectedVertices)
            {
                verticesIndices.Add(v.getHandleNumber());
            }
            if (flipped)
            {
                verticesIndices.Reverse();
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
            if (edges[edges.Count - 1].getSepNumber() <= 0)
            {
                edges[edges.Count - 1].setSepNumber(2 + count % 2);
            }

            foreach (Vertex v in f.getVertices())
            {
                if (!v.getIsCreated())
                {
                    Vertex newV = refinedMesh.addVertex(v.getPosition());
                    v.setRefinedVertex(newV);
                    v.setIsCreated(true);
                }
            }
            foreach (Edge e in f.getEdges())
            {
                if (e.getNewV1().isConnected(e.getNewV2()) == null)
                {
                    e.setVerticesOnEdge(addVerticesBetween(e.getNewV1(), e.getDirection(), e, refinedMesh));
                }
            }
            executePatch(f, refinedMesh, patchHolder);
            updateMeshes();
            return f;
        }

        public void reDrawFace(Face f)
        {
            f.resetValues();
            foreach (HalfEdge h in f.getHalfEdges())
            {
                h.resetValues();
            }
            executePatch(f, refinedMesh, patchHolder);
            updateMeshes();
        }

        public void updateMeshes()
        {
            patchHolder.updateMesh();

            refinedMesh.updateMesh();
        }

        public void refreshRefinedMesh()
        {
            patchHolder.updateMesh();

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
            foreach (Face f in patchHolder.getFaces())
            {

                Vector3 normal = f.getNormal();

                foreach (Vertex v in f.getInnerVertices())
                {
                    v.setNewPosition(fitToMesh(v.getPosition(), normal));

                }
                foreach (Edge e in f.getEdges())
                {
                    foreach (Vertex v in e.getVerticesOnEdge())
                    {
                        v.setNewPosition(fitToMesh(v.getPosition(), normal));
                    }
                }
            }
            foreach (Vertex v in refinedMesh.getVertices())
            {
                if (v.isValid())
                {
                    v.updatePosition();
                }
            }

            refinedMesh.updateMesh();
        }

        public Vector3 fitToMesh(Vector3 pos, Vector3 normal)
        {
            Ray ray = new Ray(pos + normal * 20 * InformationHolder.threshHold, -normal);
            RaycastHit[] hits = Physics.RaycastAll(ray, 200f, maskOrgOnly);

            float minDis = float.MaxValue;
            RaycastHit hit = new RaycastHit();
            if (hits.Length > 0)
            {
                foreach (RaycastHit h in hits)
                {
                    if ((h.point - pos).magnitude < minDis)
                    {
                        minDis = (h.point - pos).magnitude;
                        hit = h;
                    }
                }
                return hit.point;
            }
            if (symPlane != null)
            {
                pos = symPlane.mirroredPos(pos);
                normal = symPlane.mirroredPos(normal);
                ray = new Ray(pos + normal * 20 * InformationHolder.threshHold, -normal);
                hits = Physics.RaycastAll(ray, 200f, maskOrgOnly);
                minDis = float.MaxValue;
                if (hits.Length > 0)
                {
                    foreach (RaycastHit h in hits)
                    {
                        if ((h.point - pos).magnitude < minDis)
                        {
                            minDis = (h.point - pos).magnitude;
                            hit = h;
                        }
                    }
                    return symPlane.mirroredPos(hit.point);
                }
                return symPlane.mirroredPos(pos);
            }
            return pos;
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
                                e.getV1().setRefinedVertex(newV);
                            }
                            else if (e.getV2().Equals(v))
                            {
                                e.getV2().setRefinedVertex(newV);
                            }
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

            for (int i = 0; i < size; i++)
            {
                int prevHe = i % size;
                int curHe = (i + 1) % size;
                int nextHe = (i + 2) % size;
                int outerFlowPreset = halfEdges[curHe].getOuterFlowPreset();

                halfEdges[curHe].setOuterFlow(outerFlowPreset);
                halfEdges[prevHe].reduceSep(outerFlowPreset);
                halfEdges[nextHe].reduceSep(outerFlowPreset);
            }

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