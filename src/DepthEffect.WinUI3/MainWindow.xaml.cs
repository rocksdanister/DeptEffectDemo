using ComputeSharp;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using TerraFX.Interop.Windows;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DepthEffect.WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private float2 mousePos = new(0, 0);

        public MainWindow()
        {
            this.InitializeComponent();
            shaderPanel.ShaderRunner = new DepthParallaxRunner(() => mousePos);
        }

        private void ShaderPanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(mainGrid);
            mousePos.X = (float)((float)point.Position.X / shaderPanel.ActualWidth);
            mousePos.Y = (float)((float)point.Position.Y / shaderPanel.ActualHeight);
        }

        private void ShaderPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Reset to center
            mousePos.X = mousePos.Y = 0;
        }
    }
}
