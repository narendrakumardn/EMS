<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamTimetableadd.aspx.cs" Inherits="BTEDiploma.admin.ExamTimetableadd" MasterPageFile="~/Site1.Master" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="bodyContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <div class="container mt-4 p-4" style="background-color: #f2f8ff; border-radius: 10px;">
                <h2 class="text-center mb-4">Exam Timetable Management</h2>

                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger text-center d-block" />

                <!-- Dropdown Filters -->
                <div class="row mb-3 justify-content-center">
    <div class="col-md-3 text-center">
        <asp:Label runat="server" Text="Academic Year" CssClass="form-label d-block" />
        <asp:DropDownList ID="ddlAcademicYear" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DropdownsChanged" />
    </div>
    <div class="col-md-3 text-center">
        <asp:Label runat="server" Text="Semester" CssClass="form-label d-block" />
        <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="DropdownsChanged" />
    </div>
                    <div class="col-md-3">
                        <asp:Label runat="server" Text="Programme" CssClass="form-label" />
                        <asp:DropDownList ID="ddlProgramme" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" />
                    </div>
                </div>

                <!-- GridView with One Submit Button -->
                <asp:GridView ID="gvCourses" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered">
                    <Columns>
                        <asp:BoundField DataField="CourseCode" HeaderText="Course Code" />
                        <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                        <asp:BoundField DataField="Semester" HeaderText="Semester" />
                        <asp:TemplateField HeaderText="Exam Date">
                            <ItemTemplate>
                                <asp:TextBox ID="txtExamDate" runat="server" Text='<%# Eval("ExamDate", "{0:yyyy-MM-dd}") %>' TextMode="Date" CssClass="form-control" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Session">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlSession" runat="server" CssClass="form-control">
                                    <asp:ListItem Text="FN" Value="FN" />
                                    <asp:ListItem Text="AN" Value="AN" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <!-- Submit All Button -->
                <div class="text-center mt-3">
                    <asp:Button ID="btnSubmitAll" runat="server" Text="Submit All" CssClass="btn btn-success" OnClick="btnSubmitAll_Click" />
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
