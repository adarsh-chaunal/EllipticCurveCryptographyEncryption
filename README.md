# Elliptic Curve Cryptography Encryption

This repository contains a .NET API that demonstrates the use of ECC (Elliptic Curve Cryptography) encryption and decryption with Entity Framework Core (EF Core). The project includes an implementation for encrypting specific database columns using EF Core value converters.

## Table of Contents

- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Encryption Mechanism](#encryption-mechanism)
- [Database Configuration](#database-configuration)
- [How to Use](#how-to-use)
- [Additional ECC Encryption Details](#additional-ecc-encryption-details)
- [License](#license)

## Project Structure

- **Domain**: Contains the core domain logic including models, encryption helpers, and EF Core extensions.
- **WebAPI**: The entry point of the application, which sets up the API endpoints.
- **appsettings.Development.json**: Configuration file for development settings including logging and connection strings.
- **global.json**: Specifies the .NET SDK version to be used.
- **LICENSE.txt**: MIT License for the project.

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local instance or hosted)

### Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/your-repo/ecc-encryption-api.git
    cd ecc-encryption-api
    ```

2. Restore dependencies:
    ```bash
    dotnet restore
    ```

3. Update the connection string in `appsettings.Development.json` to point to your SQL Server instance.

4. Apply migrations to set up the database:
    ```bash
    dotnet ef database update
    ```

5. Run the application:
    ```bash
    dotnet run
    ```

6. Navigate to `https://localhost:7008/swagger` to view the API documentation.

## Configuration

The application configuration is stored in `appsettings.Development.json`. Key sections include:

- **Logging**: Configures the logging level.
- **ConnectionStrings**: Contains the connection string for the SQL Server database.
- **EncryptionSettings**: Stores the private key for ECC encryption.

## Encryption Mechanism

This project utilizes Elliptic Curve Cryptography (ECC) for securing sensitive data stored in the database. ECC is an asymmetric encryption technique that offers strong security with smaller key sizes compared to traditional methods like RSA. This makes ECC faster and more efficient in terms of performance and resource usage.

### How ECC Encryption Works

1. **Key Pair Generation**: ECC uses a pair of keys - a public key for encryption and a private key for decryption.
2. **Shared Secret Derivation**: The two parties (encryption and decryption) derive a shared secret using their private key and the other party's public key. This shared secret is then used to generate a symmetric encryption key (e.g., for AES).
3. **Data Encryption/Decryption**: The data is encrypted with AES using the derived symmetric key, and the initialization vector (IV) is prepended to the encrypted data.

### Advantages of ECC

- **Smaller Key Sizes**: ECC can achieve the same level of security as RSA with much smaller key sizes, reducing computational overhead.
- **Performance**: ECC-based systems are faster and require less processing power, making them ideal for environments where resources are limited.
- **Security**: ECC is resistant to quantum computing attacks, providing future-proof security compared to some other encryption methods.

## Database Configuration

The project uses Entity Framework Core to interact with a SQL Server database. The `ApplicationDbContext` class configures the database context and sets up encryption for specific columns.

The `Product` entity, for example, has a `Name` property that is encrypted before being stored in the database, using the `EncryptColumnAttribute` and EF Core value converters.

## How to Use

1. Define your domain models in the `Domain/Model` folder.
2. Apply the `[EncryptColumn]` attribute to any string properties you want to encrypt.
3. The encryption and decryption process will automatically occur when saving or retrieving data from the database.

## Additional ECC Encryption Details

Elliptic Curve Cryptography (ECC) is a type of public key cryptography based on the algebraic structure of elliptic curves over finite fields. ECC provides the same level of security as traditional public key cryptography methods like RSA but with much smaller key sizes.

### Key Concepts:

1. **Elliptic Curves**: ECC uses elliptic curves to define the public and private keys. An elliptic curve is defined by an equation like `y^2 = x^3 + ax + b` over a finite field.
2. **Key Size**: ECC achieves equivalent security to RSA but with smaller key sizes. For example, a 256-bit ECC key provides comparable security to a 3072-bit RSA key.
3. **Efficiency**: Due to the smaller key sizes, ECC requires less computational power, making it faster and more efficient, especially in environments with constrained resources (e.g., mobile devices).
4. **Quantum Resistance**: ECC is considered to have better resistance to quantum attacks compared to traditional algorithms like RSA and DSA.

ECC is commonly used in various security protocols like SSL/TLS, blockchain technology, and secure communication systems.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.
