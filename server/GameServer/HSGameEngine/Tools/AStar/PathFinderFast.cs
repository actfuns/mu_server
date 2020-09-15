using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace HSGameEngine.Tools.AStar
{
	// Token: 0x020008E1 RID: 2273
	public class PathFinderFast : IPathFinder
	{
		// Token: 0x1400000F RID: 15
		// (add) Token: 0x0600419B RID: 16795 RVA: 0x003C13E8 File Offset: 0x003BF5E8
		// (remove) Token: 0x0600419C RID: 16796 RVA: 0x003C1424 File Offset: 0x003BF624
		public event PathFinderDebugHandler PathFinderDebug;

		// Token: 0x0600419D RID: 16797 RVA: 0x003C1470 File Offset: 0x003BF670
		public PathFinderFast(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
			this.mGridX = (ushort)(this.mGrid.GetUpperBound(0) + 1);
			this.mGridY = (ushort)(this.mGrid.GetUpperBound(1) + 1);
			this.mGridXMinus1 = this.mGridX - 1;
			this.mGridYLog2 = (ushort)Math.Log((double)this.mGridY, 2.0);
			if (Math.Log((double)this.mGridX, 2.0) != (double)((int)Math.Log((double)this.mGridX, 2.0)) || Math.Log((double)this.mGridY, 2.0) != (double)((int)Math.Log((double)this.mGridY, 2.0)))
			{
				throw new Exception("Invalid Grid, size in X and Y must be power of 2");
			}
			if (PathFinderFast.mCalcGrid == null || PathFinderFast.mCalcGrid.Length < (int)(this.mGridX * this.mGridY))
			{
				PathFinderFast.mCalcGrid = new PathFinderFast.PathFinderNodeFast[(int)(this.mGridX * this.mGridY)];
			}
			this.mOpen = new PriorityQueueB<int>(new PathFinderFast.ComparePFNodeMatrix(PathFinderFast.mCalcGrid));
		}

		// Token: 0x1700064F RID: 1615
		// (get) Token: 0x0600419E RID: 16798 RVA: 0x003C16E8 File Offset: 0x003BF8E8
		public bool Stopped
		{
			get
			{
				return this.mStopped;
			}
		}

		// Token: 0x17000650 RID: 1616
		// (get) Token: 0x0600419F RID: 16799 RVA: 0x003C1700 File Offset: 0x003BF900
		// (set) Token: 0x060041A0 RID: 16800 RVA: 0x003C1718 File Offset: 0x003BF918
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

		// Token: 0x17000651 RID: 1617
		// (get) Token: 0x060041A1 RID: 16801 RVA: 0x003C1724 File Offset: 0x003BF924
		// (set) Token: 0x060041A2 RID: 16802 RVA: 0x003C1758 File Offset: 0x003BF958
		public bool Diagonals
		{
			get
			{
				return this.mDiagonals;
			}
			set
			{
				this.mDiagonals = value;
				if (this.mDiagonals)
				{
					this.mDirection = new sbyte[,]
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
					this.mDirection = new sbyte[,]
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
			}
		}

		// Token: 0x17000652 RID: 1618
		// (get) Token: 0x060041A3 RID: 16803 RVA: 0x003C17AC File Offset: 0x003BF9AC
		// (set) Token: 0x060041A4 RID: 16804 RVA: 0x003C17C4 File Offset: 0x003BF9C4
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

		// Token: 0x17000653 RID: 1619
		// (get) Token: 0x060041A5 RID: 16805 RVA: 0x003C17D0 File Offset: 0x003BF9D0
		// (set) Token: 0x060041A6 RID: 16806 RVA: 0x003C17E8 File Offset: 0x003BF9E8
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

		// Token: 0x17000654 RID: 1620
		// (get) Token: 0x060041A7 RID: 16807 RVA: 0x003C17F4 File Offset: 0x003BF9F4
		// (set) Token: 0x060041A8 RID: 16808 RVA: 0x003C180C File Offset: 0x003BFA0C
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

		// Token: 0x17000655 RID: 1621
		// (get) Token: 0x060041A9 RID: 16809 RVA: 0x003C1818 File Offset: 0x003BFA18
		// (set) Token: 0x060041AA RID: 16810 RVA: 0x003C1830 File Offset: 0x003BFA30
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

		// Token: 0x17000656 RID: 1622
		// (get) Token: 0x060041AB RID: 16811 RVA: 0x003C183C File Offset: 0x003BFA3C
		// (set) Token: 0x060041AC RID: 16812 RVA: 0x003C1854 File Offset: 0x003BFA54
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

		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x060041AD RID: 16813 RVA: 0x003C1860 File Offset: 0x003BFA60
		// (set) Token: 0x060041AE RID: 16814 RVA: 0x003C1878 File Offset: 0x003BFA78
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

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x060041AF RID: 16815 RVA: 0x003C1884 File Offset: 0x003BFA84
		// (set) Token: 0x060041B0 RID: 16816 RVA: 0x003C189C File Offset: 0x003BFA9C
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

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x060041B1 RID: 16817 RVA: 0x003C18A8 File Offset: 0x003BFAA8
		// (set) Token: 0x060041B2 RID: 16818 RVA: 0x003C18C0 File Offset: 0x003BFAC0
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

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x060041B3 RID: 16819 RVA: 0x003C18CC File Offset: 0x003BFACC
		// (set) Token: 0x060041B4 RID: 16820 RVA: 0x003C18E4 File Offset: 0x003BFAE4
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

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x060041B5 RID: 16821 RVA: 0x003C18F0 File Offset: 0x003BFAF0
		// (set) Token: 0x060041B6 RID: 16822 RVA: 0x003C1908 File Offset: 0x003BFB08
		public int[,] Punish
		{
			get
			{
				return this.mPunish;
			}
			set
			{
				this.mPunish = value;
			}
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x060041B7 RID: 16823 RVA: 0x003C1914 File Offset: 0x003BFB14
		// (set) Token: 0x060041B8 RID: 16824 RVA: 0x003C192C File Offset: 0x003BFB2C
		public int MaxNum
		{
			get
			{
				return this.mMaxNum;
			}
			set
			{
				this.mMaxNum = value;
			}
		}

		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x060041B9 RID: 16825 RVA: 0x003C1938 File Offset: 0x003BFB38
		// (set) Token: 0x060041BA RID: 16826 RVA: 0x003C1950 File Offset: 0x003BFB50
		public bool EnablePunish
		{
			get
			{
				return this.mEnablePunish;
			}
			set
			{
				this.mEnablePunish = value;
			}
		}

		// Token: 0x060041BB RID: 16827 RVA: 0x003C195A File Offset: 0x003BFB5A
		public void FindPathStop()
		{
			this.mStop = true;
		}

		// Token: 0x060041BC RID: 16828 RVA: 0x003C1964 File Offset: 0x003BFB64
		public List<PathFinderNode> FindPath(Point start, Point end)
		{
			return this.FindPath(new Point2D((int)start.X, (int)start.Y), new Point2D((int)end.X, (int)end.Y));
		}

		// Token: 0x060041BD RID: 16829 RVA: 0x003C19A8 File Offset: 0x003BFBA8
		private int GetPunishNum(int x, int y)
		{
			int result;
			if (!this.mEnablePunish)
			{
				result = 0;
			}
			else if (null == this.mPunish)
			{
				result = 0;
			}
			else
			{
				result = this.mMaxNum - Math.Min(this.mPunish[x, y], 3);
			}
			return result;
		}

		// Token: 0x060041BE RID: 16830 RVA: 0x003C19F8 File Offset: 0x003BFBF8
		public List<PathFinderNode> FindPath(Point2D start, Point2D end)
		{
			List<PathFinderNode> result;
			lock (this)
			{
				Array.Clear(PathFinderFast.mCalcGrid, 0, PathFinderFast.mCalcGrid.Length);
				this.mFound = false;
				this.mStop = false;
				this.mStopped = false;
				this.mCloseNodeCounter = 0;
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
				this.mLocation = (start.Y << (int)this.mGridYLog2) + start.X;
				this.mEndLocation = (end.Y << (int)this.mGridYLog2) + end.X;
				PathFinderFast.mCalcGrid[this.mLocation].G = 0;
				PathFinderFast.mCalcGrid[this.mLocation].F = this.mHEstimate;
				PathFinderFast.mCalcGrid[this.mLocation].PX = (ushort)start.X;
				PathFinderFast.mCalcGrid[this.mLocation].PY = (ushort)start.Y;
				PathFinderFast.mCalcGrid[this.mLocation].Status = this.mOpenNodeValue;
				this.mOpen.Push(this.mLocation);
				while (this.mOpen.Count > 0 && !this.mStop)
				{
					this.mLocation = this.mOpen.Pop();
					if (PathFinderFast.mCalcGrid[this.mLocation].Status != this.mCloseNodeValue)
					{
						this.mLocationX = (ushort)(this.mLocation & (int)this.mGridXMinus1);
						this.mLocationY = (ushort)(this.mLocation >> (int)this.mGridYLog2);
						if (this.mDebugProgress && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(0, 0, this.mLocation & (int)this.mGridXMinus1, this.mLocation >> (int)this.mGridYLog2, PathFinderNodeType.Current, -1, -1);
						}
						if (this.mLocation == this.mEndLocation)
						{
							PathFinderFast.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
							this.mFound = true;
							break;
						}
						if (this.mCloseNodeCounter > this.mSearchLimit)
						{
							this.mStopped = true;
							return null;
						}
						if (this.mPunishChangeDirection)
						{
							this.mHoriz = (int)(this.mLocationX - PathFinderFast.mCalcGrid[this.mLocation].PX);
						}
						for (int i = 0; i < (this.mDiagonals ? 8 : 4); i++)
						{
							this.mNewLocationX = this.mLocationX + (ushort)this.mDirection[i, 0];
							this.mNewLocationY = this.mLocationY + (ushort)this.mDirection[i, 1];
							this.mNewLocation = ((int)this.mNewLocationY << (int)this.mGridYLog2) + (int)this.mNewLocationX;
							if (this.mNewLocationX < this.mGridX && this.mNewLocationY < this.mGridY)
							{
								if (PathFinderFast.mCalcGrid[this.mNewLocation].Status != this.mCloseNodeValue || this.mReopenCloseNodes)
								{
									if (this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY] != 0)
									{
										if (this.mHeavyDiagonals && i > 3)
										{
											this.mNewG = PathFinderFast.mCalcGrid[this.mLocation].G + (int)((double)this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY] * 2.41);
										}
										else
										{
											this.mNewG = PathFinderFast.mCalcGrid[this.mLocation].G + (int)this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY];
										}
										if (this.mPunishChangeDirection)
										{
											if (this.mNewLocationX - this.mLocationX != 0)
											{
												if (this.mHoriz == 0)
												{
													this.mNewG += Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
												}
											}
											if (this.mNewLocationY - this.mLocationY != 0)
											{
												if (this.mHoriz != 0)
												{
													this.mNewG += Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
												}
											}
										}
										this.mNewG += this.GetPunishNum((int)this.mNewLocationX, (int)this.mNewLocationY);
										if (PathFinderFast.mCalcGrid[this.mNewLocation].Status == this.mOpenNodeValue || PathFinderFast.mCalcGrid[this.mNewLocation].Status == this.mCloseNodeValue)
										{
											if (PathFinderFast.mCalcGrid[this.mNewLocation].G <= this.mNewG)
											{
												goto IL_983;
											}
										}
										PathFinderFast.mCalcGrid[this.mNewLocation].PX = this.mLocationX;
										PathFinderFast.mCalcGrid[this.mNewLocation].PY = this.mLocationY;
										PathFinderFast.mCalcGrid[this.mNewLocation].G = this.mNewG;
										switch (this.mFormula)
										{
										default:
											this.mH = this.mHEstimate * (Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y));
											break;
										case HeuristicFormula.MaxDXDY:
											this.mH = this.mHEstimate * Math.Max(Math.Abs((int)this.mNewLocationX - end.X), Math.Abs((int)this.mNewLocationY - end.Y));
											break;
										case HeuristicFormula.DiagonalShortCut:
										{
											int h_diagonal = Math.Min(Math.Abs((int)this.mNewLocationX - end.X), Math.Abs((int)this.mNewLocationY - end.Y));
											int h_straight = Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
											this.mH = this.mHEstimate * 2 * h_diagonal + this.mHEstimate * (h_straight - 2 * h_diagonal);
											break;
										}
										case HeuristicFormula.Euclidean:
											this.mH = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)((int)this.mNewLocationY - end.X), 2.0) + Math.Pow((double)((int)this.mNewLocationY - end.Y), 2.0)));
											break;
										case HeuristicFormula.EuclideanNoSQR:
											this.mH = (int)((double)this.mHEstimate * (Math.Pow((double)((int)this.mNewLocationX - end.X), 2.0) + Math.Pow((double)((int)this.mNewLocationY - end.Y), 2.0)));
											break;
										case HeuristicFormula.Custom1:
										{
											Point2D dxy = new Point2D(Math.Abs(end.X - (int)this.mNewLocationX), Math.Abs(end.Y - (int)this.mNewLocationY));
											int Orthogonal = Math.Abs(dxy.X - dxy.Y);
											int Diagonal = Math.Abs((dxy.X + dxy.Y - Orthogonal) / 2);
											this.mH = this.mHEstimate * (Diagonal + Orthogonal + dxy.X + dxy.Y);
											break;
										}
										}
										if (this.mTieBreaker)
										{
											int dx = (int)this.mLocationX - end.X;
											int dy = (int)this.mLocationY - end.Y;
											int dx2 = start.X - end.X;
											int dy2 = start.Y - end.Y;
											int cross = Math.Abs(dx * dy2 - dx2 * dy);
											this.mH = (int)((double)this.mH + (double)cross * 0.001);
										}
										PathFinderFast.mCalcGrid[this.mNewLocation].F = this.mNewG + this.mH;
										if (this.mDebugProgress && this.PathFinderDebug != null)
										{
											this.PathFinderDebug((int)this.mLocationX, (int)this.mLocationY, (int)this.mNewLocationX, (int)this.mNewLocationY, PathFinderNodeType.Open, PathFinderFast.mCalcGrid[this.mNewLocation].F, PathFinderFast.mCalcGrid[this.mNewLocation].G);
										}
										this.mOpen.Push(this.mNewLocation);
										PathFinderFast.mCalcGrid[this.mNewLocation].Status = this.mOpenNodeValue;
									}
								}
							}
							IL_983:;
						}
						this.mCloseNodeCounter++;
						PathFinderFast.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
						if (this.mDebugProgress && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(0, 0, (int)this.mLocationX, (int)this.mLocationY, PathFinderNodeType.Close, PathFinderFast.mCalcGrid[this.mLocation].F, PathFinderFast.mCalcGrid[this.mLocation].G);
						}
					}
				}
				if (this.mFound)
				{
					this.mClose.Clear();
					int posX = end.X;
					int posY = end.Y;
					PathFinderFast.PathFinderNodeFast fNodeTmp = PathFinderFast.mCalcGrid[(end.Y << (int)this.mGridYLog2) + end.X];
					PathFinderNode fNode;
					fNode.F = fNodeTmp.F;
					fNode.G = fNodeTmp.G;
					fNode.H = 0;
					fNode.PX = (int)fNodeTmp.PX;
					fNode.PY = (int)fNodeTmp.PY;
					fNode.X = end.X;
					fNode.Y = end.Y;
					while (fNode.X != fNode.PX || fNode.Y != fNode.PY)
					{
						this.mClose.Add(fNode);
						if (this.mDebugFoundPath && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(fNode.PX, fNode.PY, fNode.X, fNode.Y, PathFinderNodeType.Path, fNode.F, fNode.G);
						}
						posX = fNode.PX;
						posY = fNode.PY;
						fNodeTmp = PathFinderFast.mCalcGrid[(posY << (int)this.mGridYLog2) + posX];
						fNode.F = fNodeTmp.F;
						fNode.G = fNodeTmp.G;
						fNode.H = 0;
						fNode.PX = (int)fNodeTmp.PX;
						fNode.PY = (int)fNodeTmp.PY;
						fNode.X = posX;
						fNode.Y = posY;
					}
					this.mClose.Add(fNode);
					if (this.mDebugFoundPath && this.PathFinderDebug != null)
					{
						this.PathFinderDebug(fNode.PX, fNode.PY, fNode.X, fNode.Y, PathFinderNodeType.Path, fNode.F, fNode.G);
					}
					this.mStopped = true;
					result = this.mClose;
				}
				else
				{
					this.mStopped = true;
					result = null;
				}
			}
			return result;
		}

		// Token: 0x060041BF RID: 16831 RVA: 0x003C26D8 File Offset: 0x003C08D8
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

		// Token: 0x060041C0 RID: 16832 RVA: 0x003C27EC File Offset: 0x003C09EC
		private List<PathFinderNode> ReverseList(List<PathFinderNode> floydPath)
		{
			List<PathFinderNode> myFloydPath = new List<PathFinderNode>();
			for (int i = floydPath.Count - 1; i >= 0; i--)
			{
				myFloydPath.Add(floydPath[i]);
			}
			return myFloydPath;
		}

		// Token: 0x060041C1 RID: 16833 RVA: 0x003C282F File Offset: 0x003C0A2F
		private void FloydVector(PathFinderNode target, PathFinderNode n1, PathFinderNode n2)
		{
			target.PX = n1.PX - n2.PX;
			target.PY = n1.PY - n2.PY;
		}

		// Token: 0x04004FC3 RID: 20419
		private byte[,] mGrid = null;

		// Token: 0x04004FC4 RID: 20420
		private PriorityQueueB<int> mOpen = null;

		// Token: 0x04004FC5 RID: 20421
		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		// Token: 0x04004FC6 RID: 20422
		private bool mStop = false;

		// Token: 0x04004FC7 RID: 20423
		private bool mStopped = true;

		// Token: 0x04004FC8 RID: 20424
		private int mHoriz = 0;

		// Token: 0x04004FC9 RID: 20425
		private HeuristicFormula mFormula = HeuristicFormula.DiagonalShortCut;

		// Token: 0x04004FCA RID: 20426
		private bool mDiagonals = true;

		// Token: 0x04004FCB RID: 20427
		private int mHEstimate = 2;

		// Token: 0x04004FCC RID: 20428
		private bool mPunishChangeDirection = false;

		// Token: 0x04004FCD RID: 20429
		private bool mReopenCloseNodes = true;

		// Token: 0x04004FCE RID: 20430
		private bool mTieBreaker = false;

		// Token: 0x04004FCF RID: 20431
		private bool mHeavyDiagonals = false;

		// Token: 0x04004FD0 RID: 20432
		private int mSearchLimit = 2000;

		// Token: 0x04004FD1 RID: 20433
		private double mCompletedTime = 0.0;

		// Token: 0x04004FD2 RID: 20434
		private bool mDebugProgress = false;

		// Token: 0x04004FD3 RID: 20435
		private bool mDebugFoundPath = false;

		// Token: 0x04004FD4 RID: 20436
		private static PathFinderFast.PathFinderNodeFast[] mCalcGrid = null;

		// Token: 0x04004FD5 RID: 20437
		private byte mOpenNodeValue = 1;

		// Token: 0x04004FD6 RID: 20438
		private byte mCloseNodeValue = 2;

		// Token: 0x04004FD7 RID: 20439
		private int[,] mPunish = null;

		// Token: 0x04004FD8 RID: 20440
		private int mMaxNum = 0;

		// Token: 0x04004FD9 RID: 20441
		private bool mEnablePunish = false;

		// Token: 0x04004FDA RID: 20442
		private int mH = 0;

		// Token: 0x04004FDB RID: 20443
		private int mLocation = 0;

		// Token: 0x04004FDC RID: 20444
		private int mNewLocation = 0;

		// Token: 0x04004FDD RID: 20445
		private ushort mLocationX = 0;

		// Token: 0x04004FDE RID: 20446
		private ushort mLocationY = 0;

		// Token: 0x04004FDF RID: 20447
		private ushort mNewLocationX = 0;

		// Token: 0x04004FE0 RID: 20448
		private ushort mNewLocationY = 0;

		// Token: 0x04004FE1 RID: 20449
		private int mCloseNodeCounter = 0;

		// Token: 0x04004FE2 RID: 20450
		private ushort mGridX = 0;

		// Token: 0x04004FE3 RID: 20451
		private ushort mGridY = 0;

		// Token: 0x04004FE4 RID: 20452
		private ushort mGridXMinus1 = 0;

		// Token: 0x04004FE5 RID: 20453
		private ushort mGridYLog2 = 0;

		// Token: 0x04004FE6 RID: 20454
		private bool mFound = false;

		// Token: 0x04004FE7 RID: 20455
		private sbyte[,] mDirection = new sbyte[,]
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

		// Token: 0x04004FE8 RID: 20456
		private int mEndLocation = 0;

		// Token: 0x04004FE9 RID: 20457
		private int mNewG = 0;

		// Token: 0x020008E2 RID: 2274
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct PathFinderNodeFast
		{
			// Token: 0x04004FEA RID: 20458
			public int F;

			// Token: 0x04004FEB RID: 20459
			public int G;

			// Token: 0x04004FEC RID: 20460
			public ushort PX;

			// Token: 0x04004FED RID: 20461
			public ushort PY;

			// Token: 0x04004FEE RID: 20462
			public byte Status;
		}

		// Token: 0x020008E3 RID: 2275
		internal class ComparePFNodeMatrix : IComparer<int>
		{
			// Token: 0x060041C3 RID: 16835 RVA: 0x003C2866 File Offset: 0x003C0A66
			public ComparePFNodeMatrix(PathFinderFast.PathFinderNodeFast[] matrix)
			{
				this.mMatrix = matrix;
			}

			// Token: 0x060041C4 RID: 16836 RVA: 0x003C2878 File Offset: 0x003C0A78
			public int Compare(int a, int b)
			{
				int result;
				if (this.mMatrix[a].F > this.mMatrix[b].F)
				{
					result = 1;
				}
				else if (this.mMatrix[a].F < this.mMatrix[b].F)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
				return result;
			}

			// Token: 0x04004FEF RID: 20463
			private PathFinderFast.PathFinderNodeFast[] mMatrix;
		}
	}
}
