'use client';
// import Image from 'next/image'
import styles from './page.module.css'
import SnowMakingQuickInfo from './SnowMakingQuickInfo';
import SensorReadings from './SensorReadings';
import Thresholds from './Thresholds';
import WeatherForecast from './WeatherForecast';
import WeatherReadings from './WeatherReadings';
import RefreshButton from './RefreshButton';

async function getWeatherForecastJson(weatherForecastApiUrl) {
  try {
    const res = await fetch(`${weatherForecastApiUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

async function getSensorReadingJson(sensorReadingApiUrl) {
  try {
    const res = await fetch(`${sensorReadingApiUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

async function getWeatherReadingJson(sensorReadingApiUrl) {
  try {
    const res = await fetch(`${sensorReadingApiUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

async function getThresholdJson(thresholdApiUrl) {
  try {
    const res = await fetch(`${thresholdApiUrl}`);
    return res.json();
  } catch (e) {
    console.log(e);
    return null;
  }
}

export default async function Home() {
  const weatherForecastApiUrl = process.env.weatherForecastApiUrl || "http://localhost:7071/api/ReadWeatherForecast";
  const sensorReadingApiUrl = process.env.sensorReadingApiUrl || "http://localhost:7071/api/ReadReading/sensor";
  const weatherReadingApiUrl = process.env.weatherReadingApiUrl || "http://localhost:7071/api/ReadReading/weather";
  const thresholdApiUrl = process.env.thresholdApiUrl || "http://localhost:7071/api/ReadThreshold";
  const [weatherForecastJson, sensorReadingJson, weatherReadingJson, thresholdJson] = await Promise.all([
    getWeatherForecastJson(weatherForecastApiUrl),
    getSensorReadingJson(sensorReadingApiUrl),
    getWeatherReadingJson(weatherReadingApiUrl),
    getThresholdJson(thresholdApiUrl)
  ]);

  async function handleClick() {
    console.log('Button clicked');
  }

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Silvermine Nordic<br />Snow Making</h1>
      <div className={styles.grid}>
        <h3 className={styles.cardwithoutborder}>
          <button className={styles.button} onClick={handleClick}>Refresh</button>
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