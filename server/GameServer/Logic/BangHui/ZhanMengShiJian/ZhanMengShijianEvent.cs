using System;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	
	public class ZhanMengShijianEvent : ZhanMengShijianBaseEventObject
	{
		
		public ZhanMengShijianEvent(string roleName, int bhId, int shijianType, int param1, int param2, int param3, int serverId) : base(roleName, bhId, shijianType, param1, param2, param3, serverId)
		{
			this.roleName = roleName;
			this.bhId = bhId;
			this.shijianType = shijianType;
			this.param1 = param1;
			this.param2 = param2;
			this.param3 = param3;
		}

		
		public static ZhanMengShijianEvent createCreateZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.CreateZhanMeng, -1, -1, -1, serverId);
		}

		
		public static ZhanMengShijianEvent createJoinZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.JoinZhanMeng, -1, -1, -1, serverId);
		}

		
		public static ZhanMengShijianEvent createLeaveZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.LeaveZhanMeng, -1, -1, -1, serverId);
		}

		
		public static ZhanMengShijianEvent createZhanMengJuanZengEvent(string roleName, int bhId, int money, int moneyType, int bangGong, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ZhanMengJuanZeng, money, moneyType, bangGong, serverId);
		}

		
		public static ZhanMengShijianEvent createZhanMengGoodsJuanZengEvent(string roleName, int bhId, int nGoodID, int nGoodNum, int bangGong, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ZhanMengGooodsJuanZeng, nGoodID, nGoodNum, bangGong, serverId);
		}

		
		public static ZhanMengShijianEvent createChangeZhiWuEvent(string roleName, int bhId, int zhiwu, int otherRoleID, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ChangeZhiWu, zhiwu, -1, otherRoleID, serverId);
		}

		
		public static ZhanMengShijianEvent createKillBossEvent(string roleName, int bhId, int fubenid, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.KillBoss, fubenid, -1, -1, serverId);
		}
	}
}
