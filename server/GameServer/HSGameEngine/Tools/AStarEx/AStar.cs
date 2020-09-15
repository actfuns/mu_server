using System;
using System.Collections.Generic;
using System.Diagnostics;
using Server.Tools;

namespace HSGameEngine.Tools.AStarEx
{
	// Token: 0x020008D6 RID: 2262
	public class AStar
	{
		// Token: 0x06004138 RID: 16696 RVA: 0x003BF4F0 File Offset: 0x003BD6F0
		public List<ANode> find(NodeGrid grid)
		{
			List<ANode> result;
			if (this.findPath(grid))
			{
				result = this._path;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06004139 RID: 16697 RVA: 0x003BF51C File Offset: 0x003BD71C
		public bool findPath(NodeGrid grid)
		{
			this._grid = grid;
			if (null == this._open)
			{
				this._open = new BinaryStack("f");
			}
			else
			{
				this._open.ClearAll();
			}
			grid.Clear();
			this._open._nodeGrid = grid;
			if (null == this._closed)
			{
				this._closed = new Dictionary<long, bool>(1000);
			}
			else
			{
				this._closed.Clear();
			}
			this._startNodeX = this._grid.startNodeX;
			this._startNodeY = this._grid.startNodeY;
			this._endNodeX = this._grid.endNodeX;
			this._endNodeY = this._grid.endNodeY;
			this._grid.Nodes[this._startNodeX, this._startNodeY].g = 0.0;
			this._grid.Nodes[this._startNodeX, this._startNodeY].h = this.diagonal(this._startNodeX, this._startNodeY);
			this._grid.Nodes[this._startNodeX, this._startNodeY].f = this._grid.Nodes[this._startNodeX, this._startNodeY].g + this._grid.Nodes[this._startNodeX, this._startNodeY].h;
			return this.search();
		}

		// Token: 0x0600413A RID: 16698 RVA: 0x003BF6B8 File Offset: 0x003BD8B8
		public bool search()
		{
			try
			{
				long node = ANode.GetGUID(this._startNodeX, this._startNodeY);
				long endNode = ANode.GetGUID(this._endNodeX, this._endNodeY);
				int nodex;
				int nodey;
				while (node != endNode)
				{
					nodex = ANode.GetGUID_X(node);
					nodey = ANode.GetGUID_Y(node);
					int startX = (0 > nodex - 1) ? 0 : (nodex - 1);
					int endX = (this._grid.numCols - 1 < nodex + 1) ? (this._grid.numCols - 1) : (nodex + 1);
					int startY = (0 > nodey - 1) ? 0 : (nodey - 1);
					int endY = (this._grid.numRows - 1 < nodey + 1) ? (this._grid.numRows - 1) : (nodey + 1);
					for (int i = startX; i <= endX; i++)
					{
						for (int j = startY; j <= endY; j++)
						{
							if (this._open.getLength() > AStar.MaxOpenNodeCount)
							{
								LogManager.WriteLog(LogTypes.Warning, string.Format("AStar:search()待检测的点太多，没必要再寻路: start({0}, {1}), to({2}, {3}), MaxOpenNodeCount={4}", new object[]
								{
									this._startNodeX,
									this._startNodeY,
									this._endNodeX,
									this._endNodeY,
									AStar.MaxOpenNodeCount
								}), null, true);
								return false;
							}
							long test = ANode.GetGUID(i, j);
							int testx = i;
							int testy = j;
							bool isTestWalkable = this._grid.isWalkable(testx, testy);
							if (test != node && isTestWalkable && this._grid.isDiagonalWalkable(node, test))
							{
								double cost = this._straightCost;
								if (nodex != testx && nodey != testy)
								{
									cost = this._diagCost;
								}
								double nodeg = this._grid.Nodes[nodex, nodey].g;
								double g = nodeg + cost * 1.0;
								double h = this.diagonal(testx, testy);
								double f = g + h;
								bool isInOpen = this._open.indexOf(test) != -1;
								int indexOfClose = this.IndexOfClose(test);
								if (isInOpen || indexOfClose != -1)
								{
									if (this._grid.Nodes[testx, testy].f > f)
									{
										this._grid.Nodes[testx, testy].f = f;
										this._grid.Nodes[testx, testy].g = g;
										this._grid.Nodes[testx, testy].h = h;
										this._grid.Nodes[testx, testy].parentX = nodex;
										this._grid.Nodes[testx, testy].parentY = nodey;
										if (isInOpen)
										{
											this._open.updateNode(test);
										}
									}
								}
								else
								{
									this._grid.Nodes[testx, testy].f = f;
									this._grid.Nodes[testx, testy].g = g;
									this._grid.Nodes[testx, testy].h = h;
									this._grid.Nodes[testx, testy].parentX = nodex;
									this._grid.Nodes[testx, testy].parentY = nodey;
									this._open.push(test);
								}
							}
						}
					}
					this._closed[node] = true;
					if (this._open.getLength() == 0)
					{
						return false;
					}
					node = this._open.shift();
				}
				nodex = ANode.GetGUID_X(node);
				nodey = ANode.GetGUID_Y(node);
				this._endNodeX = nodex;
				this._endNodeY = nodey;
				this.buildPath();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}
			return true;
		}

		// Token: 0x0600413B RID: 16699 RVA: 0x003BFB30 File Offset: 0x003BDD30
		private int IndexOfClose(long node)
		{
			return this._closed.ContainsKey(node) ? 0 : -1;
		}

		// Token: 0x0600413C RID: 16700 RVA: 0x003BFB58 File Offset: 0x003BDD58
		private void buildPath()
		{
			this._path = new List<ANode>();
			ANode node = new ANode(this._endNodeX, this._endNodeY);
			this._path.Add(node);
			int count = 0;
			while (node.x != this._startNodeX || node.y != this._startNodeY)
			{
				int px = this._grid.Nodes[node.x, node.y].parentX;
				int py = this._grid.Nodes[node.x, node.y].parentY;
				node = new ANode(px, py);
				this._path.Insert(0, node);
				count++;
			}
			Debug.WriteLine(string.Format("Find Path count={0}", count));
		}

		// Token: 0x0600413D RID: 16701 RVA: 0x003BFC38 File Offset: 0x003BDE38
		private bool isDiagonalWalkable(long node1, long node2)
		{
			return this._grid.isDiagonalWalkable(node1, node2);
		}

		// Token: 0x0600413E RID: 16702 RVA: 0x003BFC58 File Offset: 0x003BDE58
		private double diagonal(int nodex, int nodey)
		{
			double dx = (double)((nodex - this._endNodeX < 0) ? (this._endNodeX - nodex) : (nodex - this._endNodeX));
			double dy = (double)((nodey - this._endNodeY < 0) ? (this._endNodeY - nodey) : (nodey - this._endNodeY));
			double diag = (dx < dy) ? dx : dy;
			double straight = dx + dy;
			return this._diagCost * diag + this._straightCost * (straight - 2.0 * diag);
		}

		// Token: 0x1700062F RID: 1583
		// (get) Token: 0x0600413F RID: 16703 RVA: 0x003BFCD8 File Offset: 0x003BDED8
		public List<ANode> path
		{
			get
			{
				return this._path;
			}
		}

		// Token: 0x04004F7F RID: 20351
		public const double costMultiplier = 1.0;

		// Token: 0x04004F80 RID: 20352
		private BinaryStack _open;

		// Token: 0x04004F81 RID: 20353
		private Dictionary<long, bool> _closed;

		// Token: 0x04004F82 RID: 20354
		private NodeGrid _grid;

		// Token: 0x04004F83 RID: 20355
		private int _endNodeX;

		// Token: 0x04004F84 RID: 20356
		private int _endNodeY;

		// Token: 0x04004F85 RID: 20357
		private int _startNodeX;

		// Token: 0x04004F86 RID: 20358
		private int _startNodeY;

		// Token: 0x04004F87 RID: 20359
		private List<ANode> _path;

		// Token: 0x04004F88 RID: 20360
		private double _straightCost = 1.0;

		// Token: 0x04004F89 RID: 20361
		private double _diagCost = 1.4142135623730951;

		// Token: 0x04004F8A RID: 20362
		public static int MaxOpenNodeCount = 200;
	}
}
