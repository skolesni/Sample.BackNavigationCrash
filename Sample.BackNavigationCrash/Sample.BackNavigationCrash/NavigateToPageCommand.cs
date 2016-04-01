
namespace Sample.BackNavigationCrash
{
    using System;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class NavigateToPageCommand : ICommand
    {
        public Page Page { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.Page != null;
        }

        public async void Execute(object parameter)
        {
            if (this.Page == null)
            {
                return;
            }

            var app = Application.Current as Sample.BackNavigationCrash.App;
            if (app == null)
            {
                return;
            }

            await app.NavigateAsync(this.Page);
        }
    }
}
