using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace Server.Data
{
	
	public class CompressdGoodsDataList : List<GoodsData>, ICompressed
	{
		
		public CompressdGoodsDataList(List<GoodsData> list) : base(list)
		{
		}
	}
}
