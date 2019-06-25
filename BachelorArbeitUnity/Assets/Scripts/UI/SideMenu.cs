using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class SideMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            InformationHolder.selectVertices = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void setSelect() {
            if (InformationHolder.selectVertices)
            {
                InformationHolder.selectVertices = false;
            }
            else
            {
                InformationHolder.selectVertices = true;
            }
        }
    }
}