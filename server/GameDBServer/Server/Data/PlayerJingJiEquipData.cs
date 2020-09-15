using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x020000A0 RID: 160
	[ProtoContract]
	public class PlayerJingJiEquipData
	{
		// Token: 0x06000176 RID: 374 RVA: 0x00007E84 File Offset: 0x00006084
		public string getStringValue()
		{
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				this.EquipId,
				this.Forge_level,
				this.ExcellenceInfo,
				this.BagIndex
			});
		}

		// Token: 0x06000177 RID: 375 RVA: 0x00007EE0 File Offset: 0x000060E0
		public static PlayerJingJiEquipData createPlayerJingJiEquipData(string value)
		{
			PlayerJingJiEquipData result;
			if (value == null || value.Equals(""))
			{
				result = null;
			}
			else
			{
				PlayerJingJiEquipData data = new PlayerJingJiEquipData();
				string[] _value = value.Split(new char[]
				{
					','
				});
				if (_value.Length != 3 && _value.Length != 4)
				{
					result = null;
				}
				else
				{
					data.EquipId = Convert.ToInt32(_value[0]);
					data.Forge_level = Convert.ToInt32(_value[1]);
					data.ExcellenceInfo = Convert.ToInt32(_value[2]);
					if (_value.Length == 4)
					{
						data.BagIndex = Convert.ToInt32(_value[3]);
					}
					result = data;
				}
			}
			return result;
		}

		// Token: 0x0400038A RID: 906
		[ProtoMember(1)]
		public int EquipId;

		// Token: 0x0400038B RID: 907
		[ProtoMember(2)]
		public int Forge_level;

		// Token: 0x0400038C RID: 908
		[ProtoMember(3)]
		public int ExcellenceInfo;

		// Token: 0x0400038D RID: 909
		[ProtoMember(4)]
		public int BagIndex;
	}
}
