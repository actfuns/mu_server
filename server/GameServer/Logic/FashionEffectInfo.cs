using System;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "TransfigurationFashionEffect", "ID")]
	public class FashionEffectInfo
	{
		
		public int ID;

		
		public string ProPerty;

		
		[TemplateMappingField(Exclude = true)]
		public double[] ExtPropValues = new double[177];
	}
}
