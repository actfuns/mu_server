using System;
using ProtoBuf;
using Tmsk.Contract;

namespace Server.Data
{
	
	[ProtoContract]
	public class SCModGoods : IProtoBuffData
	{
		
		public SCModGoods()
		{
		}

		
		public SCModGoods(int state, int modType, int id, int isusing, int site, int gcount, int bagIndex, int newHint)
		{
			this.State = state;
			this.ModType = modType;
			this.ID = id;
			this.IsUsing = isusing;
			this.Site = site;
			this.Count = gcount;
			this.BagIndex = bagIndex;
			this.NewHint = newHint;
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
					this.ModType = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 3:
					this.ID = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 4:
					this.IsUsing = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 5:
					this.Site = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 6:
					this.Count = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 7:
					this.BagIndex = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
					break;
				case 8:
					this.NewHint = ProtoUtil.IntMemberFromBytes(data, wt, ref pos, ref mycount);
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
			total += ProtoUtil.GetIntSize(this.ModType, true, 2, true, 0);
			total += ProtoUtil.GetIntSize(this.ID, true, 3, true, 0);
			total += ProtoUtil.GetIntSize(this.IsUsing, true, 4, true, 0);
			total += ProtoUtil.GetIntSize(this.Site, true, 5, true, 0);
			total += ProtoUtil.GetIntSize(this.Count, true, 6, true, 0);
			total += ProtoUtil.GetIntSize(this.BagIndex, true, 7, true, 0);
			total += ProtoUtil.GetIntSize(this.NewHint, true, 8, true, 0);
			byte[] data = new byte[total];
			int offset = 0;
			ProtoUtil.IntMemberToBytes(data, 1, ref offset, this.State, true, 0);
			ProtoUtil.IntMemberToBytes(data, 2, ref offset, this.ModType, true, 0);
			ProtoUtil.IntMemberToBytes(data, 3, ref offset, this.ID, true, 0);
			ProtoUtil.IntMemberToBytes(data, 4, ref offset, this.IsUsing, true, 0);
			ProtoUtil.IntMemberToBytes(data, 5, ref offset, this.Site, true, 0);
			ProtoUtil.IntMemberToBytes(data, 6, ref offset, this.Count, true, 0);
			ProtoUtil.IntMemberToBytes(data, 7, ref offset, this.BagIndex, true, 0);
			ProtoUtil.IntMemberToBytes(data, 8, ref offset, this.NewHint, true, 0);
			return data;
		}

		
		[ProtoMember(1)]
		public int State = 0;

		
		[ProtoMember(2)]
		public int ModType = 0;

		
		[ProtoMember(3)]
		public int ID = 0;

		
		[ProtoMember(4)]
		public int IsUsing = 0;

		
		[ProtoMember(5)]
		public int Site = 0;

		
		[ProtoMember(6)]
		public int Count = 0;

		
		[ProtoMember(7)]
		public int BagIndex = 0;

		
		[ProtoMember(8)]
		public int NewHint = 0;
	}
}
