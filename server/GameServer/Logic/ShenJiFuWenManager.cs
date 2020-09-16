using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class ShenJiFuWenManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static ShenJiFuWenManager getInstance()
		{
			return ShenJiFuWenManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1080, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1081, 1, 1, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1082, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		
		public void OnLogin(GameClient client)
		{
			this.RefreshShenJiFuWenProps(client);
		}

		
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

		
		public int GetAllShenJiPointNum(GameClient client)
		{
			return this.GetCostShenJiPointNum(client) + GameManager.ClientMgr.GetShenJiPointValue(client);
		}

		
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

		
		private bool UpdateShenJiFuWenDataDB(GameClient client, int shenjiID, int lev)
		{
			return Global.sendToDB<bool, string>(13095, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, shenjiID, lev), client.ServerId);
		}

		
		private bool ClearShenJiFuWenDataDB(GameClient client)
		{
			return Global.sendToDB<bool, string>(13096, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
		}

		
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

		
		private const string ShenJi_FuWenFileName = "Config/ShenJiFuWen.xml";

		
		private const string ShenJi_PointFileName = "Config/ShenJiDian.xml";

		
		private object ConfigMutex = new object();

		
		protected Dictionary<int, ShenJiFuWenConfigData> ShenJiConfig = null;

		
		protected List<ShenJiPointConfigData> ShenJiPointConfig = null;

		
		protected int WashGoodsID = 0;

		
		protected int WashCostPerOne = 0;

		
		protected int MaxWashCost = 0;

		
		private static ShenJiFuWenManager instance = new ShenJiFuWenManager();
	}
}
