using System;
using NUnit.Framework;
using gdio.unity_api;

namespace GDIO_2DTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Launch()
        {
            Api.WaitForGame("127.0.0.1");
            //Api.Launch(@"C:\Users\neoev\Desktop\2D_Demo\2D Demo.exe");
            Api.Wait(5000);
            Api.EnableKeybordHooks();
            Api.EnableMouseHooks();


            //Capture a screenshot of the start menu
            Api.CaptureScreen("LoadingScreen.jpg");

            //Start the Game
            Api.WaitForObject("//StartButton");
            Api.ClickObject("//StartButton", Api.MouseButtons.LEFT);
        }

        [Test]
        public void Zone1()
        {
            Api.WaitForObject("//InfoPost[2]");
            Api.Wait(4000);

            //FPS is a good way to calculate how many seconds we want to perform an action,
            //which are measured in number of frames, but not how long to wait in between actions which is measured in milliseconds.
            var fps = Api.GetLastFPS();

            //Find the target infopost
            var infoPost = Api.GetObjectPosition("//InfoPost[2]").ToVector3().x;
            Console.WriteLine("InfoPost x:" + infoPost);

            //While we're in zone1, keep trying to find the InfoPost
            while (Api.GetActiveSceneName() == "Zone1")
            {
                //Update our FPS each iteration
                fps = Api.GetLastFPS();
                Console.WriteLine("FPS: " + fps);

                //If we're near the post, down-jump
                if ((int)Api.GetObjectPosition("//Ellen").ToVector3().x >= (int)infoPost - 1 && (int)Api.GetObjectPosition("//Ellen").ToVector3().x <= (int)infoPost + 1)
                {
                    Console.WriteLine("InfoPost found!");
                    Api.PressKey(KeyCode.S, (ulong)Api.GetLastFPS());
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2);
                    Api.CaptureScreen("Zone1.jpg");
                }

                //If we're on the left of the gap, find a way over
                while ((int)Api.GetObjectPosition("//Ellen").ToVector3().x < 10 && (int)Api.GetObjectPosition("//Ellen").ToVector3().y < -1)
                {
                    //Jump the chasm. This is tricky, so it might take a few tries
                    Console.WriteLine("We're going to jump!");
                    Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS() * 2); //Move right for ~2 seconds
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2); //Jump for a 1/2 second
                    Api.Wait(500);
                }

                //If we fall in the hole, need to back up and try again
                while ((int)Api.GetObjectPosition("//Ellen").ToVector3().x >= 10 && (int)Api.GetObjectPosition("//Ellen").ToVector3().y <= -3)
                {
                    Console.WriteLine("We're stuck in the hole, please stand by!");
                    Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS()); //Move left for 1 second
                    Api.Wait(500);
                    Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 4); //Jump for a 1/4 second
                    Api.Wait(250);
                }

                //Once we have crosses the gap, move right
                while ((int)Api.GetObjectPosition("//Ellen").ToVector3().x > 13 && (int)Api.GetObjectPosition("//Ellen").ToVector3().x < infoPost && (int)Api.GetObjectPosition("//Ellen").ToVector3().y > -2)
                {
                    Console.WriteLine("x:" + Api.GetObjectPosition("//Ellen").ToVector3().x + " < " + infoPost + ". Moving Right.");
                    Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS() / 3);
                    Api.Wait(333);
                }

                //If Ellen is to the right of the InfoPost, move left
                while (Api.GetObjectPosition("//Ellen").ToVector3().x > infoPost)
                {
                    Console.WriteLine("x:" + Api.GetObjectPosition("//Ellen").ToVector3().x + " > " + infoPost + ". Moving Left.");
                    Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS() / 3);
                    Api.Wait(333);
                }
            }

            //If the jump-down was successful, we should be in Zone2
            var levelCheck = Api.GetActiveSceneName();
            Assert.AreEqual(levelCheck, "Zone2");
        }

        [Test]
        public void Zone2()
        {
            Api.WaitForObject("/Key");
            Api.Wait(4000);

            var fps = Api.GetLastFPS();
            Console.WriteLine("FPS: " + fps);

            //The goal in zone 2 is the first key
            var keyVector = Api.GetObjectPosition("/Key").ToVector3();
            var ellen = Api.GetObjectPosition("//Ellen").ToVector3();
            Console.WriteLine("Ellen coordinates:" + ellen.x + "," + ellen.y);
            Console.WriteLine("Key coordinates:" + keyVector.x + "," + keyVector.y);

            while (Api.GetObjectFieldValue("(//*[@name='KeyIcon(Clone)'])[0]/Key[fn:component('UnityEngine.UI.Image')]/@color") == "RGBA(1.000, 1.000, 1.000, 0.000)")
            {
                //Check if we're on either side of the key, shaving float precision
                while (((int)Api.GetObjectPosition("//Ellen").ToVector3().x != (int)keyVector.x) & ((int)Api.GetObjectPosition("//Ellen").ToVector3().y != (int)keyVector.y))
                {

                    //If we've hit the key, break
                    if (Api.GetObjectFieldValue("(//*[@name='KeyIcon(Clone)'])[0]/Key[fn:component('UnityEngine.UI.Image')]/@color") == "RGBA(1.000, 1.000, 1.000, 1.000)")
                    {
                        Console.WriteLine("Key Get!");
                        break;
                    }
                    else
                    {
                        //If we're to the left, move right
                        while ((int)Api.GetObjectPosition("//Ellen").ToVector3().x < (int)keyVector.x)
                        {
                            Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS());
                            Api.Wait(500);

                            // While moving right, if we're lower than the key, jump
                            if (Api.GetObjectPosition("//Ellen").ToVector3().y < keyVector.y)
                            {
                                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 3);
                            }
                        }

                        //If we're to the right, move left
                        while ((int)Api.GetObjectPosition("//Ellen").ToVector3().x > (int)keyVector.x)
                        {
                            Api.PressKey(KeyCode.A, (ulong)Api.GetLastFPS());
                            Api.Wait(500);

                            // While moving left, if we're lower than the key, jump
                            if (Api.GetObjectPosition("//Ellen").ToVector3().y < keyVector.y)
                            {
                                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 3);
                            }
                        }
                    }
                }

                // Debug, need to know the Key colors
                Console.WriteLine(Api.GetObjectFieldValue("(//*[@name='KeyIcon(Clone)'])[0]/Key[fn:component('UnityEngine.UI.Image')]/@color"));
                Console.WriteLine(Api.GetObjectFieldValue("(//*[@name='KeyIcon(Clone)'])[1]/Key[fn:component('UnityEngine.UI.Image')]/@color"));
                Console.WriteLine(Api.GetObjectFieldValue("(//*[@name='KeyIcon(Clone)'])[2]/Key[fn:component('UnityEngine.UI.Image')]/@color"));


            }
            //Move to the next level entrance
            while (Api.GetActiveSceneName() != "Zone3")
            {
                Api.PressKey(KeyCode.D, (ulong)Api.GetLastFPS());
                Api.Wait(500);
                Api.PressKey(KeyCode.Space, (ulong)Api.GetLastFPS() / 2);
            }

            //If the jump-down was successful, we should be in Zone3
            var levelCheck = Api.GetActiveSceneName();
            Assert.AreEqual(levelCheck, "Zone3");
        }


        [Test]
        public void Quit()
        {
            //Api.Quit();
            Api.Detach();
        }
    }
}
