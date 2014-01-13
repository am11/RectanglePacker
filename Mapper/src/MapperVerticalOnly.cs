using System.Collections.Generic;

namespace Mapper
{
    public class MapperVerticalOnly<S> : IMapper<S> where S : class, ISprite, new()
    {
        /// <summary>
        /// Produces a mapping where all images are placed vertically.
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public S Mapping(IEnumerable<IImageInfo> images)
        {
            S spriteInfo = new S();
            int yOffset = 0;

            foreach (IImageInfo image in images)
            {
                MappedImageInfo imageLocation = new MappedImageInfo(0, yOffset, image);
                spriteInfo.AddMappedImage(imageLocation);
                yOffset += image.Height;
            }

            return spriteInfo;
        }

    }
}
