using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class CSPropAddPoint : IProtoBuffData
	{
		
		public CSPropAddPoint()
		{
			this.RoleID = 0;
			this.Strength = 0;
			this.Intelligence = 0;
			this.Dexterity = 0;
			this.Constitution = 0;
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
					this.Strength = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.Intelligence = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.Dexterity = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.Constitution = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.Strength, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.Intelligence, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.Dexterity, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.Constitution, true, 5, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.RoleID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.Strength, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.Intelligence, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.Dexterity, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.Constitution, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int RoleID = 0;

		
		[ProtoMember(2)]
		public int Strength = 0;

		
		[ProtoMember(3)]
		public int Intelligence = 0;

		
		[ProtoMember(4)]
		public int Dexterity = 0;

		
		[ProtoMember(5)]
		public int Constitution = 0;
	}
}
