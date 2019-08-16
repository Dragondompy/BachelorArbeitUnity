using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class SymmetryPlane : MonoBehaviour
    {
        public Plane symPlane;

        public Vector3 mirroredPos(Vector3 pos)
        {
            float distance = symPlane.GetDistanceToPoint(pos);

            return pos - (2 * distance * symPlane.normal);
        }

        public Vector3 getNormal()
        {
            return symPlane.normal;
        }

        public void setPlane(Vector3 a, Vector3 b, Vector3 c)
        {
            symPlane = new Plane(a, b, c);
            transform.rotation = Quaternion.LookRotation(symPlane.normal, Vector3.up);
            transform.position = a;
        }

        public Plane getPlane()
        {
            return symPlane;
        }
    }
}