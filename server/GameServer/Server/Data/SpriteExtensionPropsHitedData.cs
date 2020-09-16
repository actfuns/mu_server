using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteExtensionPropsHitedData
	{
		
		[ProtoMember(1)]
		public int roleId;

		
		[ProtoMember(2)]
		public int enemy;

		
		[ProtoMember(3)]
		public int enemyX;

		
		[ProtoMember(4)]
		public int enemyY;

		
		[ProtoMember(5)]
		public int ExtensionPropID;
	}
}
