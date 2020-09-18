using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Tools;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.BocaiSys
{
	
	public class BoCaiConfigMgr
	{
		
		public static int LoadConfig(bool isReload = true)
		{
			try
			{
				List<CaiShuZiConfig> _CaiShuZiCfgList;
				BoCaiConfigMgr.LoadCaiShuZi(out _CaiShuZiCfgList);
				List<CaiDaXiaoConfig> _CaiDaXiaoCfgList;
				BoCaiConfigMgr.LoadCaiDaXiao(out _CaiDaXiaoCfgList);
				List<BoCaiConfigMgr.DaiBiShiYongData> _DaiBiShiYongCfgList;
				BoCaiConfigMgr.LoadDaiBiShiYong(out _DaiBiShiYongCfgList);
				List<DuiHuanShangChengConfig> _DuiHuanShangChengCgfList;
				BoCaiConfigMgr.Load_DuiHuanShangCheng(out _DuiHuanShangChengCgfList);
				lock (BoCaiConfigMgr.CaiShuZiCfgList)
				{
					BoCaiConfigMgr.CaiShuZiCfgList = _CaiShuZiCfgList;
				}
				lock (BoCaiConfigMgr.CaiDaXiaoCfgList)
				{
					BoCaiConfigMgr.CaiDaXiaoCfgList = _CaiDaXiaoCfgList;
				}
				lock (BoCaiConfigMgr.DaiBiShiYongCfgList)
				{
					BoCaiConfigMgr.DaiBiShiYongCfgList = _DaiBiShiYongCfgList;
				}
				lock (BoCaiConfigMgr.DuiHuanShangChengCgfList)
				{
					BoCaiConfigMgr.DuiHuanShangChengCgfList = _DuiHuanShangChengCgfList;
				}
				if (isReload)
				{
					BoCaiCaiDaXiao.GetInstance().BigTimeUpData(true);
					BoCaiCaiShuZi.GetInstance().BigTimeUpData(true);
				}
				return 1;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return 0;
		}

		
		public static DuiHuanShangChengConfig GetBoCaiShopConfig(int ID, string WuPinID)
		{
			DuiHuanShangChengConfig cfg = null;
			try
			{
				cfg = BoCaiConfigMgr.DuiHuanShangChengCgfList.Find((DuiHuanShangChengConfig x) => x.ID == ID && x.WuPinID == WuPinID);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return cfg;
		}

		
		public static bool CanReplaceMoney(DaiBiSySType type)
		{
			try
			{
				BoCaiConfigMgr.DaiBiShiYongData cfg = BoCaiConfigMgr.DaiBiShiYongCfgList.Find((BoCaiConfigMgr.DaiBiShiYongData x) => x.XiTongMingCheng.Equals(type.ToString()));
				if (null != cfg)
				{
					return cfg.IsOpen;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl]{0}", ex.ToString()), null, true);
			}
			return false;
		}

		
		private static bool LoadCaiShuZi(out List<CaiShuZiConfig> _CaiShuZiCfgList)
		{
			_CaiShuZiCfgList = new List<CaiShuZiConfig>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/CaiShuZi.xml"), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/CaiShuZi.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						CaiShuZiConfig myData = new CaiShuZiConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.XiaoHaoDaiBi = (int)Global.GetSafeAttributeLong(xmlItem, "XiaoHaoDaiBi");
						myData.ChuFaBiZhong = (int)Global.GetSafeAttributeLong(xmlItem, "ChuFaBiZhong");
						myData.BuChongTiaoJian = (int)Global.GetSafeAttributeLong(xmlItem, "BuChongTiaoJian");
						myData.XiTongChouCheng = Global.GetSafeAttributeDouble(xmlItem, "XiTongChouCheng");
						myData.ShangChengKaiGuan = (int)Global.GetSafeAttributeLong(xmlItem, "ShangChengKaiGuan");
						myData.AnNiuList = new List<CaiShuZiAnNiu>();
						foreach (string item in Global.GetSafeAttributeStr(xmlItem, "ZhongJiangFanLi").Split(new char[]
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
							myData.JieShuShiJian = Global.GetSafeAttributeStr(xmlItem, "JieShuShiJian");
							myData.KaiQiShiJian = Global.GetSafeAttributeStr(xmlItem, "KaiQiShiJian");
							DateTime.Parse(myData.KaiQiShiJian);
							myData.KaiJiangShiJian = Global.GetSafeAttributeStr(xmlItem, "KaiJiangShiJian");
							DateTime.Parse(myData.KaiJiangShiJian);
							_CaiShuZiCfgList.Add(myData);
						}
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
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/DuiHuanShangCheng.xml"), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/DuiHuanShangCheng.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						DuiHuanShangChengConfig myData = new DuiHuanShangChengConfig();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.DaiBiJiaGe = (int)Global.GetSafeAttributeLong(xmlItem, "DaiBiJiaGe");
						myData.MeiRiShangXianDan = (int)Global.GetSafeAttributeLong(xmlItem, "MeiRiShangXianDan");
						myData.Name = Global.GetSafeAttributeStr(xmlItem, "Name");
						myData.WuPinID = Global.GetSafeAttributeStr(xmlItem, "WuPinID");
						if (null == GlobalNew.ParseGoodsData(myData.WuPinID))
						{
							LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析 WuPinID={1} err", "Config/DuiHuanShangCheng.xml", myData.WuPinID), null, true);
						}
						else
						{
							_DuiHuanShangChengCgfList.Add(myData);
						}
					}
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
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/CaiDaXiao.xml"), true);
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
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.XiaoHaoDaiBi = (int)Global.GetSafeAttributeLong(xmlItem, "XiaoHaoDaiBi");
						myData.ShangChengKaiGuan = (int)Global.GetSafeAttributeLong(xmlItem, "ShangChengKaiGuan");
						myData.ZhuShuShangXian = (int)Global.GetSafeAttributeLong(xmlItem, "ZhuShuShangXian");
						myData.HuoDongJieSu = Global.GetSafeAttributeStr(xmlItem, "HuoDongJieSu");
						myData.HuoDongKaiQi = Global.GetSafeAttributeStr(xmlItem, "HuoDongKaiQi");
						DateTime.Parse(myData.HuoDongKaiQi);
						myData.MeiRiKaiQi = Global.GetSafeAttributeStr(xmlItem, "MeiRiKaiQi");
						DateTime.Parse(myData.MeiRiKaiQi);
						myData.MeiRiJieSu = Global.GetSafeAttributeStr(xmlItem, "MeiRiJieSu");
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

		
		private static bool LoadDaiBiShiYong(out List<BoCaiConfigMgr.DaiBiShiYongData> _DaiBiShiYongCfgList)
		{
			_DaiBiShiYongCfgList = new List<BoCaiConfigMgr.DaiBiShiYongData>();
			try
			{
				XElement xml = CheckHelper.LoadXml(Global.GameResPath("Config/DaiBiShiYong.xml"), true);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("读取 {0} null == xml", "Config/DaiBiShiYong.xml"), null, true);
					return false;
				}
				IEnumerable<XElement> xmlItems = xml.Elements();
				foreach (XElement xmlItem in xmlItems)
				{
					if (null != xmlItem)
					{
						BoCaiConfigMgr.DaiBiShiYongData myData = new BoCaiConfigMgr.DaiBiShiYongData();
						myData.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						myData.IsOpen = ((int)Global.GetSafeAttributeLong(xmlItem, "DaiBiKaiGuan") > 0);
						myData.XiTongMingCheng = Global.GetSafeAttributeStr(xmlItem, "XiTongMingCheng").Trim();
						myData.ZhongWenMingCheng = Global.GetSafeAttributeStr(xmlItem, "ZhongWenMingCheng");
						_DaiBiShiYongCfgList.Add(myData);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", "Config/DuiHuanShangCheng.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		
		private const string CaiShuZi = "Config/CaiShuZi.xml";

		
		private const string CaiDaXiao = "Config/CaiDaXiao.xml";

		
		private const string DaiBiShiYong = "Config/DaiBiShiYong.xml";

		
		private const string DuiHuanShangCheng = "Config/DuiHuanShangCheng.xml";

		
		public const string StrHuanLeDuiHuanOpen = "HuanLeDuiHuan";

		
		private static List<CaiShuZiConfig> CaiShuZiCfgList = new List<CaiShuZiConfig>();

		
		private static List<CaiDaXiaoConfig> CaiDaXiaoCfgList = new List<CaiDaXiaoConfig>();

		
		private static List<BoCaiConfigMgr.DaiBiShiYongData> DaiBiShiYongCfgList = new List<BoCaiConfigMgr.DaiBiShiYongData>();

		
		private static List<DuiHuanShangChengConfig> DuiHuanShangChengCgfList = new List<DuiHuanShangChengConfig>();

		
		public class DaiBiShiYongData
		{
			
			public int ID;

			
			public bool IsOpen;

			
			public string XiTongMingCheng;

			
			public string ZhongWenMingCheng;
		}
	}
}
