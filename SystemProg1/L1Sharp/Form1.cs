using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace L1Sharp
{
    public partial class Form1 : Form
    {
        Process ChildProcess = null;
        EventWaitHandle stopEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "stopEvent");
        EventWaitHandle startEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "startEvent");
        EventWaitHandle confirmEvent = new EventWaitHandle(false, EventResetMode.ManualReset, "confirmEvent");
        EventWaitHandle quitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "quitEvent");
        int k;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (ChildProcess == null || ChildProcess.HasExited)
            {
                ChildProcess = Process.Start("L1\\L1.exe");
                ThreadList.Items.Clear();
                ThreadList.Items.Add("All threads");
                ThreadList.Items.Add("Main thread");
                threadCountField.Enabled = true;
                k = 0;
            }
            else
            {
                for (var i = 0; i < threadCountField.Value; i++)
                {
                    startEvent.Set();
                    confirmEvent.WaitOne();
                    ThreadList.Items.Add(String.Format("Thread {0}", k++));
                    confirmEvent.Reset();
                }
            }

        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (ChildProcess == null || ChildProcess.HasExited)
            {
            }
            else
            {
                if (k == 0)
                {
                    
                    quitEvent.Set();
                    confirmEvent.WaitOne();
                    ThreadList.Items.RemoveAt(ThreadList.Items.Count - 1);
                    ThreadList.Items.RemoveAt(ThreadList.Items.Count - 1);
                    threadCountField.Enabled = false;
                }
                if (ThreadList.Items.Count != 0) {
                    stopEvent.Set();
                    confirmEvent.WaitOne();
                    ThreadList.Items.RemoveAt(ThreadList.Items.Count - 1);
                    k--;
                    confirmEvent.Reset();
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ChildProcess != null && !ChildProcess.HasExited)
            {
                ChildProcess.CloseMainWindow();
                ChildProcess.Close();
            }
        }
    }
}
