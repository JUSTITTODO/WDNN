using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using Yang;
using Veneti;

namespace TDNNSTP
{
    class Program
    {
        static void Main(string[] args)
        {
            string type = Console.ReadLine().ToString();
            Random ra = new Random();
            int deadLine;
            switch (type)
            {
                case "time-window":
                    Console.WriteLine("start time-window");
                    string filePath;
                    string filename;
                    int startNode;
                    int destNode;
                    int NeuronNumber;
                    DirectoryInfo dir;
                    StreamWriter writer = new StreamWriter(@"../../result/time-window.csv");
                    for (int i=1; i<=5; i++)
                    {
                        filePath = @"../../DS/tw/tw0"+i;
                        for (int u = 1; u <= 10; u++)
                        {
                            filePath = @"../../DS/tw/tw0" + i + "/0" + i + "-50-graph-200-" + u + ".csv";
                            dir = new DirectoryInfo(filePath);
                            filename = dir.FullName;
                            for (int v = 1; v <= 20; v++)
                            {
                                startNode = ra.Next(0, 200);
                                destNode = ra.Next(0, 200);
                                NeuronNumber = 200;
                                deadLine = 50;
                                AWT(filename, startNode, destNode, NeuronNumber, deadLine, writer);
                            }
                        }
                    }
                    Console.WriteLine("end time-window");
                    writer.Close();
                goto case "number";
                case "number":
                    Console.WriteLine("start number");
                    writer = new StreamWriter(@"../../result/number.csv");
                    for (int j = 50; j<=200; j += 50)
                    {
                        for(int u = 1; u <= 10; u++)
                        {
                            filePath = @"../../DS/nm/" + j + "/05-50-graph-" + j + "-" + u + ".csv";
                            dir = new DirectoryInfo(filePath);
                            filename = dir.FullName;
                            for (int v = 1; v <= 20; v++)
                            {
                                startNode = ra.Next(0, j);
                                destNode = ra.Next(0, j);
                                NeuronNumber = j;
                                deadLine = 50;
                                AWT(filename, startNode, destNode, NeuronNumber, deadLine, writer);
                            }
                        }

                        
                    }
                    writer.Close ();
                    Console.WriteLine("end");
                 goto case  "delay";
                case "delay":
                    Console.WriteLine("start delay");
                    writer = new StreamWriter(@"../../result/delay.csv");
                    for (int i = 50; i <= 100; i += 10)
                    {
                        for (int u = 1; u <= 10; u++)
                        {
                            filePath = @"../../DS/dl/d-" + i + "/05-" + i +"-graph-200-" + u + ".csv";
                            dir = new DirectoryInfo(filePath);
                            filename = dir.FullName;
                            for( int v = 1; v <= 20; v++)
                            {
                                startNode = ra.Next(0, 200);
                                destNode = ra.Next(0, 200);
                                NeuronNumber = 200;
                                deadLine = i;
                                AWT(filename, startNode, destNode, NeuronNumber, deadLine, writer);
                            }
                        }

                    }
                    Console.WriteLine ("end");
                    writer.Close();
                break;
                case "delay-l":
                    Console.WriteLine("start delay-l");
                    writer = new StreamWriter(@"../../result/delay-l.csv");
                    for (int i = 50; i <= 100; i += 10)
                    {
                        filePath = @"../../DS/dl/4941/05-" + i + "-netsci-4941-.csv";
                        dir = new DirectoryInfo(filePath);
                        filename = dir.FullName;
                        for (int v = 1; v <= 20; v++)
                        {
                            startNode = ra.Next(0, 4941);
                            destNode = ra.Next(0, 4941);
                            NeuronNumber = 4941;
                            deadLine = i;
                            AWT(filename, startNode, destNode, NeuronNumber, deadLine, writer);
                        }
                    }
                    writer.Close();
                    Console.WriteLine("end");
                    break;
                case "delay-ll":
                    Console.WriteLine("start delay-ll");
                    writer = new StreamWriter(@"../../result/delay-l-l.csv");
                    for (int i = 50; i <= 100; i += 10)
                    {
                        filePath = @"../../DS/dl/22962/05-" + i + "-netsci-22962-.csv";
                        dir = new DirectoryInfo(filePath);
                        filename = dir.FullName;
                        for (int v = 1; v <= 20; v++)
                        {
                            startNode = ra.Next(0, 22962);
                            destNode = ra.Next(0, 22962);
                            NeuronNumber = 22962;
                            deadLine = i;
                            AWT(filename, startNode, destNode, NeuronNumber, deadLine, writer);
                        }
                    }
                    writer.Close();
                    Console.WriteLine("end");
                    break;
            }
        }
        private static void AWT(string filename, int startNode, int destNode, int NeuronNumber, int deadLine, StreamWriter writer)
        {
            char[] Label = { 'a', 'c', 'e' };
            string strFileName = filename.Substring(filename.LastIndexOf("\\") + 1, filename.Length - 1 - filename.LastIndexOf("\\"));
            
            Yang.Yang djk = new Yang.Yang(filename, NeuronNumber, deadLine, Label);
            Stopwatch djkWatch = new Stopwatch();
            djkWatch.Start();
            djk.ShortestPath(startNode, destNode);
            djkWatch.Stop();
            double djkRuntime = djkWatch.ElapsedTicks * 1000000F / Stopwatch.Frequency;
            
            TDCSP.WDNN tdcsp = new TDCSP.WDNN(filename, NeuronNumber, deadLine, Label);
            Stopwatch tdcspWatch = new Stopwatch();
            tdcsp.NIA(startNode, destNode);
            tdcspWatch.Start();
            tdcsp.TL_main();
            tdcspWatch.Stop();
            double TdcspRuntime = tdcspWatch.ElapsedTicks * 1000000F / Stopwatch.Frequency;
            
            Veneti.Veneti fls = new Veneti.Veneti(filename, NeuronNumber, deadLine, Label);
            Stopwatch flsWatch = new Stopwatch();
            flsWatch.Start();
            fls.DTLFS(startNode, destNode);
            flsWatch.Stop();
            double flsRuntime = flsWatch.ElapsedTicks * 1000000F / Stopwatch.Frequency;

            writer.WriteLine(strFileName + ",WDNN," + tdcsp.DestNodeArrivalTime + "," + TdcspRuntime + ",Veneti," + fls.DestNodeArrTime + "," + flsRuntime + ",Yang," + djk.DestNodeArrTime + "," + djkRuntime);
            writer.Flush();
        }
        
    }
}
