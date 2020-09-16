using System;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	
	[ProtoContract]
	public class OlympicsGuessData
	{
		
		public OlympicsGuessData Clone(OlympicsGuessData data)
		{
			this.ID = data.ID;
			this.DayID = data.DayID;
			this.Content = data.Content;
			this.A = data.A;
			this.B = data.B;
			this.C = data.C;
			this.D = data.D;
			this.Answer = data.Answer;
			this.Grade = data.Grade;
			this.Select = data.Select;
			return this;
		}

		
		[ProtoMember(1)]
		public int ID = 0;

		
		[ProtoMember(2)]
		public int DayID = 0;

		
		[ProtoMember(3)]
		public string Content = "";

		
		[ProtoMember(4)]
		public string A = "";

		
		[ProtoMember(5)]
		public string B = "";

		
		[ProtoMember(6)]
		public string C = "";

		
		[ProtoMember(7)]
		public string D = "";

		
		[ProtoMember(8)]
		public int Answer = -1;

		
		[ProtoMember(9)]
		public int Grade = 0;

		
		[ProtoMember(10)]
		public int Select = -1;
	}
}
