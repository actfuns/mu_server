using System;

namespace HSGameEngine.Tools.AStarEx
{
	
	public class NodeGrid : ICloneable
	{
		
		public NodeGrid(int numCols, int numRows)
		{
			this.setSize(numCols, numRows);
		}

		
		public byte[,] GetFixedObstruction()
		{
			return this._fixedObstruction;
		}

		
		public void setSize(int numCols, int numRows)
		{
			if (NodeGrid._nodes == null || NodeGrid._numCols < numCols || NodeGrid._numRows < numRows)
			{
				NodeGrid._numCols = Math.Max(numCols, NodeGrid._numCols);
				NodeGrid._numRows = Math.Max(numRows, NodeGrid._numRows);
				NodeGrid._nodes = new NodeFast[NodeGrid._numCols, NodeGrid._numRows];
			}
			this._fixedObstruction = new byte[numCols, numRows];
			for (int i = 0; i < numCols; i++)
			{
				for (int j = 0; j < numRows; j++)
				{
					this._fixedObstruction[i, j] = 1;
				}
			}
		}

		
		public void Clear()
		{
			Array.Clear(NodeGrid._nodes, 0, NodeGrid._nodes.Length);
		}

		
		
		public NodeFast[,] Nodes
		{
			get
			{
				return NodeGrid._nodes;
			}
		}

		
		public bool isDiagonalWalkable(long node1, long node2)
		{
			int node1x = ANode.GetGUID_X(node1);
			int node1y = ANode.GetGUID_Y(node1);
			int node2x = ANode.GetGUID_X(node2);
			int node2y = ANode.GetGUID_Y(node2);
			return 1 == this._fixedObstruction[node1x, node2y] && 1 == this._fixedObstruction[node2x, node1y];
		}

		
		public void setEndNode(int x, int y)
		{
			this._endNodeX = x;
			this._endNodeY = y;
		}

		
		public void setStartNode(int x, int y)
		{
			this._startNodeX = x;
			this._startNodeY = y;
		}

		
		public void setWalkable(int x, int y, bool value)
		{
			if (value)
			{
				this._fixedObstruction[x, y] = 1;
			}
			else
			{
				this._fixedObstruction[x, y] = 0;
			}
		}

		
		public bool isWalkable(int x, int y)
		{
			return 1 == this._fixedObstruction[x, y];
		}

		
		
		public int endNodeX
		{
			get
			{
				return this._endNodeX;
			}
		}

		
		
		public int endNodeY
		{
			get
			{
				return this._endNodeY;
			}
		}

		
		
		public int numCols
		{
			get
			{
				return NodeGrid._numCols;
			}
		}

		
		
		public int numRows
		{
			get
			{
				return NodeGrid._numRows;
			}
		}

		
		
		public int startNodeX
		{
			get
			{
				return this._startNodeX;
			}
		}

		
		
		public int startNodeY
		{
			get
			{
				return this._startNodeY;
			}
		}

		
		public object Clone()
		{
			NodeGrid obj = base.MemberwiseClone() as NodeGrid;
			obj._fixedObstruction = (this._fixedObstruction.Clone() as byte[,]);
			return obj;
		}

		
		private int _startNodeX;

		
		private int _startNodeY;

		
		private int _endNodeX;

		
		private int _endNodeY;

		
		private static NodeFast[,] _nodes;

		
		private byte[,] _fixedObstruction;

		
		private static int _numCols;

		
		private static int _numRows;
	}
}
