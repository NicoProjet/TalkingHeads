using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingHeads
{
    public class Configuration
    {
        // GeomWorld Creation
        public static readonly int MinNumberOfForms = 3;
        public static readonly int MaxNumberOfForms = 6;
        public static readonly int MinFormSizeDivide = 10; // Used to compute the minimal width/height of a figure using the width/height of the canvas (canvas.Width / var)
        public static readonly int MaxFormSizeDivide = 5; // Used to compute the maximal width/height of a figure using the width/height of the canvas (canvas.Width / var)
        public static readonly int MinNumberOfCorners = 3; // Minimum number of corners/edges in a polygon
        public static readonly int MaxNumberOfCorners = 6; // Maximum number of corners/edges in a polygon
        public static readonly int MaxLoopCounter = 15; // Upper bound of certain loops to avoid slowing down their computation (used in drawing triangles when trying to avoid unwanted shapes eg: straight lines)
        public static readonly bool GrayScale = false;


        // GeomWorld Processing
        public static readonly int Max_A_diff = 6;
        public static readonly int Max_R_diff = 6;
        public static readonly int Max_G_diff = 6;
        public static readonly int Max_B_diff = 6;
        public static readonly int Max_Color_diff = 15;
        public static readonly int Max_Color_Dist = 20;
        public static readonly int Min_PixelPerLine = 10;
        public static readonly int PrecisionLossCleaningCoeff = 50; // Percentage of the min size of a form needed to be considered a figure during cleaning
        public static readonly bool DynamicBackGroundColor = true; // If false -> bgColor = White, else bgColor dynamically computed from the most present color

        // Talking Head
        // Score management node
        public static readonly uint Node_Default_Score = 40;
        public static readonly uint Node_Score_To_Reduce = 10;
        public static readonly uint Node_Score_To_Split = 75;
        public static readonly uint Node_Inactive_Steps_To_Erode = 10;
        public static readonly uint Node_Score_Erosion = 1;
        public static readonly uint Node_Reward_For_Correct = 10;
        public static readonly uint Node_Malus_For_Incorrect = 3;

        // Score management word
        public static readonly uint Word_Default_Score = 50;
        public static readonly uint Word_Score_To_Trim = 5;
        public static readonly uint Word_Score_To_Dominate = 75;

        // Word management
        public static readonly double Creation_Rate = 1;
        public static readonly double Absorption_Rate = 1;

        // Word creation
        public static readonly int Rare_Consonant_Percentage = 3;
        public static readonly uint Min_Number_Letters = 4;
        public static readonly uint Max_Number_Letters = 10;
    }
}
