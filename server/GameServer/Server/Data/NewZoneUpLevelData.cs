using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class NewZoneUpLevelData
	{
		
		[ProtoMember(1)]
		public List<NewZoneUpLevelItemData> Items;
	}
}
