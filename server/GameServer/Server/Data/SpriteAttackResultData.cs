using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteAttackResultData : IProtoBuffData
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
					this.enemy = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.burst = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.injure = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.enemyLife = ProtoUtil.DoubleMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.newExperience = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.currentExperience = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.newLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.MerlinInjuer = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.MerlinType = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.armorV_p1 = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.enemy, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.burst, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.injure, true, 3, true, 0);
			total += ProtoUtil.GetDoubleSize(this.enemyLife, true, 4, true, 0.0);
			total += ProtoUtil.GetLongSize(this.newExperience, true, 5, true, 0L);
			total += ProtoUtil.GetLongSize(this.currentExperience, true, 6, true, 0L);
			total += ProtoUtil.GetIntSize(this.newLevel, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.MerlinInjuer, true, 8, true, 0);
			total += ProtoUtil.GetIntSize(this.MerlinType, true, 9, true, 0);
			total += ProtoUtil.GetIntSize(this.armorV_p1, true, 10, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.enemy, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.burst, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.injure, true, 0);
			ProtoUtil.DoubleMemberToBytes(data, 4, ref offset, this.enemyLife, true, 0.0);
			ProtoUtil.LongMemberToBytes(data, 5, ref offset, this.newExperience, true, 0L);
			ProtoUtil.LongMemberToBytes(data, 6, ref offset, this.currentExperience, true, 0L);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.newLevel, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.MerlinInjuer, true, 0);
			ProtoUtil.IntMemberToBytes(data, 9, ref offset, this.MerlinType, true, 0);
			ProtoUtil.IntMemberToBytes(data, 10, ref offset, this.armorV_p1, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int enemy = 0;

		
		[ProtoMember(2)]
		public int burst = 0;

		
		[ProtoMember(3)]
		public int injure = 0;

		
		[ProtoMember(4)]
		public double enemyLife = 0.0;

		
		[ProtoMember(5)]
		public long newExperience = 0L;

		
		[ProtoMember(6)]
		public long currentExperience = 0L;

		
		[ProtoMember(7)]
		public int newLevel = 0;

		
		[ProtoMember(8)]
		public int MerlinInjuer = 0;

		
		[ProtoMember(9)]
		public int MerlinType = 0;

		
		[ProtoMember(10)]
		public int armorV_p1;
	}
}
