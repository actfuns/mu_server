using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerRingRankingData : IComparable<PlayerRingRankingData>
	{
		
		public PlayerRingRankingData()
		{
		}

		
		public PlayerRingRankingData(RingRankingInfo ringData)
		{
			this.ringData = ringData;
		}

		
		
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.ringData.nRoleID;
			}
		}

		
		
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.ringData.strRoleName;
			}
		}

		
		
		[ProtoMember(3)]
		public int RingID
		{
			get
			{
				return this.ringData.nRingID;
			}
		}

		
		
		[ProtoMember(4)]
		public string RingAddTime
		{
			get
			{
				return this.ringData.strAddTime;
			}
		}

		
		
		[ProtoMember(5)]
		public int GoodWillLevel
		{
			get
			{
				return this.ringData.byGoodwilllevel;
			}
		}

		
		
		[ProtoMember(6)]
		public int GoodWillStar
		{
			get
			{
				return this.ringData.byGoodwillstar;
			}
		}

		
		public void UpdateData(RingRankingInfo data)
		{
			this.ringData = data;
		}

		
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.GoodWillLevel;
			this.paiHangItemData.Val2 = this.GoodWillStar;
			this.paiHangItemData.Val3 = this.RingID;
			return this.paiHangItemData;
		}

		
		public int CompareTo(PlayerRingRankingData other)
		{
			int result;
			if (this.GoodWillLevel == other.GoodWillLevel)
			{
				if (this.GoodWillStar == other.GoodWillStar)
				{
					int nRet = string.Compare(this.RingAddTime, other.RingAddTime);
					result = ((nRet < 0) ? -1 : ((nRet == 0) ? 0 : 1));
				}
				else
				{
					result = ((this.GoodWillStar < other.GoodWillStar) ? 1 : -1);
				}
			}
			else
			{
				result = ((this.GoodWillLevel < other.GoodWillLevel) ? 1 : -1);
			}
			return result;
		}

		
		public RingRankingInfo ringData;

		
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
