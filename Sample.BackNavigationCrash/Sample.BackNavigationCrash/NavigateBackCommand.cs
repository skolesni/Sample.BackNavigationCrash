
namespace Sample.BackNavigationCrash
{
    using System;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class NavigateBackCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var app = Application.Current as Sample.BackNavigationCrash.App;
            if (app == null)
            {
                return;
            }

            await app.GoBackAsync();
        }
    }
}
