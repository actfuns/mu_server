using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	
	public class PathFinder : IPathFinder
	{
		
		// (add) Token: 0x0600417A RID: 16762 RVA: 0x003C0494 File Offset: 0x003BE694
		// (remove) Token: 0x0600417B RID: 16763 RVA: 0x003C04D0 File Offset: 0x003BE6D0
		public event PathFinderDebugHandler PathFinderDebug;

		
		public PathFinder(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
		}

		
		
		public bool Stopped
		{
			get
			{
				return this.mStopped;
			}
		}

		
		
		
		public HeuristicFormula Formula
		{
			get
			{
				return this.mFormula;
			}
			set
			{
				this.mFormula = value;
			}
		}

		
		
		
		public bool Diagonals
		{
			get
			{
				return this.mDiagonals;
			}
			set
			{
				this.mDiagonals = value;
			}
		}

		
		
		
		public bool HeavyDiagonals
		{
			get
			{
				return this.mHeavyDiagonals;
			}
			set
			{
				this.mHeavyDiagonals = value;
			}
		}

		
		
		
		public int HeuristicEstimate
		{
			get
			{
				return this.mHEstimate;
			}
			set
			{
				this.mHEstimate = value;
			}
		}

		
		
		
		public bool PunishChangeDirection
		{
			get
			{
				return this.mPunishChangeDirection;
			}
			set
			{
				this.mPunishChangeDirection = value;
			}
		}

		
		
		
		public bool ReopenCloseNodes
		{
			get
			{
				return this.mReopenCloseNodes;
			}
			set
			{
				this.mReopenCloseNodes = value;
			}
		}

		
		
		
		public bool TieBreaker
		{
			get
			{
				return this.mTieBreaker;
			}
			set
			{
				this.mTieBreaker = value;
			}
		}

		
		
		
		public int SearchLimit
		{
			get
			{
				return this.mSearchLimit;
			}
			set
			{
				this.mSearchLimit = value;
			}
		}

		
		
		
		public double CompletedTime
		{
			get
			{
				return this.mCompletedTime;
			}
			set
			{
				this.mCompletedTime = value;
			}
		}

		
		
		
		public bool DebugProgress
		{
			get
			{
				return this.mDebugProgress;
			}
			set
			{
				this.mDebugProgress = value;
			}
		}

		
		
		
		public bool DebugFoundPath
		{
			get
			{
				return this.mDebugFoundPath;
			}
			set
			{
				this.mDebugFoundPath = value;
			}
		}

		
		public void FindPathStop()
		{
			this.mStop = true;
		}

		
		public List<PathFinderNode> FindPath(Point2D start, Point2D end)
		{
			bool found = false;
			int gridX = this.mGrid.GetUpperBound(0);
			int gridY = this.mGrid.GetUpperBound(1);
			this.mStop = false;
			this.mStopped = false;
			this.mOpen.Clear();
			this.mClose.Clear();
			if (this.mDebugProgress && this.PathFinderDebug != null)
			{
				this.PathFinderDebug(0, 0, start.X, start.Y, PathFinderNodeType.Start, -1, -1);
			}
			if (this.mDebugProgress && this.PathFinderDebug != null)
			{
				this.PathFinderDebug(0, 0, end.X, end.Y, PathFinderNodeType.End, -1, -1);
			}
			sbyte[,] direction;
			if (this.mDiagonals)
			{
				direction = new sbyte[,]
				{
					{
						0,
						-1
					},
					{
						1,
						0
					},
					{
						0,
						1
					},
					{
						-1,
						0
					},
					{
						1,
						-1
					},
					{
						1,
						1
					},
					{
						-1,
						1
					},
					{
						-1,
						-1
					}
				};
			}
			else
			{
				direction = new sbyte[,]
				{
					{
						0,
						-1
					},
					{
						1,
						0
					},
					{
						0,
						1
					},
					{
						-1,
						0
					}
				};
			}
			PathFinderNode parentNode;
			parentNode.G = 0;
			parentNode.H = this.mHEstimate;
			parentNode.F = parentNode.G + parentNode.H;
			parentNode.X = start.X;
			parentNode.Y = start.Y;
			parentNode.PX = parentNode.X;
			parentNode.PY = parentNode.Y;
			this.mOpen.Push(parentNode);
			while (this.mOpen.Count > 0 && !this.mStop)
			{
				parentNode = this.mOpen.Pop();
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, parentNode.X, parentNode.Y, PathFinderNodeType.Current, -1, -1);
				}
				if (parentNode.X == end.X && parentNode.Y == end.Y)
				{
					this.mClose.Add(parentNode);
					found = true;
					break;
				}
				if (this.mClose.Count > this.mSearchLimit)
				{
					this.mStopped = true;
					return null;
				}
				if (this.mPunishChangeDirection)
				{
					this.mHoriz = parentNode.X - parentNode.PX;
				}
				for (int i = 0; i < (this.mDiagonals ? 8 : 4); i++)
				{
					PathFinderNode newNode;
					newNode.X = parentNode.X + (int)direction[i, 0];
					newNode.Y = parentNode.Y + (int)direction[i, 1];
					if (newNode.X >= 0 && newNode.Y >= 0 && newNode.X < gridX && newNode.Y < gridY)
					{
						int newG;
						if (this.mHeavyDiagonals && i > 3)
						{
							newG = parentNode.G + (int)((double)this.mGrid[newNode.X, newNode.Y] * 2.41);
						}
						else
						{
							newG = parentNode.G + (int)this.mGrid[newNode.X, newNode.Y];
						}
						if (newG != parentNode.G)
						{
							if (this.mPunishChangeDirection)
							{
								if (newNode.X - parentNode.X != 0)
								{
									if (this.mHoriz == 0)
									{
										newG += 20;
									}
								}
								if (newNode.Y - parentNode.Y != 0)
								{
									if (this.mHoriz != 0)
									{
										newG += 20;
									}
								}
							}
							int foundInOpenIndex = -1;
							for (int j = 0; j < this.mOpen.Count; j++)
							{
								if (this.mOpen[j].X == newNode.X && this.mOpen[j].Y == newNode.Y)
								{
									foundInOpenIndex = j;
									break;
								}
							}
							if (foundInOpenIndex == -1 || this.mOpen[foundInOpenIndex].G > newG)
							{
								int foundInCloseIndex = -1;
								for (int j = 0; j < this.mClose.Count; j++)
								{
									if (this.mClose[j].X == newNode.X && this.mClose[j].Y == newNode.Y)
									{
										foundInCloseIndex = j;
										break;
									}
								}
								if (foundInCloseIndex == -1 || (!this.mReopenCloseNodes && this.mClose[foundInCloseIndex].G > newG))
								{
									newNode.PX = parentNode.X;
									newNode.PY = parentNode.Y;
									newNode.G = newG;
									switch (this.mFormula)
									{
									default:
										newNode.H = this.mHEstimate * (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y));
										break;
									case HeuristicFormula.MaxDXDY:
										newNode.H = this.mHEstimate * Math.Max(Math.Abs(newNode.X - end.X), Math.Abs(newNode.Y - end.Y));
										break;
									case HeuristicFormula.DiagonalShortCut:
									{
										int h_diagonal = Math.Min(Math.Abs(newNode.X - end.X), Math.Abs(newNode.Y - end.Y));
										int h_straight = Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Y - end.Y);
										newNode.H = this.mHEstimate * 2 * h_diagonal + this.mHEstimate * (h_straight - 2 * h_diagonal);
										break;
									}
									case HeuristicFormula.Euclidean:
										newNode.H = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)(newNode.X - end.X), 2.0) + Math.Pow((double)(newNode.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.EuclideanNoSQR:
										newNode.H = (int)((double)this.mHEstimate * (Math.Pow((double)(newNode.X - end.X), 2.0) + Math.Pow((double)(newNode.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.Custom1:
									{
										Point2D dxy = new Point2D(Math.Abs(end.X - newNode.X), Math.Abs(end.Y - newNode.Y));
										int Orthogonal = Math.Abs(dxy.X - dxy.Y);
										int Diagonal = Math.Abs((dxy.X + dxy.Y - Orthogonal) / 2);
										newNode.H = this.mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
										break;
									}
									}
									if (this.mTieBreaker)
									{
										int dx = parentNode.X - end.X;
										int dy = parentNode.Y - end.Y;
										int dx2 = start.X - end.X;
										int dy2 = start.Y - end.Y;
										int cross = Math.Abs(dx * dy2 - dx2 * dy);
										newNode.H = (int)((double)newNode.H + (double)cross * 0.001);
									}
									newNode.F = newNode.G + newNode.H;
									if (this.mDebugProgress && this.PathFinderDebug != null)
									{
										this.PathFinderDebug(parentNode.X, parentNode.Y, newNode.X, newNode.Y, PathFinderNodeType.Open, newNode.F, newNode.G);
									}
									this.mOpen.Push(newNode);
								}
							}
						}
					}
				}
				this.mClose.Add(parentNode);
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, parentNode.X, parentNode.Y, PathFinderNodeType.Close, parentNode.F, parentNode.G);
				}
			}
			if (found)
			{
				PathFinderNode fNode = this.mClose[this.mClose.Count - 1];
				for (int i = this.mClose.Count - 1; i >= 0; i--)
				{
					if ((fNode.PX == this.mClose[i].X && fNode.PY == this.mClose[i].Y) || i == this.mClose.Count - 1)
					{
						if (this.mDebugFoundPath && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(fNode.X, fNode.Y, this.mClose[i].X, this.mClose[i].Y, PathFinderNodeType.Path, this.mClose[i].F, this.mClose[i].G);
						}
						fNode = this.mClose[i];
					}
					else
					{
						this.mClose.RemoveAt(i);
					}
				}
				this.mStopped = true;
				return this.mClose;
			}
			this.mStopped = true;
			return null;
		}

		
		public List<PathFinderNode> Floyd(List<PathFinderNode> _floydPath)
		{
			List<PathFinderNode> result;
			if (null == _floydPath)
			{
				result = null;
			}
			else
			{
				_floydPath = this.ReverseList(_floydPath);
				int len = _floydPath.Count;
				if (len > 2)
				{
					PathFinderNode vector = default(PathFinderNode);
					PathFinderNode tempVector = default(PathFinderNode);
					this.FloydVector(vector, _floydPath[len - 1], _floydPath[len - 2]);
					for (int i = _floydPath.Count - 3; i >= 0; i--)
					{
						this.FloydVector(tempVector, _floydPath[i + 1], _floydPath[i]);
						if (vector.PX == tempVector.PX && vector.PY == tempVector.PY)
						{
							_floydPath.RemoveAt(i + 1);
						}
						else
						{
							vector.PX = tempVector.PX;
							vector.PY = tempVector.PY;
						}
					}
				}
				_floydPath = this.ReverseList(_floydPath);
				result = _floydPath;
			}
			return result;
		}

		
		private List<PathFinderNode> ReverseList(List<PathFinderNode> floydPath)
		{
			List<PathFinderNode> myFloydPath = new List<PathFinderNode>();
			for (int i = floydPath.Count - 1; i >= 0; i--)
			{
				myFloydPath.Add(floydPath[i]);
			}
			return myFloydPath;
		}

		
		private void FloydVector(PathFinderNode target, PathFinderNode n1, PathFinderNode n2)
		{
			target.PX = n1.PX - n2.PX;
			target.PY = n1.PY - n2.PY;
		}

		
		private byte[,] mGrid = null;

		
		private PriorityQueueB<PathFinderNode> mOpen = new PriorityQueueB<PathFinderNode>(new PathFinder.ComparePFNode());

		
		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		
		private bool mStop = false;

		
		private bool mStopped = true;

		
		private int mHoriz = 0;

		
		private HeuristicFormula mFormula = HeuristicFormula.Manhattan;

		
		private bool mDiagonals = true;

		
		private int mHEstimate = 2;

		
		private bool mPunishChangeDirection = false;

		
		private bool mReopenCloseNodes = false;

		
		private bool mTieBreaker = false;

		
		private bool mHeavyDiagonals = false;

		
		private int mSearchLimit = 2000;

		
		private double mCompletedTime = 0.0;

		
		private bool mDebugProgress = false;

		
		private bool mDebugFoundPath = false;

		
		internal class ComparePFNode : IComparer<PathFinderNode>
		{
			
			public int Compare(PathFinderNode x, PathFinderNode y)
			{
				int result;
				if (x.F > y.F)
				{
					result = 1;
				}
				else if (x.F < y.F)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}
	}
}
