using System;
using ProtoBuf;

namespace Server.Data
{
	
	[ProtoContract]
	public class PlayerJingJiRankingData : IComparable<PlayerJingJiRankingData>
	{
		
		public PlayerJingJiRankingData()
		{
		}

		
		public PlayerJingJiRankingData(PlayerJingJiData jingjiData)
		{
			this.jingjiData = jingjiData;
		}

		
		
		[ProtoMember(1)]
		public int roleId
		{
			get
			{
				return this.jingjiData.roleId;
			}
		}

		
		
		[ProtoMember(2)]
		public string roleName
		{
			get
			{
				return this.jingjiData.roleName;
			}
		}

		
		
		[ProtoMember(3)]
		public int combatForce
		{
			get
			{
				return this.jingjiData.combatForce;
			}
		}

		
		
		
		[ProtoMember(4)]
		public int ranking
		{
			get
			{
				return this.jingjiData.ranking;
			}
			set
			{
				this.jingjiData.ranking = value;
			}
		}

		
		public PaiHangItemData getPaiHangItemData()
		{
			this.paiHangItemData.RoleID = this.roleId;
			this.paiHangItemData.RoleName = this.roleName;
			this.paiHangItemData.Val1 = this.ranking;
			this.paiHangItemData.Val2 = this.combatForce;
			return this.paiHangItemData;
		}

		
		public int CompareTo(PlayerJingJiRankingData other)
		{
			return (this.ranking < other.ranking) ? -1 : ((this.ranking == other.ranking) ? 0 : 1);
		}

		
		public PlayerJingJiData jingjiData;

		
		private PaiHangItemData paiHangItemData = new PaiHangItemData();
	}
}
