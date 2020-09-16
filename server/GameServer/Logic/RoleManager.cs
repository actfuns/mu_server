using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic.Name;
using GameServer.Logic.Talent;
using GameServer.Server;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class RoleManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static RoleManager getInstance()
		{
			return RoleManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.PurchaseOccupationNeedZuanShi = GameManager.systemParamsList.GetParamValueIntArrayByName("PurchaseOccupation", ',');
					this.RuntimeData.PurchaseOccupationGoods = (int)GameManager.systemParamsList.GetParamValueIntByName("PurchaseOccupationGoods", 2100);
					this.RuntimeData.CanChangeOccupationMapCodes.Clear();
					fileName = "Config/Settings.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					XElement xml = XElement.Load(fullPathFileName);
					IEnumerable<XElement> nodes = ConfigHelper.GetXElements(xml, "Map");
					foreach (XElement node in nodes)
					{
						int mapCode = (int)Global.GetSafeAttributeLong(node, "Code");
						string str = Global.GetDefAttributeStr(node, "Transfer", "0");
						if (str == "1")
						{
							this.RuntimeData.CanChangeOccupationMapCodes.Add(mapCode);
						}
					}
					this.RuntimeData.DisableChangeOccupationGoodsTypes.Clear();
					for (int i = 11; i <= 21; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					for (int i = 0; i <= 6; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					for (int i = 40; i <= 45; i++)
					{
						this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(i);
					}
					int[] goodsTypes = GameManager.systemParamsList.GetParamValueIntArrayByName("OccupationUnshareCategoriy", ',');
					if (null != goodsTypes)
					{
						foreach (int goodsType in goodsTypes)
						{
							this.RuntimeData.DisableChangeOccupationGoodsTypes.Add(goodsType);
						}
					}
				}
				catch (Exception ex)
				{
					success = false;
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				}
			}
			return success;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1410, 2, 2, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1411, 2, 2, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1412, 7, 7, RoleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1410:
					result = this.PurchaseOccupation(client, nID, bytes, cmdParams);
					break;
				case 1411:
					result = this.ChangeOccupation(client, nID, bytes, cmdParams);
					break;
				case 1412:
					result = this.CreateOccupationSummoner(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		private bool IsGongNengOpened(GameClient client)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot3) && GlobalNew.IsGongNengOpened(client, GongNengIDs.ZhuanZhi, false);
		}

		
		private bool PurchaseOccupation(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			int targetOccupation = Global.SafeConvertToInt32(cmdParams[1]);
			int occupation = client.ClientData.Occupation;
			bool result2;
			if (GameManager.SummonerMgr.IsSummoner(targetOccupation) && !GameManager.SummonerMgr.IsVersionSystemOpenOfSummoner())
			{
				result2 = false;
			}
			else
			{
				GameManager.ClientMgr.StopClientStoryboard(client, -1);
				lock (this.RuntimeData.Mutex)
				{
					if (!this.IsGongNengOpened(client))
					{
						result = -12;
					}
					else if (!client.InSafeRegion)
					{
						result = -33;
					}
					else if (occupation == targetOccupation)
					{
						result = -18;
					}
					else if (!this.RuntimeData.CanChangeOccupationMapCodes.Contains(client.ClientData.MapCode))
					{
						result = -34;
					}
					else if (client.ClientData.OccupationList.Contains(targetOccupation))
					{
						result = -18;
					}
					else
					{
						int count = client.ClientData.OccupationList.Count - 1;
						if (count >= Data.RoleOccupationMaxCount)
						{
							result = -36;
						}
						else if (this.RuntimeData.PurchaseOccupationNeedZuanShi == null || this.RuntimeData.PurchaseOccupationNeedZuanShi.Length <= count)
						{
							result = -36;
						}
						else if (this.RuntimeData.PurchaseOccupationNeedZuanShi[count] < 0)
						{
							result = -12;
						}
						else if (client.ClientData.UserMoney < this.RuntimeData.PurchaseOccupationNeedZuanShi[count])
						{
							result = -10;
						}
						else if (!GameManager.ClientMgr.SubUserMoney(client, this.RuntimeData.PurchaseOccupationNeedZuanShi[count], "购买副职业", true, true, true, true, DaiBiSySType.None))
						{
							result = -10;
						}
						else
						{
							client.ClientData.OccupationList.Add(targetOccupation);
							string occupationListString = Global.ListToString<int>(client.ClientData.OccupationList, '$');
							Global.SaveRoleParamsStringToDB(client, "20017", occupationListString, true);
						}
					}
				}
				client.sendCmd<int>(nID, result, false);
				result2 = true;
			}
			return result2;
		}

		
		private bool ChangeOccupation(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			int targetOccupation = Global.SafeConvertToInt32(cmdParams[1]);
			int occupation = client.ClientData.Occupation;
			GameManager.ClientMgr.StopClientStoryboard(client, -1);
			if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else if (!client.InSafeRegion)
			{
				result = -33;
			}
			else if (occupation == targetOccupation)
			{
				result = -18;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CanChangeOccupationMapCodes.Contains(client.ClientData.MapCode))
					{
						result = -34;
						goto IL_221;
					}
					if (!client.ClientData.OccupationList.Contains(targetOccupation))
					{
						result = -20;
						goto IL_221;
					}
				}
				RoleCustomData customData = Global.sendToDB<RoleCustomData, int>(10230, client.ClientData.RoleID, client.ServerId);
				if (null == customData)
				{
					customData = new RoleCustomData
					{
						roleId = client.ClientData.RoleID
					};
				}
				this.SaveRoleCustomData(client, customData);
				if (!this.StoreRoleOccGoodsList(client))
				{
					result = -15;
				}
				if (result >= 0)
				{
					string[] args = Global.SendToDB<string>(10126, string.Format("{0}:{1}", client.ClientData.RoleID, targetOccupation), client.ServerId);
					if (args[1] != targetOccupation.ToString())
					{
						result = -15;
					}
					else
					{
						EventLogManager.AddChangeOccupationEvent(client, targetOccupation);
						client.ClientData.Occupation = targetOccupation;
						client.ClientData.IsMainOccupation = (client.ClientData.OccupationList[0] == client.ClientData.Occupation);
						this.RestoreRoleCustomData(client, customData);
						this.RestoreRoleOccGoodsList(client);
						RebornManager.getInstance().InitPlayerRebornPorperty(client);
						client.sendCmd<int>(13999, client.ClientData.RoleID, false);
					}
				}
			}
			IL_221:
			if (result < 0)
			{
				client.sendCmd<int>(nID, result, false);
			}
			return true;
		}

		
		private bool CreateOccupationSummoner(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int PurchaseOccupationGoods = 0;
			lock (this.RuntimeData.Mutex)
			{
				PurchaseOccupationGoods = this.RuntimeData.PurchaseOccupationGoods;
			}
			bool result2;
			if (!SummonerData.CreateMapSet.Contains(client.CurrentMapCode))
			{
				string strcmd = string.Format("{0}:{1}", -21, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
				{
					"",
					"",
					"",
					"",
					"",
					""
				}));
				client.sendCmd(nID, strcmd, false);
				result2 = true;
			}
			else
			{
				int GoodsNum = Global.GetTotalGoodsCountByID(client, PurchaseOccupationGoods);
				if (GoodsNum <= 0)
				{
					string strcmd = string.Format("{0}:{1}", -6, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
					{
						"",
						"",
						"",
						"",
						"",
						""
					}));
					client.sendCmd(nID, strcmd, false);
					result2 = true;
				}
				else
				{
					TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(client.strUserID);
					if (null == clientSocket)
					{
						result2 = true;
					}
					else
					{
						string userID = cmdParams[0];
						string userName = cmdParams[1];
						int sex = Convert.ToInt32(cmdParams[2]);
						int occup = Convert.ToInt32(cmdParams[3]);
						string[] nameAndPingTaiID = cmdParams[4].Split(new char[]
						{
							'$'
						});
						int zoneID = Convert.ToInt32(cmdParams[5]);
						string deviceID = clientSocket.deviceID;
						if (sex != 1 || occup != 5 || !GameManager.SummonerMgr.IsVersionSystemOpenOfSummoner())
						{
							string strcmd = string.Format("{0}:{1}", -12, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
							{
								"",
								"",
								"",
								"",
								"",
								""
							}));
							client.sendCmd(nID, strcmd, false);
							result2 = true;
						}
						else
						{
							string name = nameAndPingTaiID[0];
							int ret = NameServerNamager.CheckInvalidCharacters(name, false);
							if (ret <= 0)
							{
								string strcmd = string.Format("{0}:{1}", ret, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
								{
									"",
									"",
									"",
									"",
									"",
									""
								}));
								client.sendCmd(nID, strcmd, false);
								result2 = true;
							}
							else if (!SingletonTemplate<NameManager>.Instance().IsNameLengthOK(name))
							{
								string strcmd = string.Format("{0}:{1}", -2, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
								{
									"",
									"",
									"",
									"",
									"",
									""
								}));
								client.sendCmd(nID, strcmd, false);
								result2 = true;
							}
							else
							{
								ret = NameServerNamager.RegisterNameToNameServer(zoneID, userID, nameAndPingTaiID, 0, 0);
								if (ret <= 0)
								{
									string strcmd = string.Format("{0}:{1}", ret, string.Format("{0}${1}${2}${3}${4}${5}", new object[]
									{
										"",
										"",
										"",
										"",
										"",
										""
									}));
									client.sendCmd(nID, strcmd, false);
									result2 = true;
								}
								else
								{
									int NotifyLeftTime = 0;
									if (!SingletonTemplate<CreateRoleLimitManager>.Instance().IfCanCreateRole(userID, userName, deviceID, ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString(), out NotifyLeftTime))
									{
										string strcmd = string.Format("{0}:{1}", -7, NotifyLeftTime);
										client.sendCmd(nID, strcmd, false);
										result2 = true;
									}
									else
									{
										string cmddata = string.Format("{0}:{1}", new UTF8Encoding().GetString(bytes, 0, bytes.Length), 1);
										byte[] bytesCmd = new UTF8Encoding().GetBytes(cmddata);
										TCPOutPacket tcpOutPacket = null;
										TCPProcessCmdResults result = Global.TransferRequestToDBServer(TCPManager.getInstance(), clientSocket, Global._TCPManager.tcpClientPool, TCPManager.getInstance().tcpRandKey, Global._TCPManager.TcpOutPacketPool, 102, bytesCmd, bytesCmd.Length, out tcpOutPacket, clientSocket.ServerId);
										if (null == tcpOutPacket)
										{
											result2 = true;
										}
										else
										{
											tcpOutPacket.PacketCmdID = (ushort)nID;
											string strCmdResult = null;
											tcpOutPacket.GetPacketCmdData(out strCmdResult);
											client.sendCmd(tcpOutPacket, true);
											if (null != strCmdResult)
											{
												string[] ResultField = strCmdResult.Split(new char[]
												{
													':'
												});
												if (ResultField.Length == 2 && Global.SafeConvertToInt32(ResultField[0]) == 1)
												{
													bool usedBinding = false;
													bool usedTimeLimited = false;
													GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, PurchaseOccupationGoods, 1, false, out usedBinding, out usedTimeLimited, false);
													SingletonTemplate<CreateRoleLimitManager>.Instance().ModifyCreateRoleNum(userID, userName, deviceID, ((IPEndPoint)clientSocket.RemoteEndPoint).Address.ToString());
													string[] fields = ResultField[1].Split(new char[]
													{
														'$'
													});
													int newRoleID = Global.SafeConvertToInt32(fields[0]);
													client.sendCmd<int>(13999, newRoleID, false);
												}
											}
											result2 = true;
										}
									}
								}
							}
						}
					}
				}
			}
			return result2;
		}

		
		public bool StoreRoleOccGoodsList(GameClient client)
		{
			bool result = true;
			List<GoodsData> goodsList = Global.GetUsingGoodsList(client.ClientData);
			int site = 1000 + client.ClientData.Occupation;
			foreach (GoodsData goodsData in goodsList)
			{
				int catetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
				if (this.RuntimeData.DisableChangeOccupationGoodsTypes.Contains(catetoriy))
				{
					goodsData.Site = site;
					goodsData.Using = 0;
					if (!Global.UpdateGoodsSiteAndUsingToDB(client, goodsData))
					{
						result = false;
					}
				}
			}
			if (result)
			{
				lock (client.ClientData.GoodsDataList)
				{
					foreach (GoodsData goodsData in goodsList)
					{
						client.ClientData.GoodsDataList.Remove(goodsData);
					}
				}
			}
			return result;
		}

		
		public void RestoreRoleOccGoodsList(GameClient client)
		{
			int site = 1000 + client.ClientData.Occupation;
			List<GoodsData> occGoodslist = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, site), client.ServerId);
			if (occGoodslist != null && occGoodslist.Count > 0)
			{
				lock (client.ClientData.GoodsDataList)
				{
					foreach (GoodsData goodsData in occGoodslist)
					{
						goodsData.Site = 0;
						goodsData.Using = 1;
						Global.UpdateGoodsSiteAndUsingToDB(client, goodsData);
						client.ClientData.GoodsDataList.Add(goodsData);
					}
				}
			}
		}

		
		private void RestoreRoleCustomData(GameClient client, RoleCustomData customData)
		{
			RoleCustomDataItem roleCustomData = null;
			if (customData != null && customData.customDataList != null && customData.customDataList.Count > 0)
			{
				roleCustomData = customData.customDataList.Find((RoleCustomDataItem x) => x.Occupation == client.ClientData.Occupation);
			}
			if (null == roleCustomData)
			{
				roleCustomData = new RoleCustomDataItem();
			}
			client.ClientData.MainQuickBarKeys = roleCustomData.Main_quick_keys;
			GameManager.DBCmdMgr.AddDBCmd(10010, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 0, roleCustomData.Main_quick_keys), null, client.ServerId);
			int[] rolePoint = new int[4];
			for (int i = 0; i < 4; i++)
			{
				if (roleCustomData.RolePointList != null && i < roleCustomData.RolePointList.Count)
				{
					rolePoint[i] = roleCustomData.RolePointList[i];
				}
			}
			client.ClientData.PropStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless") + rolePoint[0];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropStrength", client.ClientData.PropStrength, true);
			client.ClientData.PropIntelligence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless") + rolePoint[1];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropIntelligence", client.ClientData.PropIntelligence, true);
			client.ClientData.PropDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless") + rolePoint[2];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropDexterity", client.ClientData.PropDexterity, true);
			client.ClientData.PropConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless") + rolePoint[3];
			Global.SaveRoleParamsInt32ValueToDB(client, "PropConstitution", client.ClientData.PropConstitution, true);
			TalentManager.DBTalentEffectClear(client.ClientData.RoleID, client.ClientData.ZoneID, client.ServerId);
			foreach (TalentEffectItem item in roleCustomData.EffectList)
			{
				TalentManager.ModifyEffect(client, item.ID, item.TalentType, item.Level);
			}
			client.ClientData.DefaultSkillLev = roleCustomData.DefaultSkillLev;
			Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillLev", client.ClientData.DefaultSkillLev, true);
			client.ClientData.DefaultSkillUseNum = roleCustomData.DefaultSkillUseNum;
			Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillUseNum", client.ClientData.DefaultSkillUseNum, true);
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > 0)
			{
				for (int i = 0; i < client.ClientData.FuWenTabList.Count; i++)
				{
					int skillID = 0;
					List<int> shenShiActiveList = new List<int>();
					if (roleCustomData.ShenShiEuipSkill != null && i < roleCustomData.ShenShiEuipSkill.Count)
					{
						skillID = roleCustomData.ShenShiEuipSkill[i].SkillEquip;
						shenShiActiveList = roleCustomData.ShenShiEuipSkill[i].ShenShiActiveList;
					}
					client.ClientData.FuWenTabList[i].SkillEquip = skillID;
					client.ClientData.FuWenTabList[i].ShenShiActiveList = shenShiActiveList;
					Global.sendToDB<int, FuWenTabData>(20316, client.ClientData.FuWenTabList[i], 0);
				}
			}
		}

		
		private void SaveRoleCustomData(GameClient client, RoleCustomData customData)
		{
			int oldIndex = -1;
			RoleCustomDataItem roleCustomDataOld = null;
			RoleCustomDataItem roleCustomData = null;
			if (customData != null && customData.customDataList != null && customData.customDataList.Count > 0)
			{
				oldIndex = customData.customDataList.FindIndex((RoleCustomDataItem x) => x.Occupation == client.ClientData.Occupation);
				if (oldIndex >= 0)
				{
					roleCustomDataOld = customData.customDataList[oldIndex];
				}
			}
			roleCustomData = new RoleCustomDataItem
			{
				Occupation = client.ClientData.Occupation
			};
			roleCustomData.Main_quick_keys = client.ClientData.MainQuickBarKeys;
			roleCustomData.RolePointList.Add(client.ClientData.PropStrength - Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless"));
			roleCustomData.RolePointList.Add(client.ClientData.PropIntelligence - Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless"));
			roleCustomData.RolePointList.Add(client.ClientData.PropDexterity - Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless"));
			roleCustomData.RolePointList.Add(client.ClientData.PropConstitution - Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless"));
			for (int i = 0; i < roleCustomData.RolePointList.Count; i++)
			{
				if (roleCustomData.RolePointList[i] < 0)
				{
					roleCustomData.RolePointList[i] = 0;
				}
			}
			int sum = roleCustomData.RolePointList.Sum();
			while (sum > client.ClientData.TotalPropPoint)
			{
				for (int i = 0; i < roleCustomData.RolePointList.Count; i++)
				{
					if (roleCustomData.RolePointList[i] > 0)
					{
						List<int> rolePointList;
						int index;
						(rolePointList = roleCustomData.RolePointList)[index = i] = rolePointList[index] - 1;
						sum--;
					}
				}
			}
			TalentData talentData = client.ClientData.MyTalentData;
			if (talentData != null && null != talentData.EffectList)
			{
				foreach (TalentEffectItem item in talentData.EffectList)
				{
					roleCustomData.EffectList.Add(new TalentEffectItem
					{
						ID = item.ID,
						Level = item.Level,
						TalentType = item.TalentType
					});
				}
			}
			roleCustomData.EffectList.Sort((TalentEffectItem x, TalentEffectItem y) => x.ID - y.ID);
			roleCustomData.DefaultSkillLev = client.ClientData.DefaultSkillLev;
			roleCustomData.DefaultSkillUseNum = client.ClientData.DefaultSkillUseNum;
			if (client.ClientData.FuWenTabList != null && client.ClientData.FuWenTabList.Count > 0)
			{
				List<SkillEquipData> skillList = new List<SkillEquipData>();
				foreach (FuWenTabData item2 in client.ClientData.FuWenTabList)
				{
					skillList.Add(new SkillEquipData
					{
						SkillEquip = item2.SkillEquip,
						ShenShiActiveList = item2.ShenShiActiveList
					});
				}
				roleCustomData.ShenShiEuipSkill = skillList;
			}
			bool changed = false;
			if (null == roleCustomDataOld)
			{
				customData.customDataList.Add(roleCustomData);
			}
			else
			{
				if (null == roleCustomDataOld.RolePointList)
				{
					changed = true;
				}
				else if (roleCustomDataOld.RolePointList.Count != roleCustomData.RolePointList.Count)
				{
					changed = true;
				}
				else
				{
					int i = 0;
					while (i < roleCustomDataOld.RolePointList.Count && i < roleCustomData.RolePointList.Count)
					{
						if (roleCustomDataOld.RolePointList[i] != roleCustomData.RolePointList[i])
						{
							changed = true;
							break;
						}
						i++;
					}
				}
				if (null == roleCustomDataOld.EffectList)
				{
					changed = true;
				}
				else if (roleCustomDataOld.EffectList.Count != roleCustomData.EffectList.Count)
				{
					changed = true;
				}
				else
				{
					roleCustomDataOld.EffectList.Sort((TalentEffectItem x, TalentEffectItem y) => x.ID - y.ID);
					for (int i = 0; i < roleCustomDataOld.EffectList.Count; i++)
					{
						TalentEffectItem x2 = roleCustomDataOld.EffectList[i];
						TalentEffectItem y2 = roleCustomData.EffectList[i];
						if (x2.ID != y2.ID || x2.Level != y2.Level)
						{
							changed = true;
							break;
						}
					}
				}
				if (roleCustomDataOld.Main_quick_keys != roleCustomData.Main_quick_keys)
				{
					changed = true;
				}
				if (roleCustomData.DefaultSkillLev != roleCustomDataOld.DefaultSkillLev || roleCustomData.DefaultSkillUseNum != roleCustomDataOld.DefaultSkillUseNum)
				{
					changed = true;
				}
				if (null != roleCustomData.ShenShiEuipSkill)
				{
					changed = true;
				}
				if (changed)
				{
					customData.customDataList[oldIndex] = roleCustomData;
				}
			}
			if (client.ClientData.IsMainOccupation)
			{
				customData.roleData4Selector = Global.RoleDataEx2RoleData4Selector(client.ClientData.GetRoleData());
			}
			Global.sendToDB<int, RoleCustomData>(10233, customData, client.ServerId);
		}

		
		public RoleData4Selector GetMainOccupationRoleDataForSelector(int roleId, int serverId)
		{
			RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, int>(10232, roleId, serverId);
			RoleData4Selector result;
			if (roleData4Selector == null || roleData4Selector.RoleID < 0)
			{
				result = null;
			}
			else
			{
				result = roleData4Selector;
			}
			return result;
		}

		
		public RoleDataMini GetRoleDataMiniFromRoleData4Selector(RoleData4Selector roleData4Selector)
		{
			RoleDataMini MyRoleDataMini = null;
			if (roleData4Selector != null && roleData4Selector.RoleID > 0)
			{
				MyRoleDataMini = new RoleDataMini();
				MyRoleDataMini.RoleID = roleData4Selector.RoleID;
				MyRoleDataMini.RoleName = roleData4Selector.RoleName;
				MyRoleDataMini.RoleSex = roleData4Selector.RoleSex;
				MyRoleDataMini.Occupation = roleData4Selector.Occupation;
				MyRoleDataMini.SubOccupation = roleData4Selector.SubOccupation;
				MyRoleDataMini.OccupationList = roleData4Selector.OccupationList;
				MyRoleDataMini.Level = roleData4Selector.Level;
				MyRoleDataMini.Faction = roleData4Selector.Faction;
				MyRoleDataMini.MyWingData = roleData4Selector.MyWingData;
				MyRoleDataMini.GoodsDataList = roleData4Selector.GoodsDataList;
				MyRoleDataMini.OtherName = roleData4Selector.OtherName;
				MyRoleDataMini.ZoneID = roleData4Selector.ZoneId;
				MyRoleDataMini.SettingBitFlags = roleData4Selector.SettingBitFlags;
				for (int i = MyRoleDataMini.RoleCommonUseIntPamams.Count; i < 53; i++)
				{
					MyRoleDataMini.RoleCommonUseIntPamams.Add(0);
				}
				MyRoleDataMini.RoleCommonUseIntPamams[26] = roleData4Selector.FashionWingsID;
				MyRoleDataMini.RoleCommonUseIntPamams[40] = roleData4Selector.BuffFashionID;
				MyRoleDataMini.LifeV = (MyRoleDataMini.MaxLifeV = 100);
				MyRoleDataMini.MagicV = (MyRoleDataMini.MaxMagicV = 100);
				MyRoleDataMini.JunTuanId = roleData4Selector.JunTuanId;
				MyRoleDataMini.JunTuanName = roleData4Selector.JunTuanName;
				MyRoleDataMini.JunTuanZhiWu = roleData4Selector.JunTuanZhiWu;
				MyRoleDataMini.LingDi = roleData4Selector.LingDi;
				MyRoleDataMini.HuiJiData = roleData4Selector.HuiJiData;
				MyRoleDataMini.CompType = roleData4Selector.CompType;
				MyRoleDataMini.CompZhiWu = roleData4Selector.CompZhiWu;
			}
			return MyRoleDataMini;
		}

		
		public void CheckSkillDataValid(GameClient client)
		{
			if (client.ClientData.SkillDataList != null)
			{
				lock (client.ClientData.SkillDataList)
				{
					List<SkillData> removeList = new List<SkillData>();
					foreach (SkillData skillData in client.ClientData.SkillDataList)
					{
						SystemXmlItem xmlMagic = null;
						if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillData.SkillID, out xmlMagic) && xmlMagic.GetIntValue("ToOcuupation", -1) != client.ClientData.Occupation)
						{
							removeList.Add(skillData);
						}
					}
					foreach (SkillData skillData in removeList)
					{
						client.ClientData.SkillDataList.Remove(skillData);
					}
				}
			}
		}

		
		public int GetRoleIDByRoleName(string roleName, int serverID)
		{
			int roleid = RoleName2IDs.FindRoleIDByName(roleName, false);
			if (roleid <= 0)
			{
				string[] dbRoleFields = Global.ExecuteDBCmd(10088, roleName, serverID);
				if (dbRoleFields == null || dbRoleFields.Length != 2 || int.Parse(dbRoleFields[0]) < 0)
				{
					return -15;
				}
				roleid = int.Parse(dbRoleFields[0]);
			}
			return roleid;
		}

		
		public const SceneUIClasses _sceneType = SceneUIClasses.ZhengDuo;

		
		public const GameTypes _gameType = GameTypes.ZhengDuo;

		
		private static RoleManager instance = new RoleManager();

		
		private RoleManagerData RuntimeData = new RoleManagerData();
	}
}
