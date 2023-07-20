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
                    {thresholdJson.map(wf => (
                        <tr>
                            <td>{wf.temperatureInCelciusLowThreshold}</td>
                            <td>{wf.temperatureInCelciusHighThreshold}</td>
                            <td>{wf.humidityLowThreshold}</td>
                            <td>{wf.humidityHighThreshold}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}