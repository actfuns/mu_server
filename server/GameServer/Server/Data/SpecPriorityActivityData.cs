using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpecPriorityActivityData
	{
		
		[ProtoMember(1)]
		public Dictionary<int, int> ConditionDict = new Dictionary<int, int>();

		
		[ProtoMember(2)]
		public List<SpecPriorityActInfo> SpecActInfoList = new List<SpecPriorityActInfo>();

		
		[ProtoMember(3)]
		public int DonateNum;

		
		[ProtoMember(4)]
		public int DonateNumKF;
	}
}
