using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000749 RID: 1865
	public class MapGrid
	{
		// Token: 0x06002EE7 RID: 12007 RVA: 0x0029FE74 File Offset: 0x0029E074
		public MapGrid(int mapCode, int mapWidth, int mapHeight, int mapGridWidth, int mapGridHeight, GameMap gameMap)
		{
			this.MapCode = mapCode;
			this.MapWidth = mapWidth;
			this.MapHeight = mapHeight;
			this._MapGridWidth = mapGridWidth;
			this._MapGridHeight = mapGridHeight;
			this._MapGridXNum = (this.MapWidth - 1) / this._MapGridWidth + 1;
			this._MapGridYNum = (this.MapHeight - 1) / this._MapGridHeight + 1;
			this._MapGridTotalNum = this._MapGridXNum * this._MapGridYNum;
			this._GameMap = gameMap;
			this.MyMapGridSpriteItem = new MapGridSpriteItem[this._MapGridTotalNum];
			for (int i = 0; i < this.MyMapGridSpriteItem.Length; i++)
			{
				this.MyMapGridSpriteItem[i].GridLock = new object();
				this.MyMapGridSpriteItem[i].ObjsList = new List<object>();
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x06002EE8 RID: 12008 RVA: 0x0029FF8C File Offset: 0x0029E18C
		public int MapGridWidth
		{
			get
			{
				return this._MapGridWidth;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x06002EE9 RID: 12009 RVA: 0x0029FFA4 File Offset: 0x0029E1A4
		public int MapGridHeight
		{
			get
			{
				return this._MapGridHeight;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06002EEA RID: 12010 RVA: 0x0029FFBC File Offset: 0x0029E1BC
		public int MapGridXNum
		{
			get
			{
				return this._MapGridXNum;
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06002EEB RID: 12011 RVA: 0x0029FFD4 File Offset: 0x0029E1D4
		public int MapGridYNum
		{
			get
			{
				return this._MapGridYNum;
			}
		}

		// Token: 0x06002EEC RID: 12012 RVA: 0x0029FFEC File Offset: 0x0029E1EC
		private int GetGridIndex(int gridX, int gridY)
		{
			return this._MapGridXNum * gridY + gridX;
		}

		// Token: 0x06002EED RID: 12013 RVA: 0x002A0008 File Offset: 0x0029E208
		private void ChangeMapGridsSpriteNum(int index, IObject obj, short addOrSubNum)
		{
			this.MyMapGridSpriteItem[index].ObjsListReadOnly = null;
			switch (obj.ObjectType)
			{
			case ObjectTypes.OT_CLIENT:
			{
				MapGridSpriteItem[] myMapGridSpriteItem = this.MyMapGridSpriteItem;
				myMapGridSpriteItem[index].RoleNum = myMapGridSpriteItem[index].RoleNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].RoleNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].RoleNum);
				break;
			}
			case ObjectTypes.OT_MONSTER:
			{
				MapGridSpriteItem[] myMapGridSpriteItem2 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem2[index].MonsterNum = myMapGridSpriteItem2[index].MonsterNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].MonsterNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].MonsterNum);
				break;
			}
			case ObjectTypes.OT_GOODSPACK:
			{
				MapGridSpriteItem[] myMapGridSpriteItem3 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem3[index].GoodsPackNum = myMapGridSpriteItem3[index].GoodsPackNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].GoodsPackNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].GoodsPackNum);
				break;
			}
			case ObjectTypes.OT_BIAOCHE:
			{
				MapGridSpriteItem[] myMapGridSpriteItem4 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem4[index].BiaoCheNum = myMapGridSpriteItem4[index].BiaoCheNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].BiaoCheNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].BiaoCheNum);
				break;
			}
			case ObjectTypes.OT_JUNQI:
			{
				MapGridSpriteItem[] myMapGridSpriteItem5 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem5[index].JunQiNum = myMapGridSpriteItem5[index].JunQiNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].JunQiNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].JunQiNum);
				break;
			}
			case ObjectTypes.OT_NPC:
			{
				MapGridSpriteItem[] myMapGridSpriteItem6 = this.MyMapGridSpriteItem;
				myMapGridSpriteItem6[index].NPCNum = myMapGridSpriteItem6[index].NPCNum + addOrSubNum;
				this.MyMapGridSpriteItem[index].NPCNum = (short)Global.GMax(0, (int)this.MyMapGridSpriteItem[index].NPCNum);
				break;
			}
			}
		}

		// Token: 0x06002EEE RID: 12014 RVA: 0x002A01FC File Offset: 0x0029E3FC
		public int GetRoleNum(int gridX, int gridY)
		{
			int roleNum = 0;
			int gridIndex = this.GetGridIndex(gridX, gridY);
			lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
			{
				roleNum = (int)this.MyMapGridSpriteItem[gridIndex].RoleNum;
			}
			return roleNum;
		}

		// Token: 0x06002EEF RID: 12015 RVA: 0x002A0274 File Offset: 0x0029E474
		public void GetObjectsNum(int gridX, int gridY, out int totalNum, out int roleNum, out int monsterNum, out int nPCNum, out int biaoCheNum, out int junQiNum, out int goodsPackNum, out int decoNum)
		{
			int gridIndex = this.GetGridIndex(gridX, gridY);
			lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
			{
				totalNum = this.MyMapGridSpriteItem[gridIndex].ObjsList.Count;
				roleNum = (int)this.MyMapGridSpriteItem[gridIndex].RoleNum;
				monsterNum = (int)this.MyMapGridSpriteItem[gridIndex].MonsterNum;
				nPCNum = (int)this.MyMapGridSpriteItem[gridIndex].NPCNum;
				biaoCheNum = (int)this.MyMapGridSpriteItem[gridIndex].BiaoCheNum;
				junQiNum = (int)this.MyMapGridSpriteItem[gridIndex].JunQiNum;
				goodsPackNum = (int)this.MyMapGridSpriteItem[gridIndex].GoodsPackNum;
				decoNum = (int)this.MyMapGridSpriteItem[gridIndex].DecoNum;
			}
		}

		// Token: 0x06002EF0 RID: 12016 RVA: 0x002A0374 File Offset: 0x0029E574
		public bool MoveObjectEx(int oldGridX, int oldGridY, int newGridX, int newGridY, IObject obj)
		{
			int oldX = oldGridX * this._MapGridWidth;
			int oldY = oldGridY * this._MapGridHeight;
			int newX = newGridX * this._MapGridWidth;
			int newY = newGridY * this._MapGridHeight;
			return this.MoveObject(oldX, oldY, newX, newY, obj);
		}

		// Token: 0x06002EF1 RID: 12017 RVA: 0x002A03BC File Offset: 0x0029E5BC
		public bool MoveObject(int oldX, int oldY, int newX, int newY, IObject obj)
		{
			bool result;
			if (newX < 0 || newY < 0 || newX >= this.MapWidth || newY >= this.MapHeight)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("坐标超出地图大小: MapCode={0}, newX={1}, newY={2}, Width={3}, Height={4}", new object[]
				{
					this.MapCode,
					newX,
					newY,
					this.MapWidth,
					this.MapHeight
				}), null, true);
				result = false;
			}
			else
			{
				int gridX = newX / this._MapGridWidth;
				int gridY = newY / this._MapGridHeight;
				int oldGridIndex = -1;
				lock (this.ModifyMutex)
				{
					this.HandleTracking(oldX, oldY, newX, newY, obj);
					lock (this._Obj2GridDict)
					{
						if (!this._Obj2GridDict.TryGetValue(obj, out oldGridIndex))
						{
							oldGridIndex = -1;
						}
					}
					int gridIndex = this.GetGridIndex(gridX, gridY);
					if (-1 != oldGridIndex && oldGridIndex == gridIndex)
					{
						return true;
					}
					if (-1 != oldGridIndex)
					{
						lock (this.MyMapGridSpriteItem[oldGridIndex].GridLock)
						{
							if (!this.MyMapGridSpriteItem[oldGridIndex].ObjsList.Remove(obj))
							{
								return false;
							}
							this.ChangeMapGridsSpriteNum(oldGridIndex, obj, -1);
						}
					}
					lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
					{
						this.MyMapGridSpriteItem[gridIndex].ObjsList.Add(obj);
						this.ChangeMapGridsSpriteNum(gridIndex, obj, 1);
					}
					lock (this._Obj2GridDict)
					{
						this._Obj2GridDict[obj] = gridIndex;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002EF2 RID: 12018 RVA: 0x002A06A8 File Offset: 0x0029E8A8
		private void HandleTracking(int oldX, int oldY, int newX, int newY, IObject obj)
		{
			if (obj is GameClient)
			{
				lock (obj)
				{
					if ((obj as GameClient).ClientData.TrackingRoleIDList.Count != 0)
					{
						foreach (int rid in (obj as GameClient).ClientData.TrackingRoleIDList)
						{
							GameClient tClient = GameManager.ClientMgr.FindClient(rid);
							if (tClient != null && tClient.ClientData.HideGM != 0)
							{
								lock (this._Obj2GridDict)
								{
									int gridIndex = -1;
									if (!this._Obj2GridDict.TryGetValue(tClient, out gridIndex))
									{
										continue;
									}
								}
								tClient.ClientData.PosX = (obj as GameClient).ClientData.PosX;
								tClient.ClientData.PosY = (obj as GameClient).ClientData.PosY;
								this.MoveObject(oldX, oldY, newX, newY, tClient);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002EF3 RID: 12019 RVA: 0x002A086C File Offset: 0x0029EA6C
		public bool RemoveObject(IObject obj)
		{
			lock (this.ModifyMutex)
			{
				int oldGridIndex = -1;
				lock (this._Obj2GridDict)
				{
					if (!this._Obj2GridDict.TryGetValue(obj, out oldGridIndex))
					{
						oldGridIndex = -1;
					}
					else
					{
						this._Obj2GridDict.Remove(obj);
					}
				}
				if (-1 == oldGridIndex)
				{
					return false;
				}
				lock (this.MyMapGridSpriteItem[oldGridIndex].GridLock)
				{
					if (this.MyMapGridSpriteItem[oldGridIndex].ObjsList.Remove(obj))
					{
						this.ChangeMapGridsSpriteNum(oldGridIndex, obj, -1);
					}
				}
			}
			return true;
		}

		// Token: 0x06002EF4 RID: 12020 RVA: 0x002A09C4 File Offset: 0x0029EBC4
		public int GetGridClientCountForConsole()
		{
			int result;
			lock (this._Obj2GridDict)
			{
				result = this._Obj2GridDict.Count((KeyValuePair<object, int> x) => x.Key is GameClient);
			}
			return result;
		}

		// Token: 0x06002EF5 RID: 12021 RVA: 0x002A0A34 File Offset: 0x0029EC34
		public List<object> FindObjects(int gridX, int gridY)
		{
			int gridIndex = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (gridIndex < 0 || gridIndex >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> listObjs2 = null;
				lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
				{
					listObjs2 = this.MyMapGridSpriteItem[gridIndex].ObjsList;
					if (listObjs2.Count == 0)
					{
						return null;
					}
					if (this.FlagOptimizeFindObjects)
					{
						if (null == this.MyMapGridSpriteItem[gridIndex].ObjsListReadOnly)
						{
							this.MyMapGridSpriteItem[gridIndex].ObjsListReadOnly = listObjs2.GetRange(0, listObjs2.Count);
						}
						return this.MyMapGridSpriteItem[gridIndex].ObjsListReadOnly;
					}
					listObjs2 = listObjs2.GetRange(0, listObjs2.Count);
				}
				result = listObjs2;
			}
			return result;
		}

		// Token: 0x06002EF6 RID: 12022 RVA: 0x002A0B58 File Offset: 0x0029ED58
		public List<object> FindGoodsPackItems(int gridX, int gridY)
		{
			int gridIndex = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (gridIndex < 0 || gridIndex >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> listObjs2 = null;
				lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
				{
					if (this.MyMapGridSpriteItem[gridIndex].GoodsPackNum > 0)
					{
						listObjs2 = this.MyMapGridSpriteItem[gridIndex].ObjsList.GetRange(0, this.MyMapGridSpriteItem[gridIndex].ObjsList.Count);
					}
				}
				result = listObjs2;
			}
			return result;
		}

		// Token: 0x06002EF7 RID: 12023 RVA: 0x002A0C2C File Offset: 0x0029EE2C
		public List<object> FindGameClient(int gridX, int gridY)
		{
			int gridIndex = this._MapGridXNum * gridY + gridX;
			List<object> result;
			if (gridIndex < 0 || gridIndex >= this._MapGridTotalNum)
			{
				result = null;
			}
			else
			{
				List<object> listObjs2 = null;
				lock (this.MyMapGridSpriteItem[gridIndex].GridLock)
				{
					if (this.MyMapGridSpriteItem[gridIndex].RoleNum > 0)
					{
						listObjs2 = this.MyMapGridSpriteItem[gridIndex].ObjsList.GetRange(0, this.MyMapGridSpriteItem[gridIndex].ObjsList.Count);
					}
				}
				result = listObjs2;
			}
			return result;
		}

		// Token: 0x06002EF8 RID: 12024 RVA: 0x002A0D00 File Offset: 0x0029EF00
		public List<object> FindObjects(int toX, int toY, int radius)
		{
			List<object> result;
			if (toX < 0 || toY < 0 || toX >= this.MapWidth || toY >= this.MapHeight)
			{
				result = null;
			}
			else
			{
				int gridX = toX / this._MapGridWidth;
				int gridY = toY / this._MapGridHeight;
				List<object> listObjs = new List<object>();
				int gridRadiusWidthNum = (radius - 1) / this._MapGridWidth + 1;
				int gridRadiusHeightNum = (radius - 1) / this._MapGridHeight + 1;
				int lowGridY = gridY - gridRadiusHeightNum;
				int hiGridY = gridY + gridRadiusHeightNum;
				int lowGridX = gridX - gridRadiusWidthNum;
				int hiGridX = gridX + gridRadiusWidthNum;
				for (int y = lowGridY; y <= hiGridY; y++)
				{
					for (int x = lowGridX; x <= hiGridX; x++)
					{
						List<object> listObjs2 = this.FindObjects(x, y);
						if (null != listObjs2)
						{
							listObjs.AddRange(listObjs2);
						}
					}
				}
				result = listObjs;
			}
			return result;
		}

		// Token: 0x06002EF9 RID: 12025 RVA: 0x002A0DEC File Offset: 0x0029EFEC
		public bool CanMove(ObjectTypes objType, int gridX, int gridY, int holdGridNum, byte holdBitSet = 0)
		{
			bool result;
			if (objType == ObjectTypes.OT_BIAOCHE)
			{
				result = true;
			}
			else if (objType == ObjectTypes.OT_FAKEROLE)
			{
				result = true;
			}
			else
			{
				int totalNum = 0;
				int roleNum = 0;
				int monsterNum = 0;
				int nPCNum = 0;
				int biaoCheNum = 0;
				int junQiNum = 0;
				int goodsPackNum = 0;
				int decNum = 0;
				this.GetObjectsNum(gridX, gridY, out totalNum, out roleNum, out monsterNum, out nPCNum, out biaoCheNum, out junQiNum, out goodsPackNum, out decNum);
				if (totalNum <= 0)
				{
					result = true;
				}
				else if (objType == ObjectTypes.OT_CLIENT)
				{
					bool canMove = true;
					if (this._GameMap.HoldRole > 0 || 1 == (holdBitSet & 1))
					{
						if (roleNum > holdGridNum)
						{
							canMove = false;
						}
					}
					if (this._GameMap.HoldMonster > 0 || 2 == (holdBitSet & 2))
					{
						if (monsterNum > holdGridNum)
						{
							canMove = false;
						}
					}
					if (this._GameMap.HoldNPC > 0 || 4 == (holdBitSet & 4))
					{
						if (nPCNum > holdGridNum || junQiNum > holdGridNum)
						{
							canMove = false;
						}
					}
					result = canMove;
				}
				else if (objType == ObjectTypes.OT_MONSTER)
				{
					bool canMove = true;
					if (roleNum > holdGridNum)
					{
						canMove = false;
					}
					if (monsterNum > holdGridNum)
					{
						canMove = false;
					}
					if (nPCNum > holdGridNum || junQiNum > holdGridNum)
					{
						canMove = false;
					}
					result = canMove;
				}
				else if (objType == ObjectTypes.OT_GOODSPACK)
				{
					bool canMove = true;
					if (goodsPackNum > holdGridNum)
					{
						canMove = false;
					}
					result = canMove;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x04003C80 RID: 15488
		private object ModifyMutex = new object();

		// Token: 0x04003C81 RID: 15489
		public GameMap _GameMap = null;

		// Token: 0x04003C82 RID: 15490
		public bool FlagOptimizeFindObjects;

		// Token: 0x04003C83 RID: 15491
		private int MapCode;

		// Token: 0x04003C84 RID: 15492
		private int MapWidth;

		// Token: 0x04003C85 RID: 15493
		private int MapHeight;

		// Token: 0x04003C86 RID: 15494
		private int _MapGridWidth;

		// Token: 0x04003C87 RID: 15495
		private int _MapGridHeight;

		// Token: 0x04003C88 RID: 15496
		private int _MapGridXNum = 0;

		// Token: 0x04003C89 RID: 15497
		private int _MapGridYNum = 0;

		// Token: 0x04003C8A RID: 15498
		private int _MapGridTotalNum = 0;

		// Token: 0x04003C8B RID: 15499
		private Dictionary<object, int> _Obj2GridDict = new Dictionary<object, int>(1000);

		// Token: 0x04003C8C RID: 15500
		private MapGridSpriteItem[] MyMapGridSpriteItem = null;
	}
}
