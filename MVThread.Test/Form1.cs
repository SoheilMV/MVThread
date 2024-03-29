﻿using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVThread.Test
{
    public partial class Form1 : Form
    {
        IRunner _runner;
        List<string> _userlist = new List<string>();
        List<string> _passlist = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _runner = RunnerFactory.Create(RunnerType.Parallel, onConfigAsync, onStarted, onStopped, onCompeleted, onException);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Reset();

            for (int i = 1; i <= 100; i++) //adds 1-100 to the userlist
            {
                _userlist.Add(i.ToString());
            }

            for (int i = 1; i <= 10; i++) //adds 1-10 to the passlist
            {
                _passlist.Add(i.ToString());
            }

            pbChecked.Maximum = _userlist.Count * _passlist.Count; //the maximum amount of progress bar is equal to the number of lists
            _runner.SetWordlist(_userlist, _passlist, ComboType.ChangePass); //add list to runner
            _runner.Start((int)numBot.Value); //add bot count in runner and start the runner
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _runner.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.Text = $"Test - Active : {_runner.Active} - CPM : {_runner.CPM}";
            lblChecked.Text = $"Checked : {_runner.Progress.Value}/{_runner.Progress.MaxValue}";
            pbChecked.Value = _runner.Progress.Value; //set progress bar value
            lblPercentage.Text = $"{_runner.Progress.Percentage}%"; //show progress percentage
        }

        private void onStarted(StartEventArgs e)
        {
            listData.Items.Add($"Runner => {_runner.RunnerType.ToString()}");
            listData.Items.Add("Started!"); //displays the start message when the runner start
        }

        private void onStopped(StopEventArgs e)
        {
            listData.Items.Add("Stoped!"); //displays the stop message when the runner stop
        }

        private void onCompeleted()
        {
            listData.Items.Add("Completed!"); //displays the completed message when the runner complete
        }

        private async Task<ConfigStatus> onConfigAsync(DataEventArgs e)
        {
            if (!e.Parameters.ContainsKey("data"))
            {
                e.Parameters["data"] = e.Data;
                return ConfigStatus.Retry;
            }

            AddToListData(e.Parameters["data"]!.ToString()!); //add data to listbox
            e.Save.WriteLine("Data.txt", e.Parameters["data"]!.ToString()!); //save data

            return ConfigStatus.OK;
        }

        private void onException(ExceptionEventArgs e)
        {
            e.Log.Write($"{e.Location}: {e.Exception.Message}"); //any error in events will show you
        }

        private void AddToListData(string item)
        {
            Invoke(new MethodInvoker(delegate()
            {
                listData.Items.Add(item);
            }));
        }

        private void Reset()
        {
            listData.Items.Clear();
            _userlist.Clear();
            _passlist.Clear();
        }
    }
}
