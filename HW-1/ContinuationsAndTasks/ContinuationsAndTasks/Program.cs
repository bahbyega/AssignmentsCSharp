using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ContinuationsAndTasks
{
    class Program
    {
        static void ShowSplash()
        {
            Console.WriteLine("Show Splash");
        }

        static Task RequestLicense()
        {
            Task task = Task.Run(() => {
                Random rnd = new Random();

                // (!) no cancelation for now 
                if (rnd.Next(2) < 0)
                {
                    throw null;
                }
                else
                {
                    Console.WriteLine("Request License");
                }
            });
            try
            {
                task.Wait();
            }
            catch (AggregateException) //if exception generated => cancel
            {
                Console.WriteLine("Task cancelled.");
            }
            return task;
        }

        static Task CheckForUpdate()
        {
            Task task = Task.Run(() => {
                Random rnd = new Random();

                // (!) no cancelation for now
                if (rnd.Next(2) < 0)
                {
                    throw null;
                }
                else
                    Console.WriteLine("Check for update");
            });
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Task cancelled.");
            }
            return task;
        }

        static void SetupMenus()
        {
            Console.WriteLine("Setup Menus");
        }
        static void DownloadUpdate()
        {
            Console.WriteLine("Download Update");
        }
        static void DisplayWelcomeScreen()
        {
            Console.WriteLine("Display Welcome Screen");
        }
        static void HideSplash()
        {
            Console.WriteLine("Hide splash");
        }

        static void ImitateLoadingStages()
        {
            // always execute
            ShowSplash();

            List<Task> tasks = new List<Task>();

            // (!) we want to run RequestLicense and CheckForUpdate in parallel
            tasks.Add(RequestLicense().ContinueWith(setMnsTask => //
            {
                /*long geezHowLong = 0;                         // test code to make 
                for (long i = 0; i < 1000000000; i++)           // 'Setup Menus' a heavy operation
                {                                               // and see that 'Check For Updates'
                    geezHowLong += Math.Min(i, geezHowLong);    // and 'Download Update'
                }*/                                             // run parallel to 'Setup Menus'
                SetupMenus();
            }));

            tasks.Add(CheckForUpdate().ContinueWith(DwnldUpd =>
            {
                DownloadUpdate();
            }));

            Task.WaitAll(tasks.ToArray()); //wait for tasks to end before Welcome Screen

            DisplayWelcomeScreen(); // always execute
            HideSplash(); // always execute

            Console.WriteLine("\nLoaded successfully");
        }

        static void Main(string[] args)
        {
            ImitateLoadingStages();
        }
    }
}
