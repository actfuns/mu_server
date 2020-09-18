using System;
using System.Collections.Generic;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	[Serializable]
	public class TianTi5v5ZhanDuiMiniData
	{
		
		[ProtoMember(1)]
		public int ZhanDuiID;

		
		[ProtoMember(2)]
		public string Name;

		
		[ProtoMember(3)]
		public string DuiZhangName;

		
		[ProtoMember(4)]
		public int DuanWeiID;

		
		[ProtoMember(5)]
		public List<RoleNameLevelData> MemberList;

		
		[ProtoMember(6)]
		public string XuanYan;

		
		[ProtoMember(7)]
		public long ZhanDouLi;
	}
}
