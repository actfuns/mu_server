using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteManaChangeData : IProtoBuffData
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
				case 4:
					goto IL_76;
				case 3:
					this.magicV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.currentMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					goto IL_76;
				}
				continue;
				IL_76:
				throw new ArgumentException("error!!!");
			}
			return pos;
		}

		
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.magicV, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.currentMagicV, true, 5, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.magicV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.currentMagicV, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int roleID;

		
		[ProtoMember(3)]
		public int magicV;

		
		[ProtoMember(5)]
		public int currentMagicV;
	}
}
