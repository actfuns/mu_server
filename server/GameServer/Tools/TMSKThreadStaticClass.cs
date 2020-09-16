using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Logic;
using Server.TCP;
using Server.Tools;

namespace GameServer.Tools
{
    
    public class TMSKThreadStaticClass
    {
        
        public static TMSKThreadStaticClass GetInstance()
        {
            if (null == TMSKThreadStaticClass.ThreadStaticClass)
            {
                TMSKThreadStaticClass.ThreadStaticClass = new TMSKThreadStaticClass();
            }
            return TMSKThreadStaticClass.ThreadStaticClass;
        }

        
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

        
        public static int Log2(int size)
        {
            return (int)Math.Ceiling(Math.Log((double)size, 2.0));
        }

        
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

        
        private const int QueueMemoryStreamMaxSize = 30;

        
        [ThreadStatic]
        private static TMSKThreadStaticClass ThreadStaticClass = null;

        
        private Queue<MemoryStream> QueueMemoryStream = new Queue<MemoryStream>();

        
        private Stack<MemoryBlock>[] MemoryBlockStackArray = new Stack<MemoryBlock>[20];

        
        private int[] MemoryBlockNumArray = new int[20];

        
        private int[] MemoryBlockSizeArray = new int[20];
    }
}
