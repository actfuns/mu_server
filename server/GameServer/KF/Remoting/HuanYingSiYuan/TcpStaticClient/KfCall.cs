using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AutoCSer;
using AutoCSer.BinarySerialize;
using AutoCSer.IOS;
using AutoCSer.Json;
using AutoCSer.Log;
using AutoCSer.Metadata;
using AutoCSer.Net;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpServer;
using AutoCSer.Net.TcpStaticServer;
using KF.Contract;
using KF.Contract.Data;
using Server.Data;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting.HuanYingSiYuan.TcpStaticClient
{
	// Token: 0x02000874 RID: 2164
	public class KfCall
	{
		// Token: 0x06003D10 RID: 15632 RVA: 0x00344028 File Offset: 0x00342228
		public static AutoCSer.Net.TcpStaticServer.Client NewTcpClient(AutoCSer.Net.TcpInternalServer.ServerAttribute attribute, Action<SubArray<byte>> onCustomData, ILog log, Func<AutoCSer.Net.TcpInternalServer.ClientSocketSender, bool> verifyMethod)
		{
			KfCall.TcpClient = new AutoCSer.Net.TcpStaticServer.Client(attribute, onCustomData, log, verifyMethod);
			CommandBase tcpClient = KfCall.TcpClient;
			Type[] array = new Type[7];
			array[0] = typeof(KfCall._p12);
			array[1] = typeof(KfCall._p15);
			array[2] = typeof(KfCall._p18);
			array[3] = typeof(KfCall._p27);
			array[4] = typeof(KfCall._p29);
			array[5] = typeof(KfCall._p30);
			Type[] simpleSerializeTypes = array;
			array = new Type[4];
			array[0] = typeof(KfCall._p6);
			array[1] = typeof(KfCall._p10);
			array[2] = typeof(KfCall._p31);
			Type[] simpleDeSerializeTypes = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p1);
			array[1] = typeof(KfCall._p3);
			array[2] = typeof(KfCall._p5);
			array[3] = typeof(KfCall._p7);
			array[4] = typeof(KfCall._p9);
			array[5] = typeof(KfCall._p11);
			array[6] = typeof(KfCall._p19);
			array[7] = typeof(KfCall._p20);
			array[8] = typeof(KfCall._p22);
			array[9] = typeof(KfCall._p23);
			array[10] = typeof(KfCall._p25);
			Type[] serializeTypes = array;
			array = new Type[12];
			array[0] = typeof(KfCall._p2);
			array[1] = typeof(KfCall._p4);
			array[2] = typeof(KfCall._p8);
			array[3] = typeof(KfCall._p13);
			array[4] = typeof(KfCall._p14);
			array[5] = typeof(KfCall._p16);
			array[6] = typeof(KfCall._p17);
			array[7] = typeof(KfCall._p21);
			array[8] = typeof(KfCall._p24);
			array[9] = typeof(KfCall._p26);
			array[10] = typeof(KfCall._p28);
			Type[] deSerializeTypes = array;
			array = new Type[1];
			Type[] jsonSerializeTypes = array;
			array = new Type[1];
			tcpClient.ClientCompileSerialize(simpleSerializeTypes, simpleDeSerializeTypes, serializeTypes, deSerializeTypes, jsonSerializeTypes, array);
			return KfCall.TcpClient;
		}

		// Token: 0x04004757 RID: 18263
		public static AutoCSer.Net.TcpStaticServer.Client TcpClient;

		// Token: 0x02000875 RID: 2165
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p1
		{
			// Token: 0x04004758 RID: 18264
			public EscapeBattleSyncData p0;
		}

		// Token: 0x02000876 RID: 2166
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p2 : IReturnParameter<EscapeBattleSyncData>
		{
			// Token: 0x170005CB RID: 1483
			
			
			[Preserve(Conditional = true)]
			public EscapeBattleSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004759 RID: 18265
			[AutoCSer.Json.IgnoreMember]
			public EscapeBattleSyncData Ret;
		}

		// Token: 0x02000877 RID: 2167
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p3
		{
			// Token: 0x0400475A RID: 18266
			public EscapeBattleFuBenData p0;

			// Token: 0x0400475B RID: 18267
			public int p1;

			// Token: 0x0400475C RID: 18268
			public int p2;

			// Token: 0x0400475D RID: 18269
			public int p3;
		}

		// Token: 0x02000878 RID: 2168
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p4 : IReturnParameter<int>
		{
			// Token: 0x170005CC RID: 1484
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x0400475E RID: 18270
			public EscapeBattleFuBenData p0;

			// Token: 0x0400475F RID: 18271
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		// Token: 0x02000879 RID: 2169
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p5
		{
			// Token: 0x04004760 RID: 18272
			public List<int> p0;

			// Token: 0x04004761 RID: 18273
			public int p1;
		}

		// Token: 0x0200087A RID: 2170
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p6 : IReturnParameter<int>
		{
			// Token: 0x170005CD RID: 1485
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004762 RID: 18274
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		// Token: 0x0200087B RID: 2171
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p7
		{
			// Token: 0x04004763 RID: 18275
			public int p0;

			// Token: 0x04004764 RID: 18276
			public int p1;

			// Token: 0x04004765 RID: 18277
			public int p2;

			// Token: 0x04004766 RID: 18278
			public int[] p3;

			// Token: 0x04004767 RID: 18279
			public string[] p4;
		}

		// Token: 0x0200087C RID: 2172
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p8 : IReturnParameter<int>
		{
			// Token: 0x170005CE RID: 1486
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004768 RID: 18280
			public int p0;

			// Token: 0x04004769 RID: 18281
			public int p1;

			// Token: 0x0400476A RID: 18282
			public int[] p2;

			// Token: 0x0400476B RID: 18283
			public string[] p3;

			// Token: 0x0400476C RID: 18284
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		// Token: 0x0200087D RID: 2173
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p9
		{
			// Token: 0x0400476D RID: 18285
			public KFBoCaiShopDB p0;

			// Token: 0x0400476E RID: 18286
			public int p1;
		}

		// Token: 0x0200087E RID: 2174
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p10 : IReturnParameter<bool>
		{
			// Token: 0x170005CF RID: 1487
			
			
			[Preserve(Conditional = true)]
			public bool Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x0400476F RID: 18287
			[AutoCSer.Json.IgnoreMember]
			public bool Ret;
		}

		// Token: 0x0200087F RID: 2175
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p11
		{
			// Token: 0x04004770 RID: 18288
			public KFBuyBocaiData p0;
		}

		// Token: 0x02000880 RID: 2176
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p12
		{
			// Token: 0x04004771 RID: 18289
			public int p0;
		}

		// Token: 0x02000881 RID: 2177
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p13 : IReturnParameter<KFStageData>
		{
			// Token: 0x170005D0 RID: 1488
			
			
			[Preserve(Conditional = true)]
			public KFStageData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004772 RID: 18290
			[AutoCSer.Json.IgnoreMember]
			public KFStageData Ret;
		}

		// Token: 0x02000882 RID: 2178
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p14 : IReturnParameter<OpenLottery>
		{
			// Token: 0x170005D1 RID: 1489
			
			
			[Preserve(Conditional = true)]
			public OpenLottery Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004773 RID: 18291
			[AutoCSer.Json.IgnoreMember]
			public OpenLottery Ret;
		}

		// Token: 0x02000883 RID: 2179
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p15
		{
			// Token: 0x04004774 RID: 18292
			public bool p0;

			// Token: 0x04004775 RID: 18293
			public int p1;

			// Token: 0x04004776 RID: 18294
			public long p2;
		}

		// Token: 0x02000884 RID: 2180
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p16 : IReturnParameter<List<OpenLottery>>
		{
			// Token: 0x170005D2 RID: 1490
			
			
			[Preserve(Conditional = true)]
			public List<OpenLottery> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004777 RID: 18295
			[AutoCSer.Json.IgnoreMember]
			public List<OpenLottery> Ret;
		}

		// Token: 0x02000885 RID: 2181
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p17 : IReturnParameter<List<KFBoCaoHistoryData>>
		{
			// Token: 0x170005D3 RID: 1491
			
			
			[Preserve(Conditional = true)]
			public List<KFBoCaoHistoryData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004778 RID: 18296
			[AutoCSer.Json.IgnoreMember]
			public List<KFBoCaoHistoryData> Ret;
		}

		// Token: 0x02000886 RID: 2182
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p18
		{
			// Token: 0x04004779 RID: 18297
			public int p0;

			// Token: 0x0400477A RID: 18298
			public int p1;

			// Token: 0x0400477B RID: 18299
			public long p2;

			// Token: 0x0400477C RID: 18300
			public string p3;
		}

		// Token: 0x02000887 RID: 2183
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p19
		{
			// Token: 0x0400477D RID: 18301
			public string[] p0;
		}

		// Token: 0x02000888 RID: 2184
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p20
		{
			// Token: 0x0400477E RID: 18302
			public KuaFuClientContext p0;
		}

		// Token: 0x02000889 RID: 2185
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p21 : IReturnParameter<KFCallMsg>
		{
			// Token: 0x170005D4 RID: 1492
			
			
			[Preserve(Conditional = true)]
			public KFCallMsg Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x0400477F RID: 18303
			[AutoCSer.Json.IgnoreMember]
			public KFCallMsg Ret;
		}

		// Token: 0x0200088A RID: 2186
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p22
		{
			// Token: 0x04004780 RID: 18304
			public Dictionary<int, int> p0;

			// Token: 0x04004781 RID: 18305
			public int p1;
		}

		// Token: 0x0200088B RID: 2187
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p23
		{
			// Token: 0x04004782 RID: 18306
			public ZhanDuiZhengBaSyncData p0;
		}

		// Token: 0x0200088C RID: 2188
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p24 : IReturnParameter<ZhanDuiZhengBaSyncData>
		{
			// Token: 0x170005D5 RID: 1493
			
			
			[Preserve(Conditional = true)]
			public ZhanDuiZhengBaSyncData Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004783 RID: 18307
			[AutoCSer.Json.IgnoreMember]
			public ZhanDuiZhengBaSyncData Ret;
		}

		// Token: 0x0200088D RID: 2189
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p25
		{
			// Token: 0x04004784 RID: 18308
			public ZhanDuiZhengBaFuBenData p0;

			// Token: 0x04004785 RID: 18309
			public int p1;

			// Token: 0x04004786 RID: 18310
			public int p2;

			// Token: 0x04004787 RID: 18311
			public int p3;
		}

		// Token: 0x0200088E RID: 2190
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p26 : IReturnParameter<int>
		{
			// Token: 0x170005D6 RID: 1494
			
			
			[Preserve(Conditional = true)]
			public int Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004788 RID: 18312
			public ZhanDuiZhengBaFuBenData p0;

			// Token: 0x04004789 RID: 18313
			[AutoCSer.Json.IgnoreMember]
			public int Ret;
		}

		// Token: 0x0200088F RID: 2191
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p27
		{
			// Token: 0x0400478A RID: 18314
			public int p0;

			// Token: 0x0400478B RID: 18315
			public int p1;
		}

		// Token: 0x02000890 RID: 2192
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p28 : IReturnParameter<List<ZhanDuiZhengBaNtfPkResultData>>
		{
			// Token: 0x170005D7 RID: 1495
			
			
			[Preserve(Conditional = true)]
			public List<ZhanDuiZhengBaNtfPkResultData> Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x0400478C RID: 18316
			[AutoCSer.Json.IgnoreMember]
			public List<ZhanDuiZhengBaNtfPkResultData> Ret;
		}

		// Token: 0x02000891 RID: 2193
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[BoxSerialize]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p29
		{
			// Token: 0x0400478D RID: 18317
			public int p0;

			// Token: 0x0400478E RID: 18318
			public int p1;

			// Token: 0x0400478F RID: 18319
			public int p2;
		}

		// Token: 0x02000892 RID: 2194
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p30
		{
			// Token: 0x04004790 RID: 18320
			public bool p0;

			// Token: 0x04004791 RID: 18321
			public int p1;
		}

		// Token: 0x02000893 RID: 2195
		[BoxSerialize]
		[AutoCSer.BinarySerialize.Serialize(IsMemberMap = false, IsReferenceMember = false)]
		[StructLayout(LayoutKind.Auto)]
		internal struct _p31 : IReturnParameter<string>
		{
			// Token: 0x170005D8 RID: 1496
			
			
			[Preserve(Conditional = true)]
			public string Return
			{
				get
				{
					return this.Ret;
				}
				set
				{
					this.Ret = value;
				}
			}

			// Token: 0x04004792 RID: 18322
			[AutoCSer.Json.IgnoreMember]
			public string Ret;
		}

		// Token: 0x02000894 RID: 2196
		public sealed class ClientConfig
		{
			// Token: 0x04004793 RID: 18323
			public AutoCSer.Net.TcpInternalServer.ServerAttribute ServerAttribute;

			// Token: 0x04004794 RID: 18324
			public Action<SubArray<byte>> OnCustomData;

			// Token: 0x04004795 RID: 18325
			public ILog Log;

			// Token: 0x04004796 RID: 18326
			public Func<AutoCSer.Net.TcpInternalServer.ClientSocketSender, bool> VerifyMethod;
		}
	}
}
