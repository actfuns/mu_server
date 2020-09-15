using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	// Token: 0x020007A1 RID: 1953
	public class StoryBoard4Client
	{
		// Token: 0x060032DF RID: 13023 RVA: 0x002D2508 File Offset: 0x002D0708
		public static StoryBoard4Client FindStoryBoard(int roleID)
		{
			StoryBoard4Client storyBd = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBd);
			}
			return storyBd;
		}

		// Token: 0x060032E0 RID: 13024 RVA: 0x002D2564 File Offset: 0x002D0764
		public static void RemoveStoryBoard(int roleID)
		{
			StoryBoard4Client storyBd = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBd))
				{
					StoryBoard4Client.StoryBoardDict.Remove(roleID);
					if (null != storyBd)
					{
						storyBd.Completed = null;
					}
				}
			}
		}

		// Token: 0x060032E1 RID: 13025 RVA: 0x002D25E0 File Offset: 0x002D07E0
		public static StoryBoard4Client StopStoryBoard(int roleID, long clientTicks)
		{
			StoryBoard4Client storyBd = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBd))
				{
					return null;
				}
				StoryBoard4Client.StoryBoardDict.Remove(roleID);
			}
			if (null != storyBd)
			{
				storyBd.Run(clientTicks);
			}
			return storyBd;
		}

		// Token: 0x060032E2 RID: 13026 RVA: 0x002D2668 File Offset: 0x002D0868
		public static void StopStoryBoard(int roleID, int stopIndex)
		{
			StoryBoard4Client storyBd = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBd))
				{
					return;
				}
			}
			if (null != storyBd)
			{
				storyBd.StopOnNextGrid(stopIndex);
			}
		}

		// Token: 0x060032E3 RID: 13027 RVA: 0x002D26D8 File Offset: 0x002D08D8
		public static int GetStoryBoardPathIndex(int roleID)
		{
			StoryBoard4Client storyBd = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBd))
				{
					return 0;
				}
			}
			int result;
			if (null != storyBd)
			{
				result = storyBd.GetStoryBoardPathIndex();
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x060032E4 RID: 13028 RVA: 0x002D2754 File Offset: 0x002D0954
		public static void ClearStoryBoard()
		{
			List<StoryBoard4Client> list = new List<StoryBoard4Client>();
			lock (StoryBoard4Client.StoryBoardDict)
			{
				foreach (StoryBoard4Client sb in StoryBoard4Client.StoryBoardDict.Values)
				{
					list.Add(sb);
				}
				StoryBoard4Client.StoryBoardDict.Clear();
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoard4Client sb = list[i];
				if (null != sb)
				{
					sb.Completed = null;
					sb.Clear();
				}
			}
		}

		// Token: 0x060032E5 RID: 13029 RVA: 0x002D283C File Offset: 0x002D0A3C
		private static long getMyTimer()
		{
			return TimeUtil.NOW();
		}

		// Token: 0x060032E6 RID: 13030 RVA: 0x002D2854 File Offset: 0x002D0A54
		public static void runStoryBoards()
		{
			long currentTicks = StoryBoard4Client.getMyTimer();
			StoryBoard4Client.LastRunStoryTicks = currentTicks;
			List<StoryBoard4Client> list = new List<StoryBoard4Client>();
			lock (StoryBoard4Client.StoryBoardDict)
			{
				foreach (StoryBoard4Client sb in StoryBoard4Client.StoryBoardDict.Values)
				{
					list.Add(sb);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoard4Client sb = list[i];
				if (null != sb)
				{
					sb.Run(currentTicks);
				}
			}
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060032E7 RID: 13031 RVA: 0x002D2938 File Offset: 0x002D0B38
		// (remove) Token: 0x060032E8 RID: 13032 RVA: 0x002D2974 File Offset: 0x002D0B74
		public event StoryBoard4Client.CompletedDelegateHandle _Completed = null;

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060032E9 RID: 13033 RVA: 0x002D29B0 File Offset: 0x002D0BB0
		// (set) Token: 0x060032EA RID: 13034 RVA: 0x002D29C8 File Offset: 0x002D0BC8
		public StoryBoard4Client.CompletedDelegateHandle Completed
		{
			get
			{
				return this._Completed;
			}
			set
			{
				this._Completed = value;
			}
		}

		// Token: 0x060032EB RID: 13035 RVA: 0x002D29D4 File Offset: 0x002D0BD4
		public StoryBoard4Client(int roleID)
		{
			this._RoleID = roleID;
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060032EC RID: 13036 RVA: 0x002D2AC8 File Offset: 0x002D0CC8
		public int RoleID
		{
			get
			{
				return this._RoleID;
			}
		}

		// Token: 0x060032ED RID: 13037 RVA: 0x002D2AE0 File Offset: 0x002D0CE0
		public void Binding()
		{
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.ContainsKey(this._RoleID))
				{
					StoryBoard4Client.StoryBoardDict.Add(this._RoleID, this);
				}
			}
		}

		// Token: 0x060032EE RID: 13038 RVA: 0x002D2B4C File Offset: 0x002D0D4C
		public void UnBinding()
		{
			this.Clear();
		}

		// Token: 0x060032EF RID: 13039 RVA: 0x002D2B58 File Offset: 0x002D0D58
		public void Clear()
		{
			if (-1 != this._RoleID)
			{
				StoryBoard4Client.RemoveStoryBoard(this._RoleID);
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x060032F0 RID: 13040 RVA: 0x002D2B84 File Offset: 0x002D0D84
		public int CurrentX
		{
			get
			{
				return this._LastTargetX;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x060032F1 RID: 13041 RVA: 0x002D2B9C File Offset: 0x002D0D9C
		public int CurrentY
		{
			get
			{
				return this._LastTargetY;
			}
		}

		// Token: 0x060032F2 RID: 13042 RVA: 0x002D2BB4 File Offset: 0x002D0DB4
		public bool Start(GameClient client, List<Point> path, int cellSizeX, int cellSizeY, long elapsedTicks)
		{
			bool result;
			lock (this.mutex)
			{
				if (this._Started)
				{
					result = false;
				}
				else
				{
					this._CellSizeX = cellSizeX;
					this._CellSizeY = cellSizeY;
					this._PathIndex = 0;
					this._LastRunTicks = StoryBoard4Client.getMyTimer() - elapsedTicks;
					this._LastTargetX = client.ClientData.PosX;
					this._LastTargetY = client.ClientData.PosY;
					this._CurrentX = (double)client.ClientData.PosX;
					this._CurrentY = (double)client.ClientData.PosY;
					this._Path = path;
					this._CompletedState = false;
					this._Started = true;
					this._Stopped = false;
					this._LastStopIndex = -1;
					this._FirstPoint = new Point(this._Path[0].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[0].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
					if (this._Path.Count <= 0)
					{
						this._LastPoint = this._FirstPoint;
					}
					else
					{
						this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060032F3 RID: 13043 RVA: 0x002D2D98 File Offset: 0x002D0F98
		private void StopOnNextGrid(int stopIndex)
		{
			lock (this.mutex)
			{
				if (!this._CompletedState)
				{
					if (stopIndex >= 0)
					{
						if (stopIndex < this._Path.Count)
						{
							this._Path.RemoveRange(stopIndex, this._Path.Count - stopIndex);
							if (this._Path.Count <= 0)
							{
								this._LastPoint = this._FirstPoint;
							}
							else
							{
								this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
							}
						}
					}
				}
			}
		}

		// Token: 0x060032F4 RID: 13044 RVA: 0x002D2EC8 File Offset: 0x002D10C8
		public bool IsStopped()
		{
			bool stopped;
			lock (this.mutex)
			{
				stopped = this._Stopped;
			}
			return stopped;
		}

		// Token: 0x060032F5 RID: 13045 RVA: 0x002D2F14 File Offset: 0x002D1114
		public int GetStoryBoardPathIndex()
		{
			int pathIndex;
			lock (this.mutex)
			{
				pathIndex = this._PathIndex;
			}
			return pathIndex;
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x060032F6 RID: 13046 RVA: 0x002D2F60 File Offset: 0x002D1160
		public Point LastPoint
		{
			get
			{
				return this._LastPoint;
			}
		}

		// Token: 0x060032F7 RID: 13047 RVA: 0x002D2F78 File Offset: 0x002D1178
		public void Run(long currentTicks)
		{
			lock (this.mutex)
			{
				if (this._Started)
				{
					if (!this._CompletedState)
					{
						long elapsedTicks = currentTicks - this._LastRunTicks;
						this._LastRunTicks = currentTicks;
						GameClient client = GameManager.ClientMgr.FindClient(this._RoleID);
						double elapsedRate = (double)elapsedTicks / 1000.0;
						double toMoveDist = elapsedRate * this._MovingSpeedPerSec * this.GetClientMoveSpeed(client);
						if (this.StepMove(toMoveDist, client))
						{
							this._CompletedState = true;
							if (null != this._Completed)
							{
								this._Completed(this, null);
							}
						}
					}
				}
			}
		}

		// Token: 0x060032F8 RID: 13048 RVA: 0x002D3064 File Offset: 0x002D1264
		private double GetClientMoveSpeed(GameClient client)
		{
			double result;
			if (null != client)
			{
				result = Math.Max(0.1, Math.Min(2.5, client.ClientData.MoveSpeed));
			}
			else
			{
				result = 1.0;
			}
			return result;
		}

		// Token: 0x060032F9 RID: 13049 RVA: 0x002D30B4 File Offset: 0x002D12B4
		private static long GetNeedTicks(bool needWalking, int dir)
		{
			int speed = needWalking ? 225 : 125;
			long result;
			if (dir == 0 || 2 == dir || 4 == dir || 6 == dir)
			{
				result = (long)((int)((float)speed / 1.414213f));
			}
			else
			{
				result = (long)speed;
			}
			return result;
		}

		// Token: 0x060032FA RID: 13050 RVA: 0x002D3100 File Offset: 0x002D1300
		private static int CalcDirection(int x1, int y1, int x2, int y2)
		{
			int result;
			if (x1 == x2)
			{
				if (y2 > y1)
				{
					result = 0;
				}
				else
				{
					result = 4;
				}
			}
			else if (y1 == y2)
			{
				if (x2 > x1)
				{
					result = 2;
				}
				else
				{
					result = 6;
				}
			}
			else if (x1 + 1 == x2 && y1 - 1 == y2)
			{
				result = 3;
			}
			else if (x1 + 1 == x2 && y1 + 1 == y2)
			{
				result = 1;
			}
			else if (x1 - 1 == x2 && y1 + 1 == y2)
			{
				result = 7;
			}
			else if (x1 - 1 == x2 && y1 - 1 == y2)
			{
				result = 5;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		// Token: 0x060032FB RID: 13051 RVA: 0x002D31CC File Offset: 0x002D13CC
		private bool StepMove(double toMoveDist, GameClient client)
		{
			StoryBoard4Client sb = StoryBoard4Client.FindStoryBoard(this._RoleID);
			bool result;
			if (null == sb)
			{
				result = false;
			}
			else
			{
				lock (this.mutex)
				{
					this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
					if (!this.DetectNextGrid())
					{
						result = true;
					}
					else
					{
						double targetX = this._Path[this._PathIndex].X * (double)this._CellSizeX + (double)this._CellSizeX / 2.0;
						double targetY = this._Path[this._PathIndex].Y * (double)this._CellSizeY + (double)this._CellSizeY / 2.0;
						int direction = (int)StoryBoard4Client.GetDirectionByTan(targetX, targetY, (double)this._LastTargetX, (double)this._LastTargetY);
						double dx = targetX - (double)this._LastTargetX;
						double dy = targetY - (double)this._LastTargetY;
						double thisGridStepDist = Math.Sqrt(dx * dx + dy * dy);
						bool needWalking = false;
						if (this._Path.Count <= 1)
						{
							needWalking = true;
						}
						if (null != client)
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
							if (gameMap.InSafeRegionList(this._Path[this._PathIndex]))
							{
								needWalking = true;
							}
						}
						int action = needWalking ? 1 : 2;
						if (needWalking)
						{
							toMoveDist *= 0.8;
						}
						double thisToMoveDist = (thisGridStepDist < toMoveDist) ? thisGridStepDist : toMoveDist;
						double angle = Math.Atan2(dy, dx);
						double speedX = thisToMoveDist * Math.Cos(angle);
						double speedY = thisToMoveDist * Math.Sin(angle);
						this._CurrentX += speedX;
						this._CurrentY += speedY;
						if (null != client)
						{
							client.ClientData.CurrentAction = action;
							if (direction != client.ClientData.RoleDirection)
							{
								client.ClientData.RoleDirection = direction;
							}
						}
						if (thisGridStepDist >= toMoveDist)
						{
							if (null != client)
							{
								GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
								int oldGridX = client.ClientData.PosX / gameMap.MapGridWidth;
								int oldGridY = client.ClientData.PosY / gameMap.MapGridHeight;
								client.ClientData.PosX = (int)this._CurrentX;
								client.ClientData.PosY = (int)this._CurrentY;
								int newGridX = client.ClientData.PosX / gameMap.MapGridWidth;
								int newGridY = client.ClientData.PosY / gameMap.MapGridHeight;
								if (oldGridX != newGridX || oldGridY != newGridY)
								{
									MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
									mapGrid.MoveObjectEx(oldGridX, oldGridY, newGridX, newGridY, client);
								}
							}
							this._LastTargetX = (int)this._CurrentX;
							this._LastTargetY = (int)this._CurrentY;
						}
						else
						{
							this._PathIndex++;
							if (this._PathIndex >= this._Path.Count)
							{
								if (null != client)
								{
									client.ClientData.PosX = (int)targetX;
									client.ClientData.PosY = (int)targetY;
								}
								return true;
							}
							this._LastTargetX = (int)targetX;
							this._LastTargetY = (int)targetY;
							toMoveDist -= thisGridStepDist;
							this.StepMove(toMoveDist, client);
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060032FC RID: 13052 RVA: 0x002D35CC File Offset: 0x002D17CC
		private static double GetDirectionByTan(double targetX, double targetY, double currentX, double currentY)
		{
			int direction = 0;
			if (targetX < currentX)
			{
				if (targetY < currentY)
				{
					direction = 5;
				}
				else if (targetY == currentY)
				{
					direction = 6;
				}
				else if (targetY > currentY)
				{
					direction = 7;
				}
			}
			else if (targetX == currentX)
			{
				if (targetY < currentY)
				{
					direction = 4;
				}
				else if (targetY > currentY)
				{
					direction = 0;
				}
			}
			else if (targetX > currentX)
			{
				if (targetY < currentY)
				{
					direction = 3;
				}
				else if (targetY == currentY)
				{
					direction = 2;
				}
				else if (targetY > currentY)
				{
					direction = 1;
				}
			}
			return (double)direction;
		}

		// Token: 0x060032FD RID: 13053 RVA: 0x002D3690 File Offset: 0x002D1890
		private bool DetectNextGrid()
		{
			bool result;
			if (this._PathIndex <= this._LastStopIndex)
			{
				result = true;
			}
			else if (this.CanMoveToNext())
			{
				result = true;
			}
			else
			{
				this._LastStopIndex = this._PathIndex;
				this._Path.RemoveRange(this._PathIndex, this._Path.Count - this._PathIndex);
				if (this._Path.Count <= 0)
				{
					this._LastPoint = this._FirstPoint;
				}
				else
				{
					this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
				}
				this._Stopped = true;
				result = false;
			}
			return result;
		}

		// Token: 0x060032FE RID: 13054 RVA: 0x002D379C File Offset: 0x002D199C
		private bool CanMoveToNext()
		{
			GameClient client = GameManager.ClientMgr.FindClient(this._RoleID);
			bool result;
			if (null == client)
			{
				result = false;
			}
			else
			{
				this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
				GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
				int gridX = client.ClientData.PosX / gameMap.MapGridWidth;
				int gridY = client.ClientData.PosY / gameMap.MapGridHeight;
				result = ((gridX == (int)this._Path[this._PathIndex].X && gridY == (int)this._Path[this._PathIndex].Y) || !Global.InObsByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, (int)this._Path[this._PathIndex].X, (int)this._Path[this._PathIndex].Y, 0, 0));
			}
			return result;
		}

		// Token: 0x04003EED RID: 16109
		private const float DiagCost = 1.414213f;

		// Token: 0x04003EEE RID: 16110
		private static Dictionary<int, StoryBoard4Client> StoryBoardDict = new Dictionary<int, StoryBoard4Client>();

		// Token: 0x04003EEF RID: 16111
		private static long LastRunStoryTicks = 0L;

		// Token: 0x04003EF1 RID: 16113
		private int _RoleID = -1;

		// Token: 0x04003EF2 RID: 16114
		private object mutex = new object();

		// Token: 0x04003EF3 RID: 16115
		private int _PathIndex = 0;

		// Token: 0x04003EF4 RID: 16116
		private int _LastTargetX = 0;

		// Token: 0x04003EF5 RID: 16117
		private int _LastTargetY = 0;

		// Token: 0x04003EF6 RID: 16118
		private double _CurrentX = 0.0;

		// Token: 0x04003EF7 RID: 16119
		private double _CurrentY = 0.0;

		// Token: 0x04003EF8 RID: 16120
		private int _CellSizeX = GameManager.MapGridWidth;

		// Token: 0x04003EF9 RID: 16121
		private int _CellSizeY = GameManager.MapGridHeight;

		// Token: 0x04003EFA RID: 16122
		private List<Point> _Path = null;

		// Token: 0x04003EFB RID: 16123
		private long _LastRunTicks = 0L;

		// Token: 0x04003EFC RID: 16124
		private bool _Started = false;

		// Token: 0x04003EFD RID: 16125
		private bool _CompletedState = false;

		// Token: 0x04003EFE RID: 16126
		private bool _Stopped = false;

		// Token: 0x04003EFF RID: 16127
		private int _LastStopIndex = -1;

		// Token: 0x04003F00 RID: 16128
		private Point _FirstPoint = new Point(0.0, 0.0);

		// Token: 0x04003F01 RID: 16129
		private Point _LastPoint = new Point(0.0, 0.0);

		// Token: 0x04003F02 RID: 16130
		private double _MovingSpeedPerSec = 500.0;

		// Token: 0x020007A2 RID: 1954
		// (Invoke) Token: 0x06003301 RID: 13057
		public delegate void CompletedDelegateHandle(object sender, EventArgs e);
	}
}
