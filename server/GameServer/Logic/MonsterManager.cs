using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.RefreshIconState;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class MonsterManager
	{
		
		public void initialize(IEnumerable<XElement> mapItems)
		{
			this.MyMonsterContainer.initialize(mapItems);
		}

		
		public static bool CanMonsterSeekRange(Monster monster)
		{
			return monster.MonsterType != 101 || monster.MonsterInfo.VLevel > MonsterManager.MinSeekRangeMonsterLevel;
		}

		
		public void AddMonster(Monster monster)
		{
			this.MyMonsterContainer.AddObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
		}

		
		public void RemoveMonster(Monster monster)
		{
			this.MyMonsterContainer.RemoveObject(monster.RoleID, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster);
		}

		
		public int GetTotalMonstersCount()
		{
			return this.MyMonsterContainer.ObjectList.Count;
		}

		
		public List<object> GetObjectsByMap(int mapCode)
		{
			return this.MyMonsterContainer.GetObjectsByMap(mapCode, -1);
		}

		
		public int GetMapMonstersCount(int mapCode)
		{
			return this.MyMonsterContainer.GetObjectsCountByMap(mapCode);
		}

		
		public List<object> GetCopyMapIDMonsterList(int copyMapID)
		{
			return this.MyMonsterContainer.GetObjectsByCopyMapID(copyMapID);
		}

		
		public int GetCopyMapIDMonstersCount(int copyMapID, int aliveType = -1)
		{
			return this.MyMonsterContainer.GetObjectsCountByCopyMapID(copyMapID, aliveType);
		}

		
		public bool IsAnyMonsterAliveByCopyMapID(int copyMapID)
		{
			return this.MyMonsterContainer.IsAnyMonsterAliveByCopyMapID(copyMapID);
		}

		
		public Monster FindMonster(int mapCode, int roleID)
		{
			object obj = this.MyMonsterContainer.FindObject(roleID, mapCode);
			return obj as Monster;
		}

		
		public List<Monster> FindMonsterAll(int mapCode)
		{
			List<object> objList = this.MyMonsterContainer.FindObjectAll(mapCode);
			List<Monster> ret = new List<Monster>();
			foreach (object item in objList)
			{
				if (item is Monster)
				{
					ret.Add(item as Monster);
				}
			}
			return ret;
		}

		
		public List<object> FindMonsterByExtensionID(int copyMapID, int extensionID)
		{
			return this.MyMonsterContainer.FindObjectsByExtensionID(extensionID, copyMapID);
		}

		
		public void LookupEnemiesInCircle(int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is Monster)
					{
						if (copyMapID == (objList[i] as Monster).CopyMapID)
						{
							if (Global.InCircle((objList[i] as Monster).SafeCoordinate, center, (double)radius))
							{
								enemiesList.Add(objList[i]);
							}
						}
					}
				}
			}
		}

		
		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> objList = new List<object>();
			this.LookupEnemiesInCircleByAngle(direction, mapCode, copyMapID, toX, toY, radius, objList, angle, near180);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as Monster).RoleID);
			}
		}

		
		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double loAngleNear = 0.0;
				double hiAngleNear = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out loAngleNear, out hiAngleNear);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is Monster)
					{
						if (copyMapID == (objList[i] as Monster).CopyMapID)
						{
							if (Global.InCircleByAngle((objList[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(objList[i]);
							}
							else if (Global.InCircle((objList[i] as Monster).SafeCoordinate, center, 100.0))
							{
								enemiesList.Add(objList[i]);
							}
						}
					}
				}
			}
		}

		
		public void LookupEnemiesInCircleByRoleAngle(int centerAngle, int mapCode, int copyMapID, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeAngle((double)centerAngle, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is Monster)
					{
						if (copyMapID == (objList[i] as Monster).CopyMapID)
						{
							if (Global.InCircleByAngle((objList[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(objList[i]);
							}
							else if (Global.InCircle((objList[i] as Monster).SafeCoordinate, center, 100.0))
							{
								enemiesList.Add(objList[i]);
							}
						}
					}
				}
			}
		}

		
		public void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(client.ClientData.PosX, client.ClientData.PosY, radius);
			if (null != objsList)
			{
				Point source = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				Point toPos = Global.GetAPointInCircle(source, radius, client.ClientData.RoleYAngle);
				int toX = (int)toPos.X;
				int toY = (int)toPos.Y;
				Point center = default(Point);
				center.X = (double)((client.ClientData.PosX + toX) / 2);
				center.Y = (double)((client.ClientData.PosY + toY) / 2);
				int fDirectionX = toX - client.ClientData.PosX;
				int fDirectionY = toY - client.ClientData.PosY;
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if ((objsList[i] as Monster).VLife > 0.0)
						{
							if (client == null || client.ClientData.CopyMapID == (objsList[i] as Monster).CopyMapID)
							{
								Point target = new Point((objsList[i] as Monster).CurrentPos.X, (objsList[i] as Monster).CurrentPos.Y);
								if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
								{
									rolesList.Add(objsList[i]);
								}
								else if (Global.InCircle(target, source, 100.0))
								{
									rolesList.Add(objsList[i]);
								}
							}
						}
					}
				}
			}
		}

		
		public void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];
			List<object> objsList = mapGrid.FindObjects(gridX, gridY);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if (attacker.CurrentCopyMapID == (objsList[i] as Monster).CopyMapID)
						{
							enemiesList.Add(objsList[i]);
						}
					}
				}
			}
		}

		
		public void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[attacker.CurrentMapCode];
			Point grid = attacker.CurrentGrid;
			int gridX = (int)grid.X;
			int gridY = (int)grid.Y;
			Point p = Global.GetGridPointByDirection(direction, gridX, gridY);
			this.LookupEnemiesAtGridXY(attacker, (int)p.X, (int)p.Y, enemiesList);
		}

		
		public void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			this.LookupAttackEnemies(attacker, direction, objList);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as Monster).RoleID);
			}
		}

		
		public void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridList = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridList.Count > 0)
			{
				for (int i = 0; i < gridList.Count; i++)
				{
					this.LookupEnemiesAtGridXY(obj, (int)gridList[i].X, (int)gridList[i].Y, enemiesList);
				}
			}
		}

		
		public void LookupRolesInSquare(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, int nWidth, List<object> rolesList, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(srcX, srcY, radius);
			if (null != objList)
			{
				Point source = new Point((double)srcX, (double)srcY);
				Point center = default(Point);
				center.X = (double)((srcX + toX) / 2);
				center.Y = (double)((srcY + toY) / 2);
				int fDirectionX = toX - srcX;
				int fDirectionY = toY - srcY;
				for (int i = 0; i < objList.Count; i++)
				{
					ObjectTypes ot = (objList[i] as IObject).ObjectType;
					if ((ot & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (ot == ObjectTypes.OT_MONSTER)
						{
							if ((objList[i] as Monster).VLife > 0.0)
							{
								if (copyMapId == (objList[i] as Monster).CopyMapID)
								{
									Point target = new Point((objList[i] as Monster).CurrentPos.X, (objList[i] as Monster).CurrentPos.Y);
									if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
									{
										rolesList.Add(objList[i]);
									}
									else if (Global.InCircle(target, source, 100.0))
									{
										rolesList.Add(objList[i]);
									}
								}
							}
						}
						else if (ot == ObjectTypes.OT_CLIENT)
						{
							GameClient cli = objList[i] as GameClient;
							if (cli != null)
							{
								if ((objList[i] as GameClient).ClientData.LifeV > 0)
								{
									if (copyMapId == (objList[i] as GameClient).ClientData.CopyMapID)
									{
										Point target = new Point((double)cli.ClientData.PosX, (double)cli.ClientData.PosY);
										if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
										{
											rolesList.Add(cli);
										}
										else if (Global.InCircle(target, source, 100.0))
										{
											rolesList.Add(cli);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double loAngleNear = 0.0;
				double hiAngleNear = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out loAngleNear, out hiAngleNear);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					ObjectTypes ot = (objsList[i] as IObject).ObjectType;
					if ((ot & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (ot == ObjectTypes.OT_MONSTER)
						{
							if (copyMapId == (objsList[i] as Monster).CopyMapID)
							{
								if ((objsList[i] as Monster).VLife > 0.0)
								{
									if (Global.InCircleByAngle((objsList[i] as Monster).SafeCoordinate, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(objsList[i]);
									}
									else if (Global.InCircle((objsList[i] as Monster).SafeCoordinate, center, 100.0))
									{
										enemiesList.Add(objsList[i]);
									}
								}
							}
						}
						else if (ot == ObjectTypes.OT_CLIENT)
						{
							GameClient cli = objsList[i] as GameClient;
							if (cli != null)
							{
								if (copyMapId == cli.ClientData.CopyMapID)
								{
									Point target = new Point((double)cli.ClientData.PosX, (double)cli.ClientData.PosY);
									if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(cli);
									}
									else if (Global.InCircle(target, center, 160.0))
									{
										enemiesList.Add(cli);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void LookupEnemiesInCircle(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, List<object> enemiesList, int type)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					ObjectTypes ot = (objsList[i] as IObject).ObjectType;
					if ((ot & (ObjectTypes)type) != ObjectTypes.OT_CLIENT)
					{
						if (ot == ObjectTypes.OT_MONSTER)
						{
							if (copyMapId == (objsList[i] as Monster).CopyMapID)
							{
								if ((objsList[i] as Monster).VLife > 0.0)
								{
									if (Global.InCircle((objsList[i] as Monster).SafeCoordinate, center, (double)radius))
									{
										enemiesList.Add(objsList[i]);
									}
								}
							}
						}
						else if (ot == ObjectTypes.OT_CLIENT)
						{
							GameClient cli = objsList[i] as GameClient;
							if (cli != null)
							{
								if (copyMapId == cli.ClientData.CopyMapID)
								{
									Point target = new Point((double)cli.ClientData.PosX, (double)cli.ClientData.PosY);
									if (Global.InCircle(target, center, (double)radius))
									{
										enemiesList.Add(cli);
									}
								}
							}
						}
					}
				}
			}
		}

		
		public void SendMonsterToClients(SocketListener sl, Monster monster, TCPOutPacketPool pool, List<object> objList, int cmd)
		{
			if (null != objList)
			{
				if (monster.VLife > 0.0 && monster.Alive)
				{
					MonsterData md = monster.GetMonsterData();
					if (md.LifeV <= 0.0)
					{
						Debug.WriteLine(string.Format("怪物 Role{0} 生命值为0， 不再发送", monster.RoleID));
					}
					else
					{
						for (int i = 0; i < objList.Count; i++)
						{
							if (objList[i] is GameClient)
							{
								TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(md, pool, cmd);
								if (!sl.SendData((objList[i] as GameClient).ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		
		public int SendMonstersToClient(SocketListener sl, GameClient client, TCPOutPacketPool pool, List<object> objList, int cmd)
		{
			int result;
			if (null == objList)
			{
				result = 0;
			}
			else
			{
				int totalCount = 0;
				int i = 0;
				while (i < objList.Count && i < 50)
				{
					if (objList[i] is Monster)
					{
						if (!(objList[i] is Robot))
						{
							if ((objList[i] as Monster).VLife > 0.0 && (objList[i] as Monster).Alive)
							{
								MonsterData md = (objList[i] as Monster).GetMonsterData();
								if (GameManager.FlagEnableHideFlags)
								{
									if (md.EquipmentBody < 0)
									{
										md.EquipmentBody = 0;
									}
								}
								TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<MonsterData>(md, pool, cmd);
								if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
								{
									break;
								}
								totalCount++;
								if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
								{
									BossLifeLog bossLifeLog = MoYuLongXue.GetBossAttackLog(client.ClientData.Faction, md.RoleID);
									if (null != bossLifeLog)
									{
										client.sendCmd<BossLifeLog>(1906, bossLifeLog, false);
									}
								}
							}
						}
					}
					i++;
				}
				result = totalCount;
			}
			return result;
		}

		
		public void ProcessMonsterDead(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster, int enemyExperience, int enemyMoney, int injure)
		{
			monster.TimedActionMgr.RemoveItem(0);
			if (attacker is GameClient)
			{
				this.ProcessMonsterDeadByClient(sl, pool, attacker as GameClient, monster, enemyExperience, enemyMoney);
				GameClient client = attacker as GameClient;
				client.passiveSkillModule.OnKillMonster(client);
			}
			else if (attacker is Monster)
			{
				this.ProcessMonsterDeadByMonster(sl, pool, attacker as Monster, monster, enemyExperience, enemyMoney);
			}
			if ((int)(Math.Pow(2.0, 31.0) - 1.0) != injure)
			{
				if (1001 == monster.MonsterType && null != monster.OwnerClient)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster.OwnerClient, StringUtil.substitute(GLang.GetLang(502, new object[0]), new object[]
					{
						monster.MonsterInfo.VSName
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 28);
				}
			}
		}

		
		private void ProcessMonsterDeadByClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
		{
			if (!monster.HandledDead)
			{
				monster.HandledDead = true;
				if (monster.MonsterType != 1001 || monster.OwnerClient != client)
				{
					GameManager.AngelTempleMgr.KillAngelBoss(client, monster);
					if (monster.MonsterInfo.FallBelongTo == 1)
					{
						int ownerRoleID = monster.GetAttackerFromList();
						if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
						{
							GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
							if (null != findClient)
							{
								client = findClient;
							}
						}
					}
					bool isTeamSharingMap = true;
					if (client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode)
					{
						isTeamSharingMap = false;
					}
					TeamData td = null;
					if (client.ClientData.TeamID > 0 && isTeamSharingMap)
					{
						td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
					}
					if (null == td)
					{
						this.ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
					}
					else
					{
						int totalTeamMemberNum = 0;
						List<GameClient> clientsList = new List<GameClient>();
						lock (td)
						{
							for (int i = 0; i < td.TeamRoles.Count; i++)
							{
								if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									totalTeamMemberNum++;
									clientsList.Add(client);
								}
								else
								{
									GameClient gc = GameManager.ClientMgr.FindClient(td.TeamRoles[i].RoleID);
									if (null != gc)
									{
										if (gc.ClientData.MapCode == client.ClientData.MapCode)
										{
											if (gc.ClientData.CopyMapID == client.ClientData.CopyMapID)
											{
												if (Global.InCircle(new Point((double)gc.ClientData.PosX, (double)gc.ClientData.PosY), monster.SafeCoordinate, 4000.0))
												{
													totalTeamMemberNum++;
													clientsList.Add(gc);
												}
											}
										}
									}
								}
							}
						}
						if (clientsList.Count >= 1)
						{
							int shareExperience = (int)((double)(enemyExperience * (clientsList.Count - 1)) * 0.1) / clientsList.Count;
							for (int i = 0; i < clientsList.Count; i++)
							{
								if (client == clientsList[i])
								{
									this.ProcessSpriteKillMonster(sl, pool, clientsList[i], monster, enemyExperience + shareExperience, 0);
								}
								else
								{
									this.ProcessSpriteKillMonster(sl, pool, clientsList[i], monster, shareExperience, 0);
								}
							}
						}
						else
						{
							this.ProcessSpriteKillMonster(sl, pool, client, monster, enemyExperience, enemyMoney);
						}
					}
					monster.WhoKillMeID = client.ClientData.RoleID;
					monster.WhoKillMeName = Global.FormatRoleName(client, client.ClientData.RoleName);
					GameManager.GuildCopyMapMgr.ProcessMonsterDead(client, monster);
					CopyTargetManager.ProcessMonsterDead(client, monster);
					List<int> attackerList = monster.GetAttackerList();
					for (int i = 0; i < attackerList.Count; i++)
					{
						if (client.ClientData.RoleID != attackerList[i])
						{
							GameClient gc = GameManager.ClientMgr.FindClient(attackerList[i]);
							if (null != gc)
							{
								if (gc.ClientData.MapCode == client.ClientData.MapCode)
								{
									if (gc.ClientData.CopyMapID == client.ClientData.CopyMapID)
									{
										if (Global.InCircle(new Point((double)gc.ClientData.PosX, (double)gc.ClientData.PosY), monster.SafeCoordinate, 500.0))
										{
											ProcessTask.Process(sl, pool, gc, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster, null, 0, -1L, null);
										}
									}
								}
							}
						}
					}
					List<GoodsPackItem> dropPacks = GameManager.GoodsPackMgr.ProcessMonster(sl, pool, client, monster);
					if (monster.MonsterType == 401)
					{
						string _dropString = string.Empty;
						int i = 0;
						while (dropPacks != null && i < dropPacks.Count)
						{
							_dropString = ((i == 0) ? EventLogManager.MakeGoodsDataPropString(dropPacks[i].GoodsDataList) : (_dropString + EventLogManager.AddGoodsDataPropString(dropPacks[i].GoodsDataList)));
							i++;
						}
						EventLogManager.AddBossDiedEvent(client, monster.MonsterInfo.ExtensionID, monster.CurrentMapCode, monster.GetMonsterBirthTick(), TimeUtil.NOW() * 10000L, _dropString);
					}
					GameManager.ClientMgr.ChangeRoleLianZhan(sl, pool, client, monster, 1);
					Global.BroadcastXKilledMonster(client, monster);
					GlobalEventSource.getInstance().fireEvent(new KillMonsterEventObject(monster, client));
					GameManager.ClientMgr.UpdateKillBoss(client, 1, monster, false);
					Global.AddBattleKilledNum(client, monster, monster.MonsterInfo.BattlePersonalJiFen, monster.MonsterInfo.BattleZhenYingJiFen);
					this.ProcessOtherEventsOnMonsterDead(sl, pool, client, monster);
					this.ProcessMonsterDeadEvents(sl, pool, client, monster);
					if (401 == monster.MonsterType)
					{
						TimerBossManager.getInstance().RemoveBoss(monster.MonsterInfo.ExtensionID);
					}
					if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
					{
						ChengJiuManager.OnMonsterKilled(client, monster);
						DailyActiveManager.ProcessDailyActiveKillMonster(client, monster);
					}
					SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.KillMonsterInMap);
					evObj.Arg1 = client.ClientData.MapCode;
					evObj.Arg2 = monster.MonsterInfo.ExtensionID;
					GlobalEventSource.getInstance().fireEvent(evObj);
					DBRoleBufferManager.ProcessWaWaGiveExperience(client, monster);
					if (client.ClientData.MapCode == 6090)
					{
						FreshPlayerCopySceneManager.KillMonsterInFreshPlayerScene(client, monster);
					}
					if (Global.IsInExperienceCopyScene(client.ClientData.MapCode))
					{
						ExperienceCopySceneManager.ExperienceCopyMapKillMonster(client, monster);
					}
					if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
					{
						GameManager.BloodCastleCopySceneMgr.KillMonsterABloodCastCopyScene(client, monster);
					}
					if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
					{
						GameManager.DaimonSquareCopySceneMgr.DaimonSquareSceneKillMonster(client, monster);
					}
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhen(client.ClientData.FuBenID))
					{
						LuoLanFaZhenCopySceneManager.OnKillMonster(client, monster);
					}
					if (ElementWarManager.getInstance().IsElementWarCopy(client.ClientData.FuBenID))
					{
						ElementWarManager.getInstance().KillMonster(client, monster);
					}
				}
			}
		}

		
		private void ProcessMonsterDeadByMonster(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster monster, int enemyExperience, int enemyMoney)
		{
			if (!monster.HandledDead)
			{
				if (null != attacker.OwnerClient)
				{
					this.ProcessMonsterDeadByClient(sl, pool, attacker.OwnerClient, monster, enemyExperience, enemyMoney);
				}
				else
				{
					monster.HandledDead = true;
					GameManager.GoodsPackMgr.ProcessMonster(sl, pool, attacker, monster);
					this.ProcessMonsterDeadEvents(sl, pool, attacker, monster);
				}
			}
		}

		
		private void ProcessSpriteKillMonster(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int enemyExperience, int enemyMoney)
		{
			int oldLevel = client.ClientData.Level;
			int overlevel = 0;
			if (enemyExperience > 0)
			{
				int origEnemyExperience = enemyExperience;
				double dblExperience = DBRoleBufferManager.ProcessDblAndThreeExperience(client);
				if (SpecailTimeManager.JugeIsDoulbeExperienceAndLingli())
				{
					dblExperience += 1.0;
				}
				HeFuAwardTimesActivity activity = HuodongCachingMgr.GetHeFuAwardTimesActivity();
				if (activity != null && activity.InActivityTime() && monster.CopyMapID <= 0)
				{
					if ((double)activity.activityTimes > 0.0 && SpecailTimeManager.InSpercailTime(activity.specialTimeID))
					{
						dblExperience += (double)activity.activityTimes - 1.0;
					}
				}
				dblExperience += Global.ProcessLingDiMonsterExperience(client, monster);
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 100))
				{
					BufferData bufferData = Global.GetBufferDataByID(client, 100);
					if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
					{
						dblExperience += (double)bufferData.BufferVal / 100.0;
					}
				}
				if (DBRoleBufferManager.ProcessMonthVIP(client) > 0.0)
				{
					double gumuExp = Global.GetVipGuMuExperienceMultiple(client);
					dblExperience += gumuExp;
				}
				dblExperience += Global.ProcessLingDiMonsterExperience(client, monster);
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 89))
				{
					BufferData bufferData = Global.GetBufferDataByID(client, 89);
					if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
					{
						int[] goodsIds = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengJiTanBUFF", ',');
						int goodsID = 0;
						if (goodsIds != null && (long)goodsIds.Length > bufferData.BufferVal)
						{
							goodsID = goodsIds[(int)(checked((IntPtr)bufferData.BufferVal))];
						}
						List<MagicActionItem> lsMagicAction = new List<MagicActionItem>();
						if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsID, out lsMagicAction))
						{
							for (int i = 0; i < lsMagicAction.Count; i++)
							{
								if (lsMagicAction[i].MagicActionID == MagicActionIDs.DB_ADD_MULTIEXP)
								{
									dblExperience += lsMagicAction[i].MagicActionParams[0];
								}
							}
						}
					}
				}
				enemyExperience = (int)((double)enemyExperience * dblExperience);
				int nWorldLevelAddExp = 0;
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 99))
				{
					if (client.ClientData.nTempWorldLevelPer > 0.0)
					{
						nWorldLevelAddExp = (int)((double)origEnemyExperience * client.ClientData.nTempWorldLevelPer);
					}
				}
				enemyExperience += nWorldLevelAddExp;
				GameManager.ClientMgr.ProcessRoleExperience(client, (long)enemyExperience, true, false, true, "none");
			}
			long rebornExp = (long)monster.MonsterInfo.RebornExp;
			if (rebornExp > 0L)
			{
				double rebornExpRate = GameManager.ClientMgr.GetLianZhanExpRate(client);
				rebornExp = (long)((double)rebornExp * (1.0 + rebornExpRate));
				double dblExperience = DBRoleBufferManager.ProcessRebornMultiExperience(client);
				rebornExp = (long)((int)((double)rebornExp * dblExperience));
				RebornManager.getInstance().ProcessRoleExperience(client, rebornExp, MoneyTypes.RebornExpMonster, true, false, true, "none");
			}
			if (enemyMoney > 0)
			{
				if (overlevel > 10)
				{
					enemyMoney = (int)((double)enemyMoney * (1.0 - (double)(overlevel - 10) * 0.045));
					enemyMoney = Global.GMax(0, enemyMoney);
				}
				enemyMoney = Global.FilterValue(client, enemyMoney);
				if (enemyMoney > 0)
				{
					enemyMoney = (int)((double)enemyMoney * DBRoleBufferManager.ProcessDblAndThreeMoney(client));
					GameManager.ClientMgr.AddMoney1(sl, Global._TCPManager.tcpClientPool, pool, client, enemyMoney, "杀死怪物", false);
				}
			}
			ProcessTask.Process(sl, pool, client, monster.RoleID, monster.MonsterInfo.ExtensionID, -1, TaskTypes.KillMonster, null, 0, -1L, null);
			GameManager.CopyMapMgr.ProcessKilledMonster(client, monster);
		}

		
		public void ProcessMonsterDeadEvents(SocketListener sl, TCPOutPacketPool pool, IObject attacker, Monster monster)
		{
			if (901 == monster.MonsterType)
			{
				GameManager.ShengXiaoGuessMgr.OnBossKilled();
			}
		}

		
		public bool ChangePosition(SocketListener sl, TCPOutPacketPool pool, Monster monster, int toMapX, int toMapY, int toMapDirection, int nID, int animation = 0)
		{
			if (toMapDirection > 0)
			{
				monster.Direction = (double)toMapDirection;
			}
			monster.LastSeekEnemyTicks = TimeUtil.NOW();
			monster.VisibleItemList = null;
			this.LoseTarget(monster);
			monster.Coordinate = new Point((double)toMapX, (double)toMapY);
			List<object> objsList = Global.GetAll9Clients(monster);
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					monster.RoleID,
					toMapX,
					toMapY,
					toMapDirection,
					animation
				});
				GameManager.ClientMgr.SendToClients(sl, pool, null, objsList, strcmd, nID);
				if (monster._Action != GActions.Stand)
				{
					List<object> listObjs = objsList;
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, listObjs);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = true;
			}
			return result;
		}

		
		public void ProcessOtherEventsOnMonsterDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster)
		{
		}

		
		public void AddSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, monster.VLife + lifeV);
					List<object> listObjs = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, listObjs, 0);
				}
			}
		}

		
		public void SubSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double lifeV)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					monster.VLife = Global.GMax(0.0, monster.VLife - lifeV);
					List<object> listObjs = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, listObjs, 0);
				}
			}
		}

		
		public void AddSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
		{
			if (monster.VLife > 0.0)
			{
				monster.VMana = (double)((int)Global.GMin(monster.MonsterInfo.VManaMax, monster.VMana + magicV));
				List<object> listObjs = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, listObjs, 0);
			}
		}

		
		public void SubSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, double magicV)
		{
			if (monster.VLife > 0.0)
			{
				monster.VMana = (double)((int)Global.GMax(0.0, monster.VMana - magicV));
				List<object> listObjs = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.Direction, monster.VLife, monster.VMana, 120, listObjs, 0);
			}
		}

		
		public int InjureToEnemy(Monster monster, int injured)
		{
			injured = DBMonsterBuffer.ProcessHuZhaoSubLifeV(monster, Math.Max(0, injured));
			injured = DBMonsterBuffer.ProcessWuDiHuZhaoNoInjured(monster, Math.Max(0, injured));
			injured = DBMonsterBuffer.ProcessMarriageFubenInjured(monster, Math.Max(0, injured));
			return Math.Max(0, injured);
		}

		
		public int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			int ret = 0;
			if ((enemy as Monster).VLife > 0.0 && (enemy as Monster).Alive)
			{
				bool selfLifeChanged = false;
				bool handled = false;
				if (enemy.ManagerType != SceneUIClasses.Normal)
				{
					PreMonsterInjureEventObject eventObject = new PreMonsterInjureEventObject(client, enemy, (int)enemy.ManagerType);
					handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObject, (int)enemy.ManagerType);
					if (handled)
					{
						injure = eventObject.Injure;
					}
				}
				if (injure <= 0)
				{
					if (!handled)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(client, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType || 2 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(client, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
				}
				MapSettingItem hurtInfo = Data.GetMapSettingInfo(client.ClientData.MapCode);
				EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
				int nMerlinInjure = 0;
				int armorV = -1;
				if (!handled)
				{
					if (injure > 0)
					{
						int lifeSteal = (int)RoleAlgorithm.GetLifeStealV(client);
						RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, "打怪击中恢复");
						if (lifeSteal > 0 && client.ClientData.CurrentLifeV > 0 && client.ClientData.CurrentLifeV < client.ClientData.LifeV)
						{
							relifeLog.hpModify = true;
							relifeLog.oldHp = client.ClientData.CurrentLifeV;
							selfLifeChanged = true;
							client.ClientData.CurrentLifeV += lifeSteal;
						}
						if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
						{
							client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						}
						relifeLog.newHp = client.ClientData.CurrentLifeV;
						SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
						injure = this.InjureToEnemy(enemy as Monster, injure);
						injure = DBRoleBufferManager.ProcessAntiBoss(client, enemy as Monster, injure);
						if (enemy is Robot)
						{
							injure /= 2;
						}
					}
					injure = (int)((double)injure * hurtInfo.NormalHuntNum);
					ret = injure;
					nMerlinInjure = GameManager.MerlinInjureMgr.CalcMerlinInjure(client, enemy, injure, ref eMerlinType);
					armorV = RoleAlgorithm.CallAttackArmor(client, enemy, ref injure, ref nMerlinInjure);
					int nRebornInjure = RebornManager.getInstance().CalcRebornInjure(client, enemy, injurePercent, baseRate, ref burst);
					injure += (int)((double)nRebornInjure * hurtInfo.RebornHuntNum);
				}
				if (enemy.ManagerType != SceneUIClasses.Normal)
				{
					AfterMonsterInjureEventObject eventObject2 = new AfterMonsterInjureEventObject(client, enemy, (int)enemy.ManagerType, injure, nMerlinInjure);
					handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObject2, (int)enemy.ManagerType);
					if (handled)
					{
						injure = eventObject2.Injure;
						nMerlinInjure = eventObject2.MerlinInjure;
					}
				}
				double nTemp = (enemy as Monster).VLife;
				(enemy as Monster).VLife -= (double)Global.GMax(0, injure + nMerlinInjure);
				(enemy as Monster).VLife = Global.GMax((enemy as Monster).VLife, 0.0);
				double enemyLife = (enemy as Monster).VLife;
				GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(enemy as Monster, client, 0));
				Monster monster = enemy as Monster;
				int nValue = (int)(nTemp - enemyLife);
				if (nTemp >= monster.MonsterInfo.VLifeMax || monster.Flags.InjureEvent)
				{
					GlobalEventSource.getInstance().fireEvent(new MonsterInjuredEventObject(enemy as Monster, client, injure));
				}
				if (client.ClientData.ExcellenceProp[15] > 0.0)
				{
					int nRan = Global.GetRandomNumber(0, 101);
					if ((double)nRan <= client.ClientData.ExcellenceProp[15] * 100.0)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						selfLifeChanged = true;
					}
				}
				if (client.ClientData.ExcellenceProp[16] > 0.0)
				{
					int nRan = Global.GetRandomNumber(0, 101);
					if ((double)nRan <= client.ClientData.ExcellenceProp[16] * 100.0)
					{
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						selfLifeChanged = true;
					}
				}
				bool hitFly = false;
				if ((enemy as Monster).VLife <= 0.0)
				{
					hitFly = true;
				}
				Point hitToGrid = new Point(-1.0, -1.0);
				if (hitFly)
				{
					hitToGrid = ChuanQiUtils.HitFly(client, enemy as Monster, ((enemy as Monster).VLife <= 0.0) ? 2 : 1);
					if ((int)hitToGrid.X > 0 && (int)hitToGrid.Y > 0)
					{
					}
				}
				if (!hitFly && nHitFlyDistance > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int nGridNum = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
					if (nGridNum > 0)
					{
						hitToGrid = ChuanQiUtils.HitFly(client, enemy, nGridNum);
					}
				}
				if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
				{
					GameManager.AngelTempleMgr.ProcessAttackBossInAngelTempleScene(client, enemy, nValue);
				}
				Interlocked.Add(ref client.SumDamageForCopyTeam, (long)nValue);
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
				{
					MoYuLongXue.ProcessAttack(client, enemy, nValue);
				}
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZhuanShengShiLian.ProcessAttack(client, enemy, nValue);
				}
				(enemy as Monster).AddAttacker(client, Global.GMax(0, injure + nMerlinInjure), null);
				client.ClientData.RoleIDAttackebByMyself = (enemy as Monster).RoleID;
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(client, enemy as Monster);
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy as Monster, client);
				if (client.ClientData.DSHideStart > 0L)
				{
					Global.RemoveBufferData(client, 41);
					client.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				SceneUIClasses sceneUIClasses = sceneType;
				if (sceneUIClasses != SceneUIClasses.Comp)
				{
					switch (sceneUIClasses)
					{
					case SceneUIClasses.CompMine:
						CompMineManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						break;
					case SceneUIClasses.ChongShengMap:
						RebornBoss.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						break;
					case SceneUIClasses.ZorkBattle:
						ZorkBattleManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						break;
					}
				}
				else
				{
					CompManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
				}
				if ((enemy as Monster).VLife <= 0.0)
				{
					Global.ProcessMonsterDieForRoleAttack(sl, pool, client, enemy, injure + nMerlinInjure);
					if (MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
					{
						if (MoYuLongXue.ProcessMonsterDie(enemy))
						{
						}
					}
					else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
					{
						ZhuanShengShiLian.ProcessBossDie(client, enemy);
					}
				}
				else if ((enemy as Monster).LockObject < 0)
				{
					(enemy as Monster).LockObject = client.ClientData.RoleID;
					(enemy as Monster).LockFocusTime = TimeUtil.NOW();
				}
				int ownerRoleID = (enemy as Monster).GetAttackerFromList();
				if (ownerRoleID >= 0 && ownerRoleID != client.ClientData.RoleID)
				{
					GameClient findClient = GameManager.ClientMgr.FindClient(ownerRoleID);
					if (null != findClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (enemy as Monster).RoleID, 0, 0, enemyLife, findClient.ClientData.Level, hitToGrid, nMerlinInjure, eMerlinType, armorV + 1);
						ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, enemy.RoleID, 0, 0, enemyLife, 0L, nMerlinInjure, eMerlinType, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as Monster).RoleID, burst, injure, enemyLife, client.ClientData.Level, hitToGrid, nMerlinInjure, eMerlinType, armorV + 1);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.RoleID, burst, injure, enemyLife, 0L, nMerlinInjure, eMerlinType, armorV + 1);
				Global.ProcessDamageThorn(sl, pool, client, enemy, injure);
				if (selfLifeChanged)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(sl, pool, client, true, false, 7);
				}
				sceneUIClasses = sceneType;
				if (sceneUIClasses <= SceneUIClasses.KingOfBattle)
				{
					if (sceneUIClasses != SceneUIClasses.YongZheZhanChang)
					{
						if (sceneUIClasses == SceneUIClasses.KingOfBattle)
						{
							KingOfBattleManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						}
					}
					else
					{
						YongZheZhanChangManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
					}
				}
				else
				{
					switch (sceneUIClasses)
					{
					case SceneUIClasses.LingDiCaiJi:
						LingDiCaiJiManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						break;
					case SceneUIClasses.YaoSaiWorld:
						break;
					case SceneUIClasses.BangHuiMatch:
						BangHuiMatchManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						break;
					default:
						if (sceneUIClasses != SceneUIClasses.WanMoXiaGu)
						{
							if (sceneUIClasses == SceneUIClasses.CompBattle)
							{
								CompBattleManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
							}
						}
						else
						{
							WanMoXiaGuManager.getInstance().OnInjureMonster(client, enemy, (long)nValue);
						}
						break;
					}
				}
				GameManager.damageMonitor.Out(client);
			}
			return ret;
		}

		
		public int Monster_NotifyInjured(SocketListener sl, TCPOutPacketPool pool, Monster attacker, Monster enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			int ret = 0;
			if ((enemy as Monster).VLife > 0.0 && (enemy as Monster).Alive)
			{
				bool handled = false;
				if (enemy.ManagerType != SceneUIClasses.Normal)
				{
					PreMonsterInjureEventObject eventObject = new PreMonsterInjureEventObject(attacker, enemy, (int)enemy.ManagerType);
					handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObject, (int)enemy.ManagerType);
					if (handled)
					{
						injure = eventObject.Injure;
					}
				}
				if (injure <= 0)
				{
					if (!handled)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(attacker, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType || 2 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(attacker, enemy as Monster, forceBurst, injurePercent, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
				}
				if (!handled)
				{
					int nRebornInjure = RebornManager.getInstance().CalcRebornInjure(attacker, enemy, injurePercent, baseRate, ref burst);
					MapSettingItem hurtInfo = Data.GetMapSettingInfo(attacker.CurrentMapCode);
					injure = (int)((double)injure * hurtInfo.NormalHuntNum) + (int)((double)nRebornInjure * hurtInfo.RebornHuntNum);
				}
				if (enemy.ManagerType != SceneUIClasses.Normal)
				{
					AfterMonsterInjureEventObject eventObject2 = new AfterMonsterInjureEventObject(attacker, enemy, (int)enemy.ManagerType, injure, 0);
					handled = GlobalEventSource4Scene.getInstance().fireEvent(eventObject2, (int)enemy.ManagerType);
					if (handled)
					{
						injure = eventObject2.Injure;
					}
				}
				ret = injure;
				double nTemp = (enemy as Monster).VLife;
				(enemy as Monster).VLife -= (double)Global.GMax(0, injure);
				(enemy as Monster).VLife = Global.GMax((enemy as Monster).VLife, 0.0);
				double enemyLife = (enemy as Monster).VLife;
				if ((enemy as Monster).VLife == 0.0)
				{
					int nValue = (int)nTemp;
				}
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy, attacker);
				GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(attacker, enemy);
				if (null != attacker.OwnerClient)
				{
					(enemy as Monster).AddAttacker(attacker.OwnerClient, Global.GMax(0, injure), attacker);
				}
				if ((enemy as Monster).VLife <= 0.0)
				{
					int enemyExperience = (enemy as Monster).MonsterInfo.VExperience;
					this.LoseTarget(enemy as Monster);
					if (null != attacker.OwnerClient)
					{
						Global.ProcessMonsterDieForRoleAttack(sl, pool, attacker.OwnerClient, enemy as Monster, injure);
					}
					else
					{
						Global.ProcessMonsterDieForMonsterAttack(sl, pool, attacker, enemy);
						this.ProcessMonsterDead(sl, pool, attacker, enemy as Monster, enemyExperience, (enemy as Monster).MonsterInfo.VMoney, injure);
						this.AddDelayDeadMonster(enemy as Monster);
					}
				}
				else if ((enemy as Monster).LockObject < 0)
				{
					(enemy as Monster).LockObject = attacker.RoleID;
					(enemy as Monster).LockFocusTime = TimeUtil.NOW();
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, attacker, attacker.MonsterZoneNode.MapCode, attacker.RoleID, (enemy as Monster).RoleID, burst, injure, enemyLife, attacker.MonsterInfo.VLevel, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
			}
			return ret;
		}

		
		public void DoMonsterHeartTimer(int mapCode = -1, int subMapCode = -1)
		{
			long ticks = TimeUtil.NOW();
			List<object> objectsList = this.MyMonsterContainer._ObjectList;
			if (mapCode != -1)
			{
				objectsList = this.MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
			}
			if (null != objectsList)
			{
				foreach (object obj in objectsList)
				{
					Monster monster = obj as Monster;
					if (monster.Alive)
					{
						monster.TimedActionMgr.Run(ticks);
						if (monster.MonsterType != 1801)
						{
							if (monster._Action != GActions.Stand)
							{
								if (ticks - monster.LastExecTimerTicks >= monster._HeartInterval)
								{
									monster.onSurvivalTick();
									monster.LastExecTimerTicks = ticks;
									monster.Timer_Tick(null, EventArgs.Empty);
								}
							}
						}
					}
				}
			}
		}

		
		private void MoveToRandomPoint(SocketListener sl, TCPOutPacketPool pool, Monster monster)
		{
		}

		
		private void CheckMonsterStandStatus(Monster monster)
		{
		}

		
		private bool CheckMonsterInObs(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			if (1001 == monster.MonsterType)
			{
				if (null != monster.OwnerClient)
				{
					Point monsterGrid = monster.CurrentGrid;
					Point clientGrid = monster.OwnerClient.CurrentGrid;
					if ((int)monsterGrid.X == (int)clientGrid.X && (int)monsterGrid.Y == (int)clientGrid.Y)
					{
						return false;
					}
				}
				if (null != monster.OwnerMonster)
				{
					Point monsterGrid = monster.CurrentGrid;
					Point ownerGrid = monster.OwnerMonster.CurrentGrid;
					if ((int)monsterGrid.X == (int)ownerGrid.X && (int)monsterGrid.Y == (int)ownerGrid.Y)
					{
						return false;
					}
				}
			}
			int mapCodeMod = monster.MonsterZoneNode.MapCode / 1000;
			bool result;
			if (6 == mapCodeMod || 5 == mapCodeMod || 7 == mapCodeMod)
			{
				result = false;
			}
			else if (monster.IsMoving)
			{
				result = false;
			}
			else
			{
				if (ticks - monster.LastInObsJugeTicks >= 3000L)
				{
					monster.LastInObsJugeTicks = ticks;
					if (Global.InObs(ObjectTypes.OT_MONSTER, monster.MonsterZoneNode.MapCode, (int)monster.Coordinate.X, (int)monster.Coordinate.Y, 1, 0))
					{
						Point grid = monster.CurrentGrid;
						bool toMove = true;
						if (monster.CopyMapID > 0)
						{
							if (ChuanQiUtils.CanMonsterMoveOnCopyMap(monster, (int)grid.X, (int)grid.Y))
							{
								toMove = false;
							}
						}
						if (toMove)
						{
							int nCurrX = (int)grid.X;
							int nCurrY = (int)grid.Y;
							ChuanQiUtils.WalkTo(monster, (Dircetions)Global.GetRandomNumber(0, 8));
							return true;
						}
						return false;
					}
				}
				result = false;
			}
			return result;
		}

		
		private void SearchViewRange(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int rolesNum)
		{
			if (ticks - monster.LastSeekEnemyTicks >= monster.NextSeekEnemyTicks)
			{
				monster.LastSeekEnemyTicks = ticks;
				if (rolesNum > 0 || monster.AllwaySearchEnemy)
				{
					GameManager.ClientMgr.SeekSpriteToLock(monster);
				}
			}
		}

		
		private void SelectTarget(Monster monster, IObject obj, long ticks)
		{
			monster.LockObject = obj.GetObjectID();
			monster.LockFocusTime = ticks;
		}

		
		public void LoseTarget(Monster monster)
		{
			monster.LockObject = -1;
			monster.LockFocusTime = 0L;
			monster.PetLockObjectPriority = 0;
		}

		
		private bool CanLock(Monster monster, IObject obj)
		{
			bool result;
			if (!Global.IsOpposition(monster, obj))
			{
				result = false;
			}
			else
			{
				GameClient targetClient = obj as GameClient;
				Monster targetMonster = obj as Monster;
				if (null != targetMonster)
				{
					targetClient = targetMonster.OwnerClient;
				}
				if (targetMonster == null && null != targetClient)
				{
					if (!Global.RoleIsVisible(targetClient))
					{
						return false;
					}
				}
				if (1001 != monster.MonsterType)
				{
					if (monster.DynamicMonster)
					{
						if (monster.DynamicPursuitRadius > 0)
						{
							if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
							{
								return false;
							}
						}
					}
					else if (monster.MonsterZoneNode.PursuitRadius > 0)
					{
						if (Global.GetTwoPointDistance(obj.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
						{
							return false;
						}
					}
				}
				int monsterType = monster.MonsterType;
				if (monsterType != 1001)
				{
					if (monsterType == 1201)
					{
						if (targetClient != null && !Global.IsRedName(targetClient) && !monster.IsAttackedBy(targetClient.ClientData.RoleID))
						{
							return false;
						}
						if (targetClient != null && obj is GameClient)
						{
						}
					}
				}
				else if (null != monster.OwnerClient)
				{
					if (2 == monster.PetAiControlType)
					{
						int curTargetPriority = 0;
						if (-1 != monster.LockObject)
						{
							curTargetPriority = monster.PetLockObjectPriority;
						}
						int objPriority = this.CalculatePetAttackMasterTargetPriority(monster, obj);
						if (objPriority == 0 || (monster.LockObject != obj.GetObjectID() && objPriority <= curTargetPriority && objPriority != 3))
						{
							return false;
						}
						if (monster.PetLockObjectPriority <= objPriority)
						{
							monster.PetLockObjectPriority = objPriority;
						}
					}
					else if (1 == monster.PetAiControlType)
					{
						if (obj is GameClient)
						{
							if (!Global.IsInBattle(monster.OwnerClient, obj))
							{
								return false;
							}
						}
						if (obj is Monster && (obj as Monster).MonsterType == 1201)
						{
							if (!Global.IsInBattle(monster.OwnerClient, obj))
							{
								return false;
							}
						}
					}
				}
				result = true;
			}
			return result;
		}

		
		public void PetAttackMasterTargetTriggerEvent(Monster monster, IObject obj)
		{
			if (1001 == monster.MonsterType && this.CanLock(monster, obj))
			{
				this.SelectTarget(monster, obj, TimeUtil.NOW());
			}
			else if (monster.CallMonster != null && this.CanLock(monster.CallMonster, obj))
			{
				this.SelectTarget(monster.CallMonster, obj, TimeUtil.NOW());
			}
		}

		
		public void PetAttackMasterTargetTriggerEvent(GameClient client, IObject obj)
		{
			Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
			if (monster != null && monster.Alive)
			{
				this.PetAttackMasterTargetTriggerEvent(monster, obj);
			}
		}

		
		private int CalculatePetAttackMasterTargetPriority(Monster monster, IObject obj)
		{
			int result;
			if (obj == null || (monster.OwnerClient == null && null == monster.OwnerMonster))
			{
				result = 0;
			}
			else
			{
				int objRoleID = -1;
				int objLockObject = -1;
				GameClient objGameClient = null;
				Monster objMonster = null;
				JunQiItem objJunQi = null;
				if (obj is GameClient)
				{
					objGameClient = (obj as GameClient);
					objRoleID = objGameClient.ClientData.RoleID;
					objLockObject = objGameClient.ClientData.RoleIDAttackebByMyself;
				}
				else if (obj is Monster)
				{
					objMonster = (obj as Monster);
					objRoleID = objMonster.RoleID;
					objLockObject = objMonster.LockObject;
				}
				else if (obj is JunQiItem)
				{
					objJunQi = (obj as JunQiItem);
					objRoleID = objJunQi.JunQiID;
				}
				if (null != monster.OwnerClient)
				{
					if (monster.OwnerClient.InSafeRegion)
					{
						return 0;
					}
					if (monster.OwnerClient.ClientData.RoleIDAttackebByMyself == objRoleID)
					{
						return 3;
					}
					if (objGameClient != null && !Global.IsOpposition(monster.OwnerClient, objGameClient))
					{
						return 0;
					}
					if (objMonster != null && !Global.IsOpposition(monster.OwnerClient, objMonster))
					{
						return 0;
					}
					if (objJunQi != null && !Global.IsOpposition(monster.OwnerClient, objJunQi))
					{
						return 0;
					}
					if (objLockObject == monster.OwnerClient.ClientData.RoleID || objRoleID == monster.OwnerClient.ClientData.RoleIDAttackMe)
					{
						return 2;
					}
					if (objLockObject == monster.RoleID || monster.IsAttackedBy(objRoleID))
					{
						return 1;
					}
				}
				else
				{
					if (monster.OwnerMonster.LockObject == objRoleID)
					{
						return 3;
					}
					if (objLockObject == monster.OwnerMonster.RoleID)
					{
						return 2;
					}
					if (objLockObject == monster.RoleID || monster.IsAttackedBy(objRoleID))
					{
						return 1;
					}
				}
				result = 0;
			}
			return result;
		}

		
		private void TryToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			bool IfTryLock = ticks - monster.LastLockEnemyTicks > 8000L || (ticks - monster.LastLockEnemyTicks > 1000L && -1 == monster.LockObject);
			if (IfTryLock && (1001 != monster.MonsterType || 2 != monster.PetAiControlType))
			{
				monster.LastLockEnemyTicks = ticks;
				if (null != monster.VisibleItemList)
				{
					int i = 0;
					while (i < monster.VisibleItemList.Count)
					{
						if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);
							if (null != gameClient)
							{
								if (!this.CanLock(monster, gameClient))
								{
									goto IL_26A;
								}
								Point monsterGrid = monster.CurrentGrid;
								int nCurrX = (int)monsterGrid.X;
								int nCurrY = (int)monsterGrid.Y;
								Point defenserGrid = gameClient.CurrentGrid;
								int nTargetCurrX = (int)defenserGrid.X;
								int nTargetCurrY = (int)defenserGrid.Y;
								if (Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY) < 999)
								{
									this.SelectTarget(monster, gameClient, ticks);
									break;
								}
							}
							goto IL_156;
						}
						goto IL_156;
						IL_26A:
						i++;
						continue;
						IL_156:
						if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
						{
							Monster enemyMonster = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);
							if (this.CanLock(monster, enemyMonster))
							{
								if (enemyMonster != null && enemyMonster.CurrentCopyMapID == monster.CurrentCopyMapID)
								{
									if (enemyMonster.OwnerClient == null || enemyMonster.OwnerClient != monster.OwnerClient)
									{
										Point monsterGrid = monster.CurrentGrid;
										int nCurrX = (int)monsterGrid.X;
										int nCurrY = (int)monsterGrid.Y;
										Point defenserGrid = enemyMonster.CurrentGrid;
										int nTargetCurrX = (int)defenserGrid.X;
										int nTargetCurrY = (int)defenserGrid.Y;
										if (Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY) < 999)
										{
											this.SelectTarget(monster, enemyMonster, ticks);
											break;
										}
									}
								}
							}
						}
						goto IL_26A;
					}
				}
			}
		}

		
		private bool IsLockObjectValid(Monster monster, GameClient gameClient, long ticks)
		{
			bool result;
			if (null == gameClient)
			{
				result = false;
			}
			else if (gameClient.ClientData.CurrentLifeV <= 0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point monsterGrid = monster.CurrentGrid;
				int nCurrX = (int)monsterGrid.X;
				int nCurrY = (int)monsterGrid.Y;
				Point defenserGrid = gameClient.CurrentGrid;
				int nTargetCurrX = (int)defenserGrid.X;
				int nTargetCurrY = (int)defenserGrid.Y;
				result = (Math.Abs(nTargetCurrX - nCurrX) <= 12 && Math.Abs(nTargetCurrY - nCurrY) <= 12);
			}
			return result;
		}

		
		private bool IsLockObjectValid(Monster monster, Monster targetMonster, long ticks)
		{
			bool result;
			if (null == targetMonster)
			{
				result = false;
			}
			else if (targetMonster.VLife <= 0.0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point monsterGrid = monster.CurrentGrid;
				int nCurrX = (int)monsterGrid.X;
				int nCurrY = (int)monsterGrid.Y;
				Point defenserGrid = targetMonster.CurrentGrid;
				int nTargetCurrX = (int)defenserGrid.X;
				int nTargetCurrY = (int)defenserGrid.Y;
				result = (Math.Abs(nTargetCurrX - nCurrX) <= 12 && Math.Abs(nTargetCurrY - nCurrY) <= 12);
			}
			return result;
		}

		
		private bool IsLockObjectValid(Monster monster, JunQiItem targetJunQi, long ticks)
		{
			bool result;
			if (null == targetJunQi)
			{
				result = false;
			}
			else if (targetJunQi.CurrentLifeV <= 0)
			{
				result = false;
			}
			else if (ticks - monster.LockFocusTime > 30000L)
			{
				result = false;
			}
			else
			{
				Point monsterGrid = monster.CurrentGrid;
				int nCurrX = (int)monsterGrid.X;
				int nCurrY = (int)monsterGrid.Y;
				Point defenserGrid = targetJunQi.CurrentGrid;
				int nTargetCurrX = (int)defenserGrid.X;
				int nTargetCurrY = (int)defenserGrid.Y;
				result = (Math.Abs(nTargetCurrX - nCurrX) <= 12 && Math.Abs(nTargetCurrY - nCurrY) <= 12);
			}
			return result;
		}

		
		private bool IsLockObjectValid(Monster monster, IObject lockObject, long ticks)
		{
			bool result;
			if (lockObject is GameClient)
			{
				result = this.IsLockObjectValid(monster, lockObject as GameClient, ticks);
			}
			else if (lockObject is Monster)
			{
				result = this.IsLockObjectValid(monster, lockObject as Monster, ticks);
			}
			else
			{
				result = (lockObject is JunQiItem && this.IsLockObjectValid(monster, lockObject as JunQiItem, ticks));
			}
			return result;
		}

		
		private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject lockObject, long ticks)
		{
			bool result;
			if (!this.CanLock(monster, lockObject))
			{
				this.LoseTarget(monster);
				result = false;
			}
			else if (!this.IsLockObjectValid(monster, lockObject, ticks))
			{
				this.LoseTarget(monster);
				if (monster._Action != GActions.Stand)
				{
					List<object> listObjs = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, listObjs);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		private bool CheckLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, Monster targetMonster, long ticks)
		{
			bool result;
			if (!this.IsLockObjectValid(monster, targetMonster, ticks))
			{
				this.LoseTarget(monster);
				if (monster._Action != GActions.Stand)
				{
					List<object> listObjs = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, listObjs);
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Action = GActions.Stand;
				}
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		
		private Dircetions GetWonderingWalkDir(Monster monster)
		{
			monster.CurrentDir = (Dircetions)Global.GetRandomNumber(0, 8);
			return monster.CurrentDir;
		}

		
		private void Wondering(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			if (1001 == monster.MonsterType)
			{
				if (null != monster.OwnerClient)
				{
					Point monsterGrid = monster.CurrentGrid;
					Point clientGrid = monster.OwnerClient.CurrentGrid;
					if ((int)monsterGrid.X == (int)clientGrid.X && (int)monsterGrid.Y == (int)clientGrid.Y)
					{
						return;
					}
				}
				if (null != monster.OwnerMonster)
				{
					Point monsterGrid = monster.CurrentGrid;
					Point ownerGrid = monster.OwnerMonster.CurrentGrid;
					if ((int)monsterGrid.X == (int)ownerGrid.X && (int)monsterGrid.Y == (int)ownerGrid.Y)
					{
						return;
					}
				}
			}
			if (monster.LockObject < 0)
			{
				if (MonsterManager.CanMonsterSeekRange(monster))
				{
					if (1001 != monster.MonsterType)
					{
						if (monster.DynamicMonster)
						{
							if (monster.DynamicPursuitRadius > 0)
							{
								if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
								{
									Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
									ChuanQiUtils.WalkTo(monster, nDir);
									return;
								}
							}
						}
						else if (monster.MonsterZoneNode.PursuitRadius > 0)
						{
							if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
							{
								Dircetions nDir = (Dircetions)Global.GetDirectionByTan(monster.FirstCoordinate.X, monster.FirstCoordinate.Y, monster.CurrentPos.X, monster.CurrentPos.Y);
								ChuanQiUtils.WalkTo(monster, nDir);
								return;
							}
						}
					}
					if (Global.GetRandomNumber(0, 10) == 0)
					{
						if (monster.MoveSpeed <= 0.0)
						{
							ChuanQiUtils.TurnTo(monster, (Dircetions)Global.GetRandomNumber(0, 8));
						}
						else if (Global.GetRandomNumber(0, 4) != 0)
						{
							Dircetions nDir = this.GetWonderingWalkDir(monster);
							ChuanQiUtils.WalkTo(monster, nDir);
						}
					}
				}
			}
		}

		
		private void DoMonsterStandAction(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks)
		{
			List<object> listObjs = Global.GetAll9Clients(monster);
			GameManager.ClientMgr.NotifyOthersDoAction(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeDirection, 0, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, 0, 0, 114, listObjs);
			monster.DestPoint = new Point(-1.0, -1.0);
			Global.RemoveStoryboard(monster.Name);
			monster.Action = GActions.Stand;
		}

		
		private void DoMonsterAI(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int count, int IndexOfMonsterAiAttack)
		{
			if (monster._Action == GActions.Attack)
			{
				this.DoMonsterStandAction(sl, pool, monster, ticks);
			}
			if (501 != monster.MonsterType && 601 != monster.MonsterType)
			{
				if (2000 != monster.MonsterType && 2001 != monster.MonsterType && 1601 != monster.MonsterType && 1701 != monster.MonsterType && 1501 != monster.MonsterType && 1001 != monster.MonsterType && 1502 != monster.MonsterType)
				{
					if (!monster.isReturn)
					{
						this.Wondering(sl, pool, monster, ticks);
					}
					else if (monster.CurrentGrid.X == monster.getFirstGrid().X && monster.CurrentGrid.Y == monster.getFirstGrid().Y)
					{
						monster.isReturn = false;
					}
					else
					{
						this.MonsterReturn(monster);
					}
				}
			}
		}

		
		private void MonsterReturn(Monster monster)
		{
			int nDir = (int)monster.CurrentDir;
			int nCurrX = (int)monster.CurrentGrid.X;
			int nCurrY = (int)monster.CurrentGrid.Y;
			int nX = (int)monster.getFirstGrid().X;
			int nY = (int)monster.getFirstGrid().Y;
			if (nCurrX != nX || nCurrY != nY)
			{
				if (nX > nCurrX)
				{
					nDir = 2;
					if (nY > nCurrY)
					{
						nDir = 1;
					}
					else if (nY < nCurrY)
					{
						nDir = 3;
					}
				}
				else if (nX < nCurrX)
				{
					nDir = 6;
					if (nY > nCurrY)
					{
						nDir = 7;
					}
					else if (nY < nCurrY)
					{
						nDir = 5;
					}
				}
				else if (nY > nCurrY)
				{
					nDir = 0;
				}
				else if (nY < nCurrY)
				{
					nDir = 4;
				}
				ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);
			}
		}

		
		private void GoToLockObject(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject obj, long ticks, bool justMove = false)
		{
			if (!monster.IsMoving)
			{
				if (monster.Action != GActions.Attack)
				{
					if (monster.MoveSpeed > 0.0)
					{
						if (!monster.IsMonsterDongJie())
						{
							int minTicks = 0;
							int mapCodeMod = monster.MonsterZoneNode.MapCode / 1000;
							if (6 == mapCodeMod)
							{
								minTicks = 1000;
							}
							else if (5 == mapCodeMod || 7 == mapCodeMod)
							{
								minTicks = 100;
							}
							if (1001 == monster.MonsterType)
							{
								minTicks = 0;
							}
							if (ticks - monster.LastActionTick >= (long)minTicks)
							{
								if (1001 != monster.MonsterType)
								{
									if (monster.DynamicMonster)
									{
										if (monster.DynamicPursuitRadius > 0)
										{
											if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.DynamicPursuitRadius)
											{
												this.LoseTarget(monster);
												return;
											}
										}
									}
									else if (monster.MonsterZoneNode.PursuitRadius > 0)
									{
										if (Global.GetTwoPointDistance(monster.CurrentPos, monster.FirstCoordinate) >= (double)monster.MonsterZoneNode.PursuitRadius)
										{
											this.LoseTarget(monster);
											return;
										}
									}
								}
								else
								{
									justMove = true;
								}
								Point monsterGrid = monster.CurrentGrid;
								int nCurrX = (int)monsterGrid.X;
								int nCurrY = (int)monsterGrid.Y;
								Point objGrid = obj.CurrentGrid;
								int nTargetCurrX = (int)objGrid.X;
								int nTargetCurrY = (int)objGrid.Y;
								int nDir = (int)monster.Direction;
								if (nCurrX != nTargetCurrX || nCurrY != nTargetCurrY)
								{
									if (monster.VisibleItemList != null && !justMove)
									{
										int i = 0;
										while (i < monster.VisibleItemList.Count)
										{
											if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_CLIENT)
											{
												GameClient gameClient = GameManager.ClientMgr.FindClient(monster.VisibleItemList[i].ItemID);
												if (null != gameClient)
												{
													if (!Global.IsOpposition(monster, gameClient))
													{
														goto IL_3E1;
													}
													if (gameClient.ClientData.CurrentLifeV > 0)
													{
														Point clientGrid = gameClient.CurrentGrid;
														int nNewTargetCurrX = (int)clientGrid.X;
														int nNewTargetCurrY = (int)clientGrid.Y;
														if (Math.Abs(nCurrX - nNewTargetCurrX) + Math.Abs(nCurrY - nNewTargetCurrY) < Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY))
														{
															this.SelectTarget(monster, gameClient, ticks);
															return;
														}
													}
												}
												goto IL_2F7;
											}
											goto IL_2F7;
											IL_3E1:
											i++;
											continue;
											IL_2F7:
											if (monster.VisibleItemList[i].ItemType == ObjectTypes.OT_MONSTER)
											{
												Monster targetMonster = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.VisibleItemList[i].ItemID);
												if (null != targetMonster)
												{
													if (Global.IsOpposition(monster, targetMonster))
													{
														if (targetMonster.VLife > 0.0)
														{
															Point clientGrid = targetMonster.CurrentGrid;
															int nNewTargetCurrX = (int)clientGrid.X;
															int nNewTargetCurrY = (int)clientGrid.Y;
															if (Math.Abs(nCurrX - nNewTargetCurrX) + Math.Abs(nCurrY - nNewTargetCurrY) < Math.Abs(nCurrX - nTargetCurrX) + Math.Abs(nCurrY - nTargetCurrY))
															{
																this.SelectTarget(monster, targetMonster, ticks);
																return;
															}
														}
													}
												}
											}
											goto IL_3E1;
										}
									}
									int nX = nTargetCurrX;
									int nY = nTargetCurrY;
									if (nX > nCurrX)
									{
										nDir = 2;
										if (nY > nCurrY)
										{
											nDir = 1;
										}
										else if (nY < nCurrY)
										{
											nDir = 3;
										}
									}
									else if (nX < nCurrX)
									{
										nDir = 6;
										if (nY > nCurrY)
										{
											nDir = 7;
										}
										else if (nY < nCurrY)
										{
											nDir = 5;
										}
									}
									else if (nY > nCurrY)
									{
										nDir = 0;
									}
									else if (nY < nCurrY)
									{
										nDir = 4;
									}
									int nOldX = nCurrX;
									int nOldY = nCurrY;
									if (monster.OwnerClient != null || monster.OwnerMonster != null)
									{
										Debug.WriteLine(string.Format("ChuanQiUtils.RunTo1, dir={0}, tick={1}", nDir, ticks % 10000L));
										ChuanQiUtils.RunTo1(monster, (Dircetions)nDir);
									}
									else
									{
										ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);
									}
									monsterGrid = monster.CurrentGrid;
									nCurrX = (int)monsterGrid.X;
									nCurrY = (int)monsterGrid.Y;
									for (int i = 0; i < 7; i++)
									{
										if (nOldX != nCurrX || nOldY != nCurrY)
										{
											break;
										}
										if (Global.GetRandomNumber(0, 3) > 0)
										{
											nDir++;
										}
										else if (nDir > 0)
										{
											nDir--;
										}
										else
										{
											nDir = 7;
										}
										if (nDir > 7)
										{
											nDir = 0;
										}
										ChuanQiUtils.WalkTo(monster, (Dircetions)nDir);
										monsterGrid = monster.CurrentGrid;
										nCurrX = (int)monsterGrid.X;
										nCurrY = (int)monsterGrid.Y;
									}
								}
							}
						}
					}
				}
			}
		}

		
		public bool TargetInAttackRange(Monster attacker, IObject defenser, out int direction)
		{
			direction = (int)attacker.Direction;
			bool result;
			if (defenser == null)
			{
				result = false;
			}
			else if (attacker.MonsterZoneNode.MapCode != defenser.CurrentMapCode)
			{
				result = false;
			}
			else
			{
				Point defenserGrid = defenser.CurrentGrid;
				int nTargetCurrX = (int)defenserGrid.X;
				int nTargetCurrY = (int)defenserGrid.Y;
				Point monsterGrid = attacker.CurrentGrid;
				int nCurrX = (int)monsterGrid.X;
				int nCurrY = (int)monsterGrid.Y;
				if (nTargetCurrX == nCurrX && nTargetCurrY == nCurrY && attacker.MoveSpeed > 0.0)
				{
					ChuanQiUtils.WalkTo(attacker, (Dircetions)Global.GetRandomNumber(0, 8));
					result = false;
				}
				else
				{
					int autoUseSkillID = attacker.GetAutoUseSkillID();
					int attackNum = this.GetSkillAttackGridNum(attacker, autoUseSkillID);
					if ((double)attackNum >= Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100.0)
					{
						result = true;
					}
					else
					{
						bool bCanGo = false;
						if (attackNum <= 2)
						{
							bCanGo = true;
						}
						if (!bCanGo && (double)attackNum < Global.GetTwoPointDistance(attacker.CurrentPos, defenser.CurrentPos) / 100.0)
						{
							attackNum--;
							bCanGo = true;
						}
						if (bCanGo)
						{
							int verifyDirection = (int)Global.GetDirectionByAspect(nTargetCurrX, nTargetCurrY, nCurrX, nCurrY);
							List<Point> gridList = Global.GetGridPointByDirection(verifyDirection, nCurrX, nCurrY, attackNum);
							for (int i = 0; i < gridList.Count; i++)
							{
								if (nTargetCurrX == (int)gridList[i].X && nTargetCurrY == (int)gridList[i].Y)
								{
									return true;
								}
							}
							result = false;
						}
						else
						{
							if (nTargetCurrX >= nCurrX - attackNum && nTargetCurrX <= nCurrX + attackNum && nTargetCurrY >= nCurrY - attackNum && nTargetCurrY <= nCurrY + attackNum)
							{
								if (nTargetCurrX < nCurrX && nTargetCurrY == nCurrY)
								{
									direction = 6;
									return true;
								}
								if (nTargetCurrX > nCurrX && nTargetCurrY == nCurrY)
								{
									direction = 2;
									return true;
								}
								if (nTargetCurrX == nCurrX && nTargetCurrY < nCurrY)
								{
									direction = 4;
									return true;
								}
								if (nTargetCurrX == nCurrX && nTargetCurrY > nCurrY)
								{
									direction = 0;
									return true;
								}
								if (nTargetCurrX < nCurrX && nTargetCurrY < nCurrY)
								{
									direction = 5;
									return true;
								}
								if (nTargetCurrX > nCurrX && nTargetCurrY < nCurrY)
								{
									direction = 3;
									return true;
								}
								if (nTargetCurrX < nCurrX && nTargetCurrY > nCurrY)
								{
									direction = 7;
									return true;
								}
								if (nTargetCurrX > nCurrX && nTargetCurrY > nCurrY)
								{
									direction = 1;
									return true;
								}
							}
							result = false;
						}
					}
				}
			}
			return result;
		}

		
		public void MonsterAttack(SocketListener sl, TCPOutPacketPool pool, Monster monster, IObject enemyObject, int direction, long ticks)
		{
			if (!monster.IsMoving)
			{
				if (null != enemyObject)
				{
					Point enemyPos = enemyObject.CurrentPos;
					bool doAttackNow = false;
					if (monster._Action != GActions.Attack && monster.Action != GActions.PreAttack)
					{
						if (monster._ToExecSkillID > 0)
						{
							doAttackNow = true;
						}
						else if (ticks - monster.LastAttackActionTick >= monster.MaxAttackTimeSlot)
						{
							doAttackNow = true;
						}
					}
					else if (monster._Action == GActions.PreAttack || monster._Action == GActions.Stand)
					{
						double newDirection = this.monsterMoving.CalcDirection(monster, enemyPos);
						if (newDirection != monster.SafeDirection && monster.CurrentMagic < 1)
						{
							direction = (int)newDirection;
							doAttackNow = true;
						}
						else if (monster.EnemyTarget != enemyPos && monster.CurrentMagic < 1)
						{
							doAttackNow = true;
						}
					}
					if (doAttackNow)
					{
						this.InstantAttack(monster, (double)direction, enemyPos);
					}
				}
			}
		}

		
		private int GetMapRolesCount(Dictionary<int, int> dict, int mapCode)
		{
			int roleNum = 0;
			int result;
			if (dict.TryGetValue(mapCode, out roleNum))
			{
				result = roleNum;
			}
			else
			{
				roleNum = GameManager.ClientMgr.GetMapClientsCount(mapCode);
				dict[mapCode] = roleNum;
				result = roleNum;
			}
			return result;
		}

		
		public void DoMonsterAttack(SocketListener sl, TCPOutPacketPool pool, int IndexOfMonsterAiAttack, int mapCode = -1, int subMapCode = -1)
		{
			Dictionary<int, int> mapRolesNumDict = new Dictionary<int, int>();
			int count = 0;
			long ticks = TimeUtil.NOW();
			Monster monster = null;
			List<object> objectsList = this.MyMonsterContainer._ObjectList;
			if (mapCode != -1)
			{
				objectsList = this.MyMonsterContainer.GetObjectsByMap(mapCode, subMapCode);
			}
			if (objectsList != null && objectsList.Count > 0)
			{
				int nIndex = 0;
				while (nIndex < objectsList.Count)
				{
					try
					{
						monster = (objectsList[nIndex] as Monster);
					}
					catch (Exception)
					{
						goto IL_4EA;
					}
					goto IL_7D;
					IL_4EA:
					nIndex++;
					continue;
					IL_7D:
					if (null == monster)
					{
						goto IL_4EA;
					}
					if (monster.MonsterType == 1801)
					{
						goto IL_4EA;
					}
					count++;
					if (monster.VLife <= 0.0 || !monster.Alive)
					{
						goto IL_4EA;
					}
					if (monster.OwnerClient != null)
					{
						if (this.DispatchMonsterOwnedByRole(monster, sl, pool, ticks))
						{
							goto IL_4EA;
						}
					}
					if (monster.OwnerMonster != null)
					{
						if (this.DispatchMonsterOwnedByMonster(monster, sl, pool, ticks))
						{
							goto IL_4EA;
						}
					}
					if (monster.ManagerType == SceneUIClasses.EMoLaiXiCopy)
					{
						EMoLaiXiCopySceneManager.MonsterMoveStepEMoLaiXiCopySenceCopyMap(monster);
						goto IL_4EA;
					}
					if (monster.MonsterType == 1501)
					{
						GlodCopySceneManager.MonsterMoveStepGoldCopySceneCopyMap(monster);
						goto IL_4EA;
					}
					if (monster.MonsterType == 1502)
					{
						CompMineManager.getInstance().MonsterMoveStep(monster);
						goto IL_4EA;
					}
					int rolesNum = this.GetMapRolesCount(mapRolesNumDict, monster.MonsterZoneNode.MapCode);
					this.DoMonsterLifeMagicV(sl, pool, monster, ticks, rolesNum);
					if (rolesNum <= 0 && !monster.AllwaySearchEnemy)
					{
						this.LoseTarget(monster);
						monster.VisibleClientsNum = 0;
						if (monster.IsAutoSearchRoad)
						{
							this.MonsterAutoSearchRoad(monster);
							goto IL_4EA;
						}
						goto IL_4EA;
					}
					else
					{
						if (2000 == monster.MonsterType || 2001 == monster.MonsterType)
						{
							goto IL_4EA;
						}
						SpriteAttack.ExecMagicsManyTimeDmageQueueEx(monster);
						if (monster.VisibleClientsNum <= 0 && !monster.AllwaySearchEnemy)
						{
							if (monster.IsAutoSearchRoad)
							{
								this.MonsterAutoSearchRoad(monster);
								goto IL_4EA;
							}
							this.LoseTarget(monster);
							goto IL_4EA;
						}
						else
						{
							GActions action = monster.Action;
							if (GameManager.FlagDisableMovingOnAttack)
							{
								if (action == GActions.Attack || action == GActions.Magic || action == GActions.Bow)
								{
									goto IL_4EA;
								}
							}
							if (this.CheckMonsterInObs(sl, pool, monster, ticks))
							{
								goto IL_4EA;
							}
							this.SearchViewRange(sl, pool, monster, ticks, rolesNum);
							this.TryToLockObject(sl, pool, monster, ticks);
							if (monster.MonsterInfo.SeekRange > 0 && null == monster.VisibleItemList)
							{
								this.LoseTarget(monster);
							}
							if (-1 == monster.LockObject && monster.IsAutoSearchRoad)
							{
								this.MonsterAutoSearchRoad(monster);
								goto IL_4EA;
							}
							if (-1 == monster.LockObject || (501 == monster.MonsterType || 601 == monster.MonsterType || 701 == monster.MonsterType || 801 == monster.MonsterType || 901 == monster.MonsterType || 1601 == monster.MonsterType || 1701 == monster.MonsterType || 1501 == monster.MonsterType) || 1502 == monster.MonsterType)
							{
								this.DoMonsterAI(sl, pool, monster, ticks, count, IndexOfMonsterAiAttack);
								goto IL_4EA;
							}
							IObject lockObject = GameManager.ClientMgr.FindClient(monster.LockObject);
							if (null == lockObject)
							{
								lockObject = GameManager.MonsterMgr.FindMonster(monster.CurrentMapCode, monster.LockObject);
							}
							if (null == lockObject)
							{
								lockObject = JunQiManager.FindJunQiByID(monster.LockObject);
							}
							if (null == lockObject)
							{
								this.LoseTarget(monster);
								goto IL_4EA;
							}
							if (!this.CheckLockObject(sl, pool, monster, lockObject, ticks))
							{
								goto IL_4EA;
							}
							if (monster.IsMoving)
							{
								goto IL_4EA;
							}
							monster.isReturn = true;
							if (monster.MonsterType != 401)
							{
							}
							int direction = 0;
							if (!this.TargetInAttackRange(monster, lockObject, out direction))
							{
								if (monster.IsAutoSearchRoad)
								{
									this.MonsterAutoSearchRoad(monster);
								}
								else
								{
									this.GoToLockObject(sl, pool, monster, lockObject, ticks, false);
								}
							}
							else
							{
								this.MonsterAttack(sl, pool, monster, lockObject, direction, ticks);
							}
							goto IL_4EA;
						}
					}
				}
			}
		}

		
		public void MonsterAutoSearchRoad(Monster monster)
		{
			long ticks = TimeUtil.NOW();
			if (ticks - monster.MoveTime >= 500L)
			{
				int nStep = monster.Step;
				if (null != monster.PatrolPath)
				{
					int nNumStep = monster.PatrolPath.Count<int[]>() - 1;
					if (nStep >= nNumStep)
					{
						monster.Step = nNumStep - 1;
						nStep = monster.Step;
					}
					int nNextStep = nStep + 1;
					int nMapCode = monster.CurrentMapCode;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[nMapCode];
					int nNextX = monster.PatrolPath[nNextStep][0];
					int nNextY = monster.PatrolPath[nNextStep][1];
					int gridX = nNextX;
					int gridY = nNextY;
					Point ToGrid = new Point((double)gridX, (double)gridY);
					Point grid = monster.CurrentGrid;
					int nCurrX = (int)grid.X;
					int nCurrY = (int)grid.Y;
					double Direction = Global.GetDirectionByAspect(gridX, gridY, nCurrX, nCurrY);
					if (ChuanQiUtils.WalkTo(monster, (Dircetions)Direction) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 7.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 9.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 6.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 10.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 5.0) % 8.0)) || ChuanQiUtils.WalkTo(monster, (Dircetions)((Direction + 11.0) % 8.0)))
					{
						monster.MoveTime = ticks;
					}
					if (Global.GetTwoPointDistance(ToGrid, grid) < 2.0)
					{
						monster.Step = nStep + 1;
					}
				}
			}
		}

		
		public bool DispatchMonsterOwnedByRole(Monster monster, SocketListener sl, TCPOutPacketPool pool, long ticks)
		{
			if (monster.OwnerClient != null)
			{
				if (monster.CurrentMapCode != monster.OwnerClient.CurrentMapCode || monster.CurrentCopyMapID != monster.OwnerClient.CurrentCopyMapID)
				{
					return !monster.OwnerClient.ClientData.WaitingNotifyChangeMap || true;
				}
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) >= 8.0 || Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) >= 8.0)
				{
					this.LoseTarget(monster);
					ChuanQiUtils.TransportTo(monster, (int)monster.OwnerClient.CurrentGrid.X, (int)monster.OwnerClient.CurrentGrid.Y, (Dircetions)monster.Direction, -1, "");
					return true;
				}
				if (monster.IsMoving)
				{
					return true;
				}
				if (monster.LockObject > 0)
				{
					return false;
				}
				bool allowGo = true;
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerClient.CurrentGrid.X) <= 2.0 && Math.Abs(monster.CurrentGrid.Y - monster.OwnerClient.CurrentGrid.Y) <= 2.0)
				{
					if (Global.GetRandomNumber(0, 10001) >= 100)
					{
						allowGo = false;
					}
				}
				if (allowGo)
				{
					this.GoToLockObject(sl, pool, monster, monster.OwnerClient, ticks, true);
					return true;
				}
			}
			return false;
		}

		
		public bool DispatchMonsterOwnedByMonster(Monster monster, SocketListener sl, TCPOutPacketPool pool, long ticks)
		{
			if (monster.OwnerMonster != null)
			{
				if (monster.CurrentMapCode != monster.OwnerMonster.CurrentMapCode || monster.CurrentCopyMapID != monster.OwnerMonster.CurrentCopyMapID)
				{
					return true;
				}
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerMonster.CurrentGrid.X) >= 8.0 || Math.Abs(monster.CurrentGrid.Y - monster.OwnerMonster.CurrentGrid.Y) >= 8.0)
				{
					this.LoseTarget(monster);
					ChuanQiUtils.TransportTo(monster, (int)monster.OwnerMonster.CurrentGrid.X, (int)monster.OwnerMonster.CurrentGrid.Y, (Dircetions)monster.Direction, -1, "");
					return true;
				}
				if (monster.IsMoving)
				{
					return true;
				}
				if (monster.LockObject > 0)
				{
					return false;
				}
				bool allowGo = true;
				if (Math.Abs(monster.CurrentGrid.X - monster.OwnerMonster.CurrentGrid.X) <= 2.0 && Math.Abs(monster.CurrentGrid.Y - monster.OwnerMonster.CurrentGrid.Y) <= 2.0)
				{
					if (Global.GetRandomNumber(0, 10001) >= 100)
					{
						allowGo = false;
					}
				}
				if (allowGo)
				{
					this.GoToLockObject(sl, pool, monster, monster.OwnerMonster, ticks, true);
					return true;
				}
			}
			return false;
		}

		
		public void DoMonsterLifeMagicV(SocketListener sl, TCPOutPacketPool pool, Monster monster, long ticks, int mapRoleNum)
		{
			DBMonsterBuffer.ProcessDSTimeAddLifeNoShow(monster);
			DBMonsterBuffer.ProcessDSTimeSubLifeNoShow(monster);
			DBMonsterBuffer.ProcessAllTimeSubLifeNoShow(monster);
			if (ticks - monster.LastLifeMagicTick >= 10000L)
			{
				monster.LastLifeMagicTick = ticks;
				bool doRelife = false;
				if (monster.VLife < monster.MonsterInfo.VLifeMax)
				{
					doRelife = true;
					double percent = RoleAlgorithm.GetLifeRecoverValPercentV(monster) + DBMonsterBuffer.ProcessHuZhaoRecoverPercent(monster);
					double lifeMax = percent * monster.MonsterInfo.VLifeMax;
					lifeMax += monster.VLife;
					monster.VLife = Global.GMin(monster.MonsterInfo.VLifeMax, lifeMax);
				}
				if (monster.VMana < monster.MonsterInfo.VManaMax)
				{
					doRelife = true;
					double percent = RoleAlgorithm.GetMagicRecoverValPercentV(monster);
					double magicMax = percent * monster.MonsterInfo.VManaMax;
					magicMax += monster.VMana;
					monster.VMana = Global.GMin(monster.MonsterInfo.VManaMax, magicMax);
				}
				if (doRelife)
				{
					List<object> listObjs = Global.GetAll9Clients(monster);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)monster.SafeDirection, monster.VLife, monster.VMana, 120, listObjs, 0);
					GlobalEventSource.getInstance().fireEvent(new MonsterBlooadChangedEventObject(monster, null, 0));
				}
			}
		}

		
		public void AddDelayDeadMonster(Monster obj)
		{
			lock (this.ListDelayDeadMonster)
			{
				if (this.ListDelayDeadMonster.IndexOf(obj) < 0)
				{
					obj.AddToDeadQueueTicks = TimeUtil.NOW();
					this.ListDelayDeadMonster.Add(obj);
				}
			}
		}

		
		public void DeadMonsterImmediately(Monster obj)
		{
			obj.OnDead();
			this.AddDelayDeadMonster(obj);
		}

		
		public void DoMonsterDeadCall()
		{
			long nowTicks = TimeUtil.NOW();
			List<Monster> lsMonster = new List<Monster>();
			lock (this.ListDelayDeadMonster)
			{
				for (int i = 0; i < this.ListDelayDeadMonster.Count; i++)
				{
					if (101 != this.ListDelayDeadMonster[i].MonsterType)
					{
						if (1801 == this.ListDelayDeadMonster[i].MonsterType || 401 == this.ListDelayDeadMonster[i].MonsterType)
						{
						}
					}
					long maxTicks;
					if (1601 == this.ListDelayDeadMonster[i].MonsterType)
					{
						maxTicks = 0L;
					}
					else
					{
						maxTicks = 1500L;
					}
					if (nowTicks - this.ListDelayDeadMonster[i].AddToDeadQueueTicks >= maxTicks)
					{
						lsMonster.Add(this.ListDelayDeadMonster[i]);
					}
				}
			}
			foreach (Monster monster in lsMonster)
			{
				lock (this.ListDelayDeadMonster)
				{
					this.ListDelayDeadMonster.Remove(monster);
				}
				monster.OnDead();
			}
		}

		
		public bool AddKilledMonsterFirst(long monsterUniqueId)
		{
			return this.DeadMonsterUniqueIdDict.TryAdd(monsterUniqueId, TimeUtil.CurrentTicksInexact + 15000L);
		}

		
		public void DoDeadMonsterUniqueIdProc(long nowTicks)
		{
			foreach (KeyValuePair<long, long> kv in this.DeadMonsterUniqueIdDict)
			{
				if (nowTicks > kv.Value)
				{
					long removedTicks;
					this.DeadMonsterUniqueIdDict.TryRemove(kv.Key, out removedTicks);
				}
			}
		}

		
		protected int GetSkillAttackGridNum(Monster monster, int skillID)
		{
			int result;
			if (skillID < 0)
			{
				result = 1;
			}
			else
			{
				SystemXmlItem systemMagic = null;
				if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagic))
				{
					result = 1;
				}
				else
				{
					int skillType = systemMagic.GetIntValue("SkillType", -1);
					if (1 == skillType)
					{
						result = 1;
					}
					else if (10 == skillType)
					{
						result = 2;
					}
					else
					{
						int nDistance = systemMagic.GetIntValue("AttackDistance", -1);
						if (nDistance > 0)
						{
							result = nDistance / 100;
						}
						else
						{
							result = Global.MaxCache9XGridNum;
						}
					}
				}
			}
			return result;
		}

		
		protected void InstantAttack(Monster monster, double direction, Point enemyPos)
		{
			int autoUseSkillID = monster.GetAutoUseSkillID();
			int attackNum = this.GetSkillAttackGridNum(monster, autoUseSkillID);
			if (autoUseSkillID > 0)
			{
				int nRet = this.DoMagicAttack(monster, autoUseSkillID, monster.LockObject, true);
			}
			if (-1 != attackNum)
			{
				double newDirection = this.monsterMoving.CalcDirection(monster, enemyPos);
				monster.EnemyTarget = enemyPos;
				this.DoAttackAction(monster, newDirection);
				if (monster.MagicFinish == -1)
				{
					List<ManyTimeDmageItem> manyTimeDmageItemList = MagicsManyTimeDmageCachingMgr.GetManyTimeDmageItems(monster.CurrentMagic);
					if (manyTimeDmageItemList != null && manyTimeDmageItemList.Count > 0)
					{
						Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
					}
					else if (monster.CurrentMagic > 0)
					{
						SystemXmlItem systemMagic = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(monster.CurrentMagic, out systemMagic))
						{
							if (systemMagic.GetIntValue("InjureType", -1) == 1)
							{
								Global.DoInjure(monster, monster.LockObject, monster.EnemyTarget);
							}
						}
					}
				}
			}
		}

		
		public void DoAttackAction(Monster monster, double direction)
		{
			if (monster.VLife > 0.0)
			{
				if (monster.IsMonsterDongJie())
				{
					monster.Action = GActions.Stand;
				}
				else
				{
					monster.Action = GActions.Attack;
				}
				Point enemyPos = monster.EnemyTarget;
				List<object> listObjs = Global.GetAll9Clients(monster);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, (int)direction, (int)monster.Action, (int)monster.SafeCoordinate.X, (int)monster.SafeCoordinate.Y, (int)enemyPos.X, (int)enemyPos.Y, 114, listObjs);
				monster.DestPoint = new Point(-1.0, -1.0);
				Global.RemoveStoryboard(monster.Name);
			}
		}

		
		public int DoMagicAttack(Monster monster, int magicCode, int lockObject, bool doAttackAction = false)
		{
			SystemXmlItem systemMagic = null;
			int result;
			if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
			{
				result = -3;
			}
			else
			{
				IObject enemyObject = Global.GetTargetObject(monster.MonsterZoneNode.MapCode, lockObject);
				if (null == enemyObject)
				{
					result = -1;
				}
				else
				{
					monster.EnemyTarget = new Point(-1.0, -1.0);
					IObject targetSprite = null;
					if (1 != systemMagic.GetIntValue("TargetPos", -1))
					{
						if (-1 == systemMagic.GetIntValue("TargetPos", -1) || 2 == systemMagic.GetIntValue("TargetPos", -1))
						{
							int attackDistance = systemMagic.GetIntValue("AttackDistance", -1);
							if (!SpriteAttack.JugeMagicDistance(systemMagic, monster, lockObject, (int)enemyObject.CurrentPos.X, (int)enemyObject.CurrentPos.Y, magicCode, false))
							{
								return -1;
							}
							targetSprite = enemyObject;
						}
						else if (3 == systemMagic.GetIntValue("TargetPos", -1))
						{
						}
					}
					if (!monster.MyMagicCoolDownMgr.SkillCoolDown(magicCode))
					{
						result = -1;
					}
					else if (monster.MagicFinish < 0)
					{
						result = -1;
					}
					else
					{
						monster.MyMagicCoolDownMgr.AddSkillCoolDown(monster, magicCode);
						monster.CurrentMagic = magicCode;
						monster.MagicFinish = -1;
						GameManager.ClientMgr.NotifyOthersMagicCode(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.RoleID, monster.MonsterZoneNode.MapCode, magicCode, 116);
						if (!doAttackAction)
						{
							if (null != targetSprite)
							{
								monster.EnemyTarget = targetSprite.CurrentPos;
							}
						}
						else
						{
							if (1 == systemMagic.GetIntValue("MagicType", -1))
							{
								if (null != targetSprite)
								{
									monster.EnemyTarget = targetSprite.CurrentPos;
								}
							}
							else if (2 == systemMagic.GetIntValue("TargetPos", -1))
							{
								if (null != targetSprite)
								{
									monster.EnemyTarget = targetSprite.CurrentPos;
								}
							}
							this.MagicAttack(monster, magicCode, 1024, 1 == systemMagic.GetIntValue("TargetPos", -1));
						}
						result = 0;
					}
				}
			}
			return result;
		}

		
		protected void MagicAttack(Monster monster, int magicCode, int magicRange, bool notChangeDirection = false)
		{
			this.SpellCasting(monster, magicCode, notChangeDirection);
		}

		
		protected double CalcDirection(Monster monster, Point p)
		{
			return Global.GetDirectionByTan(p.X, p.Y, monster.Coordinate.X, monster.Coordinate.Y);
		}

		
		protected void SpellCasting(Monster monster, int magicCode, bool notChangeDirection = false)
		{
			double newDirection = monster.Direction;
			if (!notChangeDirection)
			{
				newDirection = this.CalcDirection(monster, monster.EnemyTarget);
			}
			this.DoAttackAction(monster, newDirection);
		}

		
		public static int MinSeekRangeMonsterLevel = 0;

		
		private MonsterContainer MyMonsterContainer = new MonsterContainer();

		
		private MonsterMoving monsterMoving = new MonsterMoving();

		
		private ConcurrentDictionary<long, long> DeadMonsterUniqueIdDict = new ConcurrentDictionary<long, long>();

		
		private List<Monster> ListDelayDeadMonster = new List<Monster>();
	}
}
