using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace My_First_UWP
{
	public sealed partial class GamePage : Page
    {
        private List<BitmapImage> imageList = new List<BitmapImage>();
        private List<BitmapImage> imageList2 = new List<BitmapImage>();
        private List<int> imgType = new List<int>();
        private List<Button> btnList = new List<Button>();

        private bool isQuit = false;
        private int cHit;

        private ReaderWriterLockSlim rwl;
        private MainPage mainFrame;

        public GamePage()
        {
            this.InitializeComponent();
        }

        private async Task Keeper()
        {
            var rand = new Random(DateTime.Now.Millisecond);

            while (!isQuit)
            {
                rwl.EnterWriteLock();
                imgType.Clear();
                imgType.Add(0);

                var cMole = 0;
                var cBomb = 0;

                for (int i = 1; i <= 9; ++i)
                {
                    var index = rand.Next(imageList.Count);
                    imgType.Add(index);
                    if (index == 1)
                        cBomb++;
                    if (index == 0)
                        cMole++;
                    (btnList[i].Content as Image).Source = imageList[index];
                }

                var cThreshold = rand.Next(4) + 1;
                while (cMole > cThreshold)
                {
                    var i = rand.Next(9) + 1;
                    if (imgType[i] == 0)
                    {
                        cMole--;
                        var index = imageList.Count - 1;
                        (btnList[i].Content as Image).Source = imageList[index];
                        imgType[i] = index;
                    }
                }

                var cThreshold2 = rand.Next(2 + ((cHit > 50) ? 2 : 0)) + 1;
                while (cBomb > cThreshold2)
                {
                    var i = rand.Next(9) + 1;
                    if (imgType[i] == 1)
                    {
                        cBomb--;
                        var index = imageList.Count - 1;
                        (btnList[i].Content as Image).Source = imageList[index];
                        imgType[i] = index;
                    }
                }
                rwl.ExitWriteLock();
                await Task.Delay((int)(1000 / (Math.Log10(cHit + 1) / 3 + 1) + 250));
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainFrame = e.Parameter as MainPage;
            rwl = new ReaderWriterLockSlim();

            btnList.Add(new Button());
            btnList.Add(b1);
            btnList.Add(b2);
            btnList.Add(b3);
            btnList.Add(b4);
            btnList.Add(b5);
            btnList.Add(b6);
            btnList.Add(b7);
            btnList.Add(b8);
            btnList.Add(b9);

            // 0
            imageList.Add(new BitmapImage(new Uri("ms-appx:///Assets/mole.png")));
            imageList2.Add(new BitmapImage(new Uri("ms-appx:///Assets/mole-2.png")));
            // 1
            imageList.Add(new BitmapImage(new Uri("ms-appx:///Assets/bomb.png")));
            imageList2.Add(new BitmapImage(new Uri("ms-appx:///Assets/boom.png")));
            // ..
            imageList.Add(new BitmapImage(new Uri("ms-appx:///Assets/hole.png")));

            cHit = 0;
            isQuit = false;

            sb_load.Begin();
        }

        private async Task PlaySound(string sound)
        {
            MediaElement a = new MediaElement();
            Windows.Storage.StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            Windows.Storage.StorageFile file = await folder.GetFileAsync(sound);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            a.SetSource(stream, file.ContentType);
            a.Play();
        }

        private void DoubleAnimation_Completed(object sender, object e)
        {
            mainFrame.StartTimer();

            // Leave it running, don't wait
            Keeper();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            isQuit = true;
        }

        private async Task checkHit(int i)
        {
            rwl.EnterReadLock();
            if (imgType[i] == 0)
            {
                mainFrame.UpdateScore(1);
                imgType[i] = 2;
                (btnList[i].Content as Image).Source = imageList2[0];
				await PlaySound("hSound.wav");
                cHit++;
            }
            else if (imgType[i] == 1)
            {
                mainFrame.UpdateScore(-1);
                imgType[i] = 2;
                (btnList[i].Content as Image).Source = imageList2[1];
				await PlaySound("bomb.wav");
            }
            mainFrame.UpdateScore(0);
            rwl.ExitReadLock();
        }

        private async void b1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(1);
        }

        private async void b2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(2);
        }

        private async void b3_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(3);
        }

        private async void b4_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(4);
        }

        private async void b5_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(5);
        }

        private async void b6_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(6);
        }

        private async void b7_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(7);
        }

        private async void b8_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(8);
        }

        private async void b9_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await checkHit(9);
        }
    }
}
