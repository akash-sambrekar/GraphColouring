using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;
using ILOG.CP;
using ILOG.Concert;

namespace Colouring
{


    public class Program
    {
        static void Main(string[] args)
        {
            try
            {

                Stopwatch stopwatch = new Stopwatch();

                List<string> ListEdges = new List<string>();
                var dictNodes = InputReader.ReadData(args, ref ListEdges);
                stopwatch.Start();

                var ordereddict = dictNodes.OrderByDescending(x => x.Value.Neighbour.Count).ToDictionary(x => x.Key, y => y.Value);



                while (true)
                {
                    var NodesNotColoured = ordereddict.Where(x => x.Value.assignedColor == -1).Count();
                    if (NodesNotColoured == 0)
                        break;
                    //The function BestNodetoColour finds the best node to be coloured such that maximum nodes are coloured with the same colour
                    var bestnodename = BestNodeToColour(ordereddict);
                    var bestnode = ordereddict[bestnodename];
                    foreach (var color in bestnode.ColourList)
                    {
                        //try to colour the bestnode found,with the available colours
                        if (color.canbeassinged == true)
                        {

                            color.assigned = true;
                            bestnode.assignedColor = color.colour_name;
                            break;
                        }

                    }

                    if (bestnode.assignedColor == -1)
                    {
                        //if the bestnode was not coloured , we add new color to set of all available colours
                        // also we colour  the bestnode with the new colour
                        foreach (var gari in ordereddict)
                        {
                            gari.Value.AddNewColor();
                        }

                        var objcol = bestnode.ColourList.Last();
                        objcol.assigned = true;
                        bestnode.assignedColor = objcol.colour_name;
                    }

                    foreach (var neigh in bestnode.Neighbour)
                    {
                        //now update that bestnode color found can't be used to color its neighbours
                        if (ordereddict[neigh].assignedColor == -1)
                        {
                            ordereddict[neigh].ColourList[bestnode.assignedColor].canbeassinged = false;
                        }
                    }
                    var listneigh = bestnode.Neighbour.ToList();
                    var nodes_tobe_colored = ordereddict.Keys.Except(listneigh);
                    foreach (var node in nodes_tobe_colored)
                    {
                        if (ordereddict[node].assignedColor == -1)
                            if (ordereddict[node].ColourList[bestnode.assignedColor].canbeassinged)
                            {
                                ordereddict[node].assignedColor = bestnode.assignedColor;

                                // now we update the neighboring nodes can't be coloured with the bestnode colour
                                foreach (var neigh in ordereddict[node].Neighbour)
                                {
                                    if (ordereddict[neigh].assignedColor == -1)
                                    {
                                        ordereddict[neigh].ColourList[bestnode.assignedColor].canbeassinged = false;
                                    }
                                }

                            }

                    }

                }

                var result = ordereddict.Select(x => x.Value.assignedColor).Distinct().ToList();
                int HeuristicColours = result.Count;
                string heuristicsolution = null;
                var dict2 = ordereddict.ToDictionary(x => Convert.ToInt32(x.Key), y => y.Value).OrderBy(x => x.Key).ToDictionary(x => x.Key, y => y.Value);

                foreach (var yari in dict2)
                {
                    heuristicsolution += yari.Value.assignedColor.ToString() + " ";
                }
                int colours = HeuristicColours - 1;// since 0 is also considered as color
                string FinalCPSolution = null;

                /*********Try  CP********/


                while (true)
                {
                    colours--;
                    CP cp = new CP();
                    Dictionary<string, IIntVar> dictvariables = new Dictionary<string, IIntVar>();
                    string CPSolution = null;
                    foreach (var vari in dict2)
                    {

                        IIntVar variab = cp.IntVar(0, colours, vari.Key.ToString());
                        cp.Add(variab);
                        dictvariables.Add(vari.Key.ToString(), variab);

                    }

                    //cp.SetParameter(CP.IntParam.LogVerbosity, CP.ParameterValues.Quiet);

                    foreach (var vari in ListEdges)
                    {
                        var line = vari.Split('@').ToArray();

                        // the two nodes on the same edge can't have the same color so we add not equal constraint on both the nodes
                        cp.Add(cp.Neq(dictvariables[line[0]], dictvariables[line[1]]));

                    }

                    cp.SetParameter(CP.DoubleParam.TimeLimit, 120);
                    if (cp.Solve())
                    {

                        Console.WriteLine($"A solution was found with {colours}");

                        foreach (var vari in dictvariables)
                        {
                            CPSolution += cp.GetIntValue(vari.Value) + " ";
                        }
                        FinalCPSolution = CPSolution;
                    }
                    else
                    {
                        break;
                    }

                }
                if (colours < HeuristicColours)
                {
                    Console.WriteLine(colours + " " + 1);
                    Console.WriteLine(FinalCPSolution);
                }
                else
                {
                    Console.WriteLine(HeuristicColours + " " + 0);
                    Console.WriteLine(heuristicsolution);
                }
            }
            catch (System.Exception ex )
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            
            


        }

