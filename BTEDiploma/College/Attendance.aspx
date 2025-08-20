<%@ Page Title="Student Attendance" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Attendance.aspx.cs" Inherits="BTEDiploma.admin.Attendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h3 class="text-danger">
        ⚠️ Student attendance should be entered correctly without any issues. Please verify before saving.
    </h3>

    <div class="row mb-3">
        <div class="col-md-4">
            <asp:Label ID="lblProgram" runat="server" Text="Select Program:" AssociatedControlID="ddlProgram" />
            <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProgram_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="col-md-4">
            <asp:Label ID="lblCourse" runat="server" Text="Select Course:" AssociatedControlID="ddlCourse" />
            <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged"></asp:DropDownList>
        </div>
    </div>

    <asp:GridView ID="gvAttendance" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
        DataKeyNames="Sem_History_ID,Course_Code" OnRowEditing="gvAttendance_RowEditing"
        OnRowCancelingEdit="gvAttendance_RowCancelingEdit" OnRowUpdating="gvAttendance_RowUpdating">
        
        <Columns>
            <asp:BoundField DataField="Student_Name" HeaderText="Student Name" ReadOnly="true" />
            <asp:BoundField DataField="Register_Number" HeaderText="Register Number" ReadOnly="true" />
            <asp:BoundField DataField="Course_Name" HeaderText="Course" ReadOnly="true" />
            <asp:BoundField DataField="Classes_Conducted" HeaderText="Classes Conducted" />
            <asp:BoundField DataField="Classes_Attended" HeaderText="Classes Attended" />

            <asp:CommandField ShowEditButton="true" EditText="Edit" UpdateText="Save" CancelText="Cancel" />
        </Columns>
    </asp:GridView>

    <asp:Label ID="lblMessage" runat="server" CssClass="text-success"></asp:Label>
    <asp:Label ID="lblError" runat="server" CssClass="text-danger"></asp:Label>

</asp:Content>
