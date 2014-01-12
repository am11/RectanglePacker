using System;

/// <summary>
/// Generates random sizes (which consist of a width and a height).
/// </summary>
public class RandomSizeGenerator
{
    private int _minWidth = 10;
    private int _maxWidth = 200;

    // If this is 70, than Height will be 30% to 170% of Width
    private int _maxAspectRatioDeviationPercentage = 70;


    private Random _r = null;

    public RandomSizeGenerator(int minWidth, int maxWidth, int maxAspectRatioDeviationPercentage)
	{
        _r = new Random();

        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _maxAspectRatioDeviationPercentage = maxAspectRatioDeviationPercentage;
	}

    /// <summary>
    /// Returns the next ImageInfo with random width and height
    /// </summary>
    /// <returns></returns>
    public ImageInfo Next()
    {
        int width = _r.Next(_minWidth, _maxWidth);
        int height =
            _r.Next(
                (int)Math.Round(((100 - _maxAspectRatioDeviationPercentage) * width) / (double)100),
                (int)Math.Round(((100 + _maxAspectRatioDeviationPercentage) * width) / (double)100));

        ImageInfo s = new ImageInfo(width, height);

        return s;
    }
}