        public static string BestNodeToColour(Dictionary<string, Nodes> ordereddict)
        {
            foreach (var firstnode in ordereddict)
            {
                if (firstnode.Value.assignedColor == -1)
                {
                    foreach (var color in firstnode.Value.ColourList)
                    {
                        //try to colour the firstnode with the existing colors available
                        if (color.canbeassinged == true)
                        {
                            //tari.assigned = true;
                            firstnode.Value.assignedColor = color.colour_name;
                            break;
                        }
                    }
                    if (firstnode.Value.assignedColor == -1)
                    {
                        //if the firstnode was not coloured , we add new color to set of all available colours
                        // also we colour  the bestnode with the new colour
                        foreach (var anothernode in ordereddict)
                        {
                            anothernode.Value.AddNewColor();
                        }

                        var objcol = firstnode.Value.ColourList.Last();
                        //objcol.assigned = true;
                        firstnode.Value.assignedColor = objcol.colour_name;
                    }

                    foreach (var neigh in firstnode.Value.Neighbour)
                    {
                        if (ordereddict[neigh].assignedColor == -1)
                        {
                            ordereddict[neigh].ColourList[firstnode.Value.assignedColor].canbeassinged = false;
                        }
                    }

                    var listneigh = firstnode.Value.Neighbour.ToList(); 
                    var nodes_notcoloured = ordereddict.Keys.Except(listneigh);
                    var samecolor = firstnode.Value.assignedColor;
                    int potential = 0;

                    //now we try to check how many nodes(other than the first node and its neighbours) with the
                    //samecolor as that of the firstnode
                    foreach (var node in nodes_notcoloured)
                    {
                        
                        if (ordereddict[node].assignedColor == -1)
                            if (ordereddict[node].ColourList[samecolor].canbeassinged)
                            {
                                // we use a large number 10000 so that it can be  later initialized to -1 , for the next iteration node within the foreach loop
                                ordereddict[node].assignedColor = 10000;
                                // we increment the potential of the firstnode 1, this means if you select firstnode as the bestnode to colour
                                // then we can color "potential" number of nodes with the same colour
                                potential += 1;

                                foreach (var neigh in ordereddict[node].Neighbour)
                                {
                                    if (ordereddict[neigh].assignedColor == -1)
                                    {
                                        ordereddict[neigh].ColourList[samecolor].canbeassinged = false;
                                    }
                                }
                            }
                    }

                    //update potential of node
                    firstnode.Value.color_potential = potential;
                    //this means that if you select firstnode as the first node to begin , we can color "potential" number
                    //of nodes in the graph
                    foreach (var rari in nodes_notcoloured)
                    {
                        if (ordereddict[rari].assignedColor == 10000)
                            if (ordereddict[rari].ColourList[samecolor].canbeassinged)
                            {
                                //we remove the color of all the nodes that were coloured with samecolor
                                ordereddict[rari].assignedColor = -1;

                                foreach (var neigh in ordereddict[rari].Neighbour)
                                {
                                    if (ordereddict[neigh].assignedColor == -1)
                                    {
                                        // now we also allow the its neighbours to be colored with the same colour
                                        ordereddict[neigh].ColourList[samecolor].canbeassinged = true; 
                                    }


                                }

                            }

                    }

                    // we restore the properties of firstnode to its default values
                    foreach (var neigh in firstnode.Value.Neighbour)
                    {
                        if (ordereddict[neigh].assignedColor == -1)
                        {
                            ordereddict[neigh].ColourList[samecolor].canbeassinged = true;
                        }
                    }
                    firstnode.Value.assignedColor = -1;

                }
            }
            var result = ordereddict.Where(x => x.Value.assignedColor == -1).OrderByDescending(x => x.Value.color_potential).First().Key;
            // now select the node which has the highest potential to be colored 
            return result;
        }

    }

}



