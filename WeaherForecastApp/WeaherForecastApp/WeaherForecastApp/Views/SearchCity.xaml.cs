using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WeaherForecastApp.Helper;
using WeaherForecastApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeaherForecastApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchCity : ContentPage
    {
        public SearchCity()
        {
            InitializeComponent();
        }

        private async Task SerializeListWeather(object sender, EventArgs e)
        {
            string GetCity = WeatherInput.Text.Trim();
            if (GetCity.Length == 0)
            {
               await DisplayAlert("Увага!","Введіть повідомлення!", "OK");
            }
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={GetCity}&appid=6c35672518cb2dd4628b45bb44f1c74c&units=metric";
            var results = await ApiCaller.Get(url);
            if (results.Successful)
            {
                try
                {
                    var WeatherCity = JsonConvert.DeserializeObject<WeatherInfo>(results.Rensponce);
                    GridData girdData = new GridData
                    {
                        TitleCity = WeatherCity.name.ToUpper(),
                        image = $"w{WeatherCity.weather[0].icon}",
                        TemperatureCity = Convert.ToInt32(WeatherCity.main.temp).ToString(),
                        DescriptionWeather = WeatherCity.weather[0].description.ToUpper()
                    };
                    Locations locationsCity = new Locations(girdData);
                    Label lableoutput = new Label
                    {
                        Text = "Місто додано!",
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        TextColor = Color.Black
                    };
                    MainContainer.Children.Add(lableoutput);
                    await Navigation.PushModalAsync(locationsCity);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Warning", ex.Message, "OK");
                }
            }
        }

        private async void ApplyBtn_Clicked(object sender, EventArgs e)
        {
            await SerializeListWeather(sender, e);
        }
    }
}