<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site1.master" 
    CodeBehind="ViewAndApproveExamEligibleStudent.aspx.cs" Inherits="BTEDiploma.admin.ViewAndApproveExamEligibleStudent" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">



    
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

             
                <div class="text-center mb-3">
                    <h3 class="bg-primary text-white py-3 px-4 rounded shadow-sm d-inline-block mb-2">
                        Student Attendance
                    </h3>
                    <div class="fw-semibold" style="color:#444; font-size:1rem;">
                        <i class="bi bi-info-circle-fill text-primary me-1"></i>
                        Contact <span class="text-primary">Exam Section</span> in case of any changes in the list
                    </div>
                </div>

              
                <div class="mb-3">
                    <h5 class="mb-2 text-primary fw-bold d-flex align-items-center">
                        <i class="bi bi-calendar2-week me-2"></i> Current Exam Session
                    </h5>
                    <div class="card p-3 shadow-sm" style="max-width:520px;">
                        <div class="row">
                            <div class="col-6">
                                <strong>Exam Month:</strong>
                                <asp:Label ID="lblExamMonth" runat="server" CssClass="ms-1" />
                            </div>
                            <div class="col-6">
                                <strong>Exam Year:</strong>
                                <asp:Label ID="lblExamYear" runat="server" CssClass="ms-1" />
                            </div>
                        </div>
                    </div>
                </div>

              
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Font-Bold="true" CssClass="d-block mb-2"></asp:Label>
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Font-Bold="true" CssClass="d-block mb-3"></asp:Label>

             
                <div class="card shadow-sm mb-3">
                    <div class="card-body">
                        <div class="row g-2 align-items-end">
                            <div class="col-12 col-md-4">
                                <label for="ddlProgram" class="form-label mb-1">Program</label>
                                <asp:DropDownList ID="ddlProgram" runat="server" CssClass="form-select"
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlFilter_Changed" />
                            </div>
                            <div class="col-12 col-md-4">
                                <label for="ddlSemester" class="form-label mb-1">Semester</label>
                                <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-select" />
                            </div>
                            <div class="col-12 col-md-4 d-grid d-md-flex justify-content-md-end">
                                <asp:Button ID="btnLoad" runat="server" Text="Load Students & Courses"
                                    CssClass="btn btn-primary mt-2 mt-md-0" OnClick="btnLoad_Click" />
                            </div>
                        </div>
                    </div>
                </div>

               
                <asp:Panel ID="pnlEligibleList" runat="server" Visible="true" CssClass="p-3 border rounded bg-light shadow-sm">
                    <asp:GridView ID="grExamEligibleStudents" runat="server"
                        AutoGenerateColumns="true"
                        CssClass="table table-bordered table-striped table-sm mb-3" />
                    <div class="text-center">
                        <asp:Button ID="btnApprove" runat="server"
                            Text="Approve Eligible Students List"
                            OnClick="btnApproveEligibleList_Click"
                            CssClass="btn btn-success" />
                    </div>
                </asp:Panel>

            </div>

        </ContentTemplate>

      
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlProgram" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="btnLoad" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnApprove" EventName="Click" />
        </Triggers>
    </asp:UpdatePanel>

   
    <style>
        .form-label { font-weight: 600; }
        .table-sm th, .table-sm td { padding: .5rem; }
    </style>

</asp:Content>
