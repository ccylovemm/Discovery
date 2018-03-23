using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class ResourceManager : Singleton<ResourceManager>
{
    public delegate void Callback(AssetBundle asset);
    public delegate void CallIconBack(Sprite asset);

    private Dictionary<string, Deferred<Sprite>> iconAssets = new Dictionary<string, Deferred<Sprite>>();
    private Dictionary<string, Deferred<AssetBundle>> assetbundles = new Dictionary<string, Deferred<AssetBundle>>();

    public void LoadIcon(string fileName, CallIconBack callback)
    {
        if (iconAssets.ContainsKey(fileName.ToLower()))
        {
            iconAssets[fileName.ToLower()].Completes(icon =>
            {
                callback(icon);
            });
        }
        else
        {
            var iconDefer = new Deferred<Sprite>();
            iconAssets.Add(fileName.ToLower(), iconDefer);
            ResourceManager.Instance.LoadAsset("resourceassets/atlas/icon.assetbundle", asset =>
            {
                Sprite[] assets = asset.LoadAllAssets<Sprite>();
                for (int i = 0; i < assets.Length; i++)
                {
                    if (assets[i].name == fileName)
                    {
                        iconDefer.Resolve(assets[i]);
                        callback(assets[i]);
                        break;
                    }
                }
            });
        }
    }

    public void LoadAsset(string pathName, Callback callback)
    {
        StartCoroutine(LoadAsset_(pathName , callback));
    }

    IEnumerator LoadAsset_(string pathName , Callback callback)
    {
        if (assetbundles.ContainsKey(pathName.ToLower()))
        {
            assetbundles[pathName.ToLower()].Completes(ab =>
            {
                if(callback != null) callback(ab);
            });
        }
        else
        {
            var assetDefer = new Deferred<AssetBundle>();
            assetbundles.Add(pathName.ToLower(), assetDefer);

            AssetBundle assetBundle = null;
            if (!assetbundles.ContainsKey("StreamingAssets"))
            {
                var defer = new Deferred<AssetBundle>();
                assetbundles.Add("StreamingAssets", defer);
                WWW www = new WWW(VersionManager.GetResPath("StreamingAssets"));
                yield return www;
                assetBundle = www.assetBundle;
                defer.Resolve(assetBundle);
                www.Dispose();
            }
            else
            {
                while(assetbundles["StreamingAssets"].State != DeferredState.RESOLVED)
                {
                    yield return null;
                }
                assetbundles["StreamingAssets"].Completes(ab =>
                {
                    assetBundle = ab;
                });
                while (assetBundle == null)
                {
                    yield return null;
                }
            }

            AssetBundleManifest abManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] assetDepends = abManifest.GetAllDependencies(pathName.ToLower());

            for (int i = 0; i < assetDepends.Length; i++)
            {
                if (!assetbundles.ContainsKey(assetDepends[i]))
                {
                    var defer = new Deferred<AssetBundle>();
                    assetbundles.Add(assetDepends[i], defer);
                    WWW dependWWW = new WWW(VersionManager.GetResPath(assetDepends[i]));
                    yield return dependWWW;
                    defer.Resolve(dependWWW.assetBundle);
                    dependWWW.Dispose();
                }
                else
                {
                    while (assetbundles[assetDepends[i]].State != DeferredState.RESOLVED)
                    {
                        yield return null;
                    }
                }
            }

            WWW asset = new WWW(VersionManager.GetResPath(pathName.ToLower()));

            while (!asset.isDone)
            {
                yield return null;
            }
            yield return asset;
            if (asset.error != null)
            {
                Debug.Log(asset.error + " " + asset.url);
            }
            else
            {
                AssetBundle assetAB = asset.assetBundle;
                assetDefer.Resolve(assetAB);
                asset.Dispose();
                if(callback != null) callback(assetAB);
            }
        }
    }

    public void LoadScene(string pathName , Callback callback = null)
    {
        StartCoroutine(LoadScene_(pathName , callback));
    }

    IEnumerator LoadScene_(string pathName , Callback callback)
    {
        string path = VersionManager.GetResPath(pathName.ToLower());

        if (assetbundles.ContainsKey(pathName.ToLower()))
        {
            while (assetbundles[pathName.ToLower()].State != DeferredState.RESOLVED)
            {
                yield return null;
            }
            AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(pathName));
            while (!asyn.isDone)
            {
                yield return null;
            }
            yield return null;
            if (callback != null) callback(null);
        }
        else
        {
            var assetDefer = new Deferred<AssetBundle>();
            assetbundles.Add(pathName.ToLower(), assetDefer);

            AssetBundle assetBundle = null;
            if (!assetbundles.ContainsKey("StreamingAssets"))
            {
                var defer = new Deferred<AssetBundle>();
                assetbundles.Add("StreamingAssets", defer);
                WWW www = new WWW(VersionManager.GetResPath("StreamingAssets"));
                yield return www;
                assetBundle = www.assetBundle;
                defer.Resolve(assetBundle);
                www.Dispose();
            }
            else
            {
                while (assetbundles["StreamingAssets"].State != DeferredState.RESOLVED)
                {
                    yield return null;
                }
                assetbundles["StreamingAssets"].Completes(ab =>
                {
                    assetBundle = ab;
                });
                while (assetBundle == null)
                {
                    yield return null;
                }
            }
            AssetBundleManifest abManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string[] assetDepends = abManifest.GetAllDependencies(pathName.ToLower());

            for (int i = 0; i < assetDepends.Length; i++)
            {
                if (!assetbundles.ContainsKey(assetDepends[i]))
                {
                    var defer = new Deferred<AssetBundle>();
                    assetbundles.Add(assetDepends[i], defer);
                    WWW dependWWW = new WWW(VersionManager.GetResPath(assetDepends[i]));
                    yield return dependWWW;
                    defer.Resolve(dependWWW.assetBundle);
                    dependWWW.Dispose();
                }
                else
                {
                    while (assetbundles[assetDepends[i]].State != DeferredState.RESOLVED)
                    {
                        yield return null;
                    }
                }
            }
            WWW asset = new WWW(path);
            while (!asset.isDone)
            {
                yield return null;
            }
            yield return asset;
            if (asset.error != null)
            {
                Debug.Log(asset.error + " " + asset.url);
            }
            else
            {
                AssetBundle assetAB = asset.assetBundle;
                assetDefer.Resolve(assetAB);

                float p = assetDepends.Length * 0.2f + 0.3f;
                AsyncOperation asyn = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(System.IO.Path.GetFileNameWithoutExtension(pathName));
                while (!asyn.isDone)
                {
                    yield return null;
                }
                yield return null;
                if (callback != null) callback(null);
            }
        }
    }
}
