using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200016F RID: 367
	[ProtoContract]
	public class NewGoodsPackData : IProtoBuffData
	{
		// Token: 0x06000489 RID: 1161 RVA: 0x000402C8 File Offset: 0x0003E4C8
		public int getBytesSize()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.ownerRoleID, true, 1, true, 0);
			total += ProtoUtil.GetStringSize(this.ownerRoleName, true, 2);
			total += ProtoUtil.GetIntSize(this.autoID, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.goodsPackID, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.mapCode, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.toX, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.toY, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.goodsID, true, 8, true, 0);
			total += ProtoUtil.GetIntSize(this.goodsNum, true, 9, true, 0);
			total += ProtoUtil.GetLongSize(this.productTicks, true, 10, true, 0L);
			total += ProtoUtil.GetLongSize(this.teamID, true, 11, true, 0L);
			total += ProtoUtil.GetStringSize(this.teamRoleIDs, true, 12);
			total += ProtoUtil.GetIntSize(this.lucky, true, 13, true, 0);
			total += ProtoUtil.GetIntSize(this.excellenceInfo, true, 14, true, 0);
			total += ProtoUtil.GetIntSize(this.appendPropLev, true, 15, true, 0);
			return total + ProtoUtil.GetIntSize(this.forge_Level, true, 16, true, 0);
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00040404 File Offset: 0x0003E604
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
					this.ownerRoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.ownerRoleName = ProtoUtil.StringMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.autoID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.goodsPackID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.mapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.toX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.toY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.goodsID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 9:
					this.goodsNum = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 10:
					this.productTicks = ProtoUtil.LongMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 11:
					this.teamID = (long)ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 12:
					this.teamRoleIDs = ProtoUtil.StringMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 13:
					this.lucky = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 14:
					this.excellenceInfo = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 15:
					this.appendPropLev = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 16:
					this.forge_Level = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x000405F0 File Offset: 0x0003E7F0
		public byte[] toBytes()
		{
			int total = this.getBytesSize();
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.ownerRoleID, true, 0);
			ProtoUtil.StringMemberToBytes(data, 2, ref offset, this.ownerRoleName);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.autoID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.goodsPackID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.mapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.toX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.toY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.goodsID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 9, ref offset, this.goodsNum, true, 0);
			ProtoUtil.LongMemberToBytes(data, 10, ref offset, this.productTicks, true, 0L);
			ProtoUtil.LongMemberToBytes(data, 11, ref offset, this.teamID, true, 0L);
			ProtoUtil.StringMemberToBytes(data, 12, ref offset, this.teamRoleIDs);
			ProtoUtil.IntMemberToBytes(data, 13, ref offset, this.lucky, true, 0);
			ProtoUtil.IntMemberToBytes(data, 14, ref offset, this.excellenceInfo, true, 0);
			ProtoUtil.IntMemberToBytes(data, 15, ref offset, this.appendPropLev, true, 0);
			ProtoUtil.IntMemberToBytes(data, 16, ref offset, this.forge_Level, true, 0);
			return data;
		}

		// Token: 0x04000828 RID: 2088
		[ProtoMember(1)]
		public int ownerRoleID;

		// Token: 0x04000829 RID: 2089
		[ProtoMember(2)]
		public string ownerRoleName = "";

		// Token: 0x0400082A RID: 2090
		[ProtoMember(3)]
		public int autoID;

		// Token: 0x0400082B RID: 2091
		[ProtoMember(4)]
		public int goodsPackID;

		// Token: 0x0400082C RID: 2092
		[ProtoMember(5)]
		public int mapCode;

		// Token: 0x0400082D RID: 2093
		[ProtoMember(6)]
		public int toX;

		// Token: 0x0400082E RID: 2094
		[ProtoMember(7)]
		public int toY;

		// Token: 0x0400082F RID: 2095
		[ProtoMember(8)]
		public int goodsID;

		// Token: 0x04000830 RID: 2096
		[ProtoMember(9)]
		public int goodsNum;

		// Token: 0x04000831 RID: 2097
		[ProtoMember(10)]
		public long productTicks;

		// Token: 0x04000832 RID: 2098
		[ProtoMember(11)]
		public long teamID;

		// Token: 0x04000833 RID: 2099
		[ProtoMember(12)]
		public string teamRoleIDs = "";

		// Token: 0x04000834 RID: 2100
		[ProtoMember(13)]
		public int lucky;

		// Token: 0x04000835 RID: 2101
		[ProtoMember(14)]
		public int excellenceInfo;

		// Token: 0x04000836 RID: 2102
		[ProtoMember(15)]
		public int appendPropLev;

		// Token: 0x04000837 RID: 2103
		[ProtoMember(16)]
		public int forge_Level;
	}
}
