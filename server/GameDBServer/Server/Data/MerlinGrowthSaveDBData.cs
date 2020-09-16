using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MerlinGrowthSaveDBData
	{
		
		[ProtoMember(1)]
		public int _RoleID = 0;

		
		[ProtoMember(2)]
		public int _Occupation = 0;

		
		[ProtoMember(3)]
		public int _Level = 0;

		
		[ProtoMember(4)]
		public int _StarNum = 0;

		
		[ProtoMember(5)]
		public int _StarExp = 0;

		
		[ProtoMember(6)]
		public int _LuckyPoint = 0;

		
		[ProtoMember(7)]
		public long _ToTicks = 0L;

		
		[ProtoMember(8)]
		public long _AddTime = 0L;

		
		[ProtoMember(9)]
		public Dictionary<int, double> _ActiveAttr = new Dictionary<int, double>();

		
		[ProtoMember(10)]
		public Dictionary<int, double> _UnActiveAttr = new Dictionary<int, double>();

		
		[ProtoMember(11)]
		public int _LevelUpFailNum = 0;
	}
}
