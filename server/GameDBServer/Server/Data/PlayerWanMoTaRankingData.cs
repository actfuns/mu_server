using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerWanMoTaRankingData : IComparable<PlayerWanMoTaRankingData>
	{
		
		public PlayerWanMoTaRankingData()
		{
		}

		
		public PlayerWanMoTaRankingData(WanMotaInfo wanmotaData)
		{
			this.wanmotaData = wanmotaData;
		}

		
		
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.wanmotaData.nRoleID;
			}
		}

		
		
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.wanmotaData.strRoleName;
			}
		}

		
		
		[ProtoMember(3)]
		public long flushTime
		{
			get
			{
				return this.wanmotaData.lFlushTime;
			}
		}

		
		
		[ProtoMember(4)]
		public int passLayerCount
		{
			get
			{
				return this.wanmotaData.nPassLayerCount;
			}
		}

		
		public void UpdateData(WanMotaInfo data)
		{
			this.wanmotaData = data;
		}

		
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.passLayerCount;
			return this.paiHangItemData;
		}

		
		public int CompareTo(PlayerWanMoTaRankingData other)
		{
			int result;
			if (this.passLayerCount == other.passLayerCount)
			{
				result = ((this.flushTime < other.flushTime) ? -1 : ((this.flushTime == other.flushTime) ? 0 : 1));
			}
			else
			{
				result = ((this.passLayerCount < other.passLayerCount) ? 1 : -1);
			}
			return result;
		}

		
		public WanMotaInfo wanmotaData;

		
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
