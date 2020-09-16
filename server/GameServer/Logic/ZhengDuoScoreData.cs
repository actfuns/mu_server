using System;
using ProtoBuf;

namespace GameServer.Logic
{
	
	[ProtoContract]
	public class ZhengDuoScoreData
	{
		
		public ZhengDuoScoreData()
		{
		}

		
		public ZhengDuoScoreData(int id, string name, long score)
		{
			this.Id = id;
			this.Name = name;
			this.Score = score;
		}

		
		public static int Compare_static(ZhengDuoScoreData x, ZhengDuoScoreData y)
		{
			int result;
			if (x == y)
			{
				result = 0;
			}
			else if (x != null && y != null)
			{
				long ret = y.Score - x.Score;
				if (ret > 0L)
				{
					result = 1;
				}
				else if (ret == 0L)
				{
					result = y.Id - x.Id;
				}
				else
				{
					result = -1;
				}
			}
			else if (x == null)
			{
				result = 1;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		
		[ProtoMember(1)]
		public string Name;

		
		[ProtoMember(2)]
		public long Score;

		
		[ProtoMember(3)]
		public int Id;
	}
}
