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

namespace XamarinAzureImageUploadApp
{
    public partial class MainPage : ContentPage
    {
        string storageConnectionString = "";

        string fileName = $"{Guid.NewGuid()}.png";

        BlobServiceClient client;
        BlobContainerClient containerClient;
        BlobClient blobClient;

        public MainPage()
        {
            InitializeComponent();
        }

        private FileResult _photo;
        private string URL { get; set; }

        protected async override void OnAppearing()
        {
            string containerName = $"couponimages2{Guid.NewGuid()}";

            client = new BlobServiceClient(storageConnectionString);

            containerClient = await client.CreateBlobContainerAsync(containerName);

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
                UploadImage(stream); 
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
