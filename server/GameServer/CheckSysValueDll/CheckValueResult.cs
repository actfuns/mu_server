using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;

namespace CheckSysValueDll
{
	// Token: 0x020008EC RID: 2284
	[ProtoContract]
	public class CheckValueResult
	{
		// Token: 0x060041FD RID: 16893 RVA: 0x003C541A File Offset: 0x003C361A
		public CheckValueResult()
		{
			this.Info = "";
			this.ResultDict = new Dictionary<string, List<CheckValueResultItem>>();
		}

		// Token: 0x060041FE RID: 16894 RVA: 0x003C543C File Offset: 0x003C363C
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

		// Token: 0x060041FF RID: 16895 RVA: 0x003C54E4 File Offset: 0x003C36E4
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

		// Token: 0x04004FFE RID: 20478
		[ProtoMember(1)]
		public Dictionary<string, List<CheckValueResultItem>> ResultDict;

		// Token: 0x04004FFF RID: 20479
		[ProtoMember(2)]
		public string Info;
	}
}
