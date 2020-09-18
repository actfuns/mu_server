using System;

namespace GameServer
{
	
	public class GlobalLang
	{
		
		public static string GetLang(LangTexts id, params object[] args)
		{
			return GLang.GetLang((int)id, args);
		}
	}
}
