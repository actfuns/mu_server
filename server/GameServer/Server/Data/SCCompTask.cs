using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCCompTask : IProtoBuffData
	{
		
		public SCCompTask()
		{
		}

		
		public SCCompTask(int roleID, int npcID, int taskID, int state)
		{
			this.roleID = roleID;
			this.npcID = npcID;
			this.taskID = taskID;
			this.state = state;
		}

		
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

		
		[ProtoMember(1)]
		public int roleID = 0;

		
		[ProtoMember(2)]
		public int npcID = 0;

		
		[ProtoMember(3)]
		public int taskID = 0;

		
		[ProtoMember(4)]
		public int state = 0;
	}
}
