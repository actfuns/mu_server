using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Building;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LoginWaiting;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.MoRi;
using GameServer.Logic.MUWings;
using GameServer.Logic.Name;
using GameServer.Logic.Olympics;
using GameServer.Logic.OnePiece;
using GameServer.Logic.ProtoCheck;
using GameServer.Logic.Reborn;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.Spread;
using GameServer.Logic.Talent;
using GameServer.Logic.Tarot;
using GameServer.Logic.Ten;
using GameServer.Logic.Today;
using GameServer.Logic.TuJian;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserMoneyCharge;
using GameServer.Logic.UserReturn;
using GameServer.Logic.WanMota;
using GameServer.Logic.YueKa;
using GameServer.Logic.ZhuanPan;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	// Token: 0x020006D7 RID: 1751
	public class GMCommands
	{
		// Token: 0x060029AB RID: 10667 RVA: 0x0023A0C4 File Offset: 0x002382C4
		public void InitGMCommands(XElement xml)
		{
			if (null == xml)
			{
				try
				{
					xml = XElement.Load("GMConfig.xml");
				}
				catch (Exception)
				{
					return;
				}
			}
			string superUserNames = Global.GetSafeAttributeStr(xml, "GameManager", "SuperUserNames");
			string userNames = Global.GetSafeAttributeStr(xml, "GameManager", "UserNames");
			string gmIPs = Global.GetSafeAttributeStr(xml, "GameManager", "IPs");
			if (!string.IsNullOrEmpty(superUserNames))
			{
				this.SuperGMUserNames = superUserNames.Trim().Split(new char[]
				{
					','
				});
			}
			if (!string.IsNullOrEmpty(userNames))
			{
				this.GMUserNames = userNames.Trim().Split(new char[]
				{
					','
				});
			}
			if (!string.IsNullOrEmpty(gmIPs))
			{
				this.GMIPs = gmIPs.Trim().Split(new char[]
				{
					','
				});
			}
			Dictionary<string, int> dict = new Dictionary<string, int>();
			XElement otherNames = Global.GetXElement(xml, "OtherNames");
			if (null != otherNames)
			{
				IEnumerable<XElement> names = otherNames.Elements();
				foreach (XElement key in names)
				{
					dict[Global.GetSafeAttributeStr(key, "Name")] = (int)Global.GetSafeAttributeLong(key, "Priority");
				}
			}
			this.OtherUserNamesDict = dict;
			Dictionary<int, string[]> dict2 = new Dictionary<int, string[]>();
			XElement prioritys = Global.GetXElement(xml, "Prioritys");
			if (null != prioritys)
			{
				IEnumerable<XElement> items = prioritys.Elements();
				foreach (XElement key in items)
				{
					dict2[(int)Global.GetSafeAttributeLong(key, "ID")] = Global.GetSafeAttributeStr(key, "Cmds").Split(new char[]
					{
						','
					});
				}
			}
			this.GMCmdsDict = dict2;
		}

		// Token: 0x060029AC RID: 10668 RVA: 0x0023A310 File Offset: 0x00238510
		public int ReloadGMCommands()
		{
			XElement xml = null;
			try
			{
				xml = XElement.Load("GMConfig.xml");
			}
			catch (Exception)
			{
				return -1;
			}
			GameManager.systemGMCommands.InitGMCommands(xml);
			return 0;
		}

		// Token: 0x060029AD RID: 10669 RVA: 0x0023A358 File Offset: 0x00238558
		public bool IsSuperGMUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsSuperGMUser(userName);
		}

		// Token: 0x060029AE RID: 10670 RVA: 0x0023A380 File Offset: 0x00238580
		public bool IsSuperGMUser(string userName)
		{
			bool result;
			if (string.IsNullOrEmpty(userName))
			{
				result = false;
			}
			else
			{
				string[] userNames = this.SuperGMUserNames;
				if (userNames == null || userNames.Length <= 0)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < userNames.Length; i++)
					{
						if (userNames[i] == userName)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060029AF RID: 10671 RVA: 0x0023A3EC File Offset: 0x002385EC
		public bool IsGMUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsGMUser(userName);
		}

		// Token: 0x060029B0 RID: 10672 RVA: 0x0023A414 File Offset: 0x00238614
		public bool IsGMUser(string userName)
		{
			bool result;
			if (string.IsNullOrEmpty(userName))
			{
				result = false;
			}
			else
			{
				string[] userNames = this.GMUserNames;
				if (userNames == null || userNames.Length <= 0)
				{
					result = false;
				}
				else
				{
					for (int i = 0; i < userNames.Length; i++)
					{
						if (userNames[i].IndexOf("*") >= 0)
						{
							return true;
						}
						if (userNames[i] == userName)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060029B1 RID: 10673 RVA: 0x0023A498 File Offset: 0x00238698
		public int IsPriorityUser(TMSKSocket socket)
		{
			string userName = GameManager.OnlineUserSession.FindUserName(socket);
			return this.IsPriorityUser(userName);
		}

		// Token: 0x060029B2 RID: 10674 RVA: 0x0023A4C0 File Offset: 0x002386C0
		public int IsPriorityUser(string userName)
		{
			int result;
			if (string.IsNullOrEmpty(userName))
			{
				result = -1;
			}
			else
			{
				Dictionary<string, int> dict = this.OtherUserNamesDict;
				if (dict == null || dict.Count <= 0)
				{
					result = -1;
				}
				else
				{
					int priority = -1;
					if (!dict.TryGetValue(userName, out priority))
					{
						priority = -1;
					}
					result = priority;
				}
			}
			return result;
		}

		// Token: 0x060029B3 RID: 10675 RVA: 0x0023A518 File Offset: 0x00238718
		private bool CanExecCmd(int priority, string cmd)
		{
			bool result;
			if (priority < 0)
			{
				result = true;
			}
			else if (string.IsNullOrEmpty(cmd))
			{
				result = false;
			}
			else
			{
				Dictionary<int, string[]> dict = this.GMCmdsDict;
				if (dict == null || dict.Count <= 0)
				{
					result = false;
				}
				else
				{
					string[] cmds = null;
					if (!dict.TryGetValue(priority, out cmds))
					{
						result = false;
					}
					else if (cmds == null || cmds.Length <= 0)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < cmds.Length; i++)
						{
							if (cmds[i] == cmd)
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060029B4 RID: 10676 RVA: 0x0023A5D4 File Offset: 0x002387D4
		public bool IsValidIP(TMSKSocket socket)
		{
			string ip = Global.GetSocketRemoteEndPoint(socket, true);
			bool result;
			if (string.IsNullOrEmpty(ip))
			{
				result = false;
			}
			else
			{
				string[] ipFields = ip.Split(new char[]
				{
					':'
				});
				if (ipFields.Length <= 0)
				{
					result = false;
				}
				else
				{
					string[] IPs = this.GMIPs;
					if (IPs == null || IPs.Length <= 0)
					{
						result = false;
					}
					else
					{
						for (int i = 0; i < IPs.Length; i++)
						{
							if (IPs[i].IndexOf("*") >= 0)
							{
								return true;
							}
							if (IPs[i] == ipFields[0])
							{
								return true;
							}
						}
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060029B5 RID: 10677 RVA: 0x0023A69C File Offset: 0x0023889C
		public bool RegisterGmCmdHandler(string gmCmd, GmCmdHandler handler)
		{
			bool result;
			if (string.IsNullOrEmpty(gmCmd))
			{
				result = false;
			}
			else if (null == handler)
			{
				result = false;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					if (this.GmCmdsHandlerDict.ContainsKey(gmCmd))
					{
						result = false;
					}
					else
					{
						this.GmCmdsHandlerDict[gmCmd] = handler;
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060029B6 RID: 10678 RVA: 0x0023A728 File Offset: 0x00238928
		public bool RemoveGmCmdHandler(GmCmdHandler handler)
		{
			bool result;
			if (null == handler)
			{
				result = false;
			}
			else if (string.IsNullOrEmpty(handler.gmCmd))
			{
				result = false;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					GmCmdHandler handler2;
					if (!this.GmCmdsHandlerDict.TryGetValue(handler.gmCmd, out handler2) || handler2 != handler)
					{
						result = false;
					}
					else
					{
						this.GmCmdsHandlerDict.Remove(handler.gmCmd);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060029B7 RID: 10679 RVA: 0x0023A7D0 File Offset: 0x002389D0
		public GmCmdHandler GetHandler(string gmCmd)
		{
			GmCmdHandler result;
			if (string.IsNullOrEmpty(gmCmd))
			{
				result = null;
			}
			else
			{
				lock (this.GmCmdsHandlerDict)
				{
					GmCmdHandler handler;
					if (this.GmCmdsHandlerDict.TryGetValue(gmCmd, out handler))
					{
						return handler;
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060029B8 RID: 10680 RVA: 0x0023A848 File Offset: 0x00238A48
		public void OnClientLogin(GameClient client)
		{
			if (this.IsPriorityUser(client.strUserName) == 2)
			{
				client.ClientData.HideGM = 1;
				client.ClientData.MapCode = 6090;
				client.ClientData.PosX = 1000;
				client.ClientData.PosY = 1000;
				lock (this.GMClientList)
				{
					if (!this.GMClientList.Contains(client))
					{
						this.GMClientList.Add(client);
					}
				}
			}
		}

		// Token: 0x060029B9 RID: 10681 RVA: 0x0023A908 File Offset: 0x00238B08
		public void OnClientLogout(GameClient client)
		{
			if (this.IsPriorityUser(client.strUserName) == 2)
			{
				lock (this.GMClientList)
				{
					this.GMClientList.Remove(client);
				}
			}
		}

		// Token: 0x060029BA RID: 10682 RVA: 0x0023A970 File Offset: 0x00238B70
		public void BroadcastChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			List<GameClient> objsList;
			lock (this.GMClientList)
			{
				int count = this.GMClientList.Count;
				if (count <= 0)
				{
					return;
				}
				objsList = this.GMClientList.GetRange(0, count);
			}
			TCPOutPacket tcpOutPacket = null;
			try
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (client != objsList[i])
					{
						if (!objsList[i].LogoutState)
						{
							if (null == tcpOutPacket)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdText, 157);
							}
							if (!sl.SendData(objsList[i].ClientSocket, tcpOutPacket, false))
							{
							}
						}
					}
				}
			}
			finally
			{
				Global.PushBackTcpOutPacket(tcpOutPacket);
			}
		}

		// Token: 0x060029BB RID: 10683 RVA: 0x0023AA84 File Offset: 0x00238C84
		public bool ProcessChatMessage(TMSKSocket socket, GameClient client, string text, bool transmit)
		{
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else if (text[0] != '-')
			{
				result = false;
			}
			else if (this.NormalGMMsg(socket, client, text, transmit))
			{
				result = false;
			}
			else
			{
				bool isSuperGMUser = false;
				int priority = -1;
				if (!transmit)
				{
					int gmPriority = socket.session.gmPriority;
					switch (gmPriority)
					{
					case 0:
						return true;
					case 1:
						break;
					default:
						if (gmPriority != 1000)
						{
							priority = socket.session.gmPriority;
						}
						else
						{
							isSuperGMUser = true;
						}
						break;
					}
				}
				string[] fields = text.Trim().Split(new char[]
				{
					' '
				});
				if (fields.Length <= 0)
				{
					result = true;
				}
				else if (!this.CanExecCmd(priority, fields[0]))
				{
					result = true;
				}
				else
				{
					GmCmdHandler handler = this.GetHandler(fields[0]);
					if (null != handler)
					{
						result = handler.Process(client, text, fields, transmit, isSuperGMUser);
					}
					else
					{
						result = this.ProcessGMCommands(client, text, fields, transmit, isSuperGMUser);
					}
				}
			}
			return result;
		}

		// Token: 0x060029BC RID: 10684 RVA: 0x0023ABB0 File Offset: 0x00238DB0
		public bool NormalGMMsg(TMSKSocket socket, GameClient client, string text, bool transmit)
		{
			return text == "-guanzhan";
		}

		// Token: 0x060029BD RID: 10685 RVA: 0x0023ABDC File Offset: 0x00238DDC
		private int SafeConvertToInt32(string str)
		{
			int ret = -1;
			try
			{
				ret = Convert.ToInt32(str);
			}
			catch (Exception ex)
			{
				ret = -1;
				LogManager.WriteException(ex.ToString());
			}
			return ret;
		}

		// Token: 0x060029BE RID: 10686 RVA: 0x0023AC20 File Offset: 0x00238E20
		private string GetServerBaseInfo(string cmd = null)
		{
			string strinfo = string.Format("在线数量 {0}/{1}", GameManager.ClientMgr.GetClientCount(), Global._TCPManager.MySocketListener.ConnectedSocketsCount);
			strinfo += "\r\n";
			int workerThreads = 0;
			int completionPortThreads = 0;
			ThreadPool.GetMaxThreads(out workerThreads, out completionPortThreads);
			strinfo += string.Format("线程池信息 workerThreads={0}, completionPortThreads={1}", workerThreads, completionPortThreads);
			strinfo += "\r\n";
			strinfo += string.Format("TCP事件读写缓存数量 readPool={0}/{2}, writePool={1}/{2}", Global._TCPManager.MySocketListener.ReadPoolCount, Global._TCPManager.MySocketListener.WritePoolCount, Global._TCPManager.MySocketListener.numConnections * 3);
			strinfo += "\r\n";
			strinfo += string.Format("数据库指令数量 {0}", GameManager.DBCmdMgr.GetDBCmdCount());
			strinfo += "\r\n";
			strinfo += string.Format("与DbServer的连接数量{0}/{1}", Global._TCPManager.tcpClientPool.GetPoolCount(), Global._TCPManager.tcpClientPool.InitCount);
			strinfo += "\r\n";
			strinfo += string.Format("TcpOutPacketPool个数 {0}, 实例 {1}, TcpInPacketPool个数{2}, 实例 {3}, TCPCmdWrapper个数 {4}, SendCmdWrapper {5}", new object[]
			{
				Global._TCPManager.TcpOutPacketPool.Count,
				TCPOutPacket.GetInstanceCount(),
				Global._TCPManager.TcpInPacketPool.Count,
				TCPInPacket.GetInstanceCount(),
				TCPCmdWrapper.GetTotalCount(),
				SendCmdWrapper.GetInstanceCount()
			});
			strinfo += "\r\n";
			string info = Global._MemoryManager.GetCacheInfoStr();
			strinfo += info;
			strinfo += "\r\n";
			info = Global._FullBufferManager.GetFullBufferInfoStr();
			strinfo += info;
			strinfo += "\r\n";
			info = Global._TCPManager.GetAllCacheCmdPacketInfo();
			return strinfo + info;
		}

		// Token: 0x060029BF RID: 10687 RVA: 0x0023AE58 File Offset: 0x00239058
		private string GetServerTCPInfo(string cmd = null)
		{
			bool clear = cmd.Contains("/c");
			bool detail = cmd.Contains("/d");
			string strinfo = "";
			DateTime now = TimeUtil.NowDateTime();
			strinfo += string.Format("当前时间:{0},统计时长:{1}", now.ToString("yyyy-MM-dd HH:mm:ss"), (now - ProcessSessionTask.StartTime).ToString());
			strinfo += "\r\n";
			if (clear)
			{
				detail = true;
				ProcessSessionTask.StartTime = now;
			}
			strinfo += string.Format("总接收字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesReadSize / 1048576.0);
			strinfo += "\r\n";
			strinfo += string.Format("总发送字节: {0:0.00} MB", (double)Global._TCPManager.MySocketListener.TotalBytesWriteSize / 1048576.0);
			strinfo += "\r\n";
			strinfo += string.Format("总处理指令个数 {0}", TCPCmdHandler.TotalHandledCmdsNum);
			strinfo += "\r\n";
			strinfo += string.Format("当前正在处理指令的线程数 {0}", TCPCmdHandler.GetHandlingCmdCount());
			strinfo += "\r\n";
			strinfo += string.Format("单个指令消耗的最大时间 {0}", TCPCmdHandler.MaxUsedTicksByCmdID);
			strinfo += "\r\n";
			strinfo += string.Format("消耗的最大时间指令ID {0}", (TCPGameServerCmds)TCPCmdHandler.MaxUsedTicksCmdID);
			strinfo += "\r\n";
			strinfo += string.Format("发送调用总次数 {0}", Global._TCPManager.MySocketListener.GTotalSendCount);
			strinfo += "\r\n";
			strinfo += string.Format("发送的最大包的大小 {0}", Global._SendBufferManager.MaxOutPacketSize);
			strinfo += "\r\n";
			strinfo += string.Format("发送的最大包的指令ID {0}", (TCPGameServerCmds)Global._SendBufferManager.MaxOutPacketSizeCmdID);
			strinfo += "\r\n";
			strinfo += string.Format("指令处理平均耗时（毫秒）{0}", (ProcessSessionTask.processCmdNum != 0L) ? TimeUtil.TimeMS(ProcessSessionTask.processTotalTime / ProcessSessionTask.processCmdNum, 2) : 0.0);
			strinfo += "\r\n";
			strinfo += string.Format("指令处理耗时详情", new object[0]);
			strinfo += "\r\n";
			int count = 0;
			lock (ProcessSessionTask.cmdMoniter)
			{
				foreach (PorcessCmdMoniter i in ProcessSessionTask.cmdMoniter.Values)
				{
					if (detail)
					{
						if (count++ == 0)
						{
							strinfo += string.Format("{0, -48}{1, 6}{2, 7}{3, 7} {4, 7} {5, 4} {6, 4} {7, 5}", new object[]
							{
								"消息",
								"已处理次数",
								"平均处理时长",
								"总计消耗时长",
								"总计字节数",
								"发送次数",
								"发送字节数",
								"失败/成功/数据"
							});
							strinfo += "\r\n";
						}
						string info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##} {4, 13:0.##} {5, 8} {6, 12} {7, 4}/{8}/{9}", new object[]
						{
							(TCPGameServerCmds)i.cmd,
							i.processNum,
							TimeUtil.TimeMS(i.avgProcessTime(), 2),
							TimeUtil.TimeMS(i.processTotalTime, 2),
							i.GetTotalBytes(),
							i.SendNum,
							i.OutPutBytes,
							i.Num_Faild,
							i.Num_OK,
							i.Num_WithData
						});
						strinfo += info;
						strinfo += "\r\n";
					}
					else
					{
						if (count++ == 0)
						{
							strinfo += string.Format("{0, -48}{1, 6}{2, 7}{3, 7}", new object[]
							{
								"消息",
								"已处理次数",
								"平均处理时长",
								"总计消耗时长"
							});
							strinfo += "\r\n";
						}
						string info = string.Format("{0, -50}{1, 11}{2, 13:0.##}{3, 13:0.##}", new object[]
						{
							(TCPGameServerCmds)i.cmd,
							i.processNum,
							TimeUtil.TimeMS(i.avgProcessTime(), 2),
							TimeUtil.TimeMS(i.processTotalTime, 2)
						});
						strinfo += info;
						strinfo += "\r\n";
					}
					if (clear)
					{
						i.Reset();
					}
				}
			}
			strinfo = strinfo.Substring(0, strinfo.Length - 2);
			return strinfo;
		}

		// Token: 0x060029C0 RID: 10688 RVA: 0x0023B410 File Offset: 0x00239610
		private string GetCopyMapInfo(string cmd = null)
		{
			string info = GameManager.CopyMapMgr.GetCopyMapStrInfo();
			return info.Substring(0, info.Length - 2);
		}

		// Token: 0x060029C1 RID: 10689 RVA: 0x0023B440 File Offset: 0x00239640
		private static string GetGCInfo(string cmd = null)
		{
			Program.CalcGCInfo();
			string info = "";
			try
			{
				info += string.Format("GC计数类别    {0,-10} {1,-10} {2,-10}", "0 gen", "1 gen", "2 gen");
				info += "\r\n";
				info += string.Format("总计GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCounts[0], Program.GCCollectionCounts[1], Program.GCCollectionCounts[2]);
				info += "\r\n";
				info += string.Format("每秒GC计数    {0,-10} {1,-10} {2,-10}", Program.GCCollectionCountsNow[0], Program.GCCollectionCountsNow[1], Program.GCCollectionCountsNow[2]);
				info += "\r\n";
				info += string.Format("1秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts1s[0], Program.MaxGCCollectionCounts1s[1], Program.MaxGCCollectionCounts1s[2]);
				info += "\r\n";
				info += string.Format("5秒GC最大     {0,-10} {1,-10} {2,-10}", Program.MaxGCCollectionCounts5s[0], Program.MaxGCCollectionCounts5s[1], Program.MaxGCCollectionCounts5s[2]);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "ShowGCInfo()", false, false);
			}
			return info;
		}

		// Token: 0x060029C2 RID: 10690 RVA: 0x0023B738 File Offset: 0x00239938
		private bool ProcessGMCommands(GameClient client, string msgText, string[] cmdFields, bool transmit, bool isSuperGMUser)
		{
			if (!transmit)
			{
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText);
			}
			string strinfo = "";
			if ("-info" == cmdFields[0])
			{
				if (!transmit)
				{
					strinfo = string.Format("当前线路{0}在线人数{1}", GameManager.ServerLineID, GameManager.ClientMgr.GetClientCount());
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				}
			}
			else if ("-info2" == cmdFields[0])
			{
				if (!transmit)
				{
					string[] dbFields = Global.ExecuteDBCmd(10063, string.Format("{0}", client.ClientData.RoleID), 0);
					if (dbFields == null || dbFields.Length < 1)
					{
						strinfo = string.Format("获取所有线路在线人数时连接数据库失败", new object[0]);
						return true;
					}
					int totalOnlineNum = Global.SafeConvertToInt32(dbFields[0]);
					strinfo = string.Format("获取所有线路在线人数是{0}", totalOnlineNum);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				}
			}
			else if ("-version" == cmdFields[0])
			{
				if (!transmit)
				{
					string gameServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gameserver_version");
					string gameDBServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
					strinfo = string.Format("gameserver_version：{0},gamedb_version：{1},client：mainver-{2},resver-{3},codereversion-{4}", new object[]
					{
						gameServerVer,
						gameDBServerVer,
						client.MainExeVer,
						client.ResVer,
						client.CodeRevision
					});
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				}
			}
			else if (!("-patch" == cmdFields[0]))
			{
				if ("-serverinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						string gameDBServerVer = GameManager.GameConfigMgr.GetGameConifgItem("gamedb_version");
						strinfo = string.Format("{0}_{1}", Global.GetLocalAddressIPs(), TCPManager.ServerPort);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				else if ("-baseinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							strinfo = this.GetServerBaseInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = strinfo
							}, false);
						}
					}
				}
				else if ("-tcpinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							ServerCommandResult result = new ServerCommandResult();
							if (cmdFields.Length > 1)
							{
								strinfo = this.GetServerTCPInfo(cmdFields[0] + cmdFields[1]);
								result.Cmd = cmdFields[0] + " " + cmdFields[1];
							}
							else
							{
								strinfo = this.GetServerTCPInfo(cmdFields[0]);
								result.Cmd = cmdFields[0];
							}
							result.ResultString = strinfo;
							client.sendCmd<ServerCommandResult>(30002, result, false);
						}
					}
				}
				else if ("-copymapinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							strinfo = this.GetCopyMapInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = strinfo
							}, false);
						}
					}
				}
				else if ("-gcinfo" == cmdFields[0])
				{
					if (!transmit)
					{
						if (null != client)
						{
							strinfo = GMCommands.GetGCInfo(null);
							client.sendCmd<ServerCommandResult>(30002, new ServerCommandResult
							{
								Cmd = cmdFields[0],
								ResultString = strinfo
							}, false);
						}
					}
				}
				else if ("-hide" == cmdFields[0])
				{
					if (!transmit)
					{
						if (client.ClientData.HideGM < 1)
						{
							client.ClientData.HideGM = 1;
							List<object> objsList = Global.GetAll9Clients(client);
							GameManager.ClientMgr.NotifyOthersLeave(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList);
							GameManager.ClientMgr.RemoveRolePet(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList, false);
							strinfo = string.Format("进入隐身模式", new object[0]);
						}
						else
						{
							strinfo = string.Format("已经是隐身模式", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				else if ("-show" == cmdFields[0])
				{
					if (!transmit)
					{
						if (client.ClientData.HideGM > 0)
						{
							client.ClientData.HideGM = 0;
							List<object> objsList = Global.GetAll9Clients(client);
							GameManager.ClientMgr.NotifyOthersIamComing(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, objsList, 110);
							strinfo = string.Format("退出隐身模式", new object[0]);
						}
						else
						{
							strinfo = string.Format("已经退出隐身模式", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				else if ("-moveto" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -moveto 地图编号 X坐标 Y坐标", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int mapCode = this.SafeConvertToInt32(cmdFields[1]);
							int toX = 0;
							int toY = 0;
							if (cmdFields.Length >= 4)
							{
								toX = this.SafeConvertToInt32(cmdFields[2]);
								toY = this.SafeConvertToInt32(cmdFields[3]);
							}
							GameMap gameMap = null;
							client.CheckCheatData.GmGotoShadowMapCode = mapCode;
							if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
							{
								Point newPos2 = Global.GetAGridPointIn4Direction(ObjectTypes.OT_CLIENT, new Point((double)(toX / gameMap.MapGridWidth), (double)(toY / gameMap.MapGridHeight)), mapCode, 0, false);
								newPos2 = new Point(newPos2.X * (double)gameMap.MapGridWidth, newPos2.Y * (double)gameMap.MapGridHeight);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, (int)newPos2.X, (int)newPos2.Y, 0, 0))
								{
									if (mapCode != client.ClientData.MapCode)
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mapCode, (int)newPos2.X, (int)newPos2.Y, -1, 0);
									}
									else
									{
										GameManager.LuaMgr.GotoMap(client, mapCode, (int)newPos2.X, (int)newPos2.Y, -1);
									}
									strinfo = string.Format("执行移动到目标点({0},{1})的操作成功", newPos2.X, newPos2.Y);
								}
								else
								{
									strinfo = string.Format("目标点({0},{1})可能是障碍物", toX, toY);
								}
							}
							else
							{
								strinfo = string.Format("请输入正确的地图编号", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-moveto2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -moveto2 副本序号 地图编号 X坐标 Y坐标", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int fuBenSeqID = this.SafeConvertToInt32(cmdFields[1]);
							int mapCode = this.SafeConvertToInt32(cmdFields[2]);
							int toX = 0;
							int toY = 0;
							if (cmdFields.Length >= 5)
							{
								toX = this.SafeConvertToInt32(cmdFields[3]);
								toY = this.SafeConvertToInt32(cmdFields[4]);
							}
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
							{
								if (gameMap.IsolatedMap == 1)
								{
									client.ClientData.FuBenID = mapCode;
								}
								client.ClientData.FuBenSeqID = fuBenSeqID;
								Point newPos2 = Global.GetAGridPointIn4Direction(ObjectTypes.OT_CLIENT, new Point((double)(toX / gameMap.MapGridWidth), (double)(toY / gameMap.MapGridHeight)), mapCode, 0, false);
								newPos2 = new Point(newPos2.X * (double)gameMap.MapGridWidth, newPos2.Y * (double)gameMap.MapGridHeight);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, (int)newPos2.X, (int)newPos2.Y, 0, 0))
								{
									if (mapCode != client.ClientData.MapCode)
									{
										GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mapCode, (int)newPos2.X, (int)newPos2.Y, -1, 0);
									}
									else
									{
										GameManager.LuaMgr.GotoMap(client, mapCode, (int)newPos2.X, (int)newPos2.Y, -1);
									}
									strinfo = string.Format("执行移动到目标点({0},{1})的操作成功", newPos2.X, newPos2.Y);
								}
								else
								{
									strinfo = string.Format("目标点({0},{1})可能是障碍物", toX, toY);
								}
							}
							else
							{
								strinfo = string.Format("请输入正确的地图编号", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-line" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -line 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							string[] dbFields = Global.ExecuteDBCmd(195, string.Format("{0}:{1}:0", client.ClientData.RoleID, otherRoleName), 0);
							if (dbFields == null || dbFields.Length < 5)
							{
								strinfo = string.Format("获取{0}的线路状态时连接数据库失败", otherRoleName);
							}
							else
							{
								int onelineState = Global.SafeConvertToInt32(dbFields[4]);
								strinfo = string.Format("{0}所在的线路是{1}", otherRoleName, onelineState);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-viewum" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -viewum 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							string[] dbFields = Global.ExecuteDBCmd(10069, string.Format("{0}:{1}", client.ClientData.RoleID, otherRoleName), 0);
							if (dbFields == null || dbFields.Length < 4)
							{
								strinfo = string.Format("获取{0}的元宝数时连接数据库失败", otherRoleName);
							}
							else
							{
								string userID = dbFields[1];
								int userMoney = Global.SafeConvertToInt32(dbFields[2]);
								int realMoney = Global.SafeConvertToInt32(dbFields[3]);
								strinfo = string.Format("{0}的平台ID是{1}，元宝是{2}，ＲＭＢ{3}", new object[]
								{
									otherRoleName,
									userID,
									userMoney,
									realMoney
								});
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-follow" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -follow 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int mapCode = otherClient.ClientData.MapCode;
							int toX = otherClient.ClientData.PosX;
							int toY = otherClient.ClientData.PosY;
							GameMap gameMap = null;
							if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap) && MapTypes.Normal == Global.GetMapType(mapCode))
							{
								Point pt = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, toX, toY, 60);
								if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, (int)pt.X, (int)pt.Y, 0, 0))
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mapCode, (int)pt.X, (int)pt.Y, -1, 0);
									strinfo = string.Format("执行移动到{0}身边的操作成功", otherRoleName);
								}
								else
								{
									strinfo = string.Format("执行移动到{0}身边的操作失败", otherRoleName);
								}
							}
							else
							{
								strinfo = string.Format("{0}目前在副本地图, 无法移动到其旁边", otherRoleName);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-buff" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -buff buffid bufferParams...", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int buffid = Global.SafeConvertToInt32(cmdFields[1]);
							double[] bufferParams = new double[cmdFields.Length - 2];
							for (int i = 2; i < cmdFields.Length; i++)
							{
								bufferParams[i - 2] = Global.SafeConvertToDouble(cmdFields[i]);
							}
							Global.UpdateBufferData(client, (BufferItemTypes)buffid, bufferParams, 1, true);
						}
					}
				}
				else if ("-rolestatus" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							strinfo = string.Format("请输入： -rolestatus type startTimeType(0|1) secs moveSpeed", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int type = Global.SafeConvertToInt32(cmdFields[1]);
							int startTimeType = Global.SafeConvertToInt32(cmdFields[2]);
							int secs = Global.SafeConvertToInt32(cmdFields[3]);
							double moveSpeed = (double)Global.SafeConvertToInt32(cmdFields[4]);
							GameManager.ClientMgr.NotifyRoleStatusCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, type, (startTimeType == 0) ? 0L : TimeUtil.NOW(), secs, moveSpeed);
						}
					}
				}
				else if ("-ShenJiJiFen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -ShenJiJiFen 神迹积分变化值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int JiFen = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyShenJiJiFenValue(client, JiFen, "GM命令", false, true);
						}
					}
				}
				else if ("-compboom" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -compboom compid modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int compid = Global.SafeConvertToInt32(cmdFields[1]);
							int modval = Global.SafeConvertToInt32(cmdFields[2]);
							TianTiClient.getInstance().Comp_CompOpt(compid, 0, modval, 0);
						}
					}
				}
				else if ("-compjunxian" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -compjunxian modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int modval = Global.SafeConvertToInt32(cmdFields[1]);
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, modval);
							string broadMsg = string.Format(GLang.GetLang(4017, new object[0]), modval);
							GameManager.ClientMgr.NotifyImportantMsg(client, broadMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
				}
				else if ("-compdonate" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -compdonate modval", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int modval = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyCompDonateValue(client, modval, "GM指令-势力贡献", true, true, true);
						}
					}
				}
				else if ("-comptitle" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -comptitle buffid active", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int buffid = Global.SafeConvertToInt32(cmdFields[1]);
							int active = Global.SafeConvertToInt32(cmdFields[2]);
							if (buffid < 9000 || buffid > 9011)
							{
								strinfo = string.Format("请输入合法的职务类型9000~9011！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							if (active > 0)
							{
								double[] bufferParams = new double[]
								{
									1.0
								};
								Global.UpdateBufferData(client, (BufferItemTypes)buffid, bufferParams, 1, true);
							}
							else
							{
								double[] array = new double[1];
								double[] bufferParams = array;
								Global.UpdateBufferData(client, (BufferItemTypes)buffid, bufferParams, 1, true);
							}
						}
					}
				}
				else if ("-compzhiwu" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -compzhiwu zhiwu", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int myZhiWu = Global.SafeConvertToInt32(cmdFields[1]);
							if (myZhiWu < 1 || myZhiWu > 5)
							{
								strinfo = string.Format("请输入合法的职务类型！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							if (client.ClientData.CompType <= 0)
							{
								strinfo = string.Format("请先加入势力！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							client.ClientData.CompZhiWu = (byte)myZhiWu;
							client.sendCmd(1135, string.Format("{0}:{1}", client.ClientData.RoleID, myZhiWu), false);
						}
					}
				}
				else if ("-compnotice" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							strinfo = string.Format("请输入： -compnotice id toMapCode toPosX toPosY", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							CompScene scene = client.SceneObject as CompScene;
							if (null == scene)
							{
								strinfo = string.Format("请在势力场景内执行该GM命令！", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int noticeID = Global.SafeConvertToInt32(cmdFields[1]);
							int toMapCode = Global.SafeConvertToInt32(cmdFields[2]);
							int toPosX = Global.SafeConvertToInt32(cmdFields[3]);
							int toPosY = Global.SafeConvertToInt32(cmdFields[4]);
							KFCompNotice noticeInfo = new KFCompNotice
							{
								NoticeID = noticeID,
								CompType = scene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								Param2 = Global.FormatRoleNameWithZoneId2(client),
								toMapCode = toMapCode,
								toPosX = toPosX,
								toPosY = toPosY
							};
							CompManager.getInstance().CompNotice(scene, noticeInfo);
						}
					}
				}
				else if ("-alchemy" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -alchemy 角色ID 货币类型 回退值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							int costType = Global.SafeConvertToInt32(cmdFields[2]);
							int useNum = Global.SafeConvertToInt32(cmdFields[3]);
							string rollbackType = string.Format("{0},{1}", costType, useNum);
							if (!AlchemyManager.getInstance().AlchemyRollBackCheck(costType, useNum))
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", roleID, rollbackType), null, true);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								AlchemyManager.getInstance().AlchemyRollBackOffline(roleID, rollbackType);
								return true;
							}
							AlchemyManager.getInstance().AlchemyRollBack(otherClient, rollbackType);
						}
					}
					else if (cmdFields.Length == 4)
					{
						int roleID = Global.SafeConvertToInt32(cmdFields[1]);
						int costType = Global.SafeConvertToInt32(cmdFields[2]);
						int useNum = Global.SafeConvertToInt32(cmdFields[3]);
						string rollbackType = string.Format("{0},{1}", costType, useNum);
						if (!AlchemyManager.getInstance().AlchemyRollBackCheck(costType, useNum))
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色：【{0}】回滚炼金灌注【{1}】 失败！", roleID, rollbackType), null, true);
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							AlchemyManager.getInstance().AlchemyRollBackOffline(roleID, rollbackType);
							return true;
						}
						AlchemyManager.getInstance().AlchemyRollBack(otherClient, rollbackType);
					}
				}
				else if ("-OnePieceRoll" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -OnePieceRoll 骰子点数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int RollNum = Global.SafeConvertToInt32(cmdFields[1]);
							OnePieceManager.getInstance().OnePiece_FakeRollNum_GM = RollNum;
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePieceDice" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -OnePieceDice 骰子类型 数量", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int diceType = Global.SafeConvertToInt32(cmdFields[1]);
							int diceNum = Global.SafeConvertToInt32(cmdFields[2]);
							OnePieceManager.getInstance().GM_SetDice(client, diceType, diceNum);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePieceMove" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							strinfo = string.Format("请输入： -OnePieceMove", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							OnePieceManager.getInstance().ProcessOnePieceMoveCmd(client, 1604, null, null);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-OnePiecePos" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -OnePiecePos 目标位置", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int PosID = Global.SafeConvertToInt32(cmdFields[1]);
							OnePieceManager.getInstance().GM_SetPosID(client, PosID);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-KarenEasy" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							strinfo = string.Format("请输入： -KarenEasy 1 或 0(1打开gm模式，0关闭gm模式)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int OpenGMMode = Global.SafeConvertToInt32(cmdFields[1]);
							KarenBattleManager.getInstance().GMTest = (OpenGMMode != 0);
						}
					}
				}
				else if ("-kfmap" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							strinfo = string.Format("请输入： -kfmap mapcode lineid bossid teleportid", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string[] cmdParams = new string[cmdFields.Length - 1];
							Array.Copy(cmdFields, 1, cmdParams, 0, cmdFields.Length - 1);
							KuaFuMapManager.getInstance().ProcessKuaFuMapEnterCmd(client, 0, null, cmdParams);
						}
					}
				}
				else if ("-Karen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 1)
						{
							strinfo = string.Format("请输入： -Karen 战场ID(区分东西战场)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int BattleID = Global.SafeConvertToInt32(cmdFields[1]);
							KarenBattleSceneInfo SceneInfo = KarenBattleManager.getInstance().TryGetKarenBattleSceneInfoByBattleID(BattleID);
							if (null == SceneInfo)
							{
								return true;
							}
							SceneUIClasses sceneType = Global.GetMapSceneType(SceneInfo.MapCode);
							if (sceneType == SceneUIClasses.KarenWest)
							{
							}
							KuaFuServerInfo kfserverInfo = null;
							KarenFuBenData fubenData = JunTuanClient.getInstance().GetKarenKuaFuFuBenData(SceneInfo.MapCode);
							if (fubenData == null || !KuaFuManager.getInstance().TryGetValue(fubenData.ServerId, out kfserverInfo))
							{
								strinfo = string.Format("请确阿卡伦线路畅通", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
								clientKuaFuServerLoginData.GameId = (long)fubenData.GameId;
								clientKuaFuServerLoginData.GameType = fubenData.GameType;
								clientKuaFuServerLoginData.EndTicks = fubenData.EndTime.Ticks;
								clientKuaFuServerLoginData.ServerId = client.ServerId;
								clientKuaFuServerLoginData.ServerIp = kfserverInfo.Ip;
								clientKuaFuServerLoginData.ServerPort = kfserverInfo.Port;
								clientKuaFuServerLoginData.FuBenSeqId = 0;
							}
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
					}
				}
				else if ("-upKingOfBattlePoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -upKingOfBattlePoint 王者战场积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int upKingOfBattlePoint = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyKingOfBattlePointValue(client, upKingOfBattlePoint, "GM命令", false, true);
						}
					}
				}
				else if ("-upCharmPoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -upCharmPoint 魅力积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int upCharmPoint = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyOrnamentCharmPointValue(client, upCharmPoint, "GM命令", false, true, false);
						}
					}
				}
				else if ("-upOnePieceJiFen" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -upTreasureJiFen 藏宝积分", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int upTreasureJiFen = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyTreasureJiFenValue(client, upTreasureJiFen, "GM命令", true);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-upOnePieceXueZuan" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -upTreasureXueZuan 藏宝血钻", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int upTreasureXueZuan = Global.SafeConvertToInt32(cmdFields[1]);
							GameManager.ClientMgr.ModifyTreasureXueZuanValue(client, upTreasureXueZuan, false, true);
							OnePieceManager.getInstance().GM_PrintTreasureData(client);
						}
					}
				}
				else if ("-upBuildLev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -upBuildLev 建筑物ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int BuildID = Global.SafeConvertToInt32(cmdFields[1]);
							BuildingManager.getInstance().BuildingLevelUp_GM(client, BuildID);
						}
					}
				}
				else if ("-upInputPoints" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -upInputPoints 角色名称 充值点(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int upInputPoints = Global.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							JieriIPointsExchgActivity act = HuodongCachingMgr.GetJieriIPointsExchgActivity();
							string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								roleID,
								upInputPoints,
								act.FromDate.Replace(':', '$'),
								act.ToDate.Replace(':', '$')
							});
							Global.ExecuteDBCmd(13151, strcmd, otherClient.ServerId);
							act.NotifyInputPointsInfo(otherClient, false);
							strinfo = string.Format("{0}充值点数变化({1})", otherRoleName, upInputPoints);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, strinfo);
							otherClient._IconStateMgr.CheckJieRiActivity(otherClient, false);
							otherClient._IconStateMgr.SendIconStateToClient(otherClient);
							UserReturnManager.getInstance().CheckActivityTip(otherClient);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string strUserID = cmdFields[1];
						int upInputPoints = Global.SafeConvertToInt32(cmdFields[2]);
						TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(strUserID);
						GameClient otherClient = null;
						if (null != clientSocket)
						{
							otherClient = GameManager.ClientMgr.FindClient(clientSocket);
						}
						JieriIPointsExchgActivity act = HuodongCachingMgr.GetJieriIPointsExchgActivity();
						if (null != otherClient)
						{
							string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								otherClient.ClientData.RoleID,
								upInputPoints,
								act.FromDate.Replace(':', '$'),
								act.ToDate.Replace(':', '$')
							});
							string[] dbRsp = Global.ExecuteDBCmd(13151, strcmd, otherClient.ServerId);
							if (dbRsp != null && dbRsp.Length == 2 && Convert.ToInt32(dbRsp[1]) >= 0)
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为账号：【{0}】添加充值积分【{1}】点！", strUserID, upInputPoints), null, true);
							}
							act.NotifyInputPointsInfo(otherClient, false);
							strinfo = string.Format("{0}充值点数变化({1})", otherClient.ClientData.RoleName, upInputPoints);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, strinfo);
							otherClient._IconStateMgr.CheckJieRiActivity(otherClient, false);
							otherClient._IconStateMgr.SendIconStateToClient(otherClient);
							UserReturnManager.getInstance().CheckActivityTip(otherClient);
						}
						else
						{
							string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								strUserID,
								upInputPoints,
								act.FromDate.Replace(':', '$'),
								act.ToDate.Replace(':', '$')
							});
							string[] dbRsp = Global.ExecuteDBCmd(13152, strcmd, 0);
							if (dbRsp != null && dbRsp.Length == 2 && Convert.ToInt32(dbRsp[1]) >= 0)
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为账号：【{0}】添加充值积分【{1}】点！", strUserID, upInputPoints), null, true);
							}
						}
					}
				}
				else if ("-substrong" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入：-substrong 角色名称 减少值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int val = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							otherClient.UsingEquipMgr.GMAddEquipStrong(otherClient, val);
							strinfo = string.Format("{0}佩戴的装备耐久减少({1})", otherRoleName, val);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addstorejinbi" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						long value = Convert.ToInt64(cmdFields[1]);
						GameManager.ClientMgr.AddUserStoreYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, value, "gm增加", true);
					}
				}
				else if ("-addstorebdjinbi" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						long value = Convert.ToInt64(cmdFields[1]);
						GameManager.ClientMgr.AddUserStoreMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, value, "gm增加", false);
					}
				}
				else if ("-qingkongyuansu" == cmdFields[0])
				{
					List<GoodsData> goodslist = client.ClientData.ElementhrtsList;
					for (int i = goodslist.Count - 1; i >= 0; i--)
					{
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodslist[i], 1, false, false);
					}
				}
				else if ("-addyuansufenmo" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int val = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, val, "GM命令", true, false);
					}
				}
				else if ("-addlingjing" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int val = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.ModifyMUMoHeValue(client, val, "GM命令", false, true, false);
					}
				}
				else if ("-updatebanggoods" == cmdFields[0])
				{
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
					if (null == bangHuiMiniData)
					{
						strinfo = string.Format("你还没有帮会丫", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						return true;
					}
					if (cmdFields.Length == 8)
					{
						int num = this.SafeConvertToInt32(cmdFields[1]);
						int num2 = this.SafeConvertToInt32(cmdFields[2]);
						int num3 = this.SafeConvertToInt32(cmdFields[3]);
						int num4 = this.SafeConvertToInt32(cmdFields[4]);
						int num5 = this.SafeConvertToInt32(cmdFields[5]);
						int bangGong = this.SafeConvertToInt32(cmdFields[6]);
						int nGuildMoney = this.SafeConvertToInt32(cmdFields[7]);
						string dbcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
						{
							client.ClientData.RoleID,
							client.ClientData.Faction,
							num,
							num2,
							num3,
							num4,
							num5,
							bangGong,
							nGuildMoney
						});
						string[] fields = Global.ExecuteDBCmd(315, dbcmd, client.ServerId);
						if (fields == null || fields.Length != 4)
						{
						}
					}
				}
				else if ("-setbanglevel" == cmdFields[0])
				{
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
					if (null == bangHuiMiniData)
					{
						strinfo = string.Format("你还没有帮会丫", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						return true;
					}
					if (cmdFields.Length == 2)
					{
						int nLevel = this.SafeConvertToInt32(cmdFields[1]);
						string dbcmd = string.Format("{0}:{1}", client.ClientData.Faction, nLevel);
						string[] fields = Global.ExecuteDBCmd(10175, dbcmd, 0);
						if (fields == null || fields.Length != 1)
						{
							strinfo = string.Format("GameDBServer返回失败了", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							return true;
						}
						int retCode = Global.SafeConvertToInt32(fields[0]);
						if (retCode >= 0)
						{
							JunQiManager.NotifySyncBangHuiJunQiItemsDict(client);
							Global.BroadcastJunQiUpLevelHint(client, nLevel);
						}
					}
				}
				else if ("-setwanmota" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int nLevel = this.SafeConvertToInt32(cmdFields[1]);
						WanMoTaDBCommandManager.LayerChangeDBCommand(client, nLevel);
						GameManager.ClientMgr.SaveWanMoTaPassLayerValue(client, nLevel, true);
						client.ClientData.WanMoTaNextLayerOrder = nLevel;
					}
				}
				else if ("-addyuansu" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addyuansu 元素道具id 元素等级1-99", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int goodsID = this.SafeConvertToInt32(cmdFields[1]);
							int gcount = 1;
							int binding = 0;
							int level = this.SafeConvertToInt32(cmdFields[2]);
							int appendprop = 0;
							int lucky = 0;
							int excellenceinfo = 0;
							level = Global.GMax(1, level);
							level = Global.GMin(99, level);
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
							{
								strinfo = string.Format("系统中不存在{0}", goodsID);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int site = 0;
							int categoriy = systemGoods.GetIntValue("Categoriy", -1);
							if (categoriy >= 800 && categoriy < 816)
							{
								site = 3000;
							}
							for (int i = 0; i < gcount; i++)
							{
								if (!Global.CanAddGoods(client, goodsID, 1, binding, "1900-01-01 12:00:00", true, false))
								{
									strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
									{
										client,
										i,
										goodsID,
										gcount - i
									});
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									client.ClientData.RoleName,
									goodsID,
									1,
									level,
									0,
									binding
								}), null, true);
								List<int> elementhrtsProps = new List<int>();
								elementhrtsProps.Add(level);
								elementhrtsProps.Add(0);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsID, 1, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, elementhrtsProps, 0, true);
							}
							strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								client.ClientData.RoleName,
								goodsID,
								gcount,
								level,
								0,
								binding
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addzhangong" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						int val = this.SafeConvertToInt32(cmdFields[1]);
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref val, AddBangGongTypes.None, 0);
					}
				}
				else if ("-setviplev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入：-setviplev 角色名称 VIP等级", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int val = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							Global.GMSetVipLevel(otherClient, val);
							strinfo = string.Format("设置{0}的VIP等级为({1})", otherRoleName, val);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-kick" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -kick 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							strinfo = string.Format("将{0}踢出了当前线路", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								string gmCmdData = string.Format("-kick {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									roleID,
									"",
									0,
									"",
									0,
									gmCmdData,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								string gmCmdData = string.Format("-kick {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									roleID,
									"",
									0,
									"",
									0,
									gmCmdData,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName), null, true);
							Global.ForceCloseClient(otherClient, "被GM踢了", true);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string otherRoleName = cmdFields[1];
						int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
						if (-1 == roleID)
						{
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							return true;
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName), null, true);
						Global.ForceCloseClient(otherClient, "被GM踢了", true);
					}
				}
				else if ("-kick_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -kick_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1] + "$rid";
							strinfo = string.Format("将{0}踢出了当前线路", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							if (-1 == roleID)
							{
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName), null, true);
							Global.ForceCloseClient(otherClient, "被GM踢了", true);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string otherRoleName = cmdFields[1] + "$rid";
						int roleID = Global.SafeConvertToInt32(cmdFields[1]);
						if (-1 == roleID)
						{
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							return true;
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器...", otherRoleName), null, true);
						Global.ForceCloseClient(otherClient, "被GM踢了", true);
					}
				}
				else if ("-kicku" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -kicku 账户名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string userName = cmdFields[1];
							strinfo = string.Format("将账户{0}的角色踢出服务器", userName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserName(userName);
							if (null == clientSocket)
							{
								string gmCmdData = string.Format("-kicku {0}", cmdFields[1]);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									0,
									"",
									0,
									"",
									0,
									gmCmdData,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
							if (null == otherClient)
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的连接踢出了服务器...", userName), null, true);
								Global.ForceCloseSocket(clientSocket, "被GM踢了, 但是这个socket上没有对应的client", true);
							}
							else
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", userName), null, true);
								Global.ForceCloseClient(otherClient, "被GM踢了", true);
							}
						}
					}
					else if (cmdFields.Length >= 2)
					{
						string userName = cmdFields[1];
						if (cmdFields.Length >= 3)
						{
							int fromLine;
							if (int.TryParse(cmdFields[2], out fromLine) && fromLine == GameManager.ServerLineID)
							{
								return true;
							}
						}
						TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserName(userName);
						if (null == clientSocket)
						{
							return true;
						}
						long ticks;
						if (cmdFields.Length >= 4 && long.TryParse(cmdFields[3], out ticks))
						{
							if (ticks < clientSocket.session.SocketTime[0])
							{
								return true;
							}
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将账户{0}的角色踢出了服务器...", userName), null, true);
						GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
						if (null == otherClient)
						{
							Global.ForceCloseSocket(clientSocket, "被GM踢了, 但是这个socket上没有对应的client", true);
						}
						else
						{
							Global.ForceCloseClient(otherClient, "被GM踢了", true);
						}
					}
				}
				else if ("-ban" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -ban 角色名称 禁止的分钟(例如1表示禁止1分钟内登陆)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int minutes = Global.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 != roleID)
							{
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName), null, true);
									Global.ForceCloseClient(otherClient, "被GM踢了", true);
								}
								else
								{
									string gmCmdData = string.Format("-kick {0}", cmdFields[1]);
									GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										roleID,
										"",
										0,
										"",
										0,
										gmCmdData,
										0,
										0,
										GameManager.ServerLineID
									}), null, 0);
								}
							}
							Global.BanRoleNameToDBServer(otherRoleName, minutes);
							strinfo = string.Format("将{0}踢出了当前线路, 并禁止从任何线路再登陆", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-unban" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -unban 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							Global.BanRoleNameToDBServer(otherRoleName, 0);
							strinfo = string.Format("解除对于{0}的禁止登陆限制", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-banchat" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -banchat 角色名称 几个小时", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int banHours = this.SafeConvertToInt32(cmdFields[2]);
							if (banHours > 0)
							{
								Global.BanRoleChatToDBServer(otherRoleName, banHours);
								BanChatManager.AddBanRoleName(otherRoleName, banHours);
							}
							strinfo = string.Format("将{0}列入黑名单，禁止聊天发言", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-unbanchat" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -unbanchat 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							Global.BanRoleChatToDBServer(otherRoleName, 0);
							BanChatManager.AddBanRoleName(otherRoleName, 0);
							strinfo = string.Format("解除对于{0}的禁止聊天发言限制", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-ban_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -ban_rid 角色名称 禁止的分钟(例如1表示禁止1分钟内登陆)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1] + "$rid";
							int minutes = Global.SafeConvertToInt32(cmdFields[2]);
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							if (-1 != roleID)
							{
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName), null, true);
									Global.ForceCloseClient(otherClient, "被GM踢了", true);
								}
							}
							Global.BanRoleNameToDBServer(otherRoleName, minutes);
							strinfo = string.Format("将{0}踢出了当前线路, 并禁止从任何线路再登陆", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-banu" == cmdFields[0])
				{
					if (cmdFields.Length >= 2)
					{
						BanManager.BanUserID2Memory(cmdFields[1]);
					}
				}
				else if ("-unbanu" == cmdFields[0])
				{
					if (cmdFields.Length >= 2)
					{
						BanManager.UnBanUserID2Memory(cmdFields[1]);
					}
				}
				else if ("-unban_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -unban_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1] + "$rid";
							Global.BanRoleNameToDBServer(otherRoleName, 0);
							strinfo = string.Format("解除对于{0}的禁止登陆限制", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-banchat_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -banchat_rid 角色ID 几个小时", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1] + "$rid";
							int banHours = this.SafeConvertToInt32(cmdFields[2]);
							if (banHours > 0)
							{
								Global.BanRoleChatToDBServer(otherRoleName, banHours);
								BanChatManager.AddBanRoleName(otherRoleName, banHours);
							}
							strinfo = string.Format("将{0}列入黑名单，禁止聊天发言", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-unbanchat_rid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -unbanchat_rid 角色ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1] + "$rid";
							Global.BanRoleChatToDBServer(otherRoleName, 0);
							BanChatManager.AddBanRoleName(otherRoleName, 0);
							strinfo = string.Format("解除对于{0}的禁止聊天发言限制", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-recover" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -recover 角色名称", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}恢复血和蓝...", otherRoleName), null, true);
							RoleRelifeLog relifeLog = new RoleRelifeLog(otherClient.ClientData.RoleID, otherClient.ClientData.RoleName, otherClient.ClientData.MapCode, "GM命令");
							if (otherClient.ClientData.CurrentLifeV > 0)
							{
								bool doRelife = false;
								if (otherClient.ClientData.CurrentLifeV < otherClient.ClientData.LifeV)
								{
									doRelife = true;
									relifeLog.hpModify = true;
									relifeLog.oldHp = otherClient.ClientData.CurrentLifeV;
									int addLifeV = otherClient.ClientData.LifeV - otherClient.ClientData.CurrentLifeV;
									otherClient.ClientData.CurrentLifeV = otherClient.ClientData.LifeV;
									relifeLog.newHp = otherClient.ClientData.CurrentLifeV;
								}
								if (otherClient.ClientData.CurrentMagicV < otherClient.ClientData.MagicV)
								{
									doRelife = true;
									relifeLog.mpModify = true;
									relifeLog.oldMp = otherClient.ClientData.CurrentMagicV;
									int addMagicV = otherClient.ClientData.MagicV - otherClient.ClientData.CurrentMagicV;
									otherClient.ClientData.CurrentMagicV = otherClient.ClientData.MagicV;
									relifeLog.newMp = otherClient.ClientData.CurrentMagicV;
								}
								SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
								if (doRelife)
								{
									List<object> listObjs = Global.GetAll9Clients(otherClient);
									GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, otherClient.ClientData.MapCode, otherClient.ClientData.CopyMapID, otherClient.ClientData.RoleID, otherClient.ClientData.PosX, otherClient.ClientData.PosY, otherClient.ClientData.RoleDirection, (double)otherClient.ClientData.CurrentLifeV, (double)otherClient.ClientData.CurrentMagicV, 120, listObjs, 0);
								}
							}
							strinfo = string.Format("为{0}恢复了HP和MP", otherRoleName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setpetai" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -setpetai AI模式(1 自由攻击, 2 攻击主人的目标)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int PetAiControlType = this.SafeConvertToInt32(cmdFields[1]);
							Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
							if (null != monster)
							{
								monster.PetAiControlType = PetAiControlType;
							}
						}
					}
				}
				else if ("-showpet" == cmdFields[0])
				{
					if (!transmit)
					{
						Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
						if (null == monster)
						{
							strinfo = string.Format("请先召唤出您的召唤兽", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							strinfo = string.Format("Defense={0},MDefense={1},MinAttack={2},MaxAttack={3}", new object[]
							{
								monster.MonsterInfo.Defense,
								monster.MonsterInfo.MDefense,
								monster.MonsterInfo.MinAttack,
								monster.MonsterInfo.MaxAttack
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("VLifeMax={0},HitV={1},Dodge={2},SubAttackInjurePercent={3}", new object[]
							{
								monster.MonsterInfo.VLifeMax,
								monster.MonsterInfo.HitV,
								monster.MonsterInfo.Dodge,
								monster.MonsterInfo.MonsterSubAttackInjurePercent
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("HolyAttack={0},HolyDefense={1}", monster.MonsterInfo.ExtProps[122], monster.MonsterInfo.ExtProps[123]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("ShadowAttack={0},ShadowDefense={1}", monster.MonsterInfo.ExtProps[129], monster.MonsterInfo.ExtProps[130]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("NatureAttack={0},NatureDefense={1}", monster.MonsterInfo.ExtProps[136], monster.MonsterInfo.ExtProps[137]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("ChaosAttack={0},ChaosDefense={1}", monster.MonsterInfo.ExtProps[143], monster.MonsterInfo.ExtProps[144]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							strinfo = string.Format("IncubusAttack={0},IncubusDefense={1}", monster.MonsterInfo.ExtProps[150], monster.MonsterInfo.ExtProps[151]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-testmode" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -testmode 压测选项(1-15,1 锁生命值,2 不限测试号,4 全体PK) 地图模式(可选,0-3,0 指定地图, 1 新手场景, 2 多主线地图, 3 剧情副本地图) 指定地图编号(可选,默认1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							GameManager.TestGamePerformanceMode = (this.SafeConvertToInt32(cmdFields[1]) > 0);
							GameManager.TestGamePerformanceForAllUser = ((this.SafeConvertToInt32(cmdFields[1]) & 2) > 0);
							GameManager.TestGamePerformanceAllPK = ((this.SafeConvertToInt32(cmdFields[1]) & 4) > 0);
							GameManager.TestGamePerformanceLockLifeV = ((this.SafeConvertToInt32(cmdFields[1]) & 8) > 0);
							int pkmode = GameManager.TestGamePerformanceAllPK ? 1 : 0;
							int count = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < count; i++)
							{
								GameClient c = GameManager.ClientMgr.FindClientByNid(i);
								if (c != null && (GameManager.TestGamePerformanceForAllUser || c.strUserID == null || c.strUserID.StartsWith("mu")))
								{
									c.ClientData.PKMode = pkmode;
								}
							}
							if (cmdFields.Length > 2)
							{
								GameManager.TestGamePerformanceMapMode = this.SafeConvertToInt32(cmdFields[2]);
								if (cmdFields.Length > 3)
								{
									GameManager.TestGamePerformanceMapCode = this.SafeConvertToInt32(cmdFields[3]);
								}
							}
							do
							{
								Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 1, 10000, 10000, 8000);
								if (GameManager.TestBirthPointList1.FindIndex((Point x) => x.X / 100.0 == newPos.X / 100.0 && x.Y / 100.0 == newPos.Y / 100.0) < 0)
								{
									GameManager.TestBirthPointList1.Add(newPos);
								}
							}
							while (GameManager.TestBirthPointList1.Count < 1000);
							do
							{
								Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, 2, 5000, 5000, 4000);
								if (GameManager.TestBirthPointList2.FindIndex((Point x) => x.X / 100.0 == newPos.X / 100.0 && x.Y / 100.0 == newPos.Y / 100.0) < 0)
								{
									GameManager.TestBirthPointList2.Add(newPos);
								}
							}
							while (GameManager.TestBirthPointList2.Count < 1000);
							strinfo = string.Format("设置了压测模式 {0} {1}", GameManager.TestGamePerformanceMode, GameManager.TestGamePerformanceMapMode);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM({0})的要求设置压测模式：{1}", Global.FormatRoleName4(client), msgText), null, true);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-useworkpool" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -useworkpool 是否使用线程池(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							strinfo = string.Format("设置了命令处理模式是否使用线程池 {0}", false);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-manyattack" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -manyattack 是否使用多段攻击(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							strinfo = string.Format("多段攻击开启状态： {0}", true);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setmaxthread" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setmaxthread 后台线程数 完成端口线程数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int threadNum = Global.SafeConvertToInt32(cmdFields[1]);
							int threadNum2 = Global.SafeConvertToInt32(cmdFields[2]);
							ThreadPool.SetMaxThreads(threadNum, threadNum2);
							strinfo = string.Format("线程池最大线程数设置为：后台线程{0},完成端口线程{1}", threadNum, threadNum2);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setlev 角色名称 级别 转生次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							this.GMSetLevel(client, cmdFields);
						}
					}
				}
				else if ("-specprioritykf" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -specprioritykf TeQuanTiaoJian.xml的ID 次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int groupid = Global.SafeConvertToInt32(cmdFields[1]);
							int add = Global.SafeConvertToInt32(cmdFields[2]);
							SpecPriorityActivity act2 = HuodongCachingMgr.GetSpecPriorityActivity();
							if (null == act2)
							{
								return true;
							}
							List<SpecPConditionConfig> specpConditionList = act2.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
							if (specpConditionList == null || specpConditionList.Count == 0)
							{
								return true;
							}
							SpecPConditionConfig myConfig = specpConditionList.Find((SpecPConditionConfig x) => x.GroupID == groupid);
							if (null == myConfig)
							{
								return true;
							}
							if (act2.IfKFConditonType(myConfig.ConditionType))
							{
								lock (SpecPriorityActivity.Mutex)
								{
									act2.OnConditionNumChangeBefore();
									KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(groupid, add);
									act2.ModifySpecialPriorityActConitionInfo(groupid, add);
									act2.OnConditionNumChangeAfter();
								}
								strinfo = string.Format("特权活动条件计数{0}添加了{1}", myConfig.ConditionType, add);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if (cmdFields.Length >= 3)
					{
						int groupid = Global.SafeConvertToInt32(cmdFields[1]);
						int add = Global.SafeConvertToInt32(cmdFields[2]);
						SpecPriorityActivity act2 = HuodongCachingMgr.GetSpecPriorityActivity();
						if (null == act2)
						{
							return true;
						}
						List<SpecPConditionConfig> specpConditionList = act2.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
						if (specpConditionList == null || specpConditionList.Count == 0)
						{
							return true;
						}
						SpecPConditionConfig myConfig = specpConditionList.Find((SpecPConditionConfig x) => x.GroupID == groupid);
						if (null == myConfig)
						{
							return true;
						}
						if (act2.IfKFConditonType(myConfig.ConditionType))
						{
							lock (SpecPriorityActivity.Mutex)
							{
								act2.OnConditionNumChangeBefore();
								KFCopyRpcClient.getInstance().SpecPriority_ModifyActivityConditionNum(groupid, add);
								act2.ModifySpecialPriorityActConitionInfo(groupid, add);
								act2.OnConditionNumChangeAfter();
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求特权活动条件计数{0}添加了{1}", myConfig.ConditionType, add), null, true);
						}
					}
				}
				else if ("-specpriority" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -specpriority TeQuanTiaoJian.xml的ID 次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int groupid = Global.SafeConvertToInt32(cmdFields[1]);
							int add = Global.SafeConvertToInt32(cmdFields[2]);
							SpecPriorityActivity act2 = HuodongCachingMgr.GetSpecPriorityActivity();
							if (null == act2)
							{
								return true;
							}
							List<SpecPConditionConfig> specpConditionList = act2.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
							if (specpConditionList == null || specpConditionList.Count == 0)
							{
								return true;
							}
							SpecPConditionConfig myConfig = specpConditionList.Find((SpecPConditionConfig x) => x.GroupID == groupid);
							if (null == myConfig)
							{
								return true;
							}
							if (!act2.IfKFConditonType(myConfig.ConditionType))
							{
								lock (SpecPriorityActivity.Mutex)
								{
									act2.OnConditionNumChangeBefore();
									act2.ModifySpecialPriorityActConitionInfo(groupid, add);
									act2.OnConditionNumChangeAfter();
								}
								strinfo = string.Format("特权活动条件计数{0}添加了{1}", myConfig.ConditionType, add);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if (cmdFields.Length >= 3)
					{
						int groupid = Global.SafeConvertToInt32(cmdFields[1]);
						int add = Global.SafeConvertToInt32(cmdFields[2]);
						SpecPriorityActivity act2 = HuodongCachingMgr.GetSpecPriorityActivity();
						if (null == act2)
						{
							return true;
						}
						List<SpecPConditionConfig> specpConditionList = act2.CalSpecPConditionListByNow(TimeUtil.NowDateTime());
						if (specpConditionList == null || specpConditionList.Count == 0)
						{
							return true;
						}
						SpecPConditionConfig myConfig = specpConditionList.Find((SpecPConditionConfig x) => x.GroupID == groupid);
						if (null == myConfig)
						{
							return true;
						}
						if (!act2.IfKFConditonType(myConfig.ConditionType))
						{
							lock (SpecPriorityActivity.Mutex)
							{
								act2.OnConditionNumChangeBefore();
								act2.ModifySpecialPriorityActConitionInfo(groupid, add);
								act2.OnConditionNumChangeAfter();
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求特权活动条件计数{0}添加了{1}", myConfig.ConditionType, add), null, true);
						}
					}
				}
				else if ("-printrebornboss" == cmdFields[0])
				{
					if (!transmit)
					{
						RebornBoss.getInstance().PrintBossInfoGM(client, int.MaxValue, null);
					}
				}
				else if ("-fakerebornboss" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -fakerebornboss 虚拟数据条数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int fakeNum = Global.SafeConvertToInt32(cmdFields[1]);
							RebornBoss.getInstance().BuildFakeBossInfoGM(client, fakeNum);
							strinfo = string.Format("重生Boss排行榜添加了{0}条假数据", fakeNum);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setrebornlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setrebornlev 角色名称 级别 重生次数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							this.GMSetRebornLevel(client, cmdFields);
						}
					}
				}
				else if ("-addexpmax" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -addexpmax 角色名称 经验类型(1怪 2回收) 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							MoneyTypes moneyTypes = (Global.SafeConvertToInt32(cmdFields[2]) == 1) ? MoneyTypes.RebornExpMonster : MoneyTypes.RebornExpSale;
							long newexp = Global.SafeConvertToInt64(cmdFields[3]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加重生经验上限{1}", otherRoleName, newexp), null, true);
							GameManager.ClientMgr.ModifyRebornExpMaxAddValue(otherClient, newexp, "GM道具脚本", moneyTypes, false, true, false);
							strinfo = string.Format("为{0}添加了重生经验上限{1}", otherRoleName, newexp);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addrebornexp" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -addrebornexp 角色名称 经验类型(1怪 2回收) 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							MoneyTypes moneyTypes = (Global.SafeConvertToInt32(cmdFields[2]) == 1) ? MoneyTypes.RebornExpMonster : MoneyTypes.RebornExpSale;
							long newexp = Global.SafeConvertToInt64(cmdFields[3]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加重生经验{1}", otherRoleName, newexp), null, true);
							RebornManager.getInstance().ProcessRoleExperience(otherClient, newexp, moneyTypes, false, true, false, "none");
							strinfo = string.Format("为{0}添加了重生经验{1}", otherRoleName, newexp);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-clearguildmap" == cmdFields[0])
				{
					if (client.ClientData.Faction > 0)
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "GuildCopyMapAwardFlag", 0, true);
						GameManager.GuildCopyMapDBMgr.ResetGuildCopyMapDB(client.ClientData.Faction, 0);
					}
				}
				else if ("-showallicon" == cmdFields[0])
				{
					GameManager.ClientMgr.SendToClient(client, "", 688);
				}
				else if ("-addexp" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addexp 角色名称 经验(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							long newexp = Global.SafeConvertToInt64(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加经验{1}", otherRoleName, newexp), null, true);
							GameManager.ClientMgr.ProcessRoleExperience(otherClient, newexp, false, true, false, "none");
							strinfo = string.Format("为{0}添加了经验{1}", otherRoleName, newexp);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addexp2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -addexp2 当前等级需要经验的百分数(最大100)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int addPercent = this.SafeConvertToInt32(cmdFields[1]);
							addPercent = Math.Max(0, addPercent);
							addPercent = Math.Min(100, addPercent);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", addPercent), null, true);
							GameManager.ClientMgr.AddAllOnlieRoleExperience(addPercent);
							strinfo = string.Format("为所有在线用户添加经验百分比{0}", addPercent);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							string gmCmdData = string.Format("-addexp2 {0}", cmdFields[1]);
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								gmCmdData,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 2)
					{
						int addPercent = this.SafeConvertToInt32(cmdFields[1]);
						addPercent = Math.Max(0, addPercent);
						addPercent = Math.Min(100, addPercent);
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为所有在线用户添加经验百分比{0}", addPercent), null, true);
						GameManager.ClientMgr.AddAllOnlieRoleExperience(addPercent);
					}
				}
				else if ("-addipower" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addipower 角色名称 内力(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int interPower = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加灵力{1}", otherRoleName, interPower), null, true);
							if (interPower > 0)
							{
								GameManager.ClientMgr.AddInterPower(otherClient, interPower, false, true);
							}
							else
							{
								GameManager.ClientMgr.SubInterPower(otherClient, -interPower);
							}
							strinfo = string.Format("为{0}添加了灵力{1}", otherRoleName, interPower);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addmoney" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addmoney 角色名称 游戏币(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int money = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加游戏币{1}", otherRoleName, money), null, true);
							GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, money, "GM指令添加绑金", true);
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
							{
								otherClient.ClientData.RoleID,
								otherClient.ClientData.RoleName,
								otherClient.ClientData.Money1,
								money
							}), EventLevels.Record);
							strinfo = string.Format("为{0}添加了游戏币{1}", otherRoleName, money);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string otherRoleName = cmdFields[1];
						int money = this.SafeConvertToInt32(cmdFields[2]);
						int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
						if (-1 == roleID)
						{
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							return true;
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加游戏币{1}", otherRoleName, money), null, true);
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, money, "GM指令添加绑金", true);
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金钱, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
						{
							otherClient.ClientData.RoleID,
							otherClient.ClientData.RoleName,
							otherClient.ClientData.Money1,
							money
						}), EventLevels.Record);
					}
				}
				else if ("-addyl" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addlj 角色名称 银两(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int yinLiang = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加银两{1}", otherRoleName, yinLiang), null, true);
							if (yinLiang >= 0)
							{
								GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令添加", false);
							}
							else
							{
								GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令扣除", false);
							}
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}", new object[]
							{
								otherClient.ClientData.RoleID,
								otherClient.ClientData.RoleName,
								otherClient.ClientData.YinLiang,
								yinLiang
							}), EventLevels.Record);
							strinfo = string.Format("为{0}添加了银两{1}", otherRoleName, yinLiang);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string otherRoleName = cmdFields[1];
						int yinLiang = this.SafeConvertToInt32(cmdFields[2]);
						int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
						if (-1 == roleID)
						{
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							return true;
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加银两{1}", otherRoleName, yinLiang), null, true);
						if (yinLiang >= 0)
						{
							GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令添加", false);
						}
						else
						{
							GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(yinLiang), "GM指令扣除", false);
						}
						GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), YinLiang={2}, newYinLiang={3}", new object[]
						{
							otherClient.ClientData.RoleID,
							otherClient.ClientData.RoleName,
							otherClient.ClientData.YinLiang,
							yinLiang
						}), EventLevels.Record);
					}
				}
				else if ("-addgold" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -addlj 角色名称 金币(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int gold = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加金币{1}", otherRoleName, gold), null, true);
							if (gold >= 0)
							{
								GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(gold), "GM指令");
							}
							else
							{
								GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(gold), "GM指令", false);
							}
							GameManager.SystemServerEvents.AddEvent(string.Format("角色获取金币, roleID={0}({1}), Gold={2}, newGold={3}", new object[]
							{
								otherClient.ClientData.RoleID,
								otherClient.ClientData.RoleName,
								otherClient.ClientData.Gold,
								gold
							}), EventLevels.Record);
							strinfo = string.Format("为{0}添加了金币{1}", otherRoleName, gold);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-adddj" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -adddj 角色名称 元宝(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int dianjuan = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加元宝{1}", otherRoleName, dianjuan), null, true);
							if (dianjuan >= 0)
							{
								GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(dianjuan), "GM要求添加", ActivityTypes.None, "");
							}
							else
							{
								GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, Math.Abs(dianjuan), "GM要求扣除", true, true, false, DaiBiSySType.None);
							}
							strinfo = string.Format("为{0}添加了钻石{1}", otherRoleName, dianjuan);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-additem" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 9)
						{
							strinfo = string.Format("请输入： -additem 角色名称 物品名称 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							string goodsName = cmdFields[2];
							int gcount = this.SafeConvertToInt32(cmdFields[3]);
							gcount = Global.GMax(0, gcount);
							gcount = Global.GMin(int.MaxValue, gcount);
							int binding = this.SafeConvertToInt32(cmdFields[4]);
							int level = this.SafeConvertToInt32(cmdFields[5]);
							int appendprop = this.SafeConvertToInt32(cmdFields[6]);
							int lucky = this.SafeConvertToInt32(cmdFields[7]);
							int excellenceinfo = this.SafeConvertToInt32(cmdFields[8]);
							int goodsID = Global.GetGoodsByName(goodsName);
							if (-1 == goodsID)
							{
								strinfo = string.Format("系统中不存在{0}", goodsName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							level = Global.GMax(0, level);
							level = Global.GMin(15, level);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
							{
								strinfo = string.Format("系统中不存在{0}", goodsName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int site = 0;
							int categoriy = systemGoods.GetIntValue("Categoriy", -1);
							if (categoriy >= 800 && categoriy < 816)
							{
								site = 3000;
							}
							if (systemGoods.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < gcount; i++)
								{
									if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true, false))
									{
										strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											otherRoleName,
											i,
											goodsName,
											gcount - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
										return true;
									}
									LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
									{
										otherRoleName,
										goodsName,
										1,
										level,
										0,
										binding
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, "1900-01-01 12:00:00", true, false))
								{
									strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									otherRoleName,
									goodsName,
									gcount,
									level,
									0,
									binding
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
							}
							strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								otherRoleName,
								goodsName,
								gcount,
								level,
								0,
								binding
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-additem2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 7)
						{
							strinfo = string.Format("请输入： -additem2 角色名称 物品名称 个数 绑定(0/1) 限制日期(2011-01-01) 限制时间(00$00$00)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							string goodsName = cmdFields[2];
							int gcount = this.SafeConvertToInt32(cmdFields[3]);
							gcount = Global.GMax(0, gcount);
							gcount = Global.GMin(int.MaxValue, gcount);
							int binding = this.SafeConvertToInt32(cmdFields[4]);
							string limitDate = cmdFields[5];
							string limitTime = cmdFields[6];
							string limitDateTime = string.Format("{0} {1}", limitDate, limitTime);
							limitDateTime = limitDateTime.Replace("$", ":");
							if (Global.DateTimeTicks(limitDateTime) <= 0L)
							{
								strinfo = string.Format("限时格式错误{0} {1}", limitDate, limitTime);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int goodsID = Global.GetGoodsByName(goodsName);
							if (-1 == goodsID)
							{
								strinfo = string.Format("系统中不存在{0}", goodsName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
							if (goodsCatetoriy < 49 || (goodsCatetoriy >= 800 && goodsCatetoriy < 816))
							{
								strinfo = string.Format("不能添加限时的{0}", goodsName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
							{
								strinfo = string.Format("系统中不存在{0}", goodsName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							if (systemGoods.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < gcount; i++)
								{
									if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, limitDateTime, true, false))
									{
										strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											otherRoleName,
											i,
											goodsName,
											gcount - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
										return true;
									}
									LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", new object[]
									{
										otherRoleName,
										goodsName,
										1,
										0,
										"白色",
										binding,
										limitDateTime
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", 0, binding, 0, "", true, 1, "GM添加", limitDateTime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, limitDateTime, true, false))
								{
									strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}, 结束时间:{6}", new object[]
								{
									otherRoleName,
									goodsName,
									gcount,
									0,
									"白色",
									binding,
									limitDateTime
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", 0, binding, 0, "", true, 1, "GM添加", limitDateTime, 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
							}
							strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}, 结束时间:{6} {7}", new object[]
							{
								otherRoleName,
								goodsName,
								gcount,
								0,
								"白色",
								binding,
								limitDate,
								limitTime
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-addgoodpro" == cmdFields[0])
				{
					if (cmdFields.Length < 9)
					{
						strinfo = string.Format("请输入： -addgoodpro 角色id 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越 [洗练属性] [元素属性] [聚魂属性]", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
					int roleID = this.SafeConvertToInt32(cmdFields[1]);
					int goodsID = this.SafeConvertToInt32(cmdFields[2]);
					int gcount = this.SafeConvertToInt32(cmdFields[3]);
					gcount = Global.GMax(0, gcount);
					gcount = Global.GMin(int.MaxValue, gcount);
					int binding = this.SafeConvertToInt32(cmdFields[4]);
					int level = this.SafeConvertToInt32(cmdFields[5]);
					int appendprop = this.SafeConvertToInt32(cmdFields[6]);
					int lucky = this.SafeConvertToInt32(cmdFields[7]);
					int excellenceinfo = this.SafeConvertToInt32(cmdFields[8]);
					bool onLine = true;
					List<int> washProps = null;
					string washStr = "";
					if (cmdFields.Length > 9 && cmdFields[9] != "*")
					{
						washStr = cmdFields[9];
						byte[] wash = Convert.FromBase64String(cmdFields[9]);
						washProps = DataHelper.BytesToObject<List<int>>(wash, 0, wash.Length);
					}
					List<int> ehProps = null;
					string ehStr = "";
					if (cmdFields.Length > 10 && cmdFields[10] != "*")
					{
						ehStr = cmdFields[10];
						byte[] ehinfo = Convert.FromBase64String(cmdFields[10]);
						ehProps = DataHelper.BytesToObject<List<int>>(ehinfo, 0, ehinfo.Length);
					}
					int juHun = 0;
					if (cmdFields.Length > 11 && cmdFields[11] != "*")
					{
						juHun = this.SafeConvertToInt32(cmdFields[11]);
					}
					int quality = 0;
					SystemXmlItem systemGoods = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
					{
						strinfo = string.Format("系统中不存在{0}", goodsID);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						return true;
					}
					GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
					if (null == otherClient)
					{
						RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleID), 0);
						if (null == dbRd)
						{
							LogManager.WriteLog(LogTypes.Error, "添加角色道具，但是查不到角色数据。", null, true);
							return true;
						}
						otherClient = new GameClient
						{
							ClientData = new SafeClientData
							{
								RoleData = dbRd
							}
						};
						onLine = false;
					}
					int site = 0;
					int categoriy = systemGoods.GetIntValue("Categoriy", -1);
					if (categoriy >= 800 && categoriy < 816)
					{
						site = 3000;
					}
					else if (categoriy == 901)
					{
						site = 7000;
					}
					else if (categoriy >= 910 && categoriy <= 928)
					{
						site = 8000;
					}
					else if (categoriy == 940)
					{
						site = 11000;
					}
					else if (categoriy >= 980 && categoriy <= 981)
					{
						site = 16000;
					}
					if (categoriy != 9 && categoriy != 10)
					{
						level = Global.GMax(0, level);
						level = Global.GMin(20, level);
					}
					string goodsName = systemGoods.GetStringValue("Title");
					if (systemGoods.GetIntValue("GridNum", -1) <= 1)
					{
						for (int i = 0; i < gcount; i++)
						{
							if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true, false))
							{
								strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
								{
									roleID,
									i,
									goodsName,
									gcount - i
								});
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5} 洗练:{6} 元素:{7} 聚魂:{8}", new object[]
							{
								roleID,
								goodsName,
								1,
								level,
								quality,
								binding,
								washStr,
								ehStr,
								juHun
							}), null, true);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, washProps, ehProps, juHun, onLine);
						}
					}
					else
					{
						if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, "1900-01-01 12:00:00", true, false))
						{
							strinfo = string.Format("{0}背包已经满，无法添加{1}", roleID, goodsName);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							return true;
						}
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5} 洗练:{6} 元素:{7}", new object[]
						{
							roleID,
							goodsName,
							gcount,
							level,
							quality,
							binding,
							washStr,
							ehStr
						}), null, true);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, washProps, ehProps, juHun, onLine);
					}
					strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5} 洗练:{6} 元素:{7}", new object[]
					{
						roleID,
						goodsName,
						gcount,
						level,
						quality,
						binding,
						washStr,
						ehStr
					});
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				}
				else if ("-addid" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 9)
						{
							strinfo = string.Format("请输入： -addid 角色名称 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int goodsID = this.SafeConvertToInt32(cmdFields[2]);
							int gcount = this.SafeConvertToInt32(cmdFields[3]);
							gcount = Global.GMax(0, gcount);
							gcount = Global.GMin(int.MaxValue, gcount);
							int binding = this.SafeConvertToInt32(cmdFields[4]);
							int level = this.SafeConvertToInt32(cmdFields[5]);
							int appendprop = this.SafeConvertToInt32(cmdFields[6]);
							int lucky = this.SafeConvertToInt32(cmdFields[7]);
							int excellenceinfo = this.SafeConvertToInt32(cmdFields[8]);
							level = Global.GMax(0, level);
							level = Global.GMin(20, level);
							int quality = 0;
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							SystemXmlItem systemGoods = null;
							if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
							{
								strinfo = string.Format("系统中不存在{0}", goodsID);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int site = 0;
							int categoriy = systemGoods.GetIntValue("Categoriy", -1);
							if (categoriy >= 800 && categoriy < 816)
							{
								site = 3000;
							}
							else if (categoriy == 901)
							{
								site = 7000;
							}
							else if (categoriy >= 910 && categoriy <= 928)
							{
								site = 8000;
							}
							else if (categoriy == 940)
							{
								site = 11000;
							}
							else if (categoriy >= 980 && categoriy <= 981)
							{
								site = 16000;
							}
							string goodsName = systemGoods.GetStringValue("Title");
							if (systemGoods.GetIntValue("GridNum", -1) <= 1)
							{
								for (int i = 0; i < gcount; i++)
								{
									if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true, false) && !RebornEquip.IsRebornType(goodsID))
									{
										strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											otherRoleName,
											i,
											goodsName,
											gcount - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
										return true;
									}
									if (!RebornEquip.CanAddGoodsDataList2(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true) && RebornEquip.IsRebornType(goodsID))
									{
										strinfo = string.Format("{0}重生背包已经满，已经添加{1}个{2}, 剩余{2}个无法添加{1}", new object[]
										{
											otherRoleName,
											i,
											goodsName,
											gcount - i
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
										return true;
									}
									LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
									{
										otherRoleName,
										goodsName,
										1,
										level,
										quality,
										binding
									}), null, true);
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
								}
							}
							else
							{
								if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, "1900-01-01 12:00:00", true, false) && !RebornEquip.IsRebornType(goodsID))
								{
									strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								if (!RebornEquip.CanAddGoodsDataList2(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true) && RebornEquip.IsRebornType(goodsID))
								{
									strinfo = string.Format("{0}重生背包已经满，无法添加{1}", otherRoleName, goodsName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									otherRoleName,
									goodsName,
									gcount,
									level,
									quality,
									binding
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
							}
							strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
							{
								otherRoleName,
								goodsName,
								gcount,
								level,
								quality,
								binding
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if (cmdFields.Length >= 9)
					{
						string otherRoleName = cmdFields[1];
						int goodsID = this.SafeConvertToInt32(cmdFields[2]);
						int gcount = this.SafeConvertToInt32(cmdFields[3]);
						gcount = Global.GMax(0, gcount);
						gcount = Global.GMin(int.MaxValue, gcount);
						int binding = this.SafeConvertToInt32(cmdFields[4]);
						int level = this.SafeConvertToInt32(cmdFields[5]);
						int appendprop = this.SafeConvertToInt32(cmdFields[6]);
						int lucky = this.SafeConvertToInt32(cmdFields[7]);
						int excellenceinfo = this.SafeConvertToInt32(cmdFields[8]);
						level = Global.GMax(0, level);
						level = Global.GMin(20, level);
						int quality = 0;
						int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
						if (-1 == roleID)
						{
							strinfo = string.Format("根据GM的要求为{0}添加物品,目标不在线", otherRoleName);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
							return true;
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
						if (null == otherClient)
						{
							strinfo = string.Format("根据GM的要求为{0}添加物品,目标不在线", otherRoleName);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
							return true;
						}
						SystemXmlItem systemGoods = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
						{
							strinfo = string.Format("根据GM的要求为{1}添加物品,但系统中不存在{0}", goodsID, otherRoleName);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
							return true;
						}
						int site = 0;
						int categoriy = systemGoods.GetIntValue("Categoriy", -1);
						if (categoriy >= 800 && categoriy < 816)
						{
							site = 3000;
						}
						string goodsName = systemGoods.GetStringValue("Title");
						if (systemGoods.GetIntValue("GridNum", -1) <= 1)
						{
							for (int i = 0; i < gcount; i++)
							{
								if (!Global.CanAddGoods(otherClient, goodsID, 1, binding, "1900-01-01 12:00:00", true, false))
								{
									strinfo = string.Format("{0}背包已经满，已经添加{1}个{2}, 剩余{3}个无法添加{1}", new object[]
									{
										otherRoleName,
										i,
										goodsName,
										gcount - i
									});
									LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
									return true;
								}
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
								{
									otherRoleName,
									goodsName,
									1,
									level,
									quality,
									binding
								}), null, true);
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, 1, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
							}
						}
						else
						{
							if (!Global.CanAddGoods(otherClient, goodsID, gcount, binding, "1900-01-01 12:00:00", true, false))
							{
								strinfo = string.Format("{0}背包已经满，无法添加{1}", otherRoleName, goodsName);
								LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加物品{1}, 个数:{2}, 级别{3}, 品质:{4}, 绑定:{5}", new object[]
							{
								otherRoleName,
								goodsName,
								gcount,
								level,
								quality,
								binding
							}), null, true);
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, goodsID, gcount, 0, "", level, binding, site, "", true, 1, "GM添加", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceinfo, appendprop, 0, null, null, 0, true);
						}
					}
				}
				else if ("-setpkv" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setpkv 角色名称 pk值(最小值0) pk点(最小值0)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int pkValue = this.SafeConvertToInt32(cmdFields[2]);
							int pkPoint = this.SafeConvertToInt32(cmdFields[3]);
							pkValue = Global.GMax(0, pkValue);
							pkPoint = Global.GMax(0, pkPoint);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置PK值{1}", otherRoleName, pkValue), null, true);
							GameManager.ClientMgr.SetRolePKValuePoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, pkValue, pkPoint, true);
							Global.ProcessRedNamePunishForDebuff(otherClient);
							strinfo = string.Format("为{0}设置了PK值{1}", otherRoleName, pkValue);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-adddjpoint" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -adddjpoint 角色名称 点将积分(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int djPoint = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}添加点将积分{1}", otherRoleName, djPoint), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10023, string.Format("{0}:{1}", otherClient.ClientData.RoleID, djPoint), null, otherClient.ServerId);
							strinfo = string.Format("为{0}添加点将积分{1}", otherRoleName, djPoint);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setmaintask" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setmaintask RoleName TaskID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int taskVal = this.SafeConvertToInt32(cmdFields[2]);
							taskVal = Global.GMax(0, taskVal);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								roleID = Global.SafeConvertToInt32(otherRoleName);
								if (roleID < 200000)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置主线任务ID为{1}", otherRoleName, taskVal), null, true);
							ProcessTask.GMSetMainTaskID(otherClient, taskVal);
							SingletonTemplate<GuardStatueManager>.Instance().OnTaskComplete(otherClient);
							strinfo = string.Format("为{0}设置主线任务ID为{1}", otherRoleName, taskVal);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-settaskv" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							strinfo = string.Format("请输入： -settaskv 角色名称 任务名称 目标类型(1/2) 数值(可以是负数)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							string taskName = cmdFields[2];
							int valType = this.SafeConvertToInt32(cmdFields[3]);
							valType = Global.GMax(1, valType);
							valType = Global.GMin(2, valType);
							int taskVal = this.SafeConvertToInt32(cmdFields[4]);
							taskVal = Global.GMax(0, taskVal);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置任务{1}, 值类型:{2}, 任务值:{3}", new object[]
							{
								otherRoleName,
								taskName,
								valType,
								taskVal
							}), null, true);
							ProcessTask.ProcessTaskValue(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, taskName, valType, taskVal);
							strinfo = string.Format("为{0}设置任务{1}, 值类型{2}, 任务值{3}", new object[]
							{
								otherRoleName,
								taskName,
								valType,
								taskVal
							});
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-shutdown" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为关闭服务器: {0}", TimeUtil.NowDateTime()), null, true);
						Program.Exit();
					}
				}
				else if ("-auth" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -auth 公告开关(0/1)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int auth = this.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的切换授权开关: {0}", auth), null, true);
							GameManager.ClientMgr.NotifyGMAuthCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, auth);
						}
					}
				}
				else if ("-bull" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 6)
						{
							strinfo = string.Format("请输入： -bull 公告ID(文字和数字都可以) 开始时间(yyyy-MM-dd_HH&mm&ss) 结束时间(yyyy-MM-dd_HH&mm&ss) 间隔(数字，单位秒) 公告文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string msgID = cmdFields[1].Trim();
							string fromDate = cmdFields[2].Trim().Replace('&', ':').Replace('_', ' ');
							string toDate = cmdFields[3].Trim().Replace('&', ':').Replace('_', ' ');
							int interval = this.SafeConvertToInt32(cmdFields[4]);
							string bulletinText = cmdFields[5];
							if (string.IsNullOrEmpty(msgID))
							{
								strinfo = string.Format("公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布公告: {0} {1} {2} {3} {4}", new object[]
							{
								msgID,
								fromDate,
								toDate,
								interval,
								bulletinText
							}), null, true);
							GameManager.BulletinMsgMgr.AddBulletinMsgBackground(msgID, fromDate, toDate, interval, bulletinText);
							string gmCmdData = string.Format("-bull {0} {1} {2} {3} {4}", new object[]
							{
								msgID,
								cmdFields[2],
								cmdFields[3],
								interval,
								bulletinText
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								gmCmdData,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 6)
					{
						string msgID = cmdFields[1].Trim();
						string fromDate = cmdFields[2].Trim().Replace('&', ':').Replace('_', ' ');
						string toDate = cmdFields[3].Trim().Replace('&', ':').Replace('_', ' ');
						int interval = this.SafeConvertToInt32(cmdFields[4]);
						string bulletinText = cmdFields[5];
						GameManager.BulletinMsgMgr.AddBulletinMsgBackground(msgID, fromDate, toDate, interval, bulletinText);
					}
				}
				else if ("-rmbull" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -rmbull 公告ID(文字和数字都可以)", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string msgID = cmdFields[1].Trim();
							if (string.IsNullOrEmpty(msgID))
							{
								strinfo = string.Format("公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求删除公告: {0}", msgID), null, true);
							BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.RemoveBulletinMsg(msgID);
							if (null != bulletinMsgData)
							{
								string gmCmdData = string.Format("-rmbull {0}", msgID);
								GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
								{
									client.ClientData.RoleID,
									"",
									0,
									"",
									0,
									gmCmdData,
									0,
									0,
									GameManager.ServerLineID
								}), null, 0);
							}
						}
					}
					else if (cmdFields.Length == 2)
					{
						string msgID = cmdFields[1].Trim();
						GameManager.BulletinMsgMgr.RemoveBulletinMsg(msgID);
					}
				}
				else if ("-listbull" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举公告", new object[0]), null, true);
						GameManager.BulletinMsgMgr.SendAllBulletinMsgToGM(client);
					}
				}
				else if ("-sysmsg" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -sysmsg 临时公告ID(文字和数字都可以) 临时公告文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string msgID = cmdFields[1].Trim();
							int minutes = 0;
							int playNum = 1;
							string bulletinText = cmdFields[2];
							if (string.IsNullOrEmpty(msgID))
							{
								strinfo = string.Format("临时公告ID不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布临时公告: {0} {1} {2} {3}", new object[]
							{
								msgID,
								minutes,
								playNum,
								bulletinText
							}), null, true);
							BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText, 1);
							GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData, 0, 0);
							string gmCmdData = string.Format("-sysmsg {0} {1} {2} {3}", new object[]
							{
								msgID,
								minutes,
								playNum,
								bulletinText
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								gmCmdData,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string msgID = cmdFields[1].Trim();
						int minutes = 0;
						int playNum = 1;
						string bulletinText = cmdFields[2];
						BulletinMsgData bulletinMsgData = GameManager.BulletinMsgMgr.AddBulletinMsg(msgID, minutes, playNum, bulletinText, 1);
						GameManager.ClientMgr.NotifyAllBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, bulletinMsgData, 0, 0);
					}
				}
				else if ("-hintmsg" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -hintmsg 消息类型 显示类型 提示文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int infoType = this.SafeConvertToInt32(cmdFields[1]);
							int showType = this.SafeConvertToInt32(cmdFields[2]);
							string hintMsgText = cmdFields[3];
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布提示消息: {0} {1} {2}", infoType, showType, hintMsgText), null, true);
							GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType, 0, 0, 0, 100, 100);
							string gmCmdData = string.Format("-hintmsg {0} {1} {2}", infoType, showType, hintMsgText);
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								gmCmdData,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 4)
					{
						int infoType = this.SafeConvertToInt32(cmdFields[1]);
						int showType = this.SafeConvertToInt32(cmdFields[2]);
						string hintMsgText = cmdFields[3];
						GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType, 0, 0, 0, 100, 100);
					}
				}
				else if ("-hintmsg2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 5)
						{
							strinfo = string.Format("请输入： -hintmsg2 帮会ID 消息类型 显示类型 提示文字", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							int faction = this.SafeConvertToInt32(cmdFields[1]);
							int infoType = this.SafeConvertToInt32(cmdFields[2]);
							int showType = this.SafeConvertToInt32(cmdFields[3]);
							string hintMsgText = cmdFields[4];
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发布帮会提示消息: {0} {1} {2} {3}", new object[]
							{
								faction,
								infoType,
								showType,
								hintMsgText
							}), null, true);
							string gmCmdData = string.Format("-hintmsg2 {0} {1} {2} {3}", new object[]
							{
								faction,
								infoType,
								showType,
								hintMsgText
							});
							GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
							{
								client.ClientData.RoleID,
								"",
								0,
								"",
								0,
								gmCmdData,
								0,
								0,
								GameManager.ServerLineID
							}), null, 0);
						}
					}
					else if (cmdFields.Length >= 5)
					{
						int faction = this.SafeConvertToInt32(cmdFields[1]);
						int infoType = this.SafeConvertToInt32(cmdFields[2]);
						int showType = this.SafeConvertToInt32(cmdFields[3]);
						string hintMsgText = cmdFields[4];
						GameManager.ClientMgr.NotifyBangHuiImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, faction, hintMsgText, (GameInfoTypeIndexes)infoType, (ShowGameInfoTypes)showType, 0);
					}
				}
				else if ("-zp" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("if" == cmdFields[1])
					{
						ZhuanPanManager.getInstance().ProcessZhuanPanInfoCmd(client, 1810, null, cmdFields);
					}
					else if ("cj" == cmdFields[1])
					{
						string[] args = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						ZhuanPanManager.getInstance().ProcessZhuanPanChouJiangCmd(client, 1811, null, args);
					}
				}
				else if ("-sq" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("1" == cmdFields[1])
					{
						string[] args = new string[]
						{
							client.ClientData.RoleID.ToString()
						};
						ShenQiManager.getInstance().ProcessShenQiInfoCmd(client, 1816, null, args);
					}
					else if ("2" == cmdFields[1])
					{
						string[] args = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						ShenQiManager.getInstance().ProcessShenQiUpCmd(client, 1817, null, args);
					}
					else if ("3" == cmdFields[1])
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, Convert.ToInt32(cmdFields[2]), "GM添加", true, true);
					}
					else if ("4" == cmdFields[1])
					{
						ShenQiData data = new ShenQiData
						{
							ShenQiID = Convert.ToInt32(cmdFields[2])
						};
						client.ClientData.shenQiData = data;
						List<int> props = new List<int>();
						props.AddRange(new int[]
						{
							data.ShenQiID,
							data.LifeAdd,
							data.AttackAdd,
							data.DefenseAdd,
							data.ToughnessAdd
						});
						Global.SaveRoleParamsIntListToDB(client, props, "36", true);
						ShenQiManager.getInstance().UpdateRoleShenQiProps(client);
						ShenQiManager.getInstance().UpdateRoleTouhgnessProps(client);
						ShenQiManager.getInstance().UpdateRoleGodProps(client);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
				else if ("-caiji" == cmdFields[0])
				{
					if ("3" == cmdFields[1])
					{
						string[] args = new string[]
						{
							client.ClientData.RoleID.ToString()
						};
						LingDiCaiJiManager.getInstance().ProcessLingZhuSetDoubleOpenCmd(client, 1832, null, args);
					}
					else if ("4" == cmdFields[1])
					{
						string[] args = new string[]
						{
							client.ClientData.RoleID.ToString(),
							cmdFields[2]
						};
						LingDiCaiJiManager.getInstance().ProcessLingDiEnterCmd(client, 1829, null, args);
					}
					else if ("5" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						int lingzhu = Convert.ToInt32(cmdFields[3]);
						if (lingzhu == 1 && client.ClientData.JunTuanId > 0)
						{
							LingDiCaiJiManager.getInstance().SetLingZhu(Convert.ToInt32(cmdFields[2]), client.ClientData.RoleID, client.ClientData.JunTuanId, client.ClientData.JunTuanName, null);
						}
						else
						{
							LingDiCaiJiManager.getInstance().SetLingZhu(Convert.ToInt32(cmdFields[2]), 0, 0, "", null);
						}
					}
					else if ("6" == cmdFields[1])
					{
						if (cmdFields.Length < 3)
						{
							return true;
						}
						int lingDiType = Convert.ToInt32(cmdFields[2]);
						if (lingDiType < 0 || lingDiType > 1)
						{
							return true;
						}
						int roleNum = LingDiCaiJiManager.getInstance().GetLingDiRoleNum(lingDiType);
						string mesg = (lingDiType == 0) ? "地宫人数：" : "荒漠人数：";
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mesg + roleNum);
					}
					else if ("sync" == cmdFields[1])
					{
						LingDiCaiJiManager.getInstance().SetSync();
					}
				}
				else if ("-nlhx" == cmdFields[0])
				{
					int nengLiangType = Convert.ToInt32(cmdFields[1]);
					int addValue = Convert.ToInt32(cmdFields[2]);
					BuildingManager.getInstance().ModifyNengLiangPointsValue(client, nengLiangType, addValue, "GM修改", true, true);
				}
				else if ("-jx" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("mohua" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						int jie = Math.Max(Convert.ToInt32(cmdFields[2]) - 1, 0);
						int lel = Convert.ToInt32(cmdFields[3]);
						int realLev = jie * 11 + lel;
						AwakenLevelItem nextAwakenLevel;
						if (!JueXingManager.getInstance().JueXingRunTimeData.AwakenLevelDict.TryGetValue(realLev, out nextAwakenLevel))
						{
							return true;
						}
						Global.SaveRoleParamsInt32ValueToDB(client, "10193", realLev, true);
						client.ClientData.JueXingData.JueXingJie = nextAwakenLevel.Order;
						client.ClientData.JueXingData.JueXingJi = nextAwakenLevel.Star;
						JueXingManager.getInstance().UpdataPalyerJueXingAttr(client, true);
					}
				}
				else if ("-dacaiji" == cmdFields[0])
				{
					if (cmdFields.Length < 2)
					{
						return true;
					}
					if ("cl" == cmdFields[1])
					{
						if (cmdFields.Length < 3)
						{
							return true;
						}
						client.ClientData.CurrentLifeV = Convert.ToInt32((double)client.ClientData.LifeV * Convert.ToDouble(cmdFields[2]));
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					else if ("al" == cmdFields[1])
					{
						List<Monster> monsterList = GameManager.MonsterMgr.FindMonsterAll(client.ClientData.MapCode);
						foreach (Monster item in monsterList)
						{
							if (cmdFields.Length < 3)
							{
								return true;
							}
							item.VLife = (double)Convert.ToInt32(item.MonsterInfo.VLifeMax * Convert.ToDouble(cmdFields[2]));
							ClientManager.NotifySelfEnemyInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, item.RoleID, 0, 0, item.VLife, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					else if ("ca" == cmdFields[1])
					{
						for (int i = 177; i > 10; i--)
						{
							if (i != 18 && i != 13 && i != 15)
							{
								client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
								{
									PropsSystemTypes.GM_Temp_Props,
									i,
									0.0 - RoleAlgorithm.GetExtProp(client, i)
								});
							}
						}
						for (int i = 10; i > 6; i--)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								PropsSystemTypes.GM_Temp_Props,
								i,
								1000.0 - RoleAlgorithm.GetExtProp(client, i)
							});
						}
						for (int i = 6; i > 2; i--)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								PropsSystemTypes.GM_Temp_Props,
								i,
								0.0 - RoleAlgorithm.GetExtProp(client, i)
							});
						}
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							PropsSystemTypes.GM_Temp_Props,
							13,
							1000000.0 - RoleAlgorithm.GetExtProp(client, 13)
						});
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					else if ("0" == cmdFields[1])
					{
						if (cmdFields.Length < 4)
						{
							return true;
						}
						ExtPropIndexes attribIndex = (ExtPropIndexes)Global.SafeConvertToInt32(cmdFields[2]);
						double attribValue = Convert.ToDouble(cmdFields[3]);
						client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
						{
							PropsSystemTypes.GM_Temp_Props,
							(int)attribIndex,
							attribValue - RoleAlgorithm.GetExtProp(client, (int)attribIndex)
						});
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				else if ("-config" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -config 参数名称 参数值", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string paramName = cmdFields[1].Trim();
							string paramValue = cmdFields[2].Trim();
							if (string.IsNullOrEmpty(paramName))
							{
								strinfo = string.Format("参数名称不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							if (string.IsNullOrEmpty(paramValue))
							{
								strinfo = string.Format("参数值不能为空", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求更改游戏参数: {0}=>{1}", paramName, paramValue), null, true);
							Global.UpdateDBGameConfigg(paramName, paramValue);
						}
					}
					else if (cmdFields.Length >= 3)
					{
						string paramName = cmdFields[1].Trim();
						string paramValue = cmdFields[2].Trim();
						if (string.IsNullOrEmpty(paramName))
						{
							return true;
						}
						if (string.IsNullOrEmpty(paramValue))
						{
							return true;
						}
						if ("qinggongyan_joincount" == paramName || "qinggongyan_joinmoney" == paramName || "vip_fullpurchase" == paramName || "everydayact" == paramName || "bhmatch_goldjoin" == paramName || "era_rank_award" == paramName || "czfl_fullpurnum" == paramName || string.Compare("comp_monster_", 0, paramName, 0, "comp_monster_".Length) == 0 || string.Compare("reborn_boss_", 0, paramName, 0, "reborn_boss_".Length) == 0 || "specpact" == paramName || "ZorkAwardSeasonID" == paramName)
						{
							return true;
						}
						GameManager.GameConfigMgr.SetGameConfigItem(paramName, paramValue);
						GameManager.ServerMonitor.SetNeedReload();
						int index = 0;
						GameClient gc;
						while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
						{
							gc.sendCmd(1842, paramName + ":" + paramValue, false);
						}
						if ("kaifutime" == paramName)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("userbegintime" == paramName)
						{
							UserReturnManager.getInstance().UpdateUserReturnState();
						}
						else if ("jieristartday" == paramName)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("hefutime" == paramName)
						{
							ReloadXmlManager.ReloadAllXmlFile();
						}
						else if ("yueduchoujiangstartday" == paramName)
						{
							HuodongCachingMgr.ResetYueDuZhuanPanActivity();
						}
						else if ("whiteiplist" == paramName)
						{
							Program.LoadIPList(paramValue);
						}
						else if ("nochecktime" == paramName)
						{
							if (paramValue == "1")
							{
								GameManager.GM_NoCheckTokenTimeRemainMS = 3600000L;
							}
							else
							{
								GameManager.GM_NoCheckTokenTimeRemainMS = 0L;
							}
						}
						else if ("lixianguaji" == paramName)
						{
							GameManager.FlagLiXianGuaJi = Global.SafeConvertToInt32(paramValue);
						}
						else if ("optimization_bag_reset" == paramName)
						{
							GameManager.Flag_OptimizationBagReset = (Global.SafeConvertToInt32(paramValue) > 0);
						}
						else if ("checkservertime" == paramName)
						{
							GameManager.ConstCheckServerTimeDiffMinutes = Global.SafeConvertToInt32(paramValue);
						}
						else if ("hideflags" == paramName)
						{
							if (cmdFields.Length >= 4)
							{
								bool enable = Global.SafeConvertToInt32(paramValue) > 0;
								int type = Global.SafeConvertToInt32(cmdFields[3].Trim());
								GameManager.ResetHideFlagsMaps(enable, type);
							}
						}
						else if ("maxposcmdnum" == paramName)
						{
							TCPSession.SetMaxPosCmdNumPer5Seconds(10);
						}
						else if ("maxsubticks" == paramName)
						{
							TCPSession.MaxAntiProcessJiaSuSubTicks = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubticks", 1000);
						}
						else if ("maxsubnum" == paramName)
						{
							TCPSession.MaxAntiProcessJiaSuSubNum = GameManager.GameConfigMgr.GetGameConfigItemInt("maxsubnum", 3);
						}
						else if ("loginwebkey" == paramName)
						{
							if (!string.IsNullOrEmpty(paramValue) && paramValue.Length >= 5)
							{
								TCPCmdHandler.WebKey = paramValue;
							}
							else
							{
								TCPCmdHandler.WebKey = TCPCmdHandler.WebKeyLocal;
							}
						}
						else if ("userwaitconfig" == paramName || "vipwaitconfig" == paramName)
						{
							GameManager.loginWaitLogic.LoadConfig();
						}
						GameManager.LoadGameConfigFlags();
					}
					if (cmdFields.Length >= 3)
					{
						string paramName = cmdFields[1].Trim();
						string paramValue = cmdFields[2].Trim();
						if ("logflags" == paramName)
						{
							if (cmdFields.Length >= 3)
							{
								GameManager.SetLogFlags(Global.SafeConvertToInt64(paramValue));
								GameManager.GameConfigMgr.UpdateGameConfigItem(paramName, paramValue, true);
							}
						}
					}
				}
				else if ("-listconfig" == cmdFields[0])
				{
					if (!transmit)
					{
						LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举游戏参数", new object[0]), null, true);
						GameManager.GameConfigMgr.SendAllGameConfigItemsToGM(client);
					}
				}
				else if ("-holdqinggongyan" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						GameManager.QingGongYanMgr.HoldQingGongYan(client, Convert.ToInt32(cmdFields[1]), 0);
					}
				}
				else if ("-joinqinggongyan" == cmdFields[0])
				{
					GameManager.QingGongYanMgr.JoinQingGongYan(client);
				}
				else if ("-qingkongpet" == cmdFields[0])
				{
					List<GoodsData> goodslist = client.ClientData.PetList;
					for (int i = goodslist.Count - 1; i >= 0; i--)
					{
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodslist[i], 1, false, false);
					}
				}
				else if ("-callpet" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						string strGetGoods = "";
						CallPetManager.CallPet(client, Convert.ToInt32(cmdFields[1]), out strGetGoods);
					}
				}
				else if ("-movepet" == cmdFields[0])
				{
					if (cmdFields.Length == 2)
					{
						CallPetManager.MovePet(client, Convert.ToInt32(cmdFields[1]));
					}
				}
				else if ("-setjmrate" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -setjmrate 角色名称 冲穴成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int jmRate = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置冲穴的成功率倍数{1}", otherRoleName, jmRate), null, true);
							otherClient.ClientData.TempJMChongXueRate = jmRate;
							strinfo = string.Format("为{0}设置冲穴成功率倍数{1}", otherRoleName, jmRate);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-sethrate1" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -sethrate1 角色名称 坐骑强化成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int rate = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑强化的成功率倍数{1}", otherRoleName, rate), null, true);
							otherClient.ClientData.TempHorseEnchanceRate = rate;
							strinfo = string.Format("为{0}设置坐骑强化成功率倍数{1}", otherRoleName, rate);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-sethrate2" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -sethrate1 角色名称 坐骑进阶成功率倍数", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int rate = this.SafeConvertToInt32(cmdFields[2]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑进阶的成功率倍数{1}", otherRoleName, rate), null, true);
							otherClient.ClientData.TempHorseEnchanceRate = rate;
							strinfo = string.Format("为{0}设置坐骑进阶成功率倍数{1}", otherRoleName, rate);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setjmlev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -setjmlev 角色名称 经脉ID 穴位ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int jingMaiID = this.SafeConvertToInt32(cmdFields[2]);
							int jingMaiLevel = this.SafeConvertToInt32(cmdFields[3]);
							if (jingMaiID < 0 || jingMaiID >= 8)
							{
								strinfo = string.Format("经脉的ID{0}超过了范围限制, 应该是:{1}-{2}", jingMaiID, 0, 7);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							jingMaiLevel = Global.GMax(0, jingMaiLevel);
							jingMaiLevel = Global.GMin(Global.MaxJingMaiLevel, jingMaiLevel);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置经脉{1}的穴位为{2}", otherRoleName, Global.GetJingMaiName(jingMaiID), jingMaiLevel), null, true);
							Global.UpdateJingMaiListProps(otherClient, false);
							int jingMaiBodyLevel = otherClient.ClientData.JingMaiBodyLevel;
							int ret = Global.ProcessUpJingmaiLevel(otherClient, jingMaiBodyLevel, jingMaiID, ref jingMaiLevel, 0);
							Global.UpdateJingMaiListProps(otherClient, true);
							if (ret > 0)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
								GameManager.ClientMgr.NotifyJingMaiInfoCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
							}
							GameManager.ClientMgr.NotifyJingMaiResult(otherClient, ret, jingMaiID, jingMaiLevel);
							strinfo = string.Format("为{0}设置经脉{1}的穴位为{2}", otherRoleName, Global.GetJingMaiName(jingMaiID), jingMaiLevel);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if ("-setskilllev" == cmdFields[0])
				{
					if (!transmit)
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -setskilllev 角色名称 技能ID 级别", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							string otherRoleName = cmdFields[1];
							int skillID = this.SafeConvertToInt32(cmdFields[2]);
							int skillLevel = this.SafeConvertToInt32(cmdFields[3]);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							SystemXmlItem systemMagicItem = null;
							if (!GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(skillID, out systemMagicItem))
							{
								strinfo = string.Format("技能ID{0}在系统中不存在", skillID);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							skillLevel = Global.GMax(0, skillLevel);
							skillLevel = Global.GMin(systemMagicItem.GetIntValue("MaxLevel", -1), skillLevel);
							string skillName = systemMagicItem.GetStringValue("Name");
							SkillData skillData = Global.GetSkillDataByID(otherClient, skillID);
							if (null == skillData)
							{
								strinfo = string.Format("{0}尚未学习{1}技能", otherRoleName, skillName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置技能{1}的级别为{2}", otherRoleName, skillName, skillLevel), null, true);
							skillData.SkillLevel = skillLevel;
							GameManager.ClientMgr.UpdateSkillInfo(client, skillData, true);
							if (systemMagicItem.GetIntValue("MagicType", -1) < 0)
							{
								Global.RefreshSkillForeverProps(otherClient);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
							}
							string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								otherClient.ClientData.RoleID,
								skillData.DbID,
								skillData.SkillLevel,
								skillData.UsedNum
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 216);
							Global._TCPManager.MySocketListener.SendData(otherClient.ClientSocket, tcpOutPacket, true);
							strinfo = string.Format("为{0}设置技能{1}的级别为{2}", otherRoleName, skillName, skillLevel);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
				}
				else if (!("-setskillum" == cmdFields[0]))
				{
					if ("-del" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -del 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求从数据库删除:{0}", otherRoleName), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10068, string.Format("{0}", otherRoleName), null, 0);
								strinfo = string.Format("从数据库删除{0}", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-undel" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -undel 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为恢复删除:{0}", otherRoleName), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10052, string.Format("{0}", otherRoleName), null, 0);
								strinfo = string.Format("为{0}恢复删除", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-modlimit" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -modlimit 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int maxLimit = this.SafeConvertToInt32(cmdFields[1]);
								Global._TCPManager.MaxConnectedClientLimit = maxLimit;
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为修改最大在线人数{0}", maxLimit), null, true);
								strinfo = string.Format("修改最大在线人数限制为{0}", maxLimit);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							int maxLimit = this.SafeConvertToInt32(cmdFields[1]);
							Global._TCPManager.MaxConnectedClientLimit = maxLimit;
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为修改最大在线人数{0}", maxLimit), null, true);
						}
					}
					else if ("-listhorse" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -listhorse 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求列举{0}的坐骑ID列表", otherRoleName), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								if (null == otherClient.ClientData.HorsesDataList)
								{
									strinfo = string.Format("{0}的坐骑ID列表为空", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								}
								List<string> msgList = new List<string>();
								lock (otherClient.ClientData.HorsesDataList)
								{
									for (int i = 0; i < otherClient.ClientData.HorsesDataList.Count; i++)
									{
										strinfo = string.Format("{0} {1} {2}/{3}/{4}", new object[]
										{
											otherClient.ClientData.HorsesDataList[i].DbID,
											Global.GetHorseNameByID(otherClient.ClientData.HorsesDataList[i].HorseID),
											Global.GetHorseFailedNum(otherClient.ClientData.HorsesDataList[i]),
											Global.GetHorseHorseBlessPoint(otherClient.ClientData.HorsesDataList[i].HorseID + 1)
										});
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									}
								}
							}
						}
					}
					else if ("-sethl" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								strinfo = string.Format("请输入：-sethl 角色名称 坐骑ID(-listhorse得到的ID) 幸运值", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int horseDbID = Global.SafeConvertToInt32(cmdFields[2]);
								int luckyNum = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置坐骑ID{1}的进阶幸运值", otherRoleName, horseDbID), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								HorseData horseData = Global.GetHorseDataByDbID(otherClient, horseDbID);
								if (null == horseData)
								{
									strinfo = string.Format("在{0}的坐骑列表中没有找到ID为{1}的坐骑", otherRoleName, horseDbID);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								}
								Global.UpdateHorseIDDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient, horseData.DbID, horseData.HorseID, luckyNum, Global.GetHorseStrTempTime(horseData), horseData.JinJieTempNum, horseData.JinJieFailedDayID);
								strinfo = string.Format("为{0}的坐骑ID为{1}的设置幸运值{2}", otherRoleName, horseDbID, luckyNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setwlogin" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -setwlogin 角色名称 天数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int dayNum = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置本周的连续登录天数{1}", otherRoleName, dayNum), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								otherClient.ClientData.MyHuodongData.LoginNum = dayNum;
								strinfo = string.Format("为{0}设置本周的连续登录天数{1}", otherRoleName, dayNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setmtime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -setmtime 角色名称 在线时长(秒数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int onlineSecs = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置本月的在线时长{1}", otherRoleName, onlineSecs), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								otherClient.ClientData.MyHuodongData.CurMTime = onlineSecs;
								strinfo = string.Format("为{0}设置本周的连续登录天数{1}", otherRoleName, onlineSecs);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setnstep" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -setnstep 角色名称 当前步骤(1~5)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int newStep = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置新手见面礼物领取的步骤{1}", otherRoleName, newStep), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								long nowTicks = TimeUtil.NOW();
								otherClient.ClientData.MyHuodongData.NewStep++;
								otherClient.ClientData.MyHuodongData.StepTime = nowTicks;
								GameManager.ClientMgr.NotifyHuodongData(otherClient);
								strinfo = string.Format("为{0}设置新手见面礼物领取的步骤{1}", otherRoleName, newStep);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-updateBindgold" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 5)
							{
								string userID = cmdFields[1];
								int rid = Global.SafeConvertToInt32(cmdFields[2]);
								int bindGold = Global.SafeConvertToInt32(cmdFields[3]);
								string chargeInfo = cmdFields[4];
								TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
								GameClient otherClient = null;
								if (null != clientSocket)
								{
									otherClient = GameManager.ClientMgr.FindClient(clientSocket);
								}
								UserMoneyMgr.getInstance().ProcessSendBindGold(otherClient, bindGold, userID, rid, chargeInfo);
							}
						}
					}
					else if ("-removefriend" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 3)
							{
								int rid = Global.SafeConvertToInt32(cmdFields[1]);
								int friendDbID = Global.SafeConvertToInt32(cmdFields[2]);
								GameClient otherClient = GameManager.ClientMgr.FindClient(rid);
								if (null == otherClient)
								{
									return true;
								}
								Global.RemoveFriendData(otherClient, friendDbID);
							}
						}
					}
					else if ("-addcharge" == cmdFields[0])
					{
						long addMoney = (long)Global.SafeConvertToInt32(cmdFields[1]);
						HongBaoManager.getInstance().AddChargeValue(addMoney);
					}
					else if ("-updateyb" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 6)
							{
								string userID = cmdFields[1];
								int rid = Global.SafeConvertToInt32(cmdFields[2]);
								int addMoney2 = Global.SafeConvertToInt32(cmdFields[3]);
								int superInputFanLi = Global.SafeConvertToInt32(cmdFields[4]);
								int addItemID = Global.SafeConvertToInt32(cmdFields[5]);
								int moneyToYuanBao = GameManager.GameConfigMgr.GetGameConfigItemInt("money-to-yuanbao", 10);
								HongBaoManager.getInstance().AddChargeValue((long)(addMoney2 * moneyToYuanBao));
								SingleChargeData chargeData = Data.ChargeData;
								if (chargeData != null && addItemID == 0 && chargeData.YueKaMoney > 0 && addMoney2 == chargeData.YueKaMoney)
								{
									if (rid > 0)
									{
										int check = Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(rid, "10167", 0));
										if (check < 10)
										{
											if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(rid, 4))
											{
												GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", rid, "10167", 10 + check), null, GameCoreInterface.getinstance().GetLocalServerId());
											}
										}
									}
								}
								if (addItemID != 0)
								{
									TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
									if (null == clientSocket)
									{
										return true;
									}
									GameClient otherClient = GameManager.ClientMgr.FindClient(clientSocket);
									if (null == otherClient)
									{
										return true;
									}
									if (rid != otherClient.ClientData.RoleID)
									{
										return true;
									}
									UserMoneyMgr.getInstance().HandleClientChargeItem(otherClient, 0);
									GameManager.logDBCmdMgr.AddDBLogInfo(-1, "直购", "GM命令强迫更新", "系统", otherClient.ClientData.RoleName, "增加", addItemID, otherClient.ClientData.ZoneID, otherClient.strUserID, otherClient.ClientData.UserMoney, otherClient.ServerId, null);
								}
								else if (rid == -1)
								{
									UserMoneyMgr.getInstance().HandleSystemChargeMoney(userID, addMoney2);
								}
								else
								{
									UserMoneyMgr.getInstance().HandleClientChargeMoney(userID, rid, addMoney2, transmit, superInputFanLi);
								}
							}
						}
					}
					else if ("-buyyueka" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 3)
							{
								string userID = cmdFields[1];
								int roleID = Global.SafeConvertToInt32(cmdFields[2]);
								YueKaManager.HandleUserBuyYueKa(userID, roleID);
							}
							else if (cmdFields.Length == 2)
							{
								string userID = "not set";
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								YueKaManager.HandleUserBuyYueKa(userID, roleID);
							}
						}
					}
					else if ("-setfbnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								strinfo = string.Format("请输入： -setfbnum 角色名称 副本ID 当日次数(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int fuBenID = Global.SafeConvertToInt32(cmdFields[2]);
								int addDayNum = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", otherRoleName, fuBenID, addDayNum), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								Global.UpdateFuBenData(otherClient, fuBenID, addDayNum, addDayNum);
								strinfo = string.Format("为{0}增加副本ID{1}的当日次数{2}", otherRoleName, fuBenID, addDayNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-sethdnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 4)
							{
								strinfo = string.Format("请输入： -sethdnum 角色名称 活动代号ID 当日次数(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int huodongID = Global.SafeConvertToInt32(cmdFields[2]);
								int addDayNum = Global.SafeConvertToInt32(cmdFields[3]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}增加副本ID{1}的当日次数{2}", otherRoleName, huodongID, addDayNum), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								Global.UpdateDayActivityEnterCountToDB(otherClient, otherClient.ClientData.RoleID, TimeUtil.NowDateTime().DayOfYear, huodongID, addDayNum);
								strinfo = string.Format("为{0}增加活动{1}的当日次数{2}", otherRoleName, ((SpecialActivityTypes)huodongID).ToString(), addDayNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setfreshplayer" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -setfreshplayer 1(启用)或0(禁用)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int enabelFlag = Global.SafeConvertToInt32(cmdFields[1]);
								Global.Flag_EnabelNewPlayerScene = (enabelFlag > 0);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用"), null, true);
								strinfo = string.Format("{0}新手场景", Global.Flag_EnabelNewPlayerScene ? "启用" : "禁用");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addattack" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入：-addattack 角色名称 攻击力(值)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int addAttack = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}临时增加物攻和魔攻{1}", otherRoleName, addAttack), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								otherClient.RoleBuffer.AddForeverExtProp(7, (double)addAttack);
								otherClient.RoleBuffer.AddForeverExtProp(8, (double)addAttack);
								otherClient.RoleBuffer.AddForeverExtProp(9, (double)addAttack);
								otherClient.RoleBuffer.AddForeverExtProp(10, (double)addAttack);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
								strinfo = string.Format("为{0}临时增加物攻和魔攻{1}", otherRoleName, addAttack);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-adddefense" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入：-adddefense 角色名称 防御力(值)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int addDefense = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}临时增加物防和魔防{1}", otherRoleName, addDefense), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								otherClient.RoleBuffer.AddForeverExtProp(3, (double)addDefense);
								otherClient.RoleBuffer.AddForeverExtProp(4, (double)addDefense);
								otherClient.RoleBuffer.AddForeverExtProp(5, (double)addDefense);
								otherClient.RoleBuffer.AddForeverExtProp(6, (double)addDefense);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
								strinfo = string.Format("为{0}临时增加物防和魔防{1}", otherRoleName, addDefense);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setybstate" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入：-setybstate 角色名称 状态(0：成功，1：失败)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int state = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改押镖的状态{1}", otherRoleName, state), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								if (null != otherClient.ClientData.MyYaBiaoData)
								{
									otherClient.ClientData.MyYaBiaoData.State = state;
									GameManager.DBCmdMgr.AddDBCmd(10057, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										otherClient.ClientData.RoleID,
										otherClient.ClientData.MyYaBiaoData.YaBiaoID,
										otherClient.ClientData.MyYaBiaoData.StartTime,
										otherClient.ClientData.MyYaBiaoData.State,
										otherClient.ClientData.MyYaBiaoData.LineID,
										otherClient.ClientData.MyYaBiaoData.TouBao,
										otherClient.ClientData.MyYaBiaoData.YaBiaoDayID,
										otherClient.ClientData.MyYaBiaoData.YaBiaoNum,
										otherClient.ClientData.MyYaBiaoData.TakeGoods
									}), null, otherClient.ServerId);
									GameManager.ClientMgr.NotifyYaBiaoData(otherClient);
									strinfo = string.Format("为{0}修改押镖的状态{1}", otherRoleName, state);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								}
								else
								{
									strinfo = string.Format("{0}当前无运镖数据", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								}
							}
						}
					}
					else if ("-setybstate2" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								int state = Global.SafeConvertToInt32(cmdFields[2]);
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									return true;
								}
								if (null != otherClient.ClientData.MyYaBiaoData)
								{
									otherClient.ClientData.MyYaBiaoData.State = state;
									GameManager.DBCmdMgr.AddDBCmd(10057, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										otherClient.ClientData.RoleID,
										otherClient.ClientData.MyYaBiaoData.YaBiaoID,
										otherClient.ClientData.MyYaBiaoData.StartTime,
										otherClient.ClientData.MyYaBiaoData.State,
										otherClient.ClientData.MyYaBiaoData.LineID,
										otherClient.ClientData.MyYaBiaoData.TouBao,
										otherClient.ClientData.MyYaBiaoData.YaBiaoDayID,
										otherClient.ClientData.MyYaBiaoData.YaBiaoNum,
										otherClient.ClientData.MyYaBiaoData.TakeGoods
									}), null, otherClient.ServerId);
									GameManager.ClientMgr.NotifyYaBiaoData(otherClient);
								}
							}
						}
					}
					else if ("-reload" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-reload xml文件名称,不区分大小写(例如：config/mall.xml)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string xmlFileName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载{0}", xmlFileName), null, true);
								int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
								strinfo = string.Format("重新加载参数{0}, 结果：{1}", xmlFileName, retCode);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string xmlFileName = cmdFields[1];
							int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据转发的GM的要求重新加载{0}, 结果为:{1}", xmlFileName, retCode), null, true);
						}
					}
					else if ("-loadiplist" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-loadiplist (0|1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string openState = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState), null, true);
								Program.LoadIPList(openState);
								strinfo = string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string openState = cmdFields[1];
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载IP白名单列表,设置启用状态: {0}", openState), null, true);
							Program.LoadIPList(openState);
						}
					}
					else if ("-reloadall" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 1)
							{
								strinfo = string.Format("请输入：-reloadall", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载所有配置文件", new object[0]), null, true);
								ReloadXmlManager.ReloadAllXmlFile();
								strinfo = string.Format("重新加载所有参数配置文件", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 1)
						{
							ReloadXmlManager.ReloadAllXmlFile();
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据转发的GM的要求重新加载所有参数配置文件", new object[0]), null, true);
						}
						foreach (GameClient c in GameManager.ClientMgr.GetAllClients(true))
						{
							c.sendCmd<SystemOpenData>(718, Data.OpenData, false);
						}
					}
					else if ("-reload2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-reload2 xml文件名称,不区分大小写(例如：config/mall.xml)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string xmlFileName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求通知所有线重新加载{0}", xmlFileName), null, true);
								int retCode = ReloadXmlManager.ReloadXmlFile(xmlFileName);
								strinfo = string.Format("所有线重新加载参数{0}, 结果：{1}", xmlFileName, retCode);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								if (0 == retCode)
								{
									string gmCmdData = string.Format("-reload {0}", cmdFields[1]);
									GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										"",
										0,
										"",
										0,
										gmCmdData,
										0,
										0,
										GameManager.ServerLineID
									}), null, 0);
								}
							}
						}
					}
					else if ("-reloadgm" == cmdFields[0])
					{
						if (!transmit)
						{
							if (isSuperGMUser)
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据超级GM的要求重新加载GM参数", new object[0]), null, true);
								int retCode = this.ReloadGMCommands();
								strinfo = string.Format("重新加载GM参数, 结果：{0}", retCode);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据超级GM的要求重新加载GM参数", new object[0]), null, true);
							int retCode = this.ReloadGMCommands();
						}
					}
					else if ("-reloadph" == cmdFields[0])
					{
						if (!transmit)
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载排行榜", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10066, string.Format("{0}", client.ClientData.RoleID), null, 0);
							strinfo = string.Format("重新加载排行榜成功", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求重新加载排行榜", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10066, string.Format("{0}", 0), null, 0);
						}
					}
					else if ("-updatepaihangbang" == cmdFields[0])
					{
						for (int loop = 0; loop < 23; loop++)
						{
							string strcmd = StringUtil.substitute("{0}:{1}", new object[]
							{
								0,
								loop
							});
							PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, strcmd, 0);
							if (paiHangData != null && null != paiHangData.PaiHangList)
							{
								List<PaiHangItemData> PaiHangList = paiHangData.PaiHangList;
								int i = 0;
								while (i < 50 && i < PaiHangList.Count)
								{
									string strText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										PaiHangList[i].uid,
										PaiHangList[i].RoleID,
										PaiHangList[i].RoleName,
										PaiHangList[i].Val1,
										PaiHangList[i].Val2,
										PaiHangList[i].Val3
									});
									EventLogManager.AddRankingEvent((PaiHangTypes)loop, i + 1, strText);
									i++;
								}
							}
						}
					}
					else if ("-everyday" == cmdFields[0])
					{
						if (!transmit)
						{
							EverydayActivity act3 = HuodongCachingMgr.GetEverydayActivity();
							if (null != act3)
							{
								act3.ShowActiveConditionInfoGM(client);
							}
						}
					}
					else if ("-clrrolecache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-clrrolecache 角色名称(带区号)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleName), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 0, otherRoleName), null, 0);
								strinfo = string.Format("清空{0}角色的数据库缓存成功", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string otherRoleName = cmdFields[1];
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleName), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 0, otherRoleName), null, 0);
						}
					}
					else if ("-clrusercache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-clrusercache userid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}帐号的数据库缓存", otherRoleName), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 1, otherRoleName), null, 0);
								strinfo = string.Format("清空{0}角色的数据库缓存成功", otherRoleName);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string otherRoleName = cmdFields[1];
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}帐号的数据库缓存", otherRoleName), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", 1, otherRoleName), null, 0);
						}
					}
					else if ("-clrrolecachebyid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-clrrolecachebyid rid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleID), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", otherRoleID, ""), null, 0);
								strinfo = string.Format("清空rid={0}的角色的数据库缓存成功", "");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空{0}角色的数据库缓存", otherRoleID), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10081, string.Format("{0}:{1}", otherRoleID, ""), null, 0);
						}
					}
					else if ("-clrallrolecache" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 1)
							{
								strinfo = string.Format("请输入：-clrallrolecache", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空所有角色的数据库缓存", new object[0]), null, true);
								GameManager.DBCmdMgr.AddDBCmd(10122, string.Format("{0}", client.ClientData.RoleID), null, 0);
								strinfo = string.Format("清空所有角色的数据库缓存成功", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 1)
						{
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求清空所有角色的数据库缓存", new object[0]), null, true);
							GameManager.DBCmdMgr.AddDBCmd(10122, string.Format("{0}", 0), null, 0);
						}
					}
					else if ("-addheroidx" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入：-addheroidx 角色名称 层数(大于等于0，小于等于13)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int heroIndex = Global.SafeConvertToInt32(cmdFields[2]);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改英雄逐擂的到达层数{1}", otherRoleName, heroIndex), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameManager.ClientMgr.ChangeRoleHeroIndex(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, heroIndex, true);
								strinfo = string.Format("为{0}修改英雄逐擂的到达层数{1}", otherRoleName, heroIndex);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setlz" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入：-setlz  角色名称 连斩数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int lianZhanNum = Global.SafeConvertToInt32(cmdFields[2]);
								lianZhanNum = Global.GMin(899, lianZhanNum);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}修改连斩数{1}", otherRoleName, lianZhanNum), null, true);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameManager.ClientMgr.ChangeRoleLianZhan(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, null, lianZhanNum);
								strinfo = string.Format("为{0}修改连斩数{1}", otherRoleName, lianZhanNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-applytobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								string roleName = cmdFields[2];
								int bhid = Global.SafeConvertToInt32(cmdFields[3]);
								string bhName = cmdFields[4];
								string roleList = cmdFields[5];
								GameManager.ClientMgr.NotifyOnlineBangHuiMgrRoleApplyMsg(roleID, roleName, bhid, bhName, roleList);
							}
						}
					}
					else if ("-joinbh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 4)
							{
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								int bhid = Global.SafeConvertToInt32(cmdFields[2]);
								string bhName = cmdFields[3];
								GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
								if (null != otherClient)
								{
									GameManager.ClientMgr.NotifyJoinBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, bhid, bhName);
								}
							}
						}
					}
					else if ("-joinbhtime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length >= 2)
							{
								if (null != client)
								{
									TimeSpan timeSpan = default(TimeSpan);
									string datatimeStr;
									if (cmdFields.Length == 2)
									{
										datatimeStr = cmdFields[1].Replace('：', ':');
										if (TimeSpan.TryParse(datatimeStr, out timeSpan))
										{
											datatimeStr = TimeUtil.NowDateTime().Add(-timeSpan).ToString("yyyy-MM-dd HH:mm:ss");
										}
										else
										{
											datatimeStr = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
										}
									}
									else
									{
										datatimeStr = cmdFields[1] + " " + cmdFields[2];
									}
									datatimeStr = datatimeStr.Replace('：', ':');
									DateTime dt;
									if (DateTime.TryParse(datatimeStr, out dt))
									{
										Global.SaveRoleParamsInt32ValueToDB(client, "EnterBangHuiUnixSecs", DataHelper.SysTicksToUnixSeconds(dt.Ticks / 10000L), true);
										string strInfo = string.Format("设置角色加入帮会时间为{0}", dt.ToString().Replace(':', '：'));
										GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									}
								}
							}
						}
					}
					else if ("-leavebh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								int bhid = Global.SafeConvertToInt32(cmdFields[2]);
								string bhName = cmdFields[3];
								int leaveType = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
								if (null != otherClient)
								{
									GameManager.ClientMgr.NotifyLeaveBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, bhid, bhName, leaveType);
								}
							}
						}
					}
					else if ("-destroybh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 4)
							{
								int retCode = Global.SafeConvertToInt32(cmdFields[1]);
								int roleID = Global.SafeConvertToInt32(cmdFields[2]);
								int bhid = Global.SafeConvertToInt32(cmdFields[3]);
								GameManager.ClientMgr.NotifyBangHuiDestroy(retCode, roleID, bhid);
								HongBaoManager.getInstance().OnDestoryZhanMeng(bhid);
							}
						}
					}
					else if ("-autodestroybh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								int bhid = Global.SafeConvertToInt32(cmdFields[1]);
								GameManager.ClientMgr.NotifyBangHuiDestroy(0, 0, bhid);
								JunQiManager.SendClearJunQiCmd(bhid);
								JunQiManager.NotifySyncBangHuiJunQiItemsDict(null);
								JunQiManager.NotifySyncBangHuiLingDiItemsDict();
								Global.RemoveBangHuiMiniData(bhid);
							}
						}
					}
					else if ("-invitetobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								int inviteRoleID = Global.SafeConvertToInt32(cmdFields[2]);
								string inviteRoleName = cmdFields[3];
								int bhid = Global.SafeConvertToInt32(cmdFields[4]);
								string bhName = cmdFields[5];
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									GameManager.ClientMgr.NotifyInviteToBangHui(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, inviteRoleID, inviteRoleName, bhid, bhName, otherClient.ClientData.ChangeLifeCount);
								}
							}
						}
					}
					else if ("-refusetobh" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								string refreseRoleName = cmdFields[2];
								string bhName = cmdFields[3];
								int refuseType = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									if (0 == refuseType)
									{
										GameManager.ClientMgr.NotifyRefuseApplyToBHMember(otherClient, refreseRoleName, bhName);
									}
									else
									{
										GameManager.ClientMgr.NotifyRefuseInviteToBHMember(otherClient, refreseRoleName, bhName);
									}
								}
							}
						}
					}
					else if ("-syncjunqi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								JunQiManager.LoadBangHuiJunQiItemsDictFromDBServer();
							}
						}
					}
					else if ("-synclingdi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 1)
							{
								JunQiManager.LoadBangHuiLingDiItemsDictFromDBServer();
							}
						}
					}
					else if ("-syncldzresult" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 6)
							{
								int lingDiID = Global.SafeConvertToInt32(cmdFields[1]);
								int mapCode = Global.SafeConvertToInt32(cmdFields[2]);
								int bhid = Global.SafeConvertToInt32(cmdFields[3]);
								int zoneID = Global.SafeConvertToInt32(cmdFields[4]);
								string bhName = cmdFields[5];
								JunQiManager.HandleLingDiZhanResultByMapCode2(lingDiID, mapCode, bhid, zoneID, bhName);
							}
						}
					}
					else if ("-chbhzhiwu" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int bhid = Global.SafeConvertToInt32(cmdFields[1]);
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[2]);
								int zhiWu = Global.SafeConvertToInt32(cmdFields[3]);
								int oldZhiWuRoleID = Global.SafeConvertToInt32(cmdFields[4]);
								GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
								if (null != otherClient)
								{
									if (otherClient.ClientData.Faction == bhid)
									{
										otherClient.ClientData.BHZhiWu = zhiWu;
										GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
									}
								}
								otherClient = GameManager.ClientMgr.FindClient(oldZhiWuRoleID);
								if (null != otherClient)
								{
									if (otherClient.ClientData.Faction == bhid)
									{
										otherClient.ClientData.BHZhiWu = 0;
										GameManager.ClientMgr.ChangeBangHuiZhiWu(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
									}
								}
							}
						}
					}
					else if ("-removehuangfei" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								string huangDiRoleName = cmdFields[2];
								GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
								if (null != otherClient)
								{
									Global.UpdateRoleHuangHou(otherClient, 0, huangDiRoleName);
								}
							}
						}
					}
					else if ("-leavelaofang" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int otherRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								string huangDiRoleName = cmdFields[2];
								GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
								if (null != otherClient)
								{
									Global.ForceTakeOutLaoFangMap(otherClient, otherClient.ClientData.PKPoint);
									Global.BroadcastTakeOutLaoFangHint(huangDiRoleName, Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName));
								}
							}
						}
					}
					else if ("-clearmap" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 2)
							{
								int bhid = Global.SafeConvertToInt32(cmdFields[1]);
								JunQiManager.ProcessDelAllJunQiByBHID(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, bhid);
							}
						}
					}
					else if ("-synchuangdi" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 5)
							{
								int oldRoleID = Global.SafeConvertToInt32(cmdFields[1]);
								int roleID = Global.SafeConvertToInt32(cmdFields[2]);
								string roleName = cmdFields[3];
								string bhName = cmdFields[4];
								HuangChengManager.ProcessTakeSheLiZhiYuan(roleID, roleName, bhName, false);
								GameManager.ClientMgr.NotifyAllChgHuangDiRoleIDMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, oldRoleID, HuangChengManager.GetHuangDiRoleID());
							}
						}
					}
					else if ("-reloadxml" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入：-reloadxml  文字信息", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string notifyMsg = cmdFields[1];
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求发送重新加载xml的通知消息{0}", notifyMsg), null, true);
								notifyMsg = notifyMsg.Replace(":", "");
								GameManager.ClientMgr.NotifyAllImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, notifyMsg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 16, 0, 0, 100, 100);
								strinfo = string.Format("成功发送重新加载xml的通知信息：{0}成功", notifyMsg);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-notifymail" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 2)
							{
								string[] userIDAndMailIDs = cmdFields[1].Split(new char[]
								{
									'_'
								});
								foreach (string item2 in userIDAndMailIDs)
								{
									string[] pair = item2.Split(new char[]
									{
										'|'
									});
									int roleID = Global.SafeConvertToInt32(pair[0]);
									int mailID = Global.SafeConvertToInt32(pair[1]);
									GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
									if (null != otherClient)
									{
										GameManager.ClientMgr.NotifyLastUserMail(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, mailID);
										otherClient._IconStateMgr.CheckEmailCount(otherClient, true);
									}
								}
							}
						}
					}
					else if ("-notifygmail" == cmdFields[0])
					{
						if (transmit)
						{
							GroupMailManager.RequestNewGroupMailList();
						}
					}
					else if ("-notifyUserReturn" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 2)
							{
								string[] roleIDs = cmdFields[1].Split(new char[]
								{
									'#'
								});
								foreach (string roleID2 in roleIDs)
								{
									if (!string.IsNullOrEmpty(roleID2))
									{
										GameClient otherClient = GameManager.ClientMgr.FindClient(int.Parse(roleID2));
										if (null != otherClient)
										{
											UserReturnManager.getInstance().initUserReturnData(otherClient);
											otherClient._IconStateMgr.AddFlushIconState(14104, true);
											otherClient._IconStateMgr.SendIconStateToClient(otherClient);
										}
									}
								}
							}
						}
					}
					else if ("-resetgmail" == cmdFields[0])
					{
						if (transmit)
						{
							GroupMailManager.ResetData();
							GroupMailManager.RequestNewGroupMailList();
						}
					}
					else if ("-modifyastar" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 2)
							{
								strinfo = string.Format("请输入：-modifyastar 大于等于8的整数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								Global.ModifyMaxOpenNodeCountForAStar(Global.SafeConvertToInt32(cmdFields[1]));
							}
						}
					}
					else if ("-modifylogtype" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 2)
							{
								strinfo = string.Format("请输入：-modifylogtype -1到3的整数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								LogManager.LogTypeToWrite = (LogTypes)Global.SafeConvertToInt32(cmdFields[1]);
							}
						}
					}
					else if ("-reloadnpc" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int mapCode = Global.SafeConvertToInt32(cmdFields[1]);
							NPCGeneralManager.RemoveMapNpcs(mapCode);
							NPCGeneralManager.ReloadMapNPCRoles(mapCode);
						}
						else
						{
							strinfo = string.Format("请输入：-reloadnpc 地图编号", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-modifyparams" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 4)
							{
								string otherRoleName = cmdFields[1];
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int paramIndex = Global.SafeConvertToInt32(cmdFields[2]);
								int modifyValue = Global.SafeConvertToInt32(cmdFields[3]);
								int num7 = paramIndex;
								switch (num7)
								{
								case 0:
									GameManager.ClientMgr.ModifyChengJiuPointsValue(otherClient, modifyValue, "GM指令增加成就", true, true);
									break;
								case 1:
									GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(otherClient, modifyValue, true, true);
									break;
								case 2:
									GameManager.ClientMgr.ModifyLieShaValue(otherClient, modifyValue, true, true);
									break;
								case 3:
									GameManager.ClientMgr.ModifyWuXingValue(otherClient, modifyValue, true, true, true);
									break;
								case 4:
									GameManager.ClientMgr.ModifyZhenQiValue(otherClient, modifyValue, true, true);
									break;
								case 5:
									GameManager.ClientMgr.ModifyTianDiJingYuanValue(otherClient, modifyValue, "GM指令增加魔晶", true, true, false);
									break;
								case 6:
									GameManager.ClientMgr.ModifyShiLianLingValue(otherClient, modifyValue, true, true);
									break;
								case 7:
									break;
								case 8:
									break;
								case 9:
									GameManager.ClientMgr.ModifyZuanHuangLevelValue(otherClient, modifyValue, true, true);
									break;
								case 10:
									GameManager.ClientMgr.ModifySystemOpenValue(otherClient, modifyValue, true, true);
									break;
								case 11:
									GameManager.ClientMgr.ModifyJunGongValue(otherClient, modifyValue, true, true);
									break;
								case 12:
								case 13:
								case 16:
								case 17:
								case 20:
								case 21:
								case 22:
								case 23:
								case 24:
								case 25:
								case 28:
								case 29:
									break;
								case 14:
									GameManager.ClientMgr.ModifyZhanHunValue(otherClient, modifyValue, true, true);
									break;
								case 15:
									GameManager.ClientMgr.ModifyRongYuValue(otherClient, modifyValue, true, true);
									break;
								case 18:
									GameManager.ClientMgr.ModifyShengWangValue(otherClient, modifyValue, "GM指令(modifyparams)增加声望", true, true);
									break;
								case 19:
								{
									int oldValue = GameManager.ClientMgr.GetShengWangLevelValue(client);
									GameManager.ClientMgr.ModifyShengWangLevelValue(otherClient, modifyValue - oldValue, "GM指令(modifyparams)增加声望等级", true, true);
									break;
								}
								case 26:
									FashionManager.getInstance().ModifyFashionWingsID(otherClient, modifyValue, true, true);
									break;
								case 27:
									GameManager.ClientMgr.ModifyZaiZaoValue(otherClient, modifyValue, "GM指令增加再造点", true, true, false);
									break;
								case 30:
									FashionManager.getInstance().ModifyFashionTitleID(otherClient, modifyValue, true, true);
									break;
								default:
									if (num7 != 40)
									{
										switch (num7)
										{
										case 49:
											GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(client, modifyValue, "GM指令(modifyparams)增加符文之尘", true, true, false);
											break;
										case 51:
											GameManager.ClientMgr.ModifyEraDonateValue(client, modifyValue, "GM指令(modifyparams)增加纪元贡献", true, true, false);
											break;
										}
									}
									break;
								}
							}
							else
							{
								strinfo = string.Format("请输入：-modifyparams 角色名称 参数索引(0成就 1积分 2猎杀 3悟性 4真气 5天地精元 6试炼令[===>通天令值] 7经脉等级 8武学等级 9钻皇等级 10系统激活项 11军功) 值(正负)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-lastgold" == cmdFields[0])
					{
						if (!transmit)
						{
							BangHuiMatchManager.getInstance().SwitchLastGoldBH_GM();
						}
					}
					else if ("-fashion" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length != 3)
							{
								strinfo = string.Format("请输入：-fashion 0删除1增加 fashionid", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							int addordel = Global.SafeConvertToInt32(cmdFields[1]);
							int fashionid = Global.SafeConvertToInt32(cmdFields[2]);
							if (addordel > 0)
							{
								FashionManager.getInstance().GetFashionByMagic(client, fashionid, true);
							}
							else
							{
								FashionManager.getInstance().DelFashionByMagic(client, fashionid);
							}
						}
					}
					else if ("-modifyparams2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 4)
							{
								string otherRoleName = cmdFields[1];
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								string paramKey = cmdFields[2];
								string modifyValue2 = cmdFields[3];
								Global.UpdateRoleParamByName(client, paramKey, modifyValue2, true);
								LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求修改角色[{0}]的参数[{1}]为[{2}]", otherRoleName, paramKey, modifyValue2), null, true);
							}
							else
							{
								strinfo = string.Format("请输入：-modifyparams2 角色名称 参数名 值(正负)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-summon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int monsterID = Global.SafeConvertToInt32(cmdFields[1]);
								int num6 = Global.SafeConvertToInt32(cmdFields[2]);
								if (num6 > 0 && monsterID > 0 && num6 < 50)
								{
									GameManager.LuaMgr.AddDynamicMonsters(client, monsterID, num6, (int)client.CurrentGrid.X, (int)client.CurrentGrid.Y, 3);
									strinfo = string.Format("执行GM召唤怪物", new object[0]);
								}
								else
								{
									strinfo = string.Format("请输入：-summon 怪物id 数量[最多49]", new object[0]);
								}
							}
							else
							{
								strinfo = string.Format("请输入：-summon 怪物id 数量[最多49]", new object[0]);
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-addnpc" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int NpcID = Global.SafeConvertToInt32(cmdFields[1]);
							if (NpcID > 0)
							{
								GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
								if (null != gameMap)
								{
									GameManager.LuaMgr.AddNpcToMap(NpcID, client.ClientData.MapCode, (int)client.CurrentGrid.X * gameMap.MapGridWidth, (int)client.CurrentGrid.Y * gameMap.MapGridHeight);
									strinfo = string.Format("执行GM召唤NPC", new object[0]);
								}
								else
								{
									strinfo = string.Format("执行GM召唤NPC 失败", new object[0]);
								}
							}
							else
							{
								strinfo = string.Format("请输入：-addnpc npcid", new object[0]);
							}
						}
						else
						{
							strinfo = string.Format("请输入：-addnpc npcid", new object[0]);
						}
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
					else if ("-refreshqg" == cmdFields[0])
					{
						if (!transmit)
						{
							ReloadXmlManager.ReloadXmlFile("config/qianggou.xml");
						}
					}
					else if ("-addgumutime" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string otherRoleName = cmdFields[1];
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int timeMinute = Global.SafeConvertToInt32(cmdFields[2]);
								Global.AddGuMuMapTime(otherClient, 0, timeMinute * 60);
							}
							else
							{
								strinfo = string.Format("增减古墓buffer时间请输入：-addgumutime 角色名称 值(正负，单位分钟)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addlimitlogin" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string otherRoleName = cmdFields[1];
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								Global.UpdateLimitTimeLoginNum(otherClient);
								HuodongData huodongData = client.ClientData.MyHuodongData;
								huodongData.LimitTimeLoginNum++;
								Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, otherClient);
								GameManager.ClientMgr.NotifyHuodongData(client);
							}
							else
							{
								strinfo = string.Format("增加限制时间累计登录次数请输入：-addlimitlogin 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-runnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int maxNum = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterZoneManager.MaxRunQueueNum = Global.GMax(1, maxNum);
								strinfo = string.Format(string.Format("副本单次循环的最大限制设置为：{0}", MonsterZoneManager.MaxRunQueueNum), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("副本单次循环的最大限制设置请输入：-runnum 次数(最大500)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-waitnum" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int maxNum = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterZoneManager.MaxWaitingRunQueueNum = Global.GMax(1, maxNum);
								strinfo = string.Format(string.Format("副本队列的等待数量设置为：{0}", MonsterZoneManager.MaxRunQueueNum), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("副本队列的等待数量设置请输入：-waitnum 次数(最大500)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-sklevel" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int minLevel = Global.SafeConvertToInt32(cmdFields[1]);
								MonsterManager.MinSeekRangeMonsterLevel = Global.GMax(0, minLevel);
								strinfo = string.Format(string.Format("怪物的自动寻敌和自动走动的最低级别设置为：{0}", MonsterManager.MinSeekRangeMonsterLevel), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("怪物的自动寻敌和自动走动的最低级别设置请输入：-sklevel 级别", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-st9grid" == cmdFields[0])
					{
						if (cmdFields.Length == 5)
						{
							GameManager.MaxSlotOnUpdate9GridsTicks = Math.Max(500, Global.SafeConvertToInt32(cmdFields[1]));
							GameManager.MaxSleepOnDoMapGridMoveTicks = Math.Max(5, Global.SafeConvertToInt32(cmdFields[2]));
							GameManager.MaxCachingMonsterToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[3]));
							GameManager.MaxCachingClientToClientBytesDataTicks = Math.Max(0, Global.SafeConvertToInt32(cmdFields[4]));
							strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}", new object[]
							{
								GameManager.MaxSlotOnUpdate9GridsTicks,
								GameManager.MaxSleepOnDoMapGridMoveTicks,
								GameManager.MaxCachingMonsterToClientBytesDataTicks,
								GameManager.MaxCachingClientToClientBytesDataTicks
							}), new object[0]);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
							if (!transmit && null != client)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (!transmit && null != client)
						{
							strinfo = string.Format("九宫格相关信息设置：-st9grid 九宫更新间隔毫秒 间歇休眠毫秒 怪物缓存毫秒 角色缓存毫秒", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-st9gridinfo" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 1)
							{
								strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新间隔毫秒={0}, 间歇休眠毫秒={1}, 怪物缓存毫秒={2}, 角色缓存毫秒={3}", new object[]
								{
									GameManager.MaxSlotOnUpdate9GridsTicks,
									GameManager.MaxSleepOnDoMapGridMoveTicks,
									GameManager.MaxCachingMonsterToClientBytesDataTicks,
									GameManager.MaxCachingClientToClientBytesDataTicks
								}), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("九宫格相关信息显示：-st9gridinfo", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-st9gridmode" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								GameManager.Update9GridUsingPosition = Global.SafeConvertToInt32(cmdFields[1]);
								GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);
								strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}", GameManager.Update9GridUsingPosition, GameManager.MaxSlotOnPositionUpdate9GridsTicks), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("九宫格相关信息设置：-st9gridmode 九宫更新模式 位置更新九宫格时间间隔", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-st9gridnew" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								GameManager.MaxSlotOnPositionUpdate9GridsTicks = Global.SafeConvertToInt32(cmdFields[2]);
								strinfo = string.Format(string.Format("九宫格相关信息设置：九宫更新模式={0}, 位置更新九宫格时间间隔={1}", 0, GameManager.MaxSlotOnPositionUpdate9GridsTicks), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("九宫格相关信息设置：-st9gridnew 九宫更新模式 位置更新九宫格时间间隔", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-stzipsize" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);
								strinfo = string.Format(string.Format("压缩的zip大小：最小压缩={0}", DataHelper.MinZipBytesSize), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("压缩的zip大小设置：-stzipsize 最小压缩", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							DataHelper.MinZipBytesSize = Global.SafeConvertToInt32(cmdFields[1]);
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为设置Zip压缩最小值为{0}", DataHelper.MinZipBytesSize), null, true);
						}
					}
					else if ("-stroledatamini" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								GameManager.RoleDataMiniMode = Global.SafeConvertToInt32(cmdFields[1]);
								strinfo = string.Format(string.Format("设置roleDataMini模式：模式={0}", GameManager.RoleDataMiniMode), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("设置roleDataMini模式：-stroledatamini 模式(0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-stkaifuawardhour" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								HuodongCachingMgr.ProcessKaiFuGiftAwardHour = Global.SafeConvertToInt32(cmdFields[1]);
								strinfo = string.Format(string.Format("设置开服在线奖励的小时：hour={0}", HuodongCachingMgr.ProcessKaiFuGiftAwardHour), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("设置开服在线奖励的小时：-stkaifuawardhour 小时(0~23)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addonlinesecs" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string otherRoleName = cmdFields[1];
								int secs = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								secs *= 60;
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int oldTotalOnlineHours = client.ClientData.TotalOnlineSecs / 3600;
								otherClient.ClientData.TotalOnlineSecs += secs;
								int newTotalOnlineHours = client.ClientData.TotalOnlineSecs / 3600;
								if (oldTotalOnlineHours != newTotalOnlineHours)
								{
									HuodongCachingMgr.ProcessKaiFuGiftAward(client);
								}
								strinfo = string.Format("给{0}增加在线时长到：{1}分钟", otherRoleName, otherClient.ClientData.TotalOnlineSecs / 60);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("增加在线时长：-stkaifuawardhour 角色名称 分钟", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-banchat2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string otherRoleName = cmdFields[1];
								int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, true);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									otherClient.ClientData.BanChat = banChatVal;
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 1, banChatVal), null, 0);
								strinfo = string.Format("将{0}永久禁言设置为：{1}", otherRoleName, banChatVal);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("永久禁言：-banchat2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != otherClient)
							{
								otherClient.ClientData.BanChat = banChatVal;
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 1, banChatVal), null, 0);
						}
					}
					else if ("-banchat2_rid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string otherRoleName = cmdFields[1] + "$rid";
								int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null != otherClient)
								{
									otherClient.ClientData.BanChat = banChatVal;
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 1, banChatVal), null, 0);
								strinfo = string.Format("将{0}永久禁言设置为：{1}", otherRoleName, banChatVal);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("永久禁言：-banchat2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							int banChatVal = Math.Max(0, Global.SafeConvertToInt32(cmdFields[2]));
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != otherClient)
							{
								otherClient.ClientData.BanChat = banChatVal;
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 1, banChatVal), null, 0);
						}
					}
					else if ("-warn" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							return false;
						}
						string userID = cmdFields[1];
						int warnType = int.Parse(cmdFields[2]);
						WarnManager.WarnProcess(userID, warnType);
					}
					else if ("-ban2" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -ban2 角色名称 (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int banValue = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, true);
								if (-1 != roleID)
								{
									if (banValue > 0)
									{
										GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
										if (null != otherClient)
										{
											otherClient.CheckCheatData.IsKickedRole = true;
											LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName), null, true);
											Global.ForceCloseClient(otherClient, "被GM踢了", true);
										}
										else
										{
											string gmCmdData = string.Format("-kick {0}", cmdFields[1]);
											GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
											{
												roleID,
												"",
												0,
												"",
												0,
												gmCmdData,
												0,
												0,
												GameManager.ServerLineID
											}), null, 0);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 2, banValue), null, 0);
								strinfo = string.Format("将{0}永久禁止登陆设置为：{1}", otherRoleName, banValue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							int banValue = Global.SafeConvertToInt32(cmdFields[2]);
							if (-1 != roleID)
							{
								if (banValue > 0)
								{
									GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
									if (null != otherClient)
									{
										if (!otherClient.CheckCheatData.IsKickedRole)
										{
											otherClient.CheckCheatData.IsKickedRole = true;
											RoleData roleData = new RoleData
											{
												RoleID = -70
											};
											otherClient.sendCmd<RoleData>(104, roleData, false);
										}
										else
										{
											Global.ForceCloseClient(otherClient, "被禁止登陆", true);
										}
									}
									else
									{
										LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(roleID);
									}
								}
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 2, banValue), null, 0);
						}
					}
					else if ("-ban2_rid" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -ban2_rid 角色ID (0/1)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1] + "$rid";
								int banValue = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								if (-1 != roleID)
								{
									if (banValue > 0)
									{
										GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
										if (null != otherClient)
										{
											otherClient.CheckCheatData.IsKickedRole = true;
											LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求将{0}踢出了服务器，并禁止从任何线路再登陆", otherRoleName), null, true);
											Global.ForceCloseClient(otherClient, "被GM踢了", true);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 2, banValue), null, 0);
								strinfo = string.Format("将{0}永久禁止登陆设置为：{1}", otherRoleName, banValue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
						else if (cmdFields.Length >= 3)
						{
							int roleID = Global.SafeConvertToInt32(cmdFields[1]);
							int banValue = Global.SafeConvertToInt32(cmdFields[2]);
							if (-1 != roleID)
							{
								if (banValue > 0)
								{
									GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
									if (null != otherClient)
									{
										if (!otherClient.CheckCheatData.IsKickedRole)
										{
											otherClient.CheckCheatData.IsKickedRole = true;
											RoleData roleData = new RoleData
											{
												RoleID = -70
											};
											otherClient.sendCmd<RoleData>(104, roleData, false);
										}
										else
										{
											Global.ForceCloseClient(otherClient, "被禁止登陆", true);
										}
									}
									else
									{
										LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(roleID);
									}
								}
							}
							GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 2, banValue), null, 0);
						}
					}
					else if ("-ban3" == cmdFields[0])
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -ban3 角色名 类型(0/1/2) 封号分钟数", new object[0]);
						}
						else
						{
							string roleName = cmdFields[1];
							int banType = Global.SafeConvertToInt32(cmdFields[2]);
							int banValue = Global.SafeConvertToInt32(cmdFields[3]);
							BanManager.BanRoleName(roleName, banValue, banType);
							strinfo = string.Format("根据GM要求设置角色{0}封号状态{1},时长{2}", roleName, banType, banValue);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, strinfo, null, true);
						}
					}
					else if ("-ban4" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length >= 3)
							{
								int roleID = Global.SafeConvertToInt32(cmdFields[1]);
								int banValue = Global.SafeConvertToInt32(cmdFields[2]);
								if (-1 != roleID)
								{
									if (banValue > 0)
									{
										GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
										if (null != otherClient)
										{
											if (!otherClient.CheckCheatData.IsKickedRole)
											{
												otherClient.CheckCheatData.IsKickedRole = true;
											}
											Global.ForceCloseClient(otherClient, "被禁止登陆", true);
										}
										else
										{
											LiXianBaiTanManager.RemoveLiXianSaleGoodsItems(roleID);
										}
									}
								}
								GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", roleID, 2, banValue), null, 0);
							}
						}
					}
					else if ("-bantrade" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int roleId = Global.SafeConvertToInt32(cmdFields[1]);
							int sec = Global.SafeConvertToInt32(cmdFields[2]);
							long toTicks = 0L;
							if (sec > 0)
							{
								toTicks = TimeUtil.NowDateTime().AddSeconds((double)sec).Ticks;
							}
							LogManager.WriteLog(LogTypes.Error, string.Format("GM封禁交易, roleid={0}, sec={1}", roleId, sec), null, true);
							SingletonTemplate<TradeBlackManager>.Instance().SetBanTradeToTicks(roleId, toTicks);
						}
					}
					else if ("-getmapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -getmapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int mapCode = Global.SafeConvertToInt32(cmdFields[1]);
								int lifeVZeroNum = 0;
								int NoAliveNum = 0;
								int totalMonsterNum = 0;
								List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
								if (null != objList)
								{
									totalMonsterNum = objList.Count;
									for (int i = 0; i < objList.Count; i++)
									{
										if ((objList[i] as Monster).VLife <= 0.0)
										{
											lifeVZeroNum++;
										}
										if (!(objList[i] as Monster).Alive)
										{
											NoAliveNum++;
										}
									}
								}
								strinfo = string.Format("本地图的怪物信息，totalMonsterNum={0}，lifeVZeroNum={1}，NoAliveNum={2}", totalMonsterNum, lifeVZeroNum, NoAliveNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-setmapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -setmapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int mapCode = Global.SafeConvertToInt32(cmdFields[1]);
								int totalMonsterNum = 0;
								List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
								if (null != objList)
								{
									totalMonsterNum = objList.Count;
									for (int i = 0; i < objList.Count; i++)
									{
										if ((objList[i] as Monster).VLife <= 0.0)
										{
											if ((objList[i] as Monster).Alive)
											{
												(objList[i] as Monster).VLife = 0.0;
											}
										}
									}
								}
								strinfo = string.Format("矫正本地图的怪物复活信息状态，totalMonsterNum={0}", totalMonsterNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-forcemapalivemon" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -forcemapalivemon 地图编号", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int mapCode = Global.SafeConvertToInt32(cmdFields[1]);
								int totalMonsterNum = 0;
								List<object> objList = GameManager.MonsterMgr.GetObjectsByMap(mapCode);
								if (null != objList)
								{
									totalMonsterNum = objList.Count;
									for (int i = 0; i < objList.Count; i++)
									{
										GameManager.MonsterMgr.AddDelayDeadMonster(objList[i] as Monster);
									}
								}
								strinfo = string.Format("强制本地图的怪物重新死亡，totalMonsterNum={0}", totalMonsterNum);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addactivevalue" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -adddj 角色名称 活跃值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int activevalue = this.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int j = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
								client.ClientData.DailyActiveValues += activevalue;
								if (activevalue >= 0)
								{
									DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
									if (client.ClientData.DailyActiveValues >= 100)
									{
										WebOldPlayerManager.getInstance().ChouJiangAddCheck(client.ClientData.RoleID, 1);
									}
								}
								else
								{
									DailyActiveManager.ModifyDailyActiveInfor(client, (uint)client.ClientData.DailyActiveValues, DailyActiveDataField1.DailyActiveValue, true);
								}
								j = (int)DailyActiveManager.GetDailyActiveDataByField(client, DailyActiveDataField1.DailyActiveValue);
								DailyActiveManager.NotifyClientDailyActiveData(client, -1, false);
								strinfo = string.Format("为{0}添加了活跃{1}", otherRoleName, activevalue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addsw" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int nValue = this.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameManager.ClientMgr.ModifyShengWangValue(otherClient, nValue, "GM指令增加声望", true, true);
								strinfo = string.Format("为{0}添加了声望{1}", otherRoleName, nValue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-pkking" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -pkking 角色名称", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								string[] dbFields = Global.ExecuteDBCmd(195, string.Format("{0}:{1}:0", client.ClientData.RoleID, otherRoleName), 0);
								if (dbFields == null || dbFields.Length < 5)
								{
									strinfo = string.Format("获取{0}的角色ID时连接数据库失败", otherRoleName);
								}
								else
								{
									int otherRoleID = Global.SafeConvertToInt32(dbFields[3]);
									GameManager.ArenaBattleMgr.ClearDbKingNpc();
									GameManager.ArenaBattleMgr.SetPKKingRoleID(otherRoleID);
									GameManager.ArenaBattleMgr.ReShowPKKing();
									if (client.ClientData.RoleID == otherRoleID)
									{
										Global.UpdateBufferData(client, BufferItemTypes.PKKingBuffer, new double[]
										{
											85200.0,
											2000800.0
										}, 0, true);
									}
									strinfo = string.Format("{0}已经设置为当前的PK之王", otherRoleName);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-testdata" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -testdata 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string result2 = "";
								int funcID = Global.SafeConvertToInt32(cmdFields[1]);
								if (funcID == 0)
								{
									ExtPropIndexes attribIndex = (ExtPropIndexes)Global.SafeConvertToInt32(cmdFields[2]);
									double attribValue = Convert.ToDouble(cmdFields[3]);
									if (ExtPropIndexes.SubAttackInjurePercent == attribIndex)
									{
										attribValue /= 100.0;
									}
									client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
									{
										PropsSystemTypes.GM_Temp_Props,
										(int)attribIndex,
										attribValue
									});
									GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
									GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								}
								else if (funcID == 1)
								{
									int fallNumber = 0;
									int fallNothing = 0;
									for (int i = 0; i < 10000; i++)
									{
										int fallID = Global.SafeConvertToInt32(cmdFields[2]);
										int count = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataListLimitTime = GameManager.GoodsPackMgr.GetGoodsDataList(client, fallID, count, 0, 1.0);
										if (goodsDataListLimitTime != null && goodsDataListLimitTime.Count > 0)
										{
											fallNumber += goodsDataListLimitTime.Count;
										}
										else
										{
											fallNothing++;
										}
									}
									result2 = string.Format("总计：{0}，轮空：{1}", fallNumber, fallNothing);
								}
								else if (funcID == 2)
								{
									double AngelTempleMonsterUpgradePercent = Global.SafeConvertToDouble(cmdFields[2]);
									Global.UpdateDBGameConfigg("AngelTempleMonsterUpgradeNumber", AngelTempleMonsterUpgradePercent.ToString("0.00"));
									result2 = string.Format("将天使神殿Boss成长比例设置为{0}", AngelTempleMonsterUpgradePercent);
								}
								else if (funcID == 3)
								{
									LuoLanChengZhanManager.getInstance().GMSetLuoLanChengZhu(Global.SafeConvertToInt32(cmdFields[2]));
								}
								else if (funcID == 4)
								{
									GameManager.FlagAlowUnRegistedCmd = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (funcID == 5)
								{
									GameManager.StatisticsMode = Global.SafeConvertToInt32(cmdFields[2]);
								}
								else if (funcID == 6)
								{
									if (cmdFields.Length >= 5)
									{
										sbyte p = sbyte.Parse(cmdFields[2]);
										sbyte p2 = sbyte.Parse(cmdFields[3]);
										int p3 = Global.SafeConvertToInt32(cmdFields[4]);
										HolyItemManager.getInstance().GetHolyItemPart(client, p, p2, p3);
									}
								}
								else if (funcID == 7)
								{
									GameManager.FlaDisablegFilterMonsterDeadEvent = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (funcID == 8)
								{
									GameManager.FlagKuaFuServerExplicit = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
									GameManager.IsKuaFuServer = (GameManager.FlagKuaFuServerExplicit && GameManager.ServerLineID > 1);
								}
								else if (funcID == 9)
								{
									client.CheckCheatData.DisableAutoKuaFu = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (funcID == 10)
								{
									if (cmdFields.Length >= 3)
									{
										int npcID = Global.SafeConvertToInt32(cmdFields[2]);
										string junQiName = JunQiManager.GetJunQiNameByBHID(client.ClientData.Faction);
										int junQiLevel = JunQiManager.GetJunQiLevelByBHID(client.ClientData.Faction);
										JunQiManager.ProcessNewJunQi(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.MapCode, client.ClientData.Faction, client.ClientData.ZoneID, client.ClientData.BHName, npcID, junQiName, junQiLevel, SceneUIClasses.Normal);
									}
								}
								else if (funcID == 11)
								{
									client.ClientData.WaitingNotifyChangeMap = !client.ClientData.WaitingNotifyChangeMap;
									result2 = client.ClientData.WaitingNotifyChangeMap.ToString();
								}
								else if (funcID == 12)
								{
									Global.UpdateRoleParamByName(client, "ChengJiuData", cmdFields[2], true);
									result2 = cmdFields[2];
								}
								else if (funcID == 13)
								{
									Dictionary<int, int> cDict = new Dictionary<int, int>();
									int fallNumber = 0;
									int fallNothing = 0;
									for (int i = 0; i < 10000; i++)
									{
										int fallID = Global.SafeConvertToInt32(cmdFields[2]);
										int count = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataListLimitTime = GameManager.GoodsPackMgr.GetGoodsDataList(client, fallID, count, 0, 1.0);
										if (goodsDataListLimitTime != null && goodsDataListLimitTime.Count > 0)
										{
											fallNumber += goodsDataListLimitTime.Count;
										}
										else
										{
											fallNothing++;
										}
									}
									result2 = string.Format("总计：{0}，轮空：{1}", fallNumber, fallNothing);
								}
								else if (funcID == 14)
								{
									if (client != null)
									{
										List<object> list = Global.GetAll9GridObjects(client);
										foreach (object k in list)
										{
											if (k is Monster)
											{
												GameManager.GoodsPackMgr.ProcessMonsterByClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, k as Monster);
												result2 = string.Format("执行怪物掉落：{0}", (k as Monster).MonsterName);
											}
										}
									}
								}
								else if (funcID == 99)
								{
									RobotTaskValidator.getInstance().UseWorkThread = (Global.SafeConvertToInt32(cmdFields[2]) > 0);
								}
								else if (funcID == 100)
								{
									if (cmdFields.Length >= 4)
									{
										switch (Global.SafeConvertToInt32(cmdFields[2]))
										{
										case 1:
											result2 = string.Format("设置测试开关,跳过SocketData调用: {0}", false);
											break;
										case 2:
											result2 = string.Format("设置测试开关,跳过AddBuff调用: {0}", false);
											break;
										case 3:
											result2 = string.Format("设置测试开关,跳过TrySend调用: {0}", false);
											break;
										case 4:
											result2 = string.Format("设置测试开关,跳过Socket发送: {0}", false);
											break;
										default:
											result2 = string.Format("请输入子功能号(1-4)和值(0,1)", new object[0]);
											break;
										}
									}
									else
									{
										result2 = string.Format("请输入子功能号(1-4)和值(0,1)", new object[0]);
									}
								}
								strinfo = string.Format("执行测试功能{0}{1}", funcID, result2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-WebOldPlayer" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							LogManager.WriteLog(LogTypes.Error, "网页老玩家召回GM指令错误", null, true);
						}
						WebOldPlayerManager.getInstance().WebOldPlayerCheck(Global.SafeConvertToInt32(cmdFields[1]), Global.SafeConvertToInt32(cmdFields[2]));
					}
					else if ("-tiantidata" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 9)
							{
								strinfo = string.Format("请输入： -tiantidata 角色名 duanWeiId, duanWeiJiFen, rongYao, monthDuanRank, lianSheng, successCount, fightCount", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string result2 = "";
								string otherRoleName = cmdFields[1];
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int duanWeiId = Global.SafeConvertToInt32(cmdFields[2]);
								int duanWeiJiFen = Global.SafeConvertToInt32(cmdFields[3]);
								int rongYao = Global.SafeConvertToInt32(cmdFields[4]);
								int monthDuanRank = Global.SafeConvertToInt32(cmdFields[5]);
								int lianSheng = Global.SafeConvertToInt32(cmdFields[6]);
								int successCount = Global.SafeConvertToInt32(cmdFields[7]);
								int fightCount = Global.SafeConvertToInt32(cmdFields[8]);
								TianTiManager.getInstance().GMSetRoleData(otherClient, duanWeiId, duanWeiJiFen, rongYao, monthDuanRank, lianSheng, successCount, fightCount);
								strinfo = string.Format("根据GM的要求为{0}设置天梯信息：{1},{2},{3},{4},{5},{6},{7}", new object[]
								{
									otherRoleName,
									duanWeiId,
									duanWeiJiFen,
									rongYao,
									monthDuanRank,
									lianSheng,
									successCount,
									fightCount
								});
								LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-check" == cmdFields[0])
					{
						if (cmdFields.Length < 4)
						{
							strinfo = string.Format("请输入： -check 0 600 5000 ", new object[0]);
						}
						else
						{
							CheckCheatTypes funcID2 = (CheckCheatTypes)Global.SafeConvertToInt32(cmdFields[1]);
							int value2 = Global.SafeConvertToInt32(cmdFields[2]);
							int value3 = Global.SafeConvertToInt32(cmdFields[3]);
							CheckCheatTypes checkCheatTypes = funcID2;
							switch (checkCheatTypes)
							{
							case CheckCheatTypes.MismatchMapCode:
								GameManager.CheckMismatchMapCode = (value2 > 0);
								break;
							case CheckCheatTypes.CheatPosition:
								GameManager.CheckCheatPosition = (value2 > 0);
								GameManager.CheckCheatMaxDistance = (double)Math.Max(value2, 600);
								GameManager.CheckPositionInterval = (double)Math.Max(value3, 1000);
								break;
							case CheckCheatTypes.DisableMovingOnAttack:
								GameManager.FlagDisableMovingOnAttack = (value2 > 0);
								break;
							default:
								switch (checkCheatTypes)
								{
								case CheckCheatTypes.CheckClientDataOne:
								{
									GameClient otherClient = GameManager.ClientMgr.FindClient(value2);
									if (otherClient != null && otherClient.CheckCheatData.RobotTaskListData.Length > 0)
									{
										string dbcmd = string.Format("{0}#{1}", otherClient.ClientData.RoleID, otherClient.CheckCheatData.RobotTaskListData);
										Global.ExecuteDBCmd(13111, dbcmd, otherClient.ServerId);
									}
									break;
								}
								case CheckCheatTypes.CheckClientData:
								{
									int index = 0;
									GameClient gc;
									while ((gc = GameManager.ClientMgr.GetNextClient(ref index, false)) != null)
									{
										string log = string.Format("IP:{0},Client:{1}({2}){3}", new object[]
										{
											Global.GetSocketRemoteEndPoint(gc.ClientSocket, false),
											gc.ClientData.RoleName,
											gc.ClientData.RoleID,
											gc.CheckCheatData.RobotTaskListData
										});
										LogManager.WriteException(log);
									}
									break;
								}
								}
								break;
							}
							strinfo = string.Format("根据GM要求设置外挂检查（-check） {0} 为 {1} {2}", funcID2, value2, value3);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-hideflags" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							strinfo = string.Format("请输入： -hideflags 1 3000", new object[0]);
						}
						else
						{
							int funcID = Global.SafeConvertToInt32(cmdFields[1]);
							int value4 = Global.SafeConvertToInt32(cmdFields[2]);
							int value3 = (cmdFields.Length >= 4) ? Global.SafeConvertToInt32(cmdFields[3]) : 0;
							int num7 = funcID;
							if (num7 == 1)
							{
								if (value3 > 0)
								{
									GameManager.HideFlagsMapDict.TryAdd(value4, value3);
								}
								else
								{
									GameManager.HideFlagsMapDict.TryRemove(value4, out value3);
								}
							}
							strinfo = string.Format("根据GM要求设置效果屏蔽选项（-hideflags） {0} 为 {1} {2}", funcID, value4, value3);
							LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
						}
						if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-showprops" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -showprops 角色名", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								strinfo = GlobalNew.PrintRoleProps(otherRoleName);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (strinfo == null)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								LogManager.WriteException(strinfo);
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									-2,
									"",
									0,
									"",
									0,
									strinfo,
									0
								});
								GameManager.ClientMgr.SendChatMessage(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strcmd);
							}
						}
						else if (cmdFields.Length >= 2)
						{
							string otherRoleName = cmdFields[1];
							strinfo = GlobalNew.PrintRoleProps(otherRoleName);
							int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
							if (strinfo != null)
							{
								LogManager.WriteException(strinfo);
							}
							return true;
						}
					}
					else if ("-showForce" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -showForce 角色名", new object[0]);
							LogManager.WriteLog(LogTypes.Error, strinfo, null, true);
						}
						else
						{
							string roleName = cmdFields[1];
							int roleID = RoleName2IDs.FindRoleIDByName(roleName, false);
							if (-1 == roleID)
							{
								strinfo = string.Format("{0}不在线", roleName);
								LogManager.WriteLog(LogTypes.Error, strinfo, null, true);
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
							if (null == otherClient)
							{
								strinfo = string.Format("{0}不在线", roleName);
								LogManager.WriteLog(LogTypes.Error, strinfo, null, true);
								return true;
							}
							StringBuilder sb = new StringBuilder();
							sb.Append(roleName).Append("\t角色属性及战斗力：\n");
							double nMinAttack = RoleAlgorithm.GetMinAttackV(otherClient);
							sb.Append("物攻Min：").Append(nMinAttack).Append("\n");
							double nMaxAttack = RoleAlgorithm.GetMaxAttackV(otherClient);
							sb.Append("物攻Max：").Append(nMaxAttack).Append("\n");
							double nMinDefense = RoleAlgorithm.GetMinADefenseV(otherClient);
							sb.Append("物防Min：").Append(nMinDefense).Append("\n");
							double nMaxDefense = RoleAlgorithm.GetMaxADefenseV(otherClient);
							sb.Append("物防Max：").Append(nMaxDefense).Append("\n");
							double nMinMAttack = RoleAlgorithm.GetMinMagicAttackV(otherClient);
							sb.Append("魔攻Min：").Append(nMinMAttack).Append("\n");
							double nMaxMAttack = RoleAlgorithm.GetMaxMagicAttackV(otherClient);
							sb.Append("魔攻Max：").Append(nMaxMAttack).Append("\n");
							double nMinMDefense = RoleAlgorithm.GetMinMDefenseV(otherClient);
							sb.Append("魔防Min：").Append(nMinMDefense).Append("\n");
							double nMaxMDefense = RoleAlgorithm.GetMaxMDefenseV(otherClient);
							sb.Append("魔防Min：").Append(nMaxMDefense).Append("\n");
							double nHit = RoleAlgorithm.GetHitV(otherClient);
							sb.Append("命中：").Append(nHit).Append("\n");
							double nDodge = RoleAlgorithm.GetDodgeV(otherClient);
							sb.Append("闪避：").Append(nDodge).Append("\n");
							double addAttackInjure = RoleAlgorithm.GetAddAttackInjureValue(otherClient);
							sb.Append("伤害加成：").Append(addAttackInjure).Append("\n");
							double decreaseInjure = RoleAlgorithm.GetDecreaseInjureValue(otherClient);
							sb.Append("伤害减少：").Append(decreaseInjure).Append("\n");
							double nMaxHP = RoleAlgorithm.GetMaxLifeV(otherClient);
							sb.Append("生命Max：").Append(nMaxHP).Append("\n");
							double nMaxMP = RoleAlgorithm.GetMaxMagicV(otherClient);
							sb.Append("魔法Max：").Append(nMaxMP).Append("\n");
							double nLifeSteal = RoleAlgorithm.GetLifeStealV(otherClient);
							sb.Append("击中生命恢复：").Append(nLifeSteal).Append("\n");
							double dFireAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Fire);
							sb.Append("火系伤害：").Append(dFireAttack).Append("\n");
							double dWaterAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Water);
							sb.Append("水系伤害：").Append(dWaterAttack).Append("\n");
							double dLightningAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Lightning);
							sb.Append("雷系伤害：").Append(dLightningAttack).Append("\n");
							double dSoilAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Soil);
							sb.Append("土系伤害：").Append(dSoilAttack).Append("\n");
							double dIceAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Ice);
							sb.Append("冰系伤害：").Append(dIceAttack).Append("\n");
							double dWindAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(otherClient, EElementDamageType.EEDT_Wind);
							sb.Append("风系伤害：").Append(dWindAttack).Append("\n");
							double HolyAttack = RoleAlgorithm.GetExtProp(otherClient, 122);
							sb.Append("HolyAttack：").Append(HolyAttack).Append("\n");
							double HolyDefense = RoleAlgorithm.GetExtProp(otherClient, 123);
							sb.Append("HolyDefense：").Append(HolyDefense).Append("\n");
							double ShadowAttack = RoleAlgorithm.GetExtProp(otherClient, 129);
							sb.Append("ShadowAttack：").Append(ShadowAttack).Append("\n");
							double ShadowDefense = RoleAlgorithm.GetExtProp(otherClient, 130);
							sb.Append("ShadowDefense：").Append(ShadowDefense).Append("\n");
							double NatureAttack = RoleAlgorithm.GetExtProp(otherClient, 136);
							sb.Append("NatureAttack：").Append(NatureAttack).Append("\n");
							double NatureDefense = RoleAlgorithm.GetExtProp(otherClient, 137);
							sb.Append("NatureDefense：").Append(NatureDefense).Append("\n");
							double ChaosAttack = RoleAlgorithm.GetExtProp(otherClient, 143);
							sb.Append("ChaosAttack：").Append(ChaosAttack).Append("\n");
							double ChaosDefense = RoleAlgorithm.GetExtProp(otherClient, 144);
							sb.Append("ChaosDefense：").Append(ChaosDefense).Append("\n");
							double IncubusAttack = RoleAlgorithm.GetExtProp(otherClient, 150);
							sb.Append("IncubusAttack：").Append(IncubusAttack).Append("\n");
							double IncubusDefense = RoleAlgorithm.GetExtProp(otherClient, 151);
							sb.Append("IncubusDefense：").Append(IncubusDefense).Append("\n");
							CombatForceInfo CombatForce = Data.CombatForceDataInfo[1];
							if (CombatForce != null)
							{
								double nValue2 = (nMinAttack / CombatForce.MinPhysicsAttackModulus + nMaxAttack / CombatForce.MaxPhysicsAttackModulus) / 2.0 + (nMinDefense / CombatForce.MinPhysicsDefenseModulus + nMaxDefense / CombatForce.MaxPhysicsDefenseModulus) / 2.0 + (nMinMAttack / CombatForce.MinMagicAttackModulus + nMaxMAttack / CombatForce.MaxMagicAttackModulus) / 2.0 + (nMinMDefense / CombatForce.MinMagicDefenseModulus + nMaxMDefense / CombatForce.MaxMagicDefenseModulus) / 2.0 + addAttackInjure / CombatForce.AddAttackInjureModulus + decreaseInjure / CombatForce.DecreaseInjureModulus + nHit / CombatForce.HitValueModulus + nDodge / CombatForce.DodgeModulus + nMaxHP / CombatForce.MaxHPModulus + nMaxMP / CombatForce.MaxMPModulus + nLifeSteal / CombatForce.LifeStealModulus;
								nValue2 += dFireAttack / CombatForce.FireAttack + dWaterAttack / CombatForce.WaterAttack + dLightningAttack / CombatForce.LightningAttack + dSoilAttack / CombatForce.SoilAttack + dIceAttack / CombatForce.IceAttack + dWindAttack / CombatForce.WindAttack;
								nValue2 += HolyAttack / CombatForce.HolyAttack + HolyDefense / CombatForce.HolyDefense + ShadowAttack / CombatForce.ShadowAttack + ShadowDefense / CombatForce.ShadowDefense + NatureAttack / CombatForce.NatureAttack + NatureDefense / CombatForce.NatureDefense + ChaosAttack / CombatForce.ChaosAttack + ChaosDefense / CombatForce.ChaosDefense + IncubusAttack / CombatForce.IncubusAttack + IncubusDefense / CombatForce.IncubusDefense;
								sb.Append("战斗力：").Append(nValue2).Append("\n");
							}
							sb.Append("重生战斗力：").Append(RebornManager.getInstance().CalculateCombatForce(otherClient)).Append("\n");
							LogManager.WriteLog(LogTypes.Error, sb.ToString(), null, true);
						}
					}
					else if ("-nameserver" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -nameserver 1或0(1启用 0禁用)} [名字服务器IP] [名字服务器端口] [本服务器的平台ID编号]\n中括号内的参数可不填,'*'号表示不修改", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string result2 = "";
								int funcID = Global.SafeConvertToInt32(cmdFields[1]);
								if (funcID == 0)
								{
									Global.Flag_NameServer = false;
									result2 = string.Format("禁用名字服务器,不通过验证就可创建", new object[0]);
								}
								else
								{
									Global.Flag_NameServer = true;
									if (cmdFields.Length >= 3 && cmdFields[2] != "*")
									{
										NameServerNamager.NameServerIP = cmdFields[2];
									}
									if (cmdFields.Length >= 4 && cmdFields[3] != "*")
									{
										NameServerNamager.NameServerPort = this.SafeConvertToInt32(cmdFields[3]);
									}
									if (cmdFields.Length >= 5 && cmdFields[4] != "*")
									{
										NameServerNamager.NameServerConfig = this.SafeConvertToInt32(cmdFields[4]);
									}
									result2 = string.Format("启用名字服务器，需通过名字认证服务器验证才可创建，\nIP：{0}    端口：{1}    配置选项：{2}", NameServerNamager.NameServerIP, NameServerNamager.NameServerPort, NameServerNamager.NameServerConfig);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, result2);
							}
						}
					}
					else if ("-testproc" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -testproc 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string result2 = "";
								int funcID = Global.SafeConvertToInt32(cmdFields[1]);
								if (funcID == 0)
								{
									GameManager.AngelTempleMgr.GiveAwardAngelTempleScene(cmdFields.Length >= 3 && cmdFields[2] == "1");
								}
								else if (funcID == 1)
								{
									int fallNumber = 0;
									int fallNothing = 0;
									for (int i = 0; i < 10000; i++)
									{
										int fallID = Global.SafeConvertToInt32(cmdFields[2]);
										int count = Global.SafeConvertToInt32(cmdFields[3]);
										List<GoodsData> goodsDataListLimitTime = GameManager.GoodsPackMgr.GetGoodsDataList(client, fallID, count, 0, 1.0);
										if (goodsDataListLimitTime != null && goodsDataListLimitTime.Count > 0)
										{
											fallNumber += goodsDataListLimitTime.Count;
										}
										else
										{
											fallNothing++;
										}
									}
									result2 = string.Format("总计：{0}，轮空：{1}", fallNumber, fallNothing);
								}
								else if (funcID == 2)
								{
									LuoLanChengZhanManager.getInstance().GMStartHuoDongNow();
								}
								else if (funcID == 3)
								{
									GameManager.AngelTempleMgr.GMSetHuoDongStartNow();
								}
								else if (funcID == 4)
								{
									if (cmdFields.Length >= 3)
									{
										TianTiManager.getInstance().GMStartHuoDongNow(Global.SafeConvertToInt32(cmdFields[2]));
									}
								}
								else if (funcID == 5)
								{
									if (cmdFields.Length >= 3)
									{
										SingletonTemplate<CreateRoleLimitManager>.Instance().CreateRoleLimitMinutes = Global.SafeConvertToInt32(cmdFields[2]);
									}
								}
								else if (funcID != 6)
								{
									if (funcID == 1000)
									{
										GMCommands.GMSetTime(client, cmdFields, false);
									}
								}
								strinfo = string.Format("执行测试功能{0}{1}", funcID, result2);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-testkf" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -testkf 功能号 {参数1|参数2|参数3...}", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string result2 = "";
								int funcID = Global.SafeConvertToInt32(cmdFields[1]);
								if (funcID == 1)
								{
									result2 = string.Format("总计：{0}，轮空：{1}", 0, 0);
								}
								strinfo = string.Format("执行测试功能{0}{1}", funcID, result2);
								LogManager.WriteLog(LogTypes.SQL, strinfo, null, true);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-settime" == cmdFields[0])
					{
						if (GMCommands.EnableGMSetAllServerTime)
						{
							LogManager.WriteLog(LogTypes.SQL, msgText, null, true);
							GMCommands.GMSetTime(client, cmdFields, true);
						}
						else
						{
							LogManager.WriteLog(LogTypes.SQL, "修改时间功能未开启", null, true);
						}
					}
					else if ("-hidefakerole" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -hidefakerole 1或0(1隐藏 0不隐藏)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								GameManager.TestGameShowFakeRoleForUser = (Global.SafeConvertToInt32(cmdFields[1]) == 0);
								strinfo = (GameManager.TestGameShowFakeRoleForUser ? "设置为显示(挂机)假人" : "设置为隐藏(挂机)假人");
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-addxh" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -adddj 角色名称 声望值(可以是负数)", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int nValue = this.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameManager.ClientMgr.ModifyStarSoulValue(client, nValue, "GM增加", true, true);
								strinfo = string.Format("为{0}添加了星魂{1}", otherRoleName, nValue);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-stminsendsize" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								SendBuffer.ConstMinSendSize = Global.SafeConvertToInt32(cmdFields[1]);
								strinfo = string.Format(string.Format("发送缓冲的大小：最小缓冲={0}", SendBuffer.ConstMinSendSize), new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = string.Format("发送缓冲的大小设置：-stminsendsize 最小缓冲", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-fazhenmen" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int OpenState = this.SafeConvertToInt32(cmdFields[1]);
								if (OpenState == 1 || OpenState == 0)
								{
									LuoLanFaZhenCopySceneManager.GM_SetOpenState(OpenState);
									if (OpenState == 1)
									{
										strinfo = "显示所有传送门的隐藏状态";
									}
									else if (OpenState == 0)
									{
										strinfo = "隐藏所有传送门的状态";
									}
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								}
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-showlingyu" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 1)
							{
								List<LingYuData> dataList = LingYuManager.GetLingYuList(client);
								strinfo = string.Format("翎羽数量：{0}", dataList.Count<LingYuData>());
								for (int i = 0; i < dataList.Count<LingYuData>(); i++)
								{
									strinfo += string.Format(" [Type：{0}, Level：{1}, Suit：{2}] ", dataList[i].Type, dataList[i].Level, dataList[i].Suit);
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-wingmax" == cmdFields[0])
					{
						if (!transmit)
						{
							LingYuManager.SetLingYuMax_GM(client);
							ZhuLingZhuHunManager.SetZhuLingMax_GM(client);
							ZhuLingZhuHunManager.SetZhuHunMax_GM(client);
							strinfo = string.Format("翎羽、注灵、注魂满级！", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-lingyulevelup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int type = this.SafeConvertToInt32(cmdFields[1]);
								int useZuanshiIfNoMaterial = this.SafeConvertToInt32(cmdFields[2]);
								LingYuError lyError = LingYuManager.AdvanceLingYuLevel(client, client.ClientData.RoleID, type, useZuanshiIfNoMaterial);
								strinfo = LingYuManager.Error2Str(lyError);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-lingyusuitup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								int type = this.SafeConvertToInt32(cmdFields[1]);
								int useZuanshiIfNoMaterial = this.SafeConvertToInt32(cmdFields[2]);
								LingYuError lyError = LingYuManager.AdvanceLingYuSuit(client, client.ClientData.RoleID, type, useZuanshiIfNoMaterial);
								strinfo = LingYuManager.Error2Str(lyError);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-showzhulingzhuhun" == cmdFields[0])
					{
						if (!transmit)
						{
							strinfo = string.Format("zhuling：{0}, zhuhun：{1}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-wingzhuling" == cmdFields[0])
					{
						if (!transmit)
						{
							ZhuLingZhuHunError e = ZhuLingZhuHunManager.ReqZhuLing(client);
							strinfo = string.Format("zhuling：{0}, zhuhun：{1}, error：{2}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum, e.ToString());
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-wingzhuhun" == cmdFields[0])
					{
						if (!transmit)
						{
							ZhuLingZhuHunError e = ZhuLingZhuHunManager.ReqZhuHun(client);
							strinfo = string.Format("zhuling：{0}, zhuhun：{1}, error：{2}", client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum, e.ToString());
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-setwingsuitstar" == cmdFields[0])
					{
						if (!transmit)
						{
							this.GMSetWingSuitStar(client, cmdFields);
						}
					}
					else if ("-achievement" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text = cmdFields[1];
								if (text != null)
								{
									if (!(text == "level"))
									{
										if (!(text == "runeLevel"))
										{
											if (!(text == "runeCount"))
											{
												if (text == "runeRate")
												{
													ChengJiuManager.SetAchievementRuneRate(client, int.Parse(cmdFields[2]));
												}
											}
											else
											{
												ChengJiuManager.SetAchievementRuneCount(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											ChengJiuManager.SetAchievementRuneLevel(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										ChengJiuManager.SetAchievementLevel(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-prestige" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text = cmdFields[1];
								if (text != null)
								{
									if (!(text == "level"))
									{
										if (!(text == "medalLevel"))
										{
											if (!(text == "medalCount"))
											{
												if (text == "medalRate")
												{
													PrestigeMedalManager.SetPrestigeMedalRate(client, int.Parse(cmdFields[2]));
												}
											}
											else
											{
												PrestigeMedalManager.SetPrestigeMedalCount(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											PrestigeMedalManager.SetPrestigeMedalLevel(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										PrestigeMedalManager.SetPrestigeLevel(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-deleterole" == cmdFields[0])
					{
						if (transmit)
						{
							if (cmdFields.Length == 4)
							{
								string userID = cmdFields[1];
								int roleID = Global.SafeConvertToInt32(cmdFields[2]);
								int zoneID = Global.SafeConvertToInt32(cmdFields[3]);
								TMSKSocket clientSocket = GameManager.OnlineUserSession.FindSocketByUserID(userID);
								if (null == clientSocket)
								{
									EventLogManager.AddRemoveRoleEvent(userID, zoneID, roleID, "");
									return true;
								}
								if (clientSocket.session.SocketState < 4)
								{
									string strcmd = string.Format("{0}", roleID);
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 103);
									Global._TCPManager.MySocketListener.SendData(clientSocket, tcpOutPacket, true);
								}
								string ip = Global.GetSocketRemoteEndPoint(clientSocket, false).Replace(":", ".");
								EventLogManager.AddRemoveRoleEvent(userID, zoneID, roleID, ip);
							}
						}
					}
					else if ("-artifact" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text = cmdFields[1];
								if (text != null)
								{
									if (text == "failCount")
									{
										ArtifactManager.SetArtifactFailCount(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-die" == cmdFields[0])
					{
						if (!transmit)
						{
							client.ClientData.CurrentLifeV = 0;
							Global.RemoveBufferData(client, 86);
							Global.RemoveBufferData(client, 85);
							client.ClientData.LastRoleDeadTicks = TimeUtil.NOW();
							GameManager.SystemServerEvents.AddEvent(string.Format("角色强制死亡, roleID={0}({1}), Life={2}", client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.CurrentLifeV), EventLevels.Debug);
							GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, -1, client.ClientData.RoleID, 0, 0, 0.0, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					else if ("-mailAddId" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 9)
							{
								strinfo = string.Format("请输入： -mailAddId 角色名称 物品ID 个数(1~2147483647) 绑定(0/1) 级别(0~10) 追加 幸运 卓越", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int goodsID = this.SafeConvertToInt32(cmdFields[2]);
								int gcount = this.SafeConvertToInt32(cmdFields[3]);
								gcount = Global.GMax(0, gcount);
								gcount = Global.GMin(int.MaxValue, gcount);
								int binding = this.SafeConvertToInt32(cmdFields[4]);
								int level = this.SafeConvertToInt32(cmdFields[5]);
								int appendprop = this.SafeConvertToInt32(cmdFields[6]);
								int lucky = this.SafeConvertToInt32(cmdFields[7]);
								int excellenceinfo = this.SafeConvertToInt32(cmdFields[8]);
								level = Global.GMax(0, level);
								level = Global.GMin(15, level);
								int quality = 0;
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								SystemXmlItem systemGoods = null;
								if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemGoods))
								{
									strinfo = string.Format("系统中不存在{0}", goodsID);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								int site = 0;
								int categoriy = systemGoods.GetIntValue("Categoriy", -1);
								if (categoriy >= 800 && categoriy < 816)
								{
									site = 3000;
								}
								GoodsData goodsData = new GoodsData
								{
									Id = -1,
									GoodsID = goodsID,
									Using = 0,
									Forge_level = level,
									Starttime = "1900-01-01 12:00:00",
									Endtime = "1900-01-01 12:00:00",
									Site = 0,
									Quality = 0,
									Props = "",
									GCount = 1,
									Binding = binding,
									Jewellist = "",
									BagIndex = 0,
									AddPropIndex = 0,
									BornIndex = 0,
									Lucky = lucky,
									Strong = 0,
									ExcellenceInfo = excellenceinfo,
									AppendPropLev = appendprop,
									ChangeLifeLevForEquip = 0
								};
								Global.UseMailGivePlayerAward(client, goodsData, "GM邮件", "GM邮件奖励", 1.0);
								strinfo = string.Format("为{0}添加了物品{1}, 个数{2}, 级别{3}, 品质{4}, 绑定{5}", new object[]
								{
									otherRoleName,
									goodsID.ToString(),
									gcount,
									level,
									quality,
									binding
								});
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-rmgood" == cmdFields[0])
					{
						if (cmdFields.Length < 3)
						{
							string tip = "-rmgoods参数错误，Usage：-rmgoods roleid goodid";
							LogManager.WriteLog(LogTypes.Error, tip, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
						else
						{
							int roleID = this.SafeConvertToInt32(cmdFields[1]);
							int goodsDbID = this.SafeConvertToInt32(cmdFields[2]);
							GameClient otherClient = (roleID != -1) ? GameManager.ClientMgr.FindClient(roleID) : null;
							if (null == otherClient)
							{
								RoleDataEx dbRd = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, roleID), 0);
								GoodsData goodsData = null;
								if (null != dbRd)
								{
									if (null == dbRd.GoodsDataList)
									{
										LogManager.WriteLog(LogTypes.Error, "删除角色道具，但是查不到角色道具数据。", null, true);
									}
									else
									{
										lock (dbRd.GoodsDataList)
										{
											for (int i = 0; i < dbRd.GoodsDataList.Count; i++)
											{
												if (dbRd.GoodsDataList[i].Id == goodsDbID)
												{
													goodsData = dbRd.GoodsDataList[i];
													break;
												}
											}
										}
										if (null == goodsData)
										{
											return true;
										}
										int gcount = goodsData.GCount;
										string[] dbFields = null;
										string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
										{
											roleID,
											goodsDbID,
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											0,
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*",
											"*"
										});
										TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strcmd, out dbFields, 0);
										if (dbFields == null || dbFields.Length < 2 || Convert.ToInt32(dbFields[1]) < 0)
										{
											LogManager.WriteLog(LogTypes.Error, "删除角色道具，但是角色离线，所以转到db处理，" + strcmd + ", 但是db处理失败", null, true);
										}
										else
										{
											goodsData.GCount = 0;
											Global.ModRoleGoodsEvent(dbRd, goodsData, -gcount, "客户端修改", false);
											EventLogManager.AddGoodsEvent(dbRd, OpTypes.Forge, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, 0, goodsData.GCount, "客户端修改");
										}
									}
								}
								else
								{
									LogManager.WriteLog(LogTypes.Error, "删除角色道具，但是查不到角色数据。", null, true);
								}
							}
							else
							{
								GoodsData goodsData = Global.GetGoodsByDbID(otherClient, goodsDbID);
								if (null != goodsData)
								{
									string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										otherClient.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									int _gccount = goodsData.GCount;
									if (TCPProcessCmdResults.RESULT_OK != Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
									{
										LogManager.WriteLog(LogTypes.Error, string.Concat(new string[]
										{
											" -rmgoods 失败:",
											cmdFields[0],
											" ",
											cmdFields[1],
											" ",
											cmdFields[2]
										}), null, true);
									}
								}
							}
						}
					}
					else if ("-subRoleHuobi" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length < 4)
						{
							string tip = "-subRoleHuobi参数错误，Usage：-subRoleHuobi roleid GMHuoBiType value";
							LogManager.WriteLog(LogTypes.Error, tip, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
						else
						{
							int roleID = this.SafeConvertToInt32(cmdFields[1]);
							int huobiIndex = Global.SafeConvertToInt32(cmdFields[2]);
							int modifyValue = -this.SafeConvertToInt32(cmdFields[3]);
							GameClient otherClient = (roleID != -1) ? GameManager.ClientMgr.FindClient(roleID) : null;
							bool subRes = true;
							if (null == otherClient)
							{
								string cmd = string.Format("{0}:{1}:{2}", roleID, huobiIndex, modifyValue);
								LogManager.WriteLog(LogTypes.Error, "修改角色货币，但是角色离线，所以转到db处理，" + cmd, null, true);
								string[] dbFields = Global.ExecuteDBCmd(20398, cmd, 0);
								if (dbFields == null || dbFields.Length < 2 || Convert.ToInt32(dbFields[1]) < 0)
								{
									LogManager.WriteLog(LogTypes.Error, "修改角色货币，但是角色离线，所以转到db处理，" + cmd + ", 但是db处理失败", null, true);
									subRes = false;
								}
							}
							else
							{
								int num7 = huobiIndex;
								if (num7 <= 40)
								{
									if (num7 <= 8)
									{
										if (num7 == 1)
										{
											subRes = GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, modifyValue, "GM指令-绑金", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色绑金, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												otherClient.ClientData.Money1
											}), EventLevels.Record);
											goto IL_1784E;
										}
										if (num7 == 8)
										{
											subRes = GameManager.ClientMgr.SubUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, -modifyValue, "GM指令-金币", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色金币, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												otherClient.ClientData.YinLiang
											}), EventLevels.Record);
											goto IL_1784E;
										}
									}
									else
									{
										switch (num7)
										{
										case 13:
											subRes = GameManager.ClientMgr.ModifyTianDiJingYuanValue(otherClient, modifyValue, "GM指令-魔晶", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魔晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												GameManager.ClientMgr.GetTianDiJingYuanValue(otherClient)
											}), EventLevels.Record);
											goto IL_1784E;
										case 14:
											break;
										case 15:
											subRes = Global.AddZaJinDanJiFen(otherClient, modifyValue, "GM指令-祈福积分", true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色再造点, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(client, "ZJDJiFen")
											}), EventLevels.Record);
											goto IL_1784E;
										default:
											if (num7 == 40)
											{
												subRes = GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, -modifyValue, "GM指令-钻石", false, false, true, DaiBiSySType.None);
												GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色钻石, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
												{
													otherClient.ClientData.RoleID,
													otherClient.ClientData.RoleName,
													modifyValue,
													otherClient.ClientData.UserMoney
												}), EventLevels.Record);
												goto IL_1784E;
											}
											break;
										}
									}
								}
								else if (num7 <= 101)
								{
									if (num7 == 50)
									{
										subRes = GameManager.ClientMgr.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, otherClient, -modifyValue, "GM指令-绑钻", true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色绑钻, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											otherClient.ClientData.RoleID,
											otherClient.ClientData.RoleName,
											modifyValue,
											otherClient.ClientData.Gold
										}), EventLevels.Record);
										goto IL_1784E;
									}
									if (num7 == 101)
									{
										subRes = GameManager.ClientMgr.ModifyZaiZaoValue(otherClient, modifyValue, "GM指令-再造点", true, true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色再造点, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											otherClient.ClientData.RoleID,
											otherClient.ClientData.RoleName,
											modifyValue,
											GameManager.ClientMgr.GetZaiZaoValue(otherClient)
										}), EventLevels.Record);
										goto IL_1784E;
									}
								}
								else
								{
									switch (num7)
									{
									case 106:
										subRes = GameManager.ClientMgr.ModifyMUMoHeValue(otherClient, modifyValue, "GM指令-灵晶", true, true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											otherClient.ClientData.RoleID,
											otherClient.ClientData.RoleName,
											modifyValue,
											GameManager.ClientMgr.GetMUMoHeValue(otherClient)
										}), EventLevels.Record);
										goto IL_1784E;
									case 107:
										subRes = GameManager.ClientMgr.ModifyYuanSuFenMoValue(otherClient, modifyValue, "GM指令-元素粉末", true, true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色元素粉末, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											otherClient.ClientData.RoleID,
											otherClient.ClientData.RoleName,
											modifyValue,
											Global.GetRoleParamsInt32FromDB(otherClient, "ElementPowder")
										}), EventLevels.Record);
										goto IL_1784E;
									case 108:
										break;
									case 109:
										subRes = GameManager.FluorescentGemMgr.DecFluorescentPoint(otherClient, -modifyValue, "GM指令-荧光粉末", true);
										GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色荧光粉末, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
										{
											otherClient.ClientData.RoleID,
											otherClient.ClientData.RoleName,
											modifyValue,
											otherClient.ClientData.FluorescentPoint
										}), EventLevels.Record);
										goto IL_1784E;
									default:
										if (num7 == 119)
										{
											subRes = GameManager.ClientMgr.ModifyOrnamentCharmPointValue(otherClient, modifyValue, "GM指令-魅力点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魅力点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10153")
											}), EventLevels.Record);
											goto IL_1784E;
										}
										switch (num7)
										{
										case 129:
											subRes = GameManager.ClientMgr.ModifyFuWenZhiChenPointsValue(otherClient, modifyValue, "GM指令-符文之尘点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色符文之尘点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10187")
											}), EventLevels.Record);
											goto IL_1784E;
										case 130:
											subRes = GameManager.ClientMgr.ModifyAlchemyElementValue(otherClient, modifyValue, "GM指令-炼金元素", true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色炼金元素, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												otherClient.ClientData.AlchemyInfo.BaseData.Element
											}), EventLevels.Record);
											goto IL_1784E;
										case 131:
											subRes = GameManager.ClientMgr.ModifyJueXingZhiChenValue(otherClient, modifyValue, "GM指令-觉醒之尘", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色觉醒之尘, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10194")
											}), EventLevels.Record);
											goto IL_1784E;
										case 132:
											subRes = GameManager.ClientMgr.ModifyHunJingValue(otherClient, modifyValue, "GM指令-坐骑饲料", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色坐骑饲料, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10208")
											}), EventLevels.Record);
											goto IL_1784E;
										case 133:
											subRes = GameManager.ClientMgr.ModifyMountPointValue(otherClient, modifyValue, "GM指令-坐骑点数", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色坐骑点数, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10209")
											}), EventLevels.Record);
											goto IL_1784E;
										case 135:
											subRes = GameManager.ClientMgr.ModifyCompDonateValue(otherClient, modifyValue, "GM指令-势力贡献度", true, true, true);
											GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色势力贡献度, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
											{
												otherClient.ClientData.RoleID,
												otherClient.ClientData.RoleName,
												modifyValue,
												Global.GetRoleParamsInt32FromDB(otherClient, "10204")
											}), EventLevels.Record);
											goto IL_1784E;
										}
										break;
									}
								}
								LogManager.WriteLog(LogTypes.Error, " -modifyRoleHuobi 未注册的货币类型:" + huobiIndex, null, true);
								IL_1784E:;
							}
							GameManager.logDBCmdMgr.AddMessageLog(-1, "货币" + huobiIndex, "-subRoleHuobi", subRes ? "成功" : "失败", client.ClientData.RoleName, "GM命令", modifyValue, client.ClientData.ZoneID, client.strUserID, 0, client.ServerId, "");
						}
					}
					else if ("-modifyRoleHuobi" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length < 4)
						{
							string tip = "-modifyRoleHuobi参数错误，Usage：-modifyRoleHuobi roleid HuobiType value";
							LogManager.WriteLog(LogTypes.Error, tip, null, true);
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
						else
						{
							int roleID = this.SafeConvertToInt32(cmdFields[1]);
							string huobiType = cmdFields[2].ToLower();
							int modifyValue = this.SafeConvertToInt32(cmdFields[3]);
							GameClient otherClient = (roleID != -1) ? GameManager.ClientMgr.FindClient(roleID) : null;
							if (null == otherClient)
							{
								if ("lingjing" == huobiType || "mojing" == huobiType || "zaizao" == huobiType)
								{
									string cmd = string.Format("{0}:{1}:{2}", roleID, huobiType, modifyValue);
									LogManager.WriteLog(LogTypes.Error, "修改角色货币，但是角色离线，所以转到db处理，" + cmd, null, true);
									string[] dbFields = Global.ExecuteDBCmd(10182, cmd, 0);
									if (dbFields == null || dbFields.Length < 2 || Convert.ToInt32(dbFields[1]) < 0)
									{
										LogManager.WriteLog(LogTypes.Error, "修改角色货币，但是角色离线，所以转到db处理，" + cmd + ", 但是db处理失败", null, true);
									}
								}
							}
							else if (!("jinbi" == huobiType))
							{
								if (!("bangjin" == huobiType))
								{
									if (!("zuanshi" == huobiType))
									{
										if (!("bangzuan" == huobiType))
										{
											if ("mojing" == huobiType)
											{
												GameManager.ClientMgr.ModifyTianDiJingYuanValue(otherClient, modifyValue, "GM指令增加魔晶", true, true, false);
												GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色魔晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
												{
													otherClient.ClientData.RoleID,
													otherClient.ClientData.RoleName,
													modifyValue,
													GameManager.ClientMgr.GetTianDiJingYuanValue(otherClient)
												}), EventLevels.Record);
											}
											else if (!("chengjiu" == huobiType))
											{
												if (!("shengwang" == huobiType))
												{
													if (!("xinghun" == huobiType))
													{
														if ("lingjing" == huobiType)
														{
															GameManager.ClientMgr.ModifyMUMoHeValue(otherClient, modifyValue, "GM命令", true, true, false);
															GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
															{
																otherClient.ClientData.RoleID,
																otherClient.ClientData.RoleName,
																modifyValue,
																GameManager.ClientMgr.GetMUMoHeValue(otherClient)
															}), EventLevels.Record);
														}
														else if (!("fenmo" == huobiType))
														{
															if ("zaizao" == huobiType)
															{
																GameManager.ClientMgr.ModifyZaiZaoValue(otherClient, modifyValue, "GM指令增加再造点", true, true, false);
																GameManager.SystemServerEvents.AddEvent(string.Format("GM修改角色灵晶, roleID={0}({1}), 修改={2}, 现有={3}", new object[]
																{
																	otherClient.ClientData.RoleID,
																	otherClient.ClientData.RoleName,
																	modifyValue,
																	GameManager.ClientMgr.GetZaiZaoValue(otherClient)
																}), EventLevels.Record);
															}
															else
															{
																LogManager.WriteLog(LogTypes.Error, " -modifyRoleHuobi 未注册的货币类型:" + huobiType, null, true);
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
					}
					else if ("-resethefuluolan" == cmdFields[0])
					{
						Global.UpdateDBGameConfigg("hefu_luolan_guildid", "");
					}
					else if ("-clearsecondpassword" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							if (!transmit)
							{
								string tip = "-modifyRoleHuobi参数错误，Usage：-clearsecondpassword userid";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
						else
						{
							string userid = cmdFields[1];
							string tip = string.Format("-clearsecondpassword {0}", userid);
							if (SecondPasswordManager.ClearUserSecPwd(userid))
							{
								tip += " 成功";
							}
							else
							{
								tip += " 失败";
							}
							if (!transmit)
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
					}
					else if ("-monroledamage" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int mapcode = this.SafeConvertToInt32(cmdFields[1]);
							int roleid = this.SafeConvertToInt32(cmdFields[2]);
							GameManager.damageMonitor.Set(mapcode, roleid);
						}
					}
					else if ("-nomonroledamage" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int mapcode = this.SafeConvertToInt32(cmdFields[1]);
							int roleid = this.SafeConvertToInt32(cmdFields[2]);
							GameManager.damageMonitor.Remove(mapcode, roleid);
						}
					}
					else if ("-GuardStatueModEquip" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								string tip = "-GuardStatueModEquip参数错误，Usage：-GuardStatueModEquip slot soulType(-1表示脱装备)";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
							else
							{
								int slot = Convert.ToInt32(cmdFields[1]);
								int soul = Convert.ToInt32(cmdFields[2]);
								SingletonTemplate<GuardStatueManager>.Instance().GM_HandleModGuardSoulEquip(client, slot, soul);
							}
						}
					}
					else if ("-GuardPointQuery" == cmdFields[0])
					{
						if (!transmit)
						{
							string tip = SingletonTemplate<GuardStatueManager>.Instance().GM_QueryGuardPoint(client);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
						}
					}
					else if ("-GuardPointRecover" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length <= 1)
							{
								string tip = "-GuardPointRecover参数错误，Usage：-GuardPointRecover item,cnt item,cnt";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
							else
							{
								Dictionary<int, int> itemDict = new Dictionary<int, int>();
								for (int i = 1; i < cmdFields.Length; i++)
								{
									string[] szItem = cmdFields[i].Split(new char[]
									{
										','
									});
									if (szItem != null && szItem.Length == 2)
									{
										int itemID = Convert.ToInt32(szItem[0]);
										int itemCnt = Convert.ToInt32(szItem[1]);
										if (itemDict.ContainsKey(itemID))
										{
											Dictionary<int, int> dictionary;
											int key;
											(dictionary = itemDict)[key = itemID] = dictionary[key] + itemCnt;
										}
										else
										{
											itemDict.Add(itemID, itemCnt);
										}
									}
								}
								SingletonTemplate<GuardStatueManager>.Instance().GM_GuardPointRecover(client, itemDict);
							}
						}
					}
					else if ("-GuardStatueLevelUp" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<GuardStatueManager>.Instance().GM_HandleLevelUp(client);
						}
					}
					else if ("-GuardStatueSuitUp" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<GuardStatueManager>.Instance().GM_HandleSuitlUp(client);
						}
					}
					else if ("-GuardStatueQuery" == cmdFields[0])
					{
						if (!transmit)
						{
							string tip = SingletonTemplate<GuardStatueManager>.Instance().GM_QueryGuardStatue(client);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
						}
					}
					else if ("-ModGuardPoint" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int newVal = Convert.ToInt32(cmdFields[1]);
								string tip = SingletonTemplate<GuardStatueManager>.Instance().GM_ModGuardPoint(client, newVal);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
					}
					else if ("-ChangeName" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								SingletonTemplate<NameManager>.Instance().GM_ChangeNameTest(client, cmdFields[1]);
							}
						}
					}
					else if ("-MerlinStarUp1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinStarUp1(client);
						}
					}
					else if ("-MerlinStarUp2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinStarUp2(client);
						}
					}
					else if ("-MerlinLevelUp1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinLevelUp1(client);
						}
					}
					else if ("-MerlinLevelUp2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinLevelUp2(client);
						}
					}
					else if ("-MerlinSecret1" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretUpdate(client);
						}
					}
					else if ("-MerlinSecret2" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretReplace(client);
						}
					}
					else if ("-MerlinSecret3" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinSecretNotReplace(client);
						}
					}
					else if ("-MerlinInit" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.MerlinMagicBookMgr.GMMerlinInit(client);
						}
					}
					else if ("-MerlinLevelUp" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string tip = GameManager.MerlinMagicBookMgr.GMMerlinLevelUpToN(client, Global.SafeConvertToInt32(cmdFields[1]));
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
					}
					else if ("-MerlinStarUp" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								string tip = GameManager.MerlinMagicBookMgr.GMMerlinStarUpToN(client, Global.SafeConvertToInt32(cmdFields[1]));
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
					}
					else if ("-addholypart" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -addholypart 圣物id 部件id 碎片数量", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int nHolyItemID = Global.SafeConvertToInt32(cmdFields[1]);
								int nHolyPartID = Global.SafeConvertToInt32(cmdFields[2]);
								int nCount = Global.SafeConvertToInt32(cmdFields[3]);
								HolyItemManager.getInstance().GetHolyItemPart(client, (sbyte)nHolyItemID, (sbyte)nHolyPartID, nCount);
							}
						}
					}
					else if ("-addHolyPart" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -addholypart  碎片数量", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int nCount = Global.SafeConvertToInt32(cmdFields[1]);
								for (int h = 1; h <= 4; h++)
								{
									for (int p4 = 1; p4 <= 6; p4++)
									{
										HolyItemManager.getInstance().GetHolyItemPart(client, (sbyte)h, (sbyte)p4, nCount);
									}
								}
							}
						}
					}
					else if ("-holyitemlvup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -holyitemlvup 圣物id 部件id 阶数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int nHolyItemID = Global.SafeConvertToInt32(cmdFields[1]);
								int nHolyPartID = Global.SafeConvertToInt32(cmdFields[2]);
								int nLv = Global.SafeConvertToInt32(cmdFields[3]);
								HolyItemManager.getInstance().GMSetHolyItemLvup(client, (sbyte)nHolyItemID, (sbyte)nHolyPartID, (sbyte)nLv);
							}
						}
					}
					else if ("-tarotlvup" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								strinfo = string.Format("请输入： -tarotlvup 圣物id 部件id", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								int goodID = Convert.ToInt32(cmdFields[1]);
								string strret = Convert.ToInt32(TarotManager.getInstance().ProcessTarotUpCmd(client, goodID)).ToString();
							}
						}
					}
					else if ("-talentCount" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 2)
							{
								string tip = "-talentCount参数错误，Usage：-talentCount 数量";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
								return true;
							}
							int count = Convert.ToInt32(cmdFields[1]);
							if (count < 0 || count > 999)
							{
								string tip = "天赋点范围【1-999】";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
								return true;
							}
							if (!TalentManager.TalentAddCount(client, count))
							{
								string tip = "天赋未开放，或者添加天赋点错误";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, tip);
							}
						}
					}
					else if ("-socketcheck" == cmdFields[0])
					{
						long timeCount = Convert.ToInt64(cmdFields[1]);
						timeCount = timeCount * 60L * 1000L;
						List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
						foreach (TMSKSocket socket in socketList)
						{
							long nowSocket = TimeUtil.NOW();
							long spanSocket = nowSocket - socket.session.SocketTime[0];
							if (socket.session.SocketState < 4 && spanSocket > timeCount)
							{
								GameClient otherClient = GameManager.ClientMgr.FindClient(socket);
								if (null == otherClient)
								{
									Global.ForceCloseSocket(socket, "被GM踢了, 但是这个socket上没有对应的client", true);
								}
							}
						}
					}
					else if ("-socketcount" == cmdFields[0])
					{
						int count = 0;
						long timeCount = Convert.ToInt64(cmdFields[1]);
						timeCount = timeCount * 60L * 1000L;
						List<TMSKSocket> socketList = GameManager.OnlineUserSession.GetSocketList();
						foreach (TMSKSocket socket in socketList)
						{
							long nowSocket = TimeUtil.NOW();
							long spanSocket = nowSocket - socket.session.SocketTime[0];
							if (socket.session.SocketState < 4 && spanSocket > timeCount)
							{
								GameClient otherClient = GameManager.ClientMgr.FindClient(socket);
								if (null == otherClient)
								{
									count++;
								}
							}
						}
						string tip = string.Format("socket总数量：{0}，问题socket：{1}", socketList.Count, count);
						LogManager.WriteLog(LogTypes.Error, tip, null, true);
					}
					else if ("-enableprotocheck" == cmdFields[0])
					{
						bool flag8 = 1 == 0;
						if (cmdFields.Length == 2)
						{
							int flag = Convert.ToInt32(cmdFields[1]);
							string protoCheckInfo = string.Empty;
							if (1 == flag)
							{
								protoCheckInfo = "开启协议检查";
								SingletonTemplate<ProtoChecker>.Instance().SetEnableCheck(true);
							}
							else
							{
								protoCheckInfo = "关闭协议检查";
								SingletonTemplate<ProtoChecker>.Instance().SetEnableCheck(false);
							}
							LogManager.WriteLog(LogTypes.Error, protoCheckInfo, null, true);
						}
					}
					else if ("-outwaitcount" == cmdFields[0])
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("waitcount={0}", GameManager.loginWaitLogic.GetTotalWaitingCount()), null, true);
					}
					else if ("-outwaitinfo" == cmdFields[0])
					{
						int type = Convert.ToInt32(cmdFields[1]);
						int index = Convert.ToInt32(cmdFields[2]);
						GameManager.loginWaitLogic.OutWaitInfo((LoginWaitLogic.UserType)type, index);
					}
					else if ("-elementWar" == cmdFields[0])
					{
						string type2 = cmdFields[1];
						string text = type2;
						if (text != null)
						{
							if (!(text == "clearCount"))
							{
								if (!(text == "clearTime"))
								{
									if (text == "clear")
									{
										Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", Global.GetOffsetDayNow(), true);
										Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
										KuaFuManager.getInstance().SetCannotJoinKuaFuCopyEndTicks(client, 0L);
									}
								}
								else
								{
									KuaFuManager.getInstance().SetCannotJoinKuaFuCopyEndTicks(client, 0L);
								}
							}
							else
							{
								Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", Global.GetOffsetDayNow(), true);
								Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
							}
						}
					}
					else if ("-updateFuBenData" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int type = Convert.ToInt32(cmdFields[1]);
							TCPCmdHandler.isUpdateFuBenData = (type > 0);
						}
					}
					else if ("-onekeyactivetujian" == cmdFields[0])
					{
						if (!transmit && cmdFields.Length == 2)
						{
							int type = Convert.ToInt32(cmdFields[1]);
							string faildMsg = string.Empty;
							if (SingletonTemplate<TuJianManager>.Instance().GM_OneKeyActiveTuJianType(client, type, out faildMsg))
							{
								faildMsg = "成功";
							}
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "激活图鉴type=" + type.ToString() + " " + faildMsg);
						}
					}
					else if ("-morijudge" == cmdFields[0])
					{
						if (!transmit)
						{
							SingletonTemplate<MoRiJudgeManager>.Instance().OnGMCommand(client, cmdFields);
						}
					}
					else if ("-cleargembag" == cmdFields[0])
					{
						if (!transmit)
						{
							GameManager.FluorescentGemMgr.GMClearGemBag(client);
						}
					}
					else if ("-decygfm" == cmdFields[0])
					{
						if (!transmit)
						{
							int nDecPoint = Convert.ToInt32(cmdFields[1]);
							GameManager.FluorescentGemMgr.GMDecFluorescentPoint(client, nDecPoint);
						}
						else if (cmdFields.Length == 3)
						{
							int roleid = Convert.ToInt32(cmdFields[1]);
							int nDecPoint = Convert.ToInt32(cmdFields[2]);
							if (nDecPoint <= 0)
							{
								return true;
							}
							GameClient otherClient = GameManager.ClientMgr.FindClient(roleid);
							if (null == otherClient)
							{
								GameManager.FluorescentGemMgr.ModifyFluorescentPoint2DB(roleid, -nDecPoint);
							}
							else
							{
								GameManager.FluorescentGemMgr.GMDecFluorescentPoint(otherClient, nDecPoint);
							}
							LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为角色ID：【{0}】减少荧光宝石粉末【{1}】！", roleid, nDecPoint), null, true);
						}
					}
					else if ("-addygfm" == cmdFields[0])
					{
						if (!transmit)
						{
							int nAddPoint = Convert.ToInt32(cmdFields[1]);
							GameManager.FluorescentGemMgr.GMAddFluorescentPoint(client, nAddPoint);
						}
					}
					else if ("-setfreemodname" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int roleid = Convert.ToInt32(cmdFields[1]);
							int count = Convert.ToInt32(cmdFields[2]);
							SingletonTemplate<NameManager>.Instance().GM_SetFreeModName(roleid, count);
						}
						else if (!transmit)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "-setfreemodname roleid count");
						}
					}
					else if ("-changebhname" == cmdFields[0])
					{
						if (!transmit && cmdFields.Length >= 2)
						{
							SingletonTemplate<NameManager>.Instance().GM_ChangeBangHuiName(client, cmdFields[1]);
						}
					}
					else if ("-cancelTask" == cmdFields[0])
					{
						if (cmdFields.Length == 1)
						{
							TaskData taskData = TodayManager.GetTaoTask(client);
							if (taskData != null)
							{
								bool b = Global.CancelTask(client, taskData.DbID, taskData.DoingTaskID);
							}
						}
					}
					else if ("-banTimeOut" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 2)
							{
								int count = Convert.ToInt32(cmdFields[1]);
								client.ClientSocket.session.TimeOutCount = count;
								client.CheckCheatData.NextTaskListTimeout = TimeUtil.NOW();
							}
						}
					}
					else if ("-tenInit" == cmdFields[0])
					{
						TenManager.initConfig();
					}
					else if ("-sevenday" == cmdFields[0])
					{
						SingletonTemplate<SevenDayActivityMgr>.Instance().On_GM(client, cmdFields);
					}
					else if ("-spreadSetCount" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							try
							{
								string[] sCount = cmdFields[1].Split(new char[]
								{
									','
								});
								int[] counts = new int[]
								{
									int.Parse(sCount[0]),
									int.Parse(sCount[1]),
									int.Parse(sCount[2])
								};
								SpreadManager.getInstance().SpreadSetCount(client, counts);
							}
							catch
							{
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "-spreadSetCount 总人数,vip人数,level人数");
							}
						}
					}
					else if ("-setcitybh" == cmdFields[0])
					{
						if (cmdFields.Length >= 3)
						{
							try
							{
								string[] sCount = cmdFields[1].Split(new char[]
								{
									','
								});
								int[] args2 = new int[4];
								int cityId = int.Parse(cmdFields[1]);
								LangHunLingYuStatisticalData data2 = new LangHunLingYuStatisticalData();
								data2.CityId = cityId;
								data2.CompliteTime = TimeUtil.NowDateTime();
								for (int i = 2; i < cmdFields.Length; i++)
								{
									data2.SiteBhids[i - 2] = int.Parse(cmdFields[i]);
								}
								LangHunLingYuManager.getInstance().LangHunLingYuBuildMaxCityOwnerInfo(data2, 0);
								YongZheZhanChangClient.getInstance().GameFuBenComplete(data2);
								if (null != client)
								{
									LogManager.WriteLog(LogTypes.SQL, string.Format("GM通过-setcitybh 设置城池{0}攻防帮会信息", cityId), null, true);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format("GM通过-setcitybh 设置城池{0}攻防帮会信息", cityId));
								}
							}
							catch
							{
							}
						}
					}
					else if ("-soulstone" == cmdFields[0])
					{
						SingletonTemplate<SoulStoneManager>.Instance().GM_Test(client, cmdFields);
					}
					else if ("-bhzijin" == cmdFields[0])
					{
						strinfo = "指令格式为: -bhzijin 帮会ID(或0) 资金";
						if (cmdFields.Length >= 2)
						{
							int bhid = Global.SafeConvertToInt32(cmdFields[1]);
							int zhanMengZiJin = Global.SafeConvertToInt32(cmdFields[2]);
							if (bhid == 0 && null != client)
							{
								bhid = client.ClientData.Faction;
							}
							BangHuiDetailData data3 = Global.GetBangHuiDetailData(-1, bhid, 0);
							if (null != data3)
							{
								if (GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, bhid, zhanMengZiJin))
								{
									strinfo = "添加战盟资金: " + zhanMengZiJin;
									LogManager.WriteLog(LogTypes.SQL, string.Format("GM命令添加战盟资金,bhid={0}, Money={1}", bhid, zhanMengZiJin), null, true);
								}
							}
						}
						if (null != client)
						{
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
					}
					else if ("-detecthook" == cmdFields[0])
					{
						List<string> uidList = new List<string>();
						if (cmdFields.Length >= 2)
						{
							string[] szUids = cmdFields[1].Split(new char[]
							{
								','
							});
							foreach (string szUid in szUids)
							{
								if (!string.IsNullOrEmpty(szUid))
								{
									uidList.Add(szUid);
								}
							}
						}
						if (uidList.Count > 0)
						{
							FileBanLogic.BroadCastDetectHook(uidList);
						}
						else
						{
							FileBanLogic.BroadCastDetectHook();
						}
					}
					else if ("-clearbanmem" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int nHour = Global.SafeConvertToInt32(cmdFields[1]);
							BanManager.ClearBanMemory(nHour);
							FileBanLogic.ClearBanList();
						}
					}
					else if ("-logrolerelife" == cmdFields[0])
					{
						int roleId = 0;
						bool bLog = true;
						if (cmdFields.Length >= 2)
						{
							roleId = Convert.ToInt32(cmdFields[1]);
						}
						if (cmdFields.Length >= 3 && Convert.ToInt32(cmdFields[2]) < 0)
						{
							bLog = false;
						}
						if (roleId > 0)
						{
							SingletonTemplate<MonsterAttackerLogManager>.Instance().SetLogRoleRelife(roleId, bLog);
						}
					}
					else if ("-reshowluolan" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int rid = Convert.ToInt32(cmdFields[1]);
							LuoLanChengZhanManager.getInstance().ReShowLuolanKing(rid);
						}
					}
					else if ("-passiveskill" == cmdFields[0])
					{
						if (!transmit && null != client)
						{
							List<PassiveSkillData> list2 = new List<PassiveSkillData>();
							for (int i = 1; i <= cmdFields.Length - 6; i += 6)
							{
								list2.Add(new PassiveSkillData(Convert.ToInt32(cmdFields[i]), Convert.ToInt32(cmdFields[i + 1]), Convert.ToInt32(cmdFields[i + 2]), Convert.ToInt32(cmdFields[i + 3]), Convert.ToInt32(cmdFields[i + 4]), Convert.ToInt32(cmdFields[i + 5])));
							}
							client.passiveSkillModule.UpdateSkillList(list2);
						}
					}
					else if ("-palace" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length == 3)
							{
								string text = cmdFields[1];
								if (text != null)
								{
									if (!(text == "level"))
									{
										if (!(text == "count"))
										{
											if (text == "rate")
											{
												UnionPalaceManager.SetUnionPalaceRate(client, int.Parse(cmdFields[2]));
											}
										}
										else
										{
											UnionPalaceManager.SetUnionPalaceCount(client, int.Parse(cmdFields[2]));
										}
									}
									else
									{
										UnionPalaceManager.SetUnionPalaceLevelByID(client, int.Parse(cmdFields[2]));
									}
								}
							}
							else
							{
								strinfo = "参数数量不对";
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-reshowzhongshen" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int rid = Convert.ToInt32(cmdFields[1]);
							SingletonTemplate<ZhengBaManager>.Instance().SetZhongShengRole(rid);
						}
					}
					else if ("-reshowfenghuojiaren" == cmdFields[0])
					{
						if (cmdFields.Length == 3)
						{
							int rid2 = Convert.ToInt32(cmdFields[1]);
							int rid3 = Convert.ToInt32(cmdFields[2]);
							SingletonTemplate<CoupleArenaManager>.Instance().SetFengHuoJiaRenCouple(rid2, rid3);
						}
					}
					else if ("-giftcodecmd" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							GiftCodeNewManager.getInstance().ProcessGiftCodeList(cmdFields[1]);
						}
					}
					else if ("-olympicsGrade" == cmdFields[0])
					{
						if (cmdFields.Length == 2)
						{
							int grade = int.Parse(cmdFields[1]);
							OlympicsManager.getInstance().OlympicsGradeAdd(client, grade);
						}
					}
					else if ("-zhengduo" == cmdFields[0])
					{
						TianTiClient.getInstance().GmCommand(cmdFields, null);
					}
					else if ("-juntuantask" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 2)
						{
							int taskType = Global.SafeConvertToInt32(cmdFields[1]);
							int taskV = Global.SafeConvertToInt32(cmdFields[2]);
							JunTuanManager.getInstance().AddJunTuanTaskValue(client, taskType, taskV);
						}
					}
					else if ("-juntuanjoin2" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							string[] args = new string[]
							{
								client.ClientData.RoleID.ToString(),
								cmdFields[1],
								cmdFields[2]
							};
							JunTuanManager.getInstance().ProcessJunTuanJoinResponseCmd(client, 1234, null, args);
						}
					}
					else if ("-l" == cmdFields[0])
					{
						GLang.OutputToFile();
					}
					else if ("-yzzc" == cmdFields[0])
					{
						if (null != client)
						{
							string[] lines = null;
							if (cmdFields.Length >= 2)
							{
								if (File.Exists(cmdFields[1]))
								{
									lines = File.ReadAllLines(cmdFields[1]);
								}
							}
							string cmd = string.Format("{0} {1} {2}", "GameState", 1, 5);
							YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
							if (lines != null)
							{
								foreach (string line in lines)
								{
									string[] arr = line.Split(new char[]
									{
										','
									});
									YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(arr[1], Convert.ToInt32(arr[3]), Convert.ToInt32(arr[2]), 5, Convert.ToInt32(arr[5]), Convert.ToInt32(arr[6]));
									Thread.Sleep(10);
								}
							}
							else
							{
								for (int i = 0; i < 2158; i++)
								{
									YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp("u" + i, i, 54, 5, 1, 10000);
									Thread.Sleep(10);
								}
							}
							Thread.Sleep(1000);
							LogManager.WriteLog(LogTypes.Error, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
							cmd = string.Format("{0} {1} {2}", "GameState", 2, 5);
							YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
						}
					}
					else if ("-kfld" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int serverid = Global.SafeConvertToInt32(cmdFields[1]);
							int successid = Global.SafeConvertToInt32(cmdFields[2]);
							int kill = Global.SafeConvertToInt32(cmdFields[3]);
							KuaFuLueDuoStatisticalData data4 = new KuaFuLueDuoStatisticalData();
							data4.DestServerID = serverid;
							data4.SuccessServerID = successid;
							data4.GameId = -2L;
							data4.roleStatisticalData = new List<KuaFuLueDuoRoleData>
							{
								new KuaFuLueDuoRoleData
								{
									rid = client.ClientData.RoleID,
									kill = kill,
									rname = client.ClientData.RoleName,
									zoneid = client.ClientData.ZoneID
								}
							};
							HuanYingSiYuanClient.getInstance().GameFuBenComplete_KuaFuLueDuo(data4);
						}
					}
					else if ("-kfldcaiji" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int ziyuan = Global.SafeConvertToInt32(cmdFields[1]);
							if (null != client)
							{
								KuaFuLueDuoManager.getInstance().GMCaiJi(client, ziyuan);
							}
						}
					}
					else if ("-charge" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int money2 = Global.SafeConvertToInt32(cmdFields[1]);
							int itemid = Global.SafeConvertToInt32(cmdFields[2]);
							string time = TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
							if (null != client)
							{
								string cmdData = string.Format("-charge:{0}:{1}:{2}:{3}:{4}", new object[]
								{
									client.strUserID,
									money2,
									client.ClientData.RoleID,
									itemid,
									time
								});
								LogManager.WriteLog(LogTypes.SQL, cmdData, null, true);
								GameManager.DBCmdMgr.AddDBCmd(157, cmdData, null, client.ServerId);
							}
						}
					}
					else if ("-setmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int moneyType = Global.SafeConvertToInt32(cmdFields[1]);
							long newValue = (long)Global.SafeConvertToInt32(cmdFields[2]);
							client.ClientData.MoneyData[moneyType] = newValue;
							GameManager.ClientMgr.NotifySelfPropertyValue(client, moneyType, newValue);
						}
					}
					else if ("-maxteamcopy" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int copyId = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							ConstData.MaxCopyTeamMemberNumDict[copyId] = num6;
							string msg = string.Format("设置组队副本{0}人数上线为{1}", copyId, num6);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg);
						}
					}
					else if ("-huiji" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int id = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient target = client;
							if (rid > 0)
							{
								target = GameManager.ClientMgr.FindClient(rid);
							}
							else
							{
								rid = client.ClientData.RoleID;
							}
							if (null != target)
							{
								target.ClientData.HuiJiData.huiji = id;
								target.ClientData.HuiJiData.Exp = exp;
								client.sendCmd<HuiJiUpdateResultData>(1446, new HuiJiUpdateResultData
								{
									HuiJi = id,
									Exp = exp
								}, false);
							}
							RoleHuiJiData data5 = new RoleHuiJiData
							{
								huiji = id,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleHuiJiData>>(1446, new RoleDataCmdT<RoleHuiJiData>(rid, data5), 0);
						}
					}
					else if ("-showMonsterProperty" == cmdFields[0])
					{
						if (cmdFields.Length < 2)
						{
							strinfo = string.Format("请输入： -showMonsterProperty 怪物ID", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
						}
						else
						{
							Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, this.SafeConvertToInt32(cmdFields[1]));
							if (null == monster)
							{
								strinfo = string.Format("没有" + cmdFields[1] + "这个怪物", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
								return true;
							}
							StringBuilder sb = new StringBuilder();
							Global.PrintSomeProps(monster, ref sb);
							LogManager.WriteLog(LogTypes.Analysis, sb.ToString(), null, true);
						}
					}
					else if ("-setMonsterProperty" == cmdFields[0])
					{
						Monster monster = GameManager.MonsterMgr.FindMonster(client.ClientData.MapCode, this.SafeConvertToInt32(cmdFields[1]));
						if (null == monster)
						{
							strinfo = string.Format("没有" + cmdFields[1] + "这个怪物", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							return true;
						}
						if (this.SafeConvertToInt32(cmdFields[2]) == 13)
						{
							monster.VLife += (double)this.SafeConvertToInt32(cmdFields[3]);
						}
						else
						{
							monster.TempPropsBuffer.AddTempExtProp(this.SafeConvertToInt32(cmdFields[2]), (double)this.SafeConvertToInt32(cmdFields[3]), (TimeUtil.NOW() + 3600000L) * 10000L);
						}
						strinfo = string.Format("为怪物{0}调整了{1},{2}", cmdFields[1], Enum.GetName(typeof(ExtPropIndexes), this.SafeConvertToInt32(cmdFields[2])), cmdFields[3]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
					else if ("-bianshen" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int id = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient target = client;
							if (rid > 0)
							{
								target = GameManager.ClientMgr.FindClient(rid);
							}
							else
							{
								rid = client.ClientData.RoleID;
							}
							if (null != target)
							{
								target.ClientData.BianShenData.BianShen = id;
								target.ClientData.BianShenData.Exp = exp;
								client.sendCmd<BianShenUpdateResultData>(1449, new BianShenUpdateResultData
								{
									BianShen = id,
									Exp = exp
								}, false);
							}
							RoleBianShenData data6 = new RoleBianShenData
							{
								BianShen = id,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleBianShenData>>(1449, new RoleDataCmdT<RoleBianShenData>(rid, data6), 0);
						}
					}
					else if ("-bianshenexec" == cmdFields[0])
					{
						if (null != client)
						{
							BianShenManager.getInstance().processCmdEx(client, 1448, null, cmdFields);
						}
					}
					else if ("-armor" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 4)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int id = Global.SafeConvertToInt32(cmdFields[2]);
							int exp = Global.SafeConvertToInt32(cmdFields[3]);
							GameClient target = client;
							if (rid > 0)
							{
								target = GameManager.ClientMgr.FindClient(rid);
							}
							else
							{
								rid = client.ClientData.RoleID;
							}
							if (null != target)
							{
								target.ClientData.ArmorData.Armor = id;
								target.ClientData.ArmorData.Exp = exp;
								client.sendCmd<ArmorUpdateResultData>(1447, new ArmorUpdateResultData
								{
									Armor = id,
									Exp = exp
								}, false);
							}
							RoleArmorData data7 = new RoleArmorData
							{
								Armor = id,
								Exp = exp
							};
							Global.SendToDB<RoleDataCmdT<RoleArmorData>>(1447, new RoleDataCmdT<RoleArmorData>(rid, data7), 0);
						}
					}
					else if ("-ysjx" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							GameClient target = client;
							if (rid > 0)
							{
								target = GameManager.ClientMgr.FindClient(rid);
							}
							else
							{
								rid = client.ClientData.RoleID;
							}
							if (null != target)
							{
								JingLingYuanSuJueXingManager.getInstance().GMSetJingLingYuanSuJueXingData(target, cmdFields);
							}
						}
					}
					else if ("-fumomoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10217");
							long targetValue = oldValue2 + (long)num6;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "附魔灵石", "GM指令", client.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.FuMoMoney, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10217", (int)targetValue, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 146, targetValue);
						}
					}
					else if ("-yinjimoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
							long targetValue = oldValue2 + (long)num6;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生印记", "GM指令", client.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornLevelUpPoint, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10246", (int)targetValue, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 151, targetValue);
						}
					}
					else if ("-cuilianmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10249");
							long targetValue = oldValue2 + (long)num6;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "淬炼点", "GM指令", client.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornCuiLian, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10249", (int)targetValue, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 152, targetValue);
						}
					}
					else if ("-duanzaomoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10250");
							long targetValue = oldValue2 + (long)num6;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "锻造点", "GM指令", client.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornDuanZao, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10250", (int)targetValue, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 153, targetValue);
						}
					}
					else if ("-niepanmoney" == cmdFields[0])
					{
						if (client != null && cmdFields.Length >= 3)
						{
							int rid = Global.SafeConvertToInt32(cmdFields[1]);
							int num6 = Global.SafeConvertToInt32(cmdFields[2]);
							long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10251");
							long targetValue = oldValue2 + (long)num6;
							GameManager.logDBCmdMgr.AddDBLogInfo(-1, "涅槃点", "GM指令", client.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornNiePan, client.ServerId, null);
							Global.SaveRoleParamsInt32ValueToDB(client, "10251", (int)targetValue, true);
							GameManager.ClientMgr.NotifySelfPropertyValue(client, 154, targetValue);
						}
					}
					else if ("-fengyinmoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -fengyinmoney 角色名称 封印晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int fengyin = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10252");
								long targetValue = oldValue2 + (long)fengyin;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "封印晶石", "GM指令", otherClient.ClientData.RoleName, "系统", "修改", fengyin, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornFengYin, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "10252", (int)targetValue, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(otherClient, 155, targetValue);
							}
						}
					}
					else if ("-chongshengmoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -chongshengmoney 角色名称 重生晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int chongsheng = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10253");
								long targetValue = oldValue2 + (long)chongsheng;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生晶石", "GM指令", otherClient.ClientData.RoleName, "系统", "修改", chongsheng, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornChongSheng, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "10253", (int)targetValue, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(otherClient, 156, targetValue);
							}
						}
					}
					else if ("-xuancaimoney" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -xuancaimoney 角色名称 炫彩晶石数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int xuancai = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10254");
								long targetValue = oldValue2 + (long)xuancai;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炫彩晶石", "GM指令", otherClient.ClientData.RoleName, "系统", "修改", xuancai, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornXuanCai, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "10254", (int)targetValue, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(otherClient, 157, otherClient.ClientData.RebornXuanCai);
							}
						}
					}
					else if ("-guanzhu" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -guanzhu 角色名称 灌注次数", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int num6 = Global.SafeConvertToInt32(cmdFields[2]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								long oldValue2 = (long)Global.GetRoleParamsInt32FromDB(client, "10255");
								long targetValue = oldValue2 + (long)num6;
								GameManager.logDBCmdMgr.AddDBLogInfo(-1, "灌注次数", "GM指令", otherClient.ClientData.RoleName, "系统", "修改", num6, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.RebornXuanCai, client.ServerId, null);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "10255", (int)targetValue, true);
								GameManager.ClientMgr.NotifySelfPropertyValue(otherClient, 157, otherClient.ClientData.RebornEquipHole);
							}
						}
					}
					else if ("-cuilian" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 3)
							{
								strinfo = string.Format("请输入： -cuilian 角色名称 淬炼部位 淬炼等级", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int site = Global.SafeConvertToInt32(cmdFields[2]);
								int num6 = Global.SafeConvertToInt32(cmdFields[3]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								lock (RebornEquip.RebornEquipHole)
								{
									if (RebornEquip.RebornEquipHole.ContainsKey(site) && RebornEquip.RebornEquipHole[site].ContainsKey(num6))
									{
										if (otherClient.ClientData.RebornEquipHoleInfo == null || !otherClient.ClientData.RebornEquipHoleInfo.ContainsKey(site))
										{
											RebornEquipData newData = new RebornEquipData();
											newData.RoleID = otherClient.ClientData.RoleID;
											newData.Able = 0;
											newData.Level = num6;
											newData.HoleID = site;
											int ret = Global.sendToDB<int, RebornEquipData>(14123, newData, client.ServerId);
											if (ret < 0)
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("GM插入重生装备槽数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
												return false;
											}
											otherClient.ClientData.RebornEquipHoleInfo.Add(site, newData);
										}
										else
										{
											if (!RebornEquip.RebornEquipHoleLevelMap.ContainsKey(site))
											{
												return false;
											}
											if (num6 > RebornEquip.RebornEquipHoleLevelMap[site])
											{
												num6 = RebornEquip.RebornEquipHoleLevelMap[site];
											}
											RebornEquipData newData = new RebornEquipData();
											newData.RoleID = otherClient.ClientData.RoleID;
											newData.Able = otherClient.ClientData.RebornEquipHoleInfo[site].Able;
											newData.Level = num6;
											newData.HoleID = site;
											int ret = Global.sendToDB<int, RebornEquipData>(14124, newData, client.ServerId);
											if (ret <= 0)
											{
												LogManager.WriteLog(LogTypes.Error, string.Format("GM灌注更新数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
												return false;
											}
											otherClient.ClientData.RebornEquipHoleInfo.Remove(site);
											otherClient.ClientData.RebornEquipHoleInfo.Add(site, newData);
										}
									}
								}
								strinfo = string.Format("为{0}修改淬炼等级{1}", otherRoleName, otherClient.ClientData.RebornEquipHoleInfo[site].Level);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
						}
					}
					else if ("-mazinger" == cmdFields[0])
					{
						if (!transmit)
						{
							if (cmdFields.Length < 5)
							{
								strinfo = string.Format("请输入： -mazinger 角色名称 魔神类型 等阶 星 经验", new object[0]);
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							}
							else
							{
								string otherRoleName = cmdFields[1];
								int type = Global.SafeConvertToInt32(cmdFields[2]);
								int jie = Global.SafeConvertToInt32(cmdFields[3]);
								int xing = Global.SafeConvertToInt32(cmdFields[4]);
								int exp = Global.SafeConvertToInt32(cmdFields[5]);
								int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
								if (-1 == roleID)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
								if (null == otherClient)
								{
									strinfo = string.Format("{0}不在线", otherRoleName);
									GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
									return true;
								}
								if (MazingerStoreManager.getInstance().MazingerStar.ContainsKey(type))
								{
									if (MazingerStoreManager.getInstance().MazingerStar[type].ContainsKey(jie))
									{
										if (MazingerStoreManager.getInstance().MazingerStar[type][jie].ContainsKey(xing))
										{
											if (client.ClientData.MazingerStoreDataInfo.ContainsKey(type))
											{
												MazingerStoreData newData2 = MazingerStoreManager.getInstance().CopyMazingerStoreMemData(client, type);
												newData2.Exp = exp;
												newData2.Stage = jie;
												newData2.StarLevel = xing;
												int ret = Global.sendToDB<int, MazingerStoreData>(14126, newData2, client.ServerId);
												if (ret < 0)
												{
													LogManager.WriteLog(LogTypes.Error, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
													return true;
												}
												GameManager.logDBCmdMgr.AddDBLogInfo(-1, "GM指令魔神秘宝升星", DateTime.Now.ToString(), newData2.Type.ToString(), client.ClientData.RoleName, "系统", type, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
												EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[newData2.Type].StarLevel, newData2.StarLevel, client.ClientData.MazingerStoreDataInfo[newData2.Type].Exp, newData2.Exp, "魔神秘宝升星");
												client.ClientData.MazingerStoreDataInfo[newData2.Type] = newData2;
												return true;
											}
											else
											{
												MazingerStoreData newData2 = new MazingerStoreData();
												newData2.Type = type;
												newData2.Stage = jie;
												newData2.StarLevel = xing;
												newData2.Exp = exp;
												newData2.RoleID = client.ClientData.RoleID;
												int ret = Global.sendToDB<int, MazingerStoreData>(14125, newData2, client.ServerId);
												if (ret < 0)
												{
													LogManager.WriteLog(LogTypes.Error, string.Format("魔神秘宝修改数据出错, 玩家id RoleID={0}", client.ClientData.RoleID), null, true);
													return true;
												}
												GameManager.logDBCmdMgr.AddDBLogInfo(-1, "GM指令魔神秘宝升星", DateTime.Now.ToString(), newData2.Type.ToString(), client.ClientData.RoleName, "系统", type, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
												EventLogManager.AddMazingerStoreEvent(client, client.ClientData.MazingerStoreDataInfo[newData2.Type].StarLevel, newData2.StarLevel, client.ClientData.MazingerStoreDataInfo[newData2.Type].Exp, newData2.Exp, "魔神秘宝升星");
												if (client.ClientData.MazingerStoreDataInfo == null)
												{
													client.ClientData.MazingerStoreDataInfo = new Dictionary<int, MazingerStoreData>();
												}
												client.ClientData.MazingerStoreDataInfo.Add(newData2.Type, newData2);
												return true;
											}
										}
									}
								}
								GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "参数看魔神秘宝配置");
								return true;
							}
						}
					}
					else if ("-emptyreborn" == cmdFields[0])
					{
						if (client.ClientData.RebornGoodsDataList != null)
						{
							List<GoodsData> NewRebornGoodsDataList = new List<GoodsData>();
							NewRebornGoodsDataList.AddRange(client.ClientData.RebornGoodsDataList);
							foreach (GoodsData goodsData in NewRebornGoodsDataList)
							{
								if (goodsData != null && goodsData.Site == 15000 && goodsData.Using <= 0)
								{
									string modGoodsCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
									{
										client.ClientData.RoleID,
										4,
										goodsData.Id,
										goodsData.GoodsID,
										0,
										goodsData.Site,
										goodsData.GCount,
										goodsData.BagIndex,
										""
									});
									if (TCPProcessCmdResults.RESULT_OK == Global.ModifyGoodsByCmdParams(client, modGoodsCmd, "客户端修改", null))
									{
										client.ClientData.RebornGoodsDataList.Remove(goodsData);
									}
								}
							}
						}
					}
					else if (!transmit)
					{
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "what do you mean??? [" + cmdFields[0] + "]");
					}
				}
			}
			return true;
		}

		// Token: 0x060029C3 RID: 10691 RVA: 0x00257BC4 File Offset: 0x00255DC4
		public static void GMSetTime(GameClient client, string[] cmdFields, bool allServer = false)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			string datatimeStr;
			if (cmdFields.Length <= 2)
			{
				datatimeStr = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			}
			else if (cmdFields.Length == 3)
			{
				datatimeStr = cmdFields[2].Replace('：', ':');
				if (TimeSpan.TryParse(datatimeStr, out timeSpan))
				{
					datatimeStr = TimeUtil.NowDateTime().Add(timeSpan).ToString("yyyy-MM-dd HH:mm:ss");
				}
				else
				{
					datatimeStr = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
				}
			}
			else
			{
				datatimeStr = cmdFields[2] + " " + cmdFields[3];
			}
			datatimeStr = datatimeStr.Replace('：', ':');
			DateTime dt;
			if (DateTime.TryParse(datatimeStr, out dt))
			{
				if (dt < TimeUtil.NowDateTime())
				{
					if (null != client)
					{
						string strinfo = string.Format("禁止时间倒退！！！", new object[0]);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					}
				}
				else if (!allServer)
				{
					DateTime newTime = TimeUtil.SetTime(datatimeStr);
					Thread.Sleep(10);
					LogManager.WriteLog(LogTypes.Error, string.Format("GM命令修改时间#from={0},to={1},realtime={2}", TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"), datatimeStr, DateTime.Now.ToString()), null, true);
					int index = 0;
					GameClient c;
					while ((c = GameManager.ClientMgr.GetNextClient(ref index, true)) != null)
					{
						c.ClientData.LastClientHeartTicks = newTime.Ticks / 10000L;
						c.sendCmd(832, string.Format("{0}:{1}:{2}", c.ClientData.RoleID, 0, TimeUtil.NOW() * 10000L), false);
					}
				}
				else
				{
					string gmSetTimeCmd = string.Format("-settime 0 {0}", datatimeStr);
					YongZheZhanChangClient.getInstance().ExecuteCommand(gmSetTimeCmd);
				}
			}
			else if (null != client)
			{
				string strinfo = string.Format("错误的时间格式！！！", new object[0]);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
			}
		}

		// Token: 0x060029C4 RID: 10692 RVA: 0x00257E30 File Offset: 0x00256030
		public bool GMSetRebornLevel(GameClient client, string[] cmdFields)
		{
			string otherRoleName = cmdFields[1];
			int level = this.SafeConvertToInt32(cmdFields[2]);
			int nNewRebornCount = client.ClientData.RebornCount;
			if (cmdFields.Length >= 4)
			{
				nNewRebornCount = this.SafeConvertToInt32(cmdFields[3]);
			}
			int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
			bool result;
			if (-1 == roleID)
			{
				string strinfo = string.Format("{0}不在线", otherRoleName);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				result = true;
			}
			else
			{
				GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
				if (null == otherClient)
				{
					string strinfo = string.Format("{0}不在线", otherRoleName);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
				else if (!RebornManager.getInstance().CheckRebornCountLevelValid(client, nNewRebornCount, level))
				{
					string strinfo = string.Format("设置的级别超出了最大限制", level);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
				else
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置重生级别{1}", otherRoleName, level), null, true);
					otherClient.ClientData.RebornLevel = level;
					Global.SaveRoleParamsInt32ValueToDB(otherClient, "10241", otherClient.ClientData.RebornLevel, true);
					otherClient.ClientData.Experience = 0L;
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, 0L);
					if (cmdFields.Length >= 4)
					{
						otherClient.ClientData.RebornCount = nNewRebornCount;
						Global.SaveRoleParamsInt32ValueToDB(otherClient, "10240", otherClient.ClientData.RebornCount, true);
					}
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
					RebornManager.getInstance().NotifySelfExperience(otherClient, 0L);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(otherClient, 0L, "", MoneyTypes.RebornExpMonster, false, true, false);
					GameManager.ClientMgr.ModifyRebornExpMaxAddValue(otherClient, 0L, "", MoneyTypes.RebornExpSale, false, true, false);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取重生经验和级别, roleID={0}({1}), Level={2}, Experience={3}, newExperience={4}", new object[]
					{
						otherClient.ClientData.RoleID,
						otherClient.ClientData.RoleName,
						otherClient.ClientData.RebornCount,
						otherClient.ClientData.RebornExperience,
						0
					}), EventLevels.Hint);
					string strinfo = string.Format("为{0}设置了重生级别{1}", otherRoleName, level);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060029C5 RID: 10693 RVA: 0x00258124 File Offset: 0x00256324
		public void GMSetGoodsForgeLevel(GameClient client, GoodsData goods, int forgelev, int bind, bool ntfprops = false)
		{
			if (client != null && null != goods)
			{
				goods.Forge_level = forgelev;
				goods.Binding = bind;
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					1,
					client.ClientData.RoleID,
					goods.Id,
					goods.Forge_level,
					goods.Binding
				});
				client.sendCmd(161, strcmd, false);
				ChengJiuManager.OnRoleEquipmentQiangHua(client, goods.Forge_level);
				SevenDayGoalEventObject forgePeidaiEv = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.PeiDaiForgeEquip);
				GlobalEventSource.getInstance().fireEvent(forgePeidaiEv);
				SevenDayGoalEventObject forgeLevelEv = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ForgeEquipLevel);
				forgeLevelEv.Arg1 = goods.Forge_level;
				GlobalEventSource.getInstance().fireEvent(forgeLevelEv);
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.EquipForgeLevel, goods.Forge_level);
				string[] dbFields = null;
				string strDbCmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					goods.Id,
					"*",
					goods.Forge_level,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					goods.Binding,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				Global.RequestToDBServer(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10006, strDbCmd, out dbFields, client.ServerId);
				Global.ModRoleGoodsEvent(client, goods, 0, "强化(GM)", false);
				EventLogManager.AddGoodsEvent(client, OpTypes.Forge, OpTags.None, goods.GoodsID, (long)goods.Id, 0, goods.GCount, "强化(GM)");
				ChengJiuManager.OnFirstQiangHua(client);
				if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriStrengthen))
				{
					client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
					client._IconStateMgr.SendIconStateToClient(client);
				}
				if (ntfprops)
				{
					Global.RefreshEquipProp(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		// Token: 0x060029C6 RID: 10694 RVA: 0x00258428 File Offset: 0x00256628
		public void GMSetWingSuitStar(GameClient client, string[] cmdFields)
		{
			if (cmdFields.Length == 3)
			{
				int suit = this.SafeConvertToInt32(cmdFields[1]);
				int star = this.SafeConvertToInt32(cmdFields[2]);
				string strinfo;
				if (suit <= 0 || suit > MUWingsManager.MaxWingID)
				{
					strinfo = "阶数错误 1 - 9";
				}
				else if (star < 0 || star > MUWingsManager.MaxWingEnchanceLevel)
				{
					strinfo = "星数错误 0 - 10";
				}
				else if (MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, suit, client.ClientData.MyWingData.JinJieFailedNum, star, 0, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum) < 0)
				{
					strinfo = "存数据库错误";
				}
				else
				{
					if (1 == client.ClientData.MyWingData.Using)
					{
						MUWingsManager.UpdateWingDataProps(client, false);
					}
					client.ClientData.MyWingData.WingID = suit;
					client.ClientData.MyWingData.ForgeLevel = star;
					if (1 == client.ClientData.MyWingData.Using)
					{
						MUWingsManager.UpdateWingDataProps(client, true);
						client.sendCmd<WingData>(678, client.ClientData.MyWingData, false);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
					strinfo = "修改翅膀成功";
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
			}
			else
			{
				string strinfo = "参数数量不对";
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
			}
		}

		// Token: 0x060029C7 RID: 10695 RVA: 0x00258668 File Offset: 0x00256868
		public bool GMSetLevel(GameClient client, string[] cmdFields)
		{
			string otherRoleName = cmdFields[1];
			int level = this.SafeConvertToInt32(cmdFields[2]);
			int roleID = RoleName2IDs.FindRoleIDByName(otherRoleName, false);
			bool result;
			if (-1 == roleID)
			{
				string strinfo = string.Format("{0}不在线", otherRoleName);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
				result = true;
			}
			else
			{
				GameClient otherClient = GameManager.ClientMgr.FindClient(roleID);
				if (null == otherClient)
				{
					string strinfo = string.Format("{0}不在线", otherRoleName);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
				else if (level < 0 || level > 400)
				{
					string strinfo = string.Format("设置的级别{0}超出了最大限制400", level);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
				else
				{
					LogManager.WriteLog(LogTypes.SQL, string.Format("根据GM的要求为{0}设置级别{1}", otherRoleName, level), null, true);
					otherClient.ClientData.Level = level;
					GameManager.DBCmdMgr.AddDBCmd(10002, string.Format("{0}:{1}:{2}", otherClient.ClientData.RoleID, otherClient.ClientData.Level, otherClient.ClientData.Experience), null, otherClient.ServerId);
					otherClient.ClientData.Experience = 0L;
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, 0L);
					string strinfo;
					if (cmdFields.Length >= 4)
					{
						int nNewChangeCount = this.SafeConvertToInt32(cmdFields[3]);
						if (nNewChangeCount < 0 || nNewChangeCount > 100)
						{
							strinfo = string.Format("转生级别不在有效范围[0-100]!", new object[0]);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
							return true;
						}
						int nChangeCount = otherClient.ClientData.ChangeLifeCount;
						lock (otherClient.ClientData.PropPointMutex)
						{
							if (cmdFields.Length >= 5)
							{
								int[] points = new int[4];
								int countExt = this.SafeConvertToInt32(cmdFields[4]);
								for (int i = 0; i < points.Length; i++)
								{
									if (i < cmdFields.Length - 4)
									{
										countExt = this.SafeConvertToInt32(cmdFields[4 + i]);
									}
									points[i] = countExt;
								}
								otherClient.ClientData.PropStrength += points[0] - Global.GetRoleParamsInt32FromDB(otherClient, "PropStrengthChangeless");
								otherClient.ClientData.PropIntelligence += points[1] - Global.GetRoleParamsInt32FromDB(otherClient, "PropIntelligenceChangeless");
								otherClient.ClientData.PropDexterity += points[2] - Global.GetRoleParamsInt32FromDB(otherClient, "PropDexterityChangeless");
								otherClient.ClientData.PropConstitution += points[3] - Global.GetRoleParamsInt32FromDB(otherClient, "PropConstitutionChangeless");
								otherClient.ClientData.TotalPropPoint += points[0] - Global.GetRoleParamsInt32FromDB(otherClient, "PropStrengthChangeless");
								otherClient.ClientData.TotalPropPoint += points[1] - Global.GetRoleParamsInt32FromDB(otherClient, "PropIntelligenceChangeless");
								otherClient.ClientData.TotalPropPoint += points[2] - Global.GetRoleParamsInt32FromDB(otherClient, "PropDexterityChangeless");
								otherClient.ClientData.TotalPropPoint += points[3] - Global.GetRoleParamsInt32FromDB(otherClient, "PropConstitutionChangeless");
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropStrength", otherClient.ClientData.PropStrength, true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropIntelligence", otherClient.ClientData.PropIntelligence, true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropDexterity", otherClient.ClientData.PropDexterity, true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropConstitution", otherClient.ClientData.PropConstitution, true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "TotalPropPoint", otherClient.ClientData.TotalPropPoint, true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropStrengthChangeless", points[0], true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropIntelligenceChangeless", points[1], true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropDexterityChangeless", points[2], true);
								Global.SaveRoleParamsInt32ValueToDB(otherClient, "PropConstitutionChangeless", points[3], true);
							}
							nChangeCount = nNewChangeCount;
							otherClient.ClientData.ChangeLifeCount = nChangeCount;
						}
						GameManager.DBCmdMgr.AddDBCmd(509, string.Format("{0}:{1}", otherClient.ClientData.RoleID, otherClient.ClientData.ChangeLifeCount), null, otherClient.ServerId);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
					}
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
					GameManager.ClientMgr.NotifyTeamUpLevel(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, false);
					Global.AutoLearnSkills(client);
					GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, 0L);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取经验和级别, roleID={0}({1}), Level={2}, Experience={3}, newExperience={4}", new object[]
					{
						otherClient.ClientData.RoleID,
						otherClient.ClientData.RoleName,
						otherClient.ClientData.Level,
						otherClient.ClientData.Experience,
						0
					}), EventLevels.Hint);
					strinfo = string.Format("为{0}设置了级别{1}", otherRoleName, level);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strinfo);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0400395D RID: 14685
		private string[] SuperGMUserNames = null;

		// Token: 0x0400395E RID: 14686
		private string[] GMUserNames = null;

		// Token: 0x0400395F RID: 14687
		private string[] GMIPs = null;

		// Token: 0x04003960 RID: 14688
		private Dictionary<string, int> OtherUserNamesDict = new Dictionary<string, int>();

		// Token: 0x04003961 RID: 14689
		private Dictionary<int, string[]> GMCmdsDict = new Dictionary<int, string[]>();

		// Token: 0x04003962 RID: 14690
		private Dictionary<string, GmCmdHandler> GmCmdsHandlerDict = new Dictionary<string, GmCmdHandler>();

		// Token: 0x04003963 RID: 14691
		private List<GameClient> GMClientList = new List<GameClient>();

		// Token: 0x04003964 RID: 14692
		public static bool EnableGMSetAllServerTime = false;
	}
}
