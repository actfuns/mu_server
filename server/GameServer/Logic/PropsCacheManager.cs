using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000540 RID: 1344
	public class PropsCacheManager
	{
		// Token: 0x06001998 RID: 6552 RVA: 0x0018E8ED File Offset: 0x0018CAED
		public PropsCacheManager(GameClient client)
		{
			this.PropsCacheRoot.Client = client;
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x0018E914 File Offset: 0x0018CB14
		public int GetAge()
		{
			int age;
			lock (this.PropsCacheRoot)
			{
				age = this.PropsCacheRoot.Age;
			}
			return age;
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x0018E968 File Offset: 0x0018CB68
		public bool IsChanged()
		{
			lock (this.PropsCacheRoot)
			{
				if (this.PropsCacheRoot.LastAge != this.PropsCacheRoot.Age)
				{
					this.PropsCacheRoot.LastAge = this.PropsCacheRoot.Age;
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x0018E9EC File Offset: 0x0018CBEC
		public double[] getCopyBaseProp()
		{
			double[] baseProps = this.PropsCacheRoot.BaseProps;
			double[] copyBaseProps = new double[baseProps.Length];
			for (int i = 0; i < baseProps.Length; i++)
			{
				copyBaseProps[i] = baseProps[i];
			}
			return copyBaseProps;
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x0018EA30 File Offset: 0x0018CC30
		public double[] getCopyExtProp()
		{
			double[] extProps = this.PropsCacheRoot.ExtProps;
			double[] copyExtProps = new double[extProps.Length];
			for (int i = 0; i < extProps.Length; i++)
			{
				copyExtProps[i] = extProps[i];
			}
			return copyExtProps;
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0018EA74 File Offset: 0x0018CC74
		public void SetBaseProps(params object[] args)
		{
			PropsCacheItem parent = this.PropsCacheRoot;
			PropsCacheItem child = null;
			double[] props = null;
			object propsObject = null;
			if (args.Length > 1)
			{
				propsObject = args[args.Length - 1];
				EquipPropItem equipPropItem = args[args.Length - 1] as EquipPropItem;
				if (null != equipPropItem)
				{
					props = equipPropItem.BaseProps;
				}
				else
				{
					props = (args[args.Length - 1] as double[]);
				}
			}
			if (null != props)
			{
				lock (this.PropsCacheRoot)
				{
					foreach (object obj in args)
					{
						if (obj == propsObject)
						{
							if (child != null)
							{
								Contract.Assert(child.SubPropsItemDict.Count == 0, "only leaf node can set props!");
								int i = 0;
								while (i < 4 && i < props.Length)
								{
									child.SetBaseProp(i, props[i]);
									i++;
								}
							}
							break;
						}
						if (!parent.SubPropsItemDict.TryGetValue((int)obj, out child))
						{
							child = new PropsCacheItem(parent, Convert.ToInt32(obj));
							parent.SubPropsItemDict.Add((int)obj, child);
						}
						parent = child;
					}
				}
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0018EC00 File Offset: 0x0018CE00
		public void SetBasePropsSingle(params object[] args)
		{
			PropsCacheItem parent = this.PropsCacheRoot;
			PropsCacheItem child = null;
			double propValue = 0.0;
			int propIndex = -1;
			try
			{
				if (args.Length <= 2)
				{
					return;
				}
				propIndex = (int)args[args.Length - 2];
				propValue = Convert.ToDouble(args[args.Length - 1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			if (propIndex >= 0 && propIndex < 4)
			{
				lock (this.PropsCacheRoot)
				{
					for (int i = 0; i < args.Length - 2; i++)
					{
						if (!parent.SubPropsItemDict.TryGetValue((int)args[i], out child))
						{
							child = new PropsCacheItem(parent, Convert.ToInt32(args[i]));
							parent.SubPropsItemDict.Add((int)args[i], child);
						}
						parent = child;
					}
					if (child != null)
					{
						Contract.Assert(child.SubPropsItemDict.Count == 0, "only leaf node can set props!");
						child.SetBaseProp(propIndex, propValue);
					}
				}
			}
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x0018ED5C File Offset: 0x0018CF5C
		public void SetExtProps(params object[] args)
		{
			PropsCacheItem parent = this.PropsCacheRoot;
			PropsCacheItem child = null;
			double[] props = null;
			object propsObject = null;
			try
			{
				if (args.Length > 1)
				{
					propsObject = args[args.Length - 1];
					EquipPropItem equipPropItem = propsObject as EquipPropItem;
					if (null != equipPropItem)
					{
						props = equipPropItem.ExtProps;
					}
					else
					{
						props = (args[args.Length - 1] as double[]);
					}
				}
				lock (this.PropsCacheRoot)
				{
					foreach (object obj in args)
					{
						if (obj == propsObject)
						{
							if (child != null)
							{
								Contract.Assert(child.SubPropsItemDict.Count == 0, "only leaf node can set props!");
								if (null != props)
								{
									int i = 0;
									while (i < 177 && i < props.Length)
									{
										child.SetExtProp(i, props[i]);
										i++;
									}
								}
								else
								{
									for (int i = 0; i < 177; i++)
									{
										child.SetExtProp(i, 0.0);
									}
								}
							}
							break;
						}
						if (!parent.SubPropsItemDict.TryGetValue((int)obj, out child))
						{
							child = new PropsCacheItem(parent, Convert.ToInt32(obj));
							parent.SubPropsItemDict.Add((int)obj, child);
						}
						parent = child;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x0018EF54 File Offset: 0x0018D154
		public void SetExtPropsSingle(params object[] args)
		{
			PropsCacheItem parent = this.PropsCacheRoot;
			PropsCacheItem child = null;
			double propValue = 0.0;
			int propIndex = -1;
			try
			{
				if (args.Length <= 2)
				{
					return;
				}
				propIndex = (int)args[args.Length - 2];
				propValue = Convert.ToDouble(args[args.Length - 1]);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			if (propIndex >= 0 && propIndex < 177)
			{
				lock (this.PropsCacheRoot)
				{
					for (int i = 0; i < args.Length - 2; i++)
					{
						if (!parent.SubPropsItemDict.TryGetValue((int)args[i], out child))
						{
							child = new PropsCacheItem(parent, Convert.ToInt32(args[i]));
							parent.SubPropsItemDict.Add((int)args[i], child);
						}
						parent = child;
					}
					if (child != null)
					{
						Contract.Assert(child.SubPropsItemDict.Count == 0, "only leaf node can set props!");
						child.SetExtProp(propIndex, propValue);
					}
				}
			}
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0018F0B4 File Offset: 0x0018D2B4
		public List<PropsCacheItem> GetAllPropsCacheItems(PropsCacheItem parent = null)
		{
			List<PropsCacheItem> list = new List<PropsCacheItem>();
			if (null == parent)
			{
				parent = this.PropsCacheRoot;
			}
			lock (this.PropsCacheRoot)
			{
				if (parent.SubPropsItemDict.Count > 0)
				{
					foreach (PropsCacheItem item in parent.SubPropsItemDict.Values)
					{
						list.AddRange(this.GetAllPropsCacheItems(item));
					}
				}
				else
				{
					list.Add(parent);
				}
			}
			return list;
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x0018F19C File Offset: 0x0018D39C
		public double GetBaseProp(int index)
		{
			double tempProp = 0.0;
			lock (this.PropsCacheRoot)
			{
				tempProp = this.PropsCacheRoot.BaseProps[index];
			}
			return tempProp;
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x0018F200 File Offset: 0x0018D400
		public double GetExtProp(int index)
		{
			double tempProp = 0.0;
			lock (this.PropsCacheRoot)
			{
				tempProp = this.PropsCacheRoot.ExtProps[index];
			}
			return tempProp;
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0018F264 File Offset: 0x0018D464
		public void SetNodeState(params object[] args)
		{
			PropsCacheItem parent = this.PropsCacheRoot;
			bool enable;
			try
			{
				if (args.Length <= 1)
				{
					return;
				}
				enable = (bool)args[args.Length - 1];
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return;
			}
			lock (this.PropsCacheRoot)
			{
				PropsCacheItem child = null;
				for (int i = 0; i < args.Length - 1; i++)
				{
					if (!parent.SubPropsItemDict.TryGetValue((int)args[i], out child))
					{
						child = new PropsCacheItem(parent, Convert.ToInt32(args[i]));
						parent.SubPropsItemDict.Add((int)args[i], child);
					}
					parent = child;
				}
				if (child != null)
				{
					child.SetNodeBool(enable);
				}
			}
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x0018F374 File Offset: 0x0018D574
		public double GetExtPropFinal(int index)
		{
			double result;
			lock (this.PropsCacheRoot)
			{
				result = this.PropsCacheRoot.ExtPropsCache[index];
			}
			return result;
		}

		// Token: 0x040023F8 RID: 9208
		private PropsCacheItem PropsCacheRoot = new PropsCacheItem(null, 0);

		// Token: 0x040023F9 RID: 9209
		public static readonly double[] ConstBaseProps = new double[4];

		// Token: 0x040023FA RID: 9210
		public static readonly double[] ConstExtProps = new double[177];
	}
}
