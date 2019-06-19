using System;
using System.Collections.Generic;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Patterndecider : MonoBehaviour
	{
		private int size;
		private int alpha;
		private int posAlpha;
		private int beta;
		private int posBeta;
		private int space;

		public Patterndecider (int size, List<int> values)
		{
			this.size = size;

			if (values.Count == 4) {
				int val1 = values [0];
				int posVal1 = values [1];
				int val2 = values [2];
				int posVal2 = values [3];

				if (val1 >= val2) {
					alpha = val1;
					posAlpha = posVal1;
					beta = val2;
					posBeta = posVal2;
				} else {
					alpha = val2;
					posAlpha = posVal2;
					beta = val1;
					posBeta = posVal1;
				}
				space = Math.Min ((posAlpha - posBeta + size) % size, (posBeta - posAlpha + size) % size);
			} else if (values.Count == 2) {
				alpha = values [0];
				posAlpha = values [1];
				beta = 1;
				posBeta = -1;
				space = 0;
			} else {
				alpha = 1;
				posAlpha = -1;
				beta = 1;
				posBeta = -1;
				space = 0;
			}
		}

		public EmptyPattern choosePattern ()
		{
			switch (size) {

			case 3:
				if (alpha == 2)
					return new Pattern30 (alpha, beta);
				else
					return new Pattern31 (alpha, beta);
				

			case 4:
				if (beta == 1) {
					if (alpha == 1)
						return new Pattern40 (alpha, beta);
					/*else if (alpha == 3)
						return new Pattern42 (alpha, beta);*/
					else
						return new Pattern43 (alpha, beta);
					
				} else if (beta == alpha)
					return new Pattern41 (alpha, beta);
				else
					return new Pattern44 (alpha, beta);
				

			case 5:
				if (beta == 1) {
					if (alpha == 2)
						return new Pattern50 (alpha, beta);
					else
						return new Pattern52 (alpha, beta);
					
				} else if (beta + 1 == alpha)
					return new Pattern51 (alpha, beta);
				else
					return new Pattern53 (alpha, beta);
				

			case 6:
				if (beta == alpha) {
					if (space == 3)
						return new Pattern60 (alpha, beta);
					else
						return new Pattern61 (alpha, beta);
				} else {
					if (space == 3)
						return new Pattern62 (alpha, beta);
					else
						return new Pattern63 (alpha, beta);
				}
			}
			throw new Exception ("The right pattern could not be choosen \n \n" + this);
		}

		public int getSize ()
		{
			return size;
		}

		public int getAlpha ()
		{
			return alpha;
		}

		public int getBeta ()
		{
			return beta;
		}

		public int getPosAlpha ()
		{
			return posAlpha;
		}

		public int getPosBeta ()
		{
			return posBeta;
		}

		public int getSpace ()
		{
			return space;
		}

		public override String ToString ()
		{
			String h = "";
			h += "Size: " + size + "\n";
			h += "Alpha: " + alpha + "\n";
			h += "Pos of Alpha: " + posAlpha + "\n";
			h += "Pos of Beta: " + posBeta + "\n";
			h += "Space: " + space + "\n";
			return h;
		}
	}
}

