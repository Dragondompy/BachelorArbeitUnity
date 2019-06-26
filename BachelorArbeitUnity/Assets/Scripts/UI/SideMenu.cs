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
        public Button btnShowOriginal;
        public Button btnShowPatches;
        public Button btnShowNewMesh;
        public Button btnSaveNewMesh;
        public InputField ipfSaveMeshName;

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
        }

        public void setSelectVertex()
        {
            if (InformationHolder.selectVertices)
            {
                InformationHolder.selectVertices = false;
                setBtnActive(btnSelectVert, false);
            }
            else
            {
                InformationHolder.selectVertices = true;
                setBtnActive(btnSelectVert, true);
            }
        }

        public void setSelectEdge()
        {
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

        public void createFace()
        {
            InformationHolder.con.createFace();
        }

        public void saveNewMesh()
        {
            InformationHolder.con.saveMesh(ipfSaveMeshName.text);
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