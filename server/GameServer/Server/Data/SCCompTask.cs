using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	// Token: 0x02000181 RID: 385
	[ProtoContract]
	public class SCCompTask : IProtoBuffData
	{
		// Token: 0x060004B1 RID: 1201 RVA: 0x000414D8 File Offset: 0x0003F6D8
		public SCCompTask()
		{
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x00041500 File Offset: 0x0003F700
		public SCCompTask(int roleID, int npcID, int taskID, int state)
		{
			this.roleID = roleID;
			this.npcID = npcID;
			this.taskID = taskID;
			this.state = state;
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00041550 File Offset: 0x0003F750
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
					this.npcID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.taskID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.state = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				default:
					throw new ArgumentException("error!!!");
				}
			}
			return pos;
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00041608 File Offset: 0x0003F808
		public byte[] toBytes()
		{
			int total = 0;
			total += ProtoUtil.GetIntSize(this.roleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.npcID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.taskID, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.state, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.roleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.npcID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.taskID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.state, true, 0);
			return data;
		}

		// Token: 0x04000889 RID: 2185
		[ProtoMember(1)]
		public int roleID = 0;

		// Token: 0x0400088A RID: 2186
		[ProtoMember(2)]
		public int npcID = 0;

		// Token: 0x0400088B RID: 2187
		[ProtoMember(3)]
		public int taskID = 0;

		// Token: 0x0400088C RID: 2188
		[ProtoMember(4)]
		public int state = 0;
	}
}
