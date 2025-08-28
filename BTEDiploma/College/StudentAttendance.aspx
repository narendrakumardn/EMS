<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master" CodeBehind="StudentAttendance.aspx.cs" Inherits="BTEDiploma.admin.StudentAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Optional: small spinner while async postbacks run -->
    <asp:UpdateProgress ID="updProg" runat="server" AssociatedUpdatePanelID="updMain" DisplayAfter="150">
        <ProgressTemplate>
            <div class="position-fixed top-0 start-50 translate-middle-x mt-3 alert alert-info py-2 px-3 shadow">
                Loading...
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="updMain" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
        <ContentTemplate>

            <div class="container mt-4">

                <!-- Header -->
                <div class="text-center mb-4">
                    <h3 class="bg-primary text-white py-3 px-4 rounded shadow-sm d-inline-block">
                        Student Attendance
                    </h3>
                    <div class="fw-semibold mt-2" style="color:#444; font-size:1rem;">
                        <i class="bi bi-info-circle-fill text-primary me-1"></i>
                        Contact <span class="text-primary">Exam Section</span> in case of any changes in the list
                    </div>
                </div>

                <!-- Card wrapper for filters + content -->
                <div class="card shadow-sm">
                    <div class="card-body">

                        <!-- Filter row -->
                        <div class="row g-2 align-items-end">
                            <div class="col-12 col-md-4">
                                <label for="ddlProgram" class="form-label mb-1">Program</label>
                                <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
                            </div>

                            <div class="col-12 col-md-4">
                                <label for="ddlSemester" class="form-label mb-1">Semester</label>
                                <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
                            </div>

                            <div class="col-12 col-md-4 d-grid d-md-flex justify-content-md-end">
                                <asp:Button ID="btnLoad" runat="server" Text="Load Students & Courses"
                                    CssClass="btn btn-primary mt-2 mt-md-0" OnClick="btnLoad_Click" />
                            </div>
                        </div>

                        <!-- Messages -->
                        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger text-center d-block mt-3" />
                        <div class="text-center my-3">
                            <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red" Font-Bold="True" />
                            <asp:Label ID="lblStatus" runat="server" CssClass="fw-bold text-success d-block mb-2" Visible="false"></asp:Label>
                            <asp:Button ID="btnViewCondonable" runat="server" Text="View Condonable Students"
                                CssClass="btn btn-outline-primary" Visible="false"
                                OnClick="btnViewCondonable_Click" />
                        </div>

                        <!-- Locked banner -->
                        <asp:Panel ID="pnlLocked" runat="server" CssClass="alert alert-warning mt-2 d-none">
                            Attendance is finalized for this selection. Editing is disabled.
                        </asp:Panel>

                        <!-- Attendance Table -->
                        <div class="mt-3">
                            <asp:PlaceHolder ID="phAttendance" runat="server"></asp:PlaceHolder>
                        </div>

                        <!-- Action buttons -->
                        <div class="d-flex flex-wrap gap-2 mt-3">
                            <asp:Button ID="btnSave" runat="server" Text="Save Draft" CssClass="btn btn-warning" OnClick="btnSave_Click" />
                            <asp:Button ID="btnSubmit" runat="server" Text="Submit (Finalize)" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:HyperLink ID="lnkPreview" runat="server" Text="Preview Report" CssClass="btn btn-outline-secondary"
                                NavigateUrl="~/AttendancePreview.aspx" Target="_blank" />
                        </div>

                        <!-- Hidden fields -->
                        <asp:HiddenField ID="hfCourseList" runat="server" />
                        <asp:HiddenField ID="hfStudents" runat="server" />
                        <asp:HiddenField ID="hfCourses" runat="server" />
                        <asp:HiddenField ID="hfAttendance" runat="server" />
                        <asp:HiddenField ID="hfIsFinalized" runat="server" />

                        <!-- Result messages -->
                        <div class="mt-2">
                            <asp:Label ID="lblResult" runat="server" ForeColor="Green" Font-Bold="true"></asp:Label>
                            <asp:Label ID="lblError" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label>
                        </div>

                    </div>
                </div>

            </div>

        </ContentTemplate>

        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlProgram" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlSemester" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="btnLoad" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnViewCondonable" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

    <!-- Page-level styles -->
    <style>
        .attendance-table {
            border-collapse: collapse;
            width: 100%;
        }
        .attendance-table th, .attendance-table td {
            border: 1px solid #ccc;
            padding: 6px;
            text-align: center;
            vertical-align: middle;
        }
        .attendance-table th {
            background-color: #f8f9fa;
            font-weight: bold;
        }
        .attendance-input {
            width: 80px;
            text-align: center;
        }
    </style>

</asp:Content>
