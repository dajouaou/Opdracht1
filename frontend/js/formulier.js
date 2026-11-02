
const contactForm = document.querySelector("#contact-form");

// controleer of een e-mailadres een geldig formaat heeft
function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

// toon een foutmelding bij het juiste veld
function showError(field, message) {
    const errorElement = document.querySelector(`#${field.id}-error`);

    errorElement.textContent = message;
    field.setAttribute("aria-invalid", "true");
}

// verwijder de foutmelding als het veld opnieuw wordt gecontroleerd
function clearError(field) {
    const errorElement = document.querySelector(`#${field.id}-error`);

    errorElement.textContent = "";
    field.removeAttribute("aria-invalid");
}

// controleer alle velden voordat het formulier wordt verwerkt
function validateForm() {
    const name = document.querySelector("#name");
    const email = document.querySelector("#email");
    const message = document.querySelector("#message");

    let isValid = true;

    [name, email, message].forEach(clearError);

    if (name.value.trim() === "") {
        showError(name, "Vul je naam in.");
        isValid = false;
    }

    if (!isValidEmail(email.value.trim())) {
        showError(email, "Vul een geldig e-mailadres in.");
        isValid = false;
    }

    if (message.value.trim().length < 10) {
        showError(message, "Je bericht moet minimaal 10 tekens bevatten.");
        isValid = false;
    }

    return isValid;
}

// voorkom verzenden wanneer de invoer niet geldig is
contactForm.addEventListener("submit", function(event) {
    event.preventDefault();

    const formStatus = document.querySelector("#form-status");

    if (!validateForm()) {
        formStatus.textContent = "Controleer de foutmeldingen bij de velden.";
        return;
    }

    formStatus.textContent = "Bedankt! Je bericht is geldig ingevuld.";

    // maak het formulier leeg na een geldige invoer
    contactForm.reset();
});
