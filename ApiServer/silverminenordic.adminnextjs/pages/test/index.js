export async function getServerSideProps(context) {
    let result = {};
    const data = await fetch("http://0.0.0.0:9080/weatherforecast");
    result = await data.json();

  return {
    props: { result }
  }
}

function Page({ result }) {
  // Render the data here
  return <div>{result[0].dateTimeUtc}</div>;
}

export default Page;