using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	// Token: 0x020002AE RID: 686
	public class LogFilterConfig
	{
		// Token: 0x06000A19 RID: 2585 RVA: 0x000A18DC File Offset: 0x0009FADC
		public static bool InitConfig()
		{
			HashSet<int> goodsIdHashSet = new HashSet<int>();
			HashSet<int> moneyTypeHashSet = new HashSet<int>();
			HashSet<int> noLogGameHashSet = new HashSet<int>();
			HashSet<int> noLogOperatorHashSet = new HashSet<int>();
			string filePath = null;
			try
			{
				filePath = Global.IsolateResPath("config\\Monitoring.xml");
				XElement xmlFile = ConfigHelper.Load(filePath);
				IEnumerable<XElement> xmls = ConfigHelper.GetXElements(xmlFile, "Monitoring");
				if (null == xmls)
				{
					LogManager.WriteLog(LogTypes.Fatal, string.Format("未找到配置文件{0},请联系策划负责人获取", filePath), null, true);
					return false;
				}
				foreach (XElement xml in xmls)
				{
					int type = (int)ConfigHelper.GetElementAttributeValueLong(xml, "Type", 0L);
					int code = (int)ConfigHelper.GetElementAttributeValueLong(xml, "Code", 0L);
					if (type == 1)
					{
						goodsIdHashSet.Add(code);
					}
					else if (type == 2)
					{
						moneyTypeHashSet.Add(code);
					}
					else if (type == 3)
					{
						noLogGameHashSet.Add(code);
					}
					else if (type == 4)
					{
						noLogOperatorHashSet.Add(code);
					}
					else
					{
						LogManager.WriteLog(LogTypes.Warning, string.Format("警告：配置文件{0},配置了未定义的类型!,{1}", filePath, xml.ToString()), null, true);
					}
				}
				LogFilterConfig.NeedLogGoodsIdHashSet = goodsIdHashSet;
				LogFilterConfig.NeedLogMoneyTypeHashSet = moneyTypeHashSet;
				LogFilterConfig.NoLogGameHashSet = noLogGameHashSet;
				LogFilterConfig.NoLogOperatorHashSet = noLogOperatorHashSet;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Fatal, string.Format("读取配置{0}发生失败,{1}", filePath, ex.Message), ex, true);
				return false;
			}
			return true;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x000A1ACC File Offset: 0x0009FCCC
		public static bool LogGoodsIdLog(int goodsId)
		{
			HashSet<int> goodsIdHashSet = LogFilterConfig.NeedLogGoodsIdHashSet;
			return null == goodsIdHashSet || goodsIdHashSet.Contains(goodsId);
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x000A1AF8 File Offset: 0x0009FCF8
		public static bool LogMoneyTypeLog(int moneyType)
		{
			HashSet<int> moneyTypeHashSet = LogFilterConfig.NeedLogMoneyTypeHashSet;
			return null == moneyTypeHashSet || moneyTypeHashSet.Contains(moneyType);
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x000A1B24 File Offset: 0x0009FD24
		public static bool LogGameLog(int type)
		{
			HashSet<int> noLogGameHashSet = LogFilterConfig.NoLogGameHashSet;
			return null != noLogGameHashSet && !noLogGameHashSet.Contains(type);
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x000A1B54 File Offset: 0x0009FD54
		public static bool LogOperatorLog(int type)
		{
			HashSet<int> noLogOperatorHashSet = LogFilterConfig.NoLogOperatorHashSet;
			return null != noLogOperatorHashSet && !noLogOperatorHashSet.Contains(type);
		}

		// Token: 0x04001183 RID: 4483
		private static HashSet<int> NeedLogGoodsIdHashSet;

		// Token: 0x04001184 RID: 4484
		private static HashSet<int> NeedLogMoneyTypeHashSet;

		// Token: 0x04001185 RID: 4485
		private static HashSet<int> NoLogOperatorHashSet;

		// Token: 0x04001186 RID: 4486
		private static HashSet<int> NoLogGameHashSet;
	}
}
