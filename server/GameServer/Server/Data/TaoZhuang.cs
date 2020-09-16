using System;
using System.Collections.Generic;

namespace Server.Data
{
	
	public class TaoZhuang
	{
		
		public int ID;

		
		public int Type;

		
		public List<int> AwakenList;

		
		public int TaoZhuangProps1Num;

		
		public double[] TaoZhuangProps1 = new double[177];

		
		public int TaoZhuangProps2Num;

		
		public double[] TaoZhuangProps2 = new double[177];

		
		public int TaoZhuangProps3Num;

		
		public double[] TaoZhuangProps3 = new double[177];

		
		public int WeaponMasterNum;

		
		public int WeaponMasterType;

		
		public List<List<int>> PassiveSkill;

		
		public List<List<int>> PassiveEffect;
	}
}
