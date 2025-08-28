<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="BTEDiploma.login.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <title>Login - BTE Examination System</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="<%= ResolveUrl("~/Content/css/login.css") %>" rel="stylesheet" />
        <!-- below style not needed until u paste it into login.css-->
     <style>
.login-container {
    background: #fff;
    border-radius: 10px;
    padding: 30px;
    box-shadow: 0 0 10px rgba(0,0,0,0.1);
}
.nav-tabs .nav-link.active {
    background-color: #0d6efd;
    color: #fff;
}
.form-label {
    font-weight: 600;
}
.captcha-box {
    display: inline-block;
    background: #f1f1f1;
    padding: 10px;
    font-size: 18px;
    letter-spacing: 2px;
    font-weight: bold;
    border: 1px solid #ccc;
    min-width: 120px;
    text-align: center;
}
  </style> 


</head>
    <body>

        <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
       
        <div class="container py-5">
            <div class="row justify-content-center">
                <div class="col-lg-10">
                    <div class="row g-0">
                        <!-- Left image section -->
                        <!-- Left side with Vidhana Soudha image -->   

<div class="col-md-6 login-left">
                     
 
    <div class="overlay">
        <div class="text-center">
            <div class="bte-kannada">ಬಿ.ಟಿ.ಇ</div>
            <h2>ತಾಂತ್ರಿಕ ಪರೀಕ್ಷಾ ಮಂಡಳಿ  </h2>
        </div>
    </div>
</div>


                        <!-- Right login section -->
                        <div class="col-md-6 bg-white p-4 rounded-end">
                            <div class="login-container">
                                <h3> Login To Examination Portal</h3>
                                <!-- Tabs -->
                                <ul class="nav nav-tabs mb-3" id="loginTabs" role="tablist">
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link active" id="college-tab" data-bs-toggle="tab" data-bs-target="#college" type="button" role="tab">Institution</button>
                                    </li>
                                    <li class="nav-item" role="presentation">
                                        <button class="nav-link" id="student-tab" data-bs-toggle="tab" data-bs-target="#student" type="button" role="tab">Student</button>
                                    </li>
                                    <li class="nav-item" role="presentation">
        <button class="nav-link" id="employee-tab" data-bs-toggle="tab" data-bs-target="#college" type="button" role="tab">Employee</button>
    </li>
                                </ul>


                                <!-- Tab Contents -->
                                <div class="tab-content" id="loginTabsContent">
                                     <!-- College & Employee Login (Shared) -->
                                    <div class="tab-pane fade show active" id="college" role="tabpanel">
                                        <asp:UpdatePanel ID="updCollege" runat="server">
                                            <ContentTemplate>
                                                <div class="mb-3">
                                                    <label class="form-label">Username</label>
                                                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" ValidationGroup="College" />
                                                    <asp:RequiredFieldValidator ID="rfvUsername" runat="server" ControlToValidate="txtUsername" ErrorMessage="Username is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="College" />
                                                </div>
                                                <div class="mb-3">
                                                    <label class="form-label">Password</label>
                                                    <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" ValidationGroup="College" />
                                                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="College" />
                                                </div>
                                                <div class="mb-3">
                                                    <label class="form-label">CAPTCHA</label>
                                                    <div class="d-flex align-items-center gap-2">
                                                        <asp:Label ID="lblCaptcha" runat="server" CssClass="captcha-box" />
                                                        <asp:Button ID="btnRefreshCaptcha" runat="server" Text="⟳" CssClass="btn btn-outline-secondary" OnClick="btnRefreshCaptcha_Click" />
                                                    </div>
                                                    <asp:TextBox ID="txtCaptcha" runat="server" CssClass="form-control mt-2" placeholder="Enter CAPTCHA" ValidationGroup="College" />
                                                    <asp:RequiredFieldValidator ID="rfvCaptcha" runat="server" ControlToValidate="txtCaptcha" ErrorMessage="Captcha is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="College" />
                                                    <asp:Label ID="lblCaptchaError" runat="server" ForeColor="Red" />
                                                </div>
                                                <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-primary w-100" OnClick="btnLogin_Click" ValidationGroup="College" />
                                                <asp:Label ID="lblError" runat="server" ForeColor="Red" CssClass="d-block mt-2" />
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>

                                    <!-- Student Login -->
                                    <div class="tab-pane fade" id="student" role="tabpanel">
                                        <asp:UpdatePanel ID="updStudent" runat="server">
                                            <ContentTemplate>
                                                <div class="mb-3">
                                                    <label class="form-label">Register Number</label>
                                                    <asp:TextBox ID="txtStudentReg" runat="server" CssClass="form-control" ValidationGroup="Student" />
                                                    <asp:RequiredFieldValidator ID="rfvStudentReg" runat="server" ControlToValidate="txtStudentReg" ErrorMessage="Register number is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="Student" />
                                                </div>
                                                <div class="mb-3">
                                                    <label class="form-label">Password</label>
                                                    <asp:TextBox ID="txtStudentPassword" runat="server" TextMode="Password" CssClass="form-control" ValidationGroup="Student" />
                                                    <asp:RequiredFieldValidator ID="rfvStudentPwd" runat="server" ControlToValidate="txtStudentPassword" ErrorMessage="Password is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="Student" />
                                                </div>
                                                <asp:Button ID="btnStudentLogin" runat="server" Text="Login" CssClass="btn btn-success w-100" OnClick="btnStudentLogin_Click" ValidationGroup="Student" />
                                                <asp:Label ID="lblSuccess" runat="server" CssClass="text-success d-block mt-2" ClientIDMode="Static" Visible="false" />

                                                <asp:Label ID="lblStudentError" runat="server" ForeColor="Red" CssClass="d-block mt-2" />
                                            
                                            </ContentTemplate>
                                        </asp:UpdatePanel>
                                    </div>
                                </div>

                                <!-- Footer Links -->
                               
