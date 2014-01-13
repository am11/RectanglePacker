using System.Collections.Generic;

namespace Mapper
{
    /// <summary>
    /// Represents the contents of a sprite image.
    /// </summary>
    public interface ISprite
    {
        /// <summary>
        /// Width of the sprite image
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the sprite image
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Area of the sprite image
        /// </summary>
        int Area { get; }

        /// <summary>
        /// Holds the locations of all the individual images within the sprite image.
        /// </summary>
        IEnumerable<IMappedImageInfo> MappedImages { get; }

        /// <summary>
        /// Adds an image to the SpriteInfo, and updates the width and height of the SpriteInfo.
        /// </summary>
        /// <param name="mappedImage"></param>
        void AddMappedImage(IMappedImageInfo mappedImage);

    }
}
