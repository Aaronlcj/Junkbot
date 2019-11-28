using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oddmatics.Rzxe.Windowing.Graphics
{
    public class GifImage
    {
        private Image gifImage;
        private FrameDimension dimension;
        private int frameCount;
        private int currentFrame = -1;
        private bool reverse;
        private int step = 1;
        public int FrameCount;
        public FrameDimension Dimension;
        public GifImage(string gifName)
        {
            gifImage = Image.FromFile(Environment.CurrentDirectory + $@"\Content\RippedVideos\{gifName}.gif"); //initialize
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation
            FrameCount = frameCount;
        }

        public Image[] getFrames()
        {
            int numberOfFrames = gifImage.GetFrameCount(FrameDimension.Time);
            Image[] frames = new Image[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                gifImage.SelectActiveFrame(FrameDimension.Time, i);
                frames[i] = ((Image)gifImage.Clone());
            }

            return frames;
        }
    }
}
