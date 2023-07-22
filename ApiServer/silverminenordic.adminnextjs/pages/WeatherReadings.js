import styles from '../styles/SnowMaking.module.css';
import { CelciusToFahrenheit, RoundToOneDecimal, ConvertUtcToCentral } from './helperFunctions';

export default function WeatherReadings({ weatherReadingJson }) {
    return (
        <>
            <h3 className={styles.description}>Weather Readings</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTimeUtc</th>
                        <th>Temperature</th>
                        <th>Humidity</th>
                    </tr>
                </thead>
                <tbody>
                    {weatherReadingJson.map((wr) => (
                        <tr key={wr.id}>
                            <td>{ConvertUtcToCentral(wr.readingDateTimestampUtc)}</td>
                            <td>{CelciusToFahrenheit(wr.temperatureInCelcius)}</td>
                            <td>{RoundToOneDecimal(wr.humidity)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}