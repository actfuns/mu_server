using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetDetour
{
    
    public static class AssemblyUtil
    {
        
        public static T CreateInstance<T>(string type)
        {
            return AssemblyUtil.CreateInstance<T>(type, new object[0]);
        }

        
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

        
        public static IEnumerable<Type> GetImplementTypes<TBaseType>(this Assembly assembly)
        {
            return from t in assembly.GetExportedTypes()
                   where t.IsSubclassOf(typeof(TBaseType)) && t.IsClass && !t.IsAbstract
                   select t;
        }

        
        public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly) where TBaseInterface : class
        {
            return assembly.GetImplementedObjectsByInterface<TBaseInterface>(typeof(TBaseInterface));
        }

        
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

        
        public static IEnumerable<Assembly> GetAssembliesFromString(string assemblyDef)
        {
            return AssemblyUtil.GetAssembliesFromStrings(assemblyDef.Split(new char[]
            {
                ',',
                ';'
            }, StringSplitOptions.RemoveEmptyEntries));
        }

        
        public static IEnumerable<Assembly> GetAssembliesFromStrings(string[] assemblies)
        {
            List<Assembly> result = new List<Assembly>(assemblies.Length);
            foreach (string a in assemblies)
            {
                result.Add(Assembly.Load(a));
            }
            return result;
        }

        
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

        
        private static object[] m_EmptyObjectArray = new object[0];
    }
}
