package main

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"net/http"
	"strings"
	"time"

	_ "github.com/denisenkom/go-mssqldb" // Import the driver
	"github.com/gin-gonic/gin"
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

func main() {

	router := gin.Default()
	router.GET("/weatherforecast", getWeatherForecast3HourApi)
	router.GET("/currentweather", getCurrentWeatherApi)
	router.GET("/reading", getReadings)
	router.POST("/reading", postReading)

	router.Run("localhost:8080")
}

type Reading struct {
	Id                   int
	Type                 string
	DateTimeUtc          time.Time
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
	db, err := getSqlServerConnection()
	if err != nil {
		fmt.Println("Error connecting to database:", err)
	}

	sql := `
	INSERT INTO Reading (Type, DateTimeUtc, TemperatureInCelcius, Humidity)
	OUTPUT INSERTED.ID	
	VALUES ('Weather', ?, ?, ?)
	`
	rows, err := db.Query(sql,
		weatherCurrentModel.DateTimeUtc,
		weatherCurrentModel.TemperatureInCelcius,
		weatherCurrentModel.Humidity)
	if err != nil {
		fmt.Println("Error inserting data:", err)
		return
	}

	var id int
	for rows.Next() {
		err = rows.Scan(&id)
		if err != nil {
			c.IndentedJSON(http.StatusBadRequest, gin.H{
				"message": "Error getting inserted data: " + err.Error(),
			})
		}
	}

	reading := Reading{
		Id:                   int(id),
		Type:                 "Weather",
		DateTimeUtc:          weatherCurrentModel.DateTimeUtc,
		TemperatureInCelcius: weatherCurrentModel.TemperatureInCelcius,
		Humidity:             weatherCurrentModel.Humidity,
	}
	c.IndentedJSON(http.StatusOK, reading)
}

func getReadings(c *gin.Context) {
	db, err := getSqlServerConnection()
	if err != nil {
		fmt.Println("Error connecting to database:", err)
	}

	sql := `
	SELECT Id, Type, DateTimeUtc, TemperatureInCelcius, Humidity 
	FROM Reading 
	WHERE Type = 'Weather' 
	ORDER BY DateTimeUtc DESC
	`
	rows, err := db.Query(sql)
	if err != nil {
		fmt.Println("Error executing query:", err)
		return
	}
	defer db.Close()
	defer rows.Close()

	readings := make([]Reading, 0)
	for rows.Next() {
		var reading Reading
		err := rows.Scan(&reading.Id, &reading.Type, &reading.DateTimeUtc, &reading.TemperatureInCelcius, &reading.Humidity)
		if err != nil {
			fmt.Println("Error scanning row:", err)
			return
		}
		fmt.Println(reading)
		readings = append(readings, reading)
	}
	c.IndentedJSON(http.StatusOK, readings)
}

func getSqlServerConnection() (*sql.DB, error) {
	connString := "sqlserver://sa:@jenkins.bltmiller.com:1433?database=SilvermineNordicSnowMaking"
	db, err := sql.Open("mssql", connString)
	if err != nil {
		return nil, err
	}
	return db, nil
}
