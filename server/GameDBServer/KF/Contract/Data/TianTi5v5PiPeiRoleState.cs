using System;
using ProtoBuf;

namespace KF.Contract.Data
{
	
	[ProtoContract]
	[Serializable]
	public class TianTi5v5PiPeiRoleState
	{
		
		[ProtoMember(1)]
		public string RoleName;

		
		[ProtoMember(2)]
		public int Occupation;

		
		[ProtoMember(3)]
		public int State;
	}
}
