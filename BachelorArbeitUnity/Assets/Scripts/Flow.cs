using System;
using UnityEngine;

namespace BachelorArbeitUnity
{
    public class Flow
	{
		private int amount;
		private int fromSide;
		private int toSide;
		public Flow (int amount,int fromSide,int toSide)
		{
			setAmount (amount);
			setFromSide (fromSide);
			setToSide (toSide);	
		}
		public int getAmount(){
			return amount;
		}
		public int getFromSide(){
			return fromSide;
		}
		public int getToSide(){
			return toSide;
		}
		public void setAmount(int amount){
			this.amount = amount;
		}
		public void setFromSide(int fromSide){
			this.fromSide = fromSide;
		}
		public void setToSide(int toSide){
			this.toSide = toSide;
		}
	}
}

