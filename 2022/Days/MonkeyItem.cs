namespace AoC_2022.Days
{
    public abstract class MonkeyItem
    {
        public int CurrentMonkey { get; set; }
        public long WorryLevel { get; set; }

        public readonly int[] Inspections = new int[8];

        public void PlayRounds(int rounds)
        {
            for (int i = 0; i < rounds; i++)
            {
                PlayRound(CurrentMonkey);
            }
        }

        public void PlayRound(int newMonkey)
        {
            CurrentMonkey = newMonkey;
            Inspections[CurrentMonkey]++;
            switch (CurrentMonkey)
            {
                case 0:
                    WorryLevel = Simplify(WorryLevel * 13);
                    if (WorryLevel % 11 == 0)
                        PlayRound(3);
                    else
                        PlayRound(2);
                    break;
                case 1:
                    WorryLevel = Simplify(WorryLevel + 2);
                    if (WorryLevel % 7 == 0)
                        PlayRound(6);
                    else
                        PlayRound(7);
                    break;
                case 2:
                    WorryLevel = Simplify(WorryLevel + 6);
                    if (WorryLevel % 13 == 0)
                        PlayRound(3);
                    else
                        PlayRound(5);
                    break;
                case 3:
                    WorryLevel = Simplify(WorryLevel * WorryLevel);
                    if (WorryLevel % 5 == 0)
                        PlayRound(4);
                    else
                        PlayRound(5);
                    break;
                case 4:
                    WorryLevel = Simplify(WorryLevel + 3);
                    if (WorryLevel % 3 == 0)
                        CurrentMonkey = 1; // Throw back => round done
                    else
                        PlayRound(7);
                    break;
                case 5:
                    WorryLevel = Simplify(WorryLevel * 7);
                    if (WorryLevel % 17 == 0)
                        CurrentMonkey = 4;
                    else
                        CurrentMonkey = 1;
                    break;
                case 6:
                    WorryLevel = Simplify(WorryLevel + 4);
                    if (WorryLevel % 2 == 0)
                        CurrentMonkey = 2;
                    else
                        CurrentMonkey = 0;
                    break;
                case 7:
                    WorryLevel = Simplify(WorryLevel + 7);
                    if (WorryLevel % 19 == 0)
                        CurrentMonkey = 6;
                    else
                        CurrentMonkey = 0;
                    break;
            };
        }

        protected abstract long Simplify(long x);
    }
}