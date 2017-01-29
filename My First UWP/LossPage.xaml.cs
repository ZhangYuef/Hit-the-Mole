using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace My_First_UWP
{
	public sealed partial class LossPage : Page
	{
		private MainPage mainFrame;

		public LossPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			mainFrame = e.Parameter as MainPage;
			sb1_1.Begin();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			mainFrame.BegGame();
		}

		private void DoubleAnimation_Completed(object sender, object e)
		{
			btn.Visibility = Visibility.Visible;
			sb2_1.Begin();
			sb1_2.Begin();
		}

		private void DoubleAnimation_Completed_1(object sender, object e)
		{
			sb2_2.Begin();
		}

		private void DoubleAnimation_Completed_2(object sender, object e)
		{
			sb2_1.Begin();
		}
	}
}
