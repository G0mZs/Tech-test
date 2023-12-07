# Tech-test
 Test


Technology Choices:


Programming Language: C# (ASP.NET Core)
ASP.NET Core provides a robust and efficient framework for building RESTful APIs. It is cross-platform, highly performant, and well-supported.

Database: MongoDB
MongoDB is a NoSQL database that can handle large datasets and is suitable for storing flexible JSON-like documents.

API Documentation: Swagger/OpenAPI
Swagger/OpenAPI is widely used for documenting APIs. It provides a standardized way to describe RESTful APIs and can generate interactive documentation.

Testing Framework: xUnit
xUnit is a popular testing framework for .NET applications. It supports unit testing and integration testing.


Assumptions:


1 - The Call Type was not on the CSV so it was assumed that it should've been calculated based on the recipient and caller phone numbers.
2 - In The endpoint for the most expensive calls the user can choose the number of calls he wants to see (N).
3 - No front-end was done on this task (although it would be a good future enhancement).


Instructions to run the project:


1 - Install Visual Studio 2022 and Git

Links: Git -> https://git-scm.com/downloads
Visual Studio -> https://learn.microsoft.com/en-us/visualstudio/install/install-visual-studio?view=vs-2022

2 - Install Mongo DB Server (to run the database locally) and Mongo DB Compass (to consult the data in the local database)

Links: MongoDB Server -> https://www.mongodb.com/try/download/community (msi) (current version).
Mongo DB Compass -> https://www.mongodb.com/try/download/shell (exe) (current version)


Note: If the mongoDB server is not installed and running, the project will not work neither the integration test.

3 - Open a cmd and go to the path you would like to clone the repository

Execute the following commands:

git init
git remote add origin [C:\Users\Bernardo\Git\Tech-test\tech-test.bundle] -> Note: Use the path of the bundled file
git fetch --all
git reset --hard origin/development

After this, the folder has now the repository

Important Note: The development branch is up to date with the master (The feature branches used in this task were deleted after every pull request, but for each commit a respective branch with naming convention was created).


4 - Clean the solution and rebuild it

5 - Run the project on IIS Express (VS on any web browser)

6 - After running the project, the swagger with the necessary endpoints will appear on the web browser.

7 - Choose the upload endpoint (First one), click on "Try it out", upload the CSV and execute.

8 - After the last step, try the other endpoints. If the tester has no data to input check the database created in MongoDbCompass (Connect to mongodb://localhost:27017/ and open CallRecordsDb database and CallRecords collection or see the data)

9 - After testing the api (which also can be made trough Postman) close the api on web browser and run the test automation suite (Right-click on CallRecordApi.Tests in VS and click run tests)

This last step will run the unit tests and the integration test.


Considerations/Future Enhancements:

Authentication and Authorization:
Implement authentication mechanisms (e.g., JWT) and consider role-based access control for different API endpoints.

Logging and Monitoring:
Integrate logging to capture relevant events and errors. Implement monitoring solutions for performance and error tracking.

Pagination:
Implement pagination for endpoints that return lists of records to improve performance and user experience.

Caching:
Consider caching strategies, especially for frequently queried data, to enhance response times.

Optimizations:
Optimize queries for MongoDB based on usage patterns. Indexing and query optimization can significantly improve performance.

Data Validation and Sanitization:
Implement thorough input validation and sanitization to prevent security vulnerabilities (missed some data validations on uploading the file, when there are already CDRs with the same reference).

Dockerization:
Provide Dockerfiles and Docker Compose configurations for containerization, making it easier to deploy and scale.

Architecture:
As the volume of CDR records and new features increases, a more complex architecture is needed (Clean architecture, The standard, etc)
