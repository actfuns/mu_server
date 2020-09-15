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
	// Token: 0x020006CE RID: 1742
	public class GameMap
	{
		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060023CD RID: 9165 RVA: 0x001E82C8 File Offset: 0x001E64C8
		// (set) Token: 0x060023CE RID: 9166 RVA: 0x001E82DF File Offset: 0x001E64DF
		public int PKMode { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x060023CF RID: 9167 RVA: 0x001E82E8 File Offset: 0x001E64E8
		// (set) Token: 0x060023D0 RID: 9168 RVA: 0x001E82FF File Offset: 0x001E64FF
		public int NotLostEquip { get; set; }

		// Token: 0x1700024F RID: 591
		// (get) Token: 0x060023D1 RID: 9169 RVA: 0x001E8308 File Offset: 0x001E6508
		// (set) Token: 0x060023D2 RID: 9170 RVA: 0x001E831F File Offset: 0x001E651F
		public int IsolatedMap { get; set; }

		// Token: 0x17000250 RID: 592
		// (get) Token: 0x060023D3 RID: 9171 RVA: 0x001E8328 File Offset: 0x001E6528
		// (set) Token: 0x060023D4 RID: 9172 RVA: 0x001E833F File Offset: 0x001E653F
		public int HoldNPC { get; set; }

		// Token: 0x17000251 RID: 593
		// (get) Token: 0x060023D5 RID: 9173 RVA: 0x001E8348 File Offset: 0x001E6548
		// (set) Token: 0x060023D6 RID: 9174 RVA: 0x001E835F File Offset: 0x001E655F
		public int HoldMonster { get; set; }

		// Token: 0x17000252 RID: 594
		// (get) Token: 0x060023D7 RID: 9175 RVA: 0x001E8368 File Offset: 0x001E6568
		// (set) Token: 0x060023D8 RID: 9176 RVA: 0x001E837F File Offset: 0x001E657F
		public int HoldRole { get; set; }

		// Token: 0x17000253 RID: 595
		// (get) Token: 0x060023D9 RID: 9177 RVA: 0x001E8388 File Offset: 0x001E6588
		// (set) Token: 0x060023DA RID: 9178 RVA: 0x001E839F File Offset: 0x001E659F
		public int RealiveMode { get; set; }

		// Token: 0x17000254 RID: 596
		// (get) Token: 0x060023DB RID: 9179 RVA: 0x001E83A8 File Offset: 0x001E65A8
		// (set) Token: 0x060023DC RID: 9180 RVA: 0x001E83BF File Offset: 0x001E65BF
		public int RealiveTime { get; set; }

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x060023DD RID: 9181 RVA: 0x001E83C8 File Offset: 0x001E65C8
		// (set) Token: 0x060023DE RID: 9182 RVA: 0x001E83DF File Offset: 0x001E65DF
		public int DayLimitSecs { get; set; }

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x060023DF RID: 9183 RVA: 0x001E83E8 File Offset: 0x001E65E8
		// (set) Token: 0x060023E0 RID: 9184 RVA: 0x001E83FF File Offset: 0x001E65FF
		public DateTimeRange[] LimitTimes { get; set; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x060023E1 RID: 9185 RVA: 0x001E8408 File Offset: 0x001E6608
		// (set) Token: 0x060023E2 RID: 9186 RVA: 0x001E841F File Offset: 0x001E661F
		public int[] LimitGoodsIDs { get; set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x060023E3 RID: 9187 RVA: 0x001E8428 File Offset: 0x001E6628
		// (set) Token: 0x060023E4 RID: 9188 RVA: 0x001E843F File Offset: 0x001E663F
		public int[] LimitBufferIDs { get; set; }

		// Token: 0x17000259 RID: 601
		// (get) Token: 0x060023E5 RID: 9189 RVA: 0x001E8448 File Offset: 0x001E6648
		// (set) Token: 0x060023E6 RID: 9190 RVA: 0x001E845F File Offset: 0x001E665F
		public int LimitAuotFight { get; set; }

		// Token: 0x1700025A RID: 602
		// (get) Token: 0x060023E7 RID: 9191 RVA: 0x001E8468 File Offset: 0x001E6668
		// (set) Token: 0x060023E8 RID: 9192 RVA: 0x001E847F File Offset: 0x001E667F
		public int[] LimitMagicIDs { get; set; }

		// Token: 0x1700025B RID: 603
		// (get) Token: 0x060023E9 RID: 9193 RVA: 0x001E8488 File Offset: 0x001E6688
		// (set) Token: 0x060023EA RID: 9194 RVA: 0x001E849F File Offset: 0x001E669F
		public int MinZhuanSheng { get; set; }

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x060023EB RID: 9195 RVA: 0x001E84A8 File Offset: 0x001E66A8
		// (set) Token: 0x060023EC RID: 9196 RVA: 0x001E84BF File Offset: 0x001E66BF
		public int MinLevel { get; set; }

		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060023ED RID: 9197 RVA: 0x001E84C8 File Offset: 0x001E66C8
		// (set) Token: 0x060023EE RID: 9198 RVA: 0x001E84DF File Offset: 0x001E66DF
		public int MapCode { get; set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x060023EF RID: 9199 RVA: 0x001E84E8 File Offset: 0x001E66E8
		// (set) Token: 0x060023F0 RID: 9200 RVA: 0x001E84FF File Offset: 0x001E66FF
		public int MapPicCode { get; set; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x060023F1 RID: 9201 RVA: 0x001E8508 File Offset: 0x001E6708
		// (set) Token: 0x060023F2 RID: 9202 RVA: 0x001E851F File Offset: 0x001E671F
		public int MapWidth { get; set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x060023F3 RID: 9203 RVA: 0x001E8528 File Offset: 0x001E6728
		// (set) Token: 0x060023F4 RID: 9204 RVA: 0x001E853F File Offset: 0x001E673F
		public int MapHeight { get; set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x060023F5 RID: 9205 RVA: 0x001E8548 File Offset: 0x001E6748
		// (set) Token: 0x060023F6 RID: 9206 RVA: 0x001E855F File Offset: 0x001E675F
		public int MapGridWidth { get; set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x060023F7 RID: 9207 RVA: 0x001E8568 File Offset: 0x001E6768
		// (set) Token: 0x060023F8 RID: 9208 RVA: 0x001E857F File Offset: 0x001E677F
		public int MapGridHeight { get; set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x060023F9 RID: 9209 RVA: 0x001E8588 File Offset: 0x001E6788
		// (set) Token: 0x060023FA RID: 9210 RVA: 0x001E859F File Offset: 0x001E679F
		public int MapGridColsNum { get; set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x060023FB RID: 9211 RVA: 0x001E85A8 File Offset: 0x001E67A8
		// (set) Token: 0x060023FC RID: 9212 RVA: 0x001E85BF File Offset: 0x001E67BF
		public int MapGridRowsNum { get; set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x060023FD RID: 9213 RVA: 0x001E85C8 File Offset: 0x001E67C8
		// (set) Token: 0x060023FE RID: 9214 RVA: 0x001E85DF File Offset: 0x001E67DF
		public int DefaultBirthPosX { get; set; }

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x060023FF RID: 9215 RVA: 0x001E85E8 File Offset: 0x001E67E8
		// (set) Token: 0x06002400 RID: 9216 RVA: 0x001E85FF File Offset: 0x001E67FF
		public int DefaultBirthPosY { get; set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06002401 RID: 9217 RVA: 0x001E8608 File Offset: 0x001E6808
		// (set) Token: 0x06002402 RID: 9218 RVA: 0x001E861F File Offset: 0x001E681F
		public int BirthRadius { get; set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06002403 RID: 9219 RVA: 0x001E8628 File Offset: 0x001E6828
		public NodeGrid MyNodeGrid
		{
			get
			{
				return this._NodeGrid;
			}
		}

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06002404 RID: 9220 RVA: 0x001E8640 File Offset: 0x001E6840
		public AStar MyAStarFinder
		{
			get
			{
				return this._AStarFinder;
			}
		}

		// Token: 0x06002405 RID: 9221 RVA: 0x001E8658 File Offset: 0x001E6858
		public bool InSafeRegionList(Point grid)
		{
			return this.InSafeRegionList((int)grid.X, (int)grid.Y);
		}

		// Token: 0x06002406 RID: 9222 RVA: 0x001E8680 File Offset: 0x001E6880
		public bool InSafeRegionList(int gridX, int gridY)
		{
			return gridX >= 0 && gridY >= 0 && this.SafeRegionArray.GetUpperBound(0) > gridX && this.SafeRegionArray.GetUpperBound(1) > gridY && 1 == this.SafeRegionArray[gridX, gridY];
		}

		// Token: 0x06002407 RID: 9223 RVA: 0x001E86D4 File Offset: 0x001E68D4
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

		// Token: 0x06002408 RID: 9224 RVA: 0x001E8794 File Offset: 0x001E6994
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

		// Token: 0x06002409 RID: 9225 RVA: 0x001E882C File Offset: 0x001E6A2C
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

		// Token: 0x0600240A RID: 9226 RVA: 0x001E88E8 File Offset: 0x001E6AE8
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

		// Token: 0x0600240B RID: 9227 RVA: 0x001E8944 File Offset: 0x001E6B44
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

		// Token: 0x0600240C RID: 9228 RVA: 0x001E89FC File Offset: 0x001E6BFC
		public int CorrectWidthPointToGridPoint(int value)
		{
			return value / this.MapGridWidth * this.MapGridWidth + this.MapGridWidth / 2;
		}

		// Token: 0x0600240D RID: 9229 RVA: 0x001E8A28 File Offset: 0x001E6C28
		public int CorrectHeightPointToGridPoint(int value)
		{
			return value / this.MapGridHeight * this.MapGridHeight + this.MapGridHeight / 2;
		}

		// Token: 0x0600240E RID: 9230 RVA: 0x001E8A54 File Offset: 0x001E6C54
		public int CorrectPointToGrid(int value)
		{
			return value / this.MapGridWidth;
		}

		// Token: 0x0600240F RID: 9231 RVA: 0x001E8A70 File Offset: 0x001E6C70
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

		// Token: 0x06002410 RID: 9232 RVA: 0x001E8ABC File Offset: 0x001E6CBC
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

		// Token: 0x06002411 RID: 9233 RVA: 0x001E8CC0 File Offset: 0x001E6EC0
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

		// Token: 0x06002412 RID: 9234 RVA: 0x001E8F44 File Offset: 0x001E7144
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

		// Token: 0x06002413 RID: 9235 RVA: 0x001E9250 File Offset: 0x001E7450
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

		// Token: 0x06002414 RID: 9236 RVA: 0x001E947C File Offset: 0x001E767C
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

		// Token: 0x06002415 RID: 9237 RVA: 0x001E960C File Offset: 0x001E780C
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

		// Token: 0x06002416 RID: 9238 RVA: 0x001E97A4 File Offset: 0x001E79A4
		private void LoadPathFinderFast()
		{
			this._AStarFinder = new AStar();
		}

		// Token: 0x06002417 RID: 9239 RVA: 0x001E97B4 File Offset: 0x001E79B4
		private void InitEnterMapLuaFile()
		{
			string fileName = Global.GetMapLuaScriptFile(this.MapCode, "enterMap.lua");
			if (File.Exists(fileName))
			{
				this.EnterMapLuaFile = fileName;
			}
		}

		// Token: 0x06002418 RID: 9240 RVA: 0x001E97EC File Offset: 0x001E79EC
		public bool CanMove(int gridX, int gridY)
		{
			return gridX * this.MapGridWidth < this.MapWidth && gridX >= 0 && gridY * this.MapGridHeight < this.MapHeight && gridY >= 0 && this.MyNodeGrid.isWalkable(gridX, gridY);
		}

		// Token: 0x04003830 RID: 14384
		public bool OnlyShowNPC;

		// Token: 0x04003831 RID: 14385
		public byte[,] SafeRegionArray = null;

		// Token: 0x04003832 RID: 14386
		private List<GAreaLua> AreaLuaList;

		// Token: 0x04003833 RID: 14387
		public Dictionary<int, MapTeleport> MapTeleportDict = new Dictionary<int, MapTeleport>();

		// Token: 0x04003834 RID: 14388
		private NodeGrid _NodeGrid;

		// Token: 0x04003835 RID: 14389
		private AStar _AStarFinder;

		// Token: 0x04003836 RID: 14390
		public string EnterMapLuaFile = null;

		// Token: 0x04003837 RID: 14391
		private static Dictionary<int, NodeGrid> NodeGridCacheDict = new Dictionary<int, NodeGrid>();

		// Token: 0x04003838 RID: 14392
		private static Dictionary<int, byte[,]> AnQuanQuCacheDict = new Dictionary<int, byte[,]>();
	}
}
