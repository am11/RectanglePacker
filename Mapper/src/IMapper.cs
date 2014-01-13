using System.Collections.Generic;

namespace Mapper
{
    /// <summary>
    /// An IMapper takes a series of images, and figures out how these could be combined in a sprite.
    /// It returns the dimensions that the sprite will have, and the locations of each image within that sprite.
    /// 
    /// This object does not create the sprite image itself. It only figures out how it needs to be constructed.
    /// </summary>
    public interface IMapper<S> where S : class, ISprite, new()
    {
        /// <summary>
        /// Works out how to map a series of images into a sprite.
        /// </summary>
        /// <param name="images">
        /// The list of images to place into the sprite.
        /// </param>
        /// <returns>
        /// A SpriteInfo object. This describes the locations of the images within the sprite,
        /// and the dimensions of the sprite.
        /// </returns>
        S Mapping(IEnumerable<IImageInfo> images);
    }
}
