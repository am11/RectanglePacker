using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapper;
using System.Drawing;
using System.Web;
using System.Drawing.Imaging;
using System.IO;


/// <summary>
/// This version of Mapper.Canvas writes an image to the web site's root folder
/// after each time a rectangle is added to the canvas.
/// </summary>
public class CanvasWritingStepImages : Canvas
{
    private const string _canvasFolderName = "canvasimages";
    
    // Used to give each canvas image a unique name.
    private static int _sequenceNbr = 0;

    private List<ImagePlacementDetails> _imageDetails = null;

    private int _nbrRectanglesAdded = 0;
    private int _nbrRedimensions = 0;

    private int _canvasWidth = 0;
    private int _canvasHeight = 0;

    private int _imageZoom = 1;
    private bool _showCanvasImages = false;

    /// <summary>
    /// The images that have been generated.
    /// </summary>
    public List<ImagePlacementDetails> ImageDetails { get { return _imageDetails; } }

    public CanvasWritingStepImages(int imageZoom, bool showCanvasImages)
    {
        _imageZoom = imageZoom;
        _showCanvasImages = showCanvasImages;

        ResetCanvasImagesFolder();
    }

    public override void SetCanvasDimensions(int canvasWidth, int canvasHeight)
    {
        // Don't use a canvas size that is too big, otherwise the images get too big

        if (canvasWidth > 500) { canvasWidth = 500; }
        if (canvasHeight > 500) { canvasHeight = 500; }

        _canvasWidth = canvasWidth;
        _canvasHeight = canvasHeight;

        _nbrRedimensions++;
        
        base.SetCanvasDimensions(canvasWidth, canvasHeight);
        _imageDetails = new List<ImagePlacementDetails>();
    }

