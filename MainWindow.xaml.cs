using System;
using System.Threading.Tasks;
using System.Windows;

namespace AsyncAwaitEventListeners
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel ViewModel { get; }
        
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }

        private static Task Wait()
        {
            return Task.Delay(300);
        }
        


        private async void AsyncVoidSuccessful(object sender, RoutedEventArgs e)
        {
            ViewModel.StartCall();
            await Wait();
            ViewModel.Successful();
        }

        private async void AsyncVoidHandleException(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.StartCall();
                await Wait();
                MainWindowViewModel.ThrowException();
            }
            catch (Exception)
            {
                ViewModel.Fail();
            }
        }

        private async void AsyncVoidDontHandleException(object sender, RoutedEventArgs e)
        {
            ViewModel.StartCall();
            await Wait();
            MainWindowViewModel.ThrowException();
        }


        private void VoidSuccessful(object sender, RoutedEventArgs e)
        {
            ViewModel.StartCall();
            ViewModel.Successful();
        }

        private void VoidHandleException(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.StartCall();
                MainWindowViewModel.ThrowException();
            }
            catch (Exception)
            {
                ViewModel.Fail();
            }
        }

        private void VoidDontHandleException(object sender, RoutedEventArgs e)
        {
            ViewModel.StartCall();
            MainWindowViewModel.ThrowException();
        }
    }
}