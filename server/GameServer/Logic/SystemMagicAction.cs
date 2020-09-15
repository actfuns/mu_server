using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	// Token: 0x020007DC RID: 2012
	public class SystemMagicAction
	{
		// Token: 0x060038D2 RID: 14546 RVA: 0x00306788 File Offset: 0x00304988
		static SystemMagicAction()
		{
			for (MagicActionIDs id = MagicActionIDs.FOREVER_ADDHIT; id < MagicActionIDs.MAX; id++)
			{
				string name = id.ToString().ToLower();
				SystemMagicAction.MagicActionIDsDict.Add(name, id);
			}
		}

		// Token: 0x060038D3 RID: 14547 RVA: 0x003067D8 File Offset: 0x003049D8
		private static void PrintMaigcActionDictUsage(string name, Dictionary<string, MagicActionIDs> dict)
		{
			Console.WriteLine(string.Format("{0}个数{1}", name, dict.Count));
			foreach (KeyValuePair<string, MagicActionIDs> kv in dict)
			{
				Console.WriteLine(string.Format("{0} {1}", kv.Key, kv.Value));
			}
			Console.WriteLine("\r\n");
		}

		// Token: 0x060038D4 RID: 14548 RVA: 0x00306874 File Offset: 0x00304A74
		public static void PrintMaigcActionUsage()
		{
			SystemMagicAction.PrintMaigcActionDictUsage("MagicActionIDsDict", SystemMagicAction.MagicActionIDsDict);
		}

		// Token: 0x060038D5 RID: 14549 RVA: 0x00306888 File Offset: 0x00304A88
		private int FindIDByName(string name)
		{
			MagicActionIDs id;
			int result;
			if (SystemMagicAction.MagicActionIDsDict.TryGetValue(name.ToLower(), out id))
			{
				result = (int)id;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		// Token: 0x060038D6 RID: 14550 RVA: 0x003068BC File Offset: 0x00304ABC
		private MagicActionItem ParseParams(string item)
		{
			int start = item.IndexOf('(');
			string name;
			string paramsList;
			if (-1 != start)
			{
				int end = item.IndexOf(')', start + 1);
				if (-1 == end)
				{
					return null;
				}
				name = item.Substring(0, start);
				paramsList = item.Substring(start + 1, end - start - 1);
			}
			else if ((start = item.IndexOf(',')) != -1)
			{
				name = item.Substring(0, start);
				paramsList = item.Substring(start + 1, item.Length - start - 1);
			}
			else
			{
				name = item;
				paramsList = "";
			}
			int id = this.FindIDByName(name);
			MagicActionItem result;
			if (id < 0)
			{
				result = null;
			}
			else
			{
				double[] actionParams = null;
				if (paramsList != "")
				{
					string[] paramsArray = paramsList.Split(new char[]
					{
						','
					});
					actionParams = new double[paramsArray.Length];
					for (int i = 0; i < paramsArray.Length; i++)
					{
						if (char.IsDigit(paramsArray[i], 0) || paramsArray[i][0] == '-')
						{
							actionParams[i] = Global.SafeConvertToDouble(paramsArray[i]);
						}
						else
						{
							actionParams[i] = (double)this.FindIDByName(paramsArray[i]);
						}
					}
				}
				result = new MagicActionItem
				{
					MagicActionID = (MagicActionIDs)id,
					MagicActionParams = actionParams
				};
			}
			return result;
		}

		// Token: 0x060038D7 RID: 14551 RVA: 0x00306A50 File Offset: 0x00304C50
		public List<MagicActionItem> ParseActionsInterface(string actions)
		{
			return this.ParseActions(actions);
		}

		// Token: 0x060038D8 RID: 14552 RVA: 0x00306A6C File Offset: 0x00304C6C
		private List<MagicActionItem> ParseActions(string actions)
		{
			List<MagicActionItem> itemsList = new List<MagicActionItem>();
			string[] actionFields = actions.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < actionFields.Length; i++)
			{
				string item = actionFields[i].Trim();
				MagicActionItem magicActionItem = this.ParseParams(item);
				if (null != magicActionItem)
				{
					itemsList.Add(magicActionItem);
				}
			}
			return itemsList;
		}

		// Token: 0x060038D9 RID: 14553 RVA: 0x00306ADC File Offset: 0x00304CDC
		private void ParseMagicActions(Dictionary<int, List<MagicActionItem>> dict, int id, string actions)
		{
			actions = actions.Trim();
			if (!("" == actions))
			{
				List<MagicActionItem> magicActionList = this.ParseActions(actions);
				dict[id] = magicActionList;
			}
		}

		// Token: 0x060038DA RID: 14554 RVA: 0x00306B18 File Offset: 0x00304D18
		public List<MagicActionItem> ParseActionsOutUse(string strAction)
		{
			return this.ParseActions(strAction);
		}

		// Token: 0x1700059E RID: 1438
		// (get) Token: 0x060038DB RID: 14555 RVA: 0x00306B34 File Offset: 0x00304D34
		public Dictionary<int, List<MagicActionItem>> MagicActionsDict
		{
			get
			{
				return this._MagicActionsDict;
			}
		}

		// Token: 0x1700059F RID: 1439
		// (get) Token: 0x060038DC RID: 14556 RVA: 0x00306B4C File Offset: 0x00304D4C
		public Dictionary<int, int> MagicActionRelationDic
		{
			get
			{
				return this._MagicActionRelationDic;
			}
		}

		// Token: 0x060038DD RID: 14557 RVA: 0x00306B64 File Offset: 0x00304D64
		public void ParseMagicActions(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, List<MagicActionItem>> magicActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				string actions = systemMagicsMgr.SystemXmlItemDict[key].GetStringValue("MagicScripts");
				if (null != actions)
				{
					this.ParseMagicActions(magicActionsDict, key, actions);
				}
			}
			this._MagicActionsDict = magicActionsDict;
		}

		// Token: 0x060038DE RID: 14558 RVA: 0x00306BFC File Offset: 0x00304DFC
		public void ParseMagicActionRelations(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, int> magicActionRelationDic = new Dictionary<int, int>();
			foreach (int key in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				int nextMagicID = systemMagicsMgr.SystemXmlItemDict[key].GetIntValue("NextMagicID", -1);
				if (-1 != nextMagicID)
				{
					magicActionRelationDic[nextMagicID] = key;
				}
			}
			this._MagicActionRelationDic = magicActionRelationDic;
		}

		// Token: 0x060038DF RID: 14559 RVA: 0x00306C94 File Offset: 0x00304E94
		public void ParseMagicActions2(SystemXmlItems systemMagicsMgr)
		{
		}

		// Token: 0x060038E0 RID: 14560 RVA: 0x00306C98 File Offset: 0x00304E98
		public void ParseScanTypeActions2(SystemXmlItems systemMagicsMgr)
		{
			Dictionary<int, List<MagicActionItem>> magicActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemMagicsMgr.SystemXmlItemDict.Keys)
			{
				string actions = systemMagicsMgr.SystemXmlItemDict[key].GetStringValue("ScanType");
				if (null != actions)
				{
					this.ParseMagicActions(magicActionsDict, key, actions);
				}
			}
			this._MagicActionsDict = magicActionsDict;
		}

		// Token: 0x170005A0 RID: 1440
		// (get) Token: 0x060038E1 RID: 14561 RVA: 0x00306D30 File Offset: 0x00304F30
		public Dictionary<int, List<MagicActionItem>> GoodsActionsDict
		{
			get
			{
				return this._GoodsActionsDict;
			}
		}

		// Token: 0x060038E2 RID: 14562 RVA: 0x00306D48 File Offset: 0x00304F48
		public void ParseGoodsActions(SystemXmlItems systemGoodsMgr)
		{
			Dictionary<int, List<MagicActionItem>> goodsActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemGoodsMgr.SystemXmlItemDict.Keys)
			{
				string actions = systemGoodsMgr.SystemXmlItemDict[key].GetStringValue("ExecMagic");
				if (!string.IsNullOrEmpty(actions))
				{
					this.ParseMagicActions(goodsActionsDict, key, actions);
				}
			}
			this._GoodsActionsDict = goodsActionsDict;
		}

		// Token: 0x170005A1 RID: 1441
		// (get) Token: 0x060038E3 RID: 14563 RVA: 0x00306DE4 File Offset: 0x00304FE4
		public Dictionary<int, List<MagicActionItem>> NPCScriptActionsDict
		{
			get
			{
				return this._NPCScriptActionsDict;
			}
		}

		// Token: 0x060038E4 RID: 14564 RVA: 0x00306DFC File Offset: 0x00304FFC
		public void ParseNPCScriptActions(SystemXmlItems systemNPCScripts)
		{
			Dictionary<int, List<MagicActionItem>> npcScriptActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemNPCScripts.SystemXmlItemDict.Keys)
			{
				string actions = systemNPCScripts.SystemXmlItemDict[key].GetStringValue("ExecMagic");
				if (null != actions)
				{
					this.ParseMagicActions(npcScriptActionsDict, key, actions);
				}
			}
			this._NPCScriptActionsDict = npcScriptActionsDict;
		}

		// Token: 0x170005A2 RID: 1442
		// (get) Token: 0x060038E5 RID: 14565 RVA: 0x00306E94 File Offset: 0x00305094
		public Dictionary<int, List<MagicActionItem>> BossAIActionsDict
		{
			get
			{
				return this._BossAIActionsDict;
			}
		}

		// Token: 0x060038E6 RID: 14566 RVA: 0x00306EAC File Offset: 0x003050AC
		public void ParseBossAIActions(SystemXmlItems systemBossAI)
		{
			Dictionary<int, List<MagicActionItem>> bossAIActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemBossAI.SystemXmlItemDict.Keys)
			{
				string actions = systemBossAI.SystemXmlItemDict[key].GetStringValue("Action");
				if (!string.IsNullOrEmpty(actions))
				{
					this.ParseMagicActions(bossAIActionsDict, key, actions);
				}
			}
			this._BossAIActionsDict = bossAIActionsDict;
		}

		// Token: 0x170005A3 RID: 1443
		// (get) Token: 0x060038E7 RID: 14567 RVA: 0x00306F48 File Offset: 0x00305148
		public Dictionary<int, List<MagicActionItem>> ExtensionPropsActionsDict
		{
			get
			{
				return this._ExtensionPropsActionsDict;
			}
		}

		// Token: 0x060038E8 RID: 14568 RVA: 0x00306F60 File Offset: 0x00305160
		public void ParseExtensionPropsActions(SystemXmlItems systemExtensionProps)
		{
			Dictionary<int, List<MagicActionItem>> extensionPropsActionsDict = new Dictionary<int, List<MagicActionItem>>();
			foreach (int key in systemExtensionProps.SystemXmlItemDict.Keys)
			{
				string actions = systemExtensionProps.SystemXmlItemDict[key].GetStringValue("MagicScripts");
				if (!string.IsNullOrEmpty(actions))
				{
					this.ParseMagicActions(extensionPropsActionsDict, key, actions);
				}
			}
			this._ExtensionPropsActionsDict = extensionPropsActionsDict;
		}

		// Token: 0x040042FB RID: 17147
		private static Dictionary<string, MagicActionIDs> MagicActionIDsDict = new Dictionary<string, MagicActionIDs>();

		// Token: 0x040042FC RID: 17148
		private Dictionary<int, List<MagicActionItem>> _MagicActionsDict = null;

		// Token: 0x040042FD RID: 17149
		private Dictionary<int, int> _MagicActionRelationDic = null;

		// Token: 0x040042FE RID: 17150
		private Dictionary<int, List<MagicActionItem>> _GoodsActionsDict = null;

		// Token: 0x040042FF RID: 17151
		private Dictionary<int, List<MagicActionItem>> _NPCScriptActionsDict = null;

		// Token: 0x04004300 RID: 17152
		private Dictionary<int, List<MagicActionItem>> _BossAIActionsDict = null;

		// Token: 0x04004301 RID: 17153
		private Dictionary<int, List<MagicActionItem>> _ExtensionPropsActionsDict = null;
	}
}
