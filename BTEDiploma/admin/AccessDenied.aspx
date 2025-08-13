<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="BTEDiploma.admin.AccessDenied" %>


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Denied</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f8f9fa;
        }

        .access-denied-card {
            max-width: 500px;
            margin: 100px auto;
            padding: 30px;
            border-radius: 15px;
            background: white;
            box-shadow: 0 0 15px rgba(0, 0, 0, 0.1);
        }

        .icon {
            font-size: 48px;
            color: #dc3545;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container text-center">
            <div class="access-denied-card">
                <div class="icon mb-3">
                    <i class="fas fa-ban"></i>
                </div>
                <h2>Access Denied</h2>
                <p>You do not have permission to view this page.</p>
                <asp:Button ID="btnBack" runat="server" Text="Go Back to Dashboard" CssClass="btn btn-danger" OnClick="btnBack_Click" />
            </div>
        </div>
    </form>

    <script src="https://kit.fontawesome.com/a076d05399.js" crossorigin="anonymous"></script>
</body>
</html>
