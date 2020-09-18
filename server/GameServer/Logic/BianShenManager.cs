using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	
	public class BianShenManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		
		public static BianShenManager getInstance()
		{
			return BianShenManager.instance;
		}

		
		public bool initialize()
		{
			return this.InitConfig();
		}

		
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num != 10)
			{
				if (num != 28)
				{
					if (num == 58)
					{
						MainTaskProgressEvent mainTaskProgressEvent = eventObject as MainTaskProgressEvent;
						if (null != mainTaskProgressEvent)
						{
							this.InitDataByTask(mainTaskProgressEvent.Client);
						}
					}
				}
				else
				{
					OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
					if (null != onStartPlayGameEventObject)
					{
						if (!this.CanBianShenByMap(onStartPlayGameEventObject.Client))
						{
							this.ClearBianShen(onStartPlayGameEventObject.Client);
						}
					}
				}
			}
			else
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.ClearBianShen(playerDeadEventObject.getPlayer());
				}
			}
		}

		
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.IsGongNengOpend = GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(103);
					this.RuntimeData.BianShenFull = GameManager.systemParamsList.GetParamValueIntArrayByName("BianShenFull", ',');
					this.RuntimeData.BianShenCDSecs = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationCD", -1);
					this.RuntimeData.TransfigurationBuff = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationBuff", -1);
					this.RuntimeData.NeedGoods = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("TransfigurationGoods"), true, '|', ',');
					this.RuntimeData.FreeNum = (int)GameManager.systemParamsList.GetParamValueIntByName("TransfigurationFree", -1);
					fileName = "Config/TransfigurationFashionEffect.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.FashionEffectInfoDict.Load(fullPathFileName, null);
					foreach (FashionEffectInfo info in this.RuntimeData.FashionEffectInfoDict.Value.Values)
					{
						ConfigParser.ParseExtprops(info.ExtPropValues, info.ProPerty, "|,");
					}
					fileName = "Config/TransfigurationLevel.xml";
					fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.BianShenStarDict.Load(fullPathFileName, null);
					foreach (BianShenStarInfo starInfo in this.RuntimeData.BianShenStarDict.Value.Values)
					{
						foreach (int occu in starInfo.OccupationID)
						{
							List<BianShenStarInfo> list;
							if (!this.RuntimeData.BianShenUpDict.TryGetValue(occu, out list))
							{
								list = new List<BianShenStarInfo>();
								this.RuntimeData.BianShenUpDict[occu] = list;
							}
							int level = starInfo.Level;
							while (list.Count < level + 1)
							{
								list.Add(null);
							}
							list[level] = starInfo;
							ConfigParser.ParseExtprops(starInfo.ExtPropValues, starInfo.ProPerty, "|,");
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
			TCPCmdDispatcher.getInstance().registerProcessorEx(1448, 1, 1, BianShenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1449, 1, 1, BianShenManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(28, BianShenManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, BianShenManager.getInstance());
			GlobalEventSource.getInstance().registerListener(58, BianShenManager.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, BianShenManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, BianShenManager.getInstance());
			GlobalEventSource.getInstance().removeListener(58, BianShenManager.getInstance());
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
			switch (nID)
			{
			case 1448:
				result = this.ProcessExecuteBianShenCmd(client, nID, bytes, cmdParams);
				break;
			case 1449:
				result = this.ProcessBianShenStarUpCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		private bool IsGongNengOpened(GameClient client)
		{
			return this.RuntimeData.IsGongNengOpend && GlobalNew.IsGongNengOpened(client, GongNengIDs.BianShen, false);
		}

		
		private bool ProcessBianShenStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			BianShenUpdateResultData resultData = new BianShenUpdateResultData();
			RoleBianShenData BianShenData = client.ClientData.BianShenData;
			BianShenUpdateResultData requestData = DataHelper.BytesToObject<BianShenUpdateResultData>(bytes, 0, bytes.Length);
			int type = requestData.Type;
			int zuanshi = requestData.ZuanShi;
			int auto = requestData.Auto;
			long nowTicks = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -400;
			}
			else
			{
				string strCostList = "";
				lock (this.RuntimeData.Mutex)
				{
					List<BianShenStarInfo> list;
					if (BianShenData.BianShen != requestData.BianShen)
					{
						result = -18;
					}
					else if (!this.RuntimeData.BianShenUpDict.TryGetValue(client.ClientData.Occupation, out list))
					{
						result = -400;
					}
					else if (BianShenData.BianShen >= list.Count - 1)
					{
						result = -23;
					}
					else
					{
						BianShenStarInfo starInfo = list[BianShenData.BianShen];
						BianShenStarInfo starInfo2 = list[BianShenData.BianShen + 1];
						if (starInfo == null || starInfo2 == null)
						{
							result = -3;
						}
						else if (!GoodsUtil.CostGoodsList(client, starInfo.NeedGoods, false, ref strCostList, "变身升级"))
						{
							result = -6;
						}
						else
						{
							int exp = starInfo.GoodsExp;
							if (Global.GetRandom() < starInfo.ExpCritRate)
							{
								exp = (int)((double)exp * starInfo.ExpCritTimes);
							}
							BianShenData.Exp += exp;
							if (BianShenData.Exp >= starInfo.UpExp)
							{
								BianShenData.BianShen++;
								BianShenData.Exp -= starInfo.UpExp;
							}
							Global.SendToDB<RoleDataCmdT<RoleBianShenData>>(1449, new RoleDataCmdT<RoleBianShenData>(client.ClientData.RoleID, BianShenData), client.ServerId);
							if (BianShenData.BianShen > requestData.BianShen)
							{
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BianShen,
									starInfo2.ExtPropValues
								});
								client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
								{
									DelayExecProcIds.RecalcProps,
									DelayExecProcIds.NotifyRefreshProps
								});
								EventLogManager.AddBianShenEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.Level, starInfo2.Level, BianShenData.Exp, strCostList);
							}
							else
							{
								EventLogManager.AddBianShenEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.Level, starInfo.Level, BianShenData.Exp, strCostList);
							}
						}
					}
				}
			}
			resultData.Result = result;
			resultData.BianShen = BianShenData.BianShen;
			resultData.Exp = BianShenData.Exp;
			resultData.Auto = auto;
			resultData.Type = type;
			client.sendCmd<BianShenUpdateResultData>(nID, resultData, false);
			return true;
		}

		
		private bool ProcessExecuteBianShenCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			long nowTicks = TimeUtil.NOW();
			if (!this.CanBianShenByMap(client))
			{
				result = -21;
			}
			else if (client.ClientData.IsDongJie())
			{
				result = -500;
			}
			else
			{
				ExtData extData = ExtDataManager.GetClientExtData(client);
				if (nowTicks < extData.BianShenCDTicks)
				{
					result = -2007;
				}
				else if (!this.IsGongNengOpened(client) || client.ClientData.HideGM > 0)
				{
					result = -12;
				}
				else
				{
					ZuoQiManager.getInstance().RoleDisMount(client, true);
					long cdTime = (long)(this.RuntimeData.BianShenCDSecs * 1000);
					int durationTime = 0;
					int skillLevel = 1;
					double[] props = null;
					List<int> skillIDList = null;
					BianShenStarInfo starInfo;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.BianShenStarDict.Value.TryGetValue(client.ClientData.BianShenData.BianShen, out starInfo))
						{
							result = -20;
							goto IL_4E7;
						}
						skillLevel = starInfo.Level;
					}
					string strCostList = "";
					long dayAndCount = client.ClientData.MoneyData[147];
					int dayid = (int)(dayAndCount / 10000L);
					int dayCount = (int)(dayAndCount % 10000L);
					if (dayid != TimeUtil.GetOffsetDayNow())
					{
						dayCount = 0;
						dayid = TimeUtil.GetOffsetDayNow();
					}
					if (dayCount < this.RuntimeData.FreeNum)
					{
						dayCount++;
						dayAndCount = (long)(dayid * 10000 + dayCount);
						client.ClientData.MoneyData[147] = dayAndCount;
						Global.SaveRoleParamsInt64ValueToDB(client, "10216", dayAndCount, true);
						GameManager.ClientMgr.NotifySelfPropertyValue(client, 147, dayAndCount);
					}
					else if (!GoodsUtil.CostGoodsList(client, this.RuntimeData.NeedGoods, false, ref strCostList, "变身"))
					{
						result = -6;
						goto IL_4E7;
					}
					int damageType = OccupationUtil.GetOccuDamageType(client.ClientData.OccupationIndex);
					GoodsData goodsData = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 28);
					if (null != goodsData)
					{
						FashionBagData fashionBagData = FashionManager.getInstance().GetFashionBagData(client, goodsData);
						if (fashionBagData != null && fashionBagData.FasionTab == 7)
						{
							if (damageType == 1)
							{
								skillIDList = fashionBagData.MagicSkill;
							}
							else
							{
								skillIDList = fashionBagData.AttackSkill;
							}
							durationTime = fashionBagData.BianShenDuration;
							if (fashionBagData.BianShenEffect > 0)
							{
								lock (this.RuntimeData.Mutex)
								{
									FashionEffectInfo info;
									if (this.RuntimeData.FashionEffectInfoDict.Value.TryGetValue(fashionBagData.BianShenEffect, out info))
									{
										props = info.ExtPropValues;
									}
								}
							}
						}
					}
					else
					{
						durationTime = starInfo.Duration;
						if (damageType == 1)
						{
							skillIDList = starInfo.MagicSkill;
						}
						else
						{
							skillIDList = starInfo.AttackSkill;
						}
					}
					if (null != skillIDList)
					{
						lock (client.ClientData.SkillDataList)
						{
							using (List<int>.Enumerator enumerator = skillIDList.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									int skillID = enumerator.Current;
									SkillData mySkillData = client.ClientData.SkillDataList.Find((SkillData x) => x.SkillID == skillID);
									if (null == mySkillData)
									{
										SkillData skillData = new SkillData();
										skillData.SkillID = skillID;
										skillData.SkillLevel = skillLevel;
										client.ClientData.SkillDataList.Add(skillData);
									}
									else if (mySkillData.SkillLevel != skillLevel)
									{
										mySkillData.SkillLevel = skillLevel;
									}
								}
							}
						}
					}
					extData.skillIDList = skillIDList;
					extData.BianShenCDTicks = nowTicks + cdTime + (long)(durationTime * 1000);
					extData.BianShenCdTime = cdTime;
					extData.BianShenToTicks = nowTicks + (long)(durationTime * 1000);
					client.buffManager.SetStatusBuff(121, nowTicks, (long)(durationTime * 1000), 0L);
					this.OnBianShenStateChange(client, true, client.ClientData.BianShenData.BianShen, durationTime, props);
					GameManager.ClientMgr.NotifySkillCDTime(client, -1, extData.BianShenCDTicks, false);
				}
			}
			IL_4E7:
			client.sendCmd<int>(nID, result, false);
			return true;
		}

		
		public bool CanBianShenByMap(GameClient client)
		{
			MapSettingItem item;
			return Data.SettingsDict.Value.TryGetValue(client.ClientData.MapCode, out item) && item.Transfiguration > 0;
		}

		
		private void OnStartPlayGame(GameClient client)
		{
			if (!this.CanBianShenByMap(client))
			{
				if (client.buffManager.IsBuffEnabled(121))
				{
					this.ClearBianShen(client);
				}
			}
		}

		
		public void ClearBianShen(GameClient client)
		{
			ExtData extData = ExtDataManager.GetClientExtData(client);
			long maxCdTicks = TimeUtil.NOW();
			if (maxCdTicks < extData.BianShenToTicks)
			{
				extData.BianShenToTicks = 0L;
				extData.BianShenCDTicks = maxCdTicks + extData.BianShenCdTime;
				extData.skillIDList = null;
				client.buffManager.SetStatusBuff(121, 0L, 0L, 0L);
			}
		}

		
		public bool CanUseMagic(GameClient client, int skillID)
		{
			ExtData extData = ExtDataManager.GetClientExtData(client);
			List<int> list = extData.skillIDList;
			if (client.buffManager.IsBuffEnabled(121))
			{
				if (list != null && list.Contains(skillID))
				{
					return true;
				}
			}
			else if (list == null || !list.Contains(skillID))
			{
				return true;
			}
			return false;
		}

		
		public void OnInitGame(GameClient client)
		{
			RoleBianShenData BianShenData = client.ClientData.BianShenData;
			if (BianShenData == null)
			{
				BianShenData = (client.ClientData.BianShenData = new RoleBianShenData());
			}
			bool resetCD = false;
			int[] array = this.RuntimeData.BianShenFull;
			if (array != null && array[0] > 0)
			{
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i] == client.ClientData.MapCode)
					{
						resetCD = true;
						break;
					}
				}
			}
			ExtData extData = ExtDataManager.GetClientExtData(client);
			this.InitDataByTask(client);
			if (BianShenData.BianShen > 0)
			{
				extData.BianShenToTicks = 0L;
				extData.skillIDList = null;
				lock (this.RuntimeData.Mutex)
				{
					List<BianShenStarInfo> list;
					if (this.RuntimeData.BianShenUpDict.TryGetValue(client.ClientData.Occupation, out list) && BianShenData.BianShen < list.Count)
					{
						BianShenStarInfo starInfo = list[BianShenData.BianShen];
						if (null != starInfo)
						{
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.BianShen,
								starInfo.ExtPropValues
							});
							if (resetCD)
							{
								extData.BianShenCDTicks = 0L;
							}
							else
							{
								long nowTicks = TimeUtil.NOW();
								long maxCdTicks = nowTicks + extData.BianShenCdTime;
								if (maxCdTicks < extData.BianShenCDTicks)
								{
									extData.BianShenCDTicks = maxCdTicks;
								}
								if (nowTicks < extData.BianShenCDTicks)
								{
									GameManager.ClientMgr.NotifySkillCDTime(client, -1, extData.BianShenCDTicks, true);
								}
							}
						}
					}
				}
			}
		}

		
		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.BianShenData.BianShen <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.BianShenData.BianShen = 1;
				}
			}
		}

		
		public void OnBianShenStateChange(GameClient client, bool active, int level = 0, int keepSecs = 0, double[] props = null)
		{
			if (active)
			{
				if (null != props)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						41,
						props
					});
				}
				double[] actionParams = new double[]
				{
					(double)this.RuntimeData.TransfigurationBuff,
					(double)keepSecs
				};
				Global.UpdateBufferData(client, BufferItemTypes.BianShen, actionParams, 1, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					41,
					PropsCacheManager.ConstExtProps
				});
				double[] array = new double[2];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.BianShen, actionParams, 1, true);
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.RecalcProps,
					DelayExecProcIds.NotifyRefreshProps
				});
			}
		}

		
		public int GetBianShenLevel(GameClient client)
		{
			int result;
			if (null != client.ClientData.BianShenData)
			{
				result = client.ClientData.BianShenData.BianShen;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		
		private static BianShenManager instance = new BianShenManager();

		
		private BianShenManagerData RuntimeData = new BianShenManagerData();
	}
}
