using System.Collections.Generic;
using Mapper;

/// <summary>
/// Represents the contents of a sprite image.
/// </summary>
public class Sprite : ISprite
{
    private List<IMappedImageInfo> _mappedImages = null;
    private int _width = 0;
    private int _height = 0;

    /// <summary>
    /// Holds the locations of all the individual images within the sprite image.
    /// </summary>
    public IEnumerable<IMappedImageInfo> MappedImages { get { return _mappedImages; } }

    /// <summary>
    /// Width of the sprite image
    /// </summary>
    public int Width { get { return _width; } }

    /// <summary>
    /// Height of the sprite image
    /// </summary>
    public int Height { get { return _height; } }

    /// <summary>
    /// Area of the sprite image
    /// </summary>
    public int Area { get { return _width * _height; } }

    public Sprite()
    {
        _mappedImages = new List<IMappedImageInfo>();
        _width = 0;
        _height = 0;
    }

    /// <summary>
    /// Adds a Rectangle to the SpriteInfo, and updates the width and height of the SpriteInfo.
    /// </summary>
    /// <param name="imageLocation"></param>
    public void AddMappedImage(IMappedImageInfo imageLocation)
    {
        _mappedImages.Add(imageLocation);

        IImageInfo newImage = imageLocation.ImageInfo;

        int highestY = imageLocation.Y + newImage.Height;
        int rightMostX = imageLocation.X + newImage.Width;

        if (_height < highestY) { _height = highestY; }
        if (_width < rightMostX) { _width = rightMostX; }
    }

}
