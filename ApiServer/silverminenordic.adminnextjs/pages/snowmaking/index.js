import Head from 'next/head';
import styles from '../../styles/SnowMaking.module.css';
import SnowMakingQuickInfo from './SnowMakingQuickInfo';
import Thresholds from './Thresholds';
import WeatherForecast from './WeatherForecast';
import WeatherReadings from './WeatherReadings';

export default function snowmaking() {
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
                    <Thresholds />
                </div>
                <div styles="border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 20px;
  margin-bottom: 20px;">
                    <WeatherForecast />
                </div>
                <div styles="border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 20px;
  margin-bottom: 20px;">
                    <WeatherReadings />
                </div>
                <div styles="border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 20px;
  margin-bottom: 20px;">
                    <WeatherForecast />
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