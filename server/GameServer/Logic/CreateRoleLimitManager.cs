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
	
	public class CreateRoleLimitManager : SingletonTemplate<CreateRoleLimitManager>
	{
		
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

		
		private const string IPWhiteListfileName = "Config/IPWhiteList.xml";

		
		private const string UserWhiteListfileName = "Config/UserWhiteList.xml";

		
		public int ResetBagSlotTicks = 0;

		
		public int RefreshMarketSlotTicks = 0;

		
		public int SpriteFightSlotTicks = 0;

		
		public int AddBHMemberSlotTicks = 0;

		
		public int AddFriendSlotTicks = 0;

		
		public int CreateRoleLimitMinutes = 1440;

		
		private int DeviceIDRestrictNum = -1;

		
		private int IPRestrictNum = -1;

		
		private DateTime WorkDateTime = TimeUtil.NowDateTime();

		
		private List<IPWhiteList> _IPWhiteList = new List<IPWhiteList>();

		
		private HashSet<string> _UserWhiteList = new HashSet<string>();

		
		private LinkedList<LimitAnalysisData> DeviceIDLimitData = new LinkedList<LimitAnalysisData>();

		
		private LinkedList<LimitAnalysisData> IPLimitData = new LinkedList<LimitAnalysisData>();
	}
}
