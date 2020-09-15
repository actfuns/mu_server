using System;
using System.Collections.Generic;

namespace Server.TCP
{
	// Token: 0x020008C6 RID: 2246
	public class FullBufferManager
	{
		// Token: 0x06004006 RID: 16390 RVA: 0x003BA478 File Offset: 0x003B8678
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

		// Token: 0x06004008 RID: 16392 RVA: 0x003BA4E4 File Offset: 0x003B86E4
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

		// Token: 0x06004009 RID: 16393 RVA: 0x003BA550 File Offset: 0x003B8750
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

		// Token: 0x0600400A RID: 16394 RVA: 0x003BA5C8 File Offset: 0x003B87C8
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

		// Token: 0x04004F17 RID: 20247
		public const int Error_SendTimeOut = 0;

		// Token: 0x04004F18 RID: 20248
		public const int Error_BufferFull = 1;

		// Token: 0x04004F19 RID: 20249
		public const int Error_DiscardBigPacket = 2;

		// Token: 0x04004F1A RID: 20250
		private Dictionary<TMSKSocket, int> ErrorDict = new Dictionary<TMSKSocket, int>();

		// Token: 0x04004F1B RID: 20251
		private List<int> ListError = new List<int>();
	}
}
