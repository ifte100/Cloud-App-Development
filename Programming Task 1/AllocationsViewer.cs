using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PT1;
using WcfsServiceLibrary;
using System.Threading;
using System.ServiceModel;

namespace AllocationsApplication
{
    partial class AllocationsViewerForm : Form
    {
        #region properties
        private Allocations PT1Allocations;
        private Configuration PT1Configuration;
        private ErrorsViewer ErrorListViewer = new ErrorsViewer();
        private AboutBox AboutBox = new AboutBox();
        #endregion

        #region constructors
        public AllocationsViewerForm()
        {
            InitializeComponent();

            this.Text += String.Format(" ({0})", Application.ProductVersion);
        }
        //Autoresetevent for waiting
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        //Collection of results
        List<AllocationsData> results;
        //Counters for completed and timed out WCFS operations
        int completedOperations;
        int timedOutOperations;
        int numberOfOperations;
        //Readonly object for locking
        readonly object aLock = new object();
        #endregion

        #region File menu event handlers
        private void OpenAllocationsFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearGUI();

            // Process allocations and configuration files.
            if (comboBox1.Text != null && comboBox1.Text.Length > 0)
            {
                //getting filename from dropdownlist
                String configurationFileName = comboBox1.Text.Trim();
                // Parse configuration file.
                using (WebClient configWebClient = new WebClient())
                using (Stream configStream = configWebClient.OpenRead(configurationFileName))
                using (StreamReader configFile = new StreamReader(configStream))
                {
                    Configuration.TryParse(configFile, configurationFileName,
                        out PT1Configuration, out List<String> configurationErrors);
                }

                //Call Wcf service operation
                ConfigData cd = new ConfigData();

                cd.Duration = PT1Configuration.Program.Duration;
                cd.Energies = PT1Configuration.Energies;
                cd.Runtimes = PT1Configuration.Runtimes;
                cd.NumProcs = PT1Configuration.Program.Processors;
                cd.NumTasks = PT1Configuration.Program.Tasks;
                cd.ProcsRam = PT1Configuration.ProcessorRAM;
                cd.TaskRam = PT1Configuration.TaskRAM;
                cd.Path = Path.GetFileName(PT1Configuration.FilePath);
                cd.ProcsFreq = new double[cd.NumProcs];
                cd.TaskFreq = new double[cd.NumTasks];
                cd.TaskRuntime = new double[cd.NumTasks];
                cd.TaskDownloadSpeed = new int[cd.NumTasks];
                cd.TaskUploadSpeed = new int[cd.NumTasks];
                cd.ProcessorDownloadSpeed = new int[cd.NumProcs];
                cd.ProcessorUploadSpeed = new int[cd.NumProcs];
                for (int a = 0; a < PT1Configuration.Program.Tasks; a++)
                {
                    cd.TaskRuntime[a] = PT1Configuration.Tasks[a].Runtime;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcsFreq[i] = PT1Configuration.Processors[i].Frequency;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskFreq[i] = PT1Configuration.Tasks[i].Frequency;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskDownloadSpeed[i] = PT1Configuration.Tasks[i].DownloadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskUploadSpeed[i] = PT1Configuration.Tasks[i].UploadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcessorDownloadSpeed[i] = PT1Configuration.Processors[i].DownloadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcessorUploadSpeed[i] = PT1Configuration.Processors[i].UploadSpeed;
                }

                Greedy.ServiceClient ws = new Greedy.ServiceClient();
                AllocationsData bestAllocation = new AllocationsData();
                bestAllocation = ws.Greedy(300000, cd);

                Allocations.TryParse(bestAllocation.Description, PT1Configuration, out PT1Allocations, out List<string> errors);
                PT1Allocations.Validate();

                // Refesh GUI and Log errors.
                UpdateGUI();
                PT1Allocations.LogFileErrors(PT1Allocations.FileErrorsTXT);
                PT1Allocations.LogFileErrors(PT1Configuration.FileErrorsTXT);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region  Clear and Update GUI
        private void ClearGUI()
        {
            // As we are opening a Configuration file,
            // indicate allocations are not valid, and clear GUI.
            allocationToolStripMenuItem.Enabled = false;

            if (allocationWebBrowser.Document != null)
                allocationWebBrowser.Document.OpenNew(true);
            allocationWebBrowser.DocumentText = String.Empty;

            if (ErrorListViewer.WebBrowser.Document != null)
                ErrorListViewer.WebBrowser.Document.OpenNew(true);
            ErrorListViewer.WebBrowser.DocumentText = String.Empty;
        }

        private void UpdateGUI()
        {
            // Update GUI:
            // - enable menu
            // - display Allocations data (whether valid or invalid)
            // - display Allocations and Configuration file errors.
            if (PT1Allocations != null && PT1Allocations.FileValid &&
                PT1Configuration != null && PT1Configuration.FileValid)
                allocationToolStripMenuItem.Enabled = true;

            if (allocationWebBrowser.Document != null)
                allocationWebBrowser.Document.OpenNew(true);
            if (ErrorListViewer.WebBrowser.Document != null)
                ErrorListViewer.WebBrowser.Document.OpenNew(true);

            if (PT1Allocations != null)
            {
                allocationWebBrowser.DocumentText = PT1Allocations.ToStringHTML();
                ErrorListViewer.WebBrowser.DocumentText =
                    PT1Allocations.FileErrorsHTML +
                    PT1Configuration.FileErrorsHTML +
                    PT1Allocations.AllocationsErrorsHTML;
            }
        }
        #endregion

        #region Validate menu event handlers
        private void AllocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check if the allocations are valid.
            PT1Allocations.Validate();

            // Update GUI - display allocations file data (whether valid or invalid), 
            // allocations file errors, config file errors, and allocation errors.
            allocationWebBrowser.DocumentText = PT1Allocations.ToStringHTML();
            ErrorListViewer.WebBrowser.DocumentText =
                PT1Allocations.FileErrorsHTML +
                PT1Configuration.FileErrorsHTML +
                PT1Allocations.AllocationsErrorsHTML;

            // Log errors.
            PT1Allocations.LogFileErrors(PT1Allocations.AllocationsErrorsTXT);
        }
        #endregion

