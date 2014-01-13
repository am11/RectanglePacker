using System.Collections.Generic;

namespace Mapper
{
    public class MapperHorizontalOnly<S> : IMapper<S> where S : class, ISprite, new()
    {
        /// <summary>
        /// Produces a mapping where all images are placed horizontally.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public S Mapping(IEnumerable<IImageInfo> images)
        {
            S spriteInfo = new S();
            int xOffset = 0;

            foreach (IImageInfo image in images)
            {
                MappedImageInfo imageLocation = new MappedImageInfo(xOffset, 0, image);
                spriteInfo.AddMappedImage(imageLocation);
                xOffset += image.Width;
            }

            return spriteInfo;
        }

    }
}
