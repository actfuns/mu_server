using System;
using System.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class JingLingYuanSuJueXingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		public static JingLingYuanSuJueXingManager getInstance()
		{
			return JingLingYuanSuJueXingManager.instance;
		}

		
		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		
		public void LoadConfig()
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					string fileName = "Config/JingLingYuanSu.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.YuanSuInfoDict.Load(fullPathFileName, null);
					foreach (JingLingYuanSuInfo item in this.RuntimeData.YuanSuInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(item.ExtProps, item.Attribute, "|,");
					}
					fileName = "Config/JingLingYuanSuShuXing.xml";
					fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.ShuXingInfoDict.Load(fullPathFileName, null);
					foreach (JingLingYuanSuShuXingInfo item2 in this.RuntimeData.ShuXingInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(item2.ExtProps, item2.AcetiveElement, "|,");
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1450, 2, 2, JingLingYuanSuJueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1451, 4, 4, JingLingYuanSuJueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1450:
					result = this.ProcessJingLingYuanSuJueXingActiveCmd(client, nID, bytes, cmdParams);
					break;
				case 1451:
					result = this.ProcessJingLingYuanSuJueXingUpgradeCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		
		public bool ProcessJingLingYuanSuJueXingActiveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int result = 0;
				int activeType = Convert.ToInt32(cmdParams[1]);
				JingLingYuanSuJueXingData jueXingData = client.ClientData.JingLingYuanSuJueXingData;
				if (jueXingData == null || jueXingData.ActiveIDs == null)
				{
					jueXingData = new JingLingYuanSuJueXingData();
					jueXingData.ActiveIDs = new int[6];
					client.ClientData.JingLingYuanSuJueXingData = jueXingData;
				}
				if (jueXingData != null && null != jueXingData.ActiveIDs)
				{
					jueXingData.ActiveType = activeType;
					result = Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jueXingData), client.ServerId);
					JingLingYuanSuJueXingManager.getInstance().RefreshProps(client, false);
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						DelayExecProcIds.NotifyRefreshProps
					});
				}
				client.sendCmd(nID, string.Format("{0}:{1}", result, activeType), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JingLingYuanSuJueXing :: 获取觉醒石数据错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public void GMSetJingLingYuanSuJueXingData(GameClient client, string[] cmdFields)
		{
			JingLingYuanSuJueXingData jueXingData = client.ClientData.JingLingYuanSuJueXingData;
			if (jueXingData == null || jueXingData.ActiveIDs == null)
			{
				jueXingData = new JingLingYuanSuJueXingData();
				jueXingData.ActiveIDs = new int[6];
				client.ClientData.JingLingYuanSuJueXingData = jueXingData;
			}
			int activeType = Global.SafeConvertToInt32(cmdFields[2]);
			jueXingData.ActiveType = activeType;
			for (int i = 3; i < cmdFields.Length; i++)
			{
				jueXingData.ActiveIDs[i - 3] = Global.SafeConvertToInt32(cmdFields[i]);
			}
			Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jueXingData), client.ServerId);
			this.RefreshProps(client, false);
		}

		
		public bool ProcessJingLingYuanSuJueXingUpgradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 4))
				{
					return false;
				}
				int result = 0;
				int yuanSuType = Convert.ToInt32(cmdParams[1]);
				int shuXingType = Convert.ToInt32(cmdParams[2]);
				int useShenYou = Convert.ToInt32(cmdParams[3]);
				int newID = 0;
				JingLingYuanSuJueXingData jueXingData = client.ClientData.JingLingYuanSuJueXingData;
				if (jueXingData == null || jueXingData.ActiveIDs == null)
				{
					jueXingData = new JingLingYuanSuJueXingData();
					jueXingData.ActiveIDs = new int[6];
				}
				int idx = (yuanSuType - 1) * 2 + shuXingType - 1;
				if (idx < 0 || idx >= jueXingData.ActiveIDs.Length)
				{
					result = -18;
				}
				else
				{
					int currentLevel = 0;
					int currentID = jueXingData.ActiveIDs[idx];
					JingLingYuanSuInfo currentLevelInfo = null;
					JingLingYuanSuInfo nextLevelInfo = null;
					JingLingYuanSuInfo preLevelInfo = null;
					lock (this.RuntimeData.Mutex)
					{
						if (currentID > 0)
						{
							if (this.RuntimeData.YuanSuInfoDict.Value.TryGetValue(currentID, out currentLevelInfo))
							{
								if (currentLevelInfo.YuanSuType != yuanSuType || currentLevelInfo.ShuXingType != shuXingType)
								{
									result = -3;
									goto IL_4AB;
								}
								currentLevel = currentLevelInfo.QiangHuaLevel;
							}
						}
						int nextLevel = currentLevel + 1;
						int preLevel = currentLevel - 1;
						foreach (JingLingYuanSuInfo info in this.RuntimeData.YuanSuInfoDict.Value.Values)
						{
							if (info.YuanSuType == yuanSuType && info.ShuXingType == shuXingType)
							{
								if (info.QiangHuaLevel == currentLevel)
								{
									currentLevelInfo = info;
								}
								else if (info.QiangHuaLevel == nextLevel)
								{
									nextLevelInfo = info;
								}
								else if (info.QiangHuaLevel == preLevel)
								{
									preLevelInfo = info;
								}
							}
						}
					}
					if (currentLevelInfo == null)
					{
						result = -3;
					}
					else
					{
						newID = currentLevelInfo.ID;
						if (nextLevelInfo == null)
						{
							result = -23;
						}
						else if (client.ClientData.MoneyData[144] < (long)currentLevelInfo.JieXingCurrency)
						{
							result = -47;
						}
						else if (!GoodsUtil.CheckHasGoodsList(client, currentLevelInfo.NeedGoods, false))
						{
							result = -6;
						}
						else
						{
							if (useShenYou > 0)
							{
								if (!GoodsUtil.CheckHasGoodsList(client, currentLevelInfo.Failtofail, false))
								{
									result = -6;
									goto IL_4AB;
								}
							}
							if (!GameManager.ClientMgr.ModifyYuanSuJueXingShiValue(client, -currentLevelInfo.JieXingCurrency, "精灵元素觉醒", true, true, false))
							{
								result = -47;
							}
							else
							{
								string strCostList = "";
								if (!GoodsUtil.CostGoodsList(client, currentLevelInfo.NeedGoods, false, ref strCostList, "精灵元素觉醒"))
								{
								}
								if (useShenYou > 0)
								{
									if (!GoodsUtil.CostGoodsList(client, currentLevelInfo.Failtofail, false, ref strCostList, "精灵元素觉醒"))
									{
									}
								}
								bool upLevel = false;
								double rnd = Global.GetRandom();
								if (rnd <= currentLevelInfo.Success)
								{
									upLevel = true;
								}
								int newLevel;
								if (upLevel)
								{
									newID = nextLevelInfo.ID;
									newLevel = nextLevelInfo.QiangHuaLevel;
									jueXingData.ActiveIDs[idx] = newID;
								}
								else
								{
									if (useShenYou > 0 || null == preLevelInfo)
									{
										result = 6;
										goto IL_4AB;
									}
									newID = preLevelInfo.ID;
									newLevel = preLevelInfo.QiangHuaLevel;
									jueXingData.ActiveIDs[idx] = newID;
								}
								if (jueXingData.ActiveType == yuanSuType)
								{
									JingLingYuanSuJueXingManager.getInstance().RefreshProps(client, false);
									client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
									{
										DelayExecProcIds.NotifyRefreshProps
									});
								}
								EventLogManager.AddRoleEvent(client, OpTypes.Upgrade, OpTags.Trace, LogRecordType.JingLingYuanSuJueXing, new object[]
								{
									currentID,
									currentLevel,
									newID,
									newLevel,
									currentLevelInfo.JieXingCurrency,
									useShenYou,
									strCostList
								});
								result = Global.sendToDB<int, RoleDataCmdT<JingLingYuanSuJueXingData>>(1452, new RoleDataCmdT<JingLingYuanSuJueXingData>(client.ClientData.RoleID, jueXingData), client.ServerId);
							}
						}
					}
				}
				IL_4AB:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", result, newID, useShenYou), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("JingLingYuanSuJueXing :: 激活觉醒石错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		
		public void RefreshProps(GameClient client, bool hint = true)
		{
			JingLingYuanSuJueXingData jueXingData = client.ClientData.JingLingYuanSuJueXingData;
			if (jueXingData != null && jueXingData.ActiveIDs != null)
			{
				int[] level = new int[2];
				int activeShuXing = jueXingData.ActiveType;
				int idx = (activeShuXing - 1) * 2;
				while (idx >= 0 && idx < activeShuXing * 2 && idx < jueXingData.ActiveIDs.Length)
				{
					double[] extProps = PropsCacheManager.ConstExtProps;
					if (0 <= idx && idx < jueXingData.ActiveIDs.Length)
					{
						int id = jueXingData.ActiveIDs[idx];
						lock (this.RuntimeData.Mutex)
						{
							JingLingYuanSuInfo info;
							if (this.RuntimeData.YuanSuInfoDict.Value.TryGetValue(id, out info))
							{
								extProps = info.ExtProps;
								level[idx - (activeShuXing - 1) * 2] = info.QiangHuaLevel;
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JingLingYuanSuJueXing,
						idx % 2,
						extProps
					});
					idx++;
				}
				double[] extProps2 = PropsCacheManager.ConstExtProps;
				int minLevel = level.Min();
				if (minLevel > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						int findLevel = 0;
						foreach (JingLingYuanSuShuXingInfo info2 in this.RuntimeData.ShuXingInfoDict.Value.Values)
						{
							int needLevel = info2.Level * 4;
							if (info2.Tipe == activeShuXing && minLevel >= needLevel)
							{
								if (findLevel < needLevel)
								{
									findLevel = needLevel;
									extProps2 = info2.ExtProps;
								}
							}
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingYuanSuJueXing,
					10,
					extProps2
				});
				if (hint)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		
		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System2Dot7) && GlobalNew.IsGongNengOpened(client, GongNengIDs.JingLingJueXing, hint);
		}

		
		public JingLingYuanSuJueXingRunData RuntimeData = new JingLingYuanSuJueXingRunData();

		
		private static JingLingYuanSuJueXingManager instance = new JingLingYuanSuJueXingManager();
	}
}
