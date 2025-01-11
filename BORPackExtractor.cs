public struct fileInfoStruct
{
    public const int MAX_FILENAME_LEN = 256;
    public uint pns_len;
    public uint filestart;
    public uint filesize;
    //char[MAX_FILENAME_LEN] namebuf;
    public string namebuf;

    public fileInfoStruct(BinaryReader br)
    {
        pns_len = br.ReadUInt32();
        filestart = br.ReadUInt32();
        filesize = br.ReadUInt32();
        namebuf = br.ReadCString(MAX_FILENAME_LEN);
    }
}
public class BORPackExtractor : IDisposable
{
    public const int MAX_BUFFER_LEN = 512;
    public const int MAX_LABEL_LEN = 128;

    public const int PACKFILE_PATH_MAX = 512; // Maximum length of file path string.

    public const uint PACKVERSION = 0x00000000;
    MemoryStream ms;
    BinaryReader br;



    public Dictionary<string, fileInfoStruct> files;

    public void Dispose()
    {
        ms.Dispose();
        br.Dispose();
    }

    public BORPackExtractor(FileInfo path)
    {
        Logging.Log("Reading {0}", path);
        ms = new();
        var fs = path.OpenRead();
        fs.CopyTo(ms);
        fs.Dispose();
        ms.Seek(0, SeekOrigin.Begin);

        br = new BinaryReader(ms);
        //The magic check for the file is weird. they just check if the file is at least 4 bytes???
        //Imma skip the check here for compatibility
        _ = br.ReadInt32();
        uint version = br.ReadUInt32();
        if (version != PACKVERSION)
            Logging.Warn("Pak file version is different ( found {0} expected {1} ). This could cause errors", version, PACKVERSION);


        //Seek header start
        br.BaseStream.Seek(-4, SeekOrigin.End);
        int HeaderStart = br.ReadInt32();
        br.BaseStream.Seek(HeaderStart, SeekOrigin.Begin);

        files = new();

        try
        {
            while (true)
            {
                fileInfoStruct pname = new(br);
                try
                {
                    files.Add(pname.namebuf.ToLowerInvariant(), pname);
                }
                catch (ArgumentException)
                {
                    Logging.Log(pname.namebuf.ToLowerInvariant());
                }
            }

        }
        catch (EndOfStreamException)
        {
            // it just reads till the end of file
        }
    }
    public void WriteFile(Stream writer, fileInfoStruct fileinfo)
    {
        const int BufferSize = 64 * 1024; // 64 KB buffer
        byte[] buffer = new byte[BufferSize];
        long remaining = fileinfo.filesize;
        br.BaseStream.Seek(fileinfo.filestart, SeekOrigin.Begin);

        while (remaining > 0)
        {
            int bytesToRead = (int)Math.Min(BufferSize, remaining);
            int bytesRead = br.BaseStream.Read(buffer, 0, bytesToRead);
            writer.Write(buffer, 0, bytesRead);
            remaining -= bytesRead;
        }
    }


    public void Dump(string directory)
    {
        HashSet<string> createdDirectories = new();
        createdDirectories.Add(directory);
        Directory.CreateDirectory(directory);
        foreach (var (_, pname) in files)
        {
            String betterSlash = pname.namebuf.Replace('\\', '/');
            Logging.Log("Extracting {0}", pname.namebuf);
            string filedir = string.Empty;
            string filename = betterSlash;

            int lastslash = betterSlash.LastIndexOf("/");
            if (lastslash > 0)
            {
                filedir = betterSlash.Substring(0, lastslash);
                filename = betterSlash.Substring(lastslash + 1);
                string fulldir = Path.Combine(directory, filedir);
                if (!createdDirectories.Contains(fulldir))
                {
                    createdDirectories.Add(fulldir);
                    Directory.CreateDirectory(fulldir);
                }
            }
            var outfs = File.OpenWrite(Path.Combine(directory, filedir, filename));
            WriteFile(outfs, pname);
            outfs.Dispose();
        }
        Logging.Log("Done!");

    }
}
