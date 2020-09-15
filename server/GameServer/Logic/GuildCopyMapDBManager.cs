using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020004F9 RID: 1273
	public class GuildCopyMapDBManager
	{
		// Token: 0x060017AB RID: 6059 RVA: 0x00173140 File Offset: 0x00171340
		public GuildCopyMapDB FindGuildCopyMapDB(int guildid, int serverId)
		{
			GuildCopyMapDB result;
			if (guildid <= 0)
			{
				result = null;
			}
			else
			{
				GuildCopyMapDB data = null;
				lock (this.GuildCopyMapDBDict)
				{
					if (this.GuildCopyMapDBDict.ContainsKey(guildid))
					{
						data = this.GuildCopyMapDBDict[guildid];
					}
					else
					{
						string[] dbFields = null;
						string strDbCmd = string.Format("{0}", guildid);
						TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 711, strDbCmd, out dbFields, serverId);
						if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB dbRequestResult == TCPProcessCmdResults.RESULT_FAILED strDbCmd={0}", strDbCmd), null, true);
							return null;
						}
						if (dbFields.Length < 5 || Convert.ToInt32(dbFields[0]) <= 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB 参数数量错误或失败 strDbCmd={0}, dbFields.Length={1}", strDbCmd, dbFields.Length), null, true);
							return null;
						}
						try
						{
							data = new GuildCopyMapDB
							{
								GuildID = Convert.ToInt32(dbFields[0]),
								FuBenID = Convert.ToInt32(dbFields[1]),
								State = Convert.ToInt32(dbFields[2]),
								OpenDay = Convert.ToInt32(dbFields[3]),
								Killers = dbFields[4]
							};
							this.AddGuildCopyMapDB(data);
							if (guildid != Convert.ToInt32(dbFields[0]))
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB DB返回的id不符，guildid={0}, dbFields[0]={1}", guildid, Convert.ToInt32(dbFields[0])), null, true);
								return null;
							}
						}
						catch (Exception)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::FindGuildCopyMapDB参数解析失败？", new object[0]), null, true);
							return null;
						}
					}
				}
				result = data;
			}
			return result;
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x00173350 File Offset: 0x00171550
		public void AddGuildCopyMapDB(GuildCopyMapDB data)
		{
			if (!this.GuildCopyMapDBDict.ContainsKey(data.GuildID))
			{
				this.GuildCopyMapDBDict[data.GuildID] = data;
			}
		}

		// Token: 0x060017AD RID: 6061 RVA: 0x0017338C File Offset: 0x0017158C
		public bool UpdateGuildCopyMapDB(GuildCopyMapDB data, int serverId)
		{
			GuildCopyMapDB oldData = this.FindGuildCopyMapDB(data.GuildID, serverId);
			bool result;
			if (null == oldData)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::UpdateGuildCopyMapDB null == oldData data.GuildID={0}", data.GuildID), null, true);
				result = false;
			}
			else
			{
				lock (this.GuildCopyMapDBDict)
				{
					string[] dbFields = null;
					string strDbCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						data.GuildID,
						data.FuBenID,
						data.State,
						data.OpenDay,
						data.Killers
					});
					TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10172, strDbCmd, out dbFields, serverId);
					if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::ResetGuildCopyMapDB dbRequestResult == TCPProcessCmdResults.RESULT_FAILED strDbCmd={0}", strDbCmd), null, true);
						return false;
					}
					if (dbFields.Length < 1 || Convert.ToInt32(dbFields[0]) != 1)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("GuildCopyMapDBManager::ResetGuildCopyMapDB 参数数量错误或失败 strDbCmd={0}, dbFields.Length={1}", strDbCmd, dbFields.Length), null, true);
						return false;
					}
					this.GuildCopyMapDBDict[data.GuildID] = data;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060017AE RID: 6062 RVA: 0x00173524 File Offset: 0x00171724
		public void ResetGuildCopyMapDB(int guildid, int serverId)
		{
			GuildCopyMapDB data = new GuildCopyMapDB
			{
				GuildID = guildid,
				FuBenID = GameManager.GuildCopyMapMgr.FirstGuildCopyMapOrder,
				State = 0,
				OpenDay = Global.GetOffsetDay(TimeUtil.NowDateTime()),
				Killers = ""
			};
			this.UpdateGuildCopyMapDB(data, serverId);
		}

		// Token: 0x040021B4 RID: 8628
		private Dictionary<int, GuildCopyMapDB> GuildCopyMapDBDict = new Dictionary<int, GuildCopyMapDB>();
	}
}
