using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YueKaData
	{
		
		public YueKaData()
		{
			this.HasYueKa = false;
			this.CurrDay = 0;
			this.AwardInfo = "";
			this.RemainDay = 0;
		}

		
		[ProtoMember(1)]
		public bool HasYueKa;

		
		[ProtoMember(2)]
		public int CurrDay;

		
		[ProtoMember(3)]
		public string AwardInfo;

		
		[ProtoMember(4)]
		public int RemainDay;
	}
}
