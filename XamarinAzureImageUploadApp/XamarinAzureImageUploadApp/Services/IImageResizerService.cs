using System;
using System.Threading.Tasks;

namespace XamarinAzureImageUploadApp.Services
{
    public interface IImageResizerService
    {
        byte[] ResizeImage(byte[] imageData, float width, float height);
    }
}
