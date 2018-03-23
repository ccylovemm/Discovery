using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyerUtil
{
    static public void Test()
    {
        Dictionary<string, string> eventValue = new Dictionary<string, string>();
        eventValue.Add("af_revenue", "300");
        eventValue.Add("af_content_type", "category_a");
        eventValue.Add("af_content_id", "1234567");
        eventValue.Add("af_currency", "USD");
        AppsFlyer.trackRichEvent("af_purchase", eventValue);
    }
}
