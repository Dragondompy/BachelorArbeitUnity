using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class ControllerCSharp
    {

		public ControllerCSharp ()
		{
		}

		public Mesh refine (Mesh oldMesh, double fine)
		{
			Mesh newMesh = new Mesh ();
            newMesh.loadMeshFromMesh(oldMesh);
			foreach (Edge e in oldMesh.getEdges()) {
				Vertex v1 = e.getV1 ();
				Vertex v2 = e.getV2 ();
				double z1 = functionToFit (v1);
				double z2 = functionToFit (v2);
				e.setSepNumber (Convert.ToInt32 ((Math.Abs (z1 - z2) + 1) * fine) * 2);
			}

			addVerticesOnEdge (oldMesh, newMesh);

			foreach (Face f in oldMesh.getFaces()) {
				executePatch (f, newMesh, oldMesh);
			}
			return newMesh;
		}

		public Mesh refine (Mesh oldMesh)
        {
            Mesh newMesh = new Mesh();
            newMesh.loadMeshFromMesh(oldMesh);
            Console.WriteLine ("Specify the Number for each Edge or random or all the same ? (each/random/same)");
            String line = "same";//Console.ReadLine ();
			if (line.Equals ("each")) {
				foreach (Edge e in oldMesh.getEdges()) {
					Console.WriteLine (e);
					Console.WriteLine ("How fine should the given Edge be ?");
					int fine = (Int32.Parse (Console.ReadLine ()));
					e.setSepNumber (fine);
					Console.WriteLine ();
				}
			} else if (line.Equals ("random")) {
				Console.WriteLine ("Min Value for the Edges");
				int min = (Int32.Parse (Console.ReadLine ()))/2;
				Console.WriteLine ("Max Value for the Edges");
				int max = (Int32.Parse (Console.ReadLine ()))/2;

				System.Random random = new System.Random ();
				foreach (Edge e in oldMesh.getEdges()) {
					e.setSepNumber (Convert.ToInt32(random.Next (min, max))*2);
				}
			} else if (line.Equals ("same")) {
				Console.WriteLine ("How fine should the new Mesh be");
                int fine = 40;// (Int32.Parse (Console.ReadLine ()));
				foreach (Edge e in oldMesh.getEdges()) {
					e.setSepNumber (fine);
				}
			}


			addVerticesOnEdge (oldMesh, newMesh);

			foreach (Face f in oldMesh.getFaces()) {
				executePatch (f, newMesh, oldMesh);
			}
			return newMesh;
		}

		//Fits all Vertices to the given function
		public void fitToFunction (Mesh refinedMesh)
		{
			foreach (Vertex v in refinedMesh.getVertices()) {
                Vector3 pos = v.getPosition();
                pos.x = functionToFit(v);
                v.setPosition(pos);
			}
		}

		//Adds new Faces to the New Mesh replacing the old Face in the Old Mesh
		public void executePatch (Face face, Mesh newMesh, Mesh oldMesh)
		{
			int sumOfSepNumbers = 0;
			foreach (Edge e in face.getEdges()) {
				e.getHalfEdge (face).setReduced (e.getSepNumber ());

				sumOfSepNumbers += e.getSepNumber ();
			}

			if (sumOfSepNumbers % 2 != 0) {
				throw new Exception ("the Sum of Seperation in a face Must be even " + sumOfSepNumbers + " is not even");
			}

			reducePattern (face);

			List<int> reducedValues = new List<int> ();
			foreach (HalfEdge he in face.getHalfEdges()) {
				reducedValues.Add (he.getReduced ());
				Console.WriteLine (he.getReduced ());
			}
			Patterndecider patDec = createPatterndecider (reducedValues);
			Pattern p = new Pattern (patDec.choosePattern ());
			p.fillPatch (oldMesh, newMesh, face);
		}

		//Adds all vertices on All Edges
		public void addVerticesOnEdge (Mesh oldMesh, Mesh newMesh)
		{
			List<Edge> edges = oldMesh.getEdges ();

			foreach (Edge e in edges) {
				e.setVerticesOnEdge (addVerticesBetween (e.getV1 (), e.getDirection (), e, newMesh));
			}
		}

		//Adds the Vertices on the Edge
		public Vertex[] addVerticesBetween (Vertex v1, Vector3 direction, Edge edge, Mesh newMesh)
		{
			int sepNumber = edge.getSepNumber ();

			Vertex[] newVertices = new Vertex[edge.getSepNumber () - 1];

			for (int i = 0; i < edge.getSepNumber () - 1; i++) {
                Vector3 pos = v1.getPosition() + (direction / sepNumber * (i + 1));
				newVertices [i] = newMesh.addVertex (pos);
			}

			return newVertices;
		}

		//Reduces the seperator count on each side by adding trivial quads in the  current patch
		//Does not actually add the quads, just calculates the amount of seperations that remain
		//Also adds the flow to each HalfEdge
		//Provides better Pattern than the old Function
		public void reducePattern (Face face)
		{
			List<HalfEdge> halfEdges = face.getHalfEdges ();

			int size = halfEdges.Count;
			int noFlowCount = 0;
			int counter = size;

			while (noFlowCount < size) {
				int prevHe = counter % size;
				int curHe = (counter + 1) % size;
				int nextHe = (counter + 2) % size;

				int rem = Math.Min (halfEdges [prevHe].getReduced (), halfEdges [nextHe].getReduced ()) - 1;
				if (rem > 0) {
					halfEdges [curHe].addOuterFlow ();
					halfEdges [prevHe].reduceSep ();
					halfEdges [nextHe].reduceSep ();
					noFlowCount = 0;
				} else {
					noFlowCount++;
				}
				counter++;
			}

			foreach (HalfEdge he in halfEdges) {
				he.createVerticesArray ();
			}
		}

		//Reduces the seperator count on each side by adding trivial quads in the  current patch
		//Does not actually add the quads, just calculates the amount of seperations that remain
		//Also adds the flow to each HalfEdge
		//better patterns are created if the outerFlow is mostly equal on all sides
		//therefore use the new Pattern Function
		public void reducePatternOld (Face face)
		{
			List<HalfEdge> halfEdges = face.getHalfEdges ();

			int size = halfEdges.Count;
			int rem;

			for (int i = 0; i < size - 2; i++) {
				rem = Math.Min (halfEdges [i].getReduced (), halfEdges [i + 2].getReduced ()) - 1;
				if (rem > 0) {
					halfEdges [i + 1].setOuterFlow (rem);
					halfEdges [i].setReduced (halfEdges [i].getReduced () - rem);
					halfEdges [i + 2].setReduced (halfEdges [i + 2].getReduced () - rem);
				}
			}

			rem = Math.Min (halfEdges [size - 2].getReduced (), halfEdges [0].getReduced ()) - 1;
			halfEdges [size - 2].setReduced (halfEdges [size - 2].getReduced () - rem);
			halfEdges [0].setReduced (halfEdges [0].getReduced () - rem);
			halfEdges [size - 1].setOuterFlow (rem);

			rem = Math.Min (halfEdges [size - 1].getReduced (), halfEdges [1].getReduced ()) - 1;
			halfEdges [size - 1].setReduced (halfEdges [size - 1].getReduced () - rem);
			halfEdges [1].setReduced (halfEdges [1].getReduced () - rem);
			halfEdges [0].setOuterFlow (rem);

			foreach (HalfEdge he in halfEdges) {
				he.createVerticesArray ();
			}
		}

		//creates new class "Patterndecider" which has all the information
		//to choose the right pattern for the current patch
		//t contains the information on alhpa and beta
		public Patterndecider createPatterndecider (List<int> reducedPattern)
		{
			int c = 0;
			List<int> t = new List<int> ();
			foreach (int i in reducedPattern) {
				if (i > 1) {
					t.Add (i);
					t.Add (c);
				}
				c++;
			}
			if (t.Count > 4) {
				throw new Exception ("Error while reducing the pattern");
			}
			Patterndecider pat = new Patterndecider (reducedPattern.Count, t);
			return pat;

		}

		//makes a deepCopy of the given List
		public List<int> copyList (List<int> oldList)
		{
			List<int> newList = new List<int> ();
			foreach (int t in oldList) {
				newList.Add (t);
			}
			return newList;
		}

        public ObjMesh createGrid(float min, float max, float step)
        {
            int steps = Convert.ToInt32((max - min) / step);
            List<Vector3> vertices = new List<Vector3>();
            List<List<int>> faces = new List<List<int>>();
            for (int i = 0; i <= steps; i++)
            {
                for (int j = 0; j <= steps; j++)
                {
                    Vector3 v = new Vector3( min + (j * step), min + (i * step), 0 );
                    vertices.Add(v);
                }
            }
            for (int i = 0; i < steps; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    List<int> f = new List<int>();
                    f.Add(i * (steps + 1) + j);
                    f.Add(i * (steps + 1) + j + 1);
                    f.Add((i + 1) * (steps + 1) + j + 1);
                    f.Add((i + 1) * (steps + 1) + j);
                    faces.Add(f);
                }
            }
            string comments = "#" + ((steps + 1) * (steps + 1)) + " vertices, " + (steps * steps) + " faces";
            return new ObjMesh(vertices, faces, comments);
        }

        //returns the value of the function at (x,y)
        public float functionToFit(float x, float y)
        {
            return (25.0f - (x * y)) / 5f;
        }

        public float functionToFit(Vertex v)
        {
            return functionToFit(v.getPosition().x, v.getPosition().y);
        }
    }
}

