using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    private int version;
    private uint actorId = 1;
    private bool isDead = true;
    private bool isSound = true;
    private int diamond = 0;
    private int goldCoin = 0;

    public int Version
    {
        get
        {
            return version;
        }
        set
        {
            version = value;
            DataManager.Save();
        }
    }

    public uint ActorId
    {
        get
        {
            return actorId;
        }
        set
        {
            actorId = value;
            DataManager.Save();
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
        set
        {
            isDead = value;
            DataManager.Save();
        }
    }

    public bool IsSound
    {
        get
        {
            return isSound;
        }
        set
        {
            isSound = value;
            DataManager.Save();
        }
    }

    public int GoldCoin
    {
        get
        {
            return goldCoin;
        }
        set
        {
            goldCoin = value;
            DataManager.Save();
            EventCenter.DispatchEvent(EventEnum.UpdateMoney);
        }
    }

    public int Diamond
    {
        get
        {
            return diamond;
        }
        set
        {
            diamond = value;
            DataManager.Save();
            EventCenter.DispatchEvent(EventEnum.UpdateMoney);
        }
    }
}
