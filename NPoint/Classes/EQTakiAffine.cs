//---------------------------------------------------------------------
// Copyright � 2020 Phil Crompton
// based on original work
// Copyright � 2006 Raymund Sarmiento
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
//
// EQMODVECTOR.BAS - Matrix Transformation Routines for 3-Star Alignment
// (Renamed EQTakiAffine.cs)
// Written:  10-Dec-06   Raymund Sarmiento
//
// Edits:
//
// When      Who     What
// --------- ---     --------------------------------------------------
// 10-Dec-06 rcs     Initial edit for EQ Mount 3-Star Matrix Transformation
// 14-Dec-06 rcs     Added Taki Method on top of Affine Mapping Method for Comparison
//                   Taki Routines based on John Archbold's Excel computation
// 08-Apr-07 rcs     N-star implementation
// 13-Jun-20 jpc	 Copied into NPoint code base and converted from vb6 to C#
//---------------------------------------------------------------------
//
//

//  DISCLAIMER:

//  You can use the information on this site COMPLETELY AT YOUR OWN RISK.
//  The modification steps and other information on this site is provided
//  to you "AS IS" and WITHOUT WARRANTY OF ANY KIND, express, statutory,
//  implied or otherwise, including without limitation any warranty of
//  merchantability or fitness for any particular or intended purpose.
//  In no event the author will  be liable for any direct, indirect,
//  punitive, special, incidental or consequential damages or loss of any
//  kind whether or not the author  has been advised of the possibility
//  of such loss.

//  WARNING:

//  Circuit modifications implemented on your setup could invalidate
//  any warranty that you may have with your product. Use this
//  information at your own risk. The modifications involve direct
//  access to the stepper motor controls of your mount. Any "mis-control"
//  or "mis-command"  / "invalid parameter" or "garbage" data sent to the
//  mount could accidentally activate the stepper motors and allow it to
//  rotate "freely" damaging any equipment connected to your mount.
//  It is also possible that any garbage or invalid data sent to the mount
//  could cause its firmware to generate mis-steps pulse sequences to the
//  motors causing it to overheat. Make sure that you perform the
//  modifications and testing while there is no physical "load" or
//  dangling wires on your mount. Be sure to disconnect the power once
//  this event happens or if you notice any unusual sound coming from
//  the motor assembly.
//

using System;

namespace NPoint
{
    internal static class EQTakiAffine
    {

        //Define Affine Matrix

        public static Matrix EQMP = Matrix.CreateInstance();
        public static Matrix EQMQ = Matrix.CreateInstance();

        public static Matrix EQMI = Matrix.CreateInstance();
        public static Matrix EQMM = Matrix.CreateInstance();
        public static Coord EQCO = new Coord();


        //Define Taki Matrix

        public static Matrix EQLMN1 = Matrix.CreateInstance();
        public static Matrix EQLMN2 = Matrix.CreateInstance();

        public static Matrix EQMI_T = Matrix.CreateInstance();
        public static Matrix EQMT = Matrix.CreateInstance();
        public static Coord EQCT = new Coord();




        //Function to put coordinate values into a LMN/lmn matrix array

        internal static Matrix GETLMN(Coord p1, Coord p2, Coord p3)
        {

            Matrix temp = Matrix.CreateInstance();
            Matrix UnitVect = Matrix.CreateInstance();


            temp.Element[0, 0] = p2.x - p1.x;
            temp.Element[1, 0] = p3.x - p1.x;

            temp.Element[0, 1] = p2.Y - p1.Y;
            temp.Element[1, 1] = p3.Y - p1.Y;

            temp.Element[0, 2] = p2.z - p1.z;
            temp.Element[1, 2] = p3.z - p1.z;




            UnitVect.Element[0, 0] = (temp.Element[0, 1] * temp.Element[1, 2]) - (temp.Element[0, 2] * temp.Element[1, 1]);
            UnitVect.Element[0, 1] = (temp.Element[0, 2] * temp.Element[1, 0]) - (temp.Element[0, 0] * temp.Element[1, 2]);
            UnitVect.Element[0, 2] = (temp.Element[0, 0] * temp.Element[1, 1]) - (temp.Element[0, 1] * temp.Element[1, 0]);
            UnitVect.Element[1, 0] = Math.Pow(UnitVect.Element[0, 0], 2) + Math.Pow(UnitVect.Element[0, 1], 2) + Math.Pow(UnitVect.Element[0, 2], 2);
            UnitVect.Element[1, 1] = Math.Sqrt(UnitVect.Element[1, 0]);
            if (UnitVect.Element[1, 1] != 0)
            {
                UnitVect.Element[1, 2] = 1 / UnitVect.Element[1, 1];
            }



            temp.Element[2, 0] = UnitVect.Element[1, 2] * UnitVect.Element[0, 0];
            temp.Element[2, 1] = UnitVect.Element[1, 2] * UnitVect.Element[0, 1];
            temp.Element[2, 2] = UnitVect.Element[1, 2] * UnitVect.Element[0, 2];




            return temp;

        }

        //Function to put coordinate values into a P/Q Affine matrix array

        internal static Matrix GETPQ(Coord p1, Coord p2, Coord p3)
        {

            Matrix temp = Matrix.CreateInstance();

            temp.Element[0, 0] = p2.x - p1.x;
            temp.Element[1, 0] = p3.x - p1.x;
            temp.Element[0, 1] = p2.Y - p1.Y;
            temp.Element[1, 1] = p3.Y - p1.Y;

            return temp;

        }

        // Subroutine to draw the Transformation Matrix (Taki Method)

