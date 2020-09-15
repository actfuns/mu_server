using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Logic.ExtensionProps;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.UnionAlly;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007CF RID: 1999
	internal class SpriteAttack
	{
		// Token: 0x0600384A RID: 14410 RVA: 0x002FB630 File Offset: 0x002F9830
		private static int VerifyEnemyID(IObject attacker, int mapCode, int enemyID, int enemyX, int enemyY)
		{
			int ret = enemyID;
			int result;
			if (-1 == enemyID)
			{
				result = ret;
			}
			else
			{
				GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
				if (st == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (!Global.CompareTwoPointGridXY(monster.MonsterZoneNode.MapCode, new Point((double)enemyX, (double)enemyY), monster.SafeCoordinate))
						{
							ret = -1;
						}
					}
					else
					{
						ret = -1;
					}
				}
				else if (st == GSpriteTypes.NPC)
				{
					ret = -1;
				}
				else if (st == GSpriteTypes.Pet)
				{
					ret = -1;
				}
				else if (st == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						if (!Global.CompareTwoPointGridXY(biaoCheItem.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)biaoCheItem.PosX, (double)biaoCheItem.PosY)))
						{
							ret = -1;
						}
					}
					else
					{
						ret = -1;
					}
				}
				else if (st == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						if (!Global.CompareTwoPointGridXY(junQiItem.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)junQiItem.PosX, (double)junQiItem.PosY)))
						{
							ret = -1;
						}
					}
					else
					{
						ret = -1;
					}
				}
				else if (st == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						if (!Global.CompareTwoPointGridXY(fakeRoleItem.MyRoleDataMini.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)fakeRoleItem.MyRoleDataMini.PosX, (double)fakeRoleItem.MyRoleDataMini.PosY)))
						{
							ret = -1;
						}
					}
					else
					{
						ret = -1;
					}
				}
				else
				{
					GameClient client = GameManager.ClientMgr.FindClient(enemyID);
					if (null != client)
					{
						if (!Global.CompareTwoPointGridXY(client.ClientData.MapCode, new Point((double)enemyX, (double)enemyY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY)))
						{
							ret = -1;
						}
					}
					else
					{
						ret = -1;
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600384B RID: 14411 RVA: 0x002FB888 File Offset: 0x002F9A88
		private static bool IsOpposition(IObject me, int mapCode, int enemyID)
		{
			bool ret = true;
			bool result;
			if (-1 == enemyID)
			{
				result = ret;
			}
			else
			{
				GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
				if (st == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (me is GameClient)
						{
							ret = Global.IsOpposition(me as GameClient, monster);
						}
						else
						{
							ret = (me is Monster && Global.IsOpposition(me as Monster, monster));
						}
					}
				}
				else if (st == GSpriteTypes.NPC)
				{
					ret = false;
				}
				else if (st == GSpriteTypes.Pet)
				{
					ret = false;
				}
				else if (st == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						if (me is GameClient)
						{
							ret = Global.IsOpposition(me as GameClient, biaoCheItem);
						}
						else
						{
							ret = (me is Monster && Global.IsOpposition(me as Monster, biaoCheItem));
						}
					}
					else
					{
						ret = false;
					}
				}
				else if (st == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						if (me is GameClient)
						{
							ret = Global.IsOpposition(me as GameClient, junQiItem);
						}
						else
						{
							ret = (me is Monster && Global.IsOpposition(me as Monster, junQiItem));
						}
					}
					else
					{
						ret = false;
					}
				}
				else if (st == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						if (me is GameClient)
						{
							ret = Global.IsOpposition(me as GameClient, fakeRoleItem);
						}
						else
						{
							ret = (me is Monster && Global.IsOpposition(me as Monster, fakeRoleItem));
						}
					}
					else
					{
						ret = false;
					}
				}
				else
				{
					GameClient client = GameManager.ClientMgr.FindClient(enemyID);
					if (null != client)
					{
						if (me is GameClient)
						{
							ret = Global.IsOpposition(me as GameClient, client);
						}
						else
						{
							ret = (me is Monster && Global.IsOpposition(me as Monster, client));
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600384C RID: 14412 RVA: 0x002FBB34 File Offset: 0x002F9D34
		public static bool JugeMagicDistance(SystemXmlItem systemMagic, IObject attacker, int enemy, int enemyX, int enemyY, int magicCode, bool forceNotAttack = false)
		{
			int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
			Point clientEnemyPos = new Point((double)enemyX, (double)enemyY);
			if (systemMagic.GetIntValue("MagicType", -1) == 1)
			{
				int targetType = systemMagic.GetIntValue("TargetType", -1);
				if (1 == targetType)
				{
					return true;
				}
				if (2 == targetType || 3 == targetType || 4 == targetType)
				{
					Point toPos = new Point((double)enemyX, (double)enemyY);
					if (-1 != enemy)
					{
						SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out toPos);
						if (Global.GetTwoPointDistance(clientEnemyPos, toPos) < 300.0)
						{
							toPos = clientEnemyPos;
						}
					}
					if (systemMagic.GetIntValue("ActionType", -1) == 0 && !forceNotAttack)
					{
						if (Global.GetTwoPointDistance(attacker.CurrentPos, toPos) > (double)attackDistance)
						{
							return false;
						}
					}
					else if (Global.GetTwoPointDistance(attacker.CurrentPos, toPos) > (double)attackDistance)
					{
						return false;
					}
				}
			}
			else
			{
				if (1 == systemMagic.GetIntValue("TargetPos", -1))
				{
					return true;
				}
				Point targetPos;
				if (2 == systemMagic.GetIntValue("TargetPos", -1))
				{
					targetPos = new Point((double)enemyX, (double)enemyY);
					if (-1 != enemy)
					{
						if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out targetPos))
						{
							targetPos = new Point((double)enemyX, (double)enemyY);
						}
						else if (Global.GetTwoPointDistance(clientEnemyPos, targetPos) < 300.0)
						{
							targetPos = clientEnemyPos;
						}
					}
				}
				else
				{
					targetPos = new Point((double)enemyX, (double)enemyY);
				}
				if (Global.GetTwoPointDistance(attacker.CurrentPos, targetPos) > (double)attackDistance)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600384D RID: 14413 RVA: 0x002FBD78 File Offset: 0x002F9F78
		public static bool CanUseMaigc(GameClient client, int magicCode)
		{
			lock (client.ClientData.SkillIdHashSet)
			{
				if (client.ClientData.SkillIdHashSet.Contains(magicCode))
				{
					return true;
				}
				bool flag2;
				if (client.ClientData.SkillDataList != null)
				{
					flag2 = (null == client.ClientData.SkillDataList.Find((SkillData x) => x.SkillID == magicCode));
				}
				else
				{
					flag2 = true;
				}
				if (!flag2)
				{
					client.ClientData.SkillIdHashSet.Add(magicCode);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600384E RID: 14414 RVA: 0x002FBE58 File Offset: 0x002FA058
		private static bool CheckEnemyClientPostion(GameClient client, ref int enemyID, int realEnemyX, int realEnemyY)
		{
			bool result;
			if (-1 == enemyID)
			{
				result = true;
			}
			else if (-1 == realEnemyX || -1 == realEnemyY)
			{
				result = true;
			}
			else
			{
				GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
				if (st != GSpriteTypes.Other)
				{
					result = true;
				}
				else
				{
					bool leave = false;
					GameClient enemyClient = GameManager.ClientMgr.FindClient(enemyID);
					if (null == enemyClient)
					{
						leave = true;
					}
					else if (enemyClient.CurrentMapCode != client.CurrentMapCode)
					{
						leave = true;
					}
					else if (enemyClient.CurrentCopyMapID > 0 && enemyClient.CurrentCopyMapID != client.CurrentCopyMapID)
					{
						leave = true;
					}
					else if (Math.Abs(enemyClient.ClientData.PosX - realEnemyX) > 1600 || Math.Abs(enemyClient.ClientData.PosY - realEnemyY) > 1000)
					{
						leave = true;
					}
					if (leave)
					{
						enemyID = -1;
						client.sendCmd(127, string.Format("{0}:{1}", enemyID, 1), false);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600384F RID: 14415 RVA: 0x002FBF94 File Offset: 0x002FA194
		private static bool CheckMonsterPostion(GameClient client, int enemyID, int realEnemyX, int realEnemyY)
		{
			bool result;
			if (-1 == enemyID)
			{
				result = true;
			}
			else if (-1 == realEnemyX || -1 == realEnemyY)
			{
				result = true;
			}
			else
			{
				GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
				if (st != GSpriteTypes.Monster)
				{
					result = true;
				}
				else
				{
					Point reportPos = new Point((double)realEnemyX, (double)realEnemyY);
					Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemyID);
					if (monster == null || !monster.Alive || (monster.CopyMapID > 0 && monster.CopyMapID != client.ClientData.CopyMapID))
					{
						GameManager.ClientMgr.NotifyMyselfLeaveMonsterByID(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyID);
						result = true;
					}
					else
					{
						Point serverPos = new Point(monster.SafeCoordinate.X, monster.SafeCoordinate.Y);
						if (monster.VLife > 0.0 && monster.Alive)
						{
							if (Global.CompareTwoPointGridXY(monster.MonsterZoneNode.MapCode, reportPos, serverPos))
							{
								return true;
							}
						}
						if (!Global.JugePointAtClientGrids(client, monster, serverPos))
						{
							List<object> objsList = new List<object>();
							objsList.Add(monster);
							GameManager.ClientMgr.NotifyMyselfLeaveMonsters(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList);
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06003850 RID: 14416 RVA: 0x002FC128 File Offset: 0x002FA328
		public static bool CheckLastAttackTicks(GameClient client, bool recAttackTicks, int magicCode)
		{
			return recAttackTicks || true;
		}

		// Token: 0x06003851 RID: 14417 RVA: 0x002FC148 File Offset: 0x002FA348
		private static bool CanAutoUseZSSkill(GameClient client, int magicCode)
		{
			int nOcc = Global.CalcOriginalOccupationID(client);
			bool result;
			if (0 != nOcc)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemMagic = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
				{
					result = false;
				}
				else
				{
					int skillType = systemMagic.GetIntValue("SkillType", -1);
					if (1 != skillType && 10 != skillType)
					{
						result = false;
					}
					else
					{
						int magicType = systemMagic.GetIntValue("MagicType", -1);
						result = (1 == magicType || 2 == magicType);
					}
				}
			}
			return result;
		}

		// Token: 0x06003852 RID: 14418 RVA: 0x002FC1D4 File Offset: 0x002FA3D4
		private static bool CanRecordAttackTicks(GameClient client, int magicCode)
		{
			SystemXmlItem systemMagic = null;
			return !GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic) || 1 != systemMagic.GetIntValue("SkillType", -1);
		}

		// Token: 0x06003853 RID: 14419 RVA: 0x002FC218 File Offset: 0x002FA418
		public static bool AddManyAttackMagic(IObject obj, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			MagicsManyTimeDmageQueue queue = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			return null != queue && queue.AddManyTimeDmageQueueItemEx(enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode);
		}

		// Token: 0x06003854 RID: 14420 RVA: 0x002FC250 File Offset: 0x002FA450
		public static bool AddDelayMagic(IObject obj, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			MagicsManyTimeDmageQueue queue = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			return null != queue && queue.AddDelayMagicItemEx(enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode);
		}

		// Token: 0x06003855 RID: 14421 RVA: 0x002FC288 File Offset: 0x002FA488
		private static void ParseManyTimes(IObject obj, List<ManyTimeDmageItem> manyTimeDmageItemList, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode)
		{
			long ticks = TimeUtil.NOW();
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				for (int i = 0; i < manyTimeDmageItemList.Count; i++)
				{
					ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
					{
						ToExecTicks = ticks + manyTimeDmageItemList[i].InjuredSeconds,
						enemy = enemy,
						enemyX = enemyX,
						enemyY = enemyY,
						realEnemyX = realEnemyX,
						realEnemyY = realEnemyY,
						magicCode = magicCode,
						manyRangeIndex = i,
						manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
					};
					client.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (monster.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() <= 0)
				{
					for (int i = 0; i < manyTimeDmageItemList.Count; i++)
					{
						ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
						{
							ToExecTicks = ticks + manyTimeDmageItemList[i].InjuredSeconds,
							enemy = enemy,
							enemyX = enemyX,
							enemyY = enemyY,
							realEnemyX = realEnemyX,
							realEnemyY = realEnemyY,
							magicCode = magicCode,
							manyRangeIndex = i,
							manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
						};
						monster.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
					}
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				for (int i = 0; i < manyTimeDmageItemList.Count; i++)
				{
					ManyTimeDmageQueueItem manyTimeDmageQueueItem = new ManyTimeDmageQueueItem
					{
						ToExecTicks = ticks + manyTimeDmageItemList[i].InjuredSeconds,
						enemy = enemy,
						enemyX = enemyX,
						enemyY = enemyY,
						realEnemyX = realEnemyX,
						realEnemyY = realEnemyY,
						magicCode = magicCode,
						manyRangeIndex = i,
						manyRangeInjuredPercent = manyTimeDmageItemList[i].InjuredPercent
					};
					robot.MyMagicsManyTimeDmageQueue.AddManyTimeDmageQueueItem(manyTimeDmageQueueItem);
				}
			}
		}

		// Token: 0x06003856 RID: 14422 RVA: 0x002FC4F8 File Offset: 0x002FA6F8
		public static void ExecMagicsManyTimeDmageQueue(IObject obj)
		{
			if (obj is GameClient)
			{
				GameClient client = obj as GameClient;
				if (client.ClientData.CurrentLifeV > 0)
				{
					List<ManyTimeDmageQueueItem> itemsList = client.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < itemsList.Count; i++)
					{
						SpriteAttack.ProcessAttack(client, itemsList[i].enemy, itemsList[i].enemyX, itemsList[i].enemyY, itemsList[i].realEnemyX, itemsList[i].realEnemyY, itemsList[i].magicCode, itemsList[i].manyRangeIndex, itemsList[i].manyRangeInjuredPercent);
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (monster.VLife > 0.0)
				{
					List<ManyTimeDmageQueueItem> itemsList = monster.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < itemsList.Count; i++)
					{
						SpriteAttack.ProcessAttackByMonster(monster, itemsList[i].enemy, itemsList[i].enemyX, itemsList[i].enemyY, itemsList[i].realEnemyX, itemsList[i].realEnemyY, itemsList[i].magicCode, itemsList[i].manyRangeIndex, itemsList[i].manyRangeInjuredPercent);
					}
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (robot.VLife > 0.0)
				{
					List<ManyTimeDmageQueueItem> itemsList = robot.MyMagicsManyTimeDmageQueue.GetCanExecItems();
					for (int i = 0; i < itemsList.Count; i++)
					{
						SpriteAttack.ProcessMagicAttackByJingJiRobot(robot, itemsList[i].enemy, itemsList[i].magicCode, itemsList[i].manyRangeIndex, itemsList[i].manyRangeInjuredPercent);
					}
				}
			}
		}

		// Token: 0x06003857 RID: 14423 RVA: 0x002FC738 File Offset: 0x002FA938
		public static void ExecMagicsManyTimeDmageQueueEx(IObject obj)
		{
			MagicsManyTimeDmageQueue queue = obj.GetExtComponent<MagicsManyTimeDmageQueue>(ExtComponentTypes.ManyTimeDamageQueue);
			if (null != queue)
			{
				if (obj is GameClient)
				{
					GameClient client = obj as GameClient;
					if (client.ClientData.CurrentLifeV > 0)
					{
						ManyTimeDmageItem subItem;
						ManyTimeDmageMagicItem magicItem;
						while (null != (magicItem = queue.GetCanExecItemsEx(out subItem)))
						{
							try
							{
								SpriteAttack.ProcessAttack(client, magicItem.enemy, magicItem.enemyX, magicItem.enemyY, magicItem.realEnemyX, magicItem.realEnemyY, magicItem.magicCode, subItem.manyRangeIndex, subItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
				else if (obj is Monster)
				{
					Monster monster = obj as Monster;
					if (monster.VLife > 0.0)
					{
						ManyTimeDmageItem subItem;
						ManyTimeDmageMagicItem magicItem;
						while (null != (magicItem = queue.GetCanExecItemsEx(out subItem)))
						{
							try
							{
								SpriteAttack.ProcessAttackByMonster(monster, magicItem.enemy, magicItem.enemyX, magicItem.enemyY, magicItem.realEnemyX, magicItem.realEnemyY, magicItem.magicCode, subItem.manyRangeIndex, subItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
				else if (obj is Robot)
				{
					Robot robot = obj as Robot;
					if (robot.VLife > 0.0)
					{
						ManyTimeDmageItem subItem;
						ManyTimeDmageMagicItem magicItem;
						while (null != (magicItem = queue.GetCanExecItemsEx(out subItem)))
						{
							try
							{
								SpriteAttack.ProcessMagicAttackByJingJiRobot(robot, magicItem.enemy, magicItem.magicCode, subItem.manyRangeIndex, subItem.InjuredPercent);
							}
							catch (Exception ex)
							{
								LogManager.WriteExceptionUseCache(ex.ToString());
							}
						}
					}
				}
			}
		}

		// Token: 0x06003858 RID: 14424 RVA: 0x002FC96C File Offset: 0x002FAB6C
		public static void ProcessAttack(GameClient client, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == manyRangeIndex)
			{
				SystemXmlItem systemMagic = null;
				if (!GameManager.SystemMagicQuickMgr.MagicItemsDict.TryGetValue(magicCode, out systemMagic))
				{
					return;
				}
				if (!client.ClientData.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("拒绝技能释放#CD未结束2,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
					return;
				}
				client.ClientData.MyMagicCoolDownMgr.AddSkillCoolDown(client, magicCode);
				if (SpriteAttack.AddManyAttackMagic(client, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("记录技能释放#本次技能,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
					client.ClientData.LastSkillID = magicCode;
					return;
				}
			}
			client.passiveSkillModule.OnProcessMagic(client, enemy, enemyX, enemyY);
			if (manyRangeIndex <= 0)
			{
				client.UsingEquipMgr.AttackSomebody(client);
			}
			bool recAttackTicks = SpriteAttack.CanRecordAttackTicks(client, magicCode);
			if (manyRangeIndex > 0)
			{
				recAttackTicks = false;
			}
			SpriteAttack._ProcessAttack(client, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode, recAttackTicks, manyRangeIndex, manyRangeInjuredPercent);
		}

		// Token: 0x06003859 RID: 14425 RVA: 0x002FCAF4 File Offset: 0x002FACF4
		public static bool IsParentActionDone(GameClient client, int magicCode)
		{
			int parentMagicCode = 0;
			if (GameManager.SystemMagicActionMgr.MagicActionRelationDic.TryGetValue(magicCode, out parentMagicCode))
			{
				if (parentMagicCode != client.ClientData.LastSkillID)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600385A RID: 14426 RVA: 0x002FCB3C File Offset: 0x002FAD3C
		public static bool IsMagicEnough(GameClient client, int magicCode)
		{
			SkillData skillData = Global.GetSkillDataByID(client, magicCode);
			bool result;
			if (skillData == null)
			{
				result = false;
			}
			else
			{
				int subMagicV = Global.GetNeedMagicV(client, magicCode, skillData.SkillLevel);
				if (subMagicV > 0 && client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
				{
					int nMax = (int)RoleAlgorithm.GetMaxMagicV(client);
					subMagicV = (int)((double)nMax * ((double)subMagicV / 100.0));
					if (client.ClientData.CurrentMagicV - subMagicV < 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600385B RID: 14427 RVA: 0x002FCBE4 File Offset: 0x002FADE4
		public static int CheckMagicScripts(GameClient client, int magicCode)
		{
			int result;
			if (-1 == magicCode)
			{
				result = magicCode;
			}
			else
			{
				List<MagicActionItem> magicActionItemList = null;
				if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) || null == magicActionItemList)
				{
					if (!GameManager.SystemMagicActionMgr2.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) || null == magicActionItemList)
					{
						return -1;
					}
				}
				result = magicCode;
			}
			return result;
		}

		// Token: 0x0600385C RID: 14428 RVA: 0x002FCC58 File Offset: 0x002FAE58
		public static int CheckMagicScripts2(GameClient client, int magicCode)
		{
			int result;
			if (-1 == magicCode)
			{
				result = magicCode;
			}
			else
			{
				List<MagicActionItem> magicActionItemList = null;
				if (!GameManager.SystemMagicActionMgr2.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) || null == magicActionItemList)
				{
					result = -1;
				}
				else
				{
					result = magicCode;
				}
			}
			return result;
		}

		// Token: 0x0600385D RID: 14429 RVA: 0x002FCCA4 File Offset: 0x002FAEA4
		private static void _ProcessAttack(GameClient client, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, bool recAttackTicks, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (!SpriteAttack.CheckEnemyClientPostion(client, ref enemy, realEnemyX, realEnemyY))
			{
				if (client.ClientSocket.session.gmPriority > 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("拒绝技能释放#目标未找到,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
				}
			}
			else if (!SpriteAttack.CheckMonsterPostion(client, enemy, realEnemyX, realEnemyY))
			{
				if (client.ClientSocket.session.gmPriority > 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("拒绝技能释放#目标距离太远,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
				}
			}
			else
			{
				magicCode = SpriteAttack.CheckMagicScripts(client, magicCode);
				if (!SpriteAttack.CheckLastAttackTicks(client, recAttackTicks, magicCode))
				{
					if (client.ClientSocket.session.gmPriority > 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("拒绝技能释放#攻击速度超限,RoleID={0}({1}),MagicCode={2}", client.ClientData.RoleID, Global.FormatRoleName4(client), magicCode), null, true);
					}
				}
				else
				{
					client.CheckCheatData.LastMagicCode = magicCode;
					if (-1 == magicCode)
					{
						SpriteAttack.ProcessPhyAttack(client, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
					}
					else
					{
						SpriteAttack.ProcessMagicAttack(client, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
					}
				}
			}
		}

		// Token: 0x0600385E RID: 14430 RVA: 0x002FCE18 File Offset: 0x002FB018
		private static bool IsFriend(GameClient me, int mapCode, int enemyID)
		{
			bool ret = false;
			bool result;
			if (-1 == enemyID)
			{
				result = ret;
			}
			else if (me.ClientData.RoleID == enemyID)
			{
				result = true;
			}
			else
			{
				GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
				if (st == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
					if (null != monster)
					{
						if (null != monster.OwnerClient)
						{
							if (monster.OwnerClient.ClientData.RoleID == me.ClientData.RoleID)
							{
								return true;
							}
						}
					}
				}
				else if (st != GSpriteTypes.NPC)
				{
					if (st != GSpriteTypes.Pet)
					{
						if (st != GSpriteTypes.BiaoChe)
						{
							if (st != GSpriteTypes.JunQi)
							{
								GameClient client = GameManager.ClientMgr.FindClient(enemyID);
								if (null != client)
								{
									ret = SpriteAttack.IsFriend(me, client);
								}
							}
						}
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600385F RID: 14431 RVA: 0x002FCF3C File Offset: 0x002FB13C
		private static bool IsFriend(GameClient me, GameClient enemy)
		{
			bool ret = false;
			bool result;
			if (me.ClientData.PKMode == 0 || 1 == me.ClientData.PKMode)
			{
				result = true;
			}
			else if (Global.IsBattleMap(me))
			{
				result = (me.ClientData.BattleWhichSide == enemy.ClientData.BattleWhichSide);
			}
			else
			{
				if (2 == me.ClientData.PKMode)
				{
					if (me.ClientData.ServerPTID != enemy.ClientData.ServerPTID)
					{
						return false;
					}
					if (me.ClientData.Faction > 0 && enemy.ClientData.Faction > 0 && (me.ClientData.Faction == enemy.ClientData.Faction || AllyManager.getInstance().UnionIsAlly(me, enemy.ClientData.Faction)))
					{
						return true;
					}
				}
				else if (3 == me.ClientData.PKMode)
				{
					if (me.ClientData.TeamID > 0 && me.ClientData.TeamID == enemy.ClientData.TeamID)
					{
						ret = true;
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x06003860 RID: 14432 RVA: 0x002FD098 File Offset: 0x002FB298
		private static bool GetEnemyPos(int mapCode, int enemyID, out Point pos)
		{
			bool ret = false;
			pos = new Point(0.0, 0.0);
			GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
			if (st == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
				if (null != monster)
				{
					ret = true;
					pos = new Point(monster.SafeCoordinate.X, monster.SafeCoordinate.Y);
				}
			}
			else if (st != GSpriteTypes.Pet)
			{
				if (st == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						ret = true;
						pos = new Point((double)biaoCheItem.PosX, (double)biaoCheItem.PosY);
					}
				}
				else if (st == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						ret = true;
						pos = new Point((double)junQiItem.PosX, (double)junQiItem.PosY);
					}
				}
				else if (st == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						ret = true;
						pos = new Point((double)fakeRoleItem.MyRoleDataMini.PosX, (double)fakeRoleItem.MyRoleDataMini.PosY);
					}
				}
				else
				{
					GameClient client = GameManager.ClientMgr.FindClient(enemyID);
					if (null != client)
					{
						ret = true;
						pos = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
					}
				}
			}
			return ret;
		}

		// Token: 0x06003861 RID: 14433 RVA: 0x002FD264 File Offset: 0x002FB464
		private static object GetEnemyObject(int mapCode, int enemyID)
		{
			object obj = null;
			GSpriteTypes st = Global.GetSpriteType((uint)enemyID);
			if (st == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, enemyID);
				if (null != monster)
				{
					obj = monster;
				}
			}
			else if (st != GSpriteTypes.Pet)
			{
				if (st == GSpriteTypes.BiaoChe)
				{
					BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(enemyID);
					if (null != biaoCheItem)
					{
						obj = biaoCheItem;
					}
				}
				else if (st == GSpriteTypes.JunQi)
				{
					JunQiItem junQiItem = JunQiManager.FindJunQiByID(enemyID);
					if (null != junQiItem)
					{
						obj = junQiItem;
					}
				}
				else if (st == GSpriteTypes.FakeRole)
				{
					FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemyID);
					if (null != fakeRoleItem)
					{
						obj = fakeRoleItem;
					}
				}
				else
				{
					GameClient client = GameManager.ClientMgr.FindClient(enemyID);
					if (null != client)
					{
						obj = client;
					}
				}
			}
			return obj;
		}

		// Token: 0x06003862 RID: 14434 RVA: 0x002FD368 File Offset: 0x002FB568
		private static void ProcessPhyAttack(GameClient client, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			enemy = SpriteAttack.VerifyEnemyID(client, client.ClientData.MapCode, enemy, enemyX, enemyY);
			int attackDirection = client.ClientData.RoleDirection;
			if (-1 != enemyX && -1 != enemyY)
			{
				attackDirection = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, (double)client.ClientData.PosX, (double)client.ClientData.PosY);
			}
			List<int> enemiesList = new List<int>();
			GameManager.ClientMgr.LookupEnemiesInCircleByAngle(client, attackDirection, client.ClientData.MapCode, enemyX, enemyY, 200, enemiesList, 135.0, true);
			GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(attackDirection, client.ClientData.MapCode, client.ClientData.CopyMapID, enemyX, enemyY, 200, enemiesList, 125.0, true);
			BiaoCheManager.LookupAttackEnemyIDs(client, attackDirection, enemiesList);
			JunQiManager.LookupAttackEnemyIDs(client, attackDirection, enemiesList);
			FakeRoleManager.LookupAttackEnemyIDs(client, attackDirection, enemiesList);
			if (enemiesList.Count > 0)
			{
				if (enemiesList.IndexOf(enemy) < 0)
				{
					int index = Global.GetRandomNumber(0, enemiesList.Count);
					enemy = enemiesList[index];
				}
			}
			else
			{
				enemy = -1;
			}
			if (-1 != enemy)
			{
				if (!SpriteAttack.IsOpposition(client, client.ClientData.MapCode, enemy))
				{
					enemy = -1;
				}
			}
			int burst = 0;
			int injure = 0;
			if (enemy != -1)
			{
				List<int> actionType0_extensionPropsList = client.ClientData.ExtensionProps.GetIDs();
				if (null != actionType0_extensionPropsList)
				{
					actionType0_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType0_extensionPropsList, magicCode, 0);
				}
				List<int> actionType1_extensionPropsList = client.ClientData.ExtensionProps.GetIDs();
				if (null != actionType1_extensionPropsList)
				{
					actionType1_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType1_extensionPropsList, magicCode, 1);
				}
				GSpriteTypes st = Global.GetSpriteType((uint)enemy);
				if (st == GSpriteTypes.Monster)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemy);
					if (null != monster)
					{
						GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, monster);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, monster);
					}
				}
				else if (st == GSpriteTypes.BiaoChe)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					BiaoCheManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else if (st == GSpriteTypes.JunQi)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					JunQiManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else if (st == GSpriteTypes.FakeRole)
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					JunQiManager.NotifyInjured(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, client, client.ClientData.RoleID, enemy, enemyX, enemyY, burst, injure, 1.0, 0, 1.0, 0, 0);
				}
				else
				{
					GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemy, enemyX, enemyY, -1);
					GameClient obj = GameManager.ClientMgr.FindClient(enemy);
					if (null != obj)
					{
						GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, obj);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, obj);
					}
				}
			}
		}

		// Token: 0x06003863 RID: 14435 RVA: 0x002FD828 File Offset: 0x002FBA28
		private static void ProcessMagicAttack(GameClient client, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != magicCode)
			{
				SystemXmlItem systemMagic = null;
				if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
				{
					SkillData skillData = Global.GetSkillDataByID(client, magicCode);
					if (client.ClientData.IsFlashPlayer >= 1)
					{
						skillData = new SkillData
						{
							DbID = -1,
							SkillID = magicCode,
							SkillLevel = 1,
							UsedNum = 1
						};
					}
					if (null != skillData)
					{
						List<MagicActionItem> magicActionItemList = null;
						if (GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) && null != magicActionItemList)
						{
							List<MagicActionItem> magicScanTypeItemList = null;
							if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out magicScanTypeItemList) || null == magicScanTypeItemList)
							{
							}
							MagicActionItem magicScanTypeItem = null;
							if (magicScanTypeItemList != null && magicScanTypeItemList.Count > 0)
							{
								magicScanTypeItem = magicScanTypeItemList[0];
							}
							int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
							int maxNumHitted = systemMagic.GetIntValue("MaxNum", -1);
							MagicAction.MaxHitNum = maxNumHitted;
							if (!SpriteAttack.JugeMagicDistance(systemMagic, client, enemy, enemyX, enemyY, magicCode, false))
							{
								if (null == magicScanTypeItem)
								{
									return;
								}
								if (magicScanTypeItem != null && magicScanTypeItem.MagicActionID != MagicActionIDs.SCAN_SQUARE && magicScanTypeItem.MagicActionID != MagicActionIDs.FRONT_SECTOR && magicScanTypeItem.MagicActionID != MagicActionIDs.ROUNDSCAN)
								{
									return;
								}
							}
							int subMagicV = 0;
							List<int> actionType0_extensionPropsList = new List<int>();
							List<int> actionType1_extensionPropsList = new List<int>();
							if (manyRangeIndex <= 0)
							{
								subMagicV = Global.GetNeedMagicV(client, magicCode, skillData.SkillLevel);
								if (subMagicV > 0 && client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									int nMax = (int)RoleAlgorithm.GetMaxMagicV(client);
									subMagicV = (int)((double)nMax * ((double)subMagicV / 100.0));
									if (client.ClientData.CurrentMagicV - subMagicV < 0)
									{
										return;
									}
								}
								int addNum = 1;
								double dblSkilledDegress = DBRoleBufferManager.ProcessDblSkillUp(client);
								addNum = (int)((double)addNum * dblSkilledDegress);
								if (addNum > 0)
								{
									GameManager.ClientMgr.AddNumSkill(client, skillData, addNum, false);
								}
								GameManager.ClientMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (double)subMagicV);
								actionType0_extensionPropsList = client.ClientData.ExtensionProps.GetIDs();
								if (null != actionType0_extensionPropsList)
								{
									actionType0_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType0_extensionPropsList, skillData.SkillID, 0);
								}
								actionType1_extensionPropsList = client.ClientData.ExtensionProps.GetIDs();
								if (null != actionType1_extensionPropsList)
								{
									actionType1_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType1_extensionPropsList, skillData.SkillID, 1);
								}
							}
							int targetPlayingType = systemMagic.GetIntValue("TargetPlayingType", -1);
							int attackDirection = 0;
							if (systemMagic.GetIntValue("MagicType", -1) == 1 || systemMagic.GetIntValue("MagicType", -1) == 3)
							{
								int targetType = systemMagic.GetIntValue("TargetType", -1);
								if (1 == targetType)
								{
									if (systemMagic.GetIntValue("MagicType", -1) != 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
									}
									bool execResult = false;
									int i = 0;
									while (i < magicActionItemList.Count)
									{
										if (magicActionItemList[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
										{
											goto IL_3FF;
										}
										if (Global.GetTwoPointDistance(new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY), new Point((double)enemyX, (double)enemyY)) <= (double)attackDistance)
										{
											goto IL_3FF;
										}
										IL_457:
										i++;
										continue;
										IL_3FF:
										execResult |= MagicAction.ProcessAction(client, client, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, skillData.SkillLevel, skillData.SkillID, skillData.SkillID, client.ClientData.RoleDirection, -1, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										goto IL_457;
									}
									if (execResult)
									{
										if (systemMagic.GetIntValue("MagicType", -1) == 3)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
										}
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, client);
								}
								else if (-1 == targetType || 2 == targetType || 3 == targetType || 4 == targetType)
								{
									attackDirection = client.ClientData.RoleDirection;
									if (-1 != enemyX && -1 != enemyY)
									{
										attackDirection = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, (double)client.ClientData.PosX, (double)client.ClientData.PosY);
									}
									if (2 == targetType)
									{
										if (-1 == enemy)
										{
											enemy = client.ClientData.RoleID;
										}
										else if (client.ClientData.RoleID != enemy)
										{
											if (!SpriteAttack.IsFriend(client, client.ClientData.MapCode, enemy))
											{
												enemy = client.ClientData.RoleID;
											}
										}
									}
									else if (4 == targetType)
									{
										if (-1 == enemy)
										{
											enemy = client.ClientData.RoleID;
										}
										else if (client.ClientData.RoleID != enemy)
										{
										}
									}
									else if (-1 == targetType || 3 == targetType)
									{
										if (-1 == enemy || enemy == client.ClientData.RoleID)
										{
											if (-1 == enemy)
											{
												Point targetPos = new Point((double)enemyX, (double)enemyY);
												if (1 == systemMagic.GetIntValue("TargetPos", -1))
												{
													targetPos = new Point((double)enemyX, (double)enemyY);
												}
												else if (2 == systemMagic.GetIntValue("TargetPos", -1))
												{
													if (-1 != enemy)
													{
														if (!SpriteAttack.GetEnemyPos(client.ClientData.MapCode, enemy, out targetPos))
														{
															targetPos = new Point((double)enemyX, (double)enemyY);
														}
													}
													attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
												}
												else
												{
													attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
												}
												List<object> enemiesObjList = new List<object>();
												GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)targetPos.X, (int)targetPos.Y, 50, enemiesObjList, -1);
												GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)targetPos.X, (int)targetPos.Y, 50, enemiesObjList);
												if (enemiesObjList.Count > 0)
												{
													int index = Global.GetRandomNumber(0, enemiesObjList.Count);
													enemy = (enemiesObjList[index] as IObject).GetObjectID();
												}
											}
										}
										if (enemy > 0)
										{
											if (!SpriteAttack.IsOpposition(client, client.ClientData.MapCode, enemy))
											{
												enemy = -1;
											}
										}
									}
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
									}
									if (-1 != enemy)
									{
										GSpriteTypes st = Global.GetSpriteType((uint)enemy);
										if (st == GSpriteTypes.Monster)
										{
											Monster enemyMonster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, enemy);
											if (null != enemyMonster)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyMonster.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, enemyMonster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)enemyMonster.SafeCoordinate.X, (int)enemyMonster.SafeCoordinate.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, enemyMonster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, enemyMonster);
											}
										}
										else if (st == GSpriteTypes.BiaoChe)
										{
											BiaoCheItem enemyBiaoCheItem = BiaoCheManager.FindBiaoCheByID(enemy);
											if (null != enemyBiaoCheItem)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyBiaoCheItem.BiaoCheID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, enemyBiaoCheItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyBiaoCheItem.PosX, enemyBiaoCheItem.PosY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else if (st == GSpriteTypes.JunQi)
										{
											JunQiItem enemyJunQiItem = JunQiManager.FindJunQiByID(enemy);
											if (null != enemyJunQiItem)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyJunQiItem.JunQiID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, enemyJunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyJunQiItem.PosX, enemyJunQiItem.PosY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else if (st == GSpriteTypes.FakeRole)
										{
											FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemy);
											if (null != fakeRoleItem)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, fakeRoleItem.FakeRoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, fakeRoleItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, fakeRoleItem.MyRoleDataMini.PosX, fakeRoleItem.MyRoleDataMini.PosY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
											}
										}
										else
										{
											GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
											if (null != enemyClient)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, enemyClient.ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, enemyClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyClient.ClientData.PosX, enemyClient.ClientData.PosY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, enemyClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, enemyClient);
											}
										}
									}
								}
								else
								{
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < magicActionItemList.Count; i++)
									{
										MagicAction.ProcessAction(client, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, client.ClientData.RoleDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, client);
								}
							}
							else
							{
								int targetType = systemMagic.GetIntValue("TargetType", -1);
								attackDirection = client.ClientData.RoleDirection;
								Point targetPos;
								if (1 == systemMagic.GetIntValue("TargetPos", -1))
								{
									targetPos = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								else if (2 == systemMagic.GetIntValue("TargetPos", -1))
								{
									targetPos = new Point((double)enemyX, (double)enemyY);
									if (-1 != enemy)
									{
										if (!SpriteAttack.GetEnemyPos(client.ClientData.MapCode, enemy, out targetPos))
										{
											targetPos = new Point((double)enemyX, (double)enemyY);
										}
									}
									attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								else
								{
									if (magicScanTypeItem != null && (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR || magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN))
									{
										targetPos = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
									}
									else
									{
										targetPos = new Point((double)enemyX, (double)enemyY);
									}
									attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), (double)client.ClientData.PosX, (double)client.ClientData.PosY);
								}
								List<object> clientList = new List<object>();
								if (magicScanTypeItem != null)
								{
									if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
									{
										GameManager.ClientMgr.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], clientList);
									}
									else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
									{
										GameManager.ClientMgr.LookupEnemiesInCircleByAngle(client, client.ClientData.RoleDirection, client.ClientData.MapCode, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList, magicScanTypeItem.MagicActionParams[0], true);
									}
									else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
									{
										GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList, targetType);
									}
								}
								else
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList, -1);
								}
								if (1 != targetType)
								{
									if (2 == targetType || 4 == targetType)
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(client, client, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										if (targetPlayingType == 1)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, enemyX, enemyY, magicCode);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, client);
										int j = 0;
										while (j < clientList.Count)
										{
											if (2 != targetType)
											{
												goto IL_1224;
											}
											if (client.ClientData.TeamID > 0 && client.ClientData.TeamID == (clientList[j] as GameClient).ClientData.TeamID)
											{
												goto IL_1224;
											}
											IL_132F:
											j++;
											continue;
											IL_1224:
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(client, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (targetPlayingType == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, clientList[j] as GameClient);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, clientList[j] as GameClient);
											if (--maxNumHitted <= 0)
											{
												break;
											}
											goto IL_132F;
										}
									}
									else if (-1 == targetType || 3 == targetType)
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(client, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										List<object> monsterList = new List<object>();
										if (magicScanTypeItemList != null)
										{
											if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												GameManager.MonsterMgr.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], monsterList);
											}
											else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircleByRoleAngle(client.ClientData.RoleYAngle, client.ClientData.MapCode, client.ClientData.CopyMapID, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, magicScanTypeItem.MagicActionParams[0], true);
											}
											else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList);
											}
										}
										else
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(client.ClientData.MapCode, client.ClientData.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList);
										}
										List<object> enemyList = new List<object>();
										foreach (object tmp in monsterList)
										{
											if (Global.IsOpposition(client, tmp as Monster))
											{
												enemyList.Add(tmp);
											}
										}
										foreach (object tmp in clientList)
										{
											if ((tmp as GameClient).ClientData.RoleID != client.ClientData.RoleID)
											{
												if (Global.IsOpposition(client, tmp as GameClient))
												{
													enemyList.Add(tmp);
												}
											}
										}
										double shenShiInjurePercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(client, enemyList, magicCode);
										for (int j = 0; j < clientList.Count; j++)
										{
											if ((clientList[j] as GameClient).ClientData.RoleID != client.ClientData.RoleID)
											{
												if (Global.IsOpposition(client, clientList[j] as GameClient))
												{
													if (targetPlayingType == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
													}
													for (int i = 0; i < magicActionItemList.Count; i++)
													{
														MagicAction.ProcessAction(client, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, shenShiInjurePercent);
													}
													ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, clientList[j] as GameClient);
													ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, clientList[j] as GameClient);
													if (--maxNumHitted <= 0)
													{
														break;
													}
												}
											}
										}
										for (int j = 0; j < monsterList.Count; j++)
										{
											if (Global.IsOpposition(client, monsterList[j] as Monster))
											{
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (monsterList[j] as Monster).RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, monsterList[j] as Monster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, shenShiInjurePercent);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, monsterList[j] as Monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, client, monsterList[j] as Monster);
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
										List<object> biaoCheItemList = new List<object>();
										BiaoCheManager.LookupRangeAttackEnemies(client, (int)targetPos.X, (int)targetPos.Y, attackDirection, systemMagic.GetStringValue("AttackDistance"), biaoCheItemList);
										for (int j = 0; j < biaoCheItemList.Count; j++)
										{
											if (targetPlayingType == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (biaoCheItemList[j] as BiaoCheItem).BiaoCheID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(client, biaoCheItemList[j] as BiaoCheItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (--maxNumHitted <= 0)
											{
												break;
											}
										}
										List<object> junQiItemList = new List<object>();
										JunQiManager.LookupRangeAttackEnemies(client, (int)targetPos.X, (int)targetPos.Y, attackDirection, systemMagic.GetStringValue("AttackDistance"), junQiItemList);
										for (int j = 0; j < junQiItemList.Count; j++)
										{
											if (Global.IsOpposition(client, junQiItemList[j] as JunQiItem))
											{
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (junQiItemList[j] as JunQiItem).JunQiID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, junQiItemList[j] as JunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
										List<object> fakeRoleItemList = new List<object>();
										if (magicScanTypeItem != null)
										{
											if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												FakeRoleManager.LookupRolesInSquare(client, client.ClientData.MapCode, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], fakeRoleItemList);
											}
											else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												FakeRoleManager.LookupEnemiesInCircleByAngle(client, client.ClientData.RoleDirection, client.ClientData.MapCode, (int)client.CurrentPos.X, (int)client.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), fakeRoleItemList, magicScanTypeItem.MagicActionParams[0], true);
											}
											else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
											{
												FakeRoleManager.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), fakeRoleItemList);
											}
										}
										else
										{
											FakeRoleManager.LookupEnemiesInCircle(client, client.ClientData.MapCode, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), fakeRoleItemList);
										}
										for (int j = 0; j < fakeRoleItemList.Count; j++)
										{
											if (Global.IsOpposition(client, fakeRoleItemList[j] as FakeRoleItem))
											{
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (fakeRoleItemList[j] as FakeRoleItem).FakeRoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(client, fakeRoleItemList[j] as FakeRoleItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
									}
									else
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(client, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, skillData.SkillLevel, skillData.SkillID, 0, 0, client.ClientData.RoleDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, client, client);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003864 RID: 14436 RVA: 0x002FF720 File Offset: 0x002FD920
		public static void ProcessAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == manyRangeIndex && attacker.MagicFinish <= -1)
			{
				if (SpriteAttack.AddManyAttackMagic(attacker, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode))
				{
					attacker.MagicFinish = -2;
					return;
				}
			}
			bool recAttackTicks = false;
			SpriteAttack._ProcessAttackByMonster(attacker, enemy, enemyX, enemyY, realEnemyX, realEnemyY, magicCode, recAttackTicks, manyRangeIndex, manyRangeInjuredPercent);
		}

		// Token: 0x06003865 RID: 14437 RVA: 0x002FF788 File Offset: 0x002FD988
		public static void ProcessAttackByJingJiRobot(Robot attacker, IObject target, int magicCode, int manyRangeIndex = -1, double manyRangeInjuredPercent = 1.0)
		{
			if (-1 == magicCode)
			{
				SpriteAttack.ProcessPhyAttackByMonster(attacker, target.GetObjectID(), (int)target.CurrentPos.X, (int)target.CurrentPos.Y, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
			else
			{
				SpriteAttack.ProcessMagicAttackByJingJiRobot(attacker, target, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
		}

		// Token: 0x06003866 RID: 14438 RVA: 0x002FF7E4 File Offset: 0x002FD9E4
		private static void ProcessMagicAttackByJingJiRobot(Robot attacker, int enemy, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != enemy)
			{
				IObject obj = null;
				GSpriteTypes st = Global.GetSpriteType((uint)enemy);
				if (st == GSpriteTypes.Monster)
				{
					obj = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
				}
				else if (st == GSpriteTypes.Other)
				{
					obj = GameManager.ClientMgr.FindClient(enemy);
				}
				if (null != obj)
				{
					SpriteAttack.ProcessMagicAttackByJingJiRobot(attacker, obj, magicCode, manyRangeIndex, manyRangeInjuredPercent);
				}
			}
		}

		// Token: 0x06003867 RID: 14439 RVA: 0x002FF860 File Offset: 0x002FDA60
		private static void ProcessMagicAttackByJingJiRobot(Robot attacker, IObject target, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 != magicCode)
			{
				if (-1 == manyRangeIndex)
				{
					if (SpriteAttack.AddManyAttackMagic(attacker, target.GetObjectID(), (int)target.CurrentPos.X, (int)target.CurrentPos.Y, (int)target.CurrentPos.X, (int)target.CurrentPos.Y, magicCode))
					{
						attacker.MyMagicCoolDownMgr.AddSkillCoolDown(attacker, magicCode);
						return;
					}
				}
				SystemXmlItem systemMagic = null;
				if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
				{
					int enemy = target.GetObjectID();
					int enemyX = (int)target.CurrentPos.X;
					int enemyY = (int)target.CurrentPos.Y;
					int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
					int maxNumHitted = systemMagic.GetIntValue("MaxNum", -1);
					if (SpriteAttack.JugeMagicDistance(systemMagic, attacker, enemy, enemyX, enemyY, magicCode, false))
					{
						List<MagicActionItem> magicActionItemList = null;
						if (GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) && null != magicActionItemList)
						{
							int subMagicV = 0;
							List<int> actionType0_extensionPropsList = new List<int>();
							List<int> actionType1_extensionPropsList = new List<int>();
							if (manyRangeIndex <= 0)
							{
								subMagicV = Global.GetNeedMagicV(attacker, magicCode, 1);
								if (subMagicV > 0)
								{
									int nMax = (int)attacker.MonsterInfo.VManaMax;
									int nNeed = nMax * (subMagicV / 100);
									if (attacker.VMana - (double)nNeed <= 0.0)
									{
										return;
									}
								}
								if (!attacker.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
								{
									return;
								}
								attacker.MyMagicCoolDownMgr.AddSkillCoolDown(attacker, magicCode);
								GameManager.MonsterMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (double)subMagicV);
								actionType0_extensionPropsList = attacker.ExtensionProps.GetIDs();
								if (null != actionType0_extensionPropsList)
								{
									actionType0_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType0_extensionPropsList, magicCode, 0);
								}
								actionType1_extensionPropsList = attacker.ExtensionProps.GetIDs();
								if (null != actionType1_extensionPropsList)
								{
									actionType1_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType1_extensionPropsList, magicCode, 1);
								}
							}
							int targetPlayingType = systemMagic.GetIntValue("TargetPlayingType", -1);
							if (systemMagic.GetIntValue("MagicType", -1) == 1 || systemMagic.GetIntValue("MagicType", -1) == 3)
							{
								int targetType = systemMagic.GetIntValue("TargetType", -1);
								if (1 == targetType)
								{
									if (systemMagic.GetIntValue("MagicType", -1) != 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
									}
									bool execResult = false;
									int i = 0;
									while (i < magicActionItemList.Count)
									{
										if (magicActionItemList[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
										{
											goto IL_337;
										}
										if (Global.GetTwoPointDistance(attacker.CurrentPos, new Point((double)enemyX, (double)enemyY)) <= (double)attackDistance)
										{
											goto IL_337;
										}
										IL_389:
										i++;
										continue;
										IL_337:
										execResult |= MagicAction.ProcessAction(attacker, attacker, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										goto IL_389;
									}
									if (execResult)
									{
										if (systemMagic.GetIntValue("MagicType", -1) == 3)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
										}
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
								}
								else if (-1 == targetType || 2 == targetType || 3 == targetType)
								{
									int attackDirection = (int)attacker.CurrentDir;
									if (-1 != enemyX && -1 != enemyY)
									{
										attackDirection = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, attacker.CurrentPos.X, attacker.CurrentPos.Y);
									}
									if (2 == targetType)
									{
										if (-1 == enemy)
										{
											enemy = attacker.GetObjectID();
										}
										else if (attacker.GetObjectID() != enemy)
										{
											enemy = -1;
										}
									}
									else if (-1 == targetType || 3 == targetType)
									{
										if (-1 == enemy || enemy == attacker.GetObjectID())
										{
											if (-1 == enemy)
											{
												Point targetPos = new Point((double)enemyX, (double)enemyY);
												if (1 == systemMagic.GetIntValue("TargetPos", -1))
												{
													targetPos = new Point((double)enemyX, (double)enemyY);
												}
												else if (2 == systemMagic.GetIntValue("TargetPos", -1))
												{
													if (-1 != enemy)
													{
														if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out targetPos))
														{
															targetPos = new Point((double)enemyX, (double)enemyY);
														}
													}
													attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
												}
												else
												{
													attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
												}
												List<object> enemiesObjList = new List<object>();
												List<MagicActionItem> magicScanTypeItemList = null;
												if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out magicScanTypeItemList) || null == magicScanTypeItemList)
												{
												}
												MagicActionItem magicScanTypeItem = null;
												if (magicScanTypeItemList != null && magicScanTypeItemList.Count > 0)
												{
													magicScanTypeItem = magicScanTypeItemList[0];
												}
												if (magicScanTypeItem != null)
												{
													if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
													{
														GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], enemiesObjList);
													}
													else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
													{
														GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), enemiesObjList, magicScanTypeItem.MagicActionParams[0], true);
													}
												}
												else
												{
													GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), enemiesObjList);
												}
												if (enemiesObjList.Count > 0)
												{
													int index = Global.GetRandomNumber(0, enemiesObjList.Count);
													enemy = (enemiesObjList[index] as IObject).GetObjectID();
												}
											}
										}
										else if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, enemy))
										{
											enemy = -1;
										}
									}
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									if (-1 != enemy)
									{
										GSpriteTypes st = Global.GetSpriteType((uint)enemy);
										if (st == GSpriteTypes.Monster)
										{
											Monster enemyMonster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
											if (null != enemyMonster)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyMonster.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, enemyMonster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)enemyMonster.SafeCoordinate.X, (int)enemyMonster.SafeCoordinate.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyMonster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyMonster);
											}
										}
										else if (st == GSpriteTypes.BiaoChe)
										{
											BiaoCheItem enemyBiaoCheItem = BiaoCheManager.FindBiaoCheByID(enemy);
											if (null != enemyBiaoCheItem)
											{
											}
										}
										else if (st == GSpriteTypes.JunQi)
										{
											JunQiItem enemyJunQiItem = JunQiManager.FindJunQiByID(enemy);
											if (null != enemyJunQiItem)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyJunQiItem.GetObjectID(), enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, enemyJunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)enemyJunQiItem.CurrentPos.X, (int)enemyJunQiItem.CurrentPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyJunQiItem);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyJunQiItem);
											}
										}
										else if (st == GSpriteTypes.FakeRole)
										{
											FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemy);
											if (null != fakeRoleItem)
											{
											}
										}
										else
										{
											GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
											if (null != enemyClient)
											{
												if (1 == targetPlayingType)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyClient.ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, enemyClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyClient.ClientData.PosX, enemyClient.ClientData.PosY, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyClient);
											}
										}
									}
								}
								else
								{
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < magicActionItemList.Count; i++)
									{
										MagicAction.ProcessAction(attacker, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
								}
							}
							else
							{
								int attackDirection = (int)attacker.CurrentDir;
								Point targetPos;
								if (1 == systemMagic.GetIntValue("TargetPos", -1))
								{
									targetPos = attacker.CurrentPos;
								}
								else if (2 == systemMagic.GetIntValue("TargetPos", -1))
								{
									targetPos = new Point((double)enemyX, (double)enemyY);
									if (-1 != enemy)
									{
										if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out targetPos))
										{
											targetPos = new Point((double)enemyX, (double)enemyY);
										}
									}
									attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								else
								{
									targetPos = new Point((double)enemyX, (double)enemyY);
									attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								List<object> clientList = new List<object>();
								List<MagicActionItem> magicScanTypeItemList = null;
								if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out magicScanTypeItemList) || null == magicScanTypeItemList)
								{
								}
								MagicActionItem magicScanTypeItem = null;
								if (magicScanTypeItemList != null && magicScanTypeItemList.Count > 0)
								{
									magicScanTypeItem = magicScanTypeItemList[0];
								}
								if (magicScanTypeItem != null)
								{
									if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
									{
										GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], clientList);
									}
									else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
									{
										GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList, magicScanTypeItem.MagicActionParams[0], true);
									}
								}
								else
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList);
								}
								int targetType = systemMagic.GetIntValue("TargetType", -1);
								if (1 != targetType)
								{
									if (2 == targetType)
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(attacker, attacker, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
										for (int j = 0; j < clientList.Count; j++)
										{
											if ((clientList[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
											{
												if (!Global.IsOpposition(attacker, clientList[j] as GameClient))
												{
													for (int i = 0; i < magicActionItemList.Count; i++)
													{
														MagicAction.ProcessAction(attacker, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
													}
													if (targetPlayingType == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
													}
													ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, clientList[j] as GameClient);
													ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, clientList[j] as GameClient);
													if (--maxNumHitted <= 0)
													{
														break;
													}
												}
											}
										}
									}
									else if (-1 == targetType || 3 == targetType)
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
										}
										for (int j = 0; j < clientList.Count; j++)
										{
											if ((clientList[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
											{
												if (Global.IsOpposition(attacker, clientList[j] as GameClient))
												{
													for (int i = 0; i < magicActionItemList.Count; i++)
													{
														MagicAction.ProcessAction(attacker, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
													}
													if (targetPlayingType == 1)
													{
														GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
													}
													if (--maxNumHitted <= 0)
													{
														break;
													}
												}
											}
										}
										List<object> monsterList = new List<object>();
										if (magicScanTypeItem != null)
										{
											if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
											{
												GameManager.MonsterMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], monsterList, 1);
											}
											else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
											{
												GameManager.MonsterMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, magicScanTypeItem.MagicActionParams[0], true, 1);
											}
										}
										else
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, 1);
										}
										for (int j = 0; j < monsterList.Count; j++)
										{
											if (Global.IsOpposition(attacker, monsterList[j] as Monster))
											{
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, monsterList[j] as Monster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (monsterList[j] as Monster).RoleID, enemyX, enemyY, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, monsterList[j] as Monster);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, monsterList[j] as Monster);
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
										List<object> biaoCheItemList = new List<object>();
										BiaoCheManager.LookupRangeAttackEnemies(attacker, (int)targetPos.X, (int)targetPos.Y, attackDirection, systemMagic.GetStringValue("AttackDistance"), biaoCheItemList);
										for (int j = 0; j < biaoCheItemList.Count; j++)
										{
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, biaoCheItemList[j] as BiaoCheItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (targetPlayingType == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (biaoCheItemList[j] as BiaoCheItem).BiaoCheID, enemyX, enemyY, magicCode);
											}
											if (--maxNumHitted <= 0)
											{
												break;
											}
										}
										List<object> junQiItemList = new List<object>();
										JunQiManager.LookupRangeAttackEnemies(attacker, (int)targetPos.X, (int)targetPos.Y, attackDirection, systemMagic.GetStringValue("AttackDistance"), junQiItemList);
										for (int j = 0; j < junQiItemList.Count; j++)
										{
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, junQiItemList[j] as JunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											if (targetPlayingType == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (junQiItemList[j] as JunQiItem).JunQiID, enemyX, enemyY, magicCode);
											}
											if (--maxNumHitted <= 0)
											{
												break;
											}
										}
									}
									else
									{
										if (targetPlayingType <= 0)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
										}
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(attacker, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.skillInfos[magicCode], magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003868 RID: 14440 RVA: 0x0030113C File Offset: 0x002FF33C
		private static void _ProcessAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int realEnemyX, int realEnemyY, int magicCode, bool recAttackTicks, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			if (-1 == magicCode)
			{
				SpriteAttack.ProcessPhyAttackByMonster(attacker, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
			}
			else
			{
				bool bFindTarget = true;
				try
				{
					bFindTarget = SpriteAttack.ProcessMagicAttackByMonster(attacker, enemy, enemyX, enemyY, magicCode, manyRangeIndex, manyRangeInjuredPercent);
				}
				finally
				{
					SpriteAttack.ProcessManyAttackMagicFinish(bFindTarget, attacker);
				}
			}
		}

		// Token: 0x06003869 RID: 14441 RVA: 0x003011A0 File Offset: 0x002FF3A0
		private static void ProcessPhyAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			enemy = SpriteAttack.VerifyEnemyID(attacker, attacker.MonsterZoneNode.MapCode, enemy, enemyX, enemyY);
			if (-1 == enemy)
			{
				int attackDirection = (int)attacker.Direction;
				if (-1 != enemyX && -1 != enemyY)
				{
					attackDirection = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, attacker.SafeCoordinate.X, attacker.SafeCoordinate.Y);
				}
				List<int> enemiesList = new List<int>();
				GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY, 200, enemiesList, 135.0, true);
				GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(attackDirection, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY, 200, enemiesList, 125.0, true);
				if (enemiesList.Count > 0)
				{
					int index = Global.GetRandomNumber(0, enemiesList.Count);
					enemy = enemiesList[index];
				}
			}
			if (-1 != enemy)
			{
				if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, enemy))
				{
					enemy = -1;
				}
			}
			if (enemy != -1)
			{
				List<int> actionType0_extensionPropsList = attacker.ExtensionProps.GetIDs();
				if (null != actionType0_extensionPropsList)
				{
					actionType0_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType0_extensionPropsList, magicCode, 0);
				}
				List<int> actionType1_extensionPropsList = attacker.ExtensionProps.GetIDs();
				if (null != actionType1_extensionPropsList)
				{
					actionType1_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType1_extensionPropsList, magicCode, 1);
				}
				GSpriteTypes st = Global.GetSpriteType((uint)enemy);
				if (st == GSpriteTypes.Monster)
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
					if (null != monster)
					{
						GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, monster, 0, 0, manyRangeInjuredPercent, 0, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, monster);
						ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, monster);
					}
				}
				else if (st != GSpriteTypes.BiaoChe)
				{
					if (st == GSpriteTypes.JunQi)
					{
						JunQiItem junqi = JunQiManager.FindJunQiByID(enemy);
						if (junqi != null && null != attacker.OwnerClient)
						{
							JunQiManager.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, junqi, 0, 0, manyRangeInjuredPercent, attacker.MonsterInfo.AttackType, false, 0, 1.0, 0, 0, 1.0, 0, 0);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, junqi);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, junqi);
						}
					}
					else
					{
						GameClient obj = GameManager.ClientMgr.FindClient(enemy);
						if (null != obj)
						{
							GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, obj, 0, 0, manyRangeInjuredPercent, attacker.MonsterInfo.AttackType, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, obj);
							ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, obj);
						}
					}
				}
			}
		}

		// Token: 0x0600386A RID: 14442 RVA: 0x00301514 File Offset: 0x002FF714
		private static bool ProcessMagicAttackByMonster(Monster attacker, int enemy, int enemyX, int enemyY, int magicCode, int manyRangeIndex, double manyRangeInjuredPercent)
		{
			bool result;
			if (-1 == magicCode)
			{
				result = false;
			}
			else
			{
				SystemXmlItem systemMagic = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
				{
					result = false;
				}
				else
				{
					List<MagicActionItem> magicScanTypeItemList = null;
					if (!GameManager.SystemMagicScanTypeMgr.MagicActionsDict.TryGetValue(magicCode, out magicScanTypeItemList) || null == magicScanTypeItemList)
					{
					}
					MagicActionItem magicScanTypeItem = null;
					if (magicScanTypeItemList != null && magicScanTypeItemList.Count > 0)
					{
						magicScanTypeItem = magicScanTypeItemList[0];
					}
					int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
					int maxNumHitted = systemMagic.GetIntValue("MaxNum", -1);
					List<MagicActionItem> magicActionItemList = null;
					if (!GameManager.SystemMagicActionMgr.MagicActionsDict.TryGetValue(magicCode, out magicActionItemList) || null == magicActionItemList)
					{
						result = false;
					}
					else
					{
						int subMagicV = 0;
						List<int> actionType0_extensionPropsList = new List<int>();
						List<int> actionType1_extensionPropsList = new List<int>();
						if (manyRangeIndex <= 0)
						{
							subMagicV = Global.GetNeedMagicV(attacker, magicCode, 1);
							if (subMagicV > 0)
							{
								int nMax = (int)attacker.MonsterInfo.VManaMax;
								int nNeed = nMax * (subMagicV / 100);
								if (attacker.VMana - (double)nNeed <= 0.0)
								{
									return false;
								}
							}
							GameManager.MonsterMgr.SubSpriteMagicV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (double)subMagicV);
							actionType0_extensionPropsList = attacker.ExtensionProps.GetIDs();
							if (null != actionType0_extensionPropsList)
							{
								actionType0_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType0_extensionPropsList, magicCode, 0);
							}
							actionType1_extensionPropsList = attacker.ExtensionProps.GetIDs();
							if (null != actionType1_extensionPropsList)
							{
								actionType1_extensionPropsList = ExtensionPropsMgr.ProcessExtensionProps(actionType1_extensionPropsList, magicCode, 1);
							}
						}
						int targetPlayingType = systemMagic.GetIntValue("TargetPlayingType", -1);
						int attackDirection = 0;
						bool bFindTarget = false;
						if (systemMagic.GetIntValue("MagicType", -1) == 1 || systemMagic.GetIntValue("MagicType", -1) == 3)
						{
							bFindTarget = true;
							int targetType = systemMagic.GetIntValue("TargetType", -1);
							if (1 == targetType)
							{
								if (systemMagic.GetIntValue("MagicType", -1) != 3)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
								}
								bool execResult = false;
								int i = 0;
								while (i < magicActionItemList.Count)
								{
									if (magicActionItemList[i].MagicActionID != MagicActionIDs.INSTANT_MOVE)
									{
										goto IL_2B3;
									}
									if (Global.GetTwoPointDistance(attacker.CurrentPos, new Point((double)enemyX, (double)enemyY)) <= (double)attackDistance)
									{
										goto IL_2B3;
									}
									IL_2FF:
									i++;
									continue;
									IL_2B3:
									execResult |= MagicAction.ProcessAction(attacker, attacker, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									goto IL_2FF;
								}
								if (execResult)
								{
									if (systemMagic.GetIntValue("MagicType", -1) == 3)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.GetObjectID(), enemyX, enemyY, magicCode);
									}
								}
								ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
							}
							else if (-1 == targetType || 2 == targetType || 3 == targetType)
							{
								attackDirection = (int)attacker.CurrentDir;
								if (-1 != enemyX && -1 != enemyY)
								{
									attackDirection = (int)Global.GetDirectionByTan((double)enemyX, (double)enemyY, attacker.CurrentPos.X, attacker.CurrentPos.Y);
								}
								if (2 == targetType)
								{
									if (-1 == enemy)
									{
										enemy = attacker.GetObjectID();
									}
									else if (attacker.GetObjectID() != enemy)
									{
										enemy = -1;
									}
								}
								else if (-1 == targetType || 3 == targetType)
								{
									if (-1 == enemy || enemy == attacker.GetObjectID())
									{
										if (-1 == enemy)
										{
											Point targetPos = new Point((double)enemyX, (double)enemyY);
											if (1 == systemMagic.GetIntValue("TargetPos", -1))
											{
												targetPos = new Point((double)enemyX, (double)enemyY);
											}
											else if (2 == systemMagic.GetIntValue("TargetPos", -1))
											{
												if (-1 != enemy)
												{
													if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out targetPos))
													{
														targetPos = new Point((double)enemyX, (double)enemyY);
													}
												}
												attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
											}
											else
											{
												attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
											}
											List<object> enemiesObjList = new List<object>();
											GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)targetPos.X, (int)targetPos.Y, 50, enemiesObjList);
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)targetPos.X, (int)targetPos.Y, 50, enemiesObjList);
											if (enemiesObjList.Count > 0)
											{
												int index = Global.GetRandomNumber(0, enemiesObjList.Count);
												enemy = (enemiesObjList[index] as IObject).GetObjectID();
											}
										}
									}
									else if (!SpriteAttack.IsOpposition(attacker, attacker.CurrentMapCode, enemy))
									{
										enemy = -1;
									}
								}
								if (targetPlayingType <= 0)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
								}
								if (-1 != enemy)
								{
									GSpriteTypes st = Global.GetSpriteType((uint)enemy);
									if (st == GSpriteTypes.Monster)
									{
										Monster enemyMonster = GameManager.MonsterMgr.FindMonster(attacker.CurrentMapCode, enemy);
										if (null != enemyMonster)
										{
											if (1 == targetPlayingType)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyMonster.RoleID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, enemyMonster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)enemyMonster.SafeCoordinate.X, (int)enemyMonster.SafeCoordinate.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyMonster);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyMonster);
										}
									}
									else if (st == GSpriteTypes.BiaoChe)
									{
										BiaoCheItem enemyBiaoCheItem = BiaoCheManager.FindBiaoCheByID(enemy);
										if (null != enemyBiaoCheItem)
										{
										}
									}
									else if (st == GSpriteTypes.JunQi)
									{
										JunQiItem enemyJunQiItem = JunQiManager.FindJunQiByID(enemy);
										if (null != enemyJunQiItem)
										{
											if (1 == targetPlayingType)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyJunQiItem.GetObjectID(), enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, enemyJunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)enemyJunQiItem.CurrentPos.X, (int)enemyJunQiItem.CurrentPos.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyJunQiItem);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyJunQiItem);
										}
									}
									else if (st == GSpriteTypes.FakeRole)
									{
										FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(enemy);
										if (null != fakeRoleItem)
										{
										}
									}
									else
									{
										GameClient enemyClient = GameManager.ClientMgr.FindClient(enemy);
										if (null != enemyClient)
										{
											if (1 == targetPlayingType)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, enemyClient.ClientData.RoleID, enemyX, enemyY, magicCode);
											}
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, enemyClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyClient.ClientData.PosX, enemyClient.ClientData.PosY, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, enemyClient);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, enemyClient);
										}
									}
								}
							}
							else
							{
								if (targetPlayingType <= 0)
								{
									GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
								}
								for (int i = 0; i < magicActionItemList.Count; i++)
								{
									MagicAction.ProcessAction(attacker, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
								}
								ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
							}
						}
						else
						{
							attackDirection = (int)attacker.CurrentDir;
							Point targetPos;
							if (1 == systemMagic.GetIntValue("TargetPos", -1))
							{
								targetPos = attacker.CurrentPos;
							}
							else if (2 == systemMagic.GetIntValue("TargetPos", -1))
							{
								targetPos = new Point((double)enemyX, (double)enemyY);
								if (-1 != enemy)
								{
									if (!SpriteAttack.GetEnemyPos(attacker.CurrentMapCode, enemy, out targetPos))
									{
										targetPos = new Point((double)enemyX, (double)enemyY);
									}
								}
								attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
							}
							else
							{
								targetPos = new Point((double)enemyX, (double)enemyY);
								attackDirection = (int)Global.GetDirectionByTan((double)((int)targetPos.X), (double)((int)targetPos.Y), attacker.CurrentPos.X, attacker.CurrentPos.Y);
							}
							List<object> clientList = new List<object>();
							if (magicScanTypeItem != null)
							{
								if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
								{
									GameManager.ClientMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], clientList);
								}
								else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
								{
									GameManager.ClientMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList, magicScanTypeItem.MagicActionParams[0], true);
								}
								else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
								{
									GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList);
								}
							}
							else
							{
								GameManager.ClientMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CurrentCopyMapID, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), clientList);
							}
							int targetType = systemMagic.GetIntValue("TargetType", -1);
							if (1 != targetType)
							{
								if (2 == targetType)
								{
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < magicActionItemList.Count; i++)
									{
										MagicAction.ProcessAction(attacker, attacker, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									bFindTarget = true;
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
									for (int j = 0; j < clientList.Count; j++)
									{
										if ((clientList[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (!Global.IsOpposition(attacker, clientList[j] as GameClient))
											{
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
												}
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, clientList[j] as GameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, clientList[j] as GameClient);
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
									}
								}
								else if (-1 == targetType || 3 == targetType)
								{
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									List<object> monsterList = new List<object>();
									if (magicScanTypeItem != null)
									{
										if (magicScanTypeItem.MagicActionID == MagicActionIDs.SCAN_SQUARE)
										{
											GameManager.MonsterMgr.LookupRolesInSquare(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, (int)magicScanTypeItem.MagicActionParams[0], (int)magicScanTypeItem.MagicActionParams[1], monsterList, 1);
										}
										else if (magicScanTypeItem.MagicActionID == MagicActionIDs.FRONT_SECTOR)
										{
											GameManager.MonsterMgr.LookupEnemiesInCircleByAngle((int)attacker.Direction, attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, magicScanTypeItem.MagicActionParams[0], true, 1);
										}
										else if (magicScanTypeItem.MagicActionID == MagicActionIDs.ROUNDSCAN)
										{
											GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, 1);
										}
									}
									else
									{
										GameManager.MonsterMgr.LookupEnemiesInCircle(attacker.CurrentMapCode, attacker.CopyMapID, (int)attacker.CurrentPos.X, (int)attacker.CurrentPos.Y, (int)targetPos.X, (int)targetPos.Y, Global.SafeConvertToInt32(systemMagic.GetStringValue("AttackDistance")), monsterList, 1);
									}
									List<object> enemyList = new List<object>();
									foreach (object tmp in monsterList)
									{
										if (Global.IsOpposition(attacker, tmp as Monster))
										{
											enemyList.Add(tmp);
										}
									}
									foreach (object tmp in clientList)
									{
										if ((tmp as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (Global.IsOpposition(attacker, tmp as GameClient))
											{
												enemyList.Add(tmp);
											}
										}
									}
									double shenShiInjurePercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(attacker, enemyList, magicCode);
									for (int j = 0; j < clientList.Count; j++)
									{
										if ((clientList[j] as GameClient).ClientData.RoleID != attacker.GetObjectID())
										{
											if (Global.IsOpposition(attacker, clientList[j] as GameClient))
											{
												for (int i = 0; i < magicActionItemList.Count; i++)
												{
													MagicAction.ProcessAction(attacker, clientList[j] as GameClient, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, magicCode, shenShiInjurePercent);
												}
												bFindTarget = true;
												if (targetPlayingType == 1)
												{
													GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (clientList[j] as GameClient).ClientData.RoleID, enemyX, enemyY, magicCode);
												}
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, clientList[j] as GameClient);
												ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, clientList[j] as GameClient);
												if (--maxNumHitted <= 0)
												{
													break;
												}
											}
										}
									}
									for (int j = 0; j < monsterList.Count; j++)
									{
										if (Global.IsOpposition(attacker, monsterList[j] as Monster))
										{
											for (int i = 0; i < magicActionItemList.Count; i++)
											{
												MagicAction.ProcessAction(attacker, monsterList[j] as Monster, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, magicCode, shenShiInjurePercent);
											}
											bFindTarget = true;
											if (targetPlayingType == 1)
											{
												GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (monsterList[j] as Monster).RoleID, enemyX, enemyY, magicCode);
											}
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, monsterList[j] as Monster);
											ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType1_extensionPropsList, attacker, monsterList[j] as Monster);
											if (--maxNumHitted <= 0)
											{
												break;
											}
										}
									}
									List<object> junQiItemList = new List<object>();
									JunQiManager.LookupRangeAttackEnemies(attacker, (int)targetPos.X, (int)targetPos.Y, attackDirection, systemMagic.GetStringValue("AttackDistance"), junQiItemList);
									for (int j = 0; j < junQiItemList.Count; j++)
									{
										for (int i = 0; i < magicActionItemList.Count; i++)
										{
											MagicAction.ProcessAction(attacker, junQiItemList[j] as JunQiItem, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, (int)targetPos.X, (int)targetPos.Y, subMagicV, 1, -1, 0, 0, attackDirection, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
										}
										if (targetPlayingType == 1)
										{
											GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, (junQiItemList[j] as JunQiItem).JunQiID, enemyX, enemyY, magicCode);
										}
									}
								}
								else
								{
									if (targetPlayingType <= 0)
									{
										GameManager.ClientMgr.NotifySpriteHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, -1, enemyX, enemyY, magicCode);
									}
									for (int i = 0; i < magicActionItemList.Count; i++)
									{
										MagicAction.ProcessAction(attacker, null, magicActionItemList[i].MagicActionID, magicActionItemList[i].MagicActionParams, enemyX, enemyY, subMagicV, attacker.CurrentMagicLevel, magicCode, 0, 0, (int)attacker.CurrentDir, 0, false, false, manyRangeInjuredPercent, 1, 0.0);
									}
									ExtensionPropsMgr.ExecuteExtensionPropsActions(actionType0_extensionPropsList, attacker, attacker);
								}
							}
						}
						result = bFindTarget;
					}
				}
			}
			return result;
		}

		// Token: 0x0600386B RID: 14443 RVA: 0x00302C44 File Offset: 0x00300E44
		public static void ProcessManyAttackMagicFinish(bool bFindTarget, Monster attacker)
		{
			if (!bFindTarget || attacker.MyMagicsManyTimeDmageQueue.GetManyTimeDmageQueueItemNumEx() < 1)
			{
				attacker.MagicFinish = 1;
				attacker.CurrentMagic = -1;
				GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, attacker, attacker.RoleID, attacker.MonsterZoneNode.MapCode, -1, 116);
			}
		}
	}
}
