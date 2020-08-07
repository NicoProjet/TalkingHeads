using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkingHeads
{
    public static class Configuration
    {
        public static readonly int NumberOfBytesInBmpHeaderStream = 54;
        public static readonly bool TestMode = true;
        public static readonly uint CanvasWidth = 1091;
        public static readonly uint CanvasHeight = 656;

        // Random seed
        public static readonly Random seed = new Random();

        // GeomWorld Creation
        public static readonly int MinNumberOfForms = 3;
        public static readonly int MaxNumberOfForms = 6;
        public static readonly int MinFormSizeDivide = 10; // Used to compute the minimal width/height of a figure using the width/height of the canvas (canvas.Width / var)
        public static readonly int MaxFormSizeDivide = 5; // Used to compute the maximal width/height of a figure using the width/height of the canvas (canvas.Width / var)
        public static readonly int MinNumberOfCorners = 3; // Minimum number of corners/edges in a polygon
        public static readonly int MaxNumberOfCorners = 6; // Maximum number of corners/edges in a polygon
        public static readonly int MaxLoopCounter = 15; // Upper bound of certain loops to avoid slowing down their computation (used in drawing triangles when trying to avoid unwanted shapes eg: straight lines)
        public static readonly bool GrayScale = true;
        public static readonly int GrayScaleMinAlpha = 25;
        public static readonly bool PrintForms = false;
        public static readonly int SizeOfIdRectangle = 14;


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
        public static readonly uint Node_Reward_For_Use = 15;
        public static readonly uint Node_Reward_For_Correct = 10;
        public static readonly uint Node_Reward_For_Incorrect = 10;

        // Score management word
        public static readonly uint Word_Default_Score = 50;
        public static readonly uint Word_Score_To_Trim = 0;
        public static readonly uint Word_Score_To_Dominate = 75; // obsolete
        public static readonly uint Word_Inactive_Steps_To_Erode = 20;
        public static readonly uint Word_Score_Erosion = 1;
        // Speaker
        public static readonly uint Word_Score_Update_When_Correct = 15;
        public static readonly uint Word_Score_Update_When_Incorrect = 15;
        public static readonly uint Word_Score_Update_When_Other_Correct = 5; // score to remove when another node is deemed to have the correct word
        // Guesser
        public static readonly uint Word_Score_Update_When_Correct_Form = 25; // user enters the correct form
        public static readonly uint Word_Score_Update_When_Correct_Form_Word_Unknown = 40; // user enters the correct form, node did not contain the word
        public static readonly uint Word_Score_Update_When_Other_Correct_Guesser = 5; // score to remove when another node is deemed to have the correct word

        // Word management
        public static readonly double Creation_Rate = 1;
        public static readonly double Absorption_Rate = 1;

        // Word creation
        public static readonly int Rare_Consonant_Percentage = 3;
        public static readonly uint Min_Number_Letters = 4;
        public static readonly uint Max_Number_Letters = 10;

        // TalkingHead usage
        public static readonly bool Local = true; // or remote (server) -> load discriminants tree of a talking head online
        public static readonly string LocalPath = "C:\\Users\\Nicolas Feron\\source\\repos\\TalkingHeads\\TalkingHeads\\SavedTalkingHeads\\";
        public static readonly string SaveFileExt = ".sav";
        public static readonly string LineSeparator = "\n";
        public static readonly char Separator = '_';

        // Guess management
        public static readonly uint Number_Of_Words = 2; // number of discriminations trees/words used in a description/guess
        public static readonly char Word_Separator = ' ';
    }
}
