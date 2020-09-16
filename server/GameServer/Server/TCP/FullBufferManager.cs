using System;
using System.Collections.Generic;

namespace Server.TCP
{
	
	public class FullBufferManager
	{
		
		public static string GetErrorStr(int errorCode)
		{
			string failedReason = "未知";
			switch (errorCode)
			{
			case 0:
				failedReason = "发送数据超时";
				break;
			case 1:
				failedReason = "发送缓冲区已经满";
				break;
			case 2:
				failedReason = "缓冲区过半，大数据包被丢弃";
				break;
			}
			return failedReason;
		}

		
		public void Remove(TMSKSocket s)
		{
			if (this.ErrorDict.Count > 0)
			{
				lock (this.ErrorDict)
				{
					this.ErrorDict.Remove(s);
				}
			}
		}

		
		public void Add(TMSKSocket s, int iError)
		{
			lock (this.ErrorDict)
			{
				if (!this.ErrorDict.ContainsKey(s))
				{
					this.ErrorDict.Add(s, iError);
				}
				else
				{
					this.ErrorDict[s] = iError;
				}
			}
		}

		
		public string GetFullBufferInfoStr()
		{
			int numTimerOut = 0;
			int numBufferFull = 0;
			int numDiscardPacket = 0;
			int numOther = 0;
			if (this.ErrorDict.Count > 0)
			{
				this.ListError.Clear();
				lock (this.ErrorDict)
				{
					this.ListError.AddRange(this.ErrorDict.Values);
				}
				using (List<int>.Enumerator enumerator = this.ListError.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 0:
							numTimerOut++;
							break;
						case 1:
							numBufferFull++;
							break;
						case 2:
							numDiscardPacket++;
							break;
						default:
							numOther++;
							break;
						}
					}
				}
			}
			return string.Format("发送超时{0}个, 缓冲区满{1}个, 丢弃大数据包{2}个, 未知{3}个", new object[]
			{
				numTimerOut,
				numBufferFull,
				numDiscardPacket,
				numOther
			});
		}

		
		public const int Error_SendTimeOut = 0;

		
		public const int Error_BufferFull = 1;

		
		public const int Error_DiscardBigPacket = 2;

		
		private Dictionary<TMSKSocket, int> ErrorDict = new Dictionary<TMSKSocket, int>();

		
		private List<int> ListError = new List<int>();
	}
}
