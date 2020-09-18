using System;
using ProtoBuf;

namespace GameDBServer.Logic.UserReturn
{
	
	[ProtoContract]
	public class ReturnActivity
	{
		
		[ProtoMember(1)]
		public bool IsOpen;

		
		[ProtoMember(2)]
		public DateTime NotLoggedInBegin;

		
		[ProtoMember(3)]
		public DateTime NotLoggedInFinish;

		
		[ProtoMember(4)]
		public int Level;

		
		[ProtoMember(5)]
		public int VIPNeedExp;

		
		[ProtoMember(6)]
		public int ActivityID;

		
		[ProtoMember(7)]
		public string ActivityDay;
	}
}
