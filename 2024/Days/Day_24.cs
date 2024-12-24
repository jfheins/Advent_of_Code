using Core;

namespace AoC_2024.Days;

public sealed class Day_24 : BaseDay
{
    private readonly string[] _input;

    public Day_24()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override async ValueTask<string> Solve_1()
    {
        var par = _input.SplitBy("");

        var known = par[0].Select(line => line.Split(":", StringSplitOptions.TrimEntries))
            .ToDictionary(t => t[0], t => int.Parse(t[1]));

        // foreach (var k in known)
        // {
        //     Console.WriteLine($"var {k.Key} = {k.Value};");
        // }

        var todo = par[1].Select(ParseGate).ToList();

        while (todo.Any())
        {
            var next = todo.First(it => known.ContainsKey(it.left) && known.ContainsKey(it.right));
            todo.Remove(next);
            var res = Calculate(next, known);
            known.Add(res.var, res.value);
        }

        return known.Where(it => it.Key.StartsWith('z')).OrderByDescending(it => it.Key)
            .Aggregate(0L, (a, b) => (a << 1) | (b.Value & 1L)).ToString();
    }

    private (string var, int value) Calculate(
        (string left, string op, string right, string dest) x,
        Dictionary<string, int> known)
    {
        // var csop = x.op switch
        // {
        //     "XOR" => "^",
        //     "AND" => "&",
        //     "OR" => "|",
        //     _ => ""
        // };
        // Console.WriteLine($"var {x.dest} = {x.left} {csop} {x.right};");

        return x.op switch
        {
            "XOR" => (x.dest, Left() ^ Right()),
            "OR" => (x.dest, Left() | Right()),
            "AND" => (x.dest, Left() & Right()),
            _ => throw new ArgumentOutOfRangeException()
        };

        int Left() => known[x.left];
        int Right() => known[x.right];
    }

    private (string left, string op, string right, string dest) ParseGate(string x)
    {
        var s = x.Split("->", StringSplitOptions.TrimEntries);
        var result = s[1];
        var operands = s[0].Split(" ");
        return (operands[0], operands[1], operands[2], result);
    }

    public override async ValueTask<string> Solve_2()
    {
        for (int i = 1; i < 44; i++)
        {
            var test = 1L << i;
            var calc = CalcFast(test, test);
            if (calc != 2 * test)
                Console.WriteLine($"Input 1 << {i}, expected {test * 2}, got {calc}");
        }

        var xx = new[] {"wpd", "z11", "jqf", "skh", "mdd", "z19", "wts", "z37"};
        return string.Join(",", xx.Order());
    }

