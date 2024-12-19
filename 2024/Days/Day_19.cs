using System.Buffers;
using Core;
using System.Linq;
using System.Drawing;
using System.Text.RegularExpressions;
using MoreLinq.Extensions;

namespace AoC_2024.Days;

public sealed partial class Day_19 : BaseDay
{
    private readonly string[] _towels;
    private readonly string[] _designs;

    public Day_19()
    {
        var input = File.ReadAllLines(InputFilePath).SplitBy("");
        _towels = input[0].Single().Split(",", StringSplitOptions.TrimEntries);
        _designs = input[1].ToArray();
        
        
        AppDomain domain = AppDomain.CurrentDomain;
        // Set a timeout interval of 2 seconds.
        domain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromSeconds(2));
    }

    public override async ValueTask<string> Solve_1()
    {
        return "";
        // // Remove common prefixes
        // for (int i = 0; i < 10; i++)
        // {
        //     for (int j = 0; j < _towels.Length; j++)
        //     {
        //         for (int k = 0; k < _towels.Length; k++)
        //         {
        //             if (j != k && _towels[j].StartsWith(_towels[k]))
        //             {
        //                 if (expr)
        //                 {
        //                     
        //                 }
        //             }
        //         }
        //     }
        // }
        
        
        var c = _designs.Count(design => RegexMatch(design));
        return c.ToString();
    }

    public override async ValueTask<string> Solve_2()
    {
        var c = _designs.Sum(design => CountMatches(design));
        return c.ToString();
    }

    private static bool RegexMatch(ReadOnlySpan<char> design)
    {
        Console.Write($"Matching {design} ...");
        try
        {
            var r = MyRegex().IsMatch(design);
            Console.WriteLine(r);
            return r;
        }
        catch (TimeoutException e)
        {
            Console.WriteLine($"Timeout :-(");
            return false;
        }
    }

    private Dictionary<string, long> _cache = new();
    
    private long CountMatches(ReadOnlySpan<char> design)
    {
        if (design.IsEmpty)
            return 1;

        var l = _cache.GetAlternateLookup<ReadOnlySpan<char>>();

        if (l.ContainsKey(design))
        {
            return l[design];
        }

        var matches = 0L;
        foreach (var t in _towels)
        {
            if (design.StartsWith(t))
            {
              //  Console.WriteLine($"Recurse into {t} / {design}");
                matches += CountMatches(design[t.Length..]);
            }
        }
        
        l[design] = matches;
        return matches;
    }

    [GeneratedRegex("^(r|wr|b|g|bwu|rb|gb|br)+$")]
    private static partial Regex ExampleRegex();

    [GeneratedRegex("^(bwwww|gwwug|ruw|wrbuwrbw|ugugb|rwwgw|wwb|ubgg|rubbwu|wgbuurbb|wrrbru|gwg|uuwwg|ugrrgr|uugrb|grwb|gwrb|buwrwuw|wrb|ruuggwr|rub|guwwr|gwrur|uug|ugguwru|uwr|rwuug|bwgw|ruu|wrbb|wub|wuwgugg|grgwrww|bgubw|wruuuu|bwg|wu|wbrbgr|bgwg|bgrubu|ggru|bgguww|rubgugrw|grwguu|wbg|ugub|urbwuurr|grb|ug|gbu|gbgwurbw|rrrburr|uggru|rubuwrrw|guwgrb|rbr|uruw|gggur|ugwg|bbwrr|ggbbbru|grbug|uuubww|rruug|ubbgwgr|brb|rbbuwbu|ggbrru|uuwugb|ubbgr|bubg|grbb|rgb|rwur|ubuuu|gwuwb|bubgrgr|wuuu|bwwg|urugg|wubuwubw|wgrb|gwggrgu|rwbu|bbbubgrg|rw|ggug|gwbgwbrb|bgugww|ggurrwg|rbbwuwu|wgw|bwb|bbw|urru|ugg|uwguu|wggw|wgr|urrr|urguwr|bbb|bgwbgub|guw|gu|gurwr|rgw|wbb|wggu|brbbg|wbw|rur|gub|rrbbr|uugruww|rbbrb|bwgrgr|rwrurww|grurg|brur|wbr|rgu|gguwr|bwurb|grguw|wuu|rwu|ubu|wbbwwub|urg|rww|bgg|wrwg|ugr|uru|urwggw|buwur|bbrgbwgr|bggbbgw|bbgu|rurrr|ugww|uurbur|brbw|wgwrbr|wbuuw|gwgu|rbw|gbwugwb|gbw|bwwgwr|wurrgwu|wwbg|rburuu|wrruwg|gg|bbbwugg|g|bg|wwguuu|gruwgu|ggrr|ur|bwwru|rwgub|rgwuu|wur|wuwrugw|uw|bubggbr|ubug|gwbbrw|rubgwg|rwbuub|rgrbugb|rbgb|wwgb|brbgw|uwwr|rbrg|uwrrur|uuwb|wug|bwwgrgb|rg|wuwww|bgrb|uurgw|rrburg|wubg|rwrwur|rwb|wrw|bwrw|wg|wwgrubwg|bugb|uuubguuw|gruu|wggruuu|urwu|wuugb|rrwww|bwrugbrb|wrgg|bru|ggww|wrbrrg|ugbu|rwwrrguu|wrg|buguu|ugbbww|urwur|grr|uuw|rrub|rb|bww|wgwr|ubb|bbbuub|ggbrw|rbwb|ubw|uuwg|wgrrbr|grw|gwu|wuwwgr|rwuw|gbbr|bguur|gwgrrw|bbu|ggg|grg|urbwwrr|bwgbuw|bw|gbrw|wbwrb|rurg|uurg|guugb|uww|brrbggbu|ruww|ru|rwguw|wgwbug|wwu|wgwwbur|bu|rguu|uwwwg|rug|wwg|brg|uwu|ubr|burg|ruug|rwr|uurugu|gr|uwubu|rr|rbwrbw|gw|uwwuu|ruuwb|rbg|wwrw|wruubg|bgwgrwbr|wuru|ggrgr|br|rwuu|grgbgg|urw|uub|rgrbb|wbuwb|wgwbg|ubwu|gwb|uu|rrrr|rru|gwr|uuubu|grgw|gbb|gwug|wbggwu|gggr|wr|wuw|wgg|buw|gbuwrwu|gb|uwb|bgubgb|rggbbbu|rgubgggr|rbgr|ruwr|wwr|grgbrb|ugrubr|ubgu|ugw|ugrr|ubg|ubgb|urb|wgb|wbwrbw|rugrr|ub|gbr|bugrg|ugu|rgrw|rrrrurr|bgbw|wgwg|guu|wrrubu|www|wrbu|bgrguwr|wwur|ubgbgug|wbwu|buu|gbgubrw|b|ggr|rrg|wwwb|wgrrrw|ubgrug|rrr|ugbb|wugubbur|bgwb|buwww|ugb|buub|wbrru|wuwguur|gwrggg|bgb|gru|bwwr|bgr|gbbw|wuruw|gbrgu|uwg|ubrg|ggu|bbwg|ugrbug|bbr|r|ugbwub|ggb|bbg|bggwb|bub|gbg|burbr|brw|wbuu|gugbb|uwubg|gbgwr|bgwwru|rgugb|uubgr|bb|gug|rggb|wugub|wrgu|wrwguug|gur|wru|bgbr|bug|bwu|ubbur|uurugb|ubgug|bgrr|bgbb|uwbrrug|bburur|rgr|brbbrw|rrrrwu|rgg|urwuwr|wwgr|bur|rugwg|bgu|urbg|wwrgbuw|rrgwrbb|uuu|grwgg|bwr|wrbrb|wrr|uwuuur|rrubu|ugwrg|rwuwg|wgu|rwru|rrww|urr|rwg|bgw|rbb|bgbubrg|bwbb|u|wggurggu|bubgw|ggw|gugg|ugrru|wbu|rggrrb|ubrgggwg|bwuu|rurb|wgrrg|brr|rubr)+$")]
    private static partial Regex MyRegex();
}