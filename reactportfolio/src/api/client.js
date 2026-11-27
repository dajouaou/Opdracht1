// client.js
export async function request(url, options = {}) {
  const response = await fetch(url, {
    credentials: "same-origin",
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    const fout = new Error("Aanvraag mislukt");
    fout.status = response.status;
    throw fout;
  }

  // sommige antwoorden (DELETE, login) hebben geen inhoud
  const tekst = await response.text();
  return tekst ? JSON.parse(tekst) : null;
}

// maakt van een statuscode een begrijpelijke melding
export function foutTekst(status) {
  if (status === 401) return "401: je bent niet ingelogd. Log eerst in.";
  if (status === 403) return "403: je bent ingelogd, maar je rol mag dit niet. Alleen Editor en Admin mogen wijzigen.";
  if (status === 404) return "404: dit item bestaat niet (meer).";
  return "Er ging iets mis. Draait de API?";
}