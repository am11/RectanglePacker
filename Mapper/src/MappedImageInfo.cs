
namespace Mapper
{
    public class MappedImageInfo : IMappedImageInfo
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public IImageInfo ImageInfo { get; private set; }

        public MappedImageInfo(int x, int y, IImageInfo imageInfo)
        {
            X = x;
            Y = y;
            ImageInfo = imageInfo;
        }
    }
}
