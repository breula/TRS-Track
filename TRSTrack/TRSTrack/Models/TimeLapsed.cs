namespace TRSTrack.Models
{
    public static class TimeLapsed
    {
        private static int Hours = 0;
        private static int Minutes = 0;
        private static int Seconds = 0;
        private static int Milliseconds = 0;
        private static bool IsPaused = false;

        public static void Pause()
        {
            IsPaused = true;
        }

        public static void Play()
        {
            IsPaused = false;
        }

        public static void Reset()
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            Milliseconds = 0;
        }

        public static bool Reseted()
        {
            return Hours == 0 && Minutes == 0 && Seconds == 0 && Milliseconds == 0;
        }

        public static string Display()
        {
            if (IsPaused) return $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}:{Milliseconds:D3}";

            if (Milliseconds == 1000)
            {
                Seconds += 1;
                Milliseconds = 0;
            }
            else
            {
                Milliseconds++;
            }

            if (Seconds == 60)
            {
                Minutes += 1;
                Seconds = 0;
            }

            if (Minutes == 60)
            {
                Hours += 1;
                Minutes = 0;
            }

            return $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}:{Milliseconds:D3}";
        }
    }
}
