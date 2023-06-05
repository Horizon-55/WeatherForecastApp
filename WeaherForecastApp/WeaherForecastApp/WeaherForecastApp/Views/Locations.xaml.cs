using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using WeaherForecastApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace WeaherForecastApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Locations : ContentPage
    {
        
        public Locations(GridData ObjectGridData)
        {
            InitializeComponent();
            base.OnAppearing();
            ChangeDate(NowDay);
            XMLSerializeGrid(ObjectGridData);
            List<GridData> deserializedGrids = DeserializeXML();
            GridData NewTempGrid = new GridData();
            NewTempGrid = deserializedGrids[0];
            Grid grid = new Grid {
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 0,
                HeightRequest = 100,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Margin = new Thickness(0,-10,0,500),
                Opacity = 0.4,
                BackgroundColor = Color.Silver
            };
            StackLayout StackContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };
            Label FullNameCityLable = new Label
            {
                Text = NewTempGrid.TitleCity,
                TextColor = Color.White,
                FontSize = 13,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(10,0,0,0),
                FontAttributes = FontAttributes.Bold,
                TextTransform = TextTransform.Uppercase
            };
            StackLayout StackContainerImage = new StackLayout {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };
            Image imageObj = new Image
            {
                Source = NewTempGrid.image,
                WidthRequest = 67,
                HeightRequest = 45
            };
            Label DescriptionWeather = new Label
            {
                Text = NewTempGrid.DescriptionWeather,
                TextColor = Color.White,
                FontSize = 13,
                HorizontalOptions = LayoutOptions.Center
            };
            StackLayout StackContainerTemperature = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 0
            };
            Label NameTemperature = new Label {
                Text = NewTempGrid.TemperatureCity,
                TextColor = Color.White,
                FontSize = 70,
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Italic
            };
            Label TempatureForName = new Label
            {
                Text = "°",
                TextColor = Color.White,
                FontSize = 50,
                VerticalOptions = LayoutOptions.Center
            };
            StackContainerTemperature.Children.Add(NameTemperature);
            StackContainerTemperature.Children.Add(TempatureForName);
            StackContainerImage.Children.Add(imageObj);
            StackContainerImage.Children.Add(DescriptionWeather);
            StackContainer.Children.Add(FullNameCityLable);
            StackContainer.Children.Add(StackContainerImage);
            StackContainer.Children.Add(StackContainerTemperature);
            grid.Children.Add(StackContainer);
            InputWeather.Children.Add(grid);
        }
        public Locations()
        {
            InitializeComponent();
        }
        private static void XMLSerializeGrid(GridData GridInfo)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ListWeatherCities.xml");
            List<GridData> gridDatalist = new List<GridData>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<GridData>));
            gridDatalist.Add(GridInfo);
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, gridDatalist);
            }
        }
        private static List<GridData> DeserializeXML()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ListWeatherCities.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(List<GridData>));
            using (StreamReader reader = new StreamReader(filePath))
            {
                List<GridData> gridDataList = (List<GridData>)serializer.Deserialize(reader);
                return gridDataList;
            }
        }
        private void ImageBtn_Clicked(object sender, EventArgs e)
        {
            ReturnMain.IsEnabled = false;
            var MainPage = new CurrentWeaherPage();
            Navigation.PushModalAsync(MainPage);
        }

        private async void SelectCity_BtnClicked(object sender, EventArgs e)
        {
            BtnChangeCity.IsEnabled = false;
            var SearchPageCity = new SearchCity();
            await Navigation.PushModalAsync(SearchPageCity);
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                BtnChangeCity.IsEnabled = true;
                return false;
            });
        }
        private static Label ChangeDate(Label CurrentLableDate)
        {
            Label NewLabel = new Label();
            DayOfWeek dayOfWeek = DateTime.Today.DayOfWeek;
            string DayOfWeekStr = dayOfWeek.ToString();
            DateTime currentDate = DateTime.Now;
            string CurrectMonth = currentDate.ToString($"MMMM");
            string CurrentDayNumber = currentDate.ToString($"dd");
            NewLabel.Text = ($"{DayOfWeekStr}  {CurrectMonth}  {CurrentDayNumber}");
            CurrentLableDate.Text = NewLabel.Text;
            return CurrentLableDate;
        }
    }
}