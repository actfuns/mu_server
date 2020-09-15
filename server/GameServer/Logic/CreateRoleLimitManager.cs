using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x02000243 RID: 579
	public class CreateRoleLimitManager : SingletonTemplate<CreateRoleLimitManager>
	{
		// Token: 0x060007F5 RID: 2037 RVA: 0x0007961C File Offset: 0x0007781C
		public bool IfCanCreateRole(string UserID, string UserName, string DeviceID, string IP, out int NotifyLeftTime)
		{
			NotifyLeftTime = 0;
			LimitResultData CheckData = new LimitResultData();
			if (!this._UserWhiteList.Contains(UserID.ToLower()))
			{
				this.CheckByDeviceID(UserID, UserName, DeviceID, CheckData);
				if (CheckData.CanCreate)
				{
					this.CheckByIP(UserID, UserName, IP, CheckData);
					if (!CheckData.CanCreate)
					{
					}
				}
			}
			if (!CheckData.CanCreate)
			{
				NotifyLeftTime = this.CaculateNextAvailableTime(CheckData);
			}
			return CheckData.CanCreate;
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x000796A4 File Offset: 0x000778A4
		public void ModifyCreateRoleNum(string UserID, string UserName, string DeviceID, string IP)
		{
			lock (this.DeviceIDLimitData)
			{
				if (-1 != this.DeviceIDRestrictNum && !string.IsNullOrEmpty(DeviceID))
				{
					this.ModifyTotalNum(this.DeviceIDLimitData, DeviceID);
				}
			}
			lock (this.IPLimitData)
			{
				if (-1 != this.IPRestrictNum && !string.IsNullOrEmpty(IP))
				{
					this.ModifyTotalNum(this.IPLimitData, IP);
				}
			}
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x00079768 File Offset: 0x00077968
		private int CaculateNextAvailableTime(LimitResultData CheckData)
		{
			int result;
			if (CheckData.CanCreate)
			{
				result = 0;
			}
			else
			{
				DateTime nextDayTime = CheckData.AnalysisDataTime.AddMinutes((double)this.CreateRoleLimitMinutes);
				DateTime Now = TimeUtil.NowDateTime();
				result = (int)(nextDayTime - Now).TotalSeconds;
			}
			return result;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x000797B8 File Offset: 0x000779B8
		private void ModifyTotalNum(LinkedList<LimitAnalysisData> list, string key)
		{
			Dictionary<string, int> DataNow = this.GetHourAnalysisData(list);
			if (null != DataNow)
			{
				int CountNum = 0;
				if (!DataNow.TryGetValue(key, out CountNum))
				{
					DataNow.Add(key, 1);
				}
				else
				{
					DataNow[key] = CountNum + 1;
				}
			}
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x00079804 File Offset: 0x00077A04
		private int ComputeTotalNum(LinkedList<LimitAnalysisData> list, string key, LimitResultData CheckData)
		{
			int result = 0;
			int result2;
			if (list.Count == 0)
			{
				result2 = result;
			}
			else
			{
				this.DoHouseKeepingForAnalysisData(list);
				foreach (LimitAnalysisData data in list)
				{
					int count = 0;
					if (data.dict.TryGetValue(key, out count))
					{
						result += count;
					}
				}
				if (list.Count != 0)
				{
					CheckData.AnalysisDataTime = list.First.Value.Timestamp;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x000798C4 File Offset: 0x00077AC4
		private Dictionary<string, int> GetHourAnalysisData(LinkedList<LimitAnalysisData> list)
		{
			DateTime Now = TimeUtil.NowDateTime();
			if (list.Count == 0 || this.WorkDateTime.Hour != Now.Hour)
			{
				this.WorkDateTime = Now;
				list.AddLast(new LimitAnalysisData());
			}
			return list.Last.Value.dict;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00079928 File Offset: 0x00077B28
		private void DoHouseKeepingForAnalysisData(LinkedList<LimitAnalysisData> list)
		{
			if (list != null && list.Count != 0)
			{
				DateTime Now = TimeUtil.NowDateTime();
				LimitAnalysisData oldData = list.First.Value;
				int SpanMinutes = (int)(Now - oldData.Timestamp).TotalMinutes;
				if (SpanMinutes >= this.CreateRoleLimitMinutes)
				{
					list.RemoveFirst();
				}
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00079990 File Offset: 0x00077B90
		private void CheckByDeviceID(string UserID, string UserName, string DeviceID, LimitResultData CheckData)
		{
			if (-1 != this.DeviceIDRestrictNum && !string.IsNullOrEmpty(DeviceID))
			{
				lock (this.DeviceIDLimitData)
				{
					int CountNum = this.ComputeTotalNum(this.DeviceIDLimitData, DeviceID, CheckData);
					if (CountNum >= this.DeviceIDRestrictNum)
					{
						CheckData.CanCreate = false;
						LogManager.WriteLog(LogTypes.Error, string.Format("玩家创建角色被限制, UserID={0}, UserName={1}, DeviceID={2}, CountNum={3}", new object[]
						{
							UserID,
							UserName,
							DeviceID,
							CountNum
						}), null, true);
					}
				}
			}
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00079A54 File Offset: 0x00077C54
		private bool IfIPInWhiteList(string IP)
		{
			List<IPWhiteList> MyIPWhiteList = null;
			lock (this)
			{
				MyIPWhiteList = this._IPWhiteList;
			}
			bool result;
			if (MyIPWhiteList == null || MyIPWhiteList.Count == 0)
			{
				result = false;
			}
			else
			{
				IPAddress IPAdd = IPAddress.Parse(IP);
				if (IPAdd == null)
				{
					result = false;
				}
				else
				{
					byte[] byteMyIP = IPAdd.GetAddressBytes();
					uint myIP = (uint)((int)byteMyIP[0] << 24 | (int)byteMyIP[1] << 16 | (int)byteMyIP[2] << 8 | (int)byteMyIP[3]);
					foreach (IPWhiteList data in MyIPWhiteList)
					{
						if (data.MinIP <= myIP && data.MaxIP >= myIP)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00079B68 File Offset: 0x00077D68
		private void CheckByIP(string UserID, string UserName, string IP, LimitResultData CheckData)
		{
			if (-1 != this.IPRestrictNum && !string.IsNullOrEmpty(IP))
			{
				if (!this.IfIPInWhiteList(IP))
				{
					lock (this.IPLimitData)
					{
						int CountNum = this.ComputeTotalNum(this.IPLimitData, IP, CheckData);
						if (CountNum >= this.IPRestrictNum)
						{
							CheckData.CanCreate = false;
							LogManager.WriteLog(LogTypes.Error, string.Format("玩家创建角色被限制, UserID={0}, UserName={1}, IP={2}, CountNum={3}", new object[]
							{
								UserID,
								UserName,
								IP,
								CountNum
							}), null, true);
						}
					}
				}
			}
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00079C40 File Offset: 0x00077E40
		public void LoadConfig()
		{
			string strDeviceIDRestrict = GameManager.systemParamsList.GetParamValueByName("DeviceIDRestrict");
			if (!string.IsNullOrEmpty(strDeviceIDRestrict))
			{
				this.DeviceIDRestrictNum = Global.SafeConvertToInt32(strDeviceIDRestrict);
			}
			string strIPRestrict = GameManager.systemParamsList.GetParamValueByName("IPRestrict");
			if (!string.IsNullOrEmpty(strIPRestrict))
			{
				this.IPRestrictNum = Global.SafeConvertToInt32(strIPRestrict);
			}
			string strResetBagSlotTicks = GameManager.systemParamsList.GetParamValueByName("BagClearUpCD");
			if (!string.IsNullOrEmpty(strResetBagSlotTicks))
			{
				this.ResetBagSlotTicks = Global.SafeConvertToInt32(strResetBagSlotTicks);
			}
			string strRefreshMarketSlotTicks = GameManager.systemParamsList.GetParamValueByName("RefreshBourseCD");
			if (!string.IsNullOrEmpty(strRefreshMarketSlotTicks))
			{
				this.RefreshMarketSlotTicks = Global.SafeConvertToInt32(strRefreshMarketSlotTicks);
			}
			string strAddSlotTicks = GameManager.systemParamsList.GetParamValueByName("AddFriendCD");
			if (!string.IsNullOrEmpty(strAddSlotTicks))
			{
				this.AddFriendSlotTicks = Global.SafeConvertToInt32(strAddSlotTicks);
				this.AddBHMemberSlotTicks = Global.SafeConvertToInt32(strAddSlotTicks);
			}
			string strSpriteFightSlotTicks = GameManager.systemParamsList.GetParamValueByName("SpiritPutOnCD");
			if (!string.IsNullOrEmpty(strSpriteFightSlotTicks))
			{
				this.SpriteFightSlotTicks = Global.SafeConvertToInt32(strSpriteFightSlotTicks);
			}
			string strDeleteRoleNeedTime = GameManager.systemParamsList.GetParamValueByName("DeleteRoleNeedTime");
			GameManager.GameConfigMgr.SetGameConfigItem("DeleteRoleNeedTime", strDeleteRoleNeedTime);
			Global.UpdateDBGameConfigg("DeleteRoleNeedTime", strDeleteRoleNeedTime);
			lock (this)
			{
				this.LoadIPWhiteList();
				this.LoadUserWhiteList();
			}
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00079DD4 File Offset: 0x00077FD4
		private void LoadIPWhiteList()
		{
			try
			{
				XElement xml = XElement.Load(Global.IsolateResPath("Config/IPWhiteList.xml"));
				if (null != xml)
				{
					List<IPWhiteList> NewIPWhiteList = new List<IPWhiteList>();
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						IPWhiteList value = new IPWhiteList();
						value.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						IPAddress MinIP = IPAddress.Parse(Global.GetSafeAttributeStr(xmlItem, "MinIP"));
						byte[] byteMinIP = MinIP.GetAddressBytes();
						value.MinIP = (uint)((int)byteMinIP[0] << 24 | (int)byteMinIP[1] << 16 | (int)byteMinIP[2] << 8 | (int)byteMinIP[3]);
						IPAddress MaxIP = IPAddress.Parse(Global.GetSafeAttributeStr(xmlItem, "MaxIP"));
						byte[] byteMaxIP = MaxIP.GetAddressBytes();
						value.MaxIP = (uint)((int)byteMaxIP[0] << 24 | (int)byteMaxIP[1] << 16 | (int)byteMaxIP[2] << 8 | (int)byteMaxIP[3]);
						NewIPWhiteList.Add(value);
					}
					this._IPWhiteList = NewIPWhiteList;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPWhiteList.xml");
			}
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00079F4C File Offset: 0x0007814C
		private void LoadUserWhiteList()
		{
			try
			{
				XElement xmlFile = ConfigHelper.Load(Global.IsolateResPath("Config/UserWhiteList.xml"));
				if (null != xmlFile)
				{
					HashSet<string> NewUserWhiteList = new HashSet<string>();
					IEnumerable<XElement> xmlItems = ConfigHelper.GetXElements(xmlFile, "WhiteList");
					foreach (XElement xml in xmlItems)
					{
						string platform = ConfigHelper.GetElementAttributeValue(xml, "PinTai", "");
						if (0 == string.Compare(platform, GameCoreInterface.getinstance().GetPlatformType().ToString(), true))
						{
							string userId = ConfigHelper.GetElementAttributeValue(xml, "UserID", "");
							if (!string.IsNullOrEmpty(userId))
							{
								NewUserWhiteList.Add(userId.ToLower());
							}
						}
					}
					this._UserWhiteList = NewUserWhiteList;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPWhiteList.xml");
			}
		}

		// Token: 0x04000DCA RID: 3530
		private const string IPWhiteListfileName = "Config/IPWhiteList.xml";

		// Token: 0x04000DCB RID: 3531
		private const string UserWhiteListfileName = "Config/UserWhiteList.xml";

		// Token: 0x04000DCC RID: 3532
		public int ResetBagSlotTicks = 0;

		// Token: 0x04000DCD RID: 3533
		public int RefreshMarketSlotTicks = 0;

		// Token: 0x04000DCE RID: 3534
		public int SpriteFightSlotTicks = 0;

		// Token: 0x04000DCF RID: 3535
		public int AddBHMemberSlotTicks = 0;

		// Token: 0x04000DD0 RID: 3536
		public int AddFriendSlotTicks = 0;

		// Token: 0x04000DD1 RID: 3537
		public int CreateRoleLimitMinutes = 1440;

		// Token: 0x04000DD2 RID: 3538
		private int DeviceIDRestrictNum = -1;

		// Token: 0x04000DD3 RID: 3539
		private int IPRestrictNum = -1;

		// Token: 0x04000DD4 RID: 3540
		private DateTime WorkDateTime = TimeUtil.NowDateTime();

		// Token: 0x04000DD5 RID: 3541
		private List<IPWhiteList> _IPWhiteList = new List<IPWhiteList>();

		// Token: 0x04000DD6 RID: 3542
		private HashSet<string> _UserWhiteList = new HashSet<string>();

		// Token: 0x04000DD7 RID: 3543
		private LinkedList<LimitAnalysisData> DeviceIDLimitData = new LinkedList<LimitAnalysisData>();

		// Token: 0x04000DD8 RID: 3544
		private LinkedList<LimitAnalysisData> IPLimitData = new LinkedList<LimitAnalysisData>();
	}
}
