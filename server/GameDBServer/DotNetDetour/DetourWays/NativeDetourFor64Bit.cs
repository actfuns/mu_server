using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetDetour.DetourWays
{
	
	public class NativeDetourFor64Bit : NativeDetourFor32Bit
	{
		
		protected unsafe override void CreateOriginalMethod(MethodInfo method)
		{
			uint needSize = LDasm.SizeofMin5Byte((void*)this.srcPtr);
			byte[] src_instr = new byte[needSize];
			int i = 0;
			while ((long)i < (long)((ulong)needSize))
			{
				src_instr[i] = this.srcPtr[i];
				i++;
			}
			fixed (byte* p = &this.jmp_inst[3])
			{
				*(long*)p = (long)(this.srcPtr + needSize);
			}
			int totalLength = src_instr.Length + this.jmp_inst.Length;
			IntPtr ptr = Marshal.AllocHGlobal(totalLength);
			Marshal.Copy(src_instr, 0, ptr, src_instr.Length);
			Marshal.Copy(this.jmp_inst, 0, ptr + src_instr.Length, this.jmp_inst.Length);
			uint oldProtect;
			NativeAPI.VirtualProtect(ptr, (uint)totalLength, Protection.PAGE_EXECUTE_READWRITE, out oldProtect);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
			*(long*)((byte*)method.MethodHandle.Value.ToPointer() + 8) = (long)ptr;
		}

		
		private byte[] jmp_inst = new byte[]
		{
			80,
			72,
			184,
			144,
			144,
			144,
			144,
			144,
			144,
			144,
			144,
			80,
			72,
			139,
			68,
			36,
			8,
			194,
			8,
			0
		};
	}
}
