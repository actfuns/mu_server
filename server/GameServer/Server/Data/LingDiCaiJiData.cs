using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class LingDiCaiJiData
	{
		
		[ProtoMember(1)]
		public int LingDiType;

		
		[ProtoMember(2)]
		public DateTime BeginTime;

		
		[ProtoMember(3)]
		public DateTime EndTime;

		
		[ProtoMember(4)]
		public bool HaveJunTuan;

		
		[ProtoMember(5)]
		public string ZhanLingName;
	}
}
