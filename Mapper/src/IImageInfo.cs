
namespace Mapper
{
    /// <summary>
    /// Describes an image. Note that this only defines those properties that are relevant when
    /// it comes to mapping an image onto a sprite - its width and height. So for example image file
    /// name is not needed here.
    /// 
    /// This is called IImageInfo rather than IImage, because System.Drawing already defines an Image class.
    /// </summary>
    public interface IImageInfo
    {
        int Width { get; }
        int Height { get; }
    }
}

