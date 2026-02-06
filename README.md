# IndigoLabsAssignment

This is a .NET 10 (10.0.100) Web API that reads a temperature measurements file, aggregates statistics per city, and exposes them via HTTP endpoints. The API is served on `https://localhost:7160` and includes a Swagger UI for easy exploration.

## Prerequisites

- .NET SDK **10.0.100** or later installed
  - Check your version with:
    ```bash
    dotnet --version
    ```
- Git
- A text editor or IDE (Visual Studio / VS Code / Rider, etc.)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/merchantry/IndigoLabsAssignment.git
cd IndigoLabsAssignment
```

### 2. Configure environment variables

The project uses a `.env` file for local configuration (loaded via `dotenv.net`).

1. Copy/rename the example file:
   - Rename `.env.example` to `.env` in the repo root.

2. Open `.env` and update:

   - Point `FileSettings__Path` to your local `measurements.txt` file.
   - Set an API key of your choice.

Example `.env`:

```env
# File path for the measurements file
FileSettings__Path=C:\path\to\your\measurements.txt

# API key used by the application (also configured in appsettings.json: ApiKeyAuth:Key)
ApiKeyAuth__Key=your-secret-api-key-here
```

> Note: The API key **must not** be left as the default `super-secret-key-change-me`.
> If that default value is detected at runtime, the API will respond with an error
> instructing you to set a valid key.

### 3. Prepare the measurements file

Create the file that `FileSettings__Path` points to (for example `C:\path\to\your\measurements.txt`)
and paste content like this:

```text
Village Green-Green Ridge;67.2
Lancing;-40.9
Bassian;68.9
Soanierana;34.0
Gelemso;73.8
Çatalca;22.0
Winslow;-56.0
New Glasgow;61.9
Cajabamba;51.4
Kall;49.9
Malar;-45.1
Rostraver;-54.3
Cajati;1.4
Lubbock;55.2
Sindhn?r;-9.8
Vólos;92.5
Újfehértó;-95.1
Al Jab?yish;30.2
Petawawa;8.1
Ampasimpotsy-Gara;-43.9
```

The app expects one measurement per line, using `;` as the separator (`CityName;Temperature`).

### 4. Configuration notes

`appsettings.json` contains default values:

```json
"FileSettings": {
  "Path": "measurements.txt"
},
"ApiKeyAuth": {
  "Key": "super-secret-key-change-me"
}
```

Values from `.env` (for example `FileSettings__Path`, `ApiKeyAuth__Key`) override these.
As long as your `.env` is set up correctly, you do not have to change `appsettings.json`.

### 5. Run the application

From the repo root (where the `.csproj` lives):

```bash
dotnet run
```

By default the API will start on:

- `https://localhost:7160`

You should see console output indicating that the server is listening on that URL.

## Using Swagger UI

The project exposes Swagger/OpenAPI when running in Development.

- Open a browser and go to:
  - `https://localhost:7160/swagger`

You will see the API documentation and available routes.

### Authorizing with the API key

Before trying any endpoints from Swagger UI:

1. Click the **Authorize** button (lock icon) near the top.
2. For the `ApiKey` security scheme, paste the value you set in `.env` for `ApiKeyAuth__Key`.
3. Click **Authorize**, then **Close**.

Now all requests sent from Swagger UI will include the `X-Api-Key` header and will be accepted
by the API.

If you do not set the header (or use a wrong key), you will receive `401 Unauthorized`.
If you leave the default key (`super-secret-key-change-me`), the middleware will return an error
indicating that you must configure a real key.

## Using Postman (optional)

You can also call the API directly from Postman or any HTTP client.

- Base URL:
  - `https://localhost:7160`
- Add a header to every request:
  - `X-Api-Key: <your key from .env>`

Then call the same endpoints you see documented in Swagger (for example the routes that
return city temperature statistics).

## Quick Summary

1. Clone the repo.
2. Rename `.env.example` ? `.env`.
3. Set `FileSettings__Path` to your `measurements.txt` file (with the sample content above or your own).
4. Set `ApiKeyAuth__Key` to a non-default secret.
5. Run with `dotnet run`.
6. Visit `https://localhost:7160/swagger`, click **Authorize**, paste your API key, and call the routes.
