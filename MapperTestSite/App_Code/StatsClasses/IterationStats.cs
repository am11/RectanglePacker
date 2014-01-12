using System.Collections.Generic;

/// <summary>
/// Summary description for IterationStats
/// </summary>
public class IterationStats
{
	public IterationStats()
	{
	}

    public IterationResult Result { get; set; }
    public int MaxCanvasWidth { get; set; }
    public int MaxCanvasHeight { get; set; }
    public int IntermediateSpriteWidth { get; set; }
    public int IntermediateSpriteHeight { get; set; }
    public List<ImagePlacementDetails> ImageDetails { get; set; }
}


