using System.Globalization;
using MMStatsWatcher;
using Newtonsoft.Json;

const string url = "https://nnp.nnchan.ru/mahomaps/log.txt";

Console.ForegroundColor = ConsoleColor.White;
Console.Write("Show from date (MM.DD.YYYY): ");
var dateFrom = DateTime.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
var dateTo = DateTime.Now;
var days = Math.Ceiling((dateTo - dateFrom).TotalDays);

if (dateFrom >= dateTo)
{
    Console.WriteLine("Invalid date!");
    return;
}

using HttpClient hc = new HttpClient();
Console.WriteLine();
Console.WriteLine($"GET: {url}");
var rawLines = hc.GetStringAsync(url).Result.Split('\n');
Console.WriteLine("GET: OK");
var lines = rawLines
    .Select(JsonConvert.DeserializeObject<TempEntry>)
    .Where(x => x != null && !x.IsBroken())
    .Select(x => new Entry(x!))
    .Where(x => x.device != "сиськи" && x.device != "asd")
    .Where(x => x.date > dateFrom).ToArray();

// totals
{
    PrintUtils.PrintHeader("Total stats");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Launch count: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{lines.Length}");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Days passed: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{(int) days}");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Launches per day: ");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"~{lines.Length / days:F2}");
}

// version/device
{
    var allVers = lines.Select(x => x.v).Distinct().Order().ToArray();
    var maxlen = lines.Select(x => x.device.Length).Max();

    PrintUtils.PrintHeader("Versions usage");

    PrintLine("Versions:", "All", allVers, maxlen);
    Console.WriteLine();

    var bydev = lines.GroupBy(x => x.device).OrderByDescending(x => x.Count());
    foreach (var dev in bydev)
    {
        string name = dev.Key;
        string total = dev.Count().ToString();
        var byVers = allVers.Select(x => dev.Count(y => y.v == x).ToString());
        PrintLine(name, total, byVers, maxlen);
    }
}

// per-day
{
    PrintUtils.PrintHeader("Usage by day");

    var byday = lines.GroupBy(x => x.date.Date).OrderBy(x => x.Key).ToArray();
    var maxLaunches = byday.Select(x => x.Count()).Max();
    foreach (var day in byday)
    {
        var count = day.Count();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{day.Key.Day:00}.{day.Key.Month:00} ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(count.ToString().PadLeft(3));
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(' ');
        Console.Write(new string('#', count));
        Console.Write(new string(' ', maxLaunches - count));
        if (count != 0)
        {
            var mostUsed = day.GroupBy(x => x.device).OrderByDescending(x => x.Count()).First();
            Console.Write(" Most used: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(mostUsed.Key);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" ({mostUsed.Count()})");
        }

        Console.WriteLine();
    }
}

// geopoint
{
    PrintUtils.PrintHeader("Geopoint look choice stats");
    PrintUtils.PrintGeoTypeStat(lines, 1, "Я");
    PrintUtils.PrintGeoTypeStat(lines, 2, "Ы");
    PrintUtils.PrintGeoTypeStat(lines, 3, "Ъ");
}

// telemetry
{
    PrintUtils.PrintHeader("Functions usage");
    var funcNames = Enum.GetNames<UsageFlags>();
    var maxLen = funcNames.Select(x => x.Length).Max();
    var funcs = Enum.GetValues<UsageFlags>().Select(x => lines.Where(y => y.usage.HasFlag(x)));
    var zip = funcNames.Zip(funcs);

    foreach (var func in zip)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(func.First.PadRight(maxLen + 1));
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("Total: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write(func.Second.Count());
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(", per-day: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"~{func.Second.Count() / days:F2}");
        Console.WriteLine();
    }
}

// end
{
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine();
    Console.WriteLine();
}

static void PrintLine(string a, string b, IEnumerable<string> c, int maxALen)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write(a.PadRight(maxALen + 1));
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write(b.PadRight(9));
    foreach (var n in c)
    {
        if (n.Equals("0"))
            Console.ForegroundColor = ConsoleColor.Gray;
        else
            Console.ForegroundColor = ConsoleColor.White;
        Console.Write(n.PadRight(9));
    }

    Console.WriteLine();
}