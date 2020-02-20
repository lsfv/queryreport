SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
SET NOCOUNT ON

DECLARE @dbversion nvarchar(10)
SELECT @dbversion = Value FROM SYSTEMPARAMETER WHERE Name = N'SCHEMA_VERSION'

-- Use the following as template for update, say if there is an update for 2016-10-28
/*
IF @dbversion = N'2016-10-27'
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.objects o WHERE name = 'something')
	BEGIN
		BEGIN TRANSACTION
		BEGIN TRY
			EXEC sp_executesql N'UPDATE SYSTEMPARAMETER SET Value = N''2016-10-28'' WHERE Name = N''SCHEMA_VERSION'''
			COMMIT
			SET @dbversion = N'2016-10-28'
			PRINT 'INFO: <object> has been <action> successfully'
		END TRY
		BEGIN CATCH
			ROLLBACK
			PRINT 'ERROR: Unable to <action> <object>.'
			PRINT ERROR_MESSAGE()
		END CATCH
	END
	ELSE
	BEGIN
		PRINT 'INFO: <object> has been <action> already'
	END
END
*/