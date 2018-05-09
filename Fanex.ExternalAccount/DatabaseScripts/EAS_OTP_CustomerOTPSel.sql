USE [CustDB]
GO
/****** Object:  StoredProcedure [dbo].[EAS_OTP_CustomerOTPSel]    Script Date: 05/03/2018 2:46:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[EAS_OTP_CustomerOTPSel]
	@CustID INT
AS
/*
	Creator: David
	Date: 2018-05-03
	DB: DBCUST.CustDB
	Task: Select secret key

	Revisions:

	Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT SecretKey, [Enabled], [Reset]
	FROM dbo.EASCustOTP WITH(NOLOCK)
	WHERE CustID = @CustID
END
GO
GRANT VIEW DEFINITION ON [dbo].[EAS_OTP_CustomerOTPSel] TO [bodbExtAccNet]
GO