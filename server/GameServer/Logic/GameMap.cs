using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using HSGameEngine.Tools.AStarEx;

namespace GameServer.Logic
{
	
	public class GameMap
	{
		
		
		
		public int PKMode { get; set; }

		
		
		
		public int NotLostEquip { get; set; }

		
		
		
		public int IsolatedMap { get; set; }

		
		
		
		public int HoldNPC { get; set; }

		
		
		
		public int HoldMonster { get; set; }

		
		
		
		public int HoldRole { get; set; }

		
		
		
		public int RealiveMode { get; set; }

		
		
		
		public int RealiveTime { get; set; }

		
		
		
		public int DayLimitSecs { get; set; }

		
		
		
		public DateTimeRange[] LimitTimes { get; set; }

		
		
		
		public int[] LimitGoodsIDs { get; set; }

		
		
		
		public int[] LimitBufferIDs { get; set; }

		
		
		
		public int LimitAuotFight { get; set; }

		
		
		
		public int[] LimitMagicIDs { get; set; }

		
		
		
		public int MinZhuanSheng { get; set; }

		
		
		
		public int MinLevel { get; set; }

		
		
		
		public int MapCode { get; set; }

		
		
		
		public int MapPicCode { get; set; }

		
		
		
		public int MapWidth { get; set; }

		
		
		
		public int MapHeight { get; set; }

		
		
		
		public int MapGridWidth { get; set; }

		
		
		
		public int MapGridHeight { get; set; }

		
		
		
		public int MapGridColsNum { get; set; }

		
		
		
		public int MapGridRowsNum { get; set; }

		
		
		
		public int DefaultBirthPosX { get; set; }

		
		
		
		public int DefaultBirthPosY { get; set; }

		
		
		
		public int BirthRadius { get; set; }

		
		
		public NodeGrid MyNodeGrid
		{
			get
			{
				return this._NodeGrid;
			}
		}

		
		
