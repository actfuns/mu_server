using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Server.Tools;

namespace GameServer.Core.AssemblyPatch
{
	
	internal class AssemblyLoader : MarshalByRefObject
	{
		
		public void SetDomain(AppDomain arg_domain)
		{
			arg_domain.AssemblyResolve += AssemblyLoader.CurrentDomain_AssemblyResolve;
		}

		
		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return null;
		}

		
		
		public string FullName
		{
			get
			{
				return this.assembly.FullName;
			}
		}

		
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

		
		private Assembly assembly = null;

		
		private Dictionary<string, MethodLoader> dictMethodLoader = new Dictionary<string, MethodLoader>();
	}
}
