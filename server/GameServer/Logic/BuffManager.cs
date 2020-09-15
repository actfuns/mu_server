using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Data;

namespace GameServer.Logic
{
	// Token: 0x02000064 RID: 100
	public class BuffManager : IEventListener
	{
		// Token: 0x06000167 RID: 359 RVA: 0x000180F4 File Offset: 0x000162F4
		public static void InitConfig()
		{
			lock (BuffManager.mutex_config)
			{
				BuffManager.ZSPLHXZAttackInjurePercent.Clear();
				string strPctList = GameManager.systemParamsList.GetParamValueByName("ZSPLHXZAttackInjurePercent");
				if (!string.IsNullOrEmpty(strPctList))
				{
					string[] strPctFields = strPctList.Split(new char[]
					{
						','
					});
					for (int i = 0; i < strPctFields.Length; i++)
					{
						BuffManager.ZSPLHXZAttackInjurePercent.Add(Global.SafeConvertToDouble(strPctFields[i]));
					}
				}
			}
		}

		// Token: 0x06000168 RID: 360 RVA: 0x000181A8 File Offset: 0x000163A8
		public void processEvent(EventObject eventObject)
		{
			int nID = eventObject.getEventType();
			int num = nID;
			if (num == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getPlayer())
				{
					this.OnRoleDead(playerDeadEventObject.getPlayer());
				}
			}
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000181F2 File Offset: 0x000163F2
		public void OnRoleDead(GameClient client)
		{
			client.buffManager.SetStatusBuff(113, 0L, 0L, 0L);
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0001820C File Offset: 0x0001640C
		public static double GetZSPLHXZAttackInjurePercent(int clientNum)
		{
			double result;
			lock (BuffManager.mutex_config)
			{
				if (clientNum <= 0 || BuffManager.ZSPLHXZAttackInjurePercent == null || BuffManager.ZSPLHXZAttackInjurePercent.Count == 0)
				{
					result = 0.0;
				}
				else
				{
					result = ((clientNum > BuffManager.ZSPLHXZAttackInjurePercent.Count) ? BuffManager.ZSPLHXZAttackInjurePercent[BuffManager.ZSPLHXZAttackInjurePercent.Count - 1] : BuffManager.ZSPLHXZAttackInjurePercent[clientNum - 1]);
				}
			}
			return result;
		}

		// Token: 0x0600016B RID: 363 RVA: 0x000182B4 File Offset: 0x000164B4
		public BuffItemData GetBuffItemData(int buffId)
		{
			return this.BuffListArray[buffId];
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000182D0 File Offset: 0x000164D0
		static BuffManager()
		{
			foreach (KeyValuePair<int, int> kv in BuffManager.BufferId2Flags)
			{
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByMapCode = ((kv.Value & 4) != 0);
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByTime = ((kv.Value & 1) != 0);
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByVip = ((kv.Value & 2) != 0);
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00018494 File Offset: 0x00016694
		public BuffManager()
		{
			for (int i = 0; i < this.BuffListArray.Length; i++)
			{
				this.BuffListArray[i] = new BuffItemData
				{
					buffId = i
				};
			}
			foreach (KeyValuePair<int, int> kv in BuffManager.BufferId2Flags)
			{
				this.BuffListArray[kv.Key].flags = kv.Value;
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00018554 File Offset: 0x00016754
		public bool IsBuffEnabled(int BuffId)
		{
			return this.BuffListArray[BuffId].enabled;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00018574 File Offset: 0x00016774
		public void SetStatusBuff(int BuffId, long startTicks, long keepTicks, long buffVal = 0L)
		{
			this.BuffListArray[BuffId].startTicks = startTicks;
			this.BuffListArray[BuffId].endTicks = startTicks + keepTicks;
			this.BuffListArray[BuffId].enabled = (startTicks > 0L);
			this.BuffListArray[BuffId].buffVal = buffVal;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x000185C4 File Offset: 0x000167C4
		public void UpdateBuffData(BufferData BuffData)
		{
			if (BuffData.BufferID < 124)
			{
				if (BuffManager.BuffId2FlagsDict[BuffData.BufferID].isUpdateByVip)
				{
					if (!Global.IsBufferDataOver(BuffData, 0L))
					{
					}
				}
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00018610 File Offset: 0x00016810
		public void UpdateByTime(GameClient client, long nowTicks)
		{
			int mapCode = client.ClientData.MapCode;
			if (this.CurrentMapCode != mapCode)
			{
				this.CurrentMapCode = mapCode;
			}
			if (this.CurrentVipLevel != client.ClientData.VipLevel)
			{
				this.CurrentVipLevel = client.ClientData.VipLevel;
			}
			foreach (int id in BuffManager.UpdateByTimerBuffIdList)
			{
				this.UpdateImmediately(client, id, nowTicks);
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x000186BC File Offset: 0x000168BC
		public void UpdateImmediately(GameClient client, int id, long nowTicks)
		{
			lock (this.mutex)
			{
				bool enabledByTime = this.BuffListArray[id].endTicks > nowTicks;
				if (enabledByTime != this.BuffListArray[id].enabledByTime)
				{
					this.BuffListArray[id].enabledByTime = enabledByTime;
					int flags = this.BuffListArray[id].flags;
					this.BuffListArray[id].enabled = enabledByTime;
					if (this.BuffListArray[id].enabled != this.BuffListArray[id].clientEnabledState)
					{
						this.BuffListArray[id].clientEnabledState = this.BuffListArray[id].enabled;
						this.OnBuffStateChange(client, id, this.BuffListArray[id]);
					}
				}
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000187A4 File Offset: 0x000169A4
		public void UpdateMapLimitBuffIds(GameClient client, int[] ids)
		{
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000187A8 File Offset: 0x000169A8
		private void OnBaTiStateChange(GameClient client, bool active, BuffItemData buffItemData)
		{
			if (active)
			{
				if (buffItemData.buffVal >= 30L)
				{
					client.ClientData.DongJieStart = 0L;
					client.ClientData.DongJieSeconds = 0;
					client.RoleBuffer.SetTempExtProp(47, 0.0, 0L);
					client.RoleBuffer.SetTempExtProp(2, 0.0, 0L);
					client.RoleBuffer.SetTempExtProp(18, 0.0, 0L);
					double moveCost = RoleAlgorithm.GetMoveSpeed(client);
					client.ClientData.MoveSpeed = moveCost;
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 12, TimeUtil.NOW(), (int)(buffItemData.endTicks - buffItemData.startTicks), moveCost);
				}
				if (buffItemData.buffVal >= 50L)
				{
					buffItemData.buffValEx = BuffManager.GetZSPLHXZAttackInjurePercent(Global.GetAll9ClientsNum(client) - 1);
				}
			}
			else
			{
				if (buffItemData.buffId >= 30)
				{
					double moveCost = RoleAlgorithm.GetMoveSpeed(client);
					client.ClientData.MoveSpeed = moveCost;
					GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 12, TimeUtil.NOW(), 0, moveCost);
				}
				if (buffItemData.buffVal >= 50L)
				{
					buffItemData.buffValEx = 0.0;
				}
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0001891C File Offset: 0x00016B1C
		public void OnBuffStateChange(GameClient client, int bufferId, BuffItemData buffItemData)
		{
			if ((buffItemData.flags & 1073741824) != 0)
			{
				switch (bufferId)
				{
				case 113:
					this.OnBaTiStateChange(client, buffItemData.enabled, buffItemData);
					break;
				case 114:
					if (buffItemData.enabled)
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, GLang.GetLang(4, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 13, buffItemData.startTicks, buffItemData.buffSecs, 1.0);
					}
					else
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, GLang.GetLang(5, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 13, 0L, 0, 1.0);
					}
					break;
				case 116:
					if (!buffItemData.enabled)
					{
						HuiJiManager.getInstance().OnHuiJiStateChange(client, false, 0, 0, null);
					}
					break;
				case 120:
					if (buffItemData.enabled)
					{
						client.ClientData.MinLife = (int)buffItemData.buffVal;
					}
					else
					{
						client.ClientData.MinLife = 0;
					}
					break;
				case 121:
					if (!buffItemData.enabled)
					{
						BianShenManager.getInstance().OnBianShenStateChange(client, false, 0, 0, null);
					}
					break;
				}
			}
		}

		// Token: 0x04000242 RID: 578
		private static List<int> UpdateByTimerBuffIdList = new List<int>
		{
			114,
			116,
			117,
			118,
			113,
			120,
			121
		};

		// Token: 0x04000243 RID: 579
		private static Dictionary<int, int> BufferId2Flags = new Dictionary<int, int>
		{
			{
				114,
				1073741824
			},
			{
				116,
				1073741824
			},
			{
				117,
				1073741824
			},
			{
				118,
				1073741824
			},
			{
				113,
				1073741824
			},
			{
				120,
				1073741824
			},
			{
				121,
				1073741828
			}
		};

		// Token: 0x04000244 RID: 580
		private static List<int> UpdateByMapCodeBuffIdList = new List<int>
		{
			121
		};

		// Token: 0x04000245 RID: 581
		private static List<int> UpdateByVipBuffIdList = new List<int>();

		// Token: 0x04000246 RID: 582
		private static BuffItemFlags[] BuffId2FlagsDict = new BuffItemFlags[124];

		// Token: 0x04000247 RID: 583
		private static List<double> ZSPLHXZAttackInjurePercent = new List<double>();

		// Token: 0x04000248 RID: 584
		private object mutex = new object();

		// Token: 0x04000249 RID: 585
		private static object mutex_config = new object();

		// Token: 0x0400024A RID: 586
		private int CurrentMapCode;

		// Token: 0x0400024B RID: 587
		private int CurrentVipLevel;

		// Token: 0x0400024C RID: 588
		private int[] BuffIds;

		// Token: 0x0400024D RID: 589
		public BuffItemData[] BuffListArray = new BuffItemData[512];
	}
}
