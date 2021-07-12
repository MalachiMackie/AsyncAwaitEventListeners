using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncCommands;
using AsyncCommands.Abstraction;
using GalaSoft.MvvmLight.CommandWpf;
using PropertyChanged;

namespace AsyncAwaitEventListeners
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        private const string ContentString = "I Got Some Data!";
        private const string ErrorContentString = "I Failed To Get Data!";

        public ObservableCollection<string> Items { get; } = new ObservableCollection<string>();
        
        public MainWindowViewModel()
        {
            ClearCommand = new RelayCommand(() =>
            {
                Content = null;
                ErrorContent = null;
                IsBusy = false;
                Items.Clear();
            });

            AsyncSuccessfulCommand = new AsyncCommand(AsyncSuccessful);
            AsyncHandleExceptionCommand = new AsyncCommand(AsyncHandleException);
            AsyncDontHandleExceptionCommand = new AsyncCommand(AsyncDontHandleException);

            SyncSuccessfulCommand = new RelayCommand(SyncSuccessful);
            SyncHandleExceptionCommand = new RelayCommand(SyncHandleException);
            SyncDontHandleExceptionCommand = new RelayCommand(SyncDontHandleException);

            SyncAddItemCommand = new RelayCommand(SyncAddItem);
            AsyncAddItemCommand = new AsyncCommand(AsyncAddItem);
            AsyncAddItemFromTaskCommand = new AsyncCommand(AsyncAddItemFromTask);
            SyncAddItemFromTaskContinueWithCommand = new RelayCommand(SyncAddItemFromTaskContinueWith);
        }

        public bool IsBusy { get; set; }
        
        public IAsyncCommand AsyncSuccessfulCommand { get; }
        
        public IAsyncCommand AsyncHandleExceptionCommand { get; }
        
        public IAsyncCommand AsyncDontHandleExceptionCommand { get; }
        
        public ICommand SyncSuccessfulCommand { get; }
        
        public ICommand SyncHandleExceptionCommand { get; }
        
        public ICommand SyncDontHandleExceptionCommand { get; }
        
        public string Content { get; private set; }
        
        public string ErrorContent { get; private set; }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorContent);

        public ICommand ClearCommand { get; }

        public ICommand SyncAddItemCommand { get; }

        public IAsyncCommand AsyncAddItemCommand { get; }

        public IAsyncCommand AsyncAddItemFromTaskCommand { get; }
        public ICommand SyncAddItemFromTaskContinueWithCommand { get; }

        private void SyncAddItem()
        {
            Items.Add("An Item");
        }

        private async Task AsyncAddItem()
        {
            IsBusy = true;
            await Wait();
            Items.Add("An Item");
            IsBusy = false;
        }

        private async Task AsyncAddItemFromTask()
        {
            IsBusy = true;
            try
            {
                await Task.Run(async () =>
                {
                    await Wait();
                    Items.Add("An Item");
                });
            }
            catch (Exception e)
            {
                IsBusy = false;
                ErrorContent = e.Message;
            }
        }

        private void SyncAddItemFromTaskContinueWith()
        {
            IsBusy = true;
            Task.Run(async () =>
            {
                await Wait();
                return "An Item";
            }).ContinueWith(async result =>
            {
                Items.Add(await result);
                IsBusy = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        

        public void StartCall()
        {
            Content = null;
            ErrorContent = null;
            IsBusy = true;
        }

        public static void ThrowException()
        {
            throw new Exception();
        }

        public void Successful()
        {
            IsBusy = false;
            ErrorContent = null;
            Content = ContentString;
        }

        public void Fail()
        {
            IsBusy = false;
            ErrorContent = ErrorContentString;
            Content = null;
        }

        private static Task Wait()
        {
            return Task.Delay(300);
        }

        private void SyncSuccessful()
        {
            StartCall();
            Successful();
        }

        private void SyncHandleException()
        {
            try
            {
                StartCall();
                ThrowException();
            }
            catch (Exception)
            {
                Fail();
            }
        }

        private void SyncDontHandleException()
        {
            StartCall();
            ThrowException();
        }

        private async Task AsyncSuccessful()
        {
            StartCall();
            await Wait();
            Successful();
        }

        private async Task AsyncHandleException()
        {
            try
            {
                StartCall();
                await Wait();
                ThrowException();
            }
            catch (Exception)
            {
                Fail();
            }    
        }

        private static async Task AsyncDontHandleException()
        {
            await Wait();
            ThrowException();
        }
    }
}