        internal static int EQ_AssembleMatrix_Taki(double x, double Y, Coord a1, Coord a2, Coord a3, Coord m1, Coord m2, Coord m3)
        {


            double Det = 0;


            // Get the LMN Matrix

            EQLMN1 = GETLMN(a1, a2, a3);

            // Get the lmn Matrix

            EQLMN2 = GETLMN(m1, m2, m3);




            // Get the Determinant

            Det = EQLMN1.Element[0, 0] * ((EQLMN1.Element[1, 1] * EQLMN1.Element[2, 2]) - (EQLMN1.Element[2, 1] * EQLMN1.Element[1, 2]));
            Det -= (EQLMN1.Element[0, 1] * ((EQLMN1.Element[1, 0] * EQLMN1.Element[2, 2]) - (EQLMN1.Element[2, 0] * EQLMN1.Element[1, 2])));
            Det += (EQLMN1.Element[0, 2] * ((EQLMN1.Element[1, 0] * EQLMN1.Element[2, 1]) - (EQLMN1.Element[2, 0] * EQLMN1.Element[1, 1])));


            // Compute for the Matrix Inverse of EQLMN1

            if (Det == 0)
            {
                throw new System.Exception("999, AssembleMatrix, Cannot invert matrix with Determinant = 0");
            }
            else
            {

                EQMI_T.Element[0, 0] = ((EQLMN1.Element[1, 1] * EQLMN1.Element[2, 2]) - (EQLMN1.Element[2, 1] * EQLMN1.Element[1, 2])) / Det;
                EQMI_T.Element[0, 1] = ((EQLMN1.Element[0, 2] * EQLMN1.Element[2, 1]) - (EQLMN1.Element[0, 1] * EQLMN1.Element[2, 2])) / Det;
                EQMI_T.Element[0, 2] = ((EQLMN1.Element[0, 1] * EQLMN1.Element[1, 2]) - (EQLMN1.Element[1, 1] * EQLMN1.Element[0, 2])) / Det;
                EQMI_T.Element[1, 0] = ((EQLMN1.Element[1, 2] * EQLMN1.Element[2, 0]) - (EQLMN1.Element[2, 2] * EQLMN1.Element[1, 0])) / Det;
                EQMI_T.Element[1, 1] = ((EQLMN1.Element[0, 0] * EQLMN1.Element[2, 2]) - (EQLMN1.Element[2, 0] * EQLMN1.Element[0, 2])) / Det;
                EQMI_T.Element[1, 2] = ((EQLMN1.Element[0, 2] * EQLMN1.Element[1, 0]) - (EQLMN1.Element[1, 2] * EQLMN1.Element[0, 0])) / Det;
                EQMI_T.Element[2, 0] = ((EQLMN1.Element[1, 0] * EQLMN1.Element[2, 1]) - (EQLMN1.Element[2, 0] * EQLMN1.Element[1, 1])) / Det;
                EQMI_T.Element[2, 1] = ((EQLMN1.Element[0, 1] * EQLMN1.Element[2, 0]) - (EQLMN1.Element[2, 1] * EQLMN1.Element[0, 0])) / Det;
                EQMI_T.Element[2, 2] = ((EQLMN1.Element[0, 0] * EQLMN1.Element[1, 1]) - (EQLMN1.Element[1, 0] * EQLMN1.Element[0, 1])) / Det;
            }



            // Get the M Matrix by Multiplying EQMI and EQLMN2
            // EQMI_T - Matrix A
            // EQLMN2 - Matrix B


            EQMT.Element[0, 0] = (EQMI_T.Element[0, 0] * EQLMN2.Element[0, 0]) + (EQMI_T.Element[0, 1] * EQLMN2.Element[1, 0]) + (EQMI_T.Element[0, 2] * EQLMN2.Element[2, 0]);
            EQMT.Element[0, 1] = (EQMI_T.Element[0, 0] * EQLMN2.Element[0, 1]) + (EQMI_T.Element[0, 1] * EQLMN2.Element[1, 1]) + (EQMI_T.Element[0, 2] * EQLMN2.Element[2, 1]);
            EQMT.Element[0, 2] = (EQMI_T.Element[0, 0] * EQLMN2.Element[0, 2]) + (EQMI_T.Element[0, 1] * EQLMN2.Element[1, 2]) + (EQMI_T.Element[0, 2] * EQLMN2.Element[2, 2]);

            EQMT.Element[1, 0] = (EQMI_T.Element[1, 0] * EQLMN2.Element[0, 0]) + (EQMI_T.Element[1, 1] * EQLMN2.Element[1, 0]) + (EQMI_T.Element[1, 2] * EQLMN2.Element[2, 0]);
            EQMT.Element[1, 1] = (EQMI_T.Element[1, 0] * EQLMN2.Element[0, 1]) + (EQMI_T.Element[1, 1] * EQLMN2.Element[1, 1]) + (EQMI_T.Element[1, 2] * EQLMN2.Element[2, 1]);
            EQMT.Element[1, 2] = (EQMI_T.Element[1, 0] * EQLMN2.Element[0, 2]) + (EQMI_T.Element[1, 1] * EQLMN2.Element[1, 2]) + (EQMI_T.Element[1, 2] * EQLMN2.Element[2, 2]);

            EQMT.Element[2, 0] = (EQMI_T.Element[2, 0] * EQLMN2.Element[0, 0]) + (EQMI_T.Element[2, 1] * EQLMN2.Element[1, 0]) + (EQMI_T.Element[2, 2] * EQLMN2.Element[2, 0]);
            EQMT.Element[2, 1] = (EQMI_T.Element[2, 0] * EQLMN2.Element[0, 1]) + (EQMI_T.Element[2, 1] * EQLMN2.Element[1, 1]) + (EQMI_T.Element[2, 2] * EQLMN2.Element[2, 1]);
            EQMT.Element[2, 2] = (EQMI_T.Element[2, 0] * EQLMN2.Element[0, 2]) + (EQMI_T.Element[2, 1] * EQLMN2.Element[1, 2]) + (EQMI_T.Element[2, 2] * EQLMN2.Element[2, 2]);


            // Get the Coordinate Offset Vector and store it at EQCO Matrix

            EQCT.x = m1.x - ((a1.x * EQMT.Element[0, 0]) + (a1.Y * EQMT.Element[1, 0]) + (a1.z * EQMT.Element[2, 0]));
            EQCT.Y = m1.Y - ((a1.x * EQMT.Element[0, 1]) + (a1.Y * EQMT.Element[1, 1]) + (a1.z * EQMT.Element[2, 1]));
            EQCT.z = m1.z - ((a1.x * EQMT.Element[0, 2]) + (a1.Y * EQMT.Element[1, 2]) + (a1.z * EQMT.Element[2, 2]));


            if ((x + Y) == 0)
            {
                return 0;
            }
            else
            {
                return EQ_CheckPoint_in_Triangle(x, Y, a1.x, a1.Y, a2.x, a2.Y, a3.x, a3.Y);
            }


        }


        //Function to transform the Coordinates (Taki Method)  using the MT Matrix and Offset Vector

        internal static Coord EQ_Transform_Taki(Coord ob)
        {

            // CoordTransform = Offset + CoordObject * Matrix MT

            Coord result = new Coord();
            result.x = EQCT.x + ((ob.x * EQMT.Element[0, 0]) + (ob.Y * EQMT.Element[1, 0]) + (ob.z * EQMT.Element[2, 0]));
            result.Y = EQCT.Y + ((ob.x * EQMT.Element[0, 1]) + (ob.Y * EQMT.Element[1, 1]) + (ob.z * EQMT.Element[2, 1]));
            result.z = EQCT.z + ((ob.x * EQMT.Element[0, 2]) + (ob.Y * EQMT.Element[1, 2]) + (ob.z * EQMT.Element[2, 2]));


            return result;
        }

