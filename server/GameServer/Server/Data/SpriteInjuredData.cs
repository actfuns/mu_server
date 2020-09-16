using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SpriteInjuredData : IProtoBuffData
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
					this.attackerRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.injuredRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.burst = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.injure = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.injuredRoleLife = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.attackerLevel = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.injuredRoleMaxLifeV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.injuredRoleMagic = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.injuredRoleMaxMagicV = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.hitToGridX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 11:
					this.hitToGridY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 12:
					this.MerlinInjuer = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 13:
					this.MerlinType = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 14:
					this.stopCaiJi = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 15:
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
			total += ProtoUtil.GetIntSize(this.attackerRoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.injuredRoleID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.burst, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.injure, true, 4, true, 0);
			total += ProtoUtil.GetLongSize(this.injuredRoleLife, true, 5, true, 0L);
			total += ProtoUtil.GetIntSize(this.attackerLevel, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.injuredRoleMaxLifeV, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.injuredRoleMagic, true, 8, true, 0);
			total += ProtoUtil.GetIntSize(this.injuredRoleMaxMagicV, true, 9, true, 0);
			total += ProtoUtil.GetIntSize(this.hitToGridX, true, 10, true, 0);
			total += ProtoUtil.GetIntSize(this.hitToGridY, true, 11, true, 0);
			total += ProtoUtil.GetIntSize(this.MerlinInjuer, true, 12, true, 0);
			total += ProtoUtil.GetIntSize(this.MerlinType, true, 13, true, 0);
			total += ProtoUtil.GetIntSize(this.stopCaiJi, true, 14, true, 0);
			total += ProtoUtil.GetIntSize(this.armorV_p1, true, 15, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.attackerRoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.injuredRoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.burst, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.injure, true, 0);
			ProtoUtil.LongMemberToBytes(data, 5, ref offset, this.injuredRoleLife, true, 0L);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.attackerLevel, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.injuredRoleMaxLifeV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.injuredRoleMagic, true, 0);
			ProtoUtil.IntMemberToBytes(data, 9, ref offset, this.injuredRoleMaxMagicV, true, 0);
			ProtoUtil.IntMemberToBytes(data, 10, ref offset, this.hitToGridX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 11, ref offset, this.hitToGridY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 12, ref offset, this.MerlinInjuer, true, 0);
			ProtoUtil.IntMemberToBytes(data, 13, ref offset, this.MerlinType, true, 0);
			ProtoUtil.IntMemberToBytes(data, 14, ref offset, this.stopCaiJi, true, 0);
			ProtoUtil.IntMemberToBytes(data, 15, ref offset, this.armorV_p1, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int attackerRoleID;

		
		[ProtoMember(2)]
		public int injuredRoleID;

		
		[ProtoMember(3)]
		public int burst;

		
		[ProtoMember(4)]
		public int injure;

		
		[ProtoMember(5)]
		public long injuredRoleLife;

		
		[ProtoMember(6)]
		public int attackerLevel;

		
		[ProtoMember(7)]
		public int injuredRoleMaxLifeV;

		
		[ProtoMember(8)]
		public int injuredRoleMagic;

		
		[ProtoMember(9)]
		public int injuredRoleMaxMagicV;

		
		[ProtoMember(10)]
		public int hitToGridX;

		
		[ProtoMember(11)]
		public int hitToGridY;

		
		[ProtoMember(12)]
		public int MerlinInjuer;

		
		[ProtoMember(13)]
		public int MerlinType;

		
		[ProtoMember(14)]
		public int stopCaiJi;

		
		[ProtoMember(15)]
		public int armorV_p1;
	}
}
