using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Microsoft.Graphics.Canvas.UI;

namespace DepthEffect.UWP
{
    public sealed partial class MainPage : Page
    {
        private CanvasBitmap image, depth;
        private PixelShaderEffect depthEffect;
        private Vector2 imageSize = new Vector2(1, 1);
        private Vector2 mousePos = new Vector2(0, 0);
        private Vector2 mouseOffset = new Vector2(0, 0);

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void CanvasControl_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(Canvas_CreateResourcesAsync(sender).AsAsyncAction());
        }

        private async Task Canvas_CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            // Load shader
            var shaderFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Shaders\\Depth.bin");
            depthEffect = new PixelShaderEffect(await GetBytesFromFile(shaderFile));

            // Load images
            var imageFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\Image1.jpg");
            image = await CanvasBitmap.LoadAsync(sender, imageFile.Path);
            var depthFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\Depth1.jpg");
            depth = await CanvasBitmap.LoadAsync(sender, depthFile.Path);
            imageSize = image.Size.ToVector2();

            depthEffect.Source2 = depth;
            depthEffect.Source1 = image;
            //depthEffect.Source1Mapping = SamplerCoordinateMapping.Offset;
            //depthEffect.MaxSamplerOffset = 100;
        }

        private void CanvasControl_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            // Is it better to calculate here?
        }

        void CanvasControl_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var size = sender.Size.ToVector2();

            mouseOffset.X += (-0.075f * mousePos.X - mouseOffset.X) * 0.08f;
            mouseOffset.Y += (-0.075f * mousePos.Y - mouseOffset.Y) * 0.08f;
            depthEffect.Properties["mouse"] = new Vector2(mouseOffset.X, mouseOffset.Y);
            depthEffect.Properties["intensity"] = new Vector2(0.75f, 1f);

            // Uniform, crop to hide edge distortions
            // Strech(size.Y / imageSize.Y + 0.25f, size.X / imageSize.X + 0.25f)
            float screenAspect = size.X / size.Y;
            float textureAspect = imageSize.X / imageSize.Y;
            var scale = textureAspect > screenAspect ? new Vector2(size.Y / imageSize.Y + 0.25f) : new Vector2(size.X / imageSize.X + 0.25f);
            args.DrawingSession.Transform = Matrix3x2.CreateScale(scale);

            // Center the output
            var offset = (size - imageSize * scale) / 2;
            args.DrawingSession.DrawImage(depthEffect, offset);
        }

        // This will not work if other controls ontop of canvas
        private void CanvasControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(mainGrid);
            mousePos.X = (float)(point.Position.X);
            mousePos.Y = (float)(point.Position.Y);
        }

        private void CanvasControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Reset to center
            mousePos.X = mousePos.Y = 0;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvasControl.RemoveFromVisualTree();
            this.canvasControl = null;

            //depthEffect?.Dispose();
            //image?.Dispose();
            //depth?.Dispose();
        }

        public async Task<byte[]> GetBytesFromFile(StorageFile file)
        {
            var stream = await file.OpenStreamForReadAsync();
            byte[] bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
    }
}
