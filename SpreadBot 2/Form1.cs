using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Skype4Sharp;
using Skype4Sharp.Auth;
using Skype4Sharp.Enums;
using Skype4Sharp.Events;
using Skype4Sharp.Exceptions;
using Skype4Sharp.Helpers;

namespace SpreadBot_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static Skype4Sharp.Skype4Sharp mainSkype;
        private void buttonBlue1_Click(object sender, EventArgs e)
        {
            SkypeCredentials authCreds = new SkypeCredentials(textBox1.Text, textBox2.Text);
            try
            {
                mainSkype = new Skype4Sharp.Skype4Sharp(authCreds);
                mainSkype.Login();
                mainSkype.StartPoll();
                MessageBox.Show("Logged in!");
            } 
            catch
            {
                MessageBox.Show("[01] Failed to login."
                    + Environment.NewLine + "Make sure that you typed" 
                    + Environment.NewLine + "correctly name / password."
                    + Environment.NewLine
                    + Environment.NewLine + "Troubles? 'federicoretro' on Skype.");
            }
        }

        private static void MainSkype_contactRequestReceived(ContactRequest sentRequest, ChatMessage pMessage)
        {
            Thread.Sleep(10000);
            sentRequest.Accept();
        }

        public static void CreateEmptyFile(string filename)
        {
            File.Create(filename).Dispose();
        }


        private static void MainSkype_messageReceived(ChatMessage pMessage, Chat targetChat)
        {
            new Thread(() =>
            {
                try
                {
                    pMessage.Chat.Topic = "hey contact me babe! ;)";
                }
                catch
                {
                    string Log = "data/Log.txt";
                    string ChatEnded = "SPREADBOT <Chat ended, maybe implement Template2?>";
                    string[] Template = File.ReadAllLines("data/Template.txt");
                    string User = "user/" + pMessage.Sender.Username;
                    string UserLine = File.ReadAllText(User);
                    if (File.Exists(User))
                    {
                        foreach (string item in Template)
                        {

                            if (UserLine.Contains(item))
                            {
                                try
                                {
                                    Thread.Sleep(5000);
                                    pMessage.Chat.SendMessage(User);
                                    File.AppendAllText(Log, Environment.NewLine + "SpreadBot <TEMPLATELOG> - Sent \"" + User + "\" to '" + pMessage.Sender.Username + "' .");
                                    File.WriteAllText(User, item + 1);
                                }
                                catch
                                {
                                    File.AppendAllText(Log, Environment.NewLine + "SpreadBot <TEMPLATELOG> - Got the end of the chat, nothing to say.");
                                    File.WriteAllText(User, ChatEnded);
                                    break;
                                }
                            }
                            else if (UserLine.Contains(ChatEnded))
                            {
                                File.AppendAllText(Log, Environment.NewLine + "SpreadBot <TEMPLATELOG> - Chat ended, nothing to say.");
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        CreateEmptyFile(User);
                        File.AppendAllText(Log, Environment.NewLine + "SpreadBot <TEMPLATELOG> - Started chat with '" + pMessage.Sender.Username + "' .");
                        File.AppendAllText(User, Template[0]);
                        pMessage.Chat.SendMessage(User);
                    }
                }
            }).Start();
        }
    }
}
