using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerWingRankingData : IComparable<PlayerWingRankingData>
	{
		
		public PlayerWingRankingData()
		{
		}

		
		public PlayerWingRankingData(WingRankingInfo wingData)
		{
			this.wingData = wingData;
		}

		
		
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.wingData.nRoleID;
			}
		}

		
		
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.wingData.strRoleName;
			}
		}

		
		
		[ProtoMember(3)]
		public int Occupation
		{
			get
			{
				return this.wingData.nOccupation;
			}
		}

		
		
		[ProtoMember(4)]
		public string WingAddTime
		{
			get
			{
				return this.wingData.strAddTime;
			}
		}

		
		
		[ProtoMember(5)]
		public int WingID
		{
			get
			{
				return this.wingData.nWingID;
			}
		}

		
		
		[ProtoMember(6)]
		public int WingStarNum
		{
			get
			{
				return this.wingData.nStarNum;
			}
		}

		
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.WingID;
			this.paiHangItemData.Val2 = this.WingStarNum;
			this.paiHangItemData.Val3 = this.Occupation;
			return this.paiHangItemData;
		}

		
		public void UpdateData(WingRankingInfo data)
		{
			this.wingData = data;
		}

		
		public int CompareTo(PlayerWingRankingData other)
		{
			int result;
			if (this.WingID == other.WingID)
			{
				if (this.WingStarNum == other.WingStarNum)
				{
					int nRet = string.Compare(this.WingAddTime, other.WingAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.WingStarNum < other.WingStarNum) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.WingID < other.WingID) ? 1 : -1);
			}
			return result;
		}

		
		public WingRankingInfo wingData;

		
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
