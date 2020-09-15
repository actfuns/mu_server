using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using ComponentAce.Compression.Libs.zlib;
using ProtoBuf;
using Server.Protocol;

namespace Server.Tools
{
	// Token: 0x02000218 RID: 536
	public class DataHelper
	{
		// Token: 0x06000C8B RID: 3211 RVA: 0x000A0E90 File Offset: 0x0009F090
		static DataHelper()
		{
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

		// Token: 0x06000C8C RID: 3212 RVA: 0x000A0F6E File Offset: 0x0009F16E
		public static void CopyBytes(byte[] copyTo, int offsetTo, byte[] copyFrom, int offsetFrom, int count)
		{
			Array.Copy(copyFrom, offsetFrom, copyTo, offsetTo, count);
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x000A0F80 File Offset: 0x0009F180
		public unsafe static void SortBytes(byte[] bytesData, int offsetTo, int count)
		{
			if (count <= 32)
			{
				int tc = offsetTo + count;
				for (int x = offsetTo; x < tc; x++)
				{
					int num = x;
					bytesData[num] ^= DataHelper.SortKey;
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
						pl[i] ^= DataHelper.SortKey64;
					}
				}
				int tc = offsetTo + count;
				for (int x = offsetTo + t * 8; x < tc; x++)
				{
					int num2 = x;
					bytesData[num2] ^= DataHelper.SortKey;
				}
			}
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x000A1048 File Offset: 0x0009F248
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

		// Token: 0x06000C8F RID: 3215 RVA: 0x000A1094 File Offset: 0x0009F294
		public static void RandBytes(byte[] buffer, int offset, int count)
		{
			long tick = DateTime.Now.Ticks;
			Random rnd = new Random((int)(tick & (long)((ulong)-1)) | (int)(tick >> 32));
			for (int i = 0; i < count; i++)
			{
				buffer[offset + i] = (byte)rnd.Next(0, 255);
			}
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x000A10E8 File Offset: 0x0009F2E8
		public static string Bytes2HexString(byte[] b)
		{
			string ret = "";
			for (int i = 0; i < b.Length; i++)
			{
				ret += ((int)(b[i] & byte.MaxValue)).ToString("X2").ToUpper();
			}
			return ret;
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x000A113C File Offset: 0x0009F33C
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

		// Token: 0x06000C92 RID: 3218 RVA: 0x000A11BC File Offset: 0x0009F3BC
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
					Console.WriteLine(stringBuilder.ToString());
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x000A127C File Offset: 0x0009F47C
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

		// Token: 0x06000C94 RID: 3220 RVA: 0x000A12E8 File Offset: 0x0009F4E8
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

		// Token: 0x06000C95 RID: 3221 RVA: 0x000A1334 File Offset: 0x0009F534
		public static long ConvertToInt64(string str, long defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToInt64(str);
				}
				return defVal;
			}
			catch (Exception)
			{
			}
			return defVal;
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x000A1380 File Offset: 0x0009F580
		public static double ConvertToDouble(string str, double defVal)
		{
			try
			{
				if ("*" != str)
				{
					return Convert.ToDouble(str);
				}
				return defVal;
			}
			catch (Exception)
			{
			}
			return defVal;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x000A13CC File Offset: 0x0009F5CC
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

		// Token: 0x06000C98 RID: 3224 RVA: 0x000A13F8 File Offset: 0x0009F5F8
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

		// Token: 0x06000C99 RID: 3225 RVA: 0x000A146C File Offset: 0x0009F66C
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

		// Token: 0x06000C9A RID: 3226 RVA: 0x000A14BC File Offset: 0x0009F6BC
		public static long UnixSecondsToTicks(int secs)
		{
			return DataHelper.UnixStartTicks + (long)secs * 1000L;
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x000A14E0 File Offset: 0x0009F6E0
		public static long UnixSecondsToTicks(string secs)
		{
			int intSecs = Convert.ToInt32(secs);
			return DataHelper.UnixSecondsToTicks(intSecs);
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x000A1500 File Offset: 0x0009F700
		public static int UnixSecondsNow()
		{
			long ticks = DateTime.Now.Ticks / 10000L;
			return DataHelper.SysTicksToUnixSeconds(ticks);
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x000A1530 File Offset: 0x0009F730
		public static int SysTicksToUnixSeconds(long ticks)
		{
			long secs = (ticks - DataHelper.UnixStartTicks) / 1000L;
			return (int)secs;
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x000A1554 File Offset: 0x0009F754
		public static TCPOutPacket ObjectToTCPOutPacket<T>(T instance, TCPOutPacketPool pool, int cmdID)
		{
			byte[] bytesCmd = DataHelper.ObjectToBytes<T>(instance);
			return TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd, 0, bytesCmd.Length, cmdID);
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x000A157C File Offset: 0x0009F77C
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
					MemoryStream ms = new MemoryStream();
					Serializer.Serialize<T>(ms, instance);
					bytesCmd = new byte[ms.Length];
					ms.Position = 0L;
					ms.Read(bytesCmd, 0, bytesCmd.Length);
					ms.Dispose();
				}
				if (bytesCmd.Length > DataHelper.MinZipBytesSize)
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
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
			return new byte[0];
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x000A1658 File Offset: 0x0009F858
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
					MemoryStream ms = new MemoryStream();
					ms.Write(copyData, 0, copyData.Length);
					ms.Position = 0L;
					T t = Serializer.Deserialize<T>(ms);
					ms.Dispose();
					return t;
				}
				catch (Exception ex)
				{
				}
				result = default(T);
			}
			return result;
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x000A16F4 File Offset: 0x0009F8F4
		public static string ObjectToHexString<T>(T instance)
		{
			byte[] bytes = DataHelper.ObjectToBytes<T>(instance);
			string result;
			if (bytes == null || bytes.Length == 0)
			{
				result = "null";
			}
			else
			{
				StringBuilder sb = new StringBuilder(bytes.Length * 2 + 2);
				sb.Append("0x");
				foreach (byte b in bytes)
				{
					sb.Append(b.ToString("X2"));
				}
				result = sb.ToString();
			}
			return result;
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x000A1784 File Offset: 0x0009F984
		public static byte[] Compress(byte[] bytes)
		{
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				using (ZOutputStream outZStream = new ZOutputStream(ms, -1))
				{
					outZStream.Write(bytes, 0, bytes.Length);
					outZStream.Flush();
				}
				result = ms.ToArray();
			}
			return result;
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x000A1800 File Offset: 0x0009FA00
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

		// Token: 0x06000CA4 RID: 3236 RVA: 0x000A18C4 File Offset: 0x0009FAC4
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

		// Token: 0x06000CA5 RID: 3237 RVA: 0x000A19AC File Offset: 0x0009FBAC
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

		// Token: 0x06000CA6 RID: 3238 RVA: 0x000A1A20 File Offset: 0x0009FC20
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

		// Token: 0x0400122A RID: 4650
		private static byte SortKey = 0;

		// Token: 0x0400122B RID: 4651
		private static ulong SortKey64 = 0UL;

		// Token: 0x0400122C RID: 4652
		private static long UnixStartTicks = DataHelper.ConvertToTicks("1970-01-01 08:00");

		// Token: 0x0400122D RID: 4653
		public static int MinZipBytesSize = 65536;
	}
}
