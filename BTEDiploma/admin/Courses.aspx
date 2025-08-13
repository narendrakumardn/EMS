<%@ Page Title="Add / Update Course" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="BTEDiploma.admin.Courses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <div class="container mt-5" style="min-height: 600px;">
        <h2 class="text-center mb-4 text-primary">Add / Update Course</h2>

        <div class="row g-3 mb-4">
            <div class="col-md-4">
                <label>Course Code</label>
                <asp:TextBox ID="txtCourseCode" runat="server" CssClass="form-control" placeholder="e.g., CSE101" />
            </div>
            <div class="col-md-4">
                <label>Course Name</label>
                <asp:TextBox ID="txtCourseName" runat="server" CssClass="form-control" placeholder="Enter Course Name" />
            </div>
            <div class="col-md-4">
                <label>Credit</label>
                <asp:TextBox ID="txtCredit" runat="server" CssClass="form-control" placeholder="Numeric value" />
            </div>
            <div class="col-md-4">
                <label>Pattern ID</label>
                <asp:DropDownList ID="ddlPatternID" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Text="-- Select Pattern --" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>SEE Type</label>
                <asp:DropDownList ID="ddlCoursesSEE_Type" runat="server" CssClass="form-control">
                    <asp:ListItem Text="-- Select SEE Type --" Value="" />
                    <asp:ListItem Text="T" Value="1" />
                    <asp:ListItem Text="P" Value="2" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>Parent Program Code</label>
                <asp:DropDownList ID="ddlProgramCode" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Text="-- Select Program Code --" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>Scheme Code</label>
                <asp:DropDownList ID="ddlSchemeCode" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Text="-- Select Scheme Code --" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>Is Elective</label>
                <asp:DropDownList ID="ddlIsElective" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Text="-- Select Option --" Value="" />
                    <asp:ListItem Text="YES" Value="1" />
                    <asp:ListItem Text="NO" Value="2" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>Evaluation Pattern ID</label>
                <asp:DropDownList ID="ddlddlEval_Pattern_ID" runat="server" CssClass="form-control" AutoPostBack="true">
                    <asp:ListItem Text="-- Select Evaluation Pattern --" Value="" />
                </asp:DropDownList>
            </div>
            <div class="col-md-4">
                <label>CEE Pattern ID</label>
                <asp:DropDownList ID="ddlCEEPatternID" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Total Average" Value="1" />
                </asp:DropDownList>
            </div>
        </div>

        <asp:Button ID="btnSave" runat="server" Text="Save Course" CssClass="btn btn-success mb-3" OnClick="btnSave_Click" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary mb-3 ms-2" CausesValidation="false" />

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
    <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered mt-3"
    DataKeyNames="Course_Code"
    OnRowEditing="gvCourses_RowEditing"
    OnRowUpdating="gvCourses_RowUpdating"    
    OnRowCancelingEdit="gvCourses_RowCancelingEdit" 
    OnRowDeleting="gvCourses_RowDeleting">
    <Columns>
        <asp:BoundField DataField="Course_Code" HeaderText="Course Code" ReadOnly="True" />
        <asp:BoundField DataField="Course_Name" HeaderText="Course Name" />
        <asp:BoundField DataField="Credit" HeaderText="Credit" />
        <asp:BoundField DataField="Pattern_ID" HeaderText="Pattern ID" />
        <asp:BoundField DataField="See_Type" HeaderText="SEE Type" />
        <asp:BoundField DataField="Parent_Program_Code" HeaderText="Parent Program Code" />
        <asp:BoundField DataField="Scheme_Code" HeaderText="Scheme Code" />
        <asp:CheckBoxField DataField="Is_Elective" HeaderText="Elective" />
        <asp:BoundField DataField="Evaluation_Pattern_ID" HeaderText="Evaluation Pattern ID" />
        <asp:BoundField DataField="CIE_Pattern_ID" HeaderText="CIE Pattern ID" />

        <asp:CommandField ShowEditButton="True" ShowDeleteButton="True" />
    </Columns>
</asp:GridView>

    </div>

</asp:Content>