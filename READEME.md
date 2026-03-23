# Quickstart Guide

## Requirements

- .NET 9.0
- Docker 

## Running the Application

   ```bash
    git clone https://github.com/Lucam31/advanced-software-engineering.git

    cd advanced-software-engineering

    # Start the server and the database
    docker compose up --build -d

    cd chess-client
    dotnet run
   ```

## Running Tests

   ```bash
    dotnet test
   ```