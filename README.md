# Aggregated API - Travel Companion

## Installation and Setup

### Clone the Repository

**1. Clone the repository from GitHub:**

   ```bash
   git clone https://github.com/EvgeniaAndrikopoulou/TravelCompanyAPI.git
```
### Build and Run the Service

**2. Navigate to the project directory: Open a Git Bash terminal and navigate to the directory where you cloned the project, then:**

```bash
cd TravelCompanyAPI/AggregatedAPI_TravelCompanion
```

**3. Build and run the API: Execute the following commands to build and start the API service:**

```bash
dotnet build
dotnet run
```
The API will now be running locally on http://localhost:5133/swagger.

### Running Unit Tests

**4. To run the unit tests for the project, use the following command:**

```bash
dotnet test
```
This will execute the unit tests to validate the functionality of the aggregation service and the external API calls.

### Accessing the Swagger UI

**5. After running the API, open your browser and navigate to the Swagger UI to test the endpoints:**

```bash
http://localhost:5133/swagger
```
Swagger provides an interactive interface to test the aggregated data API and see the responses. You can use this UI to test various query parameters and view the structure of the API responses.




