using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	
	public class WeaponAdornInfo
	{
		
		public int nOccupationLimit;

		
		public WeaponTypeAndACTInfo tagWeaponTypeInfo = new WeaponTypeAndACTInfo();

		
		public List<WeaponTypeAndACTInfo> listCoexistType = new List<WeaponTypeAndACTInfo>();
	}
}
