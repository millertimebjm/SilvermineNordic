﻿
// using System;

// namespace SilvermineNordic.Models
// {
//     public class OpenWeatherApiWeatherForecastModel
//     {
//         public long dt { get; set; }
//         public DateTime? DateTimeUtc
//         {
//             get
//             {
//                 return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(dt);
//             }
//         }
//         public OpenWeatherApiWeatherForecaseMainModel Main { get; set; } = new();
//         public OpenWeatherApiWeatherForecastPrecipitationModel Snow { get; set; } = new();
//         public OpenWeatherApiWeatherForecastPrecipitationModel Rain { get; set; } = new();
//         public OpenWeatherApiWeatherForecastCloudsModel Clouds { get; set; } = new();
//     }
// }