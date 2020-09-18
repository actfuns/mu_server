using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetDetour
{
	
	public class Monitor
	{
		
		public static void InstallEx(params Assembly[] assemblies)
		{
			lock (Monitor.Mutex)
			{
				List<DestAndOri> destAndOris = new List<DestAndOri>();
				List<IMethodMonitor> monitors = new List<IMethodMonitor>();
				foreach (Assembly a in assemblies)
				{
					monitors.AddRange(a.GetImplementedObjectsByInterface<IMethodMonitor>());
				}
				foreach (IMethodMonitor monitor in monitors)
				{
					DestAndOri destAndOri = new DestAndOri();
					destAndOri.Dest = monitor.GetType().GetMethods().FirstOrDefault((MethodInfo t) => t.GetCustomAttributes(typeof(MonitorAttribute), false).Length > 0);
					destAndOri.Ori = monitor.GetType().GetMethods().FirstOrDefault((MethodInfo t) => t.GetCustomAttributes(typeof(OriginalAttribute), false).Length > 0);
					if (!(destAndOri.Dest == null) && !(destAndOri.Ori == null))
					{
						MethodInfo src = null;
						MethodInfo dest = destAndOri.Dest;
						MonitorAttribute monitorAttribute = dest.GetCustomAttributes(typeof(MonitorAttribute), false).FirstOrDefault<object>() as MonitorAttribute;
						string methodName = dest.Name;
						Type[] paramTypes = (from t in dest.GetParameters()
						select t.ParameterType).ToArray<Type>();
						if (monitorAttribute.Type != null)
						{
							src = monitorAttribute.Type.GetMethod(methodName, paramTypes);
						}
						else
						{
							string srcNamespaceAndClass = monitorAttribute.NamespaceName + "." + monitorAttribute.ClassName;
							foreach (Assembly asm in assemblies)
							{
								Type type = asm.GetType(srcNamespaceAndClass);
								if (type == null)
								{
									type = asm.GetExportedTypes().FirstOrDefault((Type t) => t.FullName == srcNamespaceAndClass);
								}
								if (type != null)
								{
									src = type.GetMethod(methodName, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, paramTypes, null);
									break;
								}
							}
						}
						if (!(src == null))
						{
							destAndOri.Src = src;
							if (destAndOri.Dest != null)
							{
								List<DestAndOri> list;
								if (!Monitor.destAndOrisDict.TryGetValue(destAndOri.Src, out list))
								{
									list = new List<DestAndOri>();
									Monitor.destAndOrisDict[destAndOri.Src] = list;
								}
								if (!list.Contains(destAndOri))
								{
									if (list.Count > 0)
									{
										destAndOri.Src = list.LastOrDefault<DestAndOri>().Dest;
									}
									list.Add(destAndOri);
									destAndOris.Add(destAndOri);
								}
							}
						}
					}
				}
				Monitor.InstallInternalEx(destAndOris);
			}
		}

		
		private static void InstallInternalEx(List<DestAndOri> destAndOris)
		{
			foreach (DestAndOri destAndOri in destAndOris)
			{
				MethodInfo src = destAndOri.Src;
				MethodInfo dest = destAndOri.Dest;
				MethodInfo ori = destAndOri.Ori;
				IDetour engine = DetourFactory.CreateDetourEngine();
				engine.Patch(src, dest, ori);
			}
		}

		
		private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
		}

		
		private static object Mutex = new object();

		
		private static Dictionary<MethodInfo, List<DestAndOri>> destAndOrisDict = new Dictionary<MethodInfo, List<DestAndOri>>();
	}
}
