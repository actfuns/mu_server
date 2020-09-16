using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting.KFBoCai
{
	
	public class KFBoCaiConfigManager
	{
		
		public static int LoadConfig(bool isReload = true)
		{
			try
			{
				List<CaiShuZiConfig> _CaiShuZiCfgList;
				KFBoCaiConfigManager.LoadCaiShuZi(out _CaiShuZiCfgList);
				List<CaiDaXiaoConfig> _CaiDaXiaoCfgList;
				KFBoCaiConfigManager.LoadCaiDaXiao(out _CaiDaXiaoCfgList);
				List<DuiHuanShangChengConfig> _DuiHuanShangChengCgfList;
				KFBoCaiConfigManager.Load_DuiHuanShangCheng(out _DuiHuanShangChengCgfList);
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					KFBoCaiConfigManager.CaiShuZiCfgList = _CaiShuZiCfgList;
				}
				lock (KFBoCaiConfigManager.CaiDaXiaoCfgList)
				{
					KFBoCaiConfigManager.CaiDaXiaoCfgList = _CaiDaXiaoCfgList;
				}
				lock (KFBoCaiConfigManager.DuiHuanShangChengCgfList)
				{
					KFBoCaiConfigManager.DuiHuanShangChengCgfList = _DuiHuanShangChengCgfList;
				}
				if (isReload)
				{
					KFBoCaiCaiDaXiao.GetInstance().UpData(true);
					KFBoCaiCaiShuzi.GetInstance().UpData(true);
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		
		public static CaiShuZiConfig GetCaiShuZiConfig()
		{
			CaiShuZiConfig cfg = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					int maxLen = KFBoCaiConfigManager.CaiShuZiCfgList.Count;
					int index = 0;
					while (index < maxLen)
					{
						if (DateTime.Parse(KFBoCaiConfigManager.CaiShuZiCfgList[index].JieShuShiJian) < TimeUtil.NowDateTime())
						{
							KFBoCaiConfigManager.CaiShuZiCfgList.RemoveAt(index);
							maxLen = KFBoCaiConfigManager.CaiShuZiCfgList.Count;
						}
						else
						{
							if (cfg == null || DateTime.Parse(cfg.KaiQiShiJian) > DateTime.Parse(KFBoCaiConfigManager.CaiShuZiCfgList[index].KaiQiShiJian))
							{
								cfg = KFBoCaiConfigManager.CaiShuZiCfgList[index];
							}
							index++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return cfg;
		}

		
		public static CaiShuZiConfig GetCaiShuZiConfig(int ID)
		{
			CaiShuZiConfig cfg = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiShuZiCfgList)
				{
					cfg = KFBoCaiConfigManager.CaiShuZiCfgList.Find((CaiShuZiConfig x) => x.ID == ID);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return cfg;
		}

		
		public static CaiDaXiaoConfig GetCaiDaXiaoConfig()
		{
			CaiDaXiaoConfig cfg = null;
			try
			{
				lock (KFBoCaiConfigManager.CaiDaXiaoCfgList)
				{
					int maxLen = KFBoCaiConfigManager.CaiDaXiaoCfgList.Count;
					int index = 0;
					while (index < maxLen)
					{
						if (DateTime.Parse(KFBoCaiConfigManager.CaiDaXiaoCfgList[index].HuoDongJieSu) < TimeUtil.NowDateTime())
						{
							KFBoCaiConfigManager.CaiDaXiaoCfgList.RemoveAt(index);
							maxLen = KFBoCaiConfigManager.CaiDaXiaoCfgList.Count;
						}
						else
						{
							if (cfg == null || DateTime.Parse(cfg.HuoDongKaiQi) > DateTime.Parse(KFBoCaiConfigManager.CaiDaXiaoCfgList[index].HuoDongKaiQi))
							{
								cfg = KFBoCaiConfigManager.CaiDaXiaoCfgList[index];
							}
							index++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return cfg;
		}

		
		private static bool LoadCaiShuZi(out List<CaiShuZiConfig> _CaiShuZiCfgList)
		{
			_CaiShuZiCfgList = new List<CaiShuZiConfig>();
			try
			{
				XElement xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/CaiShuZi.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/CaiShuZi.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					CaiShuZiConfig myData = new CaiShuZiConfig();
					myData.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
					myData.XiaoHaoDaiBi = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "XiaoHaoDaiBi", 0L);
					myData.ChuFaBiZhong = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ChuFaBiZhong", 0L);
					myData.BuChongTiaoJian = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "BuChongTiaoJian", 0L);
					myData.XiTongChouCheng = ConfigHelper.GetElementAttributeValueDouble(xmlItem, "XiTongChouCheng", 0.0);
					myData.ShangChengKaiGuan = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ShangChengKaiGuan", 0L);
					myData.AnNiuList = new List<CaiShuZiAnNiu>();
					foreach (string item in ConfigHelper.GetElementAttributeValue(xmlItem, "ZhongJiangFanLi", "").Split(new char[]
					{
						'|'
					}))
					{
						string[] temp = item.Split(new char[]
						{
							','
						});
						if (temp.Length < 2)
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现 AnNiuName err, myData.ID={1}", "Config/CaiShuZi.xml", myData.ID), null, true);
						}
						else
						{
							CaiShuZiAnNiu d = new CaiShuZiAnNiu();
							d.NO = Convert.ToInt32(temp[0]);
							d.Percent = Convert.ToDouble(temp[1]);
							myData.AnNiuList.Add(d);
							if (d.NO != myData.AnNiuList.Count)
							{
								LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现 d.NO != myData.AnNiuList.Count  myData.ID={1}", "Config/CaiShuZi.xml", myData.ID), null, true);
							}
						}
					}
					if (myData.AnNiuList.Count < 3)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现 myData.AnNiuList.Count < 3 err, myData.ID={1}", "Config/CaiShuZi.xml", myData.ID), null, true);
					}
					else
					{
						myData.KaiQiShiJian = ConfigHelper.GetElementAttributeValue(xmlItem, "KaiQiShiJian", "");
						DateTime.Parse(myData.KaiQiShiJian);
						myData.JieShuShiJian = ConfigHelper.GetElementAttributeValue(xmlItem, "JieShuShiJian", "");
						DateTime.Parse(myData.JieShuShiJian);
						myData.KaiJiangShiJian = ConfigHelper.GetElementAttributeValue(xmlItem, "KaiJiangShiJian", "");
						DateTime.Parse(myData.KaiJiangShiJian);
						_CaiShuZiCfgList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/CaiShuZi.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private static bool Load_DuiHuanShangCheng(out List<DuiHuanShangChengConfig> _DuiHuanShangChengCgfList)
		{
			_DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();
			try
			{
				XElement xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/DuiHuanShangCheng.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/DuiHuanShangCheng.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					DuiHuanShangChengConfig myData = new DuiHuanShangChengConfig();
					myData.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
					myData.DaiBiJiaGe = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "DaiBiJiaGe", 0L);
					myData.MeiRiShangXianDan = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "MeiRiShangXianDan", 0L);
					myData.Name = ConfigHelper.GetElementAttributeValue(xmlItem, "Name", "");
					myData.WuPinID = ConfigHelper.GetElementAttributeValue(xmlItem, "WuPinID", "");
					_DuiHuanShangChengCgfList.Add(myData);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/DuiHuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private static bool LoadCaiDaXiao(out List<CaiDaXiaoConfig> _CaiDaXiaoCfgList)
		{
			_CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();
			try
			{
				XElement xml = ConfigHelper.Load(KuaFuServerManager.GetResourcePath("Config/CaiDaXiao.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/CaiDaXiao.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						CaiDaXiaoConfig myData = new CaiDaXiaoConfig();
						myData.ID = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ID", 0L);
						myData.XiaoHaoDaiBi = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "XiaoHaoDaiBi", 0L);
						myData.ShangChengKaiGuan = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ShangChengKaiGuan", 0L);
						myData.HuoDongKaiQi = ConfigHelper.GetElementAttributeValue(xmlItem, "HuoDongKaiQi", "");
						myData.ZhuShuShangXian = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ZhuShuShangXian", 0L);
						DateTime.Parse(myData.HuoDongKaiQi);
						myData.HuoDongJieSu = ConfigHelper.GetElementAttributeValue(xmlItem, "HuoDongJieSu", "");
						DateTime.Parse(myData.HuoDongJieSu);
						myData.MeiRiKaiQi = ConfigHelper.GetElementAttributeValue(xmlItem, "MeiRiKaiQi", "");
						DateTime.Parse(myData.MeiRiKaiQi);
						myData.MeiRiJieSu = ConfigHelper.GetElementAttributeValue(xmlItem, "MeiRiJieSu", "");
						DateTime.Parse(myData.MeiRiJieSu);
						_CaiDaXiaoCfgList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/CaiDaXiao.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private const string CaiShuZi = "Config/CaiShuZi.xml";

		
		private const string CaiDaXiao = "Config/CaiDaXiao.xml";

		
		private const string DuiHuanShangCheng = "Config/DuiHuanShangCheng.xml";

		
		private static List<CaiShuZiConfig> CaiShuZiCfgList = new List<CaiShuZiConfig>();

		
		private static List<CaiDaXiaoConfig> CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();

		
		private static List<DuiHuanShangChengConfig> DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();
	}
}
