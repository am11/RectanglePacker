using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mapper;

/// <summary>
/// Summary description for SingleTestStats
/// </summary>
public class SingleTestStats : ISingleTestStats
{
    public enum StatisticType
    {
        // Use underscores, so when using these names as chart titles, the underscores can be replaced by spaces.
        Nbr_Images,
        Width_Standard_Deviation,
        Heigth_Standard_Deviation,
        Area_Standard_Deviation,
        Aspect_Ratio_Standard_Deviation
    }

    public Type MapperType { get; set; }
    public int TotalAreaAllImages { get; set; }
    public ISprite FinalSprite { get; set; }

    public int CandidateSpriteFails { get; set; }
    public int CandidateSpritesGenerated { get; set; }
    public int CanvasRectangleAddAttempts { get; set; }
    public int CanvasNbrCellsGenerated { get; set; }

    public float Efficiency { get; set; }
    public long ElapsedTicks { get; set; }

    public double[] ImageStats { get; set; }

    /// <summary>
    /// Returns the name of the type of a mapper in a friendly form
    /// </summary>
    public static string FriendlyMapperTypeName(Type mapperType)
    {
        string unfriendlyMapperTypeName = mapperType.ToString();
        string friendlyMapperTypeName = unfriendlyMapperTypeName.Replace("Mapper.", "").Replace("`1[SpriteInfo]", "");
        return friendlyMapperTypeName;
    }

    /// <summary>
    /// Returns the names of the fields holding observed test results
    /// </summary>
    /// <returns></returns>
    public static string[] ResultNames()
    {
        return new string[] { "Efficiency", "ElapsedTicks" };
    }

    /// <summary>
    /// Returns a header line for a CSV file, containing all field names in a SingleTestStats object.
    /// </summary>
    public static string CsvHeader()
    {
        string result =
            "MapperType, " +
            "TotalAreaAllImages, " +
            "SpriteWidth, " +
            "SpriteHeight, " +
            "SpriteArea, " +
            "CandidateSpriteFails, " +
            "CandidateSpritesGenerated, " +
            "CanvasRectangleAddAttempts, " +
            "CanvasNbrCellsGenerated, " +
            "Efficiency, " +
            "ElapsedTicks";

        foreach (string statisticTypeName in Enum.GetNames(typeof(StatisticType)))
        {
            result += ", " + statisticTypeName;
        }

        return result;
    }

    /// <summary>
    /// Returns a data line for use in a CSV file based on the info in this object.
    /// </summary>
    public string CsvDataLine()
    {
        string result = 
            string.Format(
                "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}",
                FriendlyMapperTypeName(MapperType), TotalAreaAllImages, FinalSprite.Width, FinalSprite.Height,
                FinalSprite.Area,
                CandidateSpriteFails,
                CandidateSpritesGenerated,
                CanvasRectangleAddAttempts, 
                CanvasNbrCellsGenerated, 
                Efficiency, ElapsedTicks);

        foreach (int statisticTypeIdx in Enum.GetValues(typeof(StatisticType)))
        {
            result += ", " + ImageStats[statisticTypeIdx].ToString();
        }

        return result;
    }
}

