using System;
using System.Collections.Generic;
using System.IO;
using GameDBServer.Core;

namespace GameDBServer.Logic
{
	// Token: 0x020001D8 RID: 472
	public class ServerEvents
	{
		// Token: 0x170000D9 RID: 217
		
		
		public EventLevels EventLevel { get; set; }

		// Token: 0x170000DA RID: 218
		
		
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

		// Token: 0x170000DB RID: 219
		
		
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

		// Token: 0x060009EC RID: 2540 RVA: 0x0005F15C File Offset: 0x0005D35C
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

		// Token: 0x170000DC RID: 220
		
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

		// Token: 0x060009EE RID: 2542 RVA: 0x0005F434 File Offset: 0x0005D634
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
				string logMsg = string.Format("{0}{1}", dateTime.ToString("yyyy-MM-dd HH:mm:ss  "), msg);
				try
				{
					if (this._HourID != dateTime.Hour || this._StreamWriter == null)
					{
						string fileName = string.Format("{0}{1}_{2}.log", this.EventPath, this._EventPreFileName, dateTime.ToString("HH"));
						if (null != this._StreamWriter)
						{
							this._StreamWriter.Flush();
							this._StreamWriter.Close();
							this._StreamWriter.Dispose();
							this._StreamWriter = null;
						}
						this._StreamWriter = File.AppendText(fileName);
						this._HourID = dateTime.Hour;
						this._StreamWriter.AutoFlush = true;
					}
					this._StreamWriter.WriteLine(logMsg);
				}
				catch
				{
					this._HourID = -1;
				}
				result = true;
			}
			return result;
		}

		// Token: 0x04000C0B RID: 3083
		private Queue<string> EventsQueue = new Queue<string>();

		// Token: 0x04000C0C RID: 3084
		private string _EventRootPath = "events";

		// Token: 0x04000C0D RID: 3085
		private string _EventPreFileName = "Event";

		// Token: 0x04000C0E RID: 3086
		private string _EventPath = string.Empty;

		// Token: 0x04000C0F RID: 3087
		private string _YearID = string.Empty;

		// Token: 0x04000C10 RID: 3088
		private string _MonthID = string.Empty;

		// Token: 0x04000C11 RID: 3089
		private string _DayID = string.Empty;

		// Token: 0x04000C12 RID: 3090
		private object _PathLock = new object();

		// Token: 0x04000C13 RID: 3091
		private int _HourID = -1;

		// Token: 0x04000C14 RID: 3092
		private StreamWriter _StreamWriter = null;
	}
}
