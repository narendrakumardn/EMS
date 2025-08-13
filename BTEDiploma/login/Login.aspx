<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BTEDiploma.login.Login" %>



<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title>Login - BTE Examination System</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
   
    <link href="<%= ResolveUrl("~/Content/css/login.css") %>" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />

        <div class="container py-5">
            <div class="row justify-content-center">
                <div class="col-lg-10">
                    <div class="login-container">
                        <div class="row g-0">
                            <!-- Left side with Vidhana Soudha image -->   
                            
                            <div class="col-md-6 login-left">
                     
 
                                <div class="overlay">
                                    <div class="text-center">
                                        <div class="bte-kannada">ಬಿ.ಟಿ.ಇ</div>
                                        <h2>ತಾಂತ್ರಿಕ ಪರೀಕ್ಷಾ ಮಂಡಳಿ  </h2>
                                    </div>
                                </div>
                            </div>

                            <!-- Right side with login form -->
                            <div class="col-md-6">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <div class="form-container">
                                            <div class="text-center mb-4">
                                                <h3>Login to Examination Portal</h3>
                                            </div>

                                            <div class="mb-3">
                                                <label for="txtUsername" class="form-label">Username</label>
                                                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
                                                    AutoPostBack="true"  autofocus="autofocus"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvUsername" runat="server"
        ControlToValidate="txtUsername" ErrorMessage="Username is required"
        CssClass="text-danger" Display="Dynamic" />
                                            </div>
                                           

                                            <div class="user-type-indicator">
                                                <asp:Label ID="lblUserType" runat="server" Text=""></asp:Label>
                                            </div>

                                            <div class="mb-3">
                                                <label for="txtPassword" class="form-label">Password</label>
                                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                                                  <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
        ControlToValidate="txtPassword" ErrorMessage="Password is required"
        CssClass="text-danger" Display="Dynamic" />
                                            </div>

                                           
<div class="mb-3 captcha-container">
    <label class="form-label">Enter the CAPTCHA shown below</label>
    
    <div class="d-flex align-items-center gap-2">
        <asp:Label ID="lblCaptcha" runat="server" CssClass="captcha-box">
             

        </asp:Label>
        <asp:Button ID="btnRefreshCaptcha" runat="server" Text="⟳" CssClass="btn btn-outline-secondary"
            OnClick="btnRefreshCaptcha_Click" />
    </div>
    
    <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control mt-2"
        placeholder="Enter CAPTCHA here"></asp:TextBox>
    <asp:RequiredFieldValidator ID="rfvCaptcha" runat="server"
        ControlToValidate="txtCaptcha" ErrorMessage="Captcha is required"
        CssClass="text-danger" Display="Dynamic" />

    <asp:Label ID="lblCaptchaError" runat="server" ForeColor="Red" />
</div>


                                            <asp:Button ID="btnLogin" runat="server" Text="Login"
                                                CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" />
                                                <asp:Label ID="lblError" runat="server" ForeColor="Red" />

                                            <!-- New Options Added Here -->
                                            <div class="d-flex justify-content-between mt-3">
                                                <asp:HyperLink ID="lnkForgotPassword" runat="server"
                                                    NavigateUrl="~/login/ForgotPassword.aspx" CssClass="text-decoration-none">
                                                    Forgot Password?
                                                </asp:HyperLink>
                                                <asp:HyperLink ID="lnkNewUser" runat="server"
                                                    NavigateUrl="~/login/Register.aspx" CssClass="text-decoration-none">
                                                    New User? Register
                                                </asp:HyperLink>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
