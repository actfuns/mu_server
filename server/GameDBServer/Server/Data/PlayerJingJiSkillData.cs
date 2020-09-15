using System;
using ProtoBuf;

namespace Server.Data
{
	// Token: 0x02000096 RID: 150
	[ProtoContract]
	public class PlayerJingJiSkillData
	{
		// Token: 0x0600014C RID: 332 RVA: 0x00007774 File Offset: 0x00005974
		public string getStringValue()
		{
			return string.Format("{0},{1}", this.skillID, this.skillLevel);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000077A8 File Offset: 0x000059A8
		public static PlayerJingJiSkillData createPlayerJingJiSkillData(string value)
		{
			PlayerJingJiSkillData result;
			if (value == null || value.Equals(""))
			{
				result = null;
			}
			else
			{
				string[] _value = value.Split(new char[]
				{
					','
				});
				if (_value.Length != 2)
				{
					result = null;
				}
				else
				{
					result = new PlayerJingJiSkillData
					{
						skillID = Convert.ToInt32(_value[0]),
						skillLevel = Convert.ToInt32(_value[1])
					};
				}
			}
			return result;
		}

		// Token: 0x04000369 RID: 873
		[ProtoMember(1)]
		public int skillID;

		// Token: 0x0400036A RID: 874
		[ProtoMember(2)]
		public int skillLevel;
	}
}
