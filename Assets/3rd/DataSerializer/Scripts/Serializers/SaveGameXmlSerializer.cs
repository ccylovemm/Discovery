using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class SaveGameXmlSerializer : ISaveGameSerializer
{
	public void Serialize<T> ( T obj, Stream stream, Encoding encoding )
	{
		try
		{
			XmlSerializer serializer = new XmlSerializer ( typeof ( T ) );
			serializer.Serialize ( stream, obj );
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
	}

	public T Deserialize<T> ( Stream stream, Encoding encoding )
	{
		T result = default(T);
		try
		{
			XmlSerializer serializer = new XmlSerializer ( typeof ( T ) );
			result = ( T )serializer.Deserialize ( stream );
		}
		catch ( Exception ex )
		{
			Debug.LogException ( ex );
		}
		return result;
	}
}
