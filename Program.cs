public class Program
{
    public static void Main(String[] Args)
    {
        if (Args.Length < 1)
        {
            Console.WriteLine("Usage:\n"
                    + $"./{System.AppDomain.CurrentDomain.FriendlyName} infile [outdir]");
            return;
        }
        string infile = Args[0];
        string outdir = Args.Length > 1 ? Args[1] : "output";
        try
        {
            using BORPack pak = new(infile);
            pak.Dump(outdir);
        }
        catch (Exception e)
        {
            Logging.Error(e);
        }
    }
}
