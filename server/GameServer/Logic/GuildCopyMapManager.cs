using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004F6 RID: 1270
	public class GuildCopyMapManager
	{
		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06001791 RID: 6033 RVA: 0x00172590 File Offset: 0x00170790
		public int FirstGuildCopyMapOrder
		{
			get
			{
				return 40000;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06001792 RID: 6034 RVA: 0x001725A8 File Offset: 0x001707A8
		public List<int> GuildCopyMapOrderList
		{
			get
			{
				return this.guildCopyMapOrderList;
			}
		}

		// Token: 0x06001793 RID: 6035 RVA: 0x001725C0 File Offset: 0x001707C0
		public void LoadGuildCopyMapOrder()
		{
			this.GuildCopyMapOrderList.Clear();
			this.GuildCopyMapOrderList.Add(this.FirstGuildCopyMapOrder);
			int beginOrder = this.FirstGuildCopyMapOrder;
			for (;;)
			{
				SystemXmlItem systemFuBenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(beginOrder, out systemFuBenItem))
				{
					break;
				}
				if (null == systemFuBenItem)
				{
					break;
				}
				int nDownCopyID = systemFuBenItem.GetIntValue("DownCopyID", -1);
				if (nDownCopyID <= 0)
				{
					goto Block_3;
				}
				beginOrder = nDownCopyID;
				this.GuildCopyMapOrderList.Add(nDownCopyID);
			}
			return;
			Block_3:
			this.LastGuildCopyMapOrder = beginOrder;
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x00172654 File Offset: 0x00170854
		// (set) Token: 0x06001795 RID: 6037 RVA: 0x0017266C File Offset: 0x0017086C
		public int LastGuildCopyMapOrder
		{
			get
			{
				return this.lastGuildCopyMapOrder;
			}
			set
			{
				this.lastGuildCopyMapOrder = value;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x06001796 RID: 6038 RVA: 0x00172678 File Offset: 0x00170878
		public int MaxDamageSendCount
		{
			get
			{
				return this.maxDamageSendCount;
			}
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x00172690 File Offset: 0x00170890
		public bool IsPrepareResetTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			DayOfWeek dayofweek = now.DayOfWeek;
			bool result;
			if (dayofweek != DayOfWeek.Sunday)
			{
				result = false;
			}
			else
			{
				DateTime beginTime = new DateTime(now.Year, now.Month, now.Day, 23, 55, 0);
				DateTime EndTime = new DateTime(now.Year, now.Month, now.Day, 23, 56, 0);
				result = (now >= beginTime && now <= EndTime);
			}
			return result;
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x0017271C File Offset: 0x0017091C
		public bool IsRefuseTime()
		{
			DateTime now = TimeUtil.NowDateTime();
			DayOfWeek dayofweek = now.DayOfWeek;
			bool result;
			if (dayofweek != DayOfWeek.Sunday)
			{
				result = false;
			}
			else
			{
				DateTime beginTime = new DateTime(now.Year, now.Month, now.Day, 23, 55, 0);
				DateTime EndTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
				result = (now >= beginTime && now <= EndTime);
			}
			return result;
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x001727A8 File Offset: 0x001709A8
		public bool IsGuildCopyMap(int fubenID)
		{
			return this.GuildCopyMapOrderList.IndexOf(fubenID) >= 0;
		}

		// Token: 0x0600179A RID: 6042 RVA: 0x001727CC File Offset: 0x001709CC
		public int GetGuildCopyMapIndex(int fubenID)
		{
			return this.GuildCopyMapOrderList.IndexOf(fubenID);
		}

		// Token: 0x0600179B RID: 6043 RVA: 0x001727EC File Offset: 0x001709EC
		public int GetNextGuildCopyMapIndex(int fubenID)
		{
			int result;
			if (fubenID == this.LastGuildCopyMapOrder)
			{
				result = -1;
			}
			else
			{
				int index = this.GetGuildCopyMapIndex(fubenID);
				if (index < 0)
				{
					result = -1;
				}
				else
				{
					result = this.GetGuildCopyMapOrderByIndex(index + 1);
				}
			}
			return result;
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x00172834 File Offset: 0x00170A34
		public int GetGuildCopyMapOrderByIndex(int index)
		{
			int result;
			if (index < 0 || index >= this.GuildCopyMapOrderList.Count)
			{
				result = -1;
			}
			else
			{
				result = this.GuildCopyMapOrderList[index];
			}
			return result;
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x00172870 File Offset: 0x00170A70
		public bool GetGuildCopyMapAwardDayFlag(int Flag, int day, int index)
		{
			return (Flag >> day * 2 & index) == index;
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x00172894 File Offset: 0x00170A94
		public int SetGuildCopyMapAwardDayFlag(int Flag, int day, int index)
		{
			return Flag | index << day * 2;
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x001728B0 File Offset: 0x00170AB0
		public void UpdateGuildCopyMap(int guildid, int fubenid, int seqid, int mapcode)
		{
			GuildCopyMap CopyMap = new GuildCopyMap
			{
				GuildID = guildid,
				FuBenID = fubenid,
				SeqID = seqid,
				MapCode = mapcode
			};
			this.UpdateGuildCopyMap(guildid, CopyMap);
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x001728EC File Offset: 0x00170AEC
		public void UpdateGuildCopyMap(int guildid, GuildCopyMap CopyMap)
		{
			lock (this.GuildCopyMapDict)
			{
				this.GuildCopyMapDict[guildid] = CopyMap;
			}
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x00172940 File Offset: 0x00170B40
		public GuildCopyMap FindGuildCopyMap(int guildid)
		{
			GuildCopyMap CopyMap = null;
			lock (this.GuildCopyMapDict)
			{
				if (this.GuildCopyMapDict.ContainsKey(guildid))
				{
					CopyMap = this.GuildCopyMapDict[guildid];
				}
			}
			return CopyMap;
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x001729B4 File Offset: 0x00170BB4
		public GuildCopyMap FindActiveGuildCopyMap()
		{
			GuildCopyMap CopyMap = null;
			lock (this.GuildCopyMapDict)
			{
				using (Dictionary<int, GuildCopyMap>.Enumerator enumerator = this.GuildCopyMapDict.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						KeyValuePair<int, GuildCopyMap> map = enumerator.Current;
						return map.Value;
					}
				}
			}
			return CopyMap;
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x00172A54 File Offset: 0x00170C54
		public GuildCopyMap FindGuildCopyMapBySeqID(int seqid)
		{
			lock (this.GuildCopyMapDict)
			{
				foreach (KeyValuePair<int, GuildCopyMap> map in this.GuildCopyMapDict)
				{
					if (seqid == map.Value.SeqID)
					{
						return map.Value;
					}
				}
			}
			return null;
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x00172B0C File Offset: 0x00170D0C
		public void RemoveGuildCopyMap(int guildid)
		{
			this.GuildCopyMapDict.Remove(guildid);
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x00172B1C File Offset: 0x00170D1C
		public void CheckCurrGuildCopyMap(GameClient client, out int fubenid, out int seqid, int mapcode)
		{
			fubenid = -1;
			seqid = -1;
			int guildid = client.ClientData.Faction;
			GuildCopyMapDB data = GameManager.GuildCopyMapDBMgr.FindGuildCopyMapDB(guildid, client.ServerId);
			if (null != data)
			{
				DateTime openTime = Global.GetRealDate(data.OpenDay);
				if (Global.BeginOfWeek(openTime) != Global.BeginOfWeek(TimeUtil.NowDateTime()))
				{
					GameManager.GuildCopyMapDBMgr.ResetGuildCopyMapDB(guildid, client.ServerId);
					fubenid = this.FirstGuildCopyMapOrder;
				}
				else if (data.FuBenID >= this.LastGuildCopyMapOrder && data.State == 2)
				{
					fubenid = 0;
				}
				else if (data.State == 2)
				{
					data.FuBenID = this.GetNextGuildCopyMapIndex(data.FuBenID);
					data.State = 0;
					data.OpenDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
					if (GameManager.GuildCopyMapDBMgr.UpdateGuildCopyMapDB(data, client.ServerId))
					{
						fubenid = data.FuBenID;
					}
					this.UpdateGuildCopyMap(guildid, fubenid, -1, -1);
				}
				else
				{
					fubenid = data.FuBenID;
					GuildCopyMap CopyMap = this.FindGuildCopyMap(guildid);
					if (null != CopyMap)
					{
						seqid = CopyMap.SeqID;
					}
				}
			}
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x00172C6C File Offset: 0x00170E6C
		public void EnterGuildCopyMap(GameClient client, out int fubenid, out int seqid, int mapcode)
		{
			fubenid = -1;
			seqid = -1;
			int guildid = client.ClientData.Faction;
			lock (this.GuildCopyMapDict)
			{
				this.CheckCurrGuildCopyMap(client, out fubenid, out seqid, mapcode);
				if (seqid < 0)
				{
					string[] dbFields = Global.ExecuteDBCmd(10049, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (dbFields != null && dbFields.Length >= 2)
					{
						seqid = Global.SafeConvertToInt32(dbFields[1]);
						if (seqid > 0)
						{
							this.UpdateGuildCopyMap(guildid, fubenid, seqid, mapcode);
						}
					}
				}
			}
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x00172D4C File Offset: 0x00170F4C
		public void ProcessMonsterDead(GameClient client, Monster monster)
		{
			if (this.IsGuildCopyMap(monster.CurrentMapCode))
			{
				SystemXmlItem systemFuBenItem = null;
				if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(monster.CurrentMapCode, out systemFuBenItem))
				{
					if (null != systemFuBenItem)
					{
						int nBossID = systemFuBenItem.GetIntValue("BossID", -1);
						if (nBossID == monster.MonsterInfo.ExtensionID)
						{
							CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(monster.CurrentCopyMapID);
							if (null == copyMap)
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == copyMap), CurrentCopyMapID={0}", monster.CurrentCopyMapID), null, true);
							}
							else
							{
								GuildCopyMap mapData = GameManager.GuildCopyMapMgr.FindGuildCopyMapBySeqID(copyMap.FuBenSeqID);
								if (null == mapData)
								{
									LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == mapData), copyMap.FuBenSeqID={0}", copyMap.FuBenSeqID), null, true);
								}
								else
								{
									int guildid = mapData.GuildID;
									GuildCopyMapDB data = GameManager.GuildCopyMapDBMgr.FindGuildCopyMapDB(guildid, client.ServerId);
									if (null == data)
									{
										LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == data), guildid={0}", client.ClientData.Faction), null, true);
									}
									else
									{
										List<GameClient> objsList = copyMap.GetClientsList();
										if (objsList == null || objsList.Count <= 0)
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapManager::ProcessMonsterDead (null == objsList || objsList.Count <= 0), CurrentCopyMapID={0}", monster.CurrentCopyMapID), null, true);
										}
										else
										{
											if (copyMap.FubenMapID >= data.FuBenID)
											{
												data.FuBenID = copyMap.FubenMapID;
												data.State = 2;
												if (copyMap.FubenMapID == GameManager.GuildCopyMapMgr.FirstGuildCopyMapOrder)
												{
													data.Killers = monster.WhoKillMeName;
												}
												else
												{
													GuildCopyMapDB guildCopyMapDB = data;
													guildCopyMapDB.Killers += ",";
													GuildCopyMapDB guildCopyMapDB2 = data;
													guildCopyMapDB2.Killers += monster.WhoKillMeName;
												}
											}
											GlobalEventSource.getInstance().fireEvent(ZhanMengShijianEvent.createKillBossEvent(Global.FormatRoleName4(client), client.ClientData.Faction, monster.MonsterInfo.ExtensionID, client.ServerId));
											if (!GameManager.GuildCopyMapDBMgr.UpdateGuildCopyMapDB(data, client.ServerId))
											{
												string logStr = "GuildCopyMapManager::ProcessMonsterDead (false == result), \r\n                        data.GuildID={0}, data.FuBenID={1}, data.State={2}, data.OpenDay={3}, data.Killers={4}";
												LogManager.WriteLog(LogTypes.Error, string.Format(logStr, new object[]
												{
													data.GuildID,
													data.FuBenID,
													data.State,
													data.OpenDay,
													data.Killers
												}), null, true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060017A8 RID: 6056 RVA: 0x00173030 File Offset: 0x00171230
		public int GetZhanGongAward(GameClient client, int fubenid, int awardZhanGong)
		{
			int result;
			if (!this.IsGuildCopyMap(fubenid))
			{
				result = 0;
			}
			else
			{
				int nGuildCopyMapAwardDay = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardDay");
				DateTime AwardTime = Global.GetRealDate(nGuildCopyMapAwardDay);
				if (Global.BeginOfWeek(AwardTime) != Global.BeginOfWeek(TimeUtil.NowDateTime()))
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", 0, true);
				}
				int nGuildCopyMapAwardFlag = Global.GetRoleParamsInt32FromDB(client, "GuildCopyMapAwardFlag");
				bool flag = this.GetGuildCopyMapAwardDayFlag(nGuildCopyMapAwardFlag, this.GetGuildCopyMapIndex(fubenid), 1);
				if (flag)
				{
					result = -1;
				}
				else
				{
					nGuildCopyMapAwardFlag = this.SetGuildCopyMapAwardDayFlag(nGuildCopyMapAwardFlag, this.GetGuildCopyMapIndex(fubenid), 1);
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", nGuildCopyMapAwardFlag, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardDay", Global.GetOffsetDay(TimeUtil.NowDateTime()), true);
					result = awardZhanGong;
				}
			}
			return result;
		}

		// Token: 0x040021A4 RID: 8612
		private const int firstGuildCopyMapOrder = 40000;

		// Token: 0x040021A5 RID: 8613
		private Dictionary<int, GuildCopyMap> GuildCopyMapDict = new Dictionary<int, GuildCopyMap>();

		// Token: 0x040021A6 RID: 8614
		private List<int> guildCopyMapOrderList = new List<int>();

		// Token: 0x040021A7 RID: 8615
		private int lastGuildCopyMapOrder = 40006;

		// Token: 0x040021A8 RID: 8616
		private int maxDamageSendCount = 5;

		// Token: 0x040021A9 RID: 8617
		public long lastProcessEndTicks = 0L;

		// Token: 0x040021AA RID: 8618
		public bool ProcessEndFlag = false;
	}
}
