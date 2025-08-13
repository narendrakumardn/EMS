<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintATS.aspx.cs" Inherits="BTEDiploma.forms.PrintATS"  MasterPageFile="~/Site1.Master"%>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Appearance Tag Sheet (ATS) Generator</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .ats-header {
            text-align: center;
            margin-bottom: 20px;
        }
        .ats-table th, .ats-table td {
            text-align: center;
            vertical-align: middle;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container mt-4">
            <h2 class="text-center">Generate Appearance Tag Sheet</h2>

            <div class="row mb-3">
                <div class="col-md-4">
                    <asp:Label ID="lblSelectDate" runat="server" Text="Select Exam Date:" CssClass="form-label" />
                    <asp:DropDownList ID="ddlExamDate" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="ddlExamDate_SelectedIndexChanged" />
                </div>

                <div class="col-md-4">
                    <asp:Label ID="lblCourse" runat="server" Text="Select Course Code:" CssClass="form-label" />
                    <asp:DropDownList ID="ddlCourseCode" runat="server" AutoPostBack="true" CssClass="form-select" OnSelectedIndexChanged="ddlCourseCode_SelectedIndexChanged" />
                </div>

                <div class="col-md-4">
                    <asp:Label ID="lblStudentCount" runat="server" Text="Total Students: 0" CssClass="form-label" />
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-md-3">
                    <asp:Label ID="lblFrom" runat="server" Text="From (Sl No):" CssClass="form-label" />
                    <asp:TextBox ID="txtFrom" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3">
                    <asp:Label ID="lblTo" runat="server" Text="To (Max 25):" CssClass="form-label" />
                    <asp:TextBox ID="txtTo" runat="server" CssClass="form-control" />
                </div>

                <div class="col-md-3 d-flex align-items-end">
                    <asp:Button ID="btnGenerate" runat="server" Text="Print ATS" CssClass="btn btn-success" OnClick="btnGenerate_Click" />
                </div>
            </div>

            <!-- ATS Preview Panel -->
            <asp:Panel ID="pnlATSPreview" runat="server" Visible="false">
                <div class="ats-header">
                    <img src="/images/karnataka-logo.png" alt="DTE Logo" style="height:60px;" />
                    <h4>Department of Technical Education</h4>
                    <h5>Government of Karnataka</h5>
                    <asp:Label ID="lblCollegeInfo" runat="server" CssClass="fw-bold d-block" />
                    <asp:Label ID="lblExamInfo" runat="server" CssClass="d-block" />
                </div>

                <asp:GridView ID="gvATSPreview" runat="server" CssClass="table table-bordered ats-table" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="Sl. No" DataField="SlNo" />
                        <asp:BoundField HeaderText="Reg No" DataField="RegNo" />
                        <asp:BoundField HeaderText="ATS No" DataField="ATSNumber" />
                        <asp:BoundField HeaderText="Answer Booklet No" DataField="AnswerBookletNo" />
                        <asp:TemplateField HeaderText="Student Signature">
                            <ItemTemplate>
                                __________________
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </div>
    
</asp:Content>

