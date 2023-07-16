namespace MMStatsWatcher;

public static class PrintUtils
{
    public static void PrintHeader(string text)
    {
        int width = Console.BufferWidth;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        for (int i = 0; i < width; i++)
        {
            Console.Write('=');
        }

        Console.Write("=| ");
        Console.WriteLine(text);
        for (int i = 0; i < width; i++)
        {
            Console.Write('=');
        }

        Console.WriteLine();
        Console.WriteLine();
    }

    public static void PrintGeoTypeStat(IEnumerable<Entry> data, int t, string name)
    {
        var f = data.Where(x => x.gt == t).ToArray();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Geopoint type ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"\"{name}\"");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" was used ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(f.Length);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" times.");
        var max = f.GroupBy(x => x.device).MaxBy(x => x.Count());
        if (max != null)
        {
            Console.Write(" Most used device: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(max.Key);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($", {max.Count()} times.");
        }
        else
            Console.WriteLine();
    }
}