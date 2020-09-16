using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCAutoFight : IProtoBuffData
	{
		
		public SCAutoFight()
		{
		}

		
		public SCAutoFight(int state, int roleID, int fightType, int extTag1)
		{
			this.State = state;
			this.RoleID = roleID;
			this.FightType = fightType;
			this.Tag = extTag1;
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
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.FightType = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.Tag = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.State, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.RoleID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.FightType, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.Tag, true, 4, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.FightType, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.Tag, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int State = 0;

		
		[ProtoMember(2)]
		public int RoleID = 0;

		
		[ProtoMember(3)]
		public int FightType = 0;

		
		[ProtoMember(4)]
		public int Tag = 0;
	}
}
