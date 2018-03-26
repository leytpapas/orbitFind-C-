using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using cef;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;

namespace OrbitFind
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            workerThread.DoWork += workerThread_DoWork;
            workerThread.ProgressChanged += workerThread_ProgressChanged;
            workerThread.RunWorkerCompleted += workerThread_Completed;
            workerThread.WorkerReportsProgress = true;
            workerThread.WorkerSupportsCancellation = true;
        }
        private void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            List<int> commands = e.Argument as List<int>;
            Console.WriteLine("Sleeping for " + commands.ElementAt(1) + "ms " + commands.ElementAt(0) + " times");
            for (int i = 0; i < commands.ElementAt(0); i++)
            {
                if (workerThread.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    System.Threading.Thread.Sleep(commands.ElementAt(1));
                    workerThread.ReportProgress(i * 100 / commands.ElementAt(0));
                }
            }
        }

        private void workerThread_Completed(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled == true)
            {
                Console.WriteLine("Canceled!");
            }
            else if (e.Error != null)
            {
                Console.WriteLine("Error: " + e.Error.Message);
            }
            else {
                Console.WriteLine("Done");
            }

            var task2 = browser.EvaluateScriptAsync("(function(){return loadOut(1);})();");
            task2.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&loadOut&: " + (response.Success ? (response.Result ?? "null") : response.Message));
                    Console.WriteLine(response.Result);
                    Console.WriteLine(response.Message);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Wait(1000);
            clear_b.Enabled = true;
            compute_b.Enabled = true;
            cancel_b.Enabled = false;
        }

        private void workerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("Progress: " + e.ProgressPercentage + "%");
        }
        public ChromiumWebBrowser browser;
        private CallbackObjectForJs _callBackObjectForJs;

        private Boolean showConfirmBox() {
            var result = MessageBox.Show("Are you sure to clear all markers?", "Clear Markers", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return (result == DialogResult.Yes) ? true : false;
        }

        private void clear_b_Click(object sender, EventArgs e)
        {
            if (!showConfirmBox()) {
                return;
            }
            var task = browser.EvaluateScriptAsync("clearMarkers();");
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&&" + (response.Success ? (response.Result ?? "null") : response.Message));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void paths_b_Click(object sender, EventArgs e) {
            var task = browser.EvaluateScriptAsync("polygonPath();");
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    var temp = (response.Success ? (response.Result ?? "null") : response.Message);
                    Console.WriteLine("&&" + temp);
                    var test = new Polygon(temp.ToString());
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            browser = new ChromiumWebBrowser("file:///"+"HTMLResources/html/map.html")
            {
                Dock = DockStyle.Fill,
                
            };
            panel_map.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

            _callBackObjectForJs = new CallbackObjectForJs();
            browser.RegisterAsyncJsObject("callbackObj", _callBackObjectForJs);
        }

        private void compute_b_Click(object sender, EventArgs e)
        {
            clear_b.Enabled = false;
            compute_b.Enabled = false;
            cancel_b.Enabled = true;
            var task = browser.EvaluateScriptAsync("loadIn();");
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&loadInt&: "+( response.Success ? (response.Result ?? "null") : response.Message));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            workerThread.RunWorkerAsync(new List<int>(new int[] { 100, 500}));
            /*var task2 = browser.EvaluateScriptAsync("loadOut();");
            task2.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&2&" + (response.Success ? (response.Result ?? "null") : response.Message));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            clear_b.Enabled = true;
            compute_b.Enabled = true;
            cancel_b.Enabled = false;*/
        }

        private void kmlAddr_t_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                loadKml_b_Click(sender, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }
        private void loadKml_b_Click(object sender, EventArgs e)
        {
            browser.Load(kmlAddr_t.Text);
            Console.WriteLine("Loading " + kmlAddr_t.Text);
        }

        private void viewSource_Click(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void addressUrl_t_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loadAddr_b_Click(sender, new EventArgs());

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void loadAddr_b_Click(object sender, EventArgs e)
        {
            browser.Load(addressUrl_t.Text);
            Console.WriteLine("Loading " + addressUrl_t.Text);
        }

        private void cancel_b_Click(object sender, EventArgs e)
        {
            if (workerThread.IsBusy)
            {
                workerThread.CancelAsync();
                clear_b.Enabled = true;
                compute_b.Enabled = true;
                cancel_b.Enabled = false;
            }
        }
    }
    public class CallbackObjectForJs
    {
        public void showMessage(string msg)
        {//Read Note
            Console.WriteLine(msg);
        }
    }
}
