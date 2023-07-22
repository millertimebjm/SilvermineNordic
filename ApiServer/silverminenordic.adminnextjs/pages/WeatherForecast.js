import styles from '../styles/SnowMaking.module.css';

function WeatherForecast({ weatherForecastJson }) {
    return (
        <>
            <h3 className={styles.description}>Weather Forecast</h3>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>DateTimeUtc</th>
                        <th>temperatureInCelcius</th>
                        <th>humidity</th>
                        <th>snowfallInCm</th>
                    </tr>
                </thead>
                <tbody>
                    {weatherForecastJson.map((wf, index) => (
                        <tr key={index}>
                            <td>{wf.dateTimeUtc}</td>
                            <td>{wf.temperatureInCelcius}</td>
                            <td>{wf.humidity}</td>
                            <td>{wf.snowfallInCm}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </>
    );
}

export default WeatherForecast;