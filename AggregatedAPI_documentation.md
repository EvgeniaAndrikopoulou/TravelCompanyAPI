
# Aggregated API - Travel Companion

## Overview

The **Aggregated API - Travel Companion** service is a .NET-based API aggregation service built with ASP.NET Core. It consolidates data from three external APIs—covering location, weather, and landmark information—and provides a single endpoint for accessing aggregated data. This service includes support for filtering, sorting and error handling.

## Table of Contents

1. [Installation and Setup](#installation-and-setup)
2. [Architecture](#architecture)
3. [Endpoints](#endpoints)
    - [POST /api/aggregateddata](#get-apiaggregateddata)
4. [External APIs Integrated](#external-apis-integrated)
5. [Features](#features)
6. [Error Handling](#error-handling)
7. [Unit Testing](#unit-testing)
8. [Future Improvements](#future-improvements)

## 1. [Installation and Setup](#installation-and-setup)

### Clone the Repository

1. Clone the repository from GitHub:

   ```bash
   git clone https://github.com/your-username/aggregated-api-travel-companion.git
   ```

### API Keys

The API keys for external services are **hardcoded** into the project because unit tests could not retrieve them from the `appsettings.json` file. For security reasons, it is recommended to use environment variables or configuration files for API keys in production environments.

### Build and Run the Service

2. **Navigate to the project directory**:
   Open a Git Bash terminal and navigate to the directory where you cloned the project:

   ```bash
   cd AggregatedAPI_TravelCompanion
   ```

3. **Build and run the API**:
   Execute the following commands to build and start the API service:

   ```bash
   dotnet build
   dotnet run
   ```

   The API will now be running locally on `http://localhost:5133`.

### Running Unit Tests

4. To run the unit tests for the project, use the following command:

   ```bash
   dotnet test
   ```

   This will execute the unit tests to validate the functionality of the aggregation service and the external API calls.

### Accessing the Swagger UI

5. After running the API, open your browser and navigate to the Swagger UI to test the endpoints:

   ```
   http://localhost:5133/swagger
   ```

   Swagger provides an interactive interface to test the aggregated data API and see the responses. You can use this UI to test various query parameters and view the structure of the API responses.

---

With these steps, you should be able to set up the API service, run it, and use the Swagger UI to interact with the service.


## 2. [Architecture](#architecture)

The architecture of the **Aggregated API Travel Companion** system consists of the following components:

## 1. Services
Each service is responsible for interacting with a specific external API. These services are separated to follow the **Single Responsibility Principle (SRP)**, ensuring that each service handles one responsibility.

- **Landmark Service** (`LandmarkService`)
    - Responsible for fetching landmark data from the Foursquare API.
    - Filters and sorts landmark data based on Landmark's name.
  
- **Location Service** (`LocationService`)
    - Interacts with the **Geoapify** API to fetch location information based on the user's address (city, state, etc.).
  
- **Weather Service** (`WeatherService`)
    - Connects with the **OpenWeatherMap** API to retrieve weather data for a given location (latitude, longitude) at a specific time.

## 2. Aggregation Service (`AggregationService`)
- This is the **core** of your system, where data from the external services is aggregated.
- It fetches location data, weather data, and landmark data, combining them into a unified response for the client.
- It uses the services (`LocationService`, `WeatherService`, `LandmarkService`) and combines their outputs into an `AggregatedData` object.
- If any of the individual service requests fail, it logs the error messages and still returns the aggregated data with available results.

## 3. Controller (`AggregatedDataController`)
- The **API Controller** serves as the entry point for the client application to interact with the backend.
- It exposes an HTTP POST endpoint (`/api/aggregateddata`) where the client sends an `AggregatedDataRequest` with parameters such as location, weather, and landmark criteria.
- It communicates with the `AggregationService` to retrieve the aggregated data.
- If the request is successful, the controller returns the aggregated data as an HTTP 200 response; if not, it returns an HTTP 400 error with the service response.

## 4. DTOs (Data Transfer Objects)
- **External API DTOs**: These are used to map the response from external APIs (Foursquare, Geoapify, OpenWeatherMap) into a structure that the service layer can easily use.
- **Aggregated DTO**: These represent the final request structure that the `AggregationService` gets, including appropriate data for all services (location, weather, landmarks).
- **Request DTOs**: These represent the input parameters from the client, specifying the criteria for the aggregation.

## 5. Data Flow
- The **client** sends an HTTP POST request to the `/api/aggregateddata` endpoint, including the `AggregatedDataRequest` with parameters like country, city, latitude, longitude, and any optional filters for sorting or name filtering.
- The controller calls the `AggregationService`, which fetches data from the **LandmarkService**, **LocationService**, and **WeatherService**.
- The aggregation service then combines the data into an `AggregatedData` object, which includes location, weather, and landmarks.
- The controller returns the aggregated data back to the client or error messages if any service failed.

## 6. Error Handling and Success Responses
- Each service checks if the response from the external API is successful. If it fails, an appropriate error message is set in the `ServiceResponse` object.
- The `AggregationService` combines results from each service and provides a consolidated success or failure response.
- The controller ensures that clients receive clear success or failure messages.

## 7. Example of the Flow
1. **Client Request**:
   - The client sends a POST request with the location (e.g., "New York"), time and optional filter like landmark name filtering.

2. **Controller**:
   - Receives the request and invokes `GetAggregatedDataAsync()` from `AggregationService`.

3. **AggregationService**:
   - Calls `LocationService.GetLocationInfo()` to get latitude and longitude.
   - With these coordinates, it calls `WeatherService.GetWeatherInfo()` to get weather data.
   - It then calls `LandmarkService.GetLandmarkInfo()` to get nearby landmarks.
   - Combines all data into an `AggregatedData` object.

4. **Controller Response**:
   - Returns the aggregated data (location, weather, landmarks) to the client or error messages if any service failed.

## Summary
- The **LandmarkService**, **LocationService**, and **WeatherService** handle specific API calls and data fetching.
- The **AggregationService** combines these individual results and handles the business logic for merging data.
- The **AggregatedDataController** acts as the interface for the client, providing a consolidated response from the aggregated data.
- This architecture supports scalability, maintainability, and separation of concerns, ensuring that each component handles its specific responsibility efficiently.

## 3. [Endpoints](#endpoints)

### [POST /api/aggregateddata](#get-apiaggregateddata)
This endpoint aggregates data from multiple external APIs (Location, Weather, and Landmarks) based on the provided request.

- **Request Body**: 
  - `AggregatedDataRequest` (contains location details, landmark category, and optional filters)
  
- **Responses**:
  - **200 OK**: Successfully retrieved aggregated data, including location, weather, and landmarks.
  - **400 Bad Request**: Invalid input or data not found. 

**Request Example**:
```json
{
  "Country": "USA",
  "State": "California",
  "City": "Los Angeles",
  "LandmarkCategory": "museum",
  "SortBy": "name",
  "SortOrder": "asc"
}
```

**Response Example**:
```json
{
  "isSuccess": true,
  "data": {
    "Address": "123 Example St, Los Angeles, CA",
    "Temperature": 22.5,
    "WeatherDescription": "Clear sky",
    "Landmarks": [
      {
        "Name": "The Getty Center",
        "ShortName": "museum",
        "FormattedAddress": "1200 Getty Center Dr, Los Angeles, CA"
      }
    ]
  },
  "message": "Aggregated data retrieved with available results."
}
```

## 4. [External APIs Integrated](#external-apis-integrated)

The following external APIs are integrated into the system:

1. **Foursquare API** (for Landmark Data)
   - Provides information about nearby landmarks based on the location.
   - **Endpoint**: `https://api.foursquare.com/v3/places/nearby`
   - **Data**: Returns landmark name, category, address, and more.

2. **Geoapify API** (for Location Data)
   - Provides geolocation details based on user input like country, city, state, etc.
   - **Endpoint**: `https://api.geoapify.com/v1/geocode/search`
   - **Data**: Returns latitude, longitude, and address.

3. **OpenWeatherMap API** (for Weather Data)
   - Provides weather information for a given location and time.
   - **Endpoint**: `https://api.openweathermap.org/data/3.0/onecall/timemachine`
   - **Data**: Returns temperature, weather description, and other weather metrics.

## 5. [Features](#features)

The **Aggregated API Travel Companion** provides the following features:

1. **Location Search**: 
   - Allows the user to provide a city, state, country, and other location details to fetch the geographic location (latitude, longitude) from the Geoapify API.

2. **Weather Information**: 
   - Fetches historical weather data for a location using the OpenWeatherMap API.
   - Provides temperature, weather description, and additional metrics like humidity and wind speed.

3. **Nearby Landmarks**: 
   - Retrieves information about nearby landmarks based on location coordinates.
   - Supports filtering landmarks by category (e.g., museum, park) and sorting them by name.

4. **Data Aggregation**: 
   - Combines location, weather, and landmark information into a single response, making it easy for users to get all necessary information in one place.

## 6. [Error Handling](#error-handling)

The system implements robust error handling:

1. **Service-Level Errors**: 
   - If any external API call fails, an appropriate error message is returned to the user.
   - Each service checks for successful responses from external APIs and returns error messages in the case of failure.

2. **Input Validation**:
   - If the user sends invalid data (e.g., missing required fields in the request), the API will return a 400 Bad Request with the corresponding error message.

**Error Response Example**:
```json
{
  "isSuccess": false,
  "message": "Location info unavailable: API error."
}
```

## 7. [Unit Testing](#unit-testing)

Unit testing is an essential part of ensuring the robustness of the system. The following tests have been implemented:

1. **Service Unit Tests**: 
   - Tests for LocationService and WeatherService to ensure they handle responses from external APIs correctly.
   - Mocking external API calls to verify the service's response when data is returned and when errors occur.

2. **Aggregation Service Tests**: 
   - Tests the logic for combining data from multiple services (location, weather, landmarks) into a single response.
   - Verifies that the system correctly handles missing or incomplete data.

3. **AggregatedDataController Tests**:
   - Tests the logic of succes service response and failed service responce


## 8. [Future Improvements](#future-improvements)

The following improvements are planned for future versions of the API:

1. **More Unit Tests**
   - Create funtional unit tests for LandmarkService

2. **Caching**:
   - Implement caching mechanisms to reduce the number of calls to external APIs, improving response times and reducing costs.

2. **Additional External APIs**:
   - Integrate more external APIs (e.g., for transportation or restaurants) to enhance the data aggregation.

3. **User Authentication**:
   - Implement user authentication and authorization to allow users to save their preferences and history.

4. **Error Reporting and Logging Enhancements**:
   - Improve error reporting mechanisms with more detailed logs for better debugging and tracing.

