import styles from './page.module.css'
import { CelciusToFahrenheit, RoundToOneDecimal, ConvertUtcToCentral } from '../lib/helperFunctions';

function WeatherForecast({ weatherForecastJson }) {
    return (
        <>
            <h3 className={styles.description}>Weather Forecast</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTime</th>
                        <th>Temperature</th>
                        <th>Humidity</th>
                        <th>SnowfallInCm</th>
                    </tr>
                </thead>
                <tbody>
                    {weatherForecastJson && weatherForecastJson.length > 0 ? (
                        weatherForecastJson.map((wf, index) => (
                            <tr key={index}>
                                <td>{ConvertUtcToCentral(wf.DateTimeUtc)}</td>
                                <td>{CelciusToFahrenheit(wf.TemperatureInCelcius)}</td>
                                <td>{RoundToOneDecimal(wf.Humidity)}</td>
                                <td>{wf.SnowfallInCm}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="4">No data available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </>
    );
}

export default WeatherForecast;