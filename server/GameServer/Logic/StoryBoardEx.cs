using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using HSGameEngine.Tools.AStarEx;

namespace GameServer.Logic
{
	// Token: 0x020008D3 RID: 2259
	public class StoryBoardEx
	{
		// Token: 0x06004117 RID: 16663 RVA: 0x003BED38 File Offset: 0x003BCF38
		public static bool ContainStoryBoard(string name)
		{
			return StoryBoardEx.StoryBoardDict.ContainsKey(name);
		}

		// Token: 0x06004118 RID: 16664 RVA: 0x003BED58 File Offset: 0x003BCF58
		public static StoryBoardEx FindStoryBoard(string name)
		{
			StoryBoardEx storyBd = null;
			StoryBoardEx.StoryBoardDict.TryGetValue(name, out storyBd);
			return storyBd;
		}

		// Token: 0x06004119 RID: 16665 RVA: 0x003BED7C File Offset: 0x003BCF7C
		public static void RemoveStoryBoard(string name)
		{
			StoryBoardEx sb = StoryBoardEx.FindStoryBoard(name);
			if (null != sb)
			{
				sb.Completed = null;
				sb.Clear();
			}
		}

		// Token: 0x0600411A RID: 16666 RVA: 0x003BEDAC File Offset: 0x003BCFAC
		public static void ClearStoryBoard()
		{
			foreach (StoryBoardEx sb in StoryBoardEx.StoryBoardDict.Values)
			{
				if (null != sb)
				{
					sb.Completed = null;
					sb.Clear();
				}
			}
			StoryBoardEx.StoryBoardDict.Clear();
		}

		// Token: 0x0600411B RID: 16667 RVA: 0x003BEE28 File Offset: 0x003BD028
		private static long getMyTimer()
		{
			return TimeUtil.NOW();
		}

		// Token: 0x0600411C RID: 16668 RVA: 0x003BEE40 File Offset: 0x003BD040
		public static void runStoryBoards()
		{
			long currentTicks = StoryBoardEx.getMyTimer();
			StoryBoardEx.LastRunStoryTicks = currentTicks;
			List<StoryBoardEx> list = new List<StoryBoardEx>();
			foreach (StoryBoardEx sb in StoryBoardEx.StoryBoardDict.Values)
			{
				StoryBoardEx sb;
				list.Add(sb);
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoardEx sb = list[i];
				if (null != sb)
				{
					sb.Run(currentTicks);
				}
			}
		}

		// Token: 0x1700062A RID: 1578
		// (get) Token: 0x0600411D RID: 16669 RVA: 0x003BEEEC File Offset: 0x003BD0EC
		// (set) Token: 0x0600411E RID: 16670 RVA: 0x003BEF04 File Offset: 0x003BD104
		public object Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
			}
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600411F RID: 16671 RVA: 0x003BEF10 File Offset: 0x003BD110
		// (remove) Token: 0x06004120 RID: 16672 RVA: 0x003BEF4C File Offset: 0x003BD14C
		public event StoryBoardEx.CompletedDelegateHandle _Completed = null;

		// Token: 0x1700062B RID: 1579
		// (get) Token: 0x06004121 RID: 16673 RVA: 0x003BEF88 File Offset: 0x003BD188
		// (set) Token: 0x06004122 RID: 16674 RVA: 0x003BEFA0 File Offset: 0x003BD1A0
		public StoryBoardEx.CompletedDelegateHandle Completed
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

		// Token: 0x06004123 RID: 16675 RVA: 0x003BEFAC File Offset: 0x003BD1AC
		public StoryBoardEx(string name)
		{
			this._Name = name;
		}

		// Token: 0x1700062C RID: 1580
		// (get) Token: 0x06004124 RID: 16676 RVA: 0x003BF034 File Offset: 0x003BD234
		public string Name
		{
			get
			{
				return this._Name;
			}
		}

		// Token: 0x06004125 RID: 16677 RVA: 0x003BF04C File Offset: 0x003BD24C
		public void Binding()
		{
			if (!StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Add(this._Name, this);
			}
		}

		// Token: 0x06004126 RID: 16678 RVA: 0x003BF084 File Offset: 0x003BD284
		public void Clear()
		{
			if (this._Name != null && StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Remove(this._Name);
			}
		}

		// Token: 0x1700062D RID: 1581
		// (get) Token: 0x06004127 RID: 16679 RVA: 0x003BF0C8 File Offset: 0x003BD2C8
		// (set) Token: 0x06004128 RID: 16680 RVA: 0x003BF0E0 File Offset: 0x003BD2E0
		public double OrigMovingSpeedPerFrame
		{
			get
			{
				return this._OrigMovingSpeedPerFrame;
			}
			set
			{
				this._OrigMovingSpeedPerFrame = value;
			}
		}

