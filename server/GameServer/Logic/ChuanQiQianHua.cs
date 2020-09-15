using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x020005E4 RID: 1508
	public class ChuanQiQianHua
	{
		// Token: 0x06001C6B RID: 7275 RVA: 0x001A9DC8 File Offset: 0x001A7FC8
		public static List<ChuanQiQianHuaItem> GetListChuanQiQianHuaItem(int qianHuaID)
		{
			List<ChuanQiQianHuaItem> list = null;
			Dictionary<int, List<ChuanQiQianHuaItem>> dict = ChuanQiQianHua.QianHuaItemDict;
			lock (dict)
			{
				dict.TryGetValue(qianHuaID, out list);
			}
			return list;
		}

		// Token: 0x06001C6C RID: 7276 RVA: 0x001A9E24 File Offset: 0x001A8024
		public static void LoadEquipQianHuaProps()
		{
			XElement xml = null;
			string fileName = "";
			try
			{
				fileName = string.Format("Config/QiangHua.xml", new object[0]);
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
			Dictionary<int, List<ChuanQiQianHuaItem>> dict = new Dictionary<int, List<ChuanQiQianHuaItem>>();
			IEnumerable<XElement> xmlItems = xml.Elements("QiangHua");
			foreach (XElement xmltem in xmlItems)
			{
				SystemXmlItem systemXmlItem = new SystemXmlItem
				{
					XMLNode = xmltem
				};
				int id = systemXmlItem.GetIntValue("ID", -1);
				dict[id] = ChuanQiQianHua.ParseSystemXmlItem(systemXmlItem);
			}
			ChuanQiQianHua.QianHuaItemDict = dict;
		}

		// Token: 0x06001C6D RID: 7277 RVA: 0x001A9F38 File Offset: 0x001A8138
		private static List<ChuanQiQianHuaItem> ParseSystemXmlItem(SystemXmlItem systemXmlItem)
		{
			List<ChuanQiQianHuaItem> list = new List<ChuanQiQianHuaItem>();
			string qianHua = systemXmlItem.GetStringValue("QiangHua");
			List<ChuanQiQianHuaItem> result;
			if (string.IsNullOrEmpty(qianHua))
			{
				result = list;
			}
			else
			{
				string[] qianHuaFields = qianHua.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < qianHuaFields.Length; i++)
				{
					list.AddRange(ChuanQiQianHua.ParseChuanQiQianHuaItem(systemXmlItem.GetIntValue("ID", -1), qianHuaFields[i]));
				}
				result = list;
			}
			return result;
		}

		// Token: 0x06001C6E RID: 7278 RVA: 0x001A9FBC File Offset: 0x001A81BC
		public static int GetExtPropIndexeFromStr(string str)
		{
			int index = -1;
			lock (ChuanQiQianHua.StrToExtPropIndexDict)
			{
				if (ChuanQiQianHua.StrToExtPropIndexDict.TryGetValue(str, out index))
				{
					return index;
				}
			}
			for (int i = 0; i < 177; i++)
			{
				string name = string.Format("{0}", (ExtPropIndexes)i);
				if (name == str)
				{
					index = i;
					break;
				}
			}
			lock (ChuanQiQianHua.StrToExtPropIndexDict)
			{
				ChuanQiQianHua.StrToExtPropIndexDict[str] = index;
			}
			return index;
		}

		// Token: 0x06001C6F RID: 7279 RVA: 0x001AA0B0 File Offset: 0x001A82B0
		private static List<ChuanQiQianHuaItem> ParseChuanQiQianHuaItem(int qianHuaID, string strValue)
		{
			return new List<ChuanQiQianHuaItem>();
		}

		// Token: 0x06001C70 RID: 7280 RVA: 0x001AA0CC File Offset: 0x001A82CC
		public static void ApplayEquipQianHuaProps(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<MagicActionItem> magicActionItemList = null;
			if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out magicActionItemList) && null != magicActionItemList)
			{
				if (magicActionItemList.Count > 0)
				{
					if (magicActionItemList[0].MagicActionID == MagicActionIDs.DB_ADD_YINYONG)
					{
						if (magicActionItemList[0].MagicActionParams.Length == 2)
						{
							int qianHuaID = (int)magicActionItemList[0].MagicActionParams[0];
							List<ChuanQiQianHuaItem> list = ChuanQiQianHua.GetListChuanQiQianHuaItem(qianHuaID);
							if (list != null && list.Count > 0)
							{
								for (int i = 0; i < list.Count; i++)
								{
									if (list[i].QianHuaLevel <= goodsData.Forge_level)
									{
										if (toAdd)
										{
											equipProps[list[i].PropIndex] += list[i].ItemValue;
										}
										else
										{
											equipProps[list[i].PropIndex] -= list[i].ItemValue;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x04002A62 RID: 10850
		public static Dictionary<int, List<ChuanQiQianHuaItem>> QianHuaItemDict = null;

		// Token: 0x04002A63 RID: 10851
		public static Dictionary<string, int> StrToExtPropIndexDict = new Dictionary<string, int>();
	}
}
