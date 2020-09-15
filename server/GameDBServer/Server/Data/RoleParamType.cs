using System;

namespace Server.Data
{
	// Token: 0x020000A8 RID: 168
	public class RoleParamType
	{
		// Token: 0x06000188 RID: 392 RVA: 0x0000864C File Offset: 0x0000684C
		public RoleParamType(string varName, string paramName, string tableName, string idxName, string columnName, int idxKey, int paramIndex, int type)
		{
			this.VarName = varName;
			this.ParamName = paramName;
			this.TableName = tableName;
			this.IdxName = idxName;
			this.ColumnName = columnName;
			this.IdxKey = idxKey;
			this.ParamIndex = paramIndex;
			this.Type = type;
			if (this.Type > 0)
			{
				this.KeyString = this.IdxKey.ToString();
			}
			else
			{
				this.KeyString = "'" + this.ParamName + '\'';
			}
		}

		// Token: 0x0400046E RID: 1134
		public readonly string VarName;

		// Token: 0x0400046F RID: 1135
		public readonly string ParamName;

		// Token: 0x04000470 RID: 1136
		public readonly string TableName;

		// Token: 0x04000471 RID: 1137
		public readonly string IdxName;

		// Token: 0x04000472 RID: 1138
		public readonly string ColumnName;

		// Token: 0x04000473 RID: 1139
		public readonly int ParamIndex;

		// Token: 0x04000474 RID: 1140
		public readonly int IdxKey;

		// Token: 0x04000475 RID: 1141
		public readonly int Type;

		// Token: 0x04000476 RID: 1142
		public readonly string KeyString;

		// Token: 0x020000A9 RID: 169
		public enum ValueTypes
		{
			// Token: 0x04000478 RID: 1144
			Normal,
			// Token: 0x04000479 RID: 1145
			Char128,
			// Token: 0x0400047A RID: 1146
			Long
		}
	}
}
