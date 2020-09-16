using System;
using System.Collections.Generic;
using GameServer.Logic.Goods;

namespace GameServer.Logic
{
	
	public class JingLingQiYuanData
	{
		
		public object Mutex = new object();

		
		public List<PetGroupPropertyItem> PetGroupPropertyList = new List<PetGroupPropertyItem>();

		
		public List<PetLevelAwardItem> PetLevelAwardList = new List<PetLevelAwardItem>();

		
		public List<PetSkillLevelAwardItem> PetSkillLevelAwardList = new List<PetSkillLevelAwardItem>();

		
		public List<PetTianFuAwardItem> PetTianFuAwardList = new List<PetTianFuAwardItem>();

		
		public List<PetSkillGroupInfo> PetSkillAwardList = new List<PetSkillGroupInfo>();
	}
}
