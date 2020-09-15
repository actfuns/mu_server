using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x0200017E RID: 382
	[ProtoContract]
	public class SC_SprUseGoods : IProtoBuffData
	{
		// Token: 0x060004A5 RID: 1189 RVA: 0x00040FBD File Offset: 0x0003F1BD
		public SC_SprUseGoods()
		{
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x00040FDD File Offset: 0x0003F1DD
		public SC_SprUseGoods(int error, int dbId, int cnt)
		{
			this.Error = error;
			this.DbId = dbId;
			this.Cnt = cnt;
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x00041014 File Offset: 0x0003F214
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
					this.Error = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.DbId = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.Cnt = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x000410B0 File Offset: 0x0003F2B0
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.Error, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.DbId, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.Cnt, true, 3, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.Error, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.DbId, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.Cnt, true, 0);
			return data;
		}

		// Token: 0x0400087E RID: 2174
		[ProtoMember(1)]
		public int Error = 0;

		// Token: 0x0400087F RID: 2175
		[ProtoMember(2)]
		public int DbId = 0;

		// Token: 0x04000880 RID: 2176
		[ProtoMember(3)]
		public int Cnt = 0;
	}
}
