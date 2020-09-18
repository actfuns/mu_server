using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class BulletinMsgManager
	{
		
		public void LoadBulletinMsgFromDBServer()
		{
			this._BulletinMsgDict = Global.LoadDBBulletinMsgDict();
			if (null == this._BulletinMsgDict)
			{
				this._BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
			}
		}

		
		public BulletinMsgData AddBulletinMsg(string msgID, int playMinutes, int playNum, string bulletinText, int msgType = 0)
		{
			BulletinMsgData bulletinMsgData = new BulletinMsgData
			{
				MsgID = msgID,
				PlayMinutes = playMinutes,
				ToPlayNum = playNum,
				BulletinText = bulletinText,
				BulletinTicks = TimeUtil.NOW(),
				MsgType = msgType
			};
			if (playMinutes != 0)
			{
				lock (this._BulletinMsgDict)
				{
					this._BulletinMsgDict[msgID] = bulletinMsgData;
				}
				if (playMinutes < 0)
				{
					string fromDate = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
					string toDate = TimeUtil.NowDateTime().AddMinutes((double)playMinutes).ToString("yyyy-MM-dd HH:mm:ss");
					Global.AddDBBulletinMsg(msgID, fromDate, toDate, 0, bulletinText);
				}
			}
			return bulletinMsgData;
		}

		
		public BulletinMsgData AddBulletinMsgBackground(string msgID, string fromDate, string toDate, int interval, string bulletinText)
		{
			BulletinMsgData bulletinMsgData = new BulletinMsgData
			{
				MsgID = msgID,
				Interval = interval,
				BulletinText = bulletinText,
				BulletinTicks = DataHelper.ConvertToTicks(fromDate)
			};
			long PlayTicks = DataHelper.ConvertToTicks(toDate) - bulletinMsgData.BulletinTicks;
			bulletinMsgData.PlayMinutes = (int)(PlayTicks / 60000L);
			BulletinMsgData result;
			if (string.IsNullOrEmpty(msgID) || PlayTicks < 0L || interval <= 0)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("后台公告数据错误:{0} {1} {2} {3} {4}", new object[]
				{
					msgID,
					fromDate,
					toDate,
					interval,
					bulletinText
				}), null, true);
				result = null;
			}
			else
			{
				lock (this._BulletinMsgDict)
				{
					this._BulletinMsgDict[msgID] = bulletinMsgData;
					Global.AddDBBulletinMsg(msgID, fromDate, toDate, interval, bulletinText);
				}
				result = bulletinMsgData;
			}
			return result;
		}

		
		public BulletinMsgData RemoveBulletinMsg(string msgID)
		{
			BulletinMsgData bulletinMsgData = null;
			lock (this._BulletinMsgDict)
			{
				if (this._BulletinMsgDict.TryGetValue(msgID, out bulletinMsgData))
				{
					this._BulletinMsgDict.Remove(msgID);
					if (bulletinMsgData.PlayMinutes < 0 || bulletinMsgData.Interval > 0)
					{
						Global.RemoveDBBulletinMsg(msgID);
					}
				}
			}
			return bulletinMsgData;
		}

		
		public void SendAllBulletinMsg(GameClient client)
		{
			long ticks = TimeUtil.NOW();
			List<BulletinMsgData> bulletinMsgDataList = new List<BulletinMsgData>();
			lock (this._BulletinMsgDict)
			{
				foreach (BulletinMsgData value in this._BulletinMsgDict.Values)
				{
					if (ticks >= value.BulletinTicks)
					{
						bulletinMsgDataList.Add(value);
					}
				}
			}
			for (int i = 0; i < bulletinMsgDataList.Count; i++)
			{
				GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgDataList[i]);
			}
		}

		
		public void SendAllBulletinMsgToGM(GameClient client)
		{
			List<string> msgList = new List<string>();
			lock (this._BulletinMsgDict)
			{
				foreach (string key in this._BulletinMsgDict.Keys)
				{
					BulletinMsgData bulletinMsgData = this._BulletinMsgDict[key];
					string bulletinTm = new DateTime(bulletinMsgData.BulletinTicks * 10000L).ToString("yyyy-MM-dd HH:mm:ss");
					string textMsg = string.Format("{0} {1} {2} {3} {4} {5}", new object[]
					{
						bulletinMsgData.MsgID,
						bulletinMsgData.PlayMinutes,
						bulletinMsgData.ToPlayNum,
						bulletinMsgData.Interval,
						bulletinTm,
						bulletinMsgData.BulletinText
					});
					msgList.Add(textMsg);
				}
			}
			for (int i = 0; i < msgList.Count; i++)
			{
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgList[i]);
			}
		}

		
		public void ProcessBulletinMsg()
		{
			long ticks = TimeUtil.NOW();
			List<string> bulletinMsgIDList = new List<string>();
			lock (this._BulletinMsgDict)
			{
				foreach (string key in this._BulletinMsgDict.Keys)
				{
					BulletinMsgData bulletinMsgData = this._BulletinMsgDict[key];
					if (bulletinMsgData.PlayMinutes >= 0)
					{
						if (bulletinMsgData.Interval > 0 && ticks >= bulletinMsgData.BulletinTicks && ticks - bulletinMsgData.LastBulletinTicks >= (long)(bulletinMsgData.Interval * 1000))
						{
							bulletinMsgData.LastBulletinTicks = ticks;
							BulletinMsgData msg = new BulletinMsgData
							{
								MsgID = bulletinMsgData.MsgID,
								PlayMinutes = 0,
								ToPlayNum = 1,
								BulletinText = bulletinMsgData.BulletinText,
								BulletinTicks = bulletinMsgData.BulletinTicks,
								MsgType = 0
							};
							GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, msg, 0, 0);
						}
						if (ticks - bulletinMsgData.BulletinTicks >= (long)bulletinMsgData.PlayMinutes * 60L * 1000L)
						{
							Global.RemoveDBBulletinMsg(key);
							bulletinMsgIDList.Add(key);
						}
					}
				}
				for (int i = 0; i < bulletinMsgIDList.Count; i++)
				{
					this._BulletinMsgDict.Remove(bulletinMsgIDList[i]);
				}
				bulletinMsgIDList.Clear();
				bulletinMsgIDList = null;
			}
		}

		
		private Dictionary<string, BulletinMsgData> _BulletinMsgDict = new Dictionary<string, BulletinMsgData>();
	}
}
