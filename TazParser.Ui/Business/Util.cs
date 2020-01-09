using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TazParser.Ui.Business
{
    public static class Util
    {
        public static void ClearFolder(string folderPath)
        {
            var dir = new DirectoryInfo(folderPath);

            if (!dir.Exists) return;

            foreach (var fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (var di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

        public static void CreateFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static void WriteImageFiles(IEnumerable<Image> images, string folderPath)
        {
            var i = 1;
            foreach (var image in images)
            {
                var imageName = i < 10 ? $"image_0{i}.jpeg" : $"image_{i}.jpeg";
                var path = Path.Combine(folderPath, imageName);
                image.Save(path, ImageFormat.Jpeg);
                i++;
            }
            
        }
    }
}
