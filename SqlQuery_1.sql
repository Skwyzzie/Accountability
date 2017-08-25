CREATE TABLE [dbo].[Scans]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[barcode] TEXT NOT NULL, 
	[inHouse] BIT NOT NULL,
	[time] TIME
)