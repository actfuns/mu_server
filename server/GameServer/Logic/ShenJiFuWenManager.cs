using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x02000403 RID: 1027
	public class ShenJiFuWenManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x06001216 RID: 4630 RVA: 0x0011F470 File Offset: 0x0011D670
		public static ShenJiFuWenManager getInstance()
		{
			return ShenJiFuWenManager.instance;
		}

		// Token: 0x06001217 RID: 4631 RVA: 0x0011F488 File Offset: 0x0011D688
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x06001218 RID: 4632 RVA: 0x0011F4AC File Offset: 0x0011D6AC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1080, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1081, 1, 1, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1082, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x06001219 RID: 4633 RVA: 0x0011F508 File Offset: 0x0011D708
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600121A RID: 4634 RVA: 0x0011F51C File Offset: 0x0011D71C
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0011F530 File Offset: 0x0011D730
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x0011F544 File Offset: 0x0011D744
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Building, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1080:
					result = this.ProcessShenJiAddEffectCmd(client, nID, bytes, cmdParams);
					break;
				case 1081:
					result = this.ProcessShenJiAddExpCmd(client, nID, bytes, cmdParams);
					break;
				case 1082:
					result = this.ProcessShenJiWashCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0011F5EB File Offset: 0x0011D7EB
		public void OnLogin(GameClient client)
		{
			this.RefreshShenJiFuWenProps(client);
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x0011F5F8 File Offset: 0x0011D7F8
		private void RefreshShenJiFuWenProps(GameClient client)
		{
			Dictionary<int, ShenJiFuWenConfigData> tempShenJiConfig = null;
			lock (this.ConfigMutex)
			{
				tempShenJiConfig = this.ShenJiConfig;
			}
			double[] _ExtProps = new double[177];
			foreach (ShenJiFuWenData item in client.ClientData.ShenJiDataDict.Values)
			{
				ShenJiFuWenConfigData sConfigData = null;
				if (tempShenJiConfig.TryGetValue(item.ShenJiID, out sConfigData))
				{
					ShenJiFuWenEffectData effect = sConfigData.GetEffect(item.Level);
					for (int i = 0; i < _ExtProps.Length; i++)
					{
						_ExtProps[i] += effect.ExtProps[i];
					}
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.ShenJiFuWen,
				_ExtProps
			});
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				DelayExecProcIds.RecalcProps,
				DelayExecProcIds.NotifyRefreshProps
			});
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x0011F74C File Offset: 0x0011D94C
		private ShenJiPointConfigData GetShenJiPointConfigInfo(GameClient client)
		{
			int CostPoints = this.GetCostShenJiPointNum(client);
			int CurPoints = GameManager.ClientMgr.GetShenJiPointValue(client);
			List<ShenJiPointConfigData> tempShenJiPointConfig = null;
			lock (this.ConfigMutex)
			{
				tempShenJiPointConfig = this.ShenJiPointConfig;
			}
			ShenJiPointConfigData data;
			if (CostPoints + CurPoints >= tempShenJiPointConfig.Count - 1)
			{
				data = null;
			}
			else
			{
				data = tempShenJiPointConfig[CostPoints + CurPoints];
			}
			return data;
		}

		// Token: 0x06001220 RID: 4640 RVA: 0x0011F7E4 File Offset: 0x0011D9E4
		private int GetCostShenJiPointNum(GameClient client)
		{
			Dictionary<int, ShenJiFuWenConfigData> tempShenJiConfig = null;
			lock (this.ConfigMutex)
			{
				tempShenJiConfig = this.ShenJiConfig;
			}
			int TotalCostNum = 0;
			foreach (ShenJiFuWenData item in client.ClientData.ShenJiDataDict.Values)
			{
				ShenJiFuWenConfigData sConfigData = null;
				if (tempShenJiConfig.TryGetValue(item.ShenJiID, out sConfigData))
				{
					TotalCostNum += sConfigData.UpNeed * item.Level;
				}
			}
			return TotalCostNum;
		}

		// Token: 0x06001221 RID: 4641 RVA: 0x0011F8B8 File Offset: 0x0011DAB8
		public int GetAllShenJiPointNum(GameClient client)
		{
			return this.GetCostShenJiPointNum(client) + GameManager.ClientMgr.GetShenJiPointValue(client);
		}

		// Token: 0x06001222 RID: 4642 RVA: 0x0011F8E0 File Offset: 0x0011DAE0
		private ShenJiFuWenData GetShenJiFuWenData(GameClient client, int shenjiID)
		{
			ShenJiFuWenData result;
			if (null == client.ClientData.ShenJiDataDict)
			{
				result = null;
			}
			else
			{
				ShenJiFuWenData data = null;
				if (!client.ClientData.ShenJiDataDict.TryGetValue(shenjiID, out data))
				{
					result = null;
				}
				else
				{
					result = data;
				}
			}
			return result;
		}

		// Token: 0x06001223 RID: 4643 RVA: 0x0011F92C File Offset: 0x0011DB2C
		private bool UpdateShenJiFuWenDataDB(GameClient client, int shenjiID, int lev)
		{
			return Global.sendToDB<bool, string>(13095, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, shenjiID, lev), client.ServerId);
		}

		// Token: 0x06001224 RID: 4644 RVA: 0x0011F974 File Offset: 0x0011DB74
		private bool ClearShenJiFuWenDataDB(GameClient client)
		{
			return Global.sendToDB<bool, string>(13096, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
		}

		// Token: 0x06001225 RID: 4645 RVA: 0x0011F9B0 File Offset: 0x0011DBB0
		public bool ProcessShenJiAddEffectCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				int shenjiID = Convert.ToInt32(cmdParams[1]);
				Dictionary<int, ShenJiFuWenConfigData> tempShenJiConfig = null;
				lock (this.ConfigMutex)
				{
					tempShenJiConfig = this.ShenJiConfig;
				}
				ShenJiFuWenConfigData sConfigData = null;
				if (!tempShenJiConfig.TryGetValue(shenjiID, out sConfigData))
				{
					result = 1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						shenjiID,
						0
					}), false);
					return true;
				}
				ShenJiFuWenData actData = this.GetShenJiFuWenData(client, shenjiID);
				if (actData != null && actData.Level >= sConfigData.MaxLevel)
				{
					result = 2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						shenjiID,
						0
					}), false);
					return true;
				}
				if (sConfigData.UpNeed > GameManager.ClientMgr.GetShenJiPointValue(client))
				{
					result = 2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						result,
						roleID,
						shenjiID,
						0
					}), false);
					return true;
				}
				ShenJiFuWenConfigData preConfigData = null;
				if (tempShenJiConfig.TryGetValue(sConfigData.PreShenJiID, out preConfigData))
				{
					ShenJiFuWenData preData = this.GetShenJiFuWenData(client, sConfigData.PreShenJiID);
					if (preData == null || sConfigData.PreShenJiLev > preData.Level)
					{
						result = 2;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							result,
							roleID,
							shenjiID,
							0
						}), false);
						return true;
					}
				}
				if (null == actData)
				{
					actData = new ShenJiFuWenData
					{
						ShenJiID = shenjiID
					};
					client.ClientData.ShenJiDataDict[shenjiID] = actData;
				}
				GameManager.ClientMgr.ModifyShenJiPointValue(client, -sConfigData.UpNeed, "精灵神迹升级|激活", true, true);
				actData.Level++;
				this.UpdateShenJiFuWenDataDB(client, shenjiID, actData.Level);
				this.RefreshShenJiFuWenProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					result,
					roleID,
					shenjiID,
					actData.Level
				}), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001226 RID: 4646 RVA: 0x0011FD14 File Offset: 0x0011DF14
		public bool ProcessShenJiAddExpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				if (GameManager.ClientMgr.GetShenJiJiFenValue(client) <= 0)
				{
					result = 4;
					client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
					return true;
				}
				ShenJiPointConfigData PointConfig = this.GetShenJiPointConfigInfo(client);
				if (null == PointConfig)
				{
					result = 6;
					client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
					return true;
				}
				if (Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level) < PointConfig.NeedLevel)
				{
					result = 7;
					client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
					return true;
				}
				int CurJiFen = GameManager.ClientMgr.GetShenJiJiFenValue(client);
				int JiFenAdd = GameManager.ClientMgr.GetShenJiJiFenAddValue(client);
				int CurNeedJiFen = PointConfig.NeedJiFen - JiFenAdd;
				if (CurJiFen >= CurNeedJiFen)
				{
					GameManager.ClientMgr.ModifyShenJiJiFenValue(client, -CurNeedJiFen, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiJiFenAddValue(client, -JiFenAdd, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiPointValue(client, 1, "精灵神迹积分注入", true, true);
				}
				else
				{
					GameManager.ClientMgr.ModifyShenJiJiFenValue(client, -CurJiFen, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiJiFenAddValue(client, CurJiFen, "精灵神迹积分注入", true, true);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001227 RID: 4647 RVA: 0x0011FF04 File Offset: 0x0011E104
		public bool ProcessShenJiWashCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int result = 0;
				int roleID = Convert.ToInt32(cmdParams[0]);
				ShenJiWashType washType = (ShenJiWashType)Convert.ToInt32(cmdParams[1]);
				int CostPoints = this.GetCostShenJiPointNum(client);
				if (CostPoints <= 0)
				{
					result = 1;
					client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
					return true;
				}
				if (washType == ShenJiWashType.SJWT_UseZuanShi)
				{
					int needDiamond = Math.Min(CostPoints * this.WashCostPerOne, this.MaxWashCost);
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needDiamond, "精灵神迹洗点", true, true, false, DaiBiSySType.None))
					{
						result = 3;
						client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
						return true;
					}
				}
				else
				{
					GoodsData goodsData = Global.GetGoodsByID(client, this.WashGoodsID);
					if (goodsData == null)
					{
						result = 5;
						client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
						return true;
					}
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, false))
					{
						result = 5;
						client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
						return true;
					}
				}
				GameManager.ClientMgr.ModifyShenJiPointValue(client, CostPoints, "精灵神迹点重置", true, true);
				this.ClearShenJiFuWenDataDB(client);
				client.ClientData.ShenJiDataDict.Clear();
				this.RefreshShenJiFuWenProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}", result, roleID), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001228 RID: 4648 RVA: 0x00120134 File Offset: 0x0011E334
		public bool InitConfig()
		{
			string StringShenJiFuWen = GameManager.systemParamsList.GetParamValueByName("ResettingShenJiFuWen");
			if (!string.IsNullOrEmpty(StringShenJiFuWen))
			{
				string[] Field = StringShenJiFuWen.Split(new char[]
				{
					','
				});
				if (Field.Length == 3)
				{
					this.WashGoodsID = Global.SafeConvertToInt32(Field[0]);
					this.WashCostPerOne = Global.SafeConvertToInt32(Field[1]);
					this.MaxWashCost = Global.SafeConvertToInt32(Field[2]);
				}
			}
			return this.LoadShenJiFuWenConfigFile() && this.LoadShenJiPointConfigFile();
		}

		// Token: 0x06001229 RID: 4649 RVA: 0x001201D0 File Offset: 0x0011E3D0
		private ShenJiFuWenEffectData ParseShenJiFuWenEffectData(XElement xmlItem, string Key)
		{
			string TempValueString = Global.GetSafeAttributeStr(xmlItem, string.Format("Effect{0}", Key));
			string[] ValueFileds = TempValueString.Split(new char[]
			{
				'|'
			});
			ShenJiFuWenEffectData result;
			if (ValueFileds.Length == 0)
			{
				result = null;
			}
			else
			{
				ShenJiFuWenEffectData data = new ShenJiFuWenEffectData();
				foreach (string value in ValueFileds)
				{
					string[] KvpFileds = value.Split(new char[]
					{
						','
					});
					if (KvpFileds.Length == 2)
					{
						ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
						if (index != ExtPropIndexes.Max)
						{
							data.ExtProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
						}
					}
				}
				result = data;
			}
			return result;
		}

		// Token: 0x0600122A RID: 4650 RVA: 0x001202A8 File Offset: 0x0011E4A8
		public bool LoadShenJiFuWenConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ShenJiFuWen.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ShenJiFuWen.xml"));
				if (null == xml)
				{
					return false;
				}
				Dictionary<int, ShenJiFuWenConfigData> tempShenJiConfig = new Dictionary<int, ShenJiFuWenConfigData>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					ShenJiFuWenConfigData data = new ShenJiFuWenConfigData();
					data.ShenJiID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					data.PreShenJiID = (int)Global.GetSafeAttributeLong(xmlItem, "Prev");
					data.PreShenJiLev = (int)Global.GetSafeAttributeLong(xmlItem, "PrevLevel");
					data.MaxLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MaxLevel");
					data.UpNeed = (int)Global.GetSafeAttributeLong(xmlItem, "UpNeed");
					for (int levelloop = 1; levelloop <= 5; levelloop++)
					{
						ShenJiFuWenEffectData effectItem = this.ParseShenJiFuWenEffectData(xmlItem, levelloop.ToString());
						if (null == effectItem)
						{
							break;
						}
						data.ShenJiEffectList.Add(effectItem);
					}
					tempShenJiConfig[data.ShenJiID] = data;
				}
				lock (this.ConfigMutex)
				{
					this.ShenJiConfig = tempShenJiConfig;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/ShenJiFuWen.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x0600122B RID: 4651 RVA: 0x001204B0 File Offset: 0x0011E6B0
		public bool LoadShenJiPointConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ShenJiDian.xml"));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ShenJiDian.xml"));
				if (null == xml)
				{
					return false;
				}
				List<ShenJiPointConfigData> tempShenJiPointConfig = new List<ShenJiPointConfigData>();
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					ShenJiPointConfigData data = new ShenJiPointConfigData();
					data.ShenJiPoint = (int)Global.GetSafeAttributeLong(xmlItem, "ShenJiDian");
					data.NeedJiFen = (int)Global.GetSafeAttributeLong(xmlItem, "NeedShenJi");
					string tempValue = Global.GetSafeAttributeStr(xmlItem, "NeedLevel");
					string[] ValueFileds = tempValue.Split(new char[]
					{
						'|'
					});
					if (ValueFileds.Length == 2)
					{
						int zhuanSheng = Global.SafeConvertToInt32(ValueFileds[0]);
						int level = Global.SafeConvertToInt32(ValueFileds[1]);
						data.NeedLevel = Global.GetUnionLevel2(zhuanSheng, level);
					}
					tempShenJiPointConfig.Add(data);
				}
				lock (this.ConfigMutex)
				{
					this.ShenJiPointConfig = tempShenJiPointConfig;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/ShenJiDian.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x04001B52 RID: 6994
		private const string ShenJi_FuWenFileName = "Config/ShenJiFuWen.xml";

		// Token: 0x04001B53 RID: 6995
		private const string ShenJi_PointFileName = "Config/ShenJiDian.xml";

		// Token: 0x04001B54 RID: 6996
		private object ConfigMutex = new object();

		// Token: 0x04001B55 RID: 6997
		protected Dictionary<int, ShenJiFuWenConfigData> ShenJiConfig = null;

		// Token: 0x04001B56 RID: 6998
		protected List<ShenJiPointConfigData> ShenJiPointConfig = null;

		// Token: 0x04001B57 RID: 6999
		protected int WashGoodsID = 0;

		// Token: 0x04001B58 RID: 7000
		protected int WashCostPerOne = 0;

		// Token: 0x04001B59 RID: 7001
		protected int MaxWashCost = 0;

		// Token: 0x04001B5A RID: 7002
		private static ShenJiFuWenManager instance = new ShenJiFuWenManager();
	}
}
