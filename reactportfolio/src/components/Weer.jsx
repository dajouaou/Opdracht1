// components/Weer.jsx  (je weer-API van api.js)
import { useEffect, useState } from "react";

function Weer() {
  const [weer, setWeer] = useState(null);
  const [status, setStatus] = useState("laden");

  useEffect(() => {
    fetch("https://api.open-meteo.com/v1/forecast?latitude=52.0705&longitude=4.3007&current=temperature_2m,wind_speed_10m")
      .then((response) => {
        if (!response.ok) throw new Error("mislukt");
        return response.json();
      })
      .then((data) => {
        setWeer(data.current);
        setStatus("klaar");
      })
      .catch(() => setStatus("fout"));
  }, []);

  return (
    <section className="api-section">
      <div className="container">
        <h2>Het weer in Den Haag</h2>
        <p>Hier wordt actuele weerinformatie opgehaald via een publieke API.</p>
        <div id="weather-result" aria-live="polite">
          {status === "laden" && <p>De weerinformatie wordt geladen...</p>}
          {status === "fout" && <p>De weerinformatie is op dit moment niet beschikbaar.</p>}
          {status === "klaar" && (
            <p>De temperatuur is {weer.temperature_2m} °C en de windsnelheid is {weer.wind_speed_10m} km/u.</p>
          )}
        </div>
      </div>
    </section>
  );
}

export default Weer;