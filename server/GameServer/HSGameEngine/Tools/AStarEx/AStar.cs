using System;
using System.Collections.Generic;
using System.Diagnostics;
using Server.Tools;

namespace HSGameEngine.Tools.AStarEx
{
	
	public class AStar
	{
		
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

		
		private int IndexOfClose(long node)
		{
			return this._closed.ContainsKey(node) ? 0 : -1;
		}

		
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

		
		private bool isDiagonalWalkable(long node1, long node2)
		{
			return this._grid.isDiagonalWalkable(node1, node2);
		}

		
		private double diagonal(int nodex, int nodey)
		{
			double dx = (double)((nodex - this._endNodeX < 0) ? (this._endNodeX - nodex) : (nodex - this._endNodeX));
			double dy = (double)((nodey - this._endNodeY < 0) ? (this._endNodeY - nodey) : (nodey - this._endNodeY));
			double diag = (dx < dy) ? dx : dy;
			double straight = dx + dy;
			return this._diagCost * diag + this._straightCost * (straight - 2.0 * diag);
		}

		
		
		public List<ANode> path
		{
			get
			{
				return this._path;
			}
		}

		
		public const double costMultiplier = 1.0;

		
		private BinaryStack _open;

		
		private Dictionary<long, bool> _closed;

		
		private NodeGrid _grid;

		
		private int _endNodeX;

		
		private int _endNodeY;

		
		private int _startNodeX;

		
		private int _startNodeY;

		
		private List<ANode> _path;

		
		private double _straightCost = 1.0;

		
		private double _diagCost = 1.4142135623730951;

		
		public static int MaxOpenNodeCount = 200;
	}
}
