using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class GroupMailManager
	{
		
		public static void ResetData()
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.LastMaxGroupMailID = 0;
				GroupMailManager.GroupMailDataDict.Clear();
			}
		}

		
		public static void RequestNewGroupMailList()
		{
			List<GroupMailData> GroupMailList = Global.sendToDB<List<GroupMailData>, string>(10177, string.Format("{0}", GroupMailManager.LastMaxGroupMailID), 0);
			lock (GroupMailManager.GroupMailDataDict)
			{
				if (GroupMailList != null && GroupMailList.Count > 0)
				{
					foreach (GroupMailData item in GroupMailList)
					{
						GroupMailManager.AddGroupMailData(item);
						if (item.GMailID > GroupMailManager.LastMaxGroupMailID)
						{
							GroupMailManager.LastMaxGroupMailID = item.GMailID;
						}
					}
				}
			}
		}

		
		private static void AddGroupMailData(GroupMailData gmailData)
		{
			lock (GroupMailManager.GroupMailDataDict_Mutex)
			{
				GroupMailManager.GroupMailDataDict[gmailData.GMailID] = gmailData;
			}
		}

		
		private static bool InConditions(GameClient client, GroupMailData gmailData)
		{
			bool result;
			if (string.IsNullOrEmpty(gmailData.Conditions))
			{
				result = true;
			}
			else
			{
				long currTicks = TimeUtil.NOW() * 10000L;
				if (currTicks < gmailData.InputTime || currTicks > gmailData.EndTime)
				{
					result = false;
				}
				else
				{
					string[] strFields = gmailData.Conditions.Split(new char[]
					{
						'|'
					});
					bool bError = false;
					for (int i = 0; i < strFields.Length; i++)
					{
						string[] strKey = strFields[i].Split(new char[]
						{
							','
						});
						if ("level" == strKey[0])
						{
							if (strKey.Length != 2)
							{
								bError = true;
								break;
							}
							int currLvl = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
							int cfgLvl = Global.SafeConvertToInt32(strKey[1]);
							if (currLvl < cfgLvl)
							{
								return false;
							}
						}
						else if ("levelrange" == strKey[0])
						{
							if (strKey.Length != 3)
							{
								bError = true;
								break;
							}
							int currLvl = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
							int cfgLvlMin = Global.SafeConvertToInt32(strKey[1]);
							int cfgLvlMax = Global.SafeConvertToInt32(strKey[2]);
							if (currLvl < cfgLvlMin || currLvl > cfgLvlMax)
							{
								return false;
							}
						}
						else if ("loginrange" == strKey[0])
						{
							if (strKey.Length != 3)
							{
								bError = true;
								break;
							}
							string strBeginTime = strKey[1];
							string strEndTime = strKey[2];
							DateTime beginTime = DateTime.Parse(strBeginTime);
							DateTime endTime = DateTime.Parse(strEndTime);
							if (!Global.CheckRoleIsLoginByTime(client, beginTime, endTime))
							{
								if (TimeUtil.NOW() * 10000L > endTime.Ticks)
								{
									GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, -1);
								}
								return false;
							}
						}
						else if ("vip" == strKey[0])
						{
							if (strKey.Length != 3)
							{
								bError = true;
								break;
							}
							int currLvl = client.ClientData.VipLevel;
							int cfgLvlMin = Global.SafeConvertToInt32(strKey[1]);
							int cfgLvlMax = Global.SafeConvertToInt32(strKey[2]);
							if (currLvl < cfgLvlMin || currLvl > cfgLvlMax)
							{
								return false;
							}
						}
						else
						{
							if (string.IsNullOrEmpty(strKey[0]))
							{
								break;
							}
							LogManager.WriteLogUseCache(LogTypes.Error, string.Format("GroupMailManager::InConditions Error Conditions={0}", gmailData.Conditions));
							return false;
						}
					}
					if (bError)
					{
						GroupMailManager.SetGMailIsSend(client, gmailData.GMailID);
						LogManager.WriteLog(LogTypes.Error, string.Format("GroupMailManager::InConditions Error Conditions={0}", gmailData.Conditions), null, true);
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			return result;
		}

		
		public static void CheckRoleGroupMail(GameClient client)
		{
			long currTicks = TimeUtil.NOW();
			if (client.LastCheckGMailTick + 60000L <= currTicks)
			{
				client.LastCheckGMailTick = currTicks;
				lock (GroupMailManager.GroupMailDataDict)
				{
					foreach (KeyValuePair<int, GroupMailData> item in GroupMailManager.GroupMailDataDict)
					{
						if (!GroupMailManager.IfGMailIsSend(client, item.Value.GMailID))
						{
							if (GroupMailManager.InConditions(client, item.Value))
							{
								GroupMailManager.SendGMail2Role(client, item.Value);
							}
						}
					}
				}
			}
		}

		
		private static void SendGMail2Role(GameClient client, GroupMailData gmailData)
		{
			if (GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, 0))
			{
				int mailID = Global.UseMailGivePlayerAward2(client, gmailData.GoodsList, gmailData.Subject, gmailData.Content, gmailData.Yinliang, gmailData.Tongqian, gmailData.YuanBao);
				GroupMailManager.SetGMailNeverSend(client, gmailData.GMailID, mailID);
			}
		}

		
		public static bool IfGMailIsSend(GameClient client, int gmailID)
		{
			bool result;
			lock (client.ClientData.GroupMailRecordList)
			{
				result = (client.ClientData.GroupMailRecordList.IndexOf(gmailID) >= 0);
			}
			return result;
		}

		
		public static void SetGMailIsSend(GameClient client, int gmailID)
		{
			lock (client.ClientData.GroupMailRecordList)
			{
				if (client.ClientData.GroupMailRecordList.IndexOf(gmailID) < 0)
				{
					client.ClientData.GroupMailRecordList.Add(gmailID);
				}
			}
		}

		
		public static bool SetGMailNeverSend(GameClient client, int gmailID, int mailID)
		{
			string dbCmds = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, gmailID, mailID);
			string[] dbFields = null;
			Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10178, dbCmds, out dbFields, client.ServerId);
			bool result2;
			if (dbFields == null || dbFields.Length != 1)
			{
				result2 = false;
			}
			else
			{
				int result = Convert.ToInt32(dbFields[0]);
				if (result <= 0)
				{
					result2 = false;
				}
				else
				{
					GroupMailManager.SetGMailIsSend(client, gmailID);
					result2 = true;
				}
			}
			return result2;
		}

		
		private static int LastMaxGroupMailID = 0;

		
		private static object GroupMailDataDict_Mutex = new object();

		
		private static Dictionary<int, GroupMailData> GroupMailDataDict = new Dictionary<int, GroupMailData>();
	}
}
