//---------------------------------------------------------------------
// Copyright © 2020 Phil Crompton
// based on original work
// Copyright © 2006 Raymund Sarmiento
//
// Permission is hereby granted to use this Software for any purpose
// including combining with commercial products, creating derivative
// works, and redistribution of source or binary code, without
// limitation or consideration. Any redistributed copies of this
// Software must include the above Copyright Notice.
//
// THIS SOFTWARE IS PROVIDED "AS IS". THE AUTHOR OF THIS CODE MAKES NO
// WARRANTIES REGARDING THIS SOFTWARE, EXPRESS OR IMPLIED, AS TO ITS
// SUITABILITY OR FITNESS FOR A PARTICULAR PURPOSE.
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NPoint
{
    public partial class MainForm : Form
    {
        #region private members ...
        int drawflag = 0;
        int numpoint = 0;


        int p1x = 0;
        int p1y = 0;
        int p1z = 0;
        int p2x = 0;
        int p2y = 0;
        int p2z = 0;
        int p3x = 0;
        int p3y = 0;
        int p3z = 0;
        int p4x = 0;
        int p4y = 0;
        int p4z = 0;
        int p5x = 0;
        int p5y = 0;
        int p5z = 0;
        int p6x = 0;
        int p6y = 0;
        int p6z = 0;
        int p7x = 0;
        int p7y = 0;
        int p7z = 0;
        int p8x = 0;
        int p8y = 0;
        int p8z = 0;
        int p9x = 0;
        int p9y = 0;
        int p9z = 0;
        int pax = 0;
        int pay = 0;
        int paz = 0;
        #endregion

        const int TOTPOINTS = 1000; // Number of user defined object stars
        const int NUMREFERENCE = 10; // Number of reference stars

        [Serializable]
        private struct Xformstars
        { // structure to identify the 3 stars xform matrix for a particular object star
            public int d1;
            public int d2;
            public int d3;
        }


        private Coord[] ct_Points = new Coord[10]; //Catalog Points
        private Coord[] my_Points = new Coord[10]; //My Measured Points

        private Coord[] my_Objects = new Coord[TOTPOINTS]; //My Object Points
        private Xformstars[] my_ObjectsXformStars = new Xformstars[TOTPOINTS]; // My Object Star's assigned 3 star reference ids

        // Storage for Transformed Coordinates using Taki Method
        private Coord[] my_xformObjects_Taki = new Coord[TOTPOINTS];

        // Storage for Transformed Coordinates using Affine Mapping method

        private Coord[] my_xformObjects_Affine = new Coord[TOTPOINTS];

        public MainForm()
        {
            InitializeComponent();
        }

        #region Event handlers ...
        private void MainForm_Load(Object eventSender, EventArgs eventArgs)
        {

            int i = 0;


            drawflag = 0;
            numpoint = 1;

            //Define Arbitrary points on the form as the fixed catalog star locations


            p1x = 1995;
            p1y = 2250;
            p1z = 1;

            p2x = 8430;
            p2y = 2265;
            p2z = 1;


            p3x = 3270;
            p3y = 7620;
            p3z = 1;

            p4x = 8535;
            p4y = 7335;
            p4z = 1;

            p5x = 14000;
            p5y = 2000;
            p5z = 1;


            p6x = 14000;
            p6y = 7815;
            p6z = 1;

            p7x = 5790;
            p7y = 4230;
            p7z = 1;

            p8x = 11400;
            p8y = 4710;
            p8z = 1;

            p9x = 420;
            p9y = 4635;
            p9z = 1;

            pax = 14850;
            pay = 4740;
            paz = 1;


            // Put them on the transformation matrix input parameters

            // ct_points = Catalog Star Coordinates

            ct_Points[0].x = p1x;
            ct_Points[0].Y = p1y;
            ct_Points[0].z = p1z;

            ct_Points[1].x = p2x;
            ct_Points[1].Y = p2y;
            ct_Points[1].z = p2z;

            ct_Points[2].x = p3x;
            ct_Points[2].Y = p3y;
            ct_Points[2].z = p3z;

            ct_Points[3].x = p4x;
            ct_Points[3].Y = p4y;
            ct_Points[3].z = p4z;

            ct_Points[4].x = p5x;
            ct_Points[4].Y = p5y;
            ct_Points[4].z = p5z;

            ct_Points[5].x = p6x;
            ct_Points[5].Y = p6y;
            ct_Points[5].z = p6z;

            ct_Points[6].x = p7x;
            ct_Points[6].Y = p7y;
            ct_Points[6].z = p7z;

            ct_Points[7].x = p8x;
            ct_Points[7].Y = p8y;
            ct_Points[7].z = p8z;

            ct_Points[8].x = p9x;
            ct_Points[8].Y = p9y;
            ct_Points[8].z = p9z;

            ct_Points[9].x = pax;
            ct_Points[9].Y = pay;
            ct_Points[9].z = paz;

            // my_points = Measured Star Coordinates

            my_Points[0].x = p1x;
            my_Points[0].Y = p1y;
            my_Points[0].z = p1z;

            my_Points[1].x = p2x;
            my_Points[1].Y = p2y;
            my_Points[1].z = p2z;

            my_Points[2].x = p3x;
            my_Points[2].Y = p3y;
            my_Points[2].z = p1z;

            my_Points[3].x = p4x;
            my_Points[3].Y = p4y;
            my_Points[3].z = p4z;

            my_Points[4].x = p5x;
            my_Points[4].Y = p5y;
            my_Points[4].z = p5z;

            my_Points[5].x = p6x;
            my_Points[5].Y = p6y;
            my_Points[5].z = p6z;

            my_Points[6].x = p7x;
            my_Points[6].Y = p7y;
            my_Points[6].z = p7z;

            my_Points[7].x = p8x;
            my_Points[7].Y = p8y;
            my_Points[7].z = p8z;

            my_Points[8].x = p9x;
            my_Points[8].Y = p9y;
            my_Points[8].z = p9z;

            my_Points[9].x = pax;
            my_Points[9].Y = pay;
            my_Points[9].z = paz;


            // Initialize my object points to 0 first

            for (i = 0; i < TOTPOINTS; i++)
            {

                my_Objects[i].x = 0;
                my_Objects[i].Y = 0;
                my_Objects[i].z = 1;

                my_xformObjects_Taki[i].x = 0;
                my_xformObjects_Taki[i].Y = 0;
                my_xformObjects_Taki[i].z = 1;

                my_xformObjects_Affine[i].x = 0;
                my_xformObjects_Affine[i].Y = 0;
                my_xformObjects_Affine[i].z = 1;

            }



        }

        //Button to reset points back to the original state

        private void MainForm_MouseDown(Object eventSender, MouseEventArgs eventArgs)
        {
            int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
            int Shift = ((int)Control.ModifierKeys) / 65536;
            float x = eventArgs.X * 15;
            float y = eventArgs.Y * 15;

            drawflag = Button;

            if (Button == 1)
            { // Button the move the measured stars (blue circles)

                //Erase Old Dots

                adjust_reddots(Color.Black); //erase the old reddots
                adjust_yellowdots(Color.Black); //erase the old yellowdots

                draw_bluedot(Convert.ToInt32(x), Convert.ToInt32(y)); //draw new position of measured stars

                //Draw Reddots using Taki Method
                adjust_reddots(Color.Red); //draw new location of the object stars

                //Draw Yellow Dots using Affine Mapping Method
                adjust_yellowdots(Color.Yellow); //draw new location of the object stars


                redraw_mystars(); //redraw the object catalog stars
                draw_fixedStars(); //redraw the green catalog stars
                draw_measuredStars(Color.Blue); //redraw the blue measured stars

            }

            if (Button == 2)
            {
                draw_mystars(Convert.ToInt32(x), Convert.ToInt32(y)); //draw the object catalog stars
            }

        }

        private void MainForm_MouseMove(Object eventSender, MouseEventArgs eventArgs)
        {
            int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
            int Shift = ((int)Control.ModifierKeys) / 65536;
            float x = eventArgs.X * 15;
            float y = eventArgs.Y * 15;

            label1.Text = $"X - {x}";
            label2.Text = $"Y - {y}";


            if ((drawflag == 1) && (Button == 1))
            {

                //Draw all the circles

                draw_fixedStars();
                redraw_mystars();

                // Erase old Dots
                adjust_reddots(Color.Black);
                adjust_yellowdots(Color.Black);

                draw_bluedot(Convert.ToInt32(x), Convert.ToInt32(y));
                draw_measuredStars(Color.Blue);


                // Draw Reddots using Taki Transformation Method

                adjust_reddots(Color.Red);

                // Draw Yellow Dots using Affine Mapping Transformation Method

                adjust_yellowdots(Color.Yellow);


            }


            if ((drawflag == 2) && (Button == 2))
            {

                // Draw the white circles

                draw_mystars(Convert.ToInt32(x), Convert.ToInt32(y));

            }

        }

        private void MainForm_MouseUp(Object eventSender, MouseEventArgs eventArgs)
        {
            int Button = (eventArgs.Button == MouseButtons.Left) ? 1 : ((eventArgs.Button == MouseButtons.Right) ? 2 : 4);
            int Shift = ((int)Control.ModifierKeys) / 65536;
            float x = eventArgs.X * 15;
            float y = eventArgs.Y * 15;
            drawflag = 0;
        }

        private void MainForm_Paint(Object eventSender, PaintEventArgs eventArgs)
        {

            int i = 0;

            draw_fixedStars();

            for (i = 0; i < NUMREFERENCE; i++)
            {
                this.Circle(((float)ct_Points[i].x) / 15, ((float)ct_Points[i].Y) / 15, 7, Color.Blue);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Coord obj2 = new Coord();
            int i = 0;

            for (i = 0; i < TOTPOINTS; i++)
            {

                // Cleanout current star location objects

                this.Circle(((float)my_Objects[i].x) / 15, ((float)my_Objects[i].Y) / 15, 3, Color.Black);
                this.Circle(((float)my_Objects[i].x) / 15, ((float)my_Objects[i].Y) / 15, 1, Color.Black);

                if ((my_Objects[i].x + my_Objects[i].Y) != 0)
                {

                    // Erase those that are in the current position

                    EQTakiAffine.EQ_AssembleMatrix_Taki(0, 0, ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3], my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3]);

                    obj2 = EQTakiAffine.EQ_Transform_Taki(my_Objects[i]);
                    this.Circle(((float)obj2.x) / 15, ((float)obj2.Y) / 15, 3, Color.Black);

                    EQTakiAffine.EQ_AssembleMatrix_Affine(0, 0, ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3], my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3]);

                    obj2 = EQTakiAffine.EQ_Transform_Affine(my_Objects[i]);
                    this.Circle(((float)obj2.x) / 15, ((float)obj2.Y) / 15, 1, Color.Black);

                }

                // Reset Object points back to 0

                my_Objects[i].x = 0;
                my_Objects[i].Y = 0;
                my_Objects[i].z = 1;

                // Reset also the transformed objects

                my_xformObjects_Taki[i].x = 0;
                my_xformObjects_Taki[i].Y = 0;
                my_xformObjects_Taki[i].z = 1;


                my_xformObjects_Affine[i].x = 0;
                my_xformObjects_Affine[i].Y = 0;
                my_xformObjects_Affine[i].z = 1;



            }

            // Erase the adjusted blue stars

            draw_measuredStars(Color.Black);

            numpoint = 1;


            // Restore star location back to their original catalog locations


            my_Points[0].x = p1x;
            my_Points[0].Y = p1y;
            my_Points[0].z = p1z;

            my_Points[1].x = p2x;
            my_Points[1].Y = p2y;
            my_Points[1].z = p2z;

            my_Points[2].x = p3x;
            my_Points[2].Y = p3y;
            my_Points[2].z = p3z;

            my_Points[3].x = p4x;
            my_Points[3].Y = p4y;
            my_Points[3].z = p4z;

            my_Points[4].x = p5x;
            my_Points[4].Y = p5y;
            my_Points[4].z = p5z;

            my_Points[5].x = p6x;
            my_Points[5].Y = p6y;
            my_Points[5].z = p6z;

            my_Points[6].x = p7x;
            my_Points[6].Y = p7y;
            my_Points[6].z = p7z;

            my_Points[7].x = p8x;
            my_Points[7].Y = p8y;
            my_Points[7].z = p8z;

            my_Points[8].x = p9x;
            my_Points[8].Y = p9y;
            my_Points[8].z = p9z;

            my_Points[9].x = pax;
            my_Points[9].Y = pay;
            my_Points[9].z = paz;


            draw_fixedStars();

            for (i = 0; i < NUMREFERENCE; i++)
            {
                this.Circle(((float)ct_Points[i].x) / 15, ((float)ct_Points[i].Y) / 15, 7, Color.Blue);
            }

        }


        #endregion

        // Routine to re-draw the catalog stars (Green circles)

        private void draw_fixedStars()
        {

            int i = 0;

            // Draw the green circles

            for (i = 0; i < NUMREFERENCE; i++)
            {
                this.Circle(((float)ct_Points[i].x) / 15, ((float)ct_Points[i].Y) / 15, 7, Color.Lime);
            }

        }

        // Routine to re-draw the measured stars (Blue circles)

        private void draw_measuredStars(Color color)
        {

            int i = 0;

            // Draw the measured star location

            for (i = 0; i < NUMREFERENCE; i++)
            {
                this.Circle(((float)my_Points[i].x) / 15, ((float)my_Points[i].Y) / 15, 7, color);
            }

        }

        // Routine to draw the object stars (white circles)

        private void draw_mystars(int x, int y)
        {

            int[] datholder = new int[10];
            int[] dotidholder = new int[10];
            int i = 0;

            // Get the neareast reference dot

            for (i = 0; i < NUMREFERENCE; i++)
            {

                // Distance is simply computed as total x-y distance

                datholder[i] = Convert.ToInt32(Math.Abs(ct_Points[i].x - x) + Math.Abs(ct_Points[i].Y - y));

                // Also save the star ID for sorting purposes

                dotidholder[i] = i;
            }

            // Sort the distances and choose the nearest 3 stars as the reference star for this
            // star object

            //Quick sort main list, sublist, location of 1st data, location of last data

            Quicksort(datholder, dotidholder, 0, NUMREFERENCE - 1);


            //Clear the area first before redrawing

            this.Circle(((float)my_Objects[numpoint].x) / 15, ((float)my_Objects[numpoint].Y) / 15, 3, Color.Black);

            my_Objects[numpoint].x = x;
            my_Objects[numpoint].Y = y;

            // Choose the nearest 3 reference stars as the matrix transformation stars for this object star
            // Since the list is sorted, choose the 1st three locations

            my_ObjectsXformStars[numpoint].d1 = dotidholder[0];
            my_ObjectsXformStars[numpoint].d2 = dotidholder[1];
            my_ObjectsXformStars[numpoint].d3 = dotidholder[2];

            // mark the location of this object star with a white circle

            this.Circle(((float)my_Objects[numpoint].x) / 15, ((float)my_Objects[numpoint].Y) / 15, 3, Color.White);


            //Point to the next available dot

            numpoint++;
            if (numpoint > TOTPOINTS)
            {
                numpoint = 1;
            }

        }

        // Routine to redraw the Object Stars (white circles)
        // This is needed only for this demo as the circles get erased

        private void redraw_mystars()
        {
            int i = 0;
            Coord obj = new Coord();

            // Attempt to reverse the conversion using the measured stars as anchors
            // This routine will actually demo and confirm if its possible to compute
            // for the original coordinate using the transformed data.

            // This is not needed on actual implementation, this only tests if the
            // reverse process will still be accurate.
            // Reverse function works if the WHITE dots are not moving
            // If there is movement on any of the white dots, then that means there was
            // a failure somewhere in the transformation

            for (i = 0; i < TOTPOINTS; i++)
            {

                if ((my_xformObjects_Taki[i].x + my_xformObjects_Taki[i].Y) != 0)
                {

                    // Compute for the REVERSE transformation matrix for this particular Object star

                    EQTakiAffine.EQ_AssembleMatrix_Taki(0, 0, my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3], ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3]);

                    // RE-Transform using the TAKI transformed coordinates

                    obj = EQTakiAffine.EQ_Transform_Taki(my_xformObjects_Taki[i]);

                    // Draw the white circles. If there is no movement then the
                    // reverse transformation is very accurate.

                    this.Circle(((float)obj.x) / 15, ((float)obj.Y) / 15, 3, Color.White);

                }

            }


            // Do the same thing for the Affine Method


            for (i = 0; i < TOTPOINTS; i++)
            {

                if ((my_xformObjects_Affine[i].x + my_xformObjects_Affine[i].Y) != 0)
                {

                    // Compute for the REVERSE transformation matrix for this particular Object star

                    EQTakiAffine.EQ_AssembleMatrix_Affine(0, 0, my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3], ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3]);


                    // RE-Transform using the Affined transformed coordinates

                    obj = EQTakiAffine.EQ_Transform_Affine(my_xformObjects_Affine[i]);

                    // Draw the white circles. If there is no movement then the
                    // reverse transformation is very accurate.

                    this.Circle(((float)obj.x) / 15, ((float)obj.Y) / 15, 1, Color.White);


                }

            }


        }

        // Routine to redraw the measured stars (blue circles) as they are being adjusted by the user

        private void draw_bluedot(int x, int y)
        {

            int[] datholder = new int[10];
            int[] dotidholder = new int[10];
            int i = 0;



            // Get the neareast reference dot pointed by the mouse cursor

            for (i = 0; i < NUMREFERENCE; i++)
            {

                // Compute for total X-Y distance.
                datholder[i] = Convert.ToInt32(Math.Abs(my_Points[i].x - x) + Math.Abs(my_Points[i].Y - y));

                // Also save the reference star id for this particular reference star

                dotidholder[i] = i;

            }

            // Sort the distance list

            //Quicksort main list, sub list, location of 1st data, location of last data

            Quicksort(datholder, dotidholder, 0, NUMREFERENCE - 1);

            // Choose the nearest star (found at the 1st entry of the sorted list)

            int dotid = dotidholder[0];

            // Delete the old Dot

            this.Circle(((float)my_Points[dotid].x) / 15, ((float)my_Points[dotid].Y) / 15, 7, Color.Black);

            // Update the coordinate data of the new dot

            my_Points[dotid].x = x;
            my_Points[dotid].Y = y;

            //Draw the dot

            this.Circle(((float)my_Points[dotid].x) / 15, ((float)my_Points[dotid].Y) / 15, 7, Color.Blue);
        }

        // Routine to redraw the taki dots (red circles) as the measured stars are being moved

        private void adjust_reddots(Color color)
        {
            int i = 0;
            Coord obj2 = new Coord();

            for (i = 0; i < TOTPOINTS; i++)
            {

                if ((my_Objects[i].x + my_Objects[i].Y) != 0)
                {

                    // Compute for transformation matrix for this particular Object.
                    // Chose the ASSIGNED nearest 3 reference stars for the transformation

                    EQTakiAffine.EQ_AssembleMatrix_Taki(0, 0, ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3], my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3]);

                    // Compute for the new coordinate using the Transformation Matrix

                    obj2 = EQTakiAffine.EQ_Transform_Taki(my_Objects[i]);

                    this.Circle(((float)obj2.x) / 15, ((float)obj2.Y) / 15, 3, color);

                    my_xformObjects_Taki[i].x = obj2.x;
                    my_xformObjects_Taki[i].Y = obj2.Y;

                }

            }

        }

        // Routine to redraw the Affine dots (yellow circles) as the measured stars are being moved


        private void adjust_yellowdots(Color color)
        {
            int i = 0;
            Coord obj2 = new Coord();


            for (i = 0; i < TOTPOINTS; i++)
            {

                if ((my_Objects[i].x + my_Objects[i].Y) != 0)
                {


                    // Compute for transformation matrix for this particular Object.
                    // Chose the ASSIGNED nearest 3 reference stars for the transformation

                    EQTakiAffine.EQ_AssembleMatrix_Affine(0, 0, ct_Points[my_ObjectsXformStars[i].d1], ct_Points[my_ObjectsXformStars[i].d2], ct_Points[my_ObjectsXformStars[i].d3], my_Points[my_ObjectsXformStars[i].d1], my_Points[my_ObjectsXformStars[i].d2], my_Points[my_ObjectsXformStars[i].d3]);

                    // Compute for the new coordinate using the Affine Transformation Matrix

                    obj2 = EQTakiAffine.EQ_Transform_Affine(my_Objects[i]);

                    this.Circle(((float)obj2.x) / 15, ((float)obj2.Y) / 15, 1, color);

                    my_xformObjects_Affine[i].x = obj2.x;
                    my_xformObjects_Affine[i].Y = obj2.Y;


                }

            }

        }

        // Routine to initialize the display

        // Routine to implement a quicksort. This is used to determine the nearest star

        private void Quicksort(int[] List, int[] Sublist, int min, int max)
        {


            if (min >= max)
            {
                return;
            }
            int i = Convert.ToInt32(Math.Floor((double)((max - min + 1) * EQTakiAffine.GetRnd() + min)));
            int med_value = List[i];
            int submed = Sublist[i];

            List[i] = List[min];
            Sublist[i] = Sublist[min];

            int lo = min;
            int hi = max;
            do
            {


                while (List[hi] >= med_value)
                {
                    hi--;
                    if (hi < lo)
                    {
                        break;
                    }
                };
                if (hi < lo)
                {
                    List[lo] = med_value;
                    Sublist[lo] = submed;
                    break;
                }


                List[lo] = List[hi];
                Sublist[lo] = Sublist[hi];

                lo++;

                while (List[lo] < med_value)
                {
                    lo++;
                    if (lo >= hi)
                    {
                        break;
                    }
                };
                if (lo >= hi)
                {
                    lo = hi;
                    List[hi] = med_value;
                    Sublist[hi] = submed;
                    break;
                }

                List[hi] = List[lo];
                Sublist[hi] = Sublist[lo];
            }
            while (true);

            Quicksort(List, Sublist, min, lo - 1);
            Quicksort(List, Sublist, lo + 1, max);
        }


    }
}
