using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
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

        public void fitPlane(int numberOfPoints, MeshStruct m)
        {
            List<Vertex> verts = m.getVertices();
            List<Vector3> middlePoints = new List<Vector3>();
            List<float> weightList = new List<float>();

            for (int i = 0; i < numberOfPoints; i++)
            {
                int r = Random.Range(0, verts.Count);
                Vector3 v = verts[r].getPosition();
                Vector3 vMir = mirroredPos(v);

                (float, Vector3) tup = m.minDistanceToPoint(vMir);

                if ((tup.Item2 - vMir).magnitude < 30f)
                {
                    Vector3 middlePoint = (v + tup.Item2) / 2;
                    middlePoints.Add(middlePoint);
                    weightList.Add(tup.Item1);
                }
            }
            //Draw middlePoints
            Vector3 middleOfMiddlePoints = new Vector3(0,0,0);
            Vector3 prev = middlePoints[0];
            Color color = new Color(Random.Range(0F, 1F), Random.Range(0, 1F), Random.Range(0, 1F));
            foreach (Vector3 v in middlePoints)
            {
                Debug.DrawLine(prev, v, color,float.MaxValue);
                middleOfMiddlePoints += v;
                prev = v;
            }

            middleOfMiddlePoints = middleOfMiddlePoints / middlePoints.Count;
            (Vector3, Vector3) plane = fittedPlanes(middlePoints, weightList,middleOfMiddlePoints);
            setPlane(plane.Item1, plane.Item2);
        }

        public (Vector3, Vector3) fittedPlanes(List<Vector3> middlePoints, List<float> weightList, Vector3 middleOfMiddlePoints)
        {
            double[,] matA = MatrixOfList(middlePoints,middleOfMiddlePoints);

            Matrix<double> A = DenseMatrix.OfArray(matA);
            Debug.Log(A);
            Matrix<double> eV = A.Transpose().Multiply(A).Evd().EigenVectors;
            //Debug.Log(A.Transpose().Multiply(A));
            Debug.Log(eV);
            var feV = eV.Column(0);
            //Debug.Log(feV);
            var normal = new Vector3((float)feV[0], (float)feV[1], (float)feV[2]);
            
            return (middleOfMiddlePoints, normal.normalized);
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

        public double[,] MatrixOfList(List<Vector3> points, Vector3 middleOfMiddlePoints)
        {
            double[,] matrix = new double[points.Count,3];
            for (int i = 0; i < points.Count; i++)
            {
                matrix[i,0] = points[i].x - middleOfMiddlePoints.x;
                matrix[i,1] = points[i].y - middleOfMiddlePoints.y;
                matrix[i,2] = points[i].z - middleOfMiddlePoints.z;
            }

            return matrix;
        }

        public double[,] diagMatrixOfList(List<float> list)
        {
            double[,] matrix = new double[list.Count,list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                matrix[i,i] = 1f;
            }
            return matrix;
        }
    }
}