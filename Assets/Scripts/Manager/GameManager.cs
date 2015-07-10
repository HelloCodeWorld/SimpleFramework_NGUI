﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using System.Reflection;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SimpleFramework.Manager {
    public class GameManager : LuaBehaviour {
        private string message;

        /// <summary>
        /// 初始化游戏管理器
        /// </summary>
        void Awake() {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init() {
            InitGui();
            DontDestroyOnLoad(gameObject);  //防止销毁自己

            CheckExtractResource(); //释放资源
            ZipConstants.DefaultCodePage = 65001;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = AppConst.GameFrameRate;
        }

        /// <summary>
        /// 初始化GUI
        /// </summary>
        public void InitGui() {
            string name = "GUI";
            GameObject gui = GameObject.Find(name);
            if (gui != null) return;

            GameObject prefab = Util.LoadPrefab(name);
            gui = Instantiate(prefab) as GameObject;
            gui.name = name;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void CheckExtractResource() {
            bool isExists = Directory.Exists(Util.DataPath) &&
              Directory.Exists(Util.DataPath + "lua/") && File.Exists(Util.DataPath + "files.txt");
            if (isExists || AppConst.DebugMode) {
                StartCoroutine(OnUpdateResource());
                return;   //文件已经解压过了，自己可添加检查文件列表逻辑
            }
            StartCoroutine(OnExtractResource());    //启动释放协成 
        }

        IEnumerator OnExtractResource() {
            string dataPath = Util.DataPath;  //数据目录
            string resPath = Util.AppContentPath(); //游戏包资源目录

            if (Directory.Exists(dataPath)) Directory.Delete(dataPath);
            Directory.CreateDirectory(dataPath);

            string infile = resPath + "files.txt";
            string outfile = dataPath + "files.txt";
            if (File.Exists(outfile)) File.Delete(outfile);

            message = "正在解包文件:>files.txt";
            Debug.Log(message);
            if (Application.platform == RuntimePlatform.Android) {
                WWW www = new WWW(infile);
                yield return www;

                if (www.isDone) {
                    File.WriteAllBytes(outfile, www.bytes);
                }
                yield return 0;
            } else File.Copy(infile, outfile, true);
            yield return new WaitForEndOfFrame();

            //释放所有文件到数据目录
            string[] files = File.ReadAllLines(outfile);
            foreach (var file in files) {
                string[] fs = file.Split('|');
                infile = resPath + fs[0];  //
                outfile = dataPath + fs[0];
                message = "正在解包文件:>" + fs[0];
                Debug.Log("正在解包文件:>" + infile);

                string dir = Path.GetDirectoryName(outfile);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (Application.platform == RuntimePlatform.Android) {
                    WWW www = new WWW(infile);
                    yield return www;

                    if (www.isDone) {
                        File.WriteAllBytes(outfile, www.bytes);
                    }
                    yield return 0;
                } else File.Copy(infile, outfile, true);
                yield return new WaitForEndOfFrame();
            }
            message = "解包完成!!!";
            yield return new WaitForSeconds(0.1f);
            message = string.Empty;

            //释放完成，开始启动更新资源
            StartCoroutine(OnUpdateResource());
        }

        /// <summary>
        /// 启动更新下载，这里只是个思路演示，此处可启动线程下载更新
        /// </summary>
        IEnumerator OnUpdateResource() {
            if (!AppConst.UpdateMode) {
                ResManager.initialize(OnResourceInited);
                yield break;
            }
            WWW www = null;
            string dataPath = Util.DataPath;  //数据目录
            string url = string.Empty;
#if UNITY_5 
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            url = AppConst.WebUrl + "/ios/";
        } else {
            url = AppConst.WebUrl + "android/5x/";
        }
#else
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                url = AppConst.WebUrl + "/iphone/";
            } else {
                url = AppConst.WebUrl + "android/4x/";
            }
#endif
            string random = DateTime.Now.ToString("yyyymmddhhmmss");
            string listUrl = url + "files.txt?v=" + random;
            if (Debug.isDebugBuild) Debug.LogWarning("LoadUpdate---->>>" + listUrl);

            www = new WWW(listUrl); yield return www;
            if (www.error != null) {
                OnUpdateFailed(string.Empty);
                yield break;
            }
            if (!Directory.Exists(dataPath)) {
                Directory.CreateDirectory(dataPath);
            }
            File.WriteAllBytes(dataPath + "files.txt", www.bytes);
            string filesText = www.text;
            string[] files = filesText.Split('\n');

            for (int i = 0; i < files.Length; i++) {
                if (string.IsNullOrEmpty(files[i])) continue;
                string[] keyValue = files[i].Split('|');
                string f = keyValue[0].Remove(0, 1);
                string localfile = (dataPath + f).Trim();
                string path = Path.GetDirectoryName(localfile);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                string fileUrl = url + f + "?v=" + random;
                bool canUpdate = !File.Exists(localfile);
                if (!canUpdate) {
                    string remoteMd5 = keyValue[1].Trim();
                    string localMd5 = Util.md5file(localfile);
                    canUpdate = !remoteMd5.Equals(localMd5);
                    if (canUpdate) File.Delete(localfile);
                }
                if (canUpdate) {   //本地缺少文件
                    Debug.Log(fileUrl);
                    message = "downloading>>" + fileUrl;
                    www = new WWW(fileUrl); yield return www;
                    if (www.error != null) {
                        OnUpdateFailed(path);   //
                        yield break;
                    }
                    File.WriteAllBytes(localfile, www.bytes);
                }
            }
            yield return new WaitForEndOfFrame();
            message = "更新完成!!";

            ResManager.initialize(OnResourceInited);
        }

        /// <summary>
        /// 资源初始化结束
        /// </summary>
        public void OnResourceInited() {
            LuaManager.Start();
            LuaManager.DoFile("logic/game");       //加载游戏
            LuaManager.DoFile("logic/network");    //加载网络
            initialize = true;                     //初始化完 

            NetManager.OnInit();    //初始化网络

            object[] panels = CallMethod("LuaScriptPanel");  
            //---------------------Lua面板---------------------------
            foreach (object o in panels) {
                string name = o.ToString().Trim();
                if (string.IsNullOrEmpty(name)) continue;
                name += "Panel";    //添加

                LuaManager.DoFile("View/" + name);
                Debug.LogWarning("LoadLua---->>>>" + name + ".lua");
            }
            //------------------------------------------------------------
            CallMethod("OnInitOK");   //初始化完成
        }

        void OnUpdateFailed(string file) {
            message = "更新失败!>" + file;
        }

        void OnGUI() {
            GUI.Label(new Rect(10, 120, 960, 50), message);
        }

        void Update() {
            if (LuaManager != null && initialize) {
                LuaManager.Update();
            }
        }

        void LateUpdate() {
            if (LuaManager != null && initialize) {
                LuaManager.LateUpate();
            }
        }

        void FixedUpdate() {
            if (LuaManager != null && initialize) {
                LuaManager.FixedUpdate();
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        void OnDestroy() {
            if (NetManager != null) {
                NetManager.Unload();
            }
            if (LuaManager != null) {
                LuaManager.Destroy();
            }
            Debug.Log("~GameManager was destroyed");
        }
    }
}