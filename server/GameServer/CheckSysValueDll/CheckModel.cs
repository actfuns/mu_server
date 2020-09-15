using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.CSharp.RuntimeBinder;

namespace CheckSysValueDll
{
	// Token: 0x020008E9 RID: 2281
	public class CheckModel
	{
		// Token: 0x060041E2 RID: 16866 RVA: 0x003C3044 File Offset: 0x003C1244
		private static List<SeachData> CopySeachList(List<SeachData> data)
		{
			List<SeachData> tempList = new List<SeachData>();
			tempList.AddRange(data);
			return tempList;
		}

		// Token: 0x060041E3 RID: 16867 RVA: 0x003C3068 File Offset: 0x003C1268
		private static bool IsIEnumerable(object model)
		{
			return model is IEnumerable<object>;
		}

		// Token: 0x060041E4 RID: 16868 RVA: 0x003C3084 File Offset: 0x003C1284
		private static bool IsList(object model)
		{
			return model.GetType().Name.Equals(typeof(List<object>).Name);
		}

		// Token: 0x060041E5 RID: 16869 RVA: 0x003C30B8 File Offset: 0x003C12B8
		private static bool IsArray(object model)
		{
			return model.GetType().Name.IndexOf("[]") > -1;
		}

		// Token: 0x060041E6 RID: 16870 RVA: 0x003C30E4 File Offset: 0x003C12E4
		private static bool IsDict(object model)
		{
			return typeof(Dictionary<object, object>).Name.Equals(model.GetType().Name);
		}

		// Token: 0x060041E7 RID: 16871 RVA: 0x003C3118 File Offset: 0x003C1318
		public static string Data2Json(object model)
		{
			string strJson = "err";
			try
			{
				DataContractJsonSerializer jss = new DataContractJsonSerializer(model.GetType());
				using (MemoryStream stream = new MemoryStream())
				{
					jss.WriteObject(stream, model);
					return Encoding.UTF8.GetString(stream.ToArray());
				}
			}
			catch
			{
			}
			finally
			{
				strJson = CheckModel.Data2Json1(model);
			}
			return strJson;
		}

		// Token: 0x060041E8 RID: 16872 RVA: 0x003C31B0 File Offset: 0x003C13B0
		private static string Data2Json1(object model)
		{
			string strJson = "err";
			try
			{
				JavaScriptSerializer jss = new JavaScriptSerializer();
				return jss.Serialize(model);
			}
			catch (Exception ex)
			{
				strJson = ex.ToString();
			}
			return strJson;
		}

		// Token: 0x060041E9 RID: 16873 RVA: 0x003C31F8 File Offset: 0x003C13F8
		public static CheckValueResult GetValue(GetValueModel model, Assembly assembly, bool isFirst = false)
		{
			CheckValueResult resultData = new CheckValueResult();
			resultData.Info = "";
			try
			{
				object objData = RelationMapModel.GetObject(assembly, model.TypeName, model.SeachName, ref resultData);
				if (!string.IsNullOrEmpty(resultData.Info))
				{
					return resultData;
				}
				CheckModel._getValueData(objData, model.SeachDataList, string.Format("{0}->[{1}]->", model.TypeName, model.SeachName), ref resultData, true);
			}
			catch (Exception ex)
			{
				resultData.Info = ex.ToString();
			}
			return resultData;
		}

		// Token: 0x060041EA RID: 16874 RVA: 0x003C3294 File Offset: 0x003C1494
		private static bool _comparer(string d1, string d2, SeachValueType type)
		{
			bool result;
			if (SeachValueType.Less == type)
			{
				result = (Convert.ToDouble(d1) > Convert.ToDouble(d2));
			}
			else if (SeachValueType.Greater == type)
			{
				result = (Convert.ToDouble(d1) < Convert.ToDouble(d2));
			}
			else if (SeachValueType.Equal == type)
			{
				result = d1.Equals(d2);
			}
			else
			{
				result = (SeachValueType.NoEqual == type && !d1.Equals(d2));
			}
			return result;
		}

