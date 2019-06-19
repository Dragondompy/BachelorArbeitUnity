using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class UStarter : MonoBehaviour
    {
        public GameObject Vertex;
        public GameObject Mesh;
        // Start is called before the first frame update
        void Start()
        {
            GameObject MeshOB = Instantiate(Mesh, new Vector3(0, 0, 0), Quaternion.identity);

            ObjMesh o = new ObjMesh("./Assets/Data/output.obj");
            Mesh m = MeshOB.GetComponent<Mesh>();
            m.loadMeshFromObj(o);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void testProgramm()
        {
            UnityEngine.Mesh pol = GetComponent<MeshFilter>().mesh;

            pol.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0) };
            pol.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

            gameObject.GetComponent<MeshFilter>().mesh = pol;
        }
    }
}