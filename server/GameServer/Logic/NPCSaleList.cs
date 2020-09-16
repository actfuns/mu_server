using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class NPCSaleList
	{
		
		
		public Dictionary<int, NPCSaleItem> SaleIDSDict
		{
			get
			{
				return this._SaleIDSDict;
			}
		}

		
		public bool LoadSaleList()
		{
			string fileName = string.Format("Config/NPCSaleList.xml", new object[0]);
			XElement xml = XElement.Load(Global.GameResPath(fileName));
			if (null == xml)
			{
				throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", fileName));
			}
			IEnumerable<XElement> saleNodes = xml.Elements("SaleList").Elements<XElement>();
			bool result;
			if (null == saleNodes)
			{
				result = false;
			}
			else
			{
				Dictionary<int, NPCSaleItem> saleIDSDict = new Dictionary<int, NPCSaleItem>();
				foreach (XElement xmlNode in saleNodes)
				{
					int saleType = (int)Global.GetSafeAttributeLong(xmlNode, "SaleType");
					string saleItems = Global.GetSafeAttributeStr(xmlNode, "Items");
					string[] itemFields = saleItems.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < itemFields.Length; i++)
					{
						string[] fields2 = itemFields[i].Split(new char[]
						{
							','
						});
						if (fields2.Length != 5)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("加载NPC出售列表时, 物品配置项个数错误，忽略。{0}", itemFields[i]), null, true);
						}
						else
						{
							XElement element = null;
							try
							{
								element = Global.GetSafeXElement(Global.XmlInfo["Configgoods"], "Item", "ID", fields2[0]);
							}
							catch (Exception)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("加载NPC出售列表时, 物品不存在，忽略。GoodsID={0}", itemFields[0]), null, true);
								goto IL_274;
							}
							int key = (int)Global.GetSafeAttributeLong(element, "ID");
							NPCSaleItem nPCSaleItem = null;
							if (!saleIDSDict.TryGetValue(key, out nPCSaleItem))
							{
								nPCSaleItem = new NPCSaleItem
								{
									Money1Price = (int)Global.GetSafeAttributeLong(element, "PriceOne"),
									YinLiangPrice = (int)Global.GetSafeAttributeLong(element, "PriceTwo"),
									TianDiJingYuanPrice = (int)Global.GetSafeAttributeLong(element, "JinYuanPrice"),
									LieShaZhiPrice = (int)Global.GetSafeAttributeLong(element, "LieShaPrice"),
									JiFenPrice = (int)Global.GetSafeAttributeLong(element, "JiFenPrice"),
									ZhanHunPrice = (int)Global.GetSafeAttributeLong(element, "ZhanHunPrice"),
									Forge_level = Math.Max(0, Global.SafeConvertToInt32(fields2[1])),
									AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(fields2[2])),
									Lucky = Math.Max(0, Global.SafeConvertToInt32(fields2[3])),
									ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(fields2[4]))
								};
							}
							nPCSaleItem.SaleTypesDict[saleType] = true;
							saleIDSDict[key] = nPCSaleItem;
						}
						IL_274:;
					}
				}
				this._SaleIDSDict = saleIDSDict;
				result = true;
			}
			return result;
		}

		
		public bool ReloadSaleList()
		{
			try
			{
				return this.LoadSaleList();
			}
			catch (Exception)
			{
			}
			return false;
		}

		
		private Dictionary<int, NPCSaleItem> _SaleIDSDict = null;
	}
}
