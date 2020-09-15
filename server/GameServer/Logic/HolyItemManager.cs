using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200047D RID: 1149
	internal class HolyItemManager : ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x060014DD RID: 5341 RVA: 0x00146360 File Offset: 0x00144560
		public static HolyItemManager getInstance()
		{
			return HolyItemManager.instance;
		}

		// Token: 0x060014DE RID: 5342 RVA: 0x00146378 File Offset: 0x00144578
		public void Initialize()
		{
			SystemXmlItems xml = new SystemXmlItems();
			xml.LoadFromXMlFile("Config/BuJian.xml", "", "ID", 0);
			foreach (KeyValuePair<int, SystemXmlItem> item in xml.SystemXmlItemDict)
			{
				HolyPartInfo data = new HolyPartInfo();
				data.m_nCostBandJinBi = item.Value.GetIntValue("CostBandJinBi", -1);
				data.m_sSuccessProbability = Convert.ToSByte(item.Value.GetDoubleValue("SuccessProbability") * 100.0);
				if (data.m_sSuccessProbability < 0)
				{
					data.m_sSuccessProbability = -1;
				}
				string[] strfiled = item.Value.GetStringValue("NeedGoods").Split(new char[]
				{
					','
				});
				if (strfiled.Length > 1)
				{
					data.m_nNeedGoodsCount = Global.SafeConvertToInt32(strfiled[1]);
				}
				strfiled = item.Value.GetStringValue("FailCost").Split(new char[]
				{
					','
				});
				if (strfiled.Length > 1)
				{
					data.m_nFailCostGoodsCount = Global.SafeConvertToInt32(strfiled[1]);
				}
				string strParam = item.Value.GetStringValue("Property");
				if (strParam != "-1")
				{
					data.m_PropertyList = GameManager.SystemMagicActionMgr.ParseActionsOutUse(strParam);
				}
				data.m_nMaxFailCount = item.Value.GetIntValue("FailMaxNum", -1);
				if (data.m_nMaxFailCount < 0)
				{
					data.m_nMaxFailCount = 0;
				}
				data.NeedGoods = ConfigParser.ParserIntArrayList(item.Value.GetStringValue("NeedItem"), true, '|', ',');
				data.FaildNeedGoods = ConfigParser.ParserIntArrayList(item.Value.GetStringValue("FailureConsumption"), true, '|', ',');
				this._partDataDic.Add(item.Value.GetIntValue("ID", -1), data);
				int suitID = item.Value.GetIntValue("SuitID", -1);
				HolyItemManager.MAX_HOLY_PART_LEVEL = Math.Max(HolyItemManager.MAX_HOLY_PART_LEVEL, Convert.ToSByte(suitID));
			}
			HolyItemManager.MAX_HOLY_PART_LEVEL = (sbyte)Global.GMin((int)HolyItemManager.MAX_HOLY_PART_LEVEL, (int)GameManager.systemParamsList.GetParamValueIntByName("ShengWuMax", 0));
			xml = new SystemXmlItems();
			xml.LoadFromXMlFile("Config/ShengWu.xml", "", "ID", 0);
			foreach (KeyValuePair<int, SystemXmlItem> item in xml.SystemXmlItemDict)
			{
				HolyInfo data2 = new HolyInfo();
				string strParam = item.Value.GetStringValue("ExtraProperty");
				if (strParam != "-1")
				{
					data2.m_ExtraPropertyList = GameManager.SystemMagicActionMgr.ParseActionsOutUse(strParam);
				}
				this._holyDataDic.Add(item.Value.GetIntValue("ID", -1), data2);
			}
			TCPCmdDispatcher.getInstance().registerProcessorEx(10206, 2, 2, HolyItemManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		// Token: 0x060014DF RID: 5343 RVA: 0x001466E8 File Offset: 0x001448E8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060014E0 RID: 5344 RVA: 0x001466FC File Offset: 0x001448FC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			if (nID == 10206)
			{
				if (cmdParams == null || cmdParams.Length != 2)
				{
					return false;
				}
				try
				{
					sbyte sShengWu = Convert.ToSByte(cmdParams[0]);
					sbyte sBuJian = Convert.ToSByte(cmdParams[1]);
					string strret = Convert.ToInt32(this.HolyItem_Suit_Up(client, sShengWu, sBuJian)).ToString();
					client.sendCmd(nID, strret, false);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, "CMD_DB_UPDATE_HOLYITEM", false, false);
				}
			}
			return true;
		}

		// Token: 0x060014E1 RID: 5345 RVA: 0x001467A0 File Offset: 0x001449A0
		private EHolyResult HolyItem_Suit_Up(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot)
		{
			EHolyResult result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
			{
				result = EHolyResult.NotOpen;
			}
			else if (!GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("HolyItem"))
			{
				result = EHolyResult.NotOpen;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.HolyItem, true))
			{
				result = EHolyResult.NotOpen;
			}
			else if (null == client.ClientData.MyHolyItemDataDic)
			{
				result = EHolyResult.Error;
			}
			else
			{
				Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
				HolyItemData tmpdata = null;
				HolyItemPartData tmppartdata = null;
				HolyPartInfo xmlData = null;
				if (!holyitemdata.TryGetValue(sShengWu_slot, out tmpdata))
				{
					result = EHolyResult.Error;
				}
				else if (!tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
				{
					result = EHolyResult.Error;
				}
				else if (tmppartdata.m_sSuit >= HolyItemManager.MAX_HOLY_PART_LEVEL)
				{
					result = EHolyResult.PartSuitIsMax;
				}
				else
				{
					int nDataID = HolyPartInfo.GetBujianID(sShengWu_slot, sBuJian_slot, tmppartdata.m_sSuit);
					if (!this._partDataDic.TryGetValue(nDataID, out xmlData))
					{
						result = EHolyResult.Error;
					}
					else if (-1 != xmlData.m_nCostBandJinBi && xmlData.m_nCostBandJinBi > Global.GetTotalBindTongQianAndTongQianVal(client))
					{
						result = EHolyResult.NeedGold;
					}
					else if (-1 != xmlData.m_nNeedGoodsCount && xmlData.m_nNeedGoodsCount > tmppartdata.m_nSlice)
					{
						result = EHolyResult.NeedHolyItemPart;
					}
					else
					{
						bool bSuccess = false;
						int nRank = Global.GetRandomNumber(0, 100);
						if (-1 == xmlData.m_sSuccessProbability || tmppartdata.m_nFailCount >= xmlData.m_nMaxFailCount || nRank < (int)xmlData.m_sSuccessProbability)
						{
							bSuccess = true;
							for (int i = 0; i < xmlData.NeedGoods.Count; i++)
							{
								int goodsId = xmlData.NeedGoods[i][0];
								int costCount = xmlData.NeedGoods[i][1];
								int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, goodsId);
								if (haveGoodsCnt < costCount)
								{
									return EHolyResult.NeedGoods;
								}
							}
							if (-1 != xmlData.m_nCostBandJinBi)
							{
								if (!Global.SubBindTongQianAndTongQian(client, xmlData.m_nCostBandJinBi, "圣物部件升级消耗"))
								{
									return EHolyResult.Error;
								}
							}
							if (-1 != xmlData.m_nNeedGoodsCount)
							{
								tmppartdata.m_nSlice -= xmlData.m_nNeedGoodsCount;
							}
							if (tmppartdata.m_nSlice < 0)
							{
								tmppartdata.m_nSlice = 0;
								return EHolyResult.Error;
							}
							bool bUsedBinding_just_placeholder = false;
							bool bUsedTimeLimited_just_placeholder = false;
							for (int i = 0; i < xmlData.NeedGoods.Count; i++)
							{
								int goodsId = xmlData.NeedGoods[i][0];
								int costCount = xmlData.NeedGoods[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsId, costCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("圣物部件升级时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", goodsId, costCount), null, true);
								}
								GoodsData goodsData = new GoodsData();
								goodsData.GoodsID = goodsId;
								goodsData.GCount = costCount;
							}
							HolyItemPartData holyItemPartData = tmppartdata;
							holyItemPartData.m_sSuit += 1;
							tmppartdata.m_nFailCount = 0;
						}
						else
						{
							for (int i = 0; i < xmlData.FaildNeedGoods.Count; i++)
							{
								int goodsId = xmlData.FaildNeedGoods[i][0];
								int costCount = xmlData.FaildNeedGoods[i][1];
								int haveGoodsCnt = Global.GetTotalGoodsCountByID(client, goodsId);
								if (haveGoodsCnt < costCount)
								{
									return EHolyResult.NeedGoods;
								}
							}
							if (-1 != xmlData.m_nCostBandJinBi)
							{
								if (!Global.SubBindTongQianAndTongQian(client, xmlData.m_nCostBandJinBi, "圣物部件升级消耗"))
								{
									return EHolyResult.Error;
								}
							}
							bool bUsedBinding_just_placeholder = false;
							bool bUsedTimeLimited_just_placeholder = false;
							for (int i = 0; i < xmlData.FaildNeedGoods.Count; i++)
							{
								int goodsId = xmlData.FaildNeedGoods[i][0];
								int costCount = xmlData.FaildNeedGoods[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsId, costCount, false, out bUsedBinding_just_placeholder, out bUsedTimeLimited_just_placeholder, false))
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("圣物部件升级时，消耗{1}个GoodsID={0}的物品失败", goodsId, costCount), null, true);
								}
								GoodsData goodsData2 = new GoodsData();
								goodsData2.GoodsID = goodsId;
								goodsData2.GCount = costCount;
							}
							if (-1 != xmlData.m_nFailCostGoodsCount)
							{
								tmppartdata.m_nSlice -= xmlData.m_nFailCostGoodsCount;
							}
							if (tmppartdata.m_nSlice < 0)
							{
								tmppartdata.m_nSlice = 0;
								return EHolyResult.Error;
							}
							tmppartdata.m_nFailCount++;
						}
						if (bSuccess)
						{
							this.UpdateHolyItemBuJianAttr(client, sShengWu_slot, sBuJian_slot);
							this.UpdataHolyItemExAttr(client, sShengWu_slot);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
						this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, tmppartdata);
						this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot], "圣物进阶", "系统", client.ClientData.RoleName, bSuccess ? "成功" : "失败", (xmlData.m_nCostBandJinBi != -1) ? xmlData.m_nCostBandJinBi : 0, client.ClientData.ZoneID, client.strUserID, tmppartdata.m_nSlice, client.ServerId, null);
						if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
						result = (bSuccess ? EHolyResult.Success : EHolyResult.Fail);
					}
				}
			}
			return result;
		}

		// Token: 0x060014E2 RID: 5346 RVA: 0x00146DFC File Offset: 0x00144FFC
		public void GetHolyItemPart(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, int nNum)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
			{
				Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
				HolyItemData tmpdata = null;
				HolyItemPartData tmppartdata = null;
				if (holyitemdata.TryGetValue(sShengWu_slot, out tmpdata))
				{
					if (tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
					{
						tmppartdata.m_nSlice += nNum;
						this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, tmppartdata);
						this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
						string strHint = StringUtil.substitute(GLang.GetLang(384, new object[0]), new object[]
						{
							Global.GetLang(HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot]),
							nNum
						});
						GameManager.ClientMgr.NotifyImportantMsg(client, strHint, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.PiaoFuZi, 0);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, HolyItemManager.SliceNameSet[(int)sShengWu_slot, (int)sBuJian_slot], "圣物碎片", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", nNum, client.ClientData.ZoneID, client.strUserID, tmppartdata.m_nSlice, client.ServerId, null);
					}
				}
			}
		}

		// Token: 0x060014E3 RID: 5347 RVA: 0x00146F30 File Offset: 0x00145130
		public void PlayGameAfterSend(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot7))
			{
				Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
				if (holyitemdata != null)
				{
					HolyItemData tmpdata = null;
					HolyItemPartData tmppartdata = null;
					for (sbyte i = 1; i <= HolyItemManager.MAX_HOLY_NUM; i += 1)
					{
						if (holyitemdata.TryGetValue(i, out tmpdata))
						{
							for (sbyte j = 1; j <= HolyItemManager.MAX_HOLY_PART_NUM; j += 1)
							{
								if (!tmpdata.m_PartArray.TryGetValue(j, out tmppartdata))
								{
									tmppartdata = new HolyItemPartData();
									tmpdata.m_PartArray.Add(j, tmppartdata);
								}
							}
						}
						else
						{
							tmpdata = new HolyItemData();
							holyitemdata.Add(i, tmpdata);
							tmpdata.m_sType = i;
							for (sbyte j = 1; j <= HolyItemManager.MAX_HOLY_PART_NUM; j += 1)
							{
								tmppartdata = new HolyItemPartData();
								tmpdata.m_PartArray.Add(j, tmppartdata);
							}
						}
					}
					client.sendCmd<Dictionary<sbyte, HolyItemData>>(1200, holyitemdata, false);
				}
			}
		}

		// Token: 0x060014E4 RID: 5348 RVA: 0x00147054 File Offset: 0x00145254
		public void HolyItemSendToClient(GameClient client, sbyte sShenWu_slot, sbyte sBuJian_slot)
		{
			Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
			if (holyitemdata != null)
			{
				HolyItemData tmpdata = null;
				HolyItemPartData tmppartdata = null;
				if (holyitemdata.TryGetValue(sShenWu_slot, out tmpdata))
				{
					if (tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
					{
						string strSend = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							sShenWu_slot,
							sBuJian_slot,
							tmppartdata.m_sSuit,
							tmppartdata.m_nSlice
						});
						client.sendCmd(1201, strSend, false);
					}
				}
			}
		}

		// Token: 0x060014E5 RID: 5349 RVA: 0x00147100 File Offset: 0x00145300
		public void UpdateAllHolyItemAttr(GameClient client)
		{
			for (sbyte i = 1; i <= HolyItemManager.MAX_HOLY_NUM; i += 1)
			{
				for (sbyte j = 1; j <= HolyItemManager.MAX_HOLY_PART_NUM; j += 1)
				{
					this.UpdateHolyItemBuJianAttr(client, i, j);
				}
				this.UpdataHolyItemExAttr(client, i);
			}
		}

		// Token: 0x060014E6 RID: 5350 RVA: 0x00147158 File Offset: 0x00145358
		public void UpdateHolyItemBuJianAttr(GameClient client, sbyte sShenWu_slot, sbyte sBuJian_slot)
		{
			Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
			if (null != holyitemdata)
			{
				HolyItemData tmpdata = null;
				HolyItemPartData tmppartdata = null;
				if (holyitemdata.TryGetValue(sShenWu_slot, out tmpdata))
				{
					if (tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
					{
						int nDataID = HolyPartInfo.GetBujianID(sShenWu_slot, sBuJian_slot, tmppartdata.m_sSuit);
						HolyPartInfo nXmlData = null;
						if (this._partDataDic.TryGetValue(nDataID, out nXmlData))
						{
							for (int i = 0; i < nXmlData.m_PropertyList.Count; i++)
							{
								this.ProcessAction(client, nXmlData.m_PropertyList[i].MagicActionID, nXmlData.m_PropertyList[i].MagicActionParams, 16, sShenWu_slot, sBuJian_slot);
							}
						}
					}
				}
			}
		}

		// Token: 0x060014E7 RID: 5351 RVA: 0x00147234 File Offset: 0x00145434
		public void UpdataHolyItemExAttr(GameClient client, sbyte sShenWu_slot)
		{
			Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
			if (null != holyitemdata)
			{
				HolyItemData tmpdata = null;
				HolyItemPartData tmppartdata = null;
				int sMinSuit = (int)HolyItemManager.MAX_HOLY_PART_LEVEL;
				if (holyitemdata.TryGetValue(sShenWu_slot, out tmpdata))
				{
					for (sbyte i = 1; i <= HolyItemManager.MAX_HOLY_PART_NUM; i += 1)
					{
						if (!tmpdata.m_PartArray.TryGetValue(i, out tmppartdata))
						{
							sMinSuit = 0;
							break;
						}
						if (sMinSuit > (int)tmppartdata.m_sSuit)
						{
							sMinSuit = (int)tmppartdata.m_sSuit;
						}
					}
				}
				else
				{
					sMinSuit = 0;
				}
				HolyInfo xmlData = null;
				int nDataID = HolyInfo.GetShengwuID((sbyte)sMinSuit, sShenWu_slot);
				if (this._holyDataDic.TryGetValue(nDataID, out xmlData))
				{
					for (int j = 0; j < xmlData.m_ExtraPropertyList.Count; j++)
					{
						this.ProcessAction(client, xmlData.m_ExtraPropertyList[j].MagicActionID, xmlData.m_ExtraPropertyList[j].MagicActionParams, 16, sShenWu_slot, 100);
					}
				}
			}
		}

		// Token: 0x060014E8 RID: 5352 RVA: 0x0014735C File Offset: 0x0014555C
		private void UpdateHolyItemData2DB(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, HolyItemPartData partdata = null)
		{
			Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
			HolyItemData tmpdata = null;
			HolyItemPartData tmppartdata = partdata;
			if (tmppartdata == null)
			{
				if (!holyitemdata.TryGetValue(sShengWu_slot, out tmpdata))
				{
					return;
				}
				if (!tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
				{
					return;
				}
			}
			string[] dbFields = null;
			string sCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				sShengWu_slot,
				sBuJian_slot,
				tmppartdata.m_sSuit,
				tmppartdata.m_nSlice,
				tmppartdata.m_nFailCount
			});
			TCPProcessCmdResults retcmd = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10206, sCmd, out dbFields, client.ServerId);
		}

		// Token: 0x060014E9 RID: 5353 RVA: 0x00147458 File Offset: 0x00145658
		private void ProcessAction(GameClient client, MagicActionIDs id, double[] actionParams, int nPropsSystemTypes, sbyte sShengWu_slot, sbyte sBuJian_slot)
		{
			switch (id)
			{
			case MagicActionIDs.POTION:
			case MagicActionIDs.HOLYWATER:
			case MagicActionIDs.RECOVERLIFEV:
			case MagicActionIDs.LIFESTEAL:
			case MagicActionIDs.FATALHURT:
			case MagicActionIDs.ADDATTACK:
			case MagicActionIDs.ADDATTACKINJURE:
			case MagicActionIDs.HITV:
			case MagicActionIDs.ADDDEFENSE:
			case MagicActionIDs.COUNTERACTINJUREVALUE:
			case MagicActionIDs.DAMAGETHORN:
			case MagicActionIDs.DODGE:
			case MagicActionIDs.MAXLIFEPERCENT:
			case MagicActionIDs.AddAttackPercent:
			case MagicActionIDs.AddDefensePercent:
			case MagicActionIDs.HitPercent:
			{
				ExtPropIndexes eExtProp = ExtPropIndexes.Max;
				switch (id)
				{
				case MagicActionIDs.POTION:
					eExtProp = ExtPropIndexes.Potion;
					break;
				case MagicActionIDs.HOLYWATER:
					eExtProp = ExtPropIndexes.Holywater;
					break;
				case MagicActionIDs.RECOVERLIFEV:
					eExtProp = ExtPropIndexes.RecoverLifeV;
					break;
				case MagicActionIDs.LIFESTEAL:
					eExtProp = ExtPropIndexes.LifeSteal;
					break;
				case MagicActionIDs.FATALHURT:
					eExtProp = ExtPropIndexes.Fatalhurt;
					break;
				case MagicActionIDs.ADDATTACK:
					eExtProp = ExtPropIndexes.AddAttack;
					break;
				case MagicActionIDs.ADDATTACKINJURE:
					eExtProp = ExtPropIndexes.AddAttackInjure;
					break;
				case MagicActionIDs.HITV:
					eExtProp = ExtPropIndexes.HitV;
					break;
				case MagicActionIDs.ADDDEFENSE:
					eExtProp = ExtPropIndexes.AddDefense;
					break;
				case MagicActionIDs.COUNTERACTINJUREVALUE:
					eExtProp = ExtPropIndexes.CounteractInjureValue;
					break;
				case MagicActionIDs.DAMAGETHORN:
					eExtProp = ExtPropIndexes.DamageThorn;
					break;
				case MagicActionIDs.DODGE:
					eExtProp = ExtPropIndexes.Dodge;
					break;
				case MagicActionIDs.MAXLIFEPERCENT:
					eExtProp = ExtPropIndexes.MaxLifePercent;
					break;
				case MagicActionIDs.AddAttackPercent:
					eExtProp = ExtPropIndexes.AddAttackPercent;
					break;
				case MagicActionIDs.AddDefensePercent:
					eExtProp = ExtPropIndexes.AddDefensePercent;
					break;
				case MagicActionIDs.HitPercent:
					eExtProp = ExtPropIndexes.HitPercent;
					break;
				}
				if (eExtProp != ExtPropIndexes.Max)
				{
					client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
					{
						nPropsSystemTypes,
						(int)sShengWu_slot,
						(int)sBuJian_slot,
						1000,
						(int)eExtProp,
						actionParams[0]
					});
				}
				break;
			}
			case MagicActionIDs.STRENGTH:
			{
				PropsCacheManager propsCacheManager = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 0;
				object[] array2 = array;
				int num = 4;
				double[] array3 = new double[4];
				array3[0] = actionParams[0];
				array2[num] = array3;
				propsCacheManager.SetBaseProps(array);
				break;
			}
			case MagicActionIDs.CONSTITUTION:
				client.ClientData.PropsCacheManager.SetBaseProps(new object[]
				{
					nPropsSystemTypes,
					(int)sShengWu_slot,
					(int)sBuJian_slot,
					3,
					new double[]
					{
						0.0,
						0.0,
						0.0,
						actionParams[0]
					}
				});
				break;
			case MagicActionIDs.DEXTERITY:
			{
				PropsCacheManager propsCacheManager2 = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 2;
				object[] array4 = array;
				int num2 = 4;
				double[] array3 = new double[4];
				array3[2] = actionParams[0];
				array4[num2] = array3;
				propsCacheManager2.SetBaseProps(array);
				break;
			}
			case MagicActionIDs.INTELLIGENCE:
			{
				PropsCacheManager propsCacheManager3 = client.ClientData.PropsCacheManager;
				object[] array = new object[5];
				array[0] = nPropsSystemTypes;
				array[1] = (int)sShengWu_slot;
				array[2] = (int)sBuJian_slot;
				array[3] = 1;
				object[] array5 = array;
				int num3 = 4;
				double[] array3 = new double[4];
				array3[1] = actionParams[0];
				array5[num3] = array3;
				propsCacheManager3.SetBaseProps(array);
				break;
			}
			}
		}

		// Token: 0x060014EA RID: 5354 RVA: 0x00147788 File Offset: 0x00145988
		public void GMSetHolyItemLvup(GameClient client, sbyte sShengWu_slot, sbyte sBuJian_slot, sbyte sLv)
		{
			Dictionary<sbyte, HolyItemData> holyitemdata = client.ClientData.MyHolyItemDataDic;
			HolyItemData tmpdata = null;
			HolyItemPartData tmppartdata = null;
			if (holyitemdata != null && holyitemdata.TryGetValue(sShengWu_slot, out tmpdata))
			{
				if (tmpdata.m_PartArray.TryGetValue(sBuJian_slot, out tmppartdata))
				{
					tmppartdata.m_sSuit = sLv;
					if (tmppartdata.m_sSuit > HolyItemManager.MAX_HOLY_PART_LEVEL)
					{
						tmppartdata.m_sSuit = HolyItemManager.MAX_HOLY_PART_LEVEL;
					}
					this.UpdateHolyItemBuJianAttr(client, sShengWu_slot, sBuJian_slot);
					this.UpdataHolyItemExAttr(client, sShengWu_slot);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					this.UpdateHolyItemData2DB(client, sShengWu_slot, sBuJian_slot, tmppartdata);
					this.HolyItemSendToClient(client, sShengWu_slot, sBuJian_slot);
				}
			}
		}

		// Token: 0x060014EC RID: 5356 RVA: 0x00147868 File Offset: 0x00145A68
		// Note: this type is marked as 'beforefieldinit'.
		static HolyItemManager()
		{
			string[,] array = new string[5, 7];
			array[0, 0] = "null";
			array[0, 1] = "null";
			array[0, 2] = "null";
			array[0, 3] = "null";
			array[0, 4] = "null";
			array[0, 5] = "null";
			array[0, 6] = "null";
			array[1, 0] = "null";
			array[1, 1] = "圣杯碎片1";
			array[1, 2] = "圣杯碎片2";
			array[1, 3] = "圣杯碎片3";
			array[1, 4] = "圣杯碎片4";
			array[1, 5] = "圣杯碎片5";
			array[1, 6] = "圣杯碎片6";
			array[2, 0] = "null";
			array[2, 1] = "圣冠碎片1";
			array[2, 2] = "圣冠碎片2";
			array[2, 3] = "圣冠碎片3";
			array[2, 4] = "圣冠碎片4";
			array[2, 5] = "圣冠碎片5";
			array[2, 6] = "圣冠碎片6";
			array[3, 0] = "null";
			array[3, 1] = "圣剑碎片1";
			array[3, 2] = "圣剑碎片2";
			array[3, 3] = "圣剑碎片3";
			array[3, 4] = "圣剑碎片4";
			array[3, 5] = "圣剑碎片5";
			array[3, 6] = "圣剑碎片6";
			array[4, 0] = "null";
			array[4, 1] = "圣典碎片1";
			array[4, 2] = "圣典碎片2";
			array[4, 3] = "圣典碎片3";
			array[4, 4] = "圣典碎片4";
			array[4, 5] = "圣典碎片5";
			array[4, 6] = "圣典碎片6";
			HolyItemManager.SliceNameSet = array;
			HolyItemManager.instance = new HolyItemManager();
		}

		// Token: 0x04001E38 RID: 7736
		public static sbyte MAX_HOLY_PART_LEVEL = 9;

		// Token: 0x04001E39 RID: 7737
		public static readonly sbyte MAX_HOLY_PART_NUM = 6;

		// Token: 0x04001E3A RID: 7738
		public static readonly sbyte MAX_HOLY_NUM = 4;

		// Token: 0x04001E3B RID: 7739
		private Dictionary<int, HolyPartInfo> _partDataDic = new Dictionary<int, HolyPartInfo>();

		// Token: 0x04001E3C RID: 7740
		private Dictionary<int, HolyInfo> _holyDataDic = new Dictionary<int, HolyInfo>();

		// Token: 0x04001E3D RID: 7741
		public static readonly string[,] SliceNameSet;

		// Token: 0x04001E3E RID: 7742
		private static HolyItemManager instance;
	}
}
