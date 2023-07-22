import styles from '../../styles/SnowMaking.module.css';

export default function Thresholds({ thresholdJson }) {
    return (
        <>
            <h3 className={styles.description}>Thresholds</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>temperatureInCelciusLowThreshold</th>
                        <th>temperatureInCelciusHighThreshold</th>
                        <th>humidityLowThreshold</th>
                        <th>humidityHighThreshold</th>
                    </tr>
                </thead>
                <tbody>
                    {thresholdJson.map(t => (
                        <tr key={t.id}>
                            <td>{t.temperatureInCelciusLowThreshold}</td>
                            <td>{t.temperatureInCelciusHighThreshold}</td>
                            <td>{t.humidityLowThreshold}</td>
                            <td>{t.humidityHighThreshold}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}