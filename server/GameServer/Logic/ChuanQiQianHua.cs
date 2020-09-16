using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;

namespace GameServer.Logic
{
	
	public class ChuanQiQianHua
	{
		
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

		
		private static List<ChuanQiQianHuaItem> ParseChuanQiQianHuaItem(int qianHuaID, string strValue)
		{
			return new List<ChuanQiQianHuaItem>();
		}

		
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

		
		public static Dictionary<int, List<ChuanQiQianHuaItem>> QianHuaItemDict = null;

		
		public static Dictionary<string, int> StrToExtPropIndexDict = new Dictionary<string, int>();
	}
}
