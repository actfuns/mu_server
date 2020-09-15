using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;

namespace GameServer.Logic
{
	// Token: 0x02000750 RID: 1872
	public class MapGridMagicHelper
	{
		// Token: 0x06002F18 RID: 12056 RVA: 0x002A17C8 File Offset: 0x0029F9C8
		public void AddMagicHelper(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, Point centerGridXY, int gridWidthNum, int gridHeightNum, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			List<Point> pts = new List<Point>();
			pts.Add(centerGridXY);
			int i = (int)centerGridXY.X - gridWidthNum;
			while ((double)i <= centerGridXY.X + (double)gridWidthNum)
			{
				int j = (int)centerGridXY.Y - gridHeightNum;
				while ((double)j <= centerGridXY.Y + (double)gridHeightNum)
				{
					pts.Add(new Point((double)i, (double)j));
					j++;
				}
				i++;
			}
			for (i = 0; i < pts.Count; i++)
			{
				if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)pts[i].X, (int)pts[i].Y))
				{
					Dictionary<MagicActionIDs, GridMagicHelperItem> dict = null;
					string key = string.Format("{0}_{1}_{2}", pts[i].X, pts[i].Y, copyMapID);
					lock (this._GridMagicHelperDict)
					{
						if (!this._GridMagicHelperDict.TryGetValue(key, out dict))
						{
							dict = new Dictionary<MagicActionIDs, GridMagicHelperItem>();
							this._GridMagicHelperDict[key] = dict;
						}
					}
					lock (dict)
					{
						if (dict.ContainsKey(magicActionID))
						{
							goto IL_23B;
						}
					}
					GridMagicHelperItem magicHelperItem = new GridMagicHelperItem
					{
						MagicActionID = magicActionID,
						MagicActionParams = magicActionParams,
						StartedTicks = TimeUtil.NOW(),
						LastTicks = TimeUtil.NOW(),
						ExecutedNum = 0,
						MapCode = mapCode
					};
					lock (dict)
					{
						dict[magicHelperItem.MagicActionID] = magicHelperItem;
					}
				}
				IL_23B:;
			}
		}

		// Token: 0x06002F19 RID: 12057 RVA: 0x002A1A50 File Offset: 0x0029FC50
		public void AddMagicHelperEx(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, int posX, int posY, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY))
			{
				GridMagicHelperItemKey itemKey = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = posX,
					PosY = posY,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				GridMagicHelperItemEx magicHelperItem = new GridMagicHelperItemEx
				{
					MagicActionID = magicActionID,
					MagicActionParams = magicActionParams,
					StartedTicks = TimeUtil.NOW(),
					LastTicks = TimeUtil.NOW(),
					ExecutedNum = 0,
					MapCode = mapCode
				};
				lock (this._GridMagicHelperDictEx)
				{
					if (!this._GridMagicHelperDictEx.ContainsKey(itemKey))
					{
						this._GridMagicHelperDictEx.Add(itemKey, magicHelperItem);
					}
				}
			}
		}

		// Token: 0x06002F1A RID: 12058 RVA: 0x002A1B78 File Offset: 0x0029FD78
		public void AddMagicHelperExAction(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, Point centerGridXY, int gridWidthNum, int gridHeightNum, int copyMapID = -1)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			int gridX = (int)centerGridXY.X;
			int gridY = (int)centerGridXY.Y;
			GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, gridX, gridY))
			{
				GridMagicHelperItemKey itemKey = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = gridX,
					PosY = gridY,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				GridMagicHelperItemEx magicHelperItem = new GridMagicHelperItemEx
				{
					MagicActionID = magicActionID,
					MagicActionParams = magicActionParams,
					StartedTicks = TimeUtil.NOW(),
					LastTicks = TimeUtil.NOW(),
					ExecutedNum = 0,
					MapCode = mapCode,
					AttackerRoleId = (int)magicActionParams[2]
				};
				int magicActionID2Index = 3;
				int length = magicActionParams.Length - magicActionID2Index - 1;
				itemKey.MagicActionID2 = (MagicActionIDs)magicActionParams[magicActionID2Index];
				magicHelperItem.MagicActionParams2 = new double[length];
				Array.Copy(magicActionParams, magicActionID2Index + 1, magicHelperItem.MagicActionParams2, 0, length);
				HashSet<Point> pts = new HashSet<Point>();
				pts.Add(centerGridXY);
				int i = (int)centerGridXY.X - gridWidthNum;
				while ((double)i <= centerGridXY.X + (double)gridWidthNum)
				{
					int j = (int)centerGridXY.Y - gridHeightNum;
					while ((double)j <= centerGridXY.Y + (double)gridHeightNum)
					{
						pts.Add(new Point((double)i, (double)j));
						j++;
					}
					i++;
				}
				foreach (Point pt in pts)
				{
					if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)pt.X, (int)pt.Y))
					{
						magicHelperItem.PointList.Add(pt);
					}
				}
				lock (this._GridMagicHelperDictEx)
				{
					if (!this._GridMagicHelperDictEx.ContainsKey(itemKey))
					{
						this._GridMagicHelperDictEx.Add(itemKey, magicHelperItem);
					}
				}
			}
		}

		// Token: 0x06002F1B RID: 12059 RVA: 0x002A1DF0 File Offset: 0x0029FFF0
		private bool CanExecuteItem(Dictionary<MagicActionIDs, GridMagicHelperItem> dict, GridMagicHelperItem magicHelperItem, double effectSecs, int maxNum)
		{
			long nowTicks = TimeUtil.NOW();
			long ticks = magicHelperItem.StartedTicks + (long)(effectSecs * 1000.0);
			bool result;
			if (maxNum <= 0)
			{
				if (nowTicks >= ticks)
				{
					bool flag = false;
					try
					{
						Dictionary<MagicActionIDs, GridMagicHelperItem> obj = dict;
						Monitor.Enter(dict, ref flag);
						dict.Remove(magicHelperItem.MagicActionID);
					}
					finally
					{
						if (flag)
						{
							Dictionary<MagicActionIDs, GridMagicHelperItem> obj;
							Monitor.Exit(obj);
						}
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (magicHelperItem.ExecutedNum >= maxNum)
			{
				bool flag2 = false;
				try
				{
					Dictionary<MagicActionIDs, GridMagicHelperItem> obj = dict;
					Monitor.Enter(dict, ref flag2);
					dict.Remove(magicHelperItem.MagicActionID);
				}
				finally
				{
					if (flag2)
					{
						Dictionary<MagicActionIDs, GridMagicHelperItem> obj;
						Monitor.Exit(obj);
					}
				}
				result = false;
			}
			else
			{
				long ticksSlot = (long)(effectSecs / (double)maxNum * 1000.0);
				result = (nowTicks - magicHelperItem.LastTicks >= ticksSlot);
			}
			return result;
		}

		// Token: 0x06002F1C RID: 12060 RVA: 0x002A1F0C File Offset: 0x002A010C
		private bool CanExecuteItemEx(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, double effectSecs, int maxNum, long nowTicks)
		{
			long ticks = magicHelperItem.StartedTicks + (long)(effectSecs * 1000.0);
			bool result;
			if (maxNum <= 0)
			{
				if (nowTicks >= ticks)
				{
					lock (this._GridMagicHelperDictEx)
					{
						this._GridMagicHelperDictEx.Remove(key);
					}
					result = false;
				}
				else
				{
					result = true;
				}
			}
			else if (magicHelperItem.ExecutedNum >= maxNum)
			{
				lock (this._GridMagicHelperDictEx)
				{
					this._GridMagicHelperDictEx.Remove(key);
				}
				result = false;
			}
			else
			{
				long ticksSlot = (long)(effectSecs / (double)maxNum * 1000.0);
				result = (nowTicks - magicHelperItem.LastTicks >= ticksSlot);
			}
			return result;
		}

		// Token: 0x06002F1D RID: 12061 RVA: 0x002A202C File Offset: 0x002A022C
		public void ExecuteMAttack(string gridXY, Dictionary<MagicActionIDs, GridMagicHelperItem> dict)
		{
			string[] fields = gridXY.Split(new char[]
			{
				'_'
			});
			int gridX = Global.SafeConvertToInt32(fields[0]);
			int gridY = Global.SafeConvertToInt32(fields[1]);
			GridMagicHelperItem magicHelperItem = null;
			lock (dict)
			{
				dict.TryGetValue(MagicActionIDs.FIRE_WALL, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(dict, magicHelperItem, (double)((int)magicHelperItem.MagicActionParams[0]), (int)magicHelperItem.MagicActionParams[1]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW();
					double attackPercent = magicHelperItem.MagicActionParams[2];
					int attacker = (int)magicHelperItem.MagicActionParams[3];
					if (-1 != attacker)
					{
						GameClient client = GameManager.ClientMgr.FindClient(attacker);
						if (null != client)
						{
							List<object> enemiesList = new List<object>();
							GameManager.ClientMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							BiaoCheManager.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							JunQiManager.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							for (int x = 0; x < enemiesList.Count; x++)
							{
								IObject obj = enemiesList[x] as IObject;
								if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
								{
									if (obj is GameClient)
									{
										if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(client, obj as GameClient))
										{
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as GameClient, 0, 0, 1.0, 1, false, 0, attackPercent, 0, 0, 0, 0.0, 0.0, false, false, 1.0, 0, 0, 0, 0.0);
										}
									}
									else if (obj is Monster)
									{
										if (Global.IsOpposition(client, obj as Monster))
										{
											GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster, 0, 0, 1.0, 1, false, 0, attackPercent, 0, 0, 0, 0.0, 0.0, false, 1.0, 0, 0, 0, 0.0);
										}
									}
									else if (obj is BiaoCheItem)
									{
										if (Global.IsOpposition(client, obj as BiaoCheItem))
										{
											BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
									else if (obj is JunQiItem)
									{
										if (Global.IsOpposition(client, obj as JunQiItem))
										{
											JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as JunQiItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
									else if (obj is FakeRoleItem)
									{
										if (Global.IsOpposition(client, obj as FakeRoleItem))
										{
											FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as FakeRoleItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 1.0, 0, 0);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F1E RID: 12062 RVA: 0x002A2484 File Offset: 0x002A0684
		public void ExecuteMUFireWall(int id, string gridXY, Dictionary<MagicActionIDs, GridMagicHelperItem> dict)
		{
			string[] fields = gridXY.Split(new char[]
			{
				'_'
			});
			int gridX = Global.SafeConvertToInt32(fields[0]);
			int gridY = Global.SafeConvertToInt32(fields[1]);
			GridMagicHelperItem magicHelperItem = null;
			lock (dict)
			{
				dict.TryGetValue((MagicActionIDs)id, out magicHelperItem);
			}
			if (null != magicHelperItem)
			{
				if (this.CanExecuteItem(dict, magicHelperItem, (double)((int)magicHelperItem.MagicActionParams[0]), (int)magicHelperItem.MagicActionParams[1]))
				{
					magicHelperItem.ExecutedNum++;
					magicHelperItem.LastTicks = TimeUtil.NOW();
					int addValue = (int)magicHelperItem.MagicActionParams[2];
					int attacker = (int)magicHelperItem.MagicActionParams[3];
					double baseRate = magicHelperItem.MagicActionParams[4];
					if (-1 != attacker)
					{
						GameClient client = GameManager.ClientMgr.FindClient(attacker);
						if (null != client)
						{
							List<object> enemiesList = new List<object>();
							GameManager.ClientMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							BiaoCheManager.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							JunQiManager.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
							for (int x = 0; x < enemiesList.Count; x++)
							{
								IObject obj = enemiesList[x] as IObject;
								if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
								{
									if (obj is GameClient)
									{
										if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(client, obj as GameClient))
										{
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, baseRate, addValue, 0, 0, 0.0);
										}
									}
									else if (obj is Monster)
									{
										if (Global.IsOpposition(client, obj as Monster))
										{
											GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
										}
									}
									else if (obj is BiaoCheItem)
									{
										if (Global.IsOpposition(client, obj as BiaoCheItem))
										{
											BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, baseRate, addValue, 0);
										}
									}
									else if (obj is JunQiItem)
									{
										if (Global.IsOpposition(client, obj as JunQiItem))
										{
											JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, baseRate, addValue, 0);
										}
									}
									else if (obj is FakeRoleItem)
									{
										if (Global.IsOpposition(client, obj as FakeRoleItem))
										{
											FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as FakeRoleItem, 0, 0, 1.0, 1, false, 0, baseRate, addValue, 0, 1.0, 0, 0);
										}
									}
								}
							}
						}
						else
						{
							Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attacker);
							if (null != monster)
							{
								List<object> enemiesList = new List<object>();
								GameManager.ClientMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
								GameManager.MonsterMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
								BiaoCheManager.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
								JunQiManager.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
								for (int x = 0; x < enemiesList.Count; x++)
								{
									IObject obj = enemiesList[x] as IObject;
									if (obj.CurrentCopyMapID == monster.CurrentCopyMapID)
									{
										if (obj is GameClient)
										{
											if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(monster, obj as GameClient))
											{
												GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
											}
										}
										else if (obj is Monster)
										{
											if (Global.IsOpposition(monster, obj as Monster))
											{
												GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
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

		// Token: 0x06002F1F RID: 12063 RVA: 0x002A2AD4 File Offset: 0x002A0CD4
		public void ExecuteAllItems()
		{
			List<string> list = new List<string>();
			lock (this._GridMagicHelperDict)
			{
				list = this._GridMagicHelperDict.Keys.ToList<string>();
			}
			Dictionary<MagicActionIDs, GridMagicHelperItem> dict = null;
			for (int i = 0; i < list.Count; i++)
			{
				dict = null;
				lock (this._GridMagicHelperDict)
				{
					this._GridMagicHelperDict.TryGetValue(list[i], out dict);
				}
				if (null != dict)
				{
					this.ExecuteMAttack(list[i], dict);
					this.ExecuteMUFireWall(268, list[i], dict);
					this.ExecuteMUFireWall(269, list[i], dict);
					this.ExecuteMUFireWall(270, list[i], dict);
				}
			}
		}

		// Token: 0x06002F20 RID: 12064 RVA: 0x002A2BFC File Offset: 0x002A0DFC
		public int GetObjectAddMapBuffer(int nAttackID)
		{
			int nCount = 0;
			lock (this._GridMagicHelperDictEx)
			{
				foreach (KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx> kv in this._GridMagicHelperDictEx)
				{
					if ((int)kv.Value.MagicActionParams[3] == nAttackID)
					{
						if (kv.Value.ExecutedNum < 1)
						{
							nCount++;
						}
					}
				}
			}
			return nCount;
		}

		// Token: 0x06002F21 RID: 12065 RVA: 0x002A2CCC File Offset: 0x002A0ECC
		public void ExecuteAllItemsEx()
		{
			long nowTicks = TimeUtil.NOW();
			this.Mgr.Run(nowTicks);
			List<KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx>> list = null;
			lock (this._GridMagicHelperDictEx)
			{
				foreach (KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx> kv in this._GridMagicHelperDictEx)
				{
					if (this.CanExecuteItemEx(kv.Key, kv.Value, (double)((int)kv.Value.MagicActionParams[0]), (int)kv.Value.MagicActionParams[1], nowTicks))
					{
						if (null == list)
						{
							list = new List<KeyValuePair<GridMagicHelperItemKey, GridMagicHelperItemEx>>();
						}
						list.Add(kv);
					}
				}
			}
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					switch (list[i].Key.MagicActionID)
					{
					case MagicActionIDs.MU_FIRE_WALL_X:
						this.ExecuteMUFireWall_X(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_SECTOR:
						this.ExecuteMUFireSector(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_STRAIGHT:
						this.ExecuteMUFireStraight(list[i].Key, list[i].Value, nowTicks);
						break;
					case MagicActionIDs.MU_FIRE_WALL_ACTION:
						this.ExecuteMUFireWallAction(list[i].Key, list[i].Value, nowTicks);
						break;
					}
				}
			}
		}

		// Token: 0x06002F22 RID: 12066 RVA: 0x002A2EE4 File Offset: 0x002A10E4
		public void ExecuteMUFireWall_X(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int id = (int)key.MagicActionID;
			int gridX = key.PosX;
			int gridY = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int addValue = (int)magicHelperItem.MagicActionParams[2];
			int attacker = (int)magicHelperItem.MagicActionParams[3];
			double baseRate = magicHelperItem.MagicActionParams[4];
			int radio = (int)magicHelperItem.MagicActionParams[5];
			double speedSlowRate = magicHelperItem.MagicActionParams[6];
			double speedSlowValue = magicHelperItem.MagicActionParams[7];
			double speedSlowSecs = magicHelperItem.MagicActionParams[8];
			int mapGridWidth = (int)magicHelperItem.MagicActionParams[15];
			int magicCode = (int)magicHelperItem.MagicActionParams[16];
			gridX *= mapGridWidth;
			gridY *= mapGridWidth;
			if (-1 != attacker)
			{
				int hitNum = magicHelperItem.MaxNum;
				GameClient client = GameManager.ClientMgr.FindClient(attacker);
				if (null != client)
				{
					List<object> enemiesList = new List<object>();
					GameManager.ClientMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, radio, enemiesList);
					GameManager.MonsterMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, radio, enemiesList);
					JunQiManager.LookupEnemiesInCircle(client, magicHelperItem.MapCode, gridX, gridY, radio, enemiesList);
					List<object> relEnemyList = new List<object>();
					foreach (object tmp in enemiesList)
					{
						if (tmp is Monster)
						{
							if (Global.IsOpposition(client, tmp as Monster))
							{
								relEnemyList.Add(tmp);
							}
						}
						else if (tmp is GameClient)
						{
							if ((tmp as GameClient).ClientData.RoleID != client.ClientData.RoleID)
							{
								if (Global.IsOpposition(client, tmp as GameClient))
								{
									relEnemyList.Add(tmp);
								}
							}
						}
					}
					double shenShiInjurePercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(client, relEnemyList, magicCode);
					for (int x = 0; x < enemiesList.Count; x++)
					{
						IObject obj = enemiesList[x] as IObject;
						if (client.CurrentMapCode == obj.CurrentMapCode)
						{
							if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
							{
								bool TryAddBuff = false;
								if (obj is GameClient)
								{
									if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(client, obj as GameClient))
									{
										hitNum--;
										TryAddBuff = true;
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, baseRate, addValue, 0, magicCode, shenShiInjurePercent);
									}
								}
								else if (obj is Monster)
								{
									if (Global.IsOpposition(client, obj as Monster))
									{
										hitNum--;
										TryAddBuff = true;
										GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, magicCode, shenShiInjurePercent);
									}
								}
								else if (obj is BiaoCheItem)
								{
									if (Global.IsOpposition(client, obj as BiaoCheItem))
									{
										hitNum--;
										TryAddBuff = true;
										BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, baseRate, addValue, 0);
									}
								}
								else if (obj is JunQiItem)
								{
									if (Global.IsOpposition(client, obj as JunQiItem))
									{
										hitNum--;
										TryAddBuff = true;
										JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, baseRate, addValue, 0);
									}
								}
								else if (obj is FakeRoleItem)
								{
									if (Global.IsOpposition(client, obj as FakeRoleItem))
									{
										hitNum--;
										TryAddBuff = true;
										FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as FakeRoleItem, 0, 0, 1.0, 1, false, 0, baseRate, addValue, 0, 1.0, 0, 0);
									}
								}
								if (TryAddBuff)
								{
									if (speedSlowRate > 0.9999 || (speedSlowRate > 0.0 && Global.GetRandom() < speedSlowRate))
									{
										double[] actionParams = new double[]
										{
											speedSlowValue,
											speedSlowSecs
										};
										MagicAction.ProcessAction(client, obj, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
									}
								}
								if (hitNum == 0)
								{
									break;
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attacker);
					if (null != monster)
					{
						List<object> enemiesList = new List<object>();
						GameManager.ClientMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, radio, enemiesList);
						GameManager.MonsterMgr.LookupEnemiesInCircle(magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, radio, enemiesList);
						List<object> relEnemyList = new List<object>();
						foreach (object tmp in enemiesList)
						{
							if (tmp is Monster)
							{
								if (Global.IsOpposition(monster, tmp as Monster))
								{
									relEnemyList.Add(tmp);
								}
							}
							else if (tmp is GameClient)
							{
								if (Global.IsOpposition(monster, tmp as GameClient))
								{
									relEnemyList.Add(tmp);
								}
							}
						}
						double shenShiInjurePercent = ShenShiManager.getInstance().GetMagicCodeAddPercent2(monster, relEnemyList, magicCode);
						for (int x = 0; x < enemiesList.Count; x++)
						{
							IObject obj = enemiesList[x] as IObject;
							if (monster.CurrentMapCode == obj.CurrentMapCode)
							{
								if (obj.CurrentCopyMapID == monster.CurrentCopyMapID)
								{
									bool TryAddBuff = false;
									if (obj is GameClient)
									{
										if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(monster, obj as GameClient))
										{
											hitNum--;
											TryAddBuff = true;
											GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, magicCode, shenShiInjurePercent);
										}
									}
									else if (obj is Monster)
									{
										if (Global.IsOpposition(monster, obj as Monster))
										{
											hitNum--;
											TryAddBuff = true;
											GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, magicCode, shenShiInjurePercent);
										}
									}
									if (TryAddBuff)
									{
										if (speedSlowRate > 0.9999 || (speedSlowRate > 0.0 && Global.GetRandom() < speedSlowRate))
										{
											double[] actionParams = new double[]
											{
												speedSlowValue,
												speedSlowSecs
											};
											MagicAction.ProcessAction(monster, obj, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
										}
									}
									if (hitNum == 0)
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F23 RID: 12067 RVA: 0x002A387C File Offset: 0x002A1A7C
		public void ExecuteMUFireWallAction(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int attacker = magicHelperItem.AttackerRoleId;
			if (-1 != attacker)
			{
				GameClient client = GameManager.ClientMgr.FindClient(attacker);
				if (null != client)
				{
					List<object> enemiesList = new List<object>();
					foreach (Point pt in magicHelperItem.PointList)
					{
						int gridX = (int)pt.X;
						int gridY = (int)pt.Y;
						GameManager.ClientMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
						GameManager.MonsterMgr.LookupEnemiesAtGridXY(client, gridX, gridY, enemiesList);
					}
					for (int x = 0; x < enemiesList.Count; x++)
					{
						IObject obj = enemiesList[x] as IObject;
						if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
						{
							MagicAction.ProcessAction(client, obj, key.MagicActionID2, magicHelperItem.MagicActionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attacker);
					if (null != monster)
					{
						List<object> enemiesList = new List<object>();
						foreach (Point pt in magicHelperItem.PointList)
						{
							int gridX = (int)pt.X;
							int gridY = (int)pt.Y;
							GameManager.ClientMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
							GameManager.MonsterMgr.LookupEnemiesAtGridXY(monster, gridX, gridY, enemiesList);
						}
						for (int x = 0; x < enemiesList.Count; x++)
						{
							IObject obj = enemiesList[x] as IObject;
							if (obj.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								MagicAction.ProcessAction(monster, obj, key.MagicActionID2, magicHelperItem.MagicActionParams2, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F24 RID: 12068 RVA: 0x002A3AF4 File Offset: 0x002A1CF4
		public void ExecuteMUFireSector(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int id = (int)key.MagicActionID;
			int gridX = key.PosX;
			int gridY = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int addValue = (int)magicHelperItem.MagicActionParams[2];
			int attacker = (int)magicHelperItem.MagicActionParams[3];
			double baseRate = magicHelperItem.MagicActionParams[4];
			int radio = (int)magicHelperItem.MagicActionParams[5];
			int angel = (int)magicHelperItem.MagicActionParams[6];
			int direction = (int)magicHelperItem.MagicActionParams[7];
			int mapGridWidth = (int)magicHelperItem.MagicActionParams[9];
			gridX *= mapGridWidth;
			gridY *= mapGridWidth;
			if (-1 != attacker)
			{
				GameClient client = GameManager.ClientMgr.FindClient(attacker);
				if (null != client)
				{
					List<object> enemiesList = new List<object>();
					GameManager.ClientMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, radio, enemiesList, (double)angel, false);
					GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, radio, enemiesList, (double)angel, false);
					for (int x = 0; x < enemiesList.Count; x++)
					{
						IObject obj = enemiesList[x] as IObject;
						if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
						{
							if (obj is GameClient)
							{
								if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(client, obj as GameClient))
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, baseRate, addValue, 0, 0, 0.0);
								}
							}
							else if (obj is Monster)
							{
								if (Global.IsOpposition(client, obj as Monster))
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
								}
							}
							else if (obj is BiaoCheItem)
							{
								if (Global.IsOpposition(client, obj as BiaoCheItem))
								{
									BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, baseRate, addValue, 0);
								}
							}
							else if (obj is JunQiItem)
							{
								if (Global.IsOpposition(client, obj as JunQiItem))
								{
									JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, baseRate, addValue, 0);
								}
							}
							else if (obj is FakeRoleItem)
							{
								if (Global.IsOpposition(client, obj as FakeRoleItem))
								{
									FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as FakeRoleItem, 0, 0, 1.0, 1, false, 0, baseRate, addValue, 0, 1.0, 0, 0);
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attacker);
					if (null != monster)
					{
						List<object> enemiesList = new List<object>();
						GameManager.ClientMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, radio, enemiesList, (double)angel, false);
						GameManager.MonsterMgr.LookupEnemiesInCircleByAngle(direction, magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, radio, enemiesList, (double)angel, false);
						for (int x = 0; x < enemiesList.Count; x++)
						{
							IObject obj = enemiesList[x] as IObject;
							if (obj.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								if (obj is GameClient)
								{
									if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(monster, obj as GameClient))
									{
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
									}
								}
								else if (obj is Monster)
								{
									if (Global.IsOpposition(monster, obj as Monster))
									{
										GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F25 RID: 12069 RVA: 0x002A4108 File Offset: 0x002A2308
		public void ExecuteMUFireStraight(GridMagicHelperItemKey key, GridMagicHelperItemEx magicHelperItem, long nowTicks)
		{
			int id = (int)key.MagicActionID;
			int gridX = key.PosX;
			int gridY = key.PosY;
			magicHelperItem.ExecutedNum++;
			magicHelperItem.LastTicks = nowTicks;
			int addValue = (int)magicHelperItem.MagicActionParams[2];
			int attacker = (int)magicHelperItem.MagicActionParams[3];
			double baseRate = magicHelperItem.MagicActionParams[4];
			int radio = (int)magicHelperItem.MagicActionParams[5];
			int width = (int)magicHelperItem.MagicActionParams[6];
			int deltaX = (int)magicHelperItem.MagicActionParams[7];
			int deltaY = (int)magicHelperItem.MagicActionParams[8];
			int mapGridWidth = (int)magicHelperItem.MagicActionParams[9];
			gridX *= mapGridWidth;
			gridY *= mapGridWidth;
			if (-1 != attacker)
			{
				GameClient client = GameManager.ClientMgr.FindClient(attacker);
				if (null != client)
				{
					List<object> enemiesList = new List<object>();
					GameManager.ClientMgr.LookupRolesInSquare(magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, 0, 0, radio, width, enemiesList);
					GameManager.MonsterMgr.LookupRolesInSquare(magicHelperItem.MapCode, client.ClientData.CopyMapID, gridX, gridY, 0, 0, radio, width, enemiesList, 1);
					for (int x = 0; x < enemiesList.Count; x++)
					{
						IObject obj = enemiesList[x] as IObject;
						if (obj.CurrentCopyMapID == client.CurrentCopyMapID)
						{
							if (obj is GameClient)
							{
								if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(client, obj as GameClient))
								{
									GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, false, baseRate, addValue, 0, 0, 0.0);
								}
							}
							else if (obj is Monster)
							{
								if (Global.IsOpposition(client, obj as Monster))
								{
									GameManager.MonsterMgr.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
								}
							}
							else if (obj is BiaoCheItem)
							{
								if (Global.IsOpposition(client, obj as BiaoCheItem))
								{
									BiaoCheManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as BiaoCheItem, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, baseRate, addValue, 0);
								}
							}
							else if (obj is JunQiItem)
							{
								if (Global.IsOpposition(client, obj as JunQiItem))
								{
									JunQiManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as JunQiItem, 0, 0, 1.0, 1, false, 0, 0.0, 0, 0, baseRate, addValue, 0);
								}
							}
							else if (obj is FakeRoleItem)
							{
								if (Global.IsOpposition(client, obj as FakeRoleItem))
								{
									FakeRoleManager.NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, obj as FakeRoleItem, 0, 0, 1.0, 1, false, 0, baseRate, addValue, 0, 1.0, 0, 0);
								}
							}
						}
					}
				}
				else
				{
					Monster monster = GameManager.MonsterMgr.FindMonster(magicHelperItem.MapCode, attacker);
					if (null != monster)
					{
						List<object> enemiesList = new List<object>();
						GameManager.ClientMgr.LookupRolesInSquare(magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, 0, 0, radio, width, enemiesList);
						GameManager.MonsterMgr.LookupRolesInSquare(magicHelperItem.MapCode, monster.CopyMapID, gridX, gridY, 0, 0, radio, width, enemiesList, 1);
						for (int x = 0; x < enemiesList.Count; x++)
						{
							IObject obj = enemiesList[x] as IObject;
							if (obj.CurrentCopyMapID == monster.CurrentCopyMapID)
							{
								if (obj is GameClient)
								{
									if ((obj as GameClient).ClientData.RoleID != attacker && Global.IsOpposition(monster, obj as GameClient))
									{
										GameManager.ClientMgr.NotifyOtherInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as GameClient, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
									}
								}
								else if (obj is Monster)
								{
									if (Global.IsOpposition(monster, obj as Monster))
									{
										GameManager.MonsterMgr.Monster_NotifyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, obj as Monster, 0, 0, 1.0, 1, false, 0, 1.0, 0, 0, 0, 0.0, 0.0, false, baseRate, addValue, 0, 0, 0.0);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002F26 RID: 12070 RVA: 0x002A4720 File Offset: 0x002A2920
		public void AddGridMagic(MagicActionIDs magicActionID, double[] magicActionParams, int mapCode, int posX, int posY, int DelayDecoration, int DecorationTime, int copyMapID, int maxHitCount = 8)
		{
			if (copyMapID < 0)
			{
				copyMapID = -1;
			}
			if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY))
			{
				GridMagicHelperItemKey itemKey = new GridMagicHelperItemKey
				{
					MapCode = mapCode,
					PosX = posX,
					PosY = posY,
					CopyMapID = copyMapID,
					MagicActionID = magicActionID
				};
				if (DelayDecoration > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
					Point pos = new Point((double)(posX * mapGrid.MapGridWidth + mapGrid.MapGridWidth / 2), (double)(posY * mapGrid.MapGridHeight + mapGrid.MapGridHeight / 2));
					DecorationManager.AddDecoToMap(mapCode, copyMapID, pos, DelayDecoration, DecorationTime * 1000, 2000, true);
				}
				long period = (long)magicActionParams[0] * 1000L;
				int execCount = (int)magicActionParams[1];
				GridMagicHelperItemEx magicHelperItem = new GridMagicHelperItemEx();
				magicHelperItem.MagicActionID = magicActionID;
				magicHelperItem.MagicActionParams = magicActionParams;
				magicHelperItem.LastTicks = TimeUtil.NOW();
				magicHelperItem.ExecutedNum = 0;
				magicHelperItem.MapCode = mapCode;
				magicHelperItem.MaxNum = maxHitCount;
				if (MagicActionIDs.MU_FIRE_WALL_Y == magicActionID)
				{
					magicHelperItem.StartedTicks = TimeUtil.NOW();
				}
				else
				{
					magicHelperItem.StartedTicks = TimeUtil.NOW() + period;
				}
				MapGridMagicHelper.GridMagicItem item = new MapGridMagicHelper.GridMagicItem
				{
					ItemKey = itemKey,
					MagicHelperItem = magicHelperItem
				};
				this.Mgr.AddItem(magicHelperItem.StartedTicks, period, execCount, 0, new Action<long, object>(this.FireWallActionProc), item);
			}
		}

		// Token: 0x06002F27 RID: 12071 RVA: 0x002A48BC File Offset: 0x002A2ABC
		public void FireWallActionProc(long execTicks, object state)
		{
			MapGridMagicHelper.GridMagicItem item = state as MapGridMagicHelper.GridMagicItem;
			if (null != item)
			{
				this.ExecuteMUFireWall_X(item.ItemKey, item.MagicHelperItem, execTicks);
			}
		}

		// Token: 0x04003CC2 RID: 15554
		private Dictionary<string, Dictionary<MagicActionIDs, GridMagicHelperItem>> _GridMagicHelperDict = new Dictionary<string, Dictionary<MagicActionIDs, GridMagicHelperItem>>();

		// Token: 0x04003CC3 RID: 15555
		private SortedDictionary<GridMagicHelperItemKey, GridMagicHelperItemEx> _GridMagicHelperDictEx = new SortedDictionary<GridMagicHelperItemKey, GridMagicHelperItemEx>(GridMagicHelperItemKey.Comparer);

		// Token: 0x04003CC4 RID: 15556
		private TimedActionManager Mgr = new TimedActionManager();

		// Token: 0x02000751 RID: 1873
		private class GridMagicItem
		{
			// Token: 0x04003CC5 RID: 15557
			public GridMagicHelperItemKey ItemKey;

			// Token: 0x04003CC6 RID: 15558
			public GridMagicHelperItemEx MagicHelperItem;
		}
	}
}
