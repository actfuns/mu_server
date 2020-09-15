using System;
using System.Collections.Generic;
using GameServer.Interface;

namespace GameServer.Logic.ExtensionProps
{
	// Token: 0x020006BD RID: 1725
	public class ExtensionPropsMgr
	{
		// Token: 0x0600206E RID: 8302 RVA: 0x001BEEE8 File Offset: 0x001BD0E8
		public static ExtensionPropItem FindCachingItem(int id)
		{
			ExtensionPropItem extensionPropItem = null;
			ExtensionPropItem result;
			if (!ExtensionPropsMgr._ExtensionPropsCachingDict.TryGetValue(id, out extensionPropItem))
			{
				result = null;
			}
			else
			{
				result = extensionPropItem;
			}
			return result;
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x001BEF14 File Offset: 0x001BD114
		private static Dictionary<int, byte> ParseDict(string str)
		{
			Dictionary<int, byte> dict = new Dictionary<int, byte>();
			string[] fields = str.Split(new char[]
			{
				','
			});
			for (int i = 0; i < fields.Length; i++)
			{
				dict[Global.SafeConvertToInt32(fields[i])] = 1;
			}
			return dict;
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x001BEF6C File Offset: 0x001BD16C
		private static ExtensionPropItem ParseCachingItem(SystemXmlItem systemXmlItem)
		{
			return new ExtensionPropItem
			{
				ID = systemXmlItem.GetIntValue("ID", -1),
				PrevTuoZhanShuXing = ExtensionPropsMgr.ParseDict(systemXmlItem.GetStringValue("PrevTuoZhanShuXing")),
				TargetType = systemXmlItem.GetIntValue("TargetTyp", -1),
				ActionType = systemXmlItem.GetIntValue("ActionType", -1),
				Probability = (int)(systemXmlItem.GetDoubleValue("Probability") * 100.0),
				NeedSkill = ExtensionPropsMgr.ParseDict(systemXmlItem.GetStringValue("NeedSkill")),
				Icon = systemXmlItem.GetIntValue("Icon", -1),
				TargetDecoration = systemXmlItem.GetIntValue("TargetDecoration", -1),
				DelayDecoration = systemXmlItem.GetIntValue("DelayDecoration", -1)
			};
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x001BF03C File Offset: 0x001BD23C
		public static void LoadCachingItems(SystemXmlItems systemExtensionProps)
		{
			Dictionary<int, ExtensionPropItem> cachingDict = new Dictionary<int, ExtensionPropItem>();
			foreach (int key in systemExtensionProps.SystemXmlItemDict.Keys)
			{
				SystemXmlItem systemXmlItem = systemExtensionProps.SystemXmlItemDict[key];
				ExtensionPropItem extensionPropItem = ExtensionPropsMgr.ParseCachingItem(systemXmlItem);
				if (null != extensionPropItem)
				{
					cachingDict[extensionPropItem.ID] = extensionPropItem;
				}
			}
			ExtensionPropsMgr._ExtensionPropsCachingDict = cachingDict;
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x001BF0D8 File Offset: 0x001BD2D8
		public static List<int> ProcessExtensionProps(List<int> extensionPropsIDList, int skillID, int actionType)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (null == extensionPropsIDList)
			{
				result = list;
			}
			else
			{
				Dictionary<int, byte> dict = new Dictionary<int, byte>();
				for (int i = 0; i < extensionPropsIDList.Count; i++)
				{
					int id = extensionPropsIDList[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(id);
					if (null != extensionPropItem)
					{
						if (extensionPropItem.ActionType == actionType)
						{
							if (extensionPropItem.NeedSkill.Count > 0)
							{
								if (!extensionPropItem.NeedSkill.ContainsKey(skillID))
								{
									goto IL_C2;
								}
							}
							int rndNum = Global.GetRandomNumber(0, 101);
							if (rndNum <= extensionPropItem.Probability)
							{
								list.Add(id);
								dict[id] = 1;
							}
						}
					}
					IL_C2:;
				}
				List<int> returnList = new List<int>();
				for (int i = 0; i < list.Count; i++)
				{
					int id = list[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(id);
					if (null != extensionPropItem)
					{
						if (extensionPropItem.PrevTuoZhanShuXing.Count > 0)
						{
							foreach (int key in extensionPropItem.PrevTuoZhanShuXing.Keys)
							{
								if (!dict.ContainsKey(key))
								{
								}
							}
						}
						returnList.Add(id);
					}
				}
				result = returnList;
			}
			return result;
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x001BF290 File Offset: 0x001BD490
		public static void ExecuteExtensionPropsActions(List<int> list, IObject self, IObject obj)
		{
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					int id = list[i];
					ExtensionPropItem extensionPropItem = ExtensionPropsMgr.FindCachingItem(id);
					if (null != extensionPropItem)
					{
						IObject targetObj;
						if (0 == extensionPropItem.ActionType)
						{
							targetObj = self;
							if (0 != extensionPropItem.TargetType)
							{
								targetObj = obj;
							}
						}
						else
						{
							targetObj = obj;
							if (0 != extensionPropItem.TargetType)
							{
								targetObj = self;
							}
						}
						List<MagicActionItem> magicActionItemList = null;
						if (GameManager.SystemMagicActionMgr.BossAIActionsDict.TryGetValue(extensionPropItem.ID, out magicActionItemList) && null != magicActionItemList)
						{
							for (int j = 0; j < magicActionItemList.Count; j++)
							{
								MagicAction.ProcessAction(self, targetObj, magicActionItemList[j].MagicActionID, magicActionItemList[j].MagicActionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
							}
						}
						GameManager.ClientMgr.NotifySpriteExtensionPropsHited(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, self, targetObj.GetObjectID(), (int)targetObj.CurrentPos.X, (int)targetObj.CurrentPos.Y, id);
					}
				}
			}
		}

		// Token: 0x04003676 RID: 13942
		private static Dictionary<int, ExtensionPropItem> _ExtensionPropsCachingDict = new Dictionary<int, ExtensionPropItem>();
	}
}
