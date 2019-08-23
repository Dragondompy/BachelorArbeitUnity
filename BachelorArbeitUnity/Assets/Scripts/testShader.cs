using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testShader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();

        // make changes to the Mesh by creating arrays which contain the new values
        mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) };
        mesh.colors = new Color[] { new Color(1, 0, 0, 1), new Color(1, 0, 0, 1), new Color(1, 0, 0, 1) };
        mesh.triangles = new int[] { 0, 1, 2 };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
