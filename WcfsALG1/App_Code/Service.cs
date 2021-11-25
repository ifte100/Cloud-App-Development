using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfsServiceLibrary;
using System.Diagnostics;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service" in code, svc and config file together.
public class Task
{
    public int ID { get; set; }
    public double Runtime { get; set; }
    public double Frequency { get; set; }
    public int RAM { get; set; }
    public int DownloadSpeed { get; set; }
    public int UploadSpeed { get; set; }

    public bool taskAllocated { get; set; }
    public Task(int Id, double runtime, double frequency, int download, int upload, int ram)
    {
        ID = Id;
        Runtime = runtime;
        Frequency = frequency;
        DownloadSpeed = download;
        UploadSpeed = upload;
        RAM = ram;
    }
}

public class Processor
{
    public int DownloadSpeed { get; set; }
    public int RAM { get; set; }
    public double Frequency { get; set; }
    public int ID { get; set; }
    public int UploadSpeed { get; set; }
    public Processor(int Id, double frequency, int download, int upload, int ram)
    {
        ID = Id;
        Frequency = frequency;
        DownloadSpeed = download;
        UploadSpeed = upload;
        RAM = ram;
    }
}
public class Service : IService
{
    public AllocationsData Greedy(int deadline, ConfigData cd)
    {
        //Greedy code here
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        List<Task> tasks = new List<Task>();
        List<Processor> processors = new List<Processor>();

        //this 2d array is to store all the runtime values calculated in PT1
        //processor* cd.NumTasks + task formula to convert 1D to 2D
        double[,] RuntimeOnProcessor = new double[cd.NumProcs, cd.NumTasks];
        for (int processor = 0; processor < cd.NumProcs; processor++)
        {
            for (int task = 0; task < cd.NumTasks; task++)
            {
                RuntimeOnProcessor[processor, task] = cd.Runtimes[processor * cd.NumTasks + task];
            }
        }

        double[,] Energies = new double[cd.NumProcs, cd.NumTasks];
        for (int processor = 0; processor < cd.NumProcs; processor++)
        {
            for (int task = 0; task < cd.NumTasks; task++)
            {
                Energies[processor, task] = cd.Energies[processor * cd.NumTasks + task];
            }
        }

        for (int id = 0; id < cd.NumTasks; id++)
        {
            Task newTask = new Task(id, cd.TaskRuntime[id], cd.TaskFreq[id], cd.TaskDownloadSpeed[id], cd.TaskUploadSpeed[id], cd.TaskRam[id]);
            tasks.Add(newTask);
        }

        for (int id = 0; id < cd.NumProcs; id++)
        {
            Processor newProcessor = new Processor(id, cd.ProcsFreq[id], cd.ProcessorDownloadSpeed[id], cd.ProcessorUploadSpeed[id], cd.ProcsRam[id]);
            processors.Add(newProcessor);
        }

        tasks.Sort((x, y) => (x.Frequency * x.Runtime).CompareTo(y.Frequency * y.Runtime));
        processors.Sort((x, y) => (x.Frequency).CompareTo(y.Frequency));

        int[,] validAlloc = new int[processors.Count, tasks.Count];

        for (int processor = 0; processor < processors.Count; processor++)
        {
            for (int task = 0; task < tasks.Count; task++)
            {
                if (processors[processor].RAM >= tasks[task].RAM && processors[processor].DownloadSpeed >= tasks[task].DownloadSpeed && processors[processor].UploadSpeed >= tasks[task].UploadSpeed)
                {
                    validAlloc[processor, task] = 1;
                }
                else
                {
                    validAlloc[processor, task] = 0;
                }
            }
        }

        int[,] Map = new int[processors.Count, tasks.Count];

        double taskRuntime = 0;

        for (int processor = 0; processor < processors.Count; processor++)
        {
            double processorSum = 0;
            for (int task = 0; task < tasks.Count; task++)
            {
                if (tasks[task].taskAllocated == false)
                {
                    taskRuntime = tasks[task].Runtime * (tasks[task].Frequency / processors[processor].Frequency);
                    if ((processorSum + taskRuntime <= cd.Duration) && (validAlloc[processor, task] == 1))
                    {
                        Map[processor, task] = 1;
                        processorSum += taskRuntime;
                        tasks[task].taskAllocated = true;
                    }
                }
            }
            taskRuntime = 0;
        }

        int[] map1D = Transform(Map, cd.NumProcs, cd.NumTasks);
        
        AllocationsData allocationsData = new AllocationsData();

        allocationsData.Description = TAFFData(cd.Path, 1, cd.NumTasks, cd.NumProcs, Map);
        allocationsData.Count = 1;
        allocationsData.Energy = 123.5; // this is the minimum energy

        return allocationsData;
    }

    private string TAFFData(string filename, int count, int tasks, int processors, int[,] Map)
    {
        string s = "";

        s += @"CONFIGURATION-DATA" + Environment.NewLine;
        s += @"FILENAME=" + @"""" + filename + @"""" + Environment.NewLine;
        s += @"END-CONFIGURATION-DATA" + Environment.NewLine;

        s += @"ALLOCATIONS" + Environment.NewLine;
        s += @"COUNT=" + count.ToString() + Environment.NewLine;
        s += @"TASKS=" + tasks.ToString() + Environment.NewLine;
        s += @"PROCESSORS=" + processors.ToString() + Environment.NewLine;

        s += @"ALLOCATION" + Environment.NewLine;
        s += @"ID=0"+ Environment.NewLine;
        s += @"MAP=" +  print2DArray(Map)+ Environment.NewLine;
        s += @"END-ALLOCATION" + Environment.NewLine;

        s += @"END-ALLOCATIONS" + Environment.NewLine;

        return s;
    }

    public int[] Transform(int[,] data, int rows, int columns)
    {
        int[] data1D = new int[rows * columns];

        for(int row = 0; row < rows; row++)
        {
            for(int column = 0; column < columns; column++)
            {
                data1D[row * columns + column] = data[row, column];
            }
        }

        return data1D;  
    }

    public string print2DArray(int[,] Array)
    {
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < Array.GetLength(0); i++)
        {
            for (int j = 0; j < Array.GetLength(1); j++)
            {
                sb.Append(string.Format("{0}", Array[i, j] + ","));
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append(";");
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }
}
