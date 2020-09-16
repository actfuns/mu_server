using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class GroupMailData
	{
		
		[ProtoMember(1)]
		public int GMailID = 0;

		
		[ProtoMember(2)]
		public string Subject = "";

		
		[ProtoMember(3)]
		public string Content = "";

		
		[ProtoMember(4)]
		public string Conditions = "";

		
		[ProtoMember(5)]
		public long InputTime = 0L;

		
		[ProtoMember(6)]
		public long EndTime = 0L;

		
		[ProtoMember(7)]
		public int Yinliang = 0;

		
		[ProtoMember(8)]
		public int Tongqian = 0;

		
		[ProtoMember(9)]
		public int YuanBao = 0;

		
		[ProtoMember(10)]
		public List<GoodsData> GoodsList = null;
	}
}
