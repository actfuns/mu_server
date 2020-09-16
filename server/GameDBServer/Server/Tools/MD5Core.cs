using System;
using System.Text;

namespace Server.Tools
{
	
	public sealed class MD5Core
	{
		
		private MD5Core()
		{
		}

		
		public static byte[] GetHash(string input, Encoding encoding)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			if (null == encoding)
			{
				throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
			}
			byte[] target = encoding.GetBytes(input);
			return MD5Core.GetHash(target);
		}

		
		public static byte[] GetHash(string input)
		{
			return MD5Core.GetHash(input, new UTF8Encoding());
		}

		
		public static string GetHashString(byte[] input)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			string retval = BitConverter.ToString(MD5Core.GetHash(input));
			return retval.Replace("-", "");
		}

		
		public static string GetHashString(string input, Encoding encoding)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			if (null == encoding)
			{
				throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
			}
			byte[] target = encoding.GetBytes(input);
			return MD5Core.GetHashString(target);
		}

		
		public static string GetHashString(string input)
		{
			return MD5Core.GetHashString(input, new UTF8Encoding());
		}

		
		public static byte[] GetHash(byte[] input)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
			}
			ABCDStruct abcd = default(ABCDStruct);
			abcd.A = 1732584193U;
			abcd.B = 4023233417U;
			abcd.C = 2562383102U;
			abcd.D = 271733878U;
			int startIndex;
			for (startIndex = 0; startIndex <= input.Length - 64; startIndex += 64)
			{
				MD5Core.GetHashBlock(input, ref abcd, startIndex);
			}
			return MD5Core.GetHashFinalBlock(input, startIndex, input.Length - startIndex, abcd, (long)input.Length * 8L);
		}

		
		internal static byte[] GetHashFinalBlock(byte[] input, int ibStart, int cbSize, ABCDStruct ABCD, long len)
		{
			byte[] working = new byte[64];
			byte[] length = BitConverter.GetBytes(len);
			Array.Copy(input, ibStart, working, 0, cbSize);
			working[cbSize] = 128;
			if (cbSize < 56)
			{
				Array.Copy(length, 0, working, 56, 8);
				MD5Core.GetHashBlock(working, ref ABCD, 0);
			}
			else
			{
				MD5Core.GetHashBlock(working, ref ABCD, 0);
				working = new byte[64];
				Array.Copy(length, 0, working, 56, 8);
				MD5Core.GetHashBlock(working, ref ABCD, 0);
			}
			byte[] output = new byte[16];
			Array.Copy(BitConverter.GetBytes(ABCD.A), 0, output, 0, 4);
			Array.Copy(BitConverter.GetBytes(ABCD.B), 0, output, 4, 4);
			Array.Copy(BitConverter.GetBytes(ABCD.C), 0, output, 8, 4);
			Array.Copy(BitConverter.GetBytes(ABCD.D), 0, output, 12, 4);
			return output;
		}

		
		internal static void GetHashBlock(byte[] input, ref ABCDStruct ABCDValue, int ibStart)
		{
			uint[] temp = MD5Core.Converter(input, ibStart);
			uint a = ABCDValue.A;
			uint b = ABCDValue.B;
			uint c = ABCDValue.C;
			uint d = ABCDValue.D;
			a = MD5Core.r1(a, b, c, d, temp[0], 7, 3614090360U);
			d = MD5Core.r1(d, a, b, c, temp[1], 12, 3905402710U);
			c = MD5Core.r1(c, d, a, b, temp[2], 17, 606105819U);
			b = MD5Core.r1(b, c, d, a, temp[3], 22, 3250441966U);
			a = MD5Core.r1(a, b, c, d, temp[4], 7, 4118548399U);
			d = MD5Core.r1(d, a, b, c, temp[5], 12, 1200080426U);
			c = MD5Core.r1(c, d, a, b, temp[6], 17, 2821735955U);
			b = MD5Core.r1(b, c, d, a, temp[7], 22, 4249261313U);
			a = MD5Core.r1(a, b, c, d, temp[8], 7, 1770035416U);
			d = MD5Core.r1(d, a, b, c, temp[9], 12, 2336552879U);
			c = MD5Core.r1(c, d, a, b, temp[10], 17, 4294925233U);
			b = MD5Core.r1(b, c, d, a, temp[11], 22, 2304563134U);
			a = MD5Core.r1(a, b, c, d, temp[12], 7, 1804603682U);
			d = MD5Core.r1(d, a, b, c, temp[13], 12, 4254626195U);
			c = MD5Core.r1(c, d, a, b, temp[14], 17, 2792965006U);
			b = MD5Core.r1(b, c, d, a, temp[15], 22, 1236535329U);
			a = MD5Core.r2(a, b, c, d, temp[1], 5, 4129170786U);
			d = MD5Core.r2(d, a, b, c, temp[6], 9, 3225465664U);
			c = MD5Core.r2(c, d, a, b, temp[11], 14, 643717713U);
			b = MD5Core.r2(b, c, d, a, temp[0], 20, 3921069994U);
			a = MD5Core.r2(a, b, c, d, temp[5], 5, 3593408605U);
			d = MD5Core.r2(d, a, b, c, temp[10], 9, 38016083U);
			c = MD5Core.r2(c, d, a, b, temp[15], 14, 3634488961U);
			b = MD5Core.r2(b, c, d, a, temp[4], 20, 3889429448U);
			a = MD5Core.r2(a, b, c, d, temp[9], 5, 568446438U);
			d = MD5Core.r2(d, a, b, c, temp[14], 9, 3275163606U);
			c = MD5Core.r2(c, d, a, b, temp[3], 14, 4107603335U);
			b = MD5Core.r2(b, c, d, a, temp[8], 20, 1163531501U);
			a = MD5Core.r2(a, b, c, d, temp[13], 5, 2850285829U);
			d = MD5Core.r2(d, a, b, c, temp[2], 9, 4243563512U);
			c = MD5Core.r2(c, d, a, b, temp[7], 14, 1735328473U);
			b = MD5Core.r2(b, c, d, a, temp[12], 20, 2368359562U);
			a = MD5Core.r3(a, b, c, d, temp[5], 4, 4294588738U);
			d = MD5Core.r3(d, a, b, c, temp[8], 11, 2272392833U);
			c = MD5Core.r3(c, d, a, b, temp[11], 16, 1839030562U);
			b = MD5Core.r3(b, c, d, a, temp[14], 23, 4259657740U);
			a = MD5Core.r3(a, b, c, d, temp[1], 4, 2763975236U);
			d = MD5Core.r3(d, a, b, c, temp[4], 11, 1272893353U);
			c = MD5Core.r3(c, d, a, b, temp[7], 16, 4139469664U);
			b = MD5Core.r3(b, c, d, a, temp[10], 23, 3200236656U);
			a = MD5Core.r3(a, b, c, d, temp[13], 4, 681279174U);
			d = MD5Core.r3(d, a, b, c, temp[0], 11, 3936430074U);
			c = MD5Core.r3(c, d, a, b, temp[3], 16, 3572445317U);
			b = MD5Core.r3(b, c, d, a, temp[6], 23, 76029189U);
			a = MD5Core.r3(a, b, c, d, temp[9], 4, 3654602809U);
			d = MD5Core.r3(d, a, b, c, temp[12], 11, 3873151461U);
			c = MD5Core.r3(c, d, a, b, temp[15], 16, 530742520U);
			b = MD5Core.r3(b, c, d, a, temp[2], 23, 3299628645U);
			a = MD5Core.r4(a, b, c, d, temp[0], 6, 4096336452U);
			d = MD5Core.r4(d, a, b, c, temp[7], 10, 1126891415U);
			c = MD5Core.r4(c, d, a, b, temp[14], 15, 2878612391U);
			b = MD5Core.r4(b, c, d, a, temp[5], 21, 4237533241U);
			a = MD5Core.r4(a, b, c, d, temp[12], 6, 1700485571U);
			d = MD5Core.r4(d, a, b, c, temp[3], 10, 2399980690U);
			c = MD5Core.r4(c, d, a, b, temp[10], 15, 4293915773U);
			b = MD5Core.r4(b, c, d, a, temp[1], 21, 2240044497U);
			a = MD5Core.r4(a, b, c, d, temp[8], 6, 1873313359U);
			d = MD5Core.r4(d, a, b, c, temp[15], 10, 4264355552U);
			c = MD5Core.r4(c, d, a, b, temp[6], 15, 2734768916U);
			b = MD5Core.r4(b, c, d, a, temp[13], 21, 1309151649U);
			a = MD5Core.r4(a, b, c, d, temp[4], 6, 4149444226U);
			d = MD5Core.r4(d, a, b, c, temp[11], 10, 3174756917U);
			c = MD5Core.r4(c, d, a, b, temp[2], 15, 718787259U);
			b = MD5Core.r4(b, c, d, a, temp[9], 21, 3951481745U);
			ABCDValue.A = a + ABCDValue.A;
			ABCDValue.B = b + ABCDValue.B;
			ABCDValue.C = c + ABCDValue.C;
			ABCDValue.D = d + ABCDValue.D;
		}

		
		private static uint r1(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + MD5Core.LSR(a + ((b & c) | ((b ^ uint.MaxValue) & d)) + x + t, s);
		}

		
		private static uint r2(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + MD5Core.LSR(a + ((b & d) | (c & (d ^ uint.MaxValue))) + x + t, s);
		}

		
		private static uint r3(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + MD5Core.LSR(a + (b ^ c ^ d) + x + t, s);
		}

		
		private static uint r4(uint a, uint b, uint c, uint d, uint x, int s, uint t)
		{
			return b + MD5Core.LSR(a + (c ^ (b | (d ^ uint.MaxValue))) + x + t, s);
		}

		
		private static uint LSR(uint i, int s)
		{
			return i << s | i >> 32 - s;
		}

		
		private static uint[] Converter(byte[] input, int ibStart)
		{
			if (null == input)
			{
				throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
			}
			uint[] result = new uint[16];
			for (int i = 0; i < 16; i++)
			{
				result[i] = (uint)input[ibStart + i * 4];
				result[i] += (uint)((uint)input[ibStart + i * 4 + 1] << 8);
				result[i] += (uint)((uint)input[ibStart + i * 4 + 2] << 16);
				result[i] += (uint)((uint)input[ibStart + i * 4 + 3] << 24);
			}
			return result;
		}
	}
}
