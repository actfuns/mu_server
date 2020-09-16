using System;
using System.Collections.Generic;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	[TemplateMappingOptions(null, "LegionTasks", "ID")]
	public class JunTuanTaskInfo
	{
		
		public int ID;

		
		public int CompleteType;

		
		public string Name;

		
		[TemplateMappingField(SpliteChars = ",")]
		public List<int> TypeID;

		
		public int NumInterval;

		
		public int Exp;

		
		public int ZhanGong;

		
		public int Score;

		
		[TemplateMappingField(ParseMethod = "AddNoRepeat")]
		public AwardsItemList Item;
	}
}
