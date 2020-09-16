using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class ChuanQiUtils
	{
		
		public static void TurnTo(IObject obj, Dircetions nDir)
		{
			if (nDir != obj.CurrentDir)
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[obj.CurrentMapCode];
				Point grid = obj.CurrentGrid;
				int posX = (int)((double)gameMap.MapGridWidth * grid.X + (double)(gameMap.MapGridWidth / 2));
				int posY = (int)((double)gameMap.MapGridHeight * grid.Y + (double)(gameMap.MapGridHeight / 2));
				List<object> listObjs = Global.GetAll9Clients(obj);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj, obj.CurrentMapCode, obj.CurrentCopyMapID, obj.GetObjectID(), (int)nDir, 0, posX, posY, 0, 0, 114, listObjs);
				if (obj is Monster)
				{
					Monster monster = obj as Monster;
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Direction = (double)nDir;
					monster.Action = GActions.Stand;
				}
			}
		}

		
		protected static void WalkNextPos(IObject obj, Dircetions nDir, out int nX, out int nY)
		{
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			nX = nCurrX;
			nY = nCurrY;
			switch (nDir)
			{
			case Dircetions.DR_UP:
				nX = nCurrX;
				nY = nCurrY + 1;
				break;
			case Dircetions.DR_UPRIGHT:
				nX = nCurrX + 1;
				nY = nCurrY + 1;
				break;
			case Dircetions.DR_RIGHT:
				nX = nCurrX + 1;
				nY = nCurrY;
				break;
			case Dircetions.DR_DOWNRIGHT:
				nX = nCurrX + 1;
				nY = nCurrY - 1;
				break;
			case Dircetions.DR_DOWN:
				nX = nCurrX;
				nY = nCurrY - 1;
				break;
			case Dircetions.DR_DOWNLEFT:
				nX = nCurrX - 1;
				nY = nCurrY - 1;
				break;
			case Dircetions.DR_LEFT:
				nX = nCurrX - 1;
				nY = nCurrY;
				break;
			case Dircetions.DR_UPLEFT:
				nX = nCurrX - 1;
				nY = nCurrY + 1;
				break;
			}
		}

		
		public static bool WalkTo(IObject obj, Dircetions nDir)
		{
			if (obj is Monster)
			{
				if (obj is Monster && (obj as Monster).IsMonsterDongJie())
				{
					return false;
				}
				long nowTicks = TimeUtil.NOW();
				if (obj is Monster && nowTicks - (obj as Monster).LastActionTick <= (long)(obj as Monster).GetMovingNeedTick())
				{
					return false;
				}
			}
			int nX;
			int nY;
			ChuanQiUtils.WalkNextPos(obj, nDir, out nX, out nY);
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			string pathStr = string.Format("{0}_{1}|{2}_{3}", new object[]
			{
				nCurrX,
				nCurrY,
				nX,
				nY
			});
			bool fResult = ChuanQiUtils.WalkXY(obj, nX, nY, nDir, pathStr);
			if (fResult)
			{
			}
			return fResult;
		}

		
		public static bool RunTo1(IObject obj, Dircetions nDir)
		{
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			int nX = nCurrX;
			int nY = nCurrY;
			int nWalk = 2;
			string pathStr = string.Format("{0}_{1}", nCurrX, nCurrY);
			for (int i = 0; i < nWalk; i++)
			{
				switch (nDir)
				{
				case Dircetions.DR_UP:
					nY++;
					break;
				case Dircetions.DR_UPRIGHT:
					nX++;
					nY++;
					break;
				case Dircetions.DR_RIGHT:
					nX++;
					break;
				case Dircetions.DR_DOWNRIGHT:
					nX++;
					nY--;
					break;
				case Dircetions.DR_DOWN:
					nY--;
					break;
				case Dircetions.DR_DOWNLEFT:
					nX--;
					nY--;
					break;
				case Dircetions.DR_LEFT:
					nX--;
					break;
				case Dircetions.DR_UPLEFT:
					nX--;
					nY++;
					break;
				}
				if (!ChuanQiUtils.CanMove(obj, nX, nY))
				{
					return false;
				}
				pathStr += string.Format("|{0}_{1}", nX, nY);
			}
			return ChuanQiUtils.RunXY1(obj, nX, nY, nDir, pathStr);
		}

		
		public static bool RunTo(IObject obj, Dircetions nDir)
		{
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			int nX = nCurrX;
			int nY = nCurrY;
			int nWalk = 2;
			string pathStr = string.Format("{0}_{1}", nCurrX, nCurrY);
			for (int i = 0; i < nWalk; i++)
			{
				switch (nDir)
				{
				case Dircetions.DR_UP:
					nY++;
					break;
				case Dircetions.DR_UPRIGHT:
					nX++;
					nY++;
					break;
				case Dircetions.DR_RIGHT:
					nX++;
					break;
				case Dircetions.DR_DOWNRIGHT:
					nX++;
					nY--;
					break;
				case Dircetions.DR_DOWN:
					nY--;
					break;
				case Dircetions.DR_DOWNLEFT:
					nX--;
					nY--;
					break;
				case Dircetions.DR_LEFT:
					nX--;
					break;
				case Dircetions.DR_UPLEFT:
					nX--;
					nY++;
					break;
				}
				if (!ChuanQiUtils.CanMove(obj, nX, nY))
				{
					return false;
				}
				pathStr += string.Format("|{0}_{1}", nX, nY);
			}
			return ChuanQiUtils.RunXY(obj, nX, nY, nDir, pathStr);
		}

		
		protected static bool WalkXY(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			bool result;
			if (!ChuanQiUtils.CanMove(obj, nX, nY))
			{
				result = false;
			}
			else
			{
				Point grid = obj.CurrentGrid;
				int nCurrX = (int)grid.X;
				int nCurrY = (int)grid.Y;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
				if (mapGrid.MoveObjectEx(nCurrX, nCurrY, nX, nY, obj))
				{
					ChuanQiUtils.NotifyOthersMyMoving(obj, pathStr, nCurrX, nCurrY, nX, nY, nDir);
					obj.CurrentGrid = new Point((double)nX, (double)nY);
					obj.CurrentDir = nDir;
					ChuanQiUtils.Notify9Grid(obj, false);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		protected static bool RunXY1(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			bool result;
			if (!ChuanQiUtils.CanMove(obj, nX, nY))
			{
				result = false;
			}
			else
			{
				Point grid = obj.CurrentGrid;
				int nCurrX = (int)grid.X;
				int nCurrY = (int)grid.Y;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
				if (mapGrid.MoveObjectEx(nCurrX, nCurrY, nX, nY, obj))
				{
					ChuanQiUtils.NotifyOthersMyMoving1(obj, pathStr, nCurrX, nCurrY, nX, nY, nDir);
					obj.CurrentGrid = new Point((double)nX, (double)nY);
					obj.CurrentDir = nDir;
					ChuanQiUtils.Notify9Grid(obj, false);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		
		protected static bool RunXY(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			bool result;
			if (mapGrid.MoveObjectEx(nCurrX, nCurrY, nX, nY, obj))
			{
				ChuanQiUtils.NotifyOthersMyMoving(obj, pathStr, nCurrX, nCurrY, nX, nY, nDir);
				obj.CurrentGrid = new Point((double)nX, (double)nY);
				obj.CurrentDir = nDir;
				ChuanQiUtils.Notify9Grid(obj, false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public static bool TransportTo(IObject obj, int nX, int nY, Dircetions nDir, int oldMapCode, string pathStr = "")
		{
			Point grid = obj.CurrentGrid;
			int nCurrX = (int)grid.X;
			int nCurrY = (int)grid.Y;
			if (oldMapCode > 0 && oldMapCode != obj.CurrentMapCode)
			{
				MapGrid oldMapGrid = GameManager.MapGridMgr.DictGrids[oldMapCode];
				if (oldMapGrid != null)
				{
					oldMapGrid.RemoveObject(obj);
				}
				nCurrX = -1;
				nCurrY = -1;
			}
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			bool result;
			if (mapGrid.MoveObjectEx(nCurrX, nCurrY, nX, nY, obj))
			{
				obj.CurrentGrid = new Point((double)nX, (double)nY);
				obj.CurrentDir = nDir;
				if (obj is Monster && (obj as Monster).MonsterType == 1001)
				{
					GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, (int)obj.CurrentPos.X, (int)obj.CurrentPos.Y, (int)nDir, 159, 0);
				}
				ChuanQiUtils.Notify9Grid(obj, true);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		public static bool CanMonsterMoveOnCopyMap(Monster monster, int nX, int nY)
		{
			bool result;
			if (monster.CopyMapID <= 0)
			{
				result = false;
			}
			else if (Global.InOnlyObs(monster.ObjectType, monster.CurrentMapCode, nX, nY))
			{
				result = false;
			}
			else
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[monster.CurrentMapCode];
				if (mapGrid.CanMove(monster.ObjectType, nX, nY, 0, 0))
				{
					result = true;
				}
				else
				{
					bool canMove = true;
					List<object> objsList = mapGrid.FindObjects(nX, nY);
					if (null != objsList)
					{
						for (int objIndex = 0; objIndex < objsList.Count; objIndex++)
						{
							if (objsList[objIndex] != monster)
							{
								if (objsList[objIndex] is GameClient && (objsList[objIndex] as GameClient).CurrentCopyMapID == monster.CopyMapID)
								{
									canMove = false;
									break;
								}
								if (objsList[objIndex] is NPC)
								{
									canMove = false;
									break;
								}
								if (objsList[objIndex] is Monster && (objsList[objIndex] as Monster).CopyMapID == monster.CopyMapID)
								{
									canMove = false;
									break;
								}
							}
						}
					}
					result = canMove;
				}
			}
			return result;
		}

		
		public static bool CanMove(IObject obj, int nX, int nY)
		{
			bool result;
			if (obj is Monster && (obj as Monster).CopyMapID > 0)
			{
				Monster monsterObj = obj as Monster;
				result = (monsterObj.CurrentMapCode == 5100 || (1502 == monsterObj.MonsterType && monsterObj.Tag is CompMineTruckConfig) || ChuanQiUtils.CanMonsterMoveOnCopyMap(monsterObj, nX, nY));
			}
			else
			{
				result = !Global.InObsByGridXY(obj.ObjectType, obj.CurrentMapCode, nX, nY, 0, 0);
			}
			return result;
		}

		
		private static void Notify9Grid(IObject obj, bool force = false)
		{
			if (obj is Monster)
			{
			}
		}

		
		private static void NotifyOthersMyMoving(IObject obj, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, Dircetions direction)
		{
			if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
					monster.Direction = (double)direction;
					monster.Action = GActions.Walk;
					GameMap gameMap = GameManager.MapMgr.DictMaps[monster.MonsterZoneNode.MapCode];
					int fromPosX = gameMap.MapGridWidth * nSrcGridX + gameMap.MapGridWidth / 2;
					int fromPosY = gameMap.MapGridHeight * nSrcGridY + gameMap.MapGridHeight / 2;
					int toPosX = gameMap.MapGridWidth * nDestGridX + gameMap.MapGridWidth / 2;
					int toPosY = gameMap.MapGridHeight * nDestGridY + gameMap.MapGridHeight / 2;
					string zipPathString = DataHelper.ZipStringToBase64(pathString);
					GameManager.ClientMgr.NotifyOthersToMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, Global.GetMonsterStartMoveTicks(monster), fromPosX, fromPosY, 1, toPosX, toPosY, 107, monster.MoveSpeed, zipPathString, null);
				}
			}
		}

		
		private static void NotifyOthersMyMoving1(IObject obj, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, Dircetions direction)
		{
			if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
					monster.Direction = (double)direction;
					monster.Action = GActions.Run;
					GameMap gameMap = GameManager.MapMgr.DictMaps[monster.MonsterZoneNode.MapCode];
					int fromPosX = gameMap.MapGridWidth * nSrcGridX + gameMap.MapGridWidth / 2;
					int fromPosY = gameMap.MapGridHeight * nSrcGridY + gameMap.MapGridHeight / 2;
					int toPosX = gameMap.MapGridWidth * nDestGridX + gameMap.MapGridWidth / 2;
					int toPosY = gameMap.MapGridHeight * nDestGridY + gameMap.MapGridHeight / 2;
					string zipPathString = DataHelper.ZipStringToBase64(pathString);
					GameManager.ClientMgr.NotifyOthersToMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, Global.GetMonsterStartMoveTicks(monster), fromPosX, fromPosY, 2, toPosX, toPosY, 107, monster.MoveSpeed, zipPathString, null);
				}
			}
		}

		
		public static Point HitFly(GameClient client, IObject enemy, int gridNum)
		{
			bool isDead = false;
			if (enemy is Monster)
			{
				isDead = ((enemy as Monster).VLife <= 0.0);
				if (101 != (enemy as Monster).MonsterType && 1001 != (enemy as Monster).MonsterType && 1801 != (enemy as Monster).MonsterType)
				{
					return new Point(-1.0, -1.0);
				}
			}
			Point grid = enemy.CurrentGrid;
			Point selfGrid = client.CurrentGrid;
			int direction = (int)Global.GetDirectionByAspect((int)grid.X, (int)grid.Y, (int)selfGrid.X, (int)selfGrid.Y);
			List<Point> gridList = Global.GetGridPointByDirection(direction, (int)grid.X, (int)grid.Y, gridNum);
			Point result;
			if (null == gridList)
			{
				result = new Point(-1.0, -1.0);
			}
			else
			{
				if (!isDead)
				{
					for (int i = 0; i < gridList.Count; i++)
					{
						if (Global.InOnlyObs(enemy.ObjectType, client.ClientData.MapCode, (int)gridList[i].X, (int)gridList[i].Y))
						{
							gridList.RemoveRange(i, gridList.Count - i);
							break;
						}
					}
					if (gridList.Count <= 0)
					{
						return new Point(-1.0, -1.0);
					}
				}
				Point toGrid = gridList[gridList.Count - 1];
				if (!ChuanQiUtils.TransportTo(enemy, (int)toGrid.X, (int)toGrid.Y, enemy.CurrentDir, enemy.CurrentMapCode, ""))
				{
					result = new Point(-1.0, -1.0);
				}
				else
				{
					result = toGrid;
				}
			}
			return result;
		}

		
		public static Point MonsterHitFly(Monster attacker, GameClient injurer, int gridNum)
		{
			bool isDead = false;
			Point attackGrid = attacker.CurrentGrid;
			Point injureGrid = injurer.CurrentGrid;
			int direction = (int)Global.GetDirectionByAspect((int)injureGrid.X, (int)injureGrid.Y, (int)attackGrid.X, (int)attackGrid.Y);
			List<Point> gridList = Global.GetGridPointByDirection(direction, (int)injureGrid.X, (int)injureGrid.Y, gridNum);
			Point result;
			if (null == gridList)
			{
				result = new Point(-1.0, -1.0);
			}
			else
			{
				if (!isDead)
				{
					for (int i = 0; i < gridList.Count; i++)
					{
						if (Global.InOnlyObs(attacker.ObjectType, injurer.ClientData.MapCode, (int)gridList[i].X, (int)gridList[i].Y))
						{
							gridList.RemoveRange(i, gridList.Count - i);
							break;
						}
					}
					if (gridList.Count <= 0)
					{
						return new Point(-1.0, -1.0);
					}
				}
				Point toGrid = gridList[gridList.Count - 1];
				if (!ChuanQiUtils.TransportTo(injurer, (int)toGrid.X, (int)toGrid.Y, injurer.CurrentDir, injurer.CurrentMapCode, ""))
				{
					result = new Point(-1.0, -1.0);
				}
				else
				{
					result = toGrid;
				}
			}
			return result;
		}
	}
}
