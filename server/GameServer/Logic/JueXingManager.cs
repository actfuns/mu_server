using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020002FF RID: 767
	public class JueXingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x06000C31 RID: 3121 RVA: 0x000BE9AC File Offset: 0x000BCBAC
		public static JueXingManager getInstance()
		{
			return JueXingManager.instance;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x000BE9C4 File Offset: 0x000BCBC4
		public bool initialize()
		{
			this.LoadConfig();
			this.LoadAwakenActivationXml();
			this.LoadAwakenSuitXml();
			this.LoadAwakenLevelXml();
			this.LoadAwakenRecoveryXml();
			return true;
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x000BE9FA File Offset: 0x000BCBFA
		public void LoadConfig()
		{
			this.LoadDefaultXml();
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000BEA04 File Offset: 0x000BCC04
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1886, 1, 1, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1887, 3, 3, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1888, 3, 3, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1889, 1, 1, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1890, 2, 2, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x000BEA90 File Offset: 0x000BCC90
		public bool showdown()
		{
			return true;
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x000BEAA4 File Offset: 0x000BCCA4
		public bool destroy()
		{
			return true;
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000BEAB8 File Offset: 0x000BCCB8
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x000BEACC File Offset: 0x000BCCCC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1886:
					result = this.ProcessGetJueXingDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1887:
					result = this.ProcessJueXingJiHuoCmd(client, nID, bytes, cmdParams);
					break;
				case 1888:
					result = this.ProcessTaoZhuangChangeCmd(client, nID, bytes, cmdParams);
					break;
				case 1889:
					result = this.ProcessMoHuaCmd(client, nID, bytes, cmdParams);
					break;
				case 1890:
					result = this.ProcessHuiShouCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x000BEB94 File Offset: 0x000BCD94
		public bool ProcessGetJueXingDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<JueXingShiData>(nID, client.ClientData.JueXingData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JueXing :: 获取觉醒石数据错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x000BEC3C File Offset: 0x000BCE3C
		public bool ProcessJueXingJiHuoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int suitID = Convert.ToInt32(cmdParams[1]);
				int stoneID = Convert.ToInt32(cmdParams[2]);
				TaoZhuang taoZhuang;
				int result;
				JueXingShiItem jueXingItem;
				if (!this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(suitID, out taoZhuang))
				{
					result = -2;
				}
				else if (!this.JueXingRunTimeData.JueXingShiDict.TryGetValue(stoneID, out jueXingItem))
				{
					result = -2;
				}
				else if (jueXingItem.SuitParent != suitID)
				{
					result = -5;
				}
				else
				{
					JueXingShiData jueXingData = client.ClientData.JueXingData;
					List<TaoZhuangData> jueXingList = jueXingData.TaoZhuangList;
					TaoZhuangData suitData = jueXingList.Find((TaoZhuangData _g) => _g.ID == suitID);
					if (null == suitData)
					{
						suitData = new TaoZhuangData
						{
							ID = suitID,
							ActiviteList = new List<int>()
						};
						jueXingList.Add(suitData);
					}
					if (suitData.ActiviteList.Contains(stoneID))
					{
						result = -1;
					}
					else
					{
						int needGoods = jueXingItem.NeedGoodsID;
						int needNum = jueXingItem.NeedGoodsNum;
						bool usedBinding;
						bool usedTimeLimited;
						if (Global.UseGoodsBindOrNot(client, needGoods, needNum, true, out usedBinding, out usedTimeLimited) < 1)
						{
							result = -3;
						}
						else
						{
							suitData.ActiviteList.Add(stoneID);
							string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, suitID, string.Join<int>(",", suitData.ActiviteList));
							result = Global.sendToDB<int, string>(20318, strcmd, client.ServerId);
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, suitID, stoneID), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JueXing :: 激活觉醒石错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x000BEF14 File Offset: 0x000BD114
		public bool ProcessTaoZhuangChangeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int result = 0;
				int type = Convert.ToInt32(cmdParams[1]);
				int suitID = Convert.ToInt32(cmdParams[2]);
				if (suitID > 0)
				{
					TaoZhuang taoZhuang;
					if (!this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(suitID, out taoZhuang))
					{
						result = -2;
						goto IL_1A0;
					}
					if (taoZhuang.Type != type)
					{
						result = -6;
						goto IL_1A0;
					}
					if (client.ClientData.JueXingData.TaoZhuangList.Find((TaoZhuangData x) => x.ID == suitID) == null)
					{
						result = -7;
						goto IL_1A0;
					}
				}
				if (type == 1)
				{
					client.ClientData.JueXingData.AttackEquip = suitID;
					Global.SaveRoleParamsInt32ValueToDB(client, "10191", suitID, true);
				}
				else if (type == 2)
				{
					client.ClientData.JueXingData.DefenseEquip = suitID;
					Global.SaveRoleParamsInt32ValueToDB(client, "10192", suitID, true);
				}
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				IL_1A0:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, type, suitID), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JueXing :: 更换套装错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x000BF148 File Offset: 0x000BD348
		public bool ProcessMoHuaCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.JueXingRunTimeData.MoHuaOpen)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int result = 0;
				int nextLev = Global.GetRoleParamsInt32FromDB(client, "10193");
				int jueXingJie = client.ClientData.JueXingData.JueXingJie;
				int jueXingJi = client.ClientData.JueXingData.JueXingJi;
				nextLev++;
				AwakenLevelItem nextAwakenLevel;
				if (!this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(nextLev, out nextAwakenLevel))
				{
					result = -8;
				}
				else if (client.ClientData.JueXingZhiChen < (long)nextAwakenLevel.Awakenment)
				{
					result = -9;
				}
				else
				{
					string[] needGoods = nextAwakenLevel.AwakenAdvancedment.Split(new char[]
					{
						','
					});
					if (needGoods.Length > 1)
					{
						int needGoodID = Convert.ToInt32(needGoods[0]);
						int needNum = Convert.ToInt32(needGoods[1]);
						bool usedBinding;
						bool usedTimeLimited;
						if (Global.UseGoodsBindOrNot(client, needGoodID, needNum, true, out usedBinding, out usedTimeLimited) < 1)
						{
							result = -3;
							goto IL_1BB;
						}
					}
					GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, -nextAwakenLevel.Awakenment, "觉醒魔化消耗", true, true, false);
					Global.SaveRoleParamsInt32ValueToDB(client, "10193", nextLev, true);
					client.ClientData.JueXingData.JueXingJie = nextAwakenLevel.Order;
					client.ClientData.JueXingData.JueXingJi = nextAwakenLevel.Star;
					this.UpdataPalyerJueXingAttr(client, true);
				}
				IL_1BB:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, client.ClientData.JueXingData.JueXingJie, client.ClientData.JueXingData.JueXingJi), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JueXing :: 觉醒魔化错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x000BF3B0 File Offset: 0x000BD5B0
		public bool ProcessHuiShouCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.JueXingRunTimeData.MoHuaOpen)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int result = 0;
				foreach (string one in cmdParams[1].Split(new char[]
				{
					'|'
				}))
				{
					string[] oneStr = one.Split(new char[]
					{
						','
					});
					if (oneStr.Length < 3)
					{
						break;
					}
					int goodsID = Convert.ToInt32(oneStr[0]);
					int goodsNum = Convert.ToInt32(oneStr[1]);
					int binding = Convert.ToInt32(oneStr[2]);
					int addVal = 0;
					if (!this.JueXingRunTimeData.AwakenRecoveryDict.TryGetValue(goodsID, out addVal))
					{
						result = -2;
						break;
					}
					addVal *= goodsNum;
					if (binding > 0)
					{
						if (Global.GetTotalBindGoodsCountByID(client, goodsID) < goodsNum)
						{
							result = -10;
							break;
						}
						bool useBinding;
						bool useTimeLimit;
						if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsNum, false, out useBinding, out useTimeLimit, false))
						{
							result = -10;
							break;
						}
					}
					else if (binding < 1)
					{
						if (Global.GetTotalNotBindGoodsCountByID(client, goodsID) < goodsNum)
						{
							result = -10;
							break;
						}
						bool useBinding;
						bool useTimeLimit;
						if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsNum, false, out useBinding, out useTimeLimit, false))
						{
							result = -10;
							break;
						}
					}
					GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, addVal, "碎片分解增加觉醒之尘", true, true, false);
				}
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JueXing :: 觉醒魔化错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000BF654 File Offset: 0x000BD854
		public void UpdataPalyerJueXingAttr(GameClient client, bool hint = true)
		{
			if (this.IsGongNengOpen(client, false))
			{
				double[] _ExtProps = new double[177];
				if (null != client.ClientData.JueXingData)
				{
					foreach (TaoZhuangData item in client.ClientData.JueXingData.TaoZhuangList)
					{
						foreach (int stoneID in item.ActiviteList)
						{
							if (stoneID > 0)
							{
								JueXingShiItem stone;
								if (this.JueXingRunTimeData.JueXingShiDict.TryGetValue(stoneID, out stone))
								{
									if (this.CanAddAttribute(client, stone.Position))
									{
										for (int i = 0; i < 177; i++)
										{
											_ExtProps[i] += stone.ExtProps[i];
										}
									}
								}
							}
						}
					}
					AwakenLevelItem level;
					if (this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(Global.GetRoleParamsInt32FromDB(client, "10193"), out level))
					{
						for (int j = 0; j < 177; j++)
						{
							_ExtProps[j] *= 1.0 + level.EnlargeRate / 100.0;
							_ExtProps[j] += level.ExtProps[j];
						}
					}
					client.PassiveEffectList.Clear();
					List<PassiveSkillData> passiveSkillList = new List<PassiveSkillData>();
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.WeaponMaster,
						new double[177]
					});
					this.UpdateTaoZhuangAttr(client, client.ClientData.JueXingData.GetAttackTaoZhuang(), ref passiveSkillList, ref _ExtProps);
					this.UpdateTaoZhuangAttr(client, client.ClientData.JueXingData.GetDefenseTaoZhuang(), ref passiveSkillList, ref _ExtProps);
					client.passiveSkillModule.UpdateOtherSkillList(passiveSkillList);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JueXingShi,
						_ExtProps
					});
					if (hint)
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x000BF9A0 File Offset: 0x000BDBA0
		public void UpdateTaoZhuangAttr(GameClient client, TaoZhuangData taoZhuangEquip, ref List<PassiveSkillData> passiveSkillList, ref double[] _ExtProps)
		{
			if (null != taoZhuangEquip)
			{
				TaoZhuang taoZhuang;
				if (this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(taoZhuangEquip.ID, out taoZhuang))
				{
					int canActiveCount = taoZhuangEquip.ActiviteList.FindAll(delegate(int _x)
					{
						JueXingShiItem stone;
						return this.JueXingRunTimeData.JueXingShiDict.TryGetValue(_x, out stone) && this.CanAddAttribute(client, stone.Position);
					}).Count;
					if (canActiveCount >= taoZhuang.TaoZhuangProps3Num && taoZhuang.TaoZhuangProps3Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps3[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps2[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					else if (canActiveCount >= taoZhuang.TaoZhuangProps2Num && taoZhuang.TaoZhuangProps2Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps2[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					else if (canActiveCount >= taoZhuang.TaoZhuangProps1Num && taoZhuang.TaoZhuangProps1Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					if (taoZhuang.Type == 1)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.WeaponMaster,
							new double[177]
						});
						if (canActiveCount >= taoZhuang.WeaponMasterNum)
						{
							WeaponMaster.UpdateRoleAttr(client, taoZhuang.WeaponMasterType, false);
						}
					}
					foreach (List<int> skillItem in taoZhuang.PassiveSkill)
					{
						if (skillItem.Count > 1 && canActiveCount >= skillItem[0])
						{
							for (int i = 1; i < skillItem.Count; i++)
							{
								SystemXmlItem systemMagic = null;
								if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillItem[i], out systemMagic))
								{
									passiveSkillList.Add(new PassiveSkillData
									{
										skillId = skillItem[i],
										triggerRate = (int)(systemMagic.GetDoubleValue("TriggerOdds") * 100.0),
										triggerType = systemMagic.GetIntValue("TriggerType", -1),
										coolDown = systemMagic.GetIntValue("CDTime", -1),
										triggerCD = systemMagic.GetIntValue("TriggerCD", -1)
									});
								}
							}
						}
					}
					foreach (List<int> item in taoZhuang.PassiveEffect)
					{
						if (item.Count > 1 && canActiveCount >= item[0])
						{
							for (int i = 1; i < item.Count; i++)
							{
								client.PassiveEffectList.Add(item[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x000BFDC4 File Offset: 0x000BDFC4
		public void LoadDefaultXml()
		{
			try
			{
				lock (this.JueXingRunTimeData.Mutex)
				{
					this.JueXingRunTimeData.MoHuaOpen = (1L == GameManager.systemParamsList.GetParamValueIntByName("AwakenLevelUpOpen", -1));
					int[] AwakenCondition = GameManager.systemParamsList.GetParamValueIntArrayByName("AwakenCondition", ',');
					if (AwakenCondition != null && AwakenCondition.Length == 2)
					{
						this.JueXingRunTimeData.SuitIDLimit = AwakenCondition[0];
						this.JueXingRunTimeData.ExcellencePropLimit = AwakenCondition[1];
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x000BFEC8 File Offset: 0x000BE0C8
		public void LoadAwakenActivationXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(JueXingConsts.AwakenActivation);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, JueXingShiItem> jueXingShiDict = new Dictionary<int, JueXingShiItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							int[] material = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "Material", "").Split(new char[]
							{
								','
							}), (string x) => Convert.ToInt32(x));
							if (material.Length < 2)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("加载xml配置文件:{0}, 错误。", fileName), null, true);
							}
							else
							{
								string tempValueString = Global.GetSafeAttributeStr(xmlItem, "BaseProps");
								string[] valueFileds = tempValueString.Split(new char[]
								{
									'|'
								});
								double[] extProps = new double[177];
								foreach (string value in valueFileds)
								{
									string[] KvpFileds = value.Split(new char[]
									{
										','
									});
									if (KvpFileds.Length == 2)
									{
										ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
										if (index < ExtPropIndexes.Max)
										{
											extProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
										}
									}
								}
								jueXingShiDict[id] = new JueXingShiItem
								{
									ID = id,
									Position = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Position", "0")),
									NeedGoodsID = material[0],
									NeedGoodsNum = material[1],
									ExtProps = extProps
								};
								lock (this.JueXingRunTimeData.Mutex)
								{
									this.JueXingRunTimeData.JueXingShiDict = jueXingShiDict;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x000C01F0 File Offset: 0x000BE3F0
		public void LoadAwakenSuitXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(JueXingConsts.AwakenSuit);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, TaoZhuang> taoZhuangDict = new Dictionary<int, TaoZhuang>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							string tempValueString = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangProps1");
							string[] valueFileds = tempValueString.Split(new char[]
							{
								'|'
							});
							double[] extProps = new double[177];
							foreach (string value in valueFileds)
							{
								string[] KvpFileds = value.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
									if (index < ExtPropIndexes.Max)
									{
										extProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
									}
								}
							}
							tempValueString = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangProps2");
							valueFileds = tempValueString.Split(new char[]
							{
								'|'
							});
							double[] extProps2 = new double[177];
							foreach (string value in valueFileds)
							{
								string[] KvpFileds = value.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
									if (index < ExtPropIndexes.Max)
									{
										extProps2[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
									}
								}
							}
							tempValueString = Global.GetSafeAttributeStr(xmlItem, "TaoZhuangProps3");
							valueFileds = tempValueString.Split(new char[]
							{
								'|'
							});
							double[] extProps3 = new double[177];
							foreach (string value in valueFileds)
							{
								string[] KvpFileds = value.Split(new char[]
								{
									','
								});
								if (KvpFileds.Length == 2)
								{
									ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
									if (index < ExtPropIndexes.Max)
									{
										extProps3[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
									}
								}
							}
							string[] weaponStr = Global.GetDefAttributeStr(xmlItem, "WeaponMaster", "").Split(new char[]
							{
								','
							});
							int weaponNum = 0;
							int weaponType = 0;
							if (weaponStr.Length > 1)
							{
								weaponNum = Convert.ToInt32(weaponStr[0]);
								weaponType = Convert.ToInt32(weaponStr[1]);
							}
							List<List<int>> passiveSkill = new List<List<int>>();
							string[] passiveSkillList = Global.GetDefAttributeStr(xmlItem, "Magic", "").Split(new char[]
							{
								'|'
							});
							foreach (string one in passiveSkillList)
							{
								if (!string.IsNullOrEmpty(one))
								{
									string[] skillItem = one.Split(new char[]
									{
										','
									});
									if (skillItem.Length > 1)
									{
										passiveSkill.Add(Array.ConvertAll<string, int>(skillItem, (string x) => Convert.ToInt32(x)).ToList<int>());
									}
								}
							}
							List<List<int>> passiveEffect = new List<List<int>>();
							string[] passiveEffectList = Global.GetDefAttributeStr(xmlItem, "PassiveEffect", "").Split(new char[]
							{
								'|'
							});
							foreach (string one in passiveEffectList)
							{
								if (!string.IsNullOrEmpty(one))
								{
									string[] passiveItem = one.Split(new char[]
									{
										','
									});
									if (passiveItem.Length > 1)
									{
										passiveEffect.Add(Array.ConvertAll<string, int>(passiveItem, (string x) => Convert.ToInt32(x)).ToList<int>());
									}
								}
							}
							Dictionary<int, TaoZhuang> dictionary = taoZhuangDict;
							int key = id;
							TaoZhuang taoZhuang = new TaoZhuang();
							taoZhuang.ID = id;
							taoZhuang.Type = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Type", "0"));
							taoZhuang.AwakenList = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xmlItem, "AwakenID", "").Split(new char[]
							{
								','
							}), (string x) => Convert.ToInt32(x)).ToList<int>();
							taoZhuang.TaoZhuangProps1Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "TaoZhuangProps1Num", "0"));
							taoZhuang.TaoZhuangProps1 = extProps;
							taoZhuang.TaoZhuangProps2Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "TaoZhuangProps2Num", "0"));
							taoZhuang.TaoZhuangProps2 = extProps2;
							taoZhuang.TaoZhuangProps3Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xmlItem, "TaoZhuangProps3Num", "0"));
							taoZhuang.TaoZhuangProps3 = extProps3;
							taoZhuang.WeaponMasterNum = weaponNum;
							taoZhuang.WeaponMasterType = weaponType;
							taoZhuang.PassiveSkill = passiveSkill;
							taoZhuang.PassiveEffect = passiveEffect;
							dictionary[key] = taoZhuang;
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.TaoZhuangDict = taoZhuangDict;
						foreach (TaoZhuang item in taoZhuangDict.Values)
						{
							foreach (int id in item.AwakenList)
							{
								if (this.JueXingRunTimeData.JueXingShiDict.ContainsKey(id))
								{
									this.JueXingRunTimeData.JueXingShiDict[id].SuitParent = item.ID;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000C43 RID: 3139 RVA: 0x000C0950 File Offset: 0x000BEB50
		public void LoadAwakenLevelXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(JueXingConsts.AwakenLevel);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, AwakenLevelItem> awakenLevelDict = new Dictionary<int, AwakenLevelItem>();
					IEnumerable<XElement> nodes = xml.Elements();
					double[] extProps = new double[177];
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int id = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
							string tempValueString = Global.GetSafeAttributeStr(xmlItem, "AdvancedEffect");
							string[] valueFileds = tempValueString.Split(new char[]
							{
								'|'
							});
							if (valueFileds.Length > 0)
							{
								foreach (string value in valueFileds)
								{
									string[] KvpFileds = value.Split(new char[]
									{
										','
									});
									if (KvpFileds.Length == 2)
									{
										ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
										if (index < ExtPropIndexes.Max)
										{
											extProps[(int)index] += Global.SafeConvertToDouble(KvpFileds[1]);
										}
									}
								}
							}
							awakenLevelDict[id] = new AwakenLevelItem
							{
								ID = id,
								Order = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Order", "0")),
								Star = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Star", "0")),
								Awakenment = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Awakenment", "0")),
								AwakenAdvancedment = Global.GetDefAttributeStr(xmlItem, "AwakenAdvancedment", "0"),
								EnlargeRate = Convert.ToDouble(Global.GetDefAttributeStr(xmlItem, "EnlargeRate", "0")),
								ExtProps = (double[])extProps.Clone()
							};
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.AwakenLevelDict = awakenLevelDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000C44 RID: 3140 RVA: 0x000C0C44 File Offset: 0x000BEE44
		public void LoadAwakenRecoveryXml()
		{
			string fileName = "";
			try
			{
				fileName = Global.GameResPath(JueXingConsts.AwakenRecovery);
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					Dictionary<int, int> awakenRecoveryDict = new Dictionary<int, int>();
					IEnumerable<XElement> nodes = xml.Elements();
					foreach (XElement xmlItem in nodes)
					{
						if (xmlItem != null)
						{
							int goodsID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GoodsID", "0"));
							awakenRecoveryDict[goodsID] = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "AwakenNum", "0"));
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.AwakenRecoveryDict = awakenRecoveryDict;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
			}
		}

		// Token: 0x06000C45 RID: 3141 RVA: 0x000C0D98 File Offset: 0x000BEF98
		public void InitRoleJueXingData(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.JueXingData)
				{
					List<TaoZhuangData> taoZhuangList = Global.sendToDB<List<TaoZhuangData>, string>(20317, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (null == taoZhuangList)
					{
						taoZhuangList = new List<TaoZhuangData>();
					}
					int level = Global.GetRoleParamsInt32FromDB(client, "10193");
					int jueXingJie = 1;
					int jueXingJi = 0;
					AwakenLevelItem awakenLevel;
					if (this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(level, out awakenLevel))
					{
						jueXingJie = awakenLevel.Order;
						jueXingJi = awakenLevel.Star;
					}
					client.ClientData.JueXingData = new JueXingShiData
					{
						AttackEquip = Global.GetRoleParamsInt32FromDB(client, "10191"),
						DefenseEquip = Global.GetRoleParamsInt32FromDB(client, "10192"),
						TaoZhuangList = taoZhuangList,
						JueXingJie = jueXingJie,
						JueXingJi = jueXingJi
					};
				}
				this.UpdataPalyerJueXingAttr(client, true);
			}
		}

		// Token: 0x06000C46 RID: 3142 RVA: 0x000C0EB0 File Offset: 0x000BF0B0
		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot7) && GlobalNew.IsGongNengOpened(client, GongNengIDs.JueXing, hint);
		}

		// Token: 0x06000C47 RID: 3143 RVA: 0x000C0FBC File Offset: 0x000BF1BC
		public bool CanAddAttribute(GameClient client, int position)
		{
			int begin = 0;
			int end = 0;
			int bagIndex = 3;
			switch (position)
			{
			case 1:
				bagIndex = 2;
				begin = 11;
				end = 21;
				break;
			case 2:
				begin = (end = 5);
				break;
			case 3:
				bagIndex = 0;
				begin = (end = 6);
				break;
			case 4:
				bagIndex = 1;
				begin = (end = 6);
				break;
			case 5:
				begin = (end = 0);
				break;
			case 6:
				begin = (end = 1);
				break;
			case 7:
				begin = (end = 2);
				break;
			case 8:
				begin = (end = 3);
				break;
			case 9:
				begin = (end = 4);
				break;
			default:
				return false;
			}
			return client.ClientData.GoodsDataList.Find(delegate(GoodsData _g)
			{
				bool result;
				if (_g.Using != 1)
				{
					result = false;
				}
				else if (bagIndex < 2 && _g.BagIndex != bagIndex)
				{
					result = false;
				}
				else if (Global.GetEquipExcellencePropNum(_g) < this.JueXingRunTimeData.ExcellencePropLimit)
				{
					result = false;
				}
				else
				{
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(_g.GoodsID, out systemGoods))
					{
						result = false;
					}
					else
					{
						int cateGoriy = systemGoods.GetIntValue("Categoriy", -1);
						result = (cateGoriy <= end && cateGoriy >= begin && systemGoods.GetIntValue("SuitID", -1) >= this.JueXingRunTimeData.SuitIDLimit);
					}
				}
				return result;
			}) != null;
		}

		// Token: 0x040013CF RID: 5071
		public JueXingRunData JueXingRunTimeData = new JueXingRunData();

		// Token: 0x040013D0 RID: 5072
		private static JueXingManager instance = new JueXingManager();
	}
}
