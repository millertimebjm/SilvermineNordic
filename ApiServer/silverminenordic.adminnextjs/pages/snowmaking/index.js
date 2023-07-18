import Head from 'next/head';
import styles from '../../styles/SnowMaking.module.css';
import SnowMakingQuickInfo from './SnowMakingQuickInfo';

export default function snowmaking() {
    return (
        <div className={styles.container}>
            <Head>
                <title>Silvermine Nordic Snow Making</title>
                <link rel="icon" href="/favicon.ico" />
            </Head>
            {/* <main> */}
            <h1 className={styles.title}>Silvermine Nordic<br />Snow Making</h1>

            <div className={styles.grid}>
                <h3 className={styles.cardwithoutborder}>
                    <button className={styles.button}>Refresh</button>
                </h3>
            </div>
            <SnowMakingQuickInfo />
            <div className={styles.grid}>
                <div className={styles.tablecard}>
                    <table className={styles.table}>
                        <thead>
                            <tr>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div styles="border: 1px solid #ccc;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 20px;
  margin-bottom: 20px;">
                    <table className={styles.table}>
                        <thead>
                            <tr>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                                <th>header</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                            <tr>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                                <td>first</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            {/* </main > */}
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