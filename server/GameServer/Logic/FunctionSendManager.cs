using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000080 RID: 128
	public class FunctionSendManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x060001EA RID: 490 RVA: 0x00020B1C File Offset: 0x0001ED1C
		private FunctionSendManager()
		{
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00020B40 File Offset: 0x0001ED40
		public static FunctionSendManager GetInstance()
		{
			return FunctionSendManager.instance;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00020B58 File Offset: 0x0001ED58
		public bool initialize()
		{
			return true;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00020B6C File Offset: 0x0001ED6C
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00020B80 File Offset: 0x0001ED80
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00020B94 File Offset: 0x0001ED94
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00020BA8 File Offset: 0x0001EDA8
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2089, 2, 2, FunctionSendManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2088, 2, 2, FunctionSendManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00020BEC File Offset: 0x0001EDEC
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int type = Convert.ToInt32(cmdParams[1]);
				if (nID == 2089)
				{
					this.FunctionOperation(client.ClientData.RoleID, type, true);
				}
				else if (nID == 2088)
				{
					this.FunctionOperation(client.ClientData.RoleID, type, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_欢乐代币]{0}", ex.ToString()), null, true);
			}
			return true;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00020C8C File Offset: 0x0001EE8C
		private void FunctionOperation(int roleID, int type, bool isOpen)
		{
			try
			{
				if (isOpen)
				{
					FunctionSendManager.GetInstance().AddFunction((FunctionType)type, roleID);
				}
				else
				{
					FunctionSendManager.GetInstance().DelFunction((FunctionType)type, roleID);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_博彩]CloseFunction{0}", ex.ToString()), null, true);
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00020D1C File Offset: 0x0001EF1C
		public void AddFunction(FunctionType type, int roleID)
		{
			try
			{
				lock (this.mutex)
				{
					List<int> roleList;
					if (this.FunctionDict.TryGetValue(type, out roleList))
					{
						if (roleList.Find((int x) => x == roleID) < 1)
						{
							roleList.Add(roleID);
						}
					}
					else
					{
						roleList = new List<int>
						{
							roleID
						};
						this.FunctionDict.Add(type, roleList);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_关闭功能]type={1},ex={0}", ex.ToString(), type), null, true);
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00020E50 File Offset: 0x0001F050
		public void DelFunction(FunctionType type, int roleID)
		{
			try
			{
				lock (this.mutex)
				{
					List<int> roleList;
					if (this.FunctionDict.TryGetValue(type, out roleList))
					{
						roleList.RemoveAll((int x) => x == roleID);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_关闭功能]type={1},ex={0}", ex.ToString(), type), null, true);
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00020F18 File Offset: 0x0001F118
		public void SendMsg<T>(FunctionType type, int nID, T data)
		{
			try
			{
				List<int> roleList;
				lock (this.mutex)
				{
					if (!this.FunctionDict.TryGetValue(type, out roleList))
					{
						return;
					}
				}
				if (null != roleList)
				{
					List<int> tempList = new List<int>();
					tempList.AddRange(roleList);
					foreach (int rOleID in tempList)
					{
						this.Send<T>(rOleID, type, nID, data);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_关闭功能]type={1},ex={0}", ex.ToString(), type), null, true);
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00021020 File Offset: 0x0001F220
		private void Send<T>(int roleID, FunctionType type, int nID, T data)
		{
			try
			{
				GameClient client = GameManager.ClientMgr.FindClient(roleID);
				if (null == client)
				{
					this.DelFunction(type, roleID);
				}
				else
				{
					client.sendCmd<T>(nID, data, false);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_关闭功能]type={1},ex={0}", ex.ToString(), type), null, true);
			}
		}

		// Token: 0x040002E2 RID: 738
		private static FunctionSendManager instance = new FunctionSendManager();

		// Token: 0x040002E3 RID: 739
		private Dictionary<FunctionType, List<int>> FunctionDict = new Dictionary<FunctionType, List<int>>();

		// Token: 0x040002E4 RID: 740
		private object mutex = new object();
	}
}
