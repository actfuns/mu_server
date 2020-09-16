using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using HSGameEngine.Tools.AStarEx;

namespace GameServer.Logic
{
	
	public class StoryBoardEx
	{
		
		public static bool ContainStoryBoard(string name)
		{
			return StoryBoardEx.StoryBoardDict.ContainsKey(name);
		}

		
		public static StoryBoardEx FindStoryBoard(string name)
		{
			StoryBoardEx storyBd = null;
			StoryBoardEx.StoryBoardDict.TryGetValue(name, out storyBd);
			return storyBd;
		}

		
		public static void RemoveStoryBoard(string name)
		{
			StoryBoardEx sb = StoryBoardEx.FindStoryBoard(name);
			if (null != sb)
			{
				sb.Completed = null;
				sb.Clear();
			}
		}

		
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

		
		private static long getMyTimer()
		{
			return TimeUtil.NOW();
		}

		
		public static void runStoryBoards()
		{
			long currentTicks = StoryBoardEx.getMyTimer();
			StoryBoardEx.LastRunStoryTicks = currentTicks;
			List<StoryBoardEx> list = new List<StoryBoardEx>();
			foreach (StoryBoardEx sb in StoryBoardEx.StoryBoardDict.Values)
			{
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

		
		// (add) Token: 0x0600411F RID: 16671 RVA: 0x003BEF10 File Offset: 0x003BD110
		// (remove) Token: 0x06004120 RID: 16672 RVA: 0x003BEF4C File Offset: 0x003BD14C
		public event StoryBoardEx.CompletedDelegateHandle _Completed = null;

		
		
		
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

		
		public StoryBoardEx(string name)
		{
			this._Name = name;
		}

		
		
		public string Name
		{
			get
			{
				return this._Name;
			}
		}

		
		public void Binding()
		{
			if (!StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Add(this._Name, this);
			}
		}

		
		public void Clear()
		{
			if (this._Name != null && StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Remove(this._Name);
			}
		}

		
		
		
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

		
		private static Dictionary<string, StoryBoardEx> StoryBoardDict = new Dictionary<string, StoryBoardEx>();

		
		private static long LastRunStoryTicks = 0L;

		
		private object _Tag = null;

		
		private string _Name = null;

		
		private int _PathIndex = 0;

		
		private int _CellSize = GameManager.MapGridWidth;

		
		private List<ANode> _Path = null;

		
		private long _LastRunTicks = 0L;

		
		private double _OrigMovingSpeedPerFrame = 0.0;

		
		private double _MovingSpeedPerFrame = 0.0;

		
		private Monster _MovingObj = null;

		
		private bool _Started = false;

		
		private bool _CompletedState = false;

		
		
		public delegate void CompletedDelegateHandle(object sender, EventArgs e);
	}
}
