using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CheckSysValueDll
{
	
	public class RelationMapModel
	{
		
		private static void GetEnumList()
		{
			string filePath = AppDomain.CurrentDomain.BaseDirectory + "CheckRelation\\EnumType.json";
			try
			{
				if (File.Exists(filePath))
				{
					string JsonStr = File.ReadAllText(filePath);
					JavaScriptSerializer jss = new JavaScriptSerializer();
					List<string> userJson = jss.Deserialize<List<string>>(JsonStr);
					if (null != userJson)
					{
						RelationMapModel.EnumList = userJson;
					}
				}
			}
			catch
			{
			}
		}

		
		private static string CreatDirectory()
		{
			string Path = AppDomain.CurrentDomain.BaseDirectory + "CheckRelation\\";
			if (!Directory.Exists(Path))
			{
				Directory.CreateDirectory(Path);
			}
			return Path;
		}

		
		private static void ReadRelationMap()
		{
			if (RelationMapModel.Map.Count <= 0)
			{
				string Path = RelationMapModel.CreatDirectory();
				string filePath = Path + "RelationMapAll.txt";
				try
				{
					if (File.Exists(filePath))
					{
						string key = null;
						StreamReader sr = new StreamReader(filePath);
						while (sr.Peek() >= 0)
						{
							string str = sr.ReadLine().Trim();
							if (string.IsNullOrEmpty(key))
							{
								key = str;
							}
							else if (str.IndexOf("******************") > -1)
							{
								key = null;
							}
							else if (!string.IsNullOrEmpty(str))
							{
								if (RelationMapModel.Map.ContainsKey(key))
								{
									RelationMapModel.Map[key].Add(str);
								}
								else
								{
									RelationMapModel.Map.Add(key, new List<string>
									{
										str
									});
								}
							}
						}
						sr.Close();
					}
				}
				catch
				{
				}
			}
		}

		
		public static void WriteMap(Assembly assembly)
		{
			string Path = RelationMapModel.CreatDirectory();
			if (!RelationMapModel.isWrite || RelationMapModel.Map.Count < 1)
			{
				RelationMapModel.GetRelationMap(assembly);
			}
			string path = Path + "RelationMapKey.txt";
			using (StreamWriter sw = new StreamWriter(path))
			{
				foreach (KeyValuePair<string, List<string>> d in RelationMapModel.Map)
				{
					sw.WriteLine(d.Key);
				}
			}
			path = Path + "RelationMapAll.txt";
			using (StreamWriter sw = new StreamWriter(path))
			{
				foreach (KeyValuePair<string, List<string>> d in RelationMapModel.Map)
				{
					sw.WriteLine(d.Key);
					foreach (string item in d.Value)
					{
						sw.WriteLine("     " + item);
					}
					sw.WriteLine("**************************************************************************************************");
				}
			}
			path = Path + "EnumType.json";
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.WriteLine(CheckModel.Data2Json(RelationMapModel.EnumList));
			}
			Console.WriteLine("画图完成 RelationMap end");
			RelationMapModel.isWrite = true;
		}

		
		private static void GetRelationMap(Assembly assembly)
		{
			RelationMapModel.Map.Clear();
			if (RelationMapModel.EnumList.Count < 1)
			{
				RelationMapModel.GetEnumList();
			}
			foreach (Type type in assembly.GetTypes())
			{
				if (!RelationMapModel.IsFilter(type.FullName))
				{
					if (RelationMapModel.IsEnum(type))
					{
						RelationMapModel.EnumList.Add(type.FullName);
					}
					else
					{
						FieldInfo[] Infos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						foreach (FieldInfo info in Infos)
						{
							try
							{
								if (RelationMapModel.Map.ContainsKey(type.FullName))
								{
									RelationMapModel.Map[type.FullName].Add(info.Name);
								}
								else
								{
									RelationMapModel.Map.Add(type.FullName, new List<string>
									{
										info.Name
									});
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}

		
		private static bool IsEnum(Type type)
		{
			try
			{
				return type.IsEnum;
			}
			catch
			{
			}
			return false;
		}

		
		private static bool IsFilter(string Name)
		{
			try
			{
				bool flag;
				if (Name.IndexOf("CheckSysValueDll.") <= -1)
				{
					flag = string.IsNullOrEmpty(RelationMapModel.EnumList.Find((string x) => x.Equals(Name)));
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		
		public static object GetObject(Assembly assembly, string TypeName, string AttrName, ref CheckValueResult resultData)
		{
			TypeName = TypeName.Trim();
			AttrName = AttrName.Trim();
			List<string> attrList = new List<string>();
			Type type = assembly.GetType(TypeName);
			object result;
			if (null == type)
			{
				result = null;
			}
			else
			{
				FieldInfo[] Infos = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo info in Infos)
				{
					attrList.Add(info.Name);
				}
				if (!RelationMapModel.Map.TryGetValue(TypeName, out attrList))
				{
					RelationMapModel.Map.Add(TypeName, attrList);
				}
				if (string.IsNullOrEmpty(AttrName))
				{
					resultData.Info = "只查询了类型 数据包含数据有";
					CheckValueResultItem data = new CheckValueResultItem();
					List<CheckValueResultItem> dList = new List<CheckValueResultItem>
					{
						data
					};
					data.TypeName = "只查询了类型";
					foreach (string item in attrList)
					{
						data.Childs.Add(string.Format("{0},{1}", item, ""));
					}
					resultData.ResultDict.Add("包含属性", dList);
					result = attrList;
				}
				else
				{
					FieldInfo infoData = type.GetField(AttrName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == infoData)
					{
						result = attrList;
					}
					else
					{
						result = infoData.GetValue(null);
					}
				}
			}
			return result;
		}

		
		public static List<string> FuzzySeachType(string name)
		{
			name = name.Trim();
			List<string> dlist = new List<string>();
			List<string> result;
			if (string.IsNullOrEmpty(name))
			{
				result = dlist;
			}
			else
			{
				RelationMapModel.ReadRelationMap();
				foreach (string item in RelationMapModel.Map.Keys)
				{
					if (item.ToLower().IndexOf(name.ToLower()) > -1)
					{
						dlist.Add(item);
					}
				}
				dlist.Sort();
				result = dlist;
			}
			return result;
		}

		
		public static List<string> FuzzySeach(string name, List<string> dlist)
		{
			List<string> temp = new List<string>();
			name = name.Trim();
			if (string.IsNullOrEmpty(name))
			{
				temp.AddRange(dlist);
			}
			else
			{
				foreach (string item in dlist)
				{
					if (item.ToLower().IndexOf(name.ToLower()) > -1)
					{
						temp.Add(item);
					}
				}
			}
			temp.Sort();
			return temp;
		}

		
		public static List<string> GetSeachAttr(string type)
		{
			RelationMapModel.ReadRelationMap();
			type = type.Trim();
			List<string> dlist = new List<string>();
			List<string> result;
			if (!RelationMapModel.Map.ContainsKey(type))
			{
				result = dlist;
			}
			else
			{
				foreach (string item in RelationMapModel.Map[type])
				{
					dlist.Add(item);
				}
				dlist.Sort();
				result = dlist;
			}
			return result;
		}

		
		private static bool isWrite = false;

		
		private static List<string> EnumList = new List<string>();

		
		private static Dictionary<string, List<string>> Map = new Dictionary<string, List<string>>();
	}
}
