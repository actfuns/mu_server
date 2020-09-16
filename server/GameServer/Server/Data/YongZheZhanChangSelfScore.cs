using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class YongZheZhanChangSelfScore
	{
		
		[ProtoMember(1)]
		public int AddScore;

		
		[ProtoMember(2)]
		public int ZoneID;

		
		[ProtoMember(3)]
		public string Name = "";

		
		[ProtoMember(4)]
		public int Side;

		
		[ProtoMember(5)]
		public int RoleId;

		
		[ProtoMember(6)]
		public int ByLianShaNum;

		
		[ProtoMember(7)]
		public int Occupation;

		
		[ProtoMember(8)]
		public int TotalScore;
	}
}
