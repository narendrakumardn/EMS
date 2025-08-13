
<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="BTEDiploma.login.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Chart.js CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>



<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="upPanelDashboard" runat="server">
        <ContentTemplate>

            <!-- Dashboard Header -->
            <div class="text-center my-4">
                <h2 class="fw-bold text-primary">Dashboard Overview</h2>
                <p class="text-muted">Statistics from all colleges</p>
            </div>

            <!-- Stat Cards -->
            <!-- First Row: Colleges and Students -->
<div class="row justify-content-center mb-5">
    <div class="col-md-4 mb-3">
        <div class="card shadow-lg border-0 bg-info text-white">
            <div class="card-body text-center">
                <h5 class="card-title">Total Colleges</h5>
                <h2 class="card-text"><asp:Label ID="lblColleges" runat="server" /></h2>
            </div>
        </div>
    </div>
    <div class="col-md-4 mb-3">
        <div class="card shadow-lg border-0 bg-success text-white">
            <div class="card-body text-center">
                <h5 class="card-title">Total Students</h5>
                <h2 class="card-text"><asp:Label ID="lblStudents" runat="server" /></h2>
            </div>
        </div>
    </div>
</div>

<!-- Second Row: Faculties and Fees Paid -->
<div class="row justify-content-center mb-5">
    <div class="col-md-4 mb-3">
        <div class="card shadow-lg border-0 bg-warning text-dark">
            <div class="card-body text-center">
                <h5 class="card-title">Total Faculties</h5>
                <h2 class="card-text"><asp:Label ID="lblFaculties" runat="server" /></h2>
            </div>
        </div>
    </div>
    <div class="col-md-4 mb-3">
        <div class="card shadow-lg border-0 bg-danger text-white">
            <div class="card-body text-center">
                <h5 class="card-title">Exam Fees Paid</h5>
                <h2 class="card-text"><asp:Label ID="lblFeesPaid" runat="server" /></h2>
            </div>
        </div>
    </div>
</div>

          </ContentTemplate>
    </asp:UpdatePanel>

   
</asp:Content>
