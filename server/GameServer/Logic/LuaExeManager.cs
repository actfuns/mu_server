using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using Microsoft.CSharp.RuntimeBinder;
using Neo.IronLua;

namespace GameServer.Logic
{
    
    public class LuaExeManager
    {
        
        public static LuaExeManager getInstance()
        {
            return LuaExeManager.instance;
        }

        
        public void InitLuaEnv()
        {
            this.gEnv = this.lua.CreateEnvironment();
            LuaExeManager.timerCheckDict = new Timer(100000.0);
            LuaExeManager.timerCheckDict.Elapsed += this.CheckDictLuaInfo;
            LuaExeManager.timerCheckDict.Interval = 100000.0;
            LuaExeManager.timerCheckDict.Enabled = true;
        }

        
        private void CheckDictLuaInfo(object source, ElapsedEventArgs e)
        {
            lock (this.dictLuaCache)
            {
                using (Dictionary<string, LuaExeInfo>.Enumerator enumerator = this.dictLuaCache.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        KeyValuePair<string, LuaExeInfo> kvLuaInfo = enumerator.Current;
                        KeyValuePair<string, LuaExeInfo> kvLuaInfo3 = kvLuaInfo;
                        DateTime dateNew = File.GetLastWriteTime(kvLuaInfo3.Key);
                        DateTime t = dateNew;
                        kvLuaInfo3 = kvLuaInfo;
                        if (t > kvLuaInfo3.Value.dateLastWrite)
                        {
                            Func<string> code = delegate ()
                            {
                                KeyValuePair<string, LuaExeInfo> kvLuaInfo2 = kvLuaInfo;
                                return File.ReadAllText(kvLuaInfo2.Key);
                            };
                            kvLuaInfo3 = kvLuaInfo;
                            string chunkName = Path.GetFileName(kvLuaInfo3.Key);
                            LuaChunk c = this.lua.CompileChunk(code(), chunkName, false, new KeyValuePair<string, Type>[0]);
                            kvLuaInfo3 = kvLuaInfo;
                            kvLuaInfo3.Value.dateLastWrite = dateNew;
                            kvLuaInfo3 = kvLuaInfo;
                            kvLuaInfo3.Value.luaChunk = c;
                        }
                    }
                }
            }
        }

        
        public LuaGlobal ExeLua(string strLuaPath)
        {
            LuaExeInfo exeInfo = null;
            string strFullPath = Path.GetFullPath(strLuaPath);
            lock (this.dictLuaCache)
            {
                if (!this.dictLuaCache.TryGetValue(strFullPath, out exeInfo))
                {
                    Func<string> code = () => File.ReadAllText(strFullPath);
                    string chunkName = Path.GetFileName(strFullPath);
                    LuaChunk c = this.lua.CompileChunk(code(), chunkName, false, new KeyValuePair<string, Type>[0]);
                    exeInfo = new LuaExeInfo();
                    exeInfo.dateLastWrite = File.GetLastWriteTime(strFullPath);
                    exeInfo.luaChunk = c;
                    this.dictLuaCache.Add(strFullPath, exeInfo);
                }
                this.gEnv.DoChunk(exeInfo.luaChunk, new object[0]);
            }
            return this.gEnv;
        }

        
        public LuaResult ExecLuaFunction(LuaManager luaManager, dynamic g, string strLuaFunction, GameClient client)
        {
            lock (this.dictLuaCache)
            {
                return (LuaResult)g[strLuaFunction](GameManager.LuaMgr, client, null);
            }
        }

        
        private Dictionary<string, LuaExeInfo> dictLuaCache = new Dictionary<string, LuaExeInfo>();

        
        private Lua lua = new Lua();

        
        private LuaGlobal gEnv = null;

        
        private static Timer timerCheckDict;

        
        private static LuaExeManager instance = new LuaExeManager();
    }
}
