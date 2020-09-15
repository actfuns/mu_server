using System;
using System.Collections.Generic;
using System.Linq;

namespace HSGameEngine.Tools.AStarEx
{
	// Token: 0x020008D7 RID: 2263
	public class BinaryStack
	{
		// Token: 0x06004141 RID: 16705 RVA: 0x003BFCFC File Offset: 0x003BDEFC
		public BinaryStack(string compareValue = "f")
		{
			this._data = new List<long>(1000);
			this._dict = new Dictionary<long, int>(1000);
		}

		// Token: 0x06004142 RID: 16706 RVA: 0x003BFD3C File Offset: 0x003BDF3C
		public void push(long guid)
		{
			this._data.Add(guid);
			this._dict[guid] = this._data.Count - 1;
			int len = this._data.Count;
			if (len > 1)
			{
				int index = len;
				int parentIndex = index / 2 - 1;
				while (this.compareTwoNodes(guid, this._data[parentIndex]))
				{
					long temp = this._data[parentIndex];
					this._data[parentIndex] = guid;
					this._dict[guid] = parentIndex;
					this._data[index - 1] = temp;
					this._dict[temp] = index - 1;
					index /= 2;
					parentIndex = index / 2 - 1;
					if (parentIndex < 0)
					{
						break;
					}
				}
			}
		}

		// Token: 0x06004143 RID: 16707 RVA: 0x003BFE18 File Offset: 0x003BE018
		public long shift()
		{
			long result = this._data.ElementAt(0);
			this._data.RemoveAt(0);
			this._dict.Remove(result);
			int len = this._data.Count;
			if (len > 1)
			{
				long lastNode = this._data.ElementAt(this._data.Count - 1);
				this._data.RemoveAt(this._data.Count - 1);
				this._data.Insert(0, lastNode);
				this._dict[lastNode] = 0;
				int index = 0;
				for (int childIndex = (index + 1) * 2 - 1; childIndex < len; childIndex = (index + 1) * 2 - 1)
				{
					int comparedIndex;
					if (childIndex + 1 == len)
					{
						comparedIndex = childIndex;
					}
					else
					{
						comparedIndex = (this.compareTwoNodes(this._data[childIndex], this._data[childIndex + 1]) ? childIndex : (childIndex + 1));
					}
					if (comparedIndex < 0)
					{
						break;
					}
					if (!this.compareTwoNodes(this._data[comparedIndex], lastNode))
					{
						break;
					}
					long temp = this._data[comparedIndex];
					this._data[comparedIndex] = lastNode;
					this._dict[lastNode] = comparedIndex;
					this._data[index] = temp;
					this._dict[temp] = index;
					index = comparedIndex;
				}
			}
			return result;
		}

		// Token: 0x06004144 RID: 16708 RVA: 0x003BFFB8 File Offset: 0x003BE1B8
		public void updateNode(long node)
		{
			int indexObj = this.indexOf(node);
			if (indexObj >= 0)
			{
				int index = indexObj + 1;
				int parentIndex = index / 2 - 1;
				if (parentIndex >= 0)
				{
					while (this.compareTwoNodes(node, this._data[parentIndex]))
					{
						long temp = this._data[parentIndex];
						this._data[parentIndex] = node;
						this._dict[node] = parentIndex;
						this._data[index - 1] = temp;
						this._dict[temp] = index - 1;
						index /= 2;
						parentIndex = index / 2 - 1;
						if (parentIndex < 0)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06004145 RID: 16709 RVA: 0x003C0080 File Offset: 0x003BE280
		public int indexOf(long node)
		{
			int findIndex = -1;
			int result;
			if (this._dict.TryGetValue(node, out findIndex))
			{
				result = findIndex;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x06004146 RID: 16710 RVA: 0x003C00B0 File Offset: 0x003BE2B0
		public int getLength()
		{
			return this._data.Count;
		}

		// Token: 0x06004147 RID: 16711 RVA: 0x003C00D0 File Offset: 0x003BE2D0
		private bool compareTwoNodes(long node1, long node2)
		{
			double f = this._nodeGrid.Nodes[ANode.GetGUID_X(node1), ANode.GetGUID_Y(node1)].f;
			double f2 = this._nodeGrid.Nodes[ANode.GetGUID_X(node2), ANode.GetGUID_Y(node2)].f;
			return f < f2;
		}

		// Token: 0x06004148 RID: 16712 RVA: 0x003C012C File Offset: 0x003BE32C
		public string toString()
		{
			string result = "";
			int len = this._data.Count;
			for (int i = 0; i < len; i++)
			{
				double f = this._nodeGrid.Nodes[ANode.GetGUID_X(this._data[i]), ANode.GetGUID_Y(this._data[i])].f;
				result += f;
				if (i < len - 1)
				{
					result += ",";
				}
			}
			return result;
		}

		// Token: 0x06004149 RID: 16713 RVA: 0x003C01C7 File Offset: 0x003BE3C7
		public void ClearAll()
		{
			this._data.Clear();
			this._dict.Clear();
		}

		// Token: 0x04004F8B RID: 20363
		public NodeGrid _nodeGrid = null;

		// Token: 0x04004F8C RID: 20364
		public List<long> _data = null;

		// Token: 0x04004F8D RID: 20365
		private Dictionary<long, int> _dict = null;
	}
}
