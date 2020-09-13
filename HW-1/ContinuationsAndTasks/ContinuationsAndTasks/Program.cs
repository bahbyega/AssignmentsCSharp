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
            string message = "Show Splash";
            Console.WriteLine(message);
        }

        // There could be exceptions thrown in next two methods.
        static void RequestLicense()
        {
            string message = "Request Licence";
            Console.WriteLine(message);
            //GenerateException(task); // throw some exception
        }
        static void CheckForUpdate()
        {
            string message = "Check for Update";
            Task task = Task.Run(() => Console.WriteLine(message));
            //GenerateException(task); // throw some exception
        }

        static void SetupMenus()
        {
            string message = "Setup Menus";
            Console.WriteLine(message);
        }
        static void DownloadUpdate()
        {
            string message = "Download Update";
            Console.WriteLine(message);
        }
        static void SetupMenusFaulted()
        {
            string message = "!!! No license: Setup Menus cancelled.";
            Console.WriteLine(message);
        }
        static void DownloadUpdateFaulted()
        {
            string message = "!!! Connection failure: Dowlnoad Update cancelled";
            Console.WriteLine(message);
        }

        static void DisplayWelcomeScreen()
        {
            string message = "Display Welcome Screen";
            Console.WriteLine(message);
        }
        static void HideSplash()
        {
            string message = "Hide splash";
            Console.WriteLine(message);
        }
        
        // Generate exception
        static void GenerateException(Task task)
        {
            if (GetException())
            {
                throw null;
            }
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            { }
        }
        
        // Return true if exception generated
        // false otherwise
        static bool GetException()
        {
            Random rnd = new Random();
            int chance = rnd.Next(2); // chance of exception
            if (chance == 1)
                return true;
            return false;
        }

        // Run tasks continuously
        static void ImitateLoadingStages()
        {
            // always execute
            Task showSplash = Task.Run(() => ShowSplash());
            Task rqstLicense = showSplash.ContinueWith(nextTask => RequestLicense());
            Task checkForUpd = showSplash.ContinueWith(nextTask => CheckForUpdate());

            // Handle tasks that can be faulted. 
            // Care whether they're faulted or not. 
            Task setupMenus = rqstLicense.ContinueWith(nextTask => SetupMenus(), TaskContinuationOptions.NotOnFaulted);
            Task dwnldUpd = checkForUpd.ContinueWith(nextTask => DownloadUpdate(), TaskContinuationOptions.NotOnFaulted);

            // Handle faulted tasks
            Task setupMenusException = rqstLicense.ContinueWith(nextTask => SetupMenusFaulted(), TaskContinuationOptions.OnlyOnFaulted);
            Task dwnldUpdException = checkForUpd.ContinueWith(nextTask => DownloadUpdateFaulted(), TaskContinuationOptions.OnlyOnFaulted);

            // Show welcome screen only if license good.
            // displayWlcmScreen task runs only afrer setupMenus, which means
            // it won't run if there's any excpetion. So no need to check for it.
            Task displayWlcmScreen = setupMenus.ContinueWith(nextTask => DisplayWelcomeScreen());
            Task hideSplash = displayWlcmScreen.ContinueWith(nextTask => HideSplash());

            // Display final message if all tasks loaded without exception
            if (!rqstLicense.IsFaulted && !checkForUpd.IsFaulted) // check if no exception
            {

                hideSplash.ContinueWith(finalMessage => Console.WriteLine("Loaded successfully!"));
            }
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            ImitateLoadingStages();
        }
    }
}