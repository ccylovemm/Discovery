using System;
using System.Collections.Generic;
using UnityEngine;
public enum DeferredState
{
    PENDING,
    RESOLVED,
    REJECTED,
    ABORTED
}

public class Deferred<T> : Promise<T>
{
       

    private List<Action<T>> _completeListeners = new List<Action<T>>();
    private List<Action<Exception>> _failListeners = new List<Action<Exception>>();
    protected List<Action<float>> _progressListeners = new List<Action<float>>(); 
    private Action _finalCallback;
    public DeferredState State
    {

        get { return _state; }
    }

    private DeferredState _state = DeferredState.PENDING;
    private T _outcome;
    private Exception _error;
    T _Null;
    public Deferred()
    {
         
    }



    /**
        * Notifies all 'completes' handlers that the deferred operation was succesful.  An optional outcome object
        * can be supplied which will provided to all the complete handlers.
        * 
        * @parm outcome	The optional result of the Deferred operation.
        */
    public void Resolve()
    {
        Resolve(_Null);
    }
    public void Resolve(T outcome)
    {
        if (_state != DeferredState.PENDING)
        {
            return;
        }

        _outcome = outcome;
        _state = DeferredState.RESOLVED;

        _completeListeners.ForEach(then =>
        {

                

            try
            {
                then(outcome);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred:" + e);
            }
        });

        ClearListeners();
        InvokeFinalCallback();
    }

    /**
        * Notifies all 'fails' handlers that this deferred operation has been unsuccesful.  The supplied Error object
        * will be supplied to all of the handlers.
        * 
        * @param error		Error object which explains how or why the operation was unsuccesful.
        */
    public void Reject(Exception error)
    {
        if (_state != DeferredState.PENDING)
        {
            return;
        }

        // By contact, we will always supply an Error object to the fail handlers.
        _error = error != null ? error : new Exception("Promise Rejected");
        _state = DeferredState.REJECTED;
        ClearListeners();
        InvokeFinalCallback();
    }

    /**
        * Notifies all of the 'progresses' handlers of the current progress of the deferred operation.  The supplied
        * value should be a Number between 0 and 1 (although there is no fixed validation).  Once the deferred 
        * operation has resolved further progress updates will be ignored.
        * 
        * @param ratioComplete		A number between 0 and 1 which represents the progress of the deferred oepration.
        */
    public void Progress(float ratioComplete)
    {

        _progressListeners.ForEach(progress =>
        {
            progress(ratioComplete);
        });
    }
    

    /**
        * Aborts the deferred operation; none of the handlers bound to the Promise will be invoked; typically this
        * is used when the Deferred's host needs to cancel the operation.
        */
    public void Abort()
    {
        _state = DeferredState.ABORTED;
        _outcome = _Null;
        _finalCallback = null;

        ClearListeners();
    }
    public Promise<T> Completes(Action callback)
    {
        return Completes(x => { callback(); });
    }

    public Promise<T> Completes(Action<T> callback)
    {
        if (_state == DeferredState.PENDING)
        {
            _completeListeners.Add(callback);
        }
        else if (_state == DeferredState.RESOLVED)
        {
            callback(_outcome);
        }
        if (_state == DeferredState.ABORTED)
        {
           
            
        }
        return this;
    }
    public Promise<T> Fails(Action callback)
    {
        return Fails(x => { callback(); });
    }

    public Promise<T> Fails(Action<Exception> callback)
    {
        if (_state == DeferredState.PENDING)
        {
            _failListeners.Add(callback);
        }
        else if (_state == DeferredState.REJECTED)
        {
            callback((Exception)_error);
        }

        return this;
    }


    public Promise<T> Progresses(Action<float> callback)
    {
        if (_state == DeferredState.PENDING)
        {
            _progressListeners.Add(callback);
        }

        return this;
    }

   

    public void ThenFinally(Action callback)
    {
        if (_state == DeferredState.PENDING)
        {
            _finalCallback = callback;
        }
        else
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                Debug.LogError("An error occurred:" + e);
            }
              
        }
    }

    public void ClearListeners()
    {
        _completeListeners.Clear();
        _failListeners.Clear();
        _progressListeners.Clear(); 

          
    }

    private void InvokeFinalCallback()
    {
        if (_finalCallback != null)
        {
            _finalCallback();
            _finalCallback = null;
        }
    }
    public void Destory()
    {
        Abort();
    }
    #region Promise Accessors


    Promise<T> Promise<T>.Completes(Action callback) { return (Promise<T>)Completes(callback); }
    Promise<T> Promise<T>.Completes(Action<T> callback) { return (Promise<T>)Completes(callback); }

    Promise<T> Promise<T>.Fails(Action<Exception> callback) { return (Promise<T>)Fails(callback); }
    Promise<T> Promise<T>.Fails(Action callback) { return (Promise<T>)Fails(callback); }
    Promise<T> Promise<T>.Progresses(Action<float> callback) { return (Promise<T>)Progresses(callback); }
         
    void Promise<T>.ThenFinally(Action callback) { ThenFinally(callback); }
    void Promise<T>.Destory() { Destory(); }
    #endregion
}
