using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace GameServer.Tools
{
    // Token: 0x020008FB RID: 2299
    public class TMSKThreadStaticClass
    {
        // Token: 0x0600428D RID: 17037 RVA: 0x003C9C74 File Offset: 0x003C7E74
        public static TMSKThreadStaticClass GetInstance()
        {
            if (null == TMSKThreadStaticClass.ThreadStaticClass)
            {
                TMSKThreadStaticClass.ThreadStaticClass = new TMSKThreadStaticClass();
            }
            return TMSKThreadStaticClass.ThreadStaticClass;
        }

        // Token: 0x0600428E RID: 17038 RVA: 0x003C9CA8 File Offset: 0x003C7EA8
        public TMSKThreadStaticClass()
        {
            foreach (KeyValuePair<int, int> kv in GameManager.MemoryPoolConfigDict)
            {
                int index = TMSKThreadStaticClass.Log2(kv.Key);
                if (index < this.MemoryBlockNumArray.Length)
                {
                    this.MemoryBlockStackArray[index] = new Stack<MemoryBlock>();
                    this.MemoryBlockNumArray[index] = Math.Max(10, kv.Value / 100);
                    this.MemoryBlockSizeArray[index] = kv.Key;
                }
            }
            Stack<MemoryBlock> lastStackMemoryBlock = null;
            int lastMemoryBlockNum = 10;
            int lastMemoryBlockSize = 0;
            for (int i = this.MemoryBlockStackArray.Length - 1; i >= 0; i--)
            {
                if (null == this.MemoryBlockStackArray[i])
                {
                    if (null == lastStackMemoryBlock)
                    {
                        this.MemoryBlockStackArray[i] = new Stack<MemoryBlock>();
                        this.MemoryBlockNumArray[i] = 10;
                        this.MemoryBlockSizeArray[i] = 0;
                    }
                    else
                    {
                        this.MemoryBlockStackArray[i] = lastStackMemoryBlock;
                        this.MemoryBlockNumArray[i] = lastMemoryBlockNum;
                        this.MemoryBlockSizeArray[i] = lastMemoryBlockSize;
                    }
                }
                else
                {
                    lastStackMemoryBlock = this.MemoryBlockStackArray[i];
                    lastMemoryBlockNum = this.MemoryBlockNumArray[i];
                    lastMemoryBlockSize = this.MemoryBlockSizeArray[i];
                }
            }
        }

        // Token: 0x0600428F RID: 17039 RVA: 0x003C9E54 File Offset: 0x003C8054
        ~TMSKThreadStaticClass()
        {
            for (int i = 0; i < this.QueueMemoryStream.Count; i++)
            {
                MemoryStream ms = this.QueueMemoryStream.Dequeue();
                ms.Dispose();
            }
            foreach (Stack<MemoryBlock> stack in this.MemoryBlockStackArray)
            {
                while (stack.Count > 0)
                {
                    Global._MemoryManager.Push(stack.Pop());
                }
            }
        }

        // Token: 0x06004290 RID: 17040 RVA: 0x003C9EF8 File Offset: 0x003C80F8
        public void PushMemoryStream(MemoryStream ms)
        {
            try
            {
                if (this.QueueMemoryStream.Count <= 30)
                {
                    this.QueueMemoryStream.Enqueue(ms);
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "");
            }
        }

        // Token: 0x06004291 RID: 17041 RVA: 0x003C9F50 File Offset: 0x003C8150
        public MemoryStream PopMemoryStream()
        {
            try
            {
                if (this.QueueMemoryStream.Count > 0)
                {
                    MemoryStream ms = this.QueueMemoryStream.Dequeue();
                    ms.Position = 0L;
                    ms.SetLength(0L);
                    return ms;
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "");
            }
            return new MemoryStream();
        }

        // Token: 0x06004292 RID: 17042 RVA: 0x003C9FC8 File Offset: 0x003C81C8
        public static int Log2(int size)
        {
            return (int)Math.Ceiling(Math.Log((double)size, 2.0));
        }

        // Token: 0x06004293 RID: 17043 RVA: 0x003C9FF0 File Offset: 0x003C81F0
        public void PushMemoryBlock(MemoryBlock item)
        {
            try
            {
                int index = TMSKThreadStaticClass.Log2(item.BlockSize);
                int blockSize = this.MemoryBlockSizeArray[index];
                if (blockSize > 0)
                {
                    if (blockSize < item.BlockSize)
                    {
                        index++;
                    }
                    if (this.MemoryBlockStackArray[index].Count <= this.MemoryBlockNumArray[index])
                    {
                        this.MemoryBlockStackArray[index].Push(item);
                    }
                    else
                    {
                        Global._MemoryManager.Push(item);
                    }
                }
                else if (this.MemoryBlockStackArray[index].Count <= this.MemoryBlockNumArray[index])
                {
                    this.MemoryBlockStackArray[index].Push(item);
                }
                else
                {
                    Global._MemoryManager.Push(item);
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "");
            }
        }

        // Token: 0x06004294 RID: 17044 RVA: 0x003CA0D8 File Offset: 0x003C82D8
        public MemoryBlock PopMemoryBlock(int needSize)
        {
            try
            {
                int index = TMSKThreadStaticClass.Log2(needSize);
                int blockSize = this.MemoryBlockSizeArray[index];
                if (blockSize > 0)
                {
                    if (blockSize < needSize)
                    {
                        index++;
                    }
                    if (this.MemoryBlockStackArray[index].Count > 0)
                    {
                        return this.MemoryBlockStackArray[index].Pop();
                    }
                    return Global._MemoryManager.Pop(needSize);
                }
                else
                {
                    if (this.MemoryBlockStackArray[index].Count > 0)
                    {
                        return this.MemoryBlockStackArray[index].Pop();
                    }
                    return Global._MemoryManager.Pop((int)Math.Pow(2.0, (double)index));
                }
            }
            catch (Exception ex)
            {
                DataHelper.WriteExceptionLogEx(ex, "");
            }
            return null;
        }

        // Token: 0x0400503D RID: 20541
        private const int QueueMemoryStreamMaxSize = 30;

        // Token: 0x0400503E RID: 20542
        [ThreadStatic]
        private static TMSKThreadStaticClass ThreadStaticClass = null;

        // Token: 0x0400503F RID: 20543
        private Queue<MemoryStream> QueueMemoryStream = new Queue<MemoryStream>();

        // Token: 0x04005040 RID: 20544
        private Stack<MemoryBlock>[] MemoryBlockStackArray = new Stack<MemoryBlock>[20];

        // Token: 0x04005041 RID: 20545
        private int[] MemoryBlockNumArray = new int[20];

        // Token: 0x04005042 RID: 20546
        private int[] MemoryBlockSizeArray = new int[20];
    }
}
