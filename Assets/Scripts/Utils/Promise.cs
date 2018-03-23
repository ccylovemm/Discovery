using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Promise<T>
{
    /**
		* Register a callback function which will be invoked when this Promise is in a RESOLVED state (ie: completed).
		* The supplied function should expect zero, or one argument (the outcome yeilded by the Deferred process).  
		* Note that callbacks registered after the Promise resolves will be executed immediately.  Callbacks will be
		* exectured in the order they are supplied.
		*/
    Promise<T> Completes(Action<T> callback);
    Promise<T> Completes(Action callback); 
 
	/**
		* Register a callback function which will be invoked should this Promise be rejected (ie: fail to resolve).
		* The supplied function should expect zero or one argument (an Error object yeilded by the Deferred process).
		* Note that callbacks registered after the Promise is rejected will be executed immediately.  Callbacks will be
		* exectured in the order they are supplied.
		*/
    Promise<T> Fails(Action<Exception> callback);
    Promise<T> Fails(Action callback); 

	/**
        * 
		*/
    Promise<T> Progresses(Action<float> callback);
 
         
	/**
		* Register a callback which will be executed after all other callbacks have been invoked.  Typically this 
		* is used to destroy or free the client.
		*/
	void ThenFinally(Action callback )  ;
    void Destory();
}
