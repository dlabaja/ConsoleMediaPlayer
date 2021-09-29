using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using System.Timers;
using System.Diagnostics;
using System.IO;

internal class ConsoleMediaPlayer
{
    private static string output = Path.Combine(Path.GetTempPath(), "CMP", "sample");
    private static MemoryStream media;
    private static Bitmap img;
    private static int numberOfFrames;
    private static System.Timers.Timer t;
    private static int i = 0;

    private async static Task Main(string[] args)
    {
        Console.Title = "Console Media Player";
        Console.SetBufferSize(128, 64);
        var a = new ConsoleMediaPlayer();
        await a.Converter();
        img = (Bitmap)Image.FromStream(media);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        a.GetGifFrames();
        Console.ReadKey();
    }

    private async Task Converter()
    {
        Console.WriteLine("Načítám data...");
        Console.WriteLine("Zadejte cestu k .mp4 souboru");
        string input = Console.ReadLine();

        while (!Path.IsPathFullyQualified(input))
        {
            Console.WriteLine("Neplatná cesta! Zkuste to prosím znovu.");
            input = Console.ReadLine();
        }

        Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "CMP"));

        media = new MemoryStream();
        await FFmpeg.Conversions.New().Start($"-y -i \"{ input }\" -s 64x64 -aspect 1:1 -pix_fmt monow -filter:v fps=25 \"{output}.gif\"");
        using (FileStream fs = File.OpenRead(output + ".gif"))
        {
            fs.CopyTo(media);
            fs.Close();
        }
        Console.WriteLine("Načten obraz");

        await FFmpeg.Conversions.New().Start($"-y -i \"{ input }\" \"{ output }.wav\"");
        Console.WriteLine("Načten zvuk");

        SoundPlayer player = new SoundPlayer();
        player.SoundLocation = output + ".wav";
        player.Play();
        Console.Clear();
    }

    private void GetGifFrames()
    {
        numberOfFrames = img.GetFrameCount(FrameDimension.Time);

        t = new System.Timers.Timer(1000 / 25);
        t.Elapsed += OnNewFrame;
        t.Start();
    }

    private void OnNewFrame(object sender, ElapsedEventArgs e)
    {
        i++;
        if (i >= numberOfFrames)
            End();

        Bitmap[] frames = new Bitmap[numberOfFrames];
        Console.WriteLine();

        img.SelectActiveFrame(FrameDimension.Time, i);
        frames[i] = (Bitmap)img.Clone();
        GetColor(frames[i]);
        frames[i].Dispose();
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
        Console.Write(barvy);
    }

    private void End()
    {
        t.Stop();
        t.Dispose();
        Console.Clear();
        Console.WriteLine("\nhttps://github.com/dlabaja/ConsoleMediaPlayer");
    }
}