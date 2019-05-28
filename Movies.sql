-- Create a new table called '[Movies]' in schema '[dbo]'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[Movies]', 'U') IS NOT NULL
DROP TABLE [dbo].[Movies]
GO
-- Create the table in the specified schema
CREATE TABLE [dbo].[Movies]
(
    [Id] INT NOT NULL PRIMARY KEY, -- Primary Key column
    [MovieName] NVARCHAR(50) NOT NULL,
    [Synopsis] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(250) NOT NULL,
    [ReleaseDate] DATE NOT NULL,
    [Studio] NVARCHAR (75) NOT NULL
);
GO