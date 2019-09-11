using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearRegression;

namespace BachelorArbeitUnity
{
    public class SymmetryPlane : MonoBehaviour
    {
        public Plane symPlane;
        public GameObject LineObj;
        List<GameObject> sympoints = new List<GameObject>();

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

        public void setPlane(Vector3 a, Vector3 normal)
        {
            symPlane = new Plane(normal, a);
            transform.rotation = Quaternion.LookRotation(symPlane.normal, Vector3.up);
            transform.position = a;
        }

        public Plane getPlane()
        {
            return symPlane;
        }

        public void fitPlane(List<Vector3> points, LayerMask mask, GameObject LineObj)
        {
            List<Vector3> middlePoints = new List<Vector3>();
            float size = maxDistanceToPlane(points);
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 direction = (points[i] - mirroredPos(points[i])).normalized;
                Vector3 origin = points[i] - 3 * size * direction;
                RaycastHit hit;
                Ray ray = new Ray(origin, direction);

                if (Physics.Raycast(ray, out hit, float.MaxValue, mask))
                {
                    middlePoints.Add((points[i] + hit.point) / 2);
                }
                else
                {
                    throw new System.Exception("No SymmetryPointFound");
                }
            }

            (Vector3, Vector3) plane = fittedPlanes(middlePoints);
            setPlane(plane.Item1, plane.Item2);
        }

        public void fitPlane(int numberOfPoints, MeshStruct m)
        {
            List<Vertex> verts = m.getVertices();
            List<Vector3> middlePoints = new List<Vector3>();

            for (int i = 0; i < numberOfPoints; i++)
            {
                int r = Random.Range(0, verts.Count);
                Vector3 v = verts[r].getPosition();
                Vector3 vMir = mirroredPos(v);

                (float, Vector3) tup = m.minDistanceToPoint(vMir);

                if ((tup.Item2 - vMir).magnitude < 30f)
                {
                    Vector3 middlePoint = (v + tup.Item2) / 2;
                    middlePoints.Add((v + tup.Item2) / 2);
                }
            }

            (Vector3, Vector3) plane = fittedPlanes(middlePoints);
            setPlane(plane.Item1, plane.Item2);
        }

        public (Vector3, Vector3) fittedPlanes(List<Vector3> middlePoints)
        {
            (double[,], double[]) aAndz = MatrixOfList(middlePoints);

            Matrix<double> A = DenseMatrix.OfArray(aAndz.Item1).Transpose();
            Vector<double> z = DenseVector.OfArray(aAndz.Item2);
            Vector<double> p = MultipleRegression.QR(A, z);

            Vector3 normal = new Vector3((float)p[0], (float)p[1], (float)p[2]);
            Vector3 a = normal.normalized / normal.magnitude;

            return (a, normal.normalized);
        }

        public float maxDistanceToPlane(List<Vector3> points)
        {
            float maxDist = 0f;
            foreach (Vector3 v in points)
            {
                maxDist = Mathf.Max(Mathf.Abs(symPlane.GetDistanceToPoint(v)), maxDist);
            }
            return maxDist;
        }

        public (double[,], double[]) MatrixOfList(List<Vector3> points)
        {
            double[,] matrix = new double[3, points.Count];
            double[] vector = new double[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                matrix[0, i] = points[i].x;
                matrix[1, i] = points[i].y;
                matrix[2, i] = points[i].z;
                vector[i] = 1f;
            }

            return (matrix, vector);
        }
    }
}