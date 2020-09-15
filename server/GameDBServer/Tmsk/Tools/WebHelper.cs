using System;
using System.IO;
using System.Net;
using Server.Tools;

namespace Tmsk.Tools
{
	// Token: 0x02000224 RID: 548
	public static class WebHelper
	{
		// Token: 0x06000CED RID: 3309 RVA: 0x000A38EC File Offset: 0x000A1AEC
		public static byte[] RequestByPost(string uri, byte[] bytes, int timeOut = 5000, int readWriteTimeout = 100000)
		{
			int httpStatusCode;
			return WebHelper.RequestByPost(uri, bytes, out httpStatusCode, timeOut, readWriteTimeout);
		}

		// Token: 0x06000CEE RID: 3310 RVA: 0x000A390C File Offset: 0x000A1B0C
		public static byte[] RequestByPost(string uri, byte[] bytes, out int httpStatusCode, int timeOut = 5000, int readWriteTimeout = 100000)
		{
			httpStatusCode = 404;
			byte[] bytes2 = null;
			try
			{
				HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(uri);
				myRequest.Method = "POST";
				myRequest.ContentType = "application/x-www-form-urlencoded";
				myRequest.ContentLength = (long)bytes.Length;
				myRequest.KeepAlive = false;
				myRequest.Timeout = timeOut;
				myRequest.ReadWriteTimeout = readWriteTimeout;
				using (Stream newStream = myRequest.GetRequestStream())
				{
					newStream.Write(bytes, 0, bytes.Length);
					newStream.Close();
					HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
					if (null == myResponse)
					{
						return null;
					}
					httpStatusCode = (int)myResponse.StatusCode;
					if (httpStatusCode == 200)
					{
						bytes2 = WebHelper.GetBytes(myResponse);
					}
					newStream.Close();
					myResponse.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
				bytes2 = null;
			}
			finally
			{
			}
			return bytes2;
		}

		// Token: 0x06000CEF RID: 3311 RVA: 0x000A3A40 File Offset: 0x000A1C40
		private static byte[] GetBytes(HttpWebResponse res)
		{
			byte[] buffer = null;
			try
			{
				Stream rs = res.GetResponseStream();
				try
				{
					MemoryStream memoryStream = new MemoryStream();
					buffer = new byte[256];
					for (int i = rs.Read(buffer, 0, buffer.Length); i > 0; i = rs.Read(buffer, 0, buffer.Length))
					{
						memoryStream.Write(buffer, 0, i);
					}
					buffer = memoryStream.ToArray();
				}
				finally
				{
					rs.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return buffer;
		}
	}
}
