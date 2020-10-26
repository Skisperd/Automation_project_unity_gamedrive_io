using System;
using NUnit.Framework;
using gdio.unity_api;

namespace _2DTest
{
    [TestFixture]
    public class UnitTTest
    {
        public string testHost = "localhost";
        public string testMode = "IDE";
        bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
        //string fileLoc = @"D:\Backup\GDIO\Tests\unitytestcases\2DTest\2DTest\bin\Debug\results\resources\";

        [OneTimeSetUp]
        public void Connect()
        {
            bool connected;

            if (testMode != "standalone")
            {
                //Start Unity Editor Play Mode 
                connected = Api.WaitForGame(testHost);
            }
            else if (isWindows == true)
            {
                //Launch the Windows executable
                connected = Api.Launch(@"C:\Users\neoev\Desktop\2DDemo\2DDemo.exe");
            }
            //Launch the macOS executable
            else connected = Api.Launch(@"~/Desktop/2DDemo.app/Contents/MacOS/2DDemo");

            if (connected)
            {
                Api.EnableKeybordHooks();
                Api.EnableMouseHooks();
            }

            //Set the timescale: Wartning, will break non-deterministic actions!
            Api.SetTimescale(1);

            //Capture a screenshot of the start menu
            Api.CaptureScreen("LoadingScreen");

            //Start the Game
            Api.WaitForObject("//*[@name='StartButton']");
            Api.ClickObject("//*[@name='StartButton']", Api.MouseButtons.LEFT);
        }

        [Test]
        public void Zone1()
        {
            Api.Wait(4000);
            Api.WaitForObject("//*[@name='InfoPost'][1]");

            string infoPostNum = "//*[@name='InfoPost'][1]";
            /*
            if (testMode == "IDE")
            {
                infoPostNum = "//*[@name='InfoPost']";
            }
            else
            {
                infoPostNum = "//*[@name='InfoPost'][1]";
            }
            */

            //FPS can be a good way to calculate how many seconds we want to perform an action,
            //which are measured in number of frames, but not how long to wait in between actions which is measured in milliseconds.
            var fps = Api.GetLastFPS();

            //Find the target infopost
            var infoPost = Api.GetObjectPosition(infoPostNum).ToVector3().x;
            Console.WriteLine("InfoPost x:" + infoPost);

            //While we're in zone1, keep trying to find the InfoPost
            while (Api.GetActiveSceneName() == "Zone1")
            {
                //Update our FPS each iteration
                fps = Api.GetLastFPS();
                Console.WriteLine("FPS: " + fps);

                //If we're near the post, down-jump
                if ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x >= (int)infoPost - 1 && (int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x <= (int)infoPost + 1)
                {
                    Console.WriteLine("InfoPost found!");
                    Api.PressKey(KeyCode.S, (ulong)Api.GetLastFPS());
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2);
                    Api.CaptureScreen("Zone1");
                }

                //If we're on the left of the gap, find a way over
                while ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x < 10 && (int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y < -1)
                {
                    //Jump the chasm. This is tricky, so it might take a few tries
                    Console.WriteLine("We're going to jump!");
                    Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS() * 2); //Move right for ~2 seconds
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2); //Jump for a 1/2 second
                    Api.Wait(500);
                }

                //If we fall in the hole, need to back up and try again
                while ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x >= 10 && (int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y <= -3)
                {
                    Console.WriteLine("We're stuck in the hole, please stand by!");
                    Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS()); //Move left for 1 second
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 4); //Jump for a 1/4 second
                    Api.Wait(250);
                }

                //Once we have crosses the gap, move right
                while ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x >= 13 && (int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x < infoPost && (int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y > -2)
                {
                    Console.WriteLine("x:" + Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x + " < " + infoPost + ". Moving Right.");
                    Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS() / 3);
                    Api.Wait(333);
                }

                //If Ellen is to the right of the InfoPost, move left
                while (Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x > infoPost)
                {
                    Console.WriteLine("x:" + Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x + " > " + infoPost + ". Moving Left.");
                    Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS() / 3);
                    Api.Wait(333);
                }
            }

            //If the jump-down was successful, we should be in Zone2
            var levelCheck = Api.GetActiveSceneName();
            if (levelCheck == "Zone2")
            {
                Api.Checkpoint(Api.CheckpointStatus.PASS, "Zone 1 Complete!", true);
            }
            else Api.Checkpoint(Api.CheckpointStatus.FAIL, "Zone 1 Failed!", true);

            // Assert.AreEqual(levelCheck, "Zone2");
        }

        [Test]
        public void Zone2()
        {
            //Test Pause
            Api.PauseGame("localhost");
            Api.Wait(2000);
            Api.PauseGame("localhost");

            Api.WaitForObject("/*[@name='Key']");
            Api.Wait(4000);

            var fps = Api.GetLastFPS();
            Console.WriteLine("FPS: " + fps);

            //The goal in zone 2 is the first key
            var keyVector = Api.GetObjectPosition("/*[@name='Key']").ToVector3();
            var ellen = Api.GetObjectPosition("//*[@name='Ellen']").ToVector3();
            Console.WriteLine("Ellen coordinates:" + ellen.x + "," + ellen.y);
            Console.WriteLine("Key coordinates:" + keyVector.x + "," + keyVector.y);

            while (Api.GetObjectFieldValue("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)']/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color").Equals("RGBA(1.000, 1.000, 1.000, 0.000)"))
            {
                //Check if we're on either side of the key, shaving float precision
                while (((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x != (int)keyVector.x) & ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y != (int)keyVector.y))
                {
                    //If we've hit the key, break
                    if (Api.GetObjectFieldValue("/*[@name='KeyCanvas']/*[@name='KeyIcon(Clone)']/*[@name='Key']/fn:component('UnityEngine.UI.Image')/@color").Equals("RGBA(1.000, 1.000, 1.000, 1.000)"))
                    {
                        Console.WriteLine("Key Get!");
                        break;
                    }
                    else
                    {
                        //If we're to the left, move right
                        while ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x < (int)keyVector.x)
                        {
                            Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS());
                            Api.Wait(500);

                            // While moving right, if we're lower than the key, jump
                            if (Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y < keyVector.y)
                            {
                                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 3);
                            }
                        }

                        //If we're to the right, move left
                        while ((int)Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().x > (int)keyVector.x)
                        {
                            Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS());
                            Api.Wait(500);

                            // While moving left, if we're lower than the key, jump
                            if (Api.GetObjectPosition("//*[@name='Ellen']").ToVector3().y < keyVector.y)
                            {
                                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 3);
                            }
                        }
                    }
                }
            }
            //Move to the next level entrance
            while (Api.GetActiveSceneName() != "Zone3")
            {
                Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS());
                Api.Wait(500);
                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2);
            }

            Api.Wait(10000);
            //If the jump-down was successful, we should be in Zone3
            var levelCheck = Api.GetActiveSceneName();
            Assert.AreEqual(levelCheck, "Zone3");

        }

        [OneTimeTearDown]
        public void Disconnect()
        {
            Api.DisableKeybordHooks();
            Api.Wait(3000);
            Api.DisableMouseHooks();
            Api.Wait(3000);
            Api.Quit();
            Api.Wait(3000);
        }
    }
}
