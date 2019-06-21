using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class UStarter : MonoBehaviour
    {
        public GameObject Mesh;
        public string objName;
        // Start is called before the first frame update
        void Start()
        {
            testProgramm();
            /*
            long startmil = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            GameObject emp = new GameObject();
            for (int i = 0; i < 50; i++) {
                for (int j = 0; j < 50; j++)
                {
                }
            }
            print(DateTimeOffset.Now.ToUnixTimeMilliseconds()-startmil);*/
        }

        // Update is called once per frame
        void Update()
        {

        }

        void testProgramm()
        {
            GameObject MeshOB = Instantiate(Mesh, new Vector3(0, 0, 0), Quaternion.identity);

            ObjMesh o = new ObjMesh("./Assets/Data/" + objName + ".obj");
            Mesh m = MeshOB.GetComponent<Mesh>();
            m.loadMeshFromObj(o);
        }
    }
}