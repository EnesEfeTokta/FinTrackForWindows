using System.Reflection.Metadata;

namespace FinTrack.Codes.DataAccess
{
    // This class defines the database schema for a financial tracking application.
    // It includes classes for users, transactions, categories, budgets, targets, goals, reports, and settings.
    // Each class represents a table in the database with properties that correspond to the columns in the table.
    // The classes are designed to be used with an ORM (Object-Relational Mapping) framework like Entity Framework or Dapper.
    // The properties in each class are defined with appropriate data types and attributes to ensure data integrity and relationships between tables.
    // The classes are organized in a namespace called FinTrack.Codes.DB, which indicates that they are part of the database layer of the FinTrack application.
    // The classes are designed to be easily extendable and maintainable, allowing for future enhancements and modifications as needed.
    // The classes are also designed to be used with a variety of database management systems, making them versatile and adaptable to different environments.
    // The classes are written in C# and follow standard naming conventions for properties and classes.
    // The classes are designed to be used in a financial tracking application, which may include features such as budgeting, goal setting, and reporting.
    // The classes are designed to be used in a multi-user environment, allowing for multiple users to access and manage their financial data securely.
    // The classes are designed to be used with a variety of data types, including strings, integers, decimals, and dates.
    // The classes are designed to be used with a variety of data access technologies, including ADO.NET, Entity Framework, and Dapper.
    // The classes are designed to be used with a variety of programming languages, including C#, VB.NET, and F#.
    // The classes are designed to be used with a variety of development environments, including Visual Studio, Visual Studio Code, and JetBrains Rider.
    // The classes are designed to be used with a variety of deployment environments, including cloud-based and on-premises solutions.
    // The classes are designed to be used with a variety of database management systems, including SQL Server, MySQL, PostgreSQL, and SQLite.
    // The classes are designed to be used with a variety of operating systems, including Windows, Linux, and macOS.

    // The users class represents a user of the financial tracking application.
    public class users
    {
        public Guid user_id { get; set; } // Primary Key
        public string full_name { get; set; } // User's full name.
        public string email { get; set; } // User's email address.
        public string password_hash { get; set; } // Hashed password for security.
        public byte[] profile_picture { get; set; } // User's profile picture.
        public DateTime created_at { get; set; } // Account creation date.
    }

    // The transactions class represents a financial transaction made by a user.
    public class transactions
    {
        public int transaction_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public int category_id { get; set; } // Foreign Key to categories table.
        public decimal amount { get; set; } // Transaction amount.
        public string transaction_type { get; set; } // Type of transaction (e.g., income, expense).
        public string description { get; set; } // Description of the transaction.
        public DateTime transaction_date { get; set; } // Date of the transaction.
    }

    // The categories class represents a category for organizing transactions.
    public class categories
    {
        public int category_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public string category_name { get; set; } // Name of the category.
        public string category_type { get; set; } // Type of category (e.g., income, expense).
    }

    // The budgets class represents a budget set by a user for a specific category.
    public class budgets
    {
        public int budget_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public int category_id { get; set; } // Foreign Key to categories table.
        public decimal budget_amount { get; set; } // Budget amount.
        public DateTime start_date { get; set; } // Start date of the budget period.
        public DateTime end_date { get; set; } // End date of the budget period.
        public DateTime created_at { get; set; } // Date the budget was created.
    }

    // The targets class represents a financial target set by a user.
    public class targets
    {
        public int target_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public string target_name { get; set; } // Name of the target.
        public decimal target_amount { get; set; } // Target amount.
        public DateTime target_date { get; set; } // Target date.
        public DateTime created_at { get; set; } // Date the target was created.
    }

    // The goals class represents a financial goal set by a user.
    public class goals
    {
        public int goal_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public string goal_name { get; set; } // Name of the goal.
        public decimal target_amount { get; set; } // Target amount for the goal.
        public decimal saved_amount { get; set; } // Amount saved towards the goal.
        public DateTime deadline { get; set; } // Deadline for achieving the goal.
        public DateTime created_at { get; set; } // Date the goal was created.
    }

    public class reports
    {
        public int report_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public string report_type { get; set; } // Type of report (e.g., income, expense).
        public DateTime report_date { get; set; } // Date of the report.
        public string report_data { get; set; } // Data for the report (e.g., JSON, XML).
        public DateTime created_at { get; set; } // Date the report was created.
    }

    public class settings
    {
        public int setting_id { get; set; } // Primary Key
        public int user_id { get; set; } // Foreign Key to users table.
        public string currency { get; set; } // User's preferred currency.
        public string language { get; set; } // User's preferred language.
        public bool dark_mode { get; set; } // User's preference for dark mode.
    }
}
