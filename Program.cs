using System;
using System.Threading;

namespace the_Santa_Claus_Problem
{
    class Program
    {
        private static Semaphore elf;
        private static Semaphore reindeer;
        private static Semaphore santa;
        
        private static Semaphore buffer;
        private static Semaphore elfBuffer;

        private static long elfCounter = 0;
        private static long reindeerCounter = 0;


        static void Main(string[] args)
        {
            elf = new Semaphore(0, 3);
            reindeer = new Semaphore(0, 9);
            santa = new Semaphore(0, 1);
            
            buffer = new Semaphore(1, 1);
            elfBuffer = new Semaphore(1, 1);

            Thread santaThread = new Thread(new ParameterizedThreadStart(Santa));
            santaThread.Start();

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
                elfBuffer.WaitOne();
                Console.WriteLine("Elf {0}: I am waiting in elf's team.", num);
                Interlocked.Increment(ref elfCounter);
                if (Interlocked.Read(ref elfCounter) >= 3)
                {
                    buffer.WaitOne();
                    santa.Release();
                }
                elfBuffer.Release();
                elf.WaitOne();
                Console.WriteLine("Elf {0}: I am going to work.", num);
                Thread.Sleep(250);
            }
        }

        private static void Reindeer(object num)
        {
            while (true)
            {
                Console.WriteLine("Reindeer {0}: I am waiting in reindeer's team.", num);
                Interlocked.Increment(ref reindeerCounter);
                if (Interlocked.Read(ref reindeerCounter) == 9)
                {
                    buffer.WaitOne();
                    santa.Release();
                }
                reindeer.WaitOne();
                Console.WriteLine("Reindeer {0}: I am free.", num);
                Thread.Sleep(500);
            }
        }

        private static void Santa(object num)
        {
            while (true)
            {
                Console.WriteLine("Santa: I am going to sleep.");
                santa.WaitOne();
                Console.WriteLine("Santa: I am awake.");
                if (Interlocked.Read(ref reindeerCounter) == 9)
                {
                    Console.WriteLine("Santa: I am going out with the reindeer's team.");
                    Interlocked.Exchange(ref reindeerCounter, 0);
                    reindeer.Release(9);
                }
                else if (Interlocked.Read(ref elfCounter) >= 3)
                {
                    Console.WriteLine("Santa: The meeting with the elf's team is starting.");
                    Interlocked.Add(ref elfCounter, -3);
                    elf.Release(3);
                }
                Console.WriteLine("Santa: I just finished.");
                buffer.Release();
            }
        }
    }
}
