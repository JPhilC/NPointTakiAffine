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
//---------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPoint
{
    /// <summary>
    /// A static class to hold temporary variables in place of those
    /// defined elsewhere in the EQASCOM code base.
    /// </summary>
    public static class EQASCOM
    {
        // TODO: Replace static values that were access globally in EQASCOM.
        public static int MAX_STARS = 0;

        // TODO: Replace static values that were in configuration in  EQASCOM.
        public static bool POLAR_ENABLE = true;
        public static bool CheckLocalPier = true;
        public static int gAlignmentStars_count = 0;
        public static int gPointFilter = 2;
        public static int gMaxCombinationCount = 0;
        public static int gSelectStar = 0;

        // TODO: Replace static values for conversion constants.
        public static double HRS_RAD = 0.0;
        public static double RAD_HRS = 0.0;
        public static double DEG_RAD = 0.0;
        public static double RAD_DEG = 0.0;

        public static Coord[] ct_Points = new Coord[MAX_STARS];     // Catalog Points
        public static Coord[] my_Points = new Coord[MAX_STARS];     // My Measured Points
        public static Coord[] ct_PointsC = new Coord[MAX_STARS];    // Catalog Points (Cartesian)
        public static Coord[] my_PointsC = new Coord[MAX_STARS];    // My Measured Points (Cartesian)
    }
}
