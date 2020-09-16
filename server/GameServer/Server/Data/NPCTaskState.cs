using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NPCTaskState
	{
		
		
		
		[ProtoMember(1)]
		public int NPCID { get; set; }

		
		
		
		[ProtoMember(2)]
		public int TaskState { get; set; }
	}
}
