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
    
    public class CheckModel
    {
        
        private static List<SeachData> CopySeachList(List<SeachData> data)
        {
            List<SeachData> tempList = new List<SeachData>();
            tempList.AddRange(data);
            return tempList;
        }

        
        private static bool IsIEnumerable(object model)
        {
            return model is IEnumerable<object>;
        }

        
        private static bool IsList(object model)
        {
            return model.GetType().Name.Equals(typeof(List<object>).Name);
        }

        
        private static bool IsArray(object model)
        {
            return model.GetType().Name.IndexOf("[]") > -1;
        }

        
        private static bool IsDict(object model)
        {
            return typeof(Dictionary<object, object>).Name.Equals(model.GetType().Name);
        }

        
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

        
        private static void _checkList(dynamic objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
        {
            FieldInfo info = objData.GetType().GetField("_items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (null == info)
            {
                resultData.Info = _strResultKey + " _items type.GetField = null";
            }
            else
            {
                dynamic _list = info.GetValue(objData);
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
                            object data = _list[i];
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

        
        private static void _checkDict(dynamic objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
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
                    foreach (var item in objData)
                    {
                        string arg = "{0}[key={1}]";
                        string strResultKey = string.Format(arg, _strResultKey, item.Key);
                        if (_canAdd(item, files, ref resultData))
                        {
                            if (SeachList.Count == 0)
                            {
                                resultData.AddData(item.Value, _strResultKey);
                            }
                            else
                            {
                                List<object> list = new List<object>();
                                list.Add(item.Value);
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

        
        private static void _checkArray(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
        {
            resultData.AddData(CheckModel.Data2Json(objData), _strResultKey);
        }

        
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
    }
}
