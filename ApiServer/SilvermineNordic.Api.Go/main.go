package main

import (
	"context"
	"encoding/json"
	"fmt"
	"net/http"
	"strings"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/redis/go-redis/v9"
)

type WeatherForecastApiResponseModel struct {
	List []ListModel
}

type ListModel struct {
	DateTime int64 `json:"dt"`
	Main     MainModel
	Snow     SnowModel
}

type WeatherCurrentApiModel struct {
	Main MainModel
}

type MainModel struct {
	Temp     float32
	Humidity float32
}

type SnowModel struct {
	ThreeHours float32 `json:"3h"`
}

type WeatherForecastModel struct {
	DateTimeUtc  time.Time
	Temp         float32
	Humidity     float32
	SnowfallInCm float32
}

type WeatherCurrentModel struct {
	DateTimeUtc          time.Time
	TemperatureInCelcius float32
	Humidity             float32
}

func openWeatherReplaceRequiredFields(url string) string {
	url = strings.Replace(url, "{API key}", "", -1)
	url = strings.Replace(url, "{lat}", "44.772712650825966", -1)
	url = strings.Replace(url, "{lon}", "-91.58243961934646", -1)
	fmt.Println(url)
	return url
}

func getCurrentWeatherOpenWeatherApi() (WeatherCurrentModel, error) {
	var url = "https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}&mode=json&units=metric"
	url = openWeatherReplaceRequiredFields(url)
	var weatherCurrentModel WeatherCurrentModel
	resp, err := http.Get(url)
	if err != nil {
		return weatherCurrentModel, err
	}
	var weatherCurrentApiModel WeatherCurrentApiModel
	err = json.NewDecoder(resp.Body).Decode(&weatherCurrentApiModel)
	if err != nil {
		return weatherCurrentModel, err
	}
	weatherCurrentModel = WeatherCurrentModel{
		DateTimeUtc:          time.Now().UTC(),
		TemperatureInCelcius: weatherCurrentApiModel.Main.Temp,
		Humidity:             weatherCurrentApiModel.Main.Humidity,
	}
	fmt.Println(resp.Body)
	defer resp.Body.Close()
	return weatherCurrentModel, nil
}
func getCurrentWeatherApi(c *gin.Context) {
	weatherCurrentApiModel, err := getCurrentWeatherOpenWeatherApi()
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error making request: " + err.Error(),
		})
	}
	c.IndentedJSON(http.StatusOK, weatherCurrentApiModel)
}
func getWeatherForecast3HourApi(c *gin.Context) {
	var url = "https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={API key}&mode=json&units=metric"
	url = openWeatherReplaceRequiredFields(url)

	resp, err := http.Get(url)
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error making request: " + err.Error(),
		})
		return
	}
	defer resp.Body.Close()

	var weatherForecastApiResponseModel WeatherForecastApiResponseModel
	err = json.NewDecoder(resp.Body).Decode(&weatherForecastApiResponseModel)
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error decoding JSON: " + err.Error(),
		})
		return
	}

	var weatherForecastModelList []WeatherForecastModel = make([]WeatherForecastModel, 0)
	for _, item := range weatherForecastApiResponseModel.List {
		newWeatherForecastModel := WeatherForecastModel{
			DateTimeUtc: time.Unix(item.DateTime, 0),
			Temp:        item.Main.Temp,
			Humidity:    item.Main.Humidity,
		}
		weatherForecastModelList = append(weatherForecastModelList, newWeatherForecastModel)
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
	router.GET("/weatherforecast", getWeatherForecast3HourApi)
	router.GET("/currentweather", getCurrentWeatherApi)
	router.GET("/reading", getReadings)
	router.POST("/reading", postReading)

	router.Run("localhost:8080")
}

var ctx = context.Background()

type Reading struct {
	DateTime             time.Time
	TemperatureInCelcius float32
	Humidity             float32
}

func postReading(c *gin.Context) {
	weatherCurrentModel, err := getCurrentWeatherOpenWeatherApi()
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error decoding JSON: " + err.Error(),
		})
	}
	rdb := RedisClient()
	err = rdb.HSet(ctx, "Reading:Weather:"+weatherCurrentModel.DateTimeUtc.Format("20060102:150405"), "DateTimeUtc", weatherCurrentModel.DateTimeUtc, "TemperatureInCelcius", weatherCurrentModel.TemperatureInCelcius, "Humidity", weatherCurrentModel.Humidity).Err()
	if err != nil {
		c.IndentedJSON(http.StatusBadRequest, gin.H{
			"message": "Error decoding JSON: " + err.Error(),
		})
		return
	}
	c.IndentedJSON(http.StatusOK, weatherCurrentModel)
}

func getReadings(c *gin.Context) {
	rdb := RedisClient()
	var cursor uint64
	var keys []string
	var err error

	keys, _, err = rdb.Scan(ctx, cursor, "Reading:Weather:*", 10).Result()
	if err != nil {
		panic(err)
	}

	fmt.Println(keys)

	c.IndentedJSON(http.StatusOK, keys)
}

func RedisClient() *redis.Client {
	return redis.NewClient(&redis.Options{
		Addr:     "jenkins.bltmiller.com:6379",
		Password: "", // no password set
		DB:       0,  // use default DB
		Protocol: 3,  // specify 2 for RESP 2 or 3 for RESP 3
	})
}
