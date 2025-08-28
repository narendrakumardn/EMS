<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/MainSite.Master" AutoEventWireup="true" CodeBehind="ForgotPassword1.aspx.cs" Inherits="BTEDiploma.login.ForgotPassword1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="/Content/css/login.css" rel="stylesheet" />

   
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5">
        <div class="d-flex justify-content-center">
            <div class="login-container p-4 shadow rounded bg-white" style="max-width: 600px; width: 100%;">
                <h4 class="text-center mb-4">Change Password</h4>

                <!-- User Type -->
<div class="mb-3">
    <label for="ddlUserType" class="form-label">Select User Type</label>
    <asp:DropDownList ID="ddlUserType" runat="server" CssClass="form-control">
        <asp:ListItem Text="-- Select User Type --" Value="" />
        <asp:ListItem Text="Student" Value="S" />
        <asp:ListItem Text="Institute" Value="I" />
        <asp:ListItem Text="Employee" Value="E" />
    </asp:DropDownList>
</div>

                <!-- Username -->
                <div class="mb-3">
                    <label for="txtUsername" class="form-label">Enter Username</label>
                    <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Registered Username"></asp:TextBox>
                </div>

                <!-- Send OTP Button -->
                <div class="mb-3">
                    <asp:Button ID="btnSendOTP" runat="server" CssClass="btn btn-primary w-100" Text="Send OTP" OnClick="btnSendOTP_Click" />
                </div>

                <!-- OTP + New Password -->
                <asp:Panel ID="pnlOTP" runat="server" Visible="false">
                    <!-- OTP -->
                    <div class="mb-3">
                        <label for="txtOTP" class="form-label">Enter OTP</label>
                        <asp:TextBox ID="txtOTP" runat="server" CssClass="form-control" placeholder="Enter OTP"></asp:TextBox>
                    </div>

                    <!-- Password UpdatePanel -->
       
<asp:UpdatePanel ID="updPasswordValidation" runat="server">
    <ContentTemplate>
        <!-- New Password -->
        <div class="mb-3">
            <label for="txtNewPassword" class="form-label">New Password</label>
            <asp:TextBox 
                ID="txtNewPassword" 
                runat="server" 
                CssClass="form-control"
                TextMode="Password" 
                onkeyup="triggerValidation()" 
                placeholder="New Password">
            </asp:TextBox>
        </div>

        <!-- Confirm Password -->
        <div class="mb-3">
            <label for="txtConfirmPassword" class="form-label">Confirm Password</label>
            <asp:TextBox 
                ID="txtConfirmPassword" 
                runat="server" 
                CssClass="form-control" 
                TextMode="Password" 
                placeholder="Re-enter New Password">
            </asp:TextBox>
        </div>

        <!-- Validation message -->
       <div> <asp:Label ID="lblPasswordValidation" runat="server" ></asp:Label></div>
     <div>     <asp:Label ID="lblConfirmPasswordValidation" runat="server" ></asp:Label></div>

        <!-- Reset Password Button -->
        <div class="mb-3">
            <asp:Button 
                ID="btnResetPassword" 
                runat="server" 
                CssClass="btn btn-success w-100" 
                Text="Reset Password" 
                OnClick="btnResetPassword_Click" />
        </div>
    </ContentTemplate>

    <Triggers>
       
        <asp:AsyncPostBackTrigger ControlID="btnResetPassword" EventName="Click" />
    </Triggers>
</asp:UpdatePanel>

                
                </asp:Panel>

<script>
    // Password strength validation
    document.getElementById('<%= txtNewPassword.ClientID %>').addEventListener('keyup', function () {
        let pwd = this.value;
        let pattern = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$/;
        let label = document.getElementById('<%= lblPasswordValidation.ClientID %>');

        if (!pwd) {
            label.innerHTML = "Password cannot be empty.";
            label.style.color = "red";
        } else if (!pattern.test(pwd)) {
            label.innerHTML = "Password must be at least 8 characters long, include uppercase, lowercase, number, and special character.";
            label.style.color = "red";
        } else {
            label.innerHTML = "Strong password ✔";
            label.style.color = "green";
        }

        // Also check if passwords match
        checkPasswordMatch();
    });

    // Confirm password match validation
    document.getElementById('<%= txtConfirmPassword.ClientID %>').addEventListener('keyup', checkPasswordMatch);

    function checkPasswordMatch() {
        let pwd = document.getElementById('<%= txtNewPassword.ClientID %>').value;
        let confirmPwd = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;
        let matchLabel = document.getElementById('<%= lblConfirmPasswordValidation.ClientID %>');

        if (!confirmPwd) {
            matchLabel.innerHTML = "";
        } else if (pwd !== confirmPwd) {
            matchLabel.innerHTML = "Passwords do not match.";
            matchLabel.style.color = "red";
        } else {
            matchLabel.innerHTML = "Passwords match ✔";
            matchLabel.style.color = "green";
        }
    }
</script>



                <!-- Main Message -->
                <asp:Label ID="lblMessage" runat="server" ></asp:Label>
                <asp:Label ID="lblSuccessMessage" runat="server" CssClass="text-success mt-3 d-block"></asp:Label>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hdnUserType" runat="server" ClientIDMode="Static" />
</asp:Content>
