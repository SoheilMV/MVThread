using MVThread;
using MVThread.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Example
{
    public partial class Form1 : Form
    {
        IRunner _runner;
        List<string> list = new List<string>();
        int i;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _runner = new Runner();
            _runner.OnStarted += Runner_OnStarted;
            _runner.OnStopped += Runner_OnStopped;
            _runner.OnCompleted += Runner_OnCompeleted;
            _runner.OnConfig += Runner_OnConfig;
            _runner.OnException += Runner_OnException;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Reset();

            for (int i = 0; i < 500; i++) //adds 0-499 to the list
            {
                list.Add(i.ToString());
            }

            pbChecked.Maximum = list.Count; //the maximum amount of progress bar is equal to the number of lists

            _runner.SetWordlist(list); //add list to runner
            _runner.Start((int)numBot.Value); //add bot count in runner and start the runner
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _runner.Stop(); //stop the runner
        }

        private void Runner_OnStarted(object sender, EventArgs e)
        {
            listData.Items.Add("Started!"); //displays the start message when the runner start
        }

        private void Runner_OnStopped(object sender, StopEventArgs e)
        {
            listData.Items.Add("Stoped!"); //displays the stop message when the runner stop
        }

        private void Runner_OnCompeleted(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                listData.Items.Add("Completed!"); //show end message
            }));
        }

        private void Runner_OnConfig(object sender, DataEventArgs e)
        {
            i++;
            Invoke(new MethodInvoker(delegate ()
            {
                listData.Items.Add(e.Data); //add numbers to listbox
                pbChecked.Value = i; //set progress bar value
                lblPercentage.Text = $"{(int)Math.Round((double)(i * 100) / (double)list.Count)}%"; //show progress percentage
            }));
        }

        private void Runner_OnException(object sender, ExceptionEventArgs e)
        {
            e.Log.WriteLine($"{e.Location}: {e.Exception.Message}"); //any error in events will show you
        }

        private void Reset()
        {
            listData.Items.Clear();
            list.Clear();
            i = 0;
        }
    }
}