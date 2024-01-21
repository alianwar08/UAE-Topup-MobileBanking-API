### UAE Top-Up Mobile Banking API

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

### NUnit Test Scenarios

#### Verified User Transaction Tests

1. **Over Balance Failure Test (`Test000_Verified_OverBalance_Fail`)**:
   - **Scenario**: Verifies that a top-up transaction for a verified user fails when the user's balance is insufficient.
   - **Amount**: AED 100 attempted with an available balance of only AED 10.
   - **Expectation**: Transaction should be declined due to insufficient funds.

2. **Under Balance and Limit Success Test (`Test001_Verified_UnderBalanceUnderLimit_Success`)**:
   - **Scenario**: Checks successful transaction processing for a verified user within their available balance and monthly limit.
   - **Amount**: AED 100 top-up attempted with an available balance of AED 4000.
   - **Expectation**: Transaction should be successful as it's within the user's balance and monthly limit.

3. **Over Individual Monthly Limit Failure Test (`Test002_Verified_OverIndividualMonthlyLimit_Fail`)**:
   - **Scenario**: Ensures that cumulative top-up transactions for a verified user do not exceed the monthly limit per beneficiary.
   - **Amounts**: Multiple transactions totaling over AED 1000 (e.g., five AED 100 transactions followed by one AED 75 and one AED 30 transaction) for a single beneficiary within a month.
   - **Expectation**: The final transaction(s) should fail, ensuring adherence to the monthly limit of AED 1000 per beneficiary.

#### Unverified User Transaction Tests

1. **Over Balance Failure Test (`Test000_UnVerified_OverBalance_Fail`)**:
   - **Scenario**: Similar to the verified user test but for an unverified user. Validates the transaction failure when the balance is low.
   - **Amount**: AED 100 attempted with an available balance of AED 10.
   - **Expectation**: Transaction should fail due to insufficient balance.

2. **Under Balance and Limit Success Test (`Test001_UnVerified_UnderBalanceUnderLimit_Success`)**:
   - **Scenario**: Checks transaction success for an unverified user within their lower balance and monthly limit.
   - **Amount**: AED 100 top-up attempted with an available balance of AED 4000.
   - **Expectation**: Transaction should be successful, falling within the unverified user's reduced limit.

3. **Over Individual Monthly Limit Failure Test (`Test002_UnVerified_OverIndividualMonthlyLimit_Fail`)**:
   - **Scenario**: Ensures the enforcement of a stricter monthly top-up limit for unverified users.
   - **Amounts**: Repeated transactions totaling over AED 500 for a single beneficiary within a month.
   - **Expectation**: Transactions exceeding the AED 500 limit should be declined, validating the strict limit for unverified users.

#### Beneficiary Management Tests

1. **Addition and Validation of Beneficiaries**:
   - **Scenario**: Tests adding up to 5 beneficiaries, ensuring correct validation for each addition. Includes checks for invalid data like excessively long nicknames or invalid phone numbers.
   - **Expectation**: Valid beneficiaries should be added successfully, while invalid additions should be declined.

#### Top-Up Options Tests

1. **Validation of Top-Up Options**:
   - **Scenario**: Tests the API's capability to display and process various predefined top-up options (AED 5, AED 10, AED 20, AED 30, AED 50, AED 75, AED 100).
   - **Expectation**: All options should be presented accurately, and the system should correctly process the selected top-up amounts.

#### Configuration

Configurations are pivotal for the application’s environment-specific settings:

- **API Configuration**: In `appsettings.json` within the `MobileBanking.API` project, this file includes crucial settings like database connection strings and external service URLs, dictating how the API interacts with other systems and databases.
- **NUnit Configuration**: Detailed in `appsettings.json` in the `MobileBanking.NUnit` project, these settings specify the configurations required for the NUnit test environment, including database connections and external service endpoints. These configurations ensure that the test environment is isolated from production, providing a safe sandbox for testing functionalities.

#### Asynchronous Programming

Incorporating asynchronous programming is key to the API’s responsive and efficient operation, particularly in handling operations like balance checks and top-up transactions, which require real-time data processing.

#### Conclusion

The Mobile Banking API exemplifies a modern approach to .NET Web API development. It blends asynchronous processing, rigorous testing, and a keen focus on user needs, culminating in a solution that is both scalable and maintainable, ready to meet the challenges of the dynamic digital banking sector.
