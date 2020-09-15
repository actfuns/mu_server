using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using GameServer.Core.Executor;
using GameServer.Tools;
using ProtoBuf;
using Server.Protocol;
using Tmsk.Contract;

namespace Server.Tools
{
	// Token: 0x020008F3 RID: 2291
	public class DataHelper
	{
		// Token: 0x06004219 RID: 16921 RVA: 0x003C65D8 File Offset: 0x003C47D8
		static DataHelper()
		{
			DataHelper.CurrentDirectory = Directory.GetCurrentDirectory() + "\\";
			byte[] keyBytes = BitConverter.GetBytes(1695843216);
			for (int i = 0; i < keyBytes.Length; i++)
			{
				DataHelper.SortKey += keyBytes[i];
			}
			ulong _SortKey = (ulong)DataHelper.SortKey;
			DataHelper.SortKey64 |= _SortKey;
			DataHelper.SortKey64 |= _SortKey << 8;
			DataHelper.SortKey64 |= _SortKey << 16;
			DataHelper.SortKey64 |= _SortKey << 24;
			DataHelper.SortKey64 |= _SortKey << 32;
			DataHelper.SortKey64 |= _SortKey << 40;
			DataHelper.SortKey64 |= _SortKey << 48;
			DataHelper.SortKey64 |= _SortKey << 56;
		}

		// Token: 0x0600421A RID: 16922 RVA: 0x003C66D8 File Offset: 0x003C48D8
		public static void CopyBytes(byte[] copyTo, int offsetTo, byte[] copyFrom, int offsetFrom, int count)
		{
			Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
		}

		// Token: 0x0600421B RID: 16923 RVA: 0x003C66E8 File Offset: 0x003C48E8
		public unsafe static void SortBytes(byte[] bytesData, int offsetTo, int count, ulong ulKey)
		{
			byte bKey = (byte)ulKey;
			if (count <= 32)
			{
				int tc = offsetTo + count;
				for (int x = offsetTo; x < tc; x++)
				{
					int num = x;
					bytesData[num] ^= bKey;
				}
			}
			else
			{
				int t = count / 8;
				fixed (byte* p = &bytesData[offsetTo])
				{
					ulong* pl = (ulong*)p;
					for (int i = 0; i < t; i++)
					{
						pl[i] ^= ulKey;
					}
				}
				int tc = offsetTo + count;
				for (int x = offsetTo + t * 8; x < tc; x++)
				{
					int num2 = x;
					bytesData[num2] ^= bKey;
				}
			}
		}

