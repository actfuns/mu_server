using System;
using GameServer.Core.GameEvent.EventOjectImpl;

namespace GameServer.Logic.BangHui.ZhanMengShiJian
{
	// Token: 0x020005C1 RID: 1473
	public class ZhanMengShijianEvent : ZhanMengShijianBaseEventObject
	{
		// Token: 0x06001AB7 RID: 6839 RVA: 0x00198500 File Offset: 0x00196700
		public ZhanMengShijianEvent(string roleName, int bhId, int shijianType, int param1, int param2, int param3, int serverId) : base(roleName, bhId, shijianType, param1, param2, param3, serverId)
		{
			this.roleName = roleName;
			this.bhId = bhId;
			this.shijianType = shijianType;
			this.param1 = param1;
			this.param2 = param2;
			this.param3 = param3;
		}

		// Token: 0x06001AB8 RID: 6840 RVA: 0x00198550 File Offset: 0x00196750
		public static ZhanMengShijianEvent createCreateZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.CreateZhanMeng, -1, -1, -1, serverId);
		}

		// Token: 0x06001AB9 RID: 6841 RVA: 0x00198574 File Offset: 0x00196774
		public static ZhanMengShijianEvent createJoinZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.JoinZhanMeng, -1, -1, -1, serverId);
		}

		// Token: 0x06001ABA RID: 6842 RVA: 0x00198598 File Offset: 0x00196798
		public static ZhanMengShijianEvent createLeaveZhanMengEvent(string roleName, int bhId, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.LeaveZhanMeng, -1, -1, -1, serverId);
		}

		// Token: 0x06001ABB RID: 6843 RVA: 0x001985BC File Offset: 0x001967BC
		public static ZhanMengShijianEvent createZhanMengJuanZengEvent(string roleName, int bhId, int money, int moneyType, int bangGong, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ZhanMengJuanZeng, money, moneyType, bangGong, serverId);
		}

		// Token: 0x06001ABC RID: 6844 RVA: 0x001985E0 File Offset: 0x001967E0
		public static ZhanMengShijianEvent createZhanMengGoodsJuanZengEvent(string roleName, int bhId, int nGoodID, int nGoodNum, int bangGong, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ZhanMengGooodsJuanZeng, nGoodID, nGoodNum, bangGong, serverId);
		}

		// Token: 0x06001ABD RID: 6845 RVA: 0x00198604 File Offset: 0x00196804
		public static ZhanMengShijianEvent createChangeZhiWuEvent(string roleName, int bhId, int zhiwu, int otherRoleID, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.ChangeZhiWu, zhiwu, -1, otherRoleID, serverId);
		}

		// Token: 0x06001ABE RID: 6846 RVA: 0x00198628 File Offset: 0x00196828
		public static ZhanMengShijianEvent createKillBossEvent(string roleName, int bhId, int fubenid, int serverId)
		{
			return new ZhanMengShijianEvent(roleName, bhId, ZhanMengShiJianConstants.KillBoss, fubenid, -1, -1, serverId);
		}
	}
}
