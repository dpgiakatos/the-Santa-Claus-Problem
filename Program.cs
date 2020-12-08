using System;
using System.Threading;

namespace the_Santa_Claus_Problem
{
    class Program
    {
        private static Semaphore elf;
        private static Semaphore reindeer;
        private static Semaphore santa;
        private static Semaphore butler;

        private static long elfCounter = 0;
        private static long reindeerCounter = 0;

        private static int elfReady = 0;
        private static int reindeerReady = 0;

        static void Main(string[] args)
        {
            elf = new Semaphore(0, 3);
            reindeer = new Semaphore(0, 9);
            santa = new Semaphore(0, 1);
            butler = new Semaphore(0, 1);

            Thread santaThread = new Thread(new ParameterizedThreadStart(Santa));
            santaThread.Start();

            Thread butlerThread = new Thread(new ParameterizedThreadStart(Butler));
            butlerThread.Start();

            for (int i = 0; i < 10; i++)
            {
                Thread elfThread = new Thread(new ParameterizedThreadStart(Elf));
                elfThread.Start(i);
            }
            for (int i=0; i<9; i++)
            {
                Thread reindeerThread = new Thread(new ParameterizedThreadStart(Reindeer));
                reindeerThread.Start(i);
            }
        }

        private static void Elf(object num)
        {
            while (true)
            {
                Console.WriteLine("Elf {0}: I am waiting in elf's team.", num);
                Interlocked.Increment(ref elfCounter);
                elf.WaitOne();
                Console.WriteLine("Elf {0}: I am going to work.", num);
            }
        }

        private static void Reindeer(object num)
        {
            while (true)
            {
                Console.WriteLine("Reindeer {0}: I am waiting in reindeer's team.", num);
                Interlocked.Increment(ref reindeerCounter);
                reindeer.WaitOne();
                Console.WriteLine("Reindeer {0}: I am free.", num);
            }
        }

        private static void Santa(object num)
        {
            while (true)
            {
                Console.WriteLine("Santa: I am going to sleep. Butler, you are in charge.");
                santa.WaitOne();
                Console.WriteLine("Santa: I am awake.");
                if (Interlocked.Exchange(ref elfReady, 0) == 1)
                {
                    Console.WriteLine("Santa: The meeting with the elf's team is starting.");
                    Interlocked.Add(ref elfCounter, -3);
                    elf.Release(3);
                                    }
                else if (Interlocked.Exchange(ref reindeerReady, 0) == 1)
                {
                    Console.WriteLine("Santa: I am going out with the reindeer's team.");
                    Interlocked.Exchange(ref reindeerCounter, 0);
                    reindeer.Release(9);
                }
                Console.WriteLine("Santa: I just finished.");
                butler.Release();
            }
        }

        private static void Butler(object num)
        {
            while (true)
            {
                if (Interlocked.Read(ref reindeerCounter) == 9)
                {
                    Console.WriteLine("Butler: The reindeer's team is ready. I will go to awake santa.");
                    Interlocked.Exchange(ref reindeerReady, 1);
                    santa.Release();
                    butler.WaitOne();
                                    }
                else if (Interlocked.Read(ref elfCounter) >= 3)
                {
                    Console.WriteLine("Butler: The elf's team is ready. I will go to awake santa.");
                    Interlocked.Exchange(ref elfReady, 1);
                    santa.Release();
                    butler.WaitOne();
                }
            }
        }
    }
}
