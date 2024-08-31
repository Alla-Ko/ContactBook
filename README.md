Program Description:

This application is a Contact Management System developed using the WPF (Windows Presentation Foundation) framework, which provides a modern UI for managing a list of contacts. The application allows users to perform CRUD (Create, Read, Update, Delete) operations on contact records. The application ensures data integrity by checking for duplicate entries before adding or updating a contact.


Technologies and Frameworks Used:

.NET Framework: The application is built on the .NET framework, leveraging its robust ecosystem.
WPF (Windows Presentation Foundation): Used for designing the user interface, enabling a rich and responsive UI experience.
Entity Framework Core: Used for data access and management, providing an object-relational mapping (ORM) to interact with the database.
SQLite: The application uses SQLite as the underlying database to store contact information.
Design Patterns Implemented:

MVVM (Model-View-ViewModel): The application follows the MVVM design pattern, which separates the UI logic (View) from the business logic (ViewModel) and data (Model). This ensures a clean architecture, making the code more maintainable and testable.
Command Pattern: Used for handling UI actions (e.g., button clicks) to maintain a clean separation between the user interface and the business logic.
Data Validation Pattern: Ensures that user input is validated in real-time, providing immediate feedback and preventing invalid data from being saved.
Other Features:

Custom Converters: Implemented custom value converters to handle data transformations, such as converting an integer phone number to a string for display purposes.
Property Change Notification: The application uses the INotifyPropertyChanged interface to ensure that any changes in the ViewModel are immediately reflected in the View.
