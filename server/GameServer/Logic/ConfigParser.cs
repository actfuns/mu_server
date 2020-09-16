using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public static class ConfigParser
	{
		
		static ConfigParser()
		{
			for (ExtPropIndexes i = ExtPropIndexes.Strong; i < ExtPropIndexes.Max; i++)
			{
				ConfigParser.ExtPropName2ExtPropIndexDict[i.ToString()] = i;
			}
		}

		
		public static ExtPropIndexes GetPropIndexByPropName(string propName)
		{
			ExtPropIndexes propIndex;
			ExtPropIndexes result;
			if (ConfigParser.ExtPropName2ExtPropIndexDict.TryGetValue(propName.Trim(), out propIndex))
			{
				result = propIndex;
			}
			else
			{
				result = ExtPropIndexes.Max;
			}
			return result;
		}

		
		public static void ParseExtprops(double[] extProps, string str, string splitChars = "|,")
		{
			try
			{
				string[] valueFileds = str.Split(new char[]
				{
					splitChars[0]
				});
				foreach (string value in valueFileds)
				{
					string[] KvpFileds = value.Split(new char[]
					{
						','
					});
					if (KvpFileds.Length == 2)
					{
						ExtPropIndexes index = ConfigParser.GetPropIndexByPropName(KvpFileds[0]);
						if (index < ExtPropIndexes.Max)
						{
							extProps[(int)index] = Global.SafeConvertToDouble(KvpFileds[1]);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				throw ex;
			}
		}

		
		public static bool ParseStrInt2(string str, ref int v1, ref int v2, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] strArray = str.Split(new char[]
				{
					splitChar
				});
				int t;
				int t2;
				if (strArray.Length < 2 || !int.TryParse(strArray[0], out t) || !int.TryParse(strArray[1], out t2))
				{
					result = false;
				}
				else
				{
					v1 = t;
					v2 = t2;
					result = true;
				}
			}
			return result;
		}

		
		public static bool ParseStrInt3(string str, ref int v1, ref int v2, ref int v3, char splitChar = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				string[] strArray = str.Split(new char[]
				{
					splitChar
				});
				int t;
				int t2;
				int t3;
				if (strArray.Length < 3 || !int.TryParse(strArray[0], out t) || !int.TryParse(strArray[1], out t2) || !int.TryParse(strArray[2], out t3))
				{
					result = false;
				}
				else
				{
					v1 = t;
					v2 = t2;
					v3 = t3;
					result = true;
				}
			}
			return result;
		}

		
		public static bool ParserTimeRangeList(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string rangeItem in rangeStr)
				{
					string[] timeStr = rangeItem.Split(new char[]
					{
						splitChar2
					});
					if (timeStr.Length != 2)
					{
						return false;
					}
					TimeSpan time;
					TimeSpan time2;
					if (!TimeSpan.TryParse(timeStr[0], out time) || !TimeSpan.TryParse(timeStr[1], out time2))
					{
						return false;
					}
					list.Add(time);
					list.Add(time2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		
		public static bool ParserTimeRangeListWithDay(List<TimeSpan> list, string str, bool clear = true, char splitChar1 = '|', char splitChar2 = '-', char splitChar3 = ',')
		{
			bool result;
			if (string.IsNullOrEmpty(str))
			{
				result = false;
			}
			else
			{
				if (clear)
				{
					list.Clear();
				}
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string rangeItem in rangeStr)
				{
					string[] dayAndTimeStr = rangeItem.Split(new char[]
					{
						splitChar3
					});
					if (dayAndTimeStr.Length != 2)
					{
						return false;
					}
					int day;
					if (!int.TryParse(dayAndTimeStr[0], out day))
					{
						return false;
					}
					string[] timeStr = dayAndTimeStr[1].Split(new char[]
					{
						splitChar2
					});
					if (timeStr.Length != 2)
					{
						return false;
					}
					TimeSpan time;
					TimeSpan time2;
					if (!TimeSpan.TryParse(timeStr[0], out time) || !TimeSpan.TryParse(timeStr[1], out time2))
					{
						return false;
					}
					TimeSpan dayPart = new TimeSpan(day, 0, 0, 0);
					time = time.Add(dayPart);
					time2 = time2.Add(dayPart);
					list.Add(time);
					list.Add(time2);
				}
				result = (list.Count > 0);
			}
			return result;
		}

		
		public static List<List<int>> ParserIntArrayList(string str, bool verifyColumn = true, char splitChar1 = '|', char splitChar2 = ',')
		{
			List<List<int>> list = new List<List<int>>();
			List<List<int>> result;
			if (string.IsNullOrEmpty(str))
			{
				result = list;
			}
			else
			{
				string[] rangeStr = str.Split(new char[]
				{
					splitChar1
				});
				int maxColumnCount = -1;
				foreach (string rangeItem in rangeStr)
				{
					List<int> ls = new List<int>();
					if (!string.IsNullOrEmpty(rangeItem))
					{
						string[] arr = rangeItem.Split(new char[]
						{
							splitChar2
						});
						foreach (string s in arr)
						{
							int v;
							if (int.TryParse(s, out v))
							{
								ls.Add(v);
							}
						}
					}
					list.Add(ls);
					if (verifyColumn)
					{
						if (maxColumnCount < 0)
						{
							maxColumnCount = ls.Count;
							if (maxColumnCount == 0)
							{
								break;
							}
						}
						else if (maxColumnCount != ls.Count)
						{
							list.Clear();
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}

		
		public static EquipPropItem ParseEquipPropItem(string str, bool verifyColumn = true, char splitChar1 = '|', char splitChar2 = ',', char splitChar3 = '-')
		{
			EquipPropItem equipPropItem = new EquipPropItem();
			if (!string.IsNullOrEmpty(str))
			{
				string[] propertyConfigArray = str.Split(new char[]
				{
					splitChar1
				});
				foreach (string propertyConfigItem in propertyConfigArray)
				{
					string[] nameValueArray = propertyConfigItem.Split(new char[]
					{
						splitChar2
					});
					if (nameValueArray.Length == 2)
					{
						ExtPropIndexes propIndex = ConfigParser.GetPropIndexByPropName(nameValueArray[0]);
						if (propIndex < ExtPropIndexes.Max)
						{
							double propValue;
							if (double.TryParse(nameValueArray[1], out propValue))
							{
								equipPropItem.ExtProps[(int)propIndex] = propValue;
							}
						}
						else
						{
							int propIndex2 = -1;
							int propIndex3 = -1;
							string text = nameValueArray[0];
							if (text != null)
							{
								if (!(text == "Attack"))
								{
									if (!(text == "Mattack"))
									{
										if (!(text == "Defense"))
										{
											if (text == "Mdefense")
											{
												propIndex2 = 5;
												propIndex3 = 6;
											}
										}
										else
										{
											propIndex2 = 3;
											propIndex3 = 4;
										}
									}
									else
									{
										propIndex2 = 9;
										propIndex3 = 10;
									}
								}
								else
								{
									propIndex2 = 7;
									propIndex3 = 8;
								}
							}
							string[] valueArray = nameValueArray[1].Split(new char[]
							{
								splitChar3
							});
							double propValue;
							if (propIndex2 >= 0 && double.TryParse(valueArray[0], out propValue))
							{
								equipPropItem.ExtProps[propIndex2] = propValue;
							}
							if (propIndex3 >= 0 && double.TryParse(valueArray[1], out propValue))
							{
								equipPropItem.ExtProps[propIndex3] = propValue;
							}
						}
					}
				}
			}
			return equipPropItem;
		}

		
		public static void ParseAwardsItemList(string str, ref AwardsItemList awardsItemList, char splitChar1 = '|', char splitChar2 = ',')
		{
			awardsItemList.Add(str);
		}

		
		public static void ParseAwardsGoodsList(string str, List<GoodsData> goodsDataList, char splitChar1 = '|', char splitChar2 = ',')
		{
			AwardsItemList awardsItemList = new AwardsItemList();
			awardsItemList.Add(str);
		}

		
		public static double[] ParserExtPropsFromAttrubite(XElement xml, List<KeyValuePair<int, string>> list)
		{
			double[] extProps = new double[177];
			foreach (KeyValuePair<int, string> kv in list)
			{
				extProps[kv.Key] = ConfigHelper.GetElementAttributeValueDouble(xml, kv.Value, 0.0);
			}
			return extProps;
		}

		
		private static Dictionary<string, ExtPropIndexes> ExtPropName2ExtPropIndexDict = new Dictionary<string, ExtPropIndexes>(StringComparer.OrdinalIgnoreCase);
	}
}
