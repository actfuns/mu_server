using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;

namespace CheckSysValueDll
{
	
	[ProtoContract]
	public class CheckValueResult
	{
		
		public CheckValueResult()
		{
			this.Info = "";
			this.ResultDict = new Dictionary<string, List<CheckValueResultItem>>();
		}

		
		public void AddData(object obj, string StrSeach)
		{
			CheckValueResultItem data = new CheckValueResultItem();
			if (null == obj)
			{
				data.TypeName = "null";
				data.StrValue = "null";
			}
			else
			{
				data.TypeName = obj.GetType().Name;
				data.StrValue = obj;
			}
			CheckValueResult.Data2String(ref data);
			if (this.ResultDict.ContainsKey(StrSeach))
			{
				this.ResultDict[StrSeach].Add(data);
			}
			else
			{
				this.ResultDict.Add(StrSeach, new List<CheckValueResultItem>
				{
					data
				});
			}
		}

		
		private static void Data2String(ref CheckValueResultItem model)
		{
			model.Childs = new List<string>();
			FieldInfo[] infos = model.StrValue.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (infos.Length < 1)
			{
				model.StrValue = CheckModel.Data2Json(model.StrValue);
			}
			else
			{
				foreach (FieldInfo info in infos)
				{
					object data = info.GetValue(model.StrValue);
					model.Childs.Add(string.Format("{0},{1}", info.Name, data));
				}
				model.StrValue = model.StrValue.ToString();
			}
		}

		
		[ProtoMember(1)]
		public Dictionary<string, List<CheckValueResultItem>> ResultDict;

		
		[ProtoMember(2)]
		public string Info;
	}
}
