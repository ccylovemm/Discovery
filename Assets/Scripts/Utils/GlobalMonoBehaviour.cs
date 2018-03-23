using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine;

public class GlobalMonoBehaviour : Singleton<GlobalMonoBehaviour>
{
    static private List<Deferred<object>> _allListeners = new List<Deferred<object>>();

    static public Deferred<object> CreateDeferred()
    {
        var callback = new Deferred<object>();
        OperationAdd(callback);
        
        return callback;
    }


    static  void OperationAdd(Deferred<object> callback)
    {
        _allListeners.Add(callback);
        callback.ThenFinally(() => { OperationRemove(callback); });
    }
    static public int DeferredCount()
    {
        return _allListeners.Count;
        
    }
    static  void OperationRemove(Deferred<object> callback)
    {
        _allListeners.Remove(callback);
    }
    static public void ClearAllListeners()
    {
        while (_allListeners.Count > 0)
        {
            var p = _allListeners[0]; 
            p.Abort();
            OperationRemove(p);
        }

        _allListeners.Clear();
    }

    /// <summary>
    /// Wait for all promises specified to be resolved (parallel) in order for the All Promise to be resolved, if any of them fails, it will immediately reject the promise.
    /// </summary>
    /// <param name="promises">Promises waiting for being resolved.</param>
    /// <returns></returns>
    public static Promise<object> All(params Promise<object>[] promises)
    {
        var masterDeferred = GlobalMonoBehaviour.CreateDeferred();         
        var remainingCount = promises.Length;
    //      Logger.Log("@==========================================All.remainingCount" + remainingCount);
        var args = new object[promises.Length];
        int i = 0;
        var count = 0;
        if (remainingCount == 0)
        {
            masterDeferred.Resolve(args);
        }
        foreach (var promise in promises)
        {
            HandleAllPromise(promise, i, args)
                    .Completes(x =>
                    {
                        count++;
                        masterDeferred.Progress(count);
                        --remainingCount;
                        //   Logger.Log("@==========================================--.remainingCount" + remainingCount);
                        if (remainingCount <= 0)
                        {
                            // This will never happen if any of the promises errorred.
                            masterDeferred.Resolve(args);
                        }
                    })
                    .Fails(y =>
                    {
                        masterDeferred.Reject(y);
                    });

            i++; // increment iterator.
        }

        return masterDeferred;
 
    }

    // This is a helper, in order to pass the arguments in sequence according to the promises.
    private static Promise<object> HandleAllPromise(Promise<object> promise, int index, object[] args)
    {
        return promise.Completes(x =>
        {
            args[index] = x;
            //  return x;
        });
    }

    public static Promise<object> AllSequentially(params Func<Promise<object>>[] promises)
    {
        var deferred = GlobalMonoBehaviour.CreateDeferred();

        var args = new object[promises.Length];

        var queue = new Queue<Func<Promise<object>>>(promises);
        HandleNext_AllSequentially(queue, deferred, args, 0);

        return deferred;
    }

    private static void HandleNext_AllSequentially(Queue<Func<Promise<object>>> queue, Deferred<object> masterDeferred, object[] args, int indexer)
    {
        if (queue.Count == 0)
        {
            masterDeferred.Resolve(args);
            return;
        }
        var promiseFunc = queue.Dequeue();
        promiseFunc()
                .Completes(x =>
                {
                    args[indexer] = x;
                    HandleNext_AllSequentially(queue, masterDeferred, args, indexer++);
                })
                .Fails(y =>
                {
                    masterDeferred.Reject(y);
                });
    }

    public Promise<object> WaitFor(float seconds)
    {
        return promiseTimer.WaitFor(seconds);
    }
    PromiseTimer promiseTimer = new PromiseTimer();
    /**
    * Returns a new Promise which will resolve once all the supplied promises Objects have themselves resolved.  The
    * returned promise will supply an Array of outcomes in the same order as the supplied promise Objects.  
    * 
    * If any of the supplied promises reject then the returned Promise will also reject.
    * 
    * @author Jonny Reeves.
    */
    protected override void OnInit()
    {
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        
    }

    void Update()
    {
        promiseTimer.Update(Time.deltaTime);

        //   Logger.Log("DeferredCount " + DeferredCount().ToString());
    }

}