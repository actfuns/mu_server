using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class LogFilterConfig
	{
		
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

		
		public static bool LogGoodsIdLog(int goodsId)
		{
			HashSet<int> goodsIdHashSet = LogFilterConfig.NeedLogGoodsIdHashSet;
			return null == goodsIdHashSet || goodsIdHashSet.Contains(goodsId);
		}

		
		public static bool LogMoneyTypeLog(int moneyType)
		{
			HashSet<int> moneyTypeHashSet = LogFilterConfig.NeedLogMoneyTypeHashSet;
			return null == moneyTypeHashSet || moneyTypeHashSet.Contains(moneyType);
		}

		
		public static bool LogGameLog(int type)
		{
			HashSet<int> noLogGameHashSet = LogFilterConfig.NoLogGameHashSet;
			return null != noLogGameHashSet && !noLogGameHashSet.Contains(type);
		}

		
		public static bool LogOperatorLog(int type)
		{
			HashSet<int> noLogOperatorHashSet = LogFilterConfig.NoLogOperatorHashSet;
			return null != noLogOperatorHashSet && !noLogOperatorHashSet.Contains(type);
		}

		
		private static HashSet<int> NeedLogGoodsIdHashSet;

		
		private static HashSet<int> NeedLogMoneyTypeHashSet;

		
		private static HashSet<int> NoLogOperatorHashSet;

		
		private static HashSet<int> NoLogGameHashSet;
	}
}
