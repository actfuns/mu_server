using System;
using System.Collections.Generic;
using System.Text;
using Tmsk.Contract;


internal class ProtoUtil
{
	
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

	
	public static void GetTag(byte[] data, ref int offset, ref int fieldnumber, ref int wt, ref int ncount)
	{
		int tag = ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
		fieldnumber = tag >> 3;
		wt = (tag & 7);
	}

	
	public static void IntMemberToBytes(byte[] data, int fieldnumber, ref int offset, int val, bool useDef = true, int defval = 0)
	{
		if (!useDef || val != defval)
		{
			int tag = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.IntToBytes(data, offset, val);
		}
	}

	
	public static int IntMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("int member from bytes error, type error!!!");
		}
		return ProtoUtil.IntFromBytes(data, ref offset, ref ncount);
	}

	
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

	
	public static void LongMemberToBytes(byte[] data, int fieldnumber, ref int offset, long val, bool useDef = true, long defval = 0L)
	{
		if (!useDef || val != defval)
		{
			int tag = fieldnumber << 3;
			offset += ProtoUtil.IntToBytes(data, offset, tag);
			offset += ProtoUtil.LongToBytes(data, offset, val);
		}
	}

	
	public static long LongMemberFromBytes(byte[] data, int wt, ref int offset, ref int ncount)
	{
		if (wt != 0)
		{
			throw new ArgumentException("long member from bytes error, type error!!!");
		}
		return ProtoUtil.LongFromBytes(data, ref offset, ref ncount);
	}

	
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

	
	public static int GetDictionaryMemberHeader(int fieldnumber)
	{
		return fieldnumber << 3 | 2;
	}

	
	private static bool CheckInvalidUnpack = true;
}