		public AStar MyAStarFinder
		{
			get
			{
				return this._AStarFinder;
			}
		}

		
		public bool InSafeRegionList(Point grid)
		{
			return this.InSafeRegionList((int)grid.X, (int)grid.Y);
		}

		
		public bool InSafeRegionList(int gridX, int gridY)
		{
			return gridX >= 0 && gridY >= 0 && this.SafeRegionArray.GetUpperBound(0) > gridX && this.SafeRegionArray.GetUpperBound(1) > gridY && 1 == this.SafeRegionArray[gridX, gridY];
		}

		
		public void SetPartialSafeRegion(Point grid, int gridNum)
		{
			if (null != this.SafeRegionArray)
			{
				int startGridX = Math.Max(0, (int)grid.X - gridNum);
				int startGridY = Math.Max(0, (int)grid.Y - gridNum);
				int endGridX = Math.Min(this.MapGridColsNum - 1, (int)grid.X + gridNum);
				int endGridY = Math.Min(this.MapGridRowsNum - 1, (int)grid.Y + gridNum);
				for (int x = startGridX; x <= endGridX; x++)
				{
					for (int y = startGridY; y <= endGridY; y++)
					{
						this.SafeRegionArray[x, y] = 1;
					}
				}
			}
		}

		
		public int GetAreaLuaID(Point grid)
		{
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					return this.AreaLuaList[i].ID;
				}
			}
			return -1;
		}

		
		public List<int> GetAreaLuaIDListByPoint(Point grid)
		{
			List<int> LuaIDList = null;
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					if (LuaIDList == null)
					{
						LuaIDList = new List<int>();
					}
					LuaIDList.Add(this.AreaLuaList[i].ID);
				}
			}
			return LuaIDList;
		}

		
		public GAreaLua GetAreaLuaByID(int areaLuaID)
		{
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (this.AreaLuaList[i].ID == areaLuaID)
				{
					return this.AreaLuaList[i];
				}
			}
			return null;
		}

		
		public List<GAreaLua> GetAreaLuaListByPoint(Point grid)
		{
			List<GAreaLua> GAreaLuaList = null;
			for (int i = 0; i < this.AreaLuaList.Count; i++)
			{
				if (Global.InArea((int)this.AreaLuaList[i].CenterPoint.X, (int)this.AreaLuaList[i].CenterPoint.Y, this.AreaLuaList[i].Radius, grid))
				{
					if (GAreaLuaList == null)
					{
						GAreaLuaList = new List<GAreaLua>();
					}
					GAreaLuaList.Add(this.AreaLuaList[i]);
				}
			}
			return GAreaLuaList;
		}

		
		public int CorrectWidthPointToGridPoint(int value)
		{
			return value / this.MapGridWidth * this.MapGridWidth + this.MapGridWidth / 2;
		}

		
		public int CorrectHeightPointToGridPoint(int value)
		{
			return value / this.MapGridHeight * this.MapGridHeight + this.MapGridHeight / 2;
		}

		
		public int CorrectPointToGrid(int value)
		{
			return value / this.MapGridWidth;
		}

		
		public void InitMap()
		{
			if (!this.InitGameMapBinary())
			{
				this.LoadObstruction();
				this.LoadAnQuanQuXml();
			}
			this.LoadMapTeleportDict();
			this.LoadPathFinderFast();
			this.LoadMapConfig();
			this.LoadAreaLua();
			this.InitEnterMapLuaFile();
		}

		
		public bool InitGameMapBinary()
		{
			string name = string.Format("MapConfig/{0}/obs.bytes", this.MapPicCode);
			string file = Global.MapConfigResPath(name);
			bool result;
			if (File.Exists(file))
			{
				byte[] data = File.ReadAllBytes(file);
				this.MapWidth = ((int)data[0] | (int)data[1] << 8 | (int)data[2] << 16 | (int)data[3] << 24);
				this.MapHeight = ((int)data[4] | (int)data[5] << 8 | (int)data[6] << 16 | (int)data[7] << 24);
				this.MapWidth *= 100;
				this.MapHeight *= 100;
				this.MapGridWidth = (this.MapGridHeight = 100);
				int numCols = (this.MapWidth - 1) / this.MapGridWidth + 1;
				int numRows = (this.MapHeight - 1) / this.MapGridHeight + 1;
				this.MapGridColsNum = numCols;
				this.MapGridRowsNum = numRows;
				numCols = (int)Math.Ceiling(Math.Log((double)numCols, 2.0));
				numCols = (int)Math.Pow(2.0, (double)numCols);
				numRows = (int)Math.Ceiling(Math.Log((double)numRows, 2.0));
				numRows = (int)Math.Pow(2.0, (double)numRows);
				this._NodeGrid = new NodeGrid(numCols, numRows);
				this.SafeRegionArray = new byte[this.MapGridColsNum, this.MapGridRowsNum];
				for (int i = 0; i < this.MapGridColsNum; i++)
				{
					for (int j = 0; j < this.MapGridRowsNum; j++)
					{
						byte val = data[i * this.MapGridColsNum + j + 9];
						this._NodeGrid.setWalkable(i, j, val != 0);
						if (255 == val)
						{
							this.SafeRegionArray[i, j] = 1;
						}
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		
		private void LoadMapConfig()
		{
			Trace.Assert(this.MapGridWidth > 0);
			Trace.Assert(this.MapGridHeight > 0);
			string name = string.Format("Map/{0}/MapConfig.xml", this.MapCode);
			XElement xml = null;
			try
			{
				xml = Global.GetResXml(name);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", name));
			}
			XElement xmlItem = Global.GetSafeXElement(xml, "Settings");
			this.PKMode = (int)Global.GetSafeAttributeLong(xmlItem, "PKMode");
			this.NotLostEquip = (int)Global.GetSafeAttributeLong(xmlItem, "NotLostEquip");
			this.IsolatedMap = (int)Global.GetSafeAttributeLong(xmlItem, "IsolatedMap");
			this.HoldNPC = (int)Global.GetSafeAttributeLong(xmlItem, "HoldNPC");
			this.HoldMonster = (int)Global.GetSafeAttributeLong(xmlItem, "HoldMonster");
			this.HoldRole = (int)Global.GetSafeAttributeLong(xmlItem, "HoldRole");
			this.RealiveMode = (int)Global.GetSafeAttributeLong(xmlItem, "RealiveMode");
			this.RealiveTime = (int)Global.GetSafeAttributeLong(xmlItem, "RealiveTime");
			xmlItem = Global.GetSafeXElement(xml, "Limits");
			this.DayLimitSecs = (int)Global.GetSafeAttributeLong(xmlItem, "DayLimitSecs");
			this.LimitTimes = Global.ParseDateTimeRangeStr(Global.GetSafeAttributeStr(xmlItem, "Times"));
			this.LimitGoodsIDs = Global.String2IntArray(Global.GetSafeAttributeStr(xmlItem, "GoodsIDs"), ',');
			this.LimitBufferIDs = Global.String2IntArray(Global.GetSafeAttributeStr(xmlItem, "BufferIDs"), ',');
			this.LimitAuotFight = (int)Global.GetSafeAttributeLong(xmlItem, "AutoFight");
			this.LimitMagicIDs = Global.String2IntArray(Global.GetSafeAttributeStr(xmlItem, "MagicIDs"), ',');
			this.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xmlItem, "MinZhuanSheng");
			this.MinLevel = (int)Global.GetSafeAttributeLong(xmlItem, "MinLevel");
			if (null != Global.XmlInfo["ConfigSettings"])
			{
				try
				{
					xml = Global.GetXElement(Global.XmlInfo["ConfigSettings"], "Map", "Code", this.MapCode.ToString());
					if (null != xml)
					{
						this.OnlyShowNPC = (Global.GetDefAttributeStr(xml, "ElsePeople", "0") == "1");
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("{0} MapCode={1}", ex.Message.ToString(), this.MapCode));
				}
			}
		}

		
		private void LoadAreaLua()
		{
			Trace.Assert(this.MapGridWidth > 0);
			Trace.Assert(this.MapGridHeight > 0);
			string name = string.Format("Map/{0}/AreaLua.xml", this.MapCode);
			XElement xml = null;
			try
			{
				xml = Global.GetResXml(name);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", name));
			}
			IEnumerable<XElement> images = xml.Element("Areas").Elements();
			if (null != images)
			{
				this.AreaLuaList = new List<GAreaLua>();
				foreach (XElement image_item in images)
				{
					int id = (int)Global.GetSafeAttributeLong(image_item, "ID");
					int posX = (int)Global.GetSafeAttributeLong(image_item, "X");
					int posY = (int)Global.GetSafeAttributeLong(image_item, "Y");
					int radius = (int)Global.GetSafeAttributeLong(image_item, "Radius");
					string luaScriptFile = Global.GetSafeAttributeStr(image_item, "LuaScriptFile");
					int taskId = 0;
					AddtionType addtionType = AddtionType.NowTrigger;
					Dictionary<AreaEventType, List<int>> eventDict = new Dictionary<AreaEventType, List<int>>();
					if (image_item.Attribute("Touch") != null)
					{
						string[] addtionInfo = Global.GetSafeAttributeStr(image_item, "Touch").Split(new char[]
						{
							','
						}, StringSplitOptions.RemoveEmptyEntries);
						if (addtionInfo.Length > 1)
						{
							addtionType = (AddtionType)Convert.ToInt32(addtionInfo[0]);
							taskId = Convert.ToInt32(addtionInfo[1]);
						}
						string[] eventInfos = Global.GetSafeAttributeStr(image_item, "Event").Split(new char[]
						{
							'|'
						}, StringSplitOptions.RemoveEmptyEntries);
						foreach (string eventInfo in eventInfos)
						{
							List<int> eventValues = new List<int>();
							string[] data = eventInfo.Split(new char[]
							{
								','
							}, StringSplitOptions.RemoveEmptyEntries);
							if (data.Length > 1)
							{
								for (int i = 1; i < data.Length; i++)
								{
									eventValues.Add(Convert.ToInt32(data[i]));
								}
								eventDict.Add((AreaEventType)Convert.ToInt32(data[0]), eventValues);
							}
						}
					}
					GAreaLua areaLua = new GAreaLua
					{
						ID = id,
						CenterPoint = new Point((double)(posX / this.MapGridWidth), (double)(posY / this.MapGridHeight)),
						Radius = Math.Max(radius / this.MapGridWidth, 1),
						LuaScriptFileName = luaScriptFile,
						AddtionType = addtionType,
						TaskId = taskId,
						AreaEventDict = eventDict
					};
					this.AreaLuaList.Add(areaLua);
				}
				xml = null;
			}
		}

		
		private void LoadObstruction()
		{
			string name = string.Format("MapConfig/{0}/obs.xml", this.MapPicCode);
			XElement xml = null;
			try
			{
				xml = Global.GetMapConfigResXml(name);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", name));
			}
			this.MapGridWidth = GameManager.MapGridWidth;
			this.MapGridHeight = GameManager.MapGridHeight;
			int numCols = (this.MapWidth - 1) / this.MapGridWidth + 1;
			int numRows = (this.MapHeight - 1) / this.MapGridHeight + 1;
			this.MapGridColsNum = numCols;
			this.MapGridRowsNum = numRows;
			numCols = (int)Math.Ceiling(Math.Log((double)numCols, 2.0));
			numCols = (int)Math.Pow(2.0, (double)numCols);
			numRows = (int)Math.Ceiling(Math.Log((double)numRows, 2.0));
			numRows = (int)Math.Pow(2.0, (double)numRows);
			if (!GameMap.NodeGridCacheDict.TryGetValue(this.MapPicCode, out this._NodeGrid))
			{
				this._NodeGrid = new NodeGrid(numCols, numRows);
				GameMap.NodeGridCacheDict[this.MapPicCode] = this._NodeGrid;
				string s = xml.Attribute("Value").Value;
				if (s != "")
				{
					string[] obstruction = s.Split(new char[]
					{
						','
					});
					for (int i = 0; i < obstruction.Count<string>(); i++)
					{
						if (!(obstruction[i].Trim() == ""))
						{
							string[] obstructionXY = obstruction[i].Split(new char[]
							{
								'_'
							});
							int toX = Convert.ToInt32(obstructionXY[0]) / 2;
							int toY = Convert.ToInt32(obstructionXY[1]) / 2;
							if (toX < numCols && toY < numRows)
							{
								this._NodeGrid.setWalkable(toX, toY, false);
							}
						}
					}
				}
			}
		}

		
		private void LoadAnQuanQuXml()
		{
			string name = string.Format("MapConfig/{0}/anquanqu.xml", this.MapPicCode);
			XElement xml = null;
			try
			{
				xml = Global.GetMapConfigResXml(name);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", name));
			}
			if (!GameMap.AnQuanQuCacheDict.TryGetValue(this.MapPicCode, out this.SafeRegionArray))
			{
				this.SafeRegionArray = new byte[this.MapGridColsNum, this.MapGridRowsNum];
				GameMap.AnQuanQuCacheDict[this.MapPicCode] = this.SafeRegionArray;
				string s = xml.Attribute("Value").Value;
				if (!string.IsNullOrEmpty(s))
				{
					string[] obstruction = s.Split(new char[]
					{
						','
					});
					for (int i = 0; i < obstruction.Count<string>(); i++)
					{
						if (!(obstruction[i].Trim() == ""))
						{
							string[] obstructionXY = obstruction[i].Split(new char[]
							{
								'_'
							});
							int toX = Convert.ToInt32(obstructionXY[0]) / 2;
							int toY = Convert.ToInt32(obstructionXY[1]) / 2;
							if (toX < this.MapGridColsNum && toY < this.MapGridRowsNum)
							{
								this.SafeRegionArray[toX, toY] = 1;
							}
						}
					}
				}
			}
		}

		
		private void LoadMapTeleportDict()
		{
			string name = string.Format("Map/{0}/teleports.xml", this.MapCode);
			XElement xml = null;
			try
			{
				xml = Global.GetResXml(name);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("启动时加载xml文件: {0} 失败", name));
			}
			IEnumerable<XElement> images = xml.Element("Teleports").Elements();
			if (null != images)
			{
				foreach (XElement image_item in images)
				{
					int code = (int)Global.GetSafeAttributeLong(image_item, "Key");
					int to = (int)Global.GetSafeAttributeLong(image_item, "To");
					int toX = (int)Global.GetSafeAttributeLong(image_item, "ToX");
					int toY = (int)Global.GetSafeAttributeLong(image_item, "ToY");
					int x = (int)Global.GetSafeAttributeLong(image_item, "X");
					int y = (int)Global.GetSafeAttributeLong(image_item, "Y");
					int radius = (int)Global.GetSafeAttributeLong(image_item, "Radius");
					MapTeleport mapTeleport = new MapTeleport
					{
						Code = code,
						MapID = -1,
						X = x,
						Y = y,
						ToX = toX,
						ToY = toY,
						ToMapID = to,
						Radius = radius
					};
					this.MapTeleportDict[code] = mapTeleport;
				}
				xml = null;
			}
		}

		
		private void LoadPathFinderFast()
		{
			this._AStarFinder = new AStar();
		}

		
		private void InitEnterMapLuaFile()
		{
			string fileName = Global.GetMapLuaScriptFile(this.MapCode, "enterMap.lua");
			if (File.Exists(fileName))
			{
				this.EnterMapLuaFile = fileName;
			}
		}

		
		public bool CanMove(int gridX, int gridY)
		{
			return gridX * this.MapGridWidth < this.MapWidth && gridX >= 0 && gridY * this.MapGridHeight < this.MapHeight && gridY >= 0 && this.MyNodeGrid.isWalkable(gridX, gridY);
		}

		
		public bool OnlyShowNPC;

		
		public byte[,] SafeRegionArray = null;

		
		private List<GAreaLua> AreaLuaList;

		
		public Dictionary<int, MapTeleport> MapTeleportDict = new Dictionary<int, MapTeleport>();

		
		private NodeGrid _NodeGrid;

		
		private AStar _AStarFinder;

		
		public string EnterMapLuaFile = null;

		
		private static Dictionary<int, NodeGrid> NodeGridCacheDict = new Dictionary<int, NodeGrid>();

		
		private static Dictionary<int, byte[,]> AnQuanQuCacheDict = new Dictionary<int, byte[,]>();
	}
}
