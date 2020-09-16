using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class NPCData
	{
		
		[ProtoMember(1)]
		public int MapCode = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public int NPCID = 0;

		
		[ProtoMember(4)]
		public List<int> NewTaskIDs = null;

		
		[ProtoMember(5)]
		public List<int> OperationIDs = null;

		
		[ProtoMember(6)]
		public List<int> ScriptIDs = null;

		
		[ProtoMember(7)]
		public int ExtensionID = 0;

		
		[ProtoMember(8)]
		public List<int> NewTaskIDsDoneCount = null;

		
		[ProtoMember(9)]
		public int GatherTime = 0;
	}
}
