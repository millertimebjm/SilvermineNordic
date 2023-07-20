class WeatherForecast {
  constructor(dateTimeUtc, temperatureInCelcius, humidity, snowfallInCm) {
    this.dateTimeUtc = dateTimeUtc;
    this.temperatureInCelcius = temperatureInCelcius;
    this.humidity = humidity;
    this.snowfallInCm = snowfallInCm;
  }
}

export async function getServerSideProps(context) {
  const data = await fetch("http://0.0.0.0:9080/weatherforecast");
  var result = await data.json();

  return {
    props: { result }
  }
}

// function Page({ result }) {
//   // Render the data here
//   return <div>{result.map(wf => (
//     <>{wf.dateTimeUtc}</>
//   ))}</div>;
// }

function Page({ result }) {
  return <div>{result[0].dateTimeUtc}</div>;
}

export default Page;