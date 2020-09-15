using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LiXianGuaJi;
using GameServer.Logic.LoginWaiting;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Olympics;
using GameServer.Logic.OnePiece;
using GameServer.Logic.Reborn;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.Spread;
using GameServer.Logic.Tarot;
using GameServer.Logic.UnionAlly;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserReturn;
using GameServer.Logic.WanMota;
using GameServer.Logic.YueKa;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	// Token: 0x020006C7 RID: 1735
	public class ClientManager
	{
		// Token: 0x06002132 RID: 8498 RVA: 0x001C57D4 File Offset: 0x001C39D4
		public int GetMaxClientCount()
		{
			return 2000;
		}

		// Token: 0x06002133 RID: 8499 RVA: 0x001C57EC File Offset: 0x001C39EC
		public void initialize(IEnumerable<XElement> mapItems)
		{
			this.Container.initialize(mapItems);
			for (int i = 0; i < 2000; i++)
			{
				this._ArrayClients[i] = null;
				this._FreeClientList.Add(i);
			}
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x001C5834 File Offset: 0x001C3A34
		public bool AddClient(GameClient client)
		{
			try
			{
				GameClient gc = this.FindClient(client.ClientData.RoleID);
				if (null != gc)
				{
					if (gc.ClientData.ClosingClientStep <= 0)
					{
						return false;
					}
					this.RemoveClient(gc);
				}
				int index = -1;
				lock (this._FreeClientList)
				{
					if (this._FreeClientList == null || this._FreeClientList.Count <= 0)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("ClientManager::AddClient _FreeClientList.Count <= 0", new object[0]), null, true);
						return false;
					}
					index = this._FreeClientList[0];
					this._FreeClientList.RemoveAt(0);
				}
				this._ArrayClients[index] = client;
				client.ClientSocket.Nid = index;
				lock (this._DictClientNids)
				{
					this._DictClientNids[client.ClientData.RoleID] = index;
				}
				this.AddClientToContainer(client);
			}
			catch (Exception e)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ClientManager::AddClient ==>{0}", e.ToString()), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x001C59F4 File Offset: 0x001C3BF4
		public void AddClientToContainer(GameClient client)
		{
			this.Container.AddObject(client.ClientData.RoleID, client.ClientData.MapCode, client);
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x001C5A1C File Offset: 0x001C3C1C
		public void RemoveClient(GameClient client)
		{
			try
			{
				int nNid = this.FindClientNid(client.ClientData.RoleID);
				if (nNid != client.ClientSocket.Nid)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ClientManager::RemoveClient nNid={0}, client.ClientSocket.Nid={1]", nNid, client.ClientSocket.Nid), null, true);
				}
			}
			catch (Exception e)
			{
			}
			lock (this._DictClientNids)
			{
				try
				{
					this._DictClientNids.Remove(client.ClientData.RoleID);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
					try
					{
						this._DictClientNids.Remove(client.ClientData.RoleID);
					}
					catch (Exception ex2)
					{
						LogManager.WriteException(string.Format("try agin:{0}", ex2.ToString()));
					}
				}
			}
			if (client.ClientSocket.Nid >= 0 && client.ClientSocket.Nid < 2000)
			{
				this._ArrayClients[client.ClientSocket.Nid] = null;
				lock (this._FreeClientList)
				{
					this._FreeClientList.Add(client.ClientSocket.Nid);
				}
			}
			else
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("ClientManager::RemoveClient nid={0} out range", client.ClientSocket.Nid), null, true);
			}
			client.ClientSocket.Nid = -1;
			this.RemoveClientFromContainer(client);
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x001C5C18 File Offset: 0x001C3E18
		public void RemoveClientFromContainer(GameClient client)
		{
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap) || null == gameMap)
			{
				LogManager.WriteLog(LogTypes.Error, "RemoveClientFromContainer 错误的地图编号：" + client.ClientData.MapCode, null, true);
			}
			else
			{
				bool removed = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].RemoveObject(client);
				if (!this.Container.RemoveObject(client.ClientData.RoleID, client.ClientData.MapCode, client) || !removed)
				{
					foreach (int mc in GameManager.MapMgr.DictMaps.Keys)
					{
						GameManager.MapGridMgr.DictGrids[mc].RemoveObject(client);
						this.Container.RemoveObject(client.ClientData.RoleID, mc, client);
					}
				}
			}
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x001C5D50 File Offset: 0x001C3F50
		public int FindClientNid(int RoleID)
		{
			int nNid = -1;
			lock (this._DictClientNids)
			{
				if (!this._DictClientNids.TryGetValue(RoleID, out nNid))
				{
					return -1;
				}
			}
			return nNid;
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x001C5DB8 File Offset: 0x001C3FB8
		public GameClient FindClientByNid(int nNid)
		{
			GameClient result;
			if (nNid < 0 || nNid >= 2000)
			{
				result = null;
			}
			else
			{
				result = this._ArrayClients[nNid];
			}
			return result;
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x001C5DEC File Offset: 0x001C3FEC
		public GameClient FindClient(TMSKSocket socket)
		{
			GameClient result;
			if (null == socket)
			{
				result = null;
			}
			else
			{
				result = this.FindClientByNid(socket.Nid);
			}
			return result;
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x001C5E1C File Offset: 0x001C401C
		public GameClient FindClient(int roleID)
		{
			int nNid = this.FindClientNid(roleID);
			return this.FindClientByNid(nNid);
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x001C5E40 File Offset: 0x001C4040
		public bool ClientExists(GameClient client)
		{
			object obj = null;
			lock (this.Container.ObjectDict)
			{
				this.Container.ObjectDict.TryGetValue(client.ClientData.RoleID, out obj);
			}
			return null != obj;
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x001C5EB8 File Offset: 0x001C40B8
		public GameClient GetNextClient(ref int nNid, bool loading = false)
		{
			GameClient result;
			if (nNid < 0 || nNid >= 2000)
			{
				result = null;
			}
			else
			{
				GameClient client = null;
				while (nNid < 2000)
				{
					if (null != this._ArrayClients[nNid])
					{
						client = this._ArrayClients[nNid];
						if (!loading)
						{
							if (client.ClientData.FirstPlayStart || client.ClientData.ClosingClientStep != 0)
							{
								goto IL_71;
							}
						}
						nNid++;
						break;
					}
					IL_71:
					nNid++;
				}
				result = client;
			}
			return result;
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x001C5F50 File Offset: 0x001C4150
		public List<object> GetMapClients(int mapCode)
		{
			return this.Container.GetObjectsByMap(mapCode);
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x001C5F70 File Offset: 0x001C4170
		public List<GameClient> GetMapGameClients(int mapCode)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			List<GameClient> clientList = new List<GameClient>();
			if (null != objsList)
			{
				foreach (object obj in objsList)
				{
					GameClient client = obj as GameClient;
					if (null != client)
					{
						clientList.Add(client);
					}
				}
			}
			return clientList;
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x001C6004 File Offset: 0x001C4204
		public List<GameClient> GetMapAliveClients(int mapCode)
		{
			List<GameClient> lsAliveClient = new List<GameClient>();
			List<object> lsObjects = this.GetMapClients(mapCode);
			List<GameClient> result;
			if (null == lsObjects)
			{
				result = lsAliveClient;
			}
			else
			{
				for (int i = 0; i < lsObjects.Count; i++)
				{
					GameClient client = lsObjects[i] as GameClient;
					if (client != null && client.ClientData.CurrentLifeV > 0)
					{
						lsAliveClient.Add(client);
					}
				}
				result = lsAliveClient;
			}
			return result;
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x001C608C File Offset: 0x001C428C
		public List<GameClient> GetMapAliveClientsEx(int mapCode, bool writeLog = true)
		{
			List<GameClient> lsAliveClient = new List<GameClient>();
			List<object> lsObjects = this.Container.GetObjectsByMap(mapCode);
			List<GameClient> result;
			if (null == lsObjects)
			{
				result = lsAliveClient;
			}
			else
			{
				for (int i = 0; i < lsObjects.Count; i++)
				{
					GameClient client = lsObjects[i] as GameClient;
					if (client != null && client.ClientData.CurrentLifeV > 0)
					{
						bool valid = false;
						if (!client.ClientData.WaitingNotifyChangeMap && !client.ClientData.WaitingForChangeMap)
						{
							if (client.ClientData.MapCode == mapCode && Global.IsPosReachable(mapCode, client.ClientData.PosX, client.ClientData.PosY))
							{
								valid = true;
								lsAliveClient.Add(client);
							}
							else
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数client.MapCode={0};maCode={1};", client.ClientData.MapCode, mapCode), null, true);
							}
							if (writeLog && !valid)
							{
								string reason = string.Format("存活玩家坐标非法:{6}({7}) mapCode:{0},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode,
									client.ClientData.MapCode,
									client.ClientData.WaitingNotifyChangeMap,
									client.ClientData.WaitingForChangeMap,
									client.ClientData.PosX,
									client.ClientData.PosY,
									client.ClientData.RoleID,
									client.ClientData.RoleName
								});
								LogManager.WriteLog(LogTypes.Error, reason, null, true);
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数 WaitingNotifyChangeMap={0};WaitingForChangeMap={1}", client.ClientData.WaitingNotifyChangeMap, client.ClientData.WaitingForChangeMap), null, true);
						}
					}
					else if (null != client)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数角色生命值:{0}", client.ClientData.CurrentLifeV), null, true);
					}
				}
				result = lsAliveClient;
			}
			return result;
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x001C62E4 File Offset: 0x001C44E4
		public int GetMapAliveClientCountEx(int mapCode)
		{
			int aliveClientCount = 0;
			List<object> lsObjects = this.Container.GetObjectsByMap(mapCode);
			int result;
			if (null == lsObjects)
			{
				result = aliveClientCount;
			}
			else
			{
				for (int i = 0; i < lsObjects.Count; i++)
				{
					GameClient client = lsObjects[i] as GameClient;
					if (client != null && client.ClientData.CurrentLifeV > 0)
					{
						if (!client.ClientData.WaitingNotifyChangeMap && !client.ClientData.WaitingForChangeMap)
						{
							if (client.ClientData.MapCode == mapCode && !Global.InOnlyObsByXY(ObjectTypes.OT_CLIENT, mapCode, client.ClientData.PosX, client.ClientData.PosY))
							{
								aliveClientCount++;
							}
							else
							{
								LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数client.MapCode={0};maCode={1};", client.ClientData.MapCode, mapCode), null, true);
								string reason = string.Format("玩家坐标:{6}({7}) mapCode:{0},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode,
									client.ClientData.MapCode,
									client.ClientData.WaitingNotifyChangeMap,
									client.ClientData.WaitingForChangeMap,
									client.ClientData.PosX,
									client.ClientData.PosY,
									client.ClientData.RoleID,
									client.ClientData.RoleName
								});
								LogManager.WriteLog(LogTypes.Error, reason, null, true);
							}
						}
						else
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数 WaitingNotifyChangeMap={0};WaitingForChangeMap={1}", client.ClientData.WaitingNotifyChangeMap, client.ClientData.WaitingForChangeMap), null, true);
						}
					}
					else if (null != client)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数角色生命值:{0}", client.ClientData.CurrentLifeV), null, true);
					}
				}
				result = aliveClientCount;
			}
			return result;
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x001C651C File Offset: 0x001C471C
		public int GetMapClientsCount(int mapCode)
		{
			return this.Container.GetObjectsCountByMap(mapCode);
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x001C653C File Offset: 0x001C473C
		public int GetClientCount()
		{
			int count = 0;
			lock (this._FreeClientList)
			{
				count = this._FreeClientList.Count;
			}
			return 2000 - count;
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x001C659C File Offset: 0x001C479C
		public int GetClientCountFromDict()
		{
			int count = 0;
			lock (this._DictClientNids)
			{
				count = this._DictClientNids.Count;
			}
			return count;
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x001C65F8 File Offset: 0x001C47F8
		public string GetAllMapRoleNumStr()
		{
			return this.Container.GetAllMapRoleNumStr();
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x001C6618 File Offset: 0x001C4818
		public GameClient GetFirstClient()
		{
			GameClient client = null;
			lock (this._DictClientNids)
			{
				if (this._DictClientNids.Count > 0)
				{
					using (Dictionary<int, int>.Enumerator enumerator = this._DictClientNids.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<int, int> item = enumerator.Current;
							return this.FindClientByNid(item.Value);
						}
					}
				}
			}
			return client;
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x001C66D8 File Offset: 0x001C48D8
		public GameClient GetRandomClient()
		{
			lock (this._DictClientNids)
			{
				if (this._DictClientNids.Count > 0)
				{
					int[] array = new int[2000];
					this._DictClientNids.Values.CopyTo(array, 0);
					int index = Global.GetRandomNumber(0, this._DictClientNids.Count);
					return this.FindClientByNid(array[index]);
				}
			}
			return null;
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x001C677C File Offset: 0x001C497C
		public void PushBackTcpOutPacket(TCPOutPacket tcpOutPacket)
		{
			if (null != tcpOutPacket)
			{
				Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
			}
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x001C67A8 File Offset: 0x001C49A8
		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, byte[] bytesData, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient c = objsList[i] as GameClient;
							if (null != c)
							{
								if (!c.LogoutState)
								{
									if (null == tcpOutPacket)
									{
										tcpOutPacket = pool.Pop();
										tcpOutPacket.PacketCmdID = (ushort)cmdID;
										tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
									}
									if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpOutPacket, false))
									{
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x0600214B RID: 8523 RVA: 0x001C689C File Offset: 0x001C4A9C
		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, string strCmd, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient c = objsList[i] as GameClient;
							if (null != c)
							{
								if (!c.LogoutState)
								{
									if (null == tcpOutPacket)
									{
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, cmdID);
									}
									if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpOutPacket, false))
									{
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x0600214C RID: 8524 RVA: 0x001C697C File Offset: 0x001C4B7C
		public void SendToClients<T>(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, T scData, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							if (objsList[i] is GameClient)
							{
								if (!(objsList[i] as GameClient).LogoutState)
								{
									(objsList[i] as GameClient).sendCmd<T>(cmdID, scData, false);
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x001C6A3C File Offset: 0x001C4C3C
		public void SendToClients<T1, T2>(SocketListener sl, TCPOutPacketPool pool, object self, List<T1> objsList, T2 data, int cmdID, int hideFlag, int includeRoleId)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient c = objsList[i] as GameClient;
							if (null != c)
							{
								if (c.ClientData.RoleID == includeRoleId || (c.ClientEffectHideFlag1 & hideFlag) <= 0)
								{
									if (!c.LogoutState)
									{
										if (null == tcpOutPacket)
										{
											if (data is string)
											{
												tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as string, cmdID);
											}
											else
											{
												if (!(data is byte[]))
												{
													break;
												}
												tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as byte[], cmdID);
											}
										}
										if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpOutPacket, false))
										{
										}
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x001C6BC4 File Offset: 0x001C4DC4
		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, object obj, List<object> objsList, byte[] bytesData, byte[] bytesData2, int cmdID, bool sendIfHide)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, cmdID);
				TCPOutPacket tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, bytesData2, 0, bytesData2.Length, cmdID);
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (null != c)
						{
							if (!c.LogoutState)
							{
								if (c == self || c == obj || c.ClientEffectHideFlag1 <= 0)
								{
									sl.SendData(c.ClientSocket, tcpOutPacket, false);
								}
								else if (sendIfHide)
								{
									sl.SendData(c.ClientSocket, tcpOutPacket2, false);
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
					this.PushBackTcpOutPacket(tcpOutPacket2);
				}
			}
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x001C6CC4 File Offset: 0x001C4EC4
		public void SendToClients<T1, T2>(SocketListener sl, TCPOutPacketPool pool, object self, object obj, List<T1> objsList, T2 data, T2 data2, int cmdID, int hideFlag, int includeRoleId)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				TCPOutPacket tcpOutPacket2 = null;
				if (data is string)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as string, cmdID);
					tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, data2 as string, cmdID);
				}
				else
				{
					if (!(data is byte[]))
					{
						return;
					}
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as byte[], cmdID);
					tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, data2 as byte[], cmdID);
				}
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient c = objsList[i] as GameClient;
						if (null != c)
						{
							if (c.ClientData.RoleID == includeRoleId || (c.ClientEffectHideFlag1 & hideFlag) <= 0)
							{
								if (!c.LogoutState)
								{
									if (c == self || c == obj || c.ClientEffectHideFlag1 <= 0)
									{
										sl.SendData(c.ClientSocket, tcpOutPacket, false);
									}
									else
									{
										sl.SendData(c.ClientSocket, tcpOutPacket2, false);
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
					this.PushBackTcpOutPacket(tcpOutPacket2);
				}
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x001C6E60 File Offset: 0x001C5060
		public void SendToClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strCmd, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, cmdID);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x001C6EAC File Offset: 0x001C50AC
		public void SendToClient(GameClient client, string strCmd, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strCmd, cmdID);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x001C6F08 File Offset: 0x001C5108
		public void SendToClient(GameClient client, byte[] buffer, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, buffer, 0, buffer.Length, cmdID);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x001C6F68 File Offset: 0x001C5168
		public void NotifyClientOpenWindow(GameClient client, int windowType, string strParams)
		{
			string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, windowType, strParams);
			this.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, cmd, 419);
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x001C6FBC File Offset: 0x001C51BC
		public void NotifyOthersIamComing(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, int cmd)
		{
			if (null != objsList)
			{
				RoleData roleData = Global.ClientToRoleData2(client);
				byte[] bytesData = DataHelper.ObjectToBytes<RoleData>(roleData);
				this.SendToClients(sl, pool, client, objsList, bytesData, cmd);
			}
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x001C6FF8 File Offset: 0x001C51F8
		public int NotifySelfOnlineOthers(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, int cmd)
		{
			int result;
			if (null == objsList)
			{
				result = 0;
			}
			else
			{
				int totalCount = 0;
				int i = 0;
				while (i < objsList.Count && i < 30)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							TCPOutPacket tcpOutPacket;
							if (1 == GameManager.RoleDataMiniMode)
							{
								RoleDataMini roleDataMini = Global.ClientToRoleDataMini(objsList[i] as GameClient);
								roleDataMini.BufferMiniInfo = Global.GetBufferMiniList(objsList[i] as GameClient);
								tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataMini>(roleDataMini, pool, cmd);
							}
							else
							{
								RoleData roleData = Global.ClientToRoleData2(objsList[i] as GameClient);
								tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(roleData, pool, cmd);
							}
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
								break;
							}
							totalCount++;
						}
					}
					i++;
				}
				result = totalCount;
			}
			return result;
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x001C7128 File Offset: 0x001C5328
		public void NotifySelfOnline(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int cmd)
		{
			if (null != otherClient)
			{
				RoleData roleData = Global.ClientToRoleData2(otherClient);
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(roleData, pool, cmd);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x001C716C File Offset: 0x001C536C
		public void NotifySelfOnlineData(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int cmd)
		{
			if (null != otherClient)
			{
				RoleData roleData = Global.ClientToRoleData2(otherClient);
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(roleData, pool, cmd);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x001C71B0 File Offset: 0x001C53B0
		public void NotifySelfOtherData(SocketListener sl, TCPOutPacketPool pool, GameClient client, RoleDataEx roleDataEx, int cmd)
		{
			RoleData roleData = null;
			if (null != roleDataEx)
			{
				roleData = Global.RoleDataExToRoleData(roleDataEx);
			}
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(roleData, pool, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x001C71F4 File Offset: 0x001C53F4
		public void NotifyMyselfOtherLoadAlready(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int mapCode, long startMoveTicks, int currentX, int currentY, int currentDirection, int action, int toX, int toY, double moveCost = 1.0, int extAction = 0, int currentPathIndex = 0)
		{
			GameClient otherClient = this.FindClient(otherRoleID);
			LoadAlreadyData loadAlreadyData = new LoadAlreadyData
			{
				RoleID = otherRoleID,
				MapCode = mapCode,
				StartMoveTicks = startMoveTicks,
				CurrentX = currentX,
				CurrentY = currentY,
				CurrentDirection = currentDirection,
				Action = action,
				ToX = toX,
				ToY = toY,
				MoveCost = moveCost,
				ExtAction = extAction,
				PathString = ((otherClient != null) ? otherClient.ClientData.RolePathString : ""),
				CurrentPathIndex = currentPathIndex
			};
			byte[] bytes = DataHelper.ObjectToBytes<LoadAlreadyData>(loadAlreadyData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 209);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x001C72D0 File Offset: 0x001C54D0
		public void NotifyMyselfOtherMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int mapCode, int action, long startMoveTicks, int fromX, int fromY, int toX, int toY, int cmd, double moveCost = 1.0, int extAction = 0)
		{
			GameClient otherClient = this.FindClient(otherRoleID);
			byte[] bytes = DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
			{
				roleID = otherRoleID,
				mapCode = mapCode,
				action = action,
				toX = toX,
				toY = toY,
				moveCost = moveCost,
				extAction = extAction,
				fromX = fromX,
				fromY = fromY,
				startMoveTicks = startMoveTicks,
				pathString = ((otherClient != null) ? otherClient.ClientData.RolePathString : "")
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x001C7388 File Offset: 0x001C5588
		public void NotifyMyselfOthersMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.CurrentAction == 1 || (objsList[i] as GameClient).ClientData.CurrentAction == 2)
							{
								GameManager.ClientMgr.NotifyMyselfOtherMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (objsList[i] as GameClient).ClientData.RoleID, (objsList[i] as GameClient).ClientData.MapCode, (objsList[i] as GameClient).ClientData.CurrentAction, Global.GetClientStartMoveTicks(objsList[i] as GameClient), (objsList[i] as GameClient).ClientData.PosX, (objsList[i] as GameClient).ClientData.PosY, (int)(objsList[i] as GameClient).ClientData.DestPoint.X, (int)(objsList[i] as GameClient).ClientData.DestPoint.Y, 107, (objsList[i] as GameClient).ClientData.MoveSpeed, 0);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x001C7554 File Offset: 0x001C5754
		public void NotifyOthersMyMoving(SocketListener sl, TCPOutPacketPool pool, SpriteNotifyOtherMoveData moveData, GameClient client, int cmd, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				this.SendToClients(sl, pool, client, objsList, DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(moveData), cmd);
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x001C759C File Offset: 0x001C579C
		public void NotifyOthersMyMovingEnd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int mapCode, int action, int toX, int toY, int direction, int tryRun, bool sendToSelf, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				SCMoveEnd scData = new SCMoveEnd(client.ClientData.RoleID, mapCode, action, toX, toY, direction, tryRun, 0L);
				this.SendToClients<SCMoveEnd>(sl, pool, sendToSelf ? null : client, objsList, scData, 108);
			}
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x001C7604 File Offset: 0x001C5804
		public void NotifyOthersStopMyMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int stopIndex, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, stopIndex);
				this.SendToClients(sl, pool, client, objsList, strcmd, 411);
			}
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x001C766C File Offset: 0x001C586C
		public bool NotifyOthersToMoving(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, long startMoveTicks, int currentX, int currentY, int action, int toX, int toY, int cmd, double moveCost = 1.0, string pathString = "", List<object> objsList = null)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, currentX, currentY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
				{
					roleID = roleID,
					mapCode = mapCode,
					action = action,
					toX = toX,
					toY = toY,
					moveCost = moveCost,
					extAction = 0,
					fromX = currentX,
					fromY = currentY,
					startMoveTicks = startMoveTicks,
					pathString = pathString
				}), cmd);
				result = true;
			}
			return result;
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x001C7738 File Offset: 0x001C5938
		public void NotifyMyselfMonsterLoadAlready(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID, int mapCode, long startMoveTicks, int currentX, int currentY, int currentDirection, int action, int toX, int toY, double moveCost = 1.0, int extAction = 0, string pathString = "", int currentPathIndex = 0)
		{
			LoadAlreadyData loadAlreadyData = new LoadAlreadyData
			{
				RoleID = monsterID,
				MapCode = mapCode,
				StartMoveTicks = startMoveTicks,
				CurrentX = currentX,
				CurrentY = currentY,
				CurrentDirection = currentDirection,
				Action = action,
				ToX = toX,
				ToY = toY,
				MoveCost = moveCost,
				ExtAction = extAction,
				PathString = pathString,
				CurrentPathIndex = currentPathIndex
			};
			byte[] bytes = DataHelper.ObjectToBytes<LoadAlreadyData>(loadAlreadyData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 209);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x001C77E8 File Offset: 0x001C59E8
		public void NotifyMyselfMonsterMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID, int mapCode, int action, long startMoveTicks, int fromX, int fromY, int toX, int toY, int cmd, double moveCost = 1.0, int extAction = 0, string pathString = "")
		{
			byte[] bytes = DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
			{
				roleID = monsterID,
				mapCode = mapCode,
				action = action,
				toX = toX,
				toY = toY,
				moveCost = moveCost,
				extAction = extAction,
				fromX = fromX,
				fromY = fromY,
				startMoveTicks = startMoveTicks,
				pathString = pathString
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x001C7880 File Offset: 0x001C5A80
		public void NotifyMyselfMonstersMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if ((objsList[i] as Monster).SafeAction == GActions.Walk || (objsList[i] as Monster).SafeAction == GActions.Run)
						{
							Monster monster = objsList[i] as Monster;
							GameManager.ClientMgr.NotifyMyselfMonsterMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (objsList[i] as Monster).RoleID, (objsList[i] as Monster).MonsterZoneNode.MapCode, (int)(objsList[i] as Monster).SafeAction, Global.GetMonsterStartMoveTicks(objsList[i] as Monster), (int)(objsList[i] as Monster).SafeCoordinate.X, (int)(objsList[i] as Monster).SafeCoordinate.Y, (int)(objsList[i] as Monster).DestPoint.X, (int)(objsList[i] as Monster).DestPoint.Y, 107, (objsList[i] as Monster).MoveSpeed, 0, "");
						}
					}
				}
			}
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x001C7A14 File Offset: 0x001C5C14
		public void NotifyOthersMyAction(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int mapCode, int direction, int action, int x, int y, int targetX, int targetY, int yAngle, int moveToX, int moveToY, int cmd)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteActionData>(new SpriteActionData
				{
					roleID = roleID,
					mapCode = mapCode,
					direction = direction,
					action = action,
					toX = x,
					toY = y,
					targetX = targetX,
					targetY = targetY,
					yAngle = yAngle,
					moveToX = moveToX,
					moveToY = moveToY
				}), cmd);
			}
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x001C7AA8 File Offset: 0x001C5CA8
		public void NotifyOthersMyAction(SocketListener sl, TCPOutPacketPool pool, GameClient client, SpriteActionData cmdData, int cmd)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteActionData>(cmdData), cmd);
			}
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x001C7AE0 File Offset: 0x001C5CE0
		public void NotifyOthersDoAction(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int direction, int action, int x, int y, int targetX, int targetY, int cmd, List<object> objsList)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, x, y, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteActionData>(new SpriteActionData
				{
					roleID = roleID,
					mapCode = mapCode,
					direction = direction,
					action = action,
					toX = x,
					toY = y,
					targetX = targetX,
					targetY = targetY,
					yAngle = -1,
					moveToX = 0,
					moveToY = 0
				}), cmd);
			}
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x001C7BA4 File Offset: 0x001C5DA4
		public void NotifyOthersChangeAngle(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int direction, int yAngle, int cmd)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", roleID, direction, yAngle);
				this.SendToClients(sl, pool, null, objsList, strcmd, cmd);
			}
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x001C7BF4 File Offset: 0x001C5DF4
		public void NotifyOthersMagicCode(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int roleID, int mapCode, int magicCode, int cmd)
		{
			List<object> objsList = Global.GetAll9Clients(attacker);
			if (null != objsList)
			{
				this.SendToClients(sl, pool, attacker, objsList, DataHelper.ObjectToBytes<SpriteMagicCodeData>(new SpriteMagicCodeData
				{
					roleID = roleID,
					mapCode = mapCode,
					magicCode = magicCode
				}), cmd);
			}
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x001C7C48 File Offset: 0x001C5E48
		public void NotifySpriteHited(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int enemy, int enemyX, int enemyY, int magicCode)
		{
			List<object> objsList = Global.GetAll9Clients(attacker);
			if (null != objsList)
			{
				SpriteHitedData hitedData = new SpriteHitedData();
				hitedData.roleId = attacker.GetObjectID();
				hitedData.enemy = enemy;
				hitedData.magicCode = magicCode;
				if (enemy < 0)
				{
					hitedData.enemyX = enemyX;
					hitedData.enemyY = enemyY;
				}
				if (!GameManager.FlagEnableHideFlags || !GameManager.HideFlagsMapDict.ContainsKey(attacker.CurrentMapCode))
				{
					this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteHitedData>(hitedData), 155);
				}
				else
				{
					GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
					GameClient client = attacker as GameClient;
					if (null != client)
					{
						client.sendCmd<SpriteHitedData>(155, hitedData, false);
					}
					if (spriteType == GSpriteTypes.Other && (client != null || GameManager.FlagHideFlagsType == 0))
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, objsList, DataHelper.ObjectToBytes<SpriteHitedData>(hitedData), 155, 1, enemy);
					}
				}
				this.AddDelayDecoToMap(attacker, magicCode, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY);
			}
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x001C7D6C File Offset: 0x001C5F6C
		public void AddDelayDecoToMap(IObject attacker, int magicCode, int mapCode, int copyMapID, int posX, int posY)
		{
			SystemXmlItem systemMagic = null;
			if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemMagic))
			{
				if (systemMagic.GetIntValue("DelayDecoToMap", -1) > 0)
				{
					string magicTimes = systemMagic.GetStringValue("MagicTime");
					if (!string.IsNullOrEmpty(magicTimes))
					{
						string[] magicTimeFields = magicTimes.Split(new char[]
						{
							','
						});
						if (magicTimeFields.Length > 0)
						{
							SkillData skillData = null;
							if (attacker is GameClient)
							{
								skillData = Global.GetSkillDataByID(attacker as GameClient, magicCode);
							}
							int magicTimeIndex = (skillData == null) ? 0 : (skillData.SkillLevel - 1);
							magicTimeIndex = Math.Min(magicTimeIndex, magicTimeFields.Length - 1);
							int magicTime = Global.SafeConvertToInt32(magicTimeFields[magicTimeIndex]);
							if (magicTime > 0)
							{
								int delayDeco = systemMagic.GetIntValue("DelayDecoration", -1);
								if (delayDeco > 0)
								{
									if (1 == systemMagic.GetIntValue("DelayDecoToMap", -1))
									{
										GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
										Point centerGridXY = new Point((double)(posX / gameMap.MapGridWidth), (double)(posY / gameMap.MapGridHeight));
										List<Point> pts = new List<Point>();
										pts.Add(centerGridXY);
										pts.Add(new Point(centerGridXY.X, centerGridXY.Y - 1.0));
										pts.Add(new Point(centerGridXY.X + 1.0, centerGridXY.Y));
										pts.Add(new Point(centerGridXY.X, centerGridXY.Y + 1.0));
										pts.Add(new Point(centerGridXY.X - 1.0, centerGridXY.Y));
										for (int i = 0; i < pts.Count; i++)
										{
											if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)pts[i].X, (int)pts[i].Y))
											{
												Point pos = new Point(pts[i].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), pts[i].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
												DecorationManager.AddDecoToMap(mapCode, copyMapID, pos, delayDeco, magicTime * 1000, 2000, true);
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

		// Token: 0x0600216A RID: 8554 RVA: 0x001C803C File Offset: 0x001C623C
		public void NotifyOthersRealive(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int posX, int posY, int direction)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				MonsterRealiveData monsterRealiveData = new MonsterRealiveData
				{
					RoleID = roleID,
					PosX = posX,
					PosY = posY,
					Direction = direction
				};
				byte[] bytes = DataHelper.ObjectToBytes<MonsterRealiveData>(monsterRealiveData);
				this.SendToClients(sl, pool, client, objsList, bytes, 119);
				this.NotifyTeamRealive(sl, pool, roleID, posX, posY, direction);
			}
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x001C80B0 File Offset: 0x001C62B0
		public void NotifyTeamRealive(SocketListener sl, TCPOutPacketPool pool, int roleID, int posX, int posY, int direction)
		{
			GameClient otherClient = this.FindClient(roleID);
			if (null != otherClient)
			{
				if (otherClient.ClientData.TeamID > 0)
				{
					TeamData td = GameManager.TeamMgr.FindData(otherClient.ClientData.TeamID);
					if (null != td)
					{
						List<int> roleIDsList = new List<int>();
						lock (td)
						{
							for (int i = 0; i < td.TeamRoles.Count; i++)
							{
								if (roleID != td.TeamRoles[i].RoleID)
								{
									roleIDsList.Add(td.TeamRoles[i].RoleID);
								}
							}
						}
						TCPOutPacket tcpOutPacket = null;
						try
						{
							for (int i = 0; i < roleIDsList.Count; i++)
							{
								GameClient gc = this.FindClient(roleIDsList[i]);
								if (null != gc)
								{
									if (null == tcpOutPacket)
									{
										MonsterRealiveData monsterRealiveData = new MonsterRealiveData
										{
											RoleID = roleID,
											PosX = posX,
											PosY = posY,
											Direction = direction
										};
										byte[] bytes = DataHelper.ObjectToBytes<MonsterRealiveData>(monsterRealiveData);
										tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 119);
									}
									if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
									{
									}
								}
							}
						}
						finally
						{
							this.PushBackTcpOutPacket(tcpOutPacket);
						}
					}
				}
			}
		}

		// Token: 0x0600216C RID: 8556 RVA: 0x001C8278 File Offset: 0x001C6478
		public void NotifyMySelfRealive(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int posX, int posY, int direction)
		{
			MonsterRealiveData monsterRealiveData = new MonsterRealiveData
			{
				RoleID = roleID,
				PosX = posX,
				PosY = posY,
				Direction = direction
			};
			byte[] bytes = DataHelper.ObjectToBytes<MonsterRealiveData>(monsterRealiveData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 119);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600216D RID: 8557 RVA: 0x001C82DC File Offset: 0x001C64DC
		public void NotifyMonsterRealive(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int posX, int posY, int direction, List<object> objsList)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, posX, posY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				MonsterRealiveData monsterRealiveData = new MonsterRealiveData
				{
					RoleID = roleID,
					PosX = posX,
					PosY = posY,
					Direction = direction
				};
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<MonsterRealiveData>(monsterRealiveData), 119);
			}
		}

		// Token: 0x0600216E RID: 8558 RVA: 0x001C836C File Offset: 0x001C656C
		public void NotifyOthersLeave(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, 1);
				this.SendToClients(sl, pool, null, objsList, strcmd, 127);
			}
		}

		// Token: 0x0600216F RID: 8559 RVA: 0x001C83B8 File Offset: 0x001C65B8
		public void NotifyOthersMonsterLeave(SocketListener sl, TCPOutPacketPool pool, Monster monster, List<object> objsList)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", monster.RoleID, 2);
				this.SendToClients(sl, pool, null, objsList, strcmd, 127);
			}
		}

		// Token: 0x06002170 RID: 8560 RVA: 0x001C8400 File Offset: 0x001C6600
		public void NotifyMyselfLeaveOthers(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							string strcmd = string.Format("{0}:{1}", (objsList[i] as GameClient).ClientData.RoleID, 1);
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 127);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x06002171 RID: 8561 RVA: 0x001C84D0 File Offset: 0x001C66D0
		public void NotifyMyselfLeaveMonsters(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if (!this.NotifyMyselfLeaveMonsterByID(sl, pool, client, (objsList[i] as Monster).RoleID))
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x06002172 RID: 8562 RVA: 0x001C8540 File Offset: 0x001C6740
		public bool NotifyMyselfLeaveMonsterByID(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID)
		{
			string strcmd = string.Format("{0}:{1}", monsterID, 2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 127);
			return sl.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		// Token: 0x06002173 RID: 8563 RVA: 0x001C8590 File Offset: 0x001C6790
		public void JugeSpriteDead(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.CurrentLifeV <= 0)
			{
				GameManager.SystemServerEvents.AddEvent(string.Format("角色强制死亡, roleID={0}({1}), Life={2}", client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.CurrentLifeV), EventLevels.Debug);
				this.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, -1, client.ClientData.RoleID, 0, 0, 0.0, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
			}
		}

		// Token: 0x06002174 RID: 8564 RVA: 0x001C864C File Offset: 0x001C684C
		public void NotifySelfLifeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			byte[] cmdData = DataHelper.ObjectToBytes<SpriteLifeChangeData>(new SpriteLifeChangeData
			{
				roleID = client.ClientData.RoleID,
				lifeV = client.ClientData.LifeV,
				magicV = client.ClientData.MagicV,
				currentLifeV = client.ClientData.CurrentLifeV,
				currentMagicV = client.ClientData.CurrentMagicV
			});
			this.SendToClient(client, cmdData, 164);
		}

		// Token: 0x06002175 RID: 8565 RVA: 0x001C86CC File Offset: 0x001C68CC
		public bool NotifyOthersRelife(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int x, int y, int direction, double lifeV, double magicV, int cmd, List<object> objsList, int force = 0)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, x, y, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				SpriteRelifeData relifeData = new SpriteRelifeData();
				relifeData.roleID = roleID;
				relifeData.direction = direction;
				relifeData.lifeV = lifeV;
				relifeData.magicV = magicV;
				relifeData.force = force;
				if (!GameManager.FlagEnableHideFlags)
				{
					relifeData.x = x;
					relifeData.y = y;
				}
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteRelifeData>(relifeData), cmd);
				result = true;
			}
			return result;
		}

		// Token: 0x06002176 RID: 8566 RVA: 0x001C8784 File Offset: 0x001C6984
		public void UserFullLife(GameClient client, string reason, bool allSend = true)
		{
			RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, reason);
			relifeLog.hpModify = true;
			relifeLog.mpModify = true;
			relifeLog.oldHp = client.ClientData.CurrentLifeV;
			relifeLog.oldMp = client.ClientData.CurrentMagicV;
			client.ClientData.CurrentLifeV = client.ClientData.LifeV;
			client.ClientData.CurrentMagicV = client.ClientData.MagicV;
			relifeLog.newHp = client.ClientData.CurrentLifeV;
			relifeLog.newMp = client.ClientData.CurrentMagicV;
			SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, allSend, false, 7);
		}

		// Token: 0x06002177 RID: 8567 RVA: 0x001C886C File Offset: 0x001C6A6C
		public void NotifyOthersLifeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client, bool allSend = true, bool resetMax = false, int flags = 7)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
				client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
				if (!resetMax)
				{
					client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.CurrentLifeV, client.ClientData.LifeV);
					client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.CurrentMagicV, client.ClientData.MagicV);
					client.ClientData.CurrentArmorV = Global.GMin(client.ClientData.CurrentArmorV, client.ClientData.ArmorV);
				}
				else
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.CurrentArmorV = client.ClientData.ArmorV;
				}
				SpriteLifeChangeData lifeChangeData = new SpriteLifeChangeData();
				lifeChangeData.roleID = client.ClientData.RoleID;
				if ((flags & 1) != 0)
				{
					lifeChangeData.lifeV = client.ClientData.LifeV;
					lifeChangeData.currentLifeV = client.ClientData.CurrentLifeV;
				}
				if ((flags & 2) != 0)
				{
					lifeChangeData.magicV = client.ClientData.MagicV;
					lifeChangeData.currentMagicV = client.ClientData.CurrentMagicV;
				}
				if ((flags & 4) != 0)
				{
					lifeChangeData.ArmorV = (long)client.ClientData.ArmorV;
					lifeChangeData.currentArmorV = (long)client.ClientData.CurrentArmorV;
				}
				byte[] cmdData = DataHelper.ObjectToBytes<SpriteLifeChangeData>(lifeChangeData);
				if (!allSend)
				{
					if (null != client)
					{
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, 0, cmdData.Length, 164);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
				else
				{
					List<object> objsList = Global.GetAll9Clients(client);
					if (null != objsList)
					{
						for (int i = objsList.Count - 1; i >= 0; i--)
						{
							GameClient c = objsList[i] as GameClient;
							if (c != null && c != client)
							{
								if (c.ClientData.ChangeLifeCount >= Data.OpChangeLifeCount && c.ClientData.CurrentLifeV * 8 < c.ClientData.LifeV)
								{
									objsList.RemoveAt(i);
								}
							}
						}
						this.SendToClients(sl, pool, null, objsList, cmdData, 164);
					}
				}
			}
		}

		// Token: 0x06002178 RID: 8568 RVA: 0x001C8B30 File Offset: 0x001C6D30
		public void NotifyOthersGoBack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toPosX = -1, int toPosY = -1, int direction = -1)
		{
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							client.ClientData.MapCode,
							toPosX,
							toPosY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastChangeMapTicks = TimeUtil.NOW();
			int defaultBirthPosX = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosX;
			int defaultBirthPosY = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosY;
			int defaultBirthRadius = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].BirthRadius;
			int posX = toPosX;
			int posY = toPosY;
			if (-1 == posX || -1 == posY)
			{
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				posX = (int)newPos.X;
				posY = (int)newPos.Y;
			}
			if (direction >= 0)
			{
				client.ClientData.RoleDirection = direction;
			}
			GameManager.ClientMgr.ChangePosition(sl, pool, client, posX, posY, direction, 159, 0);
		}

		// Token: 0x06002179 RID: 8569 RVA: 0x001C8CF8 File Offset: 0x001C6EF8
		public void NotifyOthersChangeEquip(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int refreshNow, WingData usingWinData = null)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				ChangeEquipData changeEquipData = new ChangeEquipData
				{
					RoleID = client.ClientData.RoleID,
					EquipGoodsData = goodsData,
					UsingWinData = usingWinData
				};
				byte[] bytesData = DataHelper.ObjectToBytes<ChangeEquipData>(changeEquipData);
				this.SendToClients(sl, pool, null, objsList, bytesData, 137);
			}
		}

		// Token: 0x0600217A RID: 8570 RVA: 0x001C8D60 File Offset: 0x001C6F60
		public void NotifyOthersRebornEquipChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowEquip);
				GameManager.ClientMgr.SendToClients(sl, pool, null, objsList, strcmd, 2052);
			}
		}

		// Token: 0x0600217B RID: 8571 RVA: 0x001C8DC4 File Offset: 0x001C6FC4
		public void NotifyOthersRebornModelChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowModel);
				GameManager.ClientMgr.SendToClients(sl, pool, null, objsList, strcmd, 2061);
			}
		}

		// Token: 0x0600217C RID: 8572 RVA: 0x001C8E28 File Offset: 0x001C7028
		public void NotifyOthersPKModeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.PKMode);
				this.SendToClients(sl, pool, null, objsList, strcmd, 149);
			}
		}

		// Token: 0x0600217D RID: 8573 RVA: 0x001C8E88 File Offset: 0x001C7088
		public void Logout(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			try
			{
				GameManager.systemGMCommands.OnClientLogout(client);
				GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutEventObject(client));
				client.TimedActionMgr.RemoveItem(0);
				Global.SystemKillSummonMonster(client, -1);
				ChengJiuManager.SaveKilledMonsterNumToDB(client, true);
				OnePieceManager.getInstance().HandleRoleLogout(client);
				if (null != client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping.StopSweeping();
				}
				Global.ProcessDBCmdByTicks(client, true);
				Global.ProcessDBSkillCmdByTicks(client, true);
				Global.ProcessDBRoleParamCmdByTicks(client, true);
				Global.ProcessDBEquipStrongCmdByTicks(client, true);
				Global.UpdateAllDBBufferData(client);
				Global.UpdateHuoDongDBCommand(pool, client);
				GameManager.BattleMgr.LeaveBattleMap(client, true);
				GameManager.ArenaBattleMgr.LeaveArenaBattleMap(client);
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				KuaFuManager.getInstance().OnLeaveScene(client, sceneType, false);
				KuaFuManager.getInstance().OnLogout(client);
				if (!client.ClientSocket.IsKuaFuLogin && client.ClientData.SaleGoodsDataList.Count > 0)
				{
					SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
					SaleGoodsManager.RemoveSaleGoodsItems(client);
					this.SaleGoodsToOfflineSale(client);
				}
				long nowTicks = TimeUtil.NOW();
				Global.ResetMeditateTime(client, nowTicks, false);
				GameManager.DBCmdMgr.AddDBCmd(10032, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TotalOnlineSecs, client.ClientData.AntiAddictionSecs), null, client.ServerId);
				this.RemoveClient(client);
				if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
				{
					GameManager.BloodCastleCopySceneMgr.LogOutWhenInBloodCastleCopyScene(client);
				}
				if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
				{
					GameManager.DaimonSquareCopySceneMgr.LogOutWhenInDaimonSquareCopyMap(client);
				}
				if (client.ClientData.IsFlashPlayer == 1)
				{
					Global.ProcessLogOutWhenINFreshPlayerStatus(client);
				}
				if (client.ClientData.bIsInAngelTempleMap)
				{
					GameManager.AngelTempleMgr.LeaveAngelTempleScene(client, true);
				}
				List<object> objsList = Global.GetAll9GridObjects(client);
				Global.GameClientHandleOldObjs(client, objsList);
				client.ClearVisibleObjects(true);
				Global.ClearCopyMap(client, true);
				GameManager.GoodsPackMgr.UnLockGoodsPackItem(client);
				Global.QuitFromTeam(client);
				if (client.ClientData.DJRoomID > 0)
				{
					if (client.ClientData.DJRoomTeamID > 0)
					{
						if (GameManager.ClientMgr.DestroyDianJiangRoom(sl, pool, client) < 0)
						{
							GameManager.ClientMgr.LeaveDianJiangRoom(sl, pool, client);
						}
						if (MapTypes.DianJiangCopy == Global.GetMapType(client.ClientData.MapCode))
						{
							GameManager.DJRoomMgr.SetRoomRolesDataRoleState(client.ClientData.DJRoomID, client.ClientData.RoleID, 2);
						}
					}
					else
					{
						this.ViewerLeaveDianJiangRoom(sl, pool, client);
					}
					client.ClientData.DJRoomID = -1;
					client.ClientData.DJRoomTeamID = -1;
					client.ClientData.HideSelf = 0;
				}
				RoleName2IDs.RemoveRoleName(Global.FormatRoleName(client, client.ClientData.RoleName));
				this.ProcessExchangeData(sl, pool, client);
				if (client.ClientData.CurrentLifeV <= 0)
				{
					client.ClientData.MapCode = -1;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
					client.ClientData.ReportPosTicks = 0L;
				}
				if (sceneType == SceneUIClasses.ShuiJingHuanJing)
				{
					client.ClientData.MapCode = -1;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
					client.ClientData.ReportPosTicks = 0L;
				}
				if (Global.CanRecordPos(client))
				{
					GameManager.DBCmdMgr.AddDBCmd(10001, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.MapCode,
						client.ClientData.RoleDirection,
						client.ClientData.PosX,
						client.ClientData.PosY
					}), null, client.ServerId);
				}
				GameManager.DBCmdMgr.AddDBCmd(10017, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					GameManager.ServerLineID,
					Global.GetSocketRemoteIP(client, false),
					client.ClientData.OnlineActiveVal,
					TimeUtil.NOW()
				}), null, client.ServerId);
				EventLogManager.AddRoleLogoutEvent(client);
				Global.UpdateRoleParamsInfo(client);
				Global.UpdateRoleDayActivityInfo(client, 0);
				DailyActiveManager.SaveRoleDailyActiveData(client);
				ChengJiuManager.SaveRoleChengJiuData(client);
				MarryLogic.ApplyLogoutClear(client);
				RobotTaskValidator.getInstance().RobotDataReset(client);
				GameManager.loginWaitLogic.AddToAllow(client.strUserID, GameManager.loginWaitLogic.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.LogouAllowMSeconds));
				SecondPasswordManager.OnUsrLogout(client.strUserID);
				SingletonTemplate<SpeedUpTickCheck>.Instance().OnLogout(client);
				SingletonTemplate<CoupleArenaManager>.Instance().OnClientLogout(client);
				try
				{
					string ip = RobotTaskValidator.getInstance().GetIp(client);
					string analysisLog = string.Format("logout server={0} account={1} player={2} dev_id={3} exp={4}", new object[]
					{
						GameManager.ServerId,
						client.strUserID,
						client.ClientData.RoleID,
						string.IsNullOrEmpty(client.deviceID) ? "" : client.deviceID,
						ip
					});
					LogManager.WriteLog(LogTypes.Analysis, analysisLog, null, true);
				}
				catch
				{
				}
				client.LogoutState = true;
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutFinishEventObject(client));
				GlobalEventSource.getInstance().fireEvent(new PlayerLeaveFuBenEventObject(client));
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			finally
			{
				client.ClientData.SceneType = SceneUIClasses.UnDefined;
				client.ClientData.SceneMapCode = 0;
			}
		}

		// Token: 0x0600217E RID: 8574 RVA: 0x001C9560 File Offset: 0x001C7760
		private void SaleGoodsToOfflineSale(GameClient client)
		{
			if (client.ClientSocket.ClientKuaFuServerLoginData.RoleId <= 0 && !client.CheckCheatData.IsKickedRole)
			{
				LiXianBaiTanManager.AddLiXianSaleGoodsItems(client, -1);
			}
		}

		// Token: 0x0600217F RID: 8575 RVA: 0x001C95A4 File Offset: 0x001C77A4
		private void ProcessFakeRoleForLiXianGuaJi(GameClient client)
		{
			if (client.ClientSocket.ClientKuaFuServerLoginData.RoleId <= 0 && !client.CheckCheatData.IsKickedRole)
			{
				int fakeRoleID = 0;
				if (GameManager.FlagLiXianGuaJi > 0)
				{
					fakeRoleID = FakeRoleManager.ProcessNewFakeRole(client.ClientData, client.ClientData.MapCode, FakeRoleTypes.LiXianGuaJi, -1, client.ClientData.PosX, client.ClientData.PosY, 0);
				}
				LiXianGuaJiManager.AddLiXianGuaJiRole(client, fakeRoleID);
			}
		}

		// Token: 0x06002180 RID: 8576 RVA: 0x001C9628 File Offset: 0x001C7828
		public void NotifyUpdateTask(SocketListener sl, TCPOutPacketPool pool, GameClient client, int dbID, int taskID, int taskVal1, int taskVal2, int taskFocus, long chengjiuValue)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				dbID,
				taskID,
				taskVal1,
				taskVal2,
				taskFocus,
				chengjiuValue
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 139);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002181 RID: 8577 RVA: 0x001C96AC File Offset: 0x001C78AC
		public void NotifyUpdateNPCTaskSate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int state)
		{
			string strcmd = string.Format("{0}:{1}", npcID, state);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 151);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002182 RID: 8578 RVA: 0x001C96FC File Offset: 0x001C78FC
		public void NotifyNPCTaskStateList(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<NPCTaskState> npcTaskStatList)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<NPCTaskState>>(npcTaskStatList, pool, 152);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002183 RID: 8579 RVA: 0x001C972C File Offset: 0x001C792C
		public bool GiveFirstTask(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, TCPRandKey tcpRandKey, GameClient client, bool bNeedTakeStartTask)
		{
			bool result;
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，无法给与新手任务", new object[0]), null, true);
				result = false;
			}
			else
			{
				int nRoleID = client.ClientData.RoleID;
				try
				{
					if (Global.GetTaskData(client, MagicSwordData.InitTaskID) == null && GameManager.MagicSwordMgr.IsFirstLoginMagicSword(client, MagicSwordData.InitChangeLifeCount))
					{
						int tmpRes = GameManager.ClientMgr.AutoCompletionTaskByTaskID(tcpMgr, tcpClientPool, pool, tcpRandKey, client, MagicSwordData.InitPrevTaskID);
						if (tmpRes != 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("魔剑士任务初始化失败，无法创建魔剑士, RoleID={0}", nRoleID), null, true);
							return false;
						}
						client.ClientData.MainTaskID = MagicSwordData.InitPrevTaskID;
						client.ClientData.MapCode = MagicSwordData.InitMapID;
						TCPOutPacket tcpOutPacketTemp = null;
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, nRoleID, MagicSwordData.InitTaskID, MagicSwordData.InitTaskNpcID, out tcpOutPacketTemp);
					}
					else if (Global.GetTaskData(client, SummonerData.InitTaskID) == null && GameManager.SummonerMgr.IsFirstLoginSummoner(client, SummonerData.InitChangeLifeCount))
					{
						int tmpRes = GameManager.ClientMgr.AutoCompletionTaskByTaskID(tcpMgr, tcpClientPool, pool, tcpRandKey, client, SummonerData.InitPrevTaskID);
						if (tmpRes != 0)
						{
							LogManager.WriteLog(LogTypes.Error, string.Format("召唤师任务初始化失败，无法创建召唤师, RoleID={0}", nRoleID), null, true);
							return false;
						}
						client.ClientData.MainTaskID = SummonerData.InitPrevTaskID;
						client.ClientData.MapCode = SummonerData.InitMapID;
						TCPOutPacket tcpOutPacketTemp = null;
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, nRoleID, SummonerData.InitTaskID, SummonerData.InitTaskNpcID, out tcpOutPacketTemp);
					}
					else if (bNeedTakeStartTask && Global.GetTaskData(client, 1000) == null && !GameManager.MagicSwordMgr.IsMagicSword(client))
					{
						client.ClientData.MainTaskID = 106;
						TCPOutPacket tcpOutPacketTemp = null;
						Global.AddOldTask(client, 106);
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, nRoleID, 1000, 60900, out tcpOutPacketTemp);
					}
					return true;
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06002184 RID: 8580 RVA: 0x001C9998 File Offset: 0x001C7B98
		public EquipPropsData GetEquipPropsStr(GameClient client)
		{
			client.propsCacheModule.ResetAllProps();
			AdvanceBufferPropsMgr.DoSpriteBuffers(client);
			double nMinAttack = RoleAlgorithm.GetMinAttackV(client);
			double nMaxAttack = RoleAlgorithm.GetMaxAttackV(client);
			double nMinDefense = RoleAlgorithm.GetMinADefenseV(client);
			double nMaxDefense = RoleAlgorithm.GetMaxADefenseV(client);
			double nMinMAttack = RoleAlgorithm.GetMinMagicAttackV(client);
			double nMaxMAttack = RoleAlgorithm.GetMaxMagicAttackV(client);
			double nMinMDefense = RoleAlgorithm.GetMinMDefenseV(client);
			double nMaxMDefense = RoleAlgorithm.GetMaxMDefenseV(client);
			double nHit = RoleAlgorithm.GetHitV(client);
			double nDodge = RoleAlgorithm.GetDodgeV(client);
			double addAttackInjure = RoleAlgorithm.GetAddAttackInjureValue(client);
			double decreaseInjure = RoleAlgorithm.GetDecreaseInjureValue(client);
			double nMaxHP = RoleAlgorithm.GetMaxLifeV(client);
			double nMaxMP = RoleAlgorithm.GetMaxMagicV(client);
			double nLifeSteal = RoleAlgorithm.GetLifeStealV(client);
			double dFireAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Fire);
			double dWaterAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Water);
			double dLightningAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Lightning);
			double dSoilAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Soil);
			double dIceAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Ice);
			double dWindAttack = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Wind);
			double HolyAttack = RoleAlgorithm.GetExtProp(client, 122);
			double HolyDefense = RoleAlgorithm.GetExtProp(client, 123);
			double ShadowAttack = RoleAlgorithm.GetExtProp(client, 129);
			double ShadowDefense = RoleAlgorithm.GetExtProp(client, 130);
			double NatureAttack = RoleAlgorithm.GetExtProp(client, 136);
			double NatureDefense = RoleAlgorithm.GetExtProp(client, 137);
			double ChaosAttack = RoleAlgorithm.GetExtProp(client, 143);
			double ChaosDefense = RoleAlgorithm.GetExtProp(client, 144);
			double IncubusAttack = RoleAlgorithm.GetExtProp(client, 150);
			double IncubusDefense = RoleAlgorithm.GetExtProp(client, 151);
			CombatForceInfo CombatForce = Data.CombatForceDataInfo[1];
			int armorMax = (int)RoleAlgorithm.GetExtProp(client, 119);
			double toughness = RoleAlgorithm.GetExtProp(client, 101);
			if (CombatForce != null)
			{
				double nValue = (nMinAttack / CombatForce.MinPhysicsAttackModulus + nMaxAttack / CombatForce.MaxPhysicsAttackModulus) / 2.0 + (nMinDefense / CombatForce.MinPhysicsDefenseModulus + nMaxDefense / CombatForce.MaxPhysicsDefenseModulus) / 2.0 + (nMinMAttack / CombatForce.MinMagicAttackModulus + nMaxMAttack / CombatForce.MaxMagicAttackModulus) / 2.0 + (nMinMDefense / CombatForce.MinMagicDefenseModulus + nMaxMDefense / CombatForce.MaxMagicDefenseModulus) / 2.0 + addAttackInjure / CombatForce.AddAttackInjureModulus + decreaseInjure / CombatForce.DecreaseInjureModulus + nHit / CombatForce.HitValueModulus + nDodge / CombatForce.DodgeModulus + nMaxHP / CombatForce.MaxHPModulus + nMaxMP / CombatForce.MaxMPModulus + nLifeSteal / CombatForce.LifeStealModulus + (double)armorMax / CombatForce.ArmorMax;
				nValue += dFireAttack / CombatForce.FireAttack + dWaterAttack / CombatForce.WaterAttack + dLightningAttack / CombatForce.LightningAttack + dSoilAttack / CombatForce.SoilAttack + dIceAttack / CombatForce.IceAttack + dWindAttack / CombatForce.WindAttack;
				nValue += HolyAttack / CombatForce.HolyAttack + HolyDefense / CombatForce.HolyDefense + ShadowAttack / CombatForce.ShadowAttack + ShadowDefense / CombatForce.ShadowDefense + NatureAttack / CombatForce.NatureAttack + NatureDefense / CombatForce.NatureDefense + ChaosAttack / CombatForce.ChaosAttack + ChaosDefense / CombatForce.ChaosDefense + IncubusAttack / CombatForce.IncubusAttack + IncubusDefense / CombatForce.IncubusDefense;
				client.ClientData.CombatForce = (int)nValue;
				HuodongCachingMgr.ProcessCombatGift(client, false);
			}
			RebornManager.getInstance().CalculateCombatForce(client);
			int nStrength = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
			int nIntelligence = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
			int nDexterity = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
			int nConstitution = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
			int addStrength = (int)RoleAlgorithm.GetStrength(client, false);
			int addIntelligence = (int)RoleAlgorithm.GetIntelligence(client, false);
			int addDexterity = (int)RoleAlgorithm.GetDexterity(client, false);
			int addConstitution = (int)RoleAlgorithm.GetConstitution(client, false);
			int addAll = addStrength + addIntelligence + addDexterity + addConstitution;
			EquipPropsData equipPropsData = new EquipPropsData
			{
				RoleID = client.ClientData.RoleID,
				Strength = (double)(addStrength + nStrength),
				Intelligence = (double)(addIntelligence + nIntelligence),
				Dexterity = (double)(addDexterity + nDexterity),
				Constitution = (double)(addConstitution + nConstitution),
				MinAttack = nMinAttack,
				MaxAttack = nMaxAttack,
				MinDefense = nMinDefense,
				MaxDefense = nMaxDefense,
				MagicSkillIncrease = RoleAlgorithm.GetMagicSkillIncrease(client),
				MinMAttack = nMinMAttack,
				MaxMAttack = nMaxMAttack,
				MinMDefense = nMinMDefense,
				MaxMDefense = nMaxMDefense,
				PhySkillIncrease = RoleAlgorithm.GetPhySkillIncrease(client),
				MaxHP = nMaxHP,
				MaxMP = nMaxMP,
				AttackSpeed = RoleAlgorithm.GetAttackSpeed(client),
				Hit = nHit,
				Dodge = nDodge,
				TotalPropPoint = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint"),
				ChangeLifeCount = client.ClientData.ChangeLifeCount,
				CombatForce = client.ClientData.CombatForce,
				TEMPStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless"),
				TEMPIntelligsence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless"),
				TEMPDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless"),
				TEMPConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless"),
				Toughness = toughness,
				ArmorMax = armorMax,
				RebornCombatForce = client.ClientData.RebornCombatForce
			};
			equipPropsData.TotalPropPoint += nStrength + nIntelligence + nDexterity + nConstitution;
			equipPropsData.TotalPropPoint += (int)client.ClientData.PropsCacheManager.GetBaseProp(0) + (int)client.ClientData.PropsCacheManager.GetBaseProp(1) + (int)client.ClientData.PropsCacheManager.GetBaseProp(2) + (int)client.ClientData.PropsCacheManager.GetBaseProp(3);
			Global.CalcAttackType(client);
			if (client.ClientData.Occupation == 3)
			{
				int subOccupation = (client.ClientData.AttackType == 0) ? 1 : 2;
				if (subOccupation != client.ClientData.SubOccupation)
				{
					client.ClientData.SubOccupation = subOccupation;
					Global.SaveRoleParamsInt32ValueToDB(client, "10213", subOccupation, true);
					string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, subOccupation);
					client.sendOthersCmd(1270, strcmd);
				}
				client.ClientData.OccupationIndex = client.ClientData.Occupation + subOccupation - 1;
			}
			else
			{
				client.ClientData.OccupationIndex = client.ClientData.Occupation;
			}
			return equipPropsData;
		}

		// Token: 0x06002185 RID: 8581 RVA: 0x001CA020 File Offset: 0x001C8220
		public void NotifyUpdateEquipProps(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				EquipPropsData equipPropsData = this.GetEquipPropsStr(client);
				byte[] bytes = DataHelper.ObjectToBytes<EquipPropsData>(equipPropsData);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 126);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
				if (client.ClientData.CombatForce != client.ClientData.LastNotifyCombatForce)
				{
					client.ClientData.LastNotifyCombatForce = client.ClientData.CombatForce;
					this.NotifyTeamCHGZhanLi(sl, pool, client);
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CombatChange));
				}
			}
		}

		// Token: 0x06002186 RID: 8582 RVA: 0x001CA0C8 File Offset: 0x001C82C8
		public void NotifyUpdateEscapeBattleProps(SocketListener sl, TCPOutPacketPool pool, GameClient client, EscapeBattlePropNotify ebProp)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				byte[] bytes = DataHelper.ObjectToBytes<EscapeBattlePropNotify>(ebProp);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 2121);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x06002187 RID: 8583 RVA: 0x001CA117 File Offset: 0x001C8317
		public void NotifyUpdateEquipProps(GameClient client)
		{
			this.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
		}

		// Token: 0x06002188 RID: 8584 RVA: 0x001CA136 File Offset: 0x001C8336
		public void NotifyUpdateWeights(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
		}

		// Token: 0x06002189 RID: 8585 RVA: 0x001CA13C File Offset: 0x001C833C
		public void NotifyUpdateEquipProps(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			EquipPropsData equipPropsData = this.GetEquipPropsStr(otherClient);
			byte[] bytes = DataHelper.ObjectToBytes<EquipPropsData>(equipPropsData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 126);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600218A RID: 8586 RVA: 0x001CA17C File Offset: 0x001C837C
		public void AddSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double lifeV, string reason)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				if (c.ClientData.CurrentLifeV < c.ClientData.LifeV)
				{
					RoleRelifeLog relifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, reason);
					relifeLog.hpModify = true;
					relifeLog.oldHp = c.ClientData.CurrentLifeV;
					c.ClientData.CurrentLifeV = (int)Global.GMin((double)c.ClientData.LifeV, (double)c.ClientData.CurrentLifeV + lifeV);
					relifeLog.newHp = c.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
					List<object> listObjs = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, listObjs, 0);
				}
			}
		}

		// Token: 0x0600218B RID: 8587 RVA: 0x001CA2C4 File Offset: 0x001C84C4
		public void SubSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double lifeV)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				c.ClientData.CurrentLifeV = (int)Global.GMax(0.0, (double)c.ClientData.CurrentLifeV - lifeV);
				List<object> listObjs = Global.GetAll9Clients(c);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, listObjs, 0);
			}
		}

		// Token: 0x0600218C RID: 8588 RVA: 0x001CA388 File Offset: 0x001C8588
		public void AddSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double magicV, string reason)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				if (c.ClientData.CurrentMagicV < c.ClientData.MagicV)
				{
					RoleRelifeLog relifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, reason);
					relifeLog.mpModify = true;
					relifeLog.oldMp = c.ClientData.CurrentMagicV;
					c.ClientData.CurrentMagicV = (int)Global.GMin((double)c.ClientData.MagicV, (double)c.ClientData.CurrentMagicV + magicV);
					relifeLog.newMp = c.ClientData.CurrentMagicV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
					List<object> listObjs = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, listObjs, 0);
				}
			}
		}

		// Token: 0x0600218D RID: 8589 RVA: 0x001CA4D0 File Offset: 0x001C86D0
		public void SubSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double magicV)
		{
			if (c.ClientData.IsFlashPlayer != 1 || c.ClientData.MapCode != 6090)
			{
				if (c.ClientData.CurrentLifeV > 0)
				{
					c.ClientData.CurrentMagicV = (int)Global.GMax(0.0, (double)c.ClientData.CurrentMagicV - magicV);
					List<object> listObjs = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, listObjs, 0);
				}
			}
		}

		// Token: 0x0600218E RID: 8590 RVA: 0x001CA5C4 File Offset: 0x001C87C4
		public void NotifyPetCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int petType, int extTag1, string extTag2, List<object> objsList)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				petType,
				extTag1,
				extTag2
			});
			if (null == objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 184);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, strcmd, 184);
			}
		}

		// Token: 0x0600218F RID: 8591 RVA: 0x001CA664 File Offset: 0x001C8864
		public void RemoveRolePet(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, bool notifySelf)
		{
			if (client.ClientData.PetDbID > 0 && client.ClientData.PetRoleID > 0)
			{
				PetData petData = Global.GetPetDataByDbID(client, client.ClientData.PetDbID);
				if (null != petData)
				{
					GameManager.ClientMgr.NotifyPetCmd(sl, pool, client, 0, 2, client.ClientData.PetRoleID, "", objsList);
				}
			}
		}

		// Token: 0x06002190 RID: 8592 RVA: 0x001CA6DC File Offset: 0x001C88DC
		public void NotifySelfPetShow(GameClient client)
		{
			if (client.ClientData.PetDbID > 0)
			{
				PetData petData = Global.GetPetDataByDbID(client, client.ClientData.PetDbID);
				if (null != petData)
				{
					if (client.ClientData.PetRoleID <= 0)
					{
						if (!Global.IsPetDead(petData))
						{
							client.ClientData.PetRoleID = (int)GameManager.PetIDMgr.GetNewID();
							Point pos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, 150);
							client.ClientData.PetPosX = (int)pos.X;
							client.ClientData.PetPosY = (int)pos.Y;
							client.ClientData.ReportPetPosTicks = 0L;
						}
					}
					if (client.ClientData.PetRoleID > 0)
					{
						double direction = Global.GetDirectionByTan((double)client.ClientData.PosX, (double)client.ClientData.PosY, (double)client.ClientData.PetPosX, (double)client.ClientData.PetPosY);
						string petInfo = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
						{
							client.ClientData.PetRoleID,
							petData.PetName,
							petData.Level,
							petData.PetID,
							client.ClientData.PetPosX,
							client.ClientData.PetPosY,
							(int)direction
						});
						GameManager.ClientMgr.NotifyPetCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, petData.DbID, petInfo, null);
					}
				}
			}
		}

		// Token: 0x06002191 RID: 8593 RVA: 0x001CA8C8 File Offset: 0x001C8AC8
		public void NotifyOthersMyPetHide(GameClient client)
		{
			if (client.ClientData.PetRoleID > 0)
			{
				List<object> objsList = Global.GetAll9Clients(client);
				GameManager.ClientMgr.NotifyPetCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 2, client.ClientData.PetRoleID, "", objsList);
			}
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x001CA928 File Offset: 0x001C8B28
		public void NotifyMySelfOnlineOtherPet(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (client.ClientData.RoleID != otherClient.ClientData.RoleID)
				{
					if (otherClient.ClientData.PetDbID > 0 && otherClient.ClientData.PetRoleID > 0)
					{
						PetData petData = Global.GetPetDataByDbID(otherClient, otherClient.ClientData.PetDbID);
						if (null != petData)
						{
							Point pos = new Point((double)otherClient.ClientData.PetPosX, (double)otherClient.ClientData.PetPosY);
							double direction = Global.GetDirectionByTan((double)otherClient.ClientData.PosX, (double)otherClient.ClientData.PosY, pos.X, pos.Y);
							string petInfo = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
							{
								otherClient.ClientData.PetRoleID,
								petData.PetName,
								petData.Level,
								petData.PetID,
								(int)pos.X,
								(int)pos.Y,
								(int)direction
							});
							string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								otherClient.ClientData.RoleID,
								1,
								otherClient.ClientData.PetRoleID,
								petInfo
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 184);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x001CAB18 File Offset: 0x001C8D18
		public void NotifyMySelfOnlineOtherPets(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.PetDbID > 0 && (objsList[i] as GameClient).ClientData.PetRoleID > 0)
							{
								PetData petData = Global.GetPetDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.PetDbID);
								if (null != petData)
								{
									Point pos = new Point((double)(objsList[i] as GameClient).ClientData.PetPosX, (double)(objsList[i] as GameClient).ClientData.PetPosY);
									double direction = Global.GetDirectionByTan((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY, pos.X, pos.Y);
									string petInfo = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
									{
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petData.PetName,
										petData.Level,
										petData.PetID,
										(int)pos.X,
										(int)pos.Y,
										(int)direction
									});
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										1,
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petInfo
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 184);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x001CADCC File Offset: 0x001C8FCC
		public void NotifySelfPetsOfflines(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.PetDbID > 0 && (objsList[i] as GameClient).ClientData.PetRoleID > 0)
							{
								PetData petData = Global.GetPetDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.PetDbID);
								if (null != petData)
								{
									Point pos = new Point((double)(objsList[i] as GameClient).ClientData.PetPosX, (double)(objsList[i] as GameClient).ClientData.PetPosY);
									double direction = Global.GetDirectionByTan((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY, pos.X, pos.Y);
									string petInfo = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
									{
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petData.PetName,
										petData.Level,
										petData.PetID,
										(int)pos.X,
										(int)pos.Y,
										(int)direction
									});
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										2,
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petInfo
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 184);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x001CB080 File Offset: 0x001C9280
		public void NotifyHorseCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int horseType, int horseDbID, int horseID, int horseBodyID, List<object> objsList)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				status,
				client.ClientData.RoleID,
				horseType,
				horseDbID,
				horseID,
				horseBodyID
			});
			if (null == objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 183);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, strcmd, 183);
			}
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x001CB130 File Offset: 0x001C9330
		public void NotifySelfOnHorse(GameClient client)
		{
			if (client.ClientData.HorseDbID > 0)
			{
				HorseData horseData = Global.GetHorseDataByDbID(client, client.ClientData.HorseDbID);
				if (null != horseData)
				{
					client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseData);
					GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseData.DbID, horseData.HorseID, horseData.BodyID, null);
				}
			}
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x001CB1B8 File Offset: 0x001C93B8
		public void NotifySelfOtherHorse(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (client.ClientData.RoleID != otherClient.ClientData.RoleID)
				{
					if (otherClient.ClientData.HorseDbID > 0)
					{
						HorseData horseData = Global.GetHorseDataByDbID(otherClient, otherClient.ClientData.HorseDbID);
						if (null != horseData)
						{
							string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								0,
								otherClient.ClientData.RoleID,
								1,
								horseData.DbID,
								horseData.HorseID,
								horseData.BodyID
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 183);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x001CB2CC File Offset: 0x001C94CC
		public void NotifySelfOthersHorse(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.HorseDbID > 0)
							{
								HorseData horseData = Global.GetHorseDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.HorseDbID);
								if (null != horseData)
								{
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										1,
										horseData.DbID,
										horseData.HorseID,
										horseData.BodyID
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 183);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x001CB45C File Offset: 0x001C965C
		public void JugeTempHorseID(GameClient client)
		{
			if (client.ClientData.StartTempHorseIDTicks > 0L)
			{
				if (client.ClientData.TempHorseID > 0)
				{
					long ticks = TimeUtil.NOW();
					if (ticks - client.ClientData.StartTempHorseIDTicks >= 180000L)
					{
						int tempHorseID = client.ClientData.TempHorseID;
						client.ClientData.StartTempHorseIDTicks = 0L;
						client.ClientData.TempHorseID = 0;
						if (client.ClientData.HorseDbID > 0)
						{
							HorseData horseData = Global.GetHorseDataByDbID(client, client.ClientData.HorseDbID);
							if (null != horseData)
							{
								string horseName = Global.GetHorseNameByID(tempHorseID);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(52, new object[0]), new object[]
								{
									horseName
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								List<object> objsList = Global.GetAll9Clients(client);
								if (null != objsList)
								{
									GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseData.DbID, horseData.HorseID, horseData.BodyID, objsList);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x001CB5C8 File Offset: 0x001C97C8
		public void NotifyJingMaiListCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			TCPOutPacket tcpOutPacket = null;
			List<JingMaiData> jingMaiDataList = client.ClientData.JingMaiDataList;
			if (null != jingMaiDataList)
			{
				lock (jingMaiDataList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 206);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 206);
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x001CB654 File Offset: 0x001C9854
		public void NotifyJingMaiInfoCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			TCPOutPacket tcpOutPacket = null;
			Dictionary<string, int> jingMaiDataDict = client.ClientData.JingMaiPropsDict;
			if (null != jingMaiDataDict)
			{
				lock (jingMaiDataDict)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, int>>(jingMaiDataDict, pool, 218);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, int>>(jingMaiDataDict, pool, 218);
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x001CB6E0 File Offset: 0x001C98E0
		public void NotifyOtherJingMaiListCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID)
		{
			TCPOutPacket tcpOutPacket = null;
			GameClient otherClient = this.FindClient(otherRoleID);
			if (null != otherClient)
			{
				List<JingMaiData> jingMaiDataList = otherClient.ClientData.JingMaiDataList;
				if (null != jingMaiDataList)
				{
					lock (jingMaiDataList)
					{
						tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 208);
					}
				}
				else
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 208);
				}
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600219D RID: 8605 RVA: 0x001CB78C File Offset: 0x001C998C
		public void NotifyEndChongXueCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}", client.ClientData.RoleID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 280);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600219E RID: 8606 RVA: 0x001CB7D8 File Offset: 0x001C99D8
		public void NotifyJingMaiResult(GameClient client, int retCode, int jingMaiID, int jingMaiLevel)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				retCode,
				client.ClientData.JingMaiBodyLevel,
				jingMaiID,
				jingMaiLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 207);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600219F RID: 8607 RVA: 0x001CB870 File Offset: 0x001C9A70
		public bool RemoveOldestEnemy(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client)
		{
			int totalCount = Global.GetFriendCountByType(client, 2);
			bool result;
			if (totalCount < 20)
			{
				result = true;
			}
			else
			{
				FriendData friendData = Global.FindFirstFriendDataByType(client, 2);
				result = (null == friendData || GameManager.ClientMgr.RemoveFriend(tcpMgr, tcpClientPool, pool, client, friendData.DbID));
			}
			return result;
		}

		// Token: 0x060021A0 RID: 8608 RVA: 0x001CB8CC File Offset: 0x001C9ACC
		public void AddToEnemyList(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int killedRoleID)
		{
			if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
			{
				GameClient findClient = this.FindClient(killedRoleID);
				if (null != findClient)
				{
					if (this.RemoveOldestEnemy(tcpMgr, tcpClientPool, pool, findClient))
					{
						int friendDbID = -1;
						FriendData friendData = Global.FindFriendData(findClient, client.ClientData.RoleID);
						if (null != friendData)
						{
							friendDbID = friendData.DbID;
						}
						int enemyCount = Global.GetFriendCountByType(findClient, 2);
						if (enemyCount >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, findClient, StringUtil.substitute(GLang.GetLang(53, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (friendData == null || (friendData.FriendType != 0 && friendData.FriendType != 2))
						{
							this.AddFriend(tcpMgr, tcpClientPool, pool, findClient, friendDbID, client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), 2);
						}
					}
				}
			}
		}

		// Token: 0x060021A1 RID: 8609 RVA: 0x001CBA1C File Offset: 0x001C9C1C
		public bool RemoveFriend(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID)
		{
			bool ret = false;
			try
			{
				string strcmd = string.Format("{0}:{1}", dbID, client.ClientData.RoleID);
				byte[] bytesCmd = new UTF8Encoding().GetBytes(strcmd);
				TCPOutPacket tcpOutPacket = null;
				TCPProcessCmdResults result = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, null, pool, 144, bytesCmd, bytesCmd.Length, out tcpOutPacket, client.ServerId);
				if (TCPProcessCmdResults.RESULT_FAILED != result)
				{
					string strData = new UTF8Encoding().GetString(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
					string[] fields = strData.Split(new char[]
					{
						':'
					});
					if (fields.Length == 3 && Convert.ToInt32(fields[2]) >= 0)
					{
						Global.RemoveFriendData(client, dbID);
					}
					ret = true;
				}
				if (!tcpMgr.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
				return ret;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return ret;
		}

		// Token: 0x060021A2 RID: 8610 RVA: 0x001CBB44 File Offset: 0x001C9D44
		public bool AddFriend(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int otherRoleID, string otherRoleName, int friendType)
		{
			bool ret = false;
			bool result2;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result2 = false;
			}
			else if (friendType == 2 && otherRoleID == client.ClientData.RoleID)
			{
				result2 = false;
			}
			else
			{
				try
				{
					FriendData friendData;
					if (otherRoleID > 0)
					{
						friendData = Global.FindFriendData(client, otherRoleID);
						if (null != friendData)
						{
							if (friendData.FriendType == friendType)
							{
								return ret;
							}
						}
					}
					int friendTypeCount = Global.GetFriendCountByType(client, friendType);
					if (0 == friendType)
					{
						if (friendTypeCount >= 50)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(54, new object[0]), new object[]
							{
								50
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return ret;
						}
					}
					else if (1 == friendType)
					{
						if (friendTypeCount >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(55, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return ret;
						}
					}
					else if (2 == friendType)
					{
						if (friendTypeCount >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(53, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return ret;
						}
					}
					string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						dbID,
						client.ClientData.RoleID,
						otherRoleName,
						friendType
					});
					byte[] bytesCmd = new UTF8Encoding().GetBytes(strcmd);
					TCPOutPacket tcpOutPacket = null;
					TCPProcessCmdResults result = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, null, pool, 143, bytesCmd, bytesCmd.Length, out tcpOutPacket, client.ServerId);
					if (null == tcpOutPacket)
					{
						return ret;
					}
					friendData = DataHelper.BytesToObject<FriendData>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
					if (friendData != null && friendData.DbID >= 0)
					{
						ret = true;
						Global.RemoveFriendData(client, friendData.DbID);
						Global.AddFriendData(client, friendData);
						if (0 == friendType)
						{
							friendTypeCount = Global.GetFriendCountByType(client, friendType);
							if (1 == friendTypeCount)
							{
								ChengJiuManager.OnFirstAddFriend(client);
							}
							ProcessTask.ProcessRoleTaskVal(client, TaskTypes.FreindNum, friendTypeCount);
						}
						GameClient otherClient = GameManager.ClientMgr.FindClient(friendData.OtherRoleID);
						if (null != otherClient)
						{
							if (friendData.FriendType == 0)
							{
								string typeName = GLang.GetLang(56, new object[0]);
								GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, otherClient, StringUtil.substitute(GLang.GetLang(59, new object[0]), new object[]
								{
									Global.FormatRoleName(client, client.ClientData.RoleName),
									typeName
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
					if (!tcpMgr.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					return ret;
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result2 = ret;
			}
			return result2;
		}

		// Token: 0x060021A3 RID: 8611 RVA: 0x001CBF2C File Offset: 0x001CA12C
		public void NotifyDianJiangData(SocketListener sl, TCPOutPacketPool pool, DJRoomData roomData)
		{
			if (null != roomData)
			{
				byte[] bytesData = null;
				lock (roomData)
				{
					bytesData = DataHelper.ObjectToBytes<DJRoomData>(roomData);
				}
				if (bytesData != null && bytesData.Length > 0)
				{
					int index = 0;
					GameClient client;
					while ((client = this.GetNextClient(ref index, false)) != null)
					{
						if (client.ClientData.ViewDJRoomDlg)
						{
							TCPOutPacket tcpOutPacket = pool.Pop();
							tcpOutPacket.PacketCmdID = 186;
							tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		// Token: 0x060021A4 RID: 8612 RVA: 0x001CC00C File Offset: 0x001CA20C
		public void NotifyDJRoomRolesData(SocketListener sl, TCPOutPacketPool pool, DJRoomRolesData djRoomRolesData)
		{
			if (null != djRoomRolesData)
			{
				lock (djRoomRolesData)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<DJRoomRolesData>(djRoomRolesData);
					for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
					{
						GameClient client = this.FindClient(djRoomRolesData.Team1[i].RoleID);
						if (null != client)
						{
							TCPOutPacket tcpOutPacket = pool.Pop();
							tcpOutPacket.PacketCmdID = 187;
							tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
					for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
					{
						GameClient client = this.FindClient(djRoomRolesData.Team2[i].RoleID);
						if (null != client)
						{
							TCPOutPacket tcpOutPacket = pool.Pop();
							tcpOutPacket.PacketCmdID = 187;
							tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		// Token: 0x060021A5 RID: 8613 RVA: 0x001CC16C File Offset: 0x001CA36C
		public void NotifyDianJiangCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int djCmdType, int extTag1, string extTag2, bool allSend = false)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				djCmdType,
				extTag1,
				extTag2
			});
			if (!allSend)
			{
				TCPOutPacket tcpOutPacket = null;
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 188);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				int index = 0;
				client = null;
				TCPOutPacket tcpOutPacket = null;
				try
				{
					while ((client = this.GetNextClient(ref index, false)) != null)
					{
						if (client.ClientData.ViewDJRoomDlg)
						{
							if (null == tcpOutPacket)
							{
								tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 188);
							}
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, false))
							{
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x060021A6 RID: 8614 RVA: 0x001CC280 File Offset: 0x001CA480
		public int DestroyDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djRoomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djRoomData)
				{
					result = -2;
				}
				else if (djRoomData.CreateRoleID != client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djRoomData)
					{
						if (djRoomData.PKState > 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djRoomRolesData)
					{
						result = -5;
					}
					else
					{
						int roomID = client.ClientData.DJRoomID;
						GameManager.DJRoomMgr.RemoveRoomData(roomID);
						GameManager.DJRoomMgr.RemoveRoomRolesData(roomID);
						lock (djRoomRolesData)
						{
							djRoomRolesData.Removed = 1;
							for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
								if (null != gc)
								{
									gc.ClientData.DJRoomID = -1;
									gc.ClientData.DJRoomTeamID = -1;
									gc.ClientData.HideSelf = 0;
								}
							}
							for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
								if (null != gc)
								{
									gc.ClientData.DJRoomID = -1;
									gc.ClientData.DJRoomTeamID = -1;
									gc.ClientData.HideSelf = 0;
								}
							}
						}
						GameManager.ClientMgr.NotifyDianJiangCmd(sl, pool, client, 0, 2, roomID, "", true);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x060021A7 RID: 8615 RVA: 0x001CC4E0 File Offset: 0x001CA6E0
		public int LeaveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djRoomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djRoomData)
				{
					result = -2;
				}
				else if (djRoomData.CreateRoleID == client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djRoomData)
					{
						if (djRoomData.PKState > 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djRoomRolesData)
					{
						result = -5;
					}
					else
					{
						int roomID = client.ClientData.DJRoomID;
						bool found = false;
						lock (djRoomRolesData)
						{
							if (djRoomRolesData.Removed > 0)
							{
								return -6;
							}
							if (djRoomRolesData.Locked > 0)
							{
								return -7;
							}
							for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
							{
								if (client.ClientData.RoleID == djRoomRolesData.Team1[i].RoleID)
								{
									found = true;
									djRoomRolesData.Team1.RemoveAt(i);
									break;
								}
							}
							if (!found)
							{
								for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
								{
									if (client.ClientData.RoleID == djRoomRolesData.Team2[i].RoleID)
									{
										found = true;
										djRoomRolesData.Team2.RemoveAt(i);
										break;
									}
								}
							}
							djRoomRolesData.TeamStates.Remove(client.ClientData.RoleID);
							djRoomRolesData.RoleStates.Remove(client.ClientData.RoleID);
						}
						if (found)
						{
							lock (djRoomData)
							{
								djRoomData.PKRoleNum--;
							}
						}
						client.ClientData.DJRoomID = -1;
						client.ClientData.DJRoomTeamID = -1;
						client.ClientData.HideSelf = 0;
						GameManager.ClientMgr.NotifyDianJiangData(sl, pool, djRoomData);
						GameManager.ClientMgr.NotifyDJRoomRolesData(sl, pool, djRoomRolesData);
						GameManager.ClientMgr.NotifyDianJiangCmd(sl, pool, client, 0, 4, roomID, Global.FormatRoleName(client, client.ClientData.RoleName), true);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x060021A8 RID: 8616 RVA: 0x001CC82C File Offset: 0x001CAA2C
		public int ViewerLeaveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else if (client.ClientData.DJRoomTeamID > 0)
			{
				result = -100;
			}
			else
			{
				DJRoomData djRoomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djRoomData)
				{
					result = -2;
				}
				else
				{
					DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djRoomRolesData)
					{
						result = -3;
					}
					else
					{
						int roomID = client.ClientData.DJRoomID;
						bool found = false;
						lock (djRoomRolesData)
						{
							if (null != djRoomRolesData.ViewRoles)
							{
								for (int i = 0; i < djRoomRolesData.ViewRoles.Count; i++)
								{
									if (client.ClientData.RoleID == djRoomRolesData.ViewRoles[i].RoleID)
									{
										found = true;
										djRoomRolesData.ViewRoles.RemoveAt(i);
										break;
									}
								}
							}
						}
						if (found)
						{
							lock (djRoomData)
							{
								djRoomData.ViewRoleNum--;
							}
						}
						client.ClientData.DJRoomID = -1;
						client.ClientData.DJRoomTeamID = -1;
						client.ClientData.HideSelf = 0;
						GameManager.ClientMgr.NotifyDianJiangData(sl, pool, djRoomData);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x060021A9 RID: 8617 RVA: 0x001CCA10 File Offset: 0x001CAC10
		public int TransportDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djRoomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djRoomData)
				{
					result = -2;
				}
				else if (djRoomData.CreateRoleID != client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djRoomData)
					{
						if (djRoomData.PKState <= 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djRoomRolesData)
					{
						result = -5;
					}
					else
					{
						lock (djRoomRolesData)
						{
							if (djRoomRolesData.Locked <= 0)
							{
								return -6;
							}
							for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
								if (null != gc)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, Global.DianJiangTaiMapCode, -1, -1, -1, 0);
								}
							}
							for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
								if (null != gc)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, Global.DianJiangTaiMapCode, -1, -1, -1, 0);
								}
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x060021AA RID: 8618 RVA: 0x001CCC54 File Offset: 0x001CAE54
		public void NotifyDianJiangFightCmd(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData, int djCmdType, string extTag2, GameClient toClient = null)
		{
			if (null != djRoomData)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					djRoomData.RoomID,
					djCmdType,
					0,
					extTag2
				});
				DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djRoomRolesData)
				{
					TCPOutPacket tcpOutPacket = null;
					if (null != toClient)
					{
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 189);
						if (!sl.SendData(toClient.ClientSocket, tcpOutPacket, true))
						{
						}
					}
					else
					{
						lock (djRoomRolesData)
						{
							tcpOutPacket = null;
							try
							{
								for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
								{
									GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
									if (null != gc)
									{
										if (null == tcpOutPacket)
										{
											tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 189);
										}
										if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
										{
										}
									}
								}
								for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
								{
									GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
									if (null != gc)
									{
										if (null == tcpOutPacket)
										{
											tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 189);
										}
										if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
										{
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpOutPacket);
							}
							tcpOutPacket = null;
							try
							{
								if (null != djRoomRolesData.ViewRoles)
								{
									strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										djRoomData.RoomID,
										djCmdType,
										1,
										extTag2
									});
									for (int i = 0; i < djRoomRolesData.ViewRoles.Count; i++)
									{
										GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.ViewRoles[i].RoleID);
										if (null != gc)
										{
											if (null == tcpOutPacket)
											{
												tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 189);
											}
											if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
											{
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpOutPacket);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021AB RID: 8619 RVA: 0x001CCF64 File Offset: 0x001CB164
		public void NotifyDJFightRoomLeaveMsg(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData)
		{
			if (null != djRoomData)
			{
				DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djRoomRolesData)
				{
					lock (djRoomRolesData)
					{
						for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
						{
							int state = 0;
							djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out state);
							if (1 == state)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
								if (null != gc)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, gc.ClientData.LastMapCode, gc.ClientData.LastPosX, gc.ClientData.LastPosY, -1, 0);
								}
							}
						}
						for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
						{
							int state = 0;
							djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out state);
							if (1 == state)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
								if (null != gc)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, gc.ClientData.LastMapCode, gc.ClientData.LastPosX, gc.ClientData.LastPosY, -1, 0);
								}
							}
						}
						if (null != djRoomRolesData.ViewRoles)
						{
							for (int i = 0; i < djRoomRolesData.ViewRoles.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.ViewRoles[i].RoleID);
								if (null != gc)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gc, gc.ClientData.LastMapCode, gc.ClientData.LastPosX, gc.ClientData.LastPosY, -1, 0);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021AC RID: 8620 RVA: 0x001CD208 File Offset: 0x001CB408
		public void NotifyDianJiangRoomRolesPoint(SocketListener sl, TCPOutPacketPool pool, DJRoomRolesPoint djRoomRolesPoint)
		{
			if (null != djRoomRolesPoint)
			{
				DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomRolesPoint.RoomID);
				if (null != djRoomRolesData)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<DJRoomRolesPoint>(djRoomRolesPoint);
					if (null != bytesData)
					{
						TCPOutPacket tcpOutPacket = null;
						lock (djRoomRolesData)
						{
							try
							{
								for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
								{
									GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
									if (null != gc)
									{
										if (null == tcpOutPacket)
										{
											tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, 190);
										}
										if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
										{
										}
									}
								}
								for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
								{
									GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
									if (null != gc)
									{
										if (null == tcpOutPacket)
										{
											tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, 190);
										}
										if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
										{
										}
									}
								}
								if (null != djRoomRolesData.ViewRoles)
								{
									for (int i = 0; i < djRoomRolesData.ViewRoles.Count; i++)
									{
										GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.ViewRoles[i].RoleID);
										if (null != gc)
										{
											if (null == tcpOutPacket)
											{
												tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, 190);
											}
											if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
											{
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpOutPacket);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021AD RID: 8621 RVA: 0x001CD45C File Offset: 0x001CB65C
		public void RemoveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData)
		{
			if (null != djRoomData)
			{
				DJRoomRolesData djRoomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djRoomRolesData)
				{
					int roomID = djRoomData.RoomID;
					GameManager.DJRoomMgr.RemoveRoomData(roomID);
					GameManager.DJRoomMgr.RemoveRoomRolesData(roomID);
					lock (djRoomRolesData)
					{
						djRoomRolesData.Removed = 1;
						for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
						{
							int state = 0;
							djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out state);
							if (1 == state)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team1[i].RoleID);
								if (null != gc)
								{
									gc.ClientData.DJRoomID = -1;
									gc.ClientData.DJRoomTeamID = -1;
									gc.ClientData.HideSelf = 0;
								}
							}
						}
						for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
						{
							int state = 0;
							djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out state);
							if (1 == state)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.Team2[i].RoleID);
								if (null != gc)
								{
									gc.ClientData.DJRoomID = -1;
									gc.ClientData.DJRoomTeamID = -1;
									gc.ClientData.HideSelf = 0;
								}
							}
						}
						if (null != djRoomRolesData.ViewRoles)
						{
							for (int i = 0; i < djRoomRolesData.ViewRoles.Count; i++)
							{
								GameClient gc = GameManager.ClientMgr.FindClient(djRoomRolesData.ViewRoles[i].RoleID);
								if (null != gc)
								{
									gc.ClientData.DJRoomID = -1;
									gc.ClientData.DJRoomTeamID = -1;
									gc.ClientData.HideSelf = 0;
								}
							}
						}
					}
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						0,
						-1,
						2,
						roomID,
						"noHint"
					});
					int index = 0;
					TCPOutPacket tcpOutPacket = null;
					try
					{
						GameClient client;
						while ((client = this.GetNextClient(ref index, false)) != null)
						{
							if (client.ClientData.ViewDJRoomDlg)
							{
								if (null == tcpOutPacket)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 188);
								}
								if (!sl.SendData(client.ClientSocket, tcpOutPacket, false))
								{
								}
							}
						}
					}
					finally
					{
						this.PushBackTcpOutPacket(tcpOutPacket);
					}
				}
			}
		}

		// Token: 0x060021AE RID: 8622 RVA: 0x001CD7B8 File Offset: 0x001CB9B8
		public void NotifyArenaBattleCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int battleType, int extTag1, int leftSecs)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				battleType,
				extTag1,
				leftSecs
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 415);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021AF RID: 8623 RVA: 0x001CD838 File Offset: 0x001CBA38
		public void NotifyAllArenaBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int minLevel, int battleType, int extTag1, int leftSecs)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.Level >= minLevel)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							0,
							client.ClientData.RoleID,
							battleType,
							extTag1,
							leftSecs
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 415);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		// Token: 0x060021B0 RID: 8624 RVA: 0x001CD918 File Offset: 0x001CBB18
		public void NotifyArenaBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int battleType, int extTag1, int leftSecs)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					battleType,
					extTag1,
					leftSecs
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 415);
			}
		}

		// Token: 0x060021B1 RID: 8625 RVA: 0x001CD994 File Offset: 0x001CBB94
		public void NotifyArenaBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, int roleNumKilled, int roleNumOnStart, int rowNumNow)
		{
			List<object> objsList = this.Container.GetObjectsByMap(GameManager.ArenaBattleMgr.BattleMapCode);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient client = objsList[i] as GameClient;
					if (null != client)
					{
						string strcmd = string.Format("{0}:{1}", client.ClientData.KingOfPkCurrentPoint, rowNumNow);
						this.SendToClient(sl, pool, client, strcmd, 416);
					}
				}
			}
		}

		// Token: 0x060021B2 RID: 8626 RVA: 0x001CDA34 File Offset: 0x001CBC34
		public void NotifyBattleCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int battleType, int extTag1, int leftSecs)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				battleType,
				extTag1,
				leftSecs
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 179);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021B3 RID: 8627 RVA: 0x001CDAB4 File Offset: 0x001CBCB4
		public void NotifyAllBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int minLevel, int battleType, int extTag1, int leftSecs)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.Level >= minLevel)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							0,
							client.ClientData.RoleID,
							battleType,
							extTag1,
							leftSecs
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 179);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		// Token: 0x060021B4 RID: 8628 RVA: 0x001CDB94 File Offset: 0x001CBD94
		public void NotifyBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int battleType, int extTag1, int leftSecs)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					battleType,
					extTag1,
					leftSecs
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 179);
			}
		}

		// Token: 0x060021B5 RID: 8629 RVA: 0x001CDC10 File Offset: 0x001CBE10
		public void BattleBeginForceLeaveg(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient client = objsList[i] as GameClient;
					if (null != client)
					{
						this.Container.RemoveObject(client.ClientData.RoleID, mapCode, client);
					}
				}
			}
		}

		// Token: 0x060021B6 RID: 8630 RVA: 0x001CDC84 File Offset: 0x001CBE84
		public void NotifyBattleLeaveMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient client = objsList[i] as GameClient;
					if (null != client)
					{
						int toMapCode = GameManager.MainMapCode;
						int toPosX = -1;
						int toPosY = -1;
						if (client.ClientData.LastMapCode != -1 && client.ClientData.LastPosX != -1 && client.ClientData.LastPosY != -1)
						{
							if (MapTypes.Normal == Global.GetMapType(client.ClientData.LastMapCode))
							{
								toMapCode = client.ClientData.LastMapCode;
								toPosX = client.ClientData.LastPosX;
								toPosY = client.ClientData.LastPosY;
							}
						}
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
						{
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, toPosX, toPosY, -1, 0);
						}
					}
				}
			}
		}

		// Token: 0x060021B7 RID: 8631 RVA: 0x001CDDBC File Offset: 0x001CBFBC
		public void NotifyBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int tangJiFen, int suiJiFen)
		{
			int nTotal = BattleManager.BattleMaxPointNow;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.BattleKilledNum,
				nTotal,
				tangJiFen,
				suiJiFen
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 282);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021B8 RID: 8632 RVA: 0x001CDE4C File Offset: 0x001CC04C
		public void NotifyBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, int suiJiFen, int tangJiFen)
		{
			List<object> objsList = this.Container.GetObjectsByMap(GameManager.BattleMgr.BattleMapCode);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-1,
					-1,
					-1,
					tangJiFen,
					suiJiFen
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 282);
			}
		}

		// Token: 0x060021B9 RID: 8633 RVA: 0x001CDED0 File Offset: 0x001CC0D0
		public void NotifyRoleBattleNameInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex);
				this.SendToClients(sl, pool, null, objsList, strcmd, 283);
			}
		}

		// Token: 0x060021BA RID: 8634 RVA: 0x001CDF40 File Offset: 0x001CC140
		public void ProcessRoleBattleNameInfoTimeOut(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.BattleNameIndex > 0)
			{
				long ticks = TimeUtil.NOW();
				if (ticks - client.ClientData.BattleNameStart >= Global.MaxBattleNameTicks)
				{
					client.ClientData.BattleNameIndex = 0;
					GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
					GameManager.ClientMgr.NotifyRoleBattleNameInfo(sl, pool, client);
				}
			}
		}

		// Token: 0x060021BB RID: 8635 RVA: 0x001CDFF8 File Offset: 0x001CC1F8
		public void NotifyRoleBattleRoleInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, int startTotalRoleNum, int allKilledRoleNum)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", startTotalRoleNum, allKilledRoleNum);
				this.SendToClients(sl, pool, null, objsList, strcmd, 285);
			}
		}

		// Token: 0x060021BC RID: 8636 RVA: 0x001CE04C File Offset: 0x001CC24C
		public void NotifyRoleBattleEndInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, List<BattleEndRoleItem> endRoleItemList)
		{
			if (endRoleItemList.Count > 0)
			{
				List<object> objsList = this.Container.GetObjectsByMap(mapCode);
				if (null != objsList)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<List<BattleEndRoleItem>>(endRoleItemList);
					this.SendToClients(sl, pool, null, objsList, bytesData, 286);
				}
			}
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x001CE0A0 File Offset: 0x001CC2A0
		public void NotifyRoleBattleSideInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BattleWhichSide);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 359);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021BE RID: 8638 RVA: 0x001CE0FC File Offset: 0x001CC2FC
		public void NotifyRoleBattlePlayerSideNumberEndInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nNum1, int nNum2)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", nNum1, nNum2);
				this.SendToClients(sl, pool, null, objsList, strcmd, 547);
			}
		}

		// Token: 0x060021BF RID: 8639 RVA: 0x001CE150 File Offset: 0x001CC350
		public void NotifyAutoFightCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int fightType, int extTag1)
		{
			SCAutoFight scData = new SCAutoFight(status, client.ClientData.RoleID, fightType, extTag1);
			client.sendCmd<SCAutoFight>(182, scData, false);
		}

		// Token: 0x060021C0 RID: 8640 RVA: 0x001CE184 File Offset: 0x001CC384
		public void NotifyTeamCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int teamType, int extTag1, string extTag2, int nOccu = -1, int nLev = -1, int nChangeLife = -1)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
			{
				status,
				client.ClientData.RoleID,
				teamType,
				extTag1,
				extTag2,
				nOccu,
				nLev,
				nChangeLife
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 176);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021C1 RID: 8641 RVA: 0x001CE21C File Offset: 0x001CC41C
		public void NotifyTeamData(SocketListener sl, TCPOutPacketPool pool, TeamData td)
		{
			if (null != td)
			{
				lock (td)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<TeamData>(td);
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient client = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != client)
						{
							TCPOutPacket tcpOutPacket = pool.Pop();
							tcpOutPacket.PacketCmdID = 177;
							tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060021C2 RID: 8642 RVA: 0x001CE2F8 File Offset: 0x001CC4F8
		public void NotifyOthersTeamIDChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TeamID, Global.GetGameClientTeamLeaderID(client.ClientData));
				this.SendToClients(sl, pool, null, objsList, strcmd, 178);
			}
		}

		// Token: 0x060021C3 RID: 8643 RVA: 0x001CE368 File Offset: 0x001CC568
		public void NotifyOthersTeamDestroy(SocketListener sl, TCPOutPacketPool pool, GameClient client, TeamData td)
		{
			if (null != td)
			{
				lock (td)
				{
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient gameClient = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != gameClient)
						{
							if (client != gameClient)
							{
								gameClient.ClientData.TeamID = 0;
								GameManager.TeamMgr.RemoveRoleID2TeamID(gameClient.ClientData.RoleID);
								this.NotifyOthersTeamIDChanged(sl, pool, gameClient);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021C4 RID: 8644 RVA: 0x001CE438 File Offset: 0x001CC638
		public void NotifyTeamUpLevel(SocketListener sl, TCPOutPacketPool pool, GameClient client, bool zhuanShengChanged = false)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != td)
				{
					lock (td)
					{
						for (int i = 0; i < td.TeamRoles.Count; i++)
						{
							GameClient gc = this.FindClient(td.TeamRoles[i].RoleID);
							if (null != gc)
							{
								if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									td.TeamRoles[i].Level = client.ClientData.Level;
									td.TeamRoles[i].ChangeLifeLev = client.ClientData.ChangeLifeCount;
								}
								string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Level, client.ClientData.ChangeLifeCount);
								TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 288);
								if (!sl.SendData(gc.ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021C5 RID: 8645 RVA: 0x001CE5E0 File Offset: 0x001CC7E0
		public void NotifySelfChgZhanLi(GameClient client, int ZhanLi)
		{
			client.sendCmd<int>(675, ZhanLi, false);
		}

		// Token: 0x060021C6 RID: 8646 RVA: 0x001CE5F4 File Offset: 0x001CC7F4
		public void NotifyTeamCHGZhanLi(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != td)
				{
					lock (td)
					{
						string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.CombatForce);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 674);
						for (int i = 0; i < td.TeamRoles.Count; i++)
						{
							GameClient gc = this.FindClient(td.TeamRoles[i].RoleID);
							if (null != gc)
							{
								if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									td.TeamRoles[i].CombatForce = client.ClientData.CombatForce;
								}
								if (gc.CodeRevision >= 1)
								{
									if (!sl.SendData(gc.ClientSocket, tcpOutPacket, false))
									{
									}
								}
							}
						}
						this.PushBackTcpOutPacket(tcpOutPacket);
					}
				}
			}
		}

		// Token: 0x060021C7 RID: 8647 RVA: 0x001CE78C File Offset: 0x001CC98C
		public void NotifyGoodsExchangeCmd(SocketListener sl, TCPOutPacketPool pool, int roleID, int otherRoleID, GameClient client, GameClient otherClient, int status, int exchangeType)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				status,
				roleID,
				otherRoleID,
				exchangeType
			});
			if (null != client)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 170);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			if (null != otherClient)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 170);
				if (!sl.SendData(otherClient.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060021C8 RID: 8648 RVA: 0x001CE830 File Offset: 0x001CCA30
		public void NotifyGoodsExchangeData(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, ExchangeData ed)
		{
			byte[] bytesData = null;
			lock (ed)
			{
				bytesData = DataHelper.ObjectToBytes<ExchangeData>(ed);
			}
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = 171;
			tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
			tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = 171;
			tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
			if (!sl.SendData(otherClient.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021C9 RID: 8649 RVA: 0x001CE8EC File Offset: 0x001CCAEC
		private void ProcessExchangeData(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.ExchangeID > 0)
			{
				ExchangeData ed = GameManager.GoodsExchangeMgr.FindData(client.ClientData.ExchangeID);
				if (null != ed)
				{
					int otherRoleID = (ed.RequestRoleID == client.ClientData.RoleID) ? ed.AgreeRoleID : ed.RequestRoleID;
					GameClient otherClient = GameManager.ClientMgr.FindClient(otherRoleID);
					if (null != otherClient)
					{
						if (otherClient.ClientData.ExchangeID > 0 && otherClient.ClientData.ExchangeID == client.ClientData.ExchangeID)
						{
							GameManager.GoodsExchangeMgr.RemoveData(client.ClientData.ExchangeID);
							Global.RestoreExchangeData(otherClient, ed);
							otherClient.ClientData.ExchangeID = 0;
							otherClient.ClientData.ExchangeTicks = 0L;
							GameManager.ClientMgr.NotifyGoodsExchangeCmd(sl, pool, client.ClientData.RoleID, otherRoleID, null, otherClient, client.ClientData.ExchangeID, 4);
						}
					}
				}
			}
		}

		// Token: 0x060021CA RID: 8650 RVA: 0x001CEA08 File Offset: 0x001CCC08
		public void NotifyMySelfNewGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int ownerRoleID, string ownerRoleName, int autoID, int goodsPackID, int mapCode, int toX, int toY, int goodsID, int goodsNum, long productTicks, int teamID, string teamRoleIDs, int lucky, int excellenceInfo, int appendPropLev, int forge_Level)
		{
			NewGoodsPackData newGoodsPackData = new NewGoodsPackData
			{
				ownerRoleID = ownerRoleID,
				ownerRoleName = ownerRoleName,
				autoID = autoID,
				goodsPackID = goodsPackID,
				mapCode = mapCode,
				toX = toX,
				toY = toY,
				goodsID = goodsID,
				goodsNum = goodsNum,
				productTicks = productTicks,
				teamID = (long)teamID,
				teamRoleIDs = teamRoleIDs,
				lucky = lucky,
				excellenceInfo = excellenceInfo,
				appendPropLev = appendPropLev,
				forge_Level = forge_Level
			};
			byte[] bytes = DataHelper.ObjectToBytes<NewGoodsPackData>(newGoodsPackData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 145);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021CB RID: 8651 RVA: 0x001CEAD0 File Offset: 0x001CCCD0
		public void NotifySelfGetThing(SocketListener sl, TCPOutPacketPool pool, GameClient client, int goodsDbID)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, goodsDbID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 148);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021CC RID: 8652 RVA: 0x001CEB28 File Offset: 0x001CCD28
		public void NotifyOthersDelGoodsPack(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int mapCode, int autoID, int toRoleID)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", autoID, toRoleID);
				this.SendToClients(sl, pool, null, objsList, strcmd, 146);
			}
		}

		// Token: 0x060021CD RID: 8653 RVA: 0x001CEB70 File Offset: 0x001CCD70
		public void NotifyMySelfDelGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID)
		{
			string strcmd = string.Format("{0}:{1}", autoID, -1);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 146);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021CE RID: 8654 RVA: 0x001CEBB8 File Offset: 0x001CCDB8
		public static void NotifySelfEnemyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int burst, int injure, double enemyLife, long newExperience, int nMerlinInjure = 0, EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None, int armorV_p1 = 0)
		{
			SpriteAttackResultData attackResultData = new SpriteAttackResultData();
			attackResultData.enemy = enemy;
			attackResultData.burst = burst;
			attackResultData.injure = injure;
			attackResultData.enemyLife = enemyLife;
			attackResultData.newExperience = newExperience;
			attackResultData.currentExperience = client.ClientData.Experience;
			attackResultData.newLevel = client.ClientData.Level;
			attackResultData.armorV_p1 = armorV_p1;
			if (nMerlinInjure > 0)
			{
				attackResultData.MerlinInjuer = nMerlinInjure;
				attackResultData.MerlinType = (int)eMerlinType;
			}
			byte[] cmdData = DataHelper.ObjectToBytes<SpriteAttackResultData>(attackResultData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdData, 0, cmdData.Length, 117);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021CF RID: 8655 RVA: 0x001CEC68 File Offset: 0x001CCE68
		private IObject GetInjuredObject(int mapCode, int injuredRoleID)
		{
			IObject injuredObj = null;
			GSpriteTypes st = Global.GetSpriteType((uint)injuredRoleID);
			if (st == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, injuredRoleID);
				if (null != monster)
				{
					injuredObj = monster;
				}
			}
			else if (st == GSpriteTypes.BiaoChe)
			{
				BiaoCheManager.FindBiaoCheByRoleID(injuredRoleID);
			}
			else
			{
				if (st == GSpriteTypes.JunQi)
				{
					return JunQiManager.FindJunQiByID(injuredRoleID);
				}
				GameClient obj = GameManager.ClientMgr.FindClient(injuredRoleID);
				if (null != obj)
				{
					injuredObj = obj;
				}
			}
			return injuredObj;
		}

		// Token: 0x060021D0 RID: 8656 RVA: 0x001CED00 File Offset: 0x001CCF00
		public void NotifySpriteInjured(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int mapCode, int attackerRoleID, int injuredRoleID, int burst, int injure, double injuredRoleLife, int attackerLevel, Point hitToGrid, int nMerlinInjure = 0, EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None, int armorV_p1 = 0)
		{
			if (hitToGrid.X < 0.0 || hitToGrid.Y < 0.0)
			{
				hitToGrid.X = 0.0;
				hitToGrid.Y = 0.0;
			}
			IObject injuredObj = this.GetInjuredObject(mapCode, injuredRoleID);
			if (null != injuredObj)
			{
				List<object> objsList = Global.GetAll9Clients(attacker);
				if (null == objsList)
				{
					objsList = new List<object>();
				}
				if (GameManager.FlagHideFlagsType == -1 || !GameManager.HideFlagsMapDict.ContainsKey(mapCode))
				{
					if (objsList.IndexOf(injuredObj) < 0)
					{
						objsList.Add(injuredObj);
					}
					int injuredRoleMagic = 0;
					int injuredRoleMaxMagicV = 0;
					int injuredRoleMaxLifeV = 0;
					GameClient injuredClient = this.FindClient(injuredRoleID);
					if (null != injuredClient)
					{
						injuredRoleMagic = injuredClient.ClientData.CurrentMagicV;
						injuredRoleMaxMagicV = injuredClient.ClientData.MagicV;
						injuredRoleMaxLifeV = injuredClient.ClientData.LifeV;
					}
					SpriteInjuredData injuredData = new SpriteInjuredData();
					injuredData.attackerRoleID = attackerRoleID;
					injuredData.injuredRoleID = injuredRoleID;
					injuredData.burst = burst;
					injuredData.injure = injure;
					injuredData.injuredRoleLife = (long)injuredRoleLife;
					injuredData.attackerLevel = attackerLevel;
					injuredData.injuredRoleMaxLifeV = injuredRoleMaxLifeV;
					injuredData.injuredRoleMagic = injuredRoleMagic;
					injuredData.injuredRoleMaxMagicV = injuredRoleMaxMagicV;
					injuredData.hitToGridX = (int)hitToGrid.X;
					injuredData.hitToGridY = (int)hitToGrid.Y;
					injuredData.MerlinInjuer = nMerlinInjure;
					injuredData.MerlinType = (int)((sbyte)eMerlinType);
					injuredData.armorV_p1 = armorV_p1;
					byte[] bytesCmd = DataHelper.ObjectToBytes<SpriteInjuredData>(injuredData);
					injuredData.burst = 999;
					injuredData.MerlinType = 999;
					byte[] bytesCmd2 = DataHelper.ObjectToBytes<SpriteInjuredData>(injuredData);
					this.SendToClients(sl, pool, attacker, injuredObj, objsList, bytesCmd, bytesCmd2, 118, injure != 0);
					if (injure != 0)
					{
						this.NotifySpriteTeamInjured(sl, pool, injuredRoleID, bytesCmd, bytesCmd2, mapCode);
					}
				}
				else
				{
					SpriteInjuredData injuredData = new SpriteInjuredData();
					injuredData.injuredRoleID = injuredRoleID;
					injuredData.injuredRoleLife = (long)injuredRoleLife;
					injuredData.burst = burst;
					injuredData.injure = injure;
					injuredData.armorV_p1 = armorV_p1;
					if (hitToGrid.X > 0.0 || hitToGrid.Y > 0.0)
					{
						injuredData.hitToGridX = (int)hitToGrid.X;
						injuredData.hitToGridY = (int)hitToGrid.Y;
						injuredData.attackerRoleID = attackerRoleID;
					}
					if (nMerlinInjure > 0)
					{
						injuredData.MerlinInjuer = nMerlinInjure;
						injuredData.MerlinType = (int)((sbyte)eMerlinType);
					}
					if (injuredObj != null && injuredObj.ObjectType == ObjectTypes.OT_CLIENT)
					{
						if (objsList.IndexOf(injuredObj) < 0)
						{
							objsList.Add(injuredObj);
						}
					}
					bool dead = injuredRoleLife <= 0.0;
					if (dead)
					{
						injuredData.attackerRoleID = attackerRoleID;
					}
					byte[] bytesCmd = DataHelper.ObjectToBytes<SpriteInjuredData>(injuredData);
					injuredData.burst = 999;
					injuredData.MerlinType = 999;
					byte[] bytesCmd2 = DataHelper.ObjectToBytes<SpriteInjuredData>(injuredData);
					if (dead)
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, injuredObj, objsList, bytesCmd, bytesCmd2, 118, 1, injuredRoleID);
					}
					else
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, injuredObj, objsList, bytesCmd, bytesCmd2, 118, 1, injuredRoleID);
					}
					if (injure != 0)
					{
						this.NotifySpriteTeamInjured(sl, pool, injuredRoleID, bytesCmd, bytesCmd2, mapCode);
					}
				}
			}
		}

		// Token: 0x060021D1 RID: 8657 RVA: 0x001CF0B4 File Offset: 0x001CD2B4
		public void NotifySpriteTeamInjured(SocketListener sl, TCPOutPacketPool pool, int injuredRoleID, byte[] bytesCmd, byte[] bytesCmd2, int mapCode)
		{
			GameClient otherClient = this.FindClient(injuredRoleID);
			if (null != otherClient)
			{
				ZorkBattleManager.getInstance().NotifySpriteInjured(otherClient);
				EscapeBattleManager.getInstance().NotifySpriteInjured(otherClient);
				if (otherClient.ClientData.TeamID > 0)
				{
					TeamData td = GameManager.TeamMgr.FindData(otherClient.ClientData.TeamID);
					if (null != td)
					{
						List<int> roleIDsList = new List<int>();
						lock (td)
						{
							for (int i = 0; i < td.TeamRoles.Count; i++)
							{
								if (injuredRoleID != td.TeamRoles[i].RoleID)
								{
									roleIDsList.Add(td.TeamRoles[i].RoleID);
								}
							}
						}
						if (roleIDsList.Count != 0)
						{
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd, 0, bytesCmd.Length, 118);
							TCPOutPacket tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd2, 0, bytesCmd2.Length, 118);
							try
							{
								for (int i = 0; i < roleIDsList.Count; i++)
								{
									GameClient gc = this.FindClient(roleIDsList[i]);
									if (null != gc)
									{
										if (gc.ClientData.MapCode == mapCode)
										{
											if (gc.ClientEffectHideFlag1 <= 0)
											{
												sl.SendData(gc.ClientSocket, tcpOutPacket, false);
											}
											else
											{
												sl.SendData(gc.ClientSocket, tcpOutPacket2, false);
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpOutPacket);
								this.PushBackTcpOutPacket(tcpOutPacket2);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021D2 RID: 8658 RVA: 0x001CF2B0 File Offset: 0x001CD4B0
		public void NotifyAllImportantMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0, int minZhuanSheng = 0, int minLevel = 0, int maxZhuanSheng = 100, int maxLevel = 100)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (client == null || gc != client)
				{
					if (gc == null || Global.GetUnionLevel(gc, false) >= Global.GetUnionLevel(minZhuanSheng, minLevel, false))
					{
						if (gc == null || Global.GetUnionLevel(gc, false) <= Global.GetUnionLevel(maxZhuanSheng, maxLevel, false))
						{
							this.NotifyImportantMsg(sl, pool, gc, msgText, typeIndex, showGameInfoType, errCode);
						}
					}
				}
			}
		}

		// Token: 0x060021D3 RID: 8659 RVA: 0x001CF350 File Offset: 0x001CD550
		public void NotifyBangHuiImportantMsg(SocketListener sl, TCPOutPacketPool pool, int faction, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (faction == gc.ClientData.Faction)
				{
					this.NotifyImportantMsg(sl, pool, gc, msgText, typeIndex, showGameInfoType, errCode);
				}
			}
		}

		// Token: 0x060021D4 RID: 8660 RVA: 0x001CF3A4 File Offset: 0x001CD5A4
		public void SendBangHuiCmd<T>(int faction, int cmdId, T cmdData, bool excludeKuaFu = true, bool normalMapNoly = false)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (faction == gc.ClientData.Faction)
				{
					if (!excludeKuaFu || !gc.ClientSocket.IsKuaFuLogin)
					{
						if (!normalMapNoly || Global.GetMapSceneType(gc.ClientData.MapCode) == SceneUIClasses.Normal)
						{
							gc.sendCmd<T>(cmdId, cmdData, false);
						}
					}
				}
			}
		}

		// Token: 0x060021D5 RID: 8661 RVA: 0x001CF430 File Offset: 0x001CD630
		public void NotifyImportantMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = msgText.Replace(":", "``");
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				(int)showGameInfoType,
				(int)typeIndex,
				msgText,
				errCode
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 194);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021D6 RID: 8662 RVA: 0x001CF4AC File Offset: 0x001CD6AC
		public void NotifyUseGoodsResult(SocketListener sl, TCPOutPacketPool pool, GameClient client, int goodsID, int useNum)
		{
			string strcmd = string.Format("{0}:{1}", goodsID, useNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 689);
			sl.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		// Token: 0x060021D7 RID: 8663 RVA: 0x001CF4F1 File Offset: 0x001CD6F1
		public void NotifyImportantMsg(GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, typeIndex, showGameInfoType, errCode);
		}

		// Token: 0x060021D8 RID: 8664 RVA: 0x001CF51C File Offset: 0x001CD71C
		public void NotifyImportantMsgWithGoods(GameClient client, MsgWithGoodsType idx, ShowGameInfoTypes showType, List<GoodsData> goodsDataList, string param1, Dictionary<int, List<GoodsData>> goodsDic = null)
		{
			NotifyMsgWithGoodsData msgData = new NotifyMsgWithGoodsData
			{
				index = (int)idx,
				type = (int)showType,
				goodsDataList = goodsDataList,
				param1 = param1,
				goodsDic = goodsDic
			};
			byte[] bytes = DataHelper.ObjectToBytes<NotifyMsgWithGoodsData>(msgData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, bytes, 0, bytes.Length, 3001);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021D9 RID: 8665 RVA: 0x001CF598 File Offset: 0x001CD798
		public void NotifyGetAwardMsg(GameClient client, RoleAwardMsg type, string notifyParam)
		{
			List<GoodsData> goodsList = client.ClientData.GetAwardRecord(type);
			if (goodsList != null && goodsList.Count != 0)
			{
				GameManager.ClientMgr.NotifyImportantMsgWithGoods(client, MsgWithGoodsType.GoodsAwards, ShowGameInfoTypes.OnlyChatBox, goodsList, notifyParam, null);
				client.ClientData.ClearAwardRecord(type);
			}
		}

		// Token: 0x060021DA RID: 8666 RVA: 0x001CF5EB File Offset: 0x001CD7EB
		public void NotifyAddExpMsg(GameClient client, long addExp)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format(GLang.GetLang(60, new object[0]), addExp), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
		}

		// Token: 0x060021DB RID: 8667 RVA: 0x001CF62C File Offset: 0x001CD82C
		public void NotifyAddJinBiMsg(GameClient client, int addJinBi)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(61, new object[0]), new object[]
			{
				addJinBi
			}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
		}

		// Token: 0x060021DC RID: 8668 RVA: 0x001CF681 File Offset: 0x001CD881
		public void NotifyHintMsg(GameClient client, string msg)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
		}

		// Token: 0x060021DD RID: 8669 RVA: 0x001CF6A8 File Offset: 0x001CD8A8
		public void NotifyHintMsgDelay(GameClient client, string msg)
		{
			msg = msg.Replace(":", "``");
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				4,
				1,
				msg,
				0
			});
			client.sendCmd(194, strcmd, false);
		}

		// Token: 0x060021DE RID: 8670 RVA: 0x001CF708 File Offset: 0x001CD908
		public void NotifyCopyMapHintMsg(GameClient client, string msg)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
		}

		// Token: 0x060021DF RID: 8671 RVA: 0x001CF730 File Offset: 0x001CD930
		public void NotifyGMAuthCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int auth)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, auth);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 211);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021E0 RID: 8672 RVA: 0x001CF784 File Offset: 0x001CD984
		public void NotifyAllBulletinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, BulletinMsgData bulletinMsgData, int minZhuanSheng = 0, int minLevel = 0)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (client == null || gc != client)
				{
					if (Global.GetUnionLevel(gc, false) >= Global.GetUnionLevel(minZhuanSheng, minLevel, false))
					{
						if (!gc.ClientSocket.IsKuaFuLogin)
						{
							this.NotifyBulletinMsg(sl, pool, gc, bulletinMsgData);
						}
					}
				}
			}
		}

		// Token: 0x060021E1 RID: 8673 RVA: 0x001CF804 File Offset: 0x001CDA04
		public void NotifyBulletinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, BulletinMsgData bulletinMsgData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BulletinMsgData>(bulletinMsgData, pool, 210);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021E2 RID: 8674 RVA: 0x001CF838 File Offset: 0x001CDA38
		public void ChangeRolePKValueAndPKPoint(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy)
		{
			if (client != enemy)
			{
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
				{
					if (!WangChengManager.IsInCityWarBattling(client))
					{
						if (!MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
						{
							if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
							{
								GameMap gameMap = null;
								if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
								{
									if (0 == gameMap.PKMode)
									{
										client.ClientData.PKValue = client.ClientData.PKValue + 1;
										int enemyNameColorIndex = Global.GetNameColorIndexByPKPoints(enemy.ClientData.PKPoint);
										if (enemyNameColorIndex < 2)
										{
											if (!Global.IsPurpleName(enemy))
											{
												client.ClientData.PKPoint = Global.GMin(Global.MaxPKPointValue, client.ClientData.PKPoint + Global.PKValueEqPKPoints);
											}
										}
										else if (Global.IsRedName(client))
										{
											if (Global.AddToTodayRoleKillRoleSet(client.ClientData.RoleID, enemy.ClientData.RoleID))
											{
												client.ClientData.PKPoint = Global.GMax(0, client.ClientData.PKPoint - Global.PKValueEqPKPoints / 2);
											}
										}
										Global.ProcessRedNamePunishForDebuff(client);
										List<object> objsList = Global.GetAll9Clients(client);
										if (null != objsList)
										{
											string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint);
											this.SendToClients(sl, pool, null, objsList, strcmd, 150);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021E3 RID: 8675 RVA: 0x001CFA64 File Offset: 0x001CDC64
		public void SetRolePKValuePoint(SocketListener sl, TCPOutPacketPool pool, GameClient client, int pkValue, int pkPoint, bool writeToDB = true)
		{
			client.ClientData.PKValue = pkValue;
			client.ClientData.PKPoint = pkPoint;
			Global.ProcessRedNamePunishForDebuff(client);
			if (writeToDB)
			{
				GameManager.DBCmdMgr.AddDBCmd(10009, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10009, nowTicks);
			}
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint);
				this.SendToClients(sl, pool, null, objsList, strcmd, 150);
			}
		}

		// Token: 0x060021E4 RID: 8676 RVA: 0x001CFB64 File Offset: 0x001CDD64
		public void ChangeRolePurpleName(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy)
		{
			if (client != enemy)
			{
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
				{
					if (enemy.ClientData.PKPoint < Global.MinRedNamePKPoints)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
						{
							if (0 == gameMap.PKMode)
							{
								client.ClientData.StartPurpleNameTicks = TimeUtil.NOW();
								List<object> objsList = Global.GetAll9Clients(client);
								if (null != objsList)
								{
									string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
									this.SendToClients(sl, pool, null, objsList, strcmd, 265);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021E5 RID: 8677 RVA: 0x001CFC78 File Offset: 0x001CDE78
		public void ForceChangeRolePurpleName2(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (!Global.IsPurpleName(client))
			{
				client.ClientData.StartPurpleNameTicks = TimeUtil.NOW();
				List<object> objsList = Global.GetAll9Clients(client);
				if (null != objsList)
				{
					string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
					this.SendToClients(sl, pool, null, objsList, strcmd, 265);
				}
			}
		}

		// Token: 0x060021E6 RID: 8678 RVA: 0x001CFCF8 File Offset: 0x001CDEF8
		public void BroadcastRolePurpleName(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.StartPurpleNameTicks > 0L)
			{
				if (!Global.IsPurpleName(client))
				{
					client.ClientData.StartPurpleNameTicks = 0L;
					List<object> objsList = Global.GetAll9Clients(client);
					if (null != objsList)
					{
						string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
						this.SendToClients(sl, pool, null, objsList, strcmd, 265);
					}
				}
			}
		}

		// Token: 0x060021E7 RID: 8679 RVA: 0x001CFD8C File Offset: 0x001CDF8C
		public void NotifyAllChatMsg(SocketListener sl, TCPOutPacketPool pool, string cmdText, GameClient sender = null)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (sender != null && client.ClientSocket.IsKuaFuLogin)
				{
					if (sender.ClientData.MapCode == client.ClientData.MapCode && sender.ClientData.CopyMapID == client.ClientData.CopyMapID)
					{
						this.SendChatMessage(sl, pool, client, cmdText);
					}
				}
				else
				{
					this.SendChatMessage(sl, pool, client, cmdText);
				}
			}
		}

		// Token: 0x060021E8 RID: 8680 RVA: 0x001CFE30 File Offset: 0x001CE030
		public void NotifyFactionChatMsg(SocketListener sl, TCPOutPacketPool pool, int faction, string cmdText, GameClient sender = null)
		{
			if (faction > 0)
			{
				int index = 0;
				GameClient gc;
				while ((gc = this.GetNextClient(ref index, false)) != null)
				{
					if (gc.ClientData.Faction == faction)
					{
						if (sender != null && gc.ClientSocket.IsKuaFuLogin)
						{
							if (sender.SceneGameId == gc.SceneGameId)
							{
								this.SendChatMessage(sl, pool, gc, cmdText);
							}
						}
						else
						{
							this.SendChatMessage(sl, pool, gc, cmdText);
						}
					}
				}
			}
		}

		// Token: 0x060021E9 RID: 8681 RVA: 0x001CFED0 File Offset: 0x001CE0D0
		public void NotifyTeamChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != td)
				{
					List<int> roleIDsList = new List<int>();
					lock (td)
					{
						for (int i = 0; i < td.TeamRoles.Count; i++)
						{
							roleIDsList.Add(td.TeamRoles[i].RoleID);
						}
					}
					if (roleIDsList.Count > 0)
					{
						for (int i = 0; i < roleIDsList.Count; i++)
						{
							GameClient gc = this.FindClient(roleIDsList[i]);
							if (null != gc)
							{
								this.SendChatMessage(sl, pool, gc, cmdText);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021EA RID: 8682 RVA: 0x001CFFE8 File Offset: 0x001CE1E8
		public void NotifyMapChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			List<object> clientList = new List<object>();
			this.LookupRolesInCircle(null, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, 1000, clientList);
			if (clientList.Count > 0)
			{
				this.SendToClients(sl, pool, null, clientList, cmdText, 157);
			}
		}

		// Token: 0x060021EB RID: 8683 RVA: 0x001D004C File Offset: 0x001CE24C
		public void NotifyCopyMapChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
				if (null != copyMap)
				{
					List<object> clientList = copyMap.GetClientsList2();
					if (clientList != null && clientList.Count > 0)
					{
						this.SendToClients(sl, pool, null, clientList, cmdText, 157);
					}
				}
			}
		}

		// Token: 0x060021EC RID: 8684 RVA: 0x001D00E0 File Offset: 0x001CE2E0
		public void NotifyBattleSideChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.BattleWhichSide > 0)
			{
				if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
				{
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
					if (null != copyMap)
					{
						List<object> clientList = copyMap.GetClientsList2();
						if (clientList != null && clientList.Count > 0)
						{
							foreach (object obj in clientList)
							{
								GameClient c = obj as GameClient;
								if (null != c)
								{
									if (client.ClientData.BattleWhichSide == c.ClientData.BattleWhichSide)
									{
										c.sendCmd(157, cmdText, false);
									}
								}
							}
						}
					}
				}
				else
				{
					List<GameClient> clientList2 = GameManager.ClientMgr.GetMapGameClients(client.ClientData.MapCode);
					foreach (GameClient c in clientList2)
					{
						if (null != c)
						{
							if (client.ClientData.BattleWhichSide == c.ClientData.BattleWhichSide)
							{
								c.sendCmd(157, cmdText, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021ED RID: 8685 RVA: 0x001D02B0 File Offset: 0x001CE4B0
		public void NotifyZhanDuiChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.ZhanDuiID > 0)
			{
				if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
				{
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
					if (null != copyMap)
					{
						List<object> clientList = copyMap.GetClientsList2();
						if (clientList != null && clientList.Count > 0)
						{
							foreach (object obj in clientList)
							{
								GameClient c = obj as GameClient;
								if (null != c)
								{
									if (client.ClientData.ZhanDuiID == c.ClientData.ZhanDuiID)
									{
										c.sendCmd(157, cmdText, false);
									}
								}
							}
						}
					}
				}
				else
				{
					List<GameClient> clientList2 = GameManager.ClientMgr.GetMapGameClients(client.ClientData.MapCode);
					foreach (GameClient c in clientList2)
					{
						if (null != c)
						{
							if (client.ClientData.ZhanDuiID == c.ClientData.ZhanDuiID)
							{
								c.sendCmd(157, cmdText, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021EE RID: 8686 RVA: 0x001D0480 File Offset: 0x001CE680
		public bool NotifyClientChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, int fromRoleID, string fromRoleName, string toRoleName, int index, string textMsg, string chatType)
		{
			bool result = true;
			GameClient toClient = null;
			int roleID = RoleName2IDs.FindRoleIDByName(toRoleName, false);
			if (-1 == roleID)
			{
				result = false;
			}
			else
			{
				toClient = this.FindClient(roleID);
				if (null == toClient)
				{
					result = false;
				}
				else if (Global.InFriendsBlackList(toClient, fromRoleID))
				{
					toClient = null;
				}
			}
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				fromRoleID,
				fromRoleName,
				0,
				toRoleName,
				index,
				textMsg,
				chatType
			});
			if (null != client)
			{
				this.SendChatMessage(sl, pool, client, strcmd);
			}
			if (null != toClient)
			{
				this.SendChatMessage(sl, pool, toClient, strcmd);
			}
			else
			{
				string offlineTip = string.Format(GLang.GetLang(62, new object[0]), toRoleName);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, offlineTip);
			}
			return result;
		}

		// Token: 0x060021EF RID: 8687 RVA: 0x001D05A0 File Offset: 0x001CE7A0
		public void SendSystemChatMessageToClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, string textMsg)
		{
			if (null != client)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					-1,
					"",
					0,
					"",
					0,
					textMsg,
					0
				});
				this.SendChatMessage(sl, pool, client, strcmd);
			}
		}

		// Token: 0x060021F0 RID: 8688 RVA: 0x001D0610 File Offset: 0x001CE810
		public void SendSystemChatMessageToClients(SocketListener sl, TCPOutPacketPool pool, string textMsg)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				-1,
				"",
				0,
				"",
				0,
				textMsg
			});
			this.NotifyAllChatMsg(sl, pool, strcmd, null);
		}

		// Token: 0x060021F1 RID: 8689 RVA: 0x001D066C File Offset: 0x001CE86C
		public void SendChatMessage(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdText, 157);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060021F2 RID: 8690 RVA: 0x001D06A0 File Offset: 0x001CE8A0
		public void HandleTransferChatMsg()
		{
			long ticks = TimeUtil.NOW();
			if (ticks - this.LastTransferTicks >= 5000L)
			{
				this.LastTransferTicks = ticks;
				TCPOutPacket tcpOutPacket = null;
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					GameManager.ServerLineID,
					GameManager.ClientMgr.GetClientCount(),
					Global.SendServerHeartCount,
					GetMapcodeOnlineNumManager.CountMapIDOnlineNum()
				});
				Global.SendServerHeartCount++;
				TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10018, strcmd, out tcpOutPacket, 0);
				if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("处理转发消息时，连接DBServer获取消息列表失败", new object[0]), null, true);
				}
				else if (null != tcpOutPacket)
				{
					List<string> chatMsgList = DataHelper.BytesToObject<List<string>>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
					Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
					if (chatMsgList != null && chatMsgList.Count > 0)
					{
						for (int i = 0; i < chatMsgList.Count; i++)
						{
							this.TransferChatMsg(chatMsgList[i]);
						}
					}
				}
			}
		}

		// Token: 0x060021F3 RID: 8691 RVA: 0x001D0810 File Offset: 0x001CEA10
		public void TransferChatMsg(string chatMsg)
		{
			try
			{
				string[] fields = chatMsg.Split(new char[]
				{
					':'
				});
				if (fields.Length == 9)
				{
					int roleID = Convert.ToInt32(fields[0]);
					string roleName = fields[1];
					int status = Convert.ToInt32(fields[2]);
					string toRoleName = fields[3];
					int index = Convert.ToInt32(fields[4]);
					string textMsg = fields[5];
					string chatType = fields[6];
					int extTag = Convert.ToInt32(fields[7]);
					int serverLineID = Convert.ToInt32(fields[8]);
					if (serverLineID != GameManager.ServerLineID)
					{
						if (!GameManager.systemGMCommands.ProcessChatMessage(null, null, textMsg, true))
						{
							if (index == 1)
							{
							}
							if (index == 2)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									roleID,
									roleName,
									0,
									toRoleName,
									index,
									textMsg,
									chatType
								});
								GameManager.ClientMgr.NotifyAllChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmdData, null);
							}
							else if (index == 3)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									roleID,
									roleName,
									0,
									toRoleName,
									index,
									textMsg,
									chatType
								});
								GameManager.ClientMgr.NotifyFactionChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, extTag, cmdData, null);
							}
							else if (index != 4)
							{
								if (index == 5)
								{
									GameManager.ClientMgr.NotifyClientChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, roleID, roleName, toRoleName, index, textMsg, chatType);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "处理GM(后台)指令异常,msg=" + ex.Message, ex, true);
			}
		}

		// Token: 0x060021F4 RID: 8692 RVA: 0x001D0A6C File Offset: 0x001CEC6C
		public void LogBatterEnemy(GameClient attacker, GameClient victim)
		{
			attacker.ClientData.RoleIDAttackebByMyself = victim.ClientData.RoleID;
			victim.ClientData.RoleIDAttackMe = attacker.ClientData.RoleID;
			GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(attacker, victim);
			GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(victim, attacker);
		}

		// Token: 0x060021F5 RID: 8693 RVA: 0x001D0AC4 File Offset: 0x001CECC4
		public int NotifyOtherInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy, int burst, int injure, double injurePercnet, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, bool dontEffectDSHide = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			if ((enemy as GameClient).ClientData.CurrentLifeV > 0)
			{
				this.LogBatterEnemy(client, enemy);
				if (injure <= 0)
				{
					if (0 == attackType)
					{
						RoleAlgorithm.AttackEnemy(client, enemy as GameClient, forceBurst, injurePercnet, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
					}
					else if (1 == attackType)
					{
						RoleAlgorithm.MAttackEnemy(client, enemy as GameClient, forceBurst, injurePercnet, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
					}
				}
				bool selfLifeChanged = false;
				if (injure > 0)
				{
					RoleRelifeLog relifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, string.Format("打人rid={0},rname={1}击中恢复", enemy.ClientData.RoleID, enemy.ClientData.RoleName));
					int lifeSteal = (int)RoleAlgorithm.GetLifeStealV(client);
					if (lifeSteal > 0 && client.ClientData.CurrentLifeV > 0 && client.ClientData.CurrentLifeV < client.ClientData.LifeV)
					{
						relifeLog.hpModify = true;
						relifeLog.oldHp = client.ClientData.CurrentLifeV;
						selfLifeChanged = true;
						client.ClientData.CurrentLifeV += lifeSteal;
					}
					if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					}
					relifeLog.newHp = client.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
					injure = this.InjureToEnemy(sl, pool, enemy, injure, attackType, ignoreDefenseAndDodge, skillLevel);
					injure = DBRoleBufferManager.ProcessAntiRole(client, enemy as GameClient, injure);
					injure /= 2;
				}
				if (enemy.buffManager.IsBuffEnabled(113))
				{
					BuffItemData buffItem = enemy.buffManager.GetBuffItemData(113);
					injure = (int)((double)injure * (1.0 - buffItem.buffValEx));
				}
				if (SceneUIClasses.Comp == Global.GetMapSceneType(client.ClientData.MapCode))
				{
					injure = CompManager.getInstance().FilterCompEnemyInjure(client, enemy, injure);
				}
				MapSettingItem hurtInfo = Data.GetMapSettingInfo(client.ClientData.MapCode);
				injure = (int)((double)injure * hurtInfo.NormalHuntNum);
				EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
				int nMerlinInjure = GameManager.MerlinInjureMgr.CalcMerlinInjure(client, enemy, injure, ref eMerlinType);
				int armorV = RoleAlgorithm.CallAttackArmor(client, enemy, ref injure, ref nMerlinInjure);
				int nRebornInjure = RebornManager.getInstance().CalcRebornInjure(client, enemy, injurePercnet, baseRate, ref burst);
				injure += (int)((double)nRebornInjure * hurtInfo.RebornHuntNum);
				if (injure > 0)
				{
					enemy.buffManager.SetStatusBuff(114, TimeUtil.NOW(), Data.FightStateTime, 0L);
				}
				if (!GameManager.TestGamePerformanceMode || !GameManager.TestGamePerformanceLockLifeV)
				{
					(enemy as GameClient).ClientData.CurrentLifeV -= Global.GMax(0, injure + nMerlinInjure);
				}
				(enemy as GameClient).ClientData.CurrentLifeV = Global.GMax((enemy as GameClient).ClientData.CurrentLifeV, 0);
				if (client.ClientData.ExcellenceProp[15] > 0.0)
				{
					int nRan = Global.GetRandomNumber(0, 101);
					if ((double)nRan <= client.ClientData.ExcellenceProp[15] * 100.0)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						selfLifeChanged = true;
					}
				}
				if (client.ClientData.ExcellenceProp[16] > 0.0)
				{
					int nRan = Global.GetRandomNumber(0, 101);
					if ((double)nRan <= client.ClientData.ExcellenceProp[16] * 100.0)
					{
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						selfLifeChanged = true;
					}
				}
				int enemyLife = (enemy as GameClient).ClientData.CurrentLifeV;
				(enemy as GameClient).UsingEquipMgr.InjuredSomebody(enemy as GameClient);
				this.SpriteInjure2Blood(sl, pool, client, injure);
				Point hitToGrid = new Point(-1.0, -1.0);
				if (nHitFlyDistance > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int nGridNum = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
					if (nGridNum > 0)
					{
						hitToGrid = ChuanQiUtils.HitFly(client, enemy, nGridNum);
					}
				}
				this.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as GameClient).ClientData.RoleID, burst, injure, (double)enemyLife, client.ClientData.Level, hitToGrid, nMerlinInjure, eMerlinType, armorV + 1);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.ClientData.RoleID, burst, injure, (double)enemyLife, 0L, nMerlinInjure, eMerlinType, armorV + 1);
				if (!dontEffectDSHide)
				{
					if (client.ClientData.DSHideStart > 0L)
					{
						Global.RemoveBufferData(client, 41);
						client.ClientData.DSHideStart = 0L;
						GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				if (enemy.ClientData.DSHideStart > 0L)
				{
					Global.RemoveBufferData(enemy, 41);
					enemy.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemy);
				}
				if (enemyLife <= 0)
				{
					Global.ProcessRoleDieForRoleAttack(sl, pool, client, enemy as GameClient);
				}
				GameManager.ClientMgr.ChangeRolePurpleName(sl, pool, client, enemy);
				Global.ProcessDamageThorn(sl, pool, client, enemy, injure);
				if (injure > 0)
				{
					enemy.passiveSkillModule.OnInjured(enemy);
					ZuoQiManager.getInstance().RoleDisMount(enemy, true);
				}
				if (selfLifeChanged)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(sl, pool, client, true, false, 7);
				}
				GameManager.damageMonitor.Out(client);
			}
			return injure;
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x001D1180 File Offset: 0x001CF380
		public void NotifyOtherInjured(SocketListener sl, TCPOutPacketPool pool, Monster monster, GameClient enemy, int burst, int injure, double injurePercnet, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			if (null != enemy)
			{
				if (enemy.ClientData.CurrentLifeV > 0)
				{
					enemy.ClientData.RoleIDAttackMe = monster.RoleID;
					GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy, monster);
					GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(monster, enemy);
					if (injure <= 0)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(monster, enemy as GameClient, false, 1.0, 0, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(monster, enemy as GameClient, false, 1.0, 0, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
					if (injure > 0)
					{
						injure = this.InjureToEnemy(sl, pool, enemy as GameClient, injure, attackType, ignoreDefenseAndDodge, skillLevel);
						injure = (int)((double)injure * injurePercnet);
					}
					MapSettingItem hurtInfo = Data.GetMapSettingInfo(enemy.ClientData.MapCode);
					injure = (int)((double)injure * hurtInfo.NormalHuntNum);
					EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
					int nMerlinInjure = GameManager.MerlinInjureMgr.CalcMerlinInjure(monster, enemy, injure, ref eMerlinType);
					int armorV = RoleAlgorithm.CallAttackArmor(monster, enemy, ref injure, ref nMerlinInjure);
					int nRebornInjure = RebornManager.getInstance().CalcRebornInjure(monster, enemy, injurePercnet, baseRate, ref burst);
					injure += (int)((double)nRebornInjure * hurtInfo.RebornHuntNum);
					injure = Global.GMax(0, injure + nMerlinInjure);
					if (enemy.buffManager.IsBuffEnabled(113))
					{
						BuffItemData buffItem = enemy.buffManager.GetBuffItemData(113);
						injure = (int)((double)injure * (1.0 - buffItem.buffValEx));
					}
					if (SceneUIClasses.Comp == Global.GetMapSceneType(enemy.ClientData.MapCode))
					{
						if (null != monster.OwnerClient)
						{
							injure = CompManager.getInstance().FilterCompEnemyInjure(monster.OwnerClient, enemy, injure);
						}
					}
					if (injure > 0)
					{
						enemy.buffManager.SetStatusBuff(114, TimeUtil.NOW(), Data.FightStateTime, 0L);
					}
					if (!GameManager.TestGamePerformanceMode || !GameManager.TestGamePerformanceLockLifeV)
					{
						(enemy as GameClient).ClientData.CurrentLifeV -= injure;
					}
					(enemy as GameClient).ClientData.CurrentLifeV = Global.GMax((enemy as GameClient).ClientData.CurrentLifeV, 0);
					int enemyLife = (enemy as GameClient).ClientData.CurrentLifeV;
					(enemy as GameClient).UsingEquipMgr.InjuredSomebody(enemy as GameClient);
					if (enemyLife <= 0)
					{
						if (null == monster.OwnerClient)
						{
							Global.ProcessRoleDieForMonsterAttack(sl, pool, monster, enemy);
						}
						else
						{
							Global.ProcessRoleDieForRoleAttack(sl, pool, monster.OwnerClient, enemy);
						}
					}
					Point hitToGrid = new Point(-1.0, -1.0);
					if (nHitFlyDistance > 0)
					{
						MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[(enemy as GameClient).ClientData.MapCode];
						int nGridNum = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
						if (nGridNum > 0)
						{
							hitToGrid = ChuanQiUtils.MonsterHitFly(monster, enemy, nGridNum);
						}
					}
					if (injure > 0)
					{
						enemy.passiveSkillModule.OnInjured(enemy);
					}
					this.NotifySpriteInjured(sl, pool, enemy as GameClient, monster.MonsterZoneNode.MapCode, monster.RoleID, (enemy as GameClient).ClientData.RoleID, burst, injure, (double)enemyLife, monster.MonsterInfo.VLevel, hitToGrid, nMerlinInjure, eMerlinType, armorV + 1);
					Global.ProcessDamageThorn(sl, pool, monster, enemy as GameClient, injure);
				}
			}
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x001D1580 File Offset: 0x001CF780
		public void SeekSpriteToLock(Monster monster)
		{
			if (monster.MonsterType == 1001)
			{
			}
			if (monster.MonsterInfo.SeekRange <= 0)
			{
				monster.VisibleItemList = null;
			}
			else if (monster.VLife <= 0.0)
			{
				monster.VisibleItemList = null;
			}
			else if (!MonsterManager.CanMonsterSeekRange(monster))
			{
				monster.VisibleItemList = null;
			}
			else
			{
				int viewRange = (2 * (Global.MaxCache9XGridNum - 1) + 1) * (2 * (Global.MaxCache9YGridNum - 1) + 1);
				Point grid = monster.CurrentGrid;
				int nCurrX = (int)grid.X;
				int nCurrY = (int)grid.Y;
				List<Point> searchList = SearchTable.GetSearchTableList();
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[monster.MonsterZoneNode.MapCode];
				monster.VisibleItemList = new List<VisibleItem>();
				int i = 0;
				while (i < viewRange && i < searchList.Count)
				{
					int nX = nCurrX + (int)searchList[i].X;
					int nY = nCurrY + (int)searchList[i].Y;
					List<object> objsList = mapGrid.FindObjects(nX, nY);
					if (null != objsList)
					{
						for (int j = 0; j < objsList.Count; j++)
						{
							if (null != objsList[j] as IObject)
							{
								if (monster.GetObjectID() != (objsList[j] as IObject).GetObjectID())
								{
									if (monster.CopyMapID > 0)
									{
										if (monster.CopyMapID != (objsList[j] as IObject).CurrentCopyMapID)
										{
											goto IL_2C4;
										}
									}
									if (objsList[j] is GameClient)
									{
										if (!Global.RoleIsVisible(objsList[j] as GameClient))
										{
											goto IL_2C4;
										}
									}
									else if (objsList[j] is Monster)
									{
										if (1001 != monster.MonsterType)
										{
											if ((objsList[j] as Monster).MonsterType <= 901)
											{
												if ((objsList[j] as Monster).Camp <= 0)
												{
													goto IL_2C4;
												}
											}
										}
									}
									monster.VisibleItemList.Add(new VisibleItem
									{
										ItemType = (objsList[j] as IObject).ObjectType,
										ItemID = (objsList[j] as IObject).GetObjectID()
									});
								}
							}
							IL_2C4:;
						}
					}
					i++;
				}
			}
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x001D1890 File Offset: 0x001CFA90
		public Point SeekMonsterPosition(GameClient client, int centerX, int centerY, int radiusGridNum, out int totalMonsterNum)
		{
			totalMonsterNum = 0;
			Point pt = new Point((double)centerX, (double)centerY);
			List<object> objsList = GameManager.MonsterMgr.GetObjectsByMap(client.ClientData.MapCode);
			Point result;
			if (null == objsList)
			{
				result = pt;
			}
			else
			{
				int radiusXY = radiusGridNum * 64;
				int lastDistance = int.MaxValue;
				Monster findMonster = null;
				int i = 0;
				while (i < objsList.Count)
				{
					if (objsList[i] is Monster)
					{
						Monster monster = objsList[i] as Monster;
						if (monster.VLife > 0.0 && monster.Alive)
						{
							if (1201 != monster.MonsterType)
							{
								if (Global.IsOpposition(client, monster))
								{
									if (monster.CopyMapID > 0)
									{
										if (monster.CopyMapID != client.ClientData.CopyMapID)
										{
											goto IL_154;
										}
									}
									if (Global.InCircle(monster.SafeCoordinate, pt, (double)radiusXY))
									{
										totalMonsterNum++;
										int distance = (int)Global.GetTwoPointDistance(pt, monster.SafeCoordinate);
										if (distance < lastDistance)
										{
											lastDistance = distance;
											findMonster = monster;
										}
									}
								}
							}
						}
					}
					IL_154:
					i++;
					continue;
					goto IL_154;
				}
				if (null != findMonster)
				{
					result = new Point(findMonster.SafeCoordinate.X, findMonster.SafeCoordinate.Y);
				}
				else
				{
					result = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				}
			}
			return result;
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x001D1A64 File Offset: 0x001CFC64
		public void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			this.LookupEnemiesInCircle(client, mapCode, toX, toY, radius, objList, -1);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as GameClient).ClientData.RoleID);
			}
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x001D1AC0 File Offset: 0x001CFCC0
		public void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> enemiesList, int nTargetType = -1)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client == null || client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || Global.IsOpposition(client, objsList[i] as GameClient) || nTargetType == 2)
							{
								if (client == null || client.ClientData.CopyMapID == (objsList[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
									if (Global.InCircle(target, center, (double)radius))
									{
										enemiesList.Add(objsList[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x001D1C38 File Offset: 0x001CFE38
		public void LookupEnemiesInCircle(int mapCode, int copyMapCode, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (copyMapCode == (objsList[i] as GameClient).ClientData.CopyMapID)
						{
							Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
							if (Global.InCircle(target, center, (double)radius))
							{
								enemiesList.Add(objsList[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x001D1D38 File Offset: 0x001CFF38
		public void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> objList = new List<object>();
			this.LookupEnemiesInCircleByAngle(client, direction, mapCode, toX, toY, radius, objList, angle, near180);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as GameClient).ClientData.RoleID);
			}
		}

		// Token: 0x060021FD RID: 8701 RVA: 0x001D1D98 File Offset: 0x001CFF98
		public void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeAngle((double)client.ClientData.RoleYAngle, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || Global.IsOpposition(client, objsList[i] as GameClient))
							{
								if (client == null || client.ClientData.CopyMapID == (objsList[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
									if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(objsList[i]);
									}
									else if (Global.InCircle(target, center, 100.0))
									{
										enemiesList.Add(objsList[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060021FE RID: 8702 RVA: 0x001D1F70 File Offset: 0x001D0170
		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapCode, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> objList = new List<object>();
			this.LookupEnemiesInCircleByAngle(direction, mapCode, copyMapCode, toX, toY, radius, objList, angle, near180);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as GameClient).ClientData.RoleID);
			}
		}

		// Token: 0x060021FF RID: 8703 RVA: 0x001D1FD0 File Offset: 0x001D01D0
		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapCode, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double loAngleNear = 0.0;
				double hiAngleNear = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out loAngleNear, out hiAngleNear);
				int nAddRadius = 100;
				if (JingJiChangManager.getInstance().IsJingJiChangMap(mapCode))
				{
					nAddRadius = 200;
				}
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (copyMapCode == (objsList[i] as GameClient).ClientData.CopyMapID)
						{
							Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
							if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(objsList[i]);
							}
							else if (Global.InCircle(target, center, (double)nAddRadius))
							{
								enemiesList.Add(objsList[i]);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002200 RID: 8704 RVA: 0x001D2178 File Offset: 0x001D0378
		public void LookupRolesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(toX, toY, radius);
			if (null != objsList)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client == null || client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || client.ClientData.CopyMapID == (objsList[i] as GameClient).ClientData.CopyMapID)
							{
								Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
								if (Global.InCircle(target, center, (double)radius))
								{
									rolesList.Add(objsList[i]);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002201 RID: 8705 RVA: 0x001D22C4 File Offset: 0x001D04C4
		public void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(client.ClientData.PosX, client.ClientData.PosY, radius);
			if (null != objsList)
			{
				Point source = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				Point toPos = Global.GetAPointInCircle(source, radius, client.ClientData.RoleYAngle);
				int toX = (int)toPos.X;
				int toY = (int)toPos.Y;
				Point center = default(Point);
				center.X = (double)((client.ClientData.PosX + toX) / 2);
				center.Y = (double)((client.ClientData.PosY + toY) / 2);
				int fDirectionX = toX - client.ClientData.PosX;
				int fDirectionY = toY - client.ClientData.PosY;
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if ((objsList[i] as GameClient).ClientData.LifeV > 0)
						{
							if (client == null || client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
							{
								if (client == null || client.ClientData.CopyMapID == (objsList[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
									if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
									{
										rolesList.Add(objsList[i]);
									}
									else if (Global.InCircle(target, source, 100.0))
									{
										rolesList.Add(objsList[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002202 RID: 8706 RVA: 0x001D2514 File Offset: 0x001D0714
		public void LookupRolesInSquare(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(srcX, srcY, radius);
			if (null != objsList)
			{
				Point source = new Point((double)srcX, (double)srcY);
				Point center = default(Point);
				center.X = (double)((srcX + toX) / 2);
				center.Y = (double)((srcY + toY) / 2);
				int fDirectionX = toX - srcX;
				int fDirectionY = toY - srcY;
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if ((objsList[i] as GameClient).ClientData.LifeV > 0)
						{
							if (copyMapId == (objsList[i] as GameClient).ClientData.CopyMapID)
							{
								Point target = new Point((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY);
								if (Global.InSquare(center, target, radius, nWidth, fDirectionX, fDirectionY))
								{
									rolesList.Add(objsList[i]);
								}
								else if (Global.InCircle(target, source, 100.0))
								{
									rolesList.Add(objsList[i]);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002203 RID: 8707 RVA: 0x001D26A8 File Offset: 0x001D08A8
		public void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> objsList = mapGrid.FindObjects(gridX, gridY);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (objsList[i] as GameClient).ClientData.CopyMapID)
						{
							enemiesList.Add(objsList[i]);
						}
					}
				}
			}
		}

		// Token: 0x06002204 RID: 8708 RVA: 0x001D2754 File Offset: 0x001D0954
		public void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int mapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			Point grid = attacker.CurrentGrid;
			int gridX = (int)grid.X;
			int gridY = (int)grid.Y;
			Point p = Global.GetGridPointByDirection(direction, gridX, gridY);
			this.LookupEnemiesAtGridXY(attacker, (int)p.X, (int)p.Y, enemiesList);
		}

		// Token: 0x06002205 RID: 8709 RVA: 0x001D27B8 File Offset: 0x001D09B8
		public void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> objList = new List<object>();
			this.LookupAttackEnemies(attacker, direction, objList);
			for (int i = 0; i < objList.Count; i++)
			{
				enemiesList.Add((objList[i] as GameClient).ClientData.RoleID);
			}
		}

		// Token: 0x06002206 RID: 8710 RVA: 0x001D280C File Offset: 0x001D0A0C
		public void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridList = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridList.Count > 0)
			{
				for (int i = 0; i < gridList.Count; i++)
				{
					this.LookupEnemiesAtGridXY(obj, (int)gridList[i].X, (int)gridList[i].Y, enemiesList);
				}
			}
		}

		// Token: 0x06002207 RID: 8711 RVA: 0x001D28AC File Offset: 0x001D0AAC
		public int InjureToEnemy(SocketListener sl, TCPOutPacketPool pool, GameClient enemy, int injured, int attackType, bool ignoreDefenseAndDodge, int skillLevel)
		{
			double totalSubValue = 0.0;
			totalSubValue += enemy.RoleMagicHelper.GetSubInjure();
			totalSubValue += enemy.RoleMagicHelper.MU_GetSubInjure2();
			injured = (int)((double)injured - totalSubValue);
			injured = (int)((double)injured * (1.0 - enemy.RoleMagicHelper.GetSubInjure1()) * (1.0 - enemy.RoleMagicHelper.MU_GetSubInjure1()));
			int result;
			if (injured <= 0)
			{
				result = 0;
			}
			else
			{
				double percent = enemy.RoleMagicHelper.GetInjure2Magic();
				if (percent > 0.0)
				{
					double magicV = percent * (double)injured;
					magicV = Global.GMin(magicV, (double)injured);
					magicV = Global.GMin((double)enemy.ClientData.CurrentMagicV, magicV);
					injured -= (int)magicV;
					this.SubSpriteMagicV(sl, pool, enemy, magicV);
				}
				double injured2Magic = enemy.RoleMagicHelper.GetNewInjure2Magic();
				if (injured2Magic > 0.0)
				{
					injured2Magic = Global.GMin(injured2Magic, (double)injured);
					injured2Magic = Global.GMin((double)enemy.ClientData.CurrentMagicV, injured2Magic);
					injured -= (int)injured2Magic;
					this.SubSpriteMagicV(sl, pool, enemy, injured2Magic);
				}
				percent = enemy.RoleMagicHelper.GetNewInjure2Magic3();
				if (percent > 0.0)
				{
					double magicV = percent * (double)injured;
					magicV = Global.GMin(magicV, (double)injured);
					magicV = Global.GMin((double)enemy.ClientData.CurrentMagicV, magicV);
					injured -= (int)magicV;
					this.SubSpriteMagicV(sl, pool, enemy, magicV);
				}
				percent = enemy.RoleMagicHelper.GetNewMagicSubInjure();
				if (percent > 0.0)
				{
					if (0 == attackType)
					{
						if (ignoreDefenseAndDodge)
						{
							skillLevel = Math.Min(skillLevel, ClientManager.IgnoreDefenseAndDogeSubPercent.Length - 1);
							skillLevel = Math.Max(0, skillLevel);
							percent = Math.Min(percent, ClientManager.IgnoreDefenseAndDogeSubPercent[skillLevel]);
						}
					}
					double magicV = percent * (double)injured;
					magicV = Global.GMin(magicV, (double)injured);
					magicV = Global.GMin((double)enemy.ClientData.CurrentMagicV, magicV);
					injured -= (int)magicV;
				}
				injured = DBRoleBufferManager.ProcessHuZhaoSubLifeV(enemy, Math.Max(0, injured));
				injured = DBRoleBufferManager.ProcessWuDiHuZhaoNoInjured(enemy, Math.Max(0, injured));
				result = Math.Max(0, injured);
			}
			return result;
		}

		// Token: 0x06002208 RID: 8712 RVA: 0x001D2AF4 File Offset: 0x001D0CF4
		public void SpriteInjure2Blood(SocketListener sl, TCPOutPacketPool pool, GameClient client, int injured)
		{
			double percent = client.RoleMagicHelper.GetInjure2Life();
			if (0.0 < percent)
			{
				injured = (int)((double)injured * percent);
				this.AddSpriteLifeV(sl, pool, client, (double)injured, "击中恢复");
			}
		}

		// Token: 0x06002209 RID: 8713 RVA: 0x001D2B3C File Offset: 0x001D0D3C
		public bool NotifyChangeMap(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toMapCode, int maxX = -1, int mapY = -1, int direction = -1, int relife = 0)
		{
			bool isKuaFuMap = KuaFuManager.getInstance().IsKuaFuMap(toMapCode);
			if (client.CheckCheatData.GmGotoShadowMapCode != toMapCode && client.ClientData.WaitingChangeMapToMapCode != toMapCode)
			{
				if (client.ClientSocket.IsKuaFuLogin && client.ClientData.KuaFuChangeMapCode != toMapCode)
				{
					KuaFuManager.getInstance().GotoLastMap(client);
					return true;
				}
				if (client.ClientSocket.IsKuaFuLogin != isKuaFuMap)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("GotoMap denied, mapCode={0},IsKuaFuLogin={1}", toMapCode, client.ClientSocket.IsKuaFuLogin), null, true);
					return false;
				}
				if (LingDiCaiJiManager.getInstance().GetLingDiType(toMapCode) != 2)
				{
					return false;
				}
			}
			client.ClientData.WaitingNotifyChangeMap = true;
			client.ClientData.WaitingChangeMapToMapCode = toMapCode;
			client.ClientData.WaitingChangeMapToPosX = maxX;
			client.ClientData.WaitingChangeMapToPosY = mapY;
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastNotifyChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							toMapCode,
							maxX,
							mapY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastNotifyChangeMapTicks = TimeUtil.NOW();
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				toMapCode,
				maxX,
				mapY,
				direction,
				relife
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 160);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
			return true;
		}

		// Token: 0x0600220A RID: 8714 RVA: 0x001D2DB4 File Offset: 0x001D0FB4
		public bool ChangeMap(SocketListener sl, TCPOutPacketPool pool, GameClient client, int teleport, int toMapCode, int toMapX, int toMapY, int toMapDirection, int nID)
		{
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							toMapCode,
							toMapX,
							toMapY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastChangeMapTicks = TimeUtil.NOW();
			client.ClientData.SceneType = SceneUIClasses.UnDefined;
			client.ClientData.SceneMapCode = 0;
			GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
			if (toMapCode > 0)
			{
				GameMap gameMap = GameManager.MapMgr.GetGameMap(toMapCode);
				if (null != gameMap)
				{
					if (!gameMap.CanMove(toMapX / gameMap.MapGridWidth, toMapY / gameMap.MapGridHeight))
					{
						toMapX = -1;
						toMapY = -1;
					}
				}
				else
				{
					toMapCode = -1;
				}
			}
			if (teleport >= 0)
			{
				Global.HandleBiaoCheChangMap(client, toMapCode, toMapX, toMapY, toMapDirection);
			}
			List<object> objsList = Global.GetAll9Clients(client);
			GameManager.ClientMgr.NotifyOthersLeave(sl, pool, client, objsList);
			if (client.ClientData.MapCode != toMapCode)
			{
				GlobalEventSource4Scene.getInstance().fireEvent(new EventObjectEx(65, new object[]
				{
					client,
					client.ClientData.MapCode,
					toMapCode
				}), 10000);
			}
			if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				if (!client.ClientSocket.IsKuaFuLogin || Global.GetMapSceneType(client.ClientData.MapCode) != Global.GetMapSceneType(toMapCode))
				{
					int toFuBenID = FuBenManager.FindFuBenIDByMapCode(toMapCode);
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
					if (fuBenInfoItem != null && toFuBenID != fuBenInfoItem.FuBenID)
					{
						GlobalEventSource.getInstance().fireEvent(new PlayerLeaveFuBenEventObject(client));
						SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
						KuaFuManager.getInstance().OnLeaveScene(client, sceneType, false);
					}
					if (-1 == toFuBenID)
					{
						if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
						{
							GameManager.BloodCastleCopySceneMgr.LeaveBloodCastCopyScene(client, true);
						}
						else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
						{
							GameManager.DaimonSquareCopySceneMgr.LeaveDaimonSquareCopyScene(client, true);
						}
						client.ClientData.FuBenSeqID = -1;
						client.ClientData.FuBenID = -1;
						FuBenManager.RemoveFuBenSeqID(client.ClientData.RoleID);
					}
					else if (null != fuBenInfoItem)
					{
						if (toFuBenID != fuBenInfoItem.FuBenID)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("外挂利用直接切换地图的操作，进行副本地图切换: Cmd={0}, RoleID={1}, 关闭连接", (TCPGameServerCmds)nID, client.ClientData.RoleID), null, true);
							return false;
						}
					}
				}
			}
			if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
			{
				Global.QuitFromTeam(client);
			}
			Global.ClearCopyMap(client, false);
			GameManager.ClientMgr.RemoveClientFromContainer(client);
			if (toMapX <= 0 || toMapY <= 0)
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosY;
				int defaultBirthRadius = GameManager.MapMgr.DictMaps[toMapCode].BirthRadius;
				if (Global.IsHuangChengMapCode(toMapCode))
				{
					Global.GetHuangChengMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref defaultBirthRadius);
				}
				else if (toMapCode == GameManager.BattleMgr.BattleMapCode)
				{
					Global.GetLastBattleSideInfo(client);
					Global.GetBattleMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref defaultBirthRadius);
				}
				Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, toMapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
				toMapX = (int)newPos.X;
				toMapY = (int)newPos.Y;
			}
			if (client.ClientData.MapCode == GameManager.BattleMgr.BattleMapCode && toMapCode != GameManager.BattleMgr.BattleMapCode)
			{
				GameManager.BattleMgr.LeaveBattleMap(client, true);
			}
			else if (client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode && toMapCode != GameManager.ArenaBattleMgr.BattleMapCode)
			{
				GameManager.ArenaBattleMgr.LeaveArenaBattleMap(client);
			}
			else if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhenMap(client.ClientData.MapCode))
			{
				LuoLanFaZhenCopySceneManager.OnLeaveFubenMap(client, toMapCode);
			}
			else if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
			{
				this.UserFullLife(client, "离开竞技场", false);
			}
			else if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode && toMapCode != GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
			{
				GameManager.AngelTempleMgr.LeaveAngelTempleScene(client, false);
			}
			Global.RemoveBufferData(client, 85);
			Global.RemoveBufferData(client, 86);
			Global.ProcessLeaveGuMuMap(client);
			if (!WanMotaCopySceneManager.IsWanMoTaMapCode(client.ClientData.MapCode) && SceneUIClasses.YaoSaiWorld != Global.GetMapSceneType(client.ClientData.MapCode))
			{
				client.ClientData.LastMapCode = client.ClientData.MapCode;
				client.ClientData.LastPosX = client.ClientData.PosX;
				client.ClientData.LastPosY = client.ClientData.PosY;
			}
			client.ClientData.WaitingForChangeMap = true;
			int oldMapCode = client.ClientData.MapCode;
			client.ClientData.MapCode = toMapCode;
			client.ClientData.PosX = toMapX;
			client.ClientData.PosY = toMapY;
			client.ClientData.ReportPosTicks = 0L;
			client.ClientData.CurrentAction = 0;
			client.ClearVisibleObjects(true);
			client.ClientData.DestPoint = new Point(-1.0, -1.0);
			if (toMapDirection > 0)
			{
				client.ClientData.RoleDirection = toMapDirection;
			}
			else
			{
				toMapDirection = client.ClientData.RoleDirection;
			}
			Global.InitCopyMap(client);
			GameManager.ClientMgr.AddClientToContainer(client);
			SceneUIClasses sceneType2 = Global.GetMapSceneType(oldMapCode);
			if (sceneType2 == SceneUIClasses.ShuiJingHuanJing)
			{
				client.ClientData.ShuiJingHuanJingTicks = TimeUtil.NOW() * 10000L;
			}
			bool result;
			if (!GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].MoveObject(-1, -1, client.ClientData.PosX, client.ClientData.PosY, client))
			{
				LogManager.WriteLog(LogTypes.Warning, string.Format("精灵移动超出了地图边界: Cmd={0}, RoleID={1}, 关闭连接", (TCPGameServerCmds)nID, client.ClientData.RoleID), null, true);
				result = false;
			}
			else
			{
				TeamData td = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != td)
				{
					if (null != td.TeamRoles)
					{
						for (int i = 0; i < td.TeamRoles.Count; i++)
						{
							if (td.TeamRoles[i].RoleID == client.ClientData.RoleID)
							{
								td.TeamRoles[i].MapCode = toMapCode;
								break;
							}
						}
						GameManager.ClientMgr.NotifyTeamData(sl, pool, td);
					}
				}
				Global.BroadcastEnterLaoFangHint(client, toMapCode);
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode)
				{
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhenMap(client.ClientData.MapCode))
					{
						LuoLanFaZhenCopySceneManager.OnEnterFubenMap(client, oldMapCode, false);
					}
				}
				Global.ProcessLimitFuBenMapNotifyMsg(client);
				client.ClearChangeGrid();
				Global.AddMapEvent(client);
				ClientCmdCheck.RecordClientPosition(client);
				client.CheckCheatData.LastNotifyLeaveGuMuTick = 0L;
				Monster monster = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
				if (null != monster)
				{
					Global.SystemKillSummonMonster(client, MonsterTypes.DSPetMonster);
					GameManager.LuaMgr.CallMonstersForGameClient(client, monster.MonsterInfo.ExtensionID, monster.CurrentMagicLevel, monster.SurvivalTime / 1000, 1001, 1);
				}
				SCMapChange scData = new SCMapChange(client.ClientData.RoleID, teleport, toMapCode, toMapX, toMapY, toMapDirection, 0);
				client.sendCmd<SCMapChange>(123, scData, false);
				result = true;
			}
			return result;
		}

		// Token: 0x0600220B RID: 8715 RVA: 0x001D3734 File Offset: 0x001D1934
		public bool ChangePosition(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toMapX, int toMapY, int toMapDirection, int nID, int animation = 0)
		{
			if (2 != animation)
			{
				GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
				if (toMapX <= 0 || toMapY <= 0)
				{
					int defaultBirthPosX = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosX;
					int defaultBirthPosY = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosY;
					int defaultBirthRadius = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].BirthRadius;
					Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
					toMapX = (int)newPos.X;
					toMapY = (int)newPos.Y;
				}
				int oldX = client.ClientData.PosX;
				int oldY = client.ClientData.PosY;
				client.ClientData.PosX = toMapX;
				client.ClientData.PosY = toMapY;
				client.ClientData.ReportPosTicks = 0L;
				if (toMapDirection > 0)
				{
					client.ClientData.RoleDirection = toMapDirection;
				}
				if (!GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].MoveObject(-1, -1, client.ClientData.PosX, client.ClientData.PosY, client))
				{
					client.ClientData.PosX = oldX;
					client.ClientData.PosY = oldY;
					client.ClientData.ReportPosTicks = 0L;
				}
				ClientManager.DoSpriteMapGridMove(client, 0);
				ClientCmdCheck.RecordClientPosition(client);
			}
			List<object> objsList = Global.GetAll9Clients(client);
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					toMapX,
					toMapY,
					toMapDirection,
					animation
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, nID);
				result = true;
			}
			return result;
		}

		// Token: 0x0600220C RID: 8716 RVA: 0x001D3968 File Offset: 0x001D1B68
		public bool ChangePosition2(SocketListener sl, TCPOutPacketPool pool, IObject obj, int roleID, int mapCode, int copyMapID, int toMapX, int toMapY, int toMapDirection, List<object> objsList)
		{
			int nID = 159;
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, toMapX, toMapY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (objsList == null)
			{
				result = true;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					toMapX,
					toMapY,
					toMapDirection
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, nID);
				result = true;
			}
			return result;
		}

		// Token: 0x0600220D RID: 8717 RVA: 0x001D3A18 File Offset: 0x001D1C18
		public void NotifySelfAddGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int nNewHint, int isPackUp = 0)
		{
			AddGoodsData addGoodsData = new AddGoodsData
			{
				roleID = client.ClientData.RoleID,
				id = goodsData.Id,
				goodsID = goodsData.GoodsID,
				forgeLevel = goodsData.Forge_level,
				quality = goodsData.Quality,
				goodsNum = goodsData.GCount,
				binding = goodsData.Binding,
				site = goodsData.Site,
				jewellist = goodsData.Jewellist,
				newHint = nNewHint,
				newEndTime = goodsData.Endtime,
				addPropIndex = goodsData.AddPropIndex,
				bornIndex = goodsData.BornIndex,
				lucky = goodsData.Lucky,
				strong = goodsData.Strong,
				ExcellenceProperty = goodsData.ExcellenceInfo,
				nAppendPropLev = goodsData.AppendPropLev,
				ChangeLifeLevForEquip = goodsData.ChangeLifeLevForEquip,
				bagIndex = goodsData.BagIndex,
				washProps = goodsData.WashProps,
				ElementhrtsProps = goodsData.ElementhrtsProps,
				juHunLevel = goodsData.JuHunID,
				PackUp = isPackUp
			};
			byte[] bytes = DataHelper.ObjectToBytes<AddGoodsData>(addGoodsData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 130);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600220E RID: 8718 RVA: 0x001D3B84 File Offset: 0x001D1D84
		public void NotifySelfAddGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewellist, int newHint, string newEndTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int ChangeLifeLevForEquip = 0, int bagIndex = 0, List<int> washProps = null, List<int> elementhrtsProps = null, int juHun_level = 0, string prop = "")
		{
			newEndTime = newEndTime.Replace(":", "$");
			AddGoodsData addGoodsData = new AddGoodsData
			{
				roleID = client.ClientData.RoleID,
				id = id,
				goodsID = goodsID,
				forgeLevel = forgeLevel,
				quality = quality,
				goodsNum = goodsNum,
				binding = binding,
				site = site,
				jewellist = jewellist,
				newHint = newHint,
				newEndTime = newEndTime,
				addPropIndex = addPropIndex,
				bornIndex = bornIndex,
				lucky = lucky,
				strong = strong,
				ExcellenceProperty = ExcellenceProperty,
				nAppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = ChangeLifeLevForEquip,
				bagIndex = bagIndex,
				washProps = washProps,
				ElementhrtsProps = elementhrtsProps,
				juHunLevel = juHun_level,
				prop = prop
			};
			byte[] bytes = DataHelper.ObjectToBytes<AddGoodsData>(addGoodsData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytes, 0, bytes.Length, 130);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600220F RID: 8719 RVA: 0x001D3C9C File Offset: 0x001D1E9C
		public void NotifyModGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int modType, int id, int isusing, int site, int gcount, int bagIndex, int newHint)
		{
			SCModGoods scData = new SCModGoods(0, modType, id, isusing, site, gcount, bagIndex, newHint);
			client.sendCmd<SCModGoods>(131, scData, false);
		}

		// Token: 0x06002210 RID: 8720 RVA: 0x001D3CD0 File Offset: 0x001D1ED0
		public void NotifyMoveGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData gd, int moveType)
		{
			if (0 == moveType)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, gd.Id);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 172);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 0, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
			}
		}

		// Token: 0x06002211 RID: 8721 RVA: 0x001D3DE4 File Offset: 0x001D1FE4
		public void NotifyGoodsInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData gd)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, gd.Id, gd.Lucky);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 426);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002212 RID: 8722 RVA: 0x001D3E48 File Offset: 0x001D2048
		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, bool usingGoods, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, 1, usingGoods, dontCalcLimitNum);
		}

		// Token: 0x06002213 RID: 8723 RVA: 0x001D3E9C File Offset: 0x001D209C
		public bool NotifyUseGoodsByDbId(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int useCount, bool usingGoods, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, useCount, usingGoods, dontCalcLimitNum);
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x001D3EF0 File Offset: 0x001D20F0
		public bool NotifyUseGoodsByDbId(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int useCount, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData.GoodsID, useCount, usingGoods, out usedBinding, out usedTimeLimited, dontCalcLimitNum);
		}

		// Token: 0x06002215 RID: 8725 RVA: 0x001D3F4C File Offset: 0x001D214C
		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int useCount, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool ret = false;
			lock (client.ClientData.GoodsDataList)
			{
				if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
				{
					return ret;
				}
				if (!usedBinding)
				{
					usedBinding = (goodsData.Binding > 0);
				}
				if (!usedTimeLimited)
				{
					usedTimeLimited = Global.IsTimeLimitGoods(goodsData);
				}
				ret = this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, useCount, usingGoods, dontCalcLimitNum);
			}
			return ret;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x001D400C File Offset: 0x001D220C
		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int subNum, bool usingGoods, bool dontCalcLimitNum = false)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				if (!dontCalcLimitNum)
				{
					if (!Global.HasEnoughGoodsDayUseNum(client, goodsData.GoodsID, subNum))
					{
						return false;
					}
				}
				if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
				{
					result = false;
				}
				else if (goodsData.GCount <= 0)
				{
					result = false;
				}
				else if (goodsData.GCount < subNum)
				{
					result = false;
				}
				else if (subNum <= 0)
				{
					result = false;
				}
				else
				{
					List<MagicActionItem> magicActionItemList = null;
					int categoriy = 0;
					if (usingGoods)
					{
						int verifyResult = UsingGoods.ProcessUsingGoodsVerify(client, goodsData.GoodsID, goodsData.Binding, out magicActionItemList, out categoriy, subNum);
						if (verifyResult < 0)
						{
							return false;
						}
						if (verifyResult == 0)
						{
							for (int i = 0; i < magicActionItemList.Count; i++)
							{
								if (magicActionItemList[i].MagicActionID == MagicActionIDs.UP_LEVEL)
								{
									int nLev = 0;
									int nAddValue = (int)magicActionItemList[i].MagicActionParams[0];
									bool bCanUp = true;
									if (nAddValue > 0)
									{
										if (client.ClientData.ChangeLifeCount > GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
										{
											bCanUp = false;
										}
										else if (client.ClientData.ChangeLifeCount == GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
										{
											ChangeLifeDataInfo infoTmp = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, 0);
											if (infoTmp == null)
											{
												bCanUp = false;
											}
											else
											{
												nLev = infoTmp.NeedLevel;
												if (client.ClientData.Level >= nLev)
												{
													bCanUp = false;
												}
											}
										}
										else
										{
											ChangeLifeDataInfo infoTmp = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, client.ClientData.ChangeLifeCount + 1);
											if (infoTmp == null)
											{
												bCanUp = false;
											}
											else
											{
												nLev = infoTmp.NeedLevel;
												if (client.ClientData.Level >= nLev)
												{
													bCanUp = false;
												}
											}
										}
										if (!bCanUp)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(64, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
										if (client.ClientData.Level + nAddValue > nLev)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(65, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
										if (client.ClientData.CurrentLifeV <= 0)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(66, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
									}
								}
								else if (magicActionItemList[i].MagicActionID == MagicActionIDs.ADD_GOODWILL)
								{
									if (!MarriageOtherLogic.getInstance().CanAddMarriageGoodWill(client))
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(67, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										return false;
									}
								}
								else if (magicActionItemList[i].MagicActionID == MagicActionIDs.MU_GETSHIZHUANG)
								{
									int fashionID = (int)magicActionItemList[i].MagicActionParams[0];
									if (!FashionManager.getInstance().FashionCanAdd(client, fashionID))
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(68, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										return false;
									}
								}
							}
						}
						else if (verifyResult == 1)
						{
							usingGoods = false;
						}
					}
					int gcount = goodsData.GCount;
					gcount = goodsData.GCount - subNum;
					string[] dbFields = null;
					string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
					{
						client.ClientData.RoleID,
						goodsData.Id,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						gcount,
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
					TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields, client.ServerId);
					if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
					{
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-1, goodsData.Id, gcount), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = false;
					}
					else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
					{
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-2, goodsData.Id, gcount), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = false;
					}
					else
					{
						if (gcount > 0)
						{
							goodsData.GCount = gcount;
						}
						else if (3000 == goodsData.Site || 3001 == goodsData.Site)
						{
							goodsData.GCount = 0;
							ElementhrtsManager.RemoveElementhrtsData(client, goodsData);
						}
						else if (7000 == goodsData.Site)
						{
							goodsData.GCount = 0;
							GameManager.FluorescentGemMgr.RemoveFluorescentGemData(client, goodsData);
						}
						else if (8000 == goodsData.Site)
						{
							goodsData.GCount = 0;
							SingletonTemplate<SoulStoneManager>.Instance().RemoveSoulStoneGoods(client, goodsData, goodsData.Site);
						}
						else if (11000 == goodsData.Site)
						{
							goodsData.GCount = gcount;
							ShenShiManager.UpdateFuWenGoodsData(client, goodsData);
						}
						else if (goodsData.Site == 16000)
						{
							goodsData.GCount = 0;
							MountHolyStampManager.RemoveGoodsData(client, goodsData);
						}
						else
						{
							goodsData.GCount = 0;
							if (RebornEquip.IsRebornType(goodsData.GoodsID))
							{
								RebornEquip.RemoveGoodsData(client, goodsData);
							}
							else
							{
								Global.RemoveGoodsData(client, goodsData);
							}
						}
						if (usingGoods)
						{
							UsingGoods.ProcessUsingGoods(client, goodsData.GoodsID, goodsData.Binding, magicActionItemList, categoriy, subNum);
						}
						if (!dontCalcLimitNum)
						{
							Global.AddGoodsLimitNum(client, goodsData.GoodsID, subNum);
						}
						Global.ModRoleGoodsEvent(client, goodsData, -subNum, "物品使用", false);
						EventLogManager.AddGoodsEvent(client, OpTypes.AddOrSub, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, -subNum, goodsData.GCount, "物品使用");
						SevenDayGoalEventObject evObj = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.UseGoodsCount);
						evObj.Arg1 = goodsData.GoodsID;
						evObj.Arg2 = subNum;
						GlobalEventSource.getInstance().fireEvent(evObj);
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(0, goodsData.Id, gcount), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06002217 RID: 8727 RVA: 0x001D48D8 File Offset: 0x001D2AD8
		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool ret = false;
			int count = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (!usedBinding)
							{
								usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
							}
							if (!usedTimeLimited)
							{
								usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
							}
							int gcount = client.ClientData.GoodsDataList[i].GCount;
							int subNum = Global.GMin(gcount, totalNum - count);
							ret = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], subNum, usingGoods, dontCalcLimitNum);
							if (!ret)
							{
								break;
							}
							count += subNum;
							if (count >= totalNum)
							{
								break;
							}
							if (subNum >= gcount)
							{
								i--;
							}
						}
					}
					IL_160:
					i++;
					continue;
					goto IL_160;
				}
			}
			return ret;
		}

		// Token: 0x06002218 RID: 8728 RVA: 0x001D4AA4 File Offset: 0x001D2CA4
		public bool NotifyUseBindGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool ret = false;
			int count = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (client.ClientData.GoodsDataList[i].Binding >= 1)
							{
								if (!usedBinding)
								{
									usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
								}
								if (!usedTimeLimited)
								{
									usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
								}
								int gcount = client.ClientData.GoodsDataList[i].GCount;
								int subNum = Global.GMin(gcount, totalNum - count);
								ret = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], subNum, usingGoods, dontCalcLimitNum);
								if (!ret)
								{
									break;
								}
								count += subNum;
								if (count >= totalNum)
								{
									break;
								}
								if (subNum >= gcount)
								{
									i--;
								}
							}
						}
					}
					IL_189:
					i++;
					continue;
					goto IL_189;
				}
			}
			return ret;
		}

		// Token: 0x06002219 RID: 8729 RVA: 0x001D4C98 File Offset: 0x001D2E98
		public bool NotifyUseNotBindGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool ret = false;
			int count = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (client.ClientData.GoodsDataList[i].Binding <= 0)
							{
								if (!usedBinding)
								{
									usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
								}
								if (!usedTimeLimited)
								{
									usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
								}
								int gcount = client.ClientData.GoodsDataList[i].GCount;
								int subNum = Global.GMin(gcount, totalNum - count);
								ret = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], subNum, usingGoods, dontCalcLimitNum);
								if (!ret)
								{
									break;
								}
								count += subNum;
								if (count >= totalNum)
								{
									break;
								}
								if (subNum >= gcount)
								{
									i--;
								}
							}
						}
					}
					IL_189:
					i++;
					continue;
					goto IL_189;
				}
			}
			return ret;
		}

		// Token: 0x0600221A RID: 8730 RVA: 0x001D4E8C File Offset: 0x001D308C
		public bool FallRoleGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
			{
				result = false;
			}
			else if (goodsData.GCount <= 0)
			{
				result = false;
			}
			else
			{
				int gcount = goodsData.GCount;
				int subNum = 1;
				if (Global.GetGoodsDefaultCount(goodsData.GoodsID) > 1)
				{
					subNum = goodsData.GCount;
				}
				gcount = goodsData.GCount - subNum;
				string[] dbFields = null;
				string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					goodsData.Id,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					gcount,
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
				TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields, client.ServerId);
				if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-1, goodsData.Id, gcount), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = false;
				}
				else if (dbFields.Length <= 0 || Convert.ToInt32(dbFields[1]) < 0)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-2, goodsData.Id, gcount), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = false;
				}
				else
				{
					if (gcount > 0)
					{
						goodsData.GCount = gcount;
					}
					else
					{
						goodsData.GCount = 0;
						Global.RemoveGoodsData(client, goodsData);
					}
					Global.ModRoleGoodsEvent(client, goodsData, -subNum, "物品掉落", false);
					EventLogManager.AddGoodsEvent(client, OpTypes.AddOrSub, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, -subNum, goodsData.GCount, "物品掉落");
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(0, goodsData.Id, gcount), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600221B RID: 8731 RVA: 0x001D518C File Offset: 0x001D338C
		public void NotifySelfPropertyValue(GameClient client, int moneyType, long value)
		{
			client.sendCmd<SCPropertyChange>(719, new SCPropertyChange
			{
				RoleID = client.ClientData.RoleID,
				MoneyType = moneyType,
				Value = value
			}, false);
		}

		// Token: 0x0600221C RID: 8732 RVA: 0x001D51CD File Offset: 0x001D33CD
		public void NotifyOthersPropertyValue(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600221D RID: 8733 RVA: 0x001D51D8 File Offset: 0x001D33D8
		public void NotifySelfMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Money1, client.ClientData.Money2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 138);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600221E RID: 8734 RVA: 0x001D5244 File Offset: 0x001D3444
		public bool AddMoney1(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addMoney, string strFrom, bool writeToDB = true)
		{
			int oldMoney = client.ClientData.Money1;
			if (addMoney > 0)
			{
				if (oldMoney >= 1000000000)
				{
					return this.AddUserStoreMoney(sl, tcpClientPool, pool, client, (long)addMoney, strFrom, false);
				}
				if (oldMoney + addMoney > 1000000000)
				{
					long newValue = (long)oldMoney + (long)addMoney - 1000000000L;
					addMoney = Global.GMax(0, 1000000000 - oldMoney);
					this.AddUserStoreMoney(sl, tcpClientPool, pool, client, newValue, strFrom, false);
				}
			}
			else if ((long)oldMoney + (long)addMoney < -2147483648L)
			{
				long totalMoney = client.ClientData.StoreMoney + (long)oldMoney + (long)addMoney;
				if (totalMoney < -2147483648L)
				{
					return false;
				}
				long addStoreMoney = (long)addMoney - (-2147483648L - (long)oldMoney);
				addMoney = int.MinValue - oldMoney;
				this.AddUserStoreMoney(sl, tcpClientPool, pool, client, addStoreMoney, strFrom, true);
			}
			bool result;
			if (0 == addMoney)
			{
				result = true;
			}
			else
			{
				if (writeToDB)
				{
					string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Money1 + addMoney);
					GameManager.DBCmdMgr.AddDBCmd(10004, strcmd, null, client.ServerId);
					long nowTicks = TimeUtil.NOW();
					Global.SetLastDBCmdTicks(client, 10004, nowTicks);
				}
				client.ClientData.Money1 = client.ClientData.Money1 + addMoney;
				GameManager.ClientMgr.NotifySelfMoneyChange(sl, pool, client);
				EventLogManager.AddResourceEvent(client, MoneyTypes.TongQian, (long)addMoney, (long)client.ClientData.Money1, strFrom);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑金", strFrom, "系统", client.ClientData.RoleName, "增加", addMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.Money1, client.ServerId, null);
				}
				GameManager.SystemServerEvents.AddEvent(string.Format("角色添加金钱, roleID={0}({1}), Money={2}, addMoney={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.Money1,
					addMoney
				}), EventLevels.Record);
				result = true;
			}
			return result;
		}

		// Token: 0x0600221F RID: 8735 RVA: 0x001D54E4 File Offset: 0x001D36E4
		public bool AddMoney1(GameClient client, int addMoney, string strFrom, bool writeToDB = true)
		{
			return this.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addMoney, strFrom, writeToDB);
		}

		// Token: 0x06002220 RID: 8736 RVA: 0x001D5520 File Offset: 0x001D3720
		public bool SubMoney1(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, string strFrom)
		{
			if (client.ClientData.Money1 - subMoney < 0)
			{
				subMoney = client.ClientData.Money1;
			}
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Money1 - subMoney);
			GameManager.DBCmdMgr.AddDBCmd(10004, strcmd, null, client.ServerId);
			long nowTicks = TimeUtil.NOW();
			Global.SetLastDBCmdTicks(client, 10004, nowTicks);
			client.ClientData.Money1 = client.ClientData.Money1 - subMoney;
			GameManager.ClientMgr.NotifySelfMoneyChange(sl, pool, client);
			if (0 != subMoney)
			{
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑金", strFrom, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.Money1, client.ServerId, null);
			}
			GameManager.SystemServerEvents.AddEvent(string.Format("角色扣除金钱, roleID={0}({1}), Money={2}, subMoney={3}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				client.ClientData.Money1,
				subMoney
			}), EventLevels.Record);
			EventLogManager.AddResourceEvent(client, MoneyTypes.TongQian, (long)(-(long)subMoney), (long)client.ClientData.Money1, strFrom);
			return true;
		}

		// Token: 0x06002221 RID: 8737 RVA: 0x001D56C4 File Offset: 0x001D38C4
		public void NotifySelfUserMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.UserMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 168);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002222 RID: 8738 RVA: 0x001D571E File Offset: 0x001D391E
		public void NotifySelfUserMoneyChange(GameClient client)
		{
			client.sendCmd(168, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.UserMoney), false);
		}

		// Token: 0x06002223 RID: 8739 RVA: 0x001D5758 File Offset: 0x001D3958
		public bool AddUserMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addMoney, string msg, ActivityTypes result = ActivityTypes.None, string param = "")
		{
			lock (client.ClientData.UserMoneyMutex)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					addMoney,
					(int)result,
					param
				});
				string[] dbFields = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 3)
				{
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(dbFields[1]);
				int nTotalMoney = Convert.ToInt32(dbFields[2]);
				if (nTotalMoney > 0)
				{
					Global.ProcessVipLevelUp(client);
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)addMoney, (long)client.ClientData.UserMoney, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", client.ClientData.RoleName, "增加", addMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(sl, pool, client);
			return true;
		}

		// Token: 0x06002224 RID: 8740 RVA: 0x001D591C File Offset: 0x001D3B1C
		public bool AddOfflineUserMoney(TCPClientPool tcpClientPool, TCPOutPacketPool pool, int otherRoleID, string roleName, int addMoney, string msg, int zoneid, string userid)
		{
			string strcmd = string.Format("{0}:{1}", otherRoleID, addMoney);
			string[] dbFields = Global.ExecuteDBCmd(10011, strcmd, 0);
			bool result;
			if (null == dbFields)
			{
				result = false;
			}
			else if (dbFields.Length != 3)
			{
				result = false;
			}
			else if (Convert.ToInt32(dbFields[1]) < 0)
			{
				result = false;
			}
			else
			{
				int saleZoneID = this.GetZoneIDByRoleID(otherRoleID);
				EventLogManager.AddResourceEvent(userid, saleZoneID, otherRoleID, MoneyTypes.YuanBao, (long)addMoney, -1L, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", roleName, "增加", addMoney, zoneid, userid, Convert.ToInt32(dbFields[1]), 0, null);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002225 RID: 8741 RVA: 0x001D59EC File Offset: 0x001D3BEC
		public bool SubUserMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, string msg, bool bIsAddVipExp = true, bool isAddFund = true, bool isGM = false, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (DaiBiSySType.None != SysType)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, subMoney, SysType, msg, false))
				{
					return true;
				}
			}
			lock (client.ClientData.UserMoneyMutex)
			{
				subMoney = Math.Abs(subMoney);
				if (client.ClientData.UserMoney < subMoney && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.UserMoney - (long)subMoney < -2147483648L)
				{
					return false;
				}
				int oldValue = client.ClientData.UserMoney;
				client.ClientData.UserMoney -= subMoney;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subMoney);
				string[] dbFields = null;
				try
				{
					dbFields = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, string.Format("CMD_DB_UPDATEUSERMONEY_CMD Faild", new object[0]));
					return false;
				}
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 3)
				{
					client.ClientData.UserMoney = oldValue;
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					client.ClientData.UserMoney = oldValue;
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(dbFields[1]);
				client._IconStateMgr.FlushUsedMoneyconState(client);
				client._IconStateMgr.CheckJieRiActivity(client, false);
				client._IconStateMgr.SendIconStateToClient(client);
				if (bIsAddVipExp)
				{
					Global.SaveConsumeLog(client, subMoney);
					if (isAddFund)
					{
						FundManager.FundMoneyCost(client, subMoney);
					}
					SpecialActivity act = HuodongCachingMgr.GetSpecialActivity();
					if (act != null)
					{
						act.MoneyConst(client, subMoney);
					}
					EverydayActivity everyAct = HuodongCachingMgr.GetEverydayActivity();
					if (everyAct != null)
					{
						everyAct.MoneyConst(client, subMoney);
					}
					SpecPriorityActivity specPAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPAct != null)
					{
						specPAct.MoneyConst(client, subMoney);
					}
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)(-(long)subMoney), (long)client.ClientData.UserMoney, msg);
				if (0 != subMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(sl, pool, client);
			return true;
		}

		// Token: 0x06002226 RID: 8742 RVA: 0x001D5D2C File Offset: 0x001D3F2C
		public bool SubUserMoney(GameClient client, int subMoney, string msg, bool savedb = true, bool bIsAddVipExp = true, bool isAddFund = true, bool isExpense = true, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (DaiBiSySType.None != SysType)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, subMoney, SysType, msg, false))
				{
					return true;
				}
			}
			lock (client.ClientData.UserMoneyMutex)
			{
				subMoney = Math.Abs(subMoney);
				if (client.ClientData.UserMoney < subMoney)
				{
					return false;
				}
				client.ClientData.UserMoney -= subMoney;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subMoney);
				string[] dbFields = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 3)
				{
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(dbFields[1]);
				if (isExpense)
				{
					client._IconStateMgr.FlushUsedMoneyconState(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
				if (savedb)
				{
					Global.SaveConsumeLog(client, subMoney);
					if (isAddFund)
					{
						FundManager.FundMoneyCost(client, subMoney);
					}
					SpecialActivity act = HuodongCachingMgr.GetSpecialActivity();
					if (act != null)
					{
						act.MoneyConst(client, subMoney);
					}
					EverydayActivity everyAct = HuodongCachingMgr.GetEverydayActivity();
					if (everyAct != null)
					{
						everyAct.MoneyConst(client, subMoney);
					}
					SpecPriorityActivity specPAct = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPAct != null)
					{
						specPAct.MoneyConst(client, subMoney);
					}
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)(-(long)subMoney), (long)client.ClientData.UserMoney, msg);
				if (0 != subMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(client);
			return true;
		}

		// Token: 0x06002227 RID: 8743 RVA: 0x001D5FA4 File Offset: 0x001D41A4
		public RoleBaseInfo QueryRoleBaseInfoFromDB(int roleID, int serverID = -1)
		{
			RoleBaseInfo roleBaseInfo = Global.sendToDB<RoleBaseInfo, string>(3004, roleID.ToString(), serverID);
			RoleBaseInfo result;
			if (roleBaseInfo != null && roleBaseInfo.RoleID != roleID)
			{
				result = null;
			}
			else
			{
				result = roleBaseInfo;
			}
			return result;
		}

		// Token: 0x06002228 RID: 8744 RVA: 0x001D5FE4 File Offset: 0x001D41E4
		public int QueryTotaoChongZhiMoney(GameClient client)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			int zoneID = client.ClientData.ZoneID;
			return this.QueryTotaoChongZhiMoney(userID, zoneID, client.ServerId);
		}

		// Token: 0x06002229 RID: 8745 RVA: 0x001D6024 File Offset: 0x001D4224
		public int QueryTotaoChongZhiMoney(string userID, int zoneID, int ServerId)
		{
			string strcmd = string.Format("{0}:{1}", userID, zoneID);
			string[] dbFields = Global.ExecuteDBCmd(10083, strcmd, ServerId);
			int result;
			if (null == dbFields)
			{
				result = 0;
			}
			else if (dbFields.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(dbFields[0]);
			}
			return result;
		}

		// Token: 0x0600222A RID: 8746 RVA: 0x001D607C File Offset: 0x001D427C
		public int[] QueryUserIdValue(string userID, int ServerId)
		{
			string strcmd = string.Format("{0}", userID);
			return Global.sendToDB<int[], string>(13001, strcmd, ServerId);
		}

		// Token: 0x0600222B RID: 8747 RVA: 0x001D60A8 File Offset: 0x001D42A8
		public int QueryTotaoChongZhiMoneyToday(GameClient client)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			int zoneID = client.ClientData.ZoneID;
			string strcmd = string.Format("{0}:{1}", userID, zoneID);
			string[] dbFields = Global.ExecuteDBCmd(10120, strcmd, client.ServerId);
			int result;
			if (null == dbFields)
			{
				result = 0;
			}
			else if (dbFields.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(dbFields[0]);
			}
			return result;
		}

		// Token: 0x0600222C RID: 8748 RVA: 0x001D612C File Offset: 0x001D432C
		public int QueryTotalChongZhiMoneyPeriod(GameClient client, DateTime fromDate, DateTime toDate)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, fromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), toDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] dbFields = Global.ExecuteDBCmd(13177, strcmd, client.ServerId);
			int result;
			if (null == dbFields)
			{
				result = 0;
			}
			else if (dbFields.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(dbFields[0]);
			}
			return result;
		}

		// Token: 0x0600222D RID: 8749 RVA: 0x001D61BC File Offset: 0x001D43BC
		public int QueryTotalChongZhiMoneyPeriod(int RoleID, DateTime fromDate, DateTime toDate)
		{
			string strcmd = string.Format("{0}:{1}:{2}", RoleID, fromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), toDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] dbFields = Global.ExecuteDBCmd(13177, strcmd, 0);
			int result;
			if (null == dbFields)
			{
				result = 0;
			}
			else if (dbFields.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(dbFields[0]);
			}
			return result;
		}

		// Token: 0x0600222E RID: 8750 RVA: 0x001D623C File Offset: 0x001D443C
		public int GetZoneIDByRoleID(int roleID)
		{
			string strcmd = string.Format("{0}", roleID);
			string[] dbFields = Global.ExecuteDBCmd(30010, strcmd, 0);
			int result;
			if (null == dbFields)
			{
				result = -1;
			}
			else if (dbFields.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(dbFields[1]);
			}
			return result;
		}

		// Token: 0x0600222F RID: 8751 RVA: 0x001D6294 File Offset: 0x001D4494
		public string GetUserIDByRoleID(int roleID)
		{
			string strcmd = string.Format("{0}", roleID);
			string[] dbFields = Global.ExecuteDBCmd(30010, strcmd, 0);
			string result;
			if (null == dbFields)
			{
				result = "";
			}
			else if (dbFields.Length != 2)
			{
				result = "";
			}
			else
			{
				result = dbFields[0];
			}
			return result;
		}

		// Token: 0x06002230 RID: 8752 RVA: 0x001D62EC File Offset: 0x001D44EC
		public bool AddUserMoneyOffLine(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int roleID, int addMoney, string msg, int zoneid, string userid)
		{
			string strcmd = string.Format("{0}:{1}", roleID, addMoney);
			string[] dbFields = Global.ExecuteDBCmd(10011, strcmd, 0);
			bool result;
			if (null == dbFields)
			{
				result = false;
			}
			else if (dbFields.Length != 3)
			{
				result = false;
			}
			else if (Convert.ToInt32(dbFields[1]) < 0)
			{
				result = false;
			}
			else
			{
				int saleZoneID = this.GetZoneIDByRoleID(roleID);
				string saleUserID = this.GetUserIDByRoleID(roleID);
				EventLogManager.AddResourceEvent(saleUserID, saleZoneID, roleID, MoneyTypes.YuanBao, (long)addMoney, -1L, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", string.Concat(roleID), "增加", addMoney, zoneid, userid, Convert.ToInt32(dbFields[1]), 0, null);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x06002231 RID: 8753 RVA: 0x001D63D8 File Offset: 0x001D45D8
		public void NotifySelfUserGoldChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Gold);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 397);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002232 RID: 8754 RVA: 0x001D6434 File Offset: 0x001D4634
		public bool AddUserGold(GameClient client, int addGold, string strFrom)
		{
			return this.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addGold, strFrom);
		}

		// Token: 0x06002233 RID: 8755 RVA: 0x001D6470 File Offset: 0x001D4670
		public bool AddUserGold(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addGold, string strFrom = "")
		{
			int oldGold = client.ClientData.Gold;
			lock (client.ClientData.GoldMutex)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, addGold);
				string[] dbFields = Global.ExecuteDBCmd(10095, strcmd, client.ServerId);
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					return false;
				}
				client.ClientData.Gold = Convert.ToInt32(dbFields[1]);
				if (0 != addGold)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", strFrom, client.ClientData.RoleName, "系统", "增加", addGold, client.ClientData.ZoneID, client.strUserID, client.ClientData.Gold, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserGoldChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.BindYuanBao, (long)addGold, (long)client.ClientData.Gold, strFrom);
			return true;
		}

		// Token: 0x06002234 RID: 8756 RVA: 0x001D65FC File Offset: 0x001D47FC
		public bool AddUserGoldOffLine(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int roleID, int addGold, string strFrom = "", string strUserID = "")
		{
			GameClient client = GameManager.ClientMgr.FindClient(roleID);
			bool result;
			if (null != client)
			{
				result = this.AddUserGold(sl, tcpClientPool, pool, client, addGold, strFrom);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}", roleID, addGold);
				string[] dbFields = Global.ExecuteDBCmd(10095, strcmd, 0);
				if (null == dbFields)
				{
					result = false;
				}
				else if (dbFields.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(dbFields[1]) < 0)
				{
					result = false;
				}
				else
				{
					if (0 != addGold)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", strFrom, string.Concat(roleID), "系统", "增加", addGold, 0, strUserID, Convert.ToInt32(dbFields[1]), 0, null);
					}
					EventLogManager.AddResourceEvent(this.GetUserIDByRoleID(roleID), this.GetZoneIDByRoleID(roleID), roleID, MoneyTypes.BindYuanBao, (long)addGold, -1L, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002235 RID: 8757 RVA: 0x001D670C File Offset: 0x001D490C
		public bool SubUserGold(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subGold, string msg = "无", bool isGM = false)
		{
			int oldGold = client.ClientData.Gold;
			lock (client.ClientData.GoldMutex)
			{
				if (client.ClientData.Gold < subGold && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.Gold - (long)subGold < -2147483648L)
				{
					return false;
				}
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subGold);
				string[] dbFields = Global.ExecuteDBCmd(10095, strcmd, client.ServerId);
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					return false;
				}
				client.ClientData.Gold = Convert.ToInt32(dbFields[1]);
				EventLogManager.AddResourceEvent(client, MoneyTypes.BindYuanBao, (long)(-(long)subGold), (long)client.ClientData.Gold, msg);
				if (0 != subGold)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", msg, client.ClientData.RoleName, "系统", "减少", subGold, client.ClientData.ZoneID, client.strUserID, client.ClientData.Gold, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserGoldChange(sl, pool, client);
			return true;
		}

		// Token: 0x06002236 RID: 8758 RVA: 0x001D68EC File Offset: 0x001D4AEC
		public bool SubUserGold(GameClient client, int subGold, string msg = "无")
		{
			return this.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subGold, msg, false);
		}

		// Token: 0x06002237 RID: 8759 RVA: 0x001D6928 File Offset: 0x001D4B28
		public void NotifySelfUserYinLiangChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.YinLiang);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 169);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002238 RID: 8760 RVA: 0x001D6984 File Offset: 0x001D4B84
		public bool AddUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addYinLiang, string strFrom, bool isGM = false)
		{
			int oldYinLiang = client.ClientData.YinLiang;
			lock (client.ClientData.YinLiangMutex)
			{
				if (addYinLiang > 0)
				{
					if (oldYinLiang >= 1000000000)
					{
						return this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, (long)addYinLiang, strFrom, false);
					}
					if (oldYinLiang + addYinLiang > 1000000000)
					{
						long newValue = (long)oldYinLiang + (long)addYinLiang - 1000000000L;
						addYinLiang = Global.GMax(0, 1000000000 - oldYinLiang);
						this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, newValue, strFrom, false);
					}
				}
				else if ((long)oldYinLiang + (long)addYinLiang < -2147483648L)
				{
					long totalMoney = client.ClientData.StoreYinLiang + (long)oldYinLiang + (long)addYinLiang;
					if (totalMoney < -2147483648L)
					{
						return false;
					}
					long addStoreMoney = (long)addYinLiang - (-2147483648L - (long)oldYinLiang);
					addYinLiang = int.MinValue - oldYinLiang;
					this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, addStoreMoney, strFrom, isGM);
				}
				if (0 == addYinLiang)
				{
					return true;
				}
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, addYinLiang);
				string[] dbFields = Global.ExecuteDBCmd(10012, strcmd, client.ServerId);
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					return false;
				}
				client.ClientData.YinLiang = Convert.ToInt32(dbFields[1]);
				if (0 != addYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, "系统", client.ClientData.RoleName, "增加", addYinLiang, client.ClientData.ZoneID, client.strUserID, client.ClientData.YinLiang, client.ServerId, null);
				}
			}
			if (addYinLiang > 0)
			{
				ChengJiuManager.OnTongQianIncrease(client);
			}
			GameManager.ClientMgr.NotifySelfUserYinLiangChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.YinLiang, (long)addYinLiang, (long)client.ClientData.YinLiang, strFrom);
			return true;
		}

		// Token: 0x06002239 RID: 8761 RVA: 0x001D6C38 File Offset: 0x001D4E38
		public bool AddUserYinLiang(GameClient client, int addYinLiang, string strFrom)
		{
			return this.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addYinLiang, strFrom, false);
		}

		// Token: 0x0600223A RID: 8762 RVA: 0x001D6C74 File Offset: 0x001D4E74
		public bool AddOfflineUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, string userID, int roleID, string roleName, int addYinLiang, string strFrom, int zoneid)
		{
			string strcmd = string.Format("{0}:{1}", roleID, addYinLiang);
			string[] dbFields = Global.ExecuteDBCmd(10012, strcmd, 0);
			bool result;
			if (null == dbFields)
			{
				result = false;
			}
			else if (dbFields.Length != 2)
			{
				result = false;
			}
			else if (Convert.ToInt32(dbFields[1]) < 0)
			{
				result = false;
			}
			else
			{
				if (0 != addYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, "系统", string.Concat(roleID), "增加", addYinLiang, zoneid, userID, Convert.ToInt32(dbFields[1]), 0, null);
				}
				int saleZoneID = this.GetZoneIDByRoleID(roleID);
				EventLogManager.AddResourceEvent(userID, saleZoneID, roleID, MoneyTypes.YinLiang, (long)addYinLiang, -1L, strFrom);
				result = true;
			}
			return result;
		}

		// Token: 0x0600223B RID: 8763 RVA: 0x001D6D50 File Offset: 0x001D4F50
		public bool SubUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subYinLiang, string strFrom, bool isGM = false)
		{
			int oldYinLiang = client.ClientData.YinLiang;
			lock (client.ClientData.YinLiangMutex)
			{
				if (client.ClientData.YinLiang < subYinLiang && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.YinLiang - (long)subYinLiang < -2147483648L)
				{
					return false;
				}
				int oldValue = client.ClientData.YinLiang;
				client.ClientData.YinLiang -= subYinLiang;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subYinLiang);
				string[] dbFields = null;
				try
				{
					dbFields = Global.ExecuteDBCmd(10012, strcmd, client.ServerId);
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, string.Format("CMD_DB_UPDATEUSERYINLIANG_CMD Faild", new object[0]));
					return false;
				}
				if (null == dbFields)
				{
					return false;
				}
				if (dbFields.Length != 2)
				{
					client.ClientData.YinLiang = oldValue;
					return false;
				}
				if (Convert.ToInt32(dbFields[1]) < 0)
				{
					client.ClientData.YinLiang = oldValue;
					return false;
				}
				client.ClientData.YinLiang = Convert.ToInt32(dbFields[1]);
				if (0 != subYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, client.ClientData.RoleName, "系统", "减少", subYinLiang, client.ClientData.ZoneID, client.strUserID, client.ClientData.YinLiang, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserYinLiangChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.YinLiang, (long)(-(long)subYinLiang), (long)client.ClientData.YinLiang, strFrom);
			return true;
		}

		// Token: 0x0600223C RID: 8764 RVA: 0x001D6FB0 File Offset: 0x001D51B0
		public bool MoveGoodsDataToOtherRole(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GoodsData gd, GameClient fromClient, GameClient toClient, bool bAddToTarget = true)
		{
			string[] dbFields = null;
			string strcmd;
			if (!RebornEquip.IsRebornType(gd.GoodsID))
			{
				strcmd = string.Format("{0}:{1}:{2}", toClient.ClientData.RoleID, fromClient.ClientData.RoleID, gd.Id);
			}
			else
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					toClient.ClientData.RoleID,
					fromClient.ClientData.RoleID,
					gd.Id,
					15000
				});
			}
			TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10013, strcmd, out dbFields, 0);
			bool result;
			if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (dbFields.Length < 4 || Convert.ToInt32(dbFields[3]) < 0)
			{
				result = false;
			}
			else
			{
				Global.AddRoleGoodsEvent(fromClient, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, -gd.GCount, "物品转给别人", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
				EventLogManager.AddGoodsEvent(fromClient, OpTypes.AddOrSub, OpTags.None, gd.GoodsID, (long)gd.Id, -gd.GCount, 0, "物品转给别人");
				GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "物品转给别人(在线)", fromClient.ClientData.RoleName, toClient.ClientData.RoleName, "移动", -gd.GCount, fromClient.ClientData.ZoneID, toClient.strUserID, -1, 0, gd);
				if (bAddToTarget)
				{
					if (!RebornEquip.IsRebornType(gd.GoodsID))
					{
						string[] dbFields2 = null;
						gd.BagIndex = Global.GetIdleSlotOfBagGoods(toClient);
						strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							toClient.ClientData.RoleID,
							gd.Id,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							gd.BagIndex,
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
						Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields2, 0);
						Global.AddGoodsData(toClient, gd);
						ZuoQiManager.ProcessZuoQiTuJian(toClient, gd.GoodsID);
					}
					else
					{
						string[] dbFields2 = null;
						gd.BagIndex = RebornEquip.GetIdleSlotOfRebornGoods(toClient);
						strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							toClient.ClientData.RoleID,
							gd.Id,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							gd.BagIndex,
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
						Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields2, 0);
						RebornEquip.AddGoodsData(toClient, gd);
					}
					Global.AddRoleGoodsEvent(toClient, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, gd.GCount, "得到他人物品", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
					EventLogManager.AddGoodsEvent(toClient, OpTypes.AddOrSub, OpTags.None, gd.GoodsID, (long)gd.Id, gd.GCount, gd.GCount, "得到他人物品");
					GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "得到他人物品(在线)", fromClient.ClientData.RoleName, toClient.ClientData.RoleName, "移动", gd.GCount, toClient.ClientData.ZoneID, toClient.strUserID, -1, 0, gd);
					ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, -1, -1, gd.GoodsID, TaskTypes.BuySomething, null, 0, -1L, null);
					GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 1, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600223D RID: 8765 RVA: 0x001D7648 File Offset: 0x001D5848
		public bool MoveGoodsDataToOfflineRole(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GoodsData gd, string fromUserID, int fromRoleID, string fromRoleName, int fromRoleLevel, string toUserID, int toRoleID, string toRoleName, int toRoleLevel, bool bAddToTarget, int zoneid)
		{
			string[] dbFields = null;
			string strcmd;
			if (!RebornEquip.IsRebornType(gd.GoodsID))
			{
				strcmd = string.Format("{0}:{1}:{2}", toRoleID, fromRoleID, gd.Id);
			}
			else
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					toRoleID,
					fromRoleID,
					gd.Id,
					15000
				});
			}
			TCPProcessCmdResults dbRequestResult = Global.RequestToDBServer(tcpClientPool, pool, 10013, strcmd, out dbFields, 0);
			bool result;
			if (dbRequestResult == TCPProcessCmdResults.RESULT_FAILED)
			{
				LogManager.WriteLog(LogTypes.SQL, string.Format("向DB请求转移物品时失败{0}->{1}", fromRoleName, toRoleName), null, true);
				result = false;
			}
			else if (dbFields.Length < 4 || Convert.ToInt32(dbFields[3]) < 0)
			{
				LogManager.WriteLog(LogTypes.SQL, string.Format("向DB请求转移物品时失败{0}->{1},错误码{2}", fromRoleName, toRoleName, dbFields[3]), null, true);
				result = false;
			}
			else
			{
				Global.AddRoleGoodsEvent(fromUserID, fromRoleID, fromRoleName, fromRoleLevel, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, -gd.GCount, "物品转给别人", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
				GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "物品转给别人(离线)", fromRoleName, toRoleName, "移动", -gd.GCount, zoneid, fromUserID, -1, 0, gd);
				if (bAddToTarget)
				{
					Global.AddRoleGoodsEvent(toUserID, toRoleID, toRoleName, toRoleLevel, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, gd.GCount, "得到他人物品", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
					GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "得到他人物品(离线)", fromRoleName, toRoleName, "移动", gd.GCount, zoneid, toUserID, -1, 0, gd);
					GameClient toClient = GameManager.ClientMgr.FindClient(toRoleID);
					if (null != toClient)
					{
						if (!RebornEquip.IsRebornType(gd.GoodsID))
						{
							string[] dbFields2 = null;
							gd.BagIndex = Global.GetIdleSlotOfBagGoods(toClient);
							strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								toRoleID,
								gd.Id,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								gd.BagIndex,
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
							Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields2, 0);
							Global.AddGoodsData(toClient, gd);
							ZuoQiManager.ProcessZuoQiTuJian(toClient, gd.GoodsID);
						}
						else
						{
							string[] dbFields2 = null;
							gd.BagIndex = RebornEquip.GetIdleSlotOfRebornGoods(toClient);
							strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								toClient.ClientData.RoleID,
								gd.Id,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								gd.BagIndex,
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
							Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out dbFields2, 0);
							RebornEquip.AddGoodsData(toClient, gd);
						}
						ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, -1, -1, gd.GoodsID, TaskTypes.BuySomething, null, 0, -1L, null);
						GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 1, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600223E RID: 8766 RVA: 0x001D7C68 File Offset: 0x001D5E68
		public void NotifyGoodsStallCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int stallType)
		{
			string strcmd = string.Format("{0}:{1}:{2}", status, client.ClientData.RoleID, stallType);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 173);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600223F RID: 8767 RVA: 0x001D7CC4 File Offset: 0x001D5EC4
		public void NotifyGoodsStallData(SocketListener sl, TCPOutPacketPool pool, GameClient client, StallData sd)
		{
			byte[] bytesData = null;
			lock (sd)
			{
				bytesData = DataHelper.ObjectToBytes<StallData>(sd);
			}
			TCPOutPacket tcpOutPacket = pool.Pop();
			tcpOutPacket.PacketCmdID = 174;
			tcpOutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002240 RID: 8768 RVA: 0x001D7D48 File Offset: 0x001D5F48
		public void NotifySpriteStartStall(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (null != client.ClientData.StallDataItem)
			{
				List<object> objsList = Global.GetAll9Clients(client);
				if (null != objsList)
				{
					string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StallDataItem.StallName);
					this.SendToClients(sl, pool, null, objsList, strcmd, 175);
				}
			}
		}

		// Token: 0x06002241 RID: 8769 RVA: 0x001D7DC0 File Offset: 0x001D5FC0
		public void NotifySpriteMarketBuy(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int result, int buyType, int goodsDbID, int goodsID, int nID = 226)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				result,
				buyType,
				client.ClientData.RoleID,
				(otherClient != null) ? otherClient.ClientData.RoleID : -1,
				(otherClient != null) ? Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName) : "",
				goodsDbID,
				goodsID
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002242 RID: 8770 RVA: 0x001D7E78 File Offset: 0x001D6078
		public void NotifySpriteMarketBuy2(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int result, int buyType, int goodsDbID, int goodsID, int otherRoleZoneID, string otherRoleName, int nID = 226)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				result,
				buyType,
				client.ClientData.RoleID,
				otherRoleID,
				Global.FormatRoleName3(otherRoleID, otherRoleName),
				goodsDbID,
				goodsID
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002243 RID: 8771 RVA: 0x001D7F08 File Offset: 0x001D6108
		public void NotifySpriteMarketName(SocketListener sl, TCPOutPacketPool pool, GameClient client, string marketName, int offlineMarket)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, marketName, offlineMarket);
				this.SendToClients(sl, pool, null, objsList, strcmd, 591);
			}
		}

		// Token: 0x06002244 RID: 8772 RVA: 0x001D7F60 File Offset: 0x001D6160
		public void RemoveCoolDown(SocketListener sl, TCPOutPacketPool pool, GameClient client, int type, int code)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, type, code);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 165);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002245 RID: 8773 RVA: 0x001D7FB8 File Offset: 0x001D61B8
		public void NotifyUpdateInterPowerCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int hintUser = 1)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.InterPower, hintUser);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 192);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002246 RID: 8774 RVA: 0x001D801C File Offset: 0x001D621C
		public bool AddInterPower(GameClient client, int addInterPower, bool enableFilter = false, bool writeToDB = true)
		{
			bool result;
			if (client.ClientData.InterPower >= 30000)
			{
				result = false;
			}
			else
			{
				if (enableFilter)
				{
					addInterPower = Global.FilterValue(client, addInterPower);
				}
				if (addInterPower <= 0)
				{
					result = false;
				}
				else
				{
					int oldInterPower = client.ClientData.InterPower;
					client.ClientData.InterPower = client.ClientData.InterPower + addInterPower;
					client.ClientData.InterPower = Global.GMin(client.ClientData.InterPower, 30000);
					if (client.ClientData.InterPower > oldInterPower)
					{
						if (writeToDB)
						{
							GameManager.DBCmdMgr.AddDBCmd(10003, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.InterPower), null, client.ServerId);
							long nowTicks = TimeUtil.NOW();
							Global.SetLastDBCmdTicks(client, 10003, nowTicks);
						}
						this.NotifyUpdateInterPowerCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1);
						GameManager.ClientMgr.UpdateRoleDailyData_LingLi(client, client.ClientData.InterPower - oldInterPower);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002247 RID: 8775 RVA: 0x001D8164 File Offset: 0x001D6364
		public bool SubInterPower(GameClient client, int subInterPower)
		{
			if (subInterPower > 0)
			{
				client.ClientData.InterPower = Global.GMax(client.ClientData.InterPower - subInterPower, 0);
				client.ClientData.InterPower = Global.GMin(client.ClientData.InterPower, 30000);
				GameManager.DBCmdMgr.AddDBCmd(10003, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.InterPower), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10003, nowTicks);
				this.NotifyUpdateInterPowerCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1);
				DBRoleBufferManager.ProcessLingLiVReserve(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
			return true;
		}

		// Token: 0x06002248 RID: 8776 RVA: 0x001D8258 File Offset: 0x001D6458
		private void UpdateRoleOnlineTimes(GameClient client, long addTicks)
		{
			this.UpdateRoleOnlineTimesForKorea(client, addTicks);
			if (!client.ClientData.FirstPlayStart)
			{
				if (client.ClientData.ForceShenFenZheng)
				{
					client.ClientData.ForceShenFenZheng = false;
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(69, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 17);
				}
				else
				{
					int oldTotalOnlineHours = client.ClientData.TotalOnlineSecs / 3600;
					client.ClientData.TotalOnlineSecs += Math.Max(0, (int)(addTicks / 1000L));
					if (client.ClientData.BagNum < Global.MaxBagGridNum)
					{
						client.ClientData.OpenGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "OpenGridTick", client.ClientData.OpenGridTime, false);
					}
					if (client.ClientData.RebornBagNum < Global.MaxBagGridNum)
					{
						client.ClientData.OpenRebornBagTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "10248", client.ClientData.OpenRebornBagTime, false);
					}
					if (client.ClientData.MyPortableBagData.ExtGridNum < Global.ExtraBagGridPriceStartPos)
					{
						client.ClientData.OpenPortableGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "OpenPortableGridTick", client.ClientData.OpenPortableGridTime, false);
					}
					if (client.ClientData.RebornGirdData.ExtGridNum < Global.ExtraBagGridPriceStartPos)
					{
						client.ClientData.OpenRebornGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "10247", client.ClientData.OpenRebornGridTime, false);
					}
					GlobalEventSource.getInstance().fireEvent(new PlayerOnlineEventObject(client));
					int newTotalOnlineHours = client.ClientData.TotalOnlineSecs / 3600;
					if (oldTotalOnlineHours != newTotalOnlineHours)
					{
						HuodongCachingMgr.ProcessKaiFuGiftAward(client);
					}
					int oldAntiAddictionHours = client.ClientData.AntiAddictionSecs / 3600;
					client.ClientData.AntiAddictionSecs += Math.Max(0, (int)(addTicks / 1000L));
					int newAntiAddictionHours = client.ClientData.AntiAddictionSecs / 3600;
					int monthID = TimeUtil.NowDateTime().Month;
					if (client.ClientData.MyHuodongData.CurMID == monthID.ToString())
					{
						client.ClientData.MyHuodongData.CurMTime += Math.Max(0, (int)(addTicks / 1000L));
					}
					else
					{
						client.ClientData.MyHuodongData.OnlineGiftState = 0;
						client.ClientData.MyHuodongData.CurMID = monthID.ToString();
						client.ClientData.MyHuodongData.LastMTime = client.ClientData.MyHuodongData.CurMTime;
						client.ClientData.MyHuodongData.CurMTime = 0;
						client.ClientData.MyHuodongData.CurMTime += Math.Max(0, (int)(addTicks / 1000L));
					}
					DailyActiveManager.ProcessOnlineForDailyActive(client);
					client._IconStateMgr.CheckJingJiChangJiangLi(client);
					client._IconStateMgr.CheckFuMeiRiZaiXian(client);
					client._IconStateMgr.SendIconStateToClient(client);
					if (!("1" != GameManager.GameConfigMgr.GetGameConfigItemStr("anti-addiction", "1")))
					{
						if (!this.UpdateRoleOnlineTimesForTengXun(client))
						{
							int isAdult = GameManager.OnlineUserSession.FindUserAdult(client.ClientSocket);
							if (isAdult <= 0)
							{
								if (oldAntiAddictionHours < 1 && newAntiAddictionHours >= 1)
								{
									BulletinMsgData bulletinMsgData = new BulletinMsgData
									{
										MsgID = "one-hour-hint-addiction",
										PlayMinutes = -1,
										ToPlayNum = -1,
										BulletinText = GLang.GetLang(70, new object[0]),
										BulletinTicks = TimeUtil.NOW(),
										playingNum = 0
									};
									this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
								}
								if (oldAntiAddictionHours < 2 && newAntiAddictionHours >= 2)
								{
									BulletinMsgData bulletinMsgData = new BulletinMsgData
									{
										MsgID = "two-hour-hint-addiction",
										PlayMinutes = -1,
										ToPlayNum = -1,
										BulletinText = GLang.GetLang(71, new object[0]),
										BulletinTicks = TimeUtil.NOW(),
										playingNum = 0
									};
									this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
								}
								int antiAddictionType = Global.GetAntiAddictionTimeType(client);
								if (antiAddictionType != client.ClientData.AntiAddictionTimeType)
								{
									client.ClientData.AntiAddictionTimeType = antiAddictionType;
									if (1 == client.ClientData.AntiAddictionTimeType)
									{
										BulletinMsgData bulletinMsgData;
										if ("0" == GameManager.GameConfigMgr.GetGameConfigItemStr("force-add-shenfenzheng", "1"))
										{
											bulletinMsgData = new BulletinMsgData
											{
												MsgID = "anti-addiction",
												PlayMinutes = -1,
												ToPlayNum = -1,
												BulletinText = GLang.GetLang(72, new object[0]),
												BulletinTicks = TimeUtil.NOW(),
												playingNum = 0
											};
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(73, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 14);
										}
										else
										{
											bulletinMsgData = new BulletinMsgData
											{
												MsgID = "anti-addiction",
												PlayMinutes = -1,
												ToPlayNum = -1,
												BulletinText = GLang.GetLang(74, new object[0]),
												BulletinTicks = TimeUtil.NOW(),
												playingNum = 0
											};
											client.ClientData.ForceShenFenZheng = true;
										}
										this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
									}
									else if (2 == client.ClientData.AntiAddictionTimeType)
									{
										BulletinMsgData bulletinMsgData = new BulletinMsgData
										{
											MsgID = "anti-addiction",
											PlayMinutes = -1,
											ToPlayNum = -1,
											BulletinText = GLang.GetLang(75, new object[0]),
											BulletinTicks = TimeUtil.NOW(),
											playingNum = 0
										};
										this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(76, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 14);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002249 RID: 8777 RVA: 0x001D89D0 File Offset: 0x001D6BD0
		private void UpdateRoleOnlineTimesForKorea(GameClient client, long addTicks)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				if (!("korea" != GameManager.GameConfigMgr.GetGameConfigItemStr("country", "")))
				{
					int oldThisTimeAntiAddictionHours = client.ClientData.ThisTimeOnlineSecs / 3600;
					client.ClientData.ThisTimeOnlineSecs += Math.Max(0, (int)(addTicks / 1000L));
					int newThisTimeAntiAddictionHours = client.ClientData.ThisTimeOnlineSecs / 3600;
					if (oldThisTimeAntiAddictionHours != newThisTimeAntiAddictionHours)
					{
						BulletinMsgData bulletinMsgData = new BulletinMsgData
						{
							MsgID = "this-time-every-one-hour-hint-addiction",
							PlayMinutes = -1,
							ToPlayNum = -1,
							BulletinText = string.Format(GLang.GetLang(77, new object[0]), newThisTimeAntiAddictionHours),
							BulletinTicks = TimeUtil.NOW(),
							playingNum = 0
						};
						this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
					}
				}
			}
		}

		// Token: 0x0600224A RID: 8778 RVA: 0x001D8AE8 File Offset: 0x001D6CE8
		private bool UpdateRoleOnlineTimesForTengXun(GameClient client)
		{
			bool result;
			if ("TengXun" != GameManager.GameConfigMgr.GetGameConfigItemStr("pingtainame", ""))
			{
				result = false;
			}
			else if (client.ClientData.TengXunFCMRate >= 1.0)
			{
				result = true;
			}
			else
			{
				int antiAddictionType = Global.GetAntiAddictionTimeType_TengXun(client);
				if (antiAddictionType == client.ClientData.AntiAddictionTimeType)
				{
					result = true;
				}
				else
				{
					client.ClientData.AntiAddictionTimeType = antiAddictionType;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600224B RID: 8779 RVA: 0x001D8B74 File Offset: 0x001D6D74
		public void NotifySelfDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
			{
				client.ClientData.RoleID,
				decoID,
				decoType,
				toBody,
				toX,
				toY,
				shakeMap,
				toX1,
				toY1,
				moveTicks,
				alphaTicks
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 229);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600224C RID: 8780 RVA: 0x001D8C34 File Offset: 0x001D6E34
		public void NotifyOthersMyDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
				{
					client.ClientData.RoleID,
					decoID,
					decoType,
					toBody,
					toX,
					toY,
					shakeMap,
					toX1,
					toY1,
					moveTicks,
					alphaTicks
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 229);
			}
		}

		// Token: 0x0600224D RID: 8781 RVA: 0x001D8D08 File Offset: 0x001D6F08
		public void NotifyOthersMyDeco(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, toX, toY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
				{
					-1,
					decoID,
					decoType,
					toBody,
					toX,
					toY,
					shakeMap,
					toX1,
					toY1,
					moveTicks,
					alphaTicks
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 229);
			}
		}

		// Token: 0x0600224E RID: 8782 RVA: 0x001D8DF4 File Offset: 0x001D6FF4
		public void NotifyBufferData(GameClient client, BufferData bufferData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BufferData>(bufferData, Global._TCPManager.TcpOutPacketPool, 230);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600224F RID: 8783 RVA: 0x001D8E38 File Offset: 0x001D7038
		public bool VertifyBuffFashion(int bufferID)
		{
			return bufferID == 39 || bufferID == 111 || bufferID == 10013 || bufferID == 103 || bufferID == 10020 || bufferID == 10022 || bufferID == 10023 || bufferID == 10012 || bufferID == 10011 || bufferID == 10010 || bufferID == 10009 || bufferID == 10008 || bufferID == 10007 || bufferID == 10001 || bufferID == 10002 || bufferID == 10003 || bufferID == 10004 || bufferID == 9000 || bufferID == 9001 || bufferID == 9002 || bufferID == 9003 || bufferID == 9004 || bufferID == 9005 || bufferID == 9006 || bufferID == 9007 || bufferID == 9008 || bufferID == 9009 || bufferID == 9010 || bufferID == 9011 || bufferID == 9012 || bufferID == 9051 || bufferID == 9052;
		}

		// Token: 0x06002250 RID: 8784 RVA: 0x001D8F84 File Offset: 0x001D7184
		public void NotifyOtherBufferData(IObject self, BufferData bufferData)
		{
			OtherBufferData otherBufferData = new OtherBufferData
			{
				BufferID = bufferData.BufferID,
				BufferVal = bufferData.BufferVal,
				BufferType = bufferData.BufferType,
				BufferSecs = bufferData.BufferSecs,
				StartTime = bufferData.StartTime
			};
			ObjectTypes objectType = self.ObjectType;
			switch (objectType)
			{
			case ObjectTypes.OT_CLIENT:
				otherBufferData.RoleID = (self as GameClient).ClientData.RoleID;
				break;
			case ObjectTypes.OT_MONSTER:
				otherBufferData.RoleID = (self as Monster).RoleID;
				break;
			default:
				switch (objectType)
				{
				case ObjectTypes.OT_NPC:
					otherBufferData.RoleID = (self as NPC).NpcID;
					goto IL_D0;
				case ObjectTypes.OT_FAKEROLE:
					otherBufferData.RoleID = (self as FakeRoleItem).FakeRoleID;
					goto IL_D0;
				}
				return;
			}
			IL_D0:
			byte[] bytes = DataHelper.ObjectToBytes<OtherBufferData>(otherBufferData);
			List<object> objsList = Global.GetAll9Clients(self);
			if (null == objsList)
			{
				objsList = new List<object>();
			}
			if (objsList.IndexOf(self) < 0)
			{
				objsList.Add(self);
			}
			foreach (object obj in objsList)
			{
				GameClient c = obj as GameClient;
				if (c != null && c.CodeRevision >= 2)
				{
					this.SendToClient(c, bytes, 676);
				}
			}
		}

		// Token: 0x06002251 RID: 8785 RVA: 0x001D9118 File Offset: 0x001D7318
		public void NotifySelfExperience(SocketListener sl, TCPOutPacketPool pool, GameClient client, long newExperience)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.Experience,
				client.ClientData.Level,
				newExperience,
				client.ClientData.ChangeLifeCount
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 141);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002252 RID: 8786 RVA: 0x001D91B0 File Offset: 0x001D73B0
		public void ProcessRoleExperience(GameClient client, long experience, bool enableFilter = true, bool writeToDB = true, bool checkDead = false, string strFrom = "none")
		{
			if (client.ClientData.HideGM <= 0)
			{
				if (!checkDead || client.ClientData.CurrentLifeV > 0)
				{
					if (experience > 0L)
					{
						if (enableFilter)
						{
							experience = Global.FilterValue(client, experience);
						}
						if (experience > 0L)
						{
							long oldExp = client.ClientData.Experience;
							int oldUnionLevel = Global.GetUnionLevel2(client);
							EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.Awards, MoneyTypes.Exp, experience, -1L, strFrom);
							int oldLevel = client.ClientData.Level;
							Global.EarnExperience(client, experience);
							if (writeToDB || oldLevel != client.ClientData.Level)
							{
								int nOccupation = Global.CalcOriginalOccupationID(client.ClientData.Occupation);
								ChangeLifeAddPointInfo tmpChangeAddPointInfo = null;
								if (!Data.ChangeLifeAddPointInfoList.TryGetValue(client.ClientData.ChangeLifeCount, out tmpChangeAddPointInfo) || tmpChangeAddPointInfo == null)
								{
									return;
								}
								lock (client.ClientData.PropPointMutex)
								{
									int nOldPoint = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
									int nAddLev = client.ClientData.Level - oldLevel;
									int nNewPoint = nAddLev * tmpChangeAddPointInfo.AddPoint + nOldPoint;
									client.ClientData.TotalPropPoint = nNewPoint;
									Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", nNewPoint, true);
								}
								GameManager.DBCmdMgr.AddDBCmd(10002, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Level, client.ClientData.Experience), null, client.ServerId);
								long nowTicks = TimeUtil.NOW();
								Global.SetLastDBCmdTicks(client, 10002, nowTicks);
							}
							if (oldLevel != client.ClientData.Level)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, true, 7);
								GameManager.ClientMgr.NotifyTeamUpLevel(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, false);
								if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									Global.AutoLearnSkills(client);
								}
								if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									ChengJiuManager.OnRoleLevelUp(client);
								}
								HuodongCachingMgr.ProcessKaiFuGiftAward(client);
								HuodongCachingMgr.ProcessUpLevelAward4_60Level_100Level(client, oldLevel, client.ClientData.Level);
								WorldLevelManager.getInstance().UpddateWorldLevelBuff(client);
								GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RoleLevelUp));
								SpreadManager.getInstance().SpreadIsLevel(client);
								SingletonTemplate<TradeBlackManager>.Instance().UpdateObjectExtData(client);
								EventLogManager.AddRoleUpgradeEvent(client, experience, oldExp, oldUnionLevel, strFrom);
								if (client._IconStateMgr.CheckReborn(client))
								{
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							GameManager.ClientMgr.UpdateRoleDailyData_Exp(client, experience);
							GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, experience);
						}
					}
				}
			}
		}

		// Token: 0x06002253 RID: 8787 RVA: 0x001D9550 File Offset: 0x001D7750
		public void AddOnlieRoleExperience(GameClient client, int addPercent)
		{
			long needExperience = 0L;
			if (client.ClientData.Level < Data.LevelUpExperienceList.Length - 1)
			{
				needExperience = Data.LevelUpExperienceList[client.ClientData.Level + 1];
			}
			if (needExperience > 0L)
			{
				int addExperience = (int)((double)needExperience * ((double)addPercent / 100.0));
				this.ProcessRoleExperience(client, (long)addExperience, false, false, false, "none");
			}
		}

		// Token: 0x06002254 RID: 8788 RVA: 0x001D95C4 File Offset: 0x001D77C4
		public void AddAllOnlieRoleExperience(int addPercent)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep <= 0)
				{
					this.AddOnlieRoleExperience(client, addPercent);
				}
			}
		}

		// Token: 0x06002255 RID: 8789 RVA: 0x001D9610 File Offset: 0x001D7810
		public long GetCurRoleLvUpNeedExp(GameClient client)
		{
			long result;
			if (client == null)
			{
				result = 0L;
			}
			else if (client.ClientData.Level >= Data.LevelUpExperienceList.Length - 1)
			{
				result = 0L;
			}
			else
			{
				long lNeedExp = Data.LevelUpExperienceList[client.ClientData.Level];
				if (client.ClientData.ChangeLifeCount > 0)
				{
					ChangeLifeDataInfo infoTmp = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, 0);
					if (infoTmp != null && infoTmp.ExpProportion > 0L)
					{
						lNeedExp *= infoTmp.ExpProportion;
					}
				}
				result = lNeedExp;
			}
			return result;
		}

		// Token: 0x06002256 RID: 8790 RVA: 0x001D96AC File Offset: 0x001D78AC
		public int AutoCompletionTaskByTaskID(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, TCPRandKey tcpRandKey, GameClient client, int nDestTaskID)
		{
			int result2;
			if (null == client)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("client不存在，服务器无法完成某任务与之前所有任务", new object[0]), null, true);
				result2 = -1;
			}
			else
			{
				try
				{
					int roleID = client.ClientData.RoleID;
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, SystemXmlItem> kv in GameManager.SystemTasksMgr.SystemXmlItemDict)
					{
						SystemXmlItem systemTask = kv.Value;
						int nTaskID = kv.Key;
						if (nTaskID > nDestTaskID)
						{
							break;
						}
						if (nTaskID > 0)
						{
							Global.AddOldTask(client, nTaskID);
							Global.AddRoleTaskEvent(client, nTaskID);
							ChengJiuManager.ProcessCompleteMainTaskForChengJiu(client, nTaskID);
							Global.UpdateTaskZhangJieProp(client, nTaskID, false);
							list.Add(nTaskID);
						}
					}
					list.Sort();
					list.Insert(0, roleID);
					byte[] bytesCmd = DataHelper.ObjectToBytes<List<int>>(list);
					TCPOutPacket tcpOutPacket = null;
					TCPProcessCmdResults result = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, tcpRandKey, pool, 10180, bytesCmd, bytesCmd.Length, out tcpOutPacket, client.ServerId);
					if (result != TCPProcessCmdResults.RESULT_DATA || null == tcpOutPacket)
					{
						return -1;
					}
					string strData = new UTF8Encoding().GetString(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
					Global.PushBackTcpOutPacket(tcpOutPacket);
					string[] fields = strData.Split(new char[]
					{
						':'
					});
					if (fields.Length != 1)
					{
						return -1;
					}
					return int.Parse(fields[0]);
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result2 = -1;
			}
			return result2;
		}

		// Token: 0x06002257 RID: 8791 RVA: 0x001D98B4 File Offset: 0x001D7AB4
		public void SearchRolesByStr(GameClient client, string roleName, int startIndex)
		{
			int index = startIndex;
			int addCount = 0;
			List<SearchRoleData> roleDataList = new List<SearchRoleData>();
			GameClient otherClient;
			while ((otherClient = this.GetNextClient(ref index, false)) != null)
			{
				if (-1 != otherClient.ClientData.RoleName.IndexOf(roleName))
				{
					roleDataList.Add(new SearchRoleData
					{
						RoleID = otherClient.ClientData.RoleID,
						RoleName = Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName),
						RoleSex = otherClient.ClientData.RoleSex,
						Level = otherClient.ClientData.Level,
						Occupation = otherClient.ClientData.Occupation,
						MapCode = otherClient.ClientData.MapCode,
						PosX = otherClient.ClientData.PosX,
						PosY = otherClient.ClientData.PosY,
						ChangeLifeLev = otherClient.ClientData.ChangeLifeCount
					});
					addCount++;
					if (addCount >= 10)
					{
						break;
					}
				}
			}
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(roleDataList, Global._TCPManager.TcpOutPacketPool, 232);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002258 RID: 8792 RVA: 0x001D9A14 File Offset: 0x001D7C14
		public void ListMapRoles(GameClient client, int startIndex)
		{
			ListRolesData listRolesData = new ListRolesData
			{
				StartIndex = startIndex,
				TotalRolesCount = 0,
				PageRolesCount = 10,
				SearchRoleDataList = new List<SearchRoleData>()
			};
			List<SearchRoleData> roleDataList = listRolesData.SearchRoleDataList;
			List<object> objsList = this.GetMapClients(client.ClientData.MapCode);
			objsList = Global.FilterHideObjsList(objsList);
			if (objsList == null || objsList.Count <= 0)
			{
				this.SendListRolesDataResult(client, listRolesData);
			}
			else
			{
				List<GameClient> clients = new List<GameClient>();
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if ((objsList[i] as GameClient).ClientData.TeamID <= 0)
						{
							clients.Add(objsList[i] as GameClient);
						}
					}
				}
				listRolesData.TotalRolesCount = clients.Count;
				if (listRolesData.TotalRolesCount <= 0)
				{
					this.SendListRolesDataResult(client, listRolesData);
				}
				else
				{
					if (startIndex >= clients.Count)
					{
						startIndex = 0;
					}
					int addCount = 0;
					for (int i = 0; i < clients.Count; i++)
					{
						if (i >= startIndex)
						{
							GameClient otherClient = clients[i];
							roleDataList.Add(new SearchRoleData
							{
								RoleID = otherClient.ClientData.RoleID,
								RoleName = Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName),
								RoleSex = otherClient.ClientData.RoleSex,
								Level = otherClient.ClientData.Level,
								Occupation = otherClient.ClientData.Occupation,
								MapCode = otherClient.ClientData.MapCode,
								PosX = otherClient.ClientData.PosX,
								PosY = otherClient.ClientData.PosY,
								CombatForce = otherClient.ClientData.CombatForce,
								ChangeLifeLev = otherClient.ClientData.ChangeLifeCount
							});
							addCount++;
							if (addCount >= 10)
							{
								break;
							}
						}
					}
					this.SendListRolesDataResult(client, listRolesData);
				}
			}
		}

		// Token: 0x06002259 RID: 8793 RVA: 0x001D9C88 File Offset: 0x001D7E88
		private void SendListRolesDataResult(GameClient client, ListRolesData listRolesData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ListRolesData>(listRolesData, Global._TCPManager.TcpOutPacketPool, 233);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600225A RID: 8794 RVA: 0x001D9CCC File Offset: 0x001D7ECC
		public void ListAllTeams(GameClient client, int startIndex)
		{
			SearchTeamData searchTeamData = new SearchTeamData
			{
				StartIndex = startIndex,
				TotalTeamsCount = 0,
				PageTeamsCount = 10,
				TeamDataList = null
			};
			searchTeamData.TotalTeamsCount = GameManager.TeamMgr.GetTotalDataCount();
			if (searchTeamData.TotalTeamsCount <= 0)
			{
				this.SendListTeamsDataResult(client, searchTeamData);
			}
			else
			{
				if (startIndex >= searchTeamData.TotalTeamsCount)
				{
					startIndex = 0;
				}
				searchTeamData.TeamDataList = GameManager.TeamMgr.GetTeamDataList(startIndex, searchTeamData.PageTeamsCount);
				this.SendListTeamsDataResult(client, searchTeamData);
			}
		}

		// Token: 0x0600225B RID: 8795 RVA: 0x001D9D5C File Offset: 0x001D7F5C
		private void SendListTeamsDataResult(GameClient client, SearchTeamData searchTeamData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SearchTeamData>(searchTeamData, Global._TCPManager.TcpOutPacketPool, 234);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600225C RID: 8796 RVA: 0x001D9DA0 File Offset: 0x001D7FA0
		public void NotifyDailyTaskData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<DailyTaskData>>(client.ClientData.MyDailyTaskDataList, Global._TCPManager.TcpOutPacketPool, 236);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600225D RID: 8797 RVA: 0x001D9DEC File Offset: 0x001D7FEC
		public void NotifyFuBenData(GameClient client, FuBenData fuBenData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenData>(fuBenData, Global._TCPManager.TcpOutPacketPool, 252);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600225E RID: 8798 RVA: 0x001D9E30 File Offset: 0x001D8030
		public void NotifyFuBenBeginInfo(GameClient client)
		{
			if (client.ClientData.IsFlashPlayer == 1 && client.ClientData.MapCode == 6090)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					client.ClientData.RoleID,
					-1,
					TimeUtil.NOW(),
					0,
					1,
					0,
					1,
					1,
					1
				});
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 260);
				if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
				{
					return;
				}
			}
			int fuBenSeqID = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
			if (fuBenSeqID > 0)
			{
				int copyMapID = client.ClientData.CopyMapID;
				if (copyMapID > 0)
				{
					int fuBenID = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (fuBenID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(copyMapID);
							if (null != copyMap)
							{
								if (Global.IsStoryCopyMapScene(client.ClientData.MapCode))
								{
									SystemXmlItem systemFuBenItem = null;
									if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemFuBenItem) && systemFuBenItem != null)
									{
										int nBossID = systemFuBenItem.GetIntValue("BossID", -1);
										int nNum = GameManager.MonsterZoneMgr.GetMapMonsterNum(client.ClientData.MapCode, nBossID);
										if (nNum == 0)
										{
											Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 1);
										}
										else
										{
											Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 2);
										}
									}
								}
								long startTicks = fuBenInfoItem.StartTicks;
								long endTicks = fuBenInfoItem.EndTicks;
								int killedNormalNum = copyMap.KilledNormalNum;
								int totalNormalNum = copyMap.TotalNormalNum;
								int killedBossNum = copyMap.KilledBossNum;
								int totalBossNum = copyMap.TotalBossNum;
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
								{
									client.ClientData.RoleID,
									fuBenID,
									startTicks,
									endTicks,
									killedNormalNum,
									totalNormalNum,
									killedBossNum,
									totalBossNum
								});
								TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 260);
								if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600225F RID: 8799 RVA: 0x001DA154 File Offset: 0x001D8354
		public void NotifyAllFuBenBeginInfo(CopyMap copyMap, int roleId, bool allKilled)
		{
			int fuBenSeqID = copyMap.FuBenSeqID;
			if (fuBenSeqID > 0)
			{
				int copyMapID = copyMap.CopyMapID;
				if (copyMapID > 0)
				{
					int fuBenID = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
					if (fuBenID > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							long startTicks = fuBenInfoItem.StartTicks;
							long endTicks = fuBenInfoItem.EndTicks;
							int killedNormalNum = copyMap.KilledNormalNum;
							int totalNormalNum = copyMap.TotalNormalNum;
							if (allKilled)
							{
								killedNormalNum = totalNormalNum;
							}
							int killedBossNum = copyMap.KilledBossNum;
							int totalBossNum = copyMap.TotalBossNum;
							if (allKilled)
							{
								killedBossNum = totalBossNum;
							}
							string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
							{
								roleId,
								fuBenID,
								startTicks,
								endTicks,
								killedNormalNum,
								totalNormalNum,
								killedBossNum,
								totalBossNum
							});
							List<object> objsList = this.GetMapClients(copyMap.MapCode);
							if (null != objsList)
							{
								objsList = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, objsList, false);
								this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 260);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002260 RID: 8800 RVA: 0x001DA2DC File Offset: 0x001D84DC
		public void NotifyAllMapFuBenBeginInfo(CopyMap copyMap, int roleId, bool allKilled)
		{
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
			if (null != fuBenInfoItem)
			{
				long startTicks = fuBenInfoItem.StartTicks;
				long endTicks = fuBenInfoItem.EndTicks;
				int killedNormalNum = copyMap.KilledNormalNum;
				int totalNormalNum = copyMap.TotalNormalNum;
				if (allKilled)
				{
					killedNormalNum = totalNormalNum;
				}
				int killedBossNum = copyMap.KilledBossNum;
				int totalBossNum = copyMap.TotalBossNum;
				if (allKilled)
				{
					killedBossNum = totalBossNum;
				}
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					roleId,
					copyMap.FubenMapID,
					startTicks,
					endTicks,
					killedNormalNum,
					totalNormalNum,
					killedBossNum,
					totalBossNum
				});
				List<object> objsList = new List<object>();
				List<int> mapCodeList = FuBenManager.FindMapCodeListByFuBenID(copyMap.FubenMapID);
				if (null != mapCodeList)
				{
					foreach (int mapcode in mapCodeList)
					{
						int copyMapID = GameManager.CopyMapMgr.FindCopyID(copyMap.FuBenSeqID, mapcode);
						if (copyMapID >= 0)
						{
							CopyMap child_map = GameManager.CopyMapMgr.FindCopyMap(copyMapID);
							if (null != child_map)
							{
								objsList.AddRange(child_map.GetClientsList());
							}
						}
					}
				}
				if (0 != objsList.Count)
				{
					this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 260);
				}
			}
		}

		// Token: 0x06002261 RID: 8801 RVA: 0x001DA4B4 File Offset: 0x001D86B4
		public void NotifyAllFuBenTongGuanJiangLi(CopyMap copyMap, byte[] bytesData)
		{
			int copyMapID = copyMap.CopyMapID;
			if (copyMapID > 0)
			{
				int fuBenID = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
				if (fuBenID > 0)
				{
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
					if (null != fuBenInfoItem)
					{
						List<object> objsList = this.GetMapClients(copyMap.MapCode);
						if (null != objsList)
						{
							objsList = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, objsList, false);
							this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, bytesData, 521);
						}
					}
				}
			}
		}

		// Token: 0x06002262 RID: 8802 RVA: 0x001DA564 File Offset: 0x001D8764
		public void NotifyAllFuBenMonstersNum(CopyMap copyMap, bool allKilled)
		{
			if (!GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID))
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				if (fuBenSeqID > 0)
				{
					int copyMapID = copyMap.CopyMapID;
					if (copyMapID > 0)
					{
						int killedNormalNum = copyMap.KilledNormalNum;
						int totalNormalNum = copyMap.TotalNormalNum;
						if (allKilled)
						{
							killedNormalNum = totalNormalNum;
						}
						int killedBossNum = copyMap.KilledBossNum;
						int totalBossNum = copyMap.TotalBossNum;
						if (allKilled)
						{
							killedBossNum = totalBossNum;
						}
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							copyMap.GetGameClientCount(),
							killedNormalNum,
							totalNormalNum,
							killedBossNum,
							totalBossNum
						});
						List<object> objsList = this.GetMapClients(copyMap.MapCode);
						if (null != objsList)
						{
							objsList = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, objsList, false);
							this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 261);
						}
					}
				}
			}
		}

		// Token: 0x06002263 RID: 8803 RVA: 0x001DA6A4 File Offset: 0x001D88A4
		public void NotifyDailyJingMaiData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DailyJingMaiData>(client.ClientData.MyDailyJingMaiData, Global._TCPManager.TcpOutPacketPool, 237);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002264 RID: 8804 RVA: 0x001DA6F0 File Offset: 0x001D88F0
		public void NotifyOtherJingMaiExp(GameClient client)
		{
			int canGetExpNum = Global.GetLeftAddJingMaiExpNum(client);
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TotalJingMaiExp, canGetExpNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 256);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002265 RID: 8805 RVA: 0x001DA76C File Offset: 0x001D896C
		public void NotifySelfAddSkill(SocketListener sl, TCPOutPacketPool pool, GameClient client, int skillDbID, int skillID, int skillLevel)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				skillDbID,
				skillID,
				skillLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 217);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002266 RID: 8806 RVA: 0x001DA7E0 File Offset: 0x001D89E0
		public void AddNumSkill(GameClient client, SkillData skillData, int addNum, bool writeToDB = true)
		{
			if (addNum != 0)
			{
				int oldUsedNum;
				if (skillData.DbID < 0)
				{
					oldUsedNum = client.ClientData.DefaultSkillUseNum;
				}
				else
				{
					oldUsedNum = skillData.UsedNum;
				}
				SystemXmlItem skillXml = MagicsCacheManager.GetMagicCacheItem(client.ClientData.Occupation, skillData.SkillID, skillData.SkillLevel);
				if (addNum > 0)
				{
					if (skillXml == null)
					{
						return;
					}
					if (skillXml.GetIntValue("ShuLianDu", -1) <= oldUsedNum)
					{
						if (skillData.DbID < 0)
						{
							client.ClientData.DefaultSkillUseNum = skillXml.GetIntValue("ShuLianDu", -1);
						}
						else
						{
							skillData.UsedNum = skillXml.GetIntValue("ShuLianDu", -1);
						}
						return;
					}
				}
				int nUseNum;
				if (skillData.DbID < 0)
				{
					client.ClientData.DefaultSkillUseNum += addNum;
					if (client.ClientData.DefaultSkillUseNum < 0)
					{
						client.ClientData.DefaultSkillUseNum = 0;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillUseNum", client.ClientData.DefaultSkillUseNum, false);
					nUseNum = client.ClientData.DefaultSkillUseNum;
				}
				else
				{
					skillData.UsedNum += addNum;
					if (skillData.UsedNum < 0)
					{
						skillData.UsedNum = 0;
					}
					this.UpdateSkillInfo(client, skillData, writeToDB);
					nUseNum = skillData.UsedNum;
				}
				if (skillXml != null && nUseNum >= skillXml.GetIntValue("ShuLianDu", -1))
				{
					GameManager.ClientMgr.NotifySkillUsedNumFull(client, skillData);
				}
			}
		}

		// Token: 0x06002267 RID: 8807 RVA: 0x001DA990 File Offset: 0x001D8B90
		public void UpdateSkillInfo(GameClient client, SkillData skillData, bool writeToDB = true)
		{
			if (writeToDB)
			{
				Global.SetLastDBSkillCmdTicks(client, skillData.SkillID, 0L);
				GameManager.DBCmdMgr.AddDBCmd(10037, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					skillData.DbID,
					skillData.SkillLevel,
					skillData.UsedNum
				}), null, client.ServerId);
			}
			else
			{
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBSkillCmdTicks(client, skillData.SkillID, nowTicks);
			}
		}

		// Token: 0x06002268 RID: 8808 RVA: 0x001DAA38 File Offset: 0x001D8C38
		public void NotifySkillUsedNumFull(GameClient client, SkillData skillData)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				skillData.DbID,
				skillData.SkillID,
				skillData.UsedNum,
				skillData.SkillLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 258);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002269 RID: 8809 RVA: 0x001DAAD7 File Offset: 0x001D8CD7
		public void NotifySkillCDTime(GameClient client, int skillid, long cdtime, bool waitEnterScene = false)
		{
			client.sendCmd(691, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, skillid, cdtime), waitEnterScene);
		}

		// Token: 0x0600226A RID: 8810 RVA: 0x001DAB10 File Offset: 0x001D8D10
		public void NotifyPortableBagData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PortableBagData>(client.ClientData.MyPortableBagData, Global._TCPManager.TcpOutPacketPool, 241);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600226B RID: 8811 RVA: 0x001DAB5C File Offset: 0x001D8D5C
		public void NotifyHuodongData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HuodongData>(client.ClientData.MyHuodongData, Global._TCPManager.TcpOutPacketPool, 245);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600226C RID: 8812 RVA: 0x001DABA7 File Offset: 0x001D8DA7
		public void NotifyGetCombatGiftData(GameClient client)
		{
		}

		// Token: 0x0600226D RID: 8813 RVA: 0x001DABAC File Offset: 0x001D8DAC
		public void NotifyGetLevelUpGiftData(GameClient client, int newLevel)
		{
			GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format("{0}:{1}", client.ClientData.RoleID, newLevel), 445);
		}

		// Token: 0x0600226E RID: 8814 RVA: 0x001DAC00 File Offset: 0x001D8E00
		public void NotifyAllChangeHuoDongID(int bigAwardID, int songLiID)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, bigAwardID, songLiID);
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 251);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x0600226F RID: 8815 RVA: 0x001DACA4 File Offset: 0x001D8EA4
		public void NotifyTeamMemberFuBenEnterMsg(GameClient client, int leaderRoleID, int fuBenID, int fuBenSeqID)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				leaderRoleID,
				fuBenID,
				fuBenSeqID
			});
			client.ClientData.NotifyFuBenID = fuBenID;
			client.ClientData.NotifyFuBenSeqID = fuBenSeqID;
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 254);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x001DAD40 File Offset: 0x001D8F40
		public void NotifyTeamFuBenEnterMsg(List<int> roleIDsList, int minLevel, int maxLevel, int leaderMapCode, int leaderRoleID, int fuBenID, int fuBenSeqID, int enterNumber, int maxFinishNum, bool igoreNumLimit = false)
		{
			if (roleIDsList != null && roleIDsList.Count > 0)
			{
				for (int i = 0; i < roleIDsList.Count; i++)
				{
					GameClient otherClient = this.FindClient(roleIDsList[i]);
					if (null != otherClient)
					{
						if (otherClient.ClientData.MapCode == leaderMapCode)
						{
							int unionLevel = Global.GetUnionLevel(otherClient.ClientData.ChangeLifeCount, otherClient.ClientData.Level, false);
							if (unionLevel >= minLevel && unionLevel <= maxLevel)
							{
								if (!igoreNumLimit)
								{
									FuBenData fuBenData = Global.GetFuBenData(otherClient, fuBenID);
									int nFinishNum;
									int haveEnterNum = Global.GetFuBenEnterNum(fuBenData, out nFinishNum);
									if ((enterNumber >= 0 && haveEnterNum >= enterNumber) || (maxFinishNum >= 0 && nFinishNum >= maxFinishNum))
									{
										goto IL_E7;
									}
								}
								this.NotifyTeamMemberFuBenEnterMsg(otherClient, leaderRoleID, fuBenID, fuBenSeqID);
							}
						}
					}
					IL_E7:;
				}
			}
		}

		// Token: 0x06002271 RID: 8817 RVA: 0x001DAE4C File Offset: 0x001D904C
		public void InitLianZhanBuff(GameClient client)
		{
			BufferData bufferData = Global.GetBufferDataByID(client, 122);
			if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
			{
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 122))
				{
					LianZhanConfig config = Global.GetLianZhanBufferVal((int)bufferData.BufferVal);
					if (config != null)
					{
						client.ClientData.LianZhanExpRate = config.RebornExp;
					}
				}
			}
		}

		// Token: 0x06002272 RID: 8818 RVA: 0x001DAEC0 File Offset: 0x001D90C0
		public double GetLianZhanExpRate(GameClient client)
		{
			double addPercent = 0.0;
			BufferData bufferData = Global.GetBufferDataByID(client, 122);
			if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
			{
				int lianZhanBufferVal = (int)bufferData.BufferVal;
				LianZhanConfig config = Global.GetLianZhanBufferVal(lianZhanBufferVal);
				if (null != config)
				{
					addPercent = config.RebornExp;
				}
			}
			return addPercent;
		}

		// Token: 0x06002273 RID: 8819 RVA: 0x001DAF24 File Offset: 0x001D9124
		public void ChangeRoleLianZhan(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int addNum = 1)
		{
			if (null != client)
			{
				if (Data.IsLianZhanMap(client.ClientData.MapCode))
				{
					int oldLianZhanNum = client.ClientData.TempLianZhan;
					if (Global.CanContinueLianZhan(client) || addNum > 1)
					{
						client.ClientData.TempLianZhan = client.ClientData.TempLianZhan + addNum;
						if (client.ClientData.TempLianZhan % Data.MinLianZhanNum == 0)
						{
							LianZhanConfig config = Global.GetLianZhanBufferVal(client.ClientData.TempLianZhan);
							if (null != config)
							{
								int oldLianZhanBufferVal = 0;
								BufferData bufferData = Global.GetBufferDataByID(client, 122);
								if (bufferData != null && !Global.IsBufferDataOver(bufferData, 0L))
								{
									oldLianZhanBufferVal = (int)bufferData.BufferVal;
								}
								if (config.Num > oldLianZhanBufferVal)
								{
									Global.UpdateBufferData(client, BufferItemTypes.LianZhanBuff, new double[]
									{
										(double)config.Time,
										(double)((long)config.Num + ((long)config.GoodsID << 32))
									}, 0, true);
									Global.AddLianZhanEvent(client, client.ClientData.TempLianZhan);
								}
								else if (config != null && config.Num == oldLianZhanBufferVal && TimeUtil.NOW() + (long)(config.Time * 1000) > bufferData.StartTime + (long)(bufferData.BufferSecs * 1000))
								{
									Global.UpdateBufferData(client, BufferItemTypes.LianZhanBuff, new double[]
									{
										(double)config.Time,
										(double)((long)config.Num + ((long)config.GoodsID << 32))
									}, 0, true);
									Global.AddLianZhanEvent(client, client.ClientData.TempLianZhan);
								}
							}
						}
					}
					else
					{
						client.ClientData.TempLianZhan = addNum;
					}
					client.ClientData.StartLianZhanTicks = TimeUtil.NOW();
					client.ClientData.WaitingLianZhanMS = (long)(Data.LianZhanContinueTime(client.ClientData.TempLianZhan) * 1000);
					RebornManager.getInstance().ProcessLianZhan(client);
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TempLianZhan, client.ClientData.StartLianZhanTicks + client.ClientData.WaitingLianZhanMS);
					client.sendCmd(266, strcmd, false);
				}
			}
		}

		// Token: 0x06002274 RID: 8820 RVA: 0x001DB1AC File Offset: 0x001D93AC
		public void UpdateRoleDailyData_Exp(GameClient client, long newExperience)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.ExpDayID)
			{
				client.ClientData.MyRoleDailyData.TodayExp += (int)newExperience;
			}
			else
			{
				client.ClientData.MyRoleDailyData.ExpDayID = dayID;
				client.ClientData.MyRoleDailyData.TodayExp = (int)newExperience;
			}
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x001DB24C File Offset: 0x001D944C
		public void UpdateRoleDailyData_LingLi(GameClient client, int newLingLi)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.LingLiDayID)
			{
				client.ClientData.MyRoleDailyData.TodayLingLi += newLingLi;
			}
			else
			{
				client.ClientData.MyRoleDailyData.LingLiDayID = dayID;
				client.ClientData.MyRoleDailyData.TodayLingLi = newLingLi;
			}
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x001DB2EC File Offset: 0x001D94EC
		public void UpdateRoleDailyData_KillBoss(GameClient client, int newKillBoss)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.KillBossDayID)
			{
				client.ClientData.MyRoleDailyData.TodayKillBoss += newKillBoss;
			}
			else
			{
				client.ClientData.MyRoleDailyData.KillBossDayID = dayID;
				client.ClientData.MyRoleDailyData.TodayKillBoss = newKillBoss;
			}
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x001DB38C File Offset: 0x001D958C
		public void UpdateRoleDailyData_FuBenNum(GameClient client, int newFuBenNum, int nLev, bool bActiveChenJiu = true)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.FuBenDayID)
			{
				client.ClientData.MyRoleDailyData.TodayFuBenNum += newFuBenNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.FuBenDayID = dayID;
				client.ClientData.MyRoleDailyData.TodayFuBenNum = newFuBenNum;
			}
			DailyActiveManager.ProcessCompleteCopyMapForDailyActive(client, nLev, 1);
			if (bActiveChenJiu)
			{
				ChengJiuManager.ProcessCompleteCopyMapForChengJiu(client, nLev, 1);
			}
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x001DB448 File Offset: 0x001D9648
		public long GetRoleDailyData_RebornExp(GameClient client, MoneyTypes types)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.RebornExpDayID)
			{
				if (types == MoneyTypes.RebornExpMonster)
				{
					return (long)client.ClientData.MyRoleDailyData.RebornExpMonster;
				}
				if (types == MoneyTypes.RebornExpSale)
				{
					return (long)client.ClientData.MyRoleDailyData.RebornExpSale;
				}
			}
			else
			{
				client.ClientData.MyRoleDailyData.RebornExpMonster = 0;
				client.ClientData.MyRoleDailyData.RebornExpSale = 0;
				client.ClientData.MyRoleDailyData.RebornExpDayID = dayID;
			}
			return 0L;
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x001DB52C File Offset: 0x001D972C
		public void UpdateRoleDailyData_RebornExp(GameClient client, MoneyTypes types, long newExperience)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.RebornExpDayID)
			{
				if (types == MoneyTypes.RebornExpMonster)
				{
					client.ClientData.MyRoleDailyData.RebornExpMonster += (int)newExperience;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					client.ClientData.MyRoleDailyData.RebornExpSale += (int)newExperience;
				}
			}
			else
			{
				client.ClientData.MyRoleDailyData.RebornExpMonster = 0;
				client.ClientData.MyRoleDailyData.RebornExpSale = 0;
				client.ClientData.MyRoleDailyData.RebornExpDayID = dayID;
				if (types == MoneyTypes.RebornExpMonster)
				{
					client.ClientData.MyRoleDailyData.RebornExpMonster = (int)newExperience;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					client.ClientData.MyRoleDailyData.RebornExpSale = (int)newExperience;
				}
			}
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x001DB65C File Offset: 0x001D985C
		public void UpdateRoleDailyData_WuXingNum(GameClient client, int newWuXingNum)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.WuXingDayID)
			{
				client.ClientData.MyRoleDailyData.WuXingNum += newWuXingNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.WuXingDayID = dayID;
				client.ClientData.MyRoleDailyData.WuXingNum = newWuXingNum;
			}
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x001DB6FC File Offset: 0x001D98FC
		public void UpdateRoleDailyData_SweepNum(GameClient client, int newWuXingNum)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID == client.ClientData.MyRoleDailyData.WuXingDayID)
			{
				client.ClientData.MyRoleDailyData.WuXingNum += newWuXingNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.WuXingDayID = dayID;
				client.ClientData.MyRoleDailyData.WuXingNum = newWuXingNum;
			}
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x001DB79C File Offset: 0x001D999C
		public void NotifyRoleDailyData(GameClient client)
		{
			RoleDailyData roleDailyData = client.ClientData.MyRoleDailyData;
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDailyData>(roleDailyData, Global._TCPManager.TcpOutPacketPool, 267);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x001DB7EC File Offset: 0x001D99EC
		public void UpdateKillBoss(GameClient client, int killBossNum, Monster monster, bool writeToDB = false)
		{
			if (401 == monster.MonsterType)
			{
				int[] ids = GameManager.systemParamsList.GetParamValueIntArrayByName("NotTuMo", ',');
				if (ids != null && ids.Length > 0)
				{
					for (int i = 0; i < ids.Length; i++)
					{
						if (monster.MonsterInfo.ExtensionID == ids[i])
						{
							return;
						}
					}
				}
				client.ClientData.KillBoss += killBossNum;
				this.UpdateRoleDailyData_KillBoss(client, killBossNum);
				if (writeToDB)
				{
					GameManager.DBCmdMgr.AddDBCmd(10055, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.KillBoss), null, client.ServerId);
					long nowTicks = TimeUtil.NOW();
					Global.SetLastDBCmdTicks(client, 10055, nowTicks);
				}
			}
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x001DB8E8 File Offset: 0x001D9AE8
		public void UpdateBattleNum(GameClient client, int addNum, bool writeToDB = false)
		{
			client.ClientData.BattleNum += addNum;
			if (writeToDB)
			{
				GameManager.DBCmdMgr.AddDBCmd(10064, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BattleNum), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10064, nowTicks);
			}
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x001DB968 File Offset: 0x001D9B68
		public void ChangeRoleHeroIndex(SocketListener sl, TCPOutPacketPool pool, GameClient client, int heroIndex, bool force = false)
		{
			if (!force)
			{
				if (heroIndex <= 0)
				{
					return;
				}
				int oldHeroIndex = client.ClientData.HeroIndex;
				if (heroIndex <= oldHeroIndex)
				{
					Global.BroadcastHeroMapOk(client, heroIndex, false);
					return;
				}
			}
			client.ClientData.HeroIndex = Math.Min(13, heroIndex);
			Global.BroadcastHeroMapOk(client, heroIndex, true);
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.HeroIndex);
				this.SendToClients(sl, pool, null, objsList, strcmd, 290);
			}
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x001DBA20 File Offset: 0x001D9C20
		public void NotifyBossInfoDictData(GameClient client)
		{
			Dictionary<int, BossData> dict = MonsterBossManager.GetBossDictData();
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BossData>>(dict, Global._TCPManager.TcpOutPacketPool, 268);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x001DBA68 File Offset: 0x001D9C68
		public void NotifyYaBiaoData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<YaBiaoData>(client.ClientData.MyYaBiaoData, Global._TCPManager.TcpOutPacketPool, 270);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x001DBAB4 File Offset: 0x001D9CB4
		public void NotifyOtherBiaoCheLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int biaoCheID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", biaoCheID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strcmd, 279);
			}
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x001DBAFC File Offset: 0x001D9CFC
		public void NotifyMySelfNewBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem biaoCheItem)
		{
			BiaoCheData biaoCheData = Global.BiaoCheItem2BiaoCheData(biaoCheItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BiaoCheData>(biaoCheData, pool, 276);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x001DBB34 File Offset: 0x001D9D34
		public void NotifyMySelfDelBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, int biaoCheID)
		{
			string strcmd = string.Format("{0}", biaoCheID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 277);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x001DBB78 File Offset: 0x001D9D78
		public void NotifyAllPopupWinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (client == null || gc != client)
				{
					if (!gc.ClientSocket.IsKuaFuLogin)
					{
						this.NotifyPopupWinMsg(sl, pool, gc, strcmd);
					}
				}
			}
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x001DBBDC File Offset: 0x001D9DDC
		public void NotifyPopupWinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 284);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002287 RID: 8839 RVA: 0x001DBC0C File Offset: 0x001D9E0C
		private void ChangeDayLoginNum(GameClient client)
		{
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (dayID < client.ClientData.LoginDayID && Math.Abs(dayID - client.ClientData.LoginDayID) < 2)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else if (dayID != client.ClientData.LoginDayID)
			{
				client.ClientData.LoginDayID = dayID;
				HuodongCachingMgr.OnJieriRoleLogin(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID), false);
				ChengJiuManager.OnRoleLogin(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID));
				HuodongCachingMgr.ProcessDayOnlineSecs(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID));
				HuodongCachingMgr.ResetRegressActiveOpen();
				UserRegressActiveManager.getInstance().RoleOnlineHandler(client);
				bool notifyHuodDongData = Global.UpdateWeekLoginNum(client);
				notifyHuodDongData |= Global.UpdateLimitTimeLoginNum(client);
				if (notifyHuodDongData)
				{
					GameManager.ClientMgr.NotifyHuodongData(client);
				}
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				Global.GiveGuMuTimeLimitAward(client);
				Global.InitRoleDailyTaskData(client, true);
				CompManager.getInstance().HandleCompTaskSomething(client, false);
				CaiJiLogic.InitRoleDailyCaiJiData(client, false, true);
				HuanYingSiYuanManager.getInstance().InitRoleDailyHYSYData(client);
				Global.ProcessUpdateFuBenData(client);
				CGetOldResourceManager.InitRoleOldResourceInfo(client, true);
				Global.UpdateHeFuLoginFlag(client);
				Global.UpdateHeFuTotalLoginFlag(client);
				if (client._IconStateMgr.CheckHeFuActivity(client) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckPaiHangState(client) || client._IconStateMgr.CheckJieRiPCKingEveryDay(client) || client._IconStateMgr.CheckSpecPriorityActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				YueKaManager.UpdateNewDay(client);
				UserReturnManager.getInstance().initUserReturnData(client);
				OlympicsManager.getInstance().CheckOlympicsOpenState(TimeUtil.NOW(), true);
				OlympicsManager.getInstance().CheckTip(client);
				FundManager.initFundData(client);
				MarriageOtherLogic.getInstance().ChangeDayUpdate(client, true);
				MarryPartyLogic.getInstance().MarryPartyJoinListClear(client, true);
				Global.UpdateRoleLoginRecord(client);
				JieriGiveActivity giveAct = HuodongCachingMgr.GetJieriGiveActivity();
				if (giveAct != null)
				{
					giveAct.UpdateNewDay(client);
				}
				JieriRecvActivity recvAct = HuodongCachingMgr.GetJieriRecvActivity();
				if (recvAct != null)
				{
					recvAct.UpdateNewDay(client);
				}
				client.sendCmd(833, string.Format("{0}", TimeUtil.NOW() * 10000L), false);
				SingletonTemplate<SevenDayActivityMgr>.Instance().OnNewDay(client);
				SingletonTemplate<ZhengBaManager>.Instance().OnNewDay(client);
				SpecPlatFuLiManager.getInstance().OnNewDay(client);
				RebornManager.getInstance().OnLogin(client, false);
				GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, -Global.GetRoleParamsInt32FromDB(client, "10255"), "跨天重生槽免费次数重置", true, true, false);
			}
		}

		// Token: 0x06002288 RID: 8840 RVA: 0x001DBF18 File Offset: 0x001DA118
		public void ChangeAllThingAddPropIndexs(GameClient client)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.AllQualityIndex,
				client.ClientData.AllForgeLevelIndex,
				client.ClientData.AllJewelLevelIndex,
				client.ClientData.AllZhuoYueNum
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 292);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002289 RID: 8841 RVA: 0x001DBFCC File Offset: 0x001DA1CC
		public void NotifyAllChangeHalfYinLiangPeriod(int halfYinLiangPeriod)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, halfYinLiangPeriod);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 293);
				if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x0600228A RID: 8842 RVA: 0x001DC050 File Offset: 0x001DA250
		public void ChangeBangHuiName(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.Faction,
					client.ClientData.BHName,
					client.ClientData.BHZhiWu
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 296);
			}
		}

		// Token: 0x0600228B RID: 8843 RVA: 0x001DC0E0 File Offset: 0x001DA2E0
		public void ChangeBangHuiZhiWu(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, client.ClientData.BHZhiWu);
				if (client.ClientData.BHZhiWu > 0)
				{
					string sBusiness = "";
					if (client.ClientData.BHZhiWu == 1)
					{
						sBusiness = GLang.GetLang(78, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 2)
					{
						sBusiness = GLang.GetLang(79, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 3)
					{
						sBusiness = GLang.GetLang(80, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 4)
					{
						sBusiness = GLang.GetLang(81, new object[0]);
					}
					Global.BroadcastBangHuiMsg(client.ClientData.RoleID, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(82, new object[0]), new object[]
					{
						client.ClientData.RoleName,
						sBusiness
					}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				}
				this.SendToClients(sl, pool, null, objsList, strcmd, 329);
			}
		}

		// Token: 0x0600228C RID: 8844 RVA: 0x001DC254 File Offset: 0x001DA454
		public void NotifyOnlineBangHuiMgrRoleApplyMsg(int roleID, string roleName, int bhid, string bhName, string roleList)
		{
			if (!string.IsNullOrEmpty(roleList))
			{
				string[] fields = roleList.Split(new char[]
				{
					','
				});
				if (fields != null && fields.Length > 0)
				{
					GameClient clientApply = GameManager.ClientMgr.FindClient(roleID);
					if (clientApply != null)
					{
						for (int i = 0; i < fields.Length; i++)
						{
							int bhMgrRoleID = Global.SafeConvertToInt32(fields[i]);
							if (bhMgrRoleID > 0)
							{
								GameClient client = GameManager.ClientMgr.FindClient(bhMgrRoleID);
								if (null != client)
								{
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
									{
										roleID,
										roleName,
										clientApply.ClientData.Occupation,
										clientApply.ClientData.Level,
										clientApply.ClientData.ChangeLifeCount,
										bhid,
										bhName
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 301);
									if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
									{
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600228D RID: 8845 RVA: 0x001DC3CC File Offset: 0x001DA5CC
		public void NotifyInviteToBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int inviteRoleID, string inviteRoleName, int bhid, string bhName, int nChangelifeLev)
		{
			if (Global.GetUnionLevel(otherClient, false) >= Global.JoinBangHuiNeedLevel)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					inviteRoleID,
					inviteRoleName,
					bhid,
					bhName,
					nChangelifeLev
				});
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 307);
				if (!sl.SendData(otherClient.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x0600228E RID: 8846 RVA: 0x001DC450 File Offset: 0x001DA650
		public void NotifyJoinBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int bhid, string bhName)
		{
			if (otherClient.ClientData.Faction <= 0)
			{
				otherClient.ClientData.Faction = bhid;
				otherClient.ClientData.BHName = bhName;
				otherClient.ClientData.BHZhiWu = 0;
				GameManager.ClientMgr.ChangeBangHuiName(sl, pool, otherClient);
				GlobalEventSource4Scene.getInstance().fireEvent(new PostBangHuiChangeEventObject(otherClient, bhid), 10000);
				Global.SaveRoleParamsInt32ValueToDB(otherClient, "EnterBangHuiUnixSecs", DataHelper.UnixSecondsNow(), true);
				Global.SaveRoleParamsDateTimeToDB(otherClient, "10182", TimeUtil.NowDateTime(), true);
				int junQiLevel = JunQiManager.GetJunQiLevelByBHID(otherClient.ClientData.Faction);
				Global.UpdateBufferData(otherClient, BufferItemTypes.JunQi, new double[]
				{
					(double)junQiLevel - 1.0
				}, 1, true);
				Global.BroadcastBangHuiMsg(otherClient.ClientData.RoleID, bhid, StringUtil.substitute(GLang.GetLang(83, new object[0]), new object[]
				{
					otherClient.ClientData.RoleName,
					otherClient.ClientData.BHName
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				ChengJiuManager.OnFirstInFaction(otherClient);
				UnionPalaceManager.initSetUnionPalaceProps(otherClient, true);
				Global.UpdateChengHaoBuff(otherClient);
				otherClient._IconStateMgr.CheckGuildIcon(otherClient, false);
			}
		}

		// Token: 0x0600228F RID: 8847 RVA: 0x001DC590 File Offset: 0x001DA790
		public void NotifyLeaveBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int bhid, string bhName, int leaveType)
		{
			if (otherClient.ClientData.Faction > 0)
			{
				MoYuLongXue.OnClientLeaveBangHui(bhid, otherClient.ClientData.RoleID);
				Global.BroadcastBangHuiMsg(otherClient.ClientData.RoleID, bhid, StringUtil.substitute(GLang.GetLang(84, new object[0]), new object[]
				{
					otherClient.ClientData.RoleName,
					(leaveType <= 0) ? GLang.GetLang(85, new object[0]) : GLang.GetLang(86, new object[0]),
					bhName
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				otherClient.ClientData.Faction = 0;
				otherClient.ClientData.BHName = "";
				otherClient.ClientData.BHZhiWu = 0;
				GameManager.ClientMgr.ChangeBangHuiName(sl, pool, otherClient);
				GlobalEventSource4Scene.getInstance().fireEvent(new PostBangHuiChangeEventObject(otherClient, bhid), 10000);
				Global.RemoveBufferData(otherClient, 16);
				UnionPalaceManager.initSetUnionPalaceProps(otherClient, true);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
				Global.UpdateChengHaoBuff(otherClient);
			}
		}

		// Token: 0x06002290 RID: 8848 RVA: 0x001DC6E0 File Offset: 0x001DA8E0
		public void NotifyBangHuiDestroy(int retCode, int roleID, int bhid)
		{
			MoYuLongXue.OnBangHuiDestroy(bhid);
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.Faction == bhid)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						client.ClientData.Faction = 0;
						client.ClientData.BHName = "";
						client.ClientData.BHZhiWu = 0;
						string strcmd = string.Format("{0}:{1}:{2}", retCode, roleID, bhid);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 305);
						if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						Global.RemoveBufferData(client, 16);
						UnionPalaceManager.initSetUnionPalaceProps(client, true);
						client.ClientData.AllyList = null;
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		// Token: 0x06002291 RID: 8849 RVA: 0x001DC82C File Offset: 0x001DAA2C
		public void NotifyBangHuiUpLevel(int bhid, int serverID, int level, bool isKF)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.Faction == bhid && !client.ClientSocket.IsKuaFuLogin)
				{
					UnionPalaceManager.initSetUnionPalaceProps(client, true);
				}
			}
			if (AllyManager.getInstance().IsAllyOpen(level))
			{
				AllyManager.getInstance().UnionDataChange(bhid, serverID, false, 0);
			}
		}

		// Token: 0x06002292 RID: 8850 RVA: 0x001DC8A8 File Offset: 0x001DAAA8
		public void NotifyBangHuiChangeName(int bhid, string newName)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.Faction == bhid)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						client.ClientData.BHName = newName;
						string strcmd = string.Format("{0}:{1}", bhid, newName);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 1315);
						if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		// Token: 0x06002293 RID: 8851 RVA: 0x001DC960 File Offset: 0x001DAB60
		public void NotifyRefuseApplyToBHMember(GameClient otherClient, string bhRoleName, string bhName)
		{
			if (otherClient.ClientData.Faction <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(87, new object[0]), new object[]
				{
					bhRoleName,
					bhName
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
		}

		// Token: 0x06002294 RID: 8852 RVA: 0x001DC9CC File Offset: 0x001DABCC
		public void NotifyRefuseInviteToBHMember(GameClient otherClient, string bhRoleName, string bhName)
		{
			if (otherClient.ClientData.Faction > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(88, new object[0]), new object[]
				{
					bhRoleName,
					bhName
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
		}

		// Token: 0x06002295 RID: 8853 RVA: 0x001DCA34 File Offset: 0x001DAC34
		public void NotifySelfBangGongChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BangGong, client.ClientData.BGMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 316);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002296 RID: 8854 RVA: 0x001DCAA0 File Offset: 0x001DACA0
		public bool AddBangGong(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, ref int addBangGong, AddBangGongTypes addBangGongType, int nBangGongLimit = 0)
		{
			int oldBangGong = client.ClientData.BangGong;
			int dayID = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.BGDayID1 != dayID)
			{
				client.ClientData.BGMoney = 0;
				client.ClientData.BGDayID1 = dayID;
			}
			if (client.ClientData.BGDayID2 != dayID)
			{
				client.ClientData.BGGoods = 0;
				client.ClientData.BGDayID2 = dayID;
			}
			if (AddBangGongTypes.BGGold == addBangGongType)
			{
				int oldBGMoney = client.ClientData.BGMoney;
				client.ClientData.BGMoney = Global.GMin(client.ClientData.BGMoney + addBangGong, nBangGongLimit);
				addBangGong = client.ClientData.BGMoney - oldBGMoney;
			}
			else if (AddBangGongTypes.BGGoods == addBangGongType)
			{
				int oldBGGoods = client.ClientData.BGGoods;
				client.ClientData.BGGoods = Global.GMin(client.ClientData.BGGoods + addBangGong, nBangGongLimit);
				addBangGong = client.ClientData.BGGoods - oldBGGoods;
			}
			bool result;
			if (0 == addBangGong)
			{
				result = true;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.BGDayID1,
					client.ClientData.BGMoney,
					client.ClientData.BGDayID2,
					client.ClientData.BGGoods,
					addBangGong
				});
				string[] dbFields = Global.ExecuteDBCmd(10071, strcmd, client.ServerId);
				if (null == dbFields)
				{
					result = false;
				}
				else if (dbFields.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(dbFields[1]) < 0)
				{
					result = false;
				}
				else
				{
					client.ClientData.BangGong = Convert.ToInt32(dbFields[1]);
					ChengJiuManager.OnRoleGuildChengJiu(client);
					GameManager.ClientMgr.NotifySelfBangGongChange(sl, pool, client);
					Global.AddRoleBangGongEvent(client, oldBangGong);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BangGong, (long)addBangGong, (long)client.ClientData.BangGong, addBangGongType.ToString());
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002297 RID: 8855 RVA: 0x001DCD38 File Offset: 0x001DAF38
		public bool AddBangGong(GameClient client, ref int addBangGong, AddBangGongTypes addBangGongType, int nBangGongLimit = 0)
		{
			return this.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref addBangGong, addBangGongType, nBangGongLimit);
		}

		// Token: 0x06002298 RID: 8856 RVA: 0x001DCD74 File Offset: 0x001DAF74
		public bool SubUserBangGong(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subBangGong)
		{
			int oldBangGong = client.ClientData.BangGong;
			bool result;
			if (client.ClientData.BangGong < subBangGong)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.BGDayID1,
					client.ClientData.BGMoney,
					client.ClientData.BGDayID2,
					client.ClientData.BGGoods,
					-subBangGong
				});
				string[] dbFields = Global.ExecuteDBCmd(10071, strcmd, client.ServerId);
				if (null == dbFields)
				{
					result = false;
				}
				else if (dbFields.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(dbFields[1]) < 0)
				{
					result = false;
				}
				else
				{
					client.ClientData.BangGong = Convert.ToInt32(dbFields[1]);
					GameManager.ClientMgr.NotifySelfBangGongChange(sl, pool, client);
					Global.AddRoleBangGongEvent(client, oldBangGong);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BangGong, (long)(-(long)subBangGong), (long)client.ClientData.BangGong, "none");
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002299 RID: 8857 RVA: 0x001DCEDC File Offset: 0x001DB0DC
		public bool AddBangHuiTongQian(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int bhid, int addMoney)
		{
			int rid = (client == null) ? 0 : client.ClientData.RoleID;
			int serverId = (client == null) ? GameManager.ServerId : client.ServerId;
			string strcmd = string.Format("{0}:{1}:{2}", rid, bhid, addMoney);
			string[] dbFields = Global.ExecuteDBCmd(10077, strcmd, serverId);
			bool result;
			if (null == dbFields)
			{
				result = false;
			}
			else if (dbFields.Length != 2)
			{
				result = false;
			}
			else if (Convert.ToInt32(dbFields[0]) < 0)
			{
				result = false;
			}
			else
			{
				if (null != client)
				{
					GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, bhid);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0600229A RID: 8858 RVA: 0x001DCFA0 File Offset: 0x001DB1A0
		public bool SubBangHuiTongQian(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, out int bhZoneID)
		{
			bhZoneID = 0;
			bool result;
			if (client.ClientData.Faction <= 0)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, subMoney);
				string[] dbFields = Global.ExecuteDBCmd(10072, strcmd, client.ServerId);
				if (null == dbFields)
				{
					result = false;
				}
				else if (dbFields.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(dbFields[0]) < 0)
				{
					result = false;
				}
				else
				{
					bhZoneID = Global.SafeConvertToInt32(dbFields[1]);
					GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, client.ClientData.Faction);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600229B RID: 8859 RVA: 0x001DD074 File Offset: 0x001DB274
		public void NotifyBangHuiZiJinChanged(GameClient client, int bhid)
		{
			int roleID = client.ClientData.RoleID;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			if (null != bangHuiDetailData)
			{
				if (roleID != bangHuiDetailData.BZRoleID)
				{
					GameClient clientBZ = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
					if (null != clientBZ)
					{
						clientBZ.sendCmd(709, string.Format("{0}:{1}", bhid, bangHuiDetailData.TotalMoney), false);
					}
				}
				if (client.ClientData.Faction == bhid)
				{
					client.sendCmd(709, string.Format("{0}:{1}", bhid, bangHuiDetailData.TotalMoney), false);
				}
			}
		}

		// Token: 0x0600229C RID: 8860 RVA: 0x001DD138 File Offset: 0x001DB338
		public void NotifyOtherJunQiLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int junQiID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", junQiID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strcmd, 320);
			}
		}

		// Token: 0x0600229D RID: 8861 RVA: 0x001DD180 File Offset: 0x001DB380
		public void NotifyMySelfNewJunQi(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem junQiItem)
		{
			JunQiData junQiData = Global.JunQiItem2JunQiData(junQiItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JunQiData>(junQiData, pool, 321);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600229E RID: 8862 RVA: 0x001DD1B8 File Offset: 0x001DB3B8
		public void NotifyMySelfDelJunQi(SocketListener sl, TCPOutPacketPool pool, GameClient client, int junQiID)
		{
			string strcmd = string.Format("{0}", junQiID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 322);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600229F RID: 8863 RVA: 0x001DD1FC File Offset: 0x001DB3FC
		public void NotifyLingDiForBHMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 323);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022A0 RID: 8864 RVA: 0x001DD22C File Offset: 0x001DB42C
		public void NotifyAllLingDiForBHMsg(SocketListener sl, TCPOutPacketPool pool, int lingDiID, int bhid, int zoneID, string bhName, int tax)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				lingDiID,
				bhid,
				zoneID,
				bhName,
				tax
			});
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (!gc.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyLingDiForBHMsg(sl, pool, gc, strcmd);
				}
			}
		}

		// Token: 0x060022A1 RID: 8865 RVA: 0x001DD2BC File Offset: 0x001DB4BC
		public void NotifyAllLuoLanChengZhanRequestInfoList(List<LuoLanChengZhanRequestInfoEx> list)
		{
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (!gc.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyLuoLanChengZhanRequestInfoList(gc, list);
				}
			}
		}

		// Token: 0x060022A2 RID: 8866 RVA: 0x001DD305 File Offset: 0x001DB505
		public void NotifyLuoLanChengZhanRequestInfoList(GameClient client, List<LuoLanChengZhanRequestInfoEx> list)
		{
			client.sendCmd<List<LuoLanChengZhanRequestInfoEx>>(708, list, false);
		}

		// Token: 0x060022A3 RID: 8867 RVA: 0x001DD318 File Offset: 0x001DB518
		public void HandleBHJunQiUpLevel(int bhid, int junQiLevel)
		{
			double[] actionParams = new double[]
			{
				(double)junQiLevel - 1.0
			};
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (gc.ClientData.Faction == bhid)
				{
					Global.UpdateBufferData(gc, BufferItemTypes.JunQi, actionParams, 1, true);
				}
			}
		}

		// Token: 0x060022A4 RID: 8868 RVA: 0x001DD37C File Offset: 0x001DB57C
		public void NotifyOtherFakeRoleLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int FakeRoleID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", FakeRoleID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strcmd, 588);
			}
		}

		// Token: 0x060022A5 RID: 8869 RVA: 0x001DD3C4 File Offset: 0x001DB5C4
		public void NotifyMySelfNewFakeRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, FakeRoleItem FakeRoleItem)
		{
			FakeRoleData FakeRoleData = Global.FakeRoleItem2FakeRoleData(FakeRoleItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FakeRoleData>(FakeRoleData, pool, 589);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022A6 RID: 8870 RVA: 0x001DD3FC File Offset: 0x001DB5FC
		public void NotifyMySelfDelFakeRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, int FakeRoleID)
		{
			string strcmd = string.Format("{0}", FakeRoleID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 590);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022A7 RID: 8871 RVA: 0x001DD440 File Offset: 0x001DB640
		public void NotifyAllDelFakeRole(SocketListener sl, TCPOutPacketPool pool, FakeRoleItem fakeRoleItem)
		{
			List<object> objsList = Global.GetAll9Clients(fakeRoleItem);
			string strcmd = string.Format("{0}", fakeRoleItem.FakeRoleID);
			this.SendToClients(sl, pool, null, objsList, strcmd, 590);
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x001DD47C File Offset: 0x001DB67C
		public void NotifyChgHuangDiRoleIDMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 324);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x001DD4AC File Offset: 0x001DB6AC
		public void NotifyAllChgHuangDiRoleIDMsg(SocketListener sl, TCPOutPacketPool pool, int oldHuangDiRoleID, int huangDiRoleID)
		{
			string strcmd = string.Format("{0}:{1}", oldHuangDiRoleID, huangDiRoleID);
			int index = 0;
			GameClient gc;
			while ((gc = this.GetNextClient(ref index, false)) != null)
			{
				if (!gc.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyChgHuangDiRoleIDMsg(sl, pool, gc, strcmd);
				}
			}
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x001DD510 File Offset: 0x001DB710
		public void NotifyInviteAddHuangFei(GameClient client, int otherRoleID, string otherRoleName, int randNum)
		{
			string strcmd = string.Format("{0}:{1}:{2}", otherRoleID, otherRoleName, randNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 351);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x001DD56C File Offset: 0x001DB76C
		public void NotifyChgHuangHou(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.HuangHou);
				this.SendToClients(sl, pool, null, objsList, strcmd, 347);
			}
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x001DD5CC File Offset: 0x001DB7CC
		public void NotifyLingDiMapInfoData(GameClient client, LingDiMapInfoData lingDiMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<LingDiMapInfoData>(lingDiMapInfoData, Global._TCPManager.TcpOutPacketPool, 348);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x001DD610 File Offset: 0x001DB810
		public void NotifyAllLingDiMapInfoData(int mapCode, LingDiMapInfoData lingDiMapInfoData)
		{
			List<object> objsList = this.GetMapClients(mapCode);
			if (null != objsList)
			{
				objsList = Global.ConvertObjsList(mapCode, -1, objsList, false);
				byte[] bytesData = DataHelper.ObjectToBytes<LingDiMapInfoData>(lingDiMapInfoData);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, bytesData, 348);
			}
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x001DD668 File Offset: 0x001DB868
		public void NotifyHuangChengMapInfoData(GameClient client, HuangChengMapInfoData huangChengMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HuangChengMapInfoData>(huangChengMapInfoData, Global._TCPManager.TcpOutPacketPool, 349);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x001DD6AC File Offset: 0x001DB8AC
		public void NotifyAllHuangChengMapInfoData(int mapCode, HuangChengMapInfoData huangChengMapInfoData)
		{
			List<object> objsList = this.GetMapClients(mapCode);
			if (null != objsList)
			{
				objsList = Global.ConvertObjsList(mapCode, -1, objsList, false);
				byte[] bytesData = DataHelper.ObjectToBytes<HuangChengMapInfoData>(huangChengMapInfoData);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, bytesData, 349);
			}
		}

		// Token: 0x060022B0 RID: 8880 RVA: 0x001DD704 File Offset: 0x001DB904
		public void NotifyWangChengMapInfoData(GameClient client, WangChengMapInfoData wangChengMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<WangChengMapInfoData>(wangChengMapInfoData, Global._TCPManager.TcpOutPacketPool, 454);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022B1 RID: 8881 RVA: 0x001DD748 File Offset: 0x001DB948
		public void NotifyAllWangChengMapInfoData(WangChengMapInfoData wangChengMapInfoData)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				this.NotifyWangChengMapInfoData(client, wangChengMapInfoData);
			}
		}

		// Token: 0x060022B2 RID: 8882 RVA: 0x001DD77C File Offset: 0x001DB97C
		public bool AddLingDiTaxMoney(int bhid, int lingDiID, int addMoney)
		{
			string strcmd = string.Format("{0}:{1}:{2}", bhid, lingDiID, addMoney);
			string[] dbFields = Global.ExecuteDBCmd(350, strcmd, 0);
			return null != dbFields && dbFields.Length == 4 && Convert.ToInt32(dbFields[0]) >= 0;
		}

		// Token: 0x060022B3 RID: 8883 RVA: 0x001DD7F0 File Offset: 0x001DB9F0
		public void NotifySelfSuiTangBattleAward(SocketListener sl, TCPOutPacketPool pool, GameClient client, int nPoint1, int nPoint2, long experience, int bindYuanBao, int chengJiu, bool bIsSuccess, int paiMing, string awardsGoods)
		{
			int nSelfPoint = client.ClientData.BattleKilledNum;
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				client.ClientData.RoleID,
				bIsSuccess,
				nPoint1,
				nPoint2,
				nSelfPoint,
				experience,
				chengJiu,
				bindYuanBao,
				paiMing,
				awardsGoods
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 360);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022B4 RID: 8884 RVA: 0x001DD8AC File Offset: 0x001DBAAC
		public bool NotifyLastUserMail(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int mailID)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, mailID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 369);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
			return true;
		}

		// Token: 0x060022B5 RID: 8885 RVA: 0x001DD904 File Offset: 0x001DBB04
		public void SendMailWhenPacketFull(GameClient client, List<GoodsData> awardsItemList, string sContent, string sSubject)
		{
			int nTotalGroup = awardsItemList.Count / 5;
			int nRemain = awardsItemList.Count % 5;
			int nCount = 0;
			if (nTotalGroup > 0)
			{
				for (int i = 0; i < nTotalGroup; i++)
				{
					List<GoodsData> goods = new List<GoodsData>();
					for (int j = 0; j < 5; j++)
					{
						goods.Add(awardsItemList[nCount]);
						nCount++;
					}
					Global.UseMailGivePlayerAward2(client, goods, sContent, sSubject, 0, 0, 0);
				}
			}
			if (nRemain > 0)
			{
				List<GoodsData> goods2 = new List<GoodsData>();
				for (int i = 0; i < nRemain; i++)
				{
					goods2.Add(awardsItemList[nCount]);
					nCount++;
				}
				Global.UseMailGivePlayerAward2(client, goods2, sContent, sSubject, 0, 0, 0);
			}
		}

		// Token: 0x060022B6 RID: 8886 RVA: 0x001DD9DC File Offset: 0x001DBBDC
		public void NotifyVipDailyData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<VipDailyData>>(client.ClientData.VipDailyDataList, Global._TCPManager.TcpOutPacketPool, 389);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022B7 RID: 8887 RVA: 0x001DDA28 File Offset: 0x001DBC28
		public void NotifyYangGongBKAwardDailyData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<YangGongBKDailyJiFenData>(client.ClientData.YangGongBKDailyJiFen, Global._TCPManager.TcpOutPacketPool, 392);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022B8 RID: 8888 RVA: 0x001DDA74 File Offset: 0x001DBC74
		public void NotifyAllShengXiaoGuessStateMsg(SocketListener sl, TCPOutPacketPool pool, int shengXiaoGuessState, int extraParams, int minLevel, int preGuessResult)
		{
			string strcmd = string.Format("{0}:{1}:{2}", shengXiaoGuessState, extraParams, preGuessResult);
			List<object> objsList = GameManager.ClientMgr.GetMapClients(GameManager.ShengXiaoGuessMgr.GuessMapCode);
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient client = objsList[i] as GameClient;
						if (client != null)
						{
							if (client.ClientData.Level >= minLevel)
							{
								if (null == tcpOutPacket)
								{
									tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 398);
								}
								if (!sl.SendData(client.ClientSocket, tcpOutPacket, false))
								{
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x060022B9 RID: 8889 RVA: 0x001DDB70 File Offset: 0x001DBD70
		public void NotifyShengXiaoGuessResultMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string sResult)
		{
			if (null != client)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, sResult);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 399);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060022BA RID: 8890 RVA: 0x001DDBCC File Offset: 0x001DBDCC
		public void NotifyClientShengXiaoGuessStateMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, int shengXiaoGuessState, int extraParams, int minLevel, int preGuessResult)
		{
			if (client != null && client.ClientData.Level >= minLevel)
			{
				string strcmd = string.Format("{0}:{1}:{2}", shengXiaoGuessState, extraParams, preGuessResult);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 398);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060022BB RID: 8891 RVA: 0x001DDC3C File Offset: 0x001DBE3C
		public void NotifyMySelfNewNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, NPC npc)
		{
			if (npc != null && null != npc.RoleBufferData)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, npc.RoleBufferData, 0, npc.RoleBufferData.Length, 406);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060022BC RID: 8892 RVA: 0x001DDC98 File Offset: 0x001DBE98
		public void NotifyMySelfNewNPCBy9Grid(SocketListener sl, TCPOutPacketPool pool, NPC npc)
		{
			if (npc != null && null != npc.RoleBufferData)
			{
				List<object> objsList = Global.GetAll9GridObjects(npc);
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						this.NotifyMySelfNewNPC(sl, pool, objsList[i] as GameClient, npc);
					}
				}
			}
		}

		// Token: 0x060022BD RID: 8893 RVA: 0x001DDD0C File Offset: 0x001DBF0C
		public void NotifyMySelfDelNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, int mapCode, int npcID)
		{
			string strcmd = string.Format("{0}:{1}", npcID, mapCode);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 407);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022BE RID: 8894 RVA: 0x001DDD54 File Offset: 0x001DBF54
		public void NotifyMySelfDelNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, NPC npc)
		{
			this.NotifyMySelfDelNPC(sl, pool, client, npc.MapCode, npc.NpcID);
		}

		// Token: 0x060022BF RID: 8895 RVA: 0x001DDD70 File Offset: 0x001DBF70
		public void NotifyMySelfDelNPCBy9Grid(SocketListener sl, TCPOutPacketPool pool, NPC npc)
		{
			List<object> objsList = Global.GetAll9GridObjects(npc);
			for (int i = 0; i < objsList.Count; i++)
			{
				if (objsList[i] is GameClient)
				{
					this.NotifyMySelfDelNPC(sl, pool, objsList[i] as GameClient, npc.MapCode, npc.NpcID);
				}
			}
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x001DDDD4 File Offset: 0x001DBFD4
		private bool TryDirectMove(GameClient client, long startMoveTicks, List<Point> path)
		{
			int endGridX = (int)path[path.Count - 1].X;
			int endGridY = (int)path[path.Count - 1].Y;
			bool result;
			if (Global.GetTwoPointDistance(client.CurrentGrid, new Point((double)endGridX, (double)endGridY)) >= 3.0)
			{
				result = false;
			}
			else if (path.Count > 2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < path.Count; i++)
				{
					Point clientGrid = client.CurrentGrid;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int gridX = (int)path[i].X;
					int gridY = (int)path[i].Y;
					if (gridX != (int)clientGrid.X || gridY != (int)clientGrid.Y)
					{
						if (Global.InObsByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, gridX, gridY, 0, 0))
						{
							int direction = client.ClientData.RoleDirection;
							int tryRun = 0;
							GameManager.ClientMgr.NotifyOthersMyMovingEnd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, 0, client.ClientData.PosX, client.ClientData.PosY, direction, tryRun, true, null);
							break;
						}
						int toX = gridX * mapGrid.MapGridWidth + mapGrid.MapGridWidth / 2;
						int toY = gridY * mapGrid.MapGridHeight + mapGrid.MapGridHeight / 2;
						client.ClientData.PosX = toX;
						client.ClientData.PosY = toY;
						mapGrid.MoveObject(-1, -1, toX, toY, client);
						client.ClientData.CurrentAction = 0;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x001DDFD8 File Offset: 0x001DC1D8
		public bool StartClientStoryboard(GameClient client, long startMoveTicks, List<Point> path, bool stepMove)
		{
			StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			bool result;
			if (path.Count <= 1)
			{
				result = false;
			}
			else
			{
				path.RemoveAt(0);
				if (this.TryDirectMove(client, startMoveTicks, path))
				{
					ClientManager.DoSpriteMapGridMove(client, 1);
					result = true;
				}
				else if (stepMove)
				{
					result = false;
				}
				else
				{
					StoryBoard4Client sb = new StoryBoard4Client(client.ClientData.RoleID);
					sb.Completed = new StoryBoard4Client.CompletedDelegateHandle(this.Move_Completed);
					GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
					long ticks = TimeUtil.NOW() * 10000L - 621356256000000000L;
					ticks /= 10000L;
					startMoveTicks -= 62135625600000L;
					long elapsedTicks = 0L;
					sb.Start(client, path, gameMap.MapGridWidth, gameMap.MapGridHeight, elapsedTicks);
					sb.Binding();
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060022C2 RID: 8898 RVA: 0x001DE0E4 File Offset: 0x001DC2E4
		public void Move_Completed(object sender, EventArgs e)
		{
			StoryBoard4Client sb = sender as StoryBoard4Client;
			StoryBoard4Client.RemoveStoryBoard(sb.RoleID);
			if (sb.IsStopped())
			{
				GameClient client = GameManager.ClientMgr.FindClient(sb.RoleID);
				if (null != client)
				{
					GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
					int toX = gameMap.CorrectWidthPointToGridPoint(client.ClientData.PosX);
					int toY = gameMap.CorrectHeightPointToGridPoint(client.ClientData.PosY);
					client.ClientData.PosX = toX;
					client.ClientData.PosY = toY;
					client.ClientData.CurrentAction = 0;
					int direction = client.ClientData.RoleDirection;
					int tryRun = 1;
					GameManager.ClientMgr.NotifyOthersMyMovingEnd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, 0, toX, toY, direction, tryRun, true, null);
					ClientManager.DoSpriteMapGridMove(client, 0);
				}
			}
			else
			{
				GameClient client = GameManager.ClientMgr.FindClient(sb.RoleID);
				if (null != client)
				{
					client.ClientData.CurrentAction = 0;
					client.ClientData.MoveSpeed = 1.0;
					client.ClientData.DestPoint = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
					ClientManager.DoSpriteMapGridMove(client, 0);
				}
			}
		}

		// Token: 0x060022C3 RID: 8899 RVA: 0x001DE264 File Offset: 0x001DC464
		public bool StopClientStoryboard(GameClient client, long clientTicks = 0L, int posX = -1, int posY = -1)
		{
			if (clientTicks > 0L)
			{
				StoryBoard4Client sb = StoryBoard4Client.StopStoryBoard(client.ClientData.RoleID, clientTicks);
				if (sb != null && posX >= 0)
				{
					if (Math.Abs(sb.LastPoint.X - (double)posX) + Math.Abs(sb.LastPoint.Y - (double)posY) >= 300.0)
					{
						ClientCmdCheck.ResetClientPosition(client, sb.CurrentX, sb.CurrentY);
						return false;
					}
				}
			}
			else
			{
				StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			}
			return true;
		}

		// Token: 0x060022C4 RID: 8900 RVA: 0x001DE314 File Offset: 0x001DC514
		public void StopClientStoryboard(GameClient client, int stopIndex)
		{
			if (stopIndex > 0)
			{
				StoryBoard4Client.StopStoryBoard(client.ClientData.RoleID, stopIndex);
			}
			else
			{
				StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			}
		}

		// Token: 0x060022C5 RID: 8901 RVA: 0x001DE358 File Offset: 0x001DC558
		public bool GetClientStoryboardLastPoint(GameClient client, out Point lastPoint)
		{
			lastPoint = new Point(0.0, 0.0);
			StoryBoard4Client sb = StoryBoard4Client.FindStoryBoard(client.ClientData.RoleID);
			bool result;
			if (null != sb)
			{
				lastPoint = sb.LastPoint;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060022C6 RID: 8902 RVA: 0x001DE3B4 File Offset: 0x001DC5B4
		public bool AddEquipStrong(GameClient client, GoodsData goodsData, int subStrong)
		{
			int maxStrong = Global.GetEquipGoodsMaxStrong(goodsData.GoodsID);
			bool result;
			if (goodsData.Strong >= maxStrong)
			{
				result = false;
			}
			else
			{
				int oldStrong = goodsData.Strong;
				int modValue = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
				goodsData.Strong = Math.Min(goodsData.Strong + subStrong, maxStrong);
				int modValue2 = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
				bool hasNotifyClient = false;
				if (modValue != modValue2)
				{
					if (goodsData.Strong < maxStrong)
					{
						Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
					}
					else
					{
						Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW() - 7200000L, false);
					}
					this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
					hasNotifyClient = true;
				}
				else
				{
					Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
				}
				if (oldStrong < maxStrong && goodsData.Strong >= maxStrong)
				{
					if (!hasNotifyClient)
					{
						Global.UpdateEquipStrong(client, goodsData);
						this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
					}
					Global.RefreshEquipPropAndNotify(client);
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060022C7 RID: 8903 RVA: 0x001DE504 File Offset: 0x001DC704
		public int SubEquipStrong(GameClient client, GoodsData goodsData, int subStrong)
		{
			int modValue = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
			goodsData.Strong = Math.Max(0, goodsData.Strong - subStrong);
			int modValue2 = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
			if (modValue != modValue2)
			{
				Global.UpdateEquipStrong(client, goodsData);
				this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
			}
			else
			{
				Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
			}
			return modValue2;
		}

		// Token: 0x060022C8 RID: 8904 RVA: 0x001DE58C File Offset: 0x001DC78C
		public void NotifyMySelfEquipStrong(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, goodsData.Id, goodsData.Strong);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 412);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x060022C9 RID: 8905 RVA: 0x001DE5F0 File Offset: 0x001DC7F0
		public void NotifyDSHideCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.DSHideStart);
				this.SendToClients(sl, pool, null, objsList, strcmd, 422);
			}
		}

		// Token: 0x060022CA RID: 8906 RVA: 0x001DE650 File Offset: 0x001DC850
		public void CheckDSHideState(GameClient client)
		{
			if (client.ClientData.DSHideStart > 0L)
			{
				long nowTicks = TimeUtil.NOW();
				if (nowTicks >= client.ClientData.DSHideStart)
				{
					Global.RemoveBufferData(client, 41);
					client.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
			}
		}

		// Token: 0x060022CB RID: 8907 RVA: 0x001DE6CC File Offset: 0x001DC8CC
		public void NotifyRoleStatusCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int statusID, long startTicks, int slotSeconds, double tag = 0.0)
		{
			switch (statusID)
			{
			case 2:
			case 3:
			case 8:
				ClientCmdCheck.MoveSpeedChange(client, tag);
				ClientCmdCheck.ClientAction(client, 0L, (long)(slotSeconds * 1000));
				break;
			case 4:
			case 11:
			case 12:
			case 14:
				ClientCmdCheck.MoveSpeedChange(client, tag);
				break;
			}
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					statusID,
					startTicks,
					slotSeconds,
					tag
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 456);
			}
		}

		// Token: 0x060022CC RID: 8908 RVA: 0x001DE7C0 File Offset: 0x001DC9C0
		public void NotifyMonsterStatusCmd(SocketListener sl, TCPOutPacketPool pool, Monster monster, int statusID, long startTicks, int slotSeconds, double tag = 0.0)
		{
			List<object> objsList = Global.GetAll9Clients(monster);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					monster.RoleID,
					statusID,
					startTicks,
					slotSeconds,
					tag
				});
				this.SendToClients(sl, pool, null, objsList, strcmd, 456);
			}
		}

		// Token: 0x060022CD RID: 8909 RVA: 0x001DE83C File Offset: 0x001DCA3C
		public void NotifyMySelfNewDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, Decoration deco)
		{
			if (null != deco)
			{
				DecorationData decoData = new DecorationData
				{
					AutoID = deco.AutoID,
					DecoID = deco.DecoID,
					MapCode = deco.MapCode,
					PosX = (int)deco.Pos.X,
					PosY = (int)deco.Pos.Y,
					StartTicks = deco.StartTicks,
					MaxLiveTicks = deco.MaxLiveTicks,
					AlphaTicks = deco.AlphaTicks
				};
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DecorationData>(decoData, Global._TCPManager.TcpOutPacketPool, 423);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060022CE RID: 8910 RVA: 0x001DE904 File Offset: 0x001DCB04
		public void NotifyMySelfDelDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, Decoration deco)
		{
			if (null != deco)
			{
				string strcmd = string.Format("{0}", deco.AutoID);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 424);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		// Token: 0x060022CF RID: 8911 RVA: 0x001DE95C File Offset: 0x001DCB5C
		public void NotifyOthersDelDeco(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int mapCode, int autoID)
		{
			if (null != objsList)
			{
				string strcmd = string.Format("{0}", autoID);
				this.SendToClients(sl, pool, null, objsList, strcmd, 424);
			}
		}

		// Token: 0x060022D0 RID: 8912 RVA: 0x001DE99C File Offset: 0x001DCB9C
		public bool ModifyFuWenZhiChenPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)client.ClientData.FuWenZhiChen;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.FuWenZhiChen = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "符文之尘", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.FuWenZhiChen, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10187", client.ClientData.FuWenZhiChen, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.FuWenZhiChen, (long)addValue, (long)client.ClientData.FuWenZhiChen, strFrom);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FuWenZhiChen, client.ClientData.FuWenZhiChen);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060022D1 RID: 8913 RVA: 0x001DEAC0 File Offset: 0x001DCCC0
		public void ModifyShenLiJingHuaPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.ShenLiJingHuaPoints += addValue;
				client.ClientData.ShenLiJingHuaPoints = Math.Max(client.ClientData.ShenLiJingHuaPoints, 0);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神力精华", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ShenLiJingHuaPoints, client.ServerId, null);
				Global.SaveRoleParamsInt32ValueToDB(client, "10157", client.ClientData.ShenLiJingHuaPoints, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenLiJingHua, (long)addValue, (long)client.ClientData.ShenLiJingHuaPoints, strFrom);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenLiJingHua, client.ClientData.ShenLiJingHuaPoints);
				}
			}
		}

		// Token: 0x060022D2 RID: 8914 RVA: 0x001DEBB0 File Offset: 0x001DCDB0
		public void ModifyChengJiuPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.ChengJiuPoints += addValue;
				client.ClientData.ChengJiuPoints = Math.Max(client.ClientData.ChengJiuPoints, 0);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ChengJiuPoints, client.ServerId, null);
				ChengJiuManager.ModifyChengJiuExtraData(client, (uint)client.ClientData.ChengJiuPoints, ChengJiuExtraDataField.ChengJiuPoints, true);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ChengJiu, (long)addValue, (long)client.ClientData.ChengJiuPoints, strFrom);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiu, client.ClientData.ChengJiuPoints);
				}
				client._IconStateMgr.CheckChengJiuUpLevelState(client);
			}
		}

		// Token: 0x060022D3 RID: 8915 RVA: 0x001DECA8 File Offset: 0x001DCEA8
		public int GetChengJiuPointsValue(GameClient client)
		{
			client.ClientData.ChengJiuPoints = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ChengJiuPoints);
			return client.ClientData.ChengJiuPoints;
		}

		// Token: 0x060022D4 RID: 8916 RVA: 0x001DECD8 File Offset: 0x001DCED8
		public int SetChengJiuLevelValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			client.ClientData.ChengJiuLevel = ChengJiuManager.GetChengJiuLevel(client);
			client.ClientData.ChengJiuLevel += addValue;
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就等级", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ChengJiuLevel, client.ServerId, null);
			ChengJiuManager.SetChengJiuLevel(client, client.ClientData.ChengJiuLevel, true);
			if (notifyClient)
			{
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiuLevel, client.ClientData.ChengJiuLevel);
			}
			return client.ClientData.ChengJiuLevel;
		}

		// Token: 0x060022D5 RID: 8917 RVA: 0x001DED9C File Offset: 0x001DCF9C
		public void ModifyZhuangBeiJiFenValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetZhuangBeiJiFenValue(client) + addValue;
				this.SaveZhuangBeiJiFenValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen, newValue);
				}
			}
		}

		// Token: 0x060022D6 RID: 8918 RVA: 0x001DEDE1 File Offset: 0x001DCFE1
		public void SaveZhuangBeiJiFenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhuangBeiJiFen", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022D7 RID: 8919 RVA: 0x001DEDF8 File Offset: 0x001DCFF8
		public int GetZhuangBeiJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhuangBeiJiFen", "2020-12-12 12:12:12");
		}

		// Token: 0x060022D8 RID: 8920 RVA: 0x001DEE1C File Offset: 0x001DD01C
		public void ModifyLieShaValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetLieShaValue(client) + addValue;
				this.SaveLieShaValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.LieShaZhi, newValue);
				}
			}
		}

		// Token: 0x060022D9 RID: 8921 RVA: 0x001DEE61 File Offset: 0x001DD061
		public void SaveLieShaValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "LieShaZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022DA RID: 8922 RVA: 0x001DEE78 File Offset: 0x001DD078
		public int GetLieShaValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "LieShaZhi", "2020-12-12 12:12:12");
		}

		// Token: 0x060022DB RID: 8923 RVA: 0x001DEE9C File Offset: 0x001DD09C
		public void ModifyWuXingValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true, bool doChangeWuXueLevel = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetWuXingValue(client) + addValue;
				this.SaveWuXingValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.WuXingZhi, newValue);
				}
				if (doChangeWuXueLevel)
				{
					if (addValue > 0)
					{
						Global.TryToActivateSpecialWuXueLevel(client);
					}
					else
					{
						Global.TryToDeActivateSpecialWuXueLevel(client);
					}
				}
			}
		}

		// Token: 0x060022DC RID: 8924 RVA: 0x001DEF0B File Offset: 0x001DD10B
		public void SaveWuXingValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WuXingZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022DD RID: 8925 RVA: 0x001DEF24 File Offset: 0x001DD124
		public int GetWuXingValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WuXingZhi", "2020-12-12 12:12:12");
		}

		// Token: 0x060022DE RID: 8926 RVA: 0x001DEF48 File Offset: 0x001DD148
		public void ModifyZhenQiValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetZhenQiValue(client) + addValue;
				this.SaveZhenQiValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhenQiZhi, newValue);
				}
			}
		}

		// Token: 0x060022DF RID: 8927 RVA: 0x001DEF8D File Offset: 0x001DD18D
		public void SaveZhenQiValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhenQiZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022E0 RID: 8928 RVA: 0x001DEFA4 File Offset: 0x001DD1A4
		public int GetZhenQiValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhenQiZhi", "2020-12-12 12:12:12");
		}

		// Token: 0x060022E1 RID: 8929 RVA: 0x001DEFC8 File Offset: 0x001DD1C8
		public void ModifyStarSoulValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.StarSoul += addValue;
				if (client.ClientData.StarSoul < 0)
				{
					client.ClientData.StarSoul = 0;
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "星魂", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.StarSoul, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.XingHun, (long)addValue, (long)client.ClientData.StarSoul, strFrom);
				Global.SaveRoleParamsInt32ValueToDB(client, "StarSoul", client.ClientData.StarSoul, true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.StarSoulValue, client.ClientData.StarSoul);
				}
			}
		}

		// Token: 0x060022E2 RID: 8930 RVA: 0x001DF0BC File Offset: 0x001DD2BC
		public void ModifyPetJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int nPetJiFen = Convert.ToInt32(Global.GetRoleParamByName(client, "PetJiFen")) + addValue;
				Global.UpdateRoleParamByName(client, "PetJiFen", nPetJiFen.ToString(), true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.PetJiFen, nPetJiFen);
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "精灵积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, nPetJiFen, client.ServerId, null);
			}
		}

		// Token: 0x060022E3 RID: 8931 RVA: 0x001DF158 File Offset: 0x001DD358
		public bool ModifyYuanSuFenMoValue(GameClient client, int addValue, string strFrom, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				int currPowder = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				long oldValue = (long)currPowder;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newPowder = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					if (newPowder == currPowder)
					{
						result = true;
					}
					else
					{
						addValue = newPowder - currPowder;
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "元素粉末", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newPowder, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.YuanSuFenMo, (long)addValue, (long)newPowder, strFrom);
						Global.SaveRoleParamsInt32ValueToDB(client, "ElementPowder", newPowder, true);
						if (notifyClient)
						{
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.YuansuFenmo, newPowder);
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060022E4 RID: 8932 RVA: 0x001DF270 File Offset: 0x001DD470
		public bool ModifyMUMoHeValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetMUMoHeValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔核", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MUMoHe, (long)addValue, (long)newValue, strFrom);
					this.SaveMUMoHeValue(client, newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MUMoHe, newValue);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060022E5 RID: 8933 RVA: 0x001DF35C File Offset: 0x001DD55C
		public void SaveMUMoHeValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "MUMoHe", nValue, writeToDB);
		}

		// Token: 0x060022E6 RID: 8934 RVA: 0x001DF370 File Offset: 0x001DD570
		public int GetMUMoHeValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "MUMoHe");
		}

		// Token: 0x060022E7 RID: 8935 RVA: 0x001DF390 File Offset: 0x001DD590
		public bool ModifyTianDiJingYuanValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetTianDiJingYuanValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔晶", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JingYuanZhi, (long)addValue, (long)newValue, strFrom);
					this.SaveTianDiJingYuanValue(client, newValue, true);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TianDiJingYuan, newValue);
					}
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.MoJingCntInBag));
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060022E8 RID: 8936 RVA: 0x001DF48C File Offset: 0x001DD68C
		public void SaveTianDiJingYuanValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "TianDiJingYuan", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022E9 RID: 8937 RVA: 0x001DF4A4 File Offset: 0x001DD6A4
		public int GetTianDiJingYuanValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "TianDiJingYuan", "2020-12-12 12:12:12");
		}

		// Token: 0x060022EA RID: 8938 RVA: 0x001DF4C8 File Offset: 0x001DD6C8
		public bool ModifyZaiZaoValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetZaiZaoValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "再造点", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZaiZao, (long)addValue, (long)newValue, strFrom);
					this.SaveZaiZaoValue(client, newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZaiZaoPoint, newValue);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060022EB RID: 8939 RVA: 0x001DF5B3 File Offset: 0x001DD7B3
		public void SaveZaiZaoValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZaiZaoPoint", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022EC RID: 8940 RVA: 0x001DF5CC File Offset: 0x001DD7CC
		public int GetZaiZaoValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZaiZaoPoint", "2020-12-12 12:12:12");
		}

		// Token: 0x060022ED RID: 8941 RVA: 0x001DF5F0 File Offset: 0x001DD7F0
		public void ModifyShiLianLingValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetShiLianLingValue(client) + addValue;
				this.SaveShiLianLingValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShiLianLing, newValue);
				}
			}
		}

		// Token: 0x060022EE RID: 8942 RVA: 0x001DF635 File Offset: 0x001DD835
		public void SaveShiLianLingValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShiLianLing", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022EF RID: 8943 RVA: 0x001DF64C File Offset: 0x001DD84C
		public int GetShiLianLingValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShiLianLing", "2020-12-12 12:12:12");
		}

		// Token: 0x060022F0 RID: 8944 RVA: 0x001DF66E File Offset: 0x001DD86E
		public void SaveJingMaiLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "JingMaiLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022F1 RID: 8945 RVA: 0x001DF684 File Offset: 0x001DD884
		public int GetJingMaiLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "JingMaiLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x060022F2 RID: 8946 RVA: 0x001DF6A6 File Offset: 0x001DD8A6
		public void SaveWuXueLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WuXueLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022F3 RID: 8947 RVA: 0x001DF6BC File Offset: 0x001DD8BC
		public int GetWuXueLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WuXueLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x060022F4 RID: 8948 RVA: 0x001DF6E0 File Offset: 0x001DD8E0
		public void ModifyZuanHuangLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetZuanHuangLevelValue(client) + addValue;
				this.SaveZuanHuangLevelValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZuanHuangLevel, newValue);
				}
			}
		}

		// Token: 0x060022F5 RID: 8949 RVA: 0x001DF726 File Offset: 0x001DD926
		public void SaveZuanHuangLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZuanHuangLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022F6 RID: 8950 RVA: 0x001DF73C File Offset: 0x001DD93C
		public int GetZuanHuangLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZuanHuangLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x060022F7 RID: 8951 RVA: 0x001DF760 File Offset: 0x001DD960
		public void ModifySystemOpenValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (addValue >= 0 && addValue <= 31)
			{
				int newValue = this.GetSystemOpenValue(client) | 1 << addValue;
				this.SaveSystemOpenValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.SystemOpenValue, newValue);
				}
			}
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x001DF7B4 File Offset: 0x001DD9B4
		public int GetScoreBoxState(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ScoreBoxState", "2020-12-12 12:12:12");
		}

		// Token: 0x060022F9 RID: 8953 RVA: 0x001DF7D8 File Offset: 0x001DD9D8
		public void ModifyScoreBoxState(GameClient client, int nOpen)
		{
			if (nOpen >= 0 && nOpen <= 2)
			{
				Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ScoreBoxState", nOpen, true, "2020-12-12 12:12:12");
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ScoreState, nOpen);
			}
		}

		// Token: 0x060022FA RID: 8954 RVA: 0x001DF81A File Offset: 0x001DDA1A
		public void SaveSystemOpenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "SystemOpenValue", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022FB RID: 8955 RVA: 0x001DF830 File Offset: 0x001DDA30
		public int GetSystemOpenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "SystemOpenValue", "2020-12-12 12:12:12");
		}

		// Token: 0x060022FC RID: 8956 RVA: 0x001DF854 File Offset: 0x001DDA54
		public void ModifyJunGongValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetJunGongValue(client) + addValue;
				this.SaveJunGongValue(client, newValue, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JunGongZhi, (long)addValue, (long)newValue, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.JunGong, newValue);
				}
			}
		}

		// Token: 0x060022FD RID: 8957 RVA: 0x001DF8AE File Offset: 0x001DDAAE
		public void SaveJunGongValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "JunGong", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x060022FE RID: 8958 RVA: 0x001DF8C4 File Offset: 0x001DDAC4
		public int GetJunGongValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "JunGong", "2020-12-12 12:12:12");
		}

		// Token: 0x060022FF RID: 8959 RVA: 0x001DF8E8 File Offset: 0x001DDAE8
		public void ModifyKaiFuOnlineDayID(GameClient client, int dayID, bool writeToDB = false, bool notifyClient = true)
		{
			if (dayID >= 1 && dayID <= 7)
			{
				this.SaveKaiFuOnlineDayID(client, dayID, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.KaiFuOnlineDayID, dayID);
				}
			}
		}

		// Token: 0x06002300 RID: 8960 RVA: 0x001DF92C File Offset: 0x001DDB2C
		public void SaveKaiFuOnlineDayID(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "KaiFuOnlineDayID", nValue, writeToDB);
		}

		// Token: 0x06002301 RID: 8961 RVA: 0x001DF940 File Offset: 0x001DDB40
		public int GetKaiFuOnlineDayID(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "KaiFuOnlineDayID");
		}

		// Token: 0x06002302 RID: 8962 RVA: 0x001DF960 File Offset: 0x001DDB60
		public void ModifyTo60or100ID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			this.SaveTo60or100ID(client, nID, writeToDB);
			if (notifyClient)
			{
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.To60or100, nID);
			}
		}

		// Token: 0x06002303 RID: 8963 RVA: 0x001DF98E File Offset: 0x001DDB8E
		public void SaveTo60or100ID(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "To60or100", nValue, writeToDB);
		}

		// Token: 0x06002304 RID: 8964 RVA: 0x001DF9A0 File Offset: 0x001DDBA0
		public int GetTo60or100ID(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "To60or100");
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x001DF9C0 File Offset: 0x001DDBC0
		public void ModifyTreasureJiFenValue(GameClient client, int addValue, string strFrom, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetTreasureJiFen(client) + addValue;
				if (addValue > 0)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "藏宝积分", strFrom, "系统", client.ClientData.RoleName, "增加", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
				}
				else
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "藏宝积分", strFrom, client.ClientData.RoleName, "系统", "减少", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
				}
				this.SaveTreasureJiFenValue(client, newValue, true);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BaoZangJiFen, (long)addValue, (long)newValue, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TreasureJiFen, newValue);
				}
			}
		}

		// Token: 0x06002306 RID: 8966 RVA: 0x001DFAB0 File Offset: 0x001DDCB0
		public void ModifyTreasureXueZuanValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetTreasureXueZuan(client) + addValue;
				this.SaveTreasureXueZuanValue(client, newValue, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BaoZangXueZuan, (long)addValue, (long)newValue, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TreasureXueZuan, newValue);
				}
			}
		}

		// Token: 0x06002307 RID: 8967 RVA: 0x001DFB08 File Offset: 0x001DDD08
		public int GetTreasureJiFen(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "TreasureJiFen");
		}

		// Token: 0x06002308 RID: 8968 RVA: 0x001DFB25 File Offset: 0x001DDD25
		public void SaveTreasureJiFenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "TreasureJiFen", nValue, writeToDB);
		}

		// Token: 0x06002309 RID: 8969 RVA: 0x001DFB38 File Offset: 0x001DDD38
		public int GetTreasureXueZuan(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "TreasureXueZuan");
		}

		// Token: 0x0600230A RID: 8970 RVA: 0x001DFB55 File Offset: 0x001DDD55
		public void SaveTreasureXueZuanValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "TreasureXueZuan", nValue, writeToDB);
		}

		// Token: 0x0600230B RID: 8971 RVA: 0x001DFB68 File Offset: 0x001DDD68
		public void ModifyZhanHunValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetZhanHunValue(client) + addValue;
				this.SaveZhanHunValue(client, newValue, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZhanHun, (long)addValue, (long)newValue, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhanHun, newValue);
				}
			}
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x001DFBC4 File Offset: 0x001DDDC4
		public bool ModifyTianTiRongYaoValue(GameClient client, int addValue, string strFrom, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.TianTiData.RongYao += addValue;
				RoleAttributeValueData roleAttributeValueData = new RoleAttributeValueData
				{
					RoleAttribyteType = 0,
					Targetvalue = client.ClientData.TianTiData.RongYao,
					AddVAlue = addValue
				};
				Global.sendToDB<int, int[]>(10202, new int[]
				{
					client.ClientData.RoleID,
					client.ClientData.TianTiData.RongYao
				}, client.ServerId);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荣耀", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.TianTiData.RongYao, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TianTiRongYao, (long)addValue, (long)client.ClientData.TianTiData.RongYao, strFrom);
				if (notifyClient)
				{
					client.sendCmd<RoleAttributeValueData>(968, roleAttributeValueData, false);
				}
			}
			return true;
		}

		// Token: 0x0600230D RID: 8973 RVA: 0x001DFCEE File Offset: 0x001DDEEE
		public void SaveZhanHunValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhanHun", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x001DFD04 File Offset: 0x001DDF04
		public int GetZhanHunValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhanHun", "2020-12-12 12:12:12");
		}

		// Token: 0x0600230F RID: 8975 RVA: 0x001DFD28 File Offset: 0x001DDF28
		public void ModifyRongYuValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetRongYuValue(client) + addValue;
				this.SaveRongYuValue(client, newValue, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RongYu, (long)addValue, (long)newValue, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.RongYu, newValue);
				}
			}
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x001DFD82 File Offset: 0x001DDF82
		public void SaveRongYuValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "RongYu", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x06002311 RID: 8977 RVA: 0x001DFD98 File Offset: 0x001DDF98
		public int GetRongYuValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "RongYu", "2020-12-12 12:12:12");
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x001DFDBC File Offset: 0x001DDFBC
		public void ModifyZhanHunLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetZhanHunLevelValue(client) + addValue;
				this.SaveZhanHunLevelValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhanHunLevel, newValue);
				}
				Global.ActiveZhanHunBuffer(client, true);
			}
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x001DFE0A File Offset: 0x001DE00A
		public void SaveZhanHunLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhanHunLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x001DFE20 File Offset: 0x001DE020
		public int GetZhanHunLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhanHunLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x001DFE44 File Offset: 0x001DE044
		public void ModifyRongYuLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetRongYuLevelValue(client) + addValue;
				this.SaveRongYuLevelValue(client, newValue, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.RongYuLevel, newValue);
				}
			}
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x001DFE8A File Offset: 0x001DE08A
		public void SaveRongYuLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "RongYuLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x06002317 RID: 8983 RVA: 0x001DFEA0 File Offset: 0x001DE0A0
		public int GetRongYuLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "RongYuLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x06002318 RID: 8984 RVA: 0x001DFEC4 File Offset: 0x001DE0C4
		public void ModifyShengWangValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetShengWangValue(client) + addValue;
				this.SaveShengWangValue(client, newValue, true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWang, newValue);
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShengWang, (long)addValue, (long)newValue, strFrom);
				if (addValue > 0)
				{
					client._IconStateMgr.CheckJingJiChangJunXian(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		// Token: 0x06002319 RID: 8985 RVA: 0x001DFF88 File Offset: 0x001DE188
		public void ModifyLangHunFenMoValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetLangHunFenMoValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "LangHunFenMo", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.LangHunFenMo, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "狼魂粉末", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.LangHunFenMo, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x0600231A RID: 8986 RVA: 0x001E004C File Offset: 0x001DE24C
		public int GetLangHunFenMoValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "LangHunFenMo");
		}

		// Token: 0x0600231B RID: 8987 RVA: 0x001E006C File Offset: 0x001DE26C
		public bool ModifyOrnamentCharmPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetOrnamentCharmPointValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10153", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.OrnamentCharmPoint, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魅力点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.OrnamentCharmPoint, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600231C RID: 8988 RVA: 0x001E0170 File Offset: 0x001DE370
		public int GetOrnamentCharmPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10153");
		}

		// Token: 0x0600231D RID: 8989 RVA: 0x001E0190 File Offset: 0x001DE390
		public bool ModifyBHMatchGuessJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetBHMatchGuessJiFenValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10190", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.BHMatchGuessJiFen, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟联赛竞猜点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BHMatchGuessJiFen, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600231E RID: 8990 RVA: 0x001E0298 File Offset: 0x001DE498
		public int GetBHMatchGuessJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10190");
		}

		// Token: 0x0600231F RID: 8991 RVA: 0x001E02B8 File Offset: 0x001DE4B8
		public bool ModifyEraDonateValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetEraDonateValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10196", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.EraDonate, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "纪元贡献点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXingZhiChen, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002320 RID: 8992 RVA: 0x001E03C0 File Offset: 0x001DE5C0
		public int GetEraDonateValue(GameClient client)
		{
			int result;
			if (0 == client.ClientData.JunTuanId)
			{
				result = 0;
			}
			else
			{
				int CurEraID = JunTuanClient.getInstance().GetCurrentEraID();
				if (CurEraID <= 0)
				{
					result = 0;
				}
				else
				{
					int EraJunTuanID = Global.GetRoleParamsInt32FromDB(client, "10197");
					int MineEraID = Global.GetRoleParamsInt32FromDB(client, "10195");
					if (CurEraID != MineEraID || EraJunTuanID != client.ClientData.JunTuanId)
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "10197", client.ClientData.JunTuanId, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10195", CurEraID, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10196", 0, true);
					}
					result = Global.GetRoleParamsInt32FromDB(client, "10196");
				}
			}
			return result;
		}

		// Token: 0x06002321 RID: 8993 RVA: 0x001E0480 File Offset: 0x001DE680
		public int GetEraDonateValueOffline(int rid)
		{
			return Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(rid, "10196", 0));
		}

		// Token: 0x06002322 RID: 8994 RVA: 0x001E04A4 File Offset: 0x001DE6A4
		public long GetRebornExpMaxAddValue(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				int nowDayID = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				int expDayID = Global.GetRoleParamsInt32FromDB(client, "10243");
				if (expDayID != nowDayID)
				{
					Global.SaveRoleParamsInt64ValueToDB(client, "10244", 0L, true);
					Global.SaveRoleParamsInt64ValueToDB(client, "10245", 0L, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "10243", nowDayID, true);
				}
				long maxaddValue = 0L;
				if (types == MoneyTypes.RebornExpMonster)
				{
					maxaddValue = Global.GetRoleParamsInt64FromDB(client, "10244");
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					maxaddValue = Global.GetRoleParamsInt64FromDB(client, "10245");
				}
				if (expDayID != nowDayID)
				{
					MoneyTypes maxtypes = (types == MoneyTypes.RebornExpMonster) ? MoneyTypes.RebornExpMonsterMax : MoneyTypes.RebornExpSaleMax;
					this.NotifySelfPropertyValue(client, (int)types, RebornManager.getInstance().GetRebornExpMaxValueLeft(client, types));
					this.NotifySelfPropertyValue(client, (int)maxtypes, RebornManager.getInstance().GetRebornExpMaxValue(client, types));
				}
				result = maxaddValue;
			}
			return result;
		}

		// Token: 0x06002323 RID: 8995 RVA: 0x001E05B8 File Offset: 0x001DE7B8
		public bool ModifyRebornExpMaxAddValue(GameClient client, long addValue, string strFrom, MoneyTypes types, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = false;
			}
			else
			{
				long oldValue = this.GetRebornExpMaxAddValue(client, types);
				long targetValue = oldValue + addValue;
				if (isGM && (targetValue > 9223372036854775807L || targetValue < -9223372036854775808L))
				{
					result = false;
				}
				else
				{
					long newValue = (long)((int)Global.Clamp(targetValue, long.MinValue, long.MaxValue));
					if (oldValue != newValue)
					{
						if (types == MoneyTypes.RebornExpMonster)
						{
							Global.SaveRoleParamsInt64ValueToDB(client, "10244", newValue, writeToDB);
						}
						else if (types == MoneyTypes.RebornExpSale)
						{
							Global.SaveRoleParamsInt64ValueToDB(client, "10245", newValue, writeToDB);
						}
					}
					long maxleft = RebornManager.getInstance().GetRebornExpMaxValueLeft(client, types);
					client.ClientData.MoneyData[(int)types] = maxleft;
					MoneyTypes maxtypes = (types == MoneyTypes.RebornExpMonster) ? MoneyTypes.RebornExpMonsterMax : MoneyTypes.RebornExpSaleMax;
					long maxvalue = RebornManager.getInstance().GetRebornExpMaxValue(client, types);
					if (client.ClientData.MoneyData[(int)maxtypes] != maxvalue)
					{
						client.ClientData.MoneyData[(int)maxtypes] = maxvalue;
						if (notifyClient)
						{
							this.NotifySelfPropertyValue(client, (int)maxtypes, maxvalue);
						}
					}
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, (int)types, maxleft);
					}
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, types, addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002324 RID: 8996 RVA: 0x001E0770 File Offset: 0x001DE970
		public int GetCompType(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10203");
		}

		// Token: 0x06002325 RID: 8997 RVA: 0x001E078D File Offset: 0x001DE98D
		public void SetCompType(GameClient client, int compType)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "10203", compType, true);
		}

		// Token: 0x06002326 RID: 8998 RVA: 0x001E07A0 File Offset: 0x001DE9A0
		public int GetCompBattleJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10159");
		}

		// Token: 0x06002327 RID: 8999 RVA: 0x001E07C0 File Offset: 0x001DE9C0
		public bool ModifyCompBattleJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetCompBattleJiFenValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[143] = (long)newValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10159", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 143, (long)newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力战积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompBattleJiFen, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002328 RID: 9000 RVA: 0x001E08E4 File Offset: 0x001DEAE4
		public int GetCompMineJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10215");
		}

		// Token: 0x06002329 RID: 9001 RVA: 0x001E0904 File Offset: 0x001DEB04
		public bool ModifyCompMineJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetCompMineJiFenValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[145] = (long)newValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10215", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 145, (long)newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力矿洞积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompMineJiFen, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600232A RID: 9002 RVA: 0x001E0A28 File Offset: 0x001DEC28
		public int GetCompDonateValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10204");
		}

		// Token: 0x0600232B RID: 9003 RVA: 0x001E0A48 File Offset: 0x001DEC48
		public bool ModifyCompDonateValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)this.GetCompDonateValue(client);
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[138] = (long)newValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10204", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 138, (long)newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力争霸贡献度", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompDonate, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600232C RID: 9004 RVA: 0x001E0B6C File Offset: 0x001DED6C
		public void ModifyShenJiPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetShenJiPointValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10172", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiPoint, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiPoints, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x0600232D RID: 9005 RVA: 0x001E0C30 File Offset: 0x001DEE30
		public bool ModifyAlchemyElementValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!GlobalNew.IsGongNengOpened(client, GongNengIDs.Alchemy, false))
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)client.ClientData.AlchemyInfo.BaseData.Element;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					int newValue = (int)Global.Clamp(targetValue, -2147483648L, 2147483647L);
					client.ClientData.AlchemyInfo.BaseData.Element = newValue;
					if (writeToDB)
					{
						AlchemyManager.getInstance().UpdateAlchemyDataDB(client);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炼金元素值", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.AlchemyElement, (long)addValue, (long)newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600232E RID: 9006 RVA: 0x001E0D6C File Offset: 0x001DEF6C
		public void ModifyShenJiJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetShenJiJiFenValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10173", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiJiFen, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiJiFen, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x0600232F RID: 9007 RVA: 0x001E0E30 File Offset: 0x001DF030
		public void ModifyShenJiJiFenAddValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetShenJiJiFenAddValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10174", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiJiFenAdd, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹积分注入", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiJiFenAdd, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x06002330 RID: 9008 RVA: 0x001E0EF4 File Offset: 0x001DF0F4
		public int GetShenJiPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10172");
		}

		// Token: 0x06002331 RID: 9009 RVA: 0x001E0F14 File Offset: 0x001DF114
		public int GetShenJiJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10173");
		}

		// Token: 0x06002332 RID: 9010 RVA: 0x001E0F34 File Offset: 0x001DF134
		public int GetShenJiJiFenAddValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10174");
		}

		// Token: 0x06002333 RID: 9011 RVA: 0x001E0F54 File Offset: 0x001DF154
		public void ModifyKingOfBattlePointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetKingOfBattlePointValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "10150", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.KingOfBattlePoint, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "王者争霸点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.KingOfBattlePoint, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x06002334 RID: 9012 RVA: 0x001E1018 File Offset: 0x001DF218
		public int GetKingOfBattlePointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10150");
		}

		// Token: 0x06002335 RID: 9013 RVA: 0x001E1038 File Offset: 0x001DF238
		public bool ModifyMoBiValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.MoBi + addValue);
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.MoBi = (int)newValue;
					this.NotifySelfPropertyValue(client, 141, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10212", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔币", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MoBi, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002336 RID: 9014 RVA: 0x001E1108 File Offset: 0x001DF308
		public bool ModifyLuckStarValue(GameClient client, int addValue, string strFrom, bool isGM = false, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (SysType != DaiBiSySType.None && addValue < 0)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, Math.Abs(addValue), SysType, strFrom, true))
				{
					return true;
				}
			}
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.LuckStar + addValue);
				if (client == null || (newValue < 0L && !isGM))
				{
					result = false;
				}
				else
				{
					client.ClientData.LuckStar = (int)newValue;
					this.NotifySelfPropertyValue(client, 163, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10224", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "幸运之星", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.LuckStar, (long)addValue, newValue, strFrom);
					LogManager.WriteLog(LogTypes.Info, string.Format("[ljl_幸运之星]{0}", string.Format("幸运之星 strFrom={0},val={1},type={2},id={3},name={4}", new object[]
					{
						strFrom,
						addValue,
						SysType,
						client.ClientData.RoleID,
						client.ClientData.RoleName
					})), null, true);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002337 RID: 9015 RVA: 0x001E1270 File Offset: 0x001DF470
		public bool ModifyTeamRongYaoValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.TeamRongYao + addValue);
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.TeamRongYao = (int)newValue;
					this.NotifySelfPropertyValue(client, 160, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10218", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "组队荣耀", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TeamRongYao, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002338 RID: 9016 RVA: 0x001E1340 File Offset: 0x001DF540
		public bool ModifyTeamPointValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.TeamPoint + addValue);
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.TeamPoint = (int)newValue;
					this.NotifySelfPropertyValue(client, 162, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10223", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "竞技点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TeamPoint, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002339 RID: 9017 RVA: 0x001E1410 File Offset: 0x001DF610
		public bool ModifyMoBiValueOffline(int rid, string roleName, int zoneId, string userId, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = 0L;
				string old = Global.GetRoleParamsFromDBByRoleID(rid, "10212", 0);
				if (!string.IsNullOrEmpty(old) && long.TryParse(old, out newValue))
				{
					newValue += (long)addValue;
				}
				else
				{
					newValue = (long)addValue;
				}
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", rid, "10212", newValue), null, GameManager.ServerId);
					Global.UpdateRoleParamByNameOffline(rid, "10212", newValue.ToString(), GameManager.ServerId);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔币", strFrom, "系统", roleName, "修改", addValue, zoneId, userId, (int)newValue, GameManager.ServerId, null);
					EventLogManager.AddMoneyEvent(GameManager.ServerId, zoneId, userId, (long)rid, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MoBi, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600233A RID: 9018 RVA: 0x001E1518 File Offset: 0x001DF718
		public bool ModifyKuaFuLueDuoBuyNumAndDayID(GameClient client, int buyNum, int dayID, string strFrom)
		{
			bool result;
			if (client == null)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(buyNum * 100000000 + dayID);
				client.ClientData.KuaFuLueDuoEnterNumBuyNum = buyNum;
				client.ClientData.KuaFuLueDuoEnterNumDayID = dayID;
				this.NotifySelfPropertyValue(client, 135, (long)buyNum);
				Global.SaveRoleParamsInt64ValueToDB(client, "10201", newValue, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "跨服掠夺进入次数", strFrom, "系统", client.ClientData.RoleName, "更新", 0, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.Trace, OpTags.Trace, MoneyTypes.KuaFuLueDuoEnterNumBuyNum, 0L, (long)buyNum, strFrom);
				result = true;
			}
			return result;
		}

		// Token: 0x0600233B RID: 9019 RVA: 0x001E15D8 File Offset: 0x001DF7D8
		public bool ModifyKuaFuLueDuoEnterNum(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.KuaFuLueDuoEnterNum + addValue);
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.KuaFuLueDuoEnterNum = (int)newValue;
					this.NotifySelfPropertyValue(client, 134, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10200", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "跨服掠夺进入次数 ", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.KuaFuLueDuoEnterNum, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600233C RID: 9020 RVA: 0x001E16A8 File Offset: 0x001DF8A8
		public bool ModifyJueXingValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long newValue = (long)(client.ClientData.JueXingPoint + addValue);
				if (newValue < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.JueXingPoint = (int)newValue;
					this.NotifySelfPropertyValue(client, 132, newValue);
					Global.SaveRoleParamsInt64ValueToDB(client, "10198", newValue, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "觉醒点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXing, (long)addValue, newValue, strFrom);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600233D RID: 9021 RVA: 0x001E1778 File Offset: 0x001DF978
		public bool ModifyJueXingZhiChenValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = client.ClientData.JueXingZhiChen;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.JueXingZhiChen = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "觉醒之尘", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.JueXingZhiChen, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10194", (int)client.ClientData.JueXingZhiChen, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXingZhiChen, (long)addValue, client.ClientData.JueXingZhiChen, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 133, client.ClientData.JueXingZhiChen);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600233E RID: 9022 RVA: 0x001E18A0 File Offset: 0x001DFAA0
		public bool ModifyYuanSuJueXingShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = client.ClientData.YuanSuJueXingShi;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.YuanSuJueXingShi = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "元素觉醒石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.YuanSuJueXingShi, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10214", (int)client.ClientData.YuanSuJueXingShi, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.YuanSuJueXingShi, (long)addValue, client.ClientData.YuanSuJueXingShi, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 144, client.ClientData.YuanSuJueXingShi);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600233F RID: 9023 RVA: 0x001E19C8 File Offset: 0x001DFBC8
		public bool ModifyHunJingValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = client.ClientData.HunJing;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.HunJing = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魂晶", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.HunJing, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10208", (int)client.ClientData.HunJing, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.HunJing, (long)addValue, client.ClientData.HunJing, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 139, client.ClientData.HunJing);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002340 RID: 9024 RVA: 0x001E1AF0 File Offset: 0x001DFCF0
		public bool ModifyMountPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = client.ClientData.MountPoint;
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.MountPoint = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "坐骑积分", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.MountPoint, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10209", (int)client.ClientData.MountPoint, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MountPoint, (long)addValue, client.ClientData.MountPoint, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 140, client.ClientData.MountPoint);
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06002341 RID: 9025 RVA: 0x001E1C18 File Offset: 0x001DFE18
		public void ModifyZhengBaPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long lNewValue = (long)this.GetZhengBaPointValue(client) + (long)addValue;
					lNewValue = Math.Min(lNewValue, 2147483647L);
					int newValue = (int)lNewValue;
					Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaPoint", newValue, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhengBaPoint, newValue);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "争霸点", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZhengBaPoint, (long)addValue, (long)newValue, strFrom);
				}
			}
		}

		// Token: 0x06002342 RID: 9026 RVA: 0x001E1CDC File Offset: 0x001DFEDC
		public int GetZhengBaPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "ZhengBaPoint");
		}

		// Token: 0x06002343 RID: 9027 RVA: 0x001E1CF9 File Offset: 0x001DFEF9
		public void SaveWanMoTaPassLayerValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WanMoTaCurrLayerOrder", nValue, true, "2020-12-12 12:12:12");
		}

		// Token: 0x06002344 RID: 9028 RVA: 0x001E1D10 File Offset: 0x001DFF10
		public int GetWanMoTaPassLayerValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WanMoTaCurrLayerOrder", "2020-12-12 12:12:12");
		}

		// Token: 0x06002345 RID: 9029 RVA: 0x001E1D32 File Offset: 0x001DFF32
		public void SaveShengWangValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWang", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x06002346 RID: 9030 RVA: 0x001E1D48 File Offset: 0x001DFF48
		public int GetShengWangValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShengWang", "2020-12-12 12:12:12");
		}

		// Token: 0x06002347 RID: 9031 RVA: 0x001E1D6C File Offset: 0x001DFF6C
		public void ModifyShengWangLevelValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int newValue = this.GetShengWangLevelValue(client) + addValue;
				this.SaveShengWangLevelValue(client, newValue, writeToDB);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望等级", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, newValue, client.ServerId, null);
				ChengJiuManager.OnRoleJunXianChengJiu(client);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWangLevel, newValue);
				}
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JunXianLevel));
			}
		}

		// Token: 0x06002348 RID: 9032 RVA: 0x001E1E14 File Offset: 0x001E0014
		public void SaveShengWangLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWangLevel", nValue, writeToDB, "2020-12-12 12:12:12");
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMilitaryRank) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		// Token: 0x06002349 RID: 9033 RVA: 0x001E1E98 File Offset: 0x001E0098
		public int GetShengWangLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShengWangLevel", "2020-12-12 12:12:12");
		}

		// Token: 0x0600234A RID: 9034 RVA: 0x001E1EBC File Offset: 0x001E00BC
		public void NotifySelfParamsValueChange(GameClient client, RoleCommonUseIntParamsIndexs index, int value)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, (int)index, value);
			this.SendToClient(client, strcmd, 427);
		}

		// Token: 0x0600234B RID: 9035 RVA: 0x001E1F00 File Offset: 0x001E0100
		public void ModifyLiXianBaiTanTicksValue(GameClient client, int addValue, bool writeToDB = false)
		{
			if (0 != addValue)
			{
				int newValue = this.GetLiXianBaiTanTicksValue(client) + addValue;
				this.SaveLiXianBaiTanTicksValue(client, newValue, writeToDB);
			}
		}

		// Token: 0x0600234C RID: 9036 RVA: 0x001E1F30 File Offset: 0x001E0130
		public void SaveLiXianBaiTanTicksValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "LiXianBaiTanTicks", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		// Token: 0x0600234D RID: 9037 RVA: 0x001E1F48 File Offset: 0x001E0148
		public int GetLiXianBaiTanTicksValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "LiXianBaiTanTicks", "2020-12-12 12:12:12");
		}

		// Token: 0x0600234E RID: 9038 RVA: 0x001E1F6C File Offset: 0x001E016C
		public void SendGameEffect(GameClient client, string effectName, int lifeTicks, GameEffectAlignModes alignMode = GameEffectAlignModes.None, string mp3Name = "")
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				effectName,
				lifeTicks,
				(int)alignMode,
				mp3Name
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 437);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600234F RID: 9039 RVA: 0x001E1FDC File Offset: 0x001E01DC
		public void BroadCastGameEffect(int mapCode, int copyMapID, string effectName, int lifeTicks, GameEffectAlignModes alignMode = GameEffectAlignModes.None, string mp3Name = "")
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				effectName,
				lifeTicks,
				(int)alignMode,
				mp3Name
			});
			List<object> objsList = this.GetMapClients(mapCode);
			if (null != objsList)
			{
				objsList = Global.ConvertObjsList(mapCode, copyMapID, objsList, false);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 437);
			}
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x001E2060 File Offset: 0x001E0260
		public void BroadcastJieriChengHao(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.JieriChengHao);
				this.SendToClients(sl, pool, null, objsList, strcmd, 477);
			}
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x001E20C0 File Offset: 0x001E02C0
		public void NotifyZaJinDanKAwardDailyData(GameClient client)
		{
			int jiFen = Global.GetZaJinDanJifen(client);
			int jiFenBits = Global.GetZaJinDanJiFenBits(client);
			string strcmd = string.Format("{0}:{1}", jiFen, jiFenBits);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 499);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002352 RID: 9042 RVA: 0x001E2128 File Offset: 0x001E0328
		public void NotifySpriteMeditate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int meditate)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			if (null != objsList)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, meditate);
				this.SendToClients(sl, pool, null, objsList, strcmd, 600);
			}
		}

		// Token: 0x06002353 RID: 9043 RVA: 0x001E2180 File Offset: 0x001E0380
		public void NotifyGetMeditateAward(GameClient client)
		{
			int msecs = Global.GetRoleParamsInt32FromDB(client, "MeditateTime") / 1000;
			int msecs2 = 0;
			int totalSecs = msecs + msecs2;
			if (totalSecs >= Data.NotifyLiXianAwardMin * 60 || totalSecs * 1000 >= 43200000)
			{
				this.NotifyImportantMsgWithGoods(client, MsgWithGoodsType.NeedQueryMeditateInfo, ShowGameInfoTypes.OnlyChatBox, null, "", null);
			}
		}

		// Token: 0x06002354 RID: 9044 RVA: 0x001E21DC File Offset: 0x001E03DC
		public void NotifyMeditateTime(GameClient client)
		{
			int msecs = Global.GetRoleParamsInt32FromDB(client, "MeditateTime") / 1000;
			int msecs2 = 0;
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, msecs, msecs2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 550);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002355 RID: 9045 RVA: 0x001E225C File Offset: 0x001E045C
		public void NotifyPlayBossAnimation(GameClient client, int monsterID, int mapCode, int toX, int toY, int effectX, int effectY)
		{
			long ticks = TimeUtil.NOW();
			int checkCode = Global.GetBossAnimationCheckCode(monsterID, mapCode, toX, toY, effectX, effectY, ticks);
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				monsterID,
				mapCode,
				toX,
				toY,
				effectX,
				effectY,
				ticks,
				checkCode
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 639);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x06002356 RID: 9046 RVA: 0x001E2334 File Offset: 0x001E0534
		public void NotifySpriteExtensionPropsHited(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int enemy, int enemyX, int enemyY, int extensionPropID)
		{
			List<object> objsList = Global.GetAll9Clients(attacker);
			if (null != objsList)
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteExtensionPropsHitedData>(new SpriteExtensionPropsHitedData
				{
					roleId = attacker.GetObjectID(),
					enemy = enemy,
					enemyX = enemyX,
					enemyY = enemyY,
					ExtensionPropID = extensionPropID
				}), 644);
			}
		}

		// Token: 0x06002357 RID: 9047 RVA: 0x001E2544 File Offset: 0x001E0744
		public IEnumerable<GameClient> GetAllClients(bool includeKuaFu = true)
		{
			int index = 0;
			GameClient client = null;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !client.ClientSocket.IsKuaFuLogin)
					{
						yield return client;
					}
				}
			}
			yield break;
		}

		// Token: 0x06002358 RID: 9048 RVA: 0x001E256C File Offset: 0x001E076C
		public void BroadcastServerCmd(int cmdId, string data, bool includeKuaFu = false)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !client.ClientSocket.IsKuaFuLogin)
					{
						client.sendCmd(cmdId, data, false);
					}
				}
			}
		}

		// Token: 0x06002359 RID: 9049 RVA: 0x001E25D4 File Offset: 0x001E07D4
		public void BroadcastServerCmd<T>(int cmdId, T data, bool includeKuaFu = false)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !client.ClientSocket.IsKuaFuLogin)
					{
						client.sendCmd<T>(cmdId, data, false);
					}
				}
			}
		}

		// Token: 0x0600235A RID: 9050 RVA: 0x001E263C File Offset: 0x001E083C
		public void BroadcastOthersCmdData<T>(GameClient client, int cmdId, T data, bool includeKuaFu = true)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			foreach (object obj in objsList)
			{
				GameClient c = obj as GameClient;
				if (c != null && c.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !c.ClientSocket.IsKuaFuLogin)
					{
						c.sendCmd<T>(cmdId, data, false);
					}
				}
			}
		}

		// Token: 0x0600235B RID: 9051 RVA: 0x001E26E4 File Offset: 0x001E08E4
		public void BroadcastOthersCmdData(GameClient client, int cmdId, string data, bool includeKuaFu = true)
		{
			List<object> objsList = Global.GetAll9Clients(client);
			foreach (object obj in objsList)
			{
				GameClient c = obj as GameClient;
				if (c != null && c.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !c.ClientSocket.IsKuaFuLogin)
					{
						c.sendCmd(cmdId, data, false);
					}
				}
			}
		}

		// Token: 0x0600235C RID: 9052 RVA: 0x001E278C File Offset: 0x001E098C
		public void BroadSpecialHintText(int mapCode, int copyMapID, string text)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				List<object> objsList2 = new List<object>();
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CopyMapID == copyMapID)
						{
							objsList2.Add(c);
						}
					}
				}
				text = text.Replace(":", " ");
				string strcmd = string.Format("{0}", text);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList2, strcmd, 666);
			}
		}

		// Token: 0x0600235D RID: 9053 RVA: 0x001E2860 File Offset: 0x001E0A60
		public void BroadSpecialMapAIEvent(int mapCode, int copyMapID, int guangMuID, int show)
		{
			string strcmd = string.Format("{0}:{1}", guangMuID, show);
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CopyMapID == copyMapID)
						{
							c.sendCmd(667, strcmd, false);
						}
					}
				}
			}
		}

		// Token: 0x0600235E RID: 9054 RVA: 0x001E2904 File Offset: 0x001E0B04
		public void BroadSpecialMapMessage(int cmdID, string strcmd, int mapCode, int copyMapID)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CopyMapID == copyMapID)
						{
							c.sendCmd(cmdID, strcmd, false);
						}
					}
				}
			}
		}

		// Token: 0x0600235F RID: 9055 RVA: 0x001E2984 File Offset: 0x001E0B84
		public void BroadSpecialMapMessage(TCPOutPacket tcpOutPacket, int mapCode, int copyMapID, bool pushBack = true)
		{
			List<object> objsList = GameManager.ClientMgr.GetMapClients(mapCode);
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i] as GameClient;
					if (c != null)
					{
						if (c.ClientData.CopyMapID == copyMapID)
						{
							c.sendCmd(tcpOutPacket, false);
						}
					}
				}
				if (pushBack)
				{
					Global.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		// Token: 0x06002360 RID: 9056 RVA: 0x001E2A14 File Offset: 0x001E0C14
		public void BroadSpecialCopyMapMessageStr(int cmdID, string strcmd, CopyMap copyMap, bool insertRoleID = false)
		{
			List<GameClient> objsList = copyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null)
					{
						if (c.ClientData.CopyMapID == copyMap.CopyMapID)
						{
							if (insertRoleID)
							{
								c.sendCmd(cmdID, strcmd.Insert(0, string.Format("{0}:", c.ClientData.RoleID)), false);
							}
							else
							{
								c.sendCmd(cmdID, strcmd, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002361 RID: 9057 RVA: 0x001E2AD0 File Offset: 0x001E0CD0
		public void BroadSpecialCopyMapMessage<T>(int cmdID, T data, CopyMap copyMap)
		{
			List<GameClient> objsList = copyMap.GetClientsList();
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null && c.ClientData.CopyMapID == copyMap.CopyMapID)
					{
						c.sendCmd<T>(cmdID, data, false);
					}
				}
			}
		}

		// Token: 0x06002362 RID: 9058 RVA: 0x001E2B48 File Offset: 0x001E0D48
		public void BroadSpecialCopyMapMessage(int cmdID, string strcmd, List<GameClient> objsList, bool insertRoleID = false)
		{
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient c = objsList[i];
					if (c != null)
					{
						if (insertRoleID)
						{
							c.sendCmd(cmdID, strcmd.Insert(0, string.Format("{0}:", c.ClientData.RoleID)), false);
						}
						else
						{
							c.sendCmd(cmdID, strcmd, false);
						}
					}
				}
			}
		}

		// Token: 0x06002363 RID: 9059 RVA: 0x001E2BDC File Offset: 0x001E0DDC
		public void BroadZhanDuiMessage<T>(int cmdID, T data, int zhanDuiID)
		{
			if (zhanDuiID != 0)
			{
				foreach (GameClient client in this.GetAllClients(false))
				{
					if (client != null && client.ClientData.ZhanDuiID == zhanDuiID)
					{
						client.sendCmd<T>(cmdID, data, false);
					}
				}
			}
		}

		// Token: 0x06002364 RID: 9060 RVA: 0x001E2C68 File Offset: 0x001E0E68
		public void BroadZhanDuiMessage(int cmdID, string strcmd, int zhanDuiID)
		{
			if (zhanDuiID != 0)
			{
				foreach (GameClient client in this.GetAllClients(false))
				{
					if (client != null && client.ClientData.ZhanDuiID == zhanDuiID)
					{
						client.sendCmd(cmdID, strcmd, false);
					}
				}
			}
		}

		// Token: 0x06002365 RID: 9061 RVA: 0x001E2CF4 File Offset: 0x001E0EF4
		public void BroadSpecialCopyMapHintMsg(CopyMap copymap, string msg)
		{
			try
			{
				msg = msg.Replace(":", "``");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					4,
					1,
					msg,
					0
				});
				this.BroadSpecialCopyMapMessageStr(194, strcmd, copymap, false);
			}
			catch
			{
			}
		}

		// Token: 0x06002366 RID: 9062 RVA: 0x001E2D70 File Offset: 0x001E0F70
		public void BroadSpecialCopyMapMsg(CopyMap copymap, string msg, ShowGameInfoTypes showGameInfoType = ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes infoType = GameInfoTypeIndexes.Hot, int error = 0)
		{
			try
			{
				msg = msg.Replace(":", "``");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)showGameInfoType,
					(int)infoType,
					msg,
					error
				});
				this.BroadSpecialCopyMapMessageStr(194, strcmd, copymap, false);
			}
			catch
			{
			}
		}

		// Token: 0x06002367 RID: 9063 RVA: 0x001E2DF0 File Offset: 0x001E0FF0
		public void NotifyChangMap2NormalMap(GameClient client)
		{
			if (Global.CanChangeMap(client, client.ClientData.LastMapCode, client.ClientData.LastPosX, client.ClientData.LastPosY, true))
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.LastMapCode, client.ClientData.LastPosX, client.ClientData.LastPosY, -1, 0);
			}
			else
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GameManager.MainMapCode, -1, -1, -1, 0);
			}
		}

		// Token: 0x06002368 RID: 9064 RVA: 0x001E2EA0 File Offset: 0x001E10A0
		private void DoSpriteLifeMagic(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long ticks = TimeUtil.NOW();
			long subTicks = ticks - c.LastLifeMagicTick;
			if (subTicks >= 10000L)
			{
				c.LastLifeMagicTick = ticks;
				RoleRelifeLog relifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, "自然恢复补血补蓝");
				if (c.ClientData.CurrentLifeV > 0)
				{
					bool doRelife = false;
					if (c.ClientData.CurrentLifeV < c.ClientData.LifeV)
					{
						doRelife = true;
						relifeLog.hpModify = true;
						relifeLog.oldHp = c.ClientData.CurrentLifeV;
						double percent = RoleAlgorithm.GetLifeRecoverValPercentV(c);
						double lifeMax = percent * (double)c.ClientData.LifeV;
						lifeMax *= 1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(c) + DBRoleBufferManager.ProcessHuZhaoRecoverPercent(c) + RoleAlgorithm.GetLifeRecoverAddPercentOnlySandR(c);
						lifeMax += (double)c.ClientData.CurrentLifeV;
						c.ClientData.CurrentLifeV = (int)Global.GMin((double)c.ClientData.LifeV, lifeMax);
						relifeLog.newHp = c.ClientData.CurrentLifeV;
					}
					if (c.ClientData.CurrentMagicV < c.ClientData.MagicV)
					{
						doRelife = true;
						relifeLog.mpModify = true;
						relifeLog.oldMp = c.ClientData.CurrentMagicV;
						double percent = RoleAlgorithm.GetMagicRecoverValPercentV(c);
						double magicMax = percent * (double)c.ClientData.MagicV;
						magicMax *= 1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(c) + RoleAlgorithm.GetMagicRecoverAddPercentOnlySandR(c);
						magicMax += (double)c.ClientData.CurrentMagicV;
						c.ClientData.CurrentMagicV = (int)Global.GMin((double)c.ClientData.MagicV, magicMax);
						relifeLog.newMp = c.ClientData.CurrentMagicV;
					}
					if (doRelife)
					{
						List<object> listObjs = Global.GetAll9Clients(c);
						GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, listObjs, 0);
					}
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(relifeLog);
				}
			}
		}

		// Token: 0x06002369 RID: 9065 RVA: 0x001E312C File Offset: 0x001E132C
		private void DoSpriteHeart(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			c.ClientData.DayOnlineSecond = c.ClientData.BakDayOnlineSecond + (int)((TimeUtil.NOW() - c.ClientData.DayOnlineRecSecond) / 1000L);
			int check = Global.GetRoleParamsInt32FromDB(c, "10167");
			int ten = (check > 10) ? 10 : 0;
			check %= 10;
			if (check == 1)
			{
				if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(c.ClientData.RoleID, 0))
				{
					Global.SaveRoleParamsInt32ValueToDB(c, "10167", 2 + ten, true);
				}
			}
			else if (check == 2 && c.ClientData.DayOnlineSecond >= 2400)
			{
				if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(c.ClientData.RoleID, 3))
				{
					Global.SaveRoleParamsInt32ValueToDB(c, "10167", 3 + ten, true);
				}
			}
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x001E3214 File Offset: 0x001E1414
		private void DoSpriteDBHeart(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long ticks = TimeUtil.NOW();
			long subTicks = ticks - c.ClientData.LastDBHeartTicks;
			if (subTicks >= 10000L)
			{
				long remainder = 0L;
				Math.DivRem(subTicks, 1000L, out remainder);
				subTicks -= remainder;
				ticks -= remainder;
				c.ClientData.LastDBHeartTicks = ticks;
				this.UpdateRoleOnlineTimes(c, subTicks);
				c._IconStateMgr.CheckFreeZhuanPanChouState(c);
				c._IconStateMgr.SendIconStateToClient(c);
			}
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x001E3298 File Offset: 0x001E1498
		private void DoSpriteAutoFight(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long nowTicks = TimeUtil.NOW();
			c.AutoGetThingsOnAutoFight(nowTicks);
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x001E32B4 File Offset: 0x001E14B4
		private void DoSpriteSitExp(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long ticks = TimeUtil.NOW();
			long subTicks = ticks - c.ClientData.LastSiteExpTicks;
			if (subTicks >= 60000L)
			{
				c.ClientData.LastSiteExpTicks = ticks;
				bool safeRegion = false;
				if (c.ClientData.MapCode == GameManager.MainMapCode)
				{
					GameMap gameMap = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(c.ClientData.MapCode, out gameMap))
					{
						safeRegion = gameMap.InSafeRegionList(c.CurrentGrid);
					}
				}
				if (safeRegion)
				{
					double multiExpNum = 0.0;
					long zhuFuSecs = DBRoleBufferManager.ProcessErGuoTouGiveExperience(c, subTicks, out multiExpNum);
					if (zhuFuSecs > 0L)
					{
						RoleSitExpItem roleSitExpItem = null;
						if (c.ClientData.Level < Data.RoleSitExpList.Length)
						{
							roleSitExpItem = Data.RoleSitExpList[c.ClientData.Level];
						}
						if (null != roleSitExpItem)
						{
							int experience = roleSitExpItem.Experience;
							double dblExperience = 1.0;
							if (SpecailTimeManager.JugeIsDoulbeKaoHuo())
							{
								dblExperience += 1.0;
							}
							dblExperience += Global.ProcessTeamZhuFuExperience(c);
							dblExperience += multiExpNum;
							experience = (int)((double)experience * dblExperience);
							GameManager.ClientMgr.ProcessRoleExperience(c, (long)experience, true, false, true, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, StringUtil.substitute(GLang.GetLang(89, new object[0]), new object[]
							{
								experience,
								zhuFuSecs / 60L,
								zhuFuSecs % 60L
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
						}
					}
				}
			}
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x001E349C File Offset: 0x001E169C
		private void DoSpriteSubPKPoint(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long ticks = TimeUtil.NOW();
			if (c.ClientData.LastSiteSubPKPointTicks == 0L)
			{
				c.ClientData.LastSiteSubPKPointTicks = ticks;
			}
			else
			{
				long subTicks = ticks - c.ClientData.LastSiteSubPKPointTicks;
				if (subTicks >= 60000L)
				{
					c.ClientData.LastSiteSubPKPointTicks = ticks;
					if (c.ClientData.PKPoint > 0)
					{
						int oldPKPoint = c.ClientData.PKPoint;
						c.ClientData.PKPoint = Global.GMax(c.ClientData.PKPoint - 10, 0);
						c.ClientData.TmpPKPoint += 10;
						if (oldPKPoint != c.ClientData.PKPoint)
						{
							if (c.ClientData.TmpPKPoint >= 60)
							{
								this.SetRolePKValuePoint(sl, pool, c, c.ClientData.PKValue, c.ClientData.PKPoint, true);
								c.ClientData.TmpPKPoint = 0;
							}
							else
							{
								this.SetRolePKValuePoint(sl, pool, c, c.ClientData.PKValue, c.ClientData.PKPoint, false);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x001E35E0 File Offset: 0x001E17E0
		private void DoSpriteBuffers(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			DBRoleBufferManager.ProcessAutoGiveExperience(c);
			DBRoleBufferManager.RemoveUpLifeLimitStatus(c);
			DBRoleBufferManager.RemoveAttackBuffer(c);
			DBRoleBufferManager.RemoveDefenseBuffer(c);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddDefense);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddMDefense);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddMAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddDSAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.PKKingBuffer);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeShiDuNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddLifeNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddDefenseNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddMDefenseNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI1);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI2);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI3);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPStrength);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPIntelligsence);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPDexterity);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPConstitution);
			DBRoleBufferManager.ProcessTimeAddLifeMagic(c);
			DBRoleBufferManager.ProcessTimeAddLifeNoShow(c);
			DBRoleBufferManager.ProcessTimeAddMagicNoShow(c);
			DBRoleBufferManager.ProcessDSTimeAddLifeNoShow(c);
			DBRoleBufferManager.ProcessDSTimeSubLifeNoShow(c);
			DBRoleBufferManager.ProcessAllTimeSubLifeNoShow(c);
			AdvanceBufferPropsMgr.DoSpriteBuffers(c);
			if (GlobalNew.IsGongNengOpened(c, GongNengIDs.TarotCard, false))
			{
				TarotManager.getInstance().RemoveTarotKingData(c);
			}
			long ticks = TimeUtil.NOW();
			long subTicks = ticks - c.ClientData.LastProcessBufferTicks;
			if (subTicks >= 60000L)
			{
				c.ClientData.LastProcessBufferTicks = ticks;
				Global.RefreshJieriChengHao(c);
			}
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x001E373C File Offset: 0x001E193C
		private void DoSpriteMapLimitTimes(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long ticks = dateTime.Ticks / 10000L;
			long subTicks = ticks - c.ClientData.LastProcessMapLimitTimesTicks;
			if (c.ClientData.EventLastMapCode != c.ClientData.MapCode)
			{
				GlobalEventSource.getInstance().fireEvent(new OnClientMapChangedEventObject(c, c.ClientData.EventLastMapCode, c.ClientData.MapCode));
				c.ClientData.EventLastMapCode = c.ClientData.MapCode;
			}
			if (subTicks >= 60000L)
			{
				int elapsedSecs = (int)((ticks - c.ClientData.LastProcessMapLimitTimesTicks) / 1000L);
				c.ClientData.LastProcessMapLimitTimesTicks = ticks;
				if (!Global.CanMapInLimitTimes(c.ClientData.MapCode, dateTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(sl, pool, c, StringUtil.substitute(GLang.GetLang(90, new object[0]), new object[]
					{
						Global.GetMapName(c.ClientData.MapCode)
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, GameManager.MainMapCode, -1, -1, -1, 0);
				}
				Global.ProcessDayLimitSecsByClient(c, elapsedSecs);
			}
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x001E3894 File Offset: 0x001E1A94
		private static void DoSpriteMapTimeLimit(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			long elapseTicks = nowTicks - client.ClientData.LastMapLimitUpdateTicks;
			if (elapseTicks >= 3000L)
			{
				Global.ProcessMingJieMapTimeLimit(client, elapseTicks);
				Global.ProcessGuMuMapTimeLimit(client, elapseTicks);
				client.ClientData.LastMapLimitUpdateTicks = nowTicks;
			}
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x001E38E8 File Offset: 0x001E1AE8
		private static void DoSpriteHintToUpdateClient(GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			long elapseTicks = nowTicks - client.ClientData.LastHintToUpdateClientTicks;
			if (elapseTicks >= 60000L)
			{
				client.ClientData.LastHintToUpdateClientTicks = nowTicks;
				int forceHintAppVer = GameManager.GameConfigMgr.GetGameConfigItemInt("hint-appver", 0);
				if (client.MainExeVer > 0 && client.MainExeVer < forceHintAppVer)
				{
					string msgID = "1";
					int minutes = 1;
					int playNum = 1;
					string bulletinText = GLang.GetLang(91, new object[0]);
					BulletinMsgData bulletinMsgData = new BulletinMsgData
					{
						MsgID = msgID,
						PlayMinutes = minutes,
						ToPlayNum = playNum,
						BulletinText = bulletinText,
						BulletinTicks = TimeUtil.NOW(),
						MsgType = 0
					};
					GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
				}
				else if (client.ResVer > 0)
				{
					int forceHintResVer = GameManager.GameConfigMgr.GetGameConfigItemInt("hint-resver", 0);
					if (client.ResVer < forceHintResVer)
					{
						string msgID = "1";
						int minutes = 1;
						int playNum = 1;
						string bulletinText = GLang.GetLang(92, new object[0]);
						BulletinMsgData bulletinMsgData = new BulletinMsgData
						{
							MsgID = msgID,
							PlayMinutes = minutes,
							ToPlayNum = playNum,
							BulletinText = bulletinText,
							BulletinTicks = TimeUtil.NOW(),
							MsgType = 0
						};
						GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
					}
				}
			}
		}

		// Token: 0x06002372 RID: 9074 RVA: 0x001E3AA8 File Offset: 0x001E1CA8
		private static void DoSpriteGoodsTimeLimit(GameClient client)
		{
			bool isFashion = false;
			bool isGoods = false;
			long nowTicks = TimeUtil.NOW();
			long elapseTicks = nowTicks - client.ClientData.LastGoodsLimitUpdateTicks;
			long fashionTicks = nowTicks - client.ClientData.LastFashionLimitUpdateTicks;
			List<GoodsData> expiredList = null;
			if (fashionTicks >= 3000L)
			{
				expiredList = Global.GetFashionTimeExpired(client);
				isFashion = true;
			}
			if (elapseTicks >= 30000L)
			{
				List<GoodsData> goodsList = Global.GetGoodsTimeExpired(client);
				if (goodsList != null)
				{
					if (expiredList == null)
					{
						expiredList = goodsList;
					}
					else
					{
						expiredList.AddRange(Global.GetGoodsTimeExpired(client));
					}
				}
				isGoods = true;
			}
			if (expiredList != null && expiredList.Count > 0)
			{
				for (int i = 0; i < expiredList.Count; i++)
				{
					GoodsData goods = expiredList[i];
					if (Global.DestroyGoods(client, goods))
					{
						Global.SendMail(client, GLang.GetLang(93, new object[0]), string.Format(GLang.GetLang(94, new object[0]), Global.GetGoodsNameByID(goods.GoodsID)));
					}
				}
			}
			if (isGoods)
			{
				client.ClientData.LastGoodsLimitUpdateTicks = nowTicks;
			}
			if (isFashion)
			{
				client.ClientData.LastFashionLimitUpdateTicks = nowTicks;
			}
		}

		// Token: 0x06002373 RID: 9075 RVA: 0x001E3C00 File Offset: 0x001E1E00
		public static void DoSpriteMapGridMove(GameClient client, int slot = 0)
		{
			long ticks = TimeUtil.NOW();
			lock (client.Current9GridMutex)
			{
				long runTicks = client.LastRefresh9GridObjectsTicks[slot];
				if (ticks >= runTicks)
				{
					client.LastRefresh9GridObjectsTicks[slot] = ticks + client.CurrentSlotTicks;
					Global.GameClientMoveGrid(client);
				}
			}
		}

		// Token: 0x06002374 RID: 9076 RVA: 0x001E3C80 File Offset: 0x001E1E80
		private void DoSpriteMeditateTime(GameClient c)
		{
			long lCurrticks = TimeUtil.NOW();
			long lTicks = lCurrticks - c.ClientData.MeditateTicks;
			if (lTicks >= 10000L)
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(c.ClientData.MapCode);
				if (SceneUIClasses.ChongShengMap != sceneType)
				{
					if (c.ClientData.StartMeditate <= 0)
					{
						if (c.ClientData.LastMovePosTicks == 0L || c.ClientData.Last10sPosX != c.ClientData.PosX || c.ClientData.Last10sPosY != c.ClientData.PosY)
						{
							c.ClientData.Last10sPosX = c.ClientData.PosX;
							c.ClientData.Last10sPosY = c.ClientData.PosY;
							c.ClientData.LastMovePosTicks = lCurrticks;
						}
						else if (GlobalNew.IsGongNengOpened(c, GongNengIDs.MingXiang, false))
						{
							if (lCurrticks - c.ClientData.LastMovePosTicks > 120000L)
							{
								Global.StartMeditate(c);
								lTicks = 60000L;
							}
						}
					}
					if (lTicks >= 60000L)
					{
						c.ClientData.MeditateTicks = lCurrticks;
						if (c.ClientData.StartMeditate > 0)
						{
							Global.UpdateMeditateTime(c, lCurrticks);
							int totalTime = c.ClientData.MeditateTime;
							if (1 == c.ClientData.StartMeditate)
							{
								if (c.ClientData.GiveMeditateGoodsInterval == 0)
								{
									c.ClientData.GiveMeditateGoodsInterval = Global.GetMingXiangGoodsInterval(c);
								}
								if (GoodsUtil.GetMeditateBagGoodsCnt(c) < Data.OfflineRW_ItemLimit && (long)totalTime >= c.ClientData.GiveMeditateAwardOffsetTicks + (long)c.ClientData.GiveMeditateGoodsInterval)
								{
									int cnt = (int)(((long)totalTime - c.ClientData.GiveMeditateAwardOffsetTicks) / (long)c.ClientData.GiveMeditateGoodsInterval);
									for (int i = 0; i < cnt; i++)
									{
										if (GoodsUtil.GetMeditateBagGoodsCnt(c) >= Data.OfflineRW_ItemLimit)
										{
											break;
										}
										GoodsUtil.GiveOneMeditateGood(c);
									}
									if (cnt > 1)
									{
										LogManager.WriteLog(LogTypes.Fatal, string.Format("角色冥想背包本次添加物品个数为  {2},角色ID = {0} ，角色roleid = {1}, 个数异常。", c.strUserID, c.ClientData.RoleID, cnt), null, false);
									}
									c.ClientData.GiveMeditateAwardOffsetTicks = GoodsUtil.GetLastGiveMeditateTime(c);
									c.ClientData.GiveMeditateGoodsInterval = Global.GetMingXiangGoodsInterval(c);
								}
							}
							GameManager.ClientMgr.NotifyMeditateTime(c);
							if (c._IconStateMgr.CheckShenYouAwardIcon(c))
							{
								c._IconStateMgr.SendIconStateToClient(c);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002375 RID: 9077 RVA: 0x001E3F80 File Offset: 0x001E2180
		private void DoSpriteDeadTime(GameClient c)
		{
			long lCurrticks = TimeUtil.NOW();
			long lTicks = lCurrticks - c.ClientData.LastProcessDeadTicks;
			if (c.ClientData.CurrentLifeV <= 0 && lTicks >= 3000L)
			{
				c.ClientData.LastProcessDeadTicks = lCurrticks;
				this.ProcessSpriteDead(c, lCurrticks);
			}
		}

		// Token: 0x06002376 RID: 9078 RVA: 0x001E3FE0 File Offset: 0x001E21E0
		private void ProcessSpriteDead(GameClient client, long nowTicks)
		{
			int posX = -1;
			int posY = -1;
			if (2 == Global.GetRoleReliveType(client) || 3 == Global.GetRoleReliveType(client))
			{
				long elapseTicks = nowTicks - client.ClientData.LastRoleDeadTicks;
				if (elapseTicks / 1000L < (long)(Global.GetRoleReliveWaitingSecs(client) + 3000))
				{
					return;
				}
			}
			else if (4 == Global.GetRoleReliveType(client))
			{
				long elapseTicks = TimeUtil.NOW() - client.ClientData.LastRoleDeadTicks;
				if (elapseTicks / 1000L < (long)(Global.GetRoleReliveWaitingSecs(client) + 3000))
				{
					return;
				}
				posX = -1;
				posY = -1;
			}
			else if (0 == Global.GetRoleReliveType(client))
			{
				if (nowTicks - client.ClientData.LastRoleDeadTicks < 35000L)
				{
					return;
				}
			}
			else
			{
				if (1 != Global.GetRoleReliveType(client))
				{
					return;
				}
				if (nowTicks - client.ClientData.LastRoleDeadTicks < 5000L)
				{
					return;
				}
			}
			if (Global.IsHuangChengMapCode(client.ClientData.MapCode) || Global.IsHuangGongMapCode(client.ClientData.MapCode))
			{
				posX = -1;
				posY = -1;
			}
			if (Global.IsBattleMap(client))
			{
				int toMapCode = GameManager.BattleMgr.BattleMapCode;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					int defaultBirthPosX = gameMap.DefaultBirthPosX;
					int defaultBirthPosY = gameMap.DefaultBirthPosY;
					int defaultBirthRadius = gameMap.BirthRadius;
					Global.GetBattleMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref defaultBirthRadius);
					Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, toMapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
					posX = (int)newPos.X;
					posY = (int)newPos.Y;
					Global.ClientRealive(client, posX, posY, client.ClientData.RoleDirection);
				}
			}
			else if (Global.IsLingDiZhanMapCode(client))
			{
				int toMapCode = client.ClientData.MapCode;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					Point newPos = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, toMapCode);
					posX = (int)newPos.X;
					posY = (int)newPos.Y;
					Global.ClientRealive(client, posX, posY, client.ClientData.RoleDirection);
				}
			}
			else
			{
				if (GameManager.ArenaBattleMgr.IsInArenaBattle(client))
				{
					posX = -1;
					posY = -1;
				}
				if (posX == -1 || posY == -1)
				{
					int toMapCode = Global.GetMapRealiveInfoByCode(client.ClientData.MapCode);
					if (toMapCode <= -1)
					{
						toMapCode = GameManager.MainMapCode;
					}
					else if (toMapCode == 0 || GameManager.ArenaBattleMgr.IsInArenaBattle(client))
					{
						toMapCode = GameManager.MainMapCode;
					}
					else if (toMapCode == 1)
					{
						toMapCode = client.ClientData.MapCode;
					}
					if (toMapCode >= 0)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(toMapCode, out gameMap))
						{
							int defaultBirthPosX = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosX;
							int defaultBirthPosY = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosY;
							int defaultBirthRadius = GameManager.MapMgr.DictMaps[toMapCode].BirthRadius;
							Point newPos = Global.GetMapPoint(ObjectTypes.OT_CLIENT, toMapCode, defaultBirthPosX, defaultBirthPosY, defaultBirthRadius);
							posX = (int)newPos.X;
							posY = (int)newPos.Y;
							client.ClientData.CurrentLifeV = client.ClientData.LifeV;
							client.ClientData.CurrentMagicV = client.ClientData.MagicV;
							client.ClientData.MoveAndActionNum = 0;
							GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, client.ClientData.RoleDirection);
							if (toMapCode != client.ClientData.MapCode)
							{
								GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection);
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toMapCode, posX, posY, -1, 1);
							}
							else
							{
								Global.ClientRealive(client, posX, posY, client.ClientData.RoleDirection);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002377 RID: 9079 RVA: 0x001E4504 File Offset: 0x001E2704
		public void DoSpriteWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteHeart(sl, pool, client);
			this.DoSpriteAutoFight(sl, pool, client);
			this.DoSpriteSitExp(sl, pool, client);
			this.DoSpriteSubPKPoint(sl, pool, client);
			this.BroadcastRolePurpleName(sl, pool, client);
			Global.ProcessQueueCmds(client);
			this.ProcessRoleBattleNameInfoTimeOut(sl, pool, client);
			this.JugeTempHorseID(client);
			this.ChangeDayLoginNum(client);
			Global.ProcessClientHeart(client);
			this.DoSpriteMapLimitTimes(sl, pool, client);
			ClientManager.DoSpriteMapTimeLimit(client);
			ClientManager.DoSpriteHintToUpdateClient(client);
			ClientManager.DoSpriteGoodsTimeLimit(client);
			this.DoSpriteMeditateTime(client);
			client._IconStateMgr.DoSpriteIconTicks(client);
			GroupMailManager.CheckRoleGroupMail(client);
			RobotTaskValidator.getInstance().KickTimeout(client);
			GameManager.MerlinMagicBookMgr.DoMerlinSecretTime(client);
			SingletonTemplate<GetInterestingDataMgr>.Instance().Update(client);
			ClientCmdCheck.WriteLifeLogs(client);
		}

		// Token: 0x06002378 RID: 9080 RVA: 0x001E45D4 File Offset: 0x001E27D4
		public void DoSpriteBackgourndWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep <= 0 && !client.ClientData.FirstPlayStart)
				{
					this.DoSpriteWorks(sl, pool, client);
				}
			}
		}

		// Token: 0x06002379 RID: 9081 RVA: 0x001E4630 File Offset: 0x001E2830
		public void DoBuffersWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteLifeMagic(sl, pool, client);
			this.DoSpriteBuffers(sl, pool, client);
			this.CheckDSHideState(client);
			client.OneSecsTimerEventObject.Client = client;
			client.OneSecsTimerEventObject.NowTicks = TimeUtil.NOW();
			client.OneSecsTimerEventObject.DeltaTicks = client.OneSecsTimerEventObject.NowTicks - client.OneSecsTimerEventObject.LastRunTicks;
			if (client.OneSecsTimerEventObject.DeltaTicks < 60000L)
			{
				GlobalEventSource.getInstance().fireEvent(client.OneSecsTimerEventObject);
			}
			client.OneSecsTimerEventObject.LastRunTicks = client.OneSecsTimerEventObject.NowTicks;
		}

		// Token: 0x0600237A RID: 9082 RVA: 0x001E46DC File Offset: 0x001E28DC
		public void DoBuffersExtension(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			SpriteAttack.ExecMagicsManyTimeDmageQueueEx(client);
			client.buffManager.UpdateByTime(client, nowTicks);
			if (client.bufferPropsManager.TimerUpdateProps(nowTicks, false))
			{
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					DelayExecProcIds.NotifyRefreshProps
				});
			}
			client.TimedActionMgr.Run(nowTicks);
			client.delayExecModule.ExecDelayProcs(client);
			SpriteMagicHelper.ExecuteAllItems(client);
		}

		// Token: 0x0600237B RID: 9083 RVA: 0x001E4758 File Offset: 0x001E2958
		public void DoDBWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteDBHeart(sl, pool, client);
			Global.ProcessDBCmdByTicks(client, false);
			Global.ProcessDBSkillCmdByTicks(client, false);
			Global.ProcessDBRoleParamCmdByTicks(client, false);
			Global.ProcessDBEquipStrongCmdByTicks(client, false);
		}

		// Token: 0x0600237C RID: 9084 RVA: 0x001E4788 File Offset: 0x001E2988
		public void DoSpriteBuffersWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep <= 0)
				{
					this.DoBuffersWorks(sl, pool, client);
				}
			}
		}

		// Token: 0x0600237D RID: 9085 RVA: 0x001E47D8 File Offset: 0x001E29D8
		public void DoSpriteExtensionWork(SocketListener sl, TCPOutPacketPool pool, int nThead, int nMaxThread)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (nMaxThread > 0)
				{
					if (client.ClientData.RoleID % nMaxThread != nThead)
					{
						continue;
					}
				}
				if (client.ClientData.ClosingClientStep <= 0)
				{
					this.DoBuffersExtension(sl, pool, client);
				}
			}
		}

		// Token: 0x0600237E RID: 9086 RVA: 0x001E484C File Offset: 0x001E2A4C
		public void DoSpriteExtensionWorkByPerMap(int mapCode = -1, int subMapCode = -1)
		{
			SocketListener sl = Global._TCPManager.MySocketListener;
			TCPOutPacketPool tp = Global._TCPManager.TcpOutPacketPool;
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count != 0)
			{
				foreach (object obj in mapClients)
				{
					if (null != obj)
					{
						GameClient client = obj as GameClient;
						if (null != client)
						{
							if (subMapCode < 0 || client.ClientData.CopyMapID == subMapCode)
							{
								if (client.ClientData.ClosingClientStep <= 0)
								{
									this.DoBuffersExtension(sl, tp, client);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600237F RID: 9087 RVA: 0x001E4954 File Offset: 0x001E2B54
		public void DoSpriteDBWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep <= 0)
				{
					this.DoDBWorks(sl, pool, client);
				}
			}
		}

		// Token: 0x06002380 RID: 9088 RVA: 0x001E49A4 File Offset: 0x001E2BA4
		public void DoSpritesMapGridMove(int nThead)
		{
			if (GameManager.Update9GridUsingPosition != 2)
			{
				int index = 0;
				int count = 0;
				long minTicks = TimeUtil.NOW() - 3000L;
				List<int> vipLevelList = new List<int>();
				List<int> mainTaskList = new List<int>();
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					int vipLevel = client.ClientData.VipLevel;
					int mainTaskId = client.ClientData.MainTaskID;
					if (nThead == 0)
					{
						vipLevelList.Add(vipLevel);
						mainTaskList.Add(mainTaskId);
					}
					if (client.ClientData.ServerPosTicks >= minTicks)
					{
						if (client.ClientData.RoleID % Program.MaxGird9UpdateWorkersNum == nThead)
						{
							if (mainTaskId < this.MinMainTaskForDoSpriteMapGridMove && vipLevel < this.MinVipLevelForDoSpriteMapGridMove)
							{
								client.CurrentSlotTicks = 2000L;
							}
							else
							{
								client.CurrentSlotTicks = (long)GameManager.MaxSlotOnUpdate9GridsTicks;
								ClientManager.DoSpriteMapGridMove(client, 1);
								if (GameManager.MaxSleepOnDoMapGridMoveTicks > 0 && count++ % 5 == 0)
								{
									Thread.Sleep(GameManager.MaxSleepOnDoMapGridMoveTicks);
								}
							}
						}
					}
				}
				if (nThead == 0)
				{
					vipLevelList.Sort();
					int i = vipLevelList.Count - 1;
					int c = 0;
					while (i >= 0 && c < 160)
					{
						if (c == 159)
						{
							this.MinVipLevelForDoSpriteMapGridMove = vipLevelList[i];
						}
						else
						{
							this.MinVipLevelForDoSpriteMapGridMove = vipLevelList[i] + 1;
						}
						i--;
						c++;
					}
					mainTaskList.Sort();
					i = mainTaskList.Count - 1;
					c = 0;
					while (i >= 0 && c < 160)
					{
						if (c < 159)
						{
							this.MinMainTaskForDoSpriteMapGridMove = mainTaskList[i];
						}
						else
						{
							this.MinMainTaskForDoSpriteMapGridMove = mainTaskList[i] + 1;
						}
						i--;
						c++;
					}
				}
			}
		}

		// Token: 0x06002381 RID: 9089 RVA: 0x001E4BEC File Offset: 0x001E2DEC
		public void DoSpritesMapGridMoveNewMode(int nThead)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.RoleID % Program.MaxGird9UpdateWorkersNum == nThead)
				{
					Global.GameClientMoveGrid(client);
				}
			}
		}

		// Token: 0x06002382 RID: 9090 RVA: 0x001E4C3C File Offset: 0x001E2E3C
		public void NotifyBloodCastleMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 517)
				{
					strcmd = string.Format("{0}:{1}", mapCode, nTimer);
				}
				else if (nCmdID == 533)
				{
					strcmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				this.SendToClients(sl, pool, null, objsList, strcmd, nCmdID);
			}
		}

		// Token: 0x06002383 RID: 9091 RVA: 0x001E4CF8 File Offset: 0x001E2EF8
		public void NotifyBloodCastleCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0, GameClient client = null)
		{
			List<GameClient> objsList = mapInfo.GetClientsList();
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 517)
				{
					strcmd = string.Format("{0}:{1}", mapInfo.FubenMapID, nTimer);
				}
				else if (nCmdID == 533)
				{
					strcmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				else if (nCmdID == 531)
				{
					BloodCastleDataInfo bcDataTmp = null;
					if (!Data.BloodCastleDataInfoList.TryGetValue(mapInfo.FubenMapID, out bcDataTmp) || bcDataTmp == null)
					{
						return;
					}
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
					{
						mapInfo.FubenMapID,
						nTimer,
						bcDataTmp.NeedKillMonster1Num,
						1,
						bcDataTmp.NeedKillMonster2Num,
						1,
						1,
						1
					});
				}
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] != client)
					{
						this.SendToClient(sl, pool, objsList[i], strcmd, nCmdID);
					}
				}
				if (null != client)
				{
					this.SendToClient(sl, pool, client, strcmd, nCmdID);
				}
			}
		}

		// Token: 0x06002384 RID: 9092 RVA: 0x001E4ECC File Offset: 0x001E30CC
		public void NotifyBloodCastleCopySceneMsgEndFight(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, BloodCastleScene bcTmp, int nCmdID, int nTimer, int nTimeAward)
		{
			BloodCastleDataInfo bcDataTmp = null;
			if (Data.BloodCastleDataInfoList.TryGetValue(mapInfo.FubenMapID, out bcDataTmp))
			{
				if (bcTmp != null && bcDataTmp != null)
				{
					bcTmp.m_bEndFlag = true;
					List<GameClient> objsList = mapInfo.GetClientsList();
					if (null != objsList)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							GameClient client = objsList[i];
							if (client.ClientData.FuBenID <= 0 || GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
							{
								string AwardItem = null;
								string AwardItem2 = null;
								client.ClientData.BloodCastleAwardPoint += nTimeAward;
								Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastlePlayerPoint", client.ClientData.BloodCastleAwardPoint, true);
								if (client.ClientData.RoleID == bcTmp.m_nRoleID)
								{
									for (int j = 0; j < bcDataTmp.AwardItem1.Length; j++)
									{
										AwardItem += bcDataTmp.AwardItem1[j];
										if (j != bcDataTmp.AwardItem1.Length - 1)
										{
											AwardItem += "|";
										}
									}
								}
								for (int k = 0; k < bcDataTmp.AwardItem2.Length; k++)
								{
									AwardItem2 += bcDataTmp.AwardItem2[k];
									if (k != bcDataTmp.AwardItem2.Length - 1)
									{
										AwardItem2 += "|";
									}
								}
								int nFlag = 0;
								if (bcTmp.m_bIsFinishTask)
								{
									nFlag = 1;
								}
								Global.SaveRoleParamsInt32ValueToDB(client, "BloodCastleSceneFinishFlag", nFlag, true);
								string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									nTimer,
									nFlag,
									client.ClientData.BloodCastleAwardPoint,
									Global.CalcExpForRoleScore(client.ClientData.BloodCastleAwardPoint, bcDataTmp.ExpModulus),
									client.ClientData.BloodCastleAwardPoint * bcDataTmp.MoneyModulus,
									AwardItem,
									AwardItem2
								});
								GameManager.ClientMgr.SendToClient(client, strcmd, nCmdID);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002385 RID: 9093 RVA: 0x001E5150 File Offset: 0x001E3350
		public void NotifyDaimonSquareMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, int nSection, int nTimer, int nWave, int nNum, int nPlayerNum)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 537)
				{
					strcmd = string.Format("{0}:{1}", nWave, nNum);
				}
				else if (nCmdID == 536)
				{
					strcmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 546)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				this.SendToClients(sl, pool, null, objsList, strcmd, nCmdID);
			}
		}

		// Token: 0x06002386 RID: 9094 RVA: 0x001E5210 File Offset: 0x001E3410
		public void NotifyDaimonSquareCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0)
		{
			List<GameClient> objsList = mapInfo.GetClientsList();
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 546)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				for (int i = 0; i < objsList.Count; i++)
				{
					this.SendToClient(sl, pool, objsList[i], strcmd, nCmdID);
				}
			}
		}

		// Token: 0x06002387 RID: 9095 RVA: 0x001E5284 File Offset: 0x001E3484
		public void NotifyDaimonSquareCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nSection, int nTimer, int nWave, int nNum, int nPlayerNum)
		{
			List<GameClient> objsList = mapInfo.GetClientsList();
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 537)
				{
					strcmd = string.Format("{0}:{1}", nWave, nNum);
				}
				else if (nCmdID == 536)
				{
					strcmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 546)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				for (int i = 0; i < objsList.Count; i++)
				{
					this.SendToClient(sl, pool, objsList[i], strcmd, nCmdID);
				}
			}
		}

		// Token: 0x06002388 RID: 9096 RVA: 0x001E5358 File Offset: 0x001E3558
		public void NotifyDaimonSquareCopySceneMsgEndFight(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, DaimonSquareScene dsInfo, int nCmdID, int nTimeAward)
		{
			DaimonSquareDataInfo bcDataTmp = null;
			if (Data.DaimonSquareDataInfoList.TryGetValue(mapInfo.FubenMapID, out bcDataTmp))
			{
				if (dsInfo != null && bcDataTmp != null)
				{
					dsInfo.m_bEndFlag = true;
					List<GameClient> objsList = mapInfo.GetClientsList();
					if (null != objsList)
					{
						for (int i = 0; i < objsList.Count; i++)
						{
							if (objsList[i] != null)
							{
								GameClient client = objsList[i];
								if (client.ClientData.FuBenID <= 0 || GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
								{
									string sAwardItem = null;
									client.ClientData.DaimonSquarePoint += nTimeAward;
									Global.SaveRoleParamsInt32ValueToDB(client, "DaimonSquarePlayerPoint", client.ClientData.DaimonSquarePoint, true);
									for (int j = 0; j < bcDataTmp.AwardItem.Length; j++)
									{
										sAwardItem += bcDataTmp.AwardItem[j];
										if (j != bcDataTmp.AwardItem.Length - 1)
										{
											sAwardItem += "|";
										}
									}
									int nFlag = 0;
									if (dsInfo.m_bIsFinishTask)
									{
										nFlag = 1;
									}
									string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										nFlag,
										client.ClientData.DaimonSquarePoint,
										Global.CalcExpForRoleScore(client.ClientData.DaimonSquarePoint, bcDataTmp.ExpModulus),
										client.ClientData.DaimonSquarePoint * bcDataTmp.MoneyModulus,
										sAwardItem
									});
									GameManager.ClientMgr.SendToClient(client, strcmd, nCmdID);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06002389 RID: 9097 RVA: 0x001E5560 File Offset: 0x001E3760
		public void NotifyAngelTempleMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, AngelTemplePointInfo[] array, int nSection, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0, double nBossHP = 0.0)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				string strcmd = "";
				if (nCmdID == 570)
				{
					strcmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 533)
				{
					strcmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strcmd = string.Format("{0}", nPlayerNum);
				}
				else if (nCmdID == 572)
				{
					double dValue = Math.Round((double)array[0].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string sName = array[0].m_RoleName;
					double dValue2 = Math.Round((double)array[1].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string sName2 = array[1].m_RoleName;
					double dValue3 = Math.Round((double)array[2].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string sName3 = array[2].m_RoleName;
					double dValue4 = Math.Round((double)array[3].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string sName4 = array[3].m_RoleName;
					double dValue5 = Math.Round((double)array[4].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string sName5 = array[4].m_RoleName;
					strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
					{
						Math.Round(nBossHP / (double)GameManager.AngelTempleMgr.m_BossHP, 2),
						sName,
						dValue,
						sName2,
						dValue2,
						sName3,
						dValue3,
						sName4,
						dValue4,
						sName5,
						dValue5
					});
				}
				this.SendToClients(sl, pool, null, objsList, strcmd, nCmdID);
			}
		}

		// Token: 0x0600238A RID: 9098 RVA: 0x001E57F4 File Offset: 0x001E39F4
		public void NotifyAngelTempleMsgBossDisappear(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objsList = this.Container.GetObjectsByMap(mapCode);
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						GameClient client = objsList[i] as GameClient;
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(95, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		// Token: 0x0600238B RID: 9099 RVA: 0x001E5890 File Offset: 0x001E3A90
		public void NotifyTeamMemberMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, TeamData td, TeamCmds nCmd)
		{
			if (null != td)
			{
				lock (td)
				{
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient gameClient = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != gameClient)
						{
							if (nCmd == TeamCmds.Quit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(96, new object[0]), new object[]
								{
									client.ClientData.RoleName
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
							}
							else if (nCmd == TeamCmds.AgreeApply)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(97, new object[0]), new object[]
								{
									client.ClientData.RoleName
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600238C RID: 9100 RVA: 0x001E59F4 File Offset: 0x001E3BF4
		public List<object> GetPlayerByMap(GameClient client)
		{
			return this.Container.GetObjectsByMap(client.ClientData.MapCode);
		}

		// Token: 0x0600238D RID: 9101 RVA: 0x001E5A20 File Offset: 0x001E3C20
		public void NotifySelfUserStoreYinLiangChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StoreYinLiang);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 763);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600238E RID: 9102 RVA: 0x001E5A7C File Offset: 0x001E3C7C
		public void NotifySelfUserStoreMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StoreMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 764);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		// Token: 0x0600238F RID: 9103 RVA: 0x001E5AD8 File Offset: 0x001E3CD8
		public bool AddUserStoreYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, long addYinLiang, string strFrom, bool isGM = false)
		{
			bool result;
			if (0L == addYinLiang)
			{
				result = true;
			}
			else
			{
				long oldYinLiang = client.ClientData.StoreYinLiang;
				lock (client.ClientData.StoreYinLiangMutex)
				{
					if (addYinLiang < 0L && !isGM && oldYinLiang < Math.Abs(addYinLiang))
					{
						return false;
					}
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, addYinLiang, isGM ? 1 : 0);
					string[] dbFields = Global.ExecuteDBCmd(10173, strcmd, client.ServerId);
					if (null == dbFields)
					{
						return false;
					}
					if (dbFields.Length != 2)
					{
						return false;
					}
					if (Convert.ToInt64(dbFields[0]) < 0L)
					{
						return false;
					}
					client.ClientData.StoreYinLiang = Convert.ToInt64(dbFields[1]);
					GameManager.ClientMgr.NotifySelfUserStoreYinLiangChange(sl, pool, client);
					if (0L != addYinLiang)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "仓库金币", strFrom, "系统", client.ClientData.RoleName, "增加", (int)addYinLiang, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.StoreYinLiang, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.StoreYinLiang, addYinLiang, client.ClientData.StoreYinLiang, strFrom);
					}
				}
				Global.AddRoleStoreYinLiangEvent(client, oldYinLiang);
				result = true;
			}
			return result;
		}

		// Token: 0x06002390 RID: 9104 RVA: 0x001E5CC4 File Offset: 0x001E3EC4
		public bool AddUserStoreMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, long addMoney, string strFrom, bool isGM = false)
		{
			bool result;
			if (0L == addMoney)
			{
				result = true;
			}
			else
			{
				long oldMoney = client.ClientData.StoreMoney;
				lock (client.ClientData.StoreMoneyMutex)
				{
					if (addMoney < 0L && !isGM && oldMoney < Math.Abs(addMoney))
					{
						return false;
					}
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, addMoney, isGM ? 1 : 0);
					string[] dbFields = Global.ExecuteDBCmd(10174, strcmd, client.ServerId);
					if (null == dbFields)
					{
						return false;
					}
					if (dbFields.Length != 2)
					{
						return false;
					}
					if (Convert.ToInt64(dbFields[0]) < 0L)
					{
						return false;
					}
					client.ClientData.StoreMoney = Convert.ToInt64(dbFields[1]);
					if (0L != addMoney)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "仓库绑定金币", strFrom, "系统", client.ClientData.RoleName, "增加", (int)addMoney, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.StoreMoney, client.ServerId, null);
					}
				}
				GameManager.ClientMgr.NotifySelfUserStoreMoneyChange(sl, pool, client);
				Global.AddRoleStoreMoneyEvent(client, oldMoney);
				result = true;
			}
			return result;
		}

		// Token: 0x06002391 RID: 9105 RVA: 0x001E5E94 File Offset: 0x001E4094
		public void NotifyAllActivityState(int type, int state, string activityTimeBegin = "", string activityTimeEnd = "", int activityID = 0)
		{
			int index = 0;
			GameClient client;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (type != 10 || !client.ClientSocket.IsKuaFuLogin)
				{
					string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						type,
						state,
						activityTimeBegin,
						activityTimeEnd,
						activityID
					});
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strcmd, 770);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		// Token: 0x06002392 RID: 9106 RVA: 0x001E5F5C File Offset: 0x001E415C
		public void NotifyAllOneDollarChongZhiState()
		{
			OneDollarChongZhi odczAct = HuodongCachingMgr.GetOneDollarChongZhiActivity();
			if (null != odczAct)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					odczAct.OnRoleLogin(client);
				}
			}
		}

		// Token: 0x06002393 RID: 9107 RVA: 0x001E5FA4 File Offset: 0x001E41A4
		public void NotifyAllInputFanLiNewState()
		{
			InputFanLiNew iflAct = HuodongCachingMgr.GetInputFanLiNewActivity();
			if (null != iflAct)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					iflAct.OnRoleLogin(client);
				}
			}
		}

		// Token: 0x06002394 RID: 9108 RVA: 0x001E5FEC File Offset: 0x001E41EC
		public void NotifyAllRegressActiveOpenState()
		{
			RegressActiveOpen iflAct = HuodongCachingMgr.GetRegressActiveOpen();
			if (null != iflAct)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					iflAct.OnRoleLogin(client);
				}
			}
		}

		// Token: 0x06002395 RID: 9109 RVA: 0x001E6034 File Offset: 0x001E4234
		public void NotifyAllRegressActiveSignGiftState()
		{
			RegressActiveSignGift iflAct = HuodongCachingMgr.GetRegressActiveSignGift();
			if (null == iflAct)
			{
			}
		}

		// Token: 0x06002396 RID: 9110 RVA: 0x001E6058 File Offset: 0x001E4258
		public void NotifyAllRegressActiveTotalRechargeState()
		{
			RegressActiveTotalRecharge iflAct = HuodongCachingMgr.GetRegressActiveTotalRecharge();
			if (null == iflAct)
			{
			}
		}

		// Token: 0x06002397 RID: 9111 RVA: 0x001E607C File Offset: 0x001E427C
		public void NotifyAllRegressActiveDayBuyState()
		{
			RegressActiveDayBuy iflAct = HuodongCachingMgr.GetRegressActiveDayBuy();
			if (null == iflAct)
			{
			}
		}

		// Token: 0x06002398 RID: 9112 RVA: 0x001E60A0 File Offset: 0x001E42A0
		public void NotifyAllRegressActiveStoreState()
		{
			RegressActiveStore iflAct = HuodongCachingMgr.GetRegressActiveStore();
			if (null == iflAct)
			{
			}
		}

		// Token: 0x06002399 RID: 9113 RVA: 0x001E60C4 File Offset: 0x001E42C4
		public void ReGenerateSpecActGroup()
		{
			SpecialActivity act = HuodongCachingMgr.GetSpecialActivity();
			if (null != act)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					act.OnRoleLogin(client, false);
					if (client._IconStateMgr.CheckSpecialActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		// Token: 0x0600239A RID: 9114 RVA: 0x001E612C File Offset: 0x001E432C
		public void ReGenerateEverydayActGroup()
		{
			EverydayActivity act = HuodongCachingMgr.GetEverydayActivity();
			if (null != act)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					act.OnRoleLogin(client);
					if (client._IconStateMgr.CheckEverydayActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		// Token: 0x0600239B RID: 9115 RVA: 0x001E6194 File Offset: 0x001E4394
		public void ReGenerateSpecPriorityActGroup()
		{
			SpecPriorityActivity act = HuodongCachingMgr.GetSpecPriorityActivity();
			if (null != act)
			{
				int index = 0;
				GameClient client;
				while ((client = this.GetNextClient(ref index, false)) != null)
				{
					act.OnRoleLogin(client, false);
					if (client._IconStateMgr.CheckSpecPriorityActivity(client))
					{
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
			}
		}

		// Token: 0x0600239C RID: 9116 RVA: 0x001E61FC File Offset: 0x001E43FC
		public bool ModifyFuMoLingShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10217");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "附魔灵石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10217", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.FuMoMoney, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 146, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600239D RID: 9117 RVA: 0x001E62F4 File Offset: 0x001E44F4
		public bool ModifyRebornYinJiPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生印记点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10246", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornLevelUpPoint, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 151, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600239E RID: 9118 RVA: 0x001E63EC File Offset: 0x001E45EC
		public bool ModifyRebornCuiLianPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10249");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶淬炼点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10249", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornCuiLian, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 152, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600239F RID: 9119 RVA: 0x001E64E4 File Offset: 0x001E46E4
		public bool ModifyRebornDuanZaoPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10250");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶锻造点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10250", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornDuanZao, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 153, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060023A0 RID: 9120 RVA: 0x001E65DC File Offset: 0x001E47DC
		public bool ModifyRebornNiePanPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10251");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶涅槃点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10251", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornNiePan, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 154, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060023A1 RID: 9121 RVA: 0x001E66D4 File Offset: 0x001E48D4
		public bool ModifyRebornFengYinJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10252");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "封印晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10252", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornFengYin, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 155, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060023A2 RID: 9122 RVA: 0x001E67CC File Offset: 0x001E49CC
		public bool ModifyRebornChongShengJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10253");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10253", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornChongSheng, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 156, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060023A3 RID: 9123 RVA: 0x001E68C4 File Offset: 0x001E4AC4
		public bool ModifyRebornXuanCaiJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10254");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炫彩晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10254", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornXuanCai, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 157, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060023A4 RID: 9124 RVA: 0x001E69BC File Offset: 0x001E4BBC
		public bool ModifyRebornEquipHoleValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long oldValue = (long)Global.GetRoleParamsInt32FromDB(client, "10255");
				long targetValue = oldValue + (long)addValue;
				if (isGM && (targetValue > 2147483647L || targetValue < -2147483648L))
				{
					result = false;
				}
				else
				{
					targetValue = (long)((int)Global.Clamp(targetValue, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生装备槽灌注次数", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)targetValue, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10255", (int)targetValue, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornEquipHole, (long)addValue, targetValue, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 161, (long)((int)targetValue));
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x040036E6 RID: 14054
		private const int MAX_CLIENT_COUNT = 2000;

		// Token: 0x040036E7 RID: 14055
		public const long Before1970Ticks = 62135625600000L;

		// Token: 0x040036E8 RID: 14056
		private GameClient[] _ArrayClients = new GameClient[2000];

		// Token: 0x040036E9 RID: 14057
		private Dictionary<int, int> _DictClientNids = new Dictionary<int, int>(2000);

		// Token: 0x040036EA RID: 14058
		private List<int> _FreeClientList = new List<int>(2000);

		// Token: 0x040036EB RID: 14059
		private SpriteContainer Container = new SpriteContainer();

		// Token: 0x040036EC RID: 14060
		private long LastTransferTicks = 0L;

		// Token: 0x040036ED RID: 14061
		private static double[] IgnoreDefenseAndDogeSubPercent = new double[]
		{
			0.05,
			0.1,
			0.2
		};

		// Token: 0x040036EE RID: 14062
		private int MinVipLevelForDoSpriteMapGridMove = 10;

		// Token: 0x040036EF RID: 14063
		private int MinMainTaskForDoSpriteMapGridMove = 5000;
	}
}
