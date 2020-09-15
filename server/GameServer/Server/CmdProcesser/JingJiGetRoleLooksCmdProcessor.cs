using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	// Token: 0x020008A0 RID: 2208
	internal class JingJiGetRoleLooksCmdProcessor : ICmdProcessor
	{
		// Token: 0x06003D62 RID: 15714 RVA: 0x00345226 File Offset: 0x00343426
		private JingJiGetRoleLooksCmdProcessor()
		{
		}

		// Token: 0x06003D63 RID: 15715 RVA: 0x00345234 File Offset: 0x00343434
		public static JingJiGetRoleLooksCmdProcessor getInstance()
		{
			return JingJiGetRoleLooksCmdProcessor.instance;
		}

		// Token: 0x06003D64 RID: 15716 RVA: 0x0034524C File Offset: 0x0034344C
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int lookWho = Convert.ToInt32(cmdParams[1]);
			PlayerJingJiData jingjiData = Global.sendToDB<PlayerJingJiData, byte[]>(10140, DataHelper.ObjectToBytes<int>(lookWho), client.ServerId);
			if (jingjiData != null)
			{
				client.sendCmd<RoleData4Selector>(1340, new RoleData4Selector
				{
					RoleID = jingjiData.roleId,
					RoleName = jingjiData.roleName,
					RoleSex = jingjiData.sex,
					Occupation = jingjiData.occupationId,
					SubOccupation = jingjiData.SubOccupation,
					OccupationList = jingjiData.OccupationList,
					Level = jingjiData.level,
					MyWingData = jingjiData.wingData,
					GoodsDataList = JingJiChangManager.GetUsingGoodsList(jingjiData.equipDatas),
					CombatForce = jingjiData.combatForce,
					AdmiredCount = jingjiData.AdmiredCount,
					SettingBitFlags = jingjiData.settingFlags,
					JunTuanId = jingjiData.JunTuanId,
					JunTuanName = jingjiData.JunTuanName,
					JunTuanZhiWu = jingjiData.JunTuanZhiWu,
					LingDi = jingjiData.LingDi,
					CompType = jingjiData.CompType,
					CompZhiWu = jingjiData.CompZhiWu
				}, false);
			}
			return true;
		}

		// Token: 0x040047B0 RID: 18352
		private static JingJiGetRoleLooksCmdProcessor instance = new JingJiGetRoleLooksCmdProcessor();
	}
}
