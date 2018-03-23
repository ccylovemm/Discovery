using System.IO;
using System.Text;

public interface ISaveGameSerializer
{
	void Serialize<T> ( T obj, Stream stream, Encoding encoding );
	T Deserialize<T> ( Stream stream, Encoding encoding );
}