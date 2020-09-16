using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteLifeChangeData : IProtoBuffData
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
					this.magicV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.currentLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.currentMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.ArmorV = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.currentArmorV = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.lifeV, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.magicV, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.currentLifeV, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.currentMagicV, true, 5, true, 0);
			total += ProtoUtil.GetLongSize(this.ArmorV, true, 6, true, 0L);
			total += ProtoUtil.GetLongSize(this.currentArmorV, true, 7, true, 0L);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.lifeV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.magicV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.currentLifeV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.currentMagicV, true, 0);
			ProtoUtil.LongMemberToBytes(data, 6, ref offset, this.ArmorV, true, 0L);
			ProtoUtil.LongMemberToBytes(data, 7, ref offset, this.currentArmorV, true, 0L);
			return data;
		}

		
		[ProtoMember(1)]
		public int roleID;

		
		[ProtoMember(2)]
		public int lifeV;

		
		[ProtoMember(3)]
		public int magicV;

		
		[ProtoMember(4)]
		public int currentLifeV;

		
		[ProtoMember(5)]
		public int currentMagicV;

		
		[ProtoMember(6)]
		public long ArmorV;

		
		[ProtoMember(7)]
		public long currentArmorV;
	}
}
