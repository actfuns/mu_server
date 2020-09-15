using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008DF RID: 2271
	public class PathFinder : IPathFinder
	{
		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600417A RID: 16762 RVA: 0x003C0494 File Offset: 0x003BE694
		// (remove) Token: 0x0600417B RID: 16763 RVA: 0x003C04D0 File Offset: 0x003BE6D0
		public event PathFinderDebugHandler PathFinderDebug;

		// Token: 0x0600417C RID: 16764 RVA: 0x003C050C File Offset: 0x003BE70C
		public PathFinder(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
		}

		// Token: 0x17000643 RID: 1603
		// (get) Token: 0x0600417D RID: 16765 RVA: 0x003C05D0 File Offset: 0x003BE7D0
		public bool Stopped
		{
			get
			{
				return this.mStopped;
			}
		}

		// Token: 0x17000644 RID: 1604
		// (get) Token: 0x0600417E RID: 16766 RVA: 0x003C05E8 File Offset: 0x003BE7E8
		// (set) Token: 0x0600417F RID: 16767 RVA: 0x003C0600 File Offset: 0x003BE800
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

		// Token: 0x17000645 RID: 1605
		// (get) Token: 0x06004180 RID: 16768 RVA: 0x003C060C File Offset: 0x003BE80C
		// (set) Token: 0x06004181 RID: 16769 RVA: 0x003C0624 File Offset: 0x003BE824
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

		// Token: 0x17000646 RID: 1606
		// (get) Token: 0x06004182 RID: 16770 RVA: 0x003C0630 File Offset: 0x003BE830
		// (set) Token: 0x06004183 RID: 16771 RVA: 0x003C0648 File Offset: 0x003BE848
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

		// Token: 0x17000647 RID: 1607
		// (get) Token: 0x06004184 RID: 16772 RVA: 0x003C0654 File Offset: 0x003BE854
		// (set) Token: 0x06004185 RID: 16773 RVA: 0x003C066C File Offset: 0x003BE86C
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

		// Token: 0x17000648 RID: 1608
		// (get) Token: 0x06004186 RID: 16774 RVA: 0x003C0678 File Offset: 0x003BE878
		// (set) Token: 0x06004187 RID: 16775 RVA: 0x003C0690 File Offset: 0x003BE890
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

		// Token: 0x17000649 RID: 1609
		// (get) Token: 0x06004188 RID: 16776 RVA: 0x003C069C File Offset: 0x003BE89C
		// (set) Token: 0x06004189 RID: 16777 RVA: 0x003C06B4 File Offset: 0x003BE8B4
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

		// Token: 0x1700064A RID: 1610
		// (get) Token: 0x0600418A RID: 16778 RVA: 0x003C06C0 File Offset: 0x003BE8C0
		// (set) Token: 0x0600418B RID: 16779 RVA: 0x003C06D8 File Offset: 0x003BE8D8
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

		// Token: 0x1700064B RID: 1611
		// (get) Token: 0x0600418C RID: 16780 RVA: 0x003C06E4 File Offset: 0x003BE8E4
		// (set) Token: 0x0600418D RID: 16781 RVA: 0x003C06FC File Offset: 0x003BE8FC
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

		// Token: 0x1700064C RID: 1612
		// (get) Token: 0x0600418E RID: 16782 RVA: 0x003C0708 File Offset: 0x003BE908
		// (set) Token: 0x0600418F RID: 16783 RVA: 0x003C0720 File Offset: 0x003BE920
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

		// Token: 0x1700064D RID: 1613
		// (get) Token: 0x06004190 RID: 16784 RVA: 0x003C072C File Offset: 0x003BE92C
		// (set) Token: 0x06004191 RID: 16785 RVA: 0x003C0744 File Offset: 0x003BE944
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

		// Token: 0x1700064E RID: 1614
		// (get) Token: 0x06004192 RID: 16786 RVA: 0x003C0750 File Offset: 0x003BE950
		// (set) Token: 0x06004193 RID: 16787 RVA: 0x003C0768 File Offset: 0x003BE968
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

		// Token: 0x06004194 RID: 16788 RVA: 0x003C0772 File Offset: 0x003BE972
		public void FindPathStop()
		{
			this.mStop = true;
		}

		// Token: 0x06004195 RID: 16789 RVA: 0x003C0798 File Offset: 0x003BE998
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

		// Token: 0x06004196 RID: 16790 RVA: 0x003C120C File Offset: 0x003BF40C
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

		// Token: 0x06004197 RID: 16791 RVA: 0x003C1320 File Offset: 0x003BF520
		private List<PathFinderNode> ReverseList(List<PathFinderNode> floydPath)
		{
			List<PathFinderNode> myFloydPath = new List<PathFinderNode>();
			for (int i = floydPath.Count - 1; i >= 0; i--)
			{
				myFloydPath.Add(floydPath[i]);
			}
			return myFloydPath;
		}

		// Token: 0x06004198 RID: 16792 RVA: 0x003C1363 File Offset: 0x003BF563
		private void FloydVector(PathFinderNode target, PathFinderNode n1, PathFinderNode n2)
		{
			target.PX = n1.PX - n2.PX;
			target.PY = n1.PY - n2.PY;
		}

		// Token: 0x04004FB1 RID: 20401
		private byte[,] mGrid = null;

		// Token: 0x04004FB2 RID: 20402
		private PriorityQueueB<PathFinderNode> mOpen = new PriorityQueueB<PathFinderNode>(new PathFinder.ComparePFNode());

		// Token: 0x04004FB3 RID: 20403
		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		// Token: 0x04004FB4 RID: 20404
		private bool mStop = false;

		// Token: 0x04004FB5 RID: 20405
		private bool mStopped = true;

		// Token: 0x04004FB6 RID: 20406
		private int mHoriz = 0;

		// Token: 0x04004FB7 RID: 20407
		private HeuristicFormula mFormula = HeuristicFormula.Manhattan;

		// Token: 0x04004FB8 RID: 20408
		private bool mDiagonals = true;

		// Token: 0x04004FB9 RID: 20409
		private int mHEstimate = 2;

		// Token: 0x04004FBA RID: 20410
		private bool mPunishChangeDirection = false;

		// Token: 0x04004FBB RID: 20411
		private bool mReopenCloseNodes = false;

		// Token: 0x04004FBC RID: 20412
		private bool mTieBreaker = false;

		// Token: 0x04004FBD RID: 20413
		private bool mHeavyDiagonals = false;

		// Token: 0x04004FBE RID: 20414
		private int mSearchLimit = 2000;

		// Token: 0x04004FBF RID: 20415
		private double mCompletedTime = 0.0;

		// Token: 0x04004FC0 RID: 20416
		private bool mDebugProgress = false;

		// Token: 0x04004FC1 RID: 20417
		private bool mDebugFoundPath = false;

		// Token: 0x020008E0 RID: 2272
		internal class ComparePFNode : IComparer<PathFinderNode>
		{
			// Token: 0x06004199 RID: 16793 RVA: 0x003C1394 File Offset: 0x003BF594
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
