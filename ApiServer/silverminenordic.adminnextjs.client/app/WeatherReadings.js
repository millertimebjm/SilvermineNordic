import styles from './page.module.css'
import { CelciusToFahrenheit, RoundToOneDecimal, ConvertUtcToCentral } from '../lib/helperFunctions';

export default function WeatherReadings({ weatherReadingJson }) {
    return (
        <>
            <h3 className={styles.description}>Weather Readings</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTime</th>
                        <th>Temperature</th>
                        <th>Humidity</th>
                    </tr>
                </thead>
                <tbody>
                    {weatherReadingJson && weatherReadingJson.length > 0 ? (
                        weatherReadingJson.map((wr) => (
                            <tr key={wr.id}>
                                <td>{ConvertUtcToCentral(wr.readingDateTimestampUtc)}</td>
                                <td>{CelciusToFahrenheit(wr.temperatureInCelcius)}</td>
                                <td>{RoundToOneDecimal(wr.humidity)}</td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="3">No data available</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </>
    );
}