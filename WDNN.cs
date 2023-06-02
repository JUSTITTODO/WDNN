using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Ulity;
namespace TDNNSTP.TDCSP
{
    class WDNN
    {
        public NeuronValue[] Neurons { set; get; }
        public List<EdgeValue> Edges { set; get; } 
        public int deadLine { set; get; }
        public int DestNodeArrivalTime { set; get; }
        public NeuronValue RootNeuron { set; get; }
        public NeuronValue DestNeuron { set; get; }
        public List<int> Path = new List<int>();
        public List<int> depTime = new List<int>();
        public char[] LabelOfNode { set; get; }
        public WDNN(string inputFile, int NeuronNumber, int deadLine, char[] label)
        {
            LabelOfNode = label;
            this.deadLine = deadLine;
            Edges = new List<EdgeValue>();
            Neurons = new NeuronValue[NeuronNumber];
            StreamReader reader = new StreamReader(inputFile);
            string aLine = reader.ReadLine();
            for (int index = 0; index<NeuronNumber; index++)
            {
                string[] rawData = aLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int node = Convert.ToInt32(rawData[0]);
                if (Neurons[node] == null)
                {
                    Neurons[node] = new NeuronValue(node, deadLine);
                }
                for(int l =1; rawData[l] != "#"; l++)
                {
                    Neurons[node].LabelOfNode.Add(Convert.ToChar(rawData[l]));
                }
                aLine = reader.ReadLine();
            }
            while (aLine != null)
            {
                string[] rawData = aLine.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                int departNode = Convert.ToInt32(rawData[0]);
                int arriveNode = Convert.ToInt32(rawData[1]);
                EdgeValue e = new EdgeValue(Neurons[departNode], Neurons[arriveNode]);
                Neurons[departNode].SucEdge.Add(e);
                Edges.Add(e);
                for(int w = 2; rawData[w] != "#";  w += 3)
                {
                    e.TimeWindows.Add(new TimeWindow(Convert.ToInt32(rawData[w]), Convert.ToInt32(rawData[w + 1]), Convert.ToInt32(rawData[w + 2])));
                }
                aLine = reader.ReadLine();
            }
            reader.Close();
        }

        public void TL_main()
        {
            DestNodeArrivalTime = 100000; 
            int t = 0;
            NUA(t);
            foreach(var ac in DestNeuron.ActiveTime)
            {
                if (ac.Label == LabelOfNode[0] || ac.Label == LabelOfNode[1] || ac.Label == LabelOfNode[2]  && DestNodeArrivalTime > ac.ActiveTime)
                {
                    DestNodeArrivalTime = ac.ActiveTime;
                }
            }
           
        }
        public void NIA(int startNode, int destNode)
        {
            RootNeuron = Neurons[startNode];
            DestNeuron = Neurons[destNode];
            RootNeuron.RootInitFuc(deadLine);
            foreach(var n in Neurons)
            {
                if(n != RootNeuron)
                {
                    n.OtherInitFuc(deadLine);
                }
            }
            foreach(var e  in Edges)
            {
                e.initTimeWindows(deadLine);
            }
        }
        public void NUA(int time)
        {
            while (time <= deadLine)
            {
                bool fl = false;
                foreach (var node in Neurons)
                {
                    node.UpdateNeuronState(time, LabelOfNode, deadLine);
                    foreach (var x in DestNeuron.ActiveTime)
                    {
                        if (x.Label == LabelOfNode[0] || x.Label == LabelOfNode[1] || x.Label == LabelOfNode[2])
                        {
                            if (x.ActiveTime != 100000)
                            {
                                DestNodeArrivalTime = x.ActiveTime;
                                fl = true;
                                break;
                            }
                        }
                    }
                    if (fl)
                    {
                        break;
                    }
                }
                time++;
            }
        }
    }
}