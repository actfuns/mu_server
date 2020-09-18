using System;
using ProtoBuf;

namespace GameDBServer.Logic.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaZhanBaoSaveDbData
	{
		
		[ProtoMember(1)]
		public CoupleArenaZhanBaoItemData ZhanBao;

		
		[ProtoMember(2)]
		public int FirstWeekday;

		
		[ProtoMember(3)]
		public int FromMan;

		
		[ProtoMember(4)]
		public int FromWife;

		
		[ProtoMember(5)]
		public int ToMan;

		
		[ProtoMember(6)]
		public int ToWife;
	}
}
