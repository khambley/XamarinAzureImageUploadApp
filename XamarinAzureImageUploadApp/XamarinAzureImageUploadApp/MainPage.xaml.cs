using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamarinAzureImageUploadApp.Services;

namespace XamarinAzureImageUploadApp
{
    public partial class MainPage : ContentPage
    {
        string storageConnectionString = "Your connection string here";

        string fileName = $"{Guid.NewGuid()}.png";

        BlobServiceClient client;
        BlobContainerClient containerClient;
        BlobClient blobClient;

        //IImageResizerService resizerService = DependencyService.Get<IImageResizerService>();

        public MainPage()
        {
            InitializeComponent();
        }

        private FileResult _photo;
        private string URL { get; set; }

        protected async override void OnAppearing()
        {
            //string containerName = $"couponimages2{Guid.NewGuid()}";
            string containerName = "couponimages1";

            client = new BlobServiceClient(storageConnectionString);

            //containerClient = await client.CreateBlobContainerAsync(containerName);
            containerClient = client.GetBlobContainerClient(containerName);

            resultsLabel.Text = "Container Created\n";

            blobClient = containerClient.GetBlobClient(fileName);

            btnUpload.IsEnabled = true;
        }

        private async void btnSelectPic_Clicked(object sender, EventArgs e)
        {
            try
            {
                _photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions 
                {
                    Title = "Select an image"
                });

                if (_photo == null) return;

                var stream = await _photo.OpenReadAsync();

                imageView.Source = ImageSource.FromStream(() => stream);

                UploadedUrl.Text = "Image URL:";
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Camera not supported", "Camera not supported on this device.", "OK");
            }
            catch (PermissionException psEx)
            {
                await DisplayAlert("Permissions Not Granted", "Permissions not granted to access photos", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async void btnCapturePic_Clicked(object sender, EventArgs e)
        {
            try
            {
                _photo = await MediaPicker.CapturePhotoAsync();

                if (_photo == null) return;

                var stream = await _photo.OpenReadAsync();

                imageView.Source = ImageSource.FromStream(() => stream);

                UploadedUrl.Text = "Image URL:";
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Camera not supported", "Camera not supported on this device.", "OK");
            }
            catch (PermissionException psEx)
            {
                await DisplayAlert("Permissions Not Granted", "Permissions not granted to access photos", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"{ex.Message}", "OK");
            }
        }

        private async void btnUpload_Clicked(object sender, EventArgs e)
        {
            if(_photo != null)
            {
                var stream = await _photo.OpenReadAsync();

                byte[] imageData;

                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    imageData = ms.ToArray();
                }

                //byte[] resizedImage = await resizerService.ResizeImage(imageData, 400, 400);

                byte[] resizedImage = DependencyService.Get<IImageResizerService>().ResizeImage(imageData, 1000, 1000);

                UploadImage(new MemoryStream(resizedImage)); 
            }
            else
            {
                await DisplayAlert("Error", "There was an error when trying to retrieve the image.", "OK");
                return;
            }
        }

        private async void UploadImage(Stream stream)
        {
            Busy();
            await containerClient.UploadBlobAsync(fileName, stream);
            resultsLabel.Text += "Blob Uploaded\n";
            URL = blobClient.Uri.OriginalString;
            UploadedUrl.Text = URL;
            NotBusy();
            await DisplayAlert("Uploaded", "Image uploaded to Blob storage successfully.", "OK");
        }
        public void Busy()
        {
            uploadIndicator.IsVisible = true;
            uploadIndicator.IsRunning = true;
            btnSelectPic.IsEnabled = false;
            btnCapturePic.IsEnabled = false;
            btnUpload.IsEnabled = false;
        }
        public void NotBusy()
        {
            uploadIndicator.IsVisible = false;
            uploadIndicator.IsRunning = false;
            btnSelectPic.IsEnabled = true;
            btnCapturePic.IsEnabled = true;
            btnUpload.IsEnabled = true;
        }
    }
}
