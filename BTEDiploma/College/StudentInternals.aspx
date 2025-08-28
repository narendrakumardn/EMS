<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentInternals.aspx.cs" 
    Inherits="BTEDiploma.admin.StudentInternals" MasterPageFile="~/Site1.Master" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-white bg-primary p-3 rounded">Add CIE Marks</h2>

        <!-- Main update panel for selections + marks entry -->
        <asp:UpdatePanel ID="updMain" runat="server">
            <ContentTemplate>
                <div class="row g-3 mt-3">
                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlProgramme" Text="Programme" CssClass="form-label" />
                        <asp:DropDownList ID="ddlProgramme" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged" />
                    </div>

                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlSemester" Text="Semester" CssClass="form-label" />
                        <asp:DropDownList ID="ddlSemester" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlSemester_SelectedIndexChanged" />
                    </div>

                    <div class="col-md-4">
                        <asp:Label runat="server" AssociatedControlID="ddlAdmissionType" Text="Admission Type" CssClass="form-label" />
                        <asp:DropDownList ID="ddlAdmissionType" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlAdmissionType_SelectedIndexChanged" />
                    </div>

                    <div class="col-md-12 mt-3">
                        <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand" OnItemDataBound="rptCourses_ItemDataBound">
                            <ItemTemplate>
                                <asp:Button ID="btnCourse" runat="server" 
                                    CssClass="btn btn-outline-primary m-1"
                                    CommandName="SelectCourse"
                                    CommandArgument='<%# Eval("Course_Code") %>'
                                    Text='<%# Eval("Course_Name") %>' />
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger text-center d-block" />

                    <!-- Marks entry area -->
                    <div class="col-md-12 mt-4">
                        <asp:Panel ID="pnlMarksEntry" runat="server" Visible="false">
                            <h5 class="mb-3">
                                Internal Assessment Marks
                                <asp:Label ID="lblSelectedCourse" runat="server" CssClass="ms-2 text-primary fw-bold" />
                            </h5>

                            <asp:Panel ID="pnlInfoAlert" runat="server" CssClass="alert alert-info border border-primary p-3 mt-3 rounded shadow-sm">
                                <h5 class="fw-bold text-primary">Important Instructions</h5>
                                <ul class="mb-0">
                                    <li><strong>For ABSENT students:</strong> Please enter <span class="text-danger fw-bold">00</span> as IA marks.</li>
                                    <li><strong>Marks shown in <span class="text-danger">RED</span> are NOT eligible.</strong> Please review carefully before saving.</li>
                                </ul>
                            </asp:Panel>

                            <asp:Table ID="tblMarks" runat="server" CssClass="table table-bordered table-striped"></asp:Table>

                            <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft"
                                CssClass="btn btn-success mt-3"
                                OnClick="btnSaveDraft_Click" />

                            <asp:Button ID="btnPreviewIA" runat="server" Text="Preview IA Marks"
                                CssClass="btn btn-warning mt-3 ms-2"
                                OnClick="btnPreviewIA_Click" />
                        </asp:Panel>
                    </div>
                </div>

                <!-- System Message Modal -->
                <asp:Panel ID="pnlServerModal" runat="server" CssClass="modal fade" Visible="false">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content border-danger">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title">System Message</h5>
                                <asp:Button runat="server" ID="btnCloseModal" CssClass="btn-close btn-close-white" OnClick="btnCloseModal_Click" />
                            </div>
                            <div class="modal-body">
                                <asp:Literal ID="litModalBody" runat="server"></asp:Literal>
                            </div>
                            <div class="modal-footer">
                                <asp:Button runat="server" ID="btnOkModal" CssClass="btn btn-secondary" Text="OK" OnClick="btnCloseModal_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- High Alert: Preview confirmation (kept) -->
                <asp:Panel ID="pnlPreviewConfirm" runat="server" CssClass="modal fade" Visible="false">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content border-danger">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title">⚠️ High Alert</h5>
                                <asp:Button runat="server" ID="btnClosePreviewModal" CssClass="btn-close btn-close-white" OnClick="btnClosePreviewModal_Click" />
                            </div>
                            <div class="modal-body text-danger fw-bold">
                                Ensure <b>ALL courses IA marks</b> are entered for <b>ALL student types</b> (ITI, PUC, Programme Change).
                                <br /><br /><span class="text-dark">Do you want to continue?</span>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnCancelPreview" runat="server" CssClass="btn btn-secondary" Text="Cancel" OnClick="btnClosePreviewModal_Click" />
                                <asp:Button ID="btnConfirmPreview" runat="server" CssClass="btn btn-danger" Text="Yes, Continue"
                                    OnClick="btnPreviewIAMarks_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Irreversible Warning Modal (kept) -->
                <asp:Panel ID="pnlIrreversible" runat="server" CssClass="modal fade" Visible="false">
                    <div class="modal-dialog modal-dialog-centered">
                        <div class="modal-content border-danger">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title">⚠️ Final Submission</h5>
                                <asp:Button runat="server" ID="btnCloseIrrev" CssClass="btn-close btn-close-white"
                                    OnClick="btnIrrevCancel_Click" />
                            </div>
                            <div class="modal-body">
                                <p class="fw-bold text-danger mb-2">
                                    Once final IA marks are submitted, you cannot edit them.
                                </p>
                                <p class="mb-0">Make sure everything is verified carefully.</p>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="btnIrrevCancel" runat="server" CssClass="btn btn-secondary"
                                    Text="Cancel" OnClick="btnIrrevCancel_Click" />
                                <asp:Button ID="btnIrrevProceed" runat="server" CssClass="btn btn-danger"
                                    Text="Yes, Submit Now" OnClick="btnIrrevProceed_Click" />
                            </div>
                        </div>
                    </div>
                </asp:Panel>

            </ContentTemplate>
        </asp:UpdatePanel>

        <!-- Report section + inline confirmation + final submit -->
        <asp:UpdatePanel ID="upReportViewer" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <rsweb:ReportViewer ID="rvIA" runat="server" Width="100%" Height="800px" />

                <!-- Inline confirmation (appears after report load) -->
                <asp:Panel ID="pnlAfterReportConfirm" runat="server"
                    CssClass="alert alert-info mt-3 d-flex justify-content-between align-items-center"
                    Visible="false">
                    <div>
                        <p class="mb-1 fw-semibold">
                            I have verified IA marks for this programme across all courses and all students.
                        </p>
                        <p class="mb-0">
                            I (Principal) understand I am responsible for correctness. Do you agree to proceed?
                        </p>
                    </div>
                    <div>
                        <asp:Button ID="btnAfterReportNo" runat="server" CssClass="btn btn-secondary me-2"
                            Text="No" OnClick="btnAfterReportNo_Click" />
                        <asp:Button ID="btnAfterReportYes" runat="server" CssClass="btn btn-success"
                            Text="Yes, I Agree" OnClick="btnAfterReportYes_Click" />
                    </div>
                </asp:Panel>

               
                <asp:Button ID="btnFinalSubmit" runat="server"
                    Text="Final Submit"
                    CssClass="btn btn-danger mt-3"
                    Visible="false"
                    OnClick="btnFinalSubmit_Click" />
            </ContentTemplate>
            <Triggers>
          
                <asp:AsyncPostBackTrigger ControlID="btnConfirmPreview" EventName="Click" />
               
                <asp:AsyncPostBackTrigger ControlID="btnAfterReportYes" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="btnAfterReportNo" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>

    </div>
</asp:Content>
