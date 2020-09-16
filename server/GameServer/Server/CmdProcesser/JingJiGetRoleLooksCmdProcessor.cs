using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	
	internal class JingJiGetRoleLooksCmdProcessor : ICmdProcessor
	{
		
		private JingJiGetRoleLooksCmdProcessor()
		{
		}

		
		public static JingJiGetRoleLooksCmdProcessor getInstance()
		{
			return JingJiGetRoleLooksCmdProcessor.instance;
		}

		
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

		
		private static JingJiGetRoleLooksCmdProcessor instance = new JingJiGetRoleLooksCmdProcessor();
	}
}
