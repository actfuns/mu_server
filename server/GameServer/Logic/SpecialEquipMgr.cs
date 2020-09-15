using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x0200079A RID: 1946
	public class SpecialEquipMgr
	{
		// Token: 0x060032C4 RID: 12996 RVA: 0x002CFEAC File Offset: 0x002CE0AC
		public static void DoEquipExtAttack(GameClient client, int categoriy, int enemy)
		{
			if (-1 != enemy)
			{
				GoodsData goodData = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
				if (null != goodData)
				{
					List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(goodData.GoodsID);
					if (magicActionItemList != null && magicActionItemList.Count > 0)
					{
						MagicActionItem item = magicActionItemList[0];
						if (MagicActionIDs.EXT_ATTACK_MABI == item.MagicActionID)
						{
							double percent = item.MagicActionParams[0];
							double time = item.MagicActionParams[1];
							if ((double)Global.GetRandomNumber(0, 101) <= percent)
							{
								int nOcc = Global.CalcOriginalOccupationID(client);
								if (0 != nOcc)
								{
									percent *= 0.5;
								}
								if (-1 != enemy)
								{
									GSpriteTypes st = Global.GetSpriteType((uint)enemy);
									if (st != GSpriteTypes.Monster)
									{
										if (st == GSpriteTypes.Other)
										{
											GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
											if (null != enemyClient)
											{
												enemyClient.ClientData.DongJieStart = TimeUtil.NOW();
												enemyClient.ClientData.DongJieSeconds = (int)time;
												GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemyClient, 2, enemyClient.ClientData.DongJieStart, enemyClient.ClientData.DongJieSeconds, 0.0);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060032C5 RID: 12997 RVA: 0x002D0064 File Offset: 0x002CE264
		public static void DoEquipRestoreBlood(GameClient client, int categoriy)
		{
			if (client.ClientData.CurrentLifeV <= 0)
			{
				GoodsData goodData = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
				if (null != goodData)
				{
					List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(goodData.GoodsID);
					if (magicActionItemList != null && magicActionItemList.Count > 0)
					{
						MagicActionItem item = magicActionItemList[0];
						if (MagicActionIDs.EXT_RESTORE_BLOOD == item.MagicActionID)
						{
							double cooldown = item.MagicActionParams[0];
							if (cooldown * 1000.0 + (double)client.ClientData.SpecialEquipLastUseTicks < (double)TimeUtil.NOW())
							{
								client.ClientData.CurrentLifeV = client.ClientData.LifeV;
								client.ClientData.SpecialEquipLastUseTicks = TimeUtil.NOW();
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(531, new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
				}
			}
		}

		// Token: 0x060032C6 RID: 12998 RVA: 0x002D0198 File Offset: 0x002CE398
		public static int DoSubInJure(GameClient client, int categoriy, int injure)
		{
			GoodsData goodData = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, categoriy);
			int result;
			if (null == goodData)
			{
				result = 0;
			}
			else
			{
				List<MagicActionItem> magicActionItemList = UsingGoods.GetMagicActionListByGoodsID(goodData.GoodsID);
				if (magicActionItemList == null || magicActionItemList.Count <= 0)
				{
					result = 0;
				}
				else
				{
					MagicActionItem item = magicActionItemList[0];
					if (MagicActionIDs.EXT_SUB_INJURE == item.MagicActionID)
					{
						double percent = item.MagicActionParams[0];
						double magicToBloodRate = item.MagicActionParams[1];
						if (percent <= 0.0 || magicToBloodRate <= 0.0)
						{
							result = 0;
						}
						else
						{
							percent /= 100.0;
							int magicValue = client.ClientData.CurrentMagicV;
							int subInjure = (int)Math.Min((double)injure * percent, (double)magicValue / magicToBloodRate);
							int oldMagicV = client.ClientData.CurrentMagicV;
							client.ClientData.CurrentMagicV -= (int)(magicToBloodRate * (double)subInjure);
							result = Math.Min(subInjure, injure);
						}
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}
	}
}
