using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class NewZoneUpLevelItemData
	{
		
		[ProtoMember(1)]
		public int LeftNum;

		
		[ProtoMember(2)]
		public bool GetAward;
	}
}