<!-- Footer Links (will be shown only for Institution login) -->
<div id="institutionLinks" class="d-flex justify-content-between mt-3" style="display:none;">
    <a id="forgotLink" href="#" class="text-decoration-none" onclick="goToForgotPassword()">Forgot Password?</a>
    <a id="newUserLink" href="#" class="text-decoration-none" onclick="goToNewUser()">New User? Register</a>
</div>
                                <!-- Main Message -->
<asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3 d-block"></asp:Label>
                            </div>
                        </div>
                    </div> <!-- end row g-0 -->
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hdnUserType" runat="server" ClientIDMode="Static" />


    </form>
   
    
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
  

        <!-- Your inline JS (for setting hidden field) -->
<script>
    document.addEventListener('DOMContentLoaded', function () {
        const forgotLink = document.getElementById('forgotLink');
        const newUserLink = document.getElementById('newUserLink');

        function clearErrors() {
            document.getElementById('<%= lblError.ClientID %>').innerText = '';
            document.getElementById('<%= lblStudentError.ClientID %>').innerText = '';
            document.getElementById('<%= lblCaptchaError.ClientID %>').innerText = '';
        }

        function setLinksVisibility(show) {
            forgotLink.style.display = show ? 'inline-block' : 'none';
            newUserLink.style.display = show ? 'inline-block' : 'none';
        }

        document.getElementById('college-tab').addEventListener('click', function () {
            document.getElementById('hdnUserType').value = 'I'; // Institution
            setLinksVisibility(true); // Show links
            clearErrors();
        });

        document.getElementById('employee-tab').addEventListener('click', function () {
            document.getElementById('hdnUserType').value = 'E'; // Employee
            setLinksVisibility(false); // Hide links
            clearErrors();
        });

        document.getElementById('student-tab').addEventListener('click', function () {
            document.getElementById('hdnUserType').value = 'S'; // Student
            setLinksVisibility(false); // Hide links
            clearErrors();
        });

        // Default state (Institution tab active)
        document.getElementById('hdnUserType').value = 'I';
        setLinksVisibility(true);
    });
</script>

    
        <script>
            function goToForgotPassword() {
                window.location.href = 'ForgotPassword1.aspx';
            }
        </script>

        <script>
            function goToForgotPassword1() {
        var userType = document.getElementById('<%= hdnUserType.ClientID %>').value;
    window.location.href = 'ForgotPassword.aspx?usertype=' + encodeURIComponent(userType);
       }

function goToNewUser() {
        var userType = document.getElementById('<%= hdnUserType.ClientID %>').value;
        window.location.href = 'Register.aspx?usertype=' + encodeURIComponent(userType);
            }
        </script>
        <script>
            setTimeout(function () {
                var msg = document.getElementById('<%= lblMessage.ClientID %>');
                if (msg) {
                    msg.style.transition = 'opacity 1s';
                    msg.style.opacity = '0';
                }
            }, 4000); // fades out after 4 seconds
        </script>
</body>
</html>