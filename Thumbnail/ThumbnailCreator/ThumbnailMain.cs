using System.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Thumbnail.ThumbnailCreator
{
    class ThumbnailMain
    {
        //private string pathWithImage = @"E:\xmen_wolverine\";
        //private string pathToSave = @"E:\output\";
        object _locker = new object();

        readonly string _pathWithImage = ConfigurationManager.AppSettings["Path"];
        readonly string _pathToSave = ConfigurationManager.AppSettings["NewPath"];

        public void Start()
        {
            for(int i = 0; i <  Directory.GetFiles(_pathWithImage, "*.jpg").Length; i++)
            {
                Console.WriteLine("Try");
                Task<Bitmap> resizeTask = new Task<Bitmap>(() => Resize(GetRandomImage(_pathWithImage)));
                Task renameAndSaveTask = resizeTask.ContinueWith(RenameAndSave);
                resizeTask.Start();
            }
        }

        private Bitmap Resize(Image image)
        {
            if (image != null)
            {
                Console.WriteLine($"Try to resize in Task: {Task.CurrentId}");

                Bitmap myImageBitmap = new Bitmap(image, 200, 400);
                myImageBitmap.SetResolution(600, 800);

                return myImageBitmap;
            }
            else
            {
                return null;
            }
        }

        private void RenameAndSave(Task<Bitmap> image)
        {
            if (image.Result != null)
            {
                Console.WriteLine($"Try to save in Task: {Task.CurrentId}(Previously: {image.Id}");

                Bitmap myImage = image.Result;
                myImage.Save($"{_pathToSave}{new Random().Next(9999999)}.jpg");

              if(IsJpgExist(_pathWithImage)) Start();
            }
        }

        private Image GetRandomImage(string path)
        {

            try
                {
                    string randomImagePath =
                        Directory.GetFiles(path, "*.jpg")[
                            new Random().Next(Directory.GetFiles(path, "*.jpg").Length - 1)];
                    Image myImage;

                    using (var imgStream = File.OpenRead(randomImagePath))
                        myImage = Image.FromStream(imgStream);

                    File.Delete(randomImagePath);

                    return myImage;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"end {Task.CurrentId}");
                    return null;
                }
        }

        private static bool IsJpgExist(string path)
        {
            try
            {
                if (Directory.GetFiles(path, "*.jpg").Length > 0) return true;
                else return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"The process failed: {e.ToString()}");
                return false;
            }
        }
    }
}
