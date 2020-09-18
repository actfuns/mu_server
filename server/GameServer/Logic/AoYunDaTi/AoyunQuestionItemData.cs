using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.AoYunDaTi
{
	
	[ProtoContract]
	public class AoyunQuestionItemData
	{
		
		[ProtoMember(1)]
		public int QuestionId = 0;

		
		[ProtoMember(2)]
		public string Question = null;

		
		[ProtoMember(3)]
		public string[] AnswerContentArray;

		
		[ProtoMember(4)]
		public bool UseTianShi;

		
		[ProtoMember(5)]
		public bool UseEMo;

		
		[ProtoMember(6)]
		public int RoleAnswer;

		
		[ProtoMember(7)]
		public DateTime EndTime;

		
		[ProtoMember(8)]
		public List<bool> QuestionState;

		
		[ProtoMember(9)]
		public int RolePoint;
	}
}
