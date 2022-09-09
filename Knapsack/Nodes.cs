using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Colouring
{
    public class Nodes
    {
        public string name;
        public List<color> ColourList;
        public int assignedColor = -1;
        public List<string> Neighbour; //List of all neighbours to which this node is connected//
        public int color_potential;

        public Nodes(string name)
        {
            this.name = name;
            this.ColourList = new List<color>(0);
            this.ColourList.Add(new color(0,true));
            this.Neighbour = new List<string>();

        }

        public void AddNewColor()
        {
            ColourList.Add(new color(ColourList.Last().colour_name+1, true));
        }

    }

    public class color
    {
        public int colour_name;   //This contains the colour code for eg. 0,1,2
        public bool assigned;
        public bool canbeassinged;

        public color(int c, bool canb12eassinged)
        {
            this.colour_name = c;
            this.canbeassinged = canb12eassinged;
        }
        

    }

    
}
