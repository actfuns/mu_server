using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic
{
	
	public class FunctionSendManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		
		private FunctionSendManager()
		{
		}

		
		public static FunctionSendManager GetInstance()
		{
			return FunctionSendManager.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2089, 2, 2, FunctionSendManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2088, 2, 2, FunctionSendManager.GetInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
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

		
		private static FunctionSendManager instance = new FunctionSendManager();

		
		private Dictionary<FunctionType, List<int>> FunctionDict = new Dictionary<FunctionType, List<int>>();

		
		private object mutex = new object();
	}
}
