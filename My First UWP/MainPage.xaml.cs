using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace My_First_UWP
{
	public class MainPageViewModel : BindableBase
	{
		private int score = 0;
		private int lp = 5;

		public int Score
		{
			get { return score; }
			set { SetProperty(ref score, value); }
		}

		public int Lp
		{
			get { return lp; }
			set { SetProperty(ref lp, value); }
		}
	}

	/// <summary>
	/// 可用于自身或导航至 Frame 内部的空白页。
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private bool isStop = false;
		private DateTime tLastHit;
		private MainPageViewModel viewModel;

		public MainPage()
		{
			this.InitializeComponent();
			viewModel = new MainPageViewModel();
			this.DataContext = viewModel;
		}

		private async Task TimerKeeper()
		{
			var tBeg = DateTime.Now;
			tLastHit = DateTime.Now;

			const int cFreshRate = 30;
			const double maxTime = 5000;

			while (!isStop)
			{
				var diff = DateTime.Now - tBeg;
				timer.Text = (diff.Minutes < 10 ? "0" : "") + diff.Minutes
					+ ":" + (diff.Seconds < 10 ? "0" : "") + diff.Seconds;

				var tlast = (DateTime.Now - tLastHit).TotalMilliseconds;
				if (tlast <= maxTime)
					pcsBar.Value = 1 - (tlast / maxTime);
				else
				{
					KillTimer();
					pcsBar.Value = 0;
					title.Text = "Time OUT!";
					gameFrame.Navigate(typeof(LossPage), this);
				}

				await Task.Delay(1000 / cFreshRate);
			}
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			BegGame();
			this.SizeChanged += MainPage_SizeChanged;
		}

		private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width < e.NewSize.Height)
			{
				v1.SetValue(Grid.RowSpanProperty, 1);
				v1.SetValue(Grid.ColumnSpanProperty, 2);
				vswg.Orientation = Orientation.Horizontal;

				gameFrame.SetValue(Grid.RowProperty, 2);
				gameFrame.SetValue(Grid.ColumnProperty, 0);
				gameFrame.SetValue(Grid.ColumnSpanProperty, 2);
				gameFrame.SetValue(Grid.RowSpanProperty, 1);
			}
			else
			{
				v1.SetValue(Grid.RowSpanProperty, 2);
				v1.SetValue(Grid.ColumnSpanProperty, 1);
				vswg.Orientation = Orientation.Vertical;

				gameFrame.SetValue(Grid.RowProperty, 1);
				gameFrame.SetValue(Grid.ColumnProperty, 2);
				gameFrame.SetValue(Grid.ColumnSpanProperty, 1);
				gameFrame.SetValue(Grid.RowSpanProperty, 2);
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.SizeChanged -= MainPage_SizeChanged;
			KillTimer();
		}

		public void UpdateScore(int sc)
		{
			if (sc > 0)
			{
				viewModel.Score++;
			}
			else if (sc < 0)
			{
				viewModel.Lp--;
				if (viewModel.Lp == 0)
				{
					KillTimer();
					title.Text = "Booommm!!!";
					gameFrame.Navigate(typeof(LossPage), this);
				}
			}

			pcsBar.Value = 1;
			tLastHit = DateTime.Now;
		}

		public async Task StartTimer()
		{
			pcsBar.Value = 1;
			isStop = false;

			// Leave it running, don't wait
			await TimerKeeper();
		}

		public void KillTimer()
		{
			isStop = true;
			pcsBar.Value = 0;
		}

		public void BegGame()
		{
			viewModel.Score = 0;
			viewModel.Lp = 5;

			timer.Text = "00:00";

			title.Text = "Hit the Mole";
			gameFrame.Navigate(typeof(GamePage), this);
		}
	}
}
