using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server.Tools;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x020000C6 RID: 198
	internal class AssemblyLoader : MarshalByRefObject
	{
		// Token: 0x06000373 RID: 883 RVA: 0x0003C368 File Offset: 0x0003A568
		public void SetDomain(AppDomain arg_domain)
		{
			arg_domain.AssemblyResolve += AssemblyLoader.CurrentDomain_AssemblyResolve;
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0003C380 File Offset: 0x0003A580
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return null;
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000375 RID: 885 RVA: 0x0003C394 File Offset: 0x0003A594
		public string FullName
		{
			get
			{
				return this.assembly.FullName;
			}
		}

		// Token: 0x06000376 RID: 886 RVA: 0x0003C3B4 File Offset: 0x0003A5B4
		public bool LoadAssembly(string assemblyName)
		{
			try
			{
				try
				{
					this.assembly = Assembly.Load(assemblyName);
				}
				catch (Exception ex)
				{
				}
				if (null == this.assembly)
				{
					IEnumerable<string> list = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), assemblyName + ".dll");
					foreach (string path in list)
					{
						try
						{
							this.assembly = Assembly.LoadFile(path);
							if (!(null == this.assembly))
							{
								if (string.Compare(assemblyName, this.assembly.GetName().Name, true) == 0)
								{
									break;
								}
							}
						}
						catch (Exception ex)
						{
						}
					}
				}
				if (null == this.assembly)
				{
					this.assembly = this.assembly;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return false;
			}
			return null != this.assembly;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x0003C514 File Offset: 0x0003A714
		public MethodLoader FindMethod(string fullClassName, string methodName)
		{
			MethodLoader methodLoader = null;
			string strKey = string.Format("{0}.{1}", fullClassName, methodName);
			MethodLoader result;
			if (this.dictMethodLoader.TryGetValue(strKey, out methodLoader))
			{
				result = methodLoader;
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000378 RID: 888 RVA: 0x0003C550 File Offset: 0x0003A750
		public void AddMethod(string fullClassName, string methodName, MethodLoader methodLoader)
		{
			try
			{
				string strKey = string.Format("{0}.{1}", fullClassName, methodName);
				this.dictMethodLoader.Add(strKey, methodLoader);
			}
			catch
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("AssemblyLoader::AddMethod Error, fullClassName={0} methodName={1}", fullClassName, methodName), null, true);
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x0003C5A8 File Offset: 0x0003A7A8
		public bool LoadMethod(string fullClassName, string methodName)
		{
			try
			{
				MethodLoader methodLoader = this.FindMethod(fullClassName, methodName);
				if (null != methodLoader)
				{
					return true;
				}
				methodLoader = new MethodLoader();
				methodLoader.tp = this.assembly.GetType(fullClassName);
				if (null == methodLoader.tp)
				{
					return false;
				}
				methodLoader.method = methodLoader.tp.GetMethod(methodName);
				if (null == methodLoader.method)
				{
					return false;
				}
				methodLoader.obj = Activator.CreateInstance(methodLoader.tp);
				if (null == methodLoader.obj)
				{
					return false;
				}
				this.AddMethod(fullClassName, methodName, methodLoader);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("AssemblyLoader::LoadMethod Error, fullClassName={0}, methodName={1}", fullClassName, methodName), ex, false);
				return false;
			}
			return true;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x0003C690 File Offset: 0x0003A890
		public object Invoke(string fullClassName, string methodName, object[] args)
		{
			MethodLoader methodLoader = null;
			string strKey = string.Format("{0}.{1}", fullClassName, methodName);
			object result;
			if (!this.dictMethodLoader.TryGetValue(strKey, out methodLoader))
			{
				result = null;
			}
			else
			{
				result = methodLoader.method.Invoke(methodLoader.obj, args);
			}
			return result;
		}

		// Token: 0x040004D0 RID: 1232
		private Assembly assembly = null;

		// Token: 0x040004D1 RID: 1233
		private Dictionary<string, MethodLoader> dictMethodLoader = new Dictionary<string, MethodLoader>();
	}
}
