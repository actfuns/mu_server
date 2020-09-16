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
		public int Forge_level = 0;

		
		[ProtoMember(5)]
		public int Quality = 0;

		
		[ProtoMember(6)]
		public string Props = "";

		
		[ProtoMember(7)]
		public int GCount = 0;

		
		[ProtoMember(8)]
		public int Binding = 0;

		
		[ProtoMember(9)]
		public int OrigHoleNum = 0;

		
		[ProtoMember(10)]
		public int RMBHoleNum = 0;

		
		[ProtoMember(11)]
		public string Jewellist = "";

		
		[ProtoMember(12)]
		public int AddPropIndex = 0;

		
		[ProtoMember(13)]
		public int BornIndex = 0;

		
		[ProtoMember(14)]
		public int Lucky = 0;

		
		[ProtoMember(15)]
		public int Strong = 0;

		
		[ProtoMember(16)]
		public int ExcellenceInfo = 0;

		
		[ProtoMember(17)]
		public int AppendPropLev = 0;

		
		[ProtoMember(18)]
		public int EquipChangeLifeLev = 0;
	}
}
