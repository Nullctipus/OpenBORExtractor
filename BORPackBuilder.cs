public class BORPackBuilder
{
    const int PACKMAGIC = 0x4B434150;
    const int VERSION = 0x00000000;
    static void RecursiveBuildFileInfos(DirectoryInfo directory, List<FileInfo> infos)
    {
        infos.AddRange(directory.GetFiles());
        foreach (var d in directory.EnumerateDirectories())
        {
            RecursiveBuildFileInfos(d, infos);
        }
    }
    public void CopyStream(Stream writer, Stream reader)
    {
        const int BufferSize = 64 * 1024; // 64 KB buffer
        byte[] buffer = new byte[BufferSize];
        long remaining = reader.Length;

        while (remaining > 0)
        {
            int bytesToRead = (int)Math.Min(BufferSize, remaining);
            int bytesRead = reader.Read(buffer, 0, bytesToRead);
            writer.Write(buffer, 0, bytesRead);
            remaining -= bytesRead;
        }
    }
    List<FileInfo> infos;
    DirectoryInfo inputdir;
    public BORPackBuilder(DirectoryInfo directory)
    {
        inputdir = directory;
        infos = new();
        RecursiveBuildFileInfos(directory, infos);
        //infos.Sort((a, b) => String.Compare(a.FullName, b.FullName));
    }
    public void BuildPak(FileInfo outfile)
    {
        using var fs = outfile.OpenWrite();
        using var bw = new BinaryWriter(fs);

        bw.Write(PACKMAGIC);
        bw.Write(VERSION);
        List<fileInfoStruct> structs = new();
        string relative = inputdir.FullName;//.Substring(0, inputdir.FullName.LastIndexOf('/'));
        foreach (var info in infos)
        {
            FileStream openfs = info.OpenRead();
            fileInfoStruct str = new();
            str.filestart = (uint)fs.Position + 1;
            str.filesize = (uint)openfs.Length;
            str.namebuf = Path.GetRelativePath(relative, info.FullName);
            Logging.Log(str.namebuf + " " + str.namebuf.Length);
            str.pns_len = (uint)(12 + str.namebuf.Length + 1);
            CopyStream(fs, openfs);
            openfs.Dispose();
            structs.Add(str);
        }
        uint headerStart = (uint)fs.Position;
        foreach (var str in structs)
        {
            bw.Write(str.pns_len);
            bw.Write(str.filestart);
            bw.Write(str.filesize);
            for (int i = 0; i < str.namebuf.Length; i++)
                bw.Write((byte)str.namebuf[i]);
            bw.Write((byte)0);
        }
        bw.Write(headerStart);

    }
}
