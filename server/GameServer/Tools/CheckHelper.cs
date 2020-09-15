using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using GameServer.Logic;
using GameServer.Server;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Tools
{
	// Token: 0x020008E8 RID: 2280
	public class CheckHelper
	{
		// Token: 0x060041D9 RID: 16857 RVA: 0x003C2D34 File Offset: 0x003C0F34
		public static XElement LoadXml(string filePath, bool isFatal = true)
		{
			XElement result;
			if (!File.Exists(filePath))
			{
				if (isFatal)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!文件不存在", filePath), null, true);
				}
				else
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("加载[{0}]时出错!!!文件不存在", filePath), null, true);
				}
				result = null;
			}
			else
			{
				XElement xml = XElement.Load(filePath);
				result = xml;
			}
			return result;
		}

		// Token: 0x060041DA RID: 16858 RVA: 0x003C2D94 File Offset: 0x003C0F94
		public static bool CheckTCPCmdFields(TMSKSocket socket, int nID, byte[] data, int count, string[] fields, int length)
		{
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return false;
			}
			fields = cmdData.Split(new char[]
			{
				':'
			});
			bool result;
			if (fields.Length != length)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), fields.Length), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060041DB RID: 16859 RVA: 0x003C2E44 File Offset: 0x003C1044
		public static bool CheckTCPCmdHandle<T>(TMSKSocket socket, int nID, byte[] data, int count, out T cmdData) where T : class, IProtoBuffData, new()
		{
			cmdData = default(T);
			try
			{
				cmdData = DataHelper.BytesToObject2<T>(data, 0, count, socket.m_Socket);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return false;
			}
			return null != cmdData;
		}

		// Token: 0x060041DC RID: 16860 RVA: 0x003C2ECC File Offset: 0x003C10CC
		public static bool CheckCmdLengthAndRole(GameClient client, int nID, string[] cmdParams, int length)
		{
			return CheckHelper.CheckCmdLength(client, nID, cmdParams, length) && CheckHelper.CheckCmdRole(client, nID, cmdParams);
		}

		// Token: 0x060041DD RID: 16861 RVA: 0x003C2EFC File Offset: 0x003C10FC
		public static bool CheckCmdLengthAndUser(GameClient client, int nID, string[] cmdParams, int length)
		{
			return CheckHelper.CheckCmdLength(client, nID, cmdParams, length) && CheckHelper.CheckCmdUser(client, nID, cmdParams);
		}

		// Token: 0x060041DE RID: 16862 RVA: 0x003C2F2C File Offset: 0x003C112C
		public static bool CheckCmdLength(GameClient client, int nID, string[] cmdParams, int length)
		{
			bool result;
			if (cmdParams.Length != length)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), cmdParams.Length), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060041DF RID: 16863 RVA: 0x003C2F7C File Offset: 0x003C117C
		public static bool CheckCmdRole(GameClient client, int nID, string[] cmdParams)
		{
			int roleID = int.Parse(cmdParams[0]);
			bool result;
			if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), roleID), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060041E0 RID: 16864 RVA: 0x003C2FE0 File Offset: 0x003C11E0
		public static bool CheckCmdUser(GameClient client, int nID, string[] cmdParams)
		{
			string userID = cmdParams[0];
			bool result;
			if (client == null || client.strUserID != userID)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("根据userID定位GameClient对象失败, CMD={0}, Client={1}, userID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), userID), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
