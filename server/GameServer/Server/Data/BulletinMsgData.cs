using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class BulletinMsgData
	{
		
		[ProtoMember(1)]
		public string MsgID = "";

		
		[ProtoMember(2)]
		public int PlayMinutes;

		
		[ProtoMember(3)]
		public int ToPlayNum = 0;

		
		[ProtoMember(4)]
		public string BulletinText = "";

		
		[ProtoMember(5)]
		public long BulletinTicks = 0L;

		
		[ProtoMember(6)]
		public int playingNum = 0;

		
		[ProtoMember(7)]
		public int MsgType = 0;

		
		[ProtoMember(8)]
		public int Interval = 0;

		
		public long LastBulletinTicks = 0L;
	}
}
