# MarvelouslyBlazor

A Blazor Server application built to explore the Marvel Comics API. Browse Marvel characters, comics, series, stories, creators, and events through a searchable interface. Includes a character guessing game component.

> **Note:** This project was built against the Marvel Comics API, which has since been discontinued. The application will not function without a working API endpoint. The code is preserved here as a demonstration of Blazor Server architecture and API integration patterns in C#/.NET.

## What it covered

- Blazor Server-side component architecture with dependency injection
- MD5 hash-based API authentication (timestamp + private key + public key pattern required by the Marvel API)
- Strongly typed response models and JSON deserialization across a complex nested API schema
- Search-driven character browser with keyboard support and navigation between related entities (characters, comics, series, stories, creators, events)
- IOptions pattern for configuration management

## Tech stack

- **ASP.NET Core / Blazor Server** (.NET 7)
- **Marvel Comics API** (discontinued)
- **Bootstrap** for layout

## Running locally

The application requires a Marvel Comics API key pair (public and private), which are no longer being issued. If you have existing credentials, configure them in `appsettings.json`:

```json
"MarvelApiSettings": {
  "PublicKey": "your_public_key",
  "PrivateKey": "your_private_key"
}
```
