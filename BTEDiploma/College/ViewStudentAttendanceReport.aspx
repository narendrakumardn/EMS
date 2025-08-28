<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master" 
    CodeBehind="ViewStudentAttendanceReport.aspx.cs" Inherits="BTEDiploma.admin.ViewStudentAttendanceReport" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    

    <div style="margin-bottom:15px;">
        
        
    </div>

    <!-- Grid preview -->
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered table-striped" />

    <!-- Optional ReportViewer (for RDLC export) -->
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="600px"
        ProcessingMode="Local" Visible="false">
    </rsweb:ReportViewer>
     <div class="text-center my-3">
         <asp:Button ID="btnExport" runat="server" Text="Download PDF" OnClick="btnExport_Click" CssClass="btn btn-success" /> </div>
</asp:Content>
