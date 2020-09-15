using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	// Token: 0x020001A3 RID: 419
	public class JieRiFuLiActivity : Activity
	{
		// Token: 0x060004E8 RID: 1256 RVA: 0x00042F08 File Offset: 0x00041108
		public bool Init()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(this.FuLiCfgFile));
				XElement xml = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(this.FuLiCfgFile));
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("加载{0}时出错!!!文件不存在", this.FuLiCfgFile), null, true);
					return false;
				}
				XElement args = xml.Element("Activities");
				if (null != args)
				{
					this.FromDate = Global.GetSafeAttributeStr(args, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(args, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(args, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(args, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(args, "AwardEndDate");
				}
				args = xml.Element("GiftList");
				foreach (XElement fuliXml in args.Elements())
				{
					JieRiFuLiItem item = new JieRiFuLiItem();
					item.Type = (EJieRiFuLiType)Global.GetSafeAttributeLong(fuliXml, "TypeID");
					item.Open = (int)Global.GetSafeAttributeLong(fuliXml, "Button");
					item.StartDate = Global.GetSafeAttributeStr(fuliXml, "AwardStartDate");
					item.EndDate = Global.GetSafeAttributeStr(fuliXml, "AwardEndDate");
					string szArg = Global.GetSafeAttributeStr(fuliXml, "Function");
					if (item.Type == EJieRiFuLiType.CallPetReplace)
					{
						item.Arg = Convert.ToInt32(szArg);
					}
					else if (item.Type == EJieRiFuLiType.SoulStoneExtFunc)
					{
						string[] fields = szArg.Split(new char[]
						{
							'|'
						});
						List<Tuple<int, int>> argList = new List<Tuple<int, int>>();
						for (int i = 0; i < fields.Length; i++)
						{
							string[] fields2 = fields[i].Split(new char[]
							{
								','
							});
							argList.Add(new Tuple<int, int>(Convert.ToInt32(fields2[0]), Convert.ToInt32(fields2[1])));
						}
						item.Arg = argList;
					}
					else if (item.Type == EJieRiFuLiType.OneDiscountDiamond)
					{
						string[] fields = szArg.Split(new char[]
						{
							'|'
						});
						List<double> argList2 = new List<double>();
						for (int i = 0; i < fields.Length; i++)
						{
							argList2.Add(Convert.ToDouble(fields[i]));
						}
						item.Arg = argList2;
					}
					else
					{
						item.Arg = szArg;
					}
					this.fuliDict.Add(item.Type, item);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("{0}解析出现异常, {1}", this.FuLiCfgFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x00043244 File Offset: 0x00041444
		public bool IsOpened(EJieRiFuLiType type, out object arg)
		{
			arg = null;
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieRiFuLiItem item = null;
				if (!this.fuliDict.TryGetValue(type, out item))
				{
					result = false;
				}
				else
				{
					DateTime startTime = DateTime.Parse(item.StartDate);
					DateTime endTime = DateTime.Parse(item.EndDate);
					if (TimeUtil.NowDateTime() < startTime || TimeUtil.NowDateTime() > endTime)
					{
						result = false;
					}
					else if (item.Open != 1)
					{
						result = false;
					}
					else
					{
						arg = item.Arg;
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x000432E0 File Offset: 0x000414E0
		public bool IsOpened(EJieRiFuLiType type)
		{
			bool result;
			if (!this.InActivityTime())
			{
				result = false;
			}
			else
			{
				JieRiFuLiItem item = null;
				if (!this.fuliDict.TryGetValue(type, out item))
				{
					result = false;
				}
				else
				{
					DateTime startTime = DateTime.Parse(item.StartDate);
					DateTime endTime = DateTime.Parse(item.EndDate);
					result = (!(TimeUtil.NowDateTime() < startTime) && !(TimeUtil.NowDateTime() > endTime) && item.Open == 1);
				}
			}
			return result;
		}

		// Token: 0x0400094F RID: 2383
		private readonly string FuLiCfgFile = "Config/JieRiGifts/JieRiFuLi.xml";

		// Token: 0x04000950 RID: 2384
		private Dictionary<EJieRiFuLiType, JieRiFuLiItem> fuliDict = new Dictionary<EJieRiFuLiType, JieRiFuLiItem>();
	}
}
