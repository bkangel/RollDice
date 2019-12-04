using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            int DICE = 3;
            int TRYCOUNT = 30;

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                if (args.Count() < 2)
                {
                    Console.WriteLine($"Usage : {Path.GetFileName(Assembly.GetEntryAssembly().Location)} [n-Sided Dice] [try count]");
                    return;
                }

                DICE = Int32.Parse(args[0]);
                TRYCOUNT = Int32.Parse(args[1]);
            }

            RollDice r = new RollDice(DICE, TRYCOUNT);
            r.Calc2();
        }
    }

    public class Element
    {
        public Element(long c, int s)
        {
            @case = c;
            sum = s;
        }
        public long @case { get; set; }
        public int sum { get; set; }


        public override string ToString()
        {
            return $"Case:{@case}, Sum:{sum}";
        }
    };

    public class RollDice
    {
        public int Face { get; private set; } = 3;
        public int TryCount { get; private set; } = 7;
        private Dictionary<long, Element[]> Prob { get; } = new Dictionary<long, Element[]>();
        private Dictionary<long, long[]> tryCount = new Dictionary<long, long[]>();
        public RollDice(int face, int tryCount)
        {
            Face = face;
            TryCount = tryCount;

            Prob.Add(1, new Element[Face]);
            for (int i = 0; i < Face; ++i)
            {
                Prob[1][i] = new Element(1, i + 1);
            }

            for (int i = 2; i <= TryCount; ++i)
            {
                Prob.Add(i, new Element[Face * i - i + 1]);
            }

            int idx = 0;
            for (int n = 2; n <= TryCount; ++n)
            {
                idx = 0;
                for (int j = n; j <= Face * n; ++j)                // FACE면을 갖는 주사위를 n번 던졌을 때 나올 수 있는 값의 (min:max == n : FACE * n)
                {
                    Prob[n][idx] = NextDice2(n, Prob[n - 1], j);
                    idx++;
                }
            }
        }
        public void Calc2()
        {
            var e = Prob[TryCount];
            long sum = 0;
            foreach (var v in e.Reverse())
            {
                //if (v.sum >= 150)
                {
                    sum += v.@case;
                }
                Console.WriteLine(" 합이 {0,3} 이상일 확률 => {1,18:F14}%", v.sum, ((sum / Math.Pow(Face, TryCount)) * 100.0d));//.ToString("F14", CultureInfo.InvariantCulture));
            }
        }

        private Element NextDice2(long n, Element[] a, int l)
        {
            long x = 0;

            for (int i = 1; i <= Face; ++i)
            {
                if ((l - i >= n - 1) && (l - i <= Face * (n - 1)))
                {
                    x = x + a[l - i - (n - 2) - 1].@case;
                }
            }
            return new Element(x, l);
        }

        public void Calc()
        {
            //int[] a1 = new int[6 * 1 - 1 + 1] { 1, 1, 1, 1, 1, 1 };
            //int[] a2 = new int[6 * 2 - 2 + 1];
            //int[] a3 = new int[6 * 3 - 3 + 1];
            //int[] a4 = new int[6 * 4 - 4 + 1];

            tryCount.Add(1, Enumerable.Repeat<long>(1, Face * 1 - 1 + 1).ToArray());
            for (int i = 2; i <= TryCount; ++i)
            {
                tryCount.Add(i, new long[Face * i - i + 1]);
            }

            int idx = 0;
            //for (int i = 2; i <= 12; ++i)
            //{
            //    tryCount[2][idx] = NextDice(2, tryCount[1], i);
            //    idx++;
            //}

            //idx = 0;
            //for (int i = 3; i <= 18; ++i)
            //{
            //    tryCount[3][idx] = NextDice(3, tryCount[2], i);
            //    idx++;
            //}

            for (int n = 2; n <= TryCount; ++n)
            {
                idx = 0;
                for (int j = n; j <= Face * n; ++j)                // FACE면을 갖는 주사위를 n번 던졌을 때 나올 수 있는 값의 (min:max == n : FACE * n)
                {
                    tryCount[n][idx] = NextDice(n, tryCount[n - 1], j);
                    idx++;
                }
            }
        }

        private long NextDice(long n, long[] a, int l)
        {
            long x = 0;

            for (int i = 1; i <= Face; ++i)
            {
                if ((l - i >= n - 1) && (l - i <= Face * (n - 1)))
                {
                    x = x + a[l - i - (n - 2) - 1];
                }
            }
            return x;
        }
    }
}
