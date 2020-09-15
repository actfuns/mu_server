using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	// Token: 0x02000798 RID: 1944
	public class SingleEquipProps
	{
		// Token: 0x060032B4 RID: 12980 RVA: 0x002CF8A8 File Offset: 0x002CDAA8
		public List<double[]> GetSingleEquipPropsList(int occupation, int categoriy, int suitID)
		{
			List<double[]> result;
			if (null == this._SingleEquipItemsDict)
			{
				result = null;
			}
			else
			{
				string key = string.Format("{0}_{1}_{2}", occupation, categoriy, suitID);
				List<double[]> propsList = null;
				if (!this._SingleEquipItemsDict.TryGetValue(key, out propsList))
				{
					result = null;
				}
				else
				{
					result = propsList;
				}
			}
			return result;
		}

		// Token: 0x060032B5 RID: 12981 RVA: 0x002CF908 File Offset: 0x002CDB08
		private List<double[]> ParseSystemXmlItem(SystemXmlItem xmlItem)
		{
			string equipProps = xmlItem.GetStringValue("EquipProps");
			List<double[]> result;
			if (string.IsNullOrEmpty(equipProps))
			{
				result = null;
			}
			else
			{
				string[] fields = equipProps.Split(new char[]
				{
					'|'
				});
				if (fields == null || fields.Length <= 0)
				{
					result = null;
				}
				else
				{
					List<double[]> propsList = new List<double[]>();
					for (int i = 0; i < fields.Length; i++)
					{
						propsList.Add(this.ParseStringProps(fields[i]));
					}
					result = propsList;
				}
			}
			return result;
		}

		// Token: 0x060032B6 RID: 12982 RVA: 0x002CF99C File Offset: 0x002CDB9C
		private double[] ParseStringProps(string props)
		{
			double[] result;
			if (string.IsNullOrEmpty(props))
			{
				result = null;
			}
			else
			{
				double[] doubleProps = Global.String2DoubleArray(props, ',');
				if (doubleProps == null || doubleProps.Length != 10)
				{
					result = null;
				}
				else
				{
					for (int i = 0; i < doubleProps.Length; i++)
					{
						doubleProps[i] = Global.GMax(0.0, doubleProps[i]);
					}
					result = doubleProps;
				}
			}
			return result;
		}

		// Token: 0x060032B7 RID: 12983 RVA: 0x002CFA0C File Offset: 0x002CDC0C
		private void LoadEquipPropItemsByOccupation(string pathName, int occupation)
		{
			XElement xml = null;
			string fileName = "";
			try
			{
				fileName = string.Format("{0}{1}.xml", pathName, occupation);
				xml = XElement.Load(Global.GameResPath(fileName));
				if (null == xml)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
				}
			}
			catch (Exception)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> xmlItems = xml.Elements("Equip");
			foreach (XElement xmltem in xmlItems)
			{
				IEnumerable<XElement> items = xmltem.Elements("Item");
				foreach (XElement item in items)
				{
					SystemXmlItem systemXmlItem = new SystemXmlItem
					{
						XMLNode = item
					};
					string key = string.Format("{0}_{1}_{2}", occupation, (int)Global.GetSafeAttributeLong(xmltem, "Categoriy"), (int)Global.GetSafeAttributeLong(item, "SuitID"));
					this._SingleEquipItemsDict[key] = this.ParseSystemXmlItem(systemXmlItem);
				}
			}
		}

		// Token: 0x060032B8 RID: 12984 RVA: 0x002CFB9C File Offset: 0x002CDD9C
		public void LoadEquipPropItems(string pathName)
		{
			this.LoadEquipPropItemsByOccupation(pathName, 0);
			this.LoadEquipPropItemsByOccupation(pathName, 1);
			this.LoadEquipPropItemsByOccupation(pathName, 2);
		}

		// Token: 0x04003EBD RID: 16061
		private Dictionary<string, List<double[]>> _SingleEquipItemsDict = new Dictionary<string, List<double[]>>();
	}
}
