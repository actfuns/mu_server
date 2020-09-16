using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiEquipData
	{
		
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

		
		[ProtoMember(1)]
		public int EquipId;

		
		[ProtoMember(2)]
		public int Forge_level;

		
		[ProtoMember(3)]
		public int ExcellenceInfo;

		
		[ProtoMember(4)]
		public int BagIndex;
	}
}
