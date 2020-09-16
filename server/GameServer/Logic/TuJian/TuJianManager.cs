using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.TuJian
{
	
	public class TuJianManager : SingletonTemplate<TuJianManager>
	{
		
		private TuJianManager()
		{
		}

		
		public void LoadConfig()
		{
			bool bFailed = false;
			if (!this.loadTuJianType() || !this.loadTuJianItem())
			{
				bFailed = true;
			}
			bool _check = true;
			if (_check && !bFailed)
			{
				Dictionary<int, int> itemCntByType = new Dictionary<int, int>();
				foreach (KeyValuePair<int, TuJianItem> kvp in this.TuJianItems)
				{
					int itemID = kvp.Key;
					int typeID = kvp.Value.TypeID;
					if (!itemCntByType.ContainsKey(typeID))
					{
						itemCntByType.Add(typeID, 0);
					}
					Dictionary<int, int> dictionary;
					int key;
					(dictionary = itemCntByType)[key = typeID] = dictionary[key] + 1;
				}
				foreach (KeyValuePair<int, int> kvp2 in itemCntByType)
				{
					int typeID = kvp2.Key;
					int itemCnt = kvp2.Value;
					TuJianType type = null;
					if (!this.TuJianTypes.TryGetValue(typeID, out type) || type.ItemCnt != itemCnt)
					{
						bFailed = true;
						break;
					}
				}
			}
			if (bFailed)
			{
				LogManager.WriteLog(LogTypes.Error, "Config/TuJianType.xml Config/TuJianItems.xml 配置文件出错，请检查文件是否存在或者配置的item个数是否一致", null, true);
			}
		}

		
		private bool loadTuJianType()
		{
			try
			{
				XElement xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath("Config/TuJianType.xml"));
				if (xmlFile == null)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0}不存在!", "Config/TuJianType.xml"), null, true);
					return false;
				}
				this.TuJianTypes.Clear();
				IEnumerable<XElement> TuJianXEle = xmlFile.Elements("TuJian").Elements<XElement>();
				foreach (XElement xmlItem in TuJianXEle)
				{
					if (null != xmlItem)
					{
						TuJianType tjType = new TuJianType();
						tjType.TypeID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
						tjType.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						tjType.ItemCnt = (int)Global.GetSafeAttributeDouble(xmlItem, "TuJianNum");
						string sLevelInfo = Global.GetSafeAttributeStr(xmlItem, "KaiQiLevel");
						string[] sArrayLevelInfo = sLevelInfo.Split(new char[]
						{
							','
						});
						tjType.OpenChangeLife = Global.SafeConvertToInt32(sArrayLevelInfo[0]);
						tjType.OpenLevel = Global.SafeConvertToInt32(sArrayLevelInfo[1]);
						string strAttrs = Global.GetSafeAttributeStr(xmlItem, "ShuXiangJiaCheng");
						tjType.AttrValue = this.analyseToAttrValues(strAttrs);
						this.TuJianTypes.Add(tjType.TypeID, tjType);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}读取出错!", "Config/TuJianType.xml"), ex, true);
				return false;
			}
			return true;
		}

		
		private bool loadTuJianItem()
		{
			try
			{
				XElement xmlFile = GeneralCachingXmlMgr.GetXElement(Global.IsolateResPath("Config/TuJianItems.xml"));
				if (null == xmlFile)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("{0}不存在!", "Config/TuJianItems.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> TuJianXEle = xmlFile.Elements();
				foreach (XElement xmlItem in TuJianXEle)
				{
					if (null != xmlItem)
					{
						TuJianItem item = new TuJianItem();
						item.TypeID = (int)Global.GetSafeAttributeDouble(xmlItem, "Type");
						item.ItemID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
						item.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						string strCostGoods = Global.GetSafeAttributeStr(xmlItem, "NeedGoods");
						if (!string.IsNullOrEmpty(strCostGoods))
						{
							string[] sArry = strCostGoods.Split(new char[]
							{
								','
							});
							item.CostGoodsID = Global.SafeConvertToInt32(sArry[0]);
							item.CostGoodsCnt = Global.SafeConvertToInt32(sArry[1]);
						}
						string strAttrs = Global.GetSafeAttributeStr(xmlItem, "ShuXing");
						item.AttrValue = this.analyseToAttrValues(strAttrs);
						this.TuJianItems.Add(item.ItemID, item);
						if (!this.TuJianTypes.ContainsKey(item.TypeID))
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("{0}配置了不存在的图鉴类型Type={1}", "Config/TuJianItems.xml", item.TypeID), null, true);
							return false;
						}
						this.TuJianTypes[item.TypeID].ItemList.Add(item.ItemID);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("{0}读取出错!", "Config/TuJianItems.xml"), ex, true);
				return false;
			}
			return true;
		}

		
		private _AttrValue analyseToAttrValues(string strAttrs)
		{
			_AttrValue result2;
			if (string.IsNullOrEmpty(strAttrs))
			{
				result2 = null;
			}
			else
			{
				string[] sArry = strAttrs.Split(new char[]
				{
					'|'
				});
				if (sArry == null || sArry.Length == 0)
				{
					result2 = null;
				}
				else
				{
					_AttrValue result = new _AttrValue();
					foreach (string str in sArry)
					{
						string[] attr = str.Split(new char[]
						{
							','
						});
						if (attr != null && attr.Length == 2)
						{
							string attrName = attr[0].ToLower();
							string attrValue = attr[1];
							string[] attrTwoValue = attrValue.Split(new char[]
							{
								'-'
							});
							if (attrName == "defense")
							{
								if (attrTwoValue != null && attrTwoValue.Length == 2)
								{
									result.MinDefense = Global.SafeConvertToInt32(attrTwoValue[0]);
									result.MaxDefense = Global.SafeConvertToInt32(attrTwoValue[1]);
								}
							}
							else if (attrName == "mdefense")
							{
								if (attrTwoValue != null && attrTwoValue.Length == 2)
								{
									result.MinMDefense = Global.SafeConvertToInt32(attrTwoValue[0]);
									result.MaxMDefense = Global.SafeConvertToInt32(attrTwoValue[1]);
								}
							}
							else if (attrName == "attack")
							{
								if (attrTwoValue != null && attrTwoValue.Length == 2)
								{
									result.MinAttack = Global.SafeConvertToInt32(attrTwoValue[0]);
									result.MaxAttack = Global.SafeConvertToInt32(attrTwoValue[1]);
								}
							}
							else if (attrName == "mattack")
							{
								if (attrTwoValue != null && attrTwoValue.Length == 2)
								{
									result.MinMAttack = Global.SafeConvertToInt32(attrTwoValue[0]);
									result.MaxMAttack = Global.SafeConvertToInt32(attrTwoValue[1]);
								}
							}
							else if (attrName == "hitv")
							{
								result.HitV = Global.SafeConvertToInt32(attrTwoValue[0]);
							}
							else if (attrName == "dodge")
							{
								result.Dodge = Global.SafeConvertToInt32(attrTwoValue[0]);
							}
							else if (attrName == "maxlifev")
							{
								result.MaxLifeV = Global.SafeConvertToInt32(attrTwoValue[0]);
							}
						}
					}
					result2 = result;
				}
			}
			return result2;
		}

		
		public void UpdateTuJianProps(GameClient client)
		{
			if (client != null)
			{
				if (client.ClientData.PictureJudgeReferInfo != null && client.ClientData.PictureJudgeReferInfo.Count != 0)
				{
					Dictionary<int, int> activeItemByType = new Dictionary<int, int>();
					_AttrValue totalAttrValue = new _AttrValue();
					foreach (KeyValuePair<int, int> kvp in client.ClientData.PictureJudgeReferInfo)
					{
						int itemID = kvp.Key;
						int itemReferCnt = kvp.Value;
						TuJianItem item = null;
						if (this.TuJianItems.TryGetValue(itemID, out item))
						{
							if (itemReferCnt >= item.CostGoodsCnt)
							{
								if (!activeItemByType.ContainsKey(item.TypeID))
								{
									activeItemByType.Add(item.TypeID, 0);
								}
								Dictionary<int, int> dictionary;
								int typeID;
								(dictionary = activeItemByType)[typeID = item.TypeID] = dictionary[typeID] + 1;
								totalAttrValue.Add(item.AttrValue);
								if (client.ClientData.ActivedTuJianItem != null && !client.ClientData.ActivedTuJianItem.Contains(itemID))
								{
									client.ClientData.ActivedTuJianItem.Add(itemID);
								}
							}
						}
					}
					foreach (KeyValuePair<int, int> kvp in activeItemByType)
					{
						TuJianType type = null;
						if (this.TuJianTypes.TryGetValue(kvp.Key, out type))
						{
							if (kvp.Value >= type.ItemCnt)
							{
								totalAttrValue.Add(type.AttrValue);
								if (client.ClientData.ActivedTuJianType != null && !client.ClientData.ActivedTuJianType.Contains(kvp.Key))
								{
									client.ClientData.ActivedTuJianType.Add(kvp.Key);
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						7,
						totalAttrValue.MinAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						8,
						totalAttrValue.MaxAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						9,
						totalAttrValue.MinMAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						10,
						totalAttrValue.MaxMAttack
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						3,
						totalAttrValue.MinDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						4,
						totalAttrValue.MaxDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						5,
						totalAttrValue.MinMDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						6,
						totalAttrValue.MaxMDefense
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						18,
						totalAttrValue.HitV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						13,
						totalAttrValue.MaxLifeV
					});
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						12,
						19,
						totalAttrValue.Dodge
					});
				}
			}
		}

		
		public void HandleActiveTuJian(GameClient client, string[] itemArr)
		{
			if (itemArr != null && itemArr.Length != 0 && client != null)
			{
				bool anySuccess = false;
				foreach (string strItemID in itemArr)
				{
					int itemID = Convert.ToInt32(strItemID);
					TuJianItem item = null;
					TuJianType type = null;
					if (this.TuJianItems.TryGetValue(itemID, out item) && this.TuJianTypes.TryGetValue(item.TypeID, out type))
					{
						if (client.ClientData.ChangeLifeCount >= type.OpenChangeLife && (client.ClientData.ChangeLifeCount != type.OpenChangeLife || client.ClientData.Level >= type.OpenLevel))
						{
							int hadReferCnt = 0;
							if (client.ClientData.PictureJudgeReferInfo.ContainsKey(itemID))
							{
								hadReferCnt = client.ClientData.PictureJudgeReferInfo[itemID];
							}
							if (hadReferCnt < item.CostGoodsCnt)
							{
								int needReferCnt = item.CostGoodsCnt - hadReferCnt;
								int hasGoodsCnt = Global.GetTotalGoodsCountByID(client, item.CostGoodsID);
								if (hasGoodsCnt > 0)
								{
									int thisTimeReferCnt = Math.Min(needReferCnt, hasGoodsCnt);
									bool usedBinding_just_placeholder = false;
									bool usedTimeLimited_just_placeholder = false;
									if (GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item.CostGoodsID, thisTimeReferCnt, false, out usedBinding_just_placeholder, out usedTimeLimited_just_placeholder, false))
									{
										string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, itemID, hadReferCnt + thisTimeReferCnt);
										string[] dbRsp = Global.ExecuteDBCmd(10155, strDbCmd, client.ServerId);
										if (dbRsp == null || dbRsp.Length != 1 || Convert.ToInt32(dbRsp[0]) <= 0)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("角色RoleID={0}，RoleName={1} 激活图鉴Item={2}时，与db通信失败，物品已扣除GoodsID={3},Cnt={4}", new object[]
											{
												client.ClientData.RoleID,
												client.ClientData.RoleName,
												itemID,
												item.CostGoodsID,
												thisTimeReferCnt
											}), null, true);
										}
										else
										{
											anySuccess = true;
											if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(itemID))
											{
												client.ClientData.PictureJudgeReferInfo.Add(itemID, hadReferCnt + thisTimeReferCnt);
											}
											else
											{
												client.ClientData.PictureJudgeReferInfo[itemID] = hadReferCnt + thisTimeReferCnt;
											}
											ProcessTask.ProcessAddTaskVal(client, TaskTypes.JiHuoTuJian, -1, 1, new object[0]);
										}
									}
								}
							}
						}
					}
				}
				if (anySuccess)
				{
					this.UpdateTuJianProps(client);
					SingletonTemplate<GuardStatueManager>.Instance().OnActiveTuJian(client);
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		
		public bool GM_OneKeyActiveTuJianType(GameClient client, int typeId, out string failedMsg)
		{
			failedMsg = string.Empty;
			bool result;
			if (client == null)
			{
				failedMsg = "unknown";
				result = false;
			}
			else
			{
				TuJianType type = null;
				if (!this.TuJianTypes.TryGetValue(typeId, out type))
				{
					failedMsg = "图鉴类型找不到: " + typeId.ToString();
					result = false;
				}
				else if (client.ClientData.ChangeLifeCount < type.OpenChangeLife || (client.ClientData.ChangeLifeCount == type.OpenChangeLife && client.ClientData.Level < type.OpenLevel))
				{
					failedMsg = string.Concat(new object[]
					{
						"该项图鉴未开启，类型=",
						typeId.ToString(),
						" ,需求转生：",
						type.OpenChangeLife,
						" , 等级：",
						type.OpenLevel
					});
					result = false;
				}
				else
				{
					bool bRealRefer = false;
					foreach (int itemId in type.ItemList)
					{
						TuJianItem item = null;
						if (this.TuJianItems.TryGetValue(itemId, out item))
						{
							if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(itemId) || client.ClientData.PictureJudgeReferInfo[itemId] < item.CostGoodsCnt)
							{
								string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, itemId, item.CostGoodsCnt);
								string[] dbRsp = Global.ExecuteDBCmd(10155, strDbCmd, client.ServerId);
								if (dbRsp == null || dbRsp.Length != 1 || Convert.ToInt32(dbRsp[0]) <= 0)
								{
									failedMsg = "数据库异常";
									return false;
								}
								bRealRefer = true;
								if (!client.ClientData.PictureJudgeReferInfo.ContainsKey(itemId))
								{
									client.ClientData.PictureJudgeReferInfo.Add(itemId, item.CostGoodsCnt);
								}
								else
								{
									client.ClientData.PictureJudgeReferInfo[itemId] = item.CostGoodsCnt;
								}
							}
						}
					}
					if (bRealRefer)
					{
						client.sendCmd(DataHelper.ObjectToTCPOutPacket<Dictionary<int, int>>(client.ClientData.PictureJudgeReferInfo, Global._TCPManager.TcpOutPacketPool, 612), true);
						this.UpdateTuJianProps(client);
						SingletonTemplate<GuardStatueManager>.Instance().OnActiveTuJian(client);
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CompleteTuJian));
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					result = true;
				}
			}
			return result;
		}

		
		private const string TuJianType_fileName = "Config/TuJianType.xml";

		
		private const string TuJianItem_fileName = "Config/TuJianItems.xml";

		
		private Dictionary<int, TuJianType> TuJianTypes = new Dictionary<int, TuJianType>();

		
		private Dictionary<int, TuJianItem> TuJianItems = new Dictionary<int, TuJianItem>();
	}
}
