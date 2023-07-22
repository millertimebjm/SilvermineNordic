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
                    {sensorReadingJson.map(sr => (
                        <tr key={sr.id}>
                            <td>{sr.readingDateTimestampUtc}</td>
                            <td>{sr.temperatureInCelcius}</td>
                            <td>{sr.humidity}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}