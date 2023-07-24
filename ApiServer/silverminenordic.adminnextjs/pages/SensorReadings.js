import styles from '../styles/SnowMaking.module.css';
import { CelciusToFahrenheit, RoundToOneDecimal, ConvertUtcToCentral } from '../lib/helperFunctions';

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
                    {sensorReadingJson && sensorReadingJson.length > 0 ? (
                        sensorReadingJson.map(sr => (
                            <tr key={sr.id}>
                                <td>{ConvertUtcToCentral(sr.readingDateTimestampUtc)}</td>
                                <td>{CelciusToFahrenheit(sr.temperatureInCelcius)}</td>
                                <td>{RoundToOneDecimal(sr.humidity)}</td>
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