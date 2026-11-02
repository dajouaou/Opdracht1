
const weatherResult = document.querySelector("#weather-result");

// zet de weergegevens om naar een leesbare tekst
function showWeather(data) {
    const temperature = data.current.temperature_2m;
    const windSpeed = data.current.wind_speed_10m;

    const weatherText = document.createElement("p");
    weatherText.textContent =
        `De temperatuur is ${temperature} °C en de windsnelheid is ${windSpeed} km/u.`;

    weatherResult.replaceChildren(weatherText);
}

// haal actuele weergegevens op uit de publieke API
async function loadWeather() {
    weatherResult.textContent = "De weerinformatie wordt geladen...";

    try {
        const response = await fetch(
            "https://api.open-meteo.com/v1/forecast?latitude=52.0705&longitude=4.3007&current=temperature_2m,wind_speed_10m"
        );

        if (!response.ok) {
            throw new Error("De weergegevens konden niet worden opgehaald.");
        }

        const data = await response.json();
        showWeather(data);

    } catch (error) {
        weatherResult.textContent =
            "De weerinformatie is op dit moment niet beschikbaar.";
    }
}

// haal de weergegevens op wanneer de pagina wordt geopend
loadWeather();
