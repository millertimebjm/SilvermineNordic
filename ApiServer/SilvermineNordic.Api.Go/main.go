package main

import (
	"encoding/json"
	"fmt"
	"net/http"
	"strings"

	"github.com/gin-gonic/gin"
)

type WeatherApiResponseModel struct {
	List []ListModel
}

type ListModel struct {
	DateTime uint64 `json:"dt"`
	Main     MainModel
	Snow     SnowModel
}

type MainModel struct {
	Temp     float32
	Humidity float32
}

type SnowModel struct {
	ThreeHours float32 `json:"3h"`
}

type WeatherForecastModel struct {
	DateTime     uint64
	Temp         float32
	Humidity     float32
	SnowfallInCm float32
}

// var weatherModelModels = []weatherForecastModel{
// 	{DateTime: time.Now().UTC(), Temp: 0.0, Humidity: 0.0, SnowfallInCm: 0.0},
// 	{DateTime: time.Now().UTC().Add(time.Hour * 3), Temp: 5.0, Humidity: 5.0, SnowfallInCm: 1.0},
// 	{DateTime: time.Now().UTC().Add(time.Hour * 6), Temp: 10.0, Humidity: 10.0, SnowfallInCm: 2.0},
// }

var openWeatherMapApi = "https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={API key}"

// getAlbums responds with the list of all albums as JSON.
func getWeatherForecast(c *gin.Context) {
	openWeatherMapApi = strings.Replace(openWeatherMapApi, "{API key}", "", -1)
	openWeatherMapApi = strings.Replace(openWeatherMapApi, "{lat}", "44.772712650825966", -1)
	openWeatherMapApi = strings.Replace(openWeatherMapApi, "{lon}", "-91.58243961934646", -1)
	fmt.Println(openWeatherMapApi)
	resp, err := http.Get(openWeatherMapApi) // Replace with the actual endpoint URL
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error making request: " + err.Error(),
		})
		return
	}
	defer resp.Body.Close() // Ensure the response body is closed

	var weatherApiResponseModel WeatherApiResponseModel // Assuming a JSON object
	err = json.NewDecoder(resp.Body).Decode(&weatherApiResponseModel)
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error decoding JSON: " + err.Error(),
		})
		return
	}

	var weatherForecastModelList []WeatherForecastModel = make([]WeatherForecastModel, 0)
	for _, item := range weatherApiResponseModel.List {
		newWeatherForecastModel := WeatherForecastModel{
			DateTime: item.DateTime,
			Temp:     item.Main.Temp,
			Humidity: item.Main.Humidity,
		}
		weatherForecastModelList = append(weatherForecastModelList, newWeatherForecastModel)
		fmt.Printf("%v,%v,%v,%v\n", item.DateTime, item.Main.Temp, item.Main.Humidity, item.Snow.ThreeHours)
	}

	c.IndentedJSON(http.StatusOK, weatherForecastModelList)
}

// postAlbums adds an album from JSON received in the request body.
// func postAlbums(c *gin.Context) {
// 	var newAlbum album

// 	// Call BindJSON to bind the received JSON to
// 	// newAlbum.
// 	if err := c.BindJSON(&newAlbum); err != nil {
// 		return
// 	}

// 	// Add the new album to the slice.
// 	albums = append(albums, newAlbum)
// 	c.IndentedJSON(http.StatusCreated, newAlbum)
// }

func main() {
	router := gin.Default()
	router.GET("/weatherforecast", getWeatherForecast)

	router.Run("localhost:8080")
}

// public async Task<IEnumerable<WeatherModel>> GetWeatherForecast()
//         {
//             //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}
//             var url = $"https://api.openweathermap.org/data/2.5/forecast?lat=44.772712650825966&lon=-91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
//             using var client = _httpClientFactory.CreateClient();
//             var openApiWeatherModel = await client.GetFromJsonAsync<OpenWeatherApiWeatherForecastListModel>(url);
//             var models = new List<WeatherModel>();
//             foreach (var forecast in openApiWeatherModel?.List ?? new List<OpenWeatherApiWeatherForecastModel>())
//             {
//                 models.Add(new WeatherModel()
//                 {
//                     DateTimeUtc = forecast.DateTimeUtc ?? DateTime.MinValue,
//                     TemperatureInCelcius = forecast.Main.Temp,
//                     Humidity = forecast.Main.Humidity,
//                     SnowfallInCm = forecast.Snow?.SnowfallAmountInCentimeters ?? 0,
//                 });
//             }
//             return models;
//         }
