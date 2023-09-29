import styles from '../styles/SnowMaking.module.css';
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
                                <td>{ConvertUtcToCentral(wf.dateTimeUtc)}</td>
                                <td>{CelciusToFahrenheit(wf.temperatureInCelcius)}</td>
                                <td>{RoundToOneDecimal(wf.humidity)}</td>
                                <td>{wf.snowfallInCm}</td>
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