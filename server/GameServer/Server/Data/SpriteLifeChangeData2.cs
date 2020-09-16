using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteLifeChangeData2 : IProtoBuffData
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
					this.roleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.lifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					goto IL_72;
				case 4:
					this.currentLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					goto IL_72;
				}
				continue;
				IL_72:
				throw new ArgumentException("error!!!");
			}
			return pos;
		}

		
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.lifeV, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.currentLifeV, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.lifeV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.currentLifeV, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int roleID;

		
		[ProtoMember(2)]
		public int lifeV;

		
		[ProtoMember(4)]
		public int currentLifeV;
	}
}
