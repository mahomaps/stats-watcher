namespace MMStatsWatcher;

public class TempEntry
{
    public string? date;
    public string? v;
    public string? parsed;
    public string? device;
    public string? gt;
    public string? uf;

    public bool IsBroken()
    {
        if (device == null && parsed == null)
            // either device or parsed device must exist
            return true;
        return v == null || date == null;
    }
}