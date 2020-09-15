using System;
using System.Collections.Generic;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.UserActivate
{
	// Token: 0x020004A1 RID: 1185
	public class UserActivateManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		// Token: 0x06001607 RID: 5639 RVA: 0x00158F48 File Offset: 0x00157148
		public static UserActivateManager getInstance()
		{
			return UserActivateManager.instance;
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00158F60 File Offset: 0x00157160
		public bool initialize()
		{
			return true;
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x00158F74 File Offset: 0x00157174
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1040, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1041, 5, 5, UserActivateManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x00158FB8 File Offset: 0x001571B8
		public bool showdown()
		{
			return true;
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x00158FCC File Offset: 0x001571CC
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x00158FE0 File Offset: 0x001571E0
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x00158FF4 File Offset: 0x001571F4
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

		// Token: 0x0600160E RID: 5646 RVA: 0x0015903C File Offset: 0x0015723C
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

		// Token: 0x0600160F RID: 5647 RVA: 0x00159194 File Offset: 0x00157394
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

		// Token: 0x06001610 RID: 5648 RVA: 0x0015923C File Offset: 0x0015743C
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

		// Token: 0x06001611 RID: 5649 RVA: 0x001593FC File Offset: 0x001575FC
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

		// Token: 0x06001612 RID: 5650 RVA: 0x00159460 File Offset: 0x00157660
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

		// Token: 0x06001613 RID: 5651 RVA: 0x001594D0 File Offset: 0x001576D0
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

		// Token: 0x06001614 RID: 5652 RVA: 0x00159528 File Offset: 0x00157728
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

		// Token: 0x04001F78 RID: 8056
		private static UserActivateManager instance = new UserActivateManager();

		// Token: 0x020004A2 RID: 1186
		private enum EUserActivateState
		{
			// Token: 0x04001F7A RID: 8058
			NotOpen = -8,
			// Token: 0x04001F7B RID: 8059
			EnoBind,
			// Token: 0x04001F7C RID: 8060
			EBag,
			// Token: 0x04001F7D RID: 8061
			ENoAward,
			// Token: 0x04001F7E RID: 8062
			EFail,
			// Token: 0x04001F7F RID: 8063
			EIsAward,
			// Token: 0x04001F80 RID: 8064
			EPlatform,
			// Token: 0x04001F81 RID: 8065
			ECheck,
			// Token: 0x04001F82 RID: 8066
			Default,
			// Token: 0x04001F83 RID: 8067
			Success
		}
	}
}
