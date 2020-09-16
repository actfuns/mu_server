using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	internal class OneKeyFindFriendData
	{
		
		[ProtoMember(1)]
		public int m_nRoleID = 0;

		
		[ProtoMember(2)]
		public string m_nRoleName = "";

		
		[ProtoMember(3)]
		public int m_nLevel = 0;

		
		[ProtoMember(4)]
		public int m_nChangeLifeLev = 0;

		
		[ProtoMember(5)]
		public int m_nOccupation = 0;
	}
}
