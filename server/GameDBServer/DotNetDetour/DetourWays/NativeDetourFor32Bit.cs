using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DotNetDetour.DetourWays
{
	// Token: 0x0200000C RID: 12
	public class NativeDetourFor32Bit : IDetour
	{
		// Token: 0x06000034 RID: 52 RVA: 0x00003238 File Offset: 0x00001438
		public unsafe virtual void Patch(MethodInfo src, MethodInfo dest, MethodInfo ori)
		{
			RuntimeTypeHandle[] typeHandles = (from t in src.DeclaringType.GetGenericArguments()
			select t.TypeHandle).ToArray<RuntimeTypeHandle>();
			RuntimeHelpers.PrepareMethod(src.MethodHandle, typeHandles);
			this.srcPtr = (byte*)src.MethodHandle.GetFunctionPointer().ToPointer();
			byte* destPtr = (byte*)dest.MethodHandle.GetFunctionPointer().ToPointer();
			if (ori != null)
			{
				this.CreateOriginalMethod(ori);
			}
			fixed (byte* newInstrPtr = this.newInstrs)
			{
				*(int*)((byte*)newInstrPtr + 1) = (int)(destPtr - this.srcPtr - 5U);
			}
			this.Patch();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003314 File Offset: 0x00001514
		protected unsafe virtual void Patch()
		{
			uint oldProtect;
			NativeAPI.VirtualProtect((IntPtr)((void*)this.srcPtr), 5U, Protection.PAGE_EXECUTE_READWRITE, out oldProtect);
			for (int i = 0; i < this.newInstrs.Length; i++)
			{
				this.srcPtr[i] = this.newInstrs[i];
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003364 File Offset: 0x00001564
		protected unsafe virtual void CreateOriginalMethod(MethodInfo method)
		{
			uint needSize = LDasm.SizeofMin5Byte((void*)this.srcPtr);
			int total_length = (int)(needSize + 5U);
			byte[] code = new byte[total_length];
			IntPtr ptr = Marshal.AllocHGlobal(total_length);
			int i = 0;
			while ((long)i < (long)((ulong)needSize))
			{
				code[i] = this.srcPtr[i];
				i++;
			}
			code[(int)((UIntPtr)needSize)] = 233;
			fixed (byte* p = &code[(int)((UIntPtr)(needSize + 1U))])
			{
				*(int*)p = (int)(this.srcPtr - (uint)((int)ptr) - 5U);
			}
			Marshal.Copy(code, 0, ptr, total_length);
			uint oldProtect;
			NativeAPI.VirtualProtect(ptr, (uint)total_length, Protection.PAGE_EXECUTE_READWRITE, out oldProtect);
			RuntimeHelpers.PrepareMethod(method.MethodHandle);
			*(int*)((byte*)method.MethodHandle.Value.ToPointer() + 8) = (int)ptr;
		}

		// Token: 0x04000013 RID: 19
		protected byte[] newInstrs = new byte[]
		{
			233,
			144,
			144,
			144,
			144
		};

		// Token: 0x04000014 RID: 20
		protected unsafe byte* srcPtr;
	}
}
