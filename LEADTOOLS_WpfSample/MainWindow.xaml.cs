using Leadtools;
using Leadtools.ImageProcessing;
using Leadtools.Windows.Media;
using LEADTOOLS_WpfSample.Models.Helpers;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
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

namespace LEADTOOLS_WpfSample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private GraphServiceClient graphClient { get; set; }

        private DriveItem currentFolder { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            LEADToolsSupport.SetLicense();

        }

        private async void connectOneDriveButton_Click(object sender, RoutedEventArgs e)
        {
            // OneDriveを操作するためのクラスを取得する
            // OneDriveはGraph APIを用いるのでgraphという名前が頻出します。
            this.graphClient = AuthenticationHelper.GetAuthenticatedClient();

            if (this.graphClient == null)
            {
                messageText.Text = "OneDriveとの接続に失敗しました。AppIDnなどが間違っていないか確認ください。";
            }

            var expandValue = "thumbnails,children($expand=thumbnails)";

            // OneDriveのrootフォルダーの情報を取得する
            // 認証が済んでいない場合はログイン画面を表示する
            this.currentFolder = await this.graphClient.Drive.Root.Request().Expand(expandValue).GetAsync();

            if (this.currentFolder != null)
            {
                if (currentFolder.Folder != null && currentFolder.Children != null && currentFolder.Children.CurrentPage != null)
                {
                    listupImage(currentFolder.Children.CurrentPage);
                }

                // 接続に成功したら画像を選択させるためのボタンを表示する
                this.connectOneDriveButton.Visibility = Visibility.Collapsed;
                //this.selectImageButton.Visibility = Visibility.Visible;

                this.messageText.Text = "OneDriveに接続されました。アップロードする画像を選択してください。";
            }
        }

        private void listupImage(IList<DriveItem> items)
        {
            var imageList =  items.Where(item => item.Name.EndsWith(".jpg"));

            foreach(var tmpImage in imageList)
            {
                imageListBox.Items.Add(tmpImage.Name);
            }
        }

        private void leadtoolsImageButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = imageListBox.SelectedItem as string;

            downloadImage(fileName);
        }

        private async void downloadImage(string fileName)
        {
            foreach(var folder in this.currentFolder.Children.CurrentPage)
            {
                if (folder.Name.Equals( fileName))
                {
                    var stream = await this.graphClient.Drive.Items[folder.Id].Content.Request().GetAsync();

                    var imageSource = new BitmapImage();
                    imageSource.BeginInit();
                    imageSource.StreamSource = stream;
                    imageSource.EndInit();

                    this.image.Source = imageSource;
                }
            }
        }

        private void addEffectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RasterImage tempImage = RasterImageConverter.ConvertFromSource(this.image.Source, ConvertFromSourceOptions.None);

                // ここではグレイスケールを適応している。
                // その他のエフェクトについてはサンプルのImageProcessingDemoを参照
                // デモはC:\LEADTOOLS 19\Examples\以下にインストールされている
                GrayscaleCommand command = new GrayscaleCommand();
                command.BitsPerPixel = 8;
                command.Run(tempImage);

                this.changedImage.Source = RasterImageConverter.ConvertToSource(tempImage, ConvertToSourceOptions.None);
            }
            // 開発用ライセンスを正常に読み込ませていない場合
            // ex.Messageが「Kernel has expired」になる
            // 開発用ライセンスはC:\LEADTOOLS 19\Common\Licenseに配置されている
            catch (RasterException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
