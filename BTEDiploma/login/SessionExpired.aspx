<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionExpired.aspx.cs" Inherits="BTEDiploma.login.SessionExpired" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SESSION EXPIRE</title>
</head>
<body>
    <form id="form1" runat="server" autocomplete="off">
        <div>
            <table align="center">
                <tr>
                    <td style="font-family: Calibri; font-size: xx-large; font-weight: bold; font-style: normal; font-variant: normal; text-transform: capitalize; color: #FF0000">SESSION EXPIRED !!! PLS TRY AGAIN
                    </td>
                </tr>
                <tr>
                    <td colspan="1">
                        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/login/Login.aspx">Back to Login</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </div>
        
    </form>
</body>
</html>