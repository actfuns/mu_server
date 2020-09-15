using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetDetour.DetourWays
{
	// Token: 0x0200000D RID: 13
	public class NativeDetourFor64Bit : NativeDetourFor32Bit
	{
		// Token: 0x06000038 RID: 56 RVA: 0x00003430 File Offset: 0x00001630
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
				*(long*)p = this.srcPtr + needSize;
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

		// Token: 0x04000016 RID: 22
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
