using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CheckSysValueDll;
using Server.Tools;

namespace GameServer.Tools.CheckSysValue
{
	
	public class CheckSysValueHelper
	{
		
		public static void WriteMap(string cmd = null)
		{
			try
			{
				CheckSysValueHelper.WriteLine("画图时间较长......", ConsoleColor.Green);
				RelationMapModel.WriteMap(Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				CheckSysValueHelper.WriteLine("失败", ConsoleColor.Green);
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_WriteMap]{0}", ex.ToString()), null, true);
			}
		}

		
		private static void WriteLine(string str, ConsoleColor Color = ConsoleColor.Green)
		{
			Console.ForegroundColor = Color;
			SysConOut.WriteLine(str);
			Console.ForegroundColor = ConsoleColor.White;
		}

		
		private static void _setSeachMgr(ref GetValueModel model)
		{
			while (string.IsNullOrEmpty(model.TypeName))
			{
				CheckSysValueHelper.WriteLine("【0】直接输完整数据类型； 【其它模糊输入】； 【q：退出】", ConsoleColor.Red);
				string cmd = Console.ReadLine();
				if (!cmd.Equals("0"))
				{
					if (!cmd.Equals("q"))
					{
						CheckSysValueHelper.WriteLine("请输入模糊类型名", ConsoleColor.Red);
						cmd = Console.ReadLine();
						cmd = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeachType(cmd), "类型名模糊结果", model);
						if (!cmd.Equals("q"))
						{
							model.TypeName = cmd;
							continue;
						}
					}
					return;
				}
				CheckSysValueHelper.WriteLine("请直接输入类型名", ConsoleColor.Red);
				model.TypeName = Console.ReadLine().Trim();
			}
			CheckSysValueHelper._setSeach(model);
			List<string> attrList = RelationMapModel.GetSeachAttr(model.TypeName);
			while (string.IsNullOrEmpty(model.SeachName))
			{
				CheckSysValueHelper.WriteLine("一共有" + attrList.Count + "条属性，【0】查看全部， 【其它】模糊查询属性，【q：退出】", ConsoleColor.Red);
				string cmd = Console.ReadLine();
				if (cmd.Equals("0"))
				{
					cmd = CheckSysValueHelper._setResult(attrList, "可查询属性", model);
				}
				else
				{
					if (cmd.Equals("q"))
					{
						return;
					}
					CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
					cmd = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), attrList), "可查询属性", model);
				}
				model.SeachName = cmd;
			}
			CheckSysValueHelper._setSeach(model);
		}

		
		private static void _setSeach(GetValueModel model)
		{
			if (null != model)
			{
				if (!string.IsNullOrEmpty(model.TypeName))
				{
					CheckSysValueHelper.WriteLine("选择 TypeName =[" + model.TypeName + "]", ConsoleColor.Yellow);
					if (string.IsNullOrEmpty(model.SeachName))
					{
						CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
					}
					else
					{
						CheckSysValueHelper.WriteLine("选择 SeachName =[" + model.SeachName + "]", ConsoleColor.Yellow);
						if (model.SeachDataList.Count < 1)
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
						}
						else
						{
							CheckSysValueHelper.WriteLine("选择筛选列表", ConsoleColor.Red);
							foreach (SeachData item in model.SeachDataList)
							{
								CheckSysValueHelper.WriteLine("筛选AttName=" + item.AttName, ConsoleColor.Yellow);
								CheckSysValueHelper.WriteLine("筛选SeachVal=" + item.SeachVal, ConsoleColor.Yellow);
								CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							}
						}
					}
				}
			}
		}

		
		private static string _getSeachKey(GetValueModel model)
		{
			string strKey = string.Format("{0},{1}", model.TypeName, model.SeachName);
			foreach (SeachData item in model.SeachDataList)
			{
				strKey += item.AttName;
			}
			return strKey;
		}

		
		private static string _setResult(List<string> dlist, string str, GetValueModel model)
		{
			int MAXLEN = 0;
			foreach (string item in dlist)
			{
				MAXLEN = ((item.Length > MAXLEN) ? item.Length : MAXLEN);
			}
			string key = CheckSysValueHelper._getSeachKey(model);
			CheckSysValueHelper.WriteLine(string.Concat(new object[]
			{
				str,
				" 一共",
				dlist.Count,
				"条，每次展示20条,【q：退出】"
			}), ConsoleColor.Red);
			int index = 0;
			try
			{
				while (index <= dlist.Count)
				{
					string log = "{1, -6}{0, " + -MAXLEN + " },";
					log = string.Format(log, dlist[index], "【" + index + "】,");
					string _key = key + dlist[index];
					if (CheckSysValueHelper.AttrTypeDict.ContainsKey(_key))
					{
						log += CheckSysValueHelper.AttrTypeDict[_key];
					}
					CheckSysValueHelper.WriteLine(log, ConsoleColor.Green);
					index++;
					if (index >= dlist.Count)
					{
						CheckSysValueHelper.WriteLine("已经最后一条 ，【确认选择直接输入序号】，【任意键重输入】， 【q：退出】", ConsoleColor.Red);
						string cmd = Console.ReadLine();
						if (cmd.Equals("q"))
						{
							return "q";
						}
						if (!string.IsNullOrEmpty(cmd))
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							return dlist[Convert.ToInt32(cmd)];
						}
						return "";
					}
					else if (index % 20 == 0)
					{
						CheckSysValueHelper.WriteLine(" 一共" + dlist.Count + "条，【确认选择直接输入序号】，【回车继续显示】,【任意键重输入】， 【q：退出】", ConsoleColor.Red);
						string cmd = Console.ReadLine();
						if (cmd.Equals("q"))
						{
							return "q";
						}
						if (!string.IsNullOrEmpty(cmd))
						{
							CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
							return dlist[Convert.ToInt32(cmd)];
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
			CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
			return "";
		}

		
		private static void _AddSeach(ref GetValueModel model)
		{
			SeachData seach = new SeachData();
			List<string> dlist;
			if (CheckSysValueHelper.AttrDict.TryGetValue(CheckSysValueHelper._getSeachKey(model), out dlist))
			{
				if (dlist.Count > 0)
				{
					while (string.IsNullOrEmpty(seach.AttName))
					{
						CheckSysValueHelper.WriteLine("一共有" + dlist.Count + "条属性，【0】查看全部， 【q：退出】 【其它】模糊查询属性", ConsoleColor.Red);
						string cmd = Console.ReadLine();
						if (cmd.Equals("0"))
						{
							cmd = CheckSysValueHelper._setResult(dlist, "可查询属性", model);
						}
						else
						{
							if (cmd.Equals("q"))
							{
								return;
							}
							CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
							cmd = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), dlist), "可查询属性", model);
						}
						if (!cmd.Equals("q"))
						{
							seach.AttName = cmd;
						}
					}
					CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
					seach.SeachVal = Console.ReadLine();
					model.SeachDataList.Add(seach);
					return;
				}
			}
			CheckSysValueHelper.WriteLine("无缓存记录 输入要查询的属性名", ConsoleColor.Red);
			seach.AttName = Console.ReadLine();
			CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
			seach.SeachVal = Console.ReadLine();
			model.SeachDataList.Add(seach);
		}

		
		private static bool _SetCheck(GetValueModel model)
		{
			CheckValueResult result = CheckModel.GetValue(model, Assembly.GetExecutingAssembly(), true);
			bool result2;
			if (string.IsNullOrEmpty(result.Info))
			{
				CheckSysValueHelper.WriteLine("查询成功", ConsoleColor.Red);
				int len = 0;
				string key = CheckSysValueHelper._getSeachKey(model);
				if (result.ResultDict.Count > 0)
				{
					if (CheckSysValueHelper.AttrDict.ContainsKey(key))
					{
						CheckSysValueHelper.AttrDict[key].Clear();
					}
					else
					{
						if (CheckSysValueHelper.AttrDict.Count > 100)
						{
							CheckSysValueHelper.AttrDict.Remove(CheckSysValueHelper.AttrDict.Keys.ToList<string>()[0]);
						}
						CheckSysValueHelper.AttrDict.Add(key, new List<string>());
					}
				}
				int MAXLEN = 0;
				foreach (KeyValuePair<string, List<CheckValueResultItem>> item in result.ResultDict)
				{
					foreach (CheckValueResultItem d in item.Value)
					{
						len++;
						foreach (string c in d.Childs)
						{
							string[] files = c.Split(new char[]
							{
								','
							});
							if (string.IsNullOrEmpty(CheckSysValueHelper.AttrDict[key].Find((string x) => x.Equals(files[0]))))
							{
								CheckSysValueHelper.AttrDict[key].Add(files[0]);
							}
							if (CheckSysValueHelper.AttrTypeDict.Count > 5000)
							{
								CheckSysValueHelper.AttrTypeDict.Remove(CheckSysValueHelper.AttrTypeDict.Keys.ToList<string>()[0]);
							}
							if (!CheckSysValueHelper.AttrTypeDict.ContainsKey(key + files[0]))
							{
								CheckSysValueHelper.AttrTypeDict.Add(key + files[0], files[1]);
							}
							MAXLEN = ((files[0].Length > MAXLEN) ? files[0].Length : MAXLEN);
						}
					}
				}
				int i = 0;
				int index = 0;
				CheckSysValueHelper.WriteLine("一共" + len + "组，每次展示20条 【q】：退出展示,返回结果", ConsoleColor.Green);
				foreach (KeyValuePair<string, List<CheckValueResultItem>> item in result.ResultDict)
				{
					foreach (CheckValueResultItem d in item.Value)
					{
						index++;
						i++;
						CheckSysValueHelper.WriteLine(string.Format("第{3}组，AttrName={0}, StrValue={1}, TypeName={2}", new object[]
						{
							item.Key,
							d.StrValue,
							d.TypeName,
							index
						}), ConsoleColor.Green);
						foreach (string c in d.Childs)
						{
							i++;
							string[] files2 = c.Split(new char[]
							{
								','
							});
							string log = "{0, " + -MAXLEN + " }, {1}";
							log = string.Format(log, files2[0], files2[1]);
							CheckSysValueHelper.WriteLine(log, ConsoleColor.Yellow);
							if (i % 20 == 0)
							{
								CheckSysValueHelper.WriteLine(string.Concat(new object[]
								{
									"一共",
									len,
									"组，现在是",
									index,
									"组，【任意键继续】， 【q：退出】"
								}), ConsoleColor.Red);
								string cmd = Console.ReadLine();
								if (cmd.Equals("q"))
								{
									return true;
								}
							}
						}
					}
					CheckSysValueHelper.WriteLine("已经展示完毕", ConsoleColor.Green);
				}
				result2 = true;
			}
			else
			{
				CheckSysValueHelper.WriteLine("查询失败" + result.Info, ConsoleColor.Red);
				result2 = false;
			}
			return result2;
		}

		
		public static void GetValue(string cmd = null)
		{
			try
			{
				cmd = "";
				GetValueModel model = new GetValueModel();
				model.SeachDataList = new List<SeachData>();
				for (;;)
				{
					if (string.IsNullOrEmpty(model.TypeName) || string.IsNullOrEmpty(model.SeachName))
					{
						CheckSysValueHelper._setSeachMgr(ref model);
					}
					if (string.IsNullOrEmpty(model.TypeName) || string.IsNullOrEmpty(model.SeachName))
					{
						break;
					}
					bool isLen = model.SeachDataList.Count > 0;
					if (isLen)
					{
						CheckSysValueHelper.WriteLine("【0】重新输入所有， 【1】清空所有seach 【2】 删除一个seach，【3】修改当前seach 【4】打印筛选条件 【5】添加筛选条件 【q】退出 【其它】查看结果", ConsoleColor.Red);
					}
					else
					{
						CheckSysValueHelper.WriteLine("【0】重新输入所有 【4】打印筛选条件 【5】添加筛选条件 【q】退出 【其它】查看结果", ConsoleColor.Red);
					}
					cmd = Console.ReadLine();
					if (cmd.Equals("0"))
					{
						model = new GetValueModel();
						model.SeachDataList = new List<SeachData>();
					}
					else if (cmd.Equals("1") && isLen)
					{
						model.SeachDataList.Clear();
					}
					else if (cmd.Equals("2") && isLen)
					{
						if (model.SeachDataList.Count > 0)
						{
							model.SeachDataList.RemoveAt(model.SeachDataList.Count - 1);
						}
					}
					else if (cmd.Equals("3") && isLen)
					{
						if (model.SeachDataList.Count > 0)
						{
							SeachData seach = model.SeachDataList[model.SeachDataList.Count - 1];
							CheckSysValueHelper.WriteLine(string.Format("当前查询属性[-{0}-]，要修改 y ？", seach.AttName), ConsoleColor.Red);
							cmd = Console.ReadLine();
							if (cmd.Equals("y"))
							{
								seach = new SeachData();
								model.SeachDataList.RemoveAt(model.SeachDataList.Count - 1);
								List<string> dlist;
								if (CheckSysValueHelper.AttrDict.TryGetValue(CheckSysValueHelper._getSeachKey(model), out dlist))
								{
									if (dlist.Count > 0)
									{
										while (string.IsNullOrEmpty(seach.AttName))
										{
											CheckSysValueHelper.WriteLine("一共有" + dlist.Count + "条属性，【0】查看全部，【1】直接输入 【其它】模糊查询属性", ConsoleColor.Red);
											cmd = Console.ReadLine();
											if (cmd.Equals("0"))
											{
												cmd = CheckSysValueHelper._setResult(dlist, "可查询属性", model);
											}
											else if (cmd.Equals("1"))
											{
												CheckSysValueHelper.WriteLine("输入要查询的属性名", ConsoleColor.Red);
												cmd = Console.ReadLine();
											}
											else
											{
												CheckSysValueHelper.WriteLine("请输入匹配关键字", ConsoleColor.Red);
												cmd = CheckSysValueHelper._setResult(RelationMapModel.FuzzySeach(Console.ReadLine(), dlist), "可查询属性", model);
											}
											if (!cmd.Equals("q"))
											{
												seach.AttName = cmd;
											}
										}
										seach.SeachVal = "";
										CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
										seach.SeachVal = Console.ReadLine();
									}
									else
									{
										CheckSysValueHelper.WriteLine("无缓存记录 输入要查询的属性名", ConsoleColor.Red);
										seach.AttName = Console.ReadLine();
										CheckSysValueHelper.WriteLine("输入筛选 名字，值，比较关系， 例如 name,liu,0", ConsoleColor.Red);
										seach.SeachVal = Console.ReadLine();
									}
									model.SeachDataList.Add(seach);
								}
							}
							else
							{
								CheckSysValueHelper.WriteLine(string.Format("当前筛选条件[-{0}-]，要修改 y ？", seach.SeachVal), ConsoleColor.Red);
								cmd = Console.ReadLine();
								if (cmd.Equals("y"))
								{
									CheckSysValueHelper.WriteLine("输入新的SeachVal", ConsoleColor.Red);
									seach.SeachVal = Console.ReadLine();
								}
							}
						}
					}
					else if (cmd.Equals("4"))
					{
						CheckSysValueHelper._setSeach(model);
					}
					else if (cmd.Equals("5"))
					{
						CheckSysValueHelper._AddSeach(ref model);
					}
					else
					{
						if (cmd.Equals("q"))
						{
							break;
						}
						CheckSysValueHelper._SetCheck(model);
						CheckSysValueHelper.WriteLine("", ConsoleColor.Green);
					}
				}
				return;
			}
			catch (Exception ex)
			{
				CheckSysValueHelper.WriteLine("异常失败", ConsoleColor.Green);
				LogManager.WriteLog(LogTypes.Exception, string.Format("[ljl_WriteMap]{0}", ex.ToString()), null, true);
			}
			CheckSysValueHelper.WriteLine("已经结束 。。。。。 help 查看帮助", ConsoleColor.Green);
		}

		
		private static Dictionary<string, List<string>> AttrDict = new Dictionary<string, List<string>>();

		
		private static Dictionary<string, string> AttrTypeDict = new Dictionary<string, string>();
	}
}
