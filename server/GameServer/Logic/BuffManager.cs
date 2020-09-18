using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using Server.Data;

namespace GameServer.Logic
{
	
	public class BuffManager : IEventListener
	{
		
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

		
		public void OnRoleDead(GameClient client)
		{
			client.buffManager.SetStatusBuff(113, 0L, 0L, 0L);
		}

		
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

		
		public BuffItemData GetBuffItemData(int buffId)
		{
			return this.BuffListArray[buffId];
		}

		
		static BuffManager()
		{
			foreach (KeyValuePair<int, int> kv in BuffManager.BufferId2Flags)
			{
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByMapCode = ((kv.Value & 4) != 0);
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByTime = ((kv.Value & 1) != 0);
				BuffManager.BuffId2FlagsDict[kv.Key].isUpdateByVip = ((kv.Value & 2) != 0);
			}
		}

		
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

		
		public bool IsBuffEnabled(int BuffId)
		{
			return this.BuffListArray[BuffId].enabled;
		}

		
		public void SetStatusBuff(int BuffId, long startTicks, long keepTicks, long buffVal = 0L)
		{
			this.BuffListArray[BuffId].startTicks = startTicks;
			this.BuffListArray[BuffId].endTicks = startTicks + keepTicks;
			this.BuffListArray[BuffId].enabled = (startTicks > 0L);
			this.BuffListArray[BuffId].buffVal = buffVal;
		}

		
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

		
		public void UpdateMapLimitBuffIds(GameClient client, int[] ids)
		{
		}

		
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

		
		private static List<int> UpdateByMapCodeBuffIdList = new List<int>
		{
			121
		};

		
		private static List<int> UpdateByVipBuffIdList = new List<int>();

		
		private static BuffItemFlags[] BuffId2FlagsDict = new BuffItemFlags[124];

		
		private static List<double> ZSPLHXZAttackInjurePercent = new List<double>();

		
		private object mutex = new object();

		
		private static object mutex_config = new object();

		
		private int CurrentMapCode;

		
		private int CurrentVipLevel;

		
		private int[] BuffIds;

		
		public BuffItemData[] BuffListArray = new BuffItemData[512];
	}
}
