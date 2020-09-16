using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class TalentEffectInfo
	{
		
		[ProtoMember(1, IsRequired = true)]
		public int EffectType = 0;

		
		[ProtoMember(2, IsRequired = true)]
		public int EffectID = 0;

		
		[ProtoMember(3, IsRequired = true)]
		public double EffectValue = 0.0;
	}
}
