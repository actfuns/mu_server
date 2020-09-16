using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class CS_ClickOn : IProtoBuffData
	{
		
		public int fromBytes(byte[] data, int offset, int count)
		{
			int pos = offset;
			int mycount = 0;
			while (mycount < count)
			{
				int fieldnumber = -1;
				int wt = -1;
				ProtoUtil.GetTag(data, ref pos, ref fieldnumber, ref wt, ref mycount);
				switch (fieldnumber)
				{
				case 1:
					this.RoleId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.MapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.NpcId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.ExtId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.RoleId, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.MapCode, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.NpcId, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.ExtId, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.MapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.NpcId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.ExtId, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int RoleId = 0;

		
		[ProtoMember(2)]
		public int MapCode = 0;

		
		[ProtoMember(3)]
		public int NpcId = 0;

		
		[ProtoMember(4)]
		public int ExtId = 0;
	}
}
