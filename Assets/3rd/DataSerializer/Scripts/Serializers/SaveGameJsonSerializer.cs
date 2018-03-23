using System;
using System.IO;
using FullSerializer;
using System.Text;
using UnityEngine;

public class SaveGameJsonSerializer : ISaveGameSerializer
{
	public void Serialize<T> ( T obj, Stream stream, Encoding encoding )
	{
		#if !UNITY_WSA || !UNITY_WINRT
		try
		{
			StreamWriter writer = new StreamWriter ( stream, encoding );
			fsSerializer serializer = new fsSerializer ();
			fsData data = new fsData ();
			serializer.TrySerialize ( obj, out data );
			writer.Write ( fsJsonPrinter.CompressedJson ( data ) );
			writer.Dispose ();
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
		#else
		StreamWriter writer = new StreamWriter ( stream, encoding );
		writer.Write ( JsonUtility.ToJson ( obj ) );
		writer.Dispose ();
		#endif
	}

	public T Deserialize<T> ( Stream stream, Encoding encoding )
	{
		T result = default(T);
		#if !UNITY_WSA || !UNITY_WINRT
		try
		{
			StreamReader reader = new StreamReader ( stream, encoding );
			fsSerializer serializer = new fsSerializer ();
			fsData data = fsJsonParser.Parse ( reader.ReadToEnd () );
			serializer.TryDeserialize ( data, ref result );
			if ( result == null )
			{
				result = default(T);
			}
			reader.Dispose ();
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
		#else
		StreamReader reader = new StreamReader ( stream, encoding );
		result = JsonUtility.FromJson<T> ( reader.ReadToEnd () );
		reader.Dispose ();
		#endif
		return result;
	}
}