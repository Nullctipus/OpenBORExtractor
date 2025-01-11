public class Program
{
    public static void Main(String[] Args)
    {
        if (Args.Length < 2)
        {
            Console.WriteLine("Usage:\n"
                    + "Extract"
                    + $"./{System.AppDomain.CurrentDomain.FriendlyName} x infile [outdir]\n"
                    + "Build"
                    + $"./{System.AppDomain.CurrentDomain.FriendlyName} b indir [outfile]");

            return;
        }
        if (Args[0] == "x")
        {
            string infile = Args[1];
            string outdir = Args.Length > 2 ? Args[2] : "output";
            try
            {
                using BORPackExtractor pak = new(new FileInfo(infile));
                pak.Dump(outdir);
            }
            catch (Exception e)
            {
                Logging.Error(e);
            }
            return;
        }
        else if (Args[0] == "b")
        {
            DirectoryInfo indir = new DirectoryInfo(Args[1]);
            FileInfo outfile = new FileInfo(Args.Length > 2 ? Args[2] : "bor.pak");
            var pak = new BORPackBuilder(indir);
            pak.BuildPak(outfile);
            return;
        }

    }
}
