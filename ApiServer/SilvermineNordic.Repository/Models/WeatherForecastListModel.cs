
namespace SilvermineNordic.Repository.Models
{
    public class WeatherForecastListModel
    {
        public string Cod { get; set; }
        public int Message { get; set; }
        public int Count { get; set; }
        public List<WeatherForecastModel> List { get; set; }
    }
}
