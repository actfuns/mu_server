using System;
using System.Collections.Generic;

namespace GameServer.Logic.ExtensionProps
{
	
	public class ExtensionPropItem
	{
		
		public int ID = 0;

		
		public Dictionary<int, byte> PrevTuoZhanShuXing = null;

		
		public int TargetType = 0;

		
		public int ActionType = 0;

		
		public int Probability = 0;

		
		public Dictionary<int, byte> NeedSkill = null;

		
		public int Icon = 0;

		
		public int TargetDecoration = 0;

		
		public int DelayDecoration = 0;
	}
}
