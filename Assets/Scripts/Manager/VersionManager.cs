using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//  Created by CaoChunYang 

public class VersionManager : Singleton<VersionManager> 
{
	static public string Resource_Version = "1";

	static public string LocalVersionFile = "LocalVersion.txt";
	static public string UpdateVersionFile = "UpdateVersion.txt";
    static public string LatestVersionFile = "LatestVersion.txt";

    static public string LocalResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return "file://" + Application.dataPath + "/StreamingAssets/";
			#elif UNITY_IPHONE
			return "file://" + Application.dataPath + "/Raw/";
			#elif UNITY_ANDROID
			return "jar:file://" + Application.dataPath + "!/assets/";
			#endif
		}
	}

	static public string LatestResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return Application.persistentDataPath + "/";   
			#elif UNITY_IPHONE
			return Application.persistentDataPath + "/";   
			#elif UNITY_ANDROID
			return Application.persistentDataPath + "/"; 
			#endif
		}
	}

	static public string ServerResUrl = "";     

	public delegate void Callback();

    public Callback CheckVersionFinish;

    static private Dictionary<string , string> LatestResVersion = new Dictionary<string, string>();   //本地所有最新资源 版本  用于远程比较
	static private Dictionary<string , string> ServerResVersion = new Dictionary<string, string>();  //远程最新资源 版本 
	static private Dictionary<string , string> UpdateResVersion = new Dictionary<string, string>();  //本地所有安装后更新资源 版本 用于查找本地最新资源

    private List<string> NeedDownFiles = new List<string>();  
	private int NeedDownCount = 0;
	private bool NeedUpdateLocalVersionFile = false;    

	static public string GetResPath(string filePath)
	{
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
                return "file://" + Application.streamingAssetsPath + "/" + filePath;
        #else
		        if(UpdateResVersion.ContainsKey(filePath))
		        {
			        return "file://" + LatestResUrl + filePath;
		        }
		        if(LatestResVersion.ContainsKey(filePath))
		        {
			        return LocalResUrl + filePath; 
		        }
		        return ServerResUrl + filePath;
        #endif
    }

    public void CheckVersion(VersionManager.Callback OnFinish)    
	{     
		CheckVersionFinish = OnFinish;
        LoadLatestVersion();
    }

	void LoadLatestVersion()
	{
		if(File.Exists(LatestResUrl + LatestVersionFile))
		{
			StartCoroutine(DownLoad(LatestResUrl + LatestVersionFile, delegate(WWW latestVersion)    
            {
				ParseVersionFile(latestVersion.bytes, LatestResVersion);
                LoadUpdateVersion();
			}));
		}
		else
		{
            StartCoroutine(DownLoad(LocalResUrl + LocalVersionFile, delegate (WWW localVersion)
            {
                ParseVersionFile(localVersion.bytes, LatestResVersion);
                LoadUpdateVersion();
            }));
        }
	}

    void LoadUpdateVersion()
    {
        if (File.Exists(LatestResUrl + UpdateVersionFile))
        {
            StartCoroutine(DownLoad("file://" + LatestResUrl + UpdateVersionFile, delegate (WWW updateVersion)
            {
                ParseVersionFile(updateVersion.bytes, UpdateResVersion);
                LoadServerVersion();
            }));
        }
        else
        {
            LoadServerVersion();
        }
    }

    void LoadServerVersion()
    {
        CheckVersionFinish();
        return;
        StartCoroutine(this.DownLoad(ServerResUrl + LocalVersionFile + "?" + Resource_Version, delegate (WWW serverVersion)
        {
            ParseVersionFile(serverVersion.bytes, ServerResVersion);
            if (System.Convert.ToInt32(LatestResVersion["VersionNumber"]) > System.Convert.ToInt32(ServerResVersion["VersionNumber"]))
            {
                CheckVersionFinish();
            }
            else
            {

                CompareVersion();
                NeedDownCount = NeedDownFiles.Count;
                if (NeedDownCount > 0)
                {
                 //   Loading.Instance.UpdateValue(0, string.Format(" 更新资源(0/{0})", NeedDownCount));
                }
                DownLoadRes();
            }
        }));
    }
	  
	void DownLoadRes()    
	{    
		if (NeedDownFiles.Count == 0)    
		{
			UpdateLocalVersionFile();    
			return;    
		}    

		string file = NeedDownFiles[0];    
		NeedDownFiles.RemoveAt(0);    

		StartCoroutine(this.DownLoad(ServerResUrl + file + "?" + UpdateResVersion[file], delegate(WWW w)    
      	{      
			ReplaceLocalRes(LatestResUrl + file, w.bytes);
         //     Loading.Instance.UpdateValue((float)(NeedDownCount - NeedDownFiles.Count) / (float) NeedDownCount , string.Format(" 更新资源({0}/{1})" , NeedDownCount - NeedDownFiles.Count ,  NeedDownCount));
			DownLoadRes();    
		}));    
	}    

	void UpdateLocalVersionFile()    
	{    
		if (NeedUpdateLocalVersionFile)    
		{    
			StringBuilder versions = new StringBuilder();    
			foreach (var item in ServerResVersion)    
			{    
				versions.Append(item.Key).Append("|").Append(item.Value).Append("\n");    
			}    

			SaveVersionFile(LatestResUrl + LatestVersionFile , versions);      

			StringBuilder versions_ = new StringBuilder();    
			foreach (var item in UpdateResVersion)    
			{    
				versions_.Append(item.Key).Append("|").Append(item.Value).Append("\n");    
			}   
			SaveVersionFile(LatestResUrl + UpdateVersionFile , versions_);  
		}     

		CheckVersionFinish ();
	}  

	void ReplaceLocalRes(string path, byte[] data)    
	{
		Debug.Log(path);
		path = path.Replace("file://" , "");
		string directoryName = path.Substring(0 , path.LastIndexOf("/"));
		if(!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if(File.Exists(path))
		{
			File.Delete(path);
		}  
		FileStream stream = File.Create(path);
		stream.Write(data, 0, data.Length);    
		stream.Flush();    
		stream.Close();    
	}     

	void SaveVersionFile(string path , StringBuilder versions)
	{
		path = path.Replace("file://" , "");
		string directoryName = path.Substring(0 , path.LastIndexOf("/"));
		if(!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if(File.Exists(path))
		{
			File.Delete(path);
		}  
		FileStream stream = new FileStream(path, FileMode.Create);      
		byte[] data = Encoding.ASCII.GetBytes(versions.ToString());    
		stream.Write(data, 0, data.Length);    
		stream.Flush();    
		stream.Close(); 
	}
	
	void CompareVersion()    
	{    
		foreach (var version in ServerResVersion)    
		{    
			string fileName = version.Key;
            string serverMd5 = version.Value.Replace("\r" , string.Empty);   
			if(fileName == "VersionNumber" || fileName == "FileCount" || fileName == "Version.txt")continue;
			if (!LatestResVersion.ContainsKey(fileName))    
			{    
				NeedDownFiles.Add(fileName);    
				UpdateResVersion.Add(fileName , serverMd5);
                LatestResVersion.Add(fileName, serverMd5);
			}    
			else    
			{    
				string localMd5;
                if (LatestResVersion.TryGetValue(fileName, out localMd5)) localMd5 = localMd5.Replace("\r", string.Empty);   
				if (!serverMd5.Equals(localMd5))    
				{    
					NeedDownFiles.Add(fileName);   
					UpdateResVersion.Add(fileName , serverMd5);
                    LatestResVersion.Add(fileName, serverMd5);
                }    
			}    
		}    
		NeedUpdateLocalVersionFile = NeedDownFiles.Count > 0;    
	}    
	
	void ParseVersionFile(byte[] bytes, Dictionary<string , string> dict)    
	{    
		dict.Clear();
        string content = Encoding.ASCII.GetString(bytes);
        if (content == null || content.Length == 0)    
		{    
			return;    
		}
		string[] items = content.Split('\n');
		foreach (string item in items)    
		{    
			string[] info = item.Split('|');    
			if (info != null && info.Length == 2)    
			{    
				if(info[0].IndexOf("VersionNumber") != -1)
				{
					dict.Add("VersionNumber", info[1]);    
				}
				else
				{
					if(dict.ContainsKey(info[0]))
					{
						Debug.Log("有相同的Key " + info[0]);
					}
					else
					{
						dict.Add(info[0], info[1]);
					}
				}
			}    
		}    
	}    
	
	IEnumerator DownLoad(string url, HandleFinishDownload finishFun)    
	{    
		WWW www = new WWW(url);    
		yield return www;    
		if (finishFun != null)    
		{    
			finishFun(www);
		}    
		www.Dispose();    
	}    
	
	public delegate void HandleFinishDownload(WWW www);
} 