		// Token: 0x060041EB RID: 16875 RVA: 0x003C3310 File Offset: 0x003C1510
		private static bool _canAdd(object data, string[] files, ref CheckValueResult resultData)
		{
			bool result;
			if (null == data)
			{
				result = false;
			}
			else if (null == files)
			{
				result = true;
			}
			else
			{
				Type dataType = data.GetType();
				foreach (FieldInfo info in dataType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (string item in files)
					{
						string[] file = item.Split(new char[]
						{
							','
						});
						string value = file[1];
						int operation = Convert.ToInt32(file[2]);
						string name = file[0];
						if (string.IsNullOrEmpty(file[0]))
						{
							string temp;
							if (typeof(short) == dataType || typeof(int) == dataType || typeof(long) == dataType || typeof(double) == dataType)
							{
								temp = Convert.ToDouble(data).ToString();
							}
							else if (typeof(bool) == dataType)
							{
								temp = Convert.ToInt32(data).ToString();
							}
							else
							{
								if (!(typeof(string) == dataType))
								{
									resultData.Info = string.Concat(new object[]
									{
										"筛选条件 不对",
										name,
										value,
										operation
									});
									return false;
								}
								temp = data.ToString();
							}
							return CheckModel._comparer(value, temp, (SeachValueType)operation);
						}
						if (info.Name.Equals(name))
						{
							object obj = info.GetValue(data);
							if (typeof(bool) == obj.GetType())
							{
								obj = Convert.ToInt32(obj);
							}
							if (CheckModel._comparer(value, obj.ToString(), (SeachValueType)operation))
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060041EC RID: 16876 RVA: 0x003C357C File Offset: 0x003C177C
		private static void _checkList(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			FieldInfo info = objData.GetType().GetField("_items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == info)
			{
				resultData.Info = _strResultKey + " _items type.GetField = null";
			}
			else
			{
				object _list = info.GetValue(objData);
				info = objData.GetType().GetField("_size", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (null == info)
				{
					resultData.Info = _strResultKey + " _size type.GetField = null";
				}
				else
				{
					int size = (int)info.GetValue(objData);
					if (size >= 1)
					{
						for (int i = 0; i < size; i++)
						{
							if (CheckModel.<_checkList>o__SiteContainer0.<>p__Site1 == null)
							{
								CheckModel.<_checkList>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
								}));
							}
							object data = CheckModel.<_checkList>o__SiteContainer0.<>p__Site1.Target(CheckModel.<_checkList>o__SiteContainer0.<>p__Site1, _list, i);
							if (CheckModel._canAdd(data, files, ref resultData))
							{
								if (SeachList.Count == 0)
								{
									resultData.AddData(data, _strResultKey);
								}
								else
								{
									CheckModel._getValueData(data, SeachList, _strResultKey, ref resultData, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060041ED RID: 16877 RVA: 0x003C36F0 File Offset: 0x003C18F0
		private static void _checkDict(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			FieldInfo info = objData.GetType().GetField("count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == info)
			{
				resultData.Info = _strResultKey + " count type.GetField = null";
			}
			else
			{
				int size = (int)info.GetValue(objData);
				if (size >= 1)
				{
					if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4 == null)
					{
						CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(CheckModel)));
					}
					foreach (object item in CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4, objData))
					{
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(CheckModel)));
						}
						Func<CallSite, object, string> target = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5.Target;
						CallSite <>p__Site = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5;
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6 = CallSite<Func<CallSite, Type, string, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Format", null, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, Type, string, string, object, object> target2 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6.Target;
						CallSite <>p__Site2 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6;
						Type typeFromHandle = typeof(string);
						string arg = "{0}[key={1}]";
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Key", typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						string strResultKey = target(<>p__Site, target2(<>p__Site2, typeFromHandle, arg, _strResultKey, CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7, item)));
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, object, bool> target3 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8.Target;
						CallSite <>p__Site3 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8;
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea = CallSite<CheckModel.<_checkDict>o__SiteContainer3.<>q__SiteDelegate9>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "_canAdd", null, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsRef, null)
							}));
						}
						if (target3(<>p__Site3, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea, typeof(CheckModel), item, files, ref resultData)))
						{
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, bool> target4 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb.Target;
							CallSite <>p__Siteb = CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb;
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, object, object> target5 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec.Target;
							CallSite <>p__Sitec = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec;
							object arg2 = null;
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							if (!target4(<>p__Siteb, target5(<>p__Sitec, arg2, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited, item))))
							{
								if (SeachList.Count == 0)
								{
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee = CallSite<Action<CallSite, CheckValueResult, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "AddData", null, typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
										}));
									}
									Action<CallSite, CheckValueResult, object, string> target6 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee.Target;
									CallSite <>p__Sitee = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee;
									CheckValueResult arg3 = resultData;
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									target6(<>p__Sitee, arg3, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef, item), strResultKey);
								}
								else
								{
									List<object> list = new List<object>();
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10 == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10 = CallSite<Action<CallSite, List<object>, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", null, typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Action<CallSite, List<object>, object> target7 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10.Target;
									CallSite <>p__Site4 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10;
									List<object> arg4 = list;
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11 == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									target7(<>p__Site4, arg4, CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11, item));
									List<object> value = list;
									List<SeachData> TempList = CheckModel.CopySeachList(SeachList);
									TempList.Insert(0, new SeachData());
									CheckModel._getValueData(value, TempList, strResultKey, ref resultData, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060041EE RID: 16878 RVA: 0x003C3CD4 File Offset: 0x003C1ED4
		private static void _checkEnumerable(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			foreach (object item in (objData as IEnumerable<object>))
			{
				if (CheckModel._canAdd(item, files, ref resultData))
				{
					if (SeachList.Count == 0)
					{
						resultData.AddData(item, _strResultKey);
					}
					else
					{
						CheckModel._getValueData(item, SeachList, _strResultKey, ref resultData, false);
					}
				}
			}
		}

		// Token: 0x060041EF RID: 16879 RVA: 0x003C3D6C File Offset: 0x003C1F6C
		private static void _checkArray(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			resultData.AddData(CheckModel.Data2Json(objData), _strResultKey);
		}

		// Token: 0x060041F0 RID: 16880 RVA: 0x003C3D80 File Offset: 0x003C1F80
		private static void _getValueData(object objData, List<SeachData> SeachList, string _strResultKey, ref CheckValueResult resultData, bool isFirst = false)
		{
			if (null == objData)
			{
				resultData.AddData(null, _strResultKey);
			}
			else if (SeachList == null || SeachList.Count < 1)
			{
				resultData.AddData(objData, _strResultKey);
			}
			else
			{
				Type type = objData.GetType();
				string strResultKey = "";
				List<SeachData> seachList = CheckModel.CopySeachList(SeachList);
				SeachData Seach = seachList[0];
				seachList.RemoveAt(0);
				object infoData;
				if (!string.IsNullOrEmpty(Seach.AttName))
				{
					FieldInfo info = type.GetField(Seach.AttName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == info)
					{
						resultData.Info = Seach.AttName + "type.GetField = null";
						return;
					}
					infoData = info.GetValue(objData);
					if (null == infoData)
					{
						resultData.AddData(null, _strResultKey);
						return;
					}
					Type infoType = infoData.GetType();
				}
				else
				{
					infoData = objData;
				}
				string[] files = null;
				if (!string.IsNullOrEmpty(Seach.SeachVal))
				{
					files = Seach.SeachVal.Split(new char[]
					{
						'|'
					});
				}
				if (CheckModel.IsList(infoData))
				{
					if (!string.IsNullOrEmpty(Seach.AttName))
					{
						strResultKey = string.Format("{0}查找[{1}][list]->", _strResultKey, Seach.AttName);
					}
					else
					{
						strResultKey = _strResultKey + "[list]->";
					}
					if (!string.IsNullOrEmpty(Seach.SeachVal))
					{
						strResultKey = string.Format("{0}筛选[{1}]->", strResultKey, Seach.SeachVal);
					}
					CheckModel._checkList(infoData, ref resultData, files, strResultKey, CheckModel.CopySeachList(seachList));
				}
				else if (CheckModel.IsDict(infoData))
				{
					if (!string.IsNullOrEmpty(Seach.AttName))
					{
						strResultKey = string.Format("{0}查找[{1}][dict]->", _strResultKey, Seach.AttName);
					}
					else
					{
						strResultKey = _strResultKey + "[dict]->";
					}
					if (!string.IsNullOrEmpty(Seach.SeachVal))
					{
						strResultKey = string.Format("{0}筛选[{1}]->", strResultKey, Seach.SeachVal);
					}
					CheckModel._checkDict(infoData, ref resultData, files, strResultKey, CheckModel.CopySeachList(seachList));
				}
				else if (CheckModel.IsIEnumerable(infoData))
				{
					if (!string.IsNullOrEmpty(Seach.AttName))
					{
						strResultKey = string.Format("{0}查找[{1}][Enumerable]->", _strResultKey, Seach.AttName);
					}
					else
					{
						strResultKey = _strResultKey + "[Enumerable]->";
					}
					if (!string.IsNullOrEmpty(Seach.SeachVal))
					{
						strResultKey = string.Format("{0}筛选[{1}]->", strResultKey, Seach.SeachVal);
					}
					CheckModel._checkEnumerable(infoData, ref resultData, files, strResultKey, CheckModel.CopySeachList(seachList));
				}
				else if (CheckModel.IsArray(infoData))
				{
					if (!string.IsNullOrEmpty(Seach.AttName))
					{
						strResultKey = string.Format("{0}查找[{1}][Array]->", _strResultKey, Seach.AttName);
					}
					else
					{
						strResultKey = _strResultKey + "[Array]->";
					}
					if (!string.IsNullOrEmpty(Seach.SeachVal))
					{
						strResultKey = string.Format("{0}筛选[{1}]->", strResultKey, Seach.SeachVal);
					}
					CheckModel._checkArray(infoData, ref resultData, files, strResultKey, CheckModel.CopySeachList(seachList));
				}
				else if (string.IsNullOrEmpty(Seach.AttName) && isFirst)
				{
					resultData.AddData(objData, strResultKey);
				}
				else if (CheckModel._canAdd(infoData, files, ref resultData))
				{
					strResultKey = _strResultKey;
					if (!string.IsNullOrEmpty(Seach.AttName))
					{
						strResultKey = string.Format("{0}查找[{1}]->", _strResultKey, Seach.AttName);
					}
					if (!string.IsNullOrEmpty(Seach.SeachVal))
					{
						strResultKey = string.Format("{0}筛选[{1}]->", strResultKey, Seach.SeachVal);
					}
					if (seachList.Count == 0)
					{
						resultData.AddData(infoData, strResultKey);
					}
					else
					{
						CheckModel._getValueData(infoData, CheckModel.CopySeachList(seachList), strResultKey, ref resultData, false);
					}
				}
			}
		}

		// Token: 0x02000A80 RID: 2688
		[CompilerGenerated]
		private static class <_checkList>o__SiteContainer0
		{
			// Token: 0x040052A9 RID: 21161
			public static CallSite<Func<CallSite, object, int, object>> <>p__Site1;
		}

		// Token: 0x02000A81 RID: 2689
		[CompilerGenerated]
		private static class <_checkDict>o__SiteContainer3
		{
			// Token: 0x040052AA RID: 21162
			public static CallSite<Func<CallSite, object, IEnumerable>> <>p__Site4;

			// Token: 0x040052AB RID: 21163
			public static CallSite<Func<CallSite, object, string>> <>p__Site5;

			// Token: 0x040052AC RID: 21164
			public static CallSite<Func<CallSite, Type, string, string, object, object>> <>p__Site6;

			// Token: 0x040052AD RID: 21165
			public static CallSite<Func<CallSite, object, object>> <>p__Site7;

			// Token: 0x040052AE RID: 21166
			public static CallSite<Func<CallSite, object, bool>> <>p__Site8;

			// Token: 0x040052AF RID: 21167
			public static CallSite<CheckModel.<_checkDict>o__SiteContainer3.<>q__SiteDelegate9> <>p__Sitea;

			// Token: 0x040052B0 RID: 21168
			public static CallSite<Func<CallSite, object, bool>> <>p__Siteb;

			// Token: 0x040052B1 RID: 21169
			public static CallSite<Func<CallSite, object, object, object>> <>p__Sitec;

			// Token: 0x040052B2 RID: 21170
			public static CallSite<Func<CallSite, object, object>> <>p__Sited;

			// Token: 0x040052B3 RID: 21171
			public static CallSite<Action<CallSite, CheckValueResult, object, string>> <>p__Sitee;

			// Token: 0x040052B4 RID: 21172
			public static CallSite<Func<CallSite, object, object>> <>p__Sitef;

			// Token: 0x040052B5 RID: 21173
			public static CallSite<Action<CallSite, List<object>, object>> <>p__Site10;

			// Token: 0x040052B6 RID: 21174
			public static CallSite<Func<CallSite, object, object>> <>p__Site11;

			// Token: 0x02000A82 RID: 2690
			// (Invoke) Token: 0x060045C3 RID: 17859
			public delegate object <>q__SiteDelegate9(CallSite param0, Type param1, dynamic param2, string[] param3, ref CheckValueResult param4);
		}
	}
}
