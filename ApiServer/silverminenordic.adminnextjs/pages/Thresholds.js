import styles from '../styles/SnowMaking.module.css';
import { CelciusToFahrenheit, RoundToOneDecimal } from './helperFunctions';

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
                    {thresholdJson.map(t => (
                        <tr key={t.id}>
                            <td>{CelciusToFahrenheit(t.temperatureInCelciusLowThreshold)}</td>
                            <td>{CelciusToFahrenheit(t.temperatureInCelciusHighThreshold)}</td>
                            <td>{RoundToOneDecimal(t.humidityLowThreshold)}</td>
                            <td>{RoundToOneDecimal(t.humidityHighThreshold)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}