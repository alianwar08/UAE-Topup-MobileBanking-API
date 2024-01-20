### README for UAE Top-Up Mobile Banking API

#### Overview

UAE Top-Up Mobile Banking API is a sophisticated backend solution designed for the online banking sector, specifically tailored to cater to the needs of underbanked employees in the UAE. This assigmnet is a part of FitTech’s initiative to provide financial inclusion services, enabling users to manage their top-up beneficiaries efficiently, explore various top-up options, and execute transactions seamlessly. This API integrates financial inclusivity with the efficiency and scalability of .NET Core, accentuated by asynchronous programming.

#### Requirements and Scope

The API offers a suite of functionalities designed for user convenience and financial control:
- Management of up to five active top-up beneficiaries per user.
- A diverse range of top-up options, from AED 5 to AED 100.
- Varied transaction limits based on user verification status, enhancing security.
- A monthly cap on top-up amounts, fostering responsible financial behavior.
- Dependency on available balance for transaction authorization.

#### Architecture and Design Patterns

The application embraces a multi-tiered architectural approach:
- **API Layer (ASP.NET Core Web API)**: Facilitates client-server communication through HTTP.
- **Business Logic Layer**: Processes and enforces business rules and validations.
- **Data Access Layer**: Utilizes Entity Framework Core for database interactions.
- **External Services**: Integrates with external systems for real-time balance updates and transactions.

Adopting the **Service Locator pattern** for dynamic dependency management, the design is further reinforced by **asynchronous programming**, ensuring efficient handling of concurrent requests.

#### Technology Stack

- **.NET Core**: Chosen for its cross-platform capabilities, performance, and modern architectural support.
- **Entity Framework Core**: Simplifies data access and manipulation.
- **ASP.NET Core Web API**: Ideal for crafting scalable web APIs.
- **SQL Server**: A dependable choice for scalable database management.
- **NUnit**: Provides a robust framework for comprehensive unit testing.

#### NUnit Test Cases

Our test suite ensures:
- **Transaction Limits for Verification Status**: Validation of transaction limits for both verified and unverified users.
- **Monthly Limit Compliance**: Testing adherence to the set monthly top-up limits.
- **Beneficiary Management**: Ensuring correct addition and management of beneficiaries.
- **Top-Up Option Integrity**: Verifying the availability and accuracy of top-up options.

#### Business Rules and Validations

Key rules include:
- **Beneficiary Limits**: Up to five active beneficiaries per user.
- **Top-Up Limits**: AED 500 monthly limit per beneficiary for unverified users; AED 1000 for verified users.
- **Monthly Transaction Cap**: AED 3000 limit across all beneficiaries.
- **Balance Verification**: Mandatory balance checks before transactions.
- **Transaction Fees**: A fixed charge of AED 1 per transaction.

#### Configuration

Configurations are pivotal for the application’s environment-specific settings:

- **API Configuration**: In `appsettings.json` within the `MobileBanking.API` project, this file includes crucial settings like database connection strings and external service URLs, dictating how the API interacts with other systems and databases.
- **NUnit Configuration**: Detailed in `appsettings.json` in the `MobileBanking.NUnit` project, these settings specify the configurations required for the NUnit test environment, including database connections and external service endpoints. These configurations ensure that the test environment is isolated from production, providing a safe sandbox for testing functionalities.

#### Asynchronous Programming

Incorporating asynchronous programming is key to the API’s responsive and efficient operation, particularly in handling operations like balance checks and top-up transactions, which require real-time data processing.

#### Conclusion

The Mobile Banking API exemplifies a modern approach to .NET Web API development. It blends asynchronous processing, rigorous testing, and a keen focus on user needs, culminating in a solution that is both scalable and maintainable, ready to meet the challenges of the dynamic digital banking sector.
