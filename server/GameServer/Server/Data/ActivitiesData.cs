using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class ActivitiesData
	{
		
		[ProtoMember(1)]
		public string ActivitiesXmlString = "";
	}
}
