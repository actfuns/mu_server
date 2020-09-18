using System;
using System.Collections.Generic;
using ProtoBuf;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	[ProtoContract]
	public class GetBoCaiResult
	{
		
		public GetBoCaiResult()
		{
			this.ItemList = new List<BoCaiBuyItem>();
			this.OpenHistory = new List<BoCaiOpenHistory>();
			this.WinLotteryRoleList = new List<KFBoCaoHistoryData>();
		}

		
		[ProtoMember(1)]
		public int Info;

		
		[ProtoMember(2)]
		public int BocaiType;

		
		[ProtoMember(3)]
		public long NowPeriods;

		
		[ProtoMember(4)]
		public List<BoCaiBuyItem> ItemList;

		
		[ProtoMember(5)]
		public List<KFBoCaoHistoryData> WinLotteryRoleList;

		
		[ProtoMember(6)]
		public long OpenTime;

		
		[ProtoMember(7)]
		public bool IsOpen;

		
		[ProtoMember(8)]
		public string Value1;

		
		[ProtoMember(9)]
		public List<BoCaiOpenHistory> OpenHistory;

		
		[ProtoMember(10)]
		public int Stage;
	}
}
