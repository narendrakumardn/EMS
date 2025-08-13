<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CreateCurriculumScheme.aspx.cs" Inherits="BTEDiploma.admin.CreateCurriculumScheme" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <div class="card shadow-sm">
            <div class="card-header bg-primary text-white">
                <h4 class="mb-0">Create Curriculum Scheme</h4>
            </div>
            <div class="card-body">
    <div class="row g-3">
        <div class="col-md-4">
            <label class="form-label">Scheme Code</label>
            <asp:TextBox ID="txtCurriculumCode" runat="server" CssClass="form-control" placeholder="Enter Scheme Code" />
        </div>
        <div class="col-md-4">
            <label class="form-label">Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control" placeholder="Enter Scheme Name" />
        </div>
        <div class="col-md-4">
            <label class="form-label">Academic Year</label>
            <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-select">
                <asp:ListItem Text="-- Select Academic Year --" Value="" />
            </asp:DropDownList>
        </div>
        <div class="col-md-4">
            <label class="form-label">Attendance Percentage</label>
            <asp:TextBox ID="txtAttendance" runat="server" CssClass="form-control" placeholder="Enter Attendance %" />
        </div>
        <div class="col-md-4">
            <label class="form-label">Condonation Allowed</label>
            <asp:TextBox ID="txtCondonation" runat="server" CssClass="form-control" placeholder="Enter Condonation %" />
        </div>
        <div class="col-md-4">
            <label class="form-label">No. of Weeks</label>
            <asp:TextBox ID="txtWeeks" runat="server" CssClass="form-control" placeholder="Enter Number of Weeks" />
        </div>
        <div class="col-md-4">
            <label class="form-label">Max Carry Over of Backlogs</label>
            <asp:TextBox ID="txtBacklogs" runat="server" CssClass="form-control" placeholder="Enter Max Backlogs" />
        </div>
    </div>
</div>

                    <div class="col-12 mt-3">
                        <asp:Button ID="btnAdd" runat="server" Text="Add Scheme" CssClass="btn btn-success me-2" OnClick="btnAdd_Click" />
                        <asp:Button ID="btnClear" runat="server" Text="Clear Fields" CssClass="btn btn-secondary" OnClick="ClearFields" />
                    </div>
                </div>
            </div>
        </div>

        <div class="card mt-4 shadow-sm">
            <div class="card-header bg-secondary text-white">
                <h5 class="mb-0">Curriculum Scheme Records</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvScheme" runat="server" AutoGenerateColumns="False" 
                    DataKeyNames="Scheme_Code"
                    OnRowEditing="gvScheme_RowEditing"
                    OnRowUpdating="gvScheme_RowUpdating"
                    OnRowCancelingEdit="gvScheme_RowCancelingEdit"
                    OnRowDeleting="gvScheme_RowDeleting"
                    CssClass="table table-bordered table-striped table-hover">
                    <Columns>
                        <asp:BoundField DataField="Scheme_Code" HeaderText="Scheme Code" ReadOnly="True" />
                        <asp:BoundField DataField="Scheme_Name" HeaderText="Scheme Name" />
                        <asp:BoundField DataField="Academic_Year" HeaderText="Academic Year" />
                        <asp:BoundField DataField="Attendance_Percentage" HeaderText="Attendance (%)" />
                        <asp:BoundField DataField="AT_Condonable_Percentage" HeaderText=" % Of Condonation" />
                        <asp:BoundField DataField="Weeks" HeaderText="Weeks" />
                        <asp:BoundField DataField="Max_Backlogs" HeaderText="Max Backlogs" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkEdit" runat="server" 
                                    CommandName="Edit" 
                                    CssClass="btn btn-sm btn-outline-primary me-1"
                                    Text="Edit"></asp:LinkButton>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:LinkButton ID="lnkUpdate" runat="server" 
                                    CommandName="Update" 
                                    CssClass="btn btn-sm btn-success me-1"
                                    Text="Update"></asp:LinkButton>
                                <asp:LinkButton ID="lnkCancel" runat="server" 
                                    CommandName="Cancel" 
                                    CssClass="btn btn-sm btn-secondary"
                                    Text="Cancel"></asp:LinkButton>
                                                            </EditItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
