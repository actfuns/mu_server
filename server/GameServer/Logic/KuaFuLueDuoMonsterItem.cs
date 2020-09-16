using System;

namespace GameServer.Logic
{
	
	public class KuaFuLueDuoMonsterItem
	{
		
		public object Clone()
		{
			return new KuaFuLueDuoMonsterItem
			{
				ID = this.ID,
				MonsterID = this.MonsterID,
				Type = this.Type,
				Name = this.Name,
				GatherTime = this.GatherTime,
				FuHuoTime = this.FuHuoTime,
				ZiYuan = this.ZiYuan,
				JiFen = this.JiFen,
				X = this.X,
				Y = this.Y
			};
		}

		
		public int ID;

		
		public int MonsterID;

		
		public int Type;

		
		public string Name;

		
		public int GatherTime;

		
		public int FuHuoTime;

		
		public int ZiYuan;

		
		public int JiFen;

		
		public int X;

		
		public int Y;

		
		public bool Alive;

		
		public long FuHuoTicks;
	}
}
