using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class PlatConfig
	{
		
		public void LoadPlatConfig()
		{
			string filePath = Global.GameResPath(this.fileName);
			this._PlatConfigNormalDict = new Dictionary<string, string>();
			this._PlatConfigWaitingDict = new Dictionary<int, WaitingConfig>();
			this._PlatConfigTradeLevelLimitList = new List<TradeLevelLimitConfig>();
			this._PlatConfigChatLevelLimitDic = new Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>>();
			try
			{
				XElement xml = XElement.Load(filePath);
				this.LoadNormalConfig(xml, this._PlatConfigNormalDict);
				this.LoadWaitingConfig(xml, this._PlatConfigWaitingDict);
				this.LoadTradeLevelLimitConfig(xml, this._PlatConfigTradeLevelLimitList);
				this.LoadChatLevelLimitConfig(xml, this._PlatConfigChatLevelLimitDic);
				this._PlatConfigTradeLimitConfigDict = this.LoadTradeLimitsConfig(xml);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, "区域平台特殊配置文件加载失败" + filePath + "\r\n" + ex.ToString(), ex, false);
			}
		}

		
		public int ReloadPlatConfig()
		{
			Dictionary<string, string> normalDict = new Dictionary<string, string>();
			Dictionary<int, WaitingConfig> waitingDict = new Dictionary<int, WaitingConfig>();
			List<TradeLevelLimitConfig> tradeLevelLimitList = new List<TradeLevelLimitConfig>();
			Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> chatLevelLimitDic = new Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath(this.fileName));
				this.LoadNormalConfig(xml, normalDict);
				this.LoadWaitingConfig(xml, waitingDict);
				this.LoadTradeLevelLimitConfig(xml, tradeLevelLimitList);
				this.LoadChatLevelLimitConfig(xml, chatLevelLimitDic);
				this._PlatConfigTradeLimitConfigDict = this.LoadTradeLimitsConfig(xml);
			}
			catch (Exception e)
			{
				LogManager.WriteException("重新加载配置文件 PlatConfig.xml  失败！！！" + e.ToString());
				return -1;
			}
			lock (this._PlatConfigNormalDict)
			{
				this._PlatConfigNormalDict = normalDict;
			}
			lock (this._PlatConfigWaitingDict)
			{
				this._PlatConfigWaitingDict = waitingDict;
			}
			lock (this._PlatConfigTradeLevelLimitList)
			{
				this._PlatConfigTradeLevelLimitList = tradeLevelLimitList;
			}
			lock (this._PlatConfigChatLevelLimitDic)
			{
				this._PlatConfigChatLevelLimitDic = chatLevelLimitDic;
			}
			TCPSession.SetMaxPosCmdNumPer5Seconds(8);
			GameManager.loginWaitLogic.LoadConfig();
			return 0;
		}

		
		private void LoadNormalConfig(XElement xml, Dictionary<string, string> normalDict)
		{
			lock (normalDict)
			{
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "DCLogs").Element("DCLog");
					normalDict.Add("tw_log_pid", Global.GetSafeAttributeStr(xmlEle, "pid"));
					normalDict.Add("tw_log_path", Global.GetSafeAttributeStr(xmlEle, "path"));
					normalDict.Add("tw_log_head", Global.GetSafeAttributeStr(xmlEle, "logHead"));
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "DCLog") + e.ToString());
				}
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "GuestTradeLevelLimits").Element("GuestTradeLevelLimit");
					normalDict.Add("GuestTradeLevelLimit", Global.GetSafeAttributeStr(xmlEle, "Limit"));
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "DCLog") + e.ToString());
				}
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "FileBans").Element("FileBanPros");
					normalDict.Add("fileban_hour", Global.GetSafeAttributeStr(xmlEle, "FileBanHour"));
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "FileBanPros") + e.ToString());
				}
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "Speeds").Element("Speed");
					normalDict.Add("ban-speed-up-minutes2", Global.GetSafeAttributeStr(xmlEle, "BanMins"));
					normalDict.Add("maxposcmdnum", Global.GetSafeAttributeStr(xmlEle, "MaxPosCmdNum"));
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "Speed") + e.ToString());
				}
				try
				{
					IEnumerable<XElement> list = xml.DescendantsAndSelf("addSettings");
					foreach (XElement settings in list)
					{
						foreach (XElement item in settings.DescendantsAndSelf("add"))
						{
							normalDict[Global.GetSafeAttributeStr(item, "key")] = Global.GetSafeAttributeStr(item, "value");
						}
					}
					list = xml.DescendantsAndSelf("addSettings");
					foreach (XElement settings in list)
					{
						if (0 == string.Compare(ConfigHelper.GetElementAttributeValue(settings, "platfromtype", ""), GameManager.PlatformType.ToString(), true))
						{
							foreach (XElement item in settings.DescendantsAndSelf("add"))
							{
								normalDict[Global.GetSafeAttributeStr(item, "key")] = Global.GetSafeAttributeStr(item, "value");
							}
						}
					}
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "addSettings") + e.ToString());
				}
				foreach (KeyValuePair<string, string> kv in normalDict)
				{
					if (this.SyncDBConfigNames.Contains(kv.Key) && !string.IsNullOrEmpty(kv.Value))
					{
						GameManager.GameConfigMgr.UpdateGameConfigItem(kv.Key, kv.Value, false);
					}
				}
			}
		}

		
		public string GetGameConfigItemStr(string paramName, string defVal)
		{
			string retStr = GameManager.GameConfigMgr.GetGameConifgItem(paramName);
			if (retStr == null)
			{
				if (paramName.Equals("trade_level_limit"))
				{
					retStr = this.GetPlatTradeLevelLimitConfig(paramName);
				}
				else if (paramName.Equals("userwaitconfig") || paramName.Equals("vipwaitconfig") || paramName.Equals("loginallow_vipexp"))
				{
					retStr = this.GetWaitingConfig(paramName);
				}
				else if (paramName.Equals("chat_world_level") || paramName.Equals("chat_family_level") || paramName.Equals("chat_team_level") || paramName.Equals("chat_private_level") || paramName.Equals("chat_near_level"))
				{
					retStr = this.GetPlatChatLevelLimitConfig(paramName);
				}
				else
				{
					retStr = this.GetNormalConfig(paramName);
				}
			}
			return (!string.IsNullOrEmpty(retStr)) ? retStr : defVal;
		}

		
		public int GetGameConfigItemInt(string paramName, int defVal)
		{
			string retStr = GameManager.GameConfigMgr.GetGameConifgItem(paramName);
			int retInt = 0;
			if (null == retStr)
			{
				if (paramName.Equals("trade_level_limit"))
				{
					retStr = this.GetPlatTradeLevelLimitConfig(paramName);
				}
				else if (paramName.Equals("userwaitconfig") || paramName.Equals("vipwaitconfig") || paramName.Equals("loginallow_vipexp"))
				{
					retStr = this.GetWaitingConfig(paramName);
				}
				else if (paramName.Equals("chat_world_level") || paramName.Equals("chat_family_level") || paramName.Equals("chat_team_level") || paramName.Equals("chat_private_level") || paramName.Equals("chat_near_level"))
				{
					retStr = this.GetPlatChatLevelLimitConfig(paramName);
				}
				else
				{
					retStr = this.GetNormalConfig(paramName);
				}
			}
			try
			{
				if (string.IsNullOrEmpty(retStr))
				{
					return defVal;
				}
				retInt = Convert.ToInt32(retStr);
			}
			catch (Exception e)
			{
				return defVal;
			}
			return retInt;
		}

		
		private string GetNormalConfig(string paramName)
		{
			string paramValue = null;
			lock (this._PlatConfigNormalDict)
			{
				if (!this._PlatConfigNormalDict.TryGetValue(paramName, out paramValue))
				{
					paramValue = null;
				}
			}
			return paramValue;
		}

		
		private string GetWaitingConfig(string paramName)
		{
			WaitingConfig waitingConfig = null;
			lock (this._PlatConfigWaitingDict)
			{
				if (!this._PlatConfigWaitingDict.TryGetValue(GameManager.ServerId, out waitingConfig))
				{
					this._PlatConfigWaitingDict.TryGetValue(0, out waitingConfig);
				}
			}
			if (waitingConfig != null)
			{
				if (paramName.Equals("userwaitconfig"))
				{
					return waitingConfig.UserWaitConfig;
				}
				if (paramName.Equals("vipwaitconfig"))
				{
					return waitingConfig.VIPWaitConfig;
				}
				if (paramName.Equals("loginallow_vipexp"))
				{
					return waitingConfig.LoginAllow_VIPExp.ToString();
				}
			}
			return null;
		}

		
		private string GetPlatTradeLevelLimitConfig(string paramName)
		{
			string str = null;
			IEnumerable<TradeLevelLimitConfig> query = null;
			lock (this._PlatConfigTradeLevelLimitList)
			{
				query = from items in this._PlatConfigTradeLevelLimitList
				orderby items.Day
				select items;
			}
			DateTime t = TimeUtil.NowDateTime();
			DateTime t2 = Global.GetKaiFuTime();
			int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), t2, true);
			foreach (TradeLevelLimitConfig v in query)
			{
				str = v.Limit;
				if (elapsedDays <= v.Day)
				{
					break;
				}
			}
			return str;
		}

		
		public TradeLimitConfig GetTradeLimitConfig()
		{
			if (null != this._PlatConfigTradeLimitConfigDict)
			{
				string platform = GameCoreInterface.getinstance().GetPlatformType().ToString().ToLower();
				lock (this._PlatConfigTradeLimitConfigDict)
				{
					TradeLimitConfig config;
					if (this._PlatConfigTradeLimitConfigDict.TryGetValue(platform, out config))
					{
						return config;
					}
				}
			}
			return null;
		}

		
		public bool CanTrade(DateTime now, int realMoney, int level)
		{
			bool result;
			if (null == this._PlatConfigTradeLimitConfigDict)
			{
				result = true;
			}
			else
			{
				string platform = GameCoreInterface.getinstance().GetPlatformType().ToString().ToLower();
				lock (this._PlatConfigTradeLimitConfigDict)
				{
					TradeLimitConfig config;
					if (!this._PlatConfigTradeLimitConfigDict.TryGetValue(platform, out config))
					{
						result = true;
					}
					else if (config.ZuanShiOpen != 1)
					{
						result = false;
					}
					else if (now < config.StartTime || now > config.EndTime)
					{
						result = true;
					}
					else if (realMoney >= config.ZuanShiLimit || level >= config.LevelLimit)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		
		private string GetPlatChatLevelLimitConfig(string paramName)
		{
			string str = null;
			ChatTypeIndexes keyName = this.GetChatKeyName(paramName);
			List<ChatLevelLimitConfig> chatConfigList = null;
			lock (this._PlatConfigChatLevelLimitDic)
			{
				chatConfigList = this._PlatConfigChatLevelLimitDic[keyName];
			}
			string result;
			if (chatConfigList == null)
			{
				result = null;
			}
			else
			{
				IOrderedEnumerable<ChatLevelLimitConfig> query = from items in chatConfigList
				orderby items.Day
				select items;
				DateTime t = TimeUtil.NowDateTime();
				DateTime t2 = Global.GetKaiFuTime();
				int elapsedDays = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), t2, true);
				foreach (ChatLevelLimitConfig v in query)
				{
					str = v.Limit;
					if (elapsedDays <= v.Day)
					{
						break;
					}
				}
				try
				{
					string[] szLevelLimit = str.Split(new char[]
					{
						','
					});
					int minChangeLife = Convert.ToInt32(szLevelLimit[0]);
					int minLevel = Convert.ToInt32(szLevelLimit[1]);
					str = (minChangeLife * 100 + minLevel).ToString();
				}
				catch (Exception e)
				{
					return null;
				}
				result = str;
			}
			return result;
		}

		
		private void LoadWaitingConfig(XElement xml, Dictionary<int, WaitingConfig> waitingDict)
		{
			lock (waitingDict)
			{
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "Waiting");
					IEnumerable<XElement> waitingNodes = xmlEle.Elements();
					foreach (XElement xmlNode in waitingNodes)
					{
						WaitingConfig waitingConfig = new WaitingConfig();
						string severID = Global.GetSafeAttributeStr(xmlNode, "ID");
						waitingConfig.SeverID = Convert.ToInt32(severID);
						int[] NeedWaitNumberArr = Global.GetSafeAttributeIntArray(xmlNode, "NeedWaitNumber", 2, ',');
						waitingConfig.NormalNeedWaitNumber = NeedWaitNumberArr[0];
						waitingConfig.VIPNeedWaitNumber = NeedWaitNumberArr[1];
						int[] MaxNumber = Global.GetSafeAttributeIntArray(xmlNode, "MaxNumber", 2, ',');
						waitingConfig.NormalMaxNumber = MaxNumber[0];
						waitingConfig.VIPMaxNumber = MaxNumber[1];
						int[] WaitingMaxNumber = Global.GetSafeAttributeIntArray(xmlNode, "WaitingMaxNumber", 2, ',');
						waitingConfig.NormalWaitingMaxNumber = WaitingMaxNumber[0];
						waitingConfig.VIPWaitingMaxNumber = WaitingMaxNumber[1];
						int[] EnterMinInt = Global.GetSafeAttributeIntArray(xmlNode, "EnterMinInt", 2, ',');
						waitingConfig.NormalEnterMinInt = EnterMinInt[0];
						waitingConfig.VIPEnterMinInt = EnterMinInt[1];
						int[] AllowMSecs = Global.GetSafeAttributeIntArray(xmlNode, "AllowMSecs", 2, ',');
						waitingConfig.NormalAllowMSecs = AllowMSecs[0];
						waitingConfig.VIPAllowMSecs = AllowMSecs[1];
						int[] LogoutAllowMSecs = Global.GetSafeAttributeIntArray(xmlNode, "LogoutAllowMSecs", 2, ',');
						waitingConfig.NormalLogoutAllowMSecs = LogoutAllowMSecs[0];
						waitingConfig.VIPLogoutAllowMSecs = LogoutAllowMSecs[1];
						waitingConfig.VipExp = Convert.ToInt32(Global.GetSafeAttributeStr(xmlNode, "vipexp"));
						waitingDict.Add(waitingConfig.SeverID, waitingConfig);
					}
					if (!waitingDict.ContainsKey(0))
					{
						throw new Exception(string.Format("配置文件 {0} 可能没有配置 {1} 项或者没有默认配置项，请正确配置后重新加载文件。", this.fileName, "waiting"));
					}
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！! !  {2}", this.fileName, "Waiting", e.ToString()));
				}
			}
		}

		
		private void LoadTradeLevelLimitConfig(XElement xml, List<TradeLevelLimitConfig> tradeLevelLimitList)
		{
			lock (tradeLevelLimitList)
			{
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "TradeLevelLimit");
					IEnumerable<XElement> waitingNodes = xmlEle.Elements();
					foreach (XElement xmlNode in waitingNodes)
					{
						TradeLevelLimitConfig tradeLevelLimitConfig = new TradeLevelLimitConfig();
						string ID = Global.GetSafeAttributeStr(xmlNode, "ID");
						string Day = Global.GetSafeAttributeStr(xmlNode, "Day");
						string Limit = Global.GetSafeAttributeStr(xmlNode, "Limit");
						tradeLevelLimitConfig.ID = Convert.ToInt32(ID);
						tradeLevelLimitConfig.Day = Convert.ToInt32(Day);
						tradeLevelLimitConfig.Limit = Limit;
						tradeLevelLimitList.Add(tradeLevelLimitConfig);
					}
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "TradeLevelLimit", e.ToString()));
				}
			}
		}

		
		private Dictionary<string, TradeLimitConfig> LoadTradeLimitsConfig(XElement xml)
		{
			Dictionary<string, TradeLimitConfig> dict = new Dictionary<string, TradeLimitConfig>();
			try
			{
				XElement xmlEle = Global.GetSafeXElement(xml, "TradeLimits");
				IEnumerable<XElement> waitingNodes = xmlEle.Elements();
				foreach (XElement xmlNode in waitingNodes)
				{
					TradeLimitConfig tradeLimit = new TradeLimitConfig();
					tradeLimit.Platform = Global.GetSafeAttributeStr(xmlNode, "Platform");
					dict[tradeLimit.Platform.ToLower()] = tradeLimit;
					int[] arr = Global.GetSafeAttributeIntArray(xmlNode, "LevelLimit", -1, ',');
					tradeLimit.LevelLimit = Global.GetUnionLevel(arr[0], arr[1], false);
					tradeLimit.ZuanShiLimit = (int)Global.GetSafeAttributeLong(xmlNode, "ZuanShiLimit");
					tradeLimit.ZuanShiOpen = (int)Global.GetSafeAttributeLong(xmlNode, "ZuanShiOpen");
					tradeLimit.Message = Global.GetSafeAttributeStr(xmlNode, "Message");
					if (!DateTime.TryParse(Global.GetSafeAttributeStr(xmlNode, "BeginTime"), out tradeLimit.StartTime))
					{
						tradeLimit.StartTime = DateTime.MinValue;
					}
					if (!DateTime.TryParse(Global.GetSafeAttributeStr(xmlNode, "EndTime"), out tradeLimit.EndTime))
					{
						tradeLimit.EndTime = DateTime.MinValue;
					}
				}
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "TradeLevelLimit", e.ToString()));
			}
			return dict;
		}

		
		private ChatTypeIndexes GetChatKeyName(string name)
		{
			ChatTypeIndexes keyName = ChatTypeIndexes.Max;
			if (name.Equals("WorldChats") || name.Equals("chat_world_level"))
			{
				keyName = ChatTypeIndexes.World;
			}
			else if (name.Equals("FamilyChats") || name.Equals("chat_family_level"))
			{
				keyName = ChatTypeIndexes.Faction;
			}
			else if (name.Equals("TeamChats") || name.Equals("chat_team_level"))
			{
				keyName = ChatTypeIndexes.Team;
			}
			else if (name.Equals("PrivateChats") || name.Equals("chat_private_level"))
			{
				keyName = ChatTypeIndexes.Private;
			}
			else if (name.Equals("NearChats") || name.Equals("chat_near_level"))
			{
				keyName = ChatTypeIndexes.Map;
			}
			return keyName;
		}

		
		private void LoadChatLevelLimitConfig(XElement xml, Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> chatLevelLimitDic)
		{
			lock (chatLevelLimitDic)
			{
				try
				{
					XElement xmlEle = Global.GetSafeXElement(xml, "Chats");
					IEnumerable<XElement> chatsNodes = xmlEle.Elements();
					foreach (XElement xmlNode in chatsNodes)
					{
						ChatTypeIndexes keyName = this.GetChatKeyName(xmlNode.Name.LocalName);
						if (keyName == ChatTypeIndexes.Max)
						{
							throw new InvalidDataException(string.Format("{0} 不在聊天类型中 ！！！", xmlNode.Name.LocalName));
						}
						List<ChatLevelLimitConfig> ChatLevelLimitConfigList = new List<ChatLevelLimitConfig>();
						chatLevelLimitDic[keyName] = ChatLevelLimitConfigList;
						foreach (XElement xmlSubNode in xmlNode.Elements())
						{
							ChatLevelLimitConfig chatLevelLimitConfig = new ChatLevelLimitConfig();
							string ID = Global.GetSafeAttributeStr(xmlSubNode, "ID");
							string Day = Global.GetSafeAttributeStr(xmlSubNode, "Day");
							string Limit = Global.GetSafeAttributeStr(xmlSubNode, "Limit");
							chatLevelLimitConfig.ID = Convert.ToInt32(ID);
							chatLevelLimitConfig.Day = Convert.ToInt32(Day);
							chatLevelLimitConfig.Limit = Limit;
							ChatLevelLimitConfigList.Add(chatLevelLimitConfig);
						}
					}
				}
				catch (Exception e)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "Chats", e.ToString()));
				}
			}
		}

		
		private Dictionary<string, string> _PlatConfigNormalDict = null;

		
		private Dictionary<int, WaitingConfig> _PlatConfigWaitingDict = null;

		
		private List<TradeLevelLimitConfig> _PlatConfigTradeLevelLimitList = null;

		
		private Dictionary<string, TradeLimitConfig> _PlatConfigTradeLimitConfigDict = new Dictionary<string, TradeLimitConfig>();

		
		private Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> _PlatConfigChatLevelLimitDic = null;

		
		private string fileName = string.Format("Config/PlatConfig.xml", new object[0]);

		
		private HashSet<string> SyncDBConfigNames = new HashSet<string>
		{
			"lipinma_v1"
		};
	}
}
