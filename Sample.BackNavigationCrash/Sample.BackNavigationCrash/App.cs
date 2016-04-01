using Xamarin.Forms;

namespace Sample.BackNavigationCrash
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sample.BackNavigationCrash.Pages;

    public class App : Application
    {
        private INavigation navigationController;

        public App()
        {
            // The root page of your application
            AsyncHelpers.RunSynchronously(async () => await this.NavigateAsync(new ContentPage1()));
        }

        public async Task NavigateAsync(Page page)
        {
            if (this.MainPage == null)
            {
                this.MainPage = new NavigationPage(page);
                this.navigationController = this.MainPage.Navigation;
            }
            else
            {
                await this.navigationController.PushAsync(page);
            }
        }

        public async Task GoBackAsync()
        {
            if (this.navigationController?.NavigationStack.LastOrDefault() == null)
            {
                return;
            }

            await this.navigationController.PopAsync();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
