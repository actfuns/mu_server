using System;

namespace HSGameEngine.Tools.AStarEx
{
	// Token: 0x020008D9 RID: 2265
	public class NodeGrid : ICloneable
	{
		// Token: 0x0600414A RID: 16714 RVA: 0x003C01E2 File Offset: 0x003BE3E2
		public NodeGrid(int numCols, int numRows)
		{
			this.setSize(numCols, numRows);
		}

		// Token: 0x0600414B RID: 16715 RVA: 0x003C01F8 File Offset: 0x003BE3F8
		public byte[,] GetFixedObstruction()
		{
			return this._fixedObstruction;
		}

		// Token: 0x0600414C RID: 16716 RVA: 0x003C0210 File Offset: 0x003BE410
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

		// Token: 0x0600414D RID: 16717 RVA: 0x003C02B5 File Offset: 0x003BE4B5
		public void Clear()
		{
			Array.Clear(NodeGrid._nodes, 0, NodeGrid._nodes.Length);
		}

		// Token: 0x17000630 RID: 1584
		// (get) Token: 0x0600414E RID: 16718 RVA: 0x003C02D0 File Offset: 0x003BE4D0
		public NodeFast[,] Nodes
		{
			get
			{
				return NodeGrid._nodes;
			}
		}

		// Token: 0x0600414F RID: 16719 RVA: 0x003C02E8 File Offset: 0x003BE4E8
		public bool isDiagonalWalkable(long node1, long node2)
		{
			int node1x = ANode.GetGUID_X(node1);
			int node1y = ANode.GetGUID_Y(node1);
			int node2x = ANode.GetGUID_X(node2);
			int node2y = ANode.GetGUID_Y(node2);
			return 1 == this._fixedObstruction[node1x, node2y] && 1 == this._fixedObstruction[node2x, node1y];
		}

		// Token: 0x06004150 RID: 16720 RVA: 0x003C034C File Offset: 0x003BE54C
		public void setEndNode(int x, int y)
		{
			this._endNodeX = x;
			this._endNodeY = y;
		}

		// Token: 0x06004151 RID: 16721 RVA: 0x003C035D File Offset: 0x003BE55D
		public void setStartNode(int x, int y)
		{
			this._startNodeX = x;
			this._startNodeY = y;
		}

		// Token: 0x06004152 RID: 16722 RVA: 0x003C0370 File Offset: 0x003BE570
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

		// Token: 0x06004153 RID: 16723 RVA: 0x003C03A8 File Offset: 0x003BE5A8
		public bool isWalkable(int x, int y)
		{
			return 1 == this._fixedObstruction[x, y];
		}

		// Token: 0x17000631 RID: 1585
		// (get) Token: 0x06004154 RID: 16724 RVA: 0x003C03CC File Offset: 0x003BE5CC
		public int endNodeX
		{
			get
			{
				return this._endNodeX;
			}
		}

		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06004155 RID: 16725 RVA: 0x003C03E4 File Offset: 0x003BE5E4
		public int endNodeY
		{
			get
			{
				return this._endNodeY;
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06004156 RID: 16726 RVA: 0x003C03FC File Offset: 0x003BE5FC
		public int numCols
		{
			get
			{
				return NodeGrid._numCols;
			}
		}

		// Token: 0x17000634 RID: 1588
		// (get) Token: 0x06004157 RID: 16727 RVA: 0x003C0414 File Offset: 0x003BE614
		public int numRows
		{
			get
			{
				return NodeGrid._numRows;
			}
		}

		// Token: 0x17000635 RID: 1589
		// (get) Token: 0x06004158 RID: 16728 RVA: 0x003C042C File Offset: 0x003BE62C
		public int startNodeX
		{
			get
			{
				return this._startNodeX;
			}
		}

		// Token: 0x17000636 RID: 1590
		// (get) Token: 0x06004159 RID: 16729 RVA: 0x003C0444 File Offset: 0x003BE644
		public int startNodeY
		{
			get
			{
				return this._startNodeY;
			}
		}

		// Token: 0x0600415A RID: 16730 RVA: 0x003C045C File Offset: 0x003BE65C
		public object Clone()
		{
			NodeGrid obj = base.MemberwiseClone() as NodeGrid;
			obj._fixedObstruction = (this._fixedObstruction.Clone() as byte[,]);
			return obj;
		}

		// Token: 0x04004F93 RID: 20371
		private int _startNodeX;

		// Token: 0x04004F94 RID: 20372
		private int _startNodeY;

		// Token: 0x04004F95 RID: 20373
		private int _endNodeX;

		// Token: 0x04004F96 RID: 20374
		private int _endNodeY;

		// Token: 0x04004F97 RID: 20375
		private static NodeFast[,] _nodes;

		// Token: 0x04004F98 RID: 20376
		private byte[,] _fixedObstruction;

		// Token: 0x04004F99 RID: 20377
		private static int _numCols;

		// Token: 0x04004F9A RID: 20378
		private static int _numRows;
	}
}
