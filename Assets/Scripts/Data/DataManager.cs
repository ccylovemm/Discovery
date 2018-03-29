using System;
using System.IO;
using LitJson;

public class DataManager
{
    public void Save(Object tobject)
    {
        string path = "";
        string serializedString = JsonMapper.ToJson(tobject);
        using (StreamWriter sw = File.CreateText(path))
        {
            sw.Write(serializedString);
        }
    }
    public T Load<T>()
    {
        string path = "";
        if (File.Exists(path) == false)
        {
            return default(T);
        }

        using (StreamReader sr = File.OpenText(path))
        {
            string stringEncrypt = sr.ReadToEnd();

            if (string.IsNullOrEmpty(stringEncrypt))
                return default(T);
            return JsonMapper.ToObject<T>(stringEncrypt);
        }
    }
}