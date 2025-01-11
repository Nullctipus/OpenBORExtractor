using System.Text;
public static class BinaryReaderExtentions
{
    public static string ReadCString(this BinaryReader br, int size)
    {
        char c;

        char[] chars = new char[size];
        int i;
        for (i = 0; i < size && (c = br.ReadChar()) != 0; i++)
            chars[i] = c;
        return string.Create(i, chars, (ret, buf) =>
        {
            for (int j = 0; j < ret.Length; j++) ret[j] = chars[j];
        });
    }
    public static string ReadCString(this BinaryReader br)
    {
        char c;
        StringBuilder sb = new();
        while ((c = br.ReadChar()) != 0)
        {
            sb.Append(c);
        }
        return sb.ToString();
    }
}
