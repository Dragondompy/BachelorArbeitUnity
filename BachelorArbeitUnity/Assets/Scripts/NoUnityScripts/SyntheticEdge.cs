using System;
using System.Collections.Generic;

namespace BachelorArbeitCSharp
{
	public class SyntheticEdge
	{
		int v1;
		int v2;
		int length;
		List<int> verticesOnEdge;

		public SyntheticEdge ()
		{
		}

		public int getV1(){
			return v1;
		}
		public int getV2(){
			return v2;
		}
		public int getLength(){
			return length;
		}
		public List<int> getVerticesOnEdge(){
			return verticesOnEdge;
		}

		public void setV1(int v1){
			this.v1 = v1;
		}
		public void setV2(int v2){
			this.v2 = v2;
		}
		public void setVerticesOnEdge(List<int> verticesOnEdge){
			this.verticesOnEdge = verticesOnEdge;
			length = verticesOnEdge.Count + 2;
		}
	}
}

