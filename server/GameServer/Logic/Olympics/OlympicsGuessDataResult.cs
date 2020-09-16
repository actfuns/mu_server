using System;
using System.Collections.Generic;
using ProtoBuf;

namespace GameServer.Logic.Olympics
{
	
	[ProtoContract]
	public class OlympicsGuessDataResult
	{
		
		[ProtoMember(1)]
		public int Type = 0;

		
		[ProtoMember(2)]
		public List<OlympicsGuessData> List = new List<OlympicsGuessData>();

		
		[ProtoMember(3)]
		public int DayID = 0;
	}
}
