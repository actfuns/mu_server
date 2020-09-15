using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CheckSysValueDll
{
	// Token: 0x020008F0 RID: 2288
	public class RelationMapModel
	{
		// Token: 0x06004203 RID: 16899 RVA: 0x003C55B4 File Offset: 0x003C37B4
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

		// Token: 0x06004204 RID: 16900 RVA: 0x003C562C File Offset: 0x003C382C
		private static string CreatDirectory()
		{
			string Path = AppDomain.CurrentDomain.BaseDirectory + "CheckRelation\\";
			if (!Directory.Exists(Path))
			{
				Directory.CreateDirectory(Path);
			}
			return Path;
		}

		// Token: 0x06004205 RID: 16901 RVA: 0x003C5668 File Offset: 0x003C3868
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

		// Token: 0x06004206 RID: 16902 RVA: 0x003C5798 File Offset: 0x003C3998
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

		// Token: 0x06004207 RID: 16903 RVA: 0x003C59AC File Offset: 0x003C3BAC
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

		// Token: 0x06004208 RID: 16904 RVA: 0x003C5B00 File Offset: 0x003C3D00
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

		// Token: 0x06004209 RID: 16905 RVA: 0x003C5B5C File Offset: 0x003C3D5C
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

		// Token: 0x0600420A RID: 16906 RVA: 0x003C5BDC File Offset: 0x003C3DDC
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

		// Token: 0x0600420B RID: 16907 RVA: 0x003C5D74 File Offset: 0x003C3F74
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

		// Token: 0x0600420C RID: 16908 RVA: 0x003C5E24 File Offset: 0x003C4024
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

		// Token: 0x0600420D RID: 16909 RVA: 0x003C5ED0 File Offset: 0x003C40D0
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

		// Token: 0x04005008 RID: 20488
		private static bool isWrite = false;

		// Token: 0x04005009 RID: 20489
		private static List<string> EnumList = new List<string>();

		// Token: 0x0400500A RID: 20490
		private static Dictionary<string, List<string>> Map = new Dictionary<string, List<string>>();
	}
}
