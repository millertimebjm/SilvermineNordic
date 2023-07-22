import styles from '../styles/SnowMaking.module.css';
import { CelciusToFahrenheit, RoundToOneDecimal, ConvertUtcToCentral } from './helperFunctions';

export default function SensorReadings({ sensorReadingJson }) {
    return (
        <>
            <h3 className={styles.description}>Sensor Readings</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTime</th>
                        <th>Temperature</th>
                        <th>Humidity</th>
                    </tr>
                </thead>
                <tbody>
                    {sensorReadingJson.map(sr => (
                        <tr key={sr.id}>
                            <td>{ConvertUtcToCentral(sr.readingDateTimestampUtc)}</td>
                            <td>{CelciusToFahrenheit(sr.temperatureInCelcius)}</td>
                            <td>{RoundToOneDecimal(sr.humidity)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}