import styles from './page.module.css'

export default function SnowMakingQuickInfo() {
    return (
        <>
            <div className={styles.grid}>
                <h3 className={styles.card}>Last Sensor</h3>
                <h3 className={styles.card}>Last Weather</h3>
                <h3 className={styles.card}>In The Zone</h3>
                <h3 className={styles.card}>Zone Forecast</h3>
            </div>
        </>
    );
}