    /// <summary>
    /// See ICanvas.
    /// </summary>
    public override bool AddRectangle(int width, int height, out int xOffset, out int yOffset, out int lowestFreeHeightDeficit)
    {
        bool rectangleAdded = base.AddRectangle(width, height, out xOffset, out yOffset, out lowestFreeHeightDeficit);

        // ------------

        Rectangle rectAdded = new Rectangle(xOffset, yOffset, width, height);

        if (rectangleAdded)
        {
            _nbrRectanglesAdded++;

            if (_showCanvasImages)
            {
                int bitmapWidth = _canvasWidth * _imageZoom;
                int bitmapHeight = _canvasHeight * _imageZoom;
                using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        int xImageOffset = 0;

                        for (int x = 0; x < CanvasCells.NbrColumns; x++)
                        {
                            int yImageOffset = 0;
                            for (int y = 0; y < CanvasCells.NbrRows; y++)
                            {
                                int cellWidth = CanvasCells.ColumnWidth(x);
                                int cellHeight = CanvasCells.RowHeight(y);
                                Rectangle rectCell = new Rectangle(xImageOffset, yImageOffset, cellWidth, cellHeight);
                                bool newlyOccupied = rectAdded.Contains(rectCell);

                                DrawRectangle(
                                    graphics,
                                    cellWidth * _imageZoom, cellHeight * _imageZoom,
                                    xImageOffset * _imageZoom, yImageOffset * _imageZoom,
                                    CanvasCells.Item(x, y).occupied,
                                    newlyOccupied);

                                yImageOffset += CanvasCells.RowHeight(y);
                            }

                            xImageOffset += CanvasCells.ColumnWidth(x);
                        }

                        string imageFileName = string.Format(_canvasFolderName + "/canvas_{0}_{1}_{2}.png", _nbrRedimensions, _nbrRectanglesAdded, _sequenceNbr);
                        _sequenceNbr++;
                        string imageServerFileName = HttpContext.Current.Server.MapPath(imageFileName);

                        ImagePlacementDetails singleImageDetails = new ImagePlacementDetails()
                        {
                            Width = width,
                            Height = height,
                            XOffset = xOffset,
                            YOffset = yOffset,
                            ImageUrl = imageFileName
                        };

                        _imageDetails.Add(singleImageDetails);

                        bitmap.Save(imageServerFileName, ImageFormat.Png);
                    }
                }
            }
        }
        else if (_showCanvasImages)
        {
            // Rectangle could not be placed. Show it on its own to make it easier to see how it relates to the canvas.

            int bitmapWidth = width * _imageZoom;
            int bitmapHeight = height * _imageZoom;
            using (Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    DrawRectangle(
                        graphics, bitmapWidth, bitmapHeight, 0, 0, true,
                        true);

                    string imageFileName = string.Format(_canvasFolderName + "/canvas_{0}_{1}_{2}.png", _nbrRedimensions, _nbrRectanglesAdded, _sequenceNbr);
                    _sequenceNbr++;
                    string imageServerFileName = HttpContext.Current.Server.MapPath(imageFileName);

                    ImagePlacementDetails singleImageDetails = new ImagePlacementDetails()
                    {
                        Width = width,
                        Height = height,
                        XOffset = -1,
                        YOffset = -1,
                        ImageUrl = imageFileName
                    };

                    _imageDetails.Add(singleImageDetails);

                    bitmap.Save(imageServerFileName, ImageFormat.Png);
                }
            }
        }

        return rectangleAdded;
    }

    /// <summary>
    /// Creates an image showing the structure of a sprite (that is, its constituent images).
    /// </summary>
    /// <param name="sprite">
    /// Sprite to be shown
    /// </param>
    /// <returns>
    /// Url of the generated image.
    /// </returns>
    public string SpriteToImage(ISprite sprite)
    {
        string imageFileName = null;

        using (Bitmap bitmap = new Bitmap(sprite.Width * _imageZoom, sprite.Height * _imageZoom))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // Draw border around the entire image
                DrawRectangle(
                    graphics,
                    sprite.Width * _imageZoom, sprite.Height * _imageZoom,
                    0, 0,
                    false, false);

                foreach (IMappedImageInfo mappedImageInfo in sprite.MappedImages)
                {
                    DrawRectangle(
                        graphics,
                        mappedImageInfo.ImageInfo.Width * _imageZoom, mappedImageInfo.ImageInfo.Height * _imageZoom,
                        mappedImageInfo.X * _imageZoom, mappedImageInfo.Y * _imageZoom,
                        true, false);
                }
            }

            imageFileName = string.Format(_canvasFolderName + "/canvas_finalsprite_{0}.png", _sequenceNbr);
            _sequenceNbr++;
            string imageServerFileName = HttpContext.Current.Server.MapPath(imageFileName);

            bitmap.Save(imageServerFileName, ImageFormat.Png);
        }

        return imageFileName;
    }

    /// <summary>
    /// Draws a rectangle on the canvas. The rectangle denotes a FreeArea.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="xOffset"></param>
    /// <param name="yOffset"></param>
    /// <param name="occupied"></param>
    private void DrawRectangle(Graphics graphics, int width, int height, int xOffset, int yOffset, bool occupied, bool newlyOccupied)
    {
        // Fill the rectangle
        Color c;
        if (occupied)
        {
            if (newlyOccupied)
            {
                c = Color.DarkGreen;
            }
            else
            {
                c = Color.LightGreen;
            }
        }
        else
        {
            c = Color.White;
        }

        SolidBrush brush = new SolidBrush(c);
        graphics.FillRectangle(brush, xOffset, yOffset, width, height);

        // Draw border 
        Pen pen = new Pen(Color.Black);
        graphics.DrawRectangle(pen, xOffset, yOffset, width - 1, height - 1);
    }

    /// <summary>
    /// Removes any canvas images folder, than creates an empty one.
    /// </summary>
    private void ResetCanvasImagesFolder()
    {
        string tildeSpriteUrlPath = "~/" + _canvasFolderName + "/";
        string resolvedUrlPath = VirtualPathUtility.ToAbsolute(tildeSpriteUrlPath);
        string fileSystemFolder = HttpContext.Current.Server.MapPath(resolvedUrlPath.Replace('\\', '/'));

        // Create the directory in case it doesn't exist. If it does, nothing happens.
        DirectoryInfo di = Directory.CreateDirectory(fileSystemFolder);

        // Delete all files in the directory in case it already existed
        FileInfo[] fiArr = di.GetFiles();
        foreach(FileInfo fi in fiArr)
        {
            fi.Delete();
        }
    }
}

