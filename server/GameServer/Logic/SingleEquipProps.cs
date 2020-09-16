using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameServer.Logic
{
	
	public class SingleEquipProps
	{
		
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

		
		public void LoadEquipPropItems(string pathName)
		{
			this.LoadEquipPropItemsByOccupation(pathName, 0);
			this.LoadEquipPropItemsByOccupation(pathName, 1);
			this.LoadEquipPropItemsByOccupation(pathName, 2);
		}

		
		private Dictionary<string, List<double[]>> _SingleEquipItemsDict = new Dictionary<string, List<double[]>>();
	}
}
