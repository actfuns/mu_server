using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x02000051 RID: 81
	public class SpecPlatFuLiManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		// Token: 0x060000F1 RID: 241 RVA: 0x000101E4 File Offset: 0x0000E3E4
		public static SpecPlatFuLiManager getInstance()
		{
			return SpecPlatFuLiManager.instance;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000101FC File Offset: 0x0000E3FC
		public bool initialize()
		{
			this.evQueryHandlerDict[SpecialPlatformType.UC.ToString()] = new SpecPlatFuLiManager.SpecHandler(this._Query_Handle_UC);
			this.evExcuteHandlerDict[SpecialPlatformType.UC.ToString()] = new SpecPlatFuLiManager.SpecHandler(this._Excute_Handle_UC);
			return this.InitConfig();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00010268 File Offset: 0x0000E468
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(692, 3, 3, SpecPlatFuLiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(693, 3, 3, SpecPlatFuLiManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000102AC File Offset: 0x0000E4AC
		public bool showdown()
		{
			return true;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000102C0 File Offset: 0x0000E4C0
		public bool destroy()
		{
			return true;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000102D4 File Offset: 0x0000E4D4
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000102E8 File Offset: 0x0000E4E8
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 692:
				result = this.ProcessQueryCmd(client, nID, bytes, cmdParams);
				break;
			case 693:
				result = this.ProcessExcuteCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00010330 File Offset: 0x0000E530
		public bool ProcessQueryCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client == null || client.ClientSocket.IsKuaFuLogin)
				{
					return true;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				string platType = cmdParams[1];
				string launch = cmdParams[2];
				string result = "0";
				SpecPlatFuLiManager.SpecHandler queryHandler;
				if (this.evQueryHandlerDict.TryGetValue(platType.ToUpper(), out queryHandler))
				{
					result = queryHandler(client, launch);
				}
				client.ClientData.platType = platType;
				client.ClientData.launch = launch;
				client.sendCmd(nID, string.Format("{0}", result), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00010408 File Offset: 0x0000E608
		public bool ProcessExcuteCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client == null || client.ClientSocket.IsKuaFuLogin)
				{
					return true;
				}
				int roleID = Convert.ToInt32(cmdParams[0]);
				string platType = cmdParams[1];
				string param = cmdParams[2];
				string result = "";
				SpecPlatFuLiManager.SpecHandler excuteHandler;
				if (this.evExcuteHandlerDict.TryGetValue(platType.ToUpper(), out excuteHandler))
				{
					result = excuteHandler(client, param);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", result, param), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x000104C8 File Offset: 0x0000E6C8
		public void OnNewDay(GameClient client)
		{
			if (client != null && !client.ClientSocket.IsKuaFuLogin)
			{
				if (string.Compare(client.ClientData.platType, SpecialPlatformType.UC.ToString(), true) == 0)
				{
					client.sendCmd(692, string.Format("{0}", 0), false);
				}
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00010538 File Offset: 0x0000E738
		private string _Query_Handle_UC(GameClient client, string param)
		{
			string result = "0";
			string result2;
			if (!this.AliGiftsSwitch)
			{
				result2 = result;
			}
			else
			{
				int launch = Global.SafeConvertToInt32(param);
				DateTime now = TimeUtil.NowDateTime();
				DateTime startTime = new DateTime(now.Year, now.Month, now.Day);
				DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
				string beginStr = startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string endStr = endTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keyFuLiStr = string.Format("fuli_{0}_{1}_{2}", beginStr, endStr, SpecialPlatformType.UC.ToString());
				string keyLoginStr = string.Format("login_{0}_{1}_{2}", beginStr, endStr, SpecialPlatformType.UC.ToString());
				string[] dbResult = Global.QeuryUserActivityInfo(client, keyFuLiStr, 1000, "0");
				if (dbResult == null || dbResult.Length == 0)
				{
					result2 = result;
				}
				else
				{
					int hasGetTimes = Global.SafeConvertToInt32(dbResult[3]);
					if (hasGetTimes > 0)
					{
						result2 = result;
					}
					else
					{
						if (launch <= 0)
						{
							dbResult = Global.QeuryUserActivityInfo(client, keyLoginStr, 1000, "0");
							if (dbResult == null || dbResult.Length == 0)
							{
								return result;
							}
							launch = Global.SafeConvertToInt32(dbResult[3]);
							if (launch <= 0)
							{
								return result;
							}
						}
						else
						{
							Global.UpdateUserActivityInfo(client, keyLoginStr, 1000, (long)launch, now.ToString("yyyy-MM-dd HH$mm$ss"));
						}
						result = "1";
						result2 = result;
					}
				}
			}
			return result2;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000106E4 File Offset: 0x0000E8E4
		private string _Excute_Handle_UC(GameClient client, string param)
		{
			int result = 0;
			PlatFuLiBaseData fuliBaseData;
			lock (this.ConfigMutex)
			{
				this.PlatFuLiBaseDataDict.TryGetValue(SpecialPlatformType.UC.ToString(), out fuliBaseData);
			}
			string result2;
			if (fuliBaseData == null || !this.AliGiftsSwitch)
			{
				result = -3;
				result2 = string.Format("{0}", result);
			}
			else if (!Global.CanAddGoodsDataList(client, ((UCPlatFuLiData)fuliBaseData).myAwardItem.GoodsDataList))
			{
				result = -100;
				result2 = string.Format("{0}", result);
			}
			else
			{
				DateTime now = TimeUtil.NowDateTime();
				DateTime startTime = new DateTime(now.Year, now.Month, now.Day);
				DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
				string beginStr = startTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string endStr = endTime.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$');
				string keyFuLiStr = string.Format("fuli_{0}_{1}_{2}", beginStr, endStr, SpecialPlatformType.UC.ToString());
				string[] dbResult = Global.QeuryUserActivityInfo(client, keyFuLiStr, 1000, "0");
				if (dbResult == null || dbResult.Length == 0)
				{
					result = -15;
					result2 = string.Format("{0}", result);
				}
				else
				{
					int hasGetTimes = Global.SafeConvertToInt32(dbResult[3]);
					if (hasGetTimes > 0)
					{
						result = -200;
						result2 = string.Format("{0}", result);
					}
					else
					{
						HuodongCachingMgr.GiveAward(client, fuliBaseData.myAwardItem, "UC平台福利");
						hasGetTimes = 1;
						Global.UpdateUserActivityInfo(client, keyFuLiStr, 1000, (long)hasGetTimes, now.ToString("yyyy-MM-dd HH$mm$ss"));
						result2 = string.Format("{0}", result);
					}
				}
			}
			return result2;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00010900 File Offset: 0x0000EB00
		public bool InitConfig()
		{
			lock (this.ConfigMutex)
			{
				UCPlatFuLiData myData = new UCPlatFuLiData();
				string goodsIDs = GameManager.systemParamsList.GetParamValueByName("AliGifts");
				if (!string.IsNullOrEmpty(goodsIDs))
				{
					string[] fields = goodsIDs.Split(new char[]
					{
						'|'
					});
					if (fields.Length > 0)
					{
						myData.myAwardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "UC平台福利奖励");
					}
				}
				this.PlatFuLiBaseDataDict[SpecialPlatformType.UC.ToString()] = myData;
				this.AliGiftsSwitch = (GameManager.systemParamsList.GetParamValueIntByName("AliGiftsSwitch", 1) > 0L);
			}
			return true;
		}

		// Token: 0x040001B6 RID: 438
		private object ConfigMutex = new object();

		// Token: 0x040001B7 RID: 439
		private Dictionary<string, SpecPlatFuLiManager.SpecHandler> evQueryHandlerDict = new Dictionary<string, SpecPlatFuLiManager.SpecHandler>();

		// Token: 0x040001B8 RID: 440
		private Dictionary<string, SpecPlatFuLiManager.SpecHandler> evExcuteHandlerDict = new Dictionary<string, SpecPlatFuLiManager.SpecHandler>();

		// Token: 0x040001B9 RID: 441
		private Dictionary<string, PlatFuLiBaseData> PlatFuLiBaseDataDict = new Dictionary<string, PlatFuLiBaseData>();

		// Token: 0x040001BA RID: 442
		private bool AliGiftsSwitch = true;

		// Token: 0x040001BB RID: 443
		private static SpecPlatFuLiManager instance = new SpecPlatFuLiManager();

		// Token: 0x02000052 RID: 82
		
		public delegate string SpecHandler(GameClient client, string param);
	}
}
