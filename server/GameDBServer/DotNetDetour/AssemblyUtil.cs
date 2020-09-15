using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetDetour
{
    // Token: 0x02000009 RID: 9
    public static class AssemblyUtil
    {
        // Token: 0x06000021 RID: 33 RVA: 0x00002C14 File Offset: 0x00000E14
        public static T CreateInstance<T>(string type)
        {
            return AssemblyUtil.CreateInstance<T>(type, new object[0]);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00002C34 File Offset: 0x00000E34
        public static T CreateInstance<T>(string type, object[] parameters)
        {
            T result = default(T);
            Type instanceType = Type.GetType(type, false, true);
            T result2;
            if (instanceType == null)
            {
                result2 = default(T);
            }
            else
            {
                object instance = Activator.CreateInstance(instanceType, parameters);
                result = (T)((object)instance);
                result2 = result;
            }
            return result2;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00002C88 File Offset: 0x00000E88
        public static T CreateInstance<T>(string assembleName, string type)
        {
            Type instanceType = null;
            T result = default(T);
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assem in asms)
            {
                if (string.Equals(assem.FullName, assembleName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Type[] types = assem.GetTypes();
                    foreach (Type t in types)
                    {
                        if (string.Equals(t.ToString(), type, StringComparison.CurrentCultureIgnoreCase))
                        {
                            instanceType = t;
                            break;
                        }
                    }
                    break;
                }
            }
            T result2;
            if (instanceType == null)
            {
                result2 = default(T);
            }
            else
            {
                object instance = Activator.CreateInstance(instanceType, new object[0]);
                result = (T)((object)instance);
                result2 = result;
            }
            return result2;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00002D74 File Offset: 0x00000F74
        public static T CreateInstance<T>(string assembleName, string type, object[] parameters)
        {
            Type instanceType = null;
            T result = default(T);
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assem in asms)
            {
                if (string.Equals(assem.FullName, assembleName, StringComparison.CurrentCultureIgnoreCase))
                {
                    Type[] types = assem.GetTypes();
                    foreach (Type t in types)
                    {
                        if (string.Equals(t.ToString(), type, StringComparison.CurrentCultureIgnoreCase))
                        {
                            instanceType = t;
                            break;
                        }
                    }
                    break;
                }
            }
            T result2;
            if (instanceType == null)
            {
                result2 = default(T);
            }
            else
            {
                object instance = Activator.CreateInstance(instanceType, parameters);
                result = (T)((object)instance);
                result2 = result;
            }
            return result2;
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00002E94 File Offset: 0x00001094
        public static IEnumerable<Type> GetImplementTypes<TBaseType>(this Assembly assembly)
        {
            return from t in assembly.GetExportedTypes()
                   where t.IsSubclassOf(typeof(TBaseType)) && t.IsClass && !t.IsAbstract
                   select t;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00002EC0 File Offset: 0x000010C0
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly) where TBaseInterface : class
        {
            return assembly.GetImplementedObjectsByInterface<TBaseInterface>(typeof(TBaseInterface));
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00002EE4 File Offset: 0x000010E4
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly, Type targetType) where TBaseInterface : class
        {
            Type[] arrType = assembly.GetExportedTypes();
            List<TBaseInterface> result = new List<TBaseInterface>();
            foreach (Type currentImplementType in arrType)
            {
                if (!currentImplementType.IsAbstract)
                {
                    if (targetType.IsAssignableFrom(currentImplementType))
                    {
                        result.Add((TBaseInterface)((object)Activator.CreateInstance(currentImplementType)));
                    }
                }
            }
            return result;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00002F54 File Offset: 0x00001154
        public static T BinaryClone<T>(this T target)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            T result;
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, target);
                ms.Position = 0L;
                result = (T)((object)formatter.Deserialize(ms));
            }
            return result;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00002FD0 File Offset: 0x000011D0
        public static T CopyPropertiesTo<T>(this T source, T target)
        {
            PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Dictionary<string, PropertyInfo> sourcePropertiesDict = properties.ToDictionary((PropertyInfo p) => p.Name);
            PropertyInfo[] targetProperties = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            int i = 0;
            while (i < targetProperties.Length)
            {
                PropertyInfo p2 = targetProperties[i];
                PropertyInfo sourceProperty;
                if (sourcePropertiesDict.TryGetValue(p2.Name, out sourceProperty))
                {
                    if (!(sourceProperty.PropertyType != p2.PropertyType))
                    {
                        if (sourceProperty.PropertyType.IsSerializable)
                        {
                            p2.SetValue(target, sourceProperty.GetValue(source, AssemblyUtil.m_EmptyObjectArray), AssemblyUtil.m_EmptyObjectArray);
                        }
                    }
                }
                IL_C0:
                i++;
                continue;
                goto IL_C0;
            }
            return target;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x000030B8 File Offset: 0x000012B8
        public static IEnumerable<Assembly> GetAssembliesFromString(string assemblyDef)
        {
            return AssemblyUtil.GetAssembliesFromStrings(assemblyDef.Split(new char[]
            {
                ',',
                ';'
            }, StringSplitOptions.RemoveEmptyEntries));
        }

        // Token: 0x0600002B RID: 43 RVA: 0x000030E8 File Offset: 0x000012E8
        public static IEnumerable<Assembly> GetAssembliesFromStrings(string[] assemblies)
        {
            List<Assembly> result = new List<Assembly>(assemblies.Length);
            foreach (string a in assemblies)
            {
                result.Add(Assembly.Load(a));
            }
            return result;
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00003134 File Offset: 0x00001334
        public static string GetAssembleVer(string filePath)
        {
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(filePath);
            return string.Format(" {0}.{1}.{2}.{3}", new object[]
            {
                fvi.ProductMajorPart,
                fvi.ProductMinorPart,
                fvi.ProductBuildPart,
                fvi.ProductPrivatePart
            });
        }

        // Token: 0x04000012 RID: 18
        private static object[] m_EmptyObjectArray = new object[0];
    }
}