		// Token: 0x0600421C RID: 16924 RVA: 0x003C67A8 File Offset: 0x003C49A8
		public static bool CompBytes(byte[] left, byte[] right)
		{
			bool result;
			if (left.Length != right.Length)
			{
				result = false;
			}
			else
			{
				bool ret = true;
				for (int i = 0; i < left.Length; i++)
				{
					if (left[i] != right[i])
					{
						ret = false;
						break;
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600421D RID: 16925 RVA: 0x003C67F4 File Offset: 0x003C49F4
		public static bool CompBytes(byte[] left, byte[] right, int count)
		{
			bool result;
			if (left.Length < count || right.Length < count)
			{
				result = false;
			}
			else
			{
				bool ret = true;
				for (int i = 0; i < count; i++)
				{
					if (left[i] != right[i])
					{
						ret = false;
						break;
					}
				}
				result = ret;
			}
			return result;
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x003C6848 File Offset: 0x003C4A48
		public static void RandBytes(byte[] buffer, int offset, int count)
		{
			long tick = TimeUtil.NOW() * 10000L;
			Random rnd = new Random((int)(tick & (long)((ulong)-1)) | (int)(tick >> 32));
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = (byte)rnd.Next(0, 255);
			}
		}

		// Token: 0x0600421F RID: 16927 RVA: 0x003C6898 File Offset: 0x003C4A98
		public static string Bytes2HexString(byte[] b)
		{
			string ret = "";
			for (int i = 0; i < b.Length; i++)
			{
				ret += ((int)(b[i] & byte.MaxValue)).ToString("X2").ToUpper();
			}
			return ret;
		}

		// Token: 0x06004220 RID: 16928 RVA: 0x003C68EC File Offset: 0x003C4AEC
		public static byte[] HexString2Bytes(string s)
		{
			byte[] result;
			if (s.Length % 2 != 0)
			{
				result = null;
			}
			else
			{
				byte[] bytesData = new byte[s.Length / 2];
				for (int i = 0; i < s.Length / 2; i++)
				{
					string hexstr = s.Substring(i * 2, 2);
					int b = int.Parse(hexstr, NumberStyles.HexNumber) & 255;
					bytesData[i] = (byte)b;
				}
				result = bytesData;
			}
			return result;
		}

		// Token: 0x06004221 RID: 16929 RVA: 0x003C696C File Offset: 0x003C4B6C
		public static void WriteFormatExceptionLog(Exception e, string extMsg, bool showMsgBox, bool finalReport = false)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("应用程序出现了异常[{0}]:\r\n{1}\r\n", finalReport ? 1 : 0, e.Message);
				stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
				if (null != e)
				{
					if (e.InnerException != null)
					{
						stringBuilder.AppendFormat("\r\n {0}", e.InnerException.Message);
					}
					stringBuilder.AppendFormat("\r\n {0}", e.StackTrace);
				}
				LogManager.WriteException(stringBuilder.ToString());
				if (showMsgBox)
				{
					SysConOut.WriteLine(stringBuilder.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004222 RID: 16930 RVA: 0x003C6A2C File Offset: 0x003C4C2C
		public static void WriteExceptionLogEx(Exception ex, string extMsg)
		{
			try
			{
				StackTrace stackTrace = new StackTrace(2, true);
				string logStr = string.Format("{0}\r\n{1}\r\n{2}", extMsg, ex.ToString(), stackTrace.ToString());
				LogManager.WriteException(logStr.ToString());
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004223 RID: 16931 RVA: 0x003C6A80 File Offset: 0x003C4C80
		public static void WriteStackTraceLog(string extMsg)
		{
			try
			{
				StackTrace stackTrace = new StackTrace(1, true);
				string logStr = string.Format("{0}\r\n{1}", extMsg, stackTrace.ToString());
				LogManager.WriteException(logStr);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004224 RID: 16932 RVA: 0x003C6ACC File Offset: 0x003C4CCC
		public static void WriteFormatStackLog(StackTrace stackTrace, string extMsg)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("应   用程序出现了对象锁定超时错误:\r\n", new object[0]);
				stringBuilder.AppendFormat("\r\n 额外信息: {0}", extMsg);
				stringBuilder.AppendFormat("\r\n {0}", stackTrace.ToString());
				LogManager.WriteException(stringBuilder.ToString());
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06004225 RID: 16933 RVA: 0x003C6B38 File Offset: 0x003C4D38
		public static int ConvertToInt32(string str, int defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToInt32(str);
				}
				return defVal;
			}
			catch (Exception)
			{
			}
			return defVal;
		}

		// Token: 0x06004226 RID: 16934 RVA: 0x003C6B84 File Offset: 0x003C4D84
		public static string ConvertToStr(string str, string defVal)
		{
			string result;
			if ("*" != str)
			{
				result = str;
			}
			else
			{
				result = defVal;
			}
			return result;
		}

		// Token: 0x06004227 RID: 16935 RVA: 0x003C6BB0 File Offset: 0x003C4DB0
		public static long ConvertToTicks(string str, long defVal)
		{
			long result;
			if ("*" == str)
			{
				result = defVal;
			}
			else
			{
				str = str.Replace('$', ':');
				try
				{
					DateTime dt;
					if (!DateTime.TryParse(str, out dt))
					{
						return 0L;
					}
					return dt.Ticks / 10000L;
				}
				catch (Exception)
				{
				}
				result = 0L;
			}
			return result;
		}

		// Token: 0x06004228 RID: 16936 RVA: 0x003C6C24 File Offset: 0x003C4E24
		public static long ConvertToTicks(string str)
		{
			try
			{
				DateTime dt;
				if (!DateTime.TryParse(str, out dt))
				{
					return 0L;
				}
				return dt.Ticks / 10000L;
			}
			catch (Exception)
			{
			}
			return 0L;
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x003C6C74 File Offset: 0x003C4E74
		public static long UnixSecondsToTicks(int secs)
		{
			return DataHelper.UnixStartTicks + (long)secs * 1000L;
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x003C6C98 File Offset: 0x003C4E98
		public static long UnixSecondsToTicks(string secs)
		{
			int intSecs = Convert.ToInt32(secs);
			return DataHelper.UnixSecondsToTicks(intSecs);
		}

		// Token: 0x0600422B RID: 16939 RVA: 0x003C6CB8 File Offset: 0x003C4EB8
		public static int UnixSecondsNow()
		{
			long ticks = TimeUtil.NowRealTime();
			return DataHelper.SysTicksToUnixSeconds(ticks);
		}

		// Token: 0x0600422C RID: 16940 RVA: 0x003C6CD8 File Offset: 0x003C4ED8
		public static int SysTicksToUnixSeconds(long ticks)
		{
			long secs = (ticks - DataHelper.UnixStartTicks) / 1000L;
			return (int)secs;
		}

		// Token: 0x0600422D RID: 16941 RVA: 0x003C6CFC File Offset: 0x003C4EFC
		public static TCPOutPacket ObjectToTCPOutPacket<T>(T instance, TCPOutPacketPool pool, int cmdID)
		{
			byte[] bytesCmd = DataHelper.ObjectToBytes<T>(instance);
			return TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd, 0, bytesCmd.Length, cmdID);
		}

		// Token: 0x0600422E RID: 16942 RVA: 0x003C6D24 File Offset: 0x003C4F24
		public static byte[] ObjectToBytes<T>(T instance)
		{
			try
			{
				byte[] bytesCmd;
				if (null == instance)
				{
					bytesCmd = new byte[0];
				}
				else
				{
					if (instance is IProtoBuffData)
					{
						return (instance as IProtoBuffData).toBytes();
					}
					TMSKThreadStaticClass tsc = TMSKThreadStaticClass.GetInstance();
					MemoryStream ms = tsc.PopMemoryStream();
					Serializer.Serialize<T>(ms, instance);
					bytesCmd = new byte[ms.Length];
					ms.Position = 0L;
					ms.Read(bytesCmd, 0, bytesCmd.Length);
					tsc.PushMemoryStream(ms);
				}
				if (bytesCmd.Length > DataHelper.MinZipBytesSize && instance is ICompressed)
				{
					byte[] newBytes = DataHelper.Compress(bytesCmd);
					if (null != newBytes)
					{
						if (newBytes.Length < bytesCmd.Length)
						{
							bytesCmd = newBytes;
						}
					}
				}
				return bytesCmd;
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "将对象转为字节流发生异常:");
			}
			return new byte[0];
		}

		// Token: 0x0600422F RID: 16943 RVA: 0x003C6E50 File Offset: 0x003C5050
		public static T BytesToObject2<T>(byte[] bytesData, int offset, int length, Socket socket) where T : class, IProtoBuffData, new()
		{
			T t = Activator.CreateInstance<T>();
			try
			{
				t.fromBytes(bytesData, offset, length);
				return t;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Data, string.Format("解析客户端发上来的数据{0}异常,IP:{1},数据内容：{2}", t.ToString(), socket.RemoteEndPoint.ToString(), Convert.ToBase64String(bytesData, offset, length)), null, true);
			}
			return default(T);
		}

		// Token: 0x06004230 RID: 16944 RVA: 0x003C6ED4 File Offset: 0x003C50D4
		public static T BytesToObject<T>(byte[] bytesData, int offset, int length)
		{
			T result;
			if (bytesData.Length == 0)
			{
				result = default(T);
			}
			else
			{
				try
				{
					byte[] copyData = new byte[length];
					DataHelper.CopyBytes(copyData, 0, bytesData, offset, length);
					copyData = DataHelper.Uncompress(copyData);
					TMSKThreadStaticClass tsc = TMSKThreadStaticClass.GetInstance();
					MemoryStream ms = tsc.PopMemoryStream();
					ms.Write(copyData, 0, copyData.Length);
					ms.Position = 0L;
					T t = Serializer.Deserialize<T>(ms);
					tsc.PushMemoryStream(ms);
					return t;
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, "将字节数据转为对象发生异常:");
				}
				result = default(T);
			}
			return result;
		}

		// Token: 0x06004231 RID: 16945 RVA: 0x003C6F88 File Offset: 0x003C5188
		public static byte[] Compress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				using (ZOutputStream outZStream = new ZOutputStream(ms, 9))
				{
					outZStream.Write(bytes, 0, bytes.Length);
					outZStream.Flush();
				}
				result = ms.ToArray();
			}
			return result;
		}

		// Token: 0x06004232 RID: 16946 RVA: 0x003C7008 File Offset: 0x003C5208
		public static byte[] Uncompress(byte[] bytes)
		{
			byte[] result;
			if (bytes.Length < 2)
			{
				result = bytes;
			}
			else if (120 != bytes[0])
			{
				result = bytes;
			}
			else if (156 != bytes[1] && 218 != bytes[1])
			{
				result = bytes;
			}
			else
			{
				using (MemoryStream ms = new MemoryStream())
				{
					using (ZOutputStream outZStream = new ZOutputStream(ms))
					{
						outZStream.Write(bytes, 0, bytes.Length);
						outZStream.Flush();
					}
					result = ms.ToArray();
				}
			}
			return result;
		}

		// Token: 0x06004233 RID: 16947 RVA: 0x003C70CC File Offset: 0x003C52CC
		public static byte[] Utf8_2_Unicode(byte[] input)
		{
			List<byte> ret = new List<byte>();
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] >= 240)
				{
					return null;
				}
				if (input[i] >= 224)
				{
					ret.Add((byte)((int)(input[i + 2] & 63) | (int)(input[i + 1] & 3) << 6));
					ret.Add((byte)((int)input[i] << 4 | (input[i + 1] & 60) >> 2));
					i += 2;
				}
				else if (input[i] >= 192)
				{
					ret.Add((byte)((int)(input[i + 1] & 63) | (int)(input[i] & 3) << 6));
					ret.Add((byte)((input[i] & 28) >> 2));
					i++;
				}
				else
				{
					ret.Add(input[i]);
					ret.Add(0);
				}
			}
			return ret.ToArray();
		}

		// Token: 0x06004234 RID: 16948 RVA: 0x003C71B4 File Offset: 0x003C53B4
		public static string ZipStringToBase64(string text)
		{
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					return "";
				}
				byte[] bytes = new UTF8Encoding().GetBytes(text);
				if (bytes.Length > 128)
				{
					bytes = DataHelper.Compress(bytes);
				}
				return Convert.ToBase64String(bytes);
			}
			catch (Exception)
			{
			}
			return "";
		}

