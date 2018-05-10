USE [CustDB]
GO
/****** Object:  StoredProcedure [dbo].[EAS_OTP_CustomerOTPReset]    Script Date: 05/10/2018 9:30:30 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EAS_OTP_CustomerOTPReset]
	@CustID		INT,
	@Reset		BIT,
	@ErrCode	INT = 0 OUTPUT,
	@ErrMsg		NVARCHAR(200) = '' OUTPUT
AS
/*
	Creator: David
	Date: 2018-05-10
	DB: DBCUST.CustDB
	Task: Reset 

	Revisions:

	Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

BEGIN TRY
	UPDATE [dbo].[EASCustOTP] WITH(ROWLOCK, UPDLOCK) 
	SET  [Reset] = @Reset
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
GRANT EXECUTE ON [dbo].[EAS_OTP_CustomerOTPReset] TO [bodbExtAccNet]
GO