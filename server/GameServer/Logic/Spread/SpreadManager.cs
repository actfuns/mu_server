using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.Spread
{
	// Token: 0x02000454 RID: 1108
	public class SpreadManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		// Token: 0x06001449 RID: 5193 RVA: 0x0013FB50 File Offset: 0x0013DD50
		public static SpreadManager getInstance()
		{
			return SpreadManager.instance;
		}

		// Token: 0x0600144A RID: 5194 RVA: 0x0013FB68 File Offset: 0x0013DD68
		public bool initialize()
		{
			SpreadManager.InitConfig();
			return true;
		}

		// Token: 0x0600144B RID: 5195 RVA: 0x0013FB84 File Offset: 0x0013DD84
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1017, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1018, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1019, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1020, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1021, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1022, 1, 1, SpreadManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10014, 10002, SpreadManager.getInstance());
			return true;
		}

		// Token: 0x0600144C RID: 5196 RVA: 0x0013FC44 File Offset: 0x0013DE44
		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10014, 10002, SpreadManager.getInstance());
			return true;
		}

		// Token: 0x0600144D RID: 5197 RVA: 0x0013FC74 File Offset: 0x0013DE74
		public bool destroy()
		{
			return true;
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x0013FC88 File Offset: 0x0013DE88
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0013FC9C File Offset: 0x0013DE9C
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1017:
				result = this.ProcessSpreadSignCmd(client, nID, bytes, cmdParams);
				break;
			case 1018:
				result = this.ProcessSpreadAwardCmd(client, nID, bytes, cmdParams);
				break;
			case 1019:
				result = this.ProcessSpreadVerifyCodeCmd(client, nID, bytes, cmdParams);
				break;
			case 1020:
				result = this.ProcessSpreadTelCodeGetCmd(client, nID, bytes, cmdParams);
				break;
			case 1021:
				result = this.ProcessSpreadTelCodeVerifyCmd(client, nID, bytes, cmdParams);
				break;
			case 1022:
				result = this.ProcessSpreadInfoCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0013FD2C File Offset: 0x0013DF2C
		public bool ProcessSpreadInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				SpreadData data = this.GetSpreadInfo(client);
				client.sendCmd<SpreadData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0013FD98 File Offset: 0x0013DF98
		public bool ProcessSpreadSignCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				string result = this.SpreadSign(client);
				client.sendCmd(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0013FE04 File Offset: 0x0013E004
		public bool ProcessSpreadAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int awardType = Convert.ToInt32(cmdParams[0]);
				SpreadData data = this.SpreadAward(client, awardType);
				client.sendCmd<SpreadData>(nID, data, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0013FE80 File Offset: 0x0013E080
		public bool ProcessSpreadVerifyCodeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string verifyCode = cmdParams[0];
				client.sendCmd(nID, ((int)this.SpreadVerifyCode(client, verifyCode)).ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0013FEFC File Offset: 0x0013E0FC
		public bool ProcessSpreadTelCodeGetCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string tel = cmdParams[0];
				client.sendCmd(nID, ((int)this.SpreadTelCodeGet(client, tel)).ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0013FF78 File Offset: 0x0013E178
		public bool ProcessSpreadTelCodeVerifyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string telCode = cmdParams[0];
				client.sendCmd(nID, ((int)this.SpreadTelCodeVerify(client, telCode)).ToString(), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0013FFF4 File Offset: 0x0013E1F4
		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10014)
			{
				KFNotifySpreadCountGameEvent e = eventObject as KFNotifySpreadCountGameEvent;
				if (null != e)
				{
					GameClient client = GameManager.ClientMgr.FindClient(e.PRoleID);
					if (null != client)
					{
						SpreadManager.initRoleSpreadData(client);
					}
					eventObject.Handled = true;
				}
			}
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x00140058 File Offset: 0x0013E258
		private SpreadData GetSpreadInfo(GameClient client)
		{
			SpreadData mySpreadData;
			lock (client.ClientData.LockSpread)
			{
				SpreadData data = client.ClientData.MySpreadData;
				if (data != null)
				{
					data.State = 0;
				}
				if (SpreadManager.IsSpreadOpen() != data.IsOpen)
				{
					SpreadManager.initRoleSpreadData(client);
				}
				mySpreadData = client.ClientData.MySpreadData;
			}
			return mySpreadData;
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x001400E8 File Offset: 0x0013E2E8
		private string SpreadSign(GameClient client)
		{
			string result2;
			lock (client.ClientData.LockSpread)
			{
				string result = "{0}:{1}";
				SpreadData data = this.GetSpreadInfo(client);
				if (!data.IsOpen)
				{
					result2 = string.Format(result, -2, "");
				}
				else if (!string.IsNullOrEmpty(data.SpreadCode))
				{
					result2 = string.Format(result, -21, "");
				}
				else
				{
					string spreadCode = this.GetSpreadCode(client);
					if (!SpreadManager.HSpreadSign(client))
					{
						result2 = string.Format(result, -21, "");
					}
					else
					{
						data.SpreadCode = spreadCode;
						Global.UpdateRoleParamByName(client, "SpreadCode", spreadCode, true);
						result2 = string.Format(result, 1, spreadCode);
					}
				}
			}
			return result2;
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x001401E8 File Offset: 0x0013E3E8
		private SpreadData SpreadAward(GameClient client, int awardType)
		{
			SpreadData result;
			lock (client.ClientData.LockSpread)
			{
				SpreadData data = this.GetSpreadInfo(client);
				if (!data.IsOpen)
				{
					data.State = -2;
					result = data;
				}
				else
				{
					ESpreadState resultType = ESpreadState.Default;
					switch (awardType)
					{
					case 1:
						resultType = this.AwardOne(client, data, awardType, SpreadManager._awardVipInfo, data.CountVip);
						break;
					case 2:
						resultType = this.AwardOne(client, data, awardType, SpreadManager._awardLevelInfo, data.CountLevel);
						break;
					case 3:
						resultType = this.AwardCount(client, data);
						break;
					case 4:
						resultType = this.AwardOne(client, data, awardType, SpreadManager._awardVerifyInfo, 1);
						break;
					}
					if (resultType != ESpreadState.Success)
					{
						data.State = (int)resultType;
					}
					result = client.ClientData.MySpreadData;
				}
			}
			return result;
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x001402E4 File Offset: 0x0013E4E4
		private ESpreadState SpreadVerifyCode(GameClient client, string verifyCode)
		{
			ESpreadState result;
			lock (client.ClientData.LockSpread)
			{
				if (string.IsNullOrEmpty(verifyCode))
				{
					result = ESpreadState.EVerifyCodeNull;
				}
				else
				{
					SpreadData data = this.GetSpreadInfo(client);
					if (!data.IsOpen)
					{
						result = ESpreadState.ENoOpen;
					}
					else if (!string.IsNullOrEmpty(data.VerifyCode))
					{
						result = ESpreadState.EVerifyCodeHave;
					}
					else
					{
						DateTime regTime = Global.GetRegTime(client.ClientData);
						if (regTime < SpreadManager._createDate)
						{
							result = ESpreadState.EVerifyOutTime;
						}
						else
						{
							string[] codes = verifyCode.Split(new char[]
							{
								'#'
							});
							if (codes.Length < 2)
							{
								result = ESpreadState.EVerifyCodeWrong;
							}
							else
							{
								int pzoneID = StringUtil.SpreadCodeToID(codes[0]);
								int proleID = StringUtil.SpreadCodeToID(codes[1]);
								if (pzoneID == client.ClientData.ZoneID && proleID == client.ClientData.RoleID)
								{
									result = ESpreadState.EVerifySelf;
								}
								else
								{
									ESpreadState checkState = this.HCheckVerifyCode(client, pzoneID, proleID);
									if (checkState == ESpreadState.EVerifyCodeHave)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(532, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										result = ESpreadState.EVerifyCodeHave;
									}
									else if (checkState != ESpreadState.Success)
									{
										result = checkState;
									}
									else
									{
										SpreadVerifyData verifyData = new SpreadVerifyData();
										verifyData.VerifyCode = verifyCode;
										client.ClientData.MySpreadVerifyData = verifyData;
										result = ESpreadState.Success;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x001404C0 File Offset: 0x0013E6C0
		private ESpreadState SpreadTelCodeGet(GameClient client, string tel)
		{
			ESpreadState result2;
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadData = this.GetSpreadInfo(client);
				if (!spreadData.IsOpen)
				{
					result2 = ESpreadState.ENoOpen;
				}
				else
				{
					SpreadVerifyData verifyData = client.ClientData.MySpreadVerifyData;
					if (verifyData == null || string.IsNullOrEmpty(verifyData.VerifyCode))
					{
						result2 = ESpreadState.EVerifyCodeNull;
					}
					else if (string.IsNullOrEmpty(tel))
					{
						result2 = ESpreadState.ETelNull;
					}
					else if (!this.IsTel(tel))
					{
						result2 = ESpreadState.ETelWrong;
					}
					else if (!string.IsNullOrEmpty(verifyData.Tel) && TimeUtil.NowDateTime() < verifyData.TelTime.AddSeconds((double)SpreadManager.TEL_CODE_VERIFY_SECOND))
					{
						result2 = ESpreadState.Success;
					}
					else
					{
						ESpreadState result = this.HTelCodeGet(client, tel);
						if (result != ESpreadState.Success)
						{
							result2 = result;
						}
						else if (string.IsNullOrEmpty(tel))
						{
							result2 = ESpreadState.ETelCodeGet;
						}
						else
						{
							verifyData.Tel = tel;
							verifyData.TelTime = TimeUtil.NowDateTime();
							result2 = ESpreadState.Success;
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00140614 File Offset: 0x0013E814
		private ESpreadState SpreadTelCodeVerify(GameClient client, string telCode)
		{
			ESpreadState result2;
			lock (client.ClientData.LockSpread)
			{
				SpreadData spreadData = this.GetSpreadInfo(client);
				if (!spreadData.IsOpen)
				{
					result2 = ESpreadState.ENoOpen;
				}
				else
				{
					SpreadVerifyData verifyData = client.ClientData.MySpreadVerifyData;
					if (verifyData == null || string.IsNullOrEmpty(verifyData.VerifyCode))
					{
						result2 = ESpreadState.EVerifyCodeNull;
					}
					else if (string.IsNullOrEmpty(verifyData.Tel))
					{
						result2 = ESpreadState.ETelNull;
					}
					else if (TimeUtil.NowDateTime() > verifyData.TelTime.AddSeconds((double)SpreadManager.TEL_CODE_VERIFY_SECOND))
					{
						result2 = ESpreadState.ETelCodeOutTime;
					}
					else if (!this.IsTelCode(telCode))
					{
						result2 = ESpreadState.ETelCodeWrong;
					}
					else
					{
						int code = int.Parse(telCode);
						ESpreadState result = this.HTelCodeVerify(client, code);
						if (result != ESpreadState.Success)
						{
							result2 = result;
						}
						else
						{
							int isVip = (client.ClientData.VipLevel >= SpreadManager._vipLimit) ? 1 : 0;
							int isLevel = (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level >= SpreadManager._levelLimit) ? 1 : 0;
							if (isVip > 0)
							{
								Global.UpdateRoleParamByName(client, "SpreadIsVip", "1", true);
							}
							if (isLevel > 0)
							{
								Global.UpdateRoleParamByName(client, "SpreadIsLevel", "1", true);
							}
							spreadData.VerifyCode = verifyData.VerifyCode;
							client.ClientData.MySpreadVerifyData = null;
							Global.UpdateRoleParamByName(client, "VerifyCode", verifyData.VerifyCode, true);
							result2 = ESpreadState.Success;
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x001407F0 File Offset: 0x0013E9F0
		private ESpreadState AwardOne(GameClient client, SpreadData data, int awardType, SpreadAwardInfo awardInfo, int countSum)
		{
			ESpreadState result2;
			lock (client.ClientData.LockSpread)
			{
				bool isAward = false;
				ESpreadState resultState = ESpreadState.Fail;
				switch (awardType)
				{
				case 1:
				case 2:
					if (string.IsNullOrEmpty(data.SpreadCode))
					{
						return ESpreadState.ESpreadNo;
					}
					break;
				case 4:
					if (string.IsNullOrEmpty(data.VerifyCode))
					{
						return ESpreadState.EVerifyNo;
					}
					break;
				}
				string countGetStr = "";
				int countGet = 0;
				data.AwardDic.TryGetValue(awardType, out countGetStr);
				if (!string.IsNullOrEmpty(countGetStr))
				{
					countGet = Math.Max(int.Parse(countGetStr), 0);
				}
				int countTotal = countSum - countGet;
				if (countTotal <= 0)
				{
					result2 = ESpreadState.ENoAward;
				}
				else
				{
					int num = (countTotal + 9) / 10;
					for (int i = 0; i < num; i++)
					{
						int left = countTotal - i * 10;
						int count = Math.Min(left, 10);
						List<GoodsData> awardList = new List<GoodsData>();
						if (awardInfo != null && awardInfo.DefaultGoodsList != null)
						{
							awardList.AddRange(awardInfo.DefaultGoodsList);
						}
						List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
						if (proGoods != null)
						{
							awardList.AddRange(proGoods);
						}
						if (!Global.CanAddGoodsDataList(client, awardList))
						{
							resultState = ESpreadState.ENoBag;
							break;
						}
						countGet += count;
						bool result = SpreadManager.DBAwardUpdate(client.ClientData.ZoneID, client.ClientData.RoleID, awardType, countGet.ToString(), client.ServerId);
						if (!result)
						{
							resultState = ESpreadState.Fail;
							break;
						}
						if (data.AwardDic.ContainsKey(awardType))
						{
							data.AwardDic[awardType] = countGet.ToString();
						}
						else
						{
							data.AwardDic.Add(awardType, countGet.ToString());
						}
						for (int j = 0; j < awardList.Count; j++)
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[j].GoodsID, awardList[j].GCount * count, awardList[j].Quality, "", awardList[j].Forge_level, awardList[j].Binding, 0, "", true, 1, "推广", "1900-01-01 12:00:00", awardList[j].AddPropIndex, awardList[j].BornIndex, awardList[j].Lucky, awardList[j].Strong, awardList[j].ExcellenceInfo, awardList[j].AppendPropLev, 0, null, null, 0, true);
						}
						isAward = true;
					}
					if (isAward)
					{
						SpreadManager.CheckActivityTip(client);
						result2 = ESpreadState.Success;
					}
					else
					{
						result2 = resultState;
					}
				}
			}
			return result2;
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x00140B70 File Offset: 0x0013ED70
		private ESpreadState AwardCount(GameClient client, SpreadData data)
		{
			ESpreadState result2;
			lock (client.ClientData.LockSpread)
			{
				bool isAward = false;
				ESpreadState resultState = ESpreadState.Fail;
				int awardType = 3;
				if (string.IsNullOrEmpty(data.SpreadCode))
				{
					result2 = ESpreadState.ESpreadNo;
				}
				else
				{
					IEnumerable<KeyValuePair<int, int>> tempDic = from dic in data.AwardCountDic
					where dic.Value == 0 && dic.Key <= data.CountRole
					select dic;
					if (!tempDic.Any<KeyValuePair<int, int>>())
					{
						result2 = ESpreadState.ENoAward;
					}
					else
					{
						Dictionary<int, int> temp = new Dictionary<int, int>();
						foreach (KeyValuePair<int, int> d in tempDic)
						{
							temp.Add(d.Key, d.Value);
						}
						foreach (KeyValuePair<int, int> d in temp)
						{
							if (!SpreadManager._awardCountDic.ContainsKey(d.Key))
							{
								resultState = ESpreadState.ENoAward;
								break;
							}
							SpreadCountAwardInfo awardInfo = SpreadManager._awardCountDic[d.Key];
							if (awardInfo == null)
							{
								resultState = ESpreadState.ENoAward;
								break;
							}
							List<GoodsData> awardList = new List<GoodsData>();
							if (awardInfo != null && awardInfo.DefaultGoodsList != null)
							{
								awardList.AddRange(awardInfo.DefaultGoodsList);
							}
							List<GoodsData> proGoods = GoodsHelper.GetAwardPro(client, awardInfo.ProGoodsList);
							if (proGoods != null)
							{
								awardList.AddRange(proGoods);
							}
							if (!Global.CanAddGoodsDataList(client, awardList))
							{
								resultState = ESpreadState.ENoBag;
								break;
							}
							data.AwardCountDic[d.Key] = 1;
							string awardString = SpreadManager.AwardCountToStr(data.AwardCountDic);
							bool result = SpreadManager.DBAwardUpdate(client.ClientData.ZoneID, client.ClientData.RoleID, 3, awardString, client.ServerId);
							if (!result)
							{
								resultState = ESpreadState.Fail;
								data.AwardCountDic[d.Key] = 0;
								break;
							}
							if (data.AwardDic.ContainsKey(awardType))
							{
								data.AwardDic[awardType] = awardString;
							}
							else
							{
								data.AwardDic.Add(awardType, awardString);
							}
							for (int i = 0; i < awardList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "推广", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
							}
							isAward = true;
						}
						if (isAward)
						{
							SpreadManager.CheckActivityTip(client);
							result2 = ESpreadState.Success;
						}
						else
						{
							result2 = resultState;
						}
					}
				}
			}
			return result2;
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x00140F90 File Offset: 0x0013F190
		public static void initRoleSpreadData(GameClient client)
		{
			lock (client.ClientData.LockSpread)
			{
				SpreadData data = new SpreadData();
				data.IsOpen = SpreadManager.IsSpreadOpen();
				if (!data.IsOpen)
				{
					client.ClientData.MySpreadData = data;
				}
				else
				{
					data.SpreadCode = Global.GetRoleParamByName(client, "SpreadCode");
					data.VerifyCode = Global.GetRoleParamByName(client, "VerifyCode");
					if (string.IsNullOrEmpty(data.SpreadCode) && string.IsNullOrEmpty(data.VerifyCode))
					{
						client.ClientData.MySpreadData = data;
					}
					else
					{
						if (!string.IsNullOrEmpty(data.SpreadCode))
						{
							int[] counts = SpreadManager.HSpreadCount(client);
							data.CountRole = counts[0];
							data.CountVip = Math.Min(counts[1], SpreadManager._vipCountMax);
							data.CountLevel = Math.Min(counts[2], SpreadManager._levelCountMax);
						}
						data.AwardDic = SpreadManager.DBAwardGet(client.ClientData.ZoneID, client.ClientData.RoleID, client.ServerId);
						string countStr = "";
						data.AwardDic.TryGetValue(3, out countStr);
						data.AwardCountDic = SpreadManager.initAwardCountDic(countStr);
						client.ClientData.MySpreadData = data;
						SpreadManager.CheckActivityTip(client);
					}
				}
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x0014111C File Offset: 0x0013F31C
		private static Dictionary<int, int> initAwardCountDic(string awardStr)
		{
			Dictionary<int, int> dic = new Dictionary<int, int>();
			if (string.IsNullOrEmpty(awardStr))
			{
				foreach (int c in SpreadManager._awardCountDic.Keys)
				{
					dic.Add(c, 0);
				}
			}
			else
			{
				string[] arr = awardStr.Split(new char[]
				{
					','
				});
				for (int i = 0; i < arr.Length; i++)
				{
					dic.Add(int.Parse(arr[i]), int.Parse(arr[++i]));
				}
			}
			return dic;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x001411EC File Offset: 0x0013F3EC
		private static string AwardCountToStr(Dictionary<int, int> dic)
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, int> d in dic)
			{
				sb.Append(string.Format("{0},{1},", d.Key, d.Value));
			}
			string str = sb.ToString();
			return str.Substring(0, str.Length - 1);
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00141334 File Offset: 0x0013F534
		private static void CheckActivityTip(GameClient client)
		{
			SpreadData data = client.ClientData.MySpreadData;
			bool isTip = false;
			if (!string.IsNullOrEmpty(data.SpreadCode))
			{
				if (data.CountLevel > 0)
				{
					if (!data.AwardDic.ContainsKey(2))
					{
						isTip = true;
					}
					else
					{
						int count = int.Parse(data.AwardDic[2]);
						if (data.CountLevel - count > 0)
						{
							isTip = true;
						}
					}
				}
				if (data.CountVip > 0)
				{
					if (!data.AwardDic.ContainsKey(1))
					{
						isTip = true;
					}
					else
					{
						IEnumerable<KeyValuePair<int, string>> temp = from info in data.AwardDic
						where info.Key == 1 && data.CountVip - int.Parse(info.Value) > 0
						select info;
						if (temp.Any<KeyValuePair<int, string>>())
						{
							isTip = true;
						}
					}
				}
				if (data.CountRole > 0)
				{
					IEnumerable<KeyValuePair<int, int>> temp2 = from info in data.AwardCountDic
					where info.Key <= data.CountRole && info.Value == 0
					select info;
					if (temp2.Any<KeyValuePair<int, int>>())
					{
						isTip = true;
					}
				}
			}
			if (!string.IsNullOrEmpty(data.VerifyCode))
			{
				IEnumerable<KeyValuePair<int, string>> temp = from info in data.AwardDic
				where info.Key == 4 && int.Parse(info.Value) <= 0
				select info;
				if (temp.Any<KeyValuePair<int, string>>())
				{
					isTip = true;
				}
			}
			if (isTip)
			{
				client._IconStateMgr.AddFlushIconState(14105, true);
			}
			else
			{
				client._IconStateMgr.AddFlushIconState(14105, false);
			}
			client._IconStateMgr.SendIconStateToClient(client);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x0014154C File Offset: 0x0013F74C
		private string GetSpreadCode(GameClient client)
		{
			int zoneID = client.ClientData.ZoneID;
			int roleID = client.ClientData.RoleID;
			return string.Format("{0}#{1}", StringUtil.SpreadIDToCode(zoneID), StringUtil.SpreadIDToCode(roleID));
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x0014158C File Offset: 0x0013F78C
		public static bool IsSpreadOpen()
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(GameFuncType.System1Dot8))
			{
				result = false;
			}
			else
			{
				int state = 0;
				switch (GameCoreInterface.getinstance().GetPlatformType())
				{
				case PlatformTypes.APP:
					state = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_APP", -1);
					break;
				case PlatformTypes.YueYu:
					state = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_YueYu", -1);
					break;
				case PlatformTypes.Android:
					state = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuang_Android", -1);
					break;
				}
				result = (state > 0);
			}
			return result;
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00141618 File Offset: 0x0013F818
		public void SpreadIsLevel(GameClient client)
		{
			SpreadData spreadData = this.GetSpreadInfo(client);
			if (spreadData != null && spreadData.IsOpen && !string.IsNullOrEmpty(spreadData.VerifyCode))
			{
				int level = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				bool isLevel = Global.GetRoleParamsInt32FromDB(client, "SpreadIsLevel") > 0;
				if (level >= SpreadManager._levelLimit && !isLevel)
				{
					string[] codes = spreadData.VerifyCode.Split(new char[]
					{
						'#'
					});
					if (codes.Length >= 2)
					{
						int pzoneID = StringUtil.SpreadCodeToID(codes[0]);
						int proleID = StringUtil.SpreadCodeToID(codes[1]);
						bool result = SpreadClient.getInstance().SpreadLevel(pzoneID, proleID, client.ClientData.ZoneID, client.ClientData.RoleID);
						if (result)
						{
							Global.UpdateRoleParamByName(client, "SpreadIsLevel", "1", true);
						}
					}
				}
			}
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x0014171C File Offset: 0x0013F91C
		public void SpreadIsVip(GameClient client)
		{
			SpreadData spreadData = this.GetSpreadInfo(client);
			if (spreadData != null && spreadData.IsOpen && !string.IsNullOrEmpty(spreadData.VerifyCode))
			{
				int vip = client.ClientData.VipLevel;
				bool isVip = Global.GetRoleParamsInt32FromDB(client, "SpreadIsVip") > 0;
				if (vip >= SpreadManager._vipLimit && !isVip)
				{
					string[] codes = spreadData.VerifyCode.Split(new char[]
					{
						'#'
					});
					if (codes.Length >= 2)
					{
						int pzoneID = StringUtil.SpreadCodeToID(codes[0]);
						int proleID = StringUtil.SpreadCodeToID(codes[1]);
						bool result = SpreadClient.getInstance().SpreadVip(pzoneID, proleID, client.ClientData.ZoneID, client.ClientData.RoleID);
						if (result)
						{
							Global.UpdateRoleParamByName(client, "SpreadIsVip", "1", true);
						}
					}
				}
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00141814 File Offset: 0x0013FA14
		public static Dictionary<int, string> DBAwardGet(int zoneID, int roleID, int serverID)
		{
			Dictionary<int, string> result = new Dictionary<int, string>();
			string awardStr = "";
			string cmd2db = string.Format("{0}:{1}", zoneID, roleID);
			string[] dbFields = Global.ExecuteDBCmd(13114, cmd2db, serverID);
			if (dbFields != null && dbFields.Length == 1)
			{
				awardStr = dbFields[0];
			}
			if (!string.IsNullOrEmpty(awardStr))
			{
				string[] awardArr = awardStr.Split(new char[]
				{
					'$'
				});
				foreach (string s in awardArr)
				{
					string[] award = s.Split(new char[]
					{
						'#'
					});
					result.Add(int.Parse(award[0]), award[1]);
				}
			}
			return result;
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x001418F0 File Offset: 0x0013FAF0
		public static bool DBAwardUpdate(int zoneID, int roleID, int awardType, string award, int serverID)
		{
			bool result = false;
			string cmd2db = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				zoneID,
				roleID,
				awardType,
				award
			});
			string[] dbFields = Global.ExecuteDBCmd(13115, cmd2db, serverID);
			if (dbFields != null && dbFields.Length == 1)
			{
				result = (dbFields[0] == "1");
			}
			return result;
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00141970 File Offset: 0x0013FB70
		public static bool HSpreadSign(GameClient client)
		{
			int result = SpreadClient.getInstance().SpreadSign(client.ClientData.ZoneID, client.ClientData.RoleID);
			return result > 0;
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x001419A8 File Offset: 0x0013FBA8
		public static int[] HSpreadCount(GameClient client)
		{
			return SpreadClient.getInstance().SpreadCount(client.ClientData.ZoneID, client.ClientData.RoleID);
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x001419DC File Offset: 0x0013FBDC
		private ESpreadState HCheckVerifyCode(GameClient client, int pzoneID, int proleID)
		{
			int isVip = (client.ClientData.VipLevel >= SpreadManager._vipLimit) ? 1 : 0;
			int isLevel = (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level >= SpreadManager._levelLimit) ? 1 : 0;
			return (ESpreadState)SpreadClient.getInstance().CheckVerifyCode(client.strUserID, client.ClientData.ZoneID, client.ClientData.RoleID, pzoneID, proleID, isVip, isLevel);
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x00141A5C File Offset: 0x0013FC5C
		private ESpreadState HTelCodeGet(GameClient client, string tel)
		{
			return (ESpreadState)SpreadClient.getInstance().TelCodeGet(client.ClientData.ZoneID, client.ClientData.RoleID, tel);
		}

		// Token: 0x0600146D RID: 5229 RVA: 0x00141A94 File Offset: 0x0013FC94
		private ESpreadState HTelCodeVerify(GameClient client, int telCode)
		{
			return (ESpreadState)SpreadClient.getInstance().TelCodeVerify(client.ClientData.ZoneID, client.ClientData.RoleID, telCode);
		}

		// Token: 0x0600146E RID: 5230 RVA: 0x00141ACC File Offset: 0x0013FCCC
		private bool IsTel(string tel)
		{
			return Regex.IsMatch(tel.ToString(), "^(0|86|17951)?(1[0-9])[0-9]{9}$");
		}

		// Token: 0x0600146F RID: 5231 RVA: 0x00141AF0 File Offset: 0x0013FCF0
		private bool IsTelCode(string tel)
		{
			return Regex.IsMatch(tel.ToString(), "^\\d{6}$");
		}

		// Token: 0x06001470 RID: 5232 RVA: 0x00141B14 File Offset: 0x0013FD14
		private static bool InitConfig()
		{
			string fileName = "";
			try
			{
				SpreadManager._awardCountDic.Clear();
				fileName = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanLeiJi.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				XElement args = xml.Element("GiftList");
				if (null == args)
				{
					return false;
				}
				IEnumerable<XElement> xmlItems = args.Elements();
				string goods;
				foreach (XElement xmlItem in xmlItems)
				{
					if (xmlItem != null)
					{
						SpreadCountAwardInfo info = new SpreadCountAwardInfo();
						info.Count = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "MinNum", "0"));
						goods = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
						if (!string.IsNullOrEmpty(goods))
						{
							string[] fields = goods.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								info.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
							}
						}
						goods = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
						if (!string.IsNullOrEmpty(goods))
						{
							string[] fields = goods.Split(new char[]
							{
								'|'
							});
							if (fields.Length > 0)
							{
								info.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
							}
						}
						SpreadManager._awardCountDic.Add(info.Count, info);
					}
				}
				SpreadManager._levelLimit = 0;
				SpreadManager._awardLevelInfo = new SpreadAwardInfo();
				fileName = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanLevel.xml");
				xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				args = xml.Element("TuiGuangYuanLevel");
				if (null == args)
				{
					return false;
				}
				int zhuanSheng = Convert.ToInt32(Global.GetDefAttributeStr(args, "MinZhuanSheng", "0"));
				int level = Convert.ToInt32(Global.GetDefAttributeStr(args, "MinLevel", "0"));
				SpreadManager._levelLimit = zhuanSheng * 100 + level;
				args = xml.Element("GiftList").Element("Award");
				if (null == args)
				{
					return false;
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsOne");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardLevelInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsTwo");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardLevelInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				SpreadManager._vipLimit = 0;
				SpreadManager._awardVipInfo = new SpreadAwardInfo();
				fileName = Global.IsolateResPath("Config/TuiGuang/TuiGuangYuanVip.xml");
				xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				args = xml.Element("TuiGuangYuanVip");
				if (null == args)
				{
					return false;
				}
				SpreadManager._vipLimit = Convert.ToInt32(Global.GetDefAttributeStr(args, "VipLevel", "0"));
				args = xml.Element("GiftList").Element("Award");
				if (null == args)
				{
					return false;
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsOne");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardVipInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsTwo");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardVipInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				SpreadManager._awardVerifyInfo = new SpreadAwardInfo();
				fileName = Global.IsolateResPath("Config/TuiGuang/TuiGuangXinYongHu.xml");
				xml = CheckHelper.LoadXml(fileName, true);
				if (null == xml)
				{
					return false;
				}
				args = xml.Element("GiftList").Element("Award");
				if (null == args)
				{
					return false;
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsOne");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardVerifyInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				goods = Global.GetSafeAttributeStr(args, "GoodsTwo");
				if (!string.IsNullOrEmpty(goods))
				{
					string[] fields = goods.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						SpreadManager._awardVerifyInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
					}
				}
				string createDate = GameManager.systemParamsList.GetParamValueByName("TuiGuangCreatData");
				SpreadManager._createDate = DateTime.Parse(createDate);
				SpreadManager._vipCountMax = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuangVIPRewardNum", SpreadManager.VIP_LEVEL_COUNT_MAX_DEFAULT);
				SpreadManager._levelCountMax = (int)GameManager.systemParamsList.GetParamValueIntByName("TuiGuangLevelRewardNum", SpreadManager.VIP_LEVEL_COUNT_MAX_DEFAULT);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("加载xml配置文件:{0}, 失败。", fileName), ex, true);
				return false;
			}
			return true;
		}

		// Token: 0x06001471 RID: 5233 RVA: 0x0014214C File Offset: 0x0014034C
		public void SpreadSetCount(GameClient client, int[] counts)
		{
			SpreadData data = this.GetSpreadInfo(client);
			data.CountRole = counts[0];
			data.CountVip = Math.Min(counts[1], SpreadManager._vipCountMax);
			data.CountLevel = Math.Min(counts[2], SpreadManager._levelCountMax);
			client.ClientData.MySpreadData = data;
		}

		// Token: 0x04001DE2 RID: 7650
		public const int _sceneType = 10002;

		// Token: 0x04001DE3 RID: 7651
		private static int TEL_CODE_VERIFY_SECOND = 70;

		// Token: 0x04001DE4 RID: 7652
		private static int AWARD_COUNT_MAX = 10;

		// Token: 0x04001DE5 RID: 7653
		private static SpreadManager instance = new SpreadManager();

		// Token: 0x04001DE6 RID: 7654
		private static Dictionary<int, SpreadCountAwardInfo> _awardCountDic = new Dictionary<int, SpreadCountAwardInfo>();

		// Token: 0x04001DE7 RID: 7655
		private static int _levelLimit = 0;

		// Token: 0x04001DE8 RID: 7656
		private static SpreadAwardInfo _awardLevelInfo = new SpreadAwardInfo();

		// Token: 0x04001DE9 RID: 7657
		private static int _vipLimit = 0;

		// Token: 0x04001DEA RID: 7658
		private static SpreadAwardInfo _awardVipInfo = new SpreadAwardInfo();

		// Token: 0x04001DEB RID: 7659
		private static SpreadAwardInfo _awardVerifyInfo = new SpreadAwardInfo();

		// Token: 0x04001DEC RID: 7660
		private static DateTime _createDate = DateTime.MinValue;

		// Token: 0x04001DED RID: 7661
		private static int _vipCountMax = 0;

		// Token: 0x04001DEE RID: 7662
		private static int _levelCountMax = 0;

		// Token: 0x04001DEF RID: 7663
		private static int VIP_LEVEL_COUNT_MAX_DEFAULT = 50;
	}
}