		// Token: 0x06004235 RID: 16949 RVA: 0x003C7230 File Offset: 0x003C5430
		public static string UnZipStringToBase64(string base64)
		{
			try
			{
				if (string.IsNullOrEmpty(base64))
				{
					return "";
				}
				byte[] bytes = Convert.FromBase64String(base64);
				bytes = DataHelper.Uncompress(bytes);
				return new UTF8Encoding().GetString(bytes, 0, bytes.Length);
			}
			catch (Exception)
			{
			}
			return "";
		}

		// Token: 0x06004236 RID: 16950 RVA: 0x003C729C File Offset: 0x003C549C
		public static string EncodeBase64(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				str = "null";
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		// Token: 0x06004237 RID: 16951 RVA: 0x003C72D8 File Offset: 0x003C54D8
		public static string DecodeBase64(string base64Str)
		{
			try
			{
				if (!string.IsNullOrEmpty(base64Str))
				{
					byte[] bytes = Convert.FromBase64String(base64Str);
					return Encoding.UTF8.GetString(bytes);
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06004238 RID: 16952 RVA: 0x003C7328 File Offset: 0x003C5528
		public static double GetOffsetSecond(DateTime date)
		{
			return (date - DataHelper.StartDate).TotalSeconds;
		}

		// Token: 0x06004239 RID: 16953 RVA: 0x003C7350 File Offset: 0x003C5550
		public static int GetOffsetDay(DateTime now)
		{
			return (int)(now - DataHelper.StartDate).TotalDays;
		}

		// Token: 0x0600423A RID: 16954 RVA: 0x003C7378 File Offset: 0x003C5578
		public static int GetOffsetDayNow()
		{
			return DataHelper.GetOffsetDay(TimeUtil.NowDateTime());
		}

		// Token: 0x0600423B RID: 16955 RVA: 0x003C7394 File Offset: 0x003C5594
		public static DateTime GetRealDate(int day)
		{
			return DataHelper.StartDate.AddDays((double)day);
		}

		// Token: 0x0600423C RID: 16956 RVA: 0x003C73B4 File Offset: 0x003C55B4
		public static double CaleTwoLongTimeDay(long First, long Second)
		{
			double result;
			if (First > Second)
			{
				result = Math.Round((double)((First - Second) / 86400000L), 4);
			}
			else
			{
				result = Math.Round((double)((Second - First) / 86400000L), 4);
			}
			return result;
		}

		// Token: 0x04005013 RID: 20499
		public static byte SortKey = 0;

		// Token: 0x04005014 RID: 20500
		public static ulong SortKey64 = 0UL;

		// Token: 0x04005015 RID: 20501
		public static int MinZipBytesSize = 1024;

		// Token: 0x04005016 RID: 20502
		public static string CurrentDirectory;

		// Token: 0x04005017 RID: 20503
		private static long UnixStartTicks = TimeUtil.Before1970Ticks;

		// Token: 0x04005018 RID: 20504
		private static DateTime StartDate = new DateTime(2011, 11, 11);
	}
}
