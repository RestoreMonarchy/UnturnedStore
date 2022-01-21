﻿CREATE PROCEDURE dbo.GetPlugin
	@LicenseKey UNIQUEIDENTIFIER,
	@ProductName NVARCHAR(50),
	@BranchName NVARCHAR(255),
	@VersionName NVARCHAR(255),
	@ServerName NVARCHAR(255),
	@Host NVARCHAR(255),
	@Port INT,
	@ErrorMessage NVARCHAR(MAX) OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @customerId AS INT;
	DECLARE @productId AS INT;

	SELECT @customerId = Id, @productId = ProductId FROM dbo.ProductCustomers WHERE LicenseKey = @LicenseKey;

	IF @productId IS NULL
	BEGIN
		SET @ErrorMessage = 'Invalid license key';
		RETURN 1;
	END
		
	IF NOT EXISTS (SELECT * FROM dbo.Products WHERE @productId = Id AND Name = @ProductName)
	BEGIN
		SET @ErrorMessage = 'Product name is not the same product as license key';
		RETURN 2;
	END

	DECLARE @branchId INT;

	SELECT @branchId = Id FROM dbo.Branches 
	WHERE IsEnabled = 1 AND ProductId = @productId AND Name = @BranchName;
	
	IF @branchId IS NULL
	BEGIN
		SET @ErrorMessage = 'Branch not found';
		RETURN 3;
	END

	DECLARE @versionId INT;

	IF @VersionName = 'latest' OR @VersionName IS NULL
	BEGIN
		SELECT TOP 1 @versionId = Id FROM dbo.Versions 
		WHERE IsEnabled = 1 AND BranchId = @branchId ORDER BY CreateDate DESC;
	END
	ELSE
	BEGIN
		SELECT @versionId = Id FROM dbo.Versions 
		WHERE IsEnabled = 1 AND BranchId = @branchId AND Name = @VersionName;
	END
		
	IF @versionId IS NULL
	BEGIN
		SET @ErrorMessage = 'Version not found';
		RETURN 4;
	END

	IF EXISTS (SELECT * FROM dbo.CustomerServers WHERE Host = @Host AND Port = @Port)
	BEGIN
		UPDATE dbo.CustomerServers SET ServerName = @ServerName, TimesLoaded = TimesLoaded + 1, UpdateDate = SYSDATETIME()
		WHERE Host = @Host AND Port = @Port;
	END
	ELSE
	BEGIN
		INSERT INTO dbo.CustomerServers (CustomerId, ServerName, Host, Port) 
		VALUES (@customerId, @ServerName, @Host, @Port);
	END

	SELECT * FROM dbo.Versions WHERE Id = @versionId;
	RETURN 0;
END