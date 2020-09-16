using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCMapChange : IProtoBuffData
	{
		
		public SCMapChange()
		{
		}

		
		public SCMapChange(int roleID, int teleportID, int newMapCode, int toNewMapX, int toNewMapY, int toNewDiection, int state)
		{
			this.RoleID = roleID;
			this.TeleportID = teleportID;
			this.NewMapCode = newMapCode;
			this.ToNewMapX = toNewMapX;
			this.ToNewMapY = toNewMapY;
			this.ToNewDiection = toNewDiection;
			this.State = state;
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
					this.RoleID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 2:
					this.TeleportID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.NewMapCode = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.ToNewMapX = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.ToNewMapY = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.ToNewDiection = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.State = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.RoleID, true, 1, true, 0);
			total += ProtoUtil.GetIntSize(this.TeleportID, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.NewMapCode, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewMapX, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewMapY, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.ToNewDiection, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.State, true, 7, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.TeleportID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.NewMapCode, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.ToNewMapX, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.ToNewMapY, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.ToNewDiection, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.State, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int TeleportID = 0;

		
		[ProtoMember(3)]
		public int NewMapCode = 0;

		
		[ProtoMember(4)]
		public int ToNewMapX = 0;

		
		[ProtoMember(5)]
		public int ToNewMapY = 0;

		
		[ProtoMember(6)]
		public int ToNewDiection = 0;

		
		[ProtoMember(7)]
		public int State = 0;
	}
}
