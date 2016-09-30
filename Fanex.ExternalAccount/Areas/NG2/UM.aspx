<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UM.aspx.cs" Inherits="Areas_NG2_UM" %>
<%
    var service = new ONEbook.UM.ThirdParty.Services.Impl.UMService();
    var um = service.GetUnderMaintenanceTime(23);
    var UMfrom = um.From;
    var UMto = um.To;
 %>
<!DOCTYPE html>
<html>
    <head>
        <title>UM</title>
        <meta charset="utf-8" />
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link href="Styles/um.css" rel="stylesheet" />
    </head>
    <body>
        <hr />
        <h1 class="title"><span class="warning-icon-title">Under Maintenance</span></h1>
        <hr />
        <h2 class="sub-title">Dear Valued Customers:</h2>
        <p>
            We are having server maintenance, website will be unavailable from <br />
            HK time <span  class="title"><%=UMfrom %> until <%=UMto%> </span>(GMT+ 8). Sorry for the inconvenience caused.
        </p>
    </body>
</html>