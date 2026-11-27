# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and [`typescript-eslint`](https://typescript-eslint.io) in your project.

# Portfolio: React + ASP.NET Core

## Opbouw
- `PortfolioApi/` ASP.NET Core API met Identity (cookies), SQL Server
- `portfolio-react/` React-app (Vite) die de API gebruikt
- `PortfolioApi.Tests/` xUnit-tests

## Starten
1. SQL Server Express starten. Database `PortfolioOpdracht3` bestaat via `dotnet ef database update`.
2. API: `cd PortfolioApi` -> `dotnet run` (Swagger: https://localhost:7001/swagger)
3. React: `cd portfolio-react` -> `npm install` -> `npm run dev` (http://localhost:5173)
4. Tests: `dotnet test` en `npm run build`

## Testrollen
Een nieuw account krijgt automatisch de rol Gast. Editor en Admin geef je via SQL:
(zet hier de INSERT-query uit stap 10 en je testaccounts)

## Hoe de koppeling werkt
React draait op poort 5173, de API op 7001. In `vite.config.js` stuurt een proxy alle
`/api`-verzoeken naar de API. Voor de browser is dat dezelfde site, dus de Identity-cookie
(HttpOnly) wordt vanzelf meegestuurd. In de frontend staan geen wachtwoorden of sleutels
en niets in localStorage. Na verversen vraagt React `GET /api/auth/me` of je nog ingelogd bent.

## Aandachtspunten voor deployment
- Cookie: `HttpOnly` aan, `SecurePolicy = Always` (https), `SameSite=Lax`. Staan frontend en API op
  verschillende domeinen, dan is `SameSite=None; Secure` en CORS met `AllowCredentials` nodig.
- CSRF: met cookie-login kan een andere site verzoeken laten sturen. `SameSite` helpt, maar voor een
  echte deployment is een antiforgery-token nodig bij POST, PUT en DELETE.
- Proxy werkt alleen lokaal; bij deployment moet de frontend dezelfde host als de API gebruiken of een
  reverse proxy krijgen.

## Uitvoer
(plak hier de uitvoer van `npm run build` en `dotnet test`)