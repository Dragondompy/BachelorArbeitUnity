using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class Starter
	{
		static void Main (string[] args)
		{
			Console.WriteLine ("Choose between grid and figure");
			String line = Console.ReadLine ();
			if (line.Equals ("grid")) {
				grid ();
			} else if(line.Equals ("figure")){
				figure ();
			}
		}

		static void grid ()
		{
			Controller con = new Controller ();

			Console.WriteLine ("Min Value of the Grid");
			int min = (Int32.Parse(Console.ReadLine()));
			Console.WriteLine ("Max Value of the Grid");
			int max = (Int32.Parse(Console.ReadLine()));
			Console.WriteLine ("Step size of the Grid");
			double step = (Int32.Parse(Console.ReadLine()));
			Console.WriteLine ("How fine should the separations be");
			double fine = (Int32.Parse(Console.ReadLine()));

			ObjMesh objm = con.createGrid (min, max, step);
			Console.WriteLine ("File to save to");
			objm.writeToFile (Console.ReadLine ());
			Mesh ownMesh = new Mesh (objm);
			Mesh refinedMesh = con.refine (ownMesh,fine);
			con.fitToFunction (refinedMesh);
			ObjMesh refined = new ObjMesh (refinedMesh);
			Console.WriteLine ("File to save to");
			refined.writeToFile (Console.ReadLine ());
		}

		static void figure(){
			Controller con = new Controller ();

			Console.WriteLine ("OBJ File:");
			ObjMesh objm = new ObjMesh ("Data/" + Console.ReadLine () + ".obj");
			Mesh ownMesh = new Mesh (objm);
			Mesh refinedMesh = con.refine (ownMesh);
			ObjMesh refined = new ObjMesh (refinedMesh);
			Console.WriteLine ("File to save to");
			refined.writeToFile (Console.ReadLine ());
		}
	}
}