        // Subroutine to draw the Transformation Matrix (Affine Mapping Method)

        internal static int EQ_AssembleMatrix_Affine(double x, double Y, Coord a1, Coord a2, Coord a3, Coord m1, Coord m2, Coord m3)
        {

            double Det = 0;

            // Get the P Matrix
            EQMP = GETPQ(a1, a2, a3);

            // Get the Q Matrix
            EQMQ = GETPQ(m1, m2, m3);

            // Get the Inverse of P
            // Get the EQMP Determinant for Inverse Computation
            Det = (EQMP.Element[0, 0] * EQMP.Element[1, 1]) - (EQMP.Element[0, 1] * EQMP.Element[1, 0]);

            // Make sure Determinant is NON ZERO
            if (Det == 0)
            {
                throw new System.Exception("999, AssembleMatrix, Cannot invert matrix with Determinant = 0");
            }
            else
            {
                //Perform the Matrix Inversion, put result to EQMI matrix
                EQMI.Element[0, 0] = (EQMP.Element[1, 1]) / Det;
                EQMI.Element[0, 1] = (-EQMP.Element[0, 1]) / Det;
                EQMI.Element[1, 0] = (-EQMP.Element[1, 0]) / Det;
                EQMI.Element[1, 1] = (EQMP.Element[0, 0]) / Det;
            }

            // Get the M Matrix by Multiplying EQMI and EQMQ
            // EQMI - Matrix A
            // EQMQ - Matrix B
            EQMM.Element[0, 0] = (EQMI.Element[0, 0] * EQMQ.Element[0, 0]) + (EQMI.Element[0, 1] * EQMQ.Element[1, 0]);
            EQMM.Element[0, 1] = (EQMI.Element[0, 0] * EQMQ.Element[0, 1]) + (EQMI.Element[0, 1] * EQMQ.Element[1, 1]);
            EQMM.Element[1, 0] = (EQMI.Element[1, 0] * EQMQ.Element[0, 0]) + (EQMI.Element[1, 1] * EQMQ.Element[1, 0]);
            EQMM.Element[1, 1] = (EQMI.Element[1, 0] * EQMQ.Element[0, 1]) + (EQMI.Element[1, 1] * EQMQ.Element[1, 1]);

            // Get the Coordinate Offset Vector and store it at EQCO Matrix
            EQCO.x = m1.x - ((a1.x * EQMM.Element[0, 0]) + (a1.Y * EQMM.Element[1, 0]));
            EQCO.Y = m1.Y - ((a1.x * EQMM.Element[0, 1]) + (a1.Y * EQMM.Element[1, 1]));

            if ((x + Y) == 0)
            {
                return 0;
            }
            else
            {
                return EQ_CheckPoint_in_Triangle(x, Y, m1.x, m1.Y, m2.x, m2.Y, m3.x, m3.Y);
            }

        }


        //Function to transform the Coordinates (Affine Mapping) using the M Matrix and Offset Vector

        internal static Coord EQ_Transform_Affine(Coord ob)
        {

            // CoordTransform = Offset + CoordObject * Matrix M

            Coord result = new Coord();
            result.x = EQCO.x + ((ob.x * EQMM.Element[0, 0]) + (ob.Y * EQMM.Element[1, 0]));
            result.Y = EQCO.Y + ((ob.x * EQMM.Element[0, 1]) + (ob.Y * EQMM.Element[1, 1]));

            return result;
        }

        //Function to convert spherical coordinates to Cartesian using the Coord structure

        internal static Coord EQ_sp2Cs(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj = new CartesCoord();
            SphereCoord tmpobj4 = new SphereCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj4 = EQ_SphericalPolar(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude);
                tmpobj = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);
                result.x = tmpobj.x;
                result.Y = tmpobj.Y;
                result.z = 1;
            }
            else
            {
                result.x = obj.x;
                result.Y = obj.Y;
                result.z = 1;
            }

