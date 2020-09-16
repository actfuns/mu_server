using System;
using ProtoBuf;

namespace GameServer.TarotData
{
	
	[ProtoContract]
	public class TarotCardData
	{
		
		public string GetDataStrInfo()
		{
			return string.Format("{0}_{1}_{2}_{3}", new object[]
			{
				this.GoodId,
				this.Level,
				this.Postion,
				this.TarotMoney
			});
		}

		
		[ProtoMember(1)]
		public int GoodId = 0;

		
		[ProtoMember(2)]
		public int Level = 0;

		
		[ProtoMember(3)]
		public byte Postion = 0;

		
		[ProtoMember(4)]
		public int TarotMoney;
	}
}
