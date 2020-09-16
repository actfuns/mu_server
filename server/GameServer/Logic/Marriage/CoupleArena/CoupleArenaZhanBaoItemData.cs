using System;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleArena
{
	
	[ProtoContract]
	public class CoupleArenaZhanBaoItemData
	{
		
		[ProtoMember(1)]
		public int TargetManZoneId;

		
		[ProtoMember(2)]
		public string TargetManRoleName;

		
		[ProtoMember(3)]
		public int TargetWifeZoneId;

		
		[ProtoMember(4)]
		public string TargetWifeRoleName;

		
		[ProtoMember(5)]
		public bool IsWin;

		
		[ProtoMember(6)]
		public int GetJiFen;
	}
}
