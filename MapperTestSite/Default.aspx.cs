using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;
using System.Web.UI.DataVisualization.Charting;
using System.Diagnostics;
using Mapper;

public partial class _Default : System.Web.UI.Page
{
    protected int _imageZoom = 4;
    CanvasWritingStepImages _canvas = null;
    float _cutoffEfficiency = 0.0F;
    int _maximumNbrCandidates = 0;

    protected bool _showGenerationDetailsOfFailures = false;

    private class SingleTest
    {
        public List<ImageInfo> ImageInfos = new List<ImageInfo>();
        public string Heading = "";
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            hlCanvasImages.Visible = false;
            phGenerationDetails.Visible = false;
            hlGraphs.Visible = false;
            h1TestResults.Visible = false;
        }

    }

    protected void btnCustom_Click(object sender, EventArgs e)
    {
        Generate();
    }

    private void Generate()
    {
        int lowestWidth = LowestWidth.IntValue;
        int highestWidth = HighestWidth.IntValue;
        int aspectRatioDeviation = AspectRatioDeviation.IntValue;

        int lowestNbrImages = LowestNbrImages.IntValue;
        int highestNbrImages = HighestNbrImages.IntValue;
        int nbrTestsPerNbrImages = NbrTestsPerNbrImages.IntValue;
        _showGenerationDetailsOfFailures = chkShowGenerationDetailsFailuresToo.Checked;
        bool showGenerationDetails = _showGenerationDetailsOfFailures || chkShowGenerationDetails.Checked;
        bool showCanvasImages = chkShowCanvasImages.Checked;

        bool useFixedTests = chkFixed.Checked;
        bool useRandomTests = chkRandom.Checked;

        _cutoffEfficiency = CutoffEfficiency.FloatValue;
        _maximumNbrCandidates = MaxCandidates.IntValue;

        RandomSizeGenerator randomSizeGenerator = 
            new RandomSizeGenerator(lowestWidth, highestWidth, aspectRatioDeviation);

        // ----------------------------------------

        hlCanvasImages.Visible = showGenerationDetails;
        phGenerationDetails.Visible = showGenerationDetails;
        hlGraphs.Visible = true;
        h1TestResults.Visible = true;

        // ----------------------------------------
        // add random tests

        List<SingleTest> tests = new List<SingleTest>();

        if (useRandomTests)
        {
            int testNbr = 1;

            for (int nbrImages = lowestNbrImages; nbrImages <= highestNbrImages; nbrImages++)
            {
                for (int i = 0; i < nbrTestsPerNbrImages; i++)
                {
                    SingleTest singleTest = new SingleTest();
                    singleTest.Heading = string.Format("Random test {0}, using {1} images", testNbr, nbrImages);

                    for (int j = 0; j < nbrImages; j++)
                    {
                        singleTest.ImageInfos.Add(randomSizeGenerator.Next());
                    }

                    tests.Add(singleTest);
                    testNbr++;
                }
            }
        }

        // ---------------------------------------------
        // Add fixed tests with Mapper test cases

        if (useFixedTests)
        {
            AddFixedTests(tests);
        }

        // -------------------------------------------
        // Show generation details

        rptrTests.Visible = showGenerationDetails;

        if (showGenerationDetails)
        {
            _imageZoom = Convert.ToInt32(ImageZoom.IntValue);
            _canvas = new CanvasWritingStepImages(_imageZoom, showCanvasImages);

            rptrTests.DataSource = tests;
            rptrTests.DataBind();
        }

        // -----------------------------------------
        // Show aggregated statistics for multiple mappers

        // Run all the test using a standard canvas rather than a canvas that writes images.
        // That way, the timings are not distorted by the generation of the images.

        ICanvas standardCanvas = new Canvas();

        IMapper<Sprite>[] mappers = 
        { 
            new MapperHorizontalOnly<Sprite>(),
            new MapperVerticalOnly<Sprite>(),
            new MapperOptimalEfficiency<Sprite>(standardCanvas, _cutoffEfficiency, _maximumNbrCandidates)
        };

        // First run a few tests without recording results. The first few test runs always take way more
        // ticks than normal, so doing this will keep bad data out of the results.

        int nbrTests = tests.Count; if (nbrTests > 3) { nbrTests = 4; }
        for (int i = 0; i < nbrTests; i++)
        {
            foreach (IMapper<Sprite> mapper in mappers)
            {
                SingleTestStats singleTestStats = new SingleTestStats();
                TestUtils.RunTest<Sprite>(mapper, tests[i].ImageInfos, singleTestStats);
            }
        }

        // Run all the tests. Use all mappers as listed above in the mappers declaration to run the tests.
        // Dump the results to a file as they become available.

        List<SingleTestStats> testResults = new List<SingleTestStats>();

        string testResultsFilePath = Server.MapPath("~/testresults.csv");

        using (StreamWriter outfile =
           new StreamWriter(testResultsFilePath))
        {
            outfile.WriteLine(SingleTestStats.CsvHeader());

            foreach (SingleTest singleTest in tests)
            {
                foreach(IMapper<Sprite> mapper in mappers)
                {
                    SingleTestStats singleTestStats = new SingleTestStats();
                    TestUtils.RunTest<Sprite>(mapper, singleTest.ImageInfos, singleTestStats);
                    testResults.Add(singleTestStats);

                    outfile.WriteLine(singleTestStats.CsvDataLine());
                    outfile.Flush();
                }
            }
        }

        ltPathToCSV.Text =
            string.Format(
                @"A CSV file with all test results is at <a href=""{0}"">{0}</a>",
                TestUtils.FilePathToUrl(testResultsFilePath));

        // -----------------------------------------------
        // Show overall results per mapper

        var resultsPerMapper =
                from t in testResults
                group t by t.MapperType into g
                select new { 
                    Mapper = SingleTestStats.FriendlyMapperTypeName(g.Key),
                    AvgCandidateSpriteFails = g.Average(p => p.CandidateSpriteFails),
                    AvgCandidateSpritesGenerated = g.Average(p => p.CandidateSpritesGenerated),
                    AvgCanvasRectangleAddAttempts = g.Average(p => p.CanvasRectangleAddAttempts),
                    AvgCanvasNbrCellsGenerated = g.Average(p => p.CanvasNbrCellsGenerated),
                    AvgEfficiency = g.Average(p => p.Efficiency),
                    AvgElapsedTicks = g.Average(p => p.ElapsedTicks)
                };

        gvOverallResults.DataSource = resultsPerMapper;
        gvOverallResults.DataBind();

        // -----------------------------------------------
        // Create graphs with the results

        int statisticTypeIdx = 0;
        foreach (int statisticTypeValue in Enum.GetValues(typeof(SingleTestStats.StatisticType)))
        {
            // For each statistic type (such as number of images, standard deviation of widths of the images in the sprite, etc.),
            // we'll generate 2 charts, one for efficiency and one for elapsed ticks.

            string statisticTypeName = Enum.GetName(typeof(SingleTestStats.StatisticType), statisticTypeIdx);
            statisticTypeIdx++;

            // ---------

            string formattedStatisticTypeName = statisticTypeName.Replace('_', ' ');

            Literal statisticTypeHeader = new Literal();
            statisticTypeHeader.Text = string.Format("<h2>Results by {0}</h2>", formattedStatisticTypeName);
            phCharts.Controls.Add(statisticTypeHeader);

            string explanation = "";
            SingleTestStats.StatisticType statisticType = (SingleTestStats.StatisticType) Enum.Parse(typeof(SingleTestStats.StatisticType), statisticTypeValue.ToString()); 

            switch(statisticType)
            {
                case SingleTestStats.StatisticType.Nbr_Images:
                    explanation = "Shows speed and efficiency of the algorithms against the number of images in the sprite";
                    break;

                case SingleTestStats.StatisticType.Width_Standard_Deviation:
                    explanation = "Shows speed and efficiency of the algorithms against the variability of the width of the images";
                    break;

                case SingleTestStats.StatisticType.Heigth_Standard_Deviation:
                    explanation = "Shows speed and efficiency of the algorithms against the variability of the height of the images";
                    break;

                case SingleTestStats.StatisticType.Area_Standard_Deviation:
                    explanation = "Shows speed and efficiency of the algorithms against the variability of the area of the images";
                    break;

                case SingleTestStats.StatisticType.Aspect_Ratio_Standard_Deviation:
                    explanation = "Shows speed and efficiency of the algorithms against the variability of the aspect ratio of the images";
                    break;
            }

            Literal statisticExplanation = new Literal();
            statisticExplanation.Text = string.Format("<p><small>{0}</small></p>", explanation);
            phCharts.Controls.Add(statisticExplanation);

            foreach(string resultName in SingleTestStats.ResultNames())
            {
                // Create a chart for each result - efficiency and elapsed ticks

                Chart chart = new Chart();
                chart.ImageType = ChartImageType.Png;
                chart.ImageLocation="~/canvasimages/ChartPic_#SEQ(300,3)";
                chart.Width = new Unit(1200, UnitType.Pixel);
                chart.Height = new Unit(500, UnitType.Pixel);
                chart.Legends.Add("Default");

                chart.ChartAreas.Clear();
                chart.ChartAreas.Add("ChartArea1");
                chart.ChartAreas[0].AxisX.Title = formattedStatisticTypeName;
                chart.ChartAreas[0].AxisY.Title = resultName;

                chart.Series.Clear();
                
                phCharts.Controls.Add(chart);

                int seriesIdx = 0;
                foreach(IMapper<Sprite> mapper in mappers)
                {
                    Type mapperType = mapper.GetType();

                    var series =
                        from r in testResults
                        where r.MapperType == mapperType
                        group r by r.ImageStats[statisticTypeValue] into g
                        select new
                        {
                            Efficiency = g.Average(r => r.Efficiency),
                            ElapsedTicks = g.Average(r => r.ElapsedTicks),
                            XValue = g.Key
                        };

                    chart.Series.Add(SingleTestStats.FriendlyMapperTypeName(mapperType)); // Ensure the legend shows the type name
                    chart.Series[seriesIdx].MarkerSize=4;
                    chart.Series[seriesIdx].ChartType = SeriesChartType.Point;
                    chart.Series[seriesIdx].Points.DataBind(series, "XValue", resultName, ""); 

                    seriesIdx++;
                }
            }
        }
    }

    protected void rptrTests_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {

        if ((e.Item.ItemType != ListItemType.Item) & (e.Item.ItemType != ListItemType.AlternatingItem))
        {
            return;
        }

        // ----------

        SingleTest singleTest = (SingleTest)e.Item.DataItem;

        // ---------------------------
        // Show the rectangles to be placed

        Literal ltHeading = (Literal)(e.Item.FindControl("ltHeading"));
        ltHeading.Text = singleTest.Heading;

        GridView gvRectangleDimensions = (GridView)(e.Item.FindControl("gvRectangleDimensions"));

        var imageInfosHighestFirst =
            singleTest.ImageInfos.OrderByDescending(p => p.Height);

        gvRectangleDimensions.DataSource = imageInfosHighestFirst;
        gvRectangleDimensions.DataBind();

        // -----------------
        // Place the rectangles on a canvas. The canvas will produce images of each stage.

        MapperOptimalEfficiency_AdditionalStats<Sprite> mapper = 
            new MapperOptimalEfficiency_AdditionalStats<Sprite>(_canvas, _cutoffEfficiency, _maximumNbrCandidates);

        DetailedSingleTestStats stats = mapper.GetMappingStats(singleTest.ImageInfos);

        // Show the results

        Literal ltFinalWidth = (Literal)(e.Item.FindControl("ltFinalWidth"));
        ltFinalWidth.Text = stats.FinalSprite.Width.ToString();

        Literal ltFinalHeight = (Literal)(e.Item.FindControl("ltFinalHeight"));
        ltFinalHeight.Text = stats.FinalSprite.Height.ToString();

        Literal ltFinalArea = (Literal)(e.Item.FindControl("ltFinalArea"));
        ltFinalArea.Text = stats.FinalSprite.Area.ToString("N0");

        Literal ltFinalEfficiency = (Literal)(e.Item.FindControl("ltFinalEfficiency"));
        ltFinalEfficiency.Text = stats.Efficiency.ToString("p");

        GridView gvSpriteImageOffsets = (GridView)(e.Item.FindControl("gvSpriteImageOffsets"));
        gvSpriteImageOffsets.DataSource = stats.FinalSprite.MappedImages;
        gvSpriteImageOffsets.DataBind();

        System.Web.UI.WebControls.Image imgSprite = (System.Web.UI.WebControls.Image)(e.Item.FindControl("imgSprite"));
        imgSprite.ImageUrl = _canvas.SpriteToImage(stats.FinalSprite);

        var EnhancedIterations =
            from s in stats.Iterations
            select new { 
                Result = s.Result,
                MaxCanvasWidth = s.MaxCanvasWidth == _canvas.UnlimitedSize ? "infinite" : s.MaxCanvasWidth.ToString(),
                MaxCanvasHeight = s.MaxCanvasHeight,
                MaxCanvasArea = s.MaxCanvasWidth == _canvas.UnlimitedSize ? "infinite" : (s.MaxCanvasWidth * s.MaxCanvasHeight).ToString("N0"),
                IntermediateSpriteWidth = s.IntermediateSpriteWidth,
                IntermediateSpriteHeight = s.IntermediateSpriteHeight,
                IntermediateSpriteArea = s.IntermediateSpriteWidth * s.IntermediateSpriteHeight,
                IntermediateSpriteEfficiency = TestUtils.Efficiency(s.IntermediateSpriteWidth, s.IntermediateSpriteHeight, stats.TotalAreaAllImages),
                ImageDetails = s.ImageDetails
            };

        Repeater rptrIterations = (Repeater)(e.Item.FindControl("rptrIterations"));
        rptrIterations.DataSource = EnhancedIterations;
        rptrIterations.DataBind();
    }

    private void AddFixedTests(List<SingleTest> tests)
    {
        int fixedTestNbr = 1;

        {
            // demonstrates why to increase height of the canvas by lowest free height deficit of the tallest rectangle that sits against 
            // the far right border of the canvas, when you decrease the canvas width by 1 after a successful sprite.
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(20, 60));
            singleTest.ImageInfos.Add(new ImageInfo(10, 50));
            tests.Add(singleTest);
        }

        {
            // Demonstrates why if an occupied area borders the top edge of the canvas, treat this as though there is a zero height free FreeArea on top of the occupied area.
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(20, 60));
            singleTest.ImageInfos.Add(new ImageInfo(10, 50));
            // the third image is exactly as high as the difference in width between 1st and second image
            singleTest.ImageInfos.Add(new ImageInfo(25, 10));
            tests.Add(singleTest);
        }

        {
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(10, 80));
            singleTest.ImageInfos.Add(new ImageInfo(20, 62));
            singleTest.ImageInfos.Add(new ImageInfo(10, 70));
            singleTest.ImageInfos.Add(new ImageInfo(30, 50));
            singleTest.ImageInfos.Add(new ImageInfo(20, 15));
            tests.Add(singleTest);
        }

        {
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(10, 60));
            singleTest.ImageInfos.Add(new ImageInfo(20, 50));
            singleTest.ImageInfos.Add(new ImageInfo(10, 55));
            singleTest.ImageInfos.Add(new ImageInfo(30, 45));
            singleTest.ImageInfos.Add(new ImageInfo(30, 30));
            tests.Add(singleTest);
        }

        {
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(10, 60));
            singleTest.ImageInfos.Add(new ImageInfo(20, 50));
            singleTest.ImageInfos.Add(new ImageInfo(10, 55));
            singleTest.ImageInfos.Add(new ImageInfo(30, 45));
            singleTest.ImageInfos.Add(new ImageInfo(30, 3));
            tests.Add(singleTest);
        }

        {
            // 3 images. lowest is wider than the second lowest, so will stick through 2 vertical columns
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(60, 60));
            singleTest.ImageInfos.Add(new ImageInfo(40, 40));
            singleTest.ImageInfos.Add(new ImageInfo(80, 20));
            tests.Add(singleTest);
        }
       
        {
            // 4 images. lowest is wider than the second lowest, so will stick through 2 vertical columns
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(60, 60));
            singleTest.ImageInfos.Add(new ImageInfo(40, 40));
            singleTest.ImageInfos.Add(new ImageInfo(20, 20));
            singleTest.ImageInfos.Add(new ImageInfo(80, 10));
            tests.Add(singleTest);
        }
        
        {
            // 4 images. lowest is wider than the second and third lowest, so will stick through 3 vertical columns
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(60, 60));
            singleTest.ImageInfos.Add(new ImageInfo(40, 40));
            singleTest.ImageInfos.Add(new ImageInfo(24, 24));
            singleTest.ImageInfos.Add(new ImageInfo(80, 10));
            tests.Add(singleTest);
        }

        {
            // Same as last, but order of images additions is changed - overlapping image is now last but one
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(60, 60));
            singleTest.ImageInfos.Add(new ImageInfo(40, 40));
            singleTest.ImageInfos.Add(new ImageInfo(20, 8));
            singleTest.ImageInfos.Add(new ImageInfo(80, 14));
            tests.Add(singleTest);
        }

        {
            // Combination test
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(60, 62));
            singleTest.ImageInfos.Add(new ImageInfo(30, 24));
            singleTest.ImageInfos.Add(new ImageInfo(26, 24));
            singleTest.ImageInfos.Add(new ImageInfo(24, 4));
            singleTest.ImageInfos.Add(new ImageInfo(80, 6));
            singleTest.ImageInfos.Add(new ImageInfo(50, 14));
            singleTest.ImageInfos.Add(new ImageInfo(16, 18));
            singleTest.ImageInfos.Add(new ImageInfo(38, 22));
            singleTest.ImageInfos.Add(new ImageInfo(24, 4));
            tests.Add(singleTest);
        }

        {
            // Combination test 2
            SingleTest singleTest = new SingleTest();
            singleTest.Heading = "Fixed test " + (fixedTestNbr++).ToString();
            singleTest.ImageInfos.Add(new ImageInfo(120, 122));
            singleTest.ImageInfos.Add(new ImageInfo(60, 48));
            singleTest.ImageInfos.Add(new ImageInfo(60, 48));
            singleTest.ImageInfos.Add(new ImageInfo(48, 8));
            singleTest.ImageInfos.Add(new ImageInfo(150, 12));
            singleTest.ImageInfos.Add(new ImageInfo(100, 30));
            singleTest.ImageInfos.Add(new ImageInfo(32, 36));
            singleTest.ImageInfos.Add(new ImageInfo(76, 44));
            singleTest.ImageInfos.Add(new ImageInfo(48, 8));
            tests.Add(singleTest);
        }
         
    }




}