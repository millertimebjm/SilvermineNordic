import styles from '../../styles/SnowMaking.module.css';

export default function WeatherReadings({ weatherReadingJson }) {
    return (
        <>
            <h3 className={styles.description}>Weather Readings</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTimeUtc</th>
                        <th>temperatureInCelcius</th>
                        <th>humidity</th>
                    </tr>
                </thead>
                <tbody>
                    {weatherReadingJson.map(wf => (
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