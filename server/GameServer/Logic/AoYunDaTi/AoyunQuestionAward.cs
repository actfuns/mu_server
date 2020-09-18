using System;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	
	[ProtoContract]
	public class AoyunQuestionAward
	{
		
		[ProtoMember(1)]
		public int Result;

		
		[ProtoMember(2)]
		public int RightAnswer;

		
		[ProtoMember(3)]
		public int TianShiCount;

		
		[ProtoMember(4)]
		public int EMoCount;

		
		[ProtoMember(5)]
		public int RolePoint;
	}
}
