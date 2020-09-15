using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x0200079F RID: 1951
	public class StarConstellationManager
	{
		// Token: 0x060032CF RID: 13007 RVA: 0x002D04C8 File Offset: 0x002CE6C8
		public void LoadStarConstellationTypeInfo()
		{
			try
			{
				string fileName = "Config/XingZuo/XingZuoType.xml";
				XElement xmlFile = Global.GetGameResXml(string.Format(fileName, new object[0]));
				if (null != xmlFile)
				{
					IEnumerable<XElement> StarXEle = xmlFile.Elements("XingZuo").Elements<XElement>();
					foreach (XElement xmlItem in StarXEle)
					{
						if (null != xmlItem)
						{
							StarConstellationTypeInfo tmpInfo = new StarConstellationTypeInfo();
							int ID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
							tmpInfo.TypeID = ID;
							string sLevelInfo = Global.GetSafeAttributeStr(xmlItem, "KaiQiLevel");
							string[] sArrayLevelInfo = sLevelInfo.Split(new char[]
							{
								','
							});
							tmpInfo.ChangeLifeLimit = Global.SafeConvertToInt32(sArrayLevelInfo[0]);
							tmpInfo.LevelLimit = Global.SafeConvertToInt32(sArrayLevelInfo[1]);
							string strInfos = Global.GetSafeAttributeStr(xmlItem, "ShuXiangJiaCheng");
							string[] sArry = strInfos.Split(new char[]
							{
								'|'
							});
							if (sArry != null)
							{
								tmpInfo.Propertyinfo = new PropertyInfo();
								for (int i = 0; i < sArry.Length; i++)
								{
									string[] sArryInfo = sArry[i].Split(new char[]
									{
										','
									});
									string strPorpName = sArryInfo[0];
									string strPorpValue = sArryInfo[1];
									string[] strArrayPorpValue = strPorpValue.Split(new char[]
									{
										'-'
									});
									if (strPorpName == "Defense")
									{
										tmpInfo.Propertyinfo.PropertyID1 = 3;
										tmpInfo.Propertyinfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID2 = 4;
										tmpInfo.Propertyinfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mdefense")
									{
										tmpInfo.Propertyinfo.PropertyID3 = 5;
										tmpInfo.Propertyinfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID4 = 6;
										tmpInfo.Propertyinfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Attack")
									{
										tmpInfo.Propertyinfo.PropertyID5 = 7;
										tmpInfo.Propertyinfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID6 = 8;
										tmpInfo.Propertyinfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mattack")
									{
										tmpInfo.Propertyinfo.PropertyID7 = 9;
										tmpInfo.Propertyinfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID8 = 10;
										tmpInfo.Propertyinfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "HitV")
									{
										tmpInfo.Propertyinfo.PropertyID9 = 18;
										tmpInfo.Propertyinfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "Dodge")
									{
										tmpInfo.Propertyinfo.PropertyID10 = 19;
										tmpInfo.Propertyinfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "MaxLifeV")
									{
										tmpInfo.Propertyinfo.PropertyID11 = 13;
										tmpInfo.Propertyinfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
								}
							}
							strInfos = Global.GetSafeAttributeStr(xmlItem, "JiaChengBiLie");
							sArry = strInfos.Split(new char[]
							{
								'|'
							});
							if (sArry != null)
							{
								tmpInfo.AddPropStarSiteLimit = new int[sArry.Length];
								tmpInfo.AddPropModulus = new int[sArry.Length];
								for (int i = 0; i < sArry.Length; i++)
								{
									string[] sArryInfo = sArry[i].Split(new char[]
									{
										','
									});
									tmpInfo.AddPropStarSiteLimit[i] = Global.SafeConvertToInt32(sArryInfo[0]);
									tmpInfo.AddPropModulus[i] = Global.SafeConvertToInt32(sArryInfo[1]);
								}
							}
							this.m_StarConstellationTypeInfo.Add(ID, tmpInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/XingZuoType.xml", new object[0])));
			}
		}

		// Token: 0x060032D0 RID: 13008 RVA: 0x002D09F8 File Offset: 0x002CEBF8
		public void LoadStarConstellationDetailInfo()
		{
			for (int i = 0; i < 6; i++)
			{
				if (i != 4)
				{
					XElement xmlFile = null;
					try
					{
						xmlFile = Global.GetGameResXml(string.Format("Config/XingZuo/XingZuo_{0}.xml", i));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/XingZuo/XingZuo_{0}.xml", i)));
					}
					IEnumerable<XElement> StarXmlItems = xmlFile.Elements("XingZuo");
					Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> tmpDicInfo = new Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>();
					foreach (XElement xmlItem in StarXmlItems)
					{
						Dictionary<int, StarConstellationDetailInfo> tmpDic = new Dictionary<int, StarConstellationDetailInfo>();
						int nID = (int)Global.GetSafeAttributeDouble(xmlItem, "ID");
						IEnumerable<XElement> XingWeiItems = xmlItem.Elements("XingWei");
						foreach (XElement XingWeiItem in XingWeiItems)
						{
							StarConstellationDetailInfo tmpInfo = new StarConstellationDetailInfo();
							int ID = (int)Global.GetSafeAttributeDouble(XingWeiItem, "ID");
							tmpInfo.StarConstellationID = ID;
							string sLevelInfo = Global.GetSafeAttributeStr(XingWeiItem, "LevelLimit");
							string[] sArrayLevelInfo = sLevelInfo.Split(new char[]
							{
								','
							});
							tmpInfo.ChangeLifeLimit = Global.SafeConvertToInt32(sArrayLevelInfo[0]);
							tmpInfo.LevelLimit = Global.SafeConvertToInt32(sArrayLevelInfo[1]);
							tmpInfo.SuccessRate = (int)(Global.GetSafeAttributeDouble(XingWeiItem, "Succeed") * 10000.0);
							tmpInfo.NeedGoodsID = 0;
							tmpInfo.NeedGoodsNum = 0;
							tmpInfo.NeedJinBi = (int)Global.GetSafeAttributeDouble(XingWeiItem, "NeedJinBi");
							tmpInfo.NeedStarSoul = (int)Global.GetSafeAttributeDouble(XingWeiItem, "XingHun");
							string strShuXingInfos = Global.GetSafeAttributeStr(XingWeiItem, "ShuXing");
							string[] sArrayPropInfo = strShuXingInfos.Split(new char[]
							{
								'|'
							});
							if (sArrayPropInfo != null)
							{
								tmpInfo.Propertyinfo = new PropertyInfo();
								for (int j = 0; j < sArrayPropInfo.Length; j++)
								{
									string[] sArryInfo = sArrayPropInfo[j].Split(new char[]
									{
										','
									});
									string strPorpName = sArryInfo[0];
									string strPorpValue = sArryInfo[1];
									string[] strArrayPorpValue = strPorpValue.Split(new char[]
									{
										'-'
									});
									if (strPorpName == "Defense")
									{
										tmpInfo.Propertyinfo.PropertyID1 = 3;
										tmpInfo.Propertyinfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID2 = 4;
										tmpInfo.Propertyinfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mdefense")
									{
										tmpInfo.Propertyinfo.PropertyID3 = 5;
										tmpInfo.Propertyinfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID4 = 6;
										tmpInfo.Propertyinfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Attack")
									{
										tmpInfo.Propertyinfo.PropertyID5 = 7;
										tmpInfo.Propertyinfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID6 = 8;
										tmpInfo.Propertyinfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "Mattack")
									{
										tmpInfo.Propertyinfo.PropertyID7 = 9;
										tmpInfo.Propertyinfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
										tmpInfo.Propertyinfo.PropertyID8 = 10;
										tmpInfo.Propertyinfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(strArrayPorpValue[1]);
									}
									else if (strPorpName == "HitV")
									{
										tmpInfo.Propertyinfo.PropertyID9 = 18;
										tmpInfo.Propertyinfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "Dodge")
									{
										tmpInfo.Propertyinfo.PropertyID10 = 19;
										tmpInfo.Propertyinfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
									else if (strPorpName == "MaxLifeV")
									{
										tmpInfo.Propertyinfo.PropertyID11 = 13;
										tmpInfo.Propertyinfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(strArrayPorpValue[0]);
									}
								}
							}
							tmpDic.Add(ID, tmpInfo);
							if (ID > this.m_MaxStarSlotID)
							{
								this.m_MaxStarSlotID = ID;
							}
						}
						if (nID > this.m_MaxStarSiteID)
						{
							this.m_MaxStarSiteID = nID;
						}
						tmpDicInfo.Add(nID, tmpDic);
					}
					this.m_StarConstellationDetailInfo.Add(i, tmpDicInfo);
				}
			}
		}

		// Token: 0x060032D1 RID: 13009 RVA: 0x002D0F8C File Offset: 0x002CF18C
		public int GetExtendPropIndex(int nValue, StarConstellationTypeInfo starInfo)
		{
			if (nValue > 0)
			{
				for (int i = 0; i < starInfo.AddPropStarSiteLimit.Length; i++)
				{
					if (nValue >= starInfo.AddPropStarSiteLimit[i])
					{
						if (nValue == starInfo.AddPropStarSiteLimit[i])
						{
							return starInfo.AddPropModulus[i];
						}
						if (nValue < starInfo.AddPropStarSiteLimit[i + 1])
						{
							return starInfo.AddPropModulus[i];
						}
					}
				}
			}
			return -1;
		}

		// Token: 0x060032D2 RID: 13010 RVA: 0x002D1018 File Offset: 0x002CF218
		public void InitPlayerStarConstellationPorperty(GameClient client)
		{
			if (client.ClientData.RoleStarConstellationInfo != null && client.ClientData.RoleStarConstellationInfo.Count > 0)
			{
				int nOccupation = client.ClientData.Occupation;
				Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dicTmp = null;
				if (this.m_StarConstellationDetailInfo.TryGetValue(nOccupation, out dicTmp) && dicTmp != null)
				{
					client.ClientData.RoleStarConstellationProp.ResetStarConstellationProps();
					foreach (KeyValuePair<int, int> StarConstellationinfo in client.ClientData.RoleStarConstellationInfo)
					{
						int nStarSiteID = StarConstellationinfo.Key;
						int nStarSlotID = StarConstellationinfo.Value;
						if (nStarSiteID >= 0 && nStarSiteID <= this.m_MaxStarSiteID && nStarSlotID >= 0 && nStarSlotID <= this.m_MaxStarSlotID)
						{
							Dictionary<int, StarConstellationDetailInfo> dicTmpInfo = null;
							if (dicTmp.TryGetValue(nStarSiteID, out dicTmpInfo) && dicTmpInfo != null)
							{
								for (int i = 0; i <= nStarSlotID; i++)
								{
									StarConstellationDetailInfo tmpInfo = null;
									if (dicTmpInfo.TryGetValue(i, out tmpInfo) && tmpInfo != null)
									{
										PropertyInfo tmpProp = tmpInfo.Propertyinfo;
										if (tmpProp == null)
										{
											return;
										}
										this.ActivationStarConstellationProp(client, tmpProp, 1);
									}
								}
								this.ActivationStarConstellationExtendProp(client, StarConstellationinfo.Key);
							}
						}
					}
				}
			}
		}

		// Token: 0x060032D3 RID: 13011 RVA: 0x002D11D0 File Offset: 0x002CF3D0
		public int ActivationStarConstellation(GameClient client, int nStarSiteID)
		{
			int result;
			if (nStarSiteID < 1 || nStarSiteID > this.m_MaxStarSiteID)
			{
				result = -1;
			}
			else
			{
				if (client.ClientData.RoleStarConstellationInfo == null)
				{
					client.ClientData.RoleStarConstellationInfo = new Dictionary<int, int>();
				}
				int nStarSlot = 0;
				client.ClientData.RoleStarConstellationInfo.TryGetValue(nStarSiteID, out nStarSlot);
				if (nStarSlot >= this.m_MaxStarSlotID)
				{
					result = -1;
				}
				else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.GamePayerRolePartXingZuo, false))
				{
					result = -1;
				}
				else
				{
					nStarSlot++;
					int nOccupation = client.ClientData.Occupation;
					Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dicTmp = null;
					if (!this.m_StarConstellationDetailInfo.TryGetValue(nOccupation, out dicTmp) || dicTmp == null)
					{
						result = -2;
					}
					else
					{
						Dictionary<int, StarConstellationDetailInfo> dicTmpInfo = null;
						if (!dicTmp.TryGetValue(nStarSiteID, out dicTmpInfo) || dicTmpInfo == null)
						{
							result = -2;
						}
						else
						{
							StarConstellationDetailInfo tmpInfo = null;
							if (!dicTmpInfo.TryGetValue(nStarSlot, out tmpInfo) || tmpInfo == null)
							{
								result = -2;
							}
							else
							{
								int nNeeChangeLife = tmpInfo.ChangeLifeLimit;
								int nNeedLev = tmpInfo.LevelLimit;
								int nReqUnionLevel = Global.GetUnionLevel(nNeeChangeLife, nNeedLev, false);
								if (Global.GetUnionLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, false) < nReqUnionLevel)
								{
									result = -3;
								}
								else
								{
									int nGoods = tmpInfo.NeedGoodsID;
									int nNum = tmpInfo.NeedGoodsNum;
									if (nGoods > 0 && nNum > 0)
									{
										GoodsData goods = Global.GetGoodsByID(client, nGoods);
										if (goods == null || goods.GCount < nNum)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(533, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -5;
										}
									}
									int nNeedStarSoul = tmpInfo.NeedStarSoul;
									if (nNeedStarSoul > 0)
									{
										if (nNeedStarSoul > client.ClientData.StarSoul)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(534, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -9;
										}
									}
									int nNeedMoney = tmpInfo.NeedJinBi;
									if (nNeedMoney > 0)
									{
										if (!Global.SubBindTongQianAndTongQian(client, nNeedMoney, "激活星座"))
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(535, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -10;
										}
									}
									if (nGoods > 0 && nNum > 0)
									{
										bool usedBinding = false;
										bool usedTimeLimited = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, nGoods, nNum, false, out usedBinding, out usedTimeLimited, false))
										{
											return -6;
										}
									}
									if (nNeedStarSoul > 0)
									{
										GameManager.ClientMgr.ModifyStarSoulValue(client, -nNeedStarSoul, "激活星座", true, true);
									}
									int nRate = Global.GetRandomNumber(1, 10001);
									if (nRate > tmpInfo.SuccessRate)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(536, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
										result = -100;
									}
									else
									{
										TCPOutPacket tcpOutPacket = null;
										string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, nStarSiteID, nStarSlot);
										TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10166, strDbCmd, out tcpOutPacket, client.ServerId);
										if (TCPProcessCmdResults.RESULT_FAILED == dbRequestResult)
										{
											result = -7;
										}
										else
										{
											Global.PushBackTcpOutPacket(tcpOutPacket);
											PropertyInfo tmpProp = tmpInfo.Propertyinfo;
											if (tmpProp == null)
											{
												result = -8;
											}
											else
											{
												client.ClientData.RoleStarConstellationInfo[nStarSiteID] = nStarSlot;
												this.ActivationStarConstellationProp(client, tmpProp, 1);
												if (0 == nStarSlot % 12)
												{
													this.ActivationStarConstellationExtendProp(client, nStarSiteID);
													GameManager.StarConstellationMgr.InitPlayerStarConstellationPorperty(client);
												}
												GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
												ProcessTask.ProcessRoleTaskVal(client, TaskTypes.XingZuoStar, -1);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
												client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
												GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												result = 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060032D4 RID: 13012 RVA: 0x002D1734 File Offset: 0x002CF934
		public void ActivationStarConstellationExtendProp(GameClient client, int nSiteID)
		{
			if (client != null && client.ClientData.RoleStarConstellationInfo != null)
			{
				int nStarSlot = 0;
				if (client.ClientData.RoleStarConstellationInfo.TryGetValue(nSiteID, out nStarSlot))
				{
					StarConstellationTypeInfo scTypeInfo = null;
					if (this.m_StarConstellationTypeInfo.TryGetValue(nSiteID, out scTypeInfo) && scTypeInfo != null)
					{
						int nModulus = 1;
						if (nStarSlot > 0)
						{
							nModulus = this.GetExtendPropIndex(nStarSlot, scTypeInfo);
						}
						if (nModulus > 0)
						{
							PropertyInfo tmpProp = scTypeInfo.Propertyinfo;
							if (tmpProp != null)
							{
								this.ActivationStarConstellationProp(client, tmpProp, nModulus);
							}
						}
					}
				}
			}
		}

		// Token: 0x060032D5 RID: 13013 RVA: 0x002D17F0 File Offset: 0x002CF9F0
		public void ActivationStarConstellationAll(GameClient client)
		{
			int nOccupation = client.ClientData.Occupation;
			Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dicTmp = null;
			if (this.m_StarConstellationDetailInfo.TryGetValue(nOccupation, out dicTmp) && dicTmp != null)
			{
				if (client.ClientData.RoleStarConstellationInfo == null)
				{
					client.ClientData.RoleStarConstellationInfo = new Dictionary<int, int>();
				}
				foreach (KeyValuePair<int, Dictionary<int, StarConstellationDetailInfo>> kvp in dicTmp)
				{
					TCPOutPacket tcpOutPacket = null;
					string strDbCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, kvp.Key, this.m_MaxStarSlotID);
					TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10166, strDbCmd, out tcpOutPacket, client.ServerId);
					if (TCPProcessCmdResults.RESULT_FAILED != dbRequestResult)
					{
						Global.PushBackTcpOutPacket(tcpOutPacket);
					}
					client.ClientData.RoleStarConstellationInfo[kvp.Key] = this.m_MaxStarSlotID;
					StarConstellationDetailInfo tmpInfo = null;
					if (kvp.Value.TryGetValue(this.m_MaxStarSlotID, out tmpInfo) && null != tmpInfo.Propertyinfo)
					{
						this.ActivationStarConstellationProp(client, tmpInfo.Propertyinfo, 1);
					}
					this.ActivationStarConstellationExtendProp(client, kvp.Key);
					GameManager.StarConstellationMgr.InitPlayerStarConstellationPorperty(client);
				}
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.XingZuoStar, -1);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
				client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
				GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				client.sendCmd<Dictionary<int, int>>(661, client.ClientData.RoleStarConstellationInfo, false);
			}
		}

		// Token: 0x060032D6 RID: 13014 RVA: 0x002D1A3C File Offset: 0x002CFC3C
		public bool IfStarConstellationPerfect(GameClient client)
		{
			int nOccupation = client.ClientData.Occupation;
			Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dicTmp = null;
			bool result;
			if (!this.m_StarConstellationDetailInfo.TryGetValue(nOccupation, out dicTmp) || dicTmp == null)
			{
				result = false;
			}
			else if (client.ClientData.RoleStarConstellationInfo == null || client.ClientData.RoleStarConstellationInfo.Count != dicTmp.Count)
			{
				result = false;
			}
			else
			{
				foreach (int star in client.ClientData.RoleStarConstellationInfo.Values)
				{
					if (star < this.m_MaxStarSlotID)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060032D7 RID: 13015 RVA: 0x002D1B24 File Offset: 0x002CFD24
		public void ActivationStarConstellationProp(GameClient client, PropertyInfo tmpProp, int nModulus = 1)
		{
			if (tmpProp.PropertyID1 >= 0 && tmpProp.AddPropertyMinValue1 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID1] += (double)(tmpProp.AddPropertyMinValue1 * nModulus);
			}
			if (tmpProp.PropertyID2 >= 0 && tmpProp.AddPropertyMaxValue1 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID2] += (double)(tmpProp.AddPropertyMaxValue1 * nModulus);
			}
			if (tmpProp.PropertyID3 >= 0 && tmpProp.AddPropertyMinValue2 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID3] += (double)(tmpProp.AddPropertyMinValue2 * nModulus);
			}
			if (tmpProp.PropertyID4 >= 0 && tmpProp.AddPropertyMaxValue2 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID4] += (double)(tmpProp.AddPropertyMaxValue2 * nModulus);
			}
			if (tmpProp.PropertyID5 >= 0 && tmpProp.AddPropertyMinValue3 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID5] += (double)(tmpProp.AddPropertyMinValue3 * nModulus);
			}
			if (tmpProp.PropertyID6 >= 0 && tmpProp.AddPropertyMaxValue3 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID6] += (double)(tmpProp.AddPropertyMaxValue3 * nModulus);
			}
			if (tmpProp.PropertyID7 >= 0 && tmpProp.AddPropertyMinValue4 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID7] += (double)(tmpProp.AddPropertyMinValue4 * nModulus);
			}
			if (tmpProp.PropertyID8 >= 0 && tmpProp.AddPropertyMaxValue4 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID8] += (double)(tmpProp.AddPropertyMaxValue4 * nModulus);
			}
			if (tmpProp.PropertyID9 >= 0 && tmpProp.AddPropertyMinValue5 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID9] += (double)(tmpProp.AddPropertyMinValue5 * nModulus);
			}
			if (tmpProp.PropertyID10 >= 0 && tmpProp.AddPropertyMinValue6 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID10] += (double)(tmpProp.AddPropertyMinValue6 * nModulus);
			}
			if (tmpProp.PropertyID11 >= 0 && tmpProp.AddPropertyMinValue7 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID11] += (double)(tmpProp.AddPropertyMinValue7 * nModulus);
			}
		}

		// Token: 0x04003EE9 RID: 16105
		public Dictionary<int, StarConstellationTypeInfo> m_StarConstellationTypeInfo = new Dictionary<int, StarConstellationTypeInfo>();

		// Token: 0x04003EEA RID: 16106
		public Dictionary<int, Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>> m_StarConstellationDetailInfo = new Dictionary<int, Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>>();

		// Token: 0x04003EEB RID: 16107
		public int m_MaxStarSiteID = 0;

		// Token: 0x04003EEC RID: 16108
		public int m_MaxStarSlotID = 0;
	}
}
