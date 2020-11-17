using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Timers;
using Google.Cloud.Translation.V2;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Translate_For_Twitch
{
    class Program
    {
        static void Main()
        {
            Bot bot = new Bot();
            do
            {

            }
            while (1 == 1);
        }
    }

    partial class Bot
    {
        // Configuration options begin
        public static string Username = string.Empty;   //optional
        public static string OAuth = string.Empty;      //optional
        public static string TargetChat = string.Empty;
        public static string InputLanguage = string.Empty;
        public static string OutputLanguage = string.Empty;
        public static double BitValue = 0.5;
        public static int SubValue = 125;

        private static void Relocate(IntPtr handle, SetWindowPosFlags flags)
        {
            SetWindowPos(handle, IntPtr.Zero, 1580, 72, 340, 960, flags);
        }
        // Configuration options end

        public static TwitchClient client;
        private static Timer duration;
        private static List<double> muteDurations = new List<double>();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        public Bot()
        {
            IntPtr handle = GetConsoleWindow();
            Relocate(handle, SetWindowPosFlags.SWP_SHOWWINDOW);
            if (Username == string.Empty)
            {
                Console.Write("Enter your Twitch username: ");
                Username = Console.ReadLine();
            }
            if (OAuth == string.Empty)
            {
                Console.Write("Enter your OAuth token: ");
                OAuth = Console.ReadLine();
            }
            duration = new Timer(100);
            duration.AutoReset = false;
            duration.Enabled = true;
            ConnectionCredentials credentials = new ConnectionCredentials(Username, OAuth);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, TargetChat);

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnGiftedSubscription += Client_OnGiftedSubscription;
            client.OnReSubscriber += Client_OnReSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (duration.Enabled)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{e.ChatMessage.Username}: ");
                Console.ResetColor();
                Console.WriteLine(e.ChatMessage.Message);
                if (e.ChatMessage.Bits > 0)
                {
                    Console.WriteLine($"Translating for {e.ChatMessage.Bits * BitValue} seconds.");
                    muteDurations.Add(e.ChatMessage.Bits * BitValue * 1000);
                }
            }
            else if (e.ChatMessage.Bits > 0)
            {
                MoveWindow();
                Console.WriteLine($"Translating for {e.ChatMessage.Bits * BitValue} seconds.");
                SetTimer(e.ChatMessage.Bits * BitValue * 1000);
            }
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            Console.WriteLine($"Subscription received, translating for {SubValue} seconds.");
            if (duration.Enabled)
                muteDurations.Add(SubValue * 1000);
            else
                SetTimer(SubValue * 1000);
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            Console.WriteLine($"Subscription received, translating for {SubValue} seconds.");
            if (duration.Enabled)
                muteDurations.Add(SubValue * 1000);
            else
                SetTimer(SubValue * 1000);
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            Console.WriteLine($"Subscription received, translating for {SubValue} seconds.");
            if (duration.Enabled)
                muteDurations.Add(SubValue * 1000);
            else
                SetTimer(SubValue * 1000);
        }

        private static void MoveWindow()
        {
            IntPtr handle = GetConsoleWindow();
            Relocate(handle, SetWindowPosFlags.SWP_SHOWWINDOW);
            SetForegroundWindow(handle);
            Console.Clear();
        }

        private async void SetTimer(double time)
        {
            duration = new Timer(time);
            duration.Elapsed += Unmute;
            duration.AutoReset = false;
            duration.Enabled = true;
            await Task.Run(() =>
            {
                do
                {
                    TranslationClient t = TranslationClient.Create();
                    var response = t.TranslateText(
                        text: Console.ReadLine(),
                        targetLanguage: OutputLanguage,
                        sourceLanguage: InputLanguage);
                    client.SendMessage(TargetChat, response.TranslatedText);
                }
                while (duration.Enabled);
            });
        }

        private void Unmute(object sender, ElapsedEventArgs e)
        {
            if (muteDurations.Count > 0)
            {
                double redoTime = muteDurations[0];
                muteDurations.RemoveAt(0);
                SetTimer(redoTime);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Mute duration expired.");
                IntPtr handle = GetConsoleWindow();
                Relocate(handle, SetWindowPosFlags.SWP_HIDEWINDOW);
            }
        }
    }
}
