USE [CustDB]
GO
/*
	Creator: David
	Date: 2018-05-10
	DB: DBCUST.CustDB
	Task: Update secret key 

	Revisions:

	Param's Explanation:
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EAS_OTP_CustomerOTPSecretKeyUpdate]
	@CustID		INT,
	@SecretKey	NVARCHAR(50),
	@ErrCode	INT = 0 OUTPUT,
	@ErrMsg		NVARCHAR(200) = '' OUTPUT
AS
BEGIN
	SET NOCOUNT ON

BEGIN TRY
	UPDATE [dbo].[EASCustOTP] WITH(ROWLOCK, UPDLOCK) 
	SET  [SecretKey] = @SecretKey,
		 [ModifiedDate] = GETDATE()
	WHERE dbo.EASCustOTP.CustID = @CustID
 END TRY
 BEGIN CATCH
	 IF @@Error <> 0
	 BEGIN   
	  SELECT @ErrMsg = 'Error! Error when reset OTP.', @ErrCode = 101   
	  RETURN
	 END  
 END CATCH
END
GO
GRANT EXECUTE ON [dbo].[EAS_OTP_CustomerOTPSecretKeyUpdate] TO [bodbExtAccNet]
GO