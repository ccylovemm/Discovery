
public class DataManager
{
    static public int dataVersion = 423;
    static public UserData userData;
    static private SaveGameBinarySerializer serializer;

    static public void Init()
    {
        serializer = new SaveGameBinarySerializer();
        userData = SaveGame.Load<UserData>("DataCenter", new UserData(), serializer);
        if(userData.Version != dataVersion)
        {
            Clear();
            userData = SaveGame.Load<UserData>("DataCenter", new UserData(), serializer);
        }
        userData.Version = dataVersion;
        userData.FreshData();
    }

    static public void Save()
    {
        SaveGame.Save<UserData>("DataCenter" , userData, serializer);
    }

    static public void Clear()
    {
        SaveGame.Clear();
    }
}
