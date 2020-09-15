using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200076F RID: 1903
	public class NPCSaleList
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x060030EE RID: 12526 RVA: 0x002B633C File Offset: 0x002B453C
		public Dictionary<int, NPCSaleItem> SaleIDSDict
		{
			get
			{
				return this._SaleIDSDict;
			}
		}

		// Token: 0x060030EF RID: 12527 RVA: 0x002B6354 File Offset: 0x002B4554
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

		// Token: 0x060030F0 RID: 12528 RVA: 0x002B6658 File Offset: 0x002B4858
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

		// Token: 0x04003D7B RID: 15739
		private Dictionary<int, NPCSaleItem> _SaleIDSDict = null;
	}
}
