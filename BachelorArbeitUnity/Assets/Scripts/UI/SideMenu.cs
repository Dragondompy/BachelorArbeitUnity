using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BachelorArbeitUnity
{
    public class SideMenu : MonoBehaviour
    {
        public Button btnSelectVert;
        public Button btnCreateFace;
        public Button btnSelectEdge;
        public Button btnSelectFace;
        public Button btnDeleteFace;
        public Button btnShowOriginal;
        public Button btnShowPatches;
        public Button btnShowNewMesh;
        public Slider sliCamSpeed;
        public Slider sliCamRotSpeed;
        public Button btnSaveNewMesh;
        public InputField itfSaveMeshName;

        // Start is called before the first frame update
        void Start()
        {
            InformationHolder.selectVertices = true;
            setBtnActive(btnSelectVert, true);

            InformationHolder.showOriginal = true;
            setBtnActive(btnShowOriginal, true);

            InformationHolder.showPatches = true;
            setBtnActive(btnShowPatches, true);

            InformationHolder.showNewMesh = true;
            setBtnActive(btnShowNewMesh, true);

            setCamSpeed();
            setCamRotSpeed();
        }

        public void setSelectVertex()
        {
            InformationHolder.con.clearSelection();
            if (InformationHolder.selectVertices)
            {
                InformationHolder.selectVertices = false;
                setBtnActive(btnSelectVert, false);
            }
            else
            {
                InformationHolder.selectVertices = true;
                setBtnActive(btnSelectVert, true);

                InformationHolder.selectEdge = false;
                setBtnActive(btnSelectEdge, false);

                InformationHolder.selectFace = false;
                setBtnActive(btnSelectFace, false);
            }
        }

        public void setSelectEdge()
        {
            InformationHolder.con.clearSelection();
            if (InformationHolder.selectEdge)
            {
                InformationHolder.selectEdge = false;
                setBtnActive(btnSelectEdge, false);
            }
            else
            {
                InformationHolder.selectEdge = true;
                setBtnActive(btnSelectEdge, true);

                InformationHolder.selectVertices = false;
                setBtnActive(btnSelectVert, false);

                InformationHolder.selectFace = false;
                setBtnActive(btnSelectFace, false);
            }
        }

        public void setSelectFace()
        {
            InformationHolder.con.clearSelection();
            if (InformationHolder.selectFace)
            {
                InformationHolder.selectFace = false;
                setBtnActive(btnSelectFace, false);
            }
            else
            {
                InformationHolder.selectFace = true;
                setBtnActive(btnSelectFace, true);

                InformationHolder.selectVertices = false;
                setBtnActive(btnSelectVert, false);

                InformationHolder.selectEdge = false;
                setBtnActive(btnSelectEdge, false);
            }
        }

        public void showOriginal()
        {
            if (InformationHolder.showOriginal)
            {
                InformationHolder.con.showOriginal(false);
                InformationHolder.showOriginal = false;
                setBtnActive(btnShowOriginal, false);
            }
            else
            {
                InformationHolder.con.showOriginal(true);
                InformationHolder.showOriginal = true;
                setBtnActive(btnShowOriginal, true);
            }
        }

        public void showPatches()
        {
            if (InformationHolder.showPatches)
            {
                InformationHolder.con.showPatches(false);
                InformationHolder.showPatches = false;
                setBtnActive(btnShowPatches, false);
            }
            else
            {
                InformationHolder.con.showPatches(true);
                InformationHolder.showPatches = true;
                setBtnActive(btnShowPatches, true);
            }
        }

        public void showNewMesh()
        {
            if (InformationHolder.showNewMesh)
            {
                InformationHolder.con.showNewMesh(false);
                InformationHolder.showNewMesh = false;
                setBtnActive(btnShowNewMesh, false);
            }
            else
            {
                InformationHolder.con.showNewMesh(true);
                InformationHolder.showNewMesh = true;
                setBtnActive(btnShowNewMesh, true);
            }
        }

        public void setCamSpeed()
        {
            InformationHolder.camSpeed = sliCamSpeed.value;
        }

        public void setCamRotSpeed()
        {
            InformationHolder.camRotSpeed = sliCamRotSpeed.value;
        }

        public void createFace()
        {
            InformationHolder.con.createFace();
        }

        public void deleteFace() {
            InformationHolder.con.deleteSelectedFace();
        }

        public void increaseSepNumber()
        {
            InformationHolder.con.increaseSepNumber();
        }

        public void decreaseSepNumber()
        {
            InformationHolder.con.decreaseSepNumber();
        }

        public void saveNewMesh()
        {
            if (!itfSaveMeshName.text.Equals(""))
            {
                InformationHolder.con.saveMesh(itfSaveMeshName.text);
            }
        }


        public void setBtnActive(Button btn, bool active)
        {
            if (active)
            {
                btn.GetComponent<Image>().color = Color.grey;
            }
            else
            {
                btn.GetComponent<Image>().color = Color.white;
            }
        }
    }
}