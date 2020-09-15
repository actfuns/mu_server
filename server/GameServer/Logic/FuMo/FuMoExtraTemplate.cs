using System;
using System.Collections.Generic;

namespace GameServer.Logic.FuMo
{
	// Token: 0x020002B8 RID: 696
	public class FuMoExtraTemplate
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000AD4 RID: 2772 RVA: 0x000AB600 File Offset: 0x000A9800
		// (set) Token: 0x06000AD5 RID: 2773 RVA: 0x000AB617 File Offset: 0x000A9817
		public int ID { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000AD6 RID: 2774 RVA: 0x000AB620 File Offset: 0x000A9820
		// (set) Token: 0x06000AD7 RID: 2775 RVA: 0x000AB637 File Offset: 0x000A9837
		public string Name { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000AD8 RID: 2776 RVA: 0x000AB640 File Offset: 0x000A9840
		// (set) Token: 0x06000AD9 RID: 2777 RVA: 0x000AB657 File Offset: 0x000A9857
		public List<int> Condition { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000ADA RID: 2778 RVA: 0x000AB660 File Offset: 0x000A9860
		// (set) Token: 0x06000ADB RID: 2779 RVA: 0x000AB677 File Offset: 0x000A9877
		public string Type { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000ADC RID: 2780 RVA: 0x000AB680 File Offset: 0x000A9880
		// (set) Token: 0x06000ADD RID: 2781 RVA: 0x000AB697 File Offset: 0x000A9897
		public Dictionary<double, double> Parameter { get; set; }
	}
}
