using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Colouring
{
    class InputReader
    {
        public static Dictionary<string, Nodes> ReadData(string[] args, ref List<string> ListEdges)
        {
            //This function reads the problem data into a dictionary of nodes and list of edges
           
            //string probpath = args[0];

            Dictionary<string, Nodes> Node_dict = new Dictionary<string, Nodes>();
            //var Lines = File.ReadLines(@probpath, Encoding.UTF8).ToArray();
            var Lines = File.ReadLines(@"C:\Users\Akash Sambrekar\Desktop\avlino assesment\graph colouring\GraphColouring\coloring\data\gc_500_1", Encoding.UTF8).ToArray();           
            int NodesCount = Convert.ToInt32(Lines[0].Split(' ').First());
            int EdgesCount = Convert.ToInt32(Lines[0].Split(' ').Last());
           
            for(int i = 1;i<= EdgesCount; i++)
            {              
                var line = Lines[i].Split(' ');
                var first = line.First();
                var second = line.Last();
                ListEdges.Add(first+"@"+second);
                if (!Node_dict.ContainsKey(first))
                {
                    Node_dict.Add(first, new Nodes(first));
                    Node_dict[first].Neighbour.Add(second);

                }
                else
                {
                    Node_dict[first].Neighbour.Add(second);
                }

                if (!Node_dict.ContainsKey(second))
                {
                    Node_dict.Add(second, new Nodes(second));
                    Node_dict[second].Neighbour.Add(first);

                }
                else
                {
                    Node_dict[second].Neighbour.Add(first);
                }

            }
            return Node_dict;

        }
    }
}
