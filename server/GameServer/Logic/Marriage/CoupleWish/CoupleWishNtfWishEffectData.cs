using System;
using System.Collections.Generic;
using KF.Contract.Data;
using ProtoBuf;

namespace GameServer.Logic.Marriage.CoupleWish
{
	
	[ProtoContract]
	public class CoupleWishNtfWishEffectData
	{
		
		[ProtoMember(1)]
		public KuaFuRoleMiniData From;

		
		[ProtoMember(2)]
		public List<KuaFuRoleMiniData> To;

		
		[ProtoMember(3)]
		public int WishType;

		
		[ProtoMember(4)]
		public string WishTxt;

		
		[ProtoMember(5)]
		public int GetBinJinBi;

		
		[ProtoMember(6)]
		public int GetBindZuanShi;

		
		[ProtoMember(7)]
		public int GetExp;
	}
}
