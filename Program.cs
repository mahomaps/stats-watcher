using System.Globalization;
using MMStatsWatcher;
using Newtonsoft.Json;

const string url = "https://nnp.nnchan.ru/mahomaps/log.txt";

Console.Write("Show from date (MM.DD.YYYY): ");
var from = DateTime.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

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
    .Where(x => x.date > from).ToArray();

Console.WriteLine();
Console.WriteLine($"Total launch count: {lines.Length}");
Console.WriteLine();
PrintGeoTypeStat(lines, 1, "Я");
PrintGeoTypeStat(lines, 2, "Ы");
PrintGeoTypeStat(lines, 3, "Ъ");

var allVers = lines.Select(x => x.v).Distinct().Order().ToArray();
var maxlen = lines.Select(x => x.device.Length).Max();

Console.WriteLine();
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

static void PrintLine(string a, string b, IEnumerable<string> c, int maxALen)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write(a.PadRight(maxALen + 1));
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write(b.PadRight(9));
    Console.ForegroundColor = ConsoleColor.White;
    foreach (var n in c)
    {
        Console.Write(n.PadRight(9));
    }

    Console.WriteLine();
}

static void PrintGeoTypeStat(IEnumerable<Entry> data, int t, string name)
{
    var f = data.Where(x => x.gt == t).ToArray();
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