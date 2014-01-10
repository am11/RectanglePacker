using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Mapper;
using System.Drawing;

/// <summary>
/// Version of MapperOptimalEfficiency that produces additional stats
/// </summary>
public class MapperOptimalEfficiency_AdditionalStats<S> : MapperOptimalEfficiency<S> where S : class, ISprite, new()
{
    ICanvas _canvas = null;
    List<IterationStats> _currentMappingIterationStats = null; 

	public MapperOptimalEfficiency_AdditionalStats(ICanvas canvas)
            : base(canvas)
    {
        _canvas = canvas;
    }

    /// <summary>
    /// See MapperIterative_Base
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="cutoffEfficiency"></param>
    /// <param name="maxNbrCandidateSprites"></param>
    public MapperOptimalEfficiency_AdditionalStats(ICanvas canvas, float cutoffEfficiency, int maxNbrCandidateSprites)
        : base(canvas, cutoffEfficiency, maxNbrCandidateSprites)
    {
        _canvas = canvas;
    }

    /// <summary>
    /// Does a mapping with the given images and returns the resulting stats.
    /// </summary>
    /// <param name="images"></param>
    /// <returns></returns>
    public DetailedSingleTestStats GetMappingStats(IEnumerable<IImageInfo> images)
    {
        // Mapping will call MappingRestrictedBox, which will add records to this collection.
        _currentMappingIterationStats = new List<IterationStats>();

        DetailedSingleTestStats stats = new DetailedSingleTestStats();
        TestUtils.RunTest<S>(this, images, stats);

        stats.Iterations = _currentMappingIterationStats;

        return stats;
    }

    /// <summary>
    /// This method is called by MapperOptimalEfficiency_AdditionalStats for each iteration
    /// </summary>
    /// <param name="images"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    protected override S MappingRestrictedBox(
        IOrderedEnumerable<IImageInfo> images, int maxWidth, int maxHeight, ICanvasStats canvasStats,
        out int lowestFreeHeightDeficitTallestRightFlushedImage)
    {
        S intermediateSpriteInfo = 
            base.MappingRestrictedBox(images, maxWidth, maxHeight, canvasStats, out lowestFreeHeightDeficitTallestRightFlushedImage);

        IterationStats stats = null;
        CanvasWritingStepImages canvasBLFStats = _canvas as CanvasWritingStepImages;

        List<ImagePlacementDetails> imageDetails = null;
        if (canvasBLFStats != null) { imageDetails = new List<ImagePlacementDetails>(canvasBLFStats.ImageDetails); }

        if (intermediateSpriteInfo != null)
        {
            stats =
                new IterationStats
                {
                    Result = IterationResult.Success,
                    MaxCanvasWidth = maxWidth,
                    MaxCanvasHeight = maxHeight,
                    IntermediateSpriteWidth = intermediateSpriteInfo.Width,
                    IntermediateSpriteHeight = intermediateSpriteInfo.Height,
                    ImageDetails = imageDetails
                };
        }
        else
        {
            stats =
                new IterationStats
                {
                    Result = IterationResult.Failure,
                    MaxCanvasWidth = maxWidth,
                    MaxCanvasHeight = maxHeight,
                    ImageDetails = imageDetails
                };
        }

        _currentMappingIterationStats.Add(stats);

        // Clear out the current image details as kept by the canvas, otherwise we get the same details again next time.
        if (canvasBLFStats != null) { canvasBLFStats.ImageDetails.Clear(); }

        return intermediateSpriteInfo;
    }

    /// <summary>
    /// </summary>
    /// <param name="images"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    protected override bool CandidateCanvasFeasable(
        int canvasMaxWidth, int canvasMaxHeight, int bestSpriteArea, int totalAreaAllImages,
        out bool candidateBiggerThanBestSprite, out bool candidateSmallerThanCombinedImages)
    {
        bool isFeasable =
            base.CandidateCanvasFeasable(
                canvasMaxWidth, canvasMaxHeight, bestSpriteArea, totalAreaAllImages,
                out candidateBiggerThanBestSprite, out candidateSmallerThanCombinedImages);

        IterationStats stats = null;

        if (candidateBiggerThanBestSprite)
        {
            stats = new IterationStats
            {
                Result = IterationResult.BiggerThanBestSprite,
                MaxCanvasWidth = canvasMaxWidth,
                MaxCanvasHeight = canvasMaxHeight
            };
        }
        else if (candidateSmallerThanCombinedImages)
        {
            stats = new IterationStats
            {
                Result = IterationResult.SmallerThanCombinedImages,
                MaxCanvasWidth = canvasMaxWidth,
                MaxCanvasHeight = canvasMaxHeight
            };
        }

        if (stats != null)
        {
            _currentMappingIterationStats.Add(stats);
        }

        return isFeasable;
    }
}


