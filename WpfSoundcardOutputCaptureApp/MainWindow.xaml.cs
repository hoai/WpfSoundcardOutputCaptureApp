using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSoundcardOutputCaptureApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WasapiLoopbackCapture capture;
        private WaveFileWriter writer;
        private Timer timer;
        private double duration;
        private string fileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void recordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new SaveFileDialog();
                dlg.FileName = "sample";
                dlg.DefaultExt = ".wav";
                dlg.Filter = "wav file (.wav)|*.wav";
                dlg.RestoreDirectory = true;

                var dlgResult = dlg.ShowDialog(this);
                if (dlgResult.HasValue && dlgResult.Value)
                {
                    fileName = dlg.FileName;

                    duration = 0;
                    timer = new Timer(1000);
                    timer.Elapsed += (s, args) =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            durationTextBlock.Text = TimeSpan.FromSeconds(duration++).ToString();
                        });
                    };

                    timer.Start();
                    capture = new WasapiLoopbackCapture();
                    writer = new WaveFileWriter(fileName, capture.WaveFormat);

                    capture.DataAvailable += Capture_DataAvailable;
                    capture.RecordingStopped += Capture_RecordingStopped;

                    capture.StartRecording();
                    recordButton.IsEnabled = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private void Capture_RecordingStopped(object sender, StoppedEventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {
            }
        }

        private async void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                await writer.WriteAsync(e.Buffer, 0, e.BytesRecorded);
            }
            catch (Exception)
            {
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                durationTextBlock.Text = TimeSpan.FromSeconds(0).ToString();
                timer.Stop();
                recordButton.IsEnabled = true;

                capture.StopRecording();
                capture.Dispose();
                writer.Dispose();
                timer.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void openFolderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrWhiteSpace(fileName))
                {
                    var dir = System.IO.Path.GetDirectoryName(fileName);
                    if (System.IO.Directory.Exists(dir))
                    {
                        Process.Start("explorer.exe", dir);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
