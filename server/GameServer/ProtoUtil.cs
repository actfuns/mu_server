using System;
using System.Collections.Generic;
using System.Text;
using Tmsk.Contract;

// Token: 0x020008F6 RID: 2294
internal class ProtoUtil
{
	// Token: 0x06004257 RID: 16983 RVA: 0x003C8228 File Offset: 0x003C6428
	public static int CalcIntSize(int val)
	{
		int result;
		if (val < 0)
		{
			result = 10;
		}
		else
		{
			int count = 0;
			do
			{
				count++;
			}
			while ((val >>= 7) != 0);
			result = count;
		}
		return result;
	}

	// Token: 0x06004258 RID: 16984 RVA: 0x003C8264 File Offset: 0x003C6464
	public static int GetIntSize(int val, bool calcMember = false, int protoMember = 0, bool useDef = true, int defval = 0)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			if (val < 0)
			{
				result = ntag + 10;
			}
			else
			{
				int count = 0;
				do
				{
					count++;
				}
				while ((val >>= 7) != 0);
				result = ntag + count;
			}
		}
		return result;
	}

	// Token: 0x06004259 RID: 16985 RVA: 0x003C82D8 File Offset: 0x003C64D8
	public static int IntToBytes(byte[] data, int offset, int val)
	{
		int count = 0;
		if (val >= 0)
		{
			do
			{
				count++;
				data[offset++] = (byte)((val & 127) | 128);
			}
			while ((val >>= 7) != 0);
			int num = offset - 1;
			data[num] &= 127;
		}
		else
		{
			data[offset] = (byte)(val | 128);
			data[offset + 1] = (byte)(val >> 7 | 128);
			data[offset + 2] = (byte)(val >> 14 | 128);
			data[offset + 3] = (byte)(val >> 21 | 128);
			data[offset + 4] = (byte)(val >> 28 | 128);
			data[offset + 5] = (data[offset + 6] = (data[offset + 7] = (data[offset + 8] = byte.MaxValue)));
			data[offset + 9] = 1;
			count = 10;
		}
		return count;
	}

	// Token: 0x0600425A RID: 16986 RVA: 0x003C83B4 File Offset: 0x003C65B4
	public static int IntFromBytes(byte[] data, ref int offset, ref int ncount)
	{
		int readPos = offset;
		int count = 0;
		uint value = (uint)data[readPos++];
		if ((value & 128U) == 0U)
		{
			count = 1;
		}
		else
		{
			value &= 127U;
			uint chunk = (uint)data[readPos++];
			value |= (chunk & 127U) << 7;
			if ((chunk & 128U) == 0U)
			{
				count = 2;
			}
			else
			{
				chunk = (uint)data[readPos++];
				value |= (chunk & 127U) << 14;
				if ((chunk & 128U) == 0U)
				{
					count = 3;
				}
				else
				{
					chunk = (uint)data[readPos++];
					value |= (chunk & 127U) << 21;
					if ((chunk & 128U) == 0U)
					{
						count = 4;
					}
					else
					{
						chunk = (uint)data[readPos];
						value |= chunk << 28;
						if ((chunk & 240U) == 0U)
						{
							count = 5;
						}
						else if ((chunk & 240U) == 240U && data[++readPos] == 255 && data[++readPos] == 255 && data[++readPos] == 255 && data[++readPos] == 255 && data[readPos + 1] == 1)
						{
							count = 10;
						}
					}
				}
			}
		}
		offset += count;
		ncount += count;
		if (ProtoUtil.CheckInvalidUnpack && count <= 0)
		{
			throw new Exception("unexcepted field Length: " + count);
		}
		return (int)value;
	}

	// Token: 0x0600425B RID: 16987 RVA: 0x003C8540 File Offset: 0x003C6740
	public static void GetTag(byte[] data, ref int offset, ref int fieldnumber, ref int wt, ref int ncount)
	{
		int tag = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
		fieldnumber = tag >> 3;
		wt = (tag & 7);
	}

	// Token: 0x0600425C RID: 16988 RVA: 0x003C8564 File Offset: 0x003C6764
	public static void IntMemberToBytes(byte[] data, int fieldnumber, ref int offset, int val, bool useDef = true, int defval = 0)
	{
		if (!useDef || val != defval)
		{
			int tag = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.IntToBytes(data, offset, val);
		}
	}

	// Token: 0x0600425D RID: 16989 RVA: 0x003C85A8 File Offset: 0x003C67A8
	public static int IntMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("int member from bytes error, type error!!!");
		}
		return ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
	}

	// Token: 0x0600425E RID: 16990 RVA: 0x003C85D8 File Offset: 0x003C67D8
	public static int GetLongSize(long val, bool calcMember = false, int protoMember = 0, bool useDef = true, long defval = 0L)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			if (val < 0L)
			{
				result = 10 + ntag;
			}
			else
			{
				int count = 0;
				do
				{
					count++;
				}
				while ((val >>= 7) != 0L);
				result = ntag + count;
			}
		}
		return result;
	}

	// Token: 0x0600425F RID: 16991 RVA: 0x003C864C File Offset: 0x003C684C
	private static int LongToBytes(byte[] data, int offset, long val)
	{
		int count = 0;
		if (val >= 0L)
		{
			do
			{
				data[offset++] = (byte)((val & 127L) | 128L);
				count++;
			}
			while ((val >>= 7) != 0L);
			int num = offset - 1;
			data[num] &= 127;
		}
		else
		{
			count = 10;
			data[offset] = (byte)(val | 128L);
			data[offset + 1] = (byte)((int)(val >> 7) | 128);
			data[offset + 2] = (byte)((int)(val >> 14) | 128);
			data[offset + 3] = (byte)((int)(val >> 21) | 128);
			data[offset + 4] = (byte)((int)(val >> 28) | 128);
			data[offset + 5] = (byte)((int)(val >> 35) | 128);
			data[offset + 6] = (byte)((int)(val >> 42) | 128);
			data[offset + 7] = (byte)((int)(val >> 49) | 128);
			data[offset + 8] = (byte)((int)(val >> 56) | 128);
			data[offset + 9] = 1;
		}
		return count;
	}

	// Token: 0x06004260 RID: 16992 RVA: 0x003C8754 File Offset: 0x003C6954
	private static long LongFromBytes(byte[] data, ref int offset, ref int ncount)
	{
		int readPos = offset;
		ulong value = (ulong)data[readPos++];
		int count;
		if ((value & 128UL) == 0UL)
		{
			count = 1;
		}
		else
		{
			value &= 127UL;
			ulong chunk = (ulong)data[readPos++];
			value |= (chunk & 127UL) << 7;
			if ((chunk & 128UL) == 0UL)
			{
				count = 2;
			}
			else
			{
				chunk = (ulong)data[readPos++];
				value |= (chunk & 127UL) << 14;
				if ((chunk & 128UL) == 0UL)
				{
					count = 3;
				}
				else
				{
					chunk = (ulong)data[readPos++];
					value |= (chunk & 127UL) << 21;
					if ((chunk & 128UL) == 0UL)
					{
						count = 4;
					}
					else
					{
						chunk = (ulong)data[readPos++];
						value |= (chunk & 127UL) << 28;
						if ((chunk & 128UL) == 0UL)
						{
							count = 5;
						}
						else
						{
							chunk = (ulong)data[readPos++];
							value |= (chunk & 127UL) << 35;
							if ((chunk & 128UL) == 0UL)
							{
								count = 6;
							}
							else
							{
								chunk = (ulong)data[readPos++];
								value |= (chunk & 127UL) << 42;
								if ((chunk & 128UL) == 0UL)
								{
									count = 7;
								}
								else
								{
									chunk = (ulong)data[readPos++];
									value |= (chunk & 127UL) << 49;
									if ((chunk & 128UL) == 0UL)
									{
										count = 8;
									}
									else
									{
										chunk = (ulong)data[readPos++];
										value |= (chunk & 127UL) << 56;
										if ((chunk & 128UL) == 0UL)
										{
											count = 9;
										}
										else
										{
											chunk = (ulong)data[readPos];
											value |= chunk << 63;
											count = 10;
											if ((chunk & 18446744073709551614UL) != 0UL)
											{
												throw new OverflowException("long parse over flow, sign bit error!");
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		offset += count;
		ncount += count;
		if (ProtoUtil.CheckInvalidUnpack && count <= 0)
		{
			throw new Exception("unexcepted field Length: " + count);
		}
		return (long)value;
	}

	// Token: 0x06004261 RID: 16993 RVA: 0x003C8988 File Offset: 0x003C6B88
	public static void LongMemberToBytes(byte[] data, int fieldnumber, ref int offset, long val, bool useDef = true, long defval = 0L)
	{
		if (!useDef || val != defval)
		{
			int tag = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.LongToBytes(data, offset, val);
		}
	}

	// Token: 0x06004262 RID: 16994 RVA: 0x003C89CC File Offset: 0x003C6BCC
	public static long LongMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("long member from bytes error, type error!!!");
		}
		return ProtoUtil.LongFromBytes(data, ref offset, ref ncount);
	}

	// Token: 0x06004263 RID: 16995 RVA: 0x003C89FC File Offset: 0x003C6BFC
	public static int GetDoubleSize(double val, bool calcMember = false, int protoMember = 0, bool useDef = true, double defval = 0.0)
	{
		int result;
		if (useDef && val == defval)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			result = ntag + 8;
		}
		return result;
	}

	// Token: 0x06004264 RID: 16996 RVA: 0x003C8A44 File Offset: 0x003C6C44
	public static void DoubleMemberToBytes(byte[] data, int fieldnumber, ref int offset, double val, bool useDef = true, double valdef = 0.0)
	{
		if (!useDef || val != valdef)
		{
			int tag = fieldnumber << 3 | 1;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			long value = BitConverter.ToInt64(BitConverter.GetBytes(val), 0);
			data[offset++] = (byte)value;
			data[offset++] = (byte)(value >> 8);
			data[offset++] = (byte)(value >> 16);
			data[offset++] = (byte)(value >> 24);
			data[offset++] = (byte)(value >> 32);
			data[offset++] = (byte)(value >> 40);
			data[offset++] = (byte)(value >> 48);
			data[offset++] = (byte)(value >> 56);
		}
	}

	// Token: 0x06004265 RID: 16997 RVA: 0x003C8B08 File Offset: 0x003C6D08
	public static double DoubleMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 1)
		{
			throw new ArgumentException("double from bytes error, type error!!!");
		}
		long value = (long)((ulong)data[offset++]);
		value |= (long)((long)((ulong)data[offset++]) << 8);
		value |= (long)((long)((ulong)data[offset++]) << 16);
		value |= (long)((long)((ulong)data[offset++]) << 24);
		value |= (long)((long)((ulong)data[offset++]) << 32);
		value |= (long)((long)((ulong)data[offset++]) << 40);
		value |= (long)((long)((ulong)data[offset++]) << 48);
		value |= (long)((long)((ulong)data[offset++]) << 56);
		ncount += 8;
		return BitConverter.ToDouble(BitConverter.GetBytes(value), 0);
	}

	// Token: 0x06004266 RID: 16998 RVA: 0x003C8BCC File Offset: 0x003C6DCC
	public static int GetStringSize(string val, bool calcMember = false, int protoMember = 0)
	{
		int result;
		if (val == null)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			int len = val.Length;
			if (len == 0)
			{
				result = 1 + ntag;
			}
			else
			{
				int predicted = new UTF8Encoding().GetByteCount(val);
				result = ntag + ProtoUtil.CalcIntSize(predicted) + predicted;
			}
		}
		return result;
	}

	// Token: 0x06004267 RID: 16999 RVA: 0x003C8C40 File Offset: 0x003C6E40
	public static void StringMemberToBytes(byte[] data, int fieldnumber, ref int offset, string val)
	{
		if (val != null)
		{
			int tag = fieldnumber << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			int len = val.Length;
			if (len == 0)
			{
				data[offset++] = 0;
			}
			else
			{
				int predicted = new UTF8Encoding().GetByteCount(val);
				offset += ProtoUtil.IntToBytes(data, offset, predicted);
				int actual = new UTF8Encoding().GetBytes(val, 0, val.Length, data, offset);
				offset += predicted;
			}
		}
	}

	// Token: 0x06004268 RID: 17000 RVA: 0x003C8CCC File Offset: 0x003C6ECC
	public static string StringMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 2)
		{
			throw new ArgumentException("string from bytes error, type error!!!");
		}
		int bytes = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
		string result;
		if (bytes == 0)
		{
			result = "";
		}
		else
		{
			string s = new UTF8Encoding().GetString(data, offset, bytes);
			offset += bytes;
			ncount += bytes;
			result = s;
		}
		return result;
	}

	// Token: 0x06004269 RID: 17001 RVA: 0x003C8D2C File Offset: 0x003C6F2C
	public static int GetListBytesSize<T>(List<T> lst, bool calcMember = false, int protoMember = 0) where T : IProtoBuffDataEx
	{
		int result;
		if (lst == null || lst.Count <= 0)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			int count = 0;
			count += ProtoUtil.CalcIntSize(lst.Count);
			int tc = lst.Count;
			for (int i = 0; i < tc; i++)
			{
				T t = lst[i];
				int ts = t.getBytesSize();
				count += ProtoUtil.CalcIntSize(ts);
				count += ts;
			}
			result = ntag + count;
		}
		return result;
	}

	// Token: 0x0600426A RID: 17002 RVA: 0x003C8DD0 File Offset: 0x003C6FD0
	public static byte[] ListToBytes<T>(List<T> lst, int member, ref int offset, byte[] data) where T : IProtoBuffData
	{
		byte[] result;
		if (lst == null || lst.Count <= 0)
		{
			result = data;
		}
		else
		{
			int tag = member << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.IntToBytes(data, offset, lst.Count);
			for (int i = 0; i < lst.Count; i++)
			{
				T t = lst[i];
				byte[] bytes = t.toBytes();
				int ts = ProtoUtil.CalcIntSize(bytes.Length);
				if (bytes.Length + offset + ts > data.Length)
				{
					byte[] data2 = new byte[data.Length * 2];
					Array.Copy(data, data2, data.Length);
					data = data2;
				}
				offset += ProtoUtil.IntToBytes(data, offset, bytes.Length);
				if (bytes.Length > 0)
				{
					Array.Copy(bytes, 0, data, offset, bytes.Length);
					offset += bytes.Length;
				}
			}
			if (offset < data.Length)
			{
				Array.Resize<byte>(ref data, offset);
			}
			result = data;
		}
		return result;
	}

	// Token: 0x0600426B RID: 17003 RVA: 0x003C8EF0 File Offset: 0x003C70F0
	public static void ListMemberFromBytes<T>(List<T> lst, byte[] data, int wt, ref int offset, ref int ncount) where T : class, IProtoBuffData, new()
	{
		if (lst != null && data != null)
		{
			if (wt != 2)
			{
				throw new ArgumentException("list member from bytes error, type error!!!");
			}
			int count = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
			for (int i = 0; i < count; i++)
			{
				int size = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
				T obj = Activator.CreateInstance<T>();
				if (size <= 0)
				{
					lst.Add(obj);
				}
				else
				{
					int newPos = obj.fromBytes(data, offset, size);
					ncount += newPos - offset;
					offset = newPos;
					lst.Add(obj);
				}
			}
		}
	}

	// Token: 0x0600426C RID: 17004 RVA: 0x003C8F9C File Offset: 0x003C719C
	public static int GetListIntBytesSize(List<int> lst, bool calcMember = false, int protoMember = 0)
	{
		int result;
		if (lst == null || lst.Count <= 0)
		{
			result = 0;
		}
		else
		{
			int ntag = 0;
			if (calcMember)
			{
				if (protoMember <= 15)
				{
					ntag = 1;
				}
				else
				{
					ntag = 2;
				}
			}
			int count = 0;
			count += ProtoUtil.CalcIntSize(lst.Count);
			int tc = lst.Count;
			for (int i = 0; i < tc; i++)
			{
				count += ProtoUtil.CalcIntSize(lst[i]);
			}
			result = ntag + count;
		}
		return result;
	}

	// Token: 0x0600426D RID: 17005 RVA: 0x003C9028 File Offset: 0x003C7228
	public static void ListIntToBytes(byte[] data, int member, ref int offset, List<int> lst)
	{
		if (lst != null && lst.Count > 0)
		{
			int tag = member << 3 | 2;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.IntToBytes(data, offset, lst.Count);
			for (int i = 0; i < lst.Count; i++)
			{
				offset += ProtoUtil.IntToBytes(data, offset, lst[i]);
			}
		}
	}

	// Token: 0x0600426E RID: 17006 RVA: 0x003C90A0 File Offset: 0x003C72A0
	public static List<int> ListIntFromBytes(byte[] data, int wt, ref int offset, ref int ncount, List<int> lst)
	{
		List<int> result;
		if (data == null)
		{
			result = lst;
		}
		else
		{
			if (wt != 2)
			{
				throw new ArgumentException("list member from bytes error, type error!!!");
			}
			int count = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
			if (count <= 0)
			{
				result = lst;
			}
			else
			{
				if (lst == null)
				{
					lst = new List<int>(count);
				}
				for (int i = 0; i < count; i++)
				{
					int val = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
					lst.Add(val);
				}
				result = lst;
			}
		}
		return result;
	}

	// Token: 0x0600426F RID: 17007 RVA: 0x003C9134 File Offset: 0x003C7334
	public static int GetDictionaryMemberHeader(int fieldnumber)
	{
		return fieldnumber << 3 | 2;
	}

	// Token: 0x04005028 RID: 20520
	private static bool CheckInvalidUnpack = true;
}
