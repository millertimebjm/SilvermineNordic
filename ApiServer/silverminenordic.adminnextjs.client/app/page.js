// import Image from 'next/image'
import styles from './page.module.css'
import SnowMakingQuickInfo from './SnowMakingQuickInfo';
import SensorReadings from './SensorReadings';
import Thresholds from './Thresholds';
import WeatherForecast from './WeatherForecast';
import WeatherReadings from './WeatherReadings';

const weatherForecastUrl = "/weatherforecast";
async function getWeatherForecastJson(silvermineNordicApiHost) {
  try {
    const res = await fetch(`${silvermineNordicApiHost}${weatherForecastUrl}`);
    return res.json();
  } catch (e) {
    logger.error(e);
    return null;
  }
}

const sensorReadingUrl = "/sensorreading/5";
async function getSensorReadingJson(silvermineNordicApiHost) {
  try {
    const res = await fetch(`${silvermineNordicApiHost}${sensorReadingUrl}`);
    return res.json();
  } catch (e) {
    logger.error(e);
    return null;
  }
}

const weatherReadingUrl = "/weatherreading/5";
async function getWeatherReadingJson(silvermineNordicApiHost) {
  try {
    const res = await fetch(`${silvermineNordicApiHost}${sensorReadingUrl}`);
    return res.json();
  } catch (e) {
    logger.error(e);
    return null;
  }
}

const thresholdUrl = "/thresholds";
async function getThresholdJson(silvermineNordicApiHost) {
  try {
    const res = await fetch(`${silvermineNordicApiHost}${sensorReadingUrl}`);
    return res.json();
  } catch (e) {
    logger.error(e);
    return null;
  }
}

export default async function Home() {
  const silvermineNordicApiProtocol = "http";
  const silvermineNordicApiDomain = process.env.silvermineNordicApiDomain || "localhost";
  const silvermineNordicApiPort = process.env.silvermineNordicApiPort || "9080";
  const silvermineNordicApiHost = silvermineNordicApiProtocol + "://" + silvermineNordicApiDomain + ":" + silvermineNordicApiPort;

  const [weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson] = await Promise.all([
    getWeatherForecastJson(silvermineNordicApiHost),
    getSensorReadingJson(silvermineNordicApiHost),
    getSensorReadingJson(silvermineNordicApiHost),
    getThresholdJson(silvermineNordicApiHost)
  ]);

  return (
    <div className={styles.container}>
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
    </div>
  )
}