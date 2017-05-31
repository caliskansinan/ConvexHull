using Microsoft.VisualBasic.PowerPacks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SinanCaliskan
{
    public partial class frmMain : Form
    {
        struct Segment//Creating for structure for 2 point line
        {
            public Point p;
            public Point q;
            public bool Contains(Point point)
            {//Checking with point is some with p or q
                if (p.Equals(point) || q.Equals(point))
                    return true;
                return false;
            }
        }
        ShapeContainer container;//defined oval,line shape container
        List<Segment> Segments = new List<Segment>();//Creating segment list
        public frmMain()//form initializing
        {
            InitializeComponent();
            container = new ShapeContainer();//Creating oval,line shape container
            container.Location = new Point(0,0);//Set the container location
            container.Size = this.Size;//Set the container size = form size
            this.Controls.Add(container);//added container in form controls
        }

        private void frmMain_MouseDown(object sender, MouseEventArgs e)
        {//frm_main mouse down event
            OvalShape os = new OvalShape(container);//creating new oval shape in ShapeContainer            
            os.Location = new Point(e.X-5, e.Y-5);//setting location mouse x-5,y-5 location
            os.Size = new Size(10, 10);//setting ovalshape size
            os.Click += os_Click;//creating ovalshape click event
            //getting ShapeContainer which items is OvalShape and Counting
            if (container.Shapes.OfType<OvalShape>().Count()>2)//if is greater than 2
            {
                InitPoints();//creating point relations with another point
                DrawConvex();//Drawing convex lines
            }
        }

        void os_Click(object sender, EventArgs e)//OvalShape click event
        {
            //Showing Location X,Y
            MessageBox.Show((sender as OvalShape).Location.ToString());
        }
        void InitPoints()
        {
            Segments.Clear();//Clearing Segment list
            //getting ShapeContainer in typeof OvalShape object and setting new list
            List<OvalShape> ovalShapeList=container.Shapes.OfType<OvalShape>().ToList();
            for (int i = 0; i < ovalShapeList.Count; i++)
            {//first ovalshape
                for (int j = 0; j < ovalShapeList.Count; j++)
                {//second ovalshape
                    if (i!=j)
                    {//if ovalshape is not same ovalshape create new Segment
                        Segment s = new Segment();//creating new Segment
                        s.p = ovalShapeList[i].Location;//set ovalshape
                        s.q = ovalShapeList[j].Location;//set ovalshape
                        Segments.Add(s);//Adding Segment in List Segments
                    }
                }
            }
        }
        void DrawConvex()
        {
            for (int i = 0; i < Segments.Count;)
            {
                Segment s=Segments[i];
                //Creating new OvalShape list for points and checking with Segment Points
                List<OvalShape> ovalShapeList = new List<OvalShape>(container.Shapes.OfType<OvalShape>().ToList());
                for (int j = 0; j < ovalShapeList.Count; )
                {
                    OvalShape ov = ovalShapeList[j];
                    //Checking Segment Contains OvalShape locations (X,Y) at the same time
                    if (s.Contains(ov.Location))
                    {//removing OvalShape in OvalShape list and starting first OvalShape object
                        ovalShapeList.Remove(ov);
                        j = 0;
                        continue;
                    }
                    j++;
                }
                //Checking Segment is not edge if its now need to remove in list
                if (!isEdge(ovalShapeList, s))
                {//removing Segment in SegmentList and starting first Segment
                    Segments.Remove(s);
                    i = 0;
                    continue;
                }
                else
                    i++;
            }
            RenderEdges();//after finished operations calling RenderEdges function
        }
        bool isEdge(List<OvalShape> ovalShapeList, Segment edge)
        {
            foreach (OvalShape ov in ovalShapeList)
            {//it checks it doesnt have point left side
                if (isLeft(edge,ov.Location))
                {//it has return false
                    return false;
                }
            }//if doesnt have return true
            return true;
        }
        bool isLeft(Segment edge, Point r)
        {
            float D,px, py, qx, qy, rx, ry;
            //The determinant
            // | 1 px py |
            // | 1 qx qy |
            // | 1 rx ry |
            //if the determinant result is positive then the point is left of the segment
            px = edge.p.X;
            py = edge.p.Y;
            qx = edge.q.X;
            qy = edge.q.Y;
            rx = r.X;
            ry = r.Y;

            D = ((qx * ry) - (qy * rx)) - (px * (ry - qy)) + (py * (rx - qx));

            if (D <= 0)
                return false;
            return true;
        }
        void RenderEdges()
        {
            //getting ShapeContainer typeof LineShape objects in new list
            List<LineShape> lineShapes = container.Shapes.OfType<LineShape>().ToList();
            foreach (LineShape item in lineShapes)
            {//removing in ShapeContainer LineShape's
                container.Shapes.Remove(item);
            }
            foreach (Segment s in Segments)
            {//creating new LineShape's for Each Segment and adding in ShapeContainer
                LineShape ls = new LineShape(container);
                ls.BorderColor = Color.Red;//setting line color
                ls.StartPoint = new Point(s.p.X+5,s.p.Y+5);//LineShape start point
                ls.EndPoint   = new Point(s.q.X + 5, s.q.Y + 5);//LineShape end point
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Segments.Clear();//it clears Segment List
            container.Shapes.Clear();//it clears ShapeContainer Shape objects
        }
    }
}