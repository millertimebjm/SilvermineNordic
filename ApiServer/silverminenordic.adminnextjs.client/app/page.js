// import Image from 'next/image'
import styles from './page.module.css'
import SnowMakingQuickInfo from './SnowMakingQuickInfo';
import SensorReadings from './SensorReadings';
import Thresholds from './Thresholds';
import WeatherForecast from './WeatherForecast';
import WeatherReadings from './WeatherReadings';

const weatherForecastUrl = "/weatherforecast";
async function getWeatherForecastJson(silvermineNordicApiUrl) {
  try {
    const res = await fetch(`${silvermineNordicApiUrl}${weatherForecastUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

const sensorReadingUrl = "/readingget/sensor/5";
async function getSensorReadingJson(silvermineNordicApiUrl) {
  try {
    const res = await fetch(`${silvermineNordicApiUrl}${sensorReadingUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

const weatherReadingUrl = "/readingget/weather/5";
async function getWeatherReadingJson(silvermineNordicApiUrl) {
  try {
    const res = await fetch(`${silvermineNordicApiUrl}${sensorReadingUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

const thresholdUrl = "/thresholdget";
async function getThresholdJson(silvermineNordicApiUrl) {
  try {
    const res = await fetch(`${silvermineNordicApiUrl}${thresholdUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

export default async function Home() {
  const silvermineNordicApiUrl = process.env.silvermineNordicApiUrl || "http://localhost:7071/api";
  var asdf = await getSensorReadingJson(silvermineNordicApiUrl);
  const [weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson] = await Promise.all([
    getWeatherForecastJson(silvermineNordicApiUrl),
    getSensorReadingJson(silvermineNordicApiUrl),
    getWeatherReadingJson(silvermineNordicApiUrl),
    getThresholdJson(silvermineNordicApiUrl)
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