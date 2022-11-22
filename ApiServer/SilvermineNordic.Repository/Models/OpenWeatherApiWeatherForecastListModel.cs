
namespace SilvermineNordic.Repository.Models
{
    public class OpenWeatherApiWeatherForecastListModel
    {
        public string Cod { get; set; }
        public int Message { get; set; }
        public int Count { get; set; }
        public List<OpenWeatherApiWeatherForecastModel> List { get; set; }
    }
}
