<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamCreation.aspx.cs" Inherits="BTEDiploma.admin.ExamCreation"
    MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-white bg-primary p-3 rounded">Exam Creation</h2>

        <div class="row g-3 mt-3">

            <div class="col-md-6">
                <asp:Label runat="server" Text="Academic Year" CssClass="form-label" />
                <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlAcademicYear_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="Year" CssClass="form-label" />
                <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control"></asp:DropDownList>
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="Month" CssClass="form-label" />
                <asp:DropDownList ID="ddlMonth" runat="server" CssClass="form-control">
                    <asp:ListItem Text="January" Value="1" />
                    <asp:ListItem Text="February" Value="2" />
                    <asp:ListItem Text="March" Value="3" />
                    <asp:ListItem Text="April" Value="4" />
                    <asp:ListItem Text="May" Value="5" />
                    <asp:ListItem Text="June" Value="6" />
                    <asp:ListItem Text="July" Value="7" />
                    <asp:ListItem Text="August" Value="8" />
                    <asp:ListItem Text="September" Value="9" />
                    <asp:ListItem Text="October" Value="10" />
                    <asp:ListItem Text="November" Value="11" />
                    <asp:ListItem Text="December" Value="12" />
                </asp:DropDownList>
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="Start Date" CssClass="form-label" />
                <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="End Date" CssClass="form-label" />
                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="Regular Exam Fee" CssClass="form-label" />
                <asp:TextBox ID="txtRegularFee" runat="server" CssClass="form-control" TextMode="Number" placeholder="Enter amount in INR" />
            </div>

            <div class="col-md-6">
                <asp:Label runat="server" Text="Due Date" CssClass="form-label" />
                <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="col-md-12 text-center mt-3">
                <asp:Button ID="btnSubmit" runat="server" CssClass="btn btn-primary px-4" Text="Submit" OnClick="btnSubmit_Click" />
            </div>

        </div>

        <hr class="my-4" />

        <asp:GridView ID="gvExams" runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered table-striped"
            DataKeyNames="Exam_ID"
            OnRowEditing="gvExams_RowEditing"
            OnRowCancelingEdit="gvExams_RowCancelingEdit"
            OnRowUpdating="gvExams_RowUpdating">
            <Columns>
                <asp:BoundField DataField="Exam_ID" HeaderText="Exam ID" ReadOnly="True" />
                <asp:BoundField DataField="Academic_Year" HeaderText="Academic Year" />
                <asp:BoundField DataField="Exam_Month" HeaderText="Month" />
                <asp:BoundField DataField="Exam_Year" HeaderText="Year" />
                <asp:BoundField DataField="Academic_Start_Date" HeaderText="Start Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Academic_End_Date" HeaderText="End Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Regular_Fee" HeaderText="Fee" />
                <asp:BoundField DataField="Due_Date" HeaderText="Due Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:CommandField ShowEditButton="True" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
