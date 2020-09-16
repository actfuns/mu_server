using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "Rose", "ID")]
	public class JingLingYuanSuShuXingInfo
	{
		
		public int ID;

		
		public int Tipe;

		
		public int Level;

		
		public string AcetiveElement;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtProps = new double[177];
	}
}
