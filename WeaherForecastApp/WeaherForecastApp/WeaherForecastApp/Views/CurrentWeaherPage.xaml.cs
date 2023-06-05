using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeaherForecastApp.Helper;
using WeaherForecastApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeaherForecastApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeaherPage : ContentPage
    {
        public CurrentWeaherPage()
        {
            InitializeComponent();
            GetCoordinates();
        }
        private string Location { get; set; } //= "San Francisco";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCity(location);

                    GetWeatherInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = places?.FirstOrDefault();

            if (currentPlace != null)
                return $"{currentPlace.Locality},{currentPlace.CountryName}";
            return null;
        }

        private async void GetBackground()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=1&page=1";
            var result = await ApiCaller.Get(url, "LUgeTKQvKoozOwGiUSG2tixDbtd8xwjmCVAXGnbzPB7xjh6dApSGlOeX");
            if (result.Successful)
            {
                var bgInfo = JsonConvert.DeserializeObject<Rootobject>(result.Rensponce);
                if (bgInfo != null && bgInfo.photos.Length > 0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(bgInfo.photos[new Random().Next(bgInfo.photos.Length - 1)].src.medium));
                } 
            }
        }

        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&appid=6c35672518cb2dd4628b45bb44f1c74c&units=metric";
            var results = await ApiCaller.Get(url);
            if (results.Successful)
            {
                try
                {
                    var WeatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(results.Rensponce);
                    descriptionTxt.Text = WeatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{WeatherInfo.weather[0].icon}";
                    cityTxt.Text = WeatherInfo.name.ToUpper();
                    temperatureTxt.Text = Convert.ToInt32(WeatherInfo.main.temp).ToString();
                    humidityTxt.Text = $"{WeatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{WeatherInfo.main.pressure}hpa";
                    windTxt.Text = $"{WeatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{WeatherInfo.clouds.all}%";

                    var dateTime = new DateTime().ToUniversalTime().AddSeconds(WeatherInfo.dt);
                    DayOfWeek dayOfWeek = DateTime.Today.DayOfWeek;
                    string DayOfWeekStr = dayOfWeek.ToString();
                    dateTxt.Text = DayOfWeekStr + dateTime.ToString($" MMM dd").ToString();
                    GetForecast();
                    GetBackground();
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");   
                }       
            }
            else
            {
                await DisplayAlert("Weather Info", "No Weather information found", "OK");
            }
        }
        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=6c35672518cb2dd4628b45bb44f1c74c&units=metric";
            var results = await ApiCaller.Get(url);
            if (results.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(results.Rensponce);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourTxt.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
             BtnLocations.IsEnabled = false;
             var locationPage = new Locations();
             await Navigation.PushModalAsync(locationPage);
             Device.StartTimer(TimeSpan.FromSeconds(1), () =>
             {
                BtnLocations.IsEnabled = true;
                return false;
             });
        }
    }
}