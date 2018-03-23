using System;
using System.IO;

#if !UNITY_WSA || !UNITY_WINRT
using System.Runtime.Serialization.Formatters.Binary;
#endif

using System.Text;
using UnityEngine;

public class SaveGameBinarySerializer : ISaveGameSerializer
{
	public void Serialize<T> ( T obj, Stream stream, Encoding encoding )
	{
		#if !UNITY_WSA || !UNITY_WINRT
		try
		{
			BinaryFormatter formatter = new BinaryFormatter ();
			formatter.Serialize ( stream, obj );
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
		#else
		Debug.LogError ( "SaveGameFree: The Binary Serialization isn't supported in Windows Store and UWP." );
		#endif
	}

	public T Deserialize<T> ( Stream stream, Encoding encoding )
	{
		T result = default(T);
		#if !UNITY_WSA || !UNITY_WINRT
		try
		{
			BinaryFormatter formatter = new BinaryFormatter ();
			result = ( T )formatter.Deserialize ( stream );
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
		#else
		Debug.LogError ( "SaveGameFree: The Binary Serialization isn't supported in Windows Store and UWP." );
		#endif
		return result;
	}
}
