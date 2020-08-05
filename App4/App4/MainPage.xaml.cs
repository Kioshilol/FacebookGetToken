using System.Threading.Tasks;
using Xamarin.Forms;


namespace App4
{
    public partial class MainPage : ContentPage
    {
        private IAccessToken _token { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Login();
        }
        
        async void Login()
        {
            await Task.Delay(10000);
            _token = DependencyService.Get<IAccessToken>();
            _token.Get();
        }
    }
}