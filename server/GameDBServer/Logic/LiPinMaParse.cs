using System;
using System.Security.Cryptography;
using System.Text;
using Server.Tools;

namespace GameDBServer.Logic
{
	// Token: 0x02000149 RID: 329
	public class LiPinMaParse
	{
		// Token: 0x06000595 RID: 1429 RVA: 0x0002F460 File Offset: 0x0002D660
		private static string GenerateUniqueId()
		{
			long i = 1L;
			foreach (byte b in Guid.NewGuid().ToByteArray())
			{
				i *= (long)(b + 1);
			}
			return string.Format("{0:X2}", i - DateTime.Now.Ticks);
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x0002F4CC File Offset: 0x0002D6CC
		public static string GenerateLiPinMa(int ptid, int ptrepeat, int zoneID)
		{
			string randStr = LiPinMaParse.GenerateUniqueId().Substring(0, 12);
			string lipinma_data = string.Format("NZ{0:000}{1:0}{2:000}{3}", new object[]
			{
				ptid,
				ptrepeat,
				zoneID,
				randStr
			});
			byte[] bytesData = new UTF8Encoding().GetBytes(lipinma_data);
			CRC32 crc32 = new CRC32();
			crc32.update(bytesData);
			uint crc32Val = crc32.getValue() % 255U;
			string str = string.Format("{0:X}", crc32Val);
			return lipinma_data + str;
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0002F56C File Offset: 0x0002D76C
		public static bool ParseLiPinMa(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			ptid = -1;
			ptrepeat = 0;
			zoneID = 0;
			nMaxUseNum = 1;
			int nAddLength = 0;
			if (lipinma.Length > 23)
			{
				nAddLength = 2;
			}
			bool result;
			if (lipinma.Length < 22 + nAddLength || lipinma.Length > 23 + nAddLength)
			{
				result = false;
			}
			else
			{
				lipinma = lipinma.ToUpper();
				if ("NZ" != lipinma.Substring(0, 2))
				{
					result = false;
				}
				else
				{
					string crc32Str = lipinma.Substring(21 + nAddLength, Math.Min(2, lipinma.Length - (21 + nAddLength)));
					int crc32Val = Global.SafeConvertToInt32(crc32Str, 16);
					byte[] bytesData = new UTF8Encoding().GetBytes(lipinma.Substring(0, 21 + nAddLength));
					CRC32 crc32 = new CRC32();
					crc32.update(bytesData);
					uint check_crc32Val = crc32.getValue() % 255U;
					if (crc32Val != (int)check_crc32Val)
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 3));
						ptrepeat = Convert.ToInt32(lipinma.Substring(5, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(6, 3));
						if (nAddLength > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(9, 2));
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0002F6BC File Offset: 0x0002D8BC
		public static bool ParseLiPinMaNX(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			ptid = -1;
			ptrepeat = 0;
			zoneID = 0;
			nMaxUseNum = 1;
			int nAddLength = 0;
			if (lipinma.Length > 24)
			{
				nAddLength = 2;
			}
			bool result;
			if (lipinma.Length < 23 + nAddLength || lipinma.Length > 24 + nAddLength)
			{
				result = false;
			}
			else
			{
				lipinma = lipinma.ToUpper();
				if ("NX" != lipinma.Substring(0, 2))
				{
					result = false;
				}
				else
				{
					string crc32Str = lipinma.Substring(22 + nAddLength, Math.Min(2, lipinma.Length - (22 + nAddLength)));
					int crc32Val = Global.SafeConvertToInt32(crc32Str, 16);
					byte[] bytesData = new UTF8Encoding().GetBytes(lipinma.Substring(0, 22 + nAddLength));
					CRC32 crc32 = new CRC32();
					crc32.update(bytesData);
					uint check_crc32Val = crc32.getValue() % 255U;
					if (crc32Val != (int)check_crc32Val)
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 4));
						ptrepeat = Convert.ToInt32(lipinma.Substring(6, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(7, 3));
						if (nAddLength > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(10, 2));
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0002F80C File Offset: 0x0002DA0C
		public static bool ParseLiPinMa2(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			bool result;
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("lipinma_v1", 0) == 1)
			{
				result = LiPinMaParse.ParseLiPinMa(lipinma, out ptid, out ptrepeat, out zoneID, out nMaxUseNum);
			}
			else
			{
				ptid = -1;
				ptrepeat = 0;
				zoneID = 0;
				nMaxUseNum = 1;
				int nAddLength = 0;
				if (lipinma.Length > 23)
				{
					nAddLength = 2;
				}
				if (lipinma.Length < 22 + nAddLength || lipinma.Length > 23 + nAddLength)
				{
					result = false;
				}
				else
				{
					lipinma = lipinma.ToUpper();
					if ("NZ" != lipinma.Substring(0, 2))
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 3));
						ptrepeat = Convert.ToInt32(lipinma.Substring(5, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(6, 3));
						if (nAddLength > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(9, 2));
						}
						string crc32Str = lipinma.Substring(9 + nAddLength);
						MD5 md5 = MD5.Create();
						byte[] data0 = new byte[25];
						for (int i = 0; i < 5; i++)
						{
							data0[i] = Convert.ToByte(crc32Str.Substring(2 * i + 4, 2), 16);
						}
						data0[5] = 31;
						data0[6] = 22;
						data0[7] = 5;
						data0[8] = 150;
						Array.Copy(BitConverter.GetBytes(ptid), 0, data0, 9, 4);
						Array.Copy(BitConverter.GetBytes(ptrepeat), 0, data0, 13, 4);
						Array.Copy(BitConverter.GetBytes(zoneID), 0, data0, 17, 4);
						Array.Copy(BitConverter.GetBytes(nMaxUseNum), 0, data0, 21, 4);
						byte[] data = md5.ComputeHash(data0);
						for (int i = 0; i < 2; i++)
						{
							if (Convert.ToByte(crc32Str.Substring(2 * i, 2), 16) != data[i])
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0002FA0C File Offset: 0x0002DC0C
		public static bool ParseLiPinMaNX2(string lipinma, out int ptid, out int ptrepeat, out int zoneID, out int nMaxUseNum)
		{
			bool result;
			if (GameDBManager.GameConfigMgr.GetGameConfigItemInt("lipinma_v1", 0) == 1)
			{
				result = LiPinMaParse.ParseLiPinMaNX(lipinma, out ptid, out ptrepeat, out zoneID, out nMaxUseNum);
			}
			else
			{
				ptid = -1;
				ptrepeat = 0;
				zoneID = 0;
				nMaxUseNum = 1;
				int nAddLength = 0;
				if (lipinma.Length > 24)
				{
					nAddLength = 2;
				}
				if (lipinma.Length < 23 + nAddLength || lipinma.Length > 24 + nAddLength)
				{
					result = false;
				}
				else
				{
					lipinma = lipinma.ToUpper();
					if ("NX" != lipinma.Substring(0, 2))
					{
						result = false;
					}
					else
					{
						ptid = Convert.ToInt32(lipinma.Substring(2, 4));
						ptrepeat = Convert.ToInt32(lipinma.Substring(6, 1));
						zoneID = Convert.ToInt32(lipinma.Substring(7, 3));
						if (nAddLength > 0)
						{
							nMaxUseNum = Convert.ToInt32(lipinma.Substring(10, 2));
						}
						string crc32Str = lipinma.Substring(10 + nAddLength);
						MD5 md5 = MD5.Create();
						byte[] data0 = new byte[25];
						for (int i = 0; i < 5; i++)
						{
							data0[i] = Convert.ToByte(crc32Str.Substring(2 * i + 4, 2), 16);
						}
						data0[5] = 31;
						data0[6] = 22;
						data0[7] = 5;
						data0[8] = 150;
						Array.Copy(BitConverter.GetBytes(ptid), 0, data0, 9, 4);
						Array.Copy(BitConverter.GetBytes(ptrepeat), 0, data0, 13, 4);
						Array.Copy(BitConverter.GetBytes(zoneID), 0, data0, 17, 4);
						Array.Copy(BitConverter.GetBytes(nMaxUseNum), 0, data0, 21, 4);
						byte[] data = md5.ComputeHash(data0);
						for (int i = 0; i < 2; i++)
						{
							if (Convert.ToByte(crc32Str.Substring(2 * i, 2), 16) != data[i])
							{
								return false;
							}
						}
						result = true;
					}
				}
			}
			return result;
		}
	}
}
