using Avalonia.Controls;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace DiabloLauncher.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //StartManualFadeOut();
        }

        private async void StartManualFadeOut()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var fadeDuration = 3.0;
            var interval = TimeSpan.FromMilliseconds(100);
            var steps = fadeDuration / interval.TotalSeconds;
            var opacityDecrement = 1.0 / steps;

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = interval
            };

            timer.Tick += (sender, e) =>
            {
                if (GreetingTextBlock.Opacity > 0)
                {
                    GreetingTextBlock.Opacity -= opacityDecrement;
                }
                else
                {
                    timer.Stop();
                }
            };

            timer.Start();
        }
    }
}
