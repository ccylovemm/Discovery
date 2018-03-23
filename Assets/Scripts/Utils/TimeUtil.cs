using System;

public class TimeUtil
{
    static public string getDate(uint s_)
    {
        uint h = s_ / 3600;
        uint m = (s_ % 3600) / 60;
        uint s = s_ % 60;
        return (h > 9 ? h.ToString() : "0" + h) + ":" + (m > 9 ? m.ToString() : "0" + m) + ":" + (s > 9 ? s.ToString() : "0" + s);
    }

    static public string getTime(uint s_)
    {
        uint m = (s_ % 3600) / 60;
        uint s = s_ % 60;
        return (m > 9 ? m.ToString() : "0" + m) + ":" + (s > 9 ? s.ToString() : "0" + s);
    }

    public static DateTime GetDateTime(uint timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
}
