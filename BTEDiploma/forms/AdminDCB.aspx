<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AdminDCB.aspx.cs" Inherits="BTEDiploma.forms.AdminDCB" %>



<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid mt-4">

        <h3 class="text-primary mb-4">All Colleges - DCB Summary</h3>
        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger fw-bold"></asp:Label>

        <!-- College-wise DCB Grid -->
        <asp:GridView ID="gvColleges" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover"
            DataKeyNames="CollegeCode" OnRowCommand="gvColleges_RowCommand">
            <Columns>
                <asp:BoundField DataField="CollegeCode" HeaderText="College Code" />
                <asp:BoundField DataField="StudentCount" HeaderText="Students" />
                <asp:BoundField DataField="TotalDemand" HeaderText="Demand" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="TotalCollection" HeaderText="Collection" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />

                <asp:ButtonField ButtonType="Button" CommandName="ViewBranches" Text="View Branches" ControlStyle-CssClass="btn btn-primary btn-sm" />
            </Columns>
        </asp:GridView>

        <!-- Branch-wise DCB Grid -->
        <asp:Panel ID="pnlBranchWise" runat="server" Visible="false" CssClass="mt-5">
            <h4 class="text-success mb-3">Branch-wise DCB for: <asp:Label ID="lblSelectedCollege" runat="server" CssClass="fw-bold text-success"></asp:Label></h4>
            <asp:GridView ID="gvBranchDCB" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered">
                <Columns>
                    <asp:BoundField DataField="BranchCode" HeaderText="Branch Code" />
                    <asp:BoundField DataField="BranchName" HeaderText="Branch Name" />
                    <asp:BoundField DataField="StudentCount" HeaderText="Students" />
                    <asp:BoundField DataField="Demand" HeaderText="Demand" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Collection" HeaderText="Collection" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="Balance" HeaderText="Balance" DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
        </asp:Panel>

    </div>
</asp:Content>
