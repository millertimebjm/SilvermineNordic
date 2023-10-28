import styles from './page.module.css'
import { CelciusToFahrenheit, RoundToOneDecimal } from '../lib/helperFunctions';

export default function Thresholds({ thresholdJson }) {
    return (
        <>
            <h3 className={styles.description}>Thresholds</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>Temperature Low</th>
                        <th>Temperature High</th>
                        <th>Humidity Low</th>
                        <th>Humidity High</th>
                    </tr>
                </thead>
                <tbody>
                    {thresholdJson && thresholdJson.length > 0 ? (
                        thresholdJson.map(t => (
                            <tr key={t.id}>
                                <td>{CelciusToFahrenheit(t.TemperatureInCelciusLowThreshold)}</td>
                                <td>{CelciusToFahrenheit(t.TemperatureInCelciusHighThreshold)}</td>
                                <td>{RoundToOneDecimal(t.HumidityLowThreshold)}</td>
                                <td>{RoundToOneDecimal(t.HumidityHighThreshold)}</td>
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