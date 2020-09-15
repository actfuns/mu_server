using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	// Token: 0x020007CD RID: 1997
	public class ServerEvents
	{
		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x0600383E RID: 14398 RVA: 0x002FB0C4 File Offset: 0x002F92C4
		// (set) Token: 0x0600383F RID: 14399 RVA: 0x002FB0DB File Offset: 0x002F92DB
		public EventLevels EventLevel { get; set; }

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x06003840 RID: 14400 RVA: 0x002FB0E4 File Offset: 0x002F92E4
		// (set) Token: 0x06003841 RID: 14401 RVA: 0x002FB0FC File Offset: 0x002F92FC
		public string EventRootPath
		{
			get
			{
				return this._EventRootPath;
			}
			set
			{
				this._EventRootPath = value;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x06003842 RID: 14402 RVA: 0x002FB108 File Offset: 0x002F9308
		// (set) Token: 0x06003843 RID: 14403 RVA: 0x002FB120 File Offset: 0x002F9320
		public string EventPreFileName
		{
			get
			{
				return this._EventPreFileName;
			}
			set
			{
				this._EventPreFileName = value;
			}
		}

		// Token: 0x06003844 RID: 14404 RVA: 0x002FB12C File Offset: 0x002F932C
		private string FormatNowTimeString()
		{
			return TimeUtil.NowDataTimeString("yyyy-MM-dd HH:mm:ss");
		}

		// Token: 0x06003845 RID: 14405 RVA: 0x002FB148 File Offset: 0x002F9348
		public void AddEvent(string msg, EventLevels eventLevel)
		{
			if (eventLevel >= this.EventLevel)
			{
				string logMsg = string.Format("{0}\t{1}", this.FormatNowTimeString(), msg);
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(logMsg);
				}
			}
		}

		// Token: 0x06003846 RID: 14406 RVA: 0x002FB1C0 File Offset: 0x002F93C0
		public void AddImporEvent(params object[] list)
		{
			if (EventLevels.Important >= this.EventLevel)
			{
				string logMsg = string.Format("{0}", this.FormatNowTimeString());
				for (int i = 0; i < list.Length; i++)
				{
					logMsg += string.Format("\t{0}", list[i]);
				}
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(logMsg);
				}
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x06003847 RID: 14407 RVA: 0x002FB260 File Offset: 0x002F9460
		public string EventPath
		{
			get
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this._PathLock)
				{
					string yearID = dateTime.ToString("yyyy");
					string monthID = dateTime.ToString("MM");
					string dayID = dateTime.ToString("dd");
					if (this._EventPath == string.Empty || yearID != this._YearID || monthID != this._MonthID || dayID != this._DayID)
					{
						this._YearID = yearID;
						this._MonthID = monthID;
						this._DayID = dayID;
						this._EventPath = DataHelper.CurrentDirectory + string.Format("{0}/", this._EventRootPath);
						try
						{
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}Year_{1}/", this._EventPath, this._YearID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}Month_{1}/", this._EventPath, this._MonthID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
						try
						{
							this._EventPath = string.Format("{0}{1}/", this._EventPath, this._DayID);
							if (!Directory.Exists(this._EventPath))
							{
								Directory.CreateDirectory(this._EventPath);
							}
						}
						catch (Exception)
						{
						}
					}
				}
				return this._EventPath;
			}
		}

		// Token: 0x06003848 RID: 14408 RVA: 0x002FB4CC File Offset: 0x002F96CC
		public bool WriteEvent()
		{
			string msg = null;
			lock (this.EventsQueue)
			{
				if (this.EventsQueue.Count > 0)
				{
					msg = this.EventsQueue.Dequeue();
				}
			}
			bool result;
			if (null == msg)
			{
				result = false;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				try
				{
					if (this._DayOfYearID != dateTime.DayOfYear || this._StreamWriter == null)
					{
						string fileName = string.Format("{0}{1}.log", this.EventPath, this._EventPreFileName);
						if (null != this._StreamWriter)
						{
							this._StreamWriter.Flush();
							this._StreamWriter.Close();
							this._StreamWriter.Dispose();
							this._StreamWriter = null;
						}
						this._StreamWriter = File.AppendText(fileName);
						this._DayOfYearID = dateTime.DayOfYear;
						this._StreamWriter.AutoFlush = true;
					}
					this._StreamWriter.WriteLine(msg);
				}
				catch
				{
					this._HourID = -1;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x0400414C RID: 16716
		private Queue<string> EventsQueue = new Queue<string>();

		// Token: 0x0400414D RID: 16717
		private string _EventRootPath = "events";

		// Token: 0x0400414E RID: 16718
		private string _EventPreFileName = "Event";

		// Token: 0x0400414F RID: 16719
		private string _EventPath = string.Empty;

		// Token: 0x04004150 RID: 16720
		private string _YearID = string.Empty;

		// Token: 0x04004151 RID: 16721
		private string _MonthID = string.Empty;

		// Token: 0x04004152 RID: 16722
		private string _DayID = string.Empty;

		// Token: 0x04004153 RID: 16723
		private object _PathLock = new object();

		// Token: 0x04004154 RID: 16724
		private int _DayOfYearID = -1;

		// Token: 0x04004155 RID: 16725
		private int _HourID = -1;

		// Token: 0x04004156 RID: 16726
		private StreamWriter _StreamWriter = null;
	}
}
