using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
            _runner = new TaskRunner();
            _runner.OnStarted += Runner_OnStarted;
            _runner.OnStopped += Runner_OnStopped;
            _runner.OnCompleted += Runner_OnCompeleted;
            _runner.OnConfig += Runner_OnConfig;
            _runner.OnException += Runner_OnException;
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
            this.Text = $"Test2 - CPM : {_runner.CPM}";
            lblChecked.Text = $"Checked : {_runner.Progress.Value}/{_runner.Progress.MaxValue}";
            pbChecked.Value = _runner.Progress.Value; //set progress bar value
            lblPercentage.Text = $"{_runner.Progress.Percentage}%"; //show progress percentage
        }

        private void Runner_OnStarted(object sender, EventArgs e)
        {
            AddToListData("Started!"); //displays the start message when the runner start
        }

        private void Runner_OnStopped(object sender, StopEventArgs e)
        {
            AddToListData("Stoped!"); //displays the stop message when the runner stop
        }

        private void Runner_OnCompeleted(object sender, EventArgs e)
        {
            AddToListData("Completed!"); //displays the completed message when the runner complete
        }

        private Status Runner_OnConfig(object sender, DataEventArgs e)
        {
            AddToListData(e.Data); //add data to listbox

            e.Save.WriteLine("Data.txt", e.Data); //save data

            return Status.OK;
        }

        private void Runner_OnException(object sender, ExceptionEventArgs e)
        {
            e.Log.WriteLine($"{e.Location}: {e.Exception.Message}"); //any error in events will show you
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
