using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class BadApple
{
    private static void Main(string[] args)
    {
        Console.Title = "Console Media Player";
        Console.SetBufferSize(128, 64);
        var a = new BadApple();
        var bitmap = (Bitmap)Image.FromFile("badapple64.gif");
        var result = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format1bppIndexed);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        a.GetGifFrames(bitmap);
        Console.ReadKey();
    }

    private void GetGifFrames(Bitmap img)
    {
        var numberOfFrames = img.GetFrameCount(FrameDimension.Time);
        Bitmap[] frames = new Bitmap[numberOfFrames];

        for (int i = 0; i < numberOfFrames; i++)
        {
            Console.WriteLine(i);
            img.SelectActiveFrame(FrameDimension.Time, i);
            Console.WriteLine(frames[i]);
            frames[i] = (Bitmap)img.Clone();
            GetColor(frames[i]);
            frames[i].Dispose();
        }
    }

    private void GetColor(Bitmap frame)
    {
        string barvy = "";
        for (int y = 0; y < frame.Height; y++)
        {
            for (int x = 0; x < frame.Width; x++)
            {
                Color clr = frame.GetPixel(x, y);
                if (clr.B < 255 && clr.R < 255 && clr.G < 255)
                {
                    barvy += "  ";
                    continue;
                }
                barvy += "██";
            }
            barvy += "\n";
        }
        Thread.Sleep(1000 / 24);
        Console.Write(barvy);
    }
}