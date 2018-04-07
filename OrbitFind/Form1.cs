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
using System.Xml;
using System.Xml.Linq;
using cef;
using CefSharp;
using CefSharp.WinForms;
using CefSharp.WinForms.Internals;
using System.Text.RegularExpressions;
using OrbitFind;
using Newtonsoft.Json;


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

            var task2 = browser.EvaluateScriptAsync("(function(){return loadOut();})();");
            task2.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&loadOut&: " + (response.Success ? (response.Result ?? "null") : response.Message));
                    Console.WriteLine(response.Result);
                    Console.WriteLine(response.Message);
                    clear_b.Enabled = true;
                    compute_b.Enabled = true;
                    loadKml_b.Enabled = true;
                    kmlAddr_t.Enabled = true;
                    cancel_b.Enabled = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Wait(1);
            
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
                    var test = new Polygon(temp.ToString(),"Polygon");
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            CefSettings settings = new CefSettings();
            Cef.Initialize(settings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            browser = new ChromiumWebBrowser("file:///"+"Resources/html/map.html")
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
            loadKml_b.Enabled = false;
            kmlAddr_t.Enabled = false;
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
            workerThread.RunWorkerAsync(new List<int>(new int[] { 20, 500}));
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
            if (e.KeyCode == Keys.Enter & compute_b.Enabled) {
                loadKml_b_Click(sender, new EventArgs());
                e.Handled = true;
                e.SuppressKeyPress = true;
            }

        }

        private void loadKml_b_Click(object sender, EventArgs e)
        {
            string geojson = parseKML(this, kmlAddr_t.Text);
            if (geojson=="") {
                return;
            }
            var task = browser.EvaluateScriptAsync("loadGeoJson("+geojson+");");
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("&loadInt&: " + (response.Success ? (response.Result ?? "null") : response.Message));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Wait(1);
            //browser.Load(kmlAddr_t.Text);
            Console.WriteLine("Loading " + kmlAddr_t.Text);
        }
        
        private string parseKML(object sender, string filepath) {
            try
            {
                var xDoc = XDocument.Load(filepath);
                XElement root = xDoc.Root;
                Console.WriteLine(root.Name.Namespace);
                XNamespace ns = root.Name.Namespace;
                //var temp = root.Descendants(ns + "Placemark");
                var geojson = new GeoJson();
                var pass = 0;
                var polytest = new List<Tuple<double, double>>();
                root = root.Element(ns+"Document").Element(ns + "Folder");
                Console.WriteLine(root.Element(ns+"name").Value);
                foreach (XElement f in root.Elements(ns + "Folder")){
                    pass += 1;

                    foreach (XElement p in f.Elements(ns + "Placemark")) {
                        if (p.Element(ns + "styleUrl").Value.EndsWith("GroundTrack"))
                        {
                            Polygon polyline = new Polygon(p.Element(ns + "LineString").Element(ns + "coordinates").Value, "Linestring");
                            geojson.addPoly(polyline);
                        }
                        else {
                            //swath
                            var temp = "";
                            foreach (XElement m in p.Element(ns+"MultiGeometry").Elements(ns+"LineString")) {
                                if (temp == ""){
                                    //left
                                    temp = m.Element(ns + "coordinates").Value;
                                }else {
                                    //right
                                    Polygon polyline = new Polygon(temp, m.Element(ns + "coordinates").Value, "swath");
                                }
                            }
                        }
                    };
                }
                
                return geojson.toString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";

        }

        private void viewSource_Click(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void cancel_b_Click(object sender, EventArgs e)
        {
            if (workerThread.IsBusy)
            {
                workerThread.CancelAsync();
                clear_b.Enabled = true;
                compute_b.Enabled = true;
                loadKml_b.Enabled = true;
                kmlAddr_t.Enabled = true;
                cancel_b.Enabled = false;
            }
        }

        private void clearKml_b_Click(object sender, EventArgs e)
        {
            var task = browser.EvaluateScriptAsync("clearKml();");
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    Console.WriteLine("clearKml(): " + (response.Success ? (response.Result ?? "null") : response.Message));
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()).Wait(1);
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