            return result;
        }


        //Function to convert spherical coordinates to Cartesian using the Coord structure

        internal static Coord EQ_sp2Cs2(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj = new CartesCoord();
            SphereCoord tmpobj4 = new SphereCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj4 = EQ_SphericalPolar(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, Math.Abs(gLatitude));
                tmpobj = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);
                result.x = tmpobj.x;
                result.Y = tmpobj.Y;
                result.z = 1;
            }
            else
            {
                result.x = obj.x;
                result.Y = obj.Y;
                result.z = 1;
            }

            return result;
        }


        //Function to convert polar coordinates to Cartesian using the Coord structure


        internal static Coord EQ_pl2Cs(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj = new CartesCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj = EQ_Polar2Cartes(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                result.x = tmpobj.x;
                result.Y = tmpobj.Y;
                result.z = 1;
            }
            else
            {
                result.x = obj.x;
                result.Y = obj.Y;
                result.z = 1;
            }

            return result;
        }

        //Implement an Affine transformation on a Polar coordinate system
        //This is done by converting the Polar Data to Cartesian, Apply affine transformation
        //Then restore the transformed Cartesian Coordinates back to polar


        internal static Coord EQ_plAffine(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj1 = new CartesCoord();
            Coord tmpobj2 = new Coord();
            Coord tmpobj3 = new Coord();
            SphereCoord tmpobj4 = new SphereCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj4 = EQ_SphericalPolar(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude);

                tmpobj1 = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                tmpobj2.x = tmpobj1.x;
                tmpobj2.Y = tmpobj1.Y;
                tmpobj2.z = 1;

                tmpobj3 = EQ_Transform_Affine(tmpobj2);

                tmpobj2 = EQ_Cartes2Polar(tmpobj3.x, tmpobj3.Y, tmpobj1.r, tmpobj1.RA, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                result = EQ_PolarSpherical(tmpobj2.x, tmpobj2.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude, tmpobj4.r);

            }
            else
            {
                tmpobj3 = EQ_Transform_Affine(obj);
                result.x = tmpobj3.x;
                result.Y = tmpobj3.Y;
                result.z = 1;
            }

            return result;
        }


        internal static Coord EQ_plAffine2(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj1 = new CartesCoord();
            Coord tmpobj2 = new Coord();
            Coord tmpobj3 = new Coord();
            SphereCoord tmpobj4 = new SphereCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj4 = EQ_SphericalPolar(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude);

                tmpobj1 = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                tmpobj2.x = tmpobj1.x;
                tmpobj2.Y = tmpobj1.Y;
                tmpobj2.z = 1;

                tmpobj3 = EQ_Transform_Affine(tmpobj2);

                tmpobj2 = EQ_Cartes2Polar(tmpobj3.x, tmpobj3.Y, tmpobj1.r, tmpobj1.RA, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);


                result = EQ_PolarSpherical(tmpobj2.x, tmpobj2.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude, tmpobj4.r);

            }
            else
            {
                tmpobj3 = EQ_Transform_Affine(obj);
                result.x = tmpobj3.x;
                result.Y = tmpobj3.Y;
                result.z = 1;
            }

            return result;
        }
        //Implement a TAKI transformation on a Polar coordinate system
        //This is done by converting the Polar Data to Cartesian, Apply TAKI transformation
        //Then restore the transformed Cartesian Coordinates back to polar

        internal static Coord EQ_plTaki(Coord obj)
        {
            Coord result = new Coord();
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            object HC = null;
            double RAEncoder_Home_pos = 0;

            CartesCoord tmpobj1 = new CartesCoord();
            Coord tmpobj2 = new Coord();
            Coord tmpobj3 = new Coord();
            SphereCoord tmpobj4 = new SphereCoord();

            if (EQASCOM.POLAR_ENABLE)
            {
                tmpobj4 = EQ_SphericalPolar(obj.x, obj.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude);
                tmpobj1 = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                tmpobj2.x = tmpobj1.x;
                tmpobj2.Y = tmpobj1.Y;
                tmpobj2.z = 1;

                tmpobj3 = EQ_Transform_Taki(tmpobj2);

                tmpobj2 = EQ_Cartes2Polar(tmpobj3.x, tmpobj3.Y, tmpobj1.r, tmpobj1.RA, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos);

                result = EQ_PolarSpherical(tmpobj2.x, tmpobj2.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude, tmpobj4.r);

            }
            else
            {
                tmpobj3 = EQ_Transform_Taki(obj);
                result.x = tmpobj3.x;
                result.Y = tmpobj3.Y;
                result.z = 1;
            }

            return result;
        }

        // Function to Convert Polar RA/DEC Stepper coordinates to Cartesian Coordinates

        internal static CartesCoord EQ_Polar2Cartes(double RA, double DEC, double TOT, double RACENTER, double DECCENTER)
        {
            CartesCoord result = new CartesCoord();
            double DEG_RAD = 0;
            double[] Range360 = null;




            double i = 0;



            // make angle stays within the 360 bound

            if (RA > RACENTER)
            {
                i = ((RA - RACENTER) / TOT) * 360;
            }
            else
            {
                i = ((RACENTER - RA) / TOT) * 360;
                i = 360 - i;
            }

            double theta = Range360[Convert.ToInt32(i)] * DEG_RAD;

            //treat y as the radius of the polar coordinate

            double radius = DEC - DECCENTER;

            double radpeak = 0;

            //  Removed

            //  If Abs(radius) > DECPEAK Then
            //      radpeak = radius
            //      If radius > 0 Then
            //          radius = (2 * DECPEAK) - radius
            //      Else
            //          radius = ((2 * DECPEAK) + radius) * -1
            //      End If
            //      radpeak = radpeak - radius
            //  End If


            // Avoid division 0 errors

            if (radius == 0)
            {
                radius = 1;
            }

            // Get the cartesian coordinates

            result.x = Math.Cos(theta) * radius;
            result.Y = Math.Sin(theta) * radius;
            result.RA = radpeak;

            // if radius is a negative number, pass this info on the next conversion routine

            if (radius > 0)
            {
                result.r = 1;
            }
            else
            {
                result.r = -1;
            }

            return result;
        }

        //Function to convert the Cartesian Coordinate data back to RA/DEC polar

        internal static Coord EQ_Cartes2Polar(double x, double Y, double r, double RA, double TOT, double RACENTER, double DECCENTER)
        {
            Coord result = new Coord();
            double[] Range360 = null;
            double PI = 0;
            double RAD_DEG = 0;


            // Ah the famous radius formula

            double radiusder = Math.Sqrt((x * x) + (Y * Y)) * r;


            // And the nasty angle compute routine (any simpler way to impelent this ?)

            double angle = 0;
            if (x > 0)
            {
                angle = Math.Atan(Y / x);
            }
            if (x < 0)
            {
                if (Y >= 0)
                {
                    angle = Math.Atan(Y / x) + PI;

                }
                else
                {
                    angle = Math.Atan(Y / x) - PI;
                }
            }
            if (x == 0)
            {
                if (Y > 0)
                {
                    angle = PI / 2d;
                }
                else
                {
                    angle = -1 * (PI / 2d);
                }
            }

            // Convert angle to degrees

            angle *= RAD_DEG;

            if (angle < 0)
            {
                angle = 360 + angle;
            }

            if (r < 0)
            {
                angle = Range360[Convert.ToInt32(angle + 180)];
            }

            if (angle > 180)
            {
                result.x = RACENTER - (((360 - angle) / 360d) * TOT);
            }
            else
            {
                result.x = ((angle / 360d) * TOT) + RACENTER;
            }

            //treat y as the polar coordinate radius (ra var not used - always 0)

            result.Y = radiusder + DECCENTER + RA;

            return result;
        }

        internal static int EQ_UpdateTaki(double x, double Y)
        {
            int g3PointAlgorithm = 0;
            int gAlignmentStars_count = 0;

            TriangleCoord tr = new TriangleCoord();
            Coord tmpcoord = new Coord();

            // Adjust only if there are four alignment stars
            if (gAlignmentStars_count < 3)
            {
                return 0;
            }


            switch (g3PointAlgorithm)
            {
                case 1:
                    // find the 50 nearest points - then find the nearest enclosing triangle 
                    tr = EQ_ChooseNearest3Points(x, Y);
                    break;
                default:
                    // find the 50 nearest points - then find the enclosing triangle with the nearest centre point 
                    tr = EQ_Choose_3Points(x, Y);
                    break;
            }

            double gTaki1 = tr.i;
            double gTaki2 = tr.j;
            double gTaki3 = tr.k;

            if (gTaki1 == 0 || gTaki2 == 0 || gTaki3 == 0)
            {
                return 0;
            }

            tmpcoord.x = x;
            tmpcoord.Y = Y;
            tmpcoord = EQ_sp2Cs(tmpcoord);
            return EQ_AssembleMatrix_Taki(tmpcoord.x, tmpcoord.Y, EQASCOM.ct_PointsC[Convert.ToInt32(gTaki1)], EQASCOM.ct_PointsC[Convert.ToInt32(gTaki2)], EQASCOM.ct_PointsC[Convert.ToInt32(gTaki3)], EQASCOM.my_PointsC[Convert.ToInt32(gTaki1)], EQASCOM.my_PointsC[Convert.ToInt32(gTaki2)], EQASCOM.my_PointsC[Convert.ToInt32(gTaki3)]);

        }

        internal static int EQ_UpdateAffine(double x, double Y)
        {
            int result = 0;
            int g3PointAlgorithm = 0;
            int gAlignmentStars_count = 0;

            Coord tmpcoord = new Coord();
            TriangleCoord tr = new TriangleCoord();

            if (gAlignmentStars_count < 3)
            {
                return result;
            }

            switch (g3PointAlgorithm)
            {
                case 1:
                    // find the 50 nearest points - then find the nearest enclosing triangle 
                    tr = EQ_ChooseNearest3Points(x, Y);
                    break;
                default:
                    // find the 50 nearest points - then find the enclosing triangle with the nearest centre point 
                    tr = EQ_Choose_3Points(x, Y);
                    break;
            }

            double gAffine1 = tr.i;
            double gAffine2 = tr.j;
            double gAffine3 = tr.k;

            if (gAffine1 == 0 || gAffine1 == 0 || gAffine1 == 0)
            {
                return 0;
            }

            tmpcoord.x = x;
            tmpcoord.Y = Y;
            tmpcoord = EQ_sp2Cs(tmpcoord);

            result = EQ_AssembleMatrix_Affine(tmpcoord.x, tmpcoord.Y, EQASCOM.my_PointsC[Convert.ToInt32(gAffine1)], EQASCOM.my_PointsC[Convert.ToInt32(gAffine2)], EQASCOM.my_PointsC[Convert.ToInt32(gAffine3)], EQASCOM.ct_PointsC[Convert.ToInt32(gAffine1)], EQASCOM.ct_PointsC[Convert.ToInt32(gAffine2)], EQASCOM.ct_PointsC[Convert.ToInt32(gAffine3)]);

            if (result == 0)
            {
                gAffine1 = 0;
                gAffine2 = 0;
                gAffine3 = 0;
            }

            return result;
        }

        // Subroutine to implement find Array index with the lowest value
        internal static int EQ_FindLowest(double[] List, int min, int max)
        {
            double val = 0;
            double newval = 0;
            int i = 0;

            int idx = -1;
            int tempForEndVar = 0;
            if (!(min >= max || max > List.GetUpperBound(0)))
            {

                val = List[min];
                tempForEndVar = max;
                for (i = min; i <= tempForEndVar; i++)
                {
                    newval = List[i];
                    if (newval <= val)
                    {
                        val = newval;
                        idx = i;
                    }
                }

            }

            return idx;
        }

        internal static void EQ_FindLowest3(double[] List, int[] Sublist, int min, int max)
        {
            double val = 0;
            double min1 = 0;
            double min2 = 0;
            double min3 = 0;
            int i = 0;

            int tempForEndVar = 0;
            if (!(min >= max || max > List.GetUpperBound(0)))
            {

                if (List[1] <= List[2] && List[1] <= List[3])
                {
                    //List 1 is first
                    min1 = List[1];
                    if (List[2] <= List[3])
                    {
                        //List2 is second
                        //List3 is third
                        min2 = List[2];
                        min3 = List[3];
                    }
                    else
                    {
                        //List3 is second
                        //List2 is third
                        min2 = List[3];
                        min3 = List[2];
                    }
                }
                else
                {
                    if (List[2] <= List[1] && List[2] <= List[3])
                    {
                        //List 2 is first
                        min1 = List[2];
                        if (List[1] <= List[3])
                        {
                            //List1 is second
                            //List3 is third
                            min2 = List[1];
                            min3 = List[3];
                        }
                        else
                        {
                            //List3 is second
                            //List1 is third
                            min2 = List[3];
                            min3 = List[1];
                        }
                    }
                    else
                    {
                        if (List[3] <= List[1] && List[3] <= List[2])
                        {
                            //List 3 is first
                            min1 = List[3];
                            if (List[1] <= List[2])
                            {
                                //List1 is second
                                //List2 is third
                                min2 = List[1];
                                min3 = List[2];
                            }
                            else
                            {
                                //List2 is second
                                //List1 is third
                                min2 = List[2];
                                min3 = List[1];
                            }
                        }
                    }
                }

                val = List[min];

                tempForEndVar = max;
                for (i = min; i <= tempForEndVar; i++)
                {
                    val = List[i];
                    if (val < min1)
                    {
                        min1 = val;
                        Sublist[3] = Sublist[2];
                        Sublist[2] = Sublist[1];
                        Sublist[1] = i;
                    }
                    else
                    {
                        if (val < min2)
                        {
                            min2 = val;
                            Sublist[3] = Sublist[2];
                            Sublist[2] = i;
                        }
                        else
                        {
                            if (val < min3)
                            {
                                Sublist[3] = i;
                            }
                        }
                    }
                }

            }


        }




        // Subroutine to implement an Array sort
        internal static void EQ_Quicksort(double[] List, double[] Sublist, int min, int max)
        {



            if (min >= max)
            {
                return;
            }

            int i = Convert.ToInt32(Math.Floor((double)((max - min + 1) * GetRnd() + min)));
            double med_value = List[i];
            double submed = Sublist[i];

            List[i] = List[min];
            Sublist[i] = Sublist[min];

            int lo = min;
            int hi = max;
            do
            {

                while (List[hi] >= med_value)
                {
                    hi--;
                    if (hi <= lo)
                    {
                        break;
                    }
                };
                if (hi <= lo)
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

            EQ_Quicksort(List, Sublist, min, lo - 1);
            EQ_Quicksort(List, Sublist, lo + 1, max);

        }


        // Subroutine to implement an Array sort

        internal static void EQ_Quicksort2(Tdatholder[] List, int min, int max)
        {

            if (min >= max)
            {
                return;
            }

            int i = Convert.ToInt32(Math.Floor((double)((max - min + 1) * GetRnd() + min)));
            Tdatholder med_value = Tdatholder.CreateInstance();

            List[i] = List[min];

            int lo = min;
            int hi = max;

            do
            {

                while (List[hi].dat >= med_value.dat)
                {
                    hi--;
                    if (hi <= lo)
                    {
                        break;
                    }
                };
                if (hi <= lo)
                {
                    List[lo] = med_value;
                    break;
                }

                List[lo] = List[hi];

                lo++;

                while (List[lo].dat < med_value.dat)
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
                    break;
                }

                List[hi] = List[lo];
            }
            while (true);

            EQ_Quicksort2(List, min, lo - 1);
            EQ_Quicksort2(List, lo + 1, max);

        }

        // Subroutine to implement an Array sort with three sublists

        internal static void EQ_Quicksort3(double[] List, double[] Sublist1, double[] Sublist2, double[] Sublist3, int min, int max)
        {

            if (min >= max)
            {
                return;
            }

            int i = Convert.ToInt32(Math.Floor((double)((max - min + 1) * GetRnd() + min)));
            double med_value = List[i];
            double submed1 = Sublist1[i];
            double submed2 = Sublist2[i];
            double submed3 = Sublist3[i];

            List[i] = List[min];
            Sublist1[i] = Sublist1[min];
            Sublist2[i] = Sublist2[min];
            Sublist3[i] = Sublist3[min];

            int lo = min;
            int hi = max;
            do
            {


                while (List[hi] >= med_value)
                {
                    hi--;
                    if (hi <= lo)
                    {
                        break;
                    }
                };
                if (hi <= lo)
                {
                    List[lo] = med_value;
                    Sublist1[lo] = submed1;
                    Sublist2[lo] = submed2;
                    Sublist3[lo] = submed3;
                    break;
                }


                List[lo] = List[hi];
                Sublist1[lo] = Sublist1[hi];
                Sublist2[lo] = Sublist2[hi];
                Sublist3[lo] = Sublist3[hi];

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
                    Sublist1[hi] = submed1;
                    Sublist2[hi] = submed2;
                    Sublist3[hi] = submed3;
                    break;
                }

                List[hi] = List[lo];
                Sublist1[hi] = Sublist1[lo];
                Sublist2[hi] = Sublist2[lo];
                Sublist3[hi] = Sublist3[lo];
            }
            while (true);

            EQ_Quicksort3(List, Sublist1, Sublist2, Sublist3, min, lo - 1);
            EQ_Quicksort3(List, Sublist1, Sublist2, Sublist3, lo + 1, max);

        }

        // Function to compute for an area of a triangle

        internal static double EQ_Triangle_Area(double px1, double py1, double px2, double py2, double px3, double py3)
        {


            //True formula is this
            //    EQ_Triangle_Area = Abs(((px2 * py1) - (px1 * py2)) + ((px3 * py2) - (px2 * py3)) + ((px1 * py3) - (px3 * py1))) / 2

            // Make LARGE  numerical value safe for Windows by adding a scaling factor

            double ta = (((px2 * py1) - (px1 * py2)) / 10000d) + (((px3 * py2) - (px2 * py3)) / 10000d) + (((px1 * py3) - (px3 * py1)) / 10000d);

            return Math.Abs(ta) / 2d;

        }

        // Function to check if a point is inside the triangle. Computed based sum of areas method

        internal static int EQ_CheckPoint_in_Triangle(double px, double py, double px1, double py1, double px2, double py2, double px3, double py3)
        {


            double ta = EQ_Triangle_Area(px1, py1, px2, py2, px3, py3);
            double t1 = EQ_Triangle_Area(px, py, px2, py2, px3, py3);
            double t2 = EQ_Triangle_Area(px1, py1, px, py, px3, py3);
            double t3 = EQ_Triangle_Area(px1, py1, px2, py2, px, py);


            if (Math.Abs(ta - t1 - t2 - t3) < 2)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }




        internal static Coord EQ_GetCenterPoint(Coord p1, Coord p2, Coord p3)
        {

            Coord result = new Coord();
            double p2x = 0;
            double p2y = 0;
            double p4x = 0;
            double p4y = 0;





            // Get the two line 4 point data

            double p1x = p1.x;
            double p1y = p1.Y;


            if (p3.x > p2.x)
            {
                p2x = ((p3.x - p2.x) / 2d) + p2.x;
            }
            else
            {
                p2x = ((p2.x - p3.x) / 2d) + p3.x;
            }

            if (p3.Y > p2.Y)
            {
                p2y = ((p3.Y - p2.Y) / 2d) + p2.Y;
            }
            else
            {
                p2y = ((p2.Y - p3.Y) / 2d) + p3.Y;
            }

            double p3x = p2.x;
            double p3y = p2.Y;


            if (p1.x > p3.x)
            {
                p4x = ((p1.x - p3.x) / 2d) + p3.x;
            }
            else
            {
                p4x = ((p3.x - p1.x) / 2d) + p1.x;
            }

            if (p1.Y > p3.Y)
            {
                p4y = ((p1.Y - p3.Y) / 2d) + p3.Y;
            }
            else
            {
                p4y = ((p3.Y - p1.Y) / 2d) + p1.Y;
            }


            double XD1 = p2x - p1x;
            double XD2 = p4x - p3x;
            double YD1 = p2y - p1y;
            double YD2 = p4y - p3y;
            double XD3 = p1x - p3x;
            double YD3 = p1y - p3y;


            double dv = (YD2 * XD1) - (XD2 * YD1);

            if (dv == 0)
            {
                dv = 0.00000001d;
            } //avoid div 0 errors


            double ua = ((XD2 * YD3) - (YD2 * XD3)) / dv;
            double ub = ((XD1 * YD3) - (YD1 * XD3)) / dv;

            result.x = p1x + (ua * XD1);
            result.Y = p1y + (ub * YD1);

            return result;
        }


        internal static SphereCoord EQ_SphericalPolar(double RA, double DEC, double TOT, double RACENTER, double DECCENTER, double Latitude)
        {
            SphereCoord result = new SphereCoord();
            double[,,,] Get_EncoderHours = null;
            double[] Range360 = null;
            object[,,,,] hadec_aa = null;
            int[,,,] Get_EncoderDegrees = null;
            double x = 0;
            double Y = 0;

            double i = Get_EncoderHours[Convert.ToInt32(RACENTER), Convert.ToInt32(RA), Convert.ToInt32(TOT), 0];
            double j = Get_EncoderDegrees[Convert.ToInt32(DECCENTER), Convert.ToInt32(DEC), Convert.ToInt32(TOT), 0] + 270;
            j = Range360[Convert.ToInt32(j)];

            object tempAuxVar = hadec_aa[Convert.ToInt32(Latitude * EQASCOM.DEG_RAD), Convert.ToInt32(i * EQASCOM.HRS_RAD), Convert.ToInt32(j * EQASCOM.DEG_RAD), Convert.ToInt32(Y), Convert.ToInt32(x)];

            result.x = ((((x * EQASCOM.RAD_DEG) - 180) / 360d) * TOT) + RACENTER;
            result.Y = ((((Y * EQASCOM.RAD_DEG) + 90) / 180d) * TOT) + DECCENTER;

            // Check if RA value is within allowed visible range
            i = TOT / 4d;
            if ((RA <= (RACENTER + i)) && (RA >= (RACENTER - i)))
            {
                result.r = 1;
            }
            else
            {
                result.r = 0;
            }

            return result;
        }

        internal static Coord EQ_PolarSpherical(double RA, double DEC, double TOT, double RACENTER, double DECCENTER, double Latitude, double range)
        {
            Coord result = new Coord();
            double[] Range360 = null;
            double[,,,,] Get_EncoderfromDegrees = null;
            double[,,,] Get_EncoderfromHours = null;
            double[] Range24 = null;
            object[,,,,] aa_hadec = null;
            double x = 0;
            double Y = 0;


            double i = (((RA - RACENTER) / TOT) * 360) + 180;
            double j = (((DEC - DECCENTER) / TOT) * 180) - 90;

            object tempAuxVar = aa_hadec[Convert.ToInt32(Latitude * EQASCOM.DEG_RAD), Convert.ToInt32(j * EQASCOM.DEG_RAD), Convert.ToInt32(i * EQASCOM.DEG_RAD), Convert.ToInt32(x), Convert.ToInt32(Y)];

            if (i > 180)
            {
                if (range == 0)
                {
                    Y = Range360[Convert.ToInt32(180 - (Y * EQASCOM.RAD_DEG))];
                }
                else
                {
                    Y = Range360[Convert.ToInt32(Y * EQASCOM.RAD_DEG)];
                }
            }
            else
            {
                if (range == 0)
                {
                    Y = Range360[Convert.ToInt32(Y * EQASCOM.RAD_DEG)];
                }
                else
                {
                    Y = Range360[Convert.ToInt32(180 - (Y * EQASCOM.RAD_DEG))];
                }
            }

            j = Range360[Convert.ToInt32(Y + 90)];

            if (j < 180)
            {
                if (range == 1)
                {
                    x = Range24[Convert.ToInt32(x * EQASCOM.RAD_HRS)];
                }
                else
                {
                    x = Range24[Convert.ToInt32(24 + (x * EQASCOM.RAD_HRS))];
                }
            }
            else
            {
                x = Range24[Convert.ToInt32(12 + (x * EQASCOM.RAD_HRS))];
            }


            result.x = Get_EncoderfromHours[Convert.ToInt32(RACENTER), Convert.ToInt32(x), Convert.ToInt32(TOT), 0];
            result.Y = Get_EncoderfromDegrees[Convert.ToInt32(DECCENTER), Convert.ToInt32(Y + 90), Convert.ToInt32(TOT), 0, 0];

            return result;
        }


        internal static CartesCoord EQ_Spherical2Cartes(double RA, double DEC, double TOT, double RACENTER, double DECCENTER)
        {
            CartesCoord result = new CartesCoord();
            double gLatitude = 0;


            SphereCoord tmpobj4 = EQ_SphericalPolar(RA, DEC, TOT, RACENTER, DECCENTER, gLatitude);

            CartesCoord tmpobj1 = EQ_Polar2Cartes(tmpobj4.x, tmpobj4.Y, TOT, RACENTER, DECCENTER);

            result.x = tmpobj1.x;
            result.Y = tmpobj1.Y;
            result.RA = tmpobj1.RA;
            result.r = tmpobj1.r;

            return result;
        }

        internal static Coord EQ_Cartes2Spherical(double x, double Y, double r, double RA, double range, double TOT, double RACENTER, double DECCENTER)
        {
            double gDECEncoder_Home_pos = 0;
            double gLatitude = 0;
            double gTot_step = 0;
            double RAEncoder_Home_pos = 0;

            Coord tmpobj2 = EQ_Cartes2Polar(x, Y, r, RA, TOT, RACENTER, DECCENTER);
            return EQ_PolarSpherical(tmpobj2.x, tmpobj2.Y, gTot_step, RAEncoder_Home_pos, gDECEncoder_Home_pos, gLatitude, range);

        }


        internal static TriangleCoord EQ_Choose_3Points(double x, double Y)
        {
            TriangleCoord result = new TriangleCoord();
            Coord tmpcoords = new Coord();
            Coord p1 = new Coord();
            Coord p2 = new Coord();
            Coord p3 = new Coord();
            Coord pc = new Coord();

            Tdatholder[] datholder = new Tdatholder[EQASCOM.MAX_STARS];
            double combi_cnt = 0;
            double tmp1 = 0;
            int tmp2 = 0;
            bool first = false;
            double last_dist = 0;
            double new_dist = 0;

            // Adjust only if there are three alignment stars

            if (EQASCOM.gAlignmentStars_count < 3)
            {
                result.i = 1;
                result.j = 2;
                result.k = 3;
                return result;
            }

            tmpcoords.x = x;
            tmpcoords.Y = Y;
            Coord tmpcoord = EQ_sp2Cs(tmpcoords);

            int Count = 0;
            for (int i = 0; i < EQASCOM.gAlignmentStars_count; i++)
            {

                datholder[Count].cc = EQASCOM.my_PointsC[i];
                switch (EQASCOM.gPointFilter)
                {
                    case 0:
                        // all points 

                        break;
                    case 1:
                        // only consider points on this side of the meridian 
                        if (datholder[Count].cc.Y * tmpcoord.Y < 0)
                        {
                            goto NextPoint;
                        }

                        break;
                    case 2:
                        // local quadrant 
                        if (!EQMath.GetQuadrant(tmpcoord).Equals(EQMath.GetQuadrant(datholder[Count].cc)))
                        {
                            goto NextPoint;
                        }

                        break;
                }

                if (EQASCOM.CheckLocalPier)
                {
                    // calculate polar distance
                    datholder[Count].dat = Math.Pow(EQASCOM.my_Points[i].x - x, 2) + Math.Pow(EQASCOM.my_Points[i].Y - Y, 2);
                }
                else
                {
                    // calculate cartesian disatnce
                    datholder[Count].dat = Math.Pow(datholder[Count].cc.x - tmpcoord.x, 2) + Math.Pow(datholder[Count].cc.Y - tmpcoord.Y, 2);
                }

                // Also save the reference star id for this particular reference star
                datholder[Count].idx = Convert.ToInt16(i);

                Count++;
            NextPoint:;
            }

            if (Count < 3)
            {
                // not enough points to do 3-point
                result.i = 0;
                result.j = 0;
                result.k = 0;
                return result;
            }

            // now sort the disatnces so the closest stars are at the top
            EQ_Quicksort2(datholder, 1, Count);

            //Just use the nearest 50 stars (max) - saves processing time
            if (Count > EQASCOM.gMaxCombinationCount - 1)
            {
                combi_cnt = EQASCOM.gMaxCombinationCount;
            }
            else
            {
                combi_cnt = Count;
            }

            //    combi_offset = 1
            tmp1 = combi_cnt - 1;
            tmp2 = Convert.ToInt32(combi_cnt - 2);
            first = true;
            // iterate through all the triangles posible using the nearest alignment points
            int l = 1;
            int m = 2;
            int n = 3;
            int tempForEndVar2 = (tmp2);
            for (int i = 1; i <= tempForEndVar2; i++)
            {
                p1 = datholder[Convert.ToInt32(i) - 1].cc;
                double tempForEndVar3 = (tmp1);
                for (int j = i + 1; j <= tempForEndVar3; j++)
                {
                    p2 = datholder[Convert.ToInt32(j) - 1].cc;
                    double tempForEndVar4 = combi_cnt;
                    for (int k = (j + 1); k <= tempForEndVar4; k++)
                    {
                        p3 = datholder[Convert.ToInt32(k) - 1].cc;

                        if (EQ_CheckPoint_in_Triangle(tmpcoord.x, tmpcoord.Y, p1.x, p1.Y, p2.x, p2.Y, p3.x, p3.Y) == 1)
                        {
                            // Compute for the center point
                            pc = EQ_GetCenterPoint(p1, p2, p3);
                            // don't need full pythagoras - sum of squares is good enough
                            new_dist = Math.Pow(pc.x - tmpcoord.x, 2) + Math.Pow(pc.Y - tmpcoord.Y, 2);

                            if (first)
                            {
                                // first time through
                                last_dist = new_dist;
                                first = false;
                                l = i;
                                m = j;
                                n = Convert.ToInt32(k);
                            }
                            else
                            {
                                if (new_dist < last_dist)
                                {
                                    l = i;
                                    m = j;
                                    n = Convert.ToInt32(k);
                                    last_dist = new_dist;
                                }
                            }
                        }
                    }
                }
            }

            if (first)
            {
                result.i = 0;
                result.j = 0;
                result.k = 0;
            }
            else
            {
                result.i = datholder[Convert.ToInt32(l) - 1].idx;
                result.j = datholder[Convert.ToInt32(m) - 1].idx;
                result.k = datholder[n - 1].idx;
            }

            return result;
        }

        internal static TriangleCoord EQ_ChooseNearest3Points(double x, double Y)
        {
            TriangleCoord result = new TriangleCoord();
            Coord tmpcoords = new Coord();
            Coord p1 = new Coord();
            Coord p2 = new Coord();
            Coord p3 = new Coord();
            Coord pc = new Coord();

            Tdatholder[] datholder = new Tdatholder[EQASCOM.MAX_STARS];
            double combi_cnt = 0;
            double tmp1 = 0;
            int tmp2 = 0;
            bool first = false;

            // Adjust only if there are three alignment stars

            if (EQASCOM.gAlignmentStars_count <= 3)
            {
                result.i = 1;
                result.j = 2;
                result.k = 3;
                return result;
            }

            tmpcoords.x = x;
            tmpcoords.Y = Y;
            Coord tmpcoord = EQ_sp2Cs(tmpcoords);

            int Count = 0;
            // first find out the distances to the alignment stars
            for (int i = 1; i <= EQASCOM.gAlignmentStars_count; i++)
            {

                datholder[Count].cc = EQASCOM.my_PointsC[Convert.ToInt32(i)];

                switch (EQASCOM.gPointFilter)
                {
                    case 0:
                        // all points 

                        break;
                    case 1:
                        // only consider points on this side of the meridian 
                        if (datholder[Count].cc.Y * tmpcoord.Y < 0)
                        {
                            goto NextPoint;
                        }

                        break;
                    case 2:
                        // local quadrant 
                        if (!EQMath.GetQuadrant(tmpcoord).Equals(EQMath.GetQuadrant(datholder[Count].cc)))
                        {
                            goto NextPoint;
                        }

                        break;
                }

                if (EQASCOM.CheckLocalPier)
                {
                    // calculate polar distance
                    datholder[Count].dat = Math.Pow(EQASCOM.my_Points[i].x - x, 2) + Math.Pow(EQASCOM.my_Points[i].Y - Y, 2);
                }
                else
                {
                    // calculate cartesian disatnce
                    datholder[Count].dat = Math.Pow(datholder[Count].cc.x - tmpcoord.x, 2) + Math.Pow(datholder[Count].cc.Y - tmpcoord.Y, 2);
                }

                // Also save the reference star id for this particular reference star
                datholder[Count].idx = Convert.ToInt16(i);

                Count++;
            NextPoint:;
            }

            if (Count < 3)
            {
                // not enough points to do 3-point
                result.i = 0;
                result.j = 0;
                result.k = 0;
                return result;
            }

            // now sort the disatnces so the closest stars are at the top
            EQ_Quicksort2(datholder, 1, Count);

            //Just use the nearest 50 stars (max) - saves processing time
            if (Count > EQASCOM.gMaxCombinationCount - 1)
            {
                combi_cnt = EQASCOM.gMaxCombinationCount;
            }
            else
            {
                combi_cnt = Count;
            }

            tmp1 = combi_cnt - 1;
            tmp2 = Convert.ToInt32(combi_cnt - 2);
            first = true;

            // iterate through all the triangles posible using the nearest alignment points
            int l = 1;
            int m = 2;
            int n = 3;
            int tempForEndVar2 = (tmp2);
            for (int i = 1; i <= tempForEndVar2; i++)
            {
                p1 = datholder[Convert.ToInt32(i) - 1].cc;
                double tempForEndVar3 = (tmp1);
                for (int j = i + 1; j <= tempForEndVar3; j++)
                {
                    p2 = datholder[Convert.ToInt32(j) - 1].cc;
                    double tempForEndVar4 = combi_cnt;
                    for (int k = (j + 1); k <= tempForEndVar4; k++)
                    {
                        p3 = datholder[Convert.ToInt32(k) - 1].cc;

                        if (EQ_CheckPoint_in_Triangle(tmpcoord.x, tmpcoord.Y, p1.x, p1.Y, p2.x, p2.Y, p3.x, p3.Y) == 1)
                        {
                            l = i;
                            m = j;
                            n = Convert.ToInt32(k);
                            goto alldone;
                        }
                    }
                }
            }

            result.i = 0;
            result.j = 0;
            result.k = 0;
            return result;

        alldone:
            result.i = datholder[Convert.ToInt32(l) - 1].idx;
            result.j = datholder[Convert.ToInt32(m) - 1].idx;
            result.k = datholder[n - 1].idx;

            return result;
        }

        public static float GetRnd()
        {
            Random r = new Random();
            return (float)(r.Next(1, 1000) / 1000.0);
        }

    }
}