using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mapper;

/// <summary>
/// Summary description for SingleTestStats
/// </summary>
public interface ISingleTestStats
{
    Type MapperType { get; set; }
    int TotalAreaAllImages { get; set; }
    ISprite FinalSprite { get; set; }

    int CandidateSpriteFails { get; set; }
    int CandidateSpritesGenerated { get; set; }
    int CanvasRectangleAddAttempts { get; set; }
    int CanvasNbrCellsGenerated { get; set; }

    // Result names
    float Efficiency { get; set; }
    long ElapsedTicks { get; set; }

    // Use the enum SingleTestStats.StatisticType as an index in this array
    double[] ImageStats { get; set;  }
}

