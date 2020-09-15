using System;
using System.Collections.Generic;
using System.IO;

namespace LogDBServer.Logic
{
	// Token: 0x0200001C RID: 28
	public class ServerEvents
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00005260 File Offset: 0x00003460
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00005277 File Offset: 0x00003477
		public EventLevels EventLevel { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00005280 File Offset: 0x00003480
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00005298 File Offset: 0x00003498
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

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000052A4 File Offset: 0x000034A4
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000052BC File Offset: 0x000034BC
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

		// Token: 0x060000A4 RID: 164 RVA: 0x000052C8 File Offset: 0x000034C8
		public void AddEvent(string msg, EventLevels eventLevel)
		{
			if (eventLevel >= this.EventLevel)
			{
				lock (this.EventsQueue)
				{
					this.EventsQueue.Enqueue(msg);
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00005330 File Offset: 0x00003530
		public string EventPath
		{
			get
			{
				DateTime dateTime = DateTime.Now;
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
						this._EventPath = AppDomain.CurrentDomain.BaseDirectory + string.Format("{0}/", this._EventRootPath);
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

		// Token: 0x060000A6 RID: 166 RVA: 0x000055A0 File Offset: 0x000037A0
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
				DateTime dateTime = DateTime.Now;
				string logMsg = string.Format("{0}{1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss  "), msg);
				try
				{
					string fileName = string.Format("{0}{1}_{2}.log", this.EventPath, this._EventPreFileName, dateTime.ToString("HH"));
					StreamWriter sw = File.AppendText(fileName);
					sw.WriteLine(logMsg);
					sw.Close();
				}
				catch
				{
				}
				result = true;
			}
			return result;
		}

		// Token: 0x04000041 RID: 65
		private Queue<string> EventsQueue = new Queue<string>();

		// Token: 0x04000042 RID: 66
		private string _EventRootPath = "events";

		// Token: 0x04000043 RID: 67
		private string _EventPreFileName = "Event";

		// Token: 0x04000044 RID: 68
		private string _EventPath = string.Empty;

		// Token: 0x04000045 RID: 69
		private string _YearID = string.Empty;

		// Token: 0x04000046 RID: 70
		private string _MonthID = string.Empty;

		// Token: 0x04000047 RID: 71
		private string _DayID = string.Empty;

		// Token: 0x04000048 RID: 72
		private object _PathLock = new object();
	}
}
