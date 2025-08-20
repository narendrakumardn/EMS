<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master"  CodeBehind="StudentUploadBulk.aspx.cs" Inherits="BTEDiploma.admin.StudentUploadBulk" %>



<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="True" />
    <div class="container mt-4">
        <h3>Upload Student Details from ACM database</h3>
        
        <!-- Filters -->
         Academic Year:
 <asp:DropDownList ID="ddlAcademicYear" runat="server" Width="300px" />

<!-- Student Grid -->

< <asp:Button ID="btnImport" runat="server" Text="Import Students" OnClick="btnImport_Click" />
        <br /><br />

        <asp:Label ID="lblResult" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>

         


    
</asp:Content>