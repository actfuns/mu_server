using System;
using ProtoBuf;

namespace GameServer.Logic
{
	// Token: 0x02000427 RID: 1063
	[ProtoContract]
	public class ZhengDuoScoreData
	{
		// Token: 0x06001361 RID: 4961 RVA: 0x001323C8 File Offset: 0x001305C8
		public ZhengDuoScoreData()
		{
		}

		// Token: 0x06001362 RID: 4962 RVA: 0x001323D3 File Offset: 0x001305D3
		public ZhengDuoScoreData(int id, string name, long score)
		{
			this.Id = id;
			this.Name = name;
			this.Score = score;
		}

		// Token: 0x06001363 RID: 4963 RVA: 0x001323F4 File Offset: 0x001305F4
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

		// Token: 0x04001C88 RID: 7304
		[ProtoMember(1)]
		public string Name;

		// Token: 0x04001C89 RID: 7305
		[ProtoMember(2)]
		public long Score;

		// Token: 0x04001C8A RID: 7306
		[ProtoMember(3)]
		public int Id;
	}
}
