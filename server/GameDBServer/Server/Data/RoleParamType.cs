using System;

namespace Server.Data
{
	
	public class RoleParamType
	{
		
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

		
		public readonly string VarName;

		
		public readonly string ParamName;

		
		public readonly string TableName;

		
		public readonly string IdxName;

		
		public readonly string ColumnName;

		
		public readonly int ParamIndex;

		
		public readonly int IdxKey;

		
		public readonly int Type;

		
		public readonly string KeyString;

		
		public enum ValueTypes
		{
			
			Normal,
			
			Char128,
			
			Long
		}
	}
}
