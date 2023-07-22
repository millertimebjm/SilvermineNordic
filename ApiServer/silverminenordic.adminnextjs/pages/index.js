import Head from 'next/head';
import styles from '../styles/SnowMaking.module.css';
import SnowMakingQuickInfo from './SnowMakingQuickInfo';
import SensorReadings from './SensorReadings';
import Thresholds from './Thresholds';
import WeatherForecast from './WeatherForecast';
import WeatherReadings from './WeatherReadings';

export async function getServerSideProps(context) {
  const [weatherForecastData, sensorReadingData, weatherReadingData, thresholdData] = await Promise.all([
    fetch("http://0.0.0.0:9080/weatherforecast"),
    fetch("http://0.0.0.0:9080/sensorreading?count=5"),
    fetch("http://0.0.0.0:9080/weatherreading?count=5"),
    fetch("http://0.0.0.0:9080/thresholds")
  ]);

  const [weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson] = await Promise.all([
    weatherForecastData.json(),
    sensorReadingData.json(),
    weatherReadingData.json(),
    thresholdData.json()
  ]);

  return {
    props: { weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson }
  }
}

export default function snowmaking({ weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson }) {
  return (
    <div className={styles.container}>
      <Head>
        <title>Silvermine Nordic Snow Making</title>
        <link rel="icon" href="/favicon.ico" />
      </Head>
      <h1 className={styles.title}>Silvermine Nordic<br />Snow Making</h1>

      <div className={styles.grid}>
        <h3 className={styles.cardwithoutborder}>
          <button className={styles.button}>Refresh</button>
        </h3>
      </div>
      <SnowMakingQuickInfo />
      <div className={styles.grid}>
        <div className={styles.tablecard}>
          <Thresholds thresholdJson={thresholdJson} />
        </div>
        <div className={styles.tablecard}>
          <SensorReadings sensorReadingJson={sensorReadingJson} />
        </div>
        <div className={styles.tablecard}>
          <WeatherReadings weatherReadingJson={weatherReadingJson} />
        </div>
        <div className={styles.tablecard}>
          <WeatherForecast weatherForecastJson={weatherForecastJson} />
        </div>
      </div>
      <style jsx global>{`
        html,
        body {
          padding: 0;
          margin: 0;
          font-family: -apple-system, BlinkMacSystemFont, Segoe UI, Roboto,
            Oxygen, Ubuntu, Cantarell, Fira Sans, Droid Sans, Helvetica Neue,
            sans-serif;
        }
        * {
          box-sizing: border-box;
        }
      `}</style>
    </div >
  )
}
