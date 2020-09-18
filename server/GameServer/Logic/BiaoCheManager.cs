using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class BiaoCheManager
	{
		
		private static BiaoCheItem AddBiaoChe(GameClient client, int yaBiaoID)
		{
			SystemXmlItem systemYaBiaoItem = null;
			BiaoCheItem result;
			if (!GameManager.systemYaBiaoMgr.SystemXmlItemDict.TryGetValue(yaBiaoID, out systemYaBiaoItem))
			{
				result = null;
			}
			else
			{
				BiaoCheItem biaoCheItem = new BiaoCheItem
				{
					OwnerRoleID = client.ClientData.RoleID,
					OwnerRoleName = Global.FormatRoleName(client, client.ClientData.RoleName),
					BiaoCheID = (int)GameManager.BiaoCheIDMgr.GetNewID(),
					BiaoCheName = Global.GetYaBiaoName(yaBiaoID),
					YaBiaoID = yaBiaoID,
					MapCode = client.ClientData.MapCode,
					PosX = client.ClientData.PosX,
					PosY = client.ClientData.PosY,
					Direction = client.ClientData.RoleDirection,
					LifeV = systemYaBiaoItem.GetIntValue("Lifev", -1),
					StartTime = TimeUtil.NOW(),
					CurrentLifeV = systemYaBiaoItem.GetIntValue("Lifev", -1),
					CutLifeV = systemYaBiaoItem.GetIntValue("CutLifeV", -1),
					BodyCode = systemYaBiaoItem.GetIntValue("BodyCode", -1),
					PicCode = systemYaBiaoItem.GetIntValue("PicCode", -1),
					DestNPC = systemYaBiaoItem.GetIntValue("DestNPC", -1),
					MinLevel = systemYaBiaoItem.GetIntValue("MinLevel", -1),
					MaxLevel = systemYaBiaoItem.GetIntValue("MaxLevel", -1)
				};
				lock (BiaoCheManager._RoleID2BiaoCheDict)
				{
					BiaoCheManager._RoleID2BiaoCheDict[biaoCheItem.OwnerRoleID] = biaoCheItem;
				}
				lock (BiaoCheManager._ID2BiaoCheDict)
				{
					BiaoCheManager._ID2BiaoCheDict[biaoCheItem.BiaoCheID] = biaoCheItem;
				}
				result = biaoCheItem;
			}
			return result;
		}

		
		public static BiaoCheItem FindBiaoCheByRoleID(int roleID)
		{
			BiaoCheItem biaoCheItem = null;
			lock (BiaoCheManager._RoleID2BiaoCheDict)
			{
				BiaoCheManager._RoleID2BiaoCheDict.TryGetValue(roleID, out biaoCheItem);
			}
			return biaoCheItem;
		}

		
		public static BiaoCheItem FindBiaoCheByID(int biaoCheID)
		{
			BiaoCheItem biaoCheItem = null;
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				BiaoCheManager._ID2BiaoCheDict.TryGetValue(biaoCheID, out biaoCheItem);
			}
			return biaoCheItem;
		}

		
		private static void RemoveBiaoChe(int biaoCheID)
		{
			BiaoCheItem biaoCheItem = null;
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				BiaoCheManager._ID2BiaoCheDict.TryGetValue(biaoCheID, out biaoCheItem);
				if (null != biaoCheItem)
				{
					BiaoCheManager._ID2BiaoCheDict.Remove(biaoCheItem.BiaoCheID);
				}
			}
			lock (BiaoCheManager._RoleID2BiaoCheDict)
			{
				if (null != biaoCheItem)
				{
					BiaoCheManager._RoleID2BiaoCheDict.Remove(biaoCheItem.OwnerRoleID);
				}
			}
		}

		
		public static void ProcessNewBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, int yaBiaoID)
		{
			BiaoCheItem biaoCheItem = BiaoCheManager.AddBiaoChe(client, yaBiaoID);
			if (null == biaoCheItem)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("为RoleID生成镖车对象时失败, Client={0}, RoleID={1}, YaBiaoID={2}", Global.GetSocketRemoteEndPoint(client.ClientSocket, false), client.ClientData.RoleID, yaBiaoID), null, true);
			}
			else
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].MoveObject(-1, -1, biaoCheItem.PosX, biaoCheItem.PosY, biaoCheItem);
			}
		}

		
		public static void ProcessDelBiaoChe(SocketListener sl, TCPOutPacketPool pool, int biaoCheID)
		{
			BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(biaoCheID);
			if (null != biaoCheItem)
			{
				BiaoCheManager.RemoveBiaoChe(biaoCheID);
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].RemoveObject(biaoCheItem);
			}
		}

		
		public static void NotifyOthersShowBiaoChe(SocketListener sl, TCPOutPacketPool pool, BiaoCheItem biaoCheItem)
		{
			if (null != biaoCheItem)
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].MoveObject(-1, -1, biaoCheItem.PosX, biaoCheItem.PosY, biaoCheItem);
			}
		}

		
		public static void NotifyOthersHideBiaoChe(SocketListener sl, TCPOutPacketPool pool, BiaoCheItem biaoCheItem)
		{
			if (null != biaoCheItem)
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].RemoveObject(biaoCheItem);
			}
		}

		
		private static bool ProcessBiaoCheOverTime(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			bool result;
			if (nowTicks - biaoCheItem.StartTime < (long)Global.MaxYaBiaoTicks)
			{
				result = false;
			}
			else
			{
				BiaoCheManager.ProcessDelBiaoChe(sl, pool, biaoCheItem.BiaoCheID);
				result = true;
			}
			return result;
		}

		
		private static bool ProcessBiaoCheDead(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			bool result;
			if (biaoCheItem.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				long subTicks = nowTicks - biaoCheItem.BiaoCheDeadTicks;
				if (subTicks < 2000L)
				{
					result = false;
				}
				else
				{
					BiaoCheManager.ProcessDelBiaoChe(sl, pool, biaoCheItem.BiaoCheID);
					result = true;
				}
			}
			return result;
		}

		
		private static void ProcessBiaoCheAddLife(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			long subTicks = nowTicks - biaoCheItem.LastLifeMagicTick;
			if (subTicks >= 5000L)
			{
				biaoCheItem.LastLifeMagicTick = nowTicks;
				if (biaoCheItem.CurrentLifeV > 0)
				{
					if (biaoCheItem.CurrentLifeV < biaoCheItem.LifeV)
					{
						double lifeMax = (double)biaoCheItem.CutLifeV;
						lifeMax += (double)biaoCheItem.CurrentLifeV;
						if (biaoCheItem.CurrentLifeV > 0)
						{
							biaoCheItem.CurrentLifeV = (int)Global.GMin((double)biaoCheItem.LifeV, lifeMax);
							List<object> objList = Global.GetAll9Clients(biaoCheItem);
							GameManager.ClientMgr.NotifyOtherBiaoCheLifeV(sl, pool, objList, biaoCheItem.BiaoCheID, biaoCheItem.CurrentLifeV);
						}
					}
				}
			}
		}

		
		public static void ProcessAllBiaoCheItems(SocketListener sl, TCPOutPacketPool pool)
		{
			List<BiaoCheItem> biaoCheItemList = new List<BiaoCheItem>();
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				foreach (BiaoCheItem val in BiaoCheManager._ID2BiaoCheDict.Values)
				{
					biaoCheItemList.Add(val);
				}
			}
			long nowTicks = TimeUtil.NOW();
			for (int i = 0; i < biaoCheItemList.Count; i++)
			{
				BiaoCheItem biaoCheItem = biaoCheItemList[i];
				if (!BiaoCheManager.ProcessBiaoCheOverTime(sl, pool, nowTicks, biaoCheItem))
				{
					if (!BiaoCheManager.ProcessBiaoCheDead(sl, pool, nowTicks, biaoCheItem))
					{
						BiaoCheManager.ProcessBiaoCheAddLife(sl, pool, nowTicks, biaoCheItem);
					}
				}
			}
		}

		
		public static void SendMySelfBiaoCheItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is BiaoCheItem)
					{
						if ((objsList[i] as BiaoCheItem).CurrentLifeV > 0)
						{
							BiaoCheItem biaoCheItem = objsList[i] as BiaoCheItem;
							GameManager.ClientMgr.NotifyMySelfNewBiaoChe(sl, pool, client, biaoCheItem);
						}
					}
				}
			}
		}

		
		public static void DelMySelfBiaoCheItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is BiaoCheItem)
					{
						BiaoCheItem biaoCheItem = objsList[i] as BiaoCheItem;
						GameManager.ClientMgr.NotifyMySelfDelBiaoChe(sl, pool, client, biaoCheItem.BiaoCheID);
					}
				}
			}
		}

		
		public static void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<int> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is BiaoCheItem)
					{
						if (client == null || Global.IsOpposition(client, objList[i] as BiaoCheItem))
						{
							if (client == null || client.ClientData.CopyMapID == (objList[i] as BiaoCheItem).CopyMapID)
							{
								Point pt = new Point((double)(objList[i] as BiaoCheItem).PosX, (double)(objList[i] as BiaoCheItem).PosY);
								if (Global.InCircle(pt, center, (double)radius))
								{
									enemiesList.Add((objList[i] as BiaoCheItem).BiaoCheID);
								}
							}
						}
					}
				}
			}
		}

		
		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<BiaoCheItem> enemiesList, double angle)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is BiaoCheItem)
					{
						if (client == null || Global.IsOpposition(client, objList[i] as BiaoCheItem))
						{
							if (client == null || client.ClientData.CopyMapID == (objList[i] as BiaoCheItem).CopyMapID)
							{
								Point pt = new Point((double)(objList[i] as BiaoCheItem).PosX, (double)(objList[i] as BiaoCheItem).PosY);
								if (Global.InCircleByAngle(pt, center, (double)radius, loAngle, hiAngle))
								{
									enemiesList.Add(objList[i] as BiaoCheItem);
								}
							}
						}
					}
				}
			}
		}

		
		public static void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objList = mapGrid.FindObjects(gridX, gridY);
			if (null != objList)
			{
				for (int i = 0; i < objList.Count; i++)
				{
					if (objList[i] is BiaoCheItem)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (objList[i] as BiaoCheItem).CopyMapID)
						{
							enemiesList.Add(objList[i]);
						}
					}
				}
			}
		}

		
		public static void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			Point grid = attacker.CurrentGrid;
			int gridX = (int)grid.X;
			int gridY = (int)grid.Y;
			Point p = Global.GetGridPointByDirection(direction, gridX, gridY);
			BiaoCheManager.LookupEnemiesAtGridXY(attacker, (int)p.X, (int)p.Y, enemiesList);
		}

		
		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			BiaoCheManager.LookupAttackEnemies(attacker, direction, objList);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as BiaoCheItem).BiaoCheID);
			}
		}

		
		public static void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridList = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridList.Count > 0)
			{
				for (int i = 0; i < gridList.Count; i++)
				{
					BiaoCheManager.LookupEnemiesAtGridXY(obj, (int)gridList[i].X, (int)gridList[i].Y, enemiesList);
				}
			}
		}

		
		public static bool CanAttack(GameClient client, BiaoCheItem enemy)
		{
			bool result;
			if (null == enemy)
			{
				result = false;
			}
			else if (enemy.YaBiaoID == BiaoCheManager.NotAttackYaBiaoID)
			{
				result = false;
			}
			else
			{
				int maxlevel = (enemy.MaxLevel < 0) ? 1000 : enemy.MaxLevel;
				result = (client.ClientData.Level <= maxlevel);
			}
			return result;
		}

		
		public static int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int ret = 0;
			if ((enemy as BiaoCheItem).CurrentLifeV > 0)
			{
				injure = (enemy as BiaoCheItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				ret = injure;
				(enemy as BiaoCheItem).CurrentLifeV -= injure;
				(enemy as BiaoCheItem).CurrentLifeV = Global.GMax((enemy as BiaoCheItem).CurrentLifeV, 0);
				double enemyLife = (double)(enemy as BiaoCheItem).CurrentLifeV;
				(enemy as BiaoCheItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				GameManager.SystemServerEvents.AddEvent(string.Format("镖车减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);
				if ((enemy as BiaoCheItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("镖车死亡, roleID={0}", (enemy as BiaoCheItem).BiaoCheID), EventLevels.Debug);
					BiaoCheManager.ProcessBiaoCheDead(sl, pool, client, enemy as BiaoCheItem);
				}
				if ((enemy as BiaoCheItem).AttackedRoleID >= 0 && (enemy as BiaoCheItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient findClient = GameManager.ClientMgr.FindClient((enemy as BiaoCheItem).AttackedRoleID);
					if (null != findClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (enemy as BiaoCheItem).BiaoCheID, 0, 0, enemyLife, findClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, enemy.BiaoCheID, 0, 0, enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as BiaoCheItem).BiaoCheID, burst, injure, enemyLife, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.BiaoCheID, burst, injure, enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return ret;
		}

		
		public static void NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int enemyX, int enemyY, int burst, int injure, double attackPercent, int addAttack, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			object obj = BiaoCheManager.FindBiaoCheByID(enemy);
			if (null != obj)
			{
				if ((obj as BiaoCheItem).CurrentLifeV > 0)
				{
					injure = (obj as BiaoCheItem).CutLifeV;
					injure = (int)((double)injure * baseRate + (double)addVlue);
					(obj as BiaoCheItem).CurrentLifeV -= injure;
					(obj as BiaoCheItem).CurrentLifeV = Global.GMax((obj as BiaoCheItem).CurrentLifeV, 0);
					double enemyLife = (double)(obj as BiaoCheItem).CurrentLifeV;
					(obj as BiaoCheItem).AttackedRoleID = client.ClientData.RoleID;
					GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
					GameManager.SystemServerEvents.AddEvent(string.Format("镖车减血, Injure={0}, Life={1}", injure, enemyLife), EventLevels.Debug);
					if ((obj as BiaoCheItem).CurrentLifeV <= 0)
					{
						GameManager.SystemServerEvents.AddEvent(string.Format("镖车死亡, roleID={0}", (obj as BiaoCheItem).BiaoCheID), EventLevels.Debug);
						BiaoCheManager.ProcessBiaoCheDead(sl, pool, client, obj as BiaoCheItem);
					}
					if ((obj as BiaoCheItem).AttackedRoleID >= 0 && (obj as BiaoCheItem).AttackedRoleID != client.ClientData.RoleID)
					{
						GameClient findClient = GameManager.ClientMgr.FindClient((obj as BiaoCheItem).AttackedRoleID);
						if (null != findClient)
						{
							GameManager.ClientMgr.NotifySpriteInjured(sl, pool, findClient, findClient.ClientData.MapCode, findClient.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, 0, 0, enemyLife, findClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							ClientManager.NotifySelfEnemyInjured(sl, pool, findClient, findClient.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, 0, 0, enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, burst, injure, enemyLife, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
					ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, burst, injure, enemyLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					if (!client.ClientData.DisableChangeRolePurpleName)
					{
						GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
					}
				}
			}
		}

		
		private static void ProcessBiaoCheDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem biaoCheItem)
		{
			if (!biaoCheItem.HandledDead)
			{
				biaoCheItem.HandledDead = true;
				biaoCheItem.BiaoCheDeadTicks = TimeUtil.NOW();
				GameClient findClient;
				if (biaoCheItem.AttackedRoleID >= 0 && biaoCheItem.AttackedRoleID != client.ClientData.RoleID)
				{
					findClient = GameManager.ClientMgr.FindClient(biaoCheItem.AttackedRoleID);
					if (null != findClient)
					{
						client = findClient;
					}
				}
				int yinLiang = 0;
				int experience = 0;
				int yaJin = 0;
				Global.GetYaBiaoReward(biaoCheItem.YaBiaoID, out yinLiang, out experience, out yaJin);
				yinLiang /= 2;
				experience = 0;
				int jieBiaoNum = Global.IncTotayJieBiaoNum(client);
				if (experience > 0)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)experience, true, false, false, "none");
				}
				if (yinLiang > 0)
				{
					yinLiang = Global.FilterValue(client, yinLiang);
					GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, yinLiang, "押镖奖励", false);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.YinLiang,
						yinLiang
					}), EventLevels.Record);
				}
				GameManager.DBCmdMgr.AddDBCmd(10058, string.Format("{0}:{1}", biaoCheItem.OwnerRoleID, 1), null, client.ServerId);
				findClient = GameManager.ClientMgr.FindClient(biaoCheItem.OwnerRoleID);
				if (null != findClient)
				{
					if (null != findClient.ClientData.MyYaBiaoData)
					{
						findClient.ClientData.MyYaBiaoData.State = 1;
						GameManager.ClientMgr.NotifyYaBiaoData(findClient);
					}
				}
				else
				{
					string gmCmdData = string.Format("-setybstate2 {0} 1", biaoCheItem.OwnerRoleID);
					GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						client.ClientData.RoleID,
						"",
						0,
						"",
						0,
						gmCmdData,
						0,
						0,
						-1
					}), null, 0);
				}
				Global.BroadcastKillBiaoCheHint(client, biaoCheItem);
			}
		}

		
		public static int NotAttackYaBiaoID = -1;

		
		private static Dictionary<int, BiaoCheItem> _RoleID2BiaoCheDict = new Dictionary<int, BiaoCheItem>();

		
		private static Dictionary<int, BiaoCheItem> _ID2BiaoCheDict = new Dictionary<int, BiaoCheItem>();
	}
}
