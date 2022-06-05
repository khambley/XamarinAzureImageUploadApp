using System;
using System.Threading.Tasks;
using XamarinAzureImageUploadApp.Services;
using System.Drawing;
using UIKit;
using CoreGraphics;
using Xamarin.Forms;
using XamarinAzureImageUploadApp.iOS.Services;

[assembly: Dependency(typeof(ImageResizerService))]
namespace XamarinAzureImageUploadApp.iOS.Services
{
    public class ImageResizerService : IImageResizerService
    {
        public ImageResizerService()
        {

        }
        
        public byte[] ResizeImage(byte[] imageData, float width, float height)
        {
            return ResizeImageIOS(imageData, width, height);

        }

        public byte[] ResizeImageIOS(byte[] imageData, float width, float height)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            UIImageOrientation orientation = originalImage.Orientation;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                                                 (int)width, (int)height, 8,
                                                 4 * (int)width, CGColorSpace.CreateDeviceRGB(),
                                                 CGImageAlphaInfo.PremultipliedFirst))
            {
                RectangleF imageRect = new RectangleF(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage(), 0, orientation);

                // save the image as a jpeg
                return resizedImage.AsJPEG().ToArray();
            }
        }
        public UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }
            UIImage image;

            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }

            return image;
        }
    }
}
