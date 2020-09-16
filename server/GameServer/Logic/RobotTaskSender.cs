using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Server.Tools;

namespace GameServer.Logic
{
	
	internal class RobotTaskSender
	{
		
		private RobotTaskSender()
		{
		}

		
		public static RobotTaskSender getInstance()
		{
			return RobotTaskSender.instance;
		}

		
		public bool Initialize(int seed, int randomCount, string pubKey)
		{
			lock (this)
			{
				try
				{
					this.m_TaskListVerifySeed = seed;
					this.m_TaskListVerifyRandomCount = randomCount;
					this.m_TaskListRSA = new RSACryptoServiceProvider();
					this.m_TaskListRSA.PersistKeyInCsp = false;
					this.m_TaskListRSA.FromXmlString(pubKey);
				}
				catch (Exception ex)
				{
					return false;
				}
			}
			return true;
		}

		
		public byte[] EncryptTaskList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> encryptedData = new List<byte>();
			lock (this)
			{
				try
				{
					int tick = Environment.TickCount;
					int randomCount = tick % this.m_TaskListVerifyRandomCount;
					int randomValue = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(randomCount));
					string output = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tick,
						randomValue,
						jailbreak ? 1 : 0,
						autoStart ? 1 : 0,
						info
					});
					List<byte> outputCompress = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(output)));
					int count = (outputCompress == null) ? output.Length : outputCompress.Count;
					for (int i = 0; i < count; i += 100)
					{
						byte[] section;
						if (outputCompress == null)
						{
							section = new UTF8Encoding().GetBytes(output.Substring(i, (i + 100 <= output.Length) ? 100 : (output.Length % 100)));
						}
						else
						{
							section = new byte[(i + 100 <= outputCompress.Count) ? 100 : (outputCompress.Count % 100)];
							outputCompress.CopyTo(i, section, 0, section.Length);
						}
						byte[] encryptedSection = this.m_TaskListRSA.Encrypt(section, false);
						int len = encryptedSection.Length;
						encryptedData.Add((byte)len);
						for (int j = 0; j < len; j++)
						{
							encryptedData.Add(encryptedSection[j]);
						}
					}
				}
				catch (Exception e)
				{
					return null;
				}
			}
			return encryptedData.ToArray();
		}

		
		public byte[] EncryptGeniusList(string taskList, bool jailbreak, bool autoStart, string info)
		{
			List<byte> encryptedData = new List<byte>();
			lock (this)
			{
				try
				{
					int tick = Environment.TickCount;
					int randomCount = tick % this.m_TaskListVerifyRandomCount;
					int randomValue = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(randomCount));
					string output = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						taskList,
						tick,
						randomValue,
						jailbreak ? 1 : 0,
						autoStart ? 1 : 0,
						info
					});
					List<byte> outputCompress = new List<byte>(DataHelper.Compress(new UTF8Encoding().GetBytes(output)));
					int count = outputCompress.Count;
					int c = 7;
					this.AddMagicBytes(encryptedData, c);
					for (int i = 0; i < count; i += 100)
					{
						byte[] section = new byte[(i + 100 <= outputCompress.Count) ? 100 : (outputCompress.Count % 100)];
						outputCompress.CopyTo(i, section, 0, section.Length);
						byte[] encryptedSection = this.m_TaskListRSA.Encrypt(section, false);
						int len = encryptedSection.Length;
						encryptedData.Add((byte)len);
						encryptedData.AddRange(encryptedSection);
						c = ((int)section[0] + c & 15);
					}
					this.AddMagicBytes(encryptedData, c);
				}
				catch (Exception e)
				{
					return null;
				}
			}
			return encryptedData.ToArray();
		}

		
		private void AddMagicBytes(List<byte> data, int c)
		{
			Random random = new Random(this.m_TaskListVerifySeed);
			int randomValue = random.Next();
			for (int i = 0; i < c; i++)
			{
				randomValue = random.Next();
				data.AddRange(BitConverter.GetBytes(randomValue));
			}
		}

		
		private uint GenMagicRandom(uint seed, int loop)
		{
			uint w = seed;
			uint z = 362436069U;
			for (int i = 0; i <= loop; i++)
			{
				z = 36969U * (z & 65535U) + (z >> 16);
				w = 18000U * (w & 65535U) + (w >> 16);
			}
			return (z << 16) + w;
		}

		
		private static RobotTaskSender instance = new RobotTaskSender();

		
		private int m_TaskListVerifySeed;

		
		private int m_TaskListVerifyRandomCount = 50;

		
		private RSACryptoServiceProvider m_TaskListRSA;
	}
}
