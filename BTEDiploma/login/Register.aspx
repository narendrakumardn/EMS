<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="BTEDiploma.login.Register" %>
<%@ MasterType VirtualPath="~/Site1.master" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title>Register New User</title>
    <!-- Bootstrap -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .header-banner {
            background: linear-gradient(to right, #0b355d, #1a4b8c);
            padding: 12px 0;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            color: #fff;
        }
        .header-logo-frame {
            background: #fff;
            padding: 8px;
            border-radius: 4px;
            display: inline-block;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        .form-container {
            max-width: 500px;
            margin: 40px auto;
            background: #f8f9fa;
            padding: 30px;
            border-radius: 12px;
            box-shadow: 0 0 15px rgba(0,0,0,0.15);
        }
        .form-title {
            text-align: center;
            margin-bottom: 25px;
            font-weight: bold;
            color: #2c3e50;
        }
    </style>
</head>
<body class="min-vh-100 d-flex flex-column">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <!-- ===== Header Banner ===== -->
        <header class="header-banner">
            <div class="container" style="max-width:1140px;">
                <div class="row align-items-center">
                    <!-- Logo -->
                    <div class="col-md-2 text-center text-md-start mb-2 mb-md-0">
                        <div class="header-logo-frame">
                            <asp:HyperLink ID="lnkHeaderLogo" runat="server" NavigateUrl="~/login/Register.aspx" ToolTip="Go to Registration">
                                <img src='<%= ResolveUrl("~/images/kar_logo.jpg") %>' 
                                     alt="BTE Logo" 
                                     style="height:70px; width:auto; object-fit:contain;" />
                            </asp:HyperLink>
                        </div>
                    </div>

                    <!-- Title -->
                    <div class="col-md-8 text-center mb-2 mb-md-0">
                        <h1 class="h5 m-0 fw-bold">BTE Examination System</h1>
                        <div class="small">User Registration</div>
                    </div>

                    <!-- Right side action (optional) -->
                    <div class="col-md-2 text-center text-md-end">
                        <%-- Uncomment if you want a Login link in header:
                        <asp:HyperLink ID="lnkHeaderLogin" runat="server" NavigateUrl="~/login/Login.aspx" CssClass="btn btn-sm btn-light">
                            Back to Login
                        </asp:HyperLink>
                        --%>
                    </div>
                </div>
            </div>
        </header>
        <!-- ===== End Header Banner ===== -->

        <!-- Main Content -->
        <div class="container flex-grow-1">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="form-container">
                        <h2 class="form-title">User Registration</h2>

                        <div class="mb-3">
                            <label for="txtUsername" class="form-label">Username</label>
                            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
                                ControlToValidate="txtUsername"
                                ErrorMessage="Username required"
                                CssClass="text-danger" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <label for="txtPassword" class="form-label">Password</label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                                ControlToValidate="txtPassword"
                                ErrorMessage="Password required"
                                CssClass="text-danger" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <label for="txtName" class="form-label">Full Name</label>
                            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvName" runat="server"
                                ControlToValidate="txtName"
                                ErrorMessage="Name required"
                                CssClass="text-danger" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <label for="ddlCollegeCode" class="form-label">College Code</label>
                            <asp:DropDownList ID="ddlCollegeCode" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                            <asp:RequiredFieldValidator ID="rfvCollege" runat="server"
                                ControlToValidate="ddlCollegeCode" InitialValue=""
                                ErrorMessage="Select a college"
                                CssClass="text-danger" Display="Dynamic" />
                        </div>

                        <div class="mb-3">
                            <label for="ddlUserType" class="form-label">User Type</label>
                            <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-select" AppendDataBoundItems="true" />
                            <asp:RequiredFieldValidator ID="rfvUserType" runat="server"
                                ControlToValidate="ddlUserType" InitialValue=""
                                ErrorMessage="Please select a user type"
                                CssClass="text-danger" Display="Dynamic" />
                        </div>

                        <!-- Mobile -->
                        <div class="mb-3">
                            <label for="txtMobile" class="form-label">Mobile Number</label>
                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" MaxLength="10" />
                            <asp:RegularExpressionValidator ID="revMobile" runat="server"
                                ControlToValidate="txtMobile"
                                ValidationExpression="^[6-9]\d{9}$"
                                ErrorMessage="Enter a valid mobile number"
                                CssClass="text-danger"
                                Display="Dynamic" />
                        </div>

                        <div class="d-grid">
                            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn btn-primary" OnClick="BtnRegister_Click" />
                        </div>

                        <div class="mt-3 text-center">
                            <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold" />
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <!-- /Main Content -->
    </form>
</body>
</html>
