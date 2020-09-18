using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GameDBServer.DB;
using Server.Tools;

namespace GameDBServer.Logic
{
	
	public class PreNamesManager
	{
		
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

		
		public static void LoadPremNamesFromDB(DBManager dbMgr)
		{
			DBQuery.QueryPreNames(dbMgr, PreNamesManager._PreNamesDict, PreNamesManager._MalePreNamesList, PreNamesManager._FemalePreNamesList);
		}

		
		private static object _Mutex = new object();

		
		private static Random rand = new Random();

		
		private static Dictionary<string, PreNameItem> _PreNamesDict = new Dictionary<string, PreNameItem>(200000);

		
		private static List<PreNameItem> _MalePreNamesList = new List<PreNameItem>(100000);

		
		private static List<PreNameItem> _FemalePreNamesList = new List<PreNameItem>(100000);

		
		private static Queue<PreNameItem> _UsedPreNamesQueue = new Queue<PreNameItem>(5000);
	}
}