    private static long CalcFast(long x, long y)
    {
        // ReSharper disable InconsistentNaming
        var x00 = (x >> 00) & 1;
        var x01 = (x >> 01) & 1;
        var x02 = (x >> 02) & 1;
        var x03 = (x >> 03) & 1;
        var x04 = (x >> 04) & 1;
        var x05 = (x >> 05) & 1;
        var x06 = (x >> 06) & 1;
        var x07 = (x >> 07) & 1;
        var x08 = (x >> 08) & 1;
        var x09 = (x >> 09) & 1;
        var x10 = (x >> 10) & 1;
        var x11 = (x >> 11) & 1;
        var x12 = (x >> 12) & 1;
        var x13 = (x >> 13) & 1;
        var x14 = (x >> 14) & 1;
        var x15 = (x >> 15) & 1;
        var x16 = (x >> 16) & 1;
        var x17 = (x >> 17) & 1;
        var x18 = (x >> 18) & 1;
        var x19 = (x >> 19) & 1;
        var x20 = (x >> 20) & 1;
        var x21 = (x >> 21) & 1;
        var x22 = (x >> 22) & 1;
        var x23 = (x >> 23) & 1;
        var x24 = (x >> 24) & 1;
        var x25 = (x >> 25) & 1;
        var x26 = (x >> 26) & 1;
        var x27 = (x >> 27) & 1;
        var x28 = (x >> 28) & 1;
        var x29 = (x >> 29) & 1;
        var x30 = (x >> 30) & 1;
        var x31 = (x >> 31) & 1;
        var x32 = (x >> 32) & 1;
        var x33 = (x >> 33) & 1;
        var x34 = (x >> 34) & 1;
        var x35 = (x >> 35) & 1;
        var x36 = (x >> 36) & 1;
        var x37 = (x >> 37) & 1;
        var x38 = (x >> 38) & 1;
        var x39 = (x >> 39) & 1;
        var x40 = (x >> 40) & 1;
        var x41 = (x >> 41) & 1;
        var x42 = (x >> 42) & 1;
        var x43 = (x >> 43) & 1;
        var x44 = (x >> 44) & 1;
        var y00 = (y >> 00) & 1;
        var y01 = (y >> 01) & 1;
        var y02 = (y >> 02) & 1;
        var y03 = (y >> 03) & 1;
        var y04 = (y >> 04) & 1;
        var y05 = (y >> 05) & 1;
        var y06 = (y >> 06) & 1;
        var y07 = (y >> 07) & 1;
        var y08 = (y >> 08) & 1;
        var y09 = (y >> 09) & 1;
        var y10 = (y >> 10) & 1;
        var y11 = (y >> 11) & 1;
        var y12 = (y >> 12) & 1;
        var y13 = (y >> 13) & 1;
        var y14 = (y >> 14) & 1;
        var y15 = (y >> 15) & 1;
        var y16 = (y >> 16) & 1;
        var y17 = (y >> 17) & 1;
        var y18 = (y >> 18) & 1;
        var y19 = (y >> 19) & 1;
        var y20 = (y >> 20) & 1;
        var y21 = (y >> 21) & 1;
        var y22 = (y >> 22) & 1;
        var y23 = (y >> 23) & 1;
        var y24 = (y >> 24) & 1;
        var y25 = (y >> 25) & 1;
        var y26 = (y >> 26) & 1;
        var y27 = (y >> 27) & 1;
        var y28 = (y >> 28) & 1;
        var y29 = (y >> 29) & 1;
        var y30 = (y >> 30) & 1;
        var y31 = (y >> 31) & 1;
        var y32 = (y >> 32) & 1;
        var y33 = (y >> 33) & 1;
        var y34 = (y >> 34) & 1;
        var y35 = (y >> 35) & 1;
        var y36 = (y >> 36) & 1;
        var y37 = (y >> 37) & 1;
        var y38 = (y >> 38) & 1;
        var y39 = (y >> 39) & 1;
        var y40 = (y >> 40) & 1;
        var y41 = (y >> 41) & 1;
        var y42 = (y >> 42) & 1;
        var y43 = (y >> 43) & 1;
        var y44 = (y >> 44) & 1;

        var z = 0L;

        z |= (x00 ^ y00) << 00;

        var hjp_c00 = y00 & x00;

        var kjs_s01 = x01 ^ y01;
        var wkq01 = kjs_s01 & hjp_c00;
        z |= (hjp_c00 ^ kjs_s01) << 01;

        var fqg01 = x01 & y01;

        var rvm02 = y02 ^ x02;
        var vdq01 = fqg01 | wkq01;
        z |= (rvm02 ^ vdq01) << 02;

        var nhp_c02 = x02 & y02;

        var hmk_c02 = vdq01 & rvm02;
        var sbt = y03 ^ x03;
        var wvt = hmk_c02 | nhp_c02;
        z |= (wvt ^ sbt) << 03;

        var gfk = x03 & y03;

        var jvf = y04 ^ x04;
        var dpv = wvt & sbt;
        var jth = gfk | dpv;
        z |= (jvf ^ jth) << 04;

        var bfn_c33 = y33 & x33;
        var rck_s32 = y32 ^ x32;
        var gns_c30 = x30 & y30;
        var hbh_s36 = y36 ^ x36;
        var wkb26 = y26 ^ x26;
        var hjq31 = x31 & y31;
        var fhk43 = y43 ^ x43;
        var kbf27 = x27 & y27;
        var hbw17 = y17 ^ x17;
        var fds05 = x05 ^ y05;
        var jbw07 = x07 & y07;
        var wnt32 = y32 & x32;
        var cgg14 = x14 ^ y14;
        var jhg41 = y41 ^ x41;
        var csw21 = y21 ^ x21;
        var rbg13 = x13 & y13;
        var jdc36 = y36 & x36;
        var gdd08 = x08 & y08;
        var rms = x31 ^ y31;
        var mcp = x23 ^ y23;
        var jhv = y38 & x38;
        var prp = y25 ^ x25;
        var tjq = y26 & x26;
        var htn = y12 ^ x12;
        var hnh = x42 & y42;
        var hpn = y05 & x05;
        var ttv = x14 & y14;
        var rjs = y08 ^ x08;
        var cng = y42 ^ x42;
        var qtv = x35 & y35;
        var rsv = y28 & x28;
        var jtg = x06 ^ y06;
        var kbb = y09 ^ x09;
        var fnc = y23 & x23;
        var hvc = y29 ^ x29;
        var jsg = x44 ^ y44;
        var bdn = x40 & y40;
        var ncf = x41 & y41;
        var tvf = y27 ^ x27;
        var bnw = y34 & x34;
        var nrg = y21 & x21;
        var sqj = x38 ^ y38;
        var wfm = y18 & x18;
        var cmp = x19 ^ y19;
        var gww = y24 ^ x24;
        var vqf = y39 ^ x39;
        var bnn = x12 & y12;
        var wmq = y33 ^ x33;
        var cqg = y25 & x25;
        var djt35 = x35 ^ y35;
        var tcq24 = y24 & x24;
        var bsw = y29 & x29;
        var skh = x15 ^ y15; // jqf, skh
        var nnn = y30 ^ x30;
        var dbj = y43 & x43;
        var cwn = x17 & y17;
        var jgg = y20 & x20;
        var gwv = x04 & y04;
        var rhd = x16 & y16;
        var hhw = x06 & y06;
        var kdd = x28 ^ y28;
        var ppk = y39 & x39;
        var nns = x40 ^ y40;
        var whn = jth & jvf;
        var nss = x34 ^ y34;
        var rhh = y37 & x37;
        var qpc = x07 ^ y07;
        var jqf = x15 & y15;
        var wwp = y22 ^ x22;
        var dmp = x13 ^ y13;
        var whf = gwv | whn;
        var vvs = fds05 & whf;
        z |= (whf ^ fds05) << 05;
        var cfn = vvs | hpn;
        var qhk = jtg & cfn;
        var qmb = hhw | qhk;
        var trv = qpc & qmb;
        var tmg = jbw07 | trv;
        var vth = tmg & rjs;
        var wpb = vth | gdd08;
        var jmp = kbb & wpb;
        z |= (wpb ^ kbb) << 09;
        z |= (rjs ^ tmg) << 08;
        var csb = y09 & x09;
        var kmr = jmp | csb;
        var rmb = x10 ^ y10;
        z |= (kmr ^ rmb) << 10;
        var fws = rmb & kmr;
        z |= (qmb ^ qpc) << 07;
        var gvj = y10 & x10;
        var qqw = fws | gvj;
        var gkc = y11 ^ x11; 
        var wpd = qqw & gkc; // wpd, z11
        var dpf = y11 & x11;
        var dtq = wpd | dpf;
        var gvh = htn & dtq;
        z |= (htn ^ dtq) << 12;
        z |= (gkc ^ qqw) << 11;
        var jkm = bnn | gvh;
        z |= (jkm ^ dmp) << 13;
        var qpw = dmp & jkm;
        var rhf = qpw | rbg13;
        var smd = rhf & cgg14;
        z |= (cgg14 ^ rhf) << 14;
        var rkt = smd | ttv;
        z |= (skh ^ rkt) << 15;
        var kjk = rkt & skh;
        var kbq = jqf | kjk;
        var rvn = y16 ^ x16;
        z |= (rvn ^ kbq) << 16;
        var vtm = kbq & rvn;
        var wrc = rhd | vtm;
        z |= (wrc ^ hbw17) << 17;
        var jks = hbw17 & wrc;
        var qvq = jks | cwn;
        var hns = x18 ^ y18;
        z |= (qvq ^ hns) << 18;
        var mts = hns & qvq;
        var wfc = wfm | mts; //mdd, z19
        z |= (wfc ^ cmp) << 19;
        var mdd = y19 & x19;
        var pbb = cmp & wfc;
        var hvn = pbb | mdd;
        var bvw20 = y20 ^ x20;
        z |= (bvw20 ^ hvn) << 20;
        var cmm = bvw20 & hvn;
        var qpm = jgg | cmm;
        z |= (qpm ^ csw21) << 21;
        var mvs = csw21 & qpm;
        var bvr = nrg | mvs;
        var cgv = wwp & bvr;
        var jsp = x44 & y44;
        z |= (jtg ^ cfn) << 06;
        z |= (wwp ^ bvr) << 22;
        var gmf = y22 & x22;
        var mkf = cgv | gmf;
        z |= (mcp ^ mkf) << 23;
        var mmk = mcp & mkf;
        var jmf = fnc | mmk;
        z |= (gww ^ jmf) << 24;
        var rjb = jmf & gww;
        var pfc = tcq24 | rjb;
        var rkh = prp & pfc;
        var jkr = rkh | cqg;
        z |= (wkb26 ^ jkr) << 26;
        z |= (pfc ^ prp) << 25;
        var hhj = wkb26 & jkr;
        var cmb = tjq | hhj;
        z |= (tvf ^ cmb) << 27;
        var mqr = cmb & tvf;
        var msf = kbf27 | mqr;
        var dpr = kdd & msf;
        var tsk = rsv | dpr;
        var bfp = hvc & tsk;
        var pwp = bsw | bfp;
        var dwg = pwp & nnn;
        z |= (pwp ^ nnn) << 30;
        z |= (tsk ^ hvc) << 29;
        var sgf = gns_c30 | dwg;
        z |= (rms ^ sgf) << 31;
        var mrg = sgf & rms;
        var ftq = mrg | hjq31;
        var hfc = rck_s32 & ftq;
        var dts = wnt32 | hfc;
        z |= (dts ^ wmq) << 33;
        z |= (msf ^ kdd) << 28;
        z |= (rck_s32 ^ ftq) << 32;
        var sbw = dts & wmq;
        var jmv = sbw | bfn_c33;
        var vtd = nss & jmv;
        var khf = vtd | bnw;
        var khs = khf & djt35;
        z |= (djt35 ^ khf) << 35;
        var rfq = qtv | khs;
        z |= (rfq ^ hbh_s36) << 36;
        z |= (jmv ^ nss) << 34;
        var pwb = rfq & hbh_s36;
        var smt = pwb | jdc36;
        var wpp = y37 ^ x37;
        var jgw = wpp & smt; // wts, z37
        var wts = jgw | rhh;
        z |= (smt ^ wpp) << 37;
        var qdn = sqj & wts;
        z |= (sqj ^ wts) << 38;
        var fkq = qdn | jhv;
        z |= (vqf ^ fkq) << 39;
        var hwb = vqf & fkq;
        var jbd = ppk | hwb;
        var nnq = jbd & nns;
        z |= (nns ^ jbd) << 40;
        var gfd = nnq | bdn;
        var bbr = jhg41 & gfd;
        z |= (jhg41 ^ gfd) << 41;
        var mwt = bbr | ncf;
        z |= (cng ^ mwt) << 42;
        var hbm = mwt & cng;
        var gsk = hbm | hnh;
        z |= (gsk ^ fhk43) << 43;
        var tnc = gsk & fhk43;
        var hks = tnc | dbj;
        z |= (jsg ^ hks) << 44;
        var nrv = jsg & hks;
        z |= (nrv | jsp) << 45;

        return z;
    }
}