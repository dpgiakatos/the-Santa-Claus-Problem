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

        /*private static long elfCounter = 0;
        private static long reindeerCounter = 0;

        private static int elfReady = 0;
        private static int reindeerReady = 0;*/

        private static volatile int elfCounter = 0;
        private static volatile int reindeerCounter = 0;

        private static volatile bool elfReady = false;
        private static volatile bool reindeerReady = false;

        static void Main(string[] args)
        {
            elf = new Semaphore(0, 10);
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
                //Interlocked.Increment(ref elfCounter);
                elfCounter++;
                elf.WaitOne();
                Console.WriteLine("Elf {0}: I am going to work.", num);
                //Thread.Sleep(250);
            }
        }

        private static void Reindeer(object num)
        {
            while (true)
            {
                Console.WriteLine("Reindeer {0}: I am waiting in reindeer's team.", num);
                //Interlocked.Increment(ref reindeerCounter);
                reindeerCounter++;
                reindeer.WaitOne();
                Console.WriteLine("Reindeer {0}: I am free.", num);
                //Thread.Sleep(750);
            }
        }

        private static void Santa(object num)
        {
            while (true)
            {
                Console.WriteLine("Santa: I am going to sleep. Butler, you are in charge.");
                butler.Release();
                santa.WaitOne();
                Console.WriteLine("Santa: I am awake.");
                //if (Interlocked.Exchange(ref elfReady, 0) == 1)
                if (elfReady)
                {
                    Console.WriteLine("Santa: The meeting with the elf's team is starting.");
                    //Interlocked.Exchange(ref elfCounter, Interlocked.Read(ref elfCounter) - 3);
                    elfReady = false;
                    elfCounter -= 3;
                    elf.Release(3);
                }
                //else if (Interlocked.Exchange(ref reindeerReady, 0) == 1)
                else if (reindeerReady)
                {
                    Console.WriteLine("Santa: I am going out with the reindeer's team.");
                    //Interlocked.Exchange(ref reindeerCounter, 0);
                    reindeerReady = false;
                    reindeerCounter = 0;
                    reindeer.Release(9);
                }
                Console.WriteLine("Santa: I just finished.");
                //Thread.Sleep(500);
            }
        }

        private static void Butler(object num)
        {
            while (true)
            {
                //if (Interlocked.Read(ref reindeerCounter) == 9)
                if (reindeerCounter == 9)
                {
                    Console.WriteLine("Butler: The reindeer's team is ready. I will go to awake santa.");
                    //Interlocked.Exchange(ref reindeerReady, 1);
                    reindeerReady = true;
                    santa.Release();
                    butler.WaitOne();
                }
                //else if (Interlocked.Read(ref elfCounter) == 3)
                else if (elfCounter == 3)
                {
                    Console.WriteLine("Butler: The elf's team is ready. I will go to awake santa.");
                    //Interlocked.Exchange(ref elfReady, 1);
                    elfReady = true;
                    santa.Release();
                    butler.WaitOne();
                }
                //Thread.Sleep(1);
            }
        }
    }
}
