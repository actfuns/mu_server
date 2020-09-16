using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiSkillData
	{
		
		public string getStringValue()
		{
			return string.Format("{0},{1}", this.skillID, this.skillLevel);
		}

		
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

		
		[ProtoMember(1)]
		public int skillID;

		
		[ProtoMember(2)]
		public int skillLevel;
	}
}
