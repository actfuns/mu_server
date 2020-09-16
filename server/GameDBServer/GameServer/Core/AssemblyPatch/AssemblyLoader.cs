using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server.Tools;

namespace GameServer.Core.AssemblyPatch
{
	// Token: 0x02000003 RID: 3
	internal class AssemblyLoader : MarshalByRefObject
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002071 File Offset: 0x00000271
		public void SetDomain(AppDomain arg_domain)
		{
			arg_domain.AssemblyResolve += AssemblyLoader.CurrentDomain_AssemblyResolve;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002088 File Offset: 0x00000288
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return null;
		}

		// Token: 0x17000001 RID: 1
		
		public string FullName
		{
			get
			{
				return this.assembly.FullName;
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000020BC File Offset: 0x000002BC
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

		// Token: 0x06000007 RID: 7 RVA: 0x0000221C File Offset: 0x0000041C
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

		// Token: 0x06000008 RID: 8 RVA: 0x00002258 File Offset: 0x00000458
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

		// Token: 0x06000009 RID: 9 RVA: 0x000022B0 File Offset: 0x000004B0
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

		// Token: 0x0600000A RID: 10 RVA: 0x00002398 File Offset: 0x00000598
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

		// Token: 0x04000004 RID: 4
		private Assembly assembly = null;

		// Token: 0x04000005 RID: 5
		private Dictionary<string, MethodLoader> dictMethodLoader = new Dictionary<string, MethodLoader>();
	}
}
