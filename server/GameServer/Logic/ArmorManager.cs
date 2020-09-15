using System;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020001F5 RID: 501
	public class ArmorManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		// Token: 0x06000648 RID: 1608 RVA: 0x00057D8C File Offset: 0x00055F8C
		public static ArmorManager getInstance()
		{
			return ArmorManager.instance;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00057DA4 File Offset: 0x00055FA4
		public bool initialize()
		{
			return this.InitConfig();
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00057DC8 File Offset: 0x00055FC8
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num != 10)
			{
				if (num == 57)
				{
					TimerEventObject timeEventObj = eventObject as TimerEventObject;
					if (null != timeEventObj)
					{
						this.OnTimer(timeEventObj);
					}
				}
			}
			else
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.OnRoleDead(playerDeadEventObject.getPlayer());
				}
			}
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x00057E38 File Offset: 0x00056038
		public bool InitConfig()
		{
			bool success = true;
			string fileName = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.HudunBaoji = GameManager.systemParamsList.GetParamValueDoubleArrayByName("HudunBaoji", ',');
					fileName = "Config/ShenshenghudunJie.xml";
					string fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.ArmorUpDict.Load(fullPathFileName, null);
					fileName = "Config/ShenshenghudunXing.xml";
					fullPathFileName = Global.GameResPath(fileName);
					this.RuntimeData.ArmorStarDict.Load(fullPathFileName, null);
					foreach (ArmorStarInfo starInfo in this.RuntimeData.ArmorStarDict.Value.Values)
					{
						ArmorUpInfo ArmorUpInfo;
						if (this.RuntimeData.ArmorUpDict.Value.TryGetValue(starInfo.ArmorupStage, out ArmorUpInfo))
						{
							starInfo.ArmorUpInfo = ArmorUpInfo;
							ArmorUpInfo.MaxStarLevel = Math.Max(ArmorUpInfo.MaxStarLevel, starInfo.StarLevel);
							starInfo.ArmorUp += ArmorUpInfo.ArmorUp;
							starInfo.AddAttack += ArmorUpInfo.AddAttack;
							starInfo.AddDefense += ArmorUpInfo.AddDefense;
							starInfo.ShenmingUP += ArmorUpInfo.ShenmingUP;
							starInfo.ExtPropValues[119] = (double)starInfo.ArmorUp;
							starInfo.ExtPropValues[45] = (double)starInfo.AddAttack;
							starInfo.ExtPropValues[46] = (double)starInfo.AddDefense;
							starInfo.ExtPropValues[13] = (double)starInfo.ShenmingUP;
							starInfo.ExtPropValues[120] = ArmorUpInfo.Damageabsorption;
							starInfo.ExtPropValues[121] = ArmorUpInfo.Armorrecovery;
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

		// Token: 0x0600064C RID: 1612 RVA: 0x000580BC File Offset: 0x000562BC
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1447, 1, 1, ArmorManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			GlobalEventSource.getInstance().registerListener(10, ArmorManager.getInstance());
			GlobalEventSource.getInstance().registerListener(57, ArmorManager.getInstance());
			return true;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x0005810C File Offset: 0x0005630C
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ArmorManager.getInstance());
			GlobalEventSource.getInstance().removeListener(57, ArmorManager.getInstance());
			return true;
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00058144 File Offset: 0x00056344
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00058158 File Offset: 0x00056358
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0005816C File Offset: 0x0005636C
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			return nID != 1447 || this.ProcessArmorStarUpCmd(client, nID, bytes, cmdParams);
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x0005819C File Offset: 0x0005639C
		private bool IsGongNengOpened(GameClient client)
		{
			return GlobalNew.IsGongNengOpened(client, GongNengIDs.Armor, false);
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000581C4 File Offset: 0x000563C4
		private bool ProcessArmorStarUpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int result = 0;
			ArmorUpdateResultData resultData = new ArmorUpdateResultData();
			RoleArmorData ArmorData = client.ClientData.ArmorData;
			int exp = 0;
			ArmorUpdateResultData requestData = DataHelper.BytesToObject<ArmorUpdateResultData>(bytes, 0, bytes.Length);
			int type = requestData.Type;
			int zuanshi = requestData.ZuanShi;
			int auto = requestData.Auto;
			long nowTicks = TimeUtil.NOW();
			if (!this.IsGongNengOpened(client))
			{
				result = -12;
			}
			else
			{
				bool updateProps = false;
				lock (this.RuntimeData.Mutex)
				{
					if (ArmorData.Armor != requestData.Armor)
					{
						result = -3;
						goto IL_611;
					}
					ArmorStarInfo starInfo;
					if (!this.RuntimeData.ArmorStarDict.Value.TryGetValue(ArmorData.Armor, out starInfo))
					{
						result = -3;
						goto IL_611;
					}
					ArmorStarInfo starInfo2;
					if (!this.RuntimeData.ArmorStarDict.Value.TryGetValue(ArmorData.Armor + 1, out starInfo2))
					{
						result = -4004;
						goto IL_611;
					}
					bool useBind = false;
					bool useTimeLimit = false;
					string strCostList;
					if (type == 0)
					{
						if (starInfo.ArmorUpInfo.MaxStarLevel == starInfo.StarLevel)
						{
							result = -4;
							goto IL_611;
						}
						if (Global.UseGoodsBindOrNot(client, starInfo.NeedGoods[0], starInfo.NeedGoods[1], true, out useBind, out useTimeLimit) < 0)
						{
							if (zuanshi <= 0 || zuanshi != starInfo.NeedDiamond)
							{
								result = -6;
								goto IL_611;
							}
							if (!GameManager.ClientMgr.SubUserMoney(client, zuanshi, "神圣护盾升星", true, true, true, true, DaiBiSySType.None))
							{
								result = -10;
								goto IL_611;
							}
							exp = starInfo.ZuanShiExp;
							strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
							{
								-zuanshi,
								client.ClientData.UserMoney + zuanshi,
								client.ClientData.UserMoney
							});
							if (Global.GetRandom() < this.RuntimeData.HudunBaoji[1])
							{
								exp = (int)((double)exp * this.RuntimeData.HudunBaoji[2]);
							}
						}
						else
						{
							exp = starInfo.GoodsExp;
							strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
							{
								GoodsID = starInfo.NeedGoods[0],
								GCount = starInfo.NeedGoods[1]
							});
							if (Global.GetRandom() < this.RuntimeData.HudunBaoji[0])
							{
								exp = (int)((double)exp * this.RuntimeData.HudunBaoji[2]);
							}
						}
						ArmorData.Exp += exp;
						if (ArmorData.Exp >= starInfo.StarExp)
						{
							ArmorData.Armor++;
							if (starInfo.StarLevel < starInfo.ArmorUpInfo.MaxStarLevel - 1)
							{
								ArmorData.Exp -= starInfo.StarExp;
							}
							else
							{
								ArmorData.Exp = 0;
							}
						}
					}
					else
					{
						if (starInfo.ArmorUpInfo.MaxStarLevel != starInfo.StarLevel)
						{
							result = -4;
							goto IL_611;
						}
						if (Global.UseGoodsBindOrNot(client, starInfo.ArmorUpInfo.NeedGoods[0], starInfo.ArmorUpInfo.NeedGoods[1], true, out useBind, out useTimeLimit) < 0)
						{
							if (zuanshi <= 0 || zuanshi != starInfo.ArmorUpInfo.NeedDiamond)
							{
								result = -6;
								goto IL_611;
							}
							if (!GameManager.ClientMgr.SubUserMoney(client, zuanshi, "神圣护盾升阶", true, true, true, true, DaiBiSySType.None))
							{
								result = -10;
								goto IL_611;
							}
							strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
							{
								-zuanshi,
								client.ClientData.UserMoney + zuanshi,
								client.ClientData.UserMoney
							});
						}
						else
						{
							strCostList = EventLogManager.NewGoodsDataPropString(new GoodsData
							{
								GoodsID = starInfo.ArmorUpInfo.NeedGoods[0],
								GCount = starInfo.ArmorUpInfo.NeedGoods[1]
							});
							exp = starInfo.GoodsExp;
						}
						ArmorData.Exp++;
						if (starInfo.ArmorUpInfo.LuckyOne + ArmorData.Exp >= 110000)
						{
							ArmorData.Armor++;
							ArmorData.Exp = 0;
						}
						else if (starInfo.ArmorUpInfo.LuckyOne + ArmorData.Exp > starInfo.ArmorUpInfo.LuckyTwo)
						{
							if (Global.GetRandom() < starInfo.ArmorUpInfo.LuckyTwoRate)
							{
								ArmorData.Armor++;
								ArmorData.Exp = 0;
							}
						}
					}
					Global.SendToDB<RoleDataCmdT<RoleArmorData>>(1447, new RoleDataCmdT<RoleArmorData>(client.ClientData.RoleID, ArmorData), client.ServerId);
					if (ArmorData.Armor > requestData.Armor)
					{
						updateProps = true;
						EventLogManager.AddArmorEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.ArmorupStage, starInfo.StarLevel, starInfo2.ArmorupStage, starInfo2.StarLevel, ArmorData.Exp, strCostList);
					}
					else
					{
						EventLogManager.AddArmorEvent(client, type, (zuanshi > 0) ? 1 : 0, exp, starInfo.ArmorupStage, starInfo.StarLevel, starInfo.ArmorupStage, starInfo.StarLevel, ArmorData.Exp, strCostList);
					}
				}
				if (updateProps)
				{
					this.ResetArmor(client, true);
				}
			}
			IL_611:
			resultData.Result = result;
			resultData.Armor = ArmorData.Armor;
			resultData.Exp = ArmorData.Exp;
			resultData.Auto = auto;
			resultData.Type = type;
			client.sendCmd<ArmorUpdateResultData>(nID, resultData, false);
			return true;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00058840 File Offset: 0x00056A40
		public void ResetArmor(GameClient client, bool reset = true)
		{
			if (client.ClientData.ArmorData.Armor > 0)
			{
				ExtData extData = ExtDataManager.GetClientExtData(client);
				lock (this.RuntimeData.Mutex)
				{
					ArmorStarInfo starInfo;
					if (this.RuntimeData.ArmorStarDict.Value.TryGetValue(client.ClientData.ArmorData.Armor, out starInfo) && starInfo.ArmorUpInfo != null)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.Armor,
							starInfo.ExtPropValues
						});
					}
				}
				client.ClientData.ArmorPercent = RoleAlgorithm.GetExtProp(client, 120);
				int max = (int)RoleAlgorithm.GetExtProp(client, 119);
				if (reset)
				{
					extData.ArmorCurrentV = extData.ArmorMaxV;
				}
				else if (max > extData.ArmorMaxV)
				{
					extData.ArmorCurrentV += max - extData.ArmorMaxV;
				}
				extData.ArmorMaxV = max;
				if (extData.ArmorMaxV != client.ClientData.ArmorV || extData.ArmorCurrentV != client.ClientData.CurrentArmorV)
				{
					client.ClientData.ArmorV = extData.ArmorMaxV;
					client.ClientData.CurrentArmorV = extData.ArmorCurrentV;
					client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
					{
						DelayExecProcIds.NotifyRefreshProps
					});
				}
			}
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x000589FC File Offset: 0x00056BFC
		public void OnRoleDead(GameClient client)
		{
			this.ResetArmor(client, true);
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00058A08 File Offset: 0x00056C08
		public void OnInitGame(GameClient client)
		{
			if (client.ClientData.ArmorData == null)
			{
				client.ClientData.ArmorData = new RoleArmorData();
			}
			this.InitDataByTask(client);
			this.ResetArmor(client, true);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00058A50 File Offset: 0x00056C50
		public void InitDataByTask(GameClient client)
		{
			if (client.ClientData.ArmorData.Armor <= 0)
			{
				if (this.IsGongNengOpened(client))
				{
					client.ClientData.ArmorData.Armor = 1;
					this.ResetArmor(client, true);
				}
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00058AA4 File Offset: 0x00056CA4
		public void OnTimer(TimerEventObject eventObj)
		{
			GameClient client = eventObj.Client;
			if (client.ClientData.CurrentArmorV < client.ClientData.ArmorV)
			{
				if (!client.buffManager.IsBuffEnabled(114))
				{
					double rate = Global.Clamp((double)eventObj.DeltaTicks / 1000.0, 0.0, 5.0);
					int max = (int)RoleAlgorithm.GetExtProp(client, 119);
					int recover = (int)((double)max * RoleAlgorithm.GetExtProp(client, 121) * rate);
					client.ClientData.CurrentArmorV += recover;
					if (client.ClientData.CurrentArmorV > max)
					{
						client.ClientData.CurrentArmorV = max;
					}
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 4);
				}
			}
		}

		// Token: 0x04000B17 RID: 2839
		private static ArmorManager instance = new ArmorManager();

		// Token: 0x04000B18 RID: 2840
		private ArmorManagerData RuntimeData = new ArmorManagerData();
	}
}
