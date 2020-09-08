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

        static Task RequestLicense(Random rnd)
        {
            Task task = Task.Run( () => {
                // random chance (50%) of throwing an exception
                if (rnd.Next(2) == 0)
                {
                    throw null; // (!) needs to be a NoLicenseException
                } else {
                    Console.WriteLine("Request License");
                }
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException) //if exception generated => cancel
            {
                Console.WriteLine("License Request : No license.");
            }

            return task;
        }

        static Task CheckForUpdate(Random rnd)
        {
            Task task = Task.Run( () => {

                if (rnd.Next(2) == 1)
                {
                    throw null; // (!) needs to be a NoLicenseException
                } else
                    Console.WriteLine("Check for update");
            });

            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Check For Update cancelled.");
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

            Random rnd = new Random(); // random for exceptions
            List<Task> tasks = new List<Task>();

            Task rqstLicense = RequestLicense(rnd);
            Task checkForUpd = CheckForUpdate(rnd);

            if (rqstLicense.IsFaulted)
            {
                rqstLicense.ContinueWith(setMnsTask =>
                    {
                        Console.WriteLine("No license. Setup Menus cancelled.");
                    }
                );
            } else {
                tasks.Add(rqstLicense.ContinueWith(setMnsTaskFault => //
                {
                /*long geezHowLong = 0;                         // test code to make 
                for (long i = 0; i < 1000000000; i++)           // 'Setup Menus' a heavy operation
                {                                               // and see that 'Check For Updates'
                    geezHowLong += Math.Min(i, geezHowLong);    // and 'Download Update'
                }*/                                             // run parallel to 'Setup Menus'
                SetupMenus();
                }));
            }

            if (checkForUpd.IsFaulted)
            {
                rqstLicense.ContinueWith(DwnldUpdkFault =>
                    {
                        Console.WriteLine("Connection fault. Can't check the update.");
                    }
                );
            } else {
                tasks.Add(checkForUpd.ContinueWith(DwnldUpd =>
                {
                    DownloadUpdate();
                }));
            }

            Task.WaitAll(tasks.ToArray()); //wait for tasks to end before Welcome Screen

            if (!rqstLicense.IsFaulted) // execute if license is OK
            {
                DisplayWelcomeScreen(); 
                HideSplash();

                if (checkForUpd.IsFaulted)
                    Console.WriteLine("\nLoaded without the update.");
                else
                    Console.WriteLine("\nLoaded successfully.");
            }

            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            ImitateLoadingStages();
        }
    }
}