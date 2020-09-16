using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class SystemMagicAction
	{
		
		static SystemMagicAction()
		{
			for (MagicActionIDs id = MagicActionIDs.FOREVER_ADDHIT; id < MagicActionIDs.MAX; id++)
			{
				string name = id.ToString().ToLower();
				SystemMagicAction.MagicActionIDsDict.Add(name, id);
			}
		}

		
		private static void PrintMaigcActionDictUsage(string name, Dictionary<string, MagicActionIDs> dict)
		{
			Console.WriteLine(string.Format("{0}个数{1}", name, dict.Count));
			foreach (KeyValuePair<string, MagicActionIDs> kv in dict)
			{
				Console.WriteLine(string.Format("{0} {1}", kv.Key, kv.Value));
			}
			Console.WriteLine("\r\n");
		}

		
		public static void PrintMaigcActionUsage()
		{
			SystemMagicAction.PrintMaigcActionDictUsage("MagicActionIDsDict", SystemMagicAction.MagicActionIDsDict);
		}

		
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

		
		public List<MagicActionItem> ParseActionsInterface(string actions)
		{
			return this.ParseActions(actions);
		}

		
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

		
		private void ParseMagicActions(Dictionary<int, List<MagicActionItem>> dict, int id, string actions)
		{
			actions = actions.Trim();
			if (!("" == actions))
			{
				List<MagicActionItem> magicActionList = this.ParseActions(actions);
				dict[id] = magicActionList;
			}
		}

		
		public List<MagicActionItem> ParseActionsOutUse(string strAction)
		{
			return this.ParseActions(strAction);
		}

		
		
		public Dictionary<int, List<MagicActionItem>> MagicActionsDict
		{
			get
			{
				return this._MagicActionsDict;
			}
		}

		
		
		public Dictionary<int, int> MagicActionRelationDic
		{
			get
			{
				return this._MagicActionRelationDic;
			}
		}

		
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

		
		public void ParseMagicActions2(SystemXmlItems systemMagicsMgr)
		{
		}

		
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

		
		
		public Dictionary<int, List<MagicActionItem>> GoodsActionsDict
		{
			get
			{
				return this._GoodsActionsDict;
			}
		}

		
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

		
		
		public Dictionary<int, List<MagicActionItem>> NPCScriptActionsDict
		{
			get
			{
				return this._NPCScriptActionsDict;
			}
		}

		
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

		
		
		public Dictionary<int, List<MagicActionItem>> BossAIActionsDict
		{
			get
			{
				return this._BossAIActionsDict;
			}
		}

		
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

		
		
		public Dictionary<int, List<MagicActionItem>> ExtensionPropsActionsDict
		{
			get
			{
				return this._ExtensionPropsActionsDict;
			}
		}

		
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

		
		private static Dictionary<string, MagicActionIDs> MagicActionIDsDict = new Dictionary<string, MagicActionIDs>();

		
		private Dictionary<int, List<MagicActionItem>> _MagicActionsDict = null;

		
		private Dictionary<int, int> _MagicActionRelationDic = null;

		
		private Dictionary<int, List<MagicActionItem>> _GoodsActionsDict = null;

		
		private Dictionary<int, List<MagicActionItem>> _NPCScriptActionsDict = null;

		
		private Dictionary<int, List<MagicActionItem>> _BossAIActionsDict = null;

		
		private Dictionary<int, List<MagicActionItem>> _ExtensionPropsActionsDict = null;
	}
}