		// Token: 0x1700062E RID: 1582
		// (get) Token: 0x06004129 RID: 16681 RVA: 0x003BF0EC File Offset: 0x003BD2EC
		// (set) Token: 0x0600412A RID: 16682 RVA: 0x003BF104 File Offset: 0x003BD304
		public double MovingSpeedPerFrame
		{
			get
			{
				return this._MovingSpeedPerFrame;
			}
			set
			{
				this._MovingSpeedPerFrame = value;
			}
		}

		// Token: 0x0600412B RID: 16683 RVA: 0x003BF110 File Offset: 0x003BD310
		public bool Start(Monster obj, List<ANode> path, double movingSpeedPerFrame, int cellSize)
		{
			bool result;
			if (this._Started)
			{
				result = false;
			}
			else
			{
				this._OrigMovingSpeedPerFrame = movingSpeedPerFrame;
				this._MovingSpeedPerFrame = movingSpeedPerFrame;
				this._MovingObj = obj;
				this._LastRunTicks = StoryBoardEx.getMyTimer();
				this._CellSize = cellSize;
				this._PathIndex = 0;
				this._Path = path;
				this._CompletedState = false;
				this._Started = true;
				result = true;
			}
			return result;
		}

		// Token: 0x0600412C RID: 16684 RVA: 0x003BF178 File Offset: 0x003BD378
		public void Run(long currentTicks)
		{
			if (this._Started)
			{
				if (!this._CompletedState)
				{
					long elapsedTicks = currentTicks - this._LastRunTicks;
					this._LastRunTicks = currentTicks;
					double ticksPerFrame = (double)(1000f / (float)Global.MovingFrameRate);
					double elapsedFrameNum = (double)elapsedTicks / ticksPerFrame;
					double toMoveDist = elapsedFrameNum * this._MovingSpeedPerFrame;
					if (this.StepMove(toMoveDist))
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

		// Token: 0x0600412D RID: 16685 RVA: 0x003BF20C File Offset: 0x003BD40C
		private bool StepMove(double toMoveDist)
		{
			this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
			double targetX = (double)(this._Path[this._PathIndex].x * this._CellSize) + (double)this._CellSize / 2.0;
			double targetY = (double)(this._Path[this._PathIndex].y * this._CellSize) + (double)this._CellSize / 2.0;
			double dx = targetX - this._MovingObj.SafeCoordinate.X;
			double dy = targetY - this._MovingObj.SafeCoordinate.Y;
			double stepDist = Math.Sqrt(dx * dx + dy * dy);
			double thisToMoveDist = (stepDist < toMoveDist) ? stepDist : toMoveDist;
			double angle = Math.Atan2(dy, dx);
			double speedX = thisToMoveDist * Math.Cos(angle);
			double speedY = thisToMoveDist * Math.Sin(angle);
			this._MovingObj.Coordinate = new Point(this._MovingObj.SafeCoordinate.X + speedX, this._MovingObj.SafeCoordinate.Y + speedY);
			if ((long)targetX != (long)this._MovingObj.SafeCoordinate.X || (long)targetY != (long)this._MovingObj.SafeCoordinate.Y)
			{
				int direction = (int)Global.GetDirectionByTan(targetX, targetY, this._MovingObj.SafeCoordinate.X, this._MovingObj.SafeCoordinate.Y);
				if ((double)direction != this._MovingObj.Direction)
				{
					this._MovingObj.Direction = (double)direction;
				}
			}
			if (stepDist < toMoveDist)
			{
				this._PathIndex++;
				if (this._PathIndex >= this._Path.Count)
				{
					this._MovingObj.Coordinate = new Point(targetX, targetY);
					return true;
				}
				toMoveDist -= stepDist;
				this.StepMove(toMoveDist);
			}
			return false;
		}

		// Token: 0x04004F6F RID: 20335
		private static Dictionary<string, StoryBoardEx> StoryBoardDict = new Dictionary<string, StoryBoardEx>();

		// Token: 0x04004F70 RID: 20336
		private static long LastRunStoryTicks = 0L;

		// Token: 0x04004F71 RID: 20337
		private object _Tag = null;

		// Token: 0x04004F73 RID: 20339
		private string _Name = null;

		// Token: 0x04004F74 RID: 20340
		private int _PathIndex = 0;

		// Token: 0x04004F75 RID: 20341
		private int _CellSize = GameManager.MapGridWidth;

		// Token: 0x04004F76 RID: 20342
		private List<ANode> _Path = null;

		// Token: 0x04004F77 RID: 20343
		private long _LastRunTicks = 0L;

		// Token: 0x04004F78 RID: 20344
		private double _OrigMovingSpeedPerFrame = 0.0;

		// Token: 0x04004F79 RID: 20345
		private double _MovingSpeedPerFrame = 0.0;

		// Token: 0x04004F7A RID: 20346
		private Monster _MovingObj = null;

		// Token: 0x04004F7B RID: 20347
		private bool _Started = false;

		// Token: 0x04004F7C RID: 20348
		private bool _CompletedState = false;

		// Token: 0x020008D4 RID: 2260
		// (Invoke) Token: 0x06004130 RID: 16688
		public delegate void CompletedDelegateHandle(object sender, EventArgs e);
	}
}
