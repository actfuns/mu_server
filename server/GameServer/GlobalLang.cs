using System;

namespace GameServer
{
	// Token: 0x02000004 RID: 4
	public class GlobalLang
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00005928 File Offset: 0x00003B28
		public static string GetLang(LangTexts id, params object[] args)
		{
			return GLang.GetLang((int)id, args);
		}
	}
}
