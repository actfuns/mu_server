using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x020001D4 RID: 468
	public class PreNamesManager
	{
		// Token: 0x060009D4 RID: 2516 RVA: 0x0005E924 File Offset: 0x0005CB24
		public static void AddPreNameItem(string name, int sex, int used)
		{
			PreNameItem preNameItem = new PreNameItem
			{
				Name = name,
				Sex = sex,
				Used = used
			};
			lock (PreNamesManager._Mutex)
			{
				PreNamesManager._PreNamesDict[name] = preNameItem;
				if (0 == sex)
				{
					PreNamesManager._MalePreNamesList.Add(preNameItem);
				}
				else
				{
					PreNamesManager._FemalePreNamesList.Add(preNameItem);
				}
			}
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0005E9C0 File Offset: 0x0005CBC0
		public static string GetRandomName(int Sex)
		{
			string preName = "";
			lock (PreNamesManager._Mutex)
			{
				List<PreNameItem> preNamesList;
				if (0 == Sex)
				{
					preNamesList = PreNamesManager._MalePreNamesList;
				}
				else
				{
					preNamesList = PreNamesManager._FemalePreNamesList;
				}
				if (preNamesList.Count > 0)
				{
					int count = 10;
					while (count-- >= 0)
					{
						int randIndex = PreNamesManager.rand.Next(0, preNamesList.Count);
						if (preNamesList[randIndex].Used <= 0)
						{
							preName = preNamesList[randIndex].Name;
							break;
						}
					}
				}
			}
			return preName;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0005EA9C File Offset: 0x0005CC9C
		public static bool SetUsedPreName(string name)
		{
			lock (PreNamesManager._Mutex)
			{
				PreNameItem preNameItem = null;
				if (PreNamesManager._PreNamesDict.TryGetValue(name, out preNameItem))
				{
					if (preNameItem.Used <= 0)
					{
						preNameItem.Used = 1;
						PreNamesManager._UsedPreNamesQueue.Enqueue(preNameItem);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0005EB2C File Offset: 0x0005CD2C
		public static void ClearUsedPreNames()
		{
			lock (PreNamesManager._Mutex)
			{
				int count = 50;
				while (PreNamesManager._UsedPreNamesQueue.Count > 0 && count-- >= 0)
				{
					PreNameItem preNameItem = PreNamesManager._UsedPreNamesQueue.Dequeue();
					if (null != preNameItem)
					{
						PreNamesManager._PreNamesDict.Remove(preNameItem.Name);
						if (0 == preNameItem.Sex)
						{
							PreNamesManager._MalePreNamesList.Remove(preNameItem);
						}
						else
						{
							PreNamesManager._FemalePreNamesList.Remove(preNameItem);
						}
					}
				}
			}
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0005EBF8 File Offset: 0x0005CDF8
		private static List<string> LoadListFromFileByName(string fileName)
		{
			List<string> strList = new List<string>();
			try
			{
				if (!File.Exists(fileName))
				{
					return strList;
				}
				StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("gb2312"));
				if (null == sr)
				{
					return strList;
				}
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					line = line.Trim();
					if (!string.IsNullOrEmpty(line))
					{
						strList.Add(line);
					}
				}
				sr.Close();
				return strList;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return strList;
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0005ECBC File Offset: 0x0005CEBC
		public static void LoadFromFiles(DBManager dbMgr)
		{
			try
			{
				if (File.Exists("./名字库/姓.txt"))
				{
					if (File.Exists("./名字库/男.txt"))
					{
						if (File.Exists("./名字库/女.txt"))
						{
							List<string> xingList = PreNamesManager.LoadListFromFileByName("./名字库/姓.txt");
							List<string> nanList = PreNamesManager.LoadListFromFileByName("./名字库/男.txt");
							List<string> nvList = PreNamesManager.LoadListFromFileByName("./名字库/女.txt");
							for (int xingIndex = 0; xingIndex < xingList.Count; xingIndex++)
							{
								for (int nanIndex = 0; nanIndex < nanList.Count; nanIndex++)
								{
									string preName = xingList[xingIndex] + nanList[nanIndex];
									if (DBWriter.InsertNewPreName(dbMgr, preName, 0) >= 0)
									{
										PreNamesManager.AddPreNameItem(preName, 0, 0);
									}
								}
								for (int nvIndex = 0; nvIndex < nvList.Count; nvIndex++)
								{
									string preName = xingList[xingIndex] + nvList[nvIndex];
									if (DBWriter.InsertNewPreName(dbMgr, preName, 1) >= 0)
									{
										PreNamesManager.AddPreNameItem(preName, 1, 0);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0005EE2C File Offset: 0x0005D02C
		public static void LoadPremNamesFromDB(DBManager dbMgr)
		{
			DBQuery.QueryPreNames(dbMgr, PreNamesManager._PreNamesDict, PreNamesManager._MalePreNamesList, PreNamesManager._FemalePreNamesList);
		}

		// Token: 0x04000BFC RID: 3068
		private static object _Mutex = new object();

		// Token: 0x04000BFD RID: 3069
		private static Random rand = new Random();

		// Token: 0x04000BFE RID: 3070
		private static Dictionary<string, PreNameItem> _PreNamesDict = new Dictionary<string, PreNameItem>(200000);

		// Token: 0x04000BFF RID: 3071
		private static List<PreNameItem> _MalePreNamesList = new List<PreNameItem>(100000);

		// Token: 0x04000C00 RID: 3072
		private static List<PreNameItem> _FemalePreNamesList = new List<PreNameItem>(100000);

		// Token: 0x04000C01 RID: 3073
		private static Queue<PreNameItem> _UsedPreNamesQueue = new Queue<PreNameItem>(5000);
	}
}
