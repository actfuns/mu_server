using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x0200053F RID: 1343
	public class PropsCacheItem
	{
		// Token: 0x06001990 RID: 6544 RVA: 0x0018E2DC File Offset: 0x0018C4DC
		public PropsCacheItem(PropsCacheItem parent, int type)
		{
			this.ParentPropsCacheItem = parent;
			if (parent != null)
			{
				this.Path.AddRange(parent.Path);
				this.Path.Add(type);
			}
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0018E37C File Offset: 0x0018C57C
		public string GetName()
		{
			string name = "";
			for (int i = 0; i < this.Path.Count; i++)
			{
				if (i == 0)
				{
					name = name + "\\" + (PropsSystemTypes)this.Path[i];
				}
				else
				{
					name = name + "\\" + this.Path[i];
				}
			}
			return name;
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0018E3FC File Offset: 0x0018C5FC
		public void SetBaseProp(int index, double value)
		{
			PropsCacheItem parent = this.ParentPropsCacheItem;
			lock (this)
			{
				double change = value - this.BaseProps[index];
				if (change != 0.0)
				{
					bool enable = this.nowkg;
					this.BaseProps[index] = value;
					while (enable)
					{
						parent.BaseProps[index] += change;
						if (null == parent.ParentPropsCacheItem)
						{
							parent.Age++;
							this.UpdateBasePropCache(index, parent);
							break;
						}
						enable = parent.nowkg;
						parent = parent.ParentPropsCacheItem;
					}
				}
			}
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0018E4EC File Offset: 0x0018C6EC
		public void SetExtProp(int index, double value)
		{
			PropsCacheItem parent = this.ParentPropsCacheItem;
			lock (this)
			{
				double change = value - this.ExtProps[index];
				if (change != 0.0)
				{
					bool enable = this.nowkg;
					this.ExtProps[index] = value;
					while (enable && null != parent)
					{
						parent.ExtProps[index] += change;
						if (null == parent.ParentPropsCacheItem)
						{
							parent.Age++;
							this.UpdateExtPropCache(index, parent);
							break;
						}
						enable = parent.nowkg;
						parent = parent.ParentPropsCacheItem;
					}
				}
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0018E5E8 File Offset: 0x0018C7E8
		public void SetNodeBool(bool kg)
		{
			PropsCacheItem parent = this.ParentPropsCacheItem;
			lock (this)
			{
				if (this.nowkg != kg)
				{
					if (this.nowkg)
					{
						for (int i = 0; i < 177; i++)
						{
							if (this.ExtProps[i] != 0.0)
							{
								parent.SetExtProp(i, parent.ExtProps[i] - this.ExtProps[i]);
							}
						}
						for (int i = 0; i < 4; i++)
						{
							if (this.BaseProps[i] != 0.0)
							{
								parent.SetBaseProp(i, parent.BaseProps[i] - this.BaseProps[i]);
							}
						}
					}
					else
					{
						for (int i = 0; i < 177; i++)
						{
							if (this.ExtProps[i] != 0.0)
							{
								parent.SetExtProp(i, parent.ExtProps[i] + this.ExtProps[i]);
							}
						}
						for (int i = 0; i < 4; i++)
						{
							if (this.BaseProps[i] != 0.0)
							{
								parent.SetBaseProp(i, parent.BaseProps[i] + this.BaseProps[i]);
							}
						}
					}
				}
				this.nowkg = kg;
			}
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0018E794 File Offset: 0x0018C994
		private void UpdateExtPropCache(int Prop, PropsCacheItem item)
		{
			item.ExtPropsCache[Prop] = RoleAlgorithm.GetExtProp(item.Client, Prop);
			if (null != RoleAlgorithm.ExtListArray[Prop])
			{
				int count = RoleAlgorithm.ExtListArray[Prop].Count;
				for (int i = 0; i < count; i++)
				{
					double value = RoleAlgorithm.GetExtProp(item.Client, (int)RoleAlgorithm.ExtListArray[Prop][i]);
					item.ExtPropsCache[(int)RoleAlgorithm.ExtListArray[Prop][i]] = value;
				}
			}
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x0018E814 File Offset: 0x0018CA14
		private void UpdateBasePropCache(int BaseProp, PropsCacheItem item)
		{
			if (null != RoleAlgorithm.BaseListArray[BaseProp])
			{
				int count = RoleAlgorithm.BaseListArray[BaseProp].Count;
				for (int i = 0; i < count; i++)
				{
					double value = RoleAlgorithm.GetExtProp(item.Client, (int)RoleAlgorithm.BaseListArray[BaseProp][i]);
					item.ExtPropsCache[(int)RoleAlgorithm.BaseListArray[BaseProp][i]] = value;
				}
			}
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x0018E880 File Offset: 0x0018CA80
		public void ResetProps()
		{
			for (int i = 0; i < 4; i++)
			{
				this.BaseProps[i] = 0.0;
			}
			for (int i = 0; i < 177; i++)
			{
				this.ExtProps[i] = 0.0;
			}
			Array.Clear(this.ExtPropsCache, 0, this.ExtPropsCache.Length);
		}

		// Token: 0x040023ED RID: 9197
		public bool nowkg = true;

		// Token: 0x040023EE RID: 9198
		public GameClient Client;

		// Token: 0x040023EF RID: 9199
		public int Age = 0;

		// Token: 0x040023F0 RID: 9200
		public int LastAge = 0;

		// Token: 0x040023F1 RID: 9201
		public DelOnPropsChanged OnPropsChanged;

		// Token: 0x040023F2 RID: 9202
		public double[] BaseProps = new double[4];

		// Token: 0x040023F3 RID: 9203
		public double[] ExtProps = new double[177];

		// Token: 0x040023F4 RID: 9204
		public double[] ExtPropsCache = new double[177];

		// Token: 0x040023F5 RID: 9205
		public PropsCacheItem ParentPropsCacheItem;

		// Token: 0x040023F6 RID: 9206
		public Dictionary<int, PropsCacheItem> SubPropsItemDict = new Dictionary<int, PropsCacheItem>();

		// Token: 0x040023F7 RID: 9207
		public List<int> Path = new List<int>();
	}
}
