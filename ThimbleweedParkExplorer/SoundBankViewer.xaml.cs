using NAudio.Vorbis;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThimbleweedLibrary;

namespace ThimbleweedParkExplorer
{
    /// <summary>
    /// Interaction logic for SoundBankViewer.xaml
    /// </summary>
    public partial class SoundBankViewer : UserControl
    {
        private FMODBankExtractor extractor = null;

        public SoundBankViewer()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public ObservableCollection<string> SoundFiles { get; set; } = new ObservableCollection<string>();

        public void SetBank(MemoryStream fmodBank)
        {
            DisposeAudio();
            extractor = new FMODBankExtractor(fmodBank);
            extractor.LogEvent += Extractor_LogEvent;

            SoundFiles.Clear();
            foreach (var file in extractor.EnumerateFiles().OrderBy(fn => fn)) SoundFiles.Add(file);
        }

        private void saveAllFiles_Click(object sender, RoutedEventArgs e)
        {
            using (var openFolder = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog())
            {
                openFolder.AllowNonFileSystemItems = true;
                openFolder.Multiselect = false;
                openFolder.IsFolderPicker = true;
                openFolder.Title = "Select a folder to save the audio into";

                if (openFolder.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                    return;
                using (MemoryStream ms = new MemoryStream())
                {
                    extractor.SaveAllToDir(openFolder.FileName);
                }
            }
        }

        private void saveSingleFile(object sender, RoutedEventArgs e)
        {
            string filename = ((sender as FrameworkElement)?.DataContext as string);
            if (string.IsNullOrWhiteSpace(filename)) return;

            byte[] data;
            try
            {
                data = extractor.ExtractSingleFile(filename, out string ext);
                filename = $"{filename}.{ext}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            using (var sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.FileName = filename;
                sfd.Title = "Select a location to save the audio file";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                }
            }
        }

        public delegate void OnLog(string log);
        public event OnLog LogEvent;

        private void Extractor_LogEvent(object sender, StringEventArgs e)
        {
            LogEvent?.Invoke(e.Message);
        }

        private void soundSelected(object sender, SelectionChangedEventArgs e)
        {
            DisposeAudio();
            playSound_click(lvSounds, null);
        }

        private WaveOutEvent outputDevice;
        private WaveStream audioReader;
        private MemoryStream audioDataStream;
        private System.Timers.Timer SoundProgressTimer;

        private void playSound_click(object sender, RoutedEventArgs e)
        {
            DisposeAudio();

            audioDataStream = new MemoryStream();
            var filename = lvSounds.SelectedItem as string;
            if (filename == null) return;

            string extension = "";
            try
            {
                var data = extractor.ExtractSingleFile(filename, out extension);
                audioDataStream = new MemoryStream(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            outputDevice = new WaveOutEvent();

            if (extension == "ogg")
            {
                audioReader = new VorbisWaveReader(audioDataStream);
            }
            else if (extension == "wav")
            {
                audioReader = new WaveFileReader(audioDataStream);
            }
            else throw new InvalidOperationException("Not a correct audio file type.");


            outputDevice.Init(audioReader);
            outputDevice.Play();
            outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;

            SoundProgressTimer = new System.Timers.Timer(1000)
            {
                AutoReset = true
            };
            SoundProgressTimer.Elapsed += SoundProgress;
            SoundProgressTimer.Start();

            sliderProgress.Minimum = 0;
            sliderProgress.Maximum = audioReader.TotalTime.TotalSeconds;
            sliderProgress.Value = 0;
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (outputDevice != null && outputDevice == sender)
            {
                DisposeAudio();
            }
        }

        private void SoundProgress(object sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                myProgressUpdate = true;
                sliderProgress.Value = audioReader?.CurrentTime.TotalSeconds ?? 0;
                myProgressUpdate = false;
            });
        }

        private void pauseSound_click(object sender, RoutedEventArgs e)
        {
            if (outputDevice != null)
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing) outputDevice.Pause();
                else if (outputDevice.PlaybackState == PlaybackState.Paused) outputDevice.Play();
            }
        }

        private void DisposeAudio()
        {
            if (outputDevice != null)
            {
                if (outputDevice.PlaybackState == PlaybackState.Playing) outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }
            if (audioReader != null)
            {
                audioReader.Dispose();
                audioReader = null;
            }
            if (audioDataStream != null)
            {
                audioDataStream.Dispose();
                audioDataStream = null;
            }

            if (SoundProgressTimer != null)
            {
                SoundProgressTimer.Stop();
                SoundProgressTimer = null;
            }

            sliderProgress.Value = 0;
        }


        bool myProgressUpdate = false;
        private void sliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!myProgressUpdate)
            {
                if (audioReader != null)
                {
                    try
                    {
                        audioReader.CurrentTime = TimeSpan.FromSeconds(sliderProgress.Value);
                    }
                    catch (Exception)
                    {
                        // sometimes the seek fails.
                    }
                }
                else
                {
                    myProgressUpdate = true;
                    sliderProgress.Value = 0;
                    myProgressUpdate = false;
                }
            }

            UpdateLabels();
        }

        private void UpdateLabels()
        {
            if (audioReader == null)
            {
                currentTime.Text = totalTime.Text = "00:00";
            }
            else
            {
                currentTime.Text = $"{(int)audioReader.CurrentTime.TotalMinutes:D2}:{audioReader.CurrentTime.Seconds:D2}";
                totalTime.Text = $"{(int)audioReader.TotalTime.TotalMinutes:D2}:{audioReader.TotalTime.Seconds:D2}";
            }
        }
    }
}
