using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapper;

public class ImageInfo: IImageInfo
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public ImageInfo(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
