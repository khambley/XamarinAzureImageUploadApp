using System;
using System.Threading.Tasks;
using XamarinAzureImageUploadApp.Services;
using Android.Graphics;
using System.IO;
using Xamarin.Forms;
using XamarinAzureImageUploadApp.Droid.Services;

[assembly: Dependency(typeof(ImageResizerService))]
namespace XamarinAzureImageUploadApp.Droid.Services
{
    public class ImageResizerService : IImageResizerService
    {
        public ImageResizerService()
        {

        }
        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            return ResizeImageAndroid(imageData, width, height);
        }

        public byte[] ResizeImageAndroid(byte[] imageData, float width, float height)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
                return ms.ToArray();
            }
        }
    }
}
