using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerMerlinRankingData : IComparable<PlayerMerlinRankingData>
	{
		
		public PlayerMerlinRankingData()
		{
		}

		
		public PlayerMerlinRankingData(MerlinRankingInfo MerlinData)
		{
			this.MerlinData = MerlinData;
		}

		
		
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.MerlinData.nRoleID;
			}
		}

		
		
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.MerlinData.strRoleName;
			}
		}

		
		
		[ProtoMember(3)]
		public int Occupation
		{
			get
			{
				return this.MerlinData.nOccupation;
			}
		}

		
		
		[ProtoMember(4)]
		public string MerlinAddTime
		{
			get
			{
				return this.MerlinData.strAddTime;
			}
		}

		
		
		[ProtoMember(5)]
		public int Level
		{
			get
			{
				return this.MerlinData.nLevel;
			}
		}

		
		
		[ProtoMember(6)]
		public int StarNum
		{
			get
			{
				return this.MerlinData.nStarNum;
			}
		}

		
		public void UpdateData(MerlinRankingInfo merlinData)
		{
			this.MerlinData = merlinData;
		}

		
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.Level;
			this.paiHangItemData.Val2 = this.StarNum;
			this.paiHangItemData.Val3 = this.Occupation;
			return this.paiHangItemData;
		}

		
		public int CompareTo(PlayerMerlinRankingData other)
		{
			int result;
			if (this.Level == other.Level)
			{
				if (this.StarNum == other.StarNum)
				{
					int nRet = string.Compare(this.MerlinAddTime, other.MerlinAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.StarNum < other.StarNum) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.Level < other.Level) ? 1 : -1);
			}
			return result;
		}

		
		public MerlinRankingInfo MerlinData;

		
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
