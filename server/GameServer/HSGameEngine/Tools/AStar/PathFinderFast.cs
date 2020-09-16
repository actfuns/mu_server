using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;

namespace HSGameEngine.Tools.AStar
{
    
    public class PathFinderFast : IPathFinder
    {
        
        // (add) Token: 0x0600419B RID: 16795 RVA: 0x003C13E8 File Offset: 0x003BF5E8
        // (remove) Token: 0x0600419C RID: 16796 RVA: 0x003C1424 File Offset: 0x003BF624
        public event PathFinderDebugHandler PathFinderDebug;

        
        public PathFinderFast(byte[,] grid)
        {
            if (grid == null)
            {
                throw new Exception("Grid cannot be null");
            }
            this.mGrid = grid;
            this.mGridX = (ushort)(this.mGrid.GetUpperBound(0) + 1);
            this.mGridY = (ushort)(this.mGrid.GetUpperBound(1) + 1);
            this.mGridXMinus1 = (ushort)(this.mGridX - 1);
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

        
        public void FindPathStop()
        {
            this.mStop = true;
        }

        
        public List<PathFinderNode> FindPath(Point start, Point end)
        {
            return this.FindPath(new Point2D((int)start.X, (int)start.Y), new Point2D((int)end.X, (int)end.Y));
        }

        
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
                            this.mNewLocationX = (ushort)(this.mLocationX + (ushort)this.mDirection[i, 0]);
                            this.mNewLocationY = (ushort)(this.mLocationY + (ushort)this.mDirection[i, 1]);
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

        
        private PriorityQueueB<int> mOpen = null;

        
        private List<PathFinderNode> mClose = new List<PathFinderNode>();

        
        private bool mStop = false;

        
        private bool mStopped = true;

        
        private int mHoriz = 0;

        
        private HeuristicFormula mFormula = HeuristicFormula.DiagonalShortCut;

        
        private bool mDiagonals = true;

        
        private int mHEstimate = 2;

        
        private bool mPunishChangeDirection = false;

        
        private bool mReopenCloseNodes = true;

        
        private bool mTieBreaker = false;

        
        private bool mHeavyDiagonals = false;

        
        private int mSearchLimit = 2000;

        
        private double mCompletedTime = 0.0;

        
        private bool mDebugProgress = false;

        
        private bool mDebugFoundPath = false;

        
        private static PathFinderFast.PathFinderNodeFast[] mCalcGrid = null;

        
        private byte mOpenNodeValue = 1;

        
        private byte mCloseNodeValue = 2;

        
        private int[,] mPunish = null;

        
        private int mMaxNum = 0;

        
        private bool mEnablePunish = false;

        
        private int mH = 0;

        
        private int mLocation = 0;

        
        private int mNewLocation = 0;

        
        private ushort mLocationX = 0;

        
        private ushort mLocationY = 0;

        
        private ushort mNewLocationX = 0;

        
        private ushort mNewLocationY = 0;

        
        private int mCloseNodeCounter = 0;

        
        private ushort mGridX = 0;

        
        private ushort mGridY = 0;

        
        private ushort mGridXMinus1 = 0;

        
        private ushort mGridYLog2 = 0;

        
        private bool mFound = false;

        
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

        
        private int mEndLocation = 0;

        
        private int mNewG = 0;

        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct PathFinderNodeFast
        {
            
            public int F;

            
            public int G;

            
            public ushort PX;

            
            public ushort PY;

            
            public byte Status;
        }

        
        internal class ComparePFNodeMatrix : IComparer<int>
        {
            
            public ComparePFNodeMatrix(PathFinderFast.PathFinderNodeFast[] matrix)
            {
                this.mMatrix = matrix;
            }

            
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

            
            private PathFinderFast.PathFinderNodeFast[] mMatrix;
        }
    }
}
