using System;
using System.Collections.Generic;
using GameServer.Logic;

namespace Server.Data
{
	
	public class TalentPropData
	{
		
		public TalentPropData()
		{
			this.ResetProps();
		}

		
		public void ResetProps()
		{
			this.PropItem.ResetProps();
			this.SkillOneValue = new Dictionary<int, int>();
			this.SkillAllValue = 0;
		}

		
		public Dictionary<int, int> SkillOneValue = new Dictionary<int, int>();

		
		public int SkillAllValue = 2;

		
		public EquipPropItem PropItem = new EquipPropItem();
	}
}
