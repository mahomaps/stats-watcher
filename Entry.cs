namespace MMStatsWatcher;

public class Entry
{
    public DateTime date;
    public string v;
    public string device;
    public int gt;

    public Entry(TempEntry te)
    {
        {
            var dsplit = te.date!.Split(' ');
            var dt = dsplit[0].Split('.').Select(int.Parse).ToArray();
            var tm = dsplit[1].Split(':').Select(int.Parse).ToArray();
            date = new DateTime(dt[2], dt[0], dt[1], tm[0], tm[1], tm[2]);
        }
        v = te.v!.Contains("1.0.9.") ? "1.0.9.x" : te.v;
        string s = (te.parsed?.Split('|').First()) ?? te.device!.Split('/').First();
        if (s.Contains("MicroEmulator"))
            device = "MicroEmulator";
        else if (s.Contains("KEm"))
            device = "KEmulator";
        else if (s.Contains("j2me"))
            device = "j2me";
        else if (s.Contains("o:Linux"))
            device = "J2MELoader";
        else
            device = s;

        gt = te.gt == null ? 0 : int.Parse(te.gt);
    }
}