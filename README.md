# Xamarin Azure Image Upload App
I created an app to pick and capture photos and it uploads them to an Azure blob storage account.  
I needed to resize them because each photo was roughly 12GB (thanks to Apple's hi-res camera). The Xamarin.Essentials Media Picker API 
no longer supports the MediaSize option. :(

I stumbled across the [XamFormsImageResize](https://github.com/xamarin/xamarin-forms-samples/tree/main/XamFormsImageResize) example here, but it was quite old. It used the [Conditional Compilation](https://docs.microsoft.com/en-us/xamarin/cross-platform/app-fundamentals/building-cross-platform-applications/platform-divergence-abstraction-divergent-implementation#conditional-compilation) way of targeting difference platforms which is all but obsolete.  

For example, the usings at the top of the ImageResizer.cs class looked like this:  
```
using System;
using System.IO;
using System.Threading.Tasks;

#if __IOS__
using System.Drawing;
using UIKit;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
#endif
...
```
So, I researched it and discovered the **"new"** way of targeting different platforms is using the DependencyService pattern with interfaces. Basically, it goes like this:  
1. **Shared Project > Services > IImageResizerService.cs** Create an interface in the shared code like I did with IImageResizerService.cs.
2. **Xamarin.Android/Xamarin.iOS > Services > ImageResizerService.cs** Implement the interface in each required project, in my case, iOS and Android. I called mine, ImageResizerService. I used the native Xamarin.iOS and Xamarin.Android graphics packages in each implementation. Be sure to inherit from your interface, like mine, IImageResizerService.
3. **Xamarin.Android/Xamarin.iOS > Services > ImageResizerService.cs** Register the platform implementations with the DependencyService. There are several ways to do it, I chose to use the DependencyAttribute in each implementation.  
`[assembly: Dependency(typeof(ImageResizerService))]` 
4. **Shared Project > MainPage.xaml.cs** Resolve the platform implementations from the shared code, and invoke them using the DependencyService.Get<T> method. In my case, if you look at Mainpage.xaml.cs, line 128, I resolve and invoke the IImageResizerService on the same line.  

`byte[] resizedImage = DependencyService.Get<IImageResizerService>().ResizeImage(imageData, 1000, 1000);`
  
There is a more detailed explanation from Microsoft [here](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/dependency-service/introduction). Hope this helps someone from pulling their hair out! :)
