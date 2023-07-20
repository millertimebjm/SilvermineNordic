import styles from '../../styles/SnowMaking.module.css';

export default function SensorReadings({ sensorReadingJson }) {
    return (
        <>
            <h3 className={styles.description}>Sensor Readings</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTimeUtc</th>
                        <th>temperatureInCelcius</th>
                        <th>humidity</th>
                    </tr>
                </thead>
                <tbody>
                    {sensorReadingJson.map(wf => (
                        <tr>
                            <td>{wf.readingDateTimestampUtc}</td>
                            <td>{wf.temperatureInCelcius}</td>
                            <td>{wf.humidity}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}