        #region View menu event handlers
        private void ErrorListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorListViewer.WindowState = FormWindowState.Normal;
            ErrorListViewer.Show();
            ErrorListViewer.Activate();
        }
        #endregion

        #region Help menu event handlers
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox.ShowDialog();
        }
        #endregion

        #region Generate Allocations
        private void generateAllocationsButton_Click(object sender, EventArgs e)
        {
            ClearGUI();

            // Process allocations and configuration files.
            if (comboBox1.Text != null && comboBox1.Text.Length > 0)
            {
                //getting filename from dropdownlist
                String configurationFileName = comboBox1.Text.Trim();
                // Parse configuration file.
                using (WebClient configWebClient = new WebClient())
                using (Stream configStream = configWebClient.OpenRead(configurationFileName))
                using (StreamReader configFile = new StreamReader(configStream))
                {
                    Configuration.TryParse(configFile, configurationFileName,
                        out PT1Configuration, out List<String> configurationErrors);
                }

                ConfigData cd = new ConfigData();

                cd.Duration = PT1Configuration.Program.Duration;
                cd.Energies = PT1Configuration.Energies;
                cd.Runtimes = PT1Configuration.Runtimes;
                cd.NumProcs = PT1Configuration.Program.Processors;
                cd.NumTasks = PT1Configuration.Program.Tasks;
                cd.ProcsRam = PT1Configuration.ProcessorRAM;
                cd.TaskRam = PT1Configuration.TaskRAM;
                cd.Path = Path.GetFileName(PT1Configuration.FilePath);
                cd.ProcsFreq = new double[cd.NumProcs];
                cd.TaskFreq = new double[cd.NumTasks];
                cd.TaskRuntime = new double[cd.NumTasks];
                cd.TaskDownloadSpeed = new int[cd.NumTasks];
                cd.TaskUploadSpeed = new int[cd.NumTasks];
                cd.ProcessorDownloadSpeed = new int[cd.NumProcs];
                cd.ProcessorUploadSpeed = new int[cd.NumProcs];
                for (int a = 0; a < PT1Configuration.Program.Tasks; a++)
                {
                    cd.TaskRuntime[a] = PT1Configuration.Tasks[a].Runtime;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcsFreq[i] = PT1Configuration.Processors[i].Frequency;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskFreq[i] = PT1Configuration.Tasks[i].Frequency;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskDownloadSpeed[i] = PT1Configuration.Tasks[i].DownloadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Tasks; i++)
                {
                    cd.TaskUploadSpeed[i] = PT1Configuration.Tasks[i].UploadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcessorDownloadSpeed[i] = PT1Configuration.Processors[i].DownloadSpeed;
                }
                for (int i = 0; i < PT1Configuration.Program.Processors; i++)
                {
                    cd.ProcessorUploadSpeed[i] = PT1Configuration.Processors[i].UploadSpeed;
                }

                //2nd THread
                System.Threading.Tasks.Task.Run(() => AsynchronousCalls(cd));
                //GUI Thread waits
                autoResetEvent.WaitOne(300000);
                //process results

                Allocations.TryParse(results[0].Description, PT1Configuration, out PT1Allocations, out List<string> errors);
                PT1Allocations.Validate();

                // Refesh GUI and Log errors.
                UpdateGUI();
                PT1Allocations.LogFileErrors(PT1Allocations.FileErrorsTXT);
                PT1Allocations.LogFileErrors(PT1Configuration.FileErrorsTXT);

            }
        }
        private void AsynchronousCalls(ConfigData cd)
        {
            //Greedy.ServiceClient ws = new Greedy.ServiceClient();
            ////Event Handler
            //ws.GreedyCompleted += Ws_GreedyCompleted;
            GreedyAWS.ServiceClient ws = new GreedyAWS.ServiceClient();
            ws.GreedyCompleted += Ws_GreedyCompleted1;

            HeuristicAWS.ServiceClient heu = new HeuristicAWS.ServiceClient();
            heu.HeuristicCompleted += Heu_HeuristicCompleted;


            results = new List<AllocationsData>();
            numberOfOperations = 10;
            completedOperations = 0;
            timedOutOperations = 0;


            AllocationsData bestAllocation = new AllocationsData();

            for (int i = 0; i <= numberOfOperations; i++)
            {
                ws.GreedyAsync(300000, cd);
            }

            for(int i = 0; i <= numberOfOperations; i++)
            {
                heu.HeuristicAsync(300000, cd);
            }

        }

        private void Heu_HeuristicCompleted(object sender, HeuristicAWS.HeuristicCompletedEventArgs e)
        {
            try
            {
                //get result
                AllocationsData data = e.Result;

                //increment completed counter
                completedOperations++;
                //store result
                results.Add(data);
                //if all completed stop waiting
                if (completedOperations == numberOfOperations)
                {
                    autoResetEvent.Set();
                }

            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                //handle sendtimeout
                //increment completed counter
                completedOperations++;
                //increment time out counter
                timedOutOperations++;
                //completed
                if (completedOperations == numberOfOperations)
                    autoResetEvent.Set();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                //handle sendtimeout
                //increment completed counter
                completedOperations++;
                //increment time out counter
                timedOutOperations++;
                //completed
                if (completedOperations == numberOfOperations)
                    autoResetEvent.Set();
            }

        }
        private void Ws_GreedyCompleted1(object sender, GreedyAWS.GreedyCompletedEventArgs e)
        {
            try
            {
                //get result
                AllocationsData data = e.Result;

                //increment completed counter
                completedOperations++;
                //store result
                results.Add(data);
                //if all completed stop waiting
                if (completedOperations == numberOfOperations)
                {
                    autoResetEvent.Set();
                }

            }
            catch (Exception ex) when (ex.InnerException is TimeoutException tex)
            {
                //handle sendtimeout
                //increment completed counter
                completedOperations++;
                //increment time out counter
                timedOutOperations++;
                //completed
                if (completedOperations == numberOfOperations)
                    autoResetEvent.Set();
            }
            catch (Exception ex) when (ex.InnerException is FaultException fex)
            {
                //handle sendtimeout
                //increment completed counter
                completedOperations++;
                //increment time out counter
                timedOutOperations++;
                //completed
                if (completedOperations == numberOfOperations)
                    autoResetEvent.Set();
            }

        }
        #endregion
    }
}