export function CelciusToFahrenheit(celcius) {
    return RoundToOneDecimal(celcius * (9 / 5) + 32);
}

export function RoundToOneDecimal(number) {
    return Math.round(number * 10) / 10;
}

export function ConvertUtcToCentral(datetime) {
    return datetime;
    // var options = {
    //     timeZone: "America/Chicago",
    //     year: 'numeric', month: 'numeric', day: 'numeric',
    //     hour: 'numeric', minute: 'numeric', second: 'numeric'
    // };

    // var formatter = new Intl.DateTimeFormat([], options);
    // var localTime = formatter.format(new Date(datetime));
    // localTime = localTime.replace(",", "");
    // return localTime;
    // var currentTime = formatter.format(new Date()); 
}