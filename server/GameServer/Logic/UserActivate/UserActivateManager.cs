using System;
using System.Collections.Generic;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.UserActivate
{
	
	public class UserActivateManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		
		public static UserActivateManager getInstance()
		{
			return UserActivateManager.instance;
		}

		
		public bool initialize()
		{
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1040, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1041, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			return true;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1040:
				result = this.ProcessCmdActivateInfo(client, nID, bytes, cmdParams);
				break;
			case 1041:
				result = this.ProcessCmdActivateAward(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		private bool ProcessCmdActivateInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot9))
				{
					client.sendCmd<int>(1040, -8, false);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 5))
				{
					return false;
				}
				int roleID = int.Parse(cmdParams[0]);
				string userID = cmdParams[1];
				int activateType = Convert.ToInt32(cmdParams[2]);
				string activateInfo = cmdParams[3].ToLower();
				string error = cmdParams[4];
				string checkInfo = this.GetCheckInfo(userID, error, activateType);
				if (checkInfo != activateInfo)
				{
					client.sendCmd<int>(1040, -1, false);
					return true;
				}
				PlatformTypes platformType = GameCoreInterface.getinstance().GetPlatformType();
				if (platformType != PlatformTypes.APP)
				{
					client.sendCmd<int>(1040, -2, false);
					return true;
				}
				if (activateType != 0)
				{
					client.sendCmd<int>(1040, -7, false);
					return true;
				}
				int awardState = this.DBActivateStateGet(client);
				client.sendCmd<int>(1040, awardState, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessCmdActivateAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 5))
				{
					return false;
				}
				int roleID = int.Parse(cmdParams[0]);
				string userID = cmdParams[1];
				int activateType = Convert.ToInt32(cmdParams[2]);
				string activateInfo = cmdParams[3].ToLower();
				string error = cmdParams[4];
				UserActivateManager.EUserActivateState state = this.ActivateAward(client, roleID, userID, activateType, activateInfo, error);
				client.sendCmd<int>(1041, (int)state, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private UserActivateManager.EUserActivateState ActivateAward(GameClient client, int roleId, string userID, int activateType, string activateInfo, string error)
		{
			string checkInfo = this.GetCheckInfo(userID, error, activateType);
			UserActivateManager.EUserActivateState result;
			if (checkInfo != activateInfo)
			{
				result = UserActivateManager.EUserActivateState.ECheck;
			}
			else if (activateType != 0)
			{
				result = UserActivateManager.EUserActivateState.EnoBind;
			}
			else
			{
				PlatformTypes platformType = GameCoreInterface.getinstance().GetPlatformType();
				if (platformType != PlatformTypes.APP)
				{
					result = UserActivateManager.EUserActivateState.EPlatform;
				}
				else
				{
					int awardState = this.DBActivateStateGet(client);
					if (awardState == 1)
					{
						result = UserActivateManager.EUserActivateState.EIsAward;
					}
					else
					{
						List<GoodsData> awardList = this.GetAwardList();
						if (awardList == null || awardList.Count <= 0)
						{
							result = UserActivateManager.EUserActivateState.ENoAward;
						}
						else if (!Global.CanAddGoodsDataList(client, awardList))
						{
							result = UserActivateManager.EUserActivateState.EBag;
						}
						else if (!this.DBActivateStateSet(client))
						{
							result = UserActivateManager.EUserActivateState.EFail;
						}
						else
						{
							for (int i = 0; i < awardList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "账号绑定奖励", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
							}
							result = UserActivateManager.EUserActivateState.Success;
						}
					}
				}
			}
			return result;
		}

		
		private string GetCheckInfo(string userID, string error, int activateType)
		{
			userID = userID.ToLower().Replace("apps", "");
			string key = "WwSiia943ui3Wej5NrqUI3rfqrf83quj";
			string result = string.Format("{0}error={1}&ret={2}&uid={3}", new object[]
			{
				key,
				error,
				activateType,
				userID
			});
			result = MD5Helper.get_md5_string(result);
			return result.ToLower();
		}

		
		private List<GoodsData> GetAwardList()
		{
			List<GoodsData> list = new List<GoodsData>();
			string str = GameManager.systemParamsList.GetParamValueByName("App_BindPhoneAward");
			List<GoodsData> result;
			if (string.IsNullOrEmpty(str))
			{
				result = null;
			}
			else
			{
				string[] fields = str.Split(new char[]
				{
					'|'
				});
				if (fields.Length > 0)
				{
					list = GoodsHelper.ParseGoodsDataList(fields, "SystemParams.xml");
				}
				result = list;
			}
			return result;
		}

		
		private int DBActivateStateGet(GameClient client)
		{
			int result = 0;
			string cmd2db = string.Format("{0}", client.strUserID);
			string[] dbFields = Global.ExecuteDBCmd(13120, cmd2db, client.ServerId);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = int.Parse(dbFields[0]);
			}
			return result;
		}

		
		private bool DBActivateStateSet(GameClient client)
		{
			bool result = false;
			string cmd2db = string.Format("{0}:{1}:{2}", client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleID);
			string[] dbFields = Global.ExecuteDBCmd(13121, cmd2db, client.ServerId);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = (dbFields[0] == "1");
			}
			return result;
		}

		
		private static UserActivateManager instance = new UserActivateManager();

		
		private enum EUserActivateState
		{
			
			NotOpen = -8,
			
			EnoBind,
			
			EBag,
			
			ENoAward,
			
			EFail,
			
			EIsAward,
			
			EPlatform,
			
			ECheck,
			
			Default,
			
			Success
		}
	}
}
