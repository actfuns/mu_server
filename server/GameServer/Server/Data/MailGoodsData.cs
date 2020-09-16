using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class MailGoodsData
	{
		
		[ProtoMember(1)]
		public int Id;

		
		[ProtoMember(2)]
		public int MailID = 0;

		
		[ProtoMember(3)]
		public int GoodsID = 0;

		
		[ProtoMember(4)]
		public int Forge_level;

		
		[ProtoMember(5)]
		public int Quality;

		
		[ProtoMember(6)]
		public string Props;

		
		[ProtoMember(7)]
		public int GCount;

		
		[ProtoMember(8)]
		public int Binding;

		
		[ProtoMember(9)]
		public int OrigHoleNum = 0;

		
		[ProtoMember(10)]
		public int RMBHoleNum = 0;

		
		[ProtoMember(11)]
		public string Jewellist;

		
		[ProtoMember(12)]
		public int AddPropIndex;

		
		[ProtoMember(13)]
		public int BornIndex;

		
		[ProtoMember(14)]
		public int Lucky;

		
		[ProtoMember(15)]
		public int Strong;

		
		[ProtoMember(16)]
		public int ExcellenceInfo;

		
		[ProtoMember(17)]
		public int AppendPropLev;

		
		[ProtoMember(18)]
		public int EquipChangeLifeLev;

		
		public int Site;
	}
}
