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
                    {weatherReadingJson.map((wr) => (
                        <tr key={wr.id}>
                            <td>{wr.readingDateTimestampUtc}</td>
                            <td>{wr.temperatureInCelcius}</td>
                            <td>{wr.humidity}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}