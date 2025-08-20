<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.Master"
    CodeBehind="PCMapping11.aspx.cs" Inherits="BTEDiploma.admin.ProgramCourseMapping" %>


<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">

    <title>Program Course Mapping</title>
    <style>
        .form-label { font-weight: 600; }
        .left-panel { border-right: 1px solid #dee2e6; padding-right: 20px; }

        /* GridView styling */
        .gridview-fixed {
            table-layout: fixed;
            width: 100%;
            word-wrap: break-word;
        }
        .gridview-fixed td, .gridview-fixed th {
            white-space: normal !important;
            vertical-align: middle;
        }
        .gridview-fixed select {
            max-width: 100%;
            width: 100%;
            box-sizing: border-box;
            white-space: normal;
        }
        .gridview-fixed select option {
            white-space: normal;
        }
    </style>
</asp:Content>


    <asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

    <div class="container mt-4">
        <h4 class="mb-4 text-primary">Program Course Mapping</h4>
        <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3"></asp:Label>

        <div class="row">
           
            <div class="col-md-4 left-panel">
                <div class="mb-3">
                    <label for="ddlScheme" class="form-label">Scheme</label>
                    <asp:DropDownList ID="ddlScheme" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlScheme_SelectedIndexChanged">
                        <asp:ListItem Text="-- Select Scheme --" Value="" />
                    </asp:DropDownList>
                </div>

                <div class="mb-3">
                    <label for="ddlProgram" class="form-label">Program</label>
                    <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlProgram_SelectedIndexChanged" />
                </div>

                <div class="mb-3">
                    <label for="ddlAdmissionType" class="form-label">Admission Type</label>
                    <asp:DropDownList ID="ddlAdmissionType" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlAdmissionType_SelectedIndexChanged" />
                </div>

                <div class="mb-3">
                    <label for="ddlCourse" class="form-label">Course</label>
                    <asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-select" />
                </div>

                <div class="mb-3">
                    <label for="txtSemester" class="form-label">Semester (1–6)</label>
                    <asp:TextBox ID="txtSemester" runat="server" CssClass="form-control"
                        TextMode="Number" min="1" max="6" />
                </div>

                <div class="mb-3">
                    <label for="txtSno" class="form-label">Serial Number (Sno)</label>
                    <asp:TextBox ID="txtSno" runat="server" CssClass="form-control"
                        TextMode="Number" min="1" max="6" />
                </div>

                <div class="text-end">
                    <asp:Button ID="btnSubmit" runat="server" Text="Save Mapping"
                        CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                </div>
            </div>

            <!-- Right Panel: GridView -->
            <div class="col-md-8">
                <asp:GridView ID="gvSemesterMapping" runat="server"
                    AutoGenerateColumns="False"
                    DataKeyNames="Program_Code,Admission_Type_ID,SemesterNo,Course1_Code,Course2_Code,Course3_Code,Course4_Code,Course5_Code,Type"
                    OnRowEditing="gvSemesterMapping_RowEditing"
                    OnRowUpdating="gvSemesterMapping_RowUpdating"
                    OnRowCancelingEdit="gvSemesterMapping_RowCancelingEdit"
                    OnRowDataBound="gvSemesterMapping_RowDataBound"
                    CssClass="table table-bordered table-striped gridview-fixed">

                    <Columns>
                        <asp:BoundField DataField="SemesterNo" HeaderText="Semester" ReadOnly="true" />
                        <asp:BoundField DataField="Type" HeaderText="Type" ReadOnly="true" />

                        <asp:TemplateField HeaderText="Course 1">
                            <ItemTemplate><%# Eval("Course1_Display") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCourse1" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Course 2">
                            <ItemTemplate><%# Eval("Course2_Display") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCourse2" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Course 3">
                            <ItemTemplate><%# Eval("Course3_Display") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCourse3" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Course 4">
                            <ItemTemplate><%# Eval("Course4_Display") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCourse4" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Course 5">
                            <ItemTemplate><%# Eval("Course5_Display") %></ItemTemplate>
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlCourse5" runat="server"></asp:DropDownList>
                            </EditItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField ShowEditButton="true" HeaderText="ACTIONS" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>

