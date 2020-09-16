using System;
using System.IO;
using System.Net;
using Server.Tools;

namespace Tmsk.Tools
{
	
	public static class WebHelper
	{
		
		public static byte[] RequestByPost(string uri, byte[] bytes, int timeOut = 5000, int readWriteTimeout = 100000)
		{
			int httpStatusCode;
			return WebHelper.RequestByPost(uri, bytes, out httpStatusCode, timeOut, readWriteTimeout);
		}

		
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
