using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimplePlayer2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }

       


        private async void openFile_click(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaElement defined in XAML
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaplayer.SetSource(stream, file.ContentType);

                mediaplayer.Play();
            }
        }
        private void TxtFilePath_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)//在输入框输入按回车
            {
                TextBox tbPath = sender as TextBox;
                if (tbPath != null)
                {
                    LoadMediaFromString(tbPath.Text);
                }
            }
        }
        private void ButtonOpenUri_Click(object sender, RoutedEventArgs e)
        {
            TextBox path = txtFilePath;
            if (path != null)
            {
                LoadMediaFromString(path.Text);
            }
        }
        private void LoadMediaFromString(string path)
        {
            try
            {
                Uri pathUri = new Uri(path);
                mediaplayer.Source = pathUri;
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    // handle exception.
                    // For example: Log error or notify user problem with file
                }
            }
        }
        public async Task<StorageFile> Load()//下载mp3文件到电脑音乐库，下载mp4文件到视频库
        {
            try
            {
                var httpClient = new HttpClient();
                var uuri = new Uri(txtFilePath.Text);
                var buffer = await httpClient.GetBufferAsync(uuri);
                if (buffer != null && buffer.Length > 0u)
                {
                     string str="NewSong.mp3";
                     StorageFolder simplefile = KnownFolders.MusicLibrary;
                    if (txtFilePath.Text.EndsWith(".mp3"))
                        {
                        str = "NewSong.mp3";
                        simplefile = KnownFolders.MusicLibrary;
                    }
                    if (txtFilePath.Text.EndsWith(".mp4"))
                    {
                        str = "NewVideo.mp4";
                        simplefile = KnownFolders.VideosLibrary;
                    }
                    StorageFile file = await simplefile.CreateFileAsync(str, CreationCollisionOption.GenerateUniqueName);//在指定文件夹中创建文件，并且在文件夹中若已有该文件，那么文件名加数字后缀
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        await stream.FlushAsync();
                    }
                    return file;
                }
            }
            catch { }
            return null;
        }
        private async void ButtonOpenDownloadUri_Click(object sender, RoutedEventArgs e)
        {
            TextBox path = txtFilePath;
            if (path != null)
            {
                LoadMediaFromString(path.Text);
            }
            await Load();
        }
    }
}
