using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mapper;

/// <summary>
/// Summary description for TestUtils
/// </summary>
public class TestUtils
{
    private static readonly int _nbrStatisticTypes = Enum.GetNames(typeof(SingleTestStats.StatisticType)).Length;

    /// <summary>
    /// Runs a performance test by generating a sprite with a number of images using a given IMapper object.
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <param name="mapper"></param>
    /// <param name="images"></param>
    /// <param name="testStats">
    /// Results of the test. Pass in a reference to an ISingleTestStats object. That object will be updated with the stats.
    /// </param>
    public static void RunTest<S>(
        IMapper<S> mapper, IEnumerable<IImageInfo> images, ISingleTestStats testStats) where S : class, ISprite, new()
    {
        S spriteInfo = null;
        MapperStats mapperStats = new MapperStats();

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        if (mapper is IMapperReturningStats<S>)
        {
            spriteInfo = ((IMapperReturningStats<S>)mapper).Mapping(images, mapperStats);
        }
        else
        {
            spriteInfo = mapper.Mapping(images);
        }

        stopWatch.Stop();
        long elapsedTickes = stopWatch.ElapsedTicks;

        int totalAreaAllImages =
            (from a in images select a.Width * a.Height).Sum();

        testStats.MapperType = mapper.GetType();
        testStats.TotalAreaAllImages = totalAreaAllImages;
        testStats.FinalSprite = spriteInfo;

        testStats.CandidateSpriteFails = mapperStats.CandidateSpriteFails;
        testStats.CandidateSpritesGenerated = mapperStats.CandidateSpritesGenerated;
        testStats.CanvasRectangleAddAttempts = mapperStats.CanvasRectangleAddAttempts;
        testStats.CanvasNbrCellsGenerated = mapperStats.CanvasNbrCellsGenerated;

        testStats.Efficiency = Efficiency(spriteInfo.Width, spriteInfo.Height, totalAreaAllImages);
        testStats.ElapsedTicks = elapsedTickes;

        // Statistics related to the input images

        testStats.ImageStats = new double[_nbrStatisticTypes];

        testStats.ImageStats[(int)SingleTestStats.StatisticType.Nbr_Images] = 
            images.Count();

        testStats.ImageStats[(int)SingleTestStats.StatisticType.Width_Standard_Deviation] = 
            StandardDeviation(from i in images select (double)i.Width);

        testStats.ImageStats[(int)SingleTestStats.StatisticType.Heigth_Standard_Deviation] = 
            StandardDeviation(from i in images select (double)i.Height);

        testStats.ImageStats[(int)SingleTestStats.StatisticType.Area_Standard_Deviation] = 
            StandardDeviation(from i in images select (double)Area(i.Width, i.Height));

        testStats.ImageStats[(int)SingleTestStats.StatisticType.Aspect_Ratio_Standard_Deviation] = 
            StandardDeviation(from i in images select (double)AspectRatio(i.Width, i.Height));

    }

    /// <summary>
    /// Returns the efficiency of a sprite, based on its widht, height and the total area of the images contained in it.
    /// </summary>
    /// <param name="spriteWidth"></param>
    /// <param name="spriteHeight"></param>
    /// <param name="totalImagesArea"></param>
    /// <returns></returns>
    public static float Efficiency(int spriteWidth, int spriteHeight, int totalImagesArea)
    {
        float spriteArea = spriteWidth * spriteHeight;
        float efficiency = totalImagesArea / spriteArea;

        return efficiency;
    }

    /// <summary>
    /// Returns the area, given a width and a height
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static int Area(int width, int height)
    {
        return width * height;
    }

    /// <summary>
    /// Returns the aspect ratio given a width and height.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static double AspectRatio(int width, int height)
    {
        return ((double)width) / height;
    }


    /// <summary>
    /// Computes the standard deviation of a series of numbers.
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
    public static double StandardDeviation(IEnumerable<double> numbers)
    {
        int nbrNumbers = numbers.Count();

        if (nbrNumbers <= 1) { return 0; }
        
        double mean = numbers.Average();

        double sumDeviations = numbers.Sum(d => Math.Pow(d - mean, 2));

        double result = Math.Sqrt(sumDeviations / (nbrNumbers - 1));

        return result;
    }

    /// <summary>
    /// Takes an absolute file path starting with the drive letter (such as created by Server.Mappath)
    /// and returns the corresponding url (of the form file:///.....)
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string FilePathToUrl(string filePath)
    {
        return "file:///" + filePath.Replace('\\', '/').Replace(" ", "%20");
    }

}
