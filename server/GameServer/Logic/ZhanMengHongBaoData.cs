using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ZhanMengHongBaoData
	{
		
		public long[] LastUpdateTicks = new long[3];

		
		public List<HongBaoSendData> HongBaoList = new List<HongBaoSendData>();

		
		public List<HongBaoRankItemData> RecvRankList = new List<HongBaoRankItemData>();

		
		public List<HongBaoRankItemData> SendRankList = new List<HongBaoRankItemData>();
	